using Microsoft.Extensions.Configuration;

namespace SimpleWebCrawler.Core.Helpers
{
    public static class ConfigHelpers
    {
        public static T?  GetConfigObject<T>(this IConfigurationRoot x, string sectionPath) where T : class
        {
            if (x != null)
            {
                var sectionObject = x.GetSection(sectionPath);
                if(sectionObject != null)
                {
                    return sectionObject.Get<T>();
                }
            }
            return default;
        }
    }
}
