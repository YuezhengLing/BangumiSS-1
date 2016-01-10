using BangumiSS.Model;
using SatrokiLibrary.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using static BangumiSS.Model.BangumiModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using BangumiSS.Properties;
using System.Linq.Expressions;

namespace BangumiSS.ViewModel
{
    public class VM管理 : ViewModelBase
    {
        public VM管理(番组 bgm = null)
        {
            Search = new DelegateCommand((o) => _Search());
            VisitHP = new DelegateCommand((o) => _VisitHP());
            VisitBgm = new DelegateCommand((o) => _VisitBgm());
            EditBgm = new DelegateCommand((o) => _EditBgm(o));
            EditAnime = new DelegateCommand((o) => _EditAnime(o));
            OpenAnime = new DelegateCommand((o) => _OpenAnime());
            AddBgm = new DelegateCommand((o) => _AddBgm(o));
            DelBgm = new DelegateCommand((o) => _DelBgm());
            DelAnime = new DelegateCommand((o) => _DelAnime());
            WinClose = new DelegateCommand((o) => _WinClose());
            DropBgm = new DelegateCommand((o) => _DropBgm(o));
            UpdateInfo = new DelegateCommand((o) => _UpdateInfo());
            MusicInfo = new DelegateCommand((o) => _MusicInfo());

            if (bgm != null)
            {
                BgmList = new ObservableCollection<番组>() { bgm };
                SelectedBangumi = bgm;
            }
        }
        #region 属性
        public string SearchKey { get; set; }
        public string[] SearchMode { get; } = new[] { "档期", "星期", "译名", "原名", "原作", "动画制作", "官网", "Bangumi编号" };
        public int Index { get; set; } = Settings.Default.上次搜索;
        private 番组 _SelectedBangumi;
        public 番组 SelectedBangumi
        {
            get { return _SelectedBangumi; }
            set { SetProperty(ref _SelectedBangumi, value); }
        }
        private 资源 _SelectedAnime;
        public 资源 SelectedAnime
        {
            get { return _SelectedAnime; }
            set { SetProperty(ref _SelectedAnime, value); }
        }

        private ObservableCollection<番组> _BgmList;
        public ObservableCollection<番组> BgmList
        {
            get { return _BgmList; }
            set { SetProperty(ref _BgmList, value); }
        }
        #endregion
        #region 命令
        public DelegateCommand Search { get; set; }
        public DelegateCommand VisitHP { get; set; }
        public DelegateCommand VisitBgm { get; set; }
        public DelegateCommand EditBgm { get; set; }
        public DelegateCommand EditAnime { get; set; }
        public DelegateCommand OpenAnime { get; set; }
        public DelegateCommand AddBgm { get; set; }
        public DelegateCommand DelBgm { get; set; }
        public DelegateCommand DelAnime { get; set; }
        public DelegateCommand WinClose { get; set; }
        public DelegateCommand DragOver { get; set; }
        public DelegateCommand DropBgm { get; set; }
        public DelegateCommand UpdateInfo { get; set; }
        public DelegateCommand MusicInfo { get; set; }

        private void _WinClose()
        {
            Settings.Default.上次搜索 = Index;
            Settings.Default.Save();
        }

        private void _Search()
        {
            if (string.IsNullOrEmpty(SearchKey))
                BgmList = new ObservableCollection<番组>(DbModel.番组.ToList());
            else
                BgmList = SearchBgm(SearchKey, SearchMode[Index]);
        }

        private void _VisitHP()
        {
            if (!String.IsNullOrEmpty(SelectedBangumi.官网))
                Process.Start(SelectedBangumi.官网);
        }

        private void _VisitBgm()
        {
            if (!string.IsNullOrEmpty(SelectedBangumi.Bangumi编号))
                Process.Start("http://bangumi.tv/subject/" + SelectedBangumi.Bangumi编号);
        }

        private void _EditBgm(dynamic o)
        {
            if (SelectedBangumi != null)
            {
                var vm = new VM番组(SelectedBangumi);
                vm.EditMode = true;
                var window = new View.W番组(vm);
                window.Owner = o;
                window.ShowDialog();
            }
        }

        private void _EditAnime(dynamic o)
        {
            if (SelectedAnime != null)
            {
                var vm = new VM新番();
                vm.Anime = SelectedAnime;
                vm.EditMode = true;
                var win = new View.W新番(vm);
                win.Owner = o;
                win.ShowDialog();
            }
        }

        private void _OpenAnime()
        {
            if (SelectedAnime != null)
                if (!string.IsNullOrEmpty(SelectedAnime.目录))
                    Process.Start(SelectedAnime.目录);
        }

        private void _AddBgm(dynamic o)
        {
            var win = new View.W番组(null);
            win.Owner = o;
            win.ShowDialog();
        }

        private void _DelBgm()
        {
            if (SelectedBangumi != null)
            {
                DbModel.番组.Remove(SelectedBangumi);
                BgmList.Remove(SelectedBangumi);
                DbModel.SaveChanges();
            }
        }

        private void _DelAnime()
        {
            if (SelectedAnime != null)
            {
                DbModel.资源.Remove(SelectedAnime);
                SelectedBangumi.资源.Remove(SelectedAnime);
                DbModel.SaveChanges();
            }
        }

        private void _DropBgm(object o)
        {
            var code = (string)o;
            var bgm = VM番组.解析信息(code, new 番组());
            bgm.Bangumi编号 = code;
            DbModel.番组.Add(bgm);
            DbModel.SaveChanges();
            BgmList.Add(bgm);
        }

        private void _UpdateInfo()
        {
            if (SelectedBangumi != null)
            {
                VM番组.解析信息(SelectedBangumi.Bangumi编号, SelectedBangumi);
                DbModel.SaveChanges();
            }
        }

        private void _MusicInfo()
        {
            var vm = new VM音乐();
            vm.List = new ObservableCollection<番组>(BgmList);

            var win = new View.W音乐(vm);
            win.Show();
        }
        #endregion
        #region 方法
        public ObservableCollection<番组> SearchBgm(string key, string p)
        {
            var typeB = typeof(番组);
            var left = Expression.Parameter(typeB, "b");
            var expr = Expression.Call(
                Expression.Property(left, typeB.GetProperty(p)),
                typeof(string).GetMethod("Contains", new Type[] { typeof(string) }),
                Expression.Constant(key)
                );
            var finalExpre = Expression.Lambda<Func<番组, bool>>(expr, new ParameterExpression[] { left });
            var list = DbModel.番组.Where(finalExpre).ToList();
            return new ObservableCollection<番组>(list);
        }
        #endregion
    }
}
