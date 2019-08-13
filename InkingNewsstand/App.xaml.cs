using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Runtime.Serialization.Formatters.Binary;
using Windows.Storage;
using Windows.Storage.Streams;
using InkingNewsstand.Models;
using InkingNewsstand.Utilities;

namespace InkingNewsstand
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            //初始化数据库
            DataOperator.MigrateData();
        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            //自定义开始
            GetFavoritesFromFile();

            //加载设置
            Settings.LoadSettings();

            Frame rootFrame = Window.Current.Content as Frame;

            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 从之前挂起的应用程序加载状态
                }

                // 将框架放在当前窗口中
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // 当导航堆栈尚未还原时，导航到第一页，
                    // 并通过将所需信息作为导航参数传入来配置
                    // 参数
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // 确保当前窗口处于活动状态
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SaveFavoritesToFile();
            NewsPaper.SaveAll();
            Settings.SaveSettings();
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }

        private bool On_BackRequested()
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
                return true;
            }
            return false;
        }

        /// <summary>
        ///  ///byte数组转换成object
        /// </summary>
        /// <param name="arrBytes"></param>
        /// <returns>字节数组</returns>
        public static Object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, System.IO.SeekOrigin.Begin);
                Object obj;
                try
                {
                    obj = binForm.Deserialize(memStream);
                }
                catch (System.Runtime.Serialization.SerializationException)
                {
                    obj = null;
                }
                return obj;
            }
        }
        /// <summary>
        /// object转换成byte数组
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Object</returns>
        public static byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                try
                {
                    bf.Serialize(ms, obj);
                }
                catch (System.Runtime.Serialization.SerializationException)
                {
                    ;
                }
                return ms.ToArray();
            }
        }

        static private StorageFile favoriteListFile;
        /// <summary>
        /// 从文件获取收藏项
        /// </summary>
        public async void GetFavoritesFromFile()
        {
            var storageFolder = ApplicationData.Current.LocalFolder;

            //0、打开文件
            string favoirtesFileName = "Favorites.dat";

            try
            {
                favoriteListFile = await storageFolder.CreateFileAsync(favoirtesFileName, CreationCollisionOption.OpenIfExists);
            }
            catch (System.IO.FileLoadException)
            {
                ;
            }

            var stream = await favoriteListFile.OpenAsync(FileAccessMode.ReadWrite); //获取文件随机存取流
            using (var inputStream = stream.GetInputStreamAt(0))//获取从0开始的输入流
            {
                var dataReader = new DataReader(inputStream); //在该输入流中附着一个数据读取器
                uint loadBytes = await dataReader.LoadAsync((uint)stream.Size); //加载数据数据到中间缓冲区
                byte[] bytes = new byte[(uint)stream.Size];
                dataReader.ReadBytes(bytes);//用来存储从文件中读出的数据
                Favorites = (List<FavoriteModel>)App.ByteArrayToObject(bytes); //将读出的数据转换成SortedDictionary<int, NewsPaper>
            }
            stream.Dispose();
            if (Favorites == null)
            {
                Favorites = new List<FavoriteModel>();
            }
        }

        public async static void SaveFavoritesToFile()
        {
            await FileIO.WriteBytesAsync(favoriteListFile, App.ObjectToByteArray(Favorites));
        }



        static public List<FavoriteModel> Favorites { get; set; } = new List<FavoriteModel>(); //收藏报纸列表
    }
}
