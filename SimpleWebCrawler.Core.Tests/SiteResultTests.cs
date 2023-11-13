using SimpleWebCrawler.Core.Results.Models;

namespace SimpleWebCrawler.Core.Tests
{
    [TestClass]
    public class SiteResultTests
    {

        SiteResult? siteResult;

        [TestInitialize]
        public void setUpTest()
        {
            siteResult = new SiteResult();
            siteResult.SiteURL = "https://acmeinc.com";
            siteResult.SiteHost = "acmeinc.com";
            siteResult.RobotIgnorePaths = new List<string>() { "/ignore/" };
        }

        [TestMethod]
        public void NormalizeUrlTests()
        {
            Assert.IsTrue(siteResult != null);
            Assert.AreEqual("test", siteResult.NormalizeToUri("/test"));
            Assert.AreEqual("test", siteResult.NormalizeToUri("https://acmeinc.com/test"));
            Assert.AreEqual("test", siteResult.NormalizeToUri("http://acmeinc.com/test"));
            Assert.AreEqual("https://acme.com/test", siteResult.NormalizeToUri("https://acme.com/test"));
            Assert.AreEqual("http://acme.com/test", siteResult.NormalizeToUri("http://acme.com/test"));

            Assert.AreEqual("test", siteResult.NormalizeToUri("/test/"));
            Assert.AreEqual("test", siteResult.NormalizeToUri("https://acmeinc.com/test/"));
            Assert.AreEqual("test", siteResult.NormalizeToUri("http://acmeinc.com/test/"));
            Assert.AreEqual("https://acme.com/test", siteResult.NormalizeToUri("https://acme.com/test/"));
            Assert.AreEqual("http://acme.com/test", siteResult.NormalizeToUri("http://acme.com/test/"));
        }
        [TestMethod]
        public void TestIgnoreUrl()
        {
            //Ignores are only for internal pages not externals
            Assert.IsTrue(siteResult != null);
            Assert.IsTrue(siteResult.IgnoreUrl("http://acmeinc.com/ignore"));
            Assert.IsTrue(siteResult.IgnoreUrl("http://acmeinc.com/ignore/"));
            Assert.IsTrue(siteResult.IgnoreUrl("http://acmeinc.com/ignore/test"));


            Assert.IsFalse(siteResult.IgnoreUrl("http://acmeinc.com/ignoreoff"));
            Assert.IsFalse(siteResult.IgnoreUrl("http://acmeinc.com/ignoreoff/"));
            Assert.IsFalse(siteResult.IgnoreUrl("http://acmeinc.com/ignoreoff/test"));


            Assert.IsFalse(siteResult.IgnoreUrl("http://acme.com/ignore"));
            Assert.IsFalse(siteResult.IgnoreUrl("http://acme.com/ignore/"));
            Assert.IsFalse(siteResult.IgnoreUrl("http://acme.com/ignore/test"));
        }

        [TestMethod]
        public void TestIsExternalPage()
        {
            Assert.IsTrue(siteResult != null);
            Assert.IsTrue(siteResult.IsExternalPage("http://acme.com/"));
            Assert.IsTrue(siteResult.IsExternalPage("https://acme.com/"));

            Assert.IsFalse(siteResult.IsExternalPage("http://acmeinc.com/"));
            Assert.IsFalse(siteResult.IsExternalPage("https://acmeinc.com/"));
            Assert.IsFalse(siteResult.IsExternalPage("/"));
            Assert.IsFalse(siteResult.IsExternalPage("test"));
            Assert.IsFalse(siteResult.IsExternalPage("/test"));
            Assert.IsFalse(siteResult.IsExternalPage("/test/"));
        }
        [TestMethod]
        public void TestNormalizeToFinializedURL()
        {
            Assert.IsTrue(siteResult != null);
            Assert.AreEqual("https://acmeinc.com/test", siteResult.NormalizeToFinializedURL("/test"));
            Assert.AreEqual("https://acmeinc.com/test", siteResult.NormalizeToFinializedURL("https://acmeinc.com/test"));
            Assert.AreEqual("https://acmeinc.com/test", siteResult.NormalizeToFinializedURL("http://acmeinc.com/test"));
            Assert.AreEqual("https://acme.com/test", siteResult.NormalizeToFinializedURL("https://acme.com/test"));
            Assert.AreEqual("http://acme.com/test", siteResult.NormalizeToFinializedURL("http://acme.com/test"));

            Assert.AreEqual("https://acmeinc.com/test", siteResult.NormalizeToFinializedURL("/test/"));
            Assert.AreEqual("https://acmeinc.com/test", siteResult.NormalizeToFinializedURL("https://acmeinc.com/test/"));
            Assert.AreEqual("https://acmeinc.com/test", siteResult.NormalizeToFinializedURL("http://acmeinc.com/test/"));
            Assert.AreEqual("https://acme.com/test", siteResult.NormalizeToFinializedURL("https://acme.com/test/"));
            Assert.AreEqual("http://acme.com/test", siteResult.NormalizeToFinializedURL("http://acme.com/test/"));
        }
        [TestMethod]
        public void TestBuildSearchPageEntry()
        {
            Assert.IsTrue(siteResult != null);
            string url = "/test/";

            var sp = siteResult.BuildSearchPageEntry(url, true);
            Assert.IsNotNull(sp);
            Assert.IsNotNull(sp.Url);
            Assert.AreEqual("https://acmeinc.com/test", sp.Url.AbsoluteUri);
        }
    }
}
