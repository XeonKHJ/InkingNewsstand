using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Syndication;
using InkingNewsstand.Utilities;
using InkingNewsstand.Classes;
using Microsoft.EntityFrameworkCore;

namespace InkingNewsstand.Classes
{
    /// <summary>
    /// 报纸类 ，旧名字为MixedFeeds
    /// </summary>
    public class NewsPaper
    {
        private Model.NewsPaper Model = new Model.NewsPaper { PaperTitle = "创建你的第一份报纸！" };

        /// <summary>
        /// 通过报纸名创建实例
        /// </summary>
        /// <param name="paperTitle"></param>
        public NewsPaper(string paperTitle)
        {
            Model = new Model.NewsPaper { PaperTitle = paperTitle };
        }

        /// <summary>
        /// 通过数据库模型创建实例
        /// </summary>
        /// <param name="newsPaperModel"></param>
        public NewsPaper(Model.NewsPaper newsPaperModel)
        {
            Model = newsPaperModel;
            foreach (var newsPaper_Feed in newsPaperModel.NewsPaper_Feeds)
            {
                foreach (var newsModel in newsPaper_Feed.Feed.News)
                {
                    News.Add(new News(newsModel));
                }
            }
        }

        static public void LoadNewsPapers()
        {
            using(var db = new Model.InkingNewsstandContext())
            {
                //1、加载报纸
                NewsPapers = (from npModel in db.NewsPapers
                              select new NewsPaper(npModel)).ToList();

                //2、加载订阅源
                Feed.Feeds = (from fModel in db.Feeds
                              select new Feed(fModel)).ToList();

                //3、加载新闻
                Classes.News.NewsList = (from n in db.News
                                         select new News(n)).ToList();

                foreach(var news in Classes.News.NewsList)
                {
                    news.Feed = Feed.Feeds.Find(f => f.Id == news.Model.FeedId);
                }

                //3、加载每个订阅源的新闻
                foreach(var feed in Feed.Feeds)
                {
                    feed.News = (from news in Classes.News.NewsList
                                       where news.Feed.Id == feed.Id
                                       select news).OrderByDescending(n => n.PublishedDate).ToList();
                }

                //3、加载关系模组
                var relations = (from r in db.NewsPaper_Feeds
                                 select r).ToList();

                //4、关联报纸和订阅源
                foreach (var newsPaper in NewsPapers)
                {
                    newsPaper.Feeds = (from feed in Feed.Feeds
                                       where relations.Exists(nf => nf.FeedId == feed.Id)
                                       select feed).ToList();
                }
            }
        }

        public int Id
        {
            get { return Model.Id; }
            private set { Model.Id = value; }
        }

        /// <summary>
        /// 报纸名
        /// </summary>
        public string PaperTitle
        {
            get { return Model.PaperTitle; }
            set { Model.PaperTitle = value; }
        }

        /// <summary>
        /// 订阅源数量
        /// </summary>
        public int Count
        {
            get { return Feeds.Count; }
        }

        public string IconType
        {
            get { return Model.IconType; }
            set { Model.IconType = value; }
        }

        /// <summary>
        /// 订阅源列表
        /// </summary>
        public List<Feed> Feeds { set; get; } = new List<Feed>();

        /// <summary>
        /// 添加报纸，并保存到数据库中。
        /// 自动忽略已经存在的报纸。
        /// </summary>
        /// <param name="newsPaper"></param>
        static public async Task AddNewsPaperAsync(NewsPaper newsPaper)
        {
            OnPaperAdding?.Invoke(newsPaper);

            NewsPapers.Add(newsPaper);

            using(var db = new Model.InkingNewsstandContext())
            {
                //先将报纸添加到数据库，为了自动获得一个Id
                var orginalNewsPapers = db.NewsPapers.ToList();
                var originalFeeds = db.Feeds.ToList();
                //若这张报纸不存在的话。
                if (!orginalNewsPapers.Exists(np => np.PaperTitle == newsPaper.PaperTitle))
                {
                    db.NewsPapers.Add(newsPaper.Model);
                    await db.SaveChangesAsync();

                    //添加原本不存在的订阅源。
                    //在这边不指望更新修改过的订阅源。
                    db.Feeds.AddRange(from feed in newsPaper.Feeds
                                                                where !originalFeeds.Exists(f => f.Id == feed.Id)
                                                                select feed.Model);

                    //重新取出该报纸并获取Id
                    newsPaper.Id = (from np in db.NewsPapers
                                    where np.PaperTitle == newsPaper.PaperTitle
                                    select np.Id).First();

                    //将关系模组添加到要添加的报纸
                    newsPaper.Model.NewsPaper_Feeds = (from feed in newsPaper.Feeds
                                                       select new Model.NewsPaper_Feed
                                                       {
                                                           FeedId = feed.Id,
                                                           NewsPaperId = newsPaper.Id
                                                       }).ToList();
                    
                    //更新报纸
                    db.NewsPapers.Update(newsPaper.Model);
                    await db.SaveChangesAsync();
                }
            }
            OnPaperAdded?.Invoke(newsPaper); //报纸添加后引发
        }

