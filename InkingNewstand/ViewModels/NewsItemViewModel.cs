using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkingNewstand.ViewModels
{
    class NewsItemViewModel
    {
        public NewsItemViewModel(List<NewsItem> newsItems)
        {
            NewsCollection = new ObservableCollection<NewsItem>(newsItems);
            NewsCollection.CollectionChanged += NewsCollection_CollectionChanged;
        }

        private void NewsCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<NewsItem> NewsCollection { get; }
    }
}
