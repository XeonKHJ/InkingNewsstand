using InkingNewsstand.Models;
using System;

namespace InkingNewsstand.EFMigrationConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var db = new InkingNewsstandDbContext())
            {
                Console.WriteLine("Fuckoff");
                db.Articles.Add(new Article
                {
                    Content = "balbal"
                }) ;

                db.SaveChanges();
            }
        }
    }
}
