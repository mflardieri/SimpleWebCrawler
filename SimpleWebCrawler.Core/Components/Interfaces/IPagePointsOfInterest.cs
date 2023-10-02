using SimpleWebCrawler.Core.Components.Enums;
using SimpleWebCrawler.Core.Components.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWebCrawler.Core.Components.Interfaces
{
    public interface IPagePointsOfInterest
    {
        public bool HasIssues { get; set; }
        #region [ Auto Checked ] 
        public bool WasHttp { get; set; }
        public int PageTrys { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public PageLoadSpeed LoadSpeed { get; set; }
        public TimeSpan? LoadTime { get; set; }
        public string? PageTitle { get; set; }
        public string? MetaKeywords { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaRobots { get; set; }
        #endregion [ Auto Checked ] 
        /// <summary>
        /// Not checked on external pages
        /// </summary>
        public List<PagePointOfInterest>? PagePointOfInterests { get; set; }
    }
}
