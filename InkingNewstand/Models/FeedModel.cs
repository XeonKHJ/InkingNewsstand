using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace InkingNewstand.Models
{
    [Serializable]
    public class FeedModel
    {
        public FeedModel(SyndicationFeed feed)
        {
            Title = feed.Title.Text;
            Id = feed.Id;
            //Description = feed.Subtitle.Text;
            Icon = feed.IconUri is null ? "Nopic" : feed.IconUri.AbsoluteUri;
        }

        public string Title { get; private set; }
        public string Id { get; private set; }
        public string Description { get; private set; }
        public string Icon { get; private set; }
    }
}
