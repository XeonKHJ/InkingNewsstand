using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InkingNewsstand.Models
{
    [Table("NewsPaper_Feed")]
    public class NewsPaperFeedModel
    {
        public int Id { set; get; }
        public NewsPaperModel NewsPaper { set; get; }
        public int NewsPaperId { set; get; }
        public FeedModel Feed { set; get; }
        public string FeedId { set; get; }
    }
}
