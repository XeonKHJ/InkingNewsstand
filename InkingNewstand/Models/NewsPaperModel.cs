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
        }

        public NewsPaperModel(string paperTitle)
        {
            PaperTitle = paperTitle;
        }

        public List<Uri> FeedUrls { get; set; } = new List<Uri>();
        public string PaperTitle { get; set; } = "未命名报纸";
        public int Count
        {
            get
            {
                return NewsList.Count;
            }
        }
        public bool ExtendMode = false; //扩展模式
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