        /// <summary>
        /// 往报纸添加订阅源，若已存在该订阅源则自动忽略
        /// </summary>
        /// <param name="feed">要添加的订阅源</param>
        public void AddFeed(Feed feed)
        {
            Feed findResultGloble = null;
            Feed findResultInNewsPaper = null;

            bool isExistInNewsPaper = false;
            if ((findResultGloble = Feed.Feeds.Find(f => f.Id == feed.Id)) == null)
            {
                Feed.Feeds.Add(feed);
                findResultGloble = feed;
                isExistInNewsPaper = false;
            }
            //否则添加到该报纸
            else if ((findResultInNewsPaper = Feeds.Find(f => f.Id == feed.Id)) == null)
            {
                isExistInNewsPaper = false;
            }
            else
            {
                isExistInNewsPaper = true;
            }

            //如果不存在报纸里，则添加报纸到订阅源
            if (!isExistInNewsPaper)
            {
                Feeds.Add(findResultGloble);
            }
        }

        /// <summary>
        /// 往报纸中添加订阅源。
        /// 会自动忽略已经添加过的订阅源。
        /// </summary>
        /// <param name="addedFeeds">要添加的订阅源列表</param>
        /// <returns></returns>
        public void AddFeeds(List<Feed> addedFeeds)
        {
            //筛选出没有在报纸中的所有的订阅源
            var feedIsNotInNewsPaper = (from feed in addedFeeds
                                        where !Feeds.Exists(f => f.Id == feed.Id)
                                        select feed).ToList();

            //筛选出新的订阅源
            var newFeeds = (from feed in feedIsNotInNewsPaper
                            where !Feed.Feeds.Exists(f => f.Id == feed.Id)
                            select feed).ToList();

            //将新的订阅源添加到数据库
            Feed.Feeds.AddRange(newFeeds);

            //将原有订阅源添加到报纸中
            Feeds.AddRange(feedIsNotInNewsPaper);
        }

        /// <summary>
        /// 从报纸中删除订阅源。
        /// 会自动忽略不在报纸中的订阅源。
        /// </summary>
        /// <param name="removedFeeds">要删除的订阅源列表</param>
        public void RemoveFeeds(List<Feed> removedFeeds)
        {
            ////筛选出被删除的订阅源列表
            //var existFeeds = (from feed in removedFeeds
            //                   where Feeds.Exists(f => f.Id == feed.Id)
            //                   select feed).ToList();

            Feeds = Feeds.Except(removedFeeds).ToList();
        }

        /// <summary>
        /// 刷新报纸，获取文章列表
        /// </summary>
        public async Task<List<News>> RefreshNewsAsync()
        {
            //触发OnNewsfreshing事件
            OnNewsRefreshing?.Invoke();

            //为刷新出来的新闻创建列表
            List<News> newNewsitems = new List<News>();

            //记录原始新闻的数量。
            int originalNewsCount = News.Count;

            //对每个订阅源依次进行刷新。
            foreach (var feed in Feeds)
            {
                var syndicationClient = new SyndicationClient();
                try
                {
                    //联网获取订阅源信息
                    var syndicationFeed = await new SyndicationClient().RetrieveFeedAsync(new Uri(feed.Id));
                    syndicationFeed.Id = feed.Id;

                    //更新Feed信息
                    feed.Update(syndicationFeed);

                    for (int retrievedNewsIndex = syndicationFeed.Items.Count - 1; retrievedNewsIndex >= 0; --retrievedNewsIndex)
                    {
                        //获取新闻链接
                        var newsLink = syndicationFeed.Items[retrievedNewsIndex].ItemUri ??
                                       syndicationFeed.Items[retrievedNewsIndex].Links.Select(l => l.Uri).FirstOrDefault();

                        //新建News实例
                        var newsItem = new News(syndicationFeed.Items[retrievedNewsIndex], newsLink, feed);

                        //如果原新闻列表中不包含改新闻，则添加到新闻列表
                        newNewsitems.Add(newsItem);
                    }

                    feed.AddNews(newNewsitems);
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine(exception.Message);
                    System.Diagnostics.Debug.WriteLine(exception.StackTrace);
                    OnUpdateFailed?.Invoke(feed.Id);
                }
            }

            //新闻数量有变动，才引发OnNewsRefreshed事件，
            if (News.Count > originalNewsCount)
            {
                SaveAsync();
                OnNewsRefreshed?.Invoke(News);
            }
            //否则引发NoNewNews（没有新的新闻）事件。
            else
            {
                NoNewNews?.Invoke();
            }

            return News;
        }

