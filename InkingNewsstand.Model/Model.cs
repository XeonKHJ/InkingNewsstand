using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace InkingNewsstand.Model
{
    public class InkingNewsstandContext : DbContext
    {
        public DbSet<NewsPaper> NewsPapers { get; set; }
        //public DbSet<News> News { get; set; }
        public DbSet<Feed> Feeds { set; get; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=InkingNewsstand.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //设置News主键。
            modelBuilder.Entity<News>()
                .HasKey(n => new { n.Title, n.Authors, n.PublishedDate }).HasName("CompositePrimaryKey_News");

            //设置报纸和订阅源的单一导航多对多关系。
            modelBuilder.Entity<NewsPaper>()
                .HasMany(np => np.Feeds).WithOne().HasConstraintName("ManyToOne_OneFeedBelongsToManyNewsPapers");

            //设置订阅源和新闻的一定多关系
            modelBuilder.Entity<News>()
                .HasOne(n => n.Feed).WithMany(f => f.News).HasForeignKey(n => n.FeedId);
        }
    }


}
