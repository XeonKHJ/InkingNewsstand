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
        private NewsPaper newsPaper;
        public NewsPaperViewModel(NewsPaper _newsPaper)
        {
            newsPaper = _newsPaper;
        }

        public string Title
        {
            get { return newsPaper.PaperTitle; }
        }

        public string Icon
        {
            get { return newsPaper.IconType; }
        }

    }
}
