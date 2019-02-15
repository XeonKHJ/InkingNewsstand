using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace InkingNewstand
{
    /// <summary>
    /// 报纸类 ，旧名字为MixedFeeds
    /// </summary>
    public class NewsPaper
    {
        private List<SyndicationFeed> feeds = new List<SyndicationFeed>();
        public NewsPaper(string paperName)
        {
            this.PaperTitle = paperName;
        }

        /// <summary>
        /// 报纸名
        /// </summary>
        public string PaperTitle { get; } = "未命名报纸";


        //public void AddFeed(SyndicationFeed feed)
        //{
        //    feeds.Add(feed);
        //}

        /// <summary>
        /// 添加订阅源
        /// </summary>
        /// <param name="feedUrl">订阅源链接</param>
        public async Task AddFeedLink(Uri feedUrl)
        {
            var feed = await new SyndicationClient().RetrieveFeedAsync(feedUrl);
            var feedXml = feed.GetXmlDocument(feed.SourceFormat);
            feeds.Add(feed);
        }
        
        /// <summary>
        /// 订阅源数量
        /// </summary>
        public int Count
        {
            get
            {
                return feeds.Count;
            }
        }
        
        /// <summary>
        /// 文章列表
        /// </summary>
        public List<NewsItem> Items
        {
            get
            {
                List<SyndicationItem> mixedItemList = new List<SyndicationItem>();
                foreach (var feed in feeds)
                {
                    mixedItemList.AddRange(feed.Items);
                }
                List<NewsItem> newsItems = new List<NewsItem>();
                foreach(var item in mixedItemList)
                {
                    newsItems.Add(new NewsItem(item));
                }
                return newsItems;
            }
        }

        /// <summary>
        /// 更新文章
        /// </summary>
        public void Refresh()
        {
            ;
        }
    }
}
