using SimpleWebCrawler.Core.Components.Interfaces;
using SimpleWebCrawler.Core.Components.Models;
using SimpleWebCrawler.Core.Results.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWebCrawler.Core.Processors.Interfaces
{
    public interface IPagePointOfInterestStrategy
    {
        public SearchPageItem ConvertSimpleHtmlElementAndCheck(SiteResult siteResult, ISearchPage searchPage, SimpleHtmlElement simpleHtmlElement);
        public void ItemChecks(SiteResult siteResult, ISearchPage searchPage, SearchPageItem searchPageItem);
        public void PostChecks(SiteResult siteResult, ISearchPage searchPage);
    }
}
