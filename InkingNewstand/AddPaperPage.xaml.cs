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
            rssInputBox = new TextBox();
            rssInputBox.Header = "RSS链接";
            rssInputBox.Width = 400;
            rssInputPanel.Children.Add(rssInputBox);
            RelativePanel.SetAlignLeftWithPanel((UIElement)rssInputBox, true);
            RelativePanel.SetBelow(rssInputBox, rssInputPanel.Children[rssInputPanel.Children.Count - 2]);
        }

        NewsPaper newsPaper;
        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            newsPaper = new NewsPaper(newspaperTitleTextBox.Text);
            foreach(var element in rssInputPanel.Children)
            {
                if ((element as TextBox).Text == "")
                {
                    continue;
                }
                await newsPaper.AddFeedLink(new Uri((element as TextBox).Text));
            }

            //!!要等上一句完成，要用同步异步操作了。
            this.Frame.Navigate(typeof(PaperPage), newsPaper);
        }
    }
}
