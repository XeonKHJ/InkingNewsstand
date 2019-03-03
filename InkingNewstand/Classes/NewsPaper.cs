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
        private List<Uri> feedUrls = new List<Uri>();
        public NewsPaper(string paperName)
        {
            this.PaperTitle = paperName;
        }

        /// <summary>
        /// 报纸名
        /// </summary>
        public string PaperTitle { get; } = "未命名报纸";


        /// <summary>
        /// 保存到文件
        /// </summary>
        /// <param name="newsPaper"></param>
        /// <returns>要保存的报纸</returns>
        static public async Task SaveToFile(NewsPaper newsPaper)
        {
            //1、获取报纸列表
            var paperListinFile = await ReadListFromFile();

            //2、获取当前报纸编号
            ////2.1、如果文件中没有保存任何东西，则新建一个paperListinFile
            if (paperListinFile == null)
            {
                paperListinFile = new SortedDictionary<int, NewsPaper>();
            }

            var paperEnumer = (from v in paperListinFile where v.Value.PaperTitle == newsPaper.PaperTitle select v); //搜索结果
            ////2.2、如果文件中没有保存此添加
            if (paperEnumer.Count() == 0)
            {
                paperListinFile.Add(paperListinFile.Count, newsPaper);
            }
            ////2.3、如果有则修改
            else if (paperEnumer.Count() == 1)
            {
                var paperEnumerResult = paperEnumer.First();
                paperListinFile[paperEnumerResult.Key] = newsPaper;
                //foreach (var paper in paperEnumer)
                //{
                //    paperListinFile[paper.Key] = newsPaper;
                //}
            }
            ////2.4若查到有多张报纸有相同名字，则显示错误
            else
            {
                throw new Exception("文件错误");
            }

            //3、将paperListinFile重新保存到文件中
            await FileIO.WriteBytesAsync(paperListFile, ObjectToByteArray(paperListinFile));

            OnPaperAdded?.Invoke(); //添加完成后引发该事件
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

            List<NewsPaper> newsPapers = new List<NewsPaper>();
            foreach (var paperPair in paperListinFile)
            {
                newsPapers.Add(paperPair.Value);
            }
            return newsPapers;
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
        private static async Task<SortedDictionary<int, NewsPaper>> ReadListFromFile()
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
            SortedDictionary<int, NewsPaper> paperListinFile = new SortedDictionary<int, NewsPaper>();

            //1、读取报纸列表数据
            var stream = await paperListFile.OpenAsync(FileAccessMode.ReadWrite); //获取文件随机存取流
            using (var inputStream = stream.GetInputStreamAt(0))//获取从0开始的输入流
            {
                var dataReader = new DataReader(inputStream); //在该输入流中附着一个数据读取器
                uint loadBytes = await dataReader.LoadAsync((uint)stream.Size); //加载数据数据到中间缓冲区
                byte[] bytes = new byte[(uint)stream.Size];
                dataReader.ReadBytes(bytes);//用来存储从文件中读出的数据
                paperListinFile = (SortedDictionary<int, NewsPaper>)ByteArrayToObject(bytes); //将读出的数据转换成SortedDictionary<int, NewsPaper>
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
            feedUrls.Add(feedUrl);
        }

        /// <summary>
        /// 订阅源数量
        /// </summary>
        public int Count
        {
            get
            {
                return feedUrls.Count;
            }
        }

        /// <summary>
        /// 文章列表
        /// </summary>
        public async Task<List<NewsItem>> GetNewsListAsync()
        {
            foreach (var feedUrl in feedUrls)
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
                        if (!newsList.Contains(newsItem))
                        {
                            newsList.Add(new NewsItem(news, newsLink, PaperTitle));
                        }
                    }
                }
                catch(Exception exception)
                {
                    OnUpdateFailed?.Invoke(feedUrl.AbsoluteUri);
                }
            }
            

            OnNewsRefreshed?.Invoke();
            return newsList;
        }

        
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

        public List<Uri> FeedUrls
        {
            get
            {
                return feedUrls;
            }
        }

        /// <summary>
        /// 新闻列表
        /// </summary>
        private List<NewsItem> newsList = new List<NewsItem>();
        public List<NewsItem> NewsList
        {
            get
            {
                return newsList;
            }
        }

        /// <summary>
        /// 删除一张报纸
        /// </summary>
        /// <param name="newsPaper">要删除的报纸</param>
        static async public Task DeleteNewsPaper(NewsPaper newsPaper)
        {
            var paperListinFile = await ReadListFromFile();
            var paperEnumer = (from v in paperListinFile where v.Value.PaperTitle == newsPaper.PaperTitle select v);
            int pairKeyToDelete = -1;
            foreach (var paperPair in paperEnumer)
            {
                pairKeyToDelete = paperPair.Key;
            }
            paperListinFile.Remove(pairKeyToDelete);
            //3、将paperListinFile重新保存到文件中
            await FileIO.WriteBytesAsync(paperListFile, ObjectToByteArray(paperListinFile));
            OnPaperDeleted?.Invoke();
        }

        public delegate void OnPaperUpdatedDelegate();
        public static event OnPaperUpdatedDelegate OnPaperAdded;
        public static event OnPaperUpdatedDelegate OnPaperDeleted;

        public delegate void OnNewsUpdatedDelegate();
        public event OnNewsUpdatedDelegate OnNewsRefreshed;

        public delegate void OnUpdateFailedDelegate(string failNewsPaperTitle);
        public event OnUpdateFailedDelegate OnUpdateFailed;
    }
}
