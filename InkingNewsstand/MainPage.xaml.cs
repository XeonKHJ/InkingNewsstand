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
using InkingNewsstand.Classes;
using InkingNewsstand.ViewModels;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace InkingNewsstand
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage ThisPage;
        public MainPage()
        {
            this.InitializeComponent();
            ThisPage = this;
            MainPageNavigationView = paperNavigationView;
            AddPaperPage.OnPaperEdited += AddPaperPage_OnPaperEdited;
            InitializePaperlistSetting();
            GetNewsPapersAtBeginning();
        }

        private void AddPaperPage_OnPaperEdited()
        {
            paperNavigationView.MenuItemsSource = null;
            Bindings.Update();
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
            else if (args.SelectedItem is NewsPaperViewModel selectedItem)
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
        }

        private void NewsPaper_OnPaperDeleted(NewsPaper newsPaper)
        {
            paperNavigationView.SelectedItem = NewsPapersViewCollection.First();
        }

        private List<NewsPaper> newsPapers = new List<NewsPaper>();

        /// <summary>
        /// 在刚启动时获取报纸列表。
        /// </summary>
        private void GetNewsPapersAtBeginning()
        {
            newsPapers = NewsPaper.NewsPapers;

            if(NewsPaper.NewsPapers.Count == 0)
            {
                newsPapers.Add(new NewsPaper("创建你的第一份报纸！"));
            }

            foreach(var newsPaper in newsPapers)
            {
                NewsPapersViewCollection.Add(new NewsPaperViewModel(newsPaper));
            }

            paperNavigationView.SelectedItem = NewsPapersViewCollection.First();
        }

        private void NewsPaper_OnPaperAdded(NewsPaper updatedNewspaper)
        {
            if (PaperPage.thisPaperpage != null)
            {
                PaperPage.thisPaperpage.NavigationCacheMode = NavigationCacheMode.Disabled;
            }

            if (NewsPapersViewCollection.Count == 1 && NewsPapersViewCollection.First().Title == "创建你的第一份报纸！")
            {
                NewsPapersViewCollection.Clear();
            }

            var updatedNewsPaperViewModel = new NewsPaperViewModel(updatedNewspaper);
            
            NewsPapersViewCollection.Add(updatedNewsPaperViewModel);

            if (NewsPaper.NewsPapers.Count != 0)
            {
                paperNavigationView.SelectedItem = updatedNewsPaperViewModel;
            }
        }

        private ObservableCollection<NewsPaperViewModel> NewsPapersViewCollection { set; get; } = new ObservableCollection<NewsPaperViewModel>();

        public bool IsNavigatedByBackButton = false;
        private void PaperNavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (contentFrame.CanGoBack)
            {
                contentFrame.GoBack();
            }
        }

        /// <summary>
        /// 收藏新闻列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FavoritesButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            paperNavigationView.SelectedItem = null;
            contentFrame.Navigate(typeof(FavoritesPage));
        }

        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            contentFrame.Navigate(typeof(AddPaperPage));
        }
    }
}
