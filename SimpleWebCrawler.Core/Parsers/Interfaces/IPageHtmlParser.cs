using SimpleWebCrawler.Core.Components.Interfaces;

namespace SimpleWebCrawler.Core.Parsers.Interfaces
{
    public interface IPageHtmlParser
    {
        public List<ISimpleHtmlElement> GetPageElements(string source);
    }
}
