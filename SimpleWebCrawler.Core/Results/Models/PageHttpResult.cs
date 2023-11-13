using System.Net;

namespace SimpleWebCrawler.Core.Results.Models
{
    public class PageHttpResult
    {
        public HttpStatusCode StatusCode { get; set; }
        public string? Response { get; set; }
        public bool IsSuccess { get; set; }
        public string? Error { get; set; }

        public TimeSpan? TimeElapsed { get; set; }
    }
}
