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
using InkingNewsstand.Utilities;
using Windows.Web.Syndication;
using InkingNewsstand.ViewModels;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Threading;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace InkingNewsstand
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class FeedsSearchingPage : Page
    {
        public FeedsSearchingPage()
        {
            Feeds.Add(new FeedViewModel("请输入要你需要订阅的网站", "", "", "NoPic"));
            AddPaperPage.isFeedsSearchingPageActive = true;
            AddPaperPage.isThereFeed = false;
            this.InitializeComponent();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            searchingProgressRing.IsActive = true;
            Feeds.Clear();
            List<Uri> feedUrls = new List<Uri>();
            //Semaphore semaphore = new Semaphore(0, feedUrls.Count);
            try
            {
                feedUrls = await FeedsFinder.SearchFromFeedly(websiteTextBox.Text);
                var client = new SyndicationClient();
                List<FeedViewModel> feeds = new List<FeedViewModel>();
                
                Parallel.ForEach(feedUrls, async (url, loopstat) =>
                {
                    try
                    {
                        var feed = await client.RetrieveFeedAsync(url);
                        lock(feeds)
                        {
                            Invoke(() =>
                            {
                                Feeds.Add(new FeedViewModel(feed, url.AbsoluteUri));
                            });
                            System.Diagnostics.Debug.WriteLine("循环内");
                        }
                    }
                    catch (Exception exception)
                    {
                        System.Diagnostics.Debug.WriteLine(exception.Message);
                    }
                });
            }
            catch (Exception)
            {
                Feeds.Add(new FeedViewModel("无结果或链接错误", "", "", "Nopic"));
            }
            System.Diagnostics.Debug.WriteLine("循环外");
            Invoke(() => searchingProgressRing.IsActive = false);
        }

        ObservableCollection<FeedViewModel> Feeds { set; get; } = new ObservableCollection<FeedViewModel>();

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            AddPaperPage.feedViewModel = (FeedViewModel)e.ClickedItem;
            AddPaperPage.isThereFeed = true;
        }

        public async void Invoke(Action action, Windows.UI.Core.CoreDispatcherPriority Priority = Windows.UI.Core.CoreDispatcherPriority.Normal)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Priority, () => { action(); });
        }
    }
}
