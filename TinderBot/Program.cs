using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TinderBot
{
    class Program
    {
        private static readonly IServiceProvider ServiceProvider;
        public static IConfiguration Configuration { get; private set; }
        #region Consts
        const int SleepingMillisecondsBetweenLiking         = 1000
            , SleepingMillisecondsBeforeGettingNewPackage   = 5000
            , SleepMillisecondsAfterFailedLiking            = 60000
            ;
        #endregion
        static async Task Main()
        {
            var tinderClient = ServiceProvider.GetService<TinderClient>();

            var likePackagesTasks = Enumerable.Range(0, 4)
                .Select(i => 
                    tinderClient.SafelySynchronouslyLikePeoplePackages(SleepingMillisecondsBetweenLiking,
                     SleepingMillisecondsBeforeGettingNewPackage, SleepMillisecondsAfterFailedLiking)
                );
            await Task.WhenAll(likePackagesTasks);
        }

        static Program()
        {
            Configuration = new ConfigurationBuilder()
               .AddJsonFile(@"appsettings.json", optional: false, reloadOnChange: true)
               .Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddOptions();
            serviceCollection.AddLogging(builder => builder.AddConsole());

            serviceCollection
                .AddHttpClient<TinderClient>();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}
