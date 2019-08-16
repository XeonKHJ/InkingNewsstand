using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using InkingNewsstand.Classes;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace InkingNewsstand
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class FavoritesPage : Page
    {
        public FavoritesPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //通过收藏列表找到对应新闻
            GetNewsFromNewsList();
        }

        private void GetNewsFromNewsList()
        {
            foreach (var favNewsModel in App.Favorites)
            {
                FavoriteNews.Add(
                    (NewsPaper.NewsPapers.Find((NewsPaper paper) => paper.PaperTitle == favNewsModel.NewsPaperTitle))
                    .News.Find((News newsItem) => newsItem.GetHashCode() == favNewsModel.HashCode)
                    );
            }
        }

        private ObservableCollection<News> FavoriteNews { set; get; } = new ObservableCollection<News>();

        private void NewsItemsGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(NewsDetailPage), e.ClickedItem);
        }
    }
}
