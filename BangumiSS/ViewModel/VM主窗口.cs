using BangumiSS.Model;
using BangumiSS.Properties;
using SatrokiLibrary.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static BangumiSS.Model.BangumiModel;

namespace BangumiSS.ViewModel
{
    public class VM主窗口 : ViewModelBase
    {
        public VM主窗口()
        {
            AddProgress = new DelegateCommand((o) => _AddProgress());
            ScanFiles = new DelegateCommand((o) => _ScanFiles(o));
            WinClose = new DelegateCommand((o) => _WinClose());
            OpenFile = new DelegateCommand((o) => _OpenFile(o));
            LocateFile = new DelegateCommand((o) => _LocateFile());
            Edit = new DelegateCommand((o) => _Edit(o));
            CopyName = new DelegateCommand((o) => _CopyName());
            Search = new DelegateCommand((o) => _Search());
            ShowDetails = new DelegateCommand((o) => _ShowDetails());
            VisitBgm = new DelegateCommand((o) => _VisitBgm());
            VisitHP = new DelegateCommand((o) => _VisitHP());
            Manage = new DelegateCommand((o) => _Manage());
            Update = new DelegateCommand((o) => _Update());
            Finish = new DelegateCommand((o) => _Finish());
            UpdateInfo = new DelegateCommand((o) => _UpdateInfo());
            MusicInfo = new DelegateCommand((o) => _MusicInfo());

            ScanFiles.Execute(null);
        }

        #region 属性
        private ObservableCollection<资源> _AniList;
        public ObservableCollection<资源> AniList
        {
            get { return _AniList; }
            set { SetProperty(ref _AniList, value); }
        }

        private 资源 _Selected;
        public 资源 Selected
        {
            get { return _Selected; }
            set { SetProperty(ref _Selected, value); KeyWord = value?.关键词; }
        }

        private string _KeyWord;
        public string KeyWord
        {
            get { return _KeyWord; }
            set { SetProperty(ref _KeyWord, value); }
        }

        public double Top
        {
            get { return Settings.Default.启动Top; }
            set { Settings.Default.启动Top = value; }
        }

        public double Left
        {
            get { return Settings.Default.启动Left; }
            set { Settings.Default.启动Left = value; }
        }

        public double Width
        {
            get { return Settings.Default.Width; }
            set { Settings.Default.Width = value; }
        }
        public double Height
        {
            get { return Settings.Default.Height; }
            set { Settings.Default.Height = value; }
        }
        #endregion

        #region 命令
        public DelegateCommand AddProgress { get; set; }
        public DelegateCommand ScanFiles { get; set; }
        public DelegateCommand WinClose { get; set; }
        public DelegateCommand OpenFile { get; set; }
        public DelegateCommand LocateFile { get; set; }
        public DelegateCommand Edit { get; set; }
        public DelegateCommand CopyName { get; set; }
        public DelegateCommand Search { get; set; }
        public DelegateCommand ShowDetails { get; set; }
        public DelegateCommand VisitBgm { get; set; }
        public DelegateCommand VisitHP { get; set; }
        public DelegateCommand Manage { get; set; }
        public DelegateCommand Update { get; set; }
        public DelegateCommand Finish { get; set; }
        public DelegateCommand UpdateInfo { get; set; }
        public DelegateCommand MusicInfo { get; set; }
        private void _AddProgress()
        {
            if (Selected != null)
                Selected.进度++;
            DbModel.SaveChanges();
        }

        private async void _ScanFiles(dynamic o)
        {
            AniList = await Task.Run(() => CreateList(o));
        }

        private void _WinClose()
        {
            Settings.Default.Save();
        }

        private void _OpenFile(object o)
        {
            if (Selected != null)
            {
                Process.Start(Selected.路径 ?? Selected.目录);
            }
        }

