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
        }

        private async void layoutNews(NewsPaper feeds)
        {
            try
            {
                newsItems = await feeds.GetNewsListAsync();
            }
            catch(Exception exception)
            {
                //同步失败信息
            }
            //titleTextBlock.Text = feeds.PaperTitle;
            Bindings.Update();
            int no = 0;
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
                feeds.OnUpdateFailed += Feeds_OnUpdateFailed;
                titleTextBlock.Text = feeds.PaperTitle;
                layoutNews(feeds);
            }
        }

        private void Feeds_OnUpdateFailed(string failNewsPaperTitle)
        {
            errorTextBlock.Text = "连接failNewsPaperTitle失败";
        }

        private void Feeds_OnNewsRefreshed()
        {
            refreshingProgressRing.IsActive = false;
        }

        static public NewsPaper feeds { get;  set; }

        List<NewsItem> newsItems;

        List<NewsItem> row1NewsItemList
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
