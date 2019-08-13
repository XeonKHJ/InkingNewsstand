using System;
using System.ComponentModel.DataAnnotations;

namespace InkingNewsstand.Model
{
    public class News
    {
        public string Content { get; set; }

        public string FeedId { set; get; }

        public Feed Feed { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public DateTimeOffset PublishedDate { get; set; }

        public string NewsLink { set; get; }

        public NewsPaper NewsPaper { set; get; }

        public string Authors { get; set; }

        public string CoverUrl{ get; set; }

        public string InnerHTML { get; set; }

        public string ExtendedHtml { get; set; }

        public byte[] InkStrokes { get; set; }

        public bool IsFavorite { get; set; } //收藏属性
    }
}
