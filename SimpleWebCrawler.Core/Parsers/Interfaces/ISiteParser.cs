using SimpleWebCrawler.Core.Components.Models;
using SimpleWebCrawler.Core.Results.Models;

namespace SimpleWebCrawler.Core.Parsers.Interfaces
{
    public interface ISiteParser
    {
        public Task GetRobotsTxtResultsAsync(SiteResult source);
        public void GetCrawlerPageItemAsync(SiteResult siteResult, SearchPage source, bool searchForOtherPages = true);
        public Task GetSiteMapsAsync(SiteResult source, bool addToCrawl = true);
    }
}
