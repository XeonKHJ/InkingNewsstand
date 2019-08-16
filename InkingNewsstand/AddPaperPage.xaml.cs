using InkingNewsstand.Classes;
using InkingNewsstand.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

namespace InkingNewsstand
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AddPaperPage : Page
    {
        public AddPaperPage()
        {
            this.InitializeComponent();
            searchingPageFlyout.Opened += SearchingPageFlyout_Opened;
            searchingPageFlyout.Closed += SearchingPageFlyout_Closed;
        }

        private void SearchingPageFlyout_Closed(object sender, object e)
        {
            isFeedsSearchingPageActive = false;
        }

        private void SearchingPageFlyout_Opened(object sender, object e)
        {
            isFeedsSearchingPageActive = true;
        }

        public static bool isFeedsSearchingPageActive = false;
        public static bool isThereFeed = false;
        public static FeedViewModel feedViewModel;

        

        private void AddFeedButton_Click(object sender, RoutedEventArgs e)
        {
            var rssInputBox = new TextBox();
            rssInputBox = new TextBox
            {
                Header = "RSS链接",
                Width = 400
            };
            if(sender is FeedViewModel)
            {
                rssInputBox.Text = ((FeedViewModel)sender).Url;
                if(rssInputBox.Text == "")
                {
                    return;
                }
            }
            rssInputPanel.Children.Add(rssInputBox);
            RelativePanel.SetAlignLeftWithPanel((UIElement)rssInputBox, true);
            RelativePanel.SetBelow(rssInputBox, rssInputPanel.Children[rssInputPanel.Children.Count - 2]);
        }

        private NewsPaper newsPaper;
        /// <summary>
        /// 保存按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //如果时编辑模式的话
            if(isEditMode)
            {
                List<Feed> editedFeeds = new List<Feed>();
                string oldTitle = newsPaper.PaperTitle;
                newsPaper.PaperTitle = newspaperTitleTextBox.Text;

                //!!
                //修改导航栏的标题
                foreach (var element in rssInputPanel.Children)
                {
                    try
                    {
                        var EditedFeed = new Feed(new Uri((element as TextBox).Text));
                        editedFeeds.Add(EditedFeed);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                //先筛选出原来就存在该报纸中的订阅源。
                var originalFeeds = newsPaper.Feeds.Union(editedFeeds).Intersect(editedFeeds).ToList();

                //从原来报纸的订阅源中，减去编辑后存在的订阅源，即为被删除的订阅源。
                var deletedFeeds = newsPaper.Feeds.Except(originalFeeds).ToList();

                //筛选出新加的订阅源。
                var newFeeds = newsPaper.Feeds.Except(originalFeeds).ToList();

                //删除相应的订阅源。
                newsPaper.RemoveFeeds(deletedFeeds);

                //添加相应的订阅源。
                newsPaper.AddFeeds(editedFeeds);

                //跳转到修改完的页面。
                this.Frame.Navigate(typeof(PaperPage), newsPaper);

                //触发OnPaperEdited事件。
                OnPaperEdited?.Invoke();
            }
            //添加新报纸模式
            else
            {
                //检测是否有新标题
                bool hasSameTitle = false;
                foreach(var paper in NewsPaper.NewsPapers)
                {
                    if(paper.PaperTitle == newspaperTitleTextBox.Text)
                    {
                        hasSameTitle = true;
                        break;
                    }
                }

                if (!hasSameTitle //如果没有同名的报纸
                    && newspaperTitleTextBox.Text != "") //并且如果名字不等与""
                {
                    newsPaper = new NewsPaper(newspaperTitleTextBox.Text);
                    NewsPaper.AddNewsPaper(newsPaper);
                    List<Feed> feedsToAdd = new List<Feed>();
                    foreach (var element in rssInputPanel.Children)
                    {
                        try
                        {
                            feedsToAdd.Add(new Feed(new Uri((element as TextBox).Text)));
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                    NewsPaper.AddNewsPaper(newsPaper);
                }
                else
                {
                    var dialog = new Windows.UI.Popups.MessageDialog("名字重复或为空");
                    await dialog.ShowAsync();
                    return;
                }
            }

            App.Invoke(() =>
            {
                PaperPage.thisPaperpage.RefreshNews();
            });
        }
        public delegate void OnPaperAddedHander();
        public static event OnPaperAddedHander OnPaperEdited;

        /// <summary>
        /// 跳转到该页面时发生的方法
        /// </summary>
        /// <param name="e">要编辑的报纸</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter is NewsPaper)
            {
                newsPaper = (NewsPaper)e.Parameter;
                newspaperTitleTextBox.Text = newsPaper.PaperTitle;
                foreach(var url in newsPaper.FeedUrls)
                {
                    ((TextBox)rssInputPanel.Children[rssInputPanel.Children.Count - 1]).Text = url.AbsoluteUri;
                    AddFeedButton_Click(null, null);
                }
                deleteButton.Visibility = Visibility.Visible;
                isEditMode = true;
            }
            else
            {
                isEditMode = false;
            }
        }

        private bool isEditMode = false;
        public void AddFeedLink(FeedViewModel feedViewModel)
        {
            AddFeedButton_Click(feedViewModel, null);
        }

        public Type FeedsSearchingPageType
        {
            get { return typeof(FeedsSearchingPage); }
        }

        /// <summary>
        /// 删除按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            NewsPaper.DeleteNewsPaper(newsPaper);
        }

        /// <summary>
        /// 从网页中获取按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetFromWebsiteButton_Click(object sender, RoutedEventArgs e)
        {
            isFeedsSearchingPageActive = true;
            AddFeedWatcher();
        }

        /// <summary>
        /// 异步实现从搜索中添加订阅源
        /// </summary>
        private async void AddFeedWatcher()
        {
            //isFeedsSearchingPageActive = true;
            await Task.Run(() =>
            {
                while (isFeedsSearchingPageActive)
                {
                    if (isThereFeed)
                    {
                        Invoke(()=> { AddFeedButton_Click(feedViewModel, null); }) ;
                        isThereFeed = false;
                    }
                }
            });
        }
    }
}
