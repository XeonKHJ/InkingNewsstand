using System;
using System.ComponentModel.DataAnnotations;

namespace InkingNewsstand.Model
{
    public class News
    {
        public string Content { get; set; }

        public Feed Feed { get; private set; }

        public string Title { get; private set; }

        public string Summary { get; private set; }

        public DateTimeOffset PublishedDate { get; private set; }

        public string NewsLink { private set; get; }

        public NewsPaper NewsPaper { set; get; }

        public string Authors { get; private set; }

        public string CoverUrl{ get; set; }

        public string InnerHTML { get; private set; }

        public string ExtendedHtml { get; set; }

        public byte[] InkStrokes { get; set; } = new byte[0];

        public bool IsFavorite { get; set; } //收藏属性
    }
}
