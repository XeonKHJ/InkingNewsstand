using InkingNewsstand.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace InkingNewsstand.Utilities
{
    public static class DataOperator
    {
        public static void MigrateData()
        {
            using(var db = new InkingNewsstandContext())
            {
                db.Database.Migrate();
            }
        }

        public static List<NewsPaper> GetNewsPapers()
        {
            List<NewsPaper> models;
            using (var db = new InkingNewsstandContext())
            {
                models = db.NewsPapers.ToList();
            }
            return models;
        }

        public static async Task SaveNewsPapers()
        {
            using (var db = new InkingNewsstandContext())
            {
                await db.SaveChangesAsync();
            }
        }
    }
}
