using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace InkingNewsstand.Models
{
    public class InkingNewsstandDbContext:DbContext
    {
        public DbSet<Feed> Feeds { set; get; }
        public DbSet<Article> Articles { set; get; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=InkingNewsstand.db");
    }
}
