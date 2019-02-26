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
    public sealed partial class AddPaperPage : Page
    {
        public AddPaperPage()
        {
            this.InitializeComponent();
        }

        private void AddFeedButton_Click(object sender, RoutedEventArgs e)
        {
            var rssInputBox = new TextBox();
            rssInputBox = new TextBox
            {
                Header = "RSS链接",
                Width = 400
            };
            rssInputPanel.Children.Add(rssInputBox);
            RelativePanel.SetAlignLeftWithPanel((UIElement)rssInputBox, true);
            RelativePanel.SetBelow(rssInputBox, rssInputPanel.Children[rssInputPanel.Children.Count - 2]);
        }

        private NewsPaper newsPaper;
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            newsPaper = new NewsPaper(newspaperTitleTextBox.Text);
            foreach(var element in rssInputPanel.Children)
            {
                try
                {
                    newsPaper.AddFeedLink(new Uri((element as TextBox).Text));
                }
                catch(Exception exception)
                {
                    continue;
                }
            }
            await NewsPaper.SaveToFile(newsPaper);
            //!!要等上一句完成，要用同步异步操作了。
            this.Frame.Navigate(typeof(PaperPage), newsPaper);
        }

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
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            await NewsPaper.DeleteNewsPaper(newsPaper);
        }
    }
}
