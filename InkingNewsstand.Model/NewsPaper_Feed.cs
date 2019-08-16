using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InkingNewsstand.Model
{
    public class NewsPaper_Feed
    {
        public int NewsPaperId { set; get; }

        public string FeedId { set; get; }

        public Feed Feed { set; get; }

        public NewsPaper NewsPaper {set;get;}
    }
}
