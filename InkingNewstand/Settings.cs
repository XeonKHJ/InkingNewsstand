using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.UI.Xaml.Media;

namespace InkingNewstand
{
    static class Settings
    {
        public static FontFamily Font { set; get; } = new FontFamily("微软雅黑");
        public static double FontSize { set; get; } = 20;
        public static double LineSpacing { set; get; } = 10;
        public static double NewsWidth { set; get; } = 700;

        /// <summary>
        /// 保存设置
        /// </summary>
        async static public void SaveSettings()
        {
            var serializingSettings = new SerializedSetting
            {
                Font = Font.Source,
                FontSize = FontSize,
                LineSpacing = LineSpacing,
                NewsWidth = NewsWidth
            };
            StorageFile settingFile;
            var storageFolder = ApplicationData.Current.LocalFolder;
            string settingFileName = "Settings.xml";

            settingFile = await storageFolder.CreateFileAsync(settingFileName, CreationCollisionOption.ReplaceExisting);
            using (var stream = await settingFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                XmlSerializer mySerializer = new XmlSerializer(typeof(SerializedSetting));
                mySerializer.Serialize(stream.AsStream(), serializingSettings);
            }
        }

        /// <summary>
        /// 加载设置
        /// </summary>
        static async public void LoadSettings()
        {
            StorageFile settingFile;
            var storageFolder = ApplicationData.Current.LocalFolder;
            string settingFileName = "Settings.xml";
            SerializedSetting serializedSetting;
            settingFile = await storageFolder.CreateFileAsync(settingFileName, CreationCollisionOption.OpenIfExists);
            using (var stream = await settingFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                XmlSerializer mySerializer = new XmlSerializer(typeof(SerializedSetting));
                try
                {
                    serializedSetting = (SerializedSetting)mySerializer.Deserialize(stream.AsStream());
                    Font = new FontFamily(serializedSetting.Font);
                    FontSize = serializedSetting.FontSize;
                    LineSpacing = serializedSetting.LineSpacing;
                    NewsWidth = serializedSetting.NewsWidth;
                }
                catch(System.InvalidOperationException)
                {
                    System.Diagnostics.Debug.WriteLine("第一次打开啦");
                }
            }
        }
    }

    public class SerializedSetting
    {
        public string Font { set; get; } = "微软雅黑";
        public double FontSize { set; get; } = 20;
        public double LineSpacing { set; get; } = 10;
        public double NewsWidth { set; get; } = 20;
    }
}
