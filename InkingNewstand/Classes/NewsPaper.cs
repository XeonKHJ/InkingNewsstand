using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
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

        /// <summary>
        /// 从文件中加载
        /// </summary>
        public async Task<StorageFile> SaveToFile(NewsPaper newsPaper)
        {
            var storageFolder = ApplicationData.Current.LocalFolder;

            //1、读取报纸列表数据，获取当前报纸编号，若没有则新建
            string paperListFileName = "PaperList.dat";
            var paperListFile = await storageFolder.CreateFileAsync(paperListFileName, CreationCollisionOption.OpenIfExists);
            

            //打开/创建保存该报纸的文件
            string fileName = "sample.dat"; //修改成报纸名字
            var saveFile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.FailIfExists);

            //读取文件
            var buffer = new Windows.Storage.Streams.Buffer((uint)System.Runtime.InteropServices.Marshal.SizeOf(this));

            await FileIO.WriteBufferAsync(saveFile, buffer);

            return saveFile;
        }

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
        /// 加载之前保存的报纸列表
        /// </summary>
        /// <returns>报纸列表</returns>
        public static async Task<List<NewsPaper>> GetNewsPapers()
        {
            var file = await ApplicationData.Current.LocalFolder.TryGetItemAsync("PaperList.dat") as StorageFile;
            if (file == null)
            {
                return new List<NewsPaper>();
            }
            return new List<NewsPaper>();
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