        private void _LocateFile()
        {
            if (Selected != null)
            {
                ProcessStartInfo psi = new ProcessStartInfo("Explorer.exe");
                psi.Arguments = " /select," + Selected.路径;
                Process.Start(psi);
            }
        }

        private void _Edit(dynamic o)
        {
            if (Selected != null)
            {
                var vm = new VM新番();
                vm.Anime = Selected;
                vm.EditMode = true;
                var window = new View.W新番(vm);
                window.Owner = o;
                window.ShowDialog();
            }
        }

        private void _CopyName()
        {
            if (Selected != null)
                System.Windows.Clipboard.SetText(Selected.番组.译名);
        }

        private void _Search()
        {
            string url = Settings.Default.动漫花园搜索 + _KeyWord.Replace(' ', '+');
            Process.Start(url);
        }

        private void _ShowDetails()
        {
            if (Selected != null)
            {
                var vm = new VM管理(Selected.番组);
                var win = new View.W管理(vm);
                win.Show();
            }
        }

        private void _VisitBgm()
        {
            if (!string.IsNullOrEmpty(Selected.番组.Bangumi编号))
                Process.Start("http://bangumi.tv/subject/" + Selected.番组.Bangumi编号);
        }

        private void _VisitHP()
        {
            if (!string.IsNullOrEmpty(Selected.番组.官网))
                Process.Start(Selected.番组.官网);
        }

        private void _Manage()
        {
            var win = new View.W管理();
            win.Show();
        }

        private void _Update()
        {
            var vm = new VM更新(AniList);
            var win = new View.W更新(vm);
            win.Show();
            vm.Update.Execute(null);
        }

        private void _Finish()
        {
            if (Selected != null)
            {
                Selected.完结 = true;
                Selected.番组.完结 = true;
                if (!Selected.在线)
                    moveDirectory(Selected);
                DbModel.SaveChanges();
                AniList.Remove(Selected);
            }
        }

        private void _UpdateInfo()
        {
            if (Selected != null)
            {
                VM番组.解析信息(Selected.番组.Bangumi编号, Selected.番组);
                DbModel.SaveChanges();
            }
        }

        private void _MusicInfo()
        {
            var vm = new VM音乐();
            vm.List = new ObservableCollection<番组>(AniList.Select(a => a.番组));

            var win = new View.W音乐(vm);
            win.Show();
        }
        #endregion

        #region 方法
        private void moveDirectory(资源 res)
        {
            try
            {
                var path = Path.Combine(Settings.Default.完结目录, res.番组.档期);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                var di = new DirectoryInfo(res.目录);
                di.MoveTo(Path.Combine(path, di.Name));
            }
            catch
            { throw; }
        }

