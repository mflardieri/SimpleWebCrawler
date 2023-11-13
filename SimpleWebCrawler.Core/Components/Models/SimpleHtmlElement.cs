using SimpleWebCrawler.Core.Components.Interfaces;

namespace SimpleWebCrawler.Core.Components.Models
{
    public class SimpleHtmlElement : ISimpleHtmlElement
    {
        public int Index { get; set; }
        public string? Node { get; set; }
        public string? Text { get; set; }
        public Dictionary<string, string>? Attributes { get; set; }
    }
}
