namespace SimpleWebCrawler.Core.Components.Models
{
    public class SiteMap
    {
        public string? URL { get; set; }
        public List<SiteMapItem>? Items { get; set; }
        public string? Status { get; set; }
        public int Trys { get; set; }
    }
}
