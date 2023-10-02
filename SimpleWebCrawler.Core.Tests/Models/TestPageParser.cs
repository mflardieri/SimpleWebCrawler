using SimpleWebCrawler.Core.Parsers.Interfaces;
using SimpleWebCrawler.Core.Results.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWebCrawler.Core.Tests.Models
{
    public class TestPageParser : IPageParser
    {

        private Task<PageHttpResult> GetTestResponse(string uri)
        {
            PageHttpResult rtnVal = new();
            switch (uri)
            {
                case "https://test.com/robots.txt":
                    rtnVal = GetTestRobotStringResponse();
                    break;
                case "https://test.com/sitemap.xml":
                    rtnVal = GetTestSiteMapResponse();
                    break;
                case "https://test.com/content-sitemap.xml":
                    rtnVal = GetTestSiteMapContentResponse();
                    break;
                case "https://test.com/content":
                    rtnVal = GetTestContentPageResponse();
                    break;
                case "https://test.com/about":
                    break;
                default:
                    rtnVal.IsSuccess = true;
                    rtnVal.TimeElapsed = new TimeSpan(0, 0, 0, 0, 10);
                    rtnVal.StatusCode = HttpStatusCode.NotFound;
                    break;
            }
            return Task.FromResult(rtnVal);
        }
        public PageHttpResult GetTestContentPageResponse()
        {
            PageHttpResult rtnVal = new();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<a href=\"/test\">Test</a>");
            sb.AppendLine("<a href=\"/about\">about</a>");
            sb.AppendLine("<a href=\"#\"><img src=\"\test.jpg\"/></a>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            rtnVal.IsSuccess = true;
            rtnVal.TimeElapsed = new TimeSpan(0, 0, 0, 0, 200);
            rtnVal.StatusCode = HttpStatusCode.OK;
            rtnVal.Response = sb.ToString();

            return rtnVal;
        }
        public PageHttpResult GetTestSiteMapContentResponse()
        {
            PageHttpResult rtnVal = new();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
            sb.AppendLine("<url><loc>https://test.com/content</loc><lastmod>2023-03-14T15:14:15+00:00</lastmod></url>");
            sb.AppendLine("<url><loc>https://test.com/about</loc><lastmod>2023-03-14T15:14:15+00:00</lastmod></url>");
            sb.AppendLine("</urlset>");
            rtnVal.IsSuccess = true;
            rtnVal.TimeElapsed = new TimeSpan(0, 0, 0, 0, 200);
            rtnVal.StatusCode = HttpStatusCode.OK;
            rtnVal.Response = sb.ToString();

            return rtnVal;
        }
        public PageHttpResult GetTestSiteMapResponse()
        {
            PageHttpResult rtnVal = new();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<sitemapindex xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
            sb.AppendLine("<sitemap><loc>https://test.com/content-sitemap.xml</loc><lastmod>2023-03-14T15:14:15+00:00</lastmod></sitemap>");
            sb.AppendLine("</sitemapindex>");

            rtnVal.IsSuccess = true;
            rtnVal.TimeElapsed = new TimeSpan(0, 0, 0, 0, 200);
            rtnVal.StatusCode = HttpStatusCode.OK;
            rtnVal.Response = sb.ToString();

            return rtnVal;
        }
        public PageHttpResult GetTestRobotStringResponse()
        {

            PageHttpResult rtnVal = new();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("# robots.txt for https://test.com/");
            sb.AppendLine("");
            sb.AppendLine("sitemap: https://test.com/sitemap.xml");
            sb.AppendLine("");
            sb.AppendLine("User-agent: *");
            sb.AppendLine("Disallow: /ignore/");
            sb.AppendLine("Disallow: /cache/");

            rtnVal.IsSuccess = true;
            rtnVal.TimeElapsed = new TimeSpan(0, 0, 0, 0, 200);
            rtnVal.StatusCode = HttpStatusCode.OK;
            rtnVal.Response = sb.ToString();
            return rtnVal;
        }
        public async Task<PageHttpResult> GetResultAsStringAsync(HttpClient httpClient, string uri, bool ensureSuccessStatusCode = true)
        {
            PageHttpResult rtnVal = new();
            try
            {
                rtnVal = await GetTestResponse(uri);
            }
            catch (Exception ex)
            {
                rtnVal.Error = ex.Message;
            }
            return rtnVal;
        }

    }
}
