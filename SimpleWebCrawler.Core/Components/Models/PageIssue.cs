using SimpleWebCrawler.Core.Components.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWebCrawler.Core.Components.Models
{
    public class PageIssue : IIssueItem
    {
        public Uri? Url { get; set; }
        public List<string>? Issues { get; set; }
    }
}
