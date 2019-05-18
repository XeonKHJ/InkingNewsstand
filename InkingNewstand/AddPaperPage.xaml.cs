using InkingNewstand.Models;
using InkingNewstand.ViewModels;
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

namespace InkingNewstand
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
            if(isEditMode)
            {
                List<Uri> editedUris = new List<Uri>();
                string oldTitle = newsPaper.PaperTitle;
                newsPaper.PaperTitle = newspaperTitleTextBox.Text;

                //修改导航栏的标题
                foreach (var element in rssInputPanel.Children)
                {
                    try
                    {
                        var editedFeedUri = new Uri((element as TextBox).Text);
                        editedUris.Add(editedFeedUri);
                    }
                    catch (Exception exception)
                    {
                        continue;
                    }
                }
                var editedUrisEnumerables = newsPaper.FeedUrls.Union(editedUris).Intersect(editedUris);
                var deletedUrisEnumerables = newsPaper.FeedUrls.Except(editedUrisEnumerables);
                List<NewsItem> deletedNews = new List<NewsItem>();
                foreach(var news in newsPaper.NewsList)
                {
                    if(deletedUrisEnumerables.Contains(new Uri(news.Feed.Id)))
                    {
                        if(news.IsFavorite)
                        {
                            App.Favorites.Remove(new FavoriteModel(news));
                        }
                        deletedNews.Add(news);
                    }
                }

                //删除相应的订阅源
                var deletedFeedModels = from feedModel in newsPaper.Feeds
                                        where deletedUrisEnumerables.Contains(new Uri(feedModel.Id))
                                        select feedModel;
                var remainedFeedModel = newsPaper.Feeds.Except(deletedFeedModels);
                newsPaper.Feeds = remainedFeedModel.ToList();

                var remainedNewsEnumerables = newsPaper.NewsList.Except(deletedNews);
                newsPaper.NewsList = new List<NewsItem>(remainedNewsEnumerables);
                newsPaper.FeedUrls = new List<Uri>(editedUrisEnumerables);
                foreach(var favNewsModel in App.Favorites)
                {
                    if(favNewsModel.NewsPaperTitle == oldTitle)
                    {
                        favNewsModel.NewsPaperTitle = newsPaper.PaperTitle;
                    }
                }
                this.Frame.Navigate(typeof(PaperPage), newsPaper);
                OnPaperEdited?.Invoke();
                NewsPaper.SaveAll();
            }
            else
            {
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
                    foreach (var element in rssInputPanel.Children)
                    {
                        try
                        {
                            newsPaper.AddFeedLink(new Uri((element as TextBox).Text));
                        }
                        catch (Exception exception)
                        {
                            continue;
                        }
                    }
                    NewsPaper.AddNewsPaper(newsPaper);
                    await NewsPaper.SaveToFile(newsPaper);
                }
                else
                {
                    var dialog = new Windows.UI.Popups.MessageDialog("名字重复或为空");
                    await dialog.ShowAsync();
                    return;
                }
            }

            Invoke(() =>
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

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            await NewsPaper.DeleteNewsPaper(newsPaper);
        }

        private void GetFromWebsiteButton_Click(object sender, RoutedEventArgs e)
        {
            isFeedsSearchingPageActive = true;
            AddFeedWatcher();
        }

        /// <summary>
        /// 异步实现从搜索中提娜佳订阅源
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

        public async void Invoke(Action action, Windows.UI.Core.CoreDispatcherPriority Priority = Windows.UI.Core.CoreDispatcherPriority.Normal)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Priority, () => { action(); });
        }
    }
}
