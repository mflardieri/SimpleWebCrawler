using SimpleWebCrawler.Core.Results.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWebCrawler.Core.Parsers.Interfaces
{
    public interface IPageParser
    {
        public Task<PageHttpResult> GetResultAsStringAsync(HttpClient httpClient, string uri, bool ensureSuccessStatusCode = true);
    }
}
