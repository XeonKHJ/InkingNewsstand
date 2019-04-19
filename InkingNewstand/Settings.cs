using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace InkingNewstand
{
    static class Settings
    {
        public static FontFamily Font { set; get; } = new FontFamily("微软雅黑");
        public static double FontSize { set; get; } = 20;
        public static int LineSpacing { set; get; } = 10;
        public static int NewsWidth { set; get; } = 20;
    }
}
