using SimpleWebCrawler.Core.Components.Models;
using SimpleWebCrawler.Core.Results.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
