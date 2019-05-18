using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkingNewstand.Models
{
    [Serializable]
    public class NewsPaperModel : IEquatable<NewsPaperModel>
    {
        public NewsPaperModel(NewsPaper newsPaper)
        {
            FeedUrls = newsPaper.FeedUrls;
            PaperTitle = newsPaper.PaperTitle;
            NewsList = newsPaper.NewsList;
            Feeds = newsPaper.Feeds;
        }

        public NewsPaperModel(string paperTitle)
        {
            PaperTitle = paperTitle;
        }

        /// <summary>
        /// 订阅源列表
        /// </summary>
        public List<FeedModel> Feeds { set; get; } = new List<FeedModel>();

        /// <summary>
        /// 存入的订阅源URL列表
        /// </summary>
        public List<Uri> FeedUrls { get; set; } = new List<Uri>();

        /// <summary>
        /// 报纸标题
        /// </summary>
        public string PaperTitle { get; set; } = "未命名报纸";

        /// <summary>
        /// 新闻数
        /// </summary>
        public int Count
        {
            get
            {
                return NewsList.Count;
            }
        }
        public bool ExtendMode = false; //扩展模式

        /// <summary>
        /// 新闻列表
        /// </summary>
        public List<NewsItem> NewsList { get; set; } = new List<NewsItem>();

        public bool Equals(NewsPaperModel other)
        {
            return this.PaperTitle == other.PaperTitle;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach(var character in PaperTitle)
            {
                hash += character;
            }
            return hash;
        }
    }
}
