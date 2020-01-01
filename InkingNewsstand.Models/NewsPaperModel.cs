using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InkingNewsstand.Models
{
    [Table("NewsPapers")]
    public class NewsPaperModel
    {
        public int Id { get; set; }
        public string Title { set; get; }
        public byte[] Icon { set; get; }
        public List<NewsPaperFeedModel> NewsPaperFeedModels { set; get; } = new List<NewsPaperFeedModel>();
    }
}