        public ObservableCollection<资源> CreateList(dynamic sender)
        {
            try
            {
                string[] strs = Settings.Default.路径格式.Split('|');
                var dirs = (new DirectoryInfo(strs[0])).GetDirectories().Where(
                    d => Regex.IsMatch(d.Name, strs[1])
                    ).ToArray();
                文件夹整理(dirs);
                string 扩展名 = Settings.Default.扩展名;

                var aniList = new List<资源>();

                foreach (var dir in dirs)
                {
                    var bgmDirs = dir.GetDirectories();
                    foreach (var bgm in bgmDirs)
                    {
                        var name = string分割(bgm.Name);
                        var mark = $"{dir}-{name[1]}";
                        var anime = DbModel.资源.SingleOrDefault(rs => rs.标识 == mark);
                        if (anime == null)
                        {
                            anime = new 资源()
                            {
                                字幕组 = name[0],
                                标识 = mark,
                                目录 = bgm.FullName
                            };

                            var vm = new VM新番()
                            {
                                Anime = anime,
                                EditMode = false
                            };
                            sender.Dispatcher.Invoke(new Action(() =>
                            {
                                var window = new View.W新番(vm);
                                window.ShowDialog();
                            }));
                            if (!vm.DialogResult)
                                continue;
                        }
                        if (anime.完结)
                            continue;
                        if (anime.目录 != bgm.FullName)
                        {
                            anime.目录 = bgm.FullName;
                            DbModel.SaveChanges();
                        }
                        anime.计数 = -1;
                        var files = bgm.GetFiles().OrderBy(f => f.CreationTime);
                        double temp = 0;
                        foreach (var file in files)
                        {
                            if (file.Extension == ".torrent")
                            {
                                file.Delete();
                                continue;
                            }
                            if (!扩展名.Contains(file.Extension.ToLower()))
                                continue;
                            var info = string分割(file.Name);
                            if (info.Length < 3)
                                continue;
                            var macth = Regex.Match(info[2], @"\d{1,3}(\.5)?");
                            if (!macth.Success && info.Length > 3)
                                macth = Regex.Match(info[3], @"\d{1,3}(\.5)?");
                            if (macth.Success)
                            {
                                temp = double.Parse(macth.Value);
                                if (temp > anime.计数)
                                {
                                    anime.计数 = temp;
                                    anime.完成时间 = file.CreationTime;
                                    anime.路径 = file.FullName;
                                }
                            }
                        }
                        aniList.Add(anime);
                    }
                }

                var online = DbModel.资源.Where(a => !a.完结 && a.在线).ToList();
                if (online.Count > 0)
                    aniList.AddRange(online.Select(a => 在线更新(a)));
                aniList.AddRange(onlineBgms());
                DbModel.SaveChanges();

                return new ObservableCollection<资源>(aniList.OrderByDescending(a => a.完成时间));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        private IEnumerable<资源> onlineBgms()
        {
            var bgms = DbModel.番组.Where(b => !b.完结 && b.资源.Count == 0 && !string.IsNullOrEmpty(b.版权链接));
            foreach (var b in bgms)
            {
                var anime = new 资源()
                {
                    在线 = true,
                    完成时间 = b.首播,
                    目录 = b.版权链接,
                    计数 = 1,
                    进度 = 0,
                    番组 = b,
                };
                anime.版权字幕组();
                在线更新(anime);
                App.MainDispatcher.Invoke(() => DbModel.资源.Add(anime));
                yield return anime;
            }
        }

        private 资源 在线更新(资源 a)
        {
            var week = (int)((DateTime.Now - a.完成时间).TotalDays / 7);
            if (week < 1)
                return a;
            a.计数 += week;
            if (a.番组.话数 > 0 && a.计数 > a.番组.话数)
                a.计数 = a.番组.话数;
            a.完成时间 = a.完成时间.AddDays(7 * week);
            return a;
        }

        private void 文件夹整理(DirectoryInfo[] dirs)
        {
            try
            {
                foreach (var dir in dirs)
                {
                    var files = dir.GetFiles();
                    foreach (var f in files)
                    {
                        try
                        {
                            var ext = f.Extension;
                            if (ext == ".torrent")
                            {
                                File.Delete(f.FullName);
                                continue;
                            }
                            if (ext == ".td")
                                continue;
                            string[] temp = string分割(f.Name);
                            StringBuilder sb = new StringBuilder();
                            sb.Append(dir.FullName).Append("\\[").Append(temp[0]).Append("][").Append(temp[1]).Append(']');
                            string dirName = sb.ToString();
                            if (!Directory.Exists(dirName))
                            {
                                Directory.CreateDirectory(dirName);
                            }
                            f.MoveTo(Path.Combine(dirName, f.Name));
                        }
                        catch (IOException)
                        { }
                    }
                }
            }
            catch (Exception)
            { throw; }
        }

        private string[] string分割(string str)
        {
            var chars = new[] { '[', ']' };
            if (Regex.IsMatch(str, @"\[Mabors Sub\].* - \d*\["))
                chars = new[] { '[', ']', '-' };
            return str.Split(chars).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
        }
        #endregion
    }
}
