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
using Windows.Web.Syndication;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace InkingNewstand
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            MainPageNavigationView = paperNavigationView;
            InitializePaperlistSetting();
            GetNewsPapersAtBeginning();
        }

        public async void Invoke(Action action, Windows.UI.Core.CoreDispatcherPriority Priority = Windows.UI.Core.CoreDispatcherPriority.Normal)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Priority, () => { action(); });
        }


        private object oldSelectedItem; //前一个选中的导航选项
        internal static bool NavigationEnabled = true; //是否可以导航
        internal static NavigationView MainPageNavigationView;
        /// <summary>
        /// 导航栏选项选中后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void PaperNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if(!NavigationEnabled)
            {
                NavigationEnabled = true;
            }
            else if (args.IsSettingsSelected)
            {
                //contentFrame.Navigate(typeof(SettingPage));//跳转到设置页面
                ShowSettingFlyout(sender);
                return;
            }
            else if (args.SelectedItem is NewsPaper selectedItem)
            {
                if(PaperPage.thisPaperpage != null)
                {
                    PaperPage.thisPaperpage.NavigationCacheMode = NavigationCacheMode.Disabled;
                }
                contentFrame.Navigate(typeof(PaperPage), selectedItem);
            }
            oldSelectedItem = args.SelectedItem;
        }

        private void ShowSettingFlyout(FrameworkElement frameworkElement)
        {
            Flyout flyout = new Flyout();
            flyout.Closed += Flyout_Closed;
            var frame = new Frame();
            flyout.Content = frame;
            frame.Navigate(typeof(SettingPage));
            flyout.ShowAt(FavoritesButton);
            
        }

        private void Flyout_Closed(object sender, object e)
        {
            NavigationEnabled = false;
            paperNavigationView.SelectedItem = oldSelectedItem;
        }

        /// <summary>
        /// 初始化设置
        /// </summary>
        private void InitializePaperlistSetting()
        {
            NewsPaper.OnPaperAdded += NewsPaper_OnPaperAdded;
            NewsPaper.OnPaperDeleted += NewsPaper_OnPaperDeleted;
            NewsPaper.OnPaperDeleting += NewsPaper_OnPaperDeleting;
        }

        private void NewsPaper_OnPaperDeleting(NewsPaper updatedNewspaper)
        {
            if(newsPapers.Count == 1)
            {
                newsPapers.Add(new NewsPaper("创建你的第一份报纸！"));
            }
        }

        private void NewsPaper_OnPaperDeleted(NewsPaper newsPaper)
        {
            newsPapers.Remove(newsPaper);
            paperNavigationView.SelectedItem = newsPapers.First();
        }

        private async void GetNewsPapersAtBeginning()
        {
            await NewsPaper.ReadFromFile();
            if(NewsPaper.NewsPapers.Count == 0)
            {
                newsPapers.Add(new NewsPaper("创建你的第一份报纸！"));
            }
            //Bindings.Update();
            //if(NewsPaper.NewsPapers.Add)
            foreach(var paper in NewsPaper.NewsPapers)
            {
                newsPapers.Add(paper);
            }

            paperNavigationView.SelectedItem = newsPapers.First();
        }

        private void NewsPaper_OnPaperAdded(NewsPaper updatedNewspaper)
        {
            if (PaperPage.thisPaperpage != null)
            {
                PaperPage.thisPaperpage.NavigationCacheMode = NavigationCacheMode.Disabled;
            }
            if (newsPapers.Count == 1 && newsPapers.First().PaperTitle == "创建你的第一份报纸！")
            {
                newsPapers.Clear();
            }
            newsPapers.Add(updatedNewspaper);
            if (NewsPaper.NewsPapers.Count != 0)
            {
                paperNavigationView.SelectedItem = updatedNewspaper;
            }
        }

        ObservableCollection<NewsPaper> newsPapers { get; set; } = new ObservableCollection<NewsPaper>();

        public bool IsNavigatedByBackButton = false;
        private void PaperNavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (contentFrame.CanGoBack)
            {
                contentFrame.GoBack();
            }
        }

        public delegate void CleanPaperPageEventHandler();
        public static event CleanPaperPageEventHandler CleanPaperPage;

        private void FavoritesButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            paperNavigationView.SelectedItem = null;
            contentFrame.Navigate(typeof(FavoritesPage));
        }
    }
}
