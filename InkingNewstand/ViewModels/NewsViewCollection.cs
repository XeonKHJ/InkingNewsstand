using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace InkingNewstand.ViewModels
{
    class NewsViewCollection: ObservableCollection<NewsItem>, ISupportIncrementalLoading
    {
        public NewsViewCollection(List<NewsItem> _newsItems)
        {
            this.newsItems = _newsItems;
            HasMoreItems = true;
        }

        public NewsViewCollection()
        {
            this.newsItems = new List<NewsItem>();
        }

        private void LoadData(int index)
        {
            System.Diagnostics.Debug.WriteLine($"LoadMoreItemsAsync{index}, {currentIndex}");
            int max = index + 10;
            for (int i = index; i< max && i < newsItems.Count; ++i)
            {
                var news = newsItems[newsItems.Count - i - 1];
                Add(newsItems[newsItems.Count - i - 1]);
            }
            currentIndex = max;
        }
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            
            return InnerLoadMoreItemsAsync(count).AsAsyncOperation();
        }

        private async Task<LoadMoreItemsResult> InnerLoadMoreItemsAsync(uint count)
        {
            if (currentIndex >= newsItems.Count)
            {
                HasMoreItems = false;
            }
            else
            {
                LoadData((int)currentIndex);
            }
            return new LoadMoreItemsResult { Count = (uint)currentIndex };
        }

        private int currentIndex = 0;

        private List<NewsItem> newsItems { set; get; } = new List<NewsItem>();

        public bool HasMoreItems
        {
            get;
            private set;
        }
    }
}
