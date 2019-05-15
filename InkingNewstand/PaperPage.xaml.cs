using InkingNewstand.ViewModels;
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
            this.NavigationCacheMode = NavigationCacheMode.Enabled; //开启页面缓存
            thisPaperpage = this;
            //if(feeds != null)
            //{
            //    feeds.OnNewsRefreshed += Feeds_OnNewsRefreshed;
            //}
        }
        public static PaperPage thisPaperpage;
        private void MainPage_CleanPaperPage()
        {
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        private void LayoutNews()
        {
            Feeds_OnNewsRefreshed(feeds.NewsList);
        }

        public async void RefreshNews()
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
                //设置导航栏
                if(MainPage.MainPageNavigationView.SelectedItem != e.Parameter)
                {
                    MainPage.NavigationEnabled = false;
                    MainPage.MainPageNavigationView.SelectedItem = e.Parameter;
                }

                feeds = (NewsPaper)e.Parameter;
                feeds.OnNewsRefreshing += Feeds_OnNewsRefreshing;
                feeds.OnNewsRefreshed += Feeds_OnNewsRefreshed;
                feeds.NoNewNews += Feeds_NoNewNews;
                feeds.OnUpdateFailed += Feeds_OnUpdateFailed;
                titleTextBlock.Text = feeds.PaperTitle;
                LayoutNews();
            }
        }



        /// <summary>
        /// 刷新报纸后
        /// </summary>
        /// <param name="newsItems"></param>
        private void Feeds_OnNewsRefreshed(List<NewsItem> newsItems)
        {
            newsList = newsItems;
            newsViewItems = new NewsViewCollection(newsItems);
            Bindings.Update();
            refreshingProgressRing.IsActive = false;
        }

        /// <summary>
        /// 无报纸
        /// </summary>
        private void Feeds_NoNewNews()
        {
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
            else
            {
                MainPage.MainPageNavigationView.SelectedItem = null;
            } 
        }

        private void Feeds_OnUpdateFailed(string failNewsPaperTitle)
        {
            errorTextBlock.Text = "连接failNewsPaperTitle失败";
        }

        static public NewsPaper feeds { get;  set; }

        private List<NewsItem> newsList { get; set; } = new List<NewsItem>();
        private NewsViewCollection newsViewItems { set; get; } = new NewsViewCollection();

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
