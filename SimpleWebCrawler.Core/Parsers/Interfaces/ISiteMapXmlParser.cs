using SimpleWebCrawler.Core.Components.Models;
using SimpleWebCrawler.Core.Results.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SimpleWebCrawler.Core.Parsers.Interfaces
{
    public interface ISiteMapXmlParser
    {
        public void GetSiteMapItemsFromXml(SiteResult siteResult, SiteMap siteMap, string sourceXml);
    }
}
