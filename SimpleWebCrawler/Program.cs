using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimpleWebCrawler.Core.Helpers;
using SimpleWebCrawler.Core.Parsers.Interfaces;
using SimpleWebCrawler.Core.Parsers.Models;
using SimpleWebCrawler.Core.Processors.Interfaces;
using SimpleWebCrawler.Core.Processors.Models;

namespace SimpleWebCrawler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Background Service Setup
            /*
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<Worker>();
            
            builder.Services.AddTransient<ISiteProcessor, SiteProcessor>();
            builder.Services.AddTransient<IRobotTextParser, RobotTextParser>();
            builder.Services.AddTransient<IPageParser, PageParser>();
            builder.Services.AddTransient<IPageHtmlParser, SimpleHtmlParser>();
            builder.Services.AddTransient<IPagePointOfInterestStrategy, SimplePagePointOfInterestStrategy>();

            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            builder.Services.AddSingleton(configuration);

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            
            using IHost host = builder.Build();

            host.Run();
            */
            //Background Process Setup
            var services = new ServiceCollection();
            services.AddTransient<ISiteProcessor, SiteProcessor>();
            services.AddTransient<IRobotTextParser, RobotTextParser>();
            services.AddTransient<IPageParser, PageParser>();
            services.AddTransient<IPageHtmlParser, SimpleHtmlParser>();
            services.AddTransient<IPagePointOfInterestStrategy, SimplePagePointOfInterestStrategy>();

            ConfigureServices(services);
            var servicesProvider = services.BuildServiceProvider();


            ISiteProcessor siteProcessor = servicesProvider.GetRequiredService<ISiteProcessor>();
            var siteResult = siteProcessor.CreateSiteResult("https://www.google.com");
            siteProcessor.ProcessPageAsync(siteResult, "https://www.google.com").Wait();
            //var siteResult = siteProcessor.CreateSiteResult("https://acme.com");
            //siteProcessor.ProcessSiteAsync(siteResult).Wait();//Long run process
            if (siteResult != null)
            {

                siteResult.ToJsonFile("output_current.json");
                var issues = siteProcessor.ConvertToPageIssues(siteResult);
                if (issues != null && issues.Count > 0)
                {
                    issues.ToJsonFile("output_issues_current.json");
                }
            }
            Console.WriteLine("Done!");
        }
        static void ConfigureServices(ServiceCollection services)
        {

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            services.AddSingleton(configuration);
            services.AddLogging(loggerBuilder =>
            {
                loggerBuilder.ClearProviders();
                loggerBuilder.AddConsole();
            });
        }
    }

    public class Worker : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _services;

        public Worker(ILogger<Worker> logger, IServiceProvider services)
        {
            _logger = logger;
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ISiteProcessor siteProcessor = _services.GetRequiredService<ISiteProcessor>();
            var siteResults = siteProcessor.CreateSiteResult("https://www.google.com");
            siteResults.pagePointOfInterestChecks = new List<Core.Components.Enums.PagePointOfInterestCheck>() { Core.Components.Enums.PagePointOfInterestCheck.SubHeaders };
            await siteProcessor.ProcessPageAsync(siteResults, "https://www.google.com");
            //await siteProcessor.ProcessSiteAsync(siteResults);//Long run process
            //siteResults.ToJsonFile("output.json");

            //SiteResult? siteResult = JsonHelpers.FromJsonFile<SiteResult>("output.json");
            /*if (siteResults != null)
            {
                //siteProcessor.ReApplyIssues(siteResults);
                var issues = siteProcessor.ConvertToPageIssues(siteResults);
                if (issues != null && issues.Count > 0)
                {
                    issues.ToJsonFile("output_issues.json");
                }
            }*/
        }
    }

}