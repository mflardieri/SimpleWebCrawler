using SimpleWebCrawler.Core.Components.Interfaces;
using SimpleWebCrawler.Core.Components.Models;
using SimpleWebCrawler.Core.Parsers.Interfaces;
using System.Text.RegularExpressions;

namespace SimpleWebCrawler.Core.Parsers.Models
{
    public class SimpleHtmlParser : IPageHtmlParser
    {
        public List<ISimpleHtmlElement> GetPageElements(string source)
        {

            List<ISimpleHtmlElement> rtnVal = new();
            //Element search pattern
            Regex eleRegex = new Regex("<title>(.*?)</title>|<a[^>]*>(.*?)</a>|<meta[^>]*>|<img.+?src=[\"'](.+?)[\"'].*?>|<h1(?: [^>]*)?>(.*?)</h1>|<h2(?: [^>]*)?>(.*?)</h2>|<h3(?: [^>]*)?>(.*?)</h3>|<h4(?: [^>]*)?>(.*?)</h4>|<h5(?: [^>]*)?>(.*?)</h5>|<h6(?: [^>]*)?>(.*?)</h6>");
            //Attribute search pattern
            Regex attrRegex = new Regex("([\\w|data-]+)=[\"']?((?:.(?![\"']?\\s+(?:\\S+)=|\\s*\\/?[>\"']))+.)[\"']?");
            //Node Value/Text pattern
            Regex textRegex = new Regex(">(.*?)<");
            //Element index -> shows order of elments
            int index = 0;
            foreach (Match m in eleRegex.Matches(source))
            {
                //Is Valid Match
                string? mstr;
                if (m.Success)
                {
                    //New Return Element
                    SimpleHtmlElement ele = new SimpleHtmlElement();
                    mstr = m.ToString().TrimStart();
                    //Set Node Name
                    //Node Name
                    ele.Node = mstr.ToLower();
                    if (ele.Node.Contains(">")) { ele.Node = ele.Node.Substring(0, ele.Node.IndexOf(">")); }
                    if (ele.Node.Contains(" ")) { ele.Node = ele.Node.Substring(0, ele.Node.IndexOf(" ")); }
                    if (ele.Node.StartsWith("<")) { ele.Node = ele.Node[1..]; }
                    //Set Node Index
                    ele.Index = index;
                    //Set Text Value
                    Match textMatch = textRegex.Match(mstr);
                    if (textMatch.Success)
                    {
                        ele.Text = textMatch.ToString().Substring(1);
                        ele.Text = ele.Text.Substring(0, ele.Text.Length - 1);
                    }
                    //Set Attributes
                    ele.Attributes = new Dictionary<string, string>();
                    foreach (Match a in attrRegex.Matches(mstr))
                    {
                        //Is Valid Match
                        string astr = a.ToString();
                        if (!string.IsNullOrWhiteSpace(astr))
                        {
                            int pos = astr.IndexOf("=");
                            //Get Atribute Name and Value
                            string attributeName = astr.Substring(0, pos);
                            string attributeValue = astr.Substring(pos + 1);
                            if (attributeValue.StartsWith("\""))
                            {
                                attributeValue = attributeValue[1..];
                            }
                            if (attributeValue.StartsWith("'"))
                            {
                                attributeValue = attributeValue[1..];
                            }
                            //attributeValue = attributeValue.Substring(0, attributeValue.Length);
                            if (attributeValue.EndsWith("\""))
                            {
                                attributeValue = attributeValue.Substring(0, attributeValue.Length - 1);
                            }
                            if (attributeValue.EndsWith("'"))
                            {
                                attributeValue = attributeValue.Substring(0, attributeValue.Length - 1);
                            }
                            //Add Attribute if has value
                            if (!string.IsNullOrWhiteSpace(attributeValue))
                            {
                                if (!ele.Attributes.ContainsKey(attributeName.ToLower()))
                                {
                                    ele.Attributes.Add(attributeName.ToLower(), attributeValue.ToLower());
                                }
                            }
                        }
                    }
                    rtnVal.Add(ele);
                }
                index++;
            }
            return rtnVal;
        }
    }
}
