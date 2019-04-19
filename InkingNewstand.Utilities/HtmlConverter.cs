using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPackForInkingNewstand;
//using SmartReader;
using ReadSharp;

namespace InkingNewstand.Utilities
{
    public class HtmlConverter
    {
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
            catch(Exception exception)
            {
                System.Diagnostics.Debug.WriteLine("fucked:");
            }
            return urlString;
        }

        public async static Task ExtractReadableContent(Uri url)
        {
            Reader reader = new ReadSharp.Reader();
            var readerSharpArticle = await reader.Read(url);
            var Html = readerSharpArticle.Content;

            OnReadingHtmlConvertCompleted?.Invoke(Html);
        }

        public delegate void OnReadingHtmlConvertCompletedDelegate(string html); 
        public static event OnReadingHtmlConvertCompletedDelegate OnReadingHtmlConvertCompleted; //阅读模式转换完成
    }
}
