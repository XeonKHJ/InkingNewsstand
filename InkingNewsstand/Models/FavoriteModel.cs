using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkingNewsstand.Models
{
    [Serializable]
    public class FavoriteModel : IEquatable<FavoriteModel>
    {
        public FavoriteModel(NewsItem newsItem)
        {
            NewsPaperTitle = newsItem.PaperTitle;
            NewsTitle = newsItem.Title;
            DateTime = newsItem.PublishedDate;
            Authors = newsItem.Authors;
            HashCode = newsItem.GetHashCode();
        }
        public string NewsPaperTitle;
        public string NewsTitle;
        public DateTimeOffset DateTime;
        public string Authors;
        public int HashCode;

        public bool Equals(FavoriteModel other)
        {
            if(NewsPaperTitle == other.NewsPaperTitle)
            {
                if(NewsTitle == other.NewsTitle)
                {
                    if (DateTime == other.DateTime)
                    {
                        if(Authors == other.Authors)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
