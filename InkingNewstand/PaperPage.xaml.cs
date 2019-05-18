using InkingNewstand.Models;
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
        }
        public static PaperPage thisPaperpage;
        private void MainPage_CleanPaperPage()
        {
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        private void LayoutNews()
        {
            Feeds_OnNewsRefreshed(paper.NewsList);
        }

        public async void RefreshNews()
        {
            await paper.GetNewsListAsync();
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

                paper = (NewsPaper)e.Parameter;
                paper.OnNewsRefreshing += Feeds_OnNewsRefreshing;
                paper.OnNewsRefreshed += Feeds_OnNewsRefreshed;
                paper.NoNewNews += Feeds_NoNewNews;
                paper.OnUpdateFailed += Feeds_OnUpdateFailed;

                //设置报纸标题
                titleTextBlock.Text = paper.PaperTitle;

                //显示新闻
                LayoutNews();
            }
        }

        /// <summary>
        /// 刷新报纸后显示新闻
        /// </summary>
        /// <param name="newsItems"></param>
        private void Feeds_OnNewsRefreshed(List<NewsItem> newsItems)
        {
            //设置订阅源选择菜单弹窗项
            if (feedsChooseMenuFlyout.Items.Count != paper.Feeds.Count) //消除缓存带来的影响
            {
                foreach (var feed in paper.Feeds)
                {
                    ToggleMenuFlyoutItem toggleMenuFlyoutItem = new ToggleMenuFlyoutItem
                    {
                        Text = feed.Title,
                        IsChecked = true,
                        Tag = feed.Id
                    };
                    toggleMenuFlyoutItem.Click += ToggleMenuFlyoutItem_Click;
                    feedsChooseMenuFlyout.Items.Add(toggleMenuFlyoutItem);
                }
            }

            var selectedFeedTitle = (from toggleItem in feedsChooseMenuFlyout.Items
                                     where (toggleItem is ToggleMenuFlyoutItem && ((ToggleMenuFlyoutItem)toggleItem).IsChecked)
                                     select ((ToggleMenuFlyoutItem)toggleItem).Text);
            var selectedNews = (from newsItem in paper.NewsList
                                where selectedFeedTitle.Contains(newsItem.Feed.Title)
                                    && (allNewsButton.IsChecked == true || selectedDates.Contains(newsItem.PublishedDate.Date))
                                select newsItem);
            newsList = selectedNews.ToList();
            newsViewItems = new NewsViewCollection(newsList);
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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.SourcePageType == typeof(PaperPage))
            {
                this.NavigationCacheMode = NavigationCacheMode.Disabled; //关闭页面缓存
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

        static public NewsPaper paper { get;  set; }

        private List<NewsItem> newsList { get; set; } = new List<NewsItem>();
        private NewsViewCollection newsViewItems { set; get; } = new NewsViewCollection();

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(NewsDetailPage), e.ClickedItem);
        }

        private void AddPaperButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AddPaperPage));
        }

        private void EditPaperButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AddPaperPage), PaperPage.paper);
        }

        private void RefreshPaperButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshNews();
        }

        private IList<DateTimeOffset> selectedDates; 

        /// <summary>
        /// 日期改变时，筛选出相应日期的新闻
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void PaperDatePicker_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            selectedDates = sender.SelectedDates;
            Feeds_OnNewsRefreshed(paper.NewsList);
        }

        /// <summary>
        /// 筛选出相应订阅源的新闻
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Feeds_OnNewsRefreshed(paper.NewsList);
        }

        private void AllNewsButton_Checked(object sender, RoutedEventArgs e)
        {
            if (paperDatePicker == null)
            {
                return;
            }
            paperDatePicker.Visibility = Visibility.Collapsed;
            Feeds_OnNewsRefreshed(paper.NewsList);
        }

        private void AllNewsButton_Unchecked(object sender, RoutedEventArgs e)
        {
            paperDatePicker.Visibility = Visibility.Visible;
        }
    }
}
