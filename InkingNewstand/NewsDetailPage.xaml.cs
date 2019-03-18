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
using Windows.UI.Input.Inking.Core;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Storage.Pickers;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using Windows.UI.Input.Inking;

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
            //var inkSouce = CoreInkIndependentInputSource.Create(newsCanvas.InkPresenter);
            //inkSouce.PointerEntering += InkSouce_PointerEntering;
            //inkSouce.PointerLost += InkSouce_PointerReleasing;
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

            //修改收藏图标
            if (News.IsFavorite)
            {
                favoriteButton.Icon = new SymbolIcon(Symbol.UnFavorite);
            }
            else
            {
                favoriteButton.Icon = new SymbolIcon(Symbol.Favorite);
            }

            GetReadingHtml(News.NewsLink);
            if (News.CoverUrl == "")
            {
                CoverUrlforPage = "Nopic.jpg";
            }
            else
            {
                CoverUrlforPage = News.CoverUrl;
            }
            LoadInkStrokes(News.InkStrokes);
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


        NewsPaper newsPaper = null;
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            newsPaper = NewsPaper.NewsPapers.Find((NewsPaper paper) => paper.PaperTitle == News.PaperTitle);

            //to-do
            //保存笔迹
            var currentStrokes = newsCanvas.InkPresenter.StrokeContainer.GetStrokes();

            byte[] serializedStrokes = await SerializeStrokes(currentStrokes);
            News.InkStrokes = serializedStrokes;
            newsPaper.UpdateNewsList(News);
            await NewsPaper.SaveToFile(newsPaper);
        }

        /// <summary>
        /// 序列化笔迹
        /// </summary>
        /// <param name="strokes">笔迹列表</param>
        /// <returns>序列化后的字节数组</returns>
        private async Task<byte[]> SerializeStrokes(IReadOnlyCollection<InkStroke> strokes)
        {
            byte[] bytes = null;
            
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                //先把笔迹输出到输出流
                using (var outputStream = stream.GetOutputStreamAt(0))
                {
                    await newsCanvas.InkPresenter.StrokeContainer.SaveAsync(outputStream);
                }

                //从输入流中读出序列化结果
                using (var inputStream = stream.GetInputStreamAt(0))
                {
                    var dataReader = new DataReader(inputStream); //在该输入流中附着一个数据读取器
                    uint loadBytes = await dataReader.LoadAsync((uint)stream.Size); //加载数据数据到中间缓冲区
                    bytes = new byte[stream.Size];
                    dataReader.ReadBytes(bytes);
                }
            }
            return bytes;
        }

        /// <summary>
        /// 加载笔迹
        /// </summary>
        /// <param name="serializedStrokes">序列化后的笔迹的字节数组</param>
        private async void LoadInkStrokes(byte[] serializedStrokes)
        {
            if (serializedStrokes.Count() > 0)
            {
                using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    using (var outputStream = stream.GetOutputStreamAt(0))
                    {
                        DataWriter writer = new DataWriter(outputStream);
                        writer.WriteBytes(serializedStrokes);
                        await writer.StoreAsync();
                    }
                    using (var inputStream = stream.GetInputStreamAt(0))
                    {
                        await newsCanvas.InkPresenter.StrokeContainer.LoadAsync(inputStream);
                    }
                }
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            News.IsFavorite = !News.IsFavorite; 
            if(News.IsFavorite)
            {
                favoriteButton.Icon = new SymbolIcon(Symbol.UnFavorite);
                App.Favorites.Add(new Models.FavoriteModel(News));
            }
            else
            {
                App.Favorites.Remove(new Models.FavoriteModel(News));
                favoriteButton.Icon = new SymbolIcon(Symbol.Favorite);
            }
            //newsPaper = NewsPaper.NewsPapers.Find((NewsPaper paper) => paper.PaperTitle == News.PaperTitle);
        }
    }
}
