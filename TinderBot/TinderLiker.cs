using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TinderBot.Models;

namespace TinderBot
{
    public class TinderLiker
    {
        private TinderClient TinderClient { get; }
        private ILogger<TinderClient> Logger { get; }
        private IUserFilter UserFilter { get; }
        public TinderLiker(TinderClient tinderClient, ILogger<TinderClient> logger, IUserFilter userFilter)
        {
            (Logger, TinderClient, UserFilter) = (logger, tinderClient, userFilter);
        }

        public async Task<Like> Like(UserData userData)
        {
            if (!UserFilter.Filter(userData)) return null;
            var like = await TinderClient.LikeUser(userData.user._id);
            return like;
        }

        public async Task<List<Like>> SynchronouslyLikePeoplePackage()
        {
            var usersDatas = await TinderClient.GetUsersData();
            if (usersDatas == null) return null;
            var usersIds = usersDatas.Select(ud => ud.user._id);

            var likes = new List<Like>();
            foreach (var userId in usersIds)
            {
                var like = await TinderClient.LikeUser(userId);
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
                var like = await TinderClient.LikeUser(userId);
                if (like != null)
                {
                    likes.Add(like);
                }
                else
                    return likes;
            }
            return likes;
        }
        public async Task SafelySynchronouslyLikePeoplePackages(int sleepMillisecondsBetweenLiking, int sleepMillisecondsBeforeGettingNewPackage, int sleepMillisecondsAfterFailedLiking)
        {
            while (true)
            {
                var userDataPackage = await TinderClient.GetUsersData();
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
        public async Task<List<Like>> LikePeoplePackage(/*int delay*/)
        {
            var usersData = await TinderClient.GetUsersData();
            if (usersData == null) return null;
            var usersIds = usersData.Select(ud => ud.user._id);

            var likes = new List<Like>();
            foreach (var userId in usersIds)
            {
                var like = await TinderClient.LikeUser(userId);
                if (like != null)
                    likes.Add(like);
                //await Task.Delay(delay);
            }
            /*
            var likeUsersTasks = usersIds.Select(LikeUser);
            var likeUsersResults = await Task.WhenAll(likeUsersTasks);
            var likes = likeUsersResults.Where(l => l != null).ToList();
            */
            return likes;
        }

        public async Task<List<Like>> LikePeoplePackages(int packagesCount)
        {
            //var packages =  0..packagesCount;
            var likePackageTasks = Enumerable.Range(0, packagesCount).Select(n => LikePeoplePackage());
            var likesPackages = await Task.WhenAll(likePackageTasks);
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
