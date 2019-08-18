using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InkingNewsstand.Model
{
    public class Feed
    {
        [Key]
        public string Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }

        public List<News> News { set; get; } = new List<News>();

        public List<NewsPaper_Feed> NewsPaper_Feeds { set; get; } = new List<NewsPaper_Feed>();
    }
}
