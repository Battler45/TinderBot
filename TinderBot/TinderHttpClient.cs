using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
    class TinderHttpClient
    {
        private const string TinderApiUrl = "https://api.gotinder.com/";
        private string GetTinderApiLikeUrl => $"{TinderApiUrl}like";
        private const string TinderApiV2Url = "https://api.gotinder.com/v2/";
        //https://api.gotinder.com/v2/recs/core
        private string GetTinderApiPeopleNearbyListUrl => $"{TinderApiV2Url}recs/core";

        private string token;
        private static TinderHttpClient client;
        public static TinderHttpClient GetClient(string token) => client ??= new TinderHttpClient(token);
        private TinderHttpClient(string token) => this.token = token;
        private HttpClient _httpClient;
        private HttpClient HttpClient
        {
            get
            {
                if (_httpClient != null) return _httpClient;
                _httpClient = new HttpClient()
                {
                    //BaseAddress = new Uri(TinderApiV2Url)
                };
                SetupHttpClientHeadersForAuthorizationByToken(token);
                //SetupHttpClientHeadersForJSONFormat();
                return _httpClient;
            }
        }
        const string TokenHeaderName = "X-Auth-Token";
        private void SetupHttpClientHeadersForAuthorizationByToken(string token)
        {
            HttpClient.DefaultRequestHeaders.Add(TokenHeaderName, token);
            //HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(TokenHeaderName, token);
        }
        private void SetupHttpClientHeadersForJSONFormat()
        {
            HttpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
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

        public async Task<List<Like>> SafelySynchronouslyLikePeoplePackage(List<UserData> userDataPackage, int sleepMillisecondsBetweenLiking, ILogger<string> logger)
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
                    logger.LogInformation($"liked");
                }
                else
                {
                    logger.LogInformation($"failed to like");
                    //return null;
                }
            }
            return likes;
        }
        public async Task SafelySynchronouslyLikePeoplePackages(int sleepMillisecondsBetweenLiking, int sleepMillisecondsBeforeGettingNewPackage, ILogger<string> logger)
        {
            while (true)
            {
                var userDataPackage = await GetUserDatas();
                if (userDataPackage == null || userDataPackage.Count == 0)
                {
                    logger.LogInformation($"Lol, this location is empty for bot, yo timeout is about 30 minutes");
                    return;
                }

                var watch = Stopwatch.StartNew();
                var likes = await SafelySynchronouslyLikePeoplePackage(userDataPackage, sleepMillisecondsBetweenLiking, logger);
                watch.Stop();
                if (likes == null || likes.Count == 0)
                {
                    logger.LogInformation($"Lol, this location is empty for bot, yo timeout is about 30 minutes");
                    return;
                }
                logger.LogInformation($"likes : {likes.Count}// time: {watch.ElapsedMilliseconds / 1000}s");
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
