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
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Printing;
using Windows.Graphics.Printing;
using RichTextControls;
using InkingNewstand.Translate;
using Windows.UI.Xaml.Documents;

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
            HtmlConverter.OnReadingHtmlConvertCompleted -= HtmlConverter_OnReadingHtmlConvertCompleted;
            HtmlConverter.OnReadingHtmlConvertCompleted += HtmlConverter_OnReadingHtmlConvertCompleted;
            SettingPage.ValueChanged += SettingPage_ValueChanged; ;
            if (!(e.Parameter is NewsItem))
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

            //修改扩展模式图标
            if (Settings.ExtendedFeeds.Contains(News.Feed.Id))
            {
                extendButton.Icon = new SymbolIcon(Symbol.DockRight);
                isExtend = true;
            }
            else
            {
                isExtend = false;
                extendButton.Icon = new SymbolIcon(Symbol.DockLeft);
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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            SettingPage.ValueChanged -= SettingPage_ValueChanged;
        }

        private void SettingPage_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Bindings.Update();
        }

        private void HtmlConverter_OnReadingHtmlConvertCompleted(string html)
        {
            Properties.SetHtml(htmlBlock, html);
            try
            {
                if (new Uri(News.CoverUrl) == new Uri(HtmlConverter.GetFirstImages(html)))
                {
                    headerImg.Visibility = Visibility.Collapsed;
                }
                else
                {
                    headerImg.Visibility = Visibility.Visible;
                }
            }
            catch(UriFormatException exception)
            {
                System.Diagnostics.Debug.WriteLine(exception.Message);
            }
        }

        private bool isExtend = false;

        /// <summary>
        /// 获取文章HTML
        /// </summary>
        /// <param name="url"></param>
        private async void GetReadingHtml(Uri url)
        {
            if (isExtend)
            {
                try
                {
                    await HtmlConverter.ExtractReadableContent(url);
                }
                catch (ReadSharp.ReadException readException)
                {
                    System.Diagnostics.Debug.WriteLine(readException.Message);
                }
            }
            else
            {
                HtmlConverter_OnReadingHtmlConvertCompleted(News.Content);
            }
        }

        public double WindowHeight
        {
            get
            {
                return ApplicationView.GetForCurrentView().VisibleBounds.Height;
            }
        }

        public double WindowsWidth
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

            //保存笔迹
            var currentStrokes = newsCanvas.InkPresenter.StrokeContainer.GetStrokes(); //获取笔迹
            byte[] serializedStrokes = await SerializeStrokes(currentStrokes); //序列化笔迹
            News.InkStrokes = serializedStrokes; //将笔迹保存到新闻实例里
            newsPaper.UpdateNewsList(News);
            await NewsPaper.SaveToFile(newsPaper); //保存报纸
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
            //// Initalize common helper class and register for printing
            //var printHelper = new PrintHelper(this);
            //printHelper.RegisterForPrinting();

            //// Initialize print content for this scenario
            //printHelper.PreparePrintContent(new NewsDetailPage());
        }

        private void PrintMan_PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            PrintTask printTask = null;
            //printTask = args.Request.CreatePrintTask(News.NewsLink); //to-do
        }

        private void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            News.IsFavorite = !News.IsFavorite;
            if (News.IsFavorite)
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

        private async void OpenInBroswerButton_Click(object sender, RoutedEventArgs e)
        {
            var success = await Windows.System.Launcher.LaunchUriAsync(News.NewsLink);
        }

        private async void OpenInEdgeReadingModeButton_Click(object sender, RoutedEventArgs e)
        {
            var options = new Windows.System.LauncherOptions
            {
                TargetApplicationPackageFamilyName = "Microsoft.MicrosoftEdge_8wekyb3d8bbwe"
            };

            var readingModeUriString = "read:" + News.NewsLink.AbsoluteUri;

            // Launch the URI
            var success = await Windows.System.Launcher.LaunchUriAsync(new Uri(readingModeUriString), options);
        }

        private void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;

            DataTransferManager.ShowShareUI();
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.SetWebLink(News.NewsLink);
            request.Data.Properties.Title = News.Title;
            request.Data.Properties.Description = "该新闻的链接";
        }

        private void ExtendButton_Click(object sender, RoutedEventArgs e)
        {
            if (isExtend)
            {
                extendButton.Icon = new SymbolIcon(Symbol.DockLeft);
                Settings.ExtendedFeeds.Remove(News.Feed.Id);
                isExtend = false;
            }
            else
            {
                Settings.ExtendedFeeds.Add(News.Feed.Id);
                extendButton.Icon = new SymbolIcon(Symbol.DockRight);
                isExtend = true;
            }
            Settings.SaveSettings();
            GetReadingHtml(News.NewsLink);
        }

        private int selectionCount = 0;
        private TextPointer selectionEnd;
        bool translationFlyoutIsNullorClosed = false;
        
        private void HtmlBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if(selectionCount == 0)
            {
                DetectSelectionFinished();
            }
            ++selectionCount;

            selectionEnd = htmlBlock.SelectionEnd;
            translationFlyoutIsNullorClosed = (translationFlyout == null) ? true : !translationFlyout.IsOpen;

            System.Diagnostics.Debug.WriteLine(selectionCount.ToString());
            WordPicker wordPicker = new WordPicker();
            var result = wordPicker.Lookfor(htmlBlock.SelectedText, Language_t.en);
        }

        private Flyout translationFlyout;
        WordPicker wordPicker = new WordPicker();
        private async void DetectSelectionFinished()
        {
            await Task.Run(async () =>
            {
                bool flyoutShowed = false;
                while (true)
                {
                    var oldSelectionCount = selectionCount;
                    var oldSelectionEnd = selectionEnd;
                    await Task.Delay(100);
                    //bool translationFlyoutIsNullorClosed = false;
                    var newSelectionEnd = selectionEnd;
                    System.Diagnostics.Debug.WriteLine(translationFlyoutIsNullorClosed.ToString());
                    if (translationFlyoutIsNullorClosed) //如果translationFlyout被初始化了但没有开着
                    {
                        if (selectionCount == oldSelectionCount)
                        {
                            selectionCount = 0;
                            if (!flyoutShowed //是否已经显示了Flyout
                                || ((oldSelectionEnd != null && newSelectionEnd != null) && oldSelectionEnd != newSelectionEnd)) //是否是重复选择
                            {

                                Invoke(() =>
                                {
                                    var selectedText = htmlBlock.SelectedText;
                                    if (selectedText != "" && selectedText != null)
                                    {
                                        try
                                        {
                                            var translatedResult = wordPicker.Lookfor(selectedText, Language_t.en);
                                            translationFlyout = new Flyout
                                            {
                                                Content = new TextBlock() { Text = translatedResult.GetResult(Language_t.zh), IsTextSelectionEnabled = true },
                                            };
                                        }
                                        catch(Exception exception)
                                        {
                                            translationFlyout = new Flyout
                                            {
                                                Content = new TextBlock() { Text = "翻译出错", IsTextSelectionEnabled = true },
                                            };
                                        }
                                        FlyoutShowOptions flyoutShowOptions = new FlyoutShowOptions
                                        {
                                            Position = new Point(newSelectionEnd.GetCharacterRect(LogicalDirection.Forward).X, newSelectionEnd.GetCharacterRect(LogicalDirection.Forward).Y),
                                            ShowMode = FlyoutShowMode.Transient
                                        };
                                        try
                                        {
                                            translationFlyout.ShowAt(htmlBlock, flyoutShowOptions);
                                        }
                                        catch(ArgumentException exception)
                                        {
                                            System.Diagnostics.Debug.WriteLine(exception.Message);
                                        }
                                        flyoutShowed = true;
                                    }
                                });
                            }
                            System.Diagnostics.Debug.WriteLine("break!");
                            break;
                        }
                    }
                }
            });
        }

        private Point GetPointerPosition()
        {
            var pointerPosition = Windows.UI.Core.CoreWindow.GetForCurrentThread().PointerPosition;

            if(pointerPosition.X > Window.Current.Bounds.Right)
            {
                pointerPosition.X = Window.Current.Bounds.Right;
            }
            else if(pointerPosition.X < Window.Current.Bounds.Left)
            {
                pointerPosition.X = Window.Current.Bounds.Left;
            }

            if(pointerPosition.Y > Window.Current.Bounds.Bottom)
            {
                pointerPosition.Y = Window.Current.Bounds.Bottom;
            }
            else if(pointerPosition.Y < Window.Current.Bounds.Top)
            {
                pointerPosition.Y = Window.Current.Bounds.Top;
            }


            var x = pointerPosition.X - Window.Current.Bounds.X;
            var y = pointerPosition.Y - Window.Current.Bounds.Y;


            var ttv = newsDetailPageGrid.TransformToVisual(Window.Current.Content);
            Point screenCoords = ttv.TransformPoint(new Point(0, 0));

            x -= screenCoords.X;
            y -= screenCoords.Y + 10;

            return new Point(x, y);
        }

        public async void Invoke(Action action, Windows.UI.Core.CoreDispatcherPriority Priority = Windows.UI.Core.CoreDispatcherPriority.Normal)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Priority, () => { action(); });
        }
    }
}
