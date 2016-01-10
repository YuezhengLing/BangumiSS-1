using BangumiSS.Model;
using SatrokiLibrary.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using static BangumiSS.Model.BangumiModel;

namespace BangumiSS.ViewModel
{
    public class VM番组 : ViewModelBase
    {
        public VM番组(番组 Bgm = null)
        {
            OK = new DelegateCommand((o) => _OK());
            Go = new DelegateCommand((o) => _Go());
            Bangumi = Bgm ?? new 番组();
        }
        public Action Close;
        #region 属性
        private 番组 _Bangumi;
        public 番组 Bangumi
        {
            get { return _Bangumi; }
            set { SetProperty(ref _Bangumi, value); }
        }
        public bool EditMode { get; set; }
        public string Title => EditMode ? "编辑番组" : "添加番组";
        public string Code
        {
            get { return Bangumi.Bangumi编号; }
            set { Bangumi.Bangumi编号 = value; }
        }
        #endregion
        #region 命令
        public DelegateCommand OK { get; set; }
        public DelegateCommand Go { get; set; }

        private void _OK()
        {
            if (!EditMode)
                DbModel.番组.Add(Bangumi);
            DbModel.SaveChanges();
            Close?.Invoke();
            DialogResult = true;
        }
        private void _Go()
        {
            Bangumi = 解析信息(Code, Bangumi);
        }
        #endregion
        #region 方法
        public static 番组 解析信息(string code, 番组 bgm)
        {
            var url = "http://bangumi.tv/subject/" + code;
            try
            {
                WebClient client = new WebClient();
                byte[] data = client.DownloadData(url);
                string page = Encoding.UTF8.GetString(data);

                int s = page.IndexOf("<h1 class");
                int e = page.IndexOf("</h1>");
                string title = page.Substring(s, e - s);
                s = page.IndexOf("<ul id=\"infobox\">");
                e = page.IndexOf("</ul>", s);
                string info = page.Substring(s, e - s);

                string temp = Regex.Match(title, ">.+</a>").Value;
                bgm.原名 = temp.Substring(1, temp.Length - 5);

                var result = new string[6];
                string[] infos = new string[] { "中文名: ", "放送开始: ", "原作: ", "动画制作: ", "官方网站: ", "话数: " };

                for (int i = 0; i < 6; i++)
                {
                    temp = Regex.Match(info, infos[i] + ".+</li>").Value;
                    if (string.IsNullOrEmpty(temp))
                        continue;
                    result[i] = Regex.Replace(temp, "<.+?>", string.Empty).Remove(0, infos[i].Length);
                }
                bgm.译名 = result[0];
                result[1] = Regex.Replace(result[1], "年|月|日", "/").Trim('/');
                if (string.IsNullOrEmpty(bgm.版权链接))
                    bgm.首播 = DateTime.Parse(result[1]);
                bgm.原作 = result[2];
                bgm.动画制作 = result[3];
                bgm.官网 = result[4];
                int hs = 0;
                int.TryParse(result[5], out hs);
                bgm.话数 = hs;
                return bgm;
            }
            catch
            { return bgm; }
        }
        #endregion
    }
}
