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
using ReadSharp;

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
            Links = new List<Windows.Web.Syndication.SyndicationLink>();
            if(!(e.Parameter is NewsItem))
            {
                throw new Exception();
            }
            News = (NewsItem)(e.Parameter);
            Links.AddRange(News.Item.Links);
            //if(News.Item.Content != null)
            //{
            //    content = News.Item.Content.Text;
            //}
            //else
            //{
            //    content = News.Item.Summary.Text;
            //}
            //Html = content;
            GetReadingHtml(News.NewsLinks);


            //this.Frame.LayoutUpdated += Frame_LayoutUpdated;
            //foreach (var link1 in news.Item.Links)
            //{
            //    ;
            //}
            //var link = news.Item.Links[0];
            //newsWebView.Source = news.Item.Links[0].Uri;
            
        }

        private async void GetReadingHtml(Uri url)
        {
            Reader reader = new Reader();
            Article article;

            try
            {
                article = await reader.Read(url);
                Html = article.Content;
                Bindings.Update();
            }
            catch (ReadException exc)
            {
                // handle exception
            }
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
    }
}
