namespace SimpleWebCrawler.Core.Components.Interfaces
{
    public interface ISimpleHtmlElement
    {
        public int Index { get; set; }
        public string? Node { get; set; }
        public string? Text { get; set; }
        public Dictionary<string, string>? Attributes { get; set; }
    }
}
