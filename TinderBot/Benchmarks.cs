using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TinderBot.Models;

namespace TinderBot
{
    static class Benchmarks
    {
        public static async Task TestLikePeoplePackageAsync(string methodName, int maxLikesCount, Func<Task<List<Like>>> LikePeoplePackage, ILogger<string> logger)
        {
            var likesCount = 0;
            var watch = Stopwatch.StartNew();
            while (likesCount < maxLikesCount)
            {
                var likes = await LikePeoplePackage();
                likesCount += likes.Count;
            }
            watch.Stop();
            logger.LogInformation($"{methodName}|| likes : {likesCount}// time: {watch.ElapsedMilliseconds / 1000}s");
        }
        public static async Task RunLike(TinderClient tinderClient, int maxLikesCount, ILogger<string> logger)
        {
            await TestLikePeoplePackageAsync(nameof(tinderClient.SynchronouslyLikePeoplePackage), maxLikesCount, tinderClient.SynchronouslyLikePeoplePackage, logger);
            await TestLikePeoplePackageAsync(nameof(tinderClient.LikePeoplePackage), maxLikesCount, tinderClient.LikePeoplePackage, logger);
            await TestLikePeoplePackageAsync(nameof(tinderClient.LikePeoplePackages), maxLikesCount, async () => await tinderClient.LikePeoplePackages(maxLikesCount / 10), logger);
        }
    }

}
