using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InkingNewsstand.Models
{
    [Table("Articles")]
    public class ArticleModel
    {
        public int Id { set; get; }
        public string Titile { set; get; }
        public string Summary { set; get; }
        public DateTime PublishDate { set; get; }
        public byte[] InkStrikes { set; get; }
        public string Content { set; get; }
        public Uri Link { set; get; }
        public string FeedId { set; get; }
        public FeedModel Feed { set; get; }
    }
}
