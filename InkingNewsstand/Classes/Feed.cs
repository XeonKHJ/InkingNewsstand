using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace InkingNewsstand.Classes
{
    public class Feed
    {
        public Feed(SyndicationFeed feed, Uri uri)
        {
            Title = feed.Title.Text;
            Id = uri.AbsoluteUri;
            //Description = feed.Subtitle.Text;
            Icon = feed.IconUri is null ? (feed.ImageUri is null ? "nopic" : feed.ImageUri.AbsoluteUri) : feed.IconUri.AbsoluteUri;
        }

        public Feed(Model.Feed feedModel)
        {
            Title = feedModel.Title;
            Id = feedModel.Id;
            Description = feedModel.Description;
            Icon = feedModel.Icon;
        }

        public void Update(SyndicationFeed feed)
        {
            Title = feed.Title.Text;
            Id = feed.Id;
            //Description = feed.Subtitle.Text;
            Icon = feed.IconUri is null ? (feed.ImageUri is null ? "nopic" : feed.ImageUri.AbsoluteUri) : feed.IconUri.AbsoluteUri;
        }

        public string Title { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }

        public bool Equals(Feed other)
        {
            return Id == other.Id;
        }
    }
}
