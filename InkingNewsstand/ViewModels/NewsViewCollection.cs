using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using InkingNewsstand.Classes;

namespace InkingNewsstand.ViewModels
{
    class NewsViewCollection: ObservableCollection<News>, ISupportIncrementalLoading
    {
        public NewsViewCollection(List<News> _newsItems)
        {
            this.newsItems = _newsItems;
            HasMoreItems = true;
        }

        public NewsViewCollection()
        {
            this.newsItems = new List<News>();
        }

        private void LoadData(int index)
        {
            System.Diagnostics.Debug.WriteLine($"LoadMoreItemsAsync{index}, {currentIndex}");
            int max = index + 20;
            for (int i = index; i< max && i < newsItems.Count; ++i)
            {
                var news = newsItems[newsItems.Count - i - 1];
                Add(newsItems[newsItems.Count - i - 1]);
            }
            currentIndex = max;
        }

        /// <summary>
        /// 如果有继续往下刷，则加载更多新闻。
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
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

        private int currentIndex = 0; //当前加载到内存的新闻的索引

        private List<News> newsItems { set; get; } = new List<News>();

        public bool HasMoreItems
        {
            get;
            private set;
        }
    }
}
