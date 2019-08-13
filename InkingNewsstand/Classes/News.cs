using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Web.Syndication;
using InkingNewsstand.Utilities;
using InkingNewsstand.Classes;

namespace InkingNewsstand
{
    [Serializable]
    public class News : IEquatable<News>
    {
        private static SyndicationItem staticItem;
        public News(SyndicationItem item, Uri url, string paperTitle, Feed feed)
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

            Feed = feed;

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

            //设置新闻简要
            Summary = (Content != null) ? (HtmlConverter.GetSummary(Content)) : null;

            //设置新闻作者
            string authorsString = "";
            for (int i = 0; i < staticItem.Authors.Count; ++i)
            {
                authorsString += staticItem.Authors[i].Name;
            }
            Authors = authorsString;
        }
        private Model.News newsModel;
        public News(Model.News newsModel)
        {
            Content = newsModel.Content;
            Feed = new Feed(newsModel.Feed);
            Title = newsModel.Title;
            Summary = newsModel.Summary;
            PublishedDate = newsModel.PublishedDate;
            NewsLink = new Uri(newsModel.NewsLink);
            Authors = newsModel.Authors;
            InnerHTML = newsModel.InnerHTML;
            ExtendedHtml = newsModel.ExtendedHtml;
            InkStrokes = newsModel.InkStrokes;
            IsFavorite = newsModel.IsFavorite;
        }

        public string Content { get; set; }

        public Feed Feed { get; private set; }

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

        public string ExtendedHtml { get; set; }

        public byte[] InkStrokes { get; set; } = new byte[0];

        public bool IsFavorite { get; set; } //收藏属性

        public bool Equals(News other)
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
        public static bool operator ==(News lhs, News rhs)
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
        public static bool operator !=(News lhs, News rhs)
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
            if (obj is News)
            {
                return this.GetHashCode() == ((News)obj).GetHashCode();
            }
            else
            {
                return false;
            }
        }
    }
}
