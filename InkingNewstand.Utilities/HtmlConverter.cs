using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InkingNewstand.Utilities
{
    public class HtmlConverter
    {
        private readonly string htmlString;
        public static List<Uri> GetImages(string html)
        {
            List<Uri> imgUrls = new List<Uri>();
            string reg = @"<img[^>]*src=([""'])?(?<src>[^'""]+)\1[^>]*>";
            var innerImgs = Regex.Matches(html, reg, RegexOptions.IgnoreCase);
            reg = @"src=([""'])?(?<src>[^'""]+)";
            foreach (var innerImg in innerImgs)
            {
                var innerImgMatch = (Match)innerImg;
                var imgSrc = Regex.Match(innerImgMatch.Value, reg);
                var imgUrl = imgSrc.Value.Substring(5);
                imgUrls.Add(new Uri(imgUrl));
            }
            return imgUrls;
        }
    }
}
