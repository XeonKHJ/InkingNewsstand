using InkingNewstand.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Syndication;

namespace InkingNewstand
{
    [Serializable]
    /// <summary>
    /// 报纸类 ，旧名字为MixedFeeds
    /// </summary>
    public class NewsPaper
    {
        static private StorageFile paperListFile;
        //private List<Uri> feedUrls = new List<Uri>();
        private NewsPaperModel newsPaperModel;
        public NewsPaper(string paperName)
        {
            newsPaperModel = new NewsPaperModel{ PaperTitle = paperName};
        }

        public NewsPaper(NewsPaperModel model)
        {
            newsPaperModel = model;
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
        }

        /// <summary>
        /// 订阅源数量
        /// </summary>
        public int Count
        {
            get
            {
                return newsPaperModel.Count;
            }
        }

        public List<Uri> FeedUrls
        {
            get
            {
                return newsPaperModel.FeedUrls;
            }
        }

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
            //1、获取报纸列表
            var paperListinFile = await ReadListFromFile();
            bool existFlag;
            //2、获取当前报纸编号
            ////2.1、如果文件中没有保存任何东西，则新建一个paperListinFile
            if (paperListinFile == null)
            {
                paperListinFile = new List<NewsPaperModel>();
            }

            var index = paperListinFile.IndexOf(newsPaper.newsPaperModel);

            ////2.2、如果文件中没有保存此添加（是新报纸）
            if (index == -1)
            {
                existFlag = false;
                paperListinFile.Add(newsPaper.newsPaperModel);
            }
            ////2.3、如果有则修改
            else
            {
                existFlag = true;
                //找到该报纸位置并替换掉 //to-do
                paperListinFile[index] = newsPaper.newsPaperModel; //to-replace
            }
            ////2.4若查到有多张报纸有相同名字，则显示错误

            //3、将paperListinFile重新保存到文件中

                await FileIO.WriteBytesAsync(paperListFile, ObjectToByteArray(paperListinFile));
            if (!existFlag)
                {
                    OnPaperSaved?.Invoke();
                }//添加完成后引发该事件
        }

        /// <summary>
        /// 从文件读取报纸列表
        /// </summary>
        /// <returns></returns>
        static public async Task<List<NewsPaper>> ReadFromFile()
        {
            //1、从文件中获取报纸列表。
            var paperListinFile = await ReadListFromFile();

            if (paperListinFile == null)
            {
                return new List<NewsPaper>();
            }

            //List<NewsPaper> newsPapers = new List<NewsPaper>();
            foreach (var paper in paperListinFile)
            {
                NewsPapers.Add(new NewsPaper(paper));
            }
            return NewsPapers;
        }

