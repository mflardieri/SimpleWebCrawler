using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleWebCrawler.Core.Processors.Bases;
using SimpleWebCrawler.Core.Processors.Interfaces;
using SimpleWebCrawler.Core.Results.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
