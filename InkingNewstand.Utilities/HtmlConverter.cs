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
    }
}
