using SimpleWebCrawler.Core.Components.Interfaces;

namespace SimpleWebCrawler.Core.Components.Models
{
    public class SearchPageItem : IIssueItem
    {
        public string? ParentUrl { get; set; }
        public SimpleHtmlElement? HtmlNode { get; set; }
        public string? HtmlNodeUrl { get; set; }
        public string? Url { get; set; }
        public List<string>? Issues { get; set; }
    }
}
