using System;
using System.Collections.Generic;
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
        }

        private async void LayoutNews(NewsPaper feeds)
        {
            try
            {
                newsItems = await feeds.GetNewsListAsync();
                Bindings.Update();
            }
            catch(Exception exception)
            {
                //同步失败信息
            }
            //titleTextBlock.Text = feeds.PaperTitle;

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
                feeds.OnNewsRefreshed += Feeds_OnNewsRefreshed;
                feeds.OnNewsUpdated += Feeds_OnNewsUpdated;
                feeds.OnUpdateFailed += Feeds_OnUpdateFailed;
                titleTextBlock.Text = feeds.PaperTitle;
                LayoutNews(feeds);
            }
        }

        /// <summary>
        /// 新闻有更新后才更新绑定
        /// </summary>
        private void Feeds_OnNewsUpdated()
        {
            //Bindings.Update();
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

        private void Feeds_OnNewsRefreshed()
        {
            //Bindings.Update();
            refreshingProgressRing.IsActive = false;
        }

        static public NewsPaper feeds { get;  set; }

        List<NewsItem> newsItems;

        List<NewsItem> Row1NewsItemList
        {
            get
            {
                return newsItems;
            }
        }

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
    }
}
