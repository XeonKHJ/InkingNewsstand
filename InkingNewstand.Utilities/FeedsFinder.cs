using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPackForInkingNewstand;
using FeedlySharp;
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

        public static async Task<List<Uri>> GetFeedsFromKeywords(string keywords)
        {
            string clientId = "863bd138-abce-4674-befc-d72feac1921c";
            string clientToken = "AzsG2thwsiGeW11b4LrAUBoNVuj6gWvToXeCLX8YW7UXHHKZjecLf-nWL3NZ-6YJOf_J859CAd8e1KnnlTL9v6hEULeMXsM-CEaFYOnYSpaP40rduioySmz4LoFa7qn38n4KqLvP7FzjtlwOt5CmWbnXDOa6xRVHjWTT4JqO7v2Ukj8cj9rrkPfefcQeJtzsM9TSqni2gFddpYvYkycpyK3LD4J42MjytqS1RIkVHkYF2AKPSRZhZVVVgco:feedlydev";
            CloudEnvironment cloudEnvironment = new CloudEnvironment();
            FeedlyClient feedlyClient = new FeedlyClient(cloudEnvironment, clientId, clientToken, "no");
            var results = await feedlyClient.FindFeeds(keywords);
            List<Uri> feedUrls = new List<Uri>();
            foreach(var feed in results)
            {
                char[] feedUrlByteArray = new char[feed.Id.Length - 5];
                feed.Id.CopyTo(5, feedUrlByteArray, 0, feed.Id.Length - 5);
                feedUrls.Add(new Uri(new string(feedUrlByteArray)));
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
