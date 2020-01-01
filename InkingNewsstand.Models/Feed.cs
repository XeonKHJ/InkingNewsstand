using System;
using System.Collections.Generic;

namespace InkingNewsstand.Models
{
    public class Feed
    {
        public string Title { set; get; }
        public string Id { set; get; }
        public string Description { set; get; }
        public byte[] Icon { set; get; }
        public List<Article> Articles { set; get; }
    }
}
