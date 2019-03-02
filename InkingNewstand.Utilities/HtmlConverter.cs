using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

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
            string urlString;
            try
            {
                urlString = htmlDocument.DocumentNode.SelectSingleNode("//img").Attributes["src"].Value;
            }
            catch(NullReferenceException exception)
            {
                urlString = "";
            }
            return urlString;
        }

        public async static void GetReadingHtml(Uri url)
        {
            Reader reader = new Reader();
            Article article;

            try
            {
                article = await reader.Read(url);
                Html = article.Content;
                OnReadingHtmlConvertCompleted?.Invoke();
            }
            catch (ReadException exc)
            {
                // handle exception
            }
        }

        public delegate void OnReadingHtmlConvertCompletedDelegate(); 
        public static event OnReadingHtmlConvertCompletedDelegate OnReadingHtmlConvertCompleted; //阅读模式转换完成
    }
}
