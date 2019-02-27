using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace InkingNewstand
{
    public class NewsItem
    {
        private SyndicationItem item;
        public NewsItem(SyndicationItem item)
        {
            this.item = item;
        }

        public override string ToString()
        {
            return item.Title.Text.ToString();
        }

        public SyndicationItem Item
        {
            get { return item; }
        }

        public string Title
        {
            get { return item.Title.Text; }
        }

        public string Summary
        {
            get { return (item.Summary != null)?(item.Summary.Text):null; }
        }

        public string CoverUrl
        {
            get
            {
                string reg = @"<img[^>]*src=([""'])?(?<src>[^'""]+)\1[^>]*>";
                var innerImg = Regex.Match(innerHTML, reg, RegexOptions.IgnoreCase);
                if (innerImg.Value == "")
                {
                    return "noPic.png";
                }
                reg = @"src=([""'])?(?<src>[^'""]+)";
                var imgSrc = Regex.Match(innerImg.Value, reg);
                var imgUrl = imgSrc.Value.Substring(5);
                return imgUrl;
            }
        }

        public string innerHTML
        {
            get
            {
                if (item.Content != null)
                {
                    return item.Content.Text;
                }
                else if (item.NodeValue != null)
                {
                    return item.NodeValue;
                }
                else return null;
            }
        }
    }
}
