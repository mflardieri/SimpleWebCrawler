using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleWebCrawler.Core.Parsers.Interfaces;
using SimpleWebCrawler.Core.Parsers.Models;
using SimpleWebCrawler.Core.Processors.Interfaces;
using SimpleWebCrawler.Core.Processors.Models;
using SimpleWebCrawler.Core.Results.Models;
using SimpleWebCrawler.Core.Tests.Models;
using System;
using System.Net.WebSockets;

namespace SimpleWebCrawler.Core.Tests
{
    [TestClass]
    public class SiteProcessorTests
    {
        private SiteProcessor? _siteProcessor { get; set; }
        private IServiceCollection _services { get; set; }
        private IServiceProvider _serviceProvider { get; set; }

        private SiteResult? siteResult { get; set; }
        public SiteProcessorTests() 
        {
            _services = new ServiceCollection();
            _services.AddTransient<SiteProcessor>();
            _services.AddTransient<IPageHtmlParser, SimpleHtmlParser>();
            _services.AddTransient<IPageParser, TestPageParser>();
            _services.AddTransient<IRobotTextParser, RobotTextParser>();
            _services.AddTransient<IPagePointOfInterestStrategy, SimplePagePointOfInterestStrategy>();


            var configuration = new ConfigurationBuilder()
                //.AddJsonFile("appsettings.json")
                .Build();
            _services.AddSingleton(configuration);

            _services.AddLogging();
            _serviceProvider = _services.BuildServiceProvider();  
            
            _siteProcessor = _serviceProvider.GetRequiredService<SiteProcessor>();
        } 

        [TestInitialize]
        public void setUpTest()
        {
            Assert.IsNotNull(_siteProcessor);
            siteResult = _siteProcessor.CreateSiteResult("https://test.com");
        }
        [TestMethod]
        public void TestGetRobotsTxtSiteMapResults()
        {
            Assert.IsNotNull(_siteProcessor);
            Assert.IsNotNull(siteResult);
            _siteProcessor.GetSiteMapsAsync(siteResult).Wait();
            Assert.IsNotNull(siteResult.RobotIgnorePaths);
            Assert.IsNotNull(siteResult.SiteMaps);
            Assert.IsTrue(siteResult.SiteMaps.Count > 0);
        }

        [TestMethod]
        public void TestProcessSite()
        {
            Assert.IsNotNull(_siteProcessor);
            Assert.IsNotNull(siteResult);
            ((ISiteProcessor)_siteProcessor).ProcessSiteAsync(siteResult).Wait();
            Assert.IsNotNull(siteResult.PageResults);
            Assert.IsTrue(siteResult.PageResults.Count > 0);

            ((ISiteProcessor)_siteProcessor).ReApplyIssues(siteResult);
            var issues =  ((ISiteProcessor)_siteProcessor).ConvertToPageIssues(siteResult);
            Assert.IsNotNull(issues);
            Assert.IsTrue(issues.Count > 0);
        }
        [TestMethod]
        public void TestProcessPage()
        {
            Assert.IsNotNull(_siteProcessor);
            Assert.IsNotNull(siteResult);
            ((ISiteProcessor)_siteProcessor).ProcessPageAsync(siteResult, siteResult.NormalizeToFinializedURL("test")).Wait();
            Assert.IsNotNull(siteResult.PageResults);
            Assert.IsTrue(siteResult.PageResults.Count > 0);
        }
    }
}