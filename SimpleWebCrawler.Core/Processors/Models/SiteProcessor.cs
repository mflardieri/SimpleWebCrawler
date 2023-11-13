using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SimpleWebCrawler.Core.Processors.Bases;

namespace SimpleWebCrawler.Core.Processors.Models
{
    public class SiteProcessor : SiteProcessorBase
    {
        private readonly ILogger Logger;
        IServiceProvider serviceProvider;
        public SiteProcessor(ILogger<SiteProcessor> logger, IServiceProvider services, IConfigurationRoot configuration) : base(logger, services, configuration)
        {
            Logger = logger;
            serviceProvider = services;
        }
    }
}
