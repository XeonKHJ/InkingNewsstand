using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Web.Syndication;
using InkingNewstand.Utilities;
using InkingNewstand.Models;

namespace InkingNewstand
{
    [Serializable]
    public class NewsItem : IEquatable<NewsItem>
    {
        private static SyndicationItem staticItem;
        public NewsItem(SyndicationItem item, Uri url, string paperTitle)
        {
            staticItem = item;

            //设置新闻链接
            NewsLink = url;

            //该新闻报纸的标题
            PaperTitle = paperTitle;

            //设置保存在订阅源中的该新闻的HTML
            if (staticItem.Content != null)
            {
                InnerHTML = staticItem.Content.Text;
            }
            else if (staticItem.NodeValue != null)
            {
                InnerHTML = staticItem.NodeValue;
            }
            else InnerHTML = null;

            //设置新闻标题
            Title = staticItem.Title.Text;

            //设置新闻简要
            Summary = (staticItem.Summary != null) ? (staticItem.Summary.Text) : null;

            //设置出版日期
            PublishedDate = staticItem.PublishedDate;

            //设置Content
            if (staticItem.Content != null)
            {
                Content = staticItem.Content.Text;
            }
            else
            {
                Content = staticItem.Summary.Text;
            }

            //设置新闻作者
            string authorsString = "";
            for (int i = 0; i < staticItem.Authors.Count; ++i)
            {
                authorsString += staticItem.Authors[i].Name;
            }
            Authors = authorsString;
        }

        public string Content { get; set; }



        public SyndicationItem Item
        {
            get { return staticItem; }
        }

        public string Title { get; private set; }

        public string Summary { get; private set; }

        public DateTimeOffset PublishedDate { get; private set; }

        public Uri NewsLink { private set; get; }

        public string PaperTitle { set; get; }

        public string Authors { get; private set; }

        public string CoverUrl
        {
            get
            {
                var coverUrl = HtmlConverter.GetFirstImages(InnerHTML);
                if(coverUrl == "")
                {
                    coverUrl = "NoPic";
                }
                return coverUrl;
            }
        }

        public string InnerHTML { get; private set; }

        public byte[] InkStrokes { get; set; } = new byte[0];

        public bool Equals(NewsItem other)
        {
            return this == other;
        }

        public override string ToString()
        {
            return staticItem.Title.Text.ToString();
        }

        /// <summary>
        /// 重载运算符==
        /// </summary>
        /// <param name="lhs">左操作数</param>
        /// <param name="rhs">右操作数</param>
        /// <returns>是否等价</returns>
        public static bool operator ==(NewsItem lhs, NewsItem rhs)
        {
            if(lhs is null || rhs is null)
            {
                return false;
            }
            if (lhs.Title == rhs.Title && lhs.PublishedDate == rhs.PublishedDate)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 重载运算符!=
        /// </summary>
        /// <param name="lhs">左操作数</param>
        /// <param name="rhs">右操作数</param>
        /// <returns>是否不等价</returns>
        public static bool operator !=(NewsItem lhs, NewsItem rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            var hash = 0;
            foreach(var character in Title)
            {
                hash += character;
            }
            hash += PublishedDate.Year + PublishedDate.Month + PublishedDate.Day + PublishedDate.DateTime.Hour + PublishedDate.Minute + PublishedDate.Second;
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is NewsItem)
            {
                return this.GetHashCode() == ((NewsItem)obj).GetHashCode();
            }
            else
            {
                return false;
            }
        }
    }
}
