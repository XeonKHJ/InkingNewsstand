using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
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
        /// 保存到文件
        /// </summary>
        /// <param name="newsPaper"></param>
        /// <returns>要保存的报纸</returns>
        static public async Task SaveToFile(NewsPaper newsPaper)
        {
            var storageFolder = ApplicationData.Current.LocalFolder;

            //0、打开文件
            string paperListFileName = "PaperList.dat";
            var paperListFile = await storageFolder.CreateFileAsync(paperListFileName, CreationCollisionOption.OpenIfExists);
            SortedDictionary<int, NewsPaper> paperListinFile = new SortedDictionary<int, NewsPaper>();

            //1、读取报纸列表数据
            var stream = await paperListFile.OpenAsync(FileAccessMode.ReadWrite); //获取文件随机存取流
            using (var inputStream = stream.GetInputStreamAt(0))//获取从0开始的输入流
            { 
                var dataReader = new DataReader(inputStream); //在该输入流中附着一个数据读取器
                uint loadBytes = await dataReader.LoadAsync((uint)stream.Size); //加载数据数据到中间缓冲区
                byte[] bytes = new byte[(uint)stream.Size]; //用来存储从文件中读出的数据
                paperListinFile = (SortedDictionary<int, NewsPaper>)ByteArrayToObject(bytes); //将读出的数据转换成SortedDictionary<int, NewsPaper>
            }

            //2、获取当前报纸编号
            ////2.1、如果文件中没有保存任何东西，则新建一个paperListinFile
            if (paperListinFile == null)
            {
                paperListinFile = new SortedDictionary<int, NewsPaper>();
            }

            var paperEnumer = (from v in paperListinFile where v.Value.PaperTitle == newsPaper.PaperTitle select v);
            ////2.2、如果文件中没有保存此添加
            if(paperEnumer.Count() == 0)
            {
                paperListinFile.Add(paperListinFile.Count, newsPaper);
            }
            ////2.3、如果有则修改
            else if(paperEnumer.Count() == 1)
            {
                foreach(var paper in paperEnumer)
                {
                    paperListinFile[paper.Key] = newsPaper;
                }
            }
            ////2.4若查到有多张报纸有相同名字，则显示错误
            else
            {
                throw new Exception("文件错误");
            }

            //3、将paperListinFile重新保存到文件中
            await FileIO.WriteBytesAsync(paperListFile, ObjectToByteArray(paperListinFile));
        }

        /// <summary>
        /// 从文件读取报纸列表
        /// </summary>
        /// <returns></returns>
        static public async Task<List<NewsPaper>> ReadFromFile()
        {
            var storageFolder = ApplicationData.Current.LocalFolder;
            //0、打开文件
            string paperListFileName = "PaperList.dat";
            var paperListFile = await storageFolder.CreateFileAsync(paperListFileName, CreationCollisionOption.OpenIfExists);
            SortedDictionary<int, NewsPaper> paperListinFile = new SortedDictionary<int, NewsPaper>();

            //1、读取报纸列表数据
            var stream = await paperListFile.OpenAsync(FileAccessMode.ReadWrite); //获取文件随机存取流
            using (var inputStream = stream.GetInputStreamAt(0))//获取从0开始的输入流
            {
                var dataReader = new DataReader(inputStream); //在该输入流中附着一个数据读取器
                uint loadBytes = await dataReader.LoadAsync((uint)stream.Size); //加载数据数据到中间缓冲区
                byte[] bytes = new byte[(uint)stream.Size]; //用来存储从文件中读出的数据
                paperListinFile = (SortedDictionary<int, NewsPaper>)ByteArrayToObject(bytes); //将读出的数据转换成SortedDictionary<int, NewsPaper>
            }

            List<NewsPaper> newsPapers = new List<NewsPaper>();
            foreach(var paperPair in paperListinFile)
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
                var obj = binForm.Deserialize(memStream);
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
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
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
