using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TinderBot.Models;

namespace TinderBot
{
    class TinderClient
    {
        #region Urls
        private const string TinderApiUrl = "https://api.gotinder.com/";
        private string GetTinderApiLikeUrl => $"{TinderApiUrl}like";
        private const string TinderApiV2Url = "https://api.gotinder.com/v2/";
        //https://api.gotinder.com/v2/recs/core
        private string GetTinderApiPeopleNearbyListUrl => $"{TinderApiV2Url}recs/core";

        #endregion
        #region HttpClient
        private HttpClient HttpClient { get; set; }
        private HttpClient SetupHttpClientByDefaultSettings(HttpClient httpClient)
        {
            SetupHttpClientHeadersForAuthorizationByToken(httpClient, _token);
            return httpClient;
            static void SetupHttpClientHeadersForAuthorizationByToken(HttpClient httpClient, string token)
            {
                const string AuthHeaderName = "X-Auth-Token";
                httpClient.DefaultRequestHeaders.Add(AuthHeaderName, token);
            }
        }
        #endregion
        private readonly string _token;
        private ILogger<TinderClient> Logger { get; }
        public TinderClient(IOptions<TinderConfig> options, HttpClient httpClient, ILogger<TinderClient> logger)
        {
            (_token, Logger) = (options.Value.Token, logger);
            HttpClient = SetupHttpClientByDefaultSettings(httpClient);
        }

        public async Task<Like> LikeUser(string userId)
        {
            var response = await HttpClient.GetAsync($"{GetTinderApiLikeUrl}/{userId}");
            var contentStream = await response.Content.ReadAsStreamAsync();
            //var contentStream = await response.Content.ReadAsStringAsync();
            Like like = null;
            try
            {
                like = await JsonSerializer.DeserializeAsync<Like>(contentStream);
            }
            catch (Exception)
            {
            }
            return like;
        }
        public async Task<List<UserData>> GetUserDatas()
        {
            //Rootobject
            var response = await HttpClient.GetAsync(GetTinderApiPeopleNearbyListUrl);
            var contentStream = await response.Content.ReadAsStreamAsync();
            var responseObject = await JsonSerializer.DeserializeAsync<Rootobject>(contentStream);
            var userDatas = responseObject?.data?.results?.ToList();
            return userDatas;
        }
        public async Task<List<Like>> SynchronouslyLikePeoplePackage()
        {
            var usersDatas = await GetUserDatas();
            if (usersDatas == null) return null;
            var usersIds = usersDatas.Select(ud => ud.user._id);

            var likes = new List<Like>();
            foreach (var userId in usersIds)
            {
                var like = await LikeUser(userId);
                if (like != null)
                    likes.Add(like);
            }
            return likes;
        }

        public async Task<List<Like>> SafelySynchronouslyLikePeoplePackage(List<UserData> userDataPackage, int sleepMillisecondsBetweenLiking, int sleepMillisecondsAfterFailedLiking)
        {
            var usersIds = userDataPackage.Select(ud => ud.user._id);

            var likes = new List<Like>();
            foreach (var userId in usersIds)
            {
                Thread.Sleep(sleepMillisecondsBetweenLiking);
                var like = await LikeUser(userId);
                if (like != null)
                {
                    likes.Add(like);
                    //logger.LogInformation($"liked");
                }
                else
                {
                    Logger.LogWarning($"failed to like");
                    return likes;
                    //Thread.Sleep(sleepMillisecondsAfterFailedLiking);
                    //return null;
                }
            }
            return likes;
        }
        public async Task SafelySynchronouslyLikePeoplePackages(int sleepMillisecondsBetweenLiking, int sleepMillisecondsBeforeGettingNewPackage, int sleepMillisecondsAfterFailedLiking)
        {
            while (true)
            {
                var userDataPackage = await GetUserDatas();
                if (userDataPackage == null || userDataPackage.Count == 0)
                {
                    Logger.LogInformation($"Lol, this location is empty for bot, yo timeout is about 30 minutes");
                    return;
                }

                var watch = Stopwatch.StartNew();
                var likes = await SafelySynchronouslyLikePeoplePackage(userDataPackage, sleepMillisecondsBetweenLiking, sleepMillisecondsAfterFailedLiking);
                watch.Stop();
                if (likes == null || likes.Count == 0)
                {
                    Logger.LogInformation($"Lol, this location is empty for bot, yo timeout is about 30 minutes");
                    return;
                }
                Logger.LogInformation($"likes : {likes.Count}// time: {watch.ElapsedMilliseconds / 1000}s");
                Thread.Sleep(sleepMillisecondsBeforeGettingNewPackage);
            }
        }
        public async Task<List<Like>> LikePeoplePackage()
        {
            var usersDatas = await GetUserDatas();
            if (usersDatas == null) return null;
            var usersIds = usersDatas.Select(ud => ud.user._id);

            var likeUsersTasks = usersIds.Select(LikeUser);
            var likeUsersResults = await Task.WhenAll(likeUsersTasks);
            var likes = likeUsersResults.Where(l => l != null).ToList();
            
            return likes;
        }

        public async Task<List<Like>> LikePeoplePackages(int packagesCount)
        {
            //var packages =  0..packagesCount;
            var likePackageTasks = Enumerable.Range(0, packagesCount).Select(n => LikePeoplePackage());
            var likesPackages = await Task.WhenAll(likePackageTasks);
            var lol = new List<Like>();
            var likes = likesPackages.Aggregate(seed: new List<Like>(), (seed, likes) =>
            {
                if (likes != null)
                    seed.AddRange(likes);
                return seed;
            });
            return likes;
        }
    }
}
