using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace InkingNewsstand.Models
{
    [Table("Feeds")]
    public class FeedModel
    {
        public string Title { set; get; }
        public string Id { set; get; }
        public string Description { set; get; }
        public byte[] Icon { set; get; }
        public List<ArticleModel> Articles { set; get; } = new List<ArticleModel>();
        public List<NewsPaperFeedModel> NewsPaperFeedModels { set; get; } = new List<NewsPaperFeedModel>();
    }
}
