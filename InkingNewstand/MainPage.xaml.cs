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
using Windows.Web.Syndication;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace InkingNewstand
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        public async void FeedSync(string rssUrl)
        {
            var feed = await new SyndicationClient().RetrieveFeedAsync(new Uri(rssUrl));
            var feedXml = feed.GetXmlDocument(feed.SourceFormat);
            var items = feed.Items;
            NewsPaper mixedFeeds = new NewsPaper("THE VERGE");
            mixedFeeds.AddFeed(feed);
            //传送items到Paper页面
            this.Frame.Navigate(typeof(Paper), mixedFeeds);

            //if(items != null)
            //{
            //    foreach(var item in items)
            //    {
            //        var links = item.Links;
            //        foreach(var link in links)
            //        {
            //            string a = link.Uri.AbsoluteUri;
            //        }
            //        Invoke(() => { textBlock_msg.Text += item.Summary.Text; });
            //    }
            //}
            //else
            //{
            //    Invoke(() => { textBlock_msg.Text = "fucked"; });
            //}
        }


        public async void Invoke(Action action, Windows.UI.Core.CoreDispatcherPriority Priority = Windows.UI.Core.CoreDispatcherPriority.Normal)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Priority, () => { action(); });
        }

        private void FeedButton_Click(object sender, RoutedEventArgs e)
        {
            FeedSync(rssTextBlock.Text);
        }

        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if(args.IsSettingsSelected)
            {
               //设置页面
            }
            else
            {
                var selectedItem = (NavigationViewItem)args.SelectedItem;
                if(selectedItem != null)
                {
                    //string selectedItemTag = ((string)selectedItem.Tag);
                    contentFrame.Navigate(Paper);
                }
            }
        }
    }
}
