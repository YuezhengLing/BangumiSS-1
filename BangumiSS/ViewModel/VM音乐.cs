using BangumiSS.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BangumiSS.ViewModel
{
    public class VM音乐 : SatrokiLibrary.MVVM.ViewModelBase
    {
        public ObservableCollection<番组> List { get; set; }

        public void Save()
        {
            BangumiModel.DbModel.SaveChanges();
        }
    }
}
