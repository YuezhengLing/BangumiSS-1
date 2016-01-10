using BangumiSS.Model;
using SatrokiLibrary.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using static BangumiSS.Model.BangumiModel;

namespace BangumiSS.ViewModel
{
    public class VM新番 : ViewModelBase
    {
        public VM新番()
        {
            OK = new DelegateCommand((o) => _OK());
        }
        public Action Close;
        #region 属性
        public List<番组> BgmList { get; } = DbModel.番组.OrderByDescending(b => b.BId).ToList();
        public 资源 Anime { get; set; }
        public bool EditMode { get; set; }
        public string Title => EditMode ? "编辑" : "添加";
        public 番组 SelectedBgm { get; set; }
        #endregion
        #region 命令
        public DelegateCommand OK { get; set; }

        private void _OK()
        {
            if (!EditMode && SelectedBgm != null)
                SelectedBgm.资源.Add(Anime);
            DbModel.SaveChanges();
            Close();
            DialogResult = true;
        }
        #endregion
        #region 方法

        #endregion
    }
}
