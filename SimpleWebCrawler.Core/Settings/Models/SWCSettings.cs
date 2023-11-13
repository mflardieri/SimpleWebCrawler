namespace SimpleWebCrawler.Core.Settings.Models
{
    public class SWCSettings
    { 
        public int? PageCheckThreshold { get; set; }
        public int? PageReTryThreshold { get; set; }
        public List<SWCKeyValuePair<string, string>>?  DefaultHeaders { get; set; }
    }
    public class SWCKeyValuePair<T,V>
    { 
        public T? Key { get; set; }
        public V? Value { get; set; }
    }
}
