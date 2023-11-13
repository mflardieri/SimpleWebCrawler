using SimpleWebCrawler.Core.Parsers.Interfaces;
using SimpleWebCrawler.Core.Results.Models;
using System.Diagnostics;
using System.Net;

namespace SimpleWebCrawler.Core.Parsers.Bases
{
    public abstract class PageParserBase : IPageParser
    {
        public async Task<PageHttpResult> GetResultAsStringAsync(HttpClient httpClient, string uri, bool ensureSuccessStatusCode = true)
        {
            PageHttpResult rtnVal = new();
            HttpResponseMessage? respMsg = null;
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                respMsg = await httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
                rtnVal.StatusCode = respMsg.StatusCode;
                if (rtnVal.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    int headerReTry = 2000;
                    if (respMsg.Headers.RetryAfter != null && respMsg.Headers.RetryAfter.Delta.HasValue)
                    {
                        headerReTry = respMsg.Headers.RetryAfter.Delta.Value.Milliseconds;
                    }
                    if (headerReTry < 2000) { headerReTry = 2000; }
                    if (headerReTry > 5000) { headerReTry = 5000; }
                    Thread.Sleep(headerReTry);
                    sw = Stopwatch.StartNew();
                    respMsg = await httpClient.GetAsync(uri);
                    rtnVal.StatusCode = respMsg.StatusCode;
                }
                if (ensureSuccessStatusCode) { respMsg.EnsureSuccessStatusCode(); }
                rtnVal.Response = await respMsg.Content.ReadAsStringAsync();
                rtnVal.IsSuccess = true;
                sw.Stop();
                rtnVal.TimeElapsed = sw.Elapsed;
            }
            catch (Exception ex)
            {
                rtnVal.Error = ex.Message;
            }
            finally
            {
                if (respMsg != null)
                {
                    try
                    {
                        respMsg.Dispose();
                    }
                    catch { }
                }
            }
            return rtnVal;
        }
    }
}
