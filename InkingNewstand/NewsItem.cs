using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace InkingNewstand
{
    public class NewsItem
    {
        private SyndicationItem item;
        public NewsItem(SyndicationItem item)
        {
            this.item = item;
        }

        public override string ToString()
        {
            return item.Title.Text.ToString();
        }

        public SyndicationItem Item
        {
            get { return item; }
        }
    }
}
