using SimpleWebCrawler.Core.Parsers.Interfaces;

namespace SimpleWebCrawler.Core.Parsers.Models
{
    public class RobotTextParser : IRobotTextParser
    {
        public List<string>? SiteMapURLs { get;  set; }

        public List<string>? IgnorePaths { get; set; }

        public void Process(string source)
        {
            if (!string.IsNullOrEmpty(source))
            {
                IgnorePaths = new List<string>();
                SiteMapURLs = new List<string>();

                source = source.Replace("\r", "\n");
                while (source.Contains("\n\n"))
                {
                    source = source.Replace("\n\n", "\n");
                }

                string[] lines = source.Split(new char[] { '\n' });
                /*
                sitemap: https://test.com/sitemap.xml
                User-agent: *
                Disallow: /test/
                Disallow: /.env
                Disallow: /cache/ 
                 */
                string temp;
                foreach (string line in lines)
                {
                    temp = line;
                    if (line.Contains(":"))
                    {
                        temp = line.Substring(line.IndexOf(':') + 1).Trim();
                    }
                    if (line.ToLower().StartsWith("sitemap:"))
                    {
                        SiteMapURLs.Add(temp);
                    }
                    else if (line.ToLower().StartsWith("disallow:"))
                    {
                        IgnorePaths.Add(temp);
                    }
                }
            }
        }
    }
}
