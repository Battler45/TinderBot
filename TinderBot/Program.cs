﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
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
            var logger = ServiceProvider.GetService<ILogger<string>>();
            await tinderClient.SafelySynchronouslyLikePeoplePackages(3000, 5000, logger);

        }

        static Program()
        {
            Configuration = new ConfigurationBuilder()
               .AddJsonFile(@"appsettings.json", optional: false, reloadOnChange: true)
               .Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddOptions();
            serviceCollection.AddLogging(builder => builder.AddConsole());

            //serviceCollection.Configure<T>(Configuration.GetSection(nameof(T)));

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}
