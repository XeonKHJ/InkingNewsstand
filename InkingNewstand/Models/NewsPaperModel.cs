using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkingNewstand.Models
{
    [Serializable]
    public class NewsPaperModel
    {
        public List<Uri> FeedUrls { get; set; } = new List<Uri>();
        public string PaperTitle { get; set; } = "未命名报纸";
        public int Count
        {
            get
            {
                return NewsList.Count;
            }
        }
        public List<NewsItem> NewsList { get; set; } = new List<NewsItem>();
    }
}