        /// <summary>
        ///  ///byte数组转换成object
        /// </summary>
        /// <param name="arrBytes"></param>
        /// <returns>字节数组</returns>
        private static Object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new System.IO.MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, System.IO.SeekOrigin.Begin);
                Object obj;
                try
                {
                    obj = binForm.Deserialize(memStream);
                }
                catch (System.Runtime.Serialization.SerializationException exception)
                {
                    obj = null;
                }
                return obj;
            }
        }
        /// <summary>
        /// object转换成byte数组
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Object</returns>
        private static byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new System.IO.MemoryStream())
            {
                try
                {
                    bf.Serialize(ms, obj);
                }
                catch(System.Runtime.Serialization.SerializationException serializationException)
                {
                    ;
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 从文件中获取报纸列表
        /// </summary>
        /// <param></param>
        private static async Task<List< NewsPaperModel>> ReadListFromFile()
        {
            var storageFolder = ApplicationData.Current.LocalFolder;

            //0、打开文件
            string paperListFileName = "PaperList.dat";

            try
            {
                paperListFile = await storageFolder.CreateFileAsync(paperListFileName, CreationCollisionOption.OpenIfExists);
            }
            catch (System.IO.FileLoadException exception)
            {
                ;
            }
            List<NewsPaperModel> paperListinFile = new List<NewsPaperModel>();

            //1、读取报纸列表数据
            var stream = await paperListFile.OpenAsync(FileAccessMode.ReadWrite); //获取文件随机存取流
            using (var inputStream = stream.GetInputStreamAt(0))//获取从0开始的输入流
            {
                var dataReader = new DataReader(inputStream); //在该输入流中附着一个数据读取器
                uint loadBytes = await dataReader.LoadAsync((uint)stream.Size); //加载数据数据到中间缓冲区
                byte[] bytes = new byte[(uint)stream.Size];
                dataReader.ReadBytes(bytes);//用来存储从文件中读出的数据
                paperListinFile = (List<NewsPaperModel>)ByteArrayToObject(bytes); //将读出的数据转换成SortedDictionary<int, NewsPaper>
            }
            stream.Dispose();
            return paperListinFile;
        }

        /// <summary>
        /// 添加订阅源
        /// </summary>
        /// <param name="feedUrl">订阅源链接</param>
        public void AddFeedLink(Uri feedUrl)
        {
            //var feed = await new SyndicationClient().RetrieveFeedAsync(feedUrl);
            //var feedXml = feed.GetXmlDocument(feed.SourceFormat);
            newsPaperModel.FeedUrls.Add(feedUrl);
        }

        /// <summary>
        /// 获取文章列表
        /// </summary>
        public async Task<List<NewsItem>> GetNewsListAsync()
        {
            OnNewsRefreshing?.Invoke();
            List<NewsItem> newNewsitems = new List<NewsItem>();
            int originalNewsCount = NewsList.Count;
            foreach (var feedUrl in FeedUrls)
            {
                var syndicationClient = new SyndicationClient();
                try
                {
                    var feed = await new SyndicationClient().RetrieveFeedAsync(feedUrl);
                    //将新闻添加到newsItems中
                    foreach(var news in feed.Items)
                    {
                        var newsLink = news.ItemUri ?? news.Links.Select(l => l.Uri).FirstOrDefault();
                        var newsItem = new NewsItem(news, newsLink, PaperTitle);

                        //如果原新闻列表中不包含改新闻，则添加到新闻列表
                        if (!NewsList.Contains(newsItem))
                        {
                            var newNewsItem = new NewsItem(news, newsLink, PaperTitle);
                            newNewsitems.Add(newNewsItem);
                            NewsList.Add(newNewsItem);
                        }
                    }
                }
                catch(Exception exception)
                {
                    OnUpdateFailed?.Invoke(feedUrl.AbsoluteUri);
                }
            }
            OnNewsRefreshed?.Invoke(NewsList);
            if (NewsList.Count != originalNewsCount)
            {
                await SaveToFile(this);
                OnNewsUpdatedToFile?.Invoke();
            }
            return NewsList;
        }
        
        //好像没用
        /// <summary>
        /// 加载之前保存的报纸列表
        /// </summary>
        /// <returns>报纸列表</returns>
        public static async Task<List<NewsPaper>> GetNewsPapers()
        {
            if (!(await ApplicationData.Current.LocalFolder.TryGetItemAsync("PaperList.dat") is StorageFile file))
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

        /// <summary>
        /// 打开的报纸列表
        /// </summary>
        public static List<NewsPaper> NewsPapers { get; private set; } = new List<NewsPaper>();

        /// <summary>
        /// 新闻列表
        /// </summary>
        //private List<NewsItem> newsList = new List<NewsItem>();
        public List<NewsItem> NewsList
        {
            get
            {
                return newsPaperModel.NewsList;
            }
        }

        /// <summary>
        /// 删除一张报纸
        /// </summary>
        /// <param name="newsPaper">要删除的报纸</param>
        static async public Task DeleteNewsPaper(NewsPaper newsPaper)
        {
            OnPaperDeleting?.Invoke(newsPaper);
            var paperListinFile = await ReadListFromFile();
            //var index = paperListinFile.IndexOf(newsPaper.newsPaperModel);
            paperListinFile.Remove(newsPaper.newsPaperModel);
            NewsPapers.Remove(newsPaper);
            //3、将paperListinFile重新保存到文件中
            await FileIO.WriteBytesAsync(paperListFile, ObjectToByteArray(paperListinFile));
            OnPaperDeleted?.Invoke(newsPaper);
        }

        public void UpdateNewsList(NewsItem newsItem)
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

        public delegate void OnNewsRefreshedDelegate(IList<NewsItem> newsItem);
        public event OnNewsRefreshedDelegate OnNewsRefreshed; //报纸刷新后

        public delegate void OnUpdateFailedDelegate(string failNewsPaperTitle);
        public event OnUpdateFailedDelegate OnUpdateFailed; //报纸更新失败后

        public delegate void OnNewsListUpdatedEventHandler(NewsItem newsItem);
        public event OnNewsListUpdatedEventHandler OnNewsListUpdated;
    }
}
