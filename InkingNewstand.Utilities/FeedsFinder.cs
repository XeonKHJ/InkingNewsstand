using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace InkingNewstand.Utilities
{
    class FeedsFinder
    {
        public static List<Uri> GetFeedFromUrl(Uri websiteUrl)
        {
            List<Uri> feedUrls = new List<Uri>();
            var htmlDoc = GetHtmlDoc(websiteUrl);
            var selectedNodes = htmlDoc.DocumentNode.SelectNodes("//link");
            foreach(var node in selectedNodes)
            {
                try
                {
                    var feedUrl = node.Attributes["href"].Value;
                    if(node.Attributes["type"].Value == "application/rss+xml")
                    {
                        if(feedUrl[0] == '/')
                        {
                            string stringUrl = websiteUrl.AbsoluteUri.Remove(websiteUrl.AbsoluteUri.Length - 1);
                            feedUrl = stringUrl + feedUrl;
                        }
                        feedUrls.Add(new Uri(feedUrl));
                    }
                }
                catch(NullReferenceException)
                {
                    continue;
                }
            }
            return feedUrls;
        }

        private HtmlDocument GetHtmlDoc(Uri url)
        {
            var htmlWeb = new HtmlWeb();
            var htmlDoc = htmlWeb.Load(uri.AbsoluteUri);
            return htmlDoc;
        }

        private HtmlDocument GetHtmlDoc(string html)
        {
            var htmlDoc = new HtmlDocument();
            HtmlDocument.LoadHtml(html);
            return HtmlDocument;
        }
    }
}
