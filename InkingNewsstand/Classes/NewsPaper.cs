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

namespace InkingNewsstand
{
    /// <summary>
    /// 报纸类 ，旧名字为MixedFeeds
    /// </summary>
    public class NewsPaper
    {
        static private StorageFile paperListFile;
        private Model.NewsPaper newsPaperModel;
        public NewsPaper(string paperTitle)
        {
            newsPaperModel = new Model.NewsPaper { PaperTitle = paperTitle };
        }

        public NewsPaper(Model.NewsPaper newsPaperModel)
        {
            List<News> newsItems = new List<News>();
            foreach (var newsModel in newsPaperModel.News)
            {
                newsItems.Add(new News(newsModel));
            }
        }

        /// <summary>
        /// 报纸名
        /// </summary>
        public string PaperTitle
        {
            get
            {
                return newsPaperModel.PaperTitle;
            }
            set
            {
                newsPaperModel.PaperTitle = value;
            }
        }

        /// <summary>
        /// 订阅源数量
        /// </summary>
        public int Count
        {
            get
            {
                return Feeds.Count;
            }
        }

        /// <summary>
        /// 订阅源列表
        /// </summary>
        public List<Feed> Feeds { get; set; }

        /// <summary>
        /// 添加报纸
        /// </summary>
        /// <param name="newsPaper"></param>
        static public void AddNewsPaper(NewsPaper newsPaper)
        {
            OnPaperAdding?.Invoke(newsPaper);
            NewsPapers.Add(newsPaper);
            OnPaperAdded?.Invoke(newsPaper); //报纸添加后引发
        }

        /// <summary>
        /// 保存到文件
        /// </summary>
        /// <param name="newsPaper"></param>
        /// <returns>要保存的报纸</returns>
        static public async Task SaveToFile(NewsPaper newsPaper)
        {
            await DataOperator.SaveNewsPapers();
            OnPaperSaved?.Invoke();
        }

        /// <summary>
        /// 保存全包报纸
        /// </summary>
        public static async void SaveAllAsync()
        {
            await DataOperator.SaveNewsPapers();
        }

        /// <summary>
        /// 添加订阅源
        /// </summary>
        /// <param name="feedUrl">订阅源链接</param>
        public void AddFeedLink(Uri feedUrl)
        {
            newsPaperModel.Feeds.Add(new Model.Feed { Id = feedUrl.AbsoluteUri});
        }

        /// <summary>
        /// 刷新报纸，获取文章列表
        /// </summary>
        public async Task<List<News>> GetNewsListAsync()
        {
            OnNewsRefreshing?.Invoke();
            List<News> newNewsitems = new List<News>();
            int originalNewsCount = NewsList.Count;

            foreach (var feed in Feeds)
            {
                var syndicationClient = new SyndicationClient();
                try
                {
                    var syndicationFeed = await new SyndicationClient().RetrieveFeedAsync(new Uri(feed.Id));
                    syndicationFeed.Id = feed.Id;
                    //更新Feed信息
                    feed.Update(syndicationFeed);

                    //将新闻添加到newsItems中
                    for (int retrievedNewsIndex = syndicationFeed.Items.Count - 1; retrievedNewsIndex >= 0; --retrievedNewsIndex)
                    {
                        var newsLink = syndicationFeed.Items[retrievedNewsIndex].ItemUri ?? syndicationFeed.Items[retrievedNewsIndex].Links.Select(l => l.Uri).FirstOrDefault();
                        var newsItem = new News(syndicationFeed.Items[retrievedNewsIndex], newsLink, PaperTitle, feed);

                        //如果原新闻列表中不包含改新闻，则添加到新闻列表
                        if (!NewsList.Contains(newsItem))
                        {
                            newNewsitems.Add(newsItem);
                            NewsList.Add(newsItem);
                        }
                    }
                    var orderedNewsList = NewsList.OrderBy(news => news.PublishedDate);
                    NewsList = orderedNewsList.ToList();
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine(exception.Message);
                    OnUpdateFailed?.Invoke(feed.Id);
                }
            }
            if (NewsList.Count > originalNewsCount)
            {
                OnNewsRefreshed?.Invoke(NewsList);
            }
            else
            {
                NoNewNews?.Invoke();
            }
            if (NewsList.Count != originalNewsCount)
            {
                await SaveToFile(this);
                OnNewsUpdatedToFile?.Invoke();
            }
            return NewsList;
        }

        /// <summary>
        /// 打开的报纸列表
        /// </summary>
        public static List<NewsPaper> NewsPapers { get; private set; } = new List<NewsPaper>();

        /// <summary>
        /// 新闻列表
        /// </summary>
        //private List<NewsItem> newsList = new List<NewsItem>();
        public List<News> NewsList { set; get; }

        /// <summary>
        /// 删除一张报纸
        /// </summary>
        /// <param name="newsPaper">要删除的报纸</param>
        static async public Task DeleteNewsPaper(NewsPaper newsPaper)
        {
            OnPaperDeleted?.Invoke(newsPaper);
        }

        public void UpdateNewsList(News newsItem)
        {
            NewsList[this.NewsList.IndexOf(newsItem)] = newsItem;
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
