using SimpleWebCrawler.Core.Components.Models;
using SimpleWebCrawler.Core.Results.Models;
using System.Xml;

namespace SimpleWebCrawler.Core.Parsers.Models
{
    public abstract class SiteMapXmlParserBase
    {
        public void GetSiteMapItemsFromXml(SiteResult siteResult, SiteMap siteMap, string sourceXml, bool addToCrawl = true)
        {
            if (!string.IsNullOrWhiteSpace(sourceXml) && siteResult != null && siteResult.PageResults != null && siteResult.SiteMaps != null && siteMap != null && siteMap.Items != null)
            {
                try
                {
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.LoadXml(sourceXml);

                    if (xdoc.DocumentElement != null)
                    {
                        if (xdoc.DocumentElement.Name == "sitemapindex" || xdoc.DocumentElement.Name == "urlset")
                        {
                            foreach (XmlNode node in xdoc.DocumentElement.ChildNodes)
                            {
                                if (node.Name == "sitemap" || node.Name == "url")
                                {
                                    SiteMapItem item = new SiteMapItem();

                                    foreach (XmlNode cnode in node.ChildNodes)
                                    {
                                        if (cnode.Name == "loc")
                                        {
                                            item.Loc = cnode.InnerText;
                                        }
                                        else if (cnode.Name == "lastmod")
                                        {
                                            DateTime dt;
                                            if (DateTime.TryParse(cnode.InnerText, out dt))
                                            {
                                                item.Lastmod = dt;
                                            }
                                        }
                                    }
                                    if (!string.IsNullOrWhiteSpace(item.Loc))
                                    {
                                        siteMap.Items.Add(item);
                                        if (node.Name == "sitemap" && item.Loc.ToLower().EndsWith(".xml"))
                                        {
                                            siteResult.SiteMaps.Add(new SiteMap() { URL = item.Loc });
                                        }
                                        if (node.Name == "url" && addToCrawl)
                                        {
                                            SearchPage? sp = siteResult.BuildSearchPageEntry(item.Loc, true);
                                            if (sp != null && sp.Url != null)
                                            {
                                                if (siteResult.PageResults.FirstOrDefault(x => x.Url != null && x.Url.AbsoluteUri == sp.Url.AbsoluteUri) == null)
                                                {
                                                    siteResult.PageResults.Add(sp);
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("What the heck!!");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    siteMap.Status = "Processed";
                }
                catch (Exception ex)
                {
                    siteMap.Status = ex.Message;
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
