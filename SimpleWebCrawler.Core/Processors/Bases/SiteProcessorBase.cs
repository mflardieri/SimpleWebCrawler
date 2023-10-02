using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleWebCrawler.Core.Components.Interfaces;
using SimpleWebCrawler.Core.Components.Models;
using SimpleWebCrawler.Core.Helpers;
using SimpleWebCrawler.Core.Parsers.Interfaces;
using SimpleWebCrawler.Core.Parsers.Models;
using SimpleWebCrawler.Core.Processors.Interfaces;
using SimpleWebCrawler.Core.Processors.Models;
using SimpleWebCrawler.Core.Results.Models;
using SimpleWebCrawler.Core.Settings.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


namespace SimpleWebCrawler.Core.Processors.Bases
{
    public abstract class SiteProcessorBase : SiteMapXmlParserBase, ISiteProcessor, ISiteParser
    {
        ILogger _logger;
        IServiceProvider _serviceProvider;

        public static bool LogVerbose { get; set; }
        private static HttpClient? _httpClient;
        private static int _pageCheckThreshold = 16;
        private static int _pageReTryThreshold = 3;
        private static object _pageLock = new object();
        private static object _pageChangeLock = new object();
        private static HashSet<string> _pageChecks = new HashSet<string>();
        private static HashSet<string> _pageProcessed = new HashSet<string>();
        private static IPageParser? _pageParser;
        private static IRobotTextParser? _robotTextParser;
        private static IPageHtmlParser? _pageHtmlParser;
        private static IPagePointOfInterestStrategy? _pagePointOfInterestStrategy;
        public SiteProcessorBase(ILogger logger, IServiceProvider services, IConfigurationRoot configuration)
        {
            SWCSettings? settings;
            _httpClient = new();
            _httpClient.Timeout = new TimeSpan(0, 0, 0, 25, 0, 0);
            _logger = logger;
            _serviceProvider = services;
            //Apply Settings
            if (configuration != null)
            {
                settings = configuration.GetConfigObject<SWCSettings>("SWCSettings");
                if (settings != null)
                {
                    if (settings.PageCheckThreshold.GetValueOrDefault() > 0)
                    {
                        _pageCheckThreshold = settings.PageCheckThreshold.GetValueOrDefault();
                    }
                    if (settings.PageReTryThreshold.GetValueOrDefault() > 0)
                    {
                        _pageReTryThreshold = settings.PageReTryThreshold.GetValueOrDefault();
                    }
                    if (settings.DefaultHeaders != null)
                    {
                        foreach (var header in settings.DefaultHeaders)
                        {
                            if (!string.IsNullOrWhiteSpace(header.Key))
                            {
                                _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                            }
                        }
                    }
                }
            }
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            _pageParser = _serviceProvider.GetRequiredService<IPageParser>();
            //if (_pageParser == null)
            //{
            //    throw new ArgumentNullException("You must configure a page parser");
            //}
            _robotTextParser = _serviceProvider.GetRequiredService<IRobotTextParser>();
            //if (_robotTextParser == null)
            //{
            //    throw new ArgumentNullException("You must configure a robot test parser");
            //}
            _pageHtmlParser = _serviceProvider.GetRequiredService<IPageHtmlParser>();
            //if (_pageHtmlParser == null)
            //{
            //    throw new ArgumentNullException("You must configure a page html parser");
            //}
            _pagePointOfInterestStrategy = _serviceProvider.GetRequiredService<IPagePointOfInterestStrategy>();
            //if (_pagePointOfInterestStrategy == null)
            //{
            //    throw new ArgumentNullException("You must configure a page point of interest strategy");
            //}
        }
        public SiteResult CreateSiteResult(string sourceSiteURL)
        {
            SiteResult results = new SiteResult();
            Uri uri = new Uri(sourceSiteURL);
            results.SiteURL = $"{uri.Scheme}://{uri.Host}";
            results.SiteHost = uri.Host;
            return results;
        }

