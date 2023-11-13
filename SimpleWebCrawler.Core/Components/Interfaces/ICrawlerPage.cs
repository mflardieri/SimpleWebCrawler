using System.Net;

namespace SimpleWebCrawler.Core.Components.Interfaces
{
    internal interface ICrawlerPage : IPageItem
    {
        public DateTime? CrawlDateTime { get; set; }
        public string? Status { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public int PageTrys { get; set; }
    }
}
