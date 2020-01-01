using InkingNewsstand.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace InkingNewsstand.EFMigrationConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using var db = new InkingNewsstandDbContext();
            var newsPaper = db.NewsPapers.Include(n => n.NewsPaperFeedModels).ThenInclude(nf => nf.Feed).ThenInclude(f=>f.Articles).ThenInclude(a => a.Feed).ToList();

            db.SaveChanges();
        }
    }
}
