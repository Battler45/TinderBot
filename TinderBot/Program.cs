using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace TinderBot
{
    class Program
    {
        private static readonly IServiceProvider ServiceProvider;
        public static IConfiguration Configuration { get; private set; }
        static async Task Main()
        {
            var token = Configuration["Token"];
            var tinderClient = TinderHttpClient.GetClient(token);
            while (true)
            {
                var watch = Stopwatch.StartNew();
                var likes = await tinderClient.SafelySynchronouslyLikePeoplePackage();
                watch.Stop();
                if (likes == null || likes.Count == 0)
                {
                    Logger.Log($"Lol, this location is empty for bot, yo timeout is about 30 minutes");
                    return;
                }   
                Logger.Log($"likes : {likes.Count}// time: {watch.ElapsedMilliseconds / 1000}s");
            }


        }

        static Program()
        {
            Configuration = new ConfigurationBuilder()
               .AddJsonFile(@"appsettings.json", optional: false, reloadOnChange: true)
               .Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddOptions();
            //serviceCollection.Configure<T>(Configuration.GetSection(nameof(T)));

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}
