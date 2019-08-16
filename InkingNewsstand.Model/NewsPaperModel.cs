using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InkingNewsstand.Model
{
    public class NewsPaper
    {

        public List<NewsPaper_Feed> NewsPaper_Feeds { get; set; }

        public int Id { set; get; }

        /// <summary>
        /// 报纸标题。
        /// </summary>
        public string PaperTitle { get; set; }

        public bool ExtendMode { get; set; }

        public string IconType { get; set; }
    }
}
