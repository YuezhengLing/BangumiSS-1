using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SatrokiLibrary.MVVM;
using System.Collections.ObjectModel;
using STuple = System.Tuple<string, string>;
using System.Collections;
using BangumiSS.Model;
using BangumiSS.Properties;
using System.Xml;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;

namespace BangumiSS.ViewModel
{
    public class VM更新 : ViewModelBase
    {
        public VM更新(IEnumerable<资源> aniList)
        {
            Update = new DelegateCommand((o) => _Update());
            Download = new DelegateCommand((o) => _Download());
            Add = new DelegateCommand((o) => _Add());
            Delete = new DelegateCommand((o) => _Delete());
            this.aniList = aniList;
        }

        private IEnumerable<资源> aniList;

        #region 属性
        public IList SelectedInAll { get; set; }
        private ObservableCollection<STuple> _AllItems;
        public ObservableCollection<STuple> AllItems
        {
            get { return _AllItems; }
            set { SetProperty(ref _AllItems, value); }
        }

        public IList SelectedInSelected { get; set; }
        private ObservableCollection<STuple> _SelectedItems;
        public ObservableCollection<STuple> SelectedItems
        {
            get { return _SelectedItems; }
            set { SetProperty(ref _SelectedItems, value); }
        }

        private bool _IsEnabled = true;
        public bool IsButtonEnabled
        {
            get { return _IsEnabled; }
            set { SetProperty(ref _IsEnabled, value); }
        }

        private string _Message;
        public string Message
        {
            get { return _Message; }
            set { SetProperty(ref _Message, value); }
        }
        #endregion

        #region 命令
        public DelegateCommand Update { get; set; }
        public DelegateCommand Download { get; set; }
        public DelegateCommand Add { get; set; }
        public DelegateCommand Delete { get; set; }

        private async void _Update()
        {
            IsButtonEnabled = false;
            try
            {
                AllItems = new ObservableCollection<Tuple<string, string>>();
                SelectedItems = new ObservableCollection<Tuple<string, string>>();
                var rss = Settings.Default.动漫花园RSS;
                XmlDocument doc = new XmlDocument();
                Message = "正在获取RSS……";
                await Task.Run(() =>
                    doc.LoadXml(SatrokiLibrary.Web.StaticMethod.DownloadHtmlPage(rss, Encoding.UTF8, "GET", 30000)));
                Message = "获取完成，开始扫描……";
                var list = doc.GetElementsByTagName("item");
                var str = 生成匹配(aniList);
                扫描项目(list, str);
                Settings.Default.Reload();
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            finally
            {
                IsButtonEnabled = true;
            }
        }

        private void _Download()
        {
            IsButtonEnabled = false;
            try
            {
                while (SelectedItems.Count > 0)
                {
                    string file = SelectedItems[0].Item2.Substring(0, 52);
                    Process.Start(file);
                    SelectedItems.RemoveAt(0);
                    if (SelectedItems.Count > 0)
                        Thread.Sleep(2000);
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            finally
            {
                IsButtonEnabled = true;
            }
        }

        private void _Add()
        {
            if (SelectedInAll != null)
                foreach (STuple item in SelectedInAll)
                    SelectedItems.Add(item);
        }

        private void _Delete()
        {
            if (SelectedInSelected != null)
                foreach (var item in SelectedInSelected.Cast<STuple>().ToArray())
                    SelectedItems.Remove(item);
        }
        #endregion

        #region 方法
        private string 生成匹配(IEnumerable<资源> aniList)
        {
            if (aniList == null)
                return Settings.Default.新番匹配;
            var sb = new StringBuilder();
            foreach (var a in aniList)
            {
                if (!string.IsNullOrEmpty(a.关键词))
                {
                    var keys = a.关键词.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    sb.Append("((.*)");
                    foreach (var s in keys)
                        sb.Append(s).Append("(.*)");
                    sb.Append(")|");
                }
            }
            var result = sb.ToString().TrimEnd('|');
            Settings.Default.新番匹配 = result;
            Settings.Default.Save();
            return result;
        }

        private void 扫描项目(XmlNodeList list, string pattern)
        {
            var lastTime = Settings.Default.上次更新;
            Settings.Default.上次更新 = Convert.ToDateTime(((XmlElement)list[0]).GetElementsByTagName("pubDate")[0].InnerText);
            Settings.Default.Save();
            var temp = DateTime.Now;
            foreach (XmlNode node in list)
            {
                XmlElement ele = (XmlElement)node;
                string title = ele.GetElementsByTagName("title")[0].InnerText;
                string url = ele.GetElementsByTagName("enclosure")[0].Attributes["url"].Value;
                DateTime time = DateTime.Parse(ele.GetElementsByTagName("pubDate")[0].InnerText);
                temp = time;
                if (time <= lastTime)
                {
                    Message = "扫描已达上次更新时间：" + temp.ToString();
                    return;
                }
                AllItems.Add(new STuple(title, url));
                if (Regex.IsMatch(title, pattern, RegexOptions.IgnoreCase))
                    SelectedItems.Add(new STuple(title, url));
            }
            Message = "扫描完成。起始时间：" + temp.ToString();
        }
        #endregion
    }
}
