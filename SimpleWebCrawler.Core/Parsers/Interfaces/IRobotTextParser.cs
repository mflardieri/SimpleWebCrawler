namespace SimpleWebCrawler.Core.Parsers.Interfaces
{
    public interface IRobotTextParser
    { 
        public List<string>? SiteMapURLs { get; set; }

        public List<string>? IgnorePaths { get; set; }

        public void Process(string source);
    }
}
