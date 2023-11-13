using SimpleWebCrawler.Core.Components.Models;

namespace SimpleWebCrawler.Core.Components.Interfaces
{
    public interface IPageItem
    {
        public Uri? Url { get; set; }
        public bool IsExternalPage { get; set; }
        public bool WasOnSiteMap { get; set; }
        public List<SearchPageItem>? PageItems { get; set; }
        public string? FirstParentUrl { get; set; }
    }
}
