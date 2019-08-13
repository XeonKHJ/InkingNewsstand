using System;
using System.Collections.Generic;
using System.Text;

namespace InkingNewsstand.Model
{
    public class NewsPaper
    {
        /// <summary>
        /// 订阅源列表
        /// </summary>
        public List<Feed> Feeds { set; get; } = new List<Feed>();

        /// <summary>
        /// 存入的订阅源URL列表
        /// </summary>
        public List<string> FeedUrls { get; set; }

        public List<News> News { get; set; } 

        /// <summary>
        /// 报纸标题
        /// </summary>
        public string PaperTitle { get; set; } = "未命名报纸";
        public bool ExtendMode = false; //扩展模式
    }
}
