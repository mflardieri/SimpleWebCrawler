using SimpleWebCrawler.Core.Components.Models;
using System.Net;

namespace SimpleWebCrawler.Core.Results.Models
{
    public class CrawlerResult
    {
        public DateTime? CrawlDateTime { get; private set; }
        private string? _Url { get; set; }
        public string? Url
        {
            get { return _Url; }
            set { if (!CrawlDateTime.HasValue) { CrawlDateTime = DateTime.Now; } _Url = value; }
        }
        public string? Status { get; set; }
        public bool WasOnSiteMap { get; set; }
        public bool IsExternalPage { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public int PageTrys { get; set; }
        public List<string>? FoundOnURLs { get; set; }
        public List<SearchPageItem>? PageItems { get; set; }
        public List<string>? Issues { get; set; }
    }
}
