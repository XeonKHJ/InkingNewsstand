using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Windows.Web.Http;
using Newtonsoft.Json.Linq;
using System.Web;

namespace InkingNewstand.Utilities
{
    public class FeedsFinder
    {
        /// <summary>
        /// 通过链接获取订阅源
        /// </summary>
        /// <param name="websiteUrl"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 通过URL获取HTML页面
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns></returns>
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
        /// 从Feedly中进行搜索获取订阅源
        /// </summary>
        /// <param name="rawQuery">查询字符串</param>
        /// <returns>获取的订阅源URL列表</returns>
        public async static Task<List<Uri>> SearchFromFeedly(string rawQuery)
        {
            string query = HttpUtility.UrlEncode(rawQuery);

            //准备请求的URL
            Uri requestUrl= new Uri("https://cloud.feedly.com/v3/search/feeds?query=" + query);

            //发送HTTP请求
            HttpClient httpClient = new HttpClient();
            var httpResponseMessage = await httpClient.GetAsync(requestUrl);

            //解析返回的Json数据
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
