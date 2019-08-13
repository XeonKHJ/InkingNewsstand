using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace InkingNewsstand.Model
{
    public class InkingNewsstandContext : DbContext
    {
        //public DbSet<NewsPaper> NewsPapers { get; set; }
        //public DbSet<News> News { get; set; }
        public DbSet<Feed> Feeds { set; get; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=InkingNewsstand.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<News>()
            //    .HasKey(n => new { n.Title, n.Authors, n.PublishedDate }).HasName("CompositePrimaryKey_News");

            //modelBuilder.Entity<News>()
            //    .HasOne(n => n.NewsPaper).WithMany(np => np.News).HasForeignKey(n => n.NewsPaper).HasConstraintName("ForeignKey_NewsInNewsList");
        }
    }


}
