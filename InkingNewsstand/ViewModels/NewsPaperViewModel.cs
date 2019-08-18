using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InkingNewsstand.Classes;

namespace InkingNewsstand.ViewModels
{
    public class NewsPaperViewModel
    {
        public NewsPaper NewsPaper { set; get; }
        public NewsPaperViewModel(NewsPaper newsPaper)
        {
            NewsPaper = newsPaper;
        }

        public NewsPaperViewModel(string title)
        {
            NewsPaper = new NewsPaper(Title);
        }

        public string Title
        {
            get { return NewsPaper.PaperTitle; }
        }

        public string IconType
        {
            get { return NewsPaper.IconType; }
        }
    }
}
