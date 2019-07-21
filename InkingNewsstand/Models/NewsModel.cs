using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input.Inking;

namespace InkingNewsstand.Models
{
    [Serializable]
    public class NewsModel
    {
        public string Title { get;  set; }

        public string Summary { get;  set; }

        public DateTimeOffset PublishedDate { get; set; }

        public Uri NewsLink { set; get; }

        public string PaperTitle { set; get; }

        public string Authors { get; set; }

        public string CoverUrl { get; set; }

        public string InnerHTML { get; set; }

        public string ExtendedHtml { get; set; } 

        public static bool operator ==(NewsModel lhs, NewsModel rhs)
        {
            try
            {
                if (lhs.Title == rhs.Title && lhs.PublishedDate == rhs.PublishedDate)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        public static bool operator !=(NewsModel lhs, NewsModel rhs)
        {
            return !(lhs == rhs);
        }

        public IReadOnlyList<InkStroke> currentStrokes { get; set; }
    }
}