        public async Task ProcessSiteAsync(SiteResult source)
        {
            await GetSiteMapsAsync(source);

            CrawlPageItems(source);
        }
        public async Task ProcessPageAsync(SiteResult source, string url)
        {
            await GetSiteMapsAsync(source, false);

            bool wasOnSiteMap = WasOnSiteMap(source, url);

            SearchPage? sp = source.BuildSearchPageEntry(url, wasOnSiteMap);
            if (sp != null)
            {
                source.PageResults = new List<SearchPage>() { sp };
                CrawlPageItems(source, false);
            }
        }

        public bool WasOnSiteMap(SiteResult siteResult, string url)
        {
            return WasOnSiteMap(siteResult, new Uri(url));
        }
        public bool WasOnSiteMap(SiteResult siteResult, Uri url)
        {
            if (siteResult != null && siteResult.SiteMaps != null)
            {
                foreach (var sm in siteResult.SiteMaps)
                {
                    //Exact Compare on AbsoluteUri / SiteMaps should be Exact for speed
                    if (sm.Items != null)
                    {
                        foreach (var item in sm.Items)
                        {
                            if (item.Loc != null && siteResult.NormalizeToFinializedURL(item.Loc) == url.AbsoluteUri)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public void CrawlPageItems(SiteResult source, bool searchForOtherPages = true)
        {
            Stopwatch sw = Stopwatch.StartNew();

            if (source != null && source.PageResults != null)
            {
                _pageChecks.Clear();
                _pageProcessed.Clear();
                ServicePointManager.DefaultConnectionLimit = _pageCheckThreshold;
                bool init = false;
                int showProgressInterval = 0;
                List<SearchPage> results = new List<SearchPage>();
                int zeroChecks = 0;
                int run = 0;
                while (!init || results != null && results.Count > 0 || _pageChecks.Count > 0)
                {
                    run++;
                    //New Pages maybe found during each run. This will trigger another run.
                    try
                    {
                        init = true;
                        LogVerbose = false;

                        lock (_pageChangeLock)
                        {
                            results = source.PageResults.Where(x => string.IsNullOrWhiteSpace(x.Status) && x.PageTrys <= _pageReTryThreshold).OrderBy(x => x.PageTrys).ThenBy(x => x.CrawlDateTime.GetValueOrDefault()).ToList();
                        }
                        zeroChecks++;
                        if (results != null && results.Count > 0)
                        {
                            zeroChecks = 0;
                            int processed = 0;
                            foreach (SearchPage result in results)
                            {

                                while (_pageChecks.Count >= _pageCheckThreshold)
                                {
                                    Thread.Sleep(1000);
                                }
                                if (showProgressInterval != sw.Elapsed.Minutes)
                                {
                                    showProgressInterval = sw.Elapsed.Minutes;
                                    _logger.LogInformation($">>>>>>>>Run: {run} Left: {results.Count - processed} Processed: {_pageProcessed.Count} <<<<<<<<<< Duration: {sw.Elapsed.ToString(@"dd\.hh\:mm\:ss")}");
                                }
                                
                                lock (_pageChangeLock)
                                {
                                    if (result.Url != null && result.Url.AbsoluteUri != null && !_pageChecks.Contains(result.Url.AbsoluteUri) && !_pageProcessed.Contains(result.Url.AbsoluteUri))
                                    {
                                        //Add Page for locked resource
                                        _pageChecks.Add(result.Url.AbsoluteUri);
                                        //Build thread worker
                                        var th = new Thread(() => GetCrawlerPageItemAsync(source, result, searchForOtherPages));
                                        th.Name = result.Url.AbsoluteUri;
                                        //Start new thread worker
                                        th.Start();
                                        processed++;
                                    }

                                }

                            }
                            _logger.LogInformation($">>>>>>>>Run: {run} Left: {results.Count - processed} Processed: {_pageProcessed.Count} <<<<<<<<<< Duration: {sw.Elapsed.ToString(@"dd\.hh\:mm\:ss")}");

                        }
                        Thread.Sleep(1000);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error: {ex.Message}");
                    }
                    if (zeroChecks >= 20)
                    {
                        break;
                    }
                }
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed.ToString(@"dd\.hh\:mm\:ss"));
        }
        public async void GetCrawlerPageItemAsync(SiteResult siteResult, SearchPage source, bool searchForOtherPages = true)
        {
            if (source != null && source.Url != null && _pageParser != null && _httpClient != null)
            {
                try
                {
                    source.PageTrys++;
                    if (source.PageTrys > 1)
                    {
                        _logger.LogWarning($"{source.Url} => {source.PageTrys}");
                    }
                    else
                    {
                        if (LogVerbose)
                        {
                            _logger.LogInformation($"{source.Url}");
                        }
                    }
                    PageHttpResult? result = await _pageParser.GetResultAsStringAsync(_httpClient, source.Url.AbsoluteUri);

                    if (result != null && result.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        if (source.PageTrys > 1) { source.PageTrys--; }
                    }


                    if (result != null)
                    {
                        source.HasIssues = false;
                        source.StatusCode = result.StatusCode;
                        source.LoadTime = result.TimeElapsed;
                        source.PageItems = new List<SearchPageItem>();
                        if (source.PageTrys > 1 && (int)source.StatusCode >= 200 && (int)source.StatusCode < 300)
                        {
                            _logger.LogInformation($"resolved: {source.Url} => {source.PageTrys}");
                        }
                        if (result.IsSuccess)
                        {
                            if (!source.IsExternalPage && !string.IsNullOrWhiteSpace(result.Response) && _pageHtmlParser != null && _pagePointOfInterestStrategy != null)
                            {
                                List<ISimpleHtmlElement> simpleHtmlElements = _pageHtmlParser.GetPageElements(result.Response);

                                if (simpleHtmlElements != null)
                                {
                                    foreach (SimpleHtmlElement ele in simpleHtmlElements)
                                    {
                                        SearchPageItem searchPageItem = _pagePointOfInterestStrategy.ConvertSimpleHtmlElementAndCheck(siteResult, source, ele);
                                        if (searchPageItem != null && searchPageItem.ParentUrl != null)
                                        {
                                            source.PageItems.Add(searchPageItem);
                                            if (searchPageItem.Url != null && searchForOtherPages)
                                            {
                                                lock (_pageChangeLock)
                                                {
                                                    SearchPage? sp = siteResult.BuildSearchPageEntry(searchPageItem.Url, false);
                                                    if (sp != null)
                                                    {
                                                        if (siteResult.PageResults != null && sp.Url != null && !siteResult.IgnoreUrl(sp.Url.AbsoluteUri) && !sp.IsExternalPage && siteResult.PageResults.FirstOrDefault(x => x.Url != null && x.Url.AbsoluteUri == sp.Url.AbsoluteUri) == null)
                                                        {
                                                            siteResult.PageResults.Add(sp);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (_pagePointOfInterestStrategy != null)
                            {
                                _pagePointOfInterestStrategy.PostChecks(siteResult, source);
                            }
                            lock (_pageChangeLock)
                            {
                                _pageProcessed.Add(source.Url.AbsoluteUri);
                                source.Status = "Processed";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex.Message);
                }
                lock (_pageChangeLock)
                {
                    _pageChecks.Remove(source.Url.AbsoluteUri);
                }
            }
        }
        public List<PageIssue> ConvertToPageIssues(SiteResult siteResult)
        {
            List<PageIssue> issues = new List<PageIssue>();
            if (siteResult != null)
            {
                if (siteResult.PageResults != null)
                {
                    foreach (var pageResult in siteResult.PageResults)
                    {
                        PageIssue pi = new PageIssue() { Url = pageResult.Url };
                        if (pageResult.HasIssues || pageResult.Issues != null && pageResult.Url != null)
                        {
                            pi.Issues = new List<string>();
                            if (pageResult.Issues != null)
                            {
                                pi.Issues.AddRange(pageResult.Issues);
                            }
                            if (pageResult.PageItems != null)
                            {
                                foreach (var item in pageResult.PageItems)
                                {
                                    if (item.Issues != null)
                                    {
                                        pi.Issues.AddRange(item.Issues);
                                    }
                                }
                            }
                            if (pi.Issues.Count > 0)
                            {
                                issues.Add(pi);
                            }
                        }
                    }
                }
            }
            return issues;
        }
        /// <summary>
        /// For Space savings one could ingore Issue properties on writing to source and then reapply each time when reading.
        /// </summary>
        /// <param name="siteResult"></param>
        public void ReApplyIssues(SiteResult siteResult)
        {
            if (siteResult != null && siteResult.PageResults != null)
            {
                foreach (var sp in siteResult.PageResults)
                {
                    sp.HasIssues = false;
                    sp.Issues = new List<string> { };
                    sp.PagePointOfInterests = null;
                    if (sp.PageItems != null)
                    {
                        foreach (var item in sp.PageItems)
                        {
                            item.Issues = new List<string>();
                            if (_pagePointOfInterestStrategy != null)
                            {
                                _pagePointOfInterestStrategy.ItemChecks(siteResult, sp, item);
                            }
                        }
                        if (_pagePointOfInterestStrategy != null)
                        {
                            _pagePointOfInterestStrategy.PostChecks(siteResult, sp);
                        }
                    }
                }
            }
        }


        public async Task GetRobotsTxtResultsAsync(SiteResult source)
        {
            if (_robotTextParser != null && _pageParser != null && _httpClient != null)
            {
                source.SiteMaps = new List<SiteMap>();
                source.RobotIgnorePaths = new List<string>();
                PageHttpResult rtnVal = await _pageParser.GetResultAsStringAsync(_httpClient, source.NormalizeToFinializedURL("robots.txt"));
                if (rtnVal.IsSuccess && !string.IsNullOrWhiteSpace(rtnVal.Response))
                {
                    _robotTextParser.Process(rtnVal.Response);
                    if (_robotTextParser.IgnorePaths != null && _robotTextParser.IgnorePaths.Count > 0)
                    {
                        source.RobotIgnorePaths.AddRange(_robotTextParser.IgnorePaths);
                    }
                    if (_robotTextParser.SiteMapURLs != null && _robotTextParser.SiteMapURLs.Count > 0)
                    {
                        source.SiteMaps.AddRange(_robotTextParser.SiteMapURLs.Select(x => new SiteMap() { URL = x }));
                    }
                }
            }
        }


        public async Task GetSiteMapsAsync(SiteResult source, bool addToCrawl = true)
        {
            _logger.LogInformation("Getting Site Maps...");
            source.PageResults = new List<SearchPage>();

            _logger.LogInformation("Check Robots.txt...");
            await GetRobotsTxtResultsAsync(source);
            //Todo search for site maps if robots.txt does not exist

            if (source.SiteMaps != null && source.SiteMaps.Count > 0)
            {
                _logger.LogInformation("Processing Site maps");
                //Process Site Maps to Items
                List<SiteMap>? temp = null;
                bool init = false;
                while (!init || temp != null && temp.Count > 0)
                {
                    Thread.Sleep(1000);
                    temp = source.SiteMaps.Where(x => string.IsNullOrWhiteSpace(x.Status) && x.Trys < 10).ToList();
                    init = true;

                    foreach (var map in temp)
                    {
                        await GetSiteMapItems(source, map, addToCrawl);
                    }
                }
            }
        }

        public async Task GetSiteMapItems(SiteResult siteResult, SiteMap siteMap, bool addToCrawl = true)
        {
            if (siteResult != null && siteResult.SiteMaps != null && siteMap != null && !string.IsNullOrWhiteSpace(siteMap.URL))
            {
                if (siteResult.SiteMaps.Count % 10 == 0)
                {
                    if (LogVerbose) { _logger.LogInformation("More..."); }
                }
                Console.Write(".");
                SiteMap? xSiteMap = siteResult.SiteMaps.FirstOrDefault(x => string.IsNullOrWhiteSpace(x.Status) && x.URL == siteMap.URL);
                if (xSiteMap != null)
                {
                    xSiteMap.Trys++;
                    xSiteMap.Items = new List<SiteMapItem>();
                    //string siteSearchURI = GetSiteSearchURI(siteResult, siteMap.URL);
                    if (!string.IsNullOrWhiteSpace(siteMap.URL) && _pageParser != null && _httpClient != null)
                    {
                        PageHttpResult rtnVal = await _pageParser.GetResultAsStringAsync(_httpClient, siteMap.URL);
                        if (rtnVal.IsSuccess && !string.IsNullOrWhiteSpace(rtnVal.Response))
                        {
                            GetSiteMapItemsFromXml(siteResult, xSiteMap, rtnVal.Response, addToCrawl);
                        }
                    }
                }
            }
        }
    }
}
