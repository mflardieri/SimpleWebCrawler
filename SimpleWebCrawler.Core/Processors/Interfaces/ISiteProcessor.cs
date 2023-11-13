using SimpleWebCrawler.Core.Components.Models;
using SimpleWebCrawler.Core.Results.Models;

namespace SimpleWebCrawler.Core.Processors.Interfaces
{
    public interface ISiteProcessor
    {
        public SiteResult CreateSiteResult(string sourceSiteURL);
        public Task ProcessSiteAsync(SiteResult source);
        public Task ProcessPageAsync(SiteResult source, string url);
        void ReApplyIssues(SiteResult siteResult);
        List<PageIssue> ConvertToPageIssues(SiteResult siteResult);
    }
}
