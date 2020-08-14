using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TinderBot
{
    class TinderDataDownloader
    {
        private ILogger<TinderDataDownloader> Logger { get; }
        private TinderClient TinderClient { get; }
        public TinderDataDownloader(TinderClient tinderClient, ILogger<TinderDataDownloader> logger) => (TinderClient, Logger) = (tinderClient, logger);

    }
}
