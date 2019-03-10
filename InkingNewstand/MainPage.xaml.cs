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
            InitializePaperlistSetting();
            GetNewsPapersAtBeginning();
        }

        public async void Invoke(Action action, Windows.UI.Core.CoreDispatcherPriority Priority = Windows.UI.Core.CoreDispatcherPriority.Normal)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Priority, () => { action(); });
        }

        /// <summary>
        /// 导航栏选项选中后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void PaperNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                contentFrame.Navigate(typeof(SettingPage));//跳转到设置页面
            }
            else
            {
                var selectedItem = (NewsPaper)args.SelectedItem;
                if (selectedItem.Count == 0)
                {
                    contentFrame.Navigate(typeof(AddPaperPage));
                }
                contentFrame.Navigate(typeof(PaperPage), selectedItem);
            }
        }

        private void InitializePaperlistSetting()
        {
            //NewsPaper.OnPaperAdding += NewsPaper_OnPaperAdding;
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

        /// <summary>
        /// 更新报纸列表
        /// </summary>
        /// <param name="newsPaper">更新完后要显示的报纸</param>
        //private async void GetNewsPapers(NewsPaper newsPaper)
        //{
        //    newsPapers = await NewsPaper.ReadFromFile();
        //    if (newsPapers.Count == 0)
        //    {
        //        newsPapers.Add(new NewsPaper("添加第一份报纸！"));
        //    }
        //    Bindings.Update();
        //    if (newsPapers.Count != 0)
        //    {
        //        paperNavigationView.SelectedItem = newsPapers[0];
        //        //contentFrame.Navigate(typeof(PaperPage), newsPapers[0]);
        //    }
        //}

        private void NewsPaper_OnPaperAdded(NewsPaper updatedNewspaper)
        {
            if(newsPapers.Count == 1 && newsPapers.First().PaperTitle == "创建你的第一份报纸！")
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

        private void PaperNavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (contentFrame.CanGoBack)
            {
                contentFrame.GoBack();
            }
        }
    }
}
