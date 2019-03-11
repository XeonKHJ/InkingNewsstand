using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using InkingNewstand.Utilities;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace InkingNewstand
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NewsDetailPage : Page
    {
        public NewsDetailPage()
        {
            this.InitializeComponent();
        }

        NewsItem News { set; get; }
        List<Windows.Web.Syndication.SyndicationLink> Links;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HtmlConverter.OnReadingHtmlConvertCompleted += HtmlConverter_OnReadingHtmlConvertCompleted;
            if(!(e.Parameter is NewsItem))
            {
                throw new Exception();
            }
            News = (NewsItem)(e.Parameter);
            GetReadingHtml(News.NewsLink);
            if (News.CoverUrl == "")
            {
                CoverUrlforPage = "Nopic.jpg";
            }
            else
            {
                CoverUrlforPage = News.CoverUrl;
            }
            
        }

        private void HtmlConverter_OnReadingHtmlConvertCompleted(string html)
        {
            Html = html;
            Bindings.Update();
        }

        private async void GetReadingHtml(Uri url)
        {
            await HtmlConverter.ExtractReadableContent(url);
        }

        public Double WindowHeight
        {
            get
            {
                return ApplicationView.GetForCurrentView().VisibleBounds.Height;
            }
        }

        public Double WindowsWidth
        {
            get
            {
                return ApplicationView.GetForCurrentView().VisibleBounds.Width;
            }
        }

        public string Html { get; set; }

        string CoverUrlforPage { set; get; }
    }
}
