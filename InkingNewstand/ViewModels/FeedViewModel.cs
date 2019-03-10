using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace InkingNewstand.ViewModels
{
    public class FeedViewModel
    {
        public FeedViewModel(SyndicationFeed feed, string url)
        {
            Title = feed.Title.Text;
            Url = feed.Id == "" ? url : feed.Id;
            //Description = feed.Subtitle.Text;
            Icon = feed.IconUri is null ? "Nopic": feed.IconUri.AbsoluteUri;
        }

        public FeedViewModel(string title, string url, string description, string icon)
        {
            Title = title;
            Url = url;
            Description = description;
            Icon = icon;
        }

        public string Title { get; private set; }
        public string Url { get; private set; }
        public string Description { get; private set; }
        public string Icon { get; private set; }
    }
}
