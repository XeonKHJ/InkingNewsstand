using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.UI.Xaml.Media;

namespace InkingNewsstand
{
    static class Settings
    {
        public static FontFamily Font { set; get; } = new FontFamily("LeeLawadee UI");
        public static double FontSize { set; get; } = 20;
        public static double LineSpacing { set; get; } = 10;
        public static double NewsWidth { set; get; } = 900;
        public static bool BindingNewsWidthwithFrame { set; get;} = true;
        public static List<string> ExtendedFeeds { set; get; } = new List<string>();
        /// <summary>
        /// 保存设置
        /// </summary>
        static public void SaveSettings()
        {

            //本地设置项
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["LineSpacing"] = LineSpacing;
            localSettings.Values["NewsWidth"] = NewsWidth;
            localSettings.Values["BindingNewsWidthwithFrame"] = BindingNewsWidthwithFrame;
            var extendedFeedsContainer = localSettings.CreateContainer("ExtendedFeeds", ApplicationDataCreateDisposition.Always).Values;
            extendedFeedsContainer.Clear();
            foreach(var extendedFeed in ExtendedFeeds)
            {
                extendedFeedsContainer[extendedFeed] = extendedFeed;
            }

            //漫游设置项
            var roamingSettings = ApplicationData.Current.RoamingSettings;
            roamingSettings.Values["Font"] = Font.Source;
            roamingSettings.Values["FontSize"] = FontSize;
            roamingSettings.Values["LineSpacing"] = LineSpacing;
        }

        /// <summary>
        /// 加载设置
        /// </summary>
        static public void LoadSettings()
        {
            //本地设置项
            try
            {
                var localSettings = ApplicationData.Current.LocalSettings;
                LineSpacing = (double)localSettings.Values["LineSpacing"];
                NewsWidth = (double)localSettings.Values["NewsWidth"];
                BindingNewsWidthwithFrame = (bool)localSettings.Values["BindingNewsWidthwithFrame"];
                var extendedFeedsContainer = localSettings.CreateContainer("ExtendedFeeds", ApplicationDataCreateDisposition.Always).Values;
                foreach (var extendedFeedId in extendedFeedsContainer)
                {
                    ExtendedFeeds.Add(extendedFeedId.Key);
                }

                //漫游设置项
                var roamingSettings = ApplicationData.Current.RoamingSettings;
                Font = new FontFamily((string)roamingSettings.Values["Font"]);
                FontSize = (double)roamingSettings.Values["FontSize"];
                LineSpacing = (double)roamingSettings.Values["LineSpacing"];
            }
            catch (NullReferenceException)
            {
                System.Diagnostics.Debug.WriteLine("第一次打开");
                IsFirstTime = true;
            }
            catch (ArgumentNullException)
            {
                System.Diagnostics.Debug.WriteLine("第一次打开");
                IsFirstTime = true;
            }
        }
        public static bool IsFirstTime { get; set; } = false;
    }

    public class SerializedSetting
    {
        public string Font { set; get; } = "微软雅黑";
        public double FontSize { set; get; } = 20;
        public double LineSpacing { set; get; } = 10;
        public double NewsWidth { set; get; } = 700;
        public static List<string> ExtendedFeeds { set; get; } = new List<string>();
    }
}