        /// <summary>
        /// 对报纸进行操作。
        /// 其中并不对新闻进行保存。
        /// </summary>
        public async void SaveAsync()
        {
            //往报纸中添加关系
            Model.NewsPaper_Feeds = (from feed in Feeds
                                     select new Model.NewsPaper_Feed
                                     {
                                         FeedId = feed.Id,
                                         NewsPaperId = Id
                                     }).ToList();
            
            //往订阅源中添加关系
            foreach(var feed in Feeds)
            {
                feed.Model.NewsPaper_Feeds.Clear();
                if(!feed.Model.NewsPaper_Feeds.Exists(nf => nf.FeedId == feed.Id))
                {
                    feed.Model.NewsPaper_Feeds.Add(Model.NewsPaper_Feeds.Find(nf => nf.FeedId == feed.Id));
                }
            }

            using(var db = new Model.InkingNewsstandContext())
            {
                //将该报纸更新到数据库
                db.NewsPapers.Update(Model);

                db.Feeds.UpdateRange(from feed in Feed.Feeds select feed.Model);
                
                //保存设置。
                await db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 打开的报纸列表
        /// </summary>
        public static List<NewsPaper> NewsPapers { set; get; } = new List<NewsPaper>();

        /// <summary>
        /// 新闻列表，没有排序
        /// </summary>
        //private List<NewsItem> newsList = new List<NewsItem>();
        public List<News> News
        {
            get
            {
                List<News> news = new List<News>();

                foreach (var feed in Feeds)
                {
                    news.AddRange(from n in feed.News select n);
                }

                return news;
            }
        }

        /// <summary>
        /// 删除一张报纸
        /// </summary>
        /// <param name="newsPaper">要删除的报纸</param>
        static public async void DeleteNewsPaper(NewsPaper newsPaper)
        {
            //删除内存中的报纸
            NewsPapers.Remove(newsPaper);

            //删除数据库中的报纸
            await DataOperator.DeleteNewsPaperAsync(newsPaper.Model);

            //触发OnPaperDeleted事件
            OnPaperDeleted?.Invoke(newsPaper);
        }

        public void UpdateNewsList(News newsItem)
        {
            News[this.News.IndexOf(newsItem)] = newsItem;
            OnNewsListUpdated?.Invoke(newsItem);
        }

        public delegate void OnPaperUpdatedDelegate(NewsPaper updatedNewspaper);
        public static event OnPaperUpdatedDelegate OnPaperAdding;
        public static event OnPaperUpdatedDelegate OnPaperAdded; //报纸添加后
        public static event OnPaperUpdatedDelegate OnPaperDeleted;
        public static event OnPaperUpdatedDelegate OnPaperDeleting;

        public delegate void OnPaperFileUpdated();
        //报纸删除后
        public static event OnPaperFileUpdated OnPaperSaved;

        public delegate void OnNewsUpdatedDelegate();

        public event OnNewsUpdatedDelegate OnNewsUpdatedToFile; //新闻有更新时引发
        public event OnNewsUpdatedDelegate OnNewsRefreshing; //报纸刷新前

        public delegate void NoNewNewsHandler();
        public event NoNewNewsHandler NoNewNews;

        public delegate void OnNewsRefreshedDelegate(List<News> newsItem);
        public event OnNewsRefreshedDelegate OnNewsRefreshed; //报纸刷新后

        public delegate void OnUpdateFailedDelegate(string failNewsPaperTitle);
        public event OnUpdateFailedDelegate OnUpdateFailed; //报纸更新失败后

        public delegate void OnNewsListUpdatedEventHandler(News newsItem);
        public event OnNewsListUpdatedEventHandler OnNewsListUpdated;
    }
}
