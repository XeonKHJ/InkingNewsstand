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
    public sealed partial class Paper : Page
    {
        public Paper()
        {
            this.InitializeComponent();
        }

        private void layoutNews(MixedFeeds feeds)
        {
            var items = feeds.Items;
            titleTextBlock.Text = feeds.PaperTitle;
            int no = 0;
            //foreach(var item in items)
            //{
            //    var newsBlock = new RelativePanel();
            //    var newsTitleTextBlock = new TextBlock();
            //    newsTitleTextBlock.Text = item.Title.Text;
            //    newsTitleTextBlock.FontSize = 50;
            //    contentPanel.Children.Add(newsTitleTextBlock);
            //    RelativePanel.SetAlignRightWithPanel(newsTitleTextBlock, true);
            //    RelativePanel.SetAlignLeftWithPanel(newsTitleTextBlock, true);
            //    if (no == 0)
            //    {
            //        RelativePanel.SetAlignTopWithPanel(newsTitleTextBlock, true);
            //    }
            //    else
            //    {
            //        RelativePanel.SetBelow(newsTitleTextBlock, contentPanel.Children[contentPanel.Children.Count - 2]);
            //    }
            //    ++no;
            //}
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!(e.Parameter is MixedFeeds))
            {
                throw new Exception();
            }
            feeds = (MixedFeeds)e.Parameter;
            layoutNews(feeds);
        }

        public MixedFeeds feeds { get;  set; }

        List<NewsItem> row1NewsItemList
        {
            get
            {
                return feeds.Items.GetRange(0, 2);
            }
        }
        
        List<NewsItem> row2NewsItemList
        {
            get
            {
                return feeds.Items.GetRange(2, 2);
            }
        }
        List<NewsItem> row3NewsItemList
        {
            get
            {
                return feeds.Items.GetRange(6, 2);
            }
        }
        List<NewsItem> restNewsItemList
        {
            get
            {
                return feeds.Items.GetRange(8, feeds.Items.Count - 8);
            }
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(NewsDetail), e.ClickedItem);
        }
    }
}
