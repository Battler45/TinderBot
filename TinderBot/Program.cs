using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace TinderBot
{
    class Program
    {
        private static readonly IServiceProvider ServiceProvider;
        private static IConfiguration Configuration { get;  set; }
        #region Consts
        const int SleepingMillisecondsBetweenLiking         = 1000
            , SleepingMillisecondsBeforeGettingNewPackage   = 5000
            , SleepMillisecondsAfterFailedLiking            = 60000
            ;
        #endregion
        static async Task Main()
        {
            var tinderClient = ServiceProvider.GetService<TinderClient>();
            var usersData = await tinderClient.GetUsersData();

            /*
            var likePackagesTasks = Enumerable.Range(0, 4)
                .Select(i => 
                    tinderClient.SafelySynchronouslyLikePeoplePackages(SleepingMillisecondsBetweenLiking,
                     SleepingMillisecondsBeforeGettingNewPackage, SleepMillisecondsAfterFailedLiking)
                );
            await Task.WhenAll(likePackagesTasks);
            */
        }

        static Program()
        {
            Configuration = new ConfigurationBuilder()
               .AddJsonFile(@"appsettings.json", optional: false, reloadOnChange: true)
               .Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddConsole());

            serviceCollection
                .AddHttpClient<TinderClient>();

            serviceCollection.AddOptions();
            serviceCollection.Configure<TinderConfig>(Configuration.GetSection(nameof(TinderConfig)));
            // UserKeywordsFilter : IUserFilter
            serviceCollection.AddTransient<IUserFilter, UserKeywordsFilter>();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}
