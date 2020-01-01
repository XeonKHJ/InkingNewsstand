using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace InkingNewsstand.Models
{
    public class InkingNewsstandDbContext:DbContext
    {
        public DbSet<FeedModel> Feeds { set; get; }
        public DbSet<ArticleModel> Articles { set; get; }
        public DbSet<NewsPaperModel> NewsPapers { set; get; }
        public DbSet<NewsPaperFeedModel> NewsPaperFeedModels { set; get; } 
        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=InkingNewsstand.db");
    }
}
