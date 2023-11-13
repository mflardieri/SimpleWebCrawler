using SimpleWebCrawler.Core.Components.Enums;
using SimpleWebCrawler.Core.Components.Interfaces;
using System.Net;

namespace SimpleWebCrawler.Core.Components.Models
{
    public class SearchPage : ISearchPage, ICrawlerPage
    {
        public bool HasIssues { get; set; }
        public bool WasHttp { get; set; }
        public int PageTrys { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public PageLoadSpeed LoadSpeed { get; set; }
        public TimeSpan? LoadTime { get; set; }
        public List<PagePointOfInterest>? PagePointOfInterests { get; set; }
        public Uri? Url { get; set; }
        public bool IsExternalPage { get; set; }
        public List<string>? Issues { get; set; }
        public DateTime? CrawlDateTime { get; set; }
        public string? Status { get; set; }
        public bool WasOnSiteMap { get; set; }
        public List<SearchPageItem>? PageItems { get; set; }
        public string? PageTitle { get; set; }
        public string? MetaKeywords { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaRobots { get; set; }
        public string? FirstParentUrl { get; set; }
    }
}
