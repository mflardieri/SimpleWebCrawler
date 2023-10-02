using SimpleWebCrawler.Core.Components.Enums;

namespace SimpleWebCrawler.Core.Components.Models
{
    /// <summary>
    /// Not checked on enternal pages
    /// </summary>
    public class PagePointOfInterest
    {
        public PagePointOfInterestCheck PointOfInterestCheck { get; set; }
        public bool IssueFound { get; set; }
    }
}
