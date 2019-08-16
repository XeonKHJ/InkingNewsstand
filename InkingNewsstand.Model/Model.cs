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
            modelBuilder.Entity<NewsPaper_Feed>().HasKey(nf => new { nf.FeedId, nf.NewsPaperId });

            //设置订阅源和报纸的多对多关系
            modelBuilder.Entity<NewsPaper_Feed>()
                .HasOne(nf => nf.Feed).WithMany(f => f.NewsPaper_Feeds).HasForeignKey(nf => nf.FeedId);
            modelBuilder.Entity<NewsPaper_Feed>()
                .HasOne(nf => nf.NewsPaper).WithMany(np => np.NewsPaper_Feeds).HasForeignKey(nf => nf.NewsPaperId);

            //设置订阅源和新闻的一对多关系
            modelBuilder.Entity<News>().HasOne(f => f.Feed).WithMany(n => n.News).HasForeignKey(n => n.FeedId);
        }
    }
}
