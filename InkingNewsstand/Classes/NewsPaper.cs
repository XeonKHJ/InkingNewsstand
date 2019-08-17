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

        public int Id
        {
            get { return Model.Id; }
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
        public List<Feed> Feeds
        {
            get
            {
                var feeds = from nf in Model.NewsPaper_Feeds
                            select new Feed(nf.Feed);
                return feeds.ToList();
            }
        }

        /// <summary>
        /// 添加报纸
        /// </summary>
        /// <param name="newsPaper"></param>
        static public async Task AddNewsPaperAsync(NewsPaper newsPaper)
        {
            OnPaperAdding?.Invoke(newsPaper);

            //添加到数据库中
            using (var db = new Model.InkingNewsstandContext())
            {
                //将该报纸写进数据库
                db.NewsPapers.Add(newsPaper.Model);

                //保存数据库
                await db.SaveChangesAsync();
            }

            OnPaperAdded?.Invoke(newsPaper); //报纸添加后引发
        }

        /// <summary>
        /// 往报纸添加订阅源
        /// </summary>
        /// <param name="feed">要添加的订阅源</param>
        public async void AddFeedAsync(Feed feed)
        {
            using (var db = new Model.InkingNewsstandContext())
            {
                //先查找该订阅源是否已存在数据库
                var feedResult = await db.Feeds.FindAsync(feed);

                //如果没有则添加到数据库
                if (feedResult == null)
                {
                    db.Feeds.Add(feed.Model);
                }

                //更新报纸
                if (feedResult == null  //如果之前FeedResult没有的话，就说明在这张报纸里面肯定没有这个订阅源，因此一定要添加。
                    || !(this.Feeds.Exists(f => f.Id == feed.Id))) //如果之前报纸中不存在这个订阅源的话。
                {
                    var relation = new Model.NewsPaper_Feed { FeedId = feed.Id, NewsPaperId = this.Id };
                    db.Add(relation);
                    Model.NewsPaper_Feeds.Add(relation);
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task AddFeedsAsync(List<Feed> addedFeeds)
        {
            using (var db = new Model.InkingNewsstandContext())
            {
                var feeds = db.Feeds.ToList();
                //筛选出没有在报纸中的所有的订阅源
                var feedModelsNotInNewsPaper = (from feed in addedFeeds
                                          where !Feeds.Exists(f => f.Id == feed.Id)
                                          select feed.Model).ToList();

                //筛选出新的订阅源
                var newFeedModels = (from feedModel in feedModelsNotInNewsPaper
                               where !feeds.Exists(f => f.Id == feedModel.Id)
                               select feedModel).ToList();

                //筛选出已经在数据库存在但在报纸中不存在的订阅源
                var oldFeeds = (from feedModel in feedModelsNotInNewsPaper
                               where Feeds.Exists(f => f.Id == feedModel.Id)
                               select feedModel).ToList();

                //将新的订阅源添加到数据库
                db.Feeds.AddRange(newFeedModels);

                //新建关系实例
                var relation = (from feedModel in feedModelsNotInNewsPaper
                               select (new Model.NewsPaper_Feed
                               {
                                   NewsPaperId = Id,
                                   FeedId = feedModel.Id
                               })).ToList();

                //更新报纸
                db.AddRange(relation);
                //保存数据库
                await db.SaveChangesAsync();
            }
        }

        public async void RemoveFeedsAsync(IEnumerable<Feed> removedFeeds)
        {
            using (var db = new Model.InkingNewsstandContext())
            {
                var removedFeedsList = removedFeeds.ToList();

                //筛选出被删除的关系模型
                var removedList = (from nf in Model.NewsPaper_Feeds
                                   where removedFeedsList.Exists(f => f.Id == nf.FeedId)
                                   select nf).ToList();

                //从该数据库模型中剔除被删除的关系。
                Model.NewsPaper_Feeds.Except(removedList);

                //更新数据库
                db.RemoveRange(removedList);

                //保存到数据库
                await db.SaveChangesAsync();
            }
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

                    await feed.AddNewsAsync(newNewsitems);
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
        /// 打开的报纸列表
        /// </summary>
        public static List<NewsPaper> NewsPapers
        {
            get
            {
                List<NewsPaper> newsPapers;
                using (var db = new Model.InkingNewsstandContext())
                {
                    newsPapers = (from newsPaperModel in db.NewsPapers
                                  select new NewsPaper(newsPaperModel)).ToList();
                }
                return newsPapers;
            }
        }

        /// <summary>
        /// 新闻列表
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
