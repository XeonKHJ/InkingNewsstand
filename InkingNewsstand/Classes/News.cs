using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Web.Syndication;
using InkingNewsstand.Utilities;
using InkingNewsstand.Classes;

namespace InkingNewsstand.Classes
{
    public class News : IEquatable<News>
    {
        public News(SyndicationItem item, Uri url, Feed feed)
        {
            //设置新闻链接
            NewsLink = url;

            //设置保存在订阅源中的该新闻的HTML
            if (item.Content != null)
            {
                InnerHTML = item.Content.Text;
            }
            else if (item.NodeValue != null)
            {
                InnerHTML = item.NodeValue;
            }
            else InnerHTML = null;

            //设置新闻标题
            Title = item.Title.Text;

            Feed = feed;

            //设置出版日期
            PublishedDate = item.PublishedDate;

            //设置Content
            if (item.Content != null)
            {
                Content = item.Content.Text;
            }
            else
            {
                Content = item.Summary.Text;
            }

            //设置新闻简要
            Summary = (Content != null) ? (HtmlConverter.GetSummary(Content)) : null;

            //设置新闻作者
            string authorsString = "";
            for (int i = 0; i < item.Authors.Count; ++i)
            {
                authorsString += item.Authors[i].Name;
            }
            Authors = authorsString;
        }

        /// <summary>
        /// 数据库模型实例
        /// </summary>
        public Model.News Model { set; get; } = new Model.News();

        public News(Model.News newsModel)
        {
            Model = newsModel;
        }

        public static List<News> NewsList { set; get; } = new List<News>();

        public int Id
        {
            get { return Model.Id; }
        }

        public string Content
        {
            get { return Model.Content; }
            set { Model.Content = value; }
        }

        public Feed Feed
        {
            get { return new Feed(Model.Feed); }
            set { Model.Feed = value.Model; }
        }

        public string Title
        {
            get { return Model.Title; }
            private set { Model.Title = value; }
        }

        public string Summary
        {
            get { return Model.Summary; }
            private set { Model.Summary = value; }
        }

        public DateTimeOffset PublishedDate
        {
            get { return Model.PublishedDate; }
            private set { Model.PublishedDate = value; }
        }

        public Uri NewsLink
        {
            get { return new Uri(Model.NewsLink); }
            private set { Model.NewsLink = value.AbsoluteUri; }
        }

        public string Authors
        {
            get { return Model.Authors; }
            private set { Model.Authors = value; }
        }

        public string CoverUrl
        {
            get
            {
                var coverUrl = HtmlConverter.GetFirstImages(InnerHTML);
                if (coverUrl == "")
                {
                    coverUrl = "NoPic";
                }
                return coverUrl;
            }
        }

        public string InnerHTML { get { return Model.InnerHTML; } private set { Model.InnerHTML = value; } }

        public async Task SaveAsync()
        {
            using(var db = new Model.InkingNewsstandContext())
            {
                db.Update(Model);
                await db.SaveChangesAsync();
            }
        }

        public string ExtendedHtml
        {
            get { return Model.ExtendedHtml; }
            set { Model.ExtendedHtml = value; }
        }

        public byte[] InkStrokes
        {
            get { return Model.InkStrokes; }
            set { Model.InkStrokes = value; }
        }

        /// <summary>
        /// 是否收藏
        /// </summary>
        public bool IsFavorite
        {
            get { return Model.IsFavorite; }
            set { Model.IsFavorite = value; }
        }

        public bool Equals(News other)
        {
            return this == other;
        }

        public override string ToString()
        {
            return Title;
        }

        /*------------必须提供不同于数据库中主键的不同的相等判定方法，避免更新新闻时更新相同的新闻。---------------*/

        /// <summary>
        /// 重载运算符==
        /// </summary>
        /// <param name="lhs">左操作数</param>
        /// <param name="rhs">右操作数</param>
        /// <returns>是否等价</returns>
        public static bool operator ==(News lhs, News rhs)
        {
            if (lhs is null || rhs is null)
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
            foreach (var character in Title)
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
