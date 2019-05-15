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
                }
                else
                {
                    var dialog = new Windows.UI.Popups.MessageDialog("名字重复或为空");
                    await dialog.ShowAsync();
                    return;
                }
            }
            PaperPage.thisPaperpage.RefreshNews();
            //页面跳转由PaperAdded事件发生，在MainPage中实现

            await NewsPaper.SaveToFile(newsPaper);
            //System.Diagnostics.Debug.WriteLine("Just for testing");
            //!!要等上一句完成，要用同步异步操作了。
            //this.Frame.Navigate(typeof(PaperPage), newsPaper);
        }
        public delegate void OnPaperAddedHander();
        public static event OnPaperAddedHander OnPaperAdded;

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
