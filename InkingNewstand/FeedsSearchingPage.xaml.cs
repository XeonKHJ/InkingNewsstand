﻿using System;
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
using InkingNewstand.Utilities;
using Windows.Web.Syndication;
using InkingNewstand.ViewModels;
using System.Threading.Tasks;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace InkingNewstand
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class FeedsSearchingPage : Page
    {
        public FeedsSearchingPage()
        {
            Feeds.Add(new FeedViewModel("aaaaaaaaaaa", "d", "c", "d"));
            AddPaperPage.isFeedsSearchingPageActive = true;
            AddPaperPage.isThereFeed = false;
            this.InitializeComponent();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            List<Uri> feedUrls = new List<Uri>();
            feedUrls = FeedsFinder.GetFeedsFromUrl(new Uri(websiteTextBox.Text));
            var client = new SyndicationClient();
            var feeds = new List<FeedViewModel>();
            foreach (var url in feedUrls)
            {
                var feed = await client.RetrieveFeedAsync(url);
                feeds.Add(new FeedViewModel(feed));
            }
            Feeds = feeds;
            Bindings.Update();
        }

        public List<FeedViewModel> Feeds = new List<FeedViewModel>();

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
