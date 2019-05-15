using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPackForInkingNewstand;

namespace InkingNewstand.Utilities
{
    public class FeedsFinder
    {
        public static async Task<List<Uri>> GetFeedsFromUrl(Uri websiteUrl)
        {
            List<Uri> feedUrls = new List<Uri>();
            HtmlDocument htmlDoc = null;
            await Task.Run(() =>
                {
                    htmlDoc = GetHtmlDoc(websiteUrl);
                });
            var selectedNodes = htmlDoc.DocumentNode.SelectNodes("//link");
            if (selectedNodes != null)
            {
                foreach (var node in selectedNodes)
                {
                    try
                    {
                        var feedUrl = node.Attributes["href"].Value;
                        if (node.Attributes["type"].Value == "application/rss+xml")
                        {
                            if (feedUrl[0] == '/')
                            {
                                string stringUrl = websiteUrl.AbsoluteUri.Remove(websiteUrl.AbsoluteUri.Length - 1);
                                feedUrl = stringUrl + feedUrl;
                            }
                            feedUrls.Add(new Uri(feedUrl));
                        }
                    }
                    catch (NullReferenceException)
                    {
                        continue;
                    }
                }
            }
            return feedUrls;
        }

        private static HtmlDocument GetHtmlDoc(Uri url)
        {
            var htmlWeb = new HtmlWeb();
            HtmlDocument htmlDoc = new HtmlDocument();
            try
            {
                htmlDoc = htmlWeb.Load(url.AbsoluteUri);
            }
            catch (Exception exception)
            {
                //to-do
            }
            return htmlDoc;
        }

        private static HtmlDocument GetHtmlDoc(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            return htmlDoc;
        }
    }
}
