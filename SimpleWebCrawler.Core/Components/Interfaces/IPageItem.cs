using SimpleWebCrawler.Core.Components.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWebCrawler.Core.Components.Interfaces
{
    public interface IPageItem
    {
        public Uri? Url { get; set; }
        public bool IsExternalPage { get; set; }
        public bool WasOnSiteMap { get; set; }
        public List<SearchPageItem>? PageItems { get; set; }
    }
}
