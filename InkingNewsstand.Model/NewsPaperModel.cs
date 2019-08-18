using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InkingNewsstand.Model
{
    public class NewsPaper
    {
        public int Id { set; get; }

        public List<NewsPaper_Feed> NewsPaper_Feeds { get; set; } = new List<NewsPaper_Feed>();

        /// <summary>
        /// 报纸标题。
        /// </summary>
        public string PaperTitle { get; set; }

        public bool ExtendMode { get; set; }

        public string IconType { get; set; }
    }
}
