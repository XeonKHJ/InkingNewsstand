using System;
using System.Collections.Generic;
using System.Text;

namespace InkingNewsstand.Models
{
    public class Article
    {
        public int Id { set; get; }
        public string Titile { set; get; }
        public string Summary { set; get; }
        public DateTime PublishDate { set; get; }
        public byte[] InkStrikes { set; get; }
        public string Content { set; get; }
        public Uri Link { set; get; }
        public Feed Feed { set; get; }
    }
}
