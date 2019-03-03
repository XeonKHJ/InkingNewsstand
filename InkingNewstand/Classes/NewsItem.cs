using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Web.Syndication;
using InkingNewstand.Utilities;

namespace InkingNewstand
{
    public class NewsItem
    {
        private SyndicationItem item;
        public NewsItem(SyndicationItem item, Uri url, string paperTitle)
        {
            this.item = item;
            NewsLink = url;
            PaperTitle = paperTitle;
            //NewsLinks = item.ItemUri ?? item.Links.Select(l => l.Uri).FirstOrDefault();
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

        public string PublishedDate
        {
            get { return (item.PublishedDate.ToString()); }
        }

        public Uri NewsLink { private set; get; }

        public string PaperTitle { set; get; }

        public string Authors
        {
            get
            {
                string authorsString = "";
                for(int i = 0; i < item.Authors.Count; ++i)
                {
                    authorsString += item.Authors[i].Name;
                }
                return authorsString;
            }
        }
        public string CoverUrl
        {
            get
            {
                return HtmlConverter.GetFirstImages(InnerHTML);
            }
        }

        public string InnerHTML
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
