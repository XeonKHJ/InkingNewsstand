using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using SmartReader;
using ReadSharp;

namespace InkingNewstand.Utilities
{
    public class HtmlConverter
    {
        /// <summary>
        /// 获取给定HTML中的所有图片
        /// </summary>
        /// <param name="html">要从中获取图片的HTML</param>
        /// <returns>图片URL列表</returns>
        public static List<Uri> GetImages(string html)
        {
            List<Uri> imgUrls = new List<Uri>();
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            var imgNodes = htmlDocument.DocumentNode.SelectNodes("//img");
            foreach(var imgNode in imgNodes)
            {
                imgUrls.Add(new Uri(imgNode.Attributes["src"].Value));
            }
            return imgUrls;
        }

        /// <summary>
        /// 获取给定HTML中的第一张图片
        /// </summary>
        /// <param name="html">要获取图片的HTML</param>
        /// <returns></returns>
        public static string GetFirstImages(string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            string urlString = "";
            try
            {
                var imgNode = htmlDocument.DocumentNode.SelectSingleNode("//img");
                urlString = imgNode is null ? "" : imgNode.Attributes["src"].Value;
            }
            catch(Exception)
            {
                System.Diagnostics.Debug.WriteLine("fucked:");
            }
            return urlString;
        }

        public static string GetSummary(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            string innerText = htmlDoc.DocumentNode.InnerText;
            var firstParagraphEndIndex = innerText.IndexOf('\n');
            if(firstParagraphEndIndex > 0)
            {
                return innerText.Substring(0, firstParagraphEndIndex);
            }
            else
            {
                return innerText;
            }
        }

        private const int THRESHOLD = 100;
        private static string FindSummary(HtmlNode node)
        {
            foreach (HtmlNode childNode in node.ChildNodes)
            {
                if (childNode is HtmlTextNode)
                {
                        return childNode.InnerText;
                }
                String summary = FindSummary(childNode);
                if (summary.Length >= THRESHOLD)
                {
                    return summary;
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// 提取可阅读的内容
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async static Task ExtractReadableContent(Uri url)
        {
            //Reader reader = new ReadSharp.Reader();
            //var readerSharpArticle = await reader.Read(url);
            //var Html = readerSharpArticle.Content;

            var article = await SmartReader.Reader.ParseArticleAsync(url.AbsoluteUri);
            var Html = article.Content;

            OnReadingHtmlConvertCompleted?.Invoke(Html);
        }

        public delegate void OnReadingHtmlConvertCompletedDelegate(string html); 
        public static event OnReadingHtmlConvertCompletedDelegate OnReadingHtmlConvertCompleted; //阅读模式转换完成
    }
}
