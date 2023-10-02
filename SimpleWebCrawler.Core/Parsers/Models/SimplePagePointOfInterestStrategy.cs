using SimpleWebCrawler.Core.Components.Enums;
using SimpleWebCrawler.Core.Components.Interfaces;
using SimpleWebCrawler.Core.Components.Models;
using SimpleWebCrawler.Core.Processors.Interfaces;
using SimpleWebCrawler.Core.Results.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWebCrawler.Core.Parsers.Models
{
    public class SimplePagePointOfInterestStrategy : IPagePointOfInterestStrategy
    {
        public void PostChecks(SiteResult siteResult, ISearchPage searchPage)
        {
            if (searchPage.Issues == null) { searchPage.Issues = new List<string>(); }
            if (searchPage.PageItems != null && searchPage.PageItems.Count > 0)
            {
                if (searchPage.WasHttp)
                {
                    searchPage.Issues.Add("Http protocol on url.");
                }
                if ((int)searchPage.StatusCode > 299)
                { 
                    searchPage.Issues.Add($"Page Responsed with: {((int)searchPage.StatusCode)} ... {searchPage.StatusCode}");
                }
                //Set Load Speed
                if (searchPage.LoadTime != null)
                {
                    if (searchPage.LoadTime.Value.TotalSeconds < 1)
                    {
                        searchPage.LoadSpeed = PageLoadSpeed.SuperFast;
                    }
                    else if (searchPage.LoadTime.Value.TotalSeconds < 2)
                    {
                        searchPage.LoadSpeed = PageLoadSpeed.Fast;
                    }
                    else if (searchPage.LoadTime.Value.TotalSeconds < 3)
                    {
                        searchPage.LoadSpeed = PageLoadSpeed.Medium;
                    }
                    else if (searchPage.LoadTime.Value.TotalSeconds < 4)
                    {
                        searchPage.LoadSpeed = PageLoadSpeed.Slow;
                    }
                    else if (searchPage.LoadTime.Value.TotalSeconds >= 4)
                    {
                        searchPage.LoadSpeed = PageLoadSpeed.SuperSlow;
                    }
                }
                if (searchPage.LoadSpeed != PageLoadSpeed.None && 
                    searchPage.LoadSpeed != PageLoadSpeed.SuperFast && 
                    searchPage.LoadSpeed != PageLoadSpeed.Fast && 
                    searchPage.LoadSpeed != PageLoadSpeed.Medium && searchPage.LoadTime.HasValue)
                {
 
                    searchPage.Issues.Add($"Page Load Time: {searchPage.LoadTime.Value.ToString(@"dd\.hh\:mm\:ss")} is {searchPage.LoadSpeed}");
                    Console.Write("");
                }
                
                if (string.IsNullOrWhiteSpace(searchPage.MetaDescription))
                {
                    searchPage.Issues.Add("Has no meta description for page.");
                }
                else
                { 
                    if (searchPage.MetaDescription.Length < 150)
                    {
                        searchPage.Issues.Add("Meta description is too short. Make it between 150 to 160 charaters.");
                    }
                    if (searchPage.MetaDescription.Length > 160)
                    {
                        searchPage.Issues.Add("Meta description is too long. Make it between 150 to 160 charaters.");
                    }
                }
                if (string.IsNullOrWhiteSpace(searchPage.MetaKeywords))
                {
                    searchPage.Issues.Add("Has no meta keywords for page.");
                }
                if (string.IsNullOrWhiteSpace(searchPage.MetaRobots))
                {
                    searchPage.Issues.Add("Has no meta robots for page.");
                }

                if (string.IsNullOrWhiteSpace(searchPage.PageTitle))
                {
                    searchPage.Issues.Add("Has no title for page.");
                }
                else
                {
                    if (searchPage.PageTitle.Length < 30)
                    {
                        searchPage.Issues.Add("Page Title is too short. Make it between 30 to 60 charaters.");
                    }
                    if (searchPage.PageTitle.Length > 60)
                    {
                        searchPage.Issues.Add("Page Title is too long. Make it between 30 to 60 charaters.");
                    }
                }
                if (searchPage.WasOnSiteMap && searchPage.HasIssues)
                {
                    searchPage.Issues.Add("This page is on the site map with issues");
                }
            }
            if (!searchPage.IsExternalPage && searchPage.PagePointOfInterests != null)
            {
                //Text plus link url
                //Duplicate Links on a page
                if (searchPage.PagePointOfInterests != null && searchPage.PageItems != null)
                {
                    HashSet<string> duplinks = new HashSet<string>();

                    var DuplicateLinksChecks = searchPage.PagePointOfInterests.FirstOrDefault(x => x.PointOfInterestCheck == PagePointOfInterestCheck.DuplicatedLinks);
                    foreach (var item in searchPage.PageItems)
                    {
                        if (item.Issues == null) { item.Issues = new List<string>(); }
                        if (DuplicateLinksChecks != null && item.Url != null && item.HtmlNode != null && item.HtmlNode.Node == "a")
                        {
                            string linkCheck = $"{item.Url}=>{item.HtmlNode.Text}";
                            if (duplinks.Contains(linkCheck))
                            {
                                searchPage.HasIssues = true;
                                DuplicateLinksChecks.IssueFound = true;
                                item.Issues.Add($"Duplicate Link: '{item.HtmlNode.Node}' Text:'{item.HtmlNode.Text}' Link: {item.Url} at Index: {item.HtmlNode.Index}");
                            }
                            else
                            {
                                duplinks.Add(linkCheck);
                            }
                        }
                    }
                }
            }


            if (!searchPage.HasIssues && searchPage.Issues.Count > 0) { searchPage.HasIssues = true; }
        }
        public void ItemChecks(SiteResult siteResult, ISearchPage searchPage, SearchPageItem searchPageItem)
        {
            if (searchPageItem != null && searchPageItem.HtmlNode != null)
            {
                if (searchPage.Issues == null) { searchPage.Issues = new List<string>(); }
                searchPageItem.Issues = new List<string>();
                if (siteResult != null && siteResult.pagePointOfInterestChecks != null)
                {
                    searchPage.PagePointOfInterests = new List<PagePointOfInterest>();
                    foreach (var check in siteResult.pagePointOfInterestChecks)
                    {
                        switch (check)
                        {
                            case PagePointOfInterestCheck.None: break;
                            case PagePointOfInterestCheck.DuplicatedLinks:
                            case PagePointOfInterestCheck.SubHeaders:
                                searchPage.PagePointOfInterests.Add(new PagePointOfInterest() { PointOfInterestCheck = check });
                                break;
                            default:
                                throw new NotImplementedException($"Check: {check.ToString()} not mapped");
                        }
                    }
                }
                switch (searchPageItem.HtmlNode.Node)
                {
                    case "":
                        break;
                    case "title":
                        if (!string.IsNullOrWhiteSpace(searchPageItem.HtmlNode.Text) && string.IsNullOrWhiteSpace(searchPage.PageTitle))
                        {
                            searchPage.PageTitle = searchPageItem.HtmlNode.Text;
                        }
                        break;
                    case "a":
                    case "img":
                        string altValue = "";
                        if (searchPageItem.HtmlNode.Attributes != null)
                        {
                            foreach (var kv in searchPageItem.HtmlNode.Attributes)
                            {
                                if (kv.Key == "href")
                                {
                                    searchPageItem.HtmlNodeUrl = kv.Value;
                                }
                                else if (kv.Key == "src")
                                {
                                    searchPageItem.HtmlNodeUrl = kv.Value;
                                }
                                else if (kv.Key == "alt")
                                {
                                    altValue = kv.Value;
                                }
                                else if (kv.Key == "title" && !string.IsNullOrWhiteSpace(altValue))
                                {
                                    altValue = kv.Value;
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(searchPageItem.HtmlNodeUrl))
                            {
                                if (searchPageItem.HtmlNodeUrl.StartsWith("/")) { searchPageItem.HtmlNodeUrl = searchPageItem.HtmlNodeUrl[1..]; }
                                searchPageItem.Url = searchPageItem.HtmlNodeUrl;
                            }
                            if (string.IsNullOrWhiteSpace(altValue) && (searchPageItem.HtmlNode.Node == "img" || string.IsNullOrWhiteSpace(searchPageItem.HtmlNode.Text)))
                            {
                                searchPage.HasIssues = true;
                                searchPageItem.Issues.Add($"Node: '{searchPageItem.HtmlNode.Node}' Text: '{searchPageItem.HtmlNode.Text}' at index {searchPageItem.HtmlNode.Index} has no alt/title text. URL: {searchPageItem.HtmlNodeUrl}");
                            }
                            else if(altValue.Length > 60)
                            {
                                searchPage.HasIssues = true;
                                searchPageItem.Issues.Add($"Node: '{searchPageItem.HtmlNode.Node}' Text: '{searchPageItem.HtmlNode.Text}' at index {searchPageItem.HtmlNode.Index} alt/title text is too long. Keep in under 60 characters. URL: {searchPageItem.HtmlNodeUrl}");
                            }
                        }
                        break;
                    case "meta":
                        if (searchPageItem.HtmlNode.Attributes != null)
                        {
                            foreach (var kv in searchPageItem.HtmlNode.Attributes)
                            {
                                if (kv.Key == "name")
                                {
                                    if (searchPageItem.HtmlNode.Attributes.ContainsKey("content"))
                                    {
                                        string content = searchPageItem.HtmlNode.Attributes["content"];
                                        switch (kv.Value)
                                        {
                                            case "description":
                                                if (string.IsNullOrWhiteSpace(searchPage.MetaDescription))
                                                {
                                                    searchPage.MetaDescription = content;
                                                }
                                                break;
                                            case "keywords":
                                                if (string.IsNullOrWhiteSpace(searchPage.MetaKeywords))
                                                {
                                                    searchPage.MetaKeywords = content;
                                                }
                                                break;
                                            case "robots":
                                                if (string.IsNullOrWhiteSpace(searchPage.MetaRobots))
                                                {
                                                    searchPage.MetaRobots = content;
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "h1":
                    case "h2":
                    case "h3":
                    case "h4":
                    case "h5":
                    case "h6":
                        //Check Subheader SEO
                        if (searchPage.PagePointOfInterests != null)
                        {
                            var SubHeaderCheck = searchPage.PagePointOfInterests.FirstOrDefault(x => x.PointOfInterestCheck == PagePointOfInterestCheck.SubHeaders);
                            if (SubHeaderCheck != null)
                            {
                                if (searchPage.PageItems != null)
                                {
                                    foreach (var sele in searchPage.PageItems.Where(x => x.HtmlNode != null && x.HtmlNode.Node != null && new string[] { "h1", "h2", "h3", "h4", "h5", "h6" }.Contains(x.HtmlNode.Node)))
                                    {
                                        if (sele.HtmlNode != null && sele.HtmlNode.Node != null)
                                        {
                                            int c = int.Parse(searchPageItem.HtmlNode.Node[1..]);
                                            int s = int.Parse(sele.HtmlNode.Node[1..]);

                                            if (c < s)
                                            {
                                                searchPage.HasIssues = true;
                                                SubHeaderCheck.IssueFound = true;
                                                //Add Issue
                                                searchPageItem.Issues.Add($"SubHeader violation: '{searchPageItem.HtmlNode.Node}' on '{sele.HtmlNode.Node}' at Index: {searchPageItem.HtmlNode.Index}");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        Console.WriteLine($"Node type not mapped: {searchPageItem.HtmlNode.Node}");
                        break;
                }
            }
        }
        public SearchPageItem ConvertSimpleHtmlElementAndCheck(SiteResult siteResult, ISearchPage searchPage, SimpleHtmlElement simpleHtmlElement)
        {
            SearchPageItem rtnVal = new SearchPageItem();
            if (!searchPage.IsExternalPage && searchPage != null && searchPage.Url != null && simpleHtmlElement != null && !string.IsNullOrWhiteSpace(simpleHtmlElement.Node))
            {
                rtnVal.HtmlNode = simpleHtmlElement;
                rtnVal.ParentUrl = searchPage.Url.AbsoluteUri;
                ItemChecks(siteResult, searchPage, rtnVal);
            }
            return rtnVal;
        }
    }
}
