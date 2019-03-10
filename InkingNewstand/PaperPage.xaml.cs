using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace InkingNewstand
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PaperPage : Page
    {
        public PaperPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled; //开启页面缓存
            if(feeds != null)
            {
                feeds.OnNewsRefreshed += Feeds_OnNewsRefreshed;
            }
        }

        private async void LayoutNews()
        {
            Feeds_OnNewsRefreshed(feeds.NewsList);
        }

        private async void RefreshNews()
        {
            await feeds.GetNewsListAsync();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!(e.Parameter is NewsPaper))
            {
                throw new Exception();
            }
            else
            {
                feeds = (NewsPaper)e.Parameter;
                feeds.OnNewsRefreshing += Feeds_OnNewsRefreshing;
                feeds.OnNewsRefreshed += Feeds_OnNewsRefreshed;
                feeds.OnUpdateFailed += Feeds_OnUpdateFailed;
                titleTextBlock.Text = feeds.PaperTitle;
                LayoutNews();
            }
        }

        private void Feeds_OnNewsRefreshed(IList<NewsItem> newsItem)
        {
            foreach(var item in newsItem)
            {
                if(!newsItems.Contains(item))
                {
                    newsItems.Insert(0, item);
                }
            }
            refreshingProgressRing.IsActive = false;
        }

        private void Feeds_OnNewsRefreshing()
        {
            refreshingProgressRing.IsActive = true;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if(e.Parameter is NewsPaper)
            {
                this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled; //关闭页面缓存
            }
        }

        private void Feeds_OnUpdateFailed(string failNewsPaperTitle)
        {
            errorTextBlock.Text = "连接failNewsPaperTitle失败";
        }

        static public NewsPaper feeds { get;  set; }

        ObservableCollection<NewsItem> newsItems { get; set; } = new ObservableCollection<NewsItem>();

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(NewsDetailPage), e.ClickedItem);
        }

        private void AddPaperButton_Click(object sender, RoutedEventArgs e)
        {
            addPaperButton.Visibility = Visibility.Collapsed;
            this.Frame.Navigate(typeof(AddPaperPage));
        }

        private void EditPaperButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AddPaperPage), PaperPage.feeds);
        }

        private void RefreshPaperButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshNews();
        }
    }
}
