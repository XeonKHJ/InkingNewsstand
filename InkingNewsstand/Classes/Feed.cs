using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace InkingNewsstand.Classes
{
    public class Feed
    {
        public Feed(SyndicationFeed feed, Uri uri)
        {
            Title = feed.Title.Text;
            Id = uri.AbsoluteUri;
            Description = feed.Subtitle.Text;
            Icon = feed.IconUri.AbsoluteUri;
        }

        public Feed(Uri uri)
        {
            Id = uri.AbsoluteUri;
        }

        public Feed(Model.Feed feedModel)
        {
            Model = feedModel;
        }

        public void Update(SyndicationFeed feed)
        {
            Title = feed.Title.Text;
            Id = feed.Id;
            Icon = feed.IconUri.AbsoluteUri;
        }

        public Model.Feed Model { get; set; } = new Model.Feed();

        public string Title { get { return Model.Title; } set { Model.Title = value; } }

        /// <summary>
        /// Id即为该订阅源的URL。
        /// </summary>
        public string Id
        {
            get { return Model.Id; }
            set { Model.Id = value; }
        }

        public string Description
        {
            get { return Model.Description; }
            set { Model.Description = value; }
        }

        /// <summary>
        /// 对参数中且该订阅源中没有的新闻进行按出版日期降序排列，然后添加到数据库中。
        /// </summary>
        /// <param name="news">新闻列表</param>
        public async Task AddNewsAsync(List<News> news)
        {
            var newNewsList = (from n1 in news
                              where (!News.Exists(n2 => n2.Title == n1.Title))
                              select n1.Model).OrderByDescending(n=>n.PublishedDate);

            Model.News.AddRange(newNewsList);

            using (var db = new Model.InkingNewsstandContext())
            {
                db.Feeds.Update(Model);
                await db.SaveChangesAsync();
            }
        }

        public List<News> News
        {
            get
            {
                return (from news in Model.News select new News(news)).ToList();
            }
        }

        public string Icon
        {
            get { return Model.Icon; }
            set { Model.Icon = value; }
        }

        public bool Equals(Feed other)
        {
            return Id == other.Id;
        }
    }
}
