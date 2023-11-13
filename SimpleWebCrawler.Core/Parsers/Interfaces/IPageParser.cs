using SimpleWebCrawler.Core.Results.Models;

namespace SimpleWebCrawler.Core.Parsers.Interfaces
{
    public interface IPageParser
    {
        public Task<PageHttpResult> GetResultAsStringAsync(HttpClient httpClient, string uri, bool ensureSuccessStatusCode = true);
    }
}
