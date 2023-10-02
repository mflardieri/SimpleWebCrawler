using SimpleWebCrawler.Core.Components.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
