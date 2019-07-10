using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPackForInkingNewstand;
using Windows.Web.Http;
using Newtonsoft.Json.Linq;
using System.Web;

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
            catch (Exception)
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

        /// <summary>
        /// 从Feedly中进行搜索
        /// </summary>
        /// <param name="rawQuery"></param>
        /// <returns></returns>
        public async static Task<List<Uri>> SearchFromFeedly(string rawQuery)
        {
            string query = HttpUtility.UrlEncode(rawQuery);
            Uri requestUrl= new Uri("https://cloud.feedly.com/v3/search/feeds?query=" + query);
            HttpClient httpClient = new HttpClient();
            var httpResponseMessage = await httpClient.GetAsync(requestUrl);
            List<Uri> uris = new List<Uri>();
            JObject jObject = JObject.Parse(httpResponseMessage.Content.ToString());
            var results = jObject.GetValue("results");
            foreach (var searchResult in results.Children())
            {
                string id = searchResult.Value<string>("id");
                char[] feedUrlByteArray = new char[id.Length - 5];
                id.CopyTo(5, feedUrlByteArray, 0, id.Length - 5);
                uris.Add(new Uri(new string(feedUrlByteArray)));
            }
            return uris;
        }
    }
}
