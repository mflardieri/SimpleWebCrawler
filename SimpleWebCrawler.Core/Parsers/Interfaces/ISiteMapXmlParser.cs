using SimpleWebCrawler.Core.Components.Models;
using SimpleWebCrawler.Core.Results.Models;

namespace SimpleWebCrawler.Core.Parsers.Interfaces
{
    public interface ISiteMapXmlParser
    {
        public void GetSiteMapItemsFromXml(SiteResult siteResult, SiteMap siteMap, string sourceXml);
    }
}
