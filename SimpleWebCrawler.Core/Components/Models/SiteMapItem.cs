using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWebCrawler.Core.Components.Models
{
    public class SiteMapItem
    {
        public string? Loc { get; set; }
        public DateTime? Lastmod { get; set; }
    }
}
