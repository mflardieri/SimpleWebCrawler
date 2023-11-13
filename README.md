# SimpleWebCrawler
## Basic info

This project is a simple implementation of a Web crawler with some basic SEO checks written in C# (.NET7 / VS 2022).
You can either crawl a single page or website.

This Crawler is made up of 4 main Interfaces.
1. ISiteProcessor:

    Main Processor for run setup and conversion of results.

1. IPageParser:

    Retrieves Page Responses and prepares page for processing.

1. IPageHtmlParser:

    Parses configured html elements for processing.

1. IPagePointOfInterestStrategy:

    Applies Point of Interest compares for finding page issues.

## Configuration
PageCheckThreshold: The threshold of how many pages will be checked at one time.

PageReTryThreshold: The threshold of how many times a page will retry until it is skipped.

DefaultHeaders: Applies default headers to the underline _httpclient_.

## Setup
You can setup the processing for either background worker or inline. By default the console app is setup as inline.

### Result Setup
	
First you need to detemine what domain or website you would like the result for.
_www.google.com_ is the domain we want. (**Note: if you crawl google's site it will hurt :(**)
.
```csharp
var siteResult = siteProcessor.CreateSiteResult("https://www.google.com");
```
Once you have selected your domain or website for the result you will either need to setup a page run or a website run.
### Page Run Setup
The below example is for single page run result.
```csharp
siteProcessor.ProcessPageAsync(siteResult, "https://www.google.com").Wait();
```
In the example, the page url is set to google's home page. This doesn't need to be to case just any page in the result domain.
When a page run is done you will get results back in the _siteResult_ object

***Single Page run will still search for the site maps.***

### Site Run Setup
>[!IMPORTANT]
>If you choose to run a website, this process will take sometime depending on how many pages the crawler finds.
>The crawler will do mutiple runs on the collecting/finding of pages.

Below example is for web site run result.

```csharp
siteProcessor.ProcessSiteAsync(siteResult).Wait();//Long run process
```
This will run based on which domain/website you have chosen.
When the site run is done you will get results back in the _siteResult_ object


### Results
At this point you could save the results as a json file and review it when you want.

>Result Output:
```
{
  "SiteURL": "https://www.google.com",
  "SiteHost": "www.google.com",
  "SiteMaps": [
    {
      "URL": "https://www.google.com/sitemap.xml",
      "Items": [
        {
          "Loc": "https://www.google.com/gmail/sitemap.xml",
          "Lastmod": null
        },
        {
          "Loc": "https://www.google.com/forms/sitemaps.xml",
          "Lastmod": null
        },
        {
          "Loc": "https://www.google.com/slides/sitemaps.xml",
          "Lastmod": null
        },
...
        {
          "Loc": "https://www.google.com/intl/es_us/chromebook/discover/pdp-samsung-galaxy-chromebook-go/sku-samsung-galaxy-chromebook-go-4gb-32gb/",
          "Lastmod": null
        }
      ],
      "Status": "Processed",
      "Trys": 1
    }
  ],
  "pagePointOfInterestChecks": null,
  "RobotIgnorePaths": [
    "/search",
    "/sdch",
    "/groups",
    "/index.html?", 
...
    "/m/",
    "/groups",
    "/hosted/images/",
    "/m/"
  ],
  "PageResults": [
    {
      "HasIssues": true,
      "WasHttp": false,
      "PageTrys": 1,
      "StatusCode": 200,
      "LoadSpeed": 5,
      "LoadTime": "00:00:00.1448360",
      "PagePointOfInterests": null,
      "Url": "https://www.google.com/",
      "IsExternalPage": false,
      "Issues": [
        "Has no meta description for page.",
        "Has no meta keywords for page.",
        "Has no meta robots for page.",
        "Page Title is too short. Make it between 50 to 60 charaters.",
        "This page is on the site map with issues"
      ],
      "CrawlDateTime": "2023-11-09T14:22:47.6953397-08:00",
      "Status": "Processed",
      "WasOnSiteMap": true,
      "PageItems": [
        {
          "ParentUrl": "https://www.google.com/",
          "HtmlNode": {
            "Index": 0,
            "Node": "meta",
            "Text": null,
            "Attributes": {
              "charset": "UTF-8"
            }
          },
          "HtmlNodeUrl": null,
          "Url": null,
          "Issues": []
        },
        {
          "ParentUrl": "https://www.google.com/",
          "HtmlNode": {
            "Index": 1,
            "Node": "meta",
            "Text": null,
            "Attributes": {
              "content": "origin",
              "name": "referrer"
            }
          },
          "HtmlNodeUrl": null,
          "Url": null,
          "Issues": []
        },
        {
          "ParentUrl": "https://www.google.com/",
          "HtmlNode": {
            "Index": 2,
            "Node": "meta",
            "Text": null,
            "Attributes": {
              "content": "/images/branding/googleg/1x/googleg_standard_color_128dp.png",
              "itemprop": "image"
            }
          },
          "HtmlNodeUrl": null,
          "Url": null,
          "Issues": []
        },
...        
        {
          "ParentUrl": "https://www.google.com/",
          "HtmlNode": {
            "Index": 23,
            "Node": "a",
            "Text": "Search help",
            "Attributes": {
              "href": "https://support.google.com/websearch/?p=ws_results_help\u0026amp;hl=en\u0026amp;fg=1",
              "role": "menuitem",
              "tabindex": "-1"
            }
          },
          "HtmlNodeUrl": "https://support.google.com/websearch/?p=ws_results_help\u0026amp;hl=en\u0026amp;fg=1",
          "Url": "https://support.google.com/websearch/?p=ws_results_help\u0026amp;hl=en\u0026amp;fg=1",
          "Issues": []
        }
      ],
      "PageTitle": "Google",
      "MetaKeywords": null,
      "MetaDescription": null,
      "MetaRobots": null,
      "FirstParentUrl": null
    }
  ]
}
```

But if you want to just see the SEO issues then you should do second stage convert to page issues.

>Page Issues Result Output:
```
[
  {
    "Url": "https://google.com/",
    "Issues": [
      "Meta description is too short. Make it between 150 to 160 charaters.",
      "Has no meta keywords for page.",
      "Page Title is too short. Make it between 50 to 60 charaters.",
      "Node: \u0027a\u0027 Text: \u0027\u0027 at index 16 has no alt/title text. URL: https://www.google.com/webhp?tab=ww",
      "Node: \u0027a\u0027 Text: \u0027\u0027 at index 17 has no alt/title text. URL: https://www.google.com/imghp?hl=en\u0026tab=wi",
      "Node: \u0027a\u0027 Text: \u0027\u0027 at index 18 has no alt/title text. URL: https://maps.google.com/maps?hl=en\u0026tab=wl",
      "Node: \u0027a\u0027 Text: \u0027\u0027 at index 19 has no alt/title text. URL: https://play.google.com/?hl=en\u0026tab=w8",
      "Node: \u0027a\u0027 Text: \u0027\u0027 at index 20 has no alt/title text. URL: https://www.youtube.com/?tab=w1",
      "Node: \u0027a\u0027 Text: \u0027\u0027 at index 21 has no alt/title text. URL: https://news.google.com/?tab=wn",
      "Node: \u0027a\u0027 Text: \u0027\u0027 at index 22 has no alt/title text. URL: https://mail.google.com/mail/?tab=wm",
      "Node: \u0027a\u0027 Text: \u0027\u0027 at index 23 has no alt/title text. URL: https://drive.google.com/?tab=wo",
      "Node: \u0027a\u0027 Text: \u0027\u0027 at index 24 has no alt/title text. URL: https://www.google.com/intl/en/about/products?tab=wh",
      "Node: \u0027a\u0027 Text: \u0027\u0027 at index 36 has no alt/title text. URL: https://accounts.google.com/servicelogin?hl=en\u0026passive=true\u0026continue=https://www.google.com/\u0026ec=gazaaq",
      "Node: \u0027a\u0027 Text: \u0027\u0027 at index 37 has no alt/title text. URL: http://www.google.com/preferences?hl=en"
    ]
  }
]
```

If you just save the all results you can always reapply the issues.
This is because you may want to change which _IPageHtmlParser_ and/or _IPagePointOfInterestStrategy_ you are using or a part of the logic within them.

***If you change the _IPageHtmlParser_ you more than likely will have to rerun the page/website run.***

Example below:
```csharp
SiteResult siteResults = JsonHelpers.FromJsonFile<SiteResult>("output.json");
if (siteResults != null)
{
    siteProcessor.ReApplyIssues(siteResults);
    var issues = siteProcessor.ConvertToPageIssues(siteResults);
    if (issues != null && issues.Count > 0)
    {
        issues.ToJsonFile("output_issues.json");
    }
}
```

## The Rub

### SEO Basics

There are many SEO tools out there to use and some are free.
The Main reason the simple web crawler does SEO checks is to ensure pages are following basic SEO practices.

### Website Audit

You might be planning on updating or moving your website. 
This is a great use of this tool or tools like it. 
The results will let you know what is there so you can make the best decision for the project at hand
or simply you might want to check load times of your pages, etc.

### Cross Checks
Certain Tools will do different checks and you might have to pay for those tools.
It is a good idea to check your website periodically; one for the end users and two for your management of projects to come.
Eitherway whatever tool you choose do more than one. More information available to you is never a bad thing.