using SimpleWebCrawler.Core.Components.Enums;
using SimpleWebCrawler.Core.Components.Models;

namespace SimpleWebCrawler.Core.Results.Models
{
    public class SiteResult
    {
        public string? SiteURL { get; set; }
        public string? SiteHost { get; set; }
        public List<SiteMap>? SiteMaps { get; set; }
        public List<PagePointOfInterestCheck>? pagePointOfInterestChecks { get; set; }
        public List<string>? RobotIgnorePaths { get; set; }
        public List<SearchPage>? PageResults { get; set; }


        public SearchPage? BuildSearchPageEntry(string url, bool wasOnSiteMap)
        {
            SearchPage? searchPage = null;
            if (url != null)
            {
                searchPage = new SearchPage();
                searchPage.Issues = new List<string>();
                try
                {

                    if (url != null && url.ToLower().Trim().StartsWith("http:") && !url.ToLower().Trim().StartsWith("https:"))
                    {
                        searchPage.WasHttp = true;//Not good for the user's trust
                    }
                    string finializedUrl = url ?? "";
                    if (!IsExternalPage(finializedUrl))
                    {
                        finializedUrl = NormalizeToFinializedURL(finializedUrl);
                    }
                    else
                    {
                        searchPage.IsExternalPage = true;
                    }

                    searchPage.Url = new Uri(finializedUrl);
                    searchPage.CrawlDateTime = DateTime.Now;
                    searchPage.WasOnSiteMap = wasOnSiteMap;
                }
                catch (Exception ex)
                {
                    searchPage.Issues.Add(ex.Message);
                    searchPage.Issues.Add(url);
                    Console.WriteLine(ex.ToString());
                }
            }
            return searchPage;
        }

        public bool IsExternalPage(string uri)
        {
            return !string.IsNullOrWhiteSpace(uri) && !uri.StartsWith("/") && (uri.StartsWith("http:") || uri.StartsWith("https:")) && !string.IsNullOrWhiteSpace(SiteHost) && !uri.ToLower().StartsWith($"http://{SiteHost.ToLower()}") && !uri.ToLower().StartsWith($"https://{SiteHost.ToLower()}");
        }
        public string NormalizeToFinializedURL(string url)
        {
            string finializedUrl = NormalizeToUri(url ?? "");
            if (!IsExternalPage(finializedUrl))
            {
                finializedUrl = $"{SiteURL}/{finializedUrl}";
            }
            return finializedUrl;
        }
        public string NormalizeToUri(string uri)
        {
            if (!string.IsNullOrWhiteSpace(uri) && !string.IsNullOrWhiteSpace(SiteHost))
            {
                if (uri.ToLower().StartsWith($"https://{SiteHost.ToLower()}")) { uri = uri.Substring($"https://{SiteHost}".Length); }
                if (uri.ToLower().StartsWith($"http://{SiteHost.ToLower()}")) { uri = uri.Substring($"http://{SiteHost}".Length); }
                while (uri.StartsWith("/"))
                {
                    uri = uri[1..];
                }
                while (uri.EndsWith("/"))
                {
                    uri = uri.Substring(0, uri.Length - 1);
                }
            }
            return uri;
        }
        public bool IgnoreUrl(string uri)
        {
            if (!string.IsNullOrWhiteSpace(uri) && !string.IsNullOrWhiteSpace(SiteHost))
            {
                if (!IsExternalPage(uri))
                {
                    uri = NormalizeToUri(uri);
                    if (uri.ToLower().StartsWith("tel:"))
                    {
                        return true;
                    }
                    if (uri.ToLower().StartsWith("mailto:"))
                    {
                        return true;
                    }
                    if (RobotIgnorePaths != null)
                    {
                        foreach (var item in RobotIgnorePaths)
                        {
                            if (!string.IsNullOrWhiteSpace(item))
                            {
                                if ($"/{uri.ToLower()}/".StartsWith(item.ToLower()))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
