using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace InkingNewstand
{
    public class MixedFeeds
    {
        private string paperName = "未命名报纸";
        private List<SyndicationFeed> feeds = new List<SyndicationFeed>();
        public MixedFeeds(string paperName)
        {
            this.paperName = paperName;
        }

        public void AddFeed(SyndicationFeed feed)
        {
            feeds.Add(feed);
        }

        public int Count
        {
            get
            {
                return feeds.Count;
            }
        }
        
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

        public string PaperTitle
        {
            get
            {
                return paperName;
            }
        }
    }
}
