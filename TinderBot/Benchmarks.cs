using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TinderBot.Models;

namespace TinderBot
{
    static class Benchmarks
    {
        public static async Task TestLikePeoplePackageAsync(string methodName, int maxLikesCount, Func<Task<List<Like>>> LikePeoplePackage)
        {
            var likesCount = 0;
            var watch = Stopwatch.StartNew();
            while (likesCount < maxLikesCount)
            {
                var likes = await LikePeoplePackage();
                likesCount += likes.Count;
            }
            watch.Stop();
            Logger.Log($"{methodName}|| likes : {likesCount}// time: {watch.ElapsedMilliseconds / 1000}s");
        }
        public static async Task RunLike(TinderHttpClient tinderClient, int maxLikesCount)
        {
            await TestLikePeoplePackageAsync(nameof(tinderClient.SynchronouslyLikePeoplePackage), maxLikesCount, tinderClient.SynchronouslyLikePeoplePackage);
            await TestLikePeoplePackageAsync(nameof(tinderClient.LikePeoplePackage), maxLikesCount, tinderClient.LikePeoplePackage);
            await TestLikePeoplePackageAsync(nameof(tinderClient.LikePeoplePackages), maxLikesCount, async () => await tinderClient.LikePeoplePackages(maxLikesCount / 10));
        }
    }

}
