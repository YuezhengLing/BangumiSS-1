namespace BangumiSS.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Windows.Media;

    public partial class 资源 : SatrokiLibrary.MVVM.ViewModelBase
    {
        [Key]
        public int DId { get; set; }

        public int BId { get; set; }

        private string _字幕组;
        [StringLength(256)]
        public string 字幕组
        {
            get { return _字幕组; }
            set { SetProperty(ref _字幕组, value); }
        }

        private string _标识;
        [StringLength(256)]
        public string 标识
        {
            get { return _标识; }
            set { SetProperty(ref _标识, value); }
        }

        private string _关键词;
        [StringLength(256)]
        public string 关键词
        {
            get { return _关键词; }
            set { SetProperty(ref _关键词, value); }
        }

        [StringLength(256)]
        public string 目录 { get; set; }

        private int _进度;
        public int 进度
        {
            get { return _进度; }
            set { SetProperty(ref _进度, value); OnPropertyChanged(nameof(状态)); }
        }

        public bool 完结 { get; set; }

        public bool 在线 { get; set; }

        public virtual 番组 番组 { get; set; }

        private double? _计数;
        public double? 计数
        {
            get { return _计数; }
            set { SetProperty(ref _计数, value); OnPropertyChanged(nameof(状态)); }
        }
        [NotMapped]
        public string 路径 { get; set; }

        public DateTime 完成时间 { get; set; } = new DateTime(2000, 1, 1);
        [NotMapped]
        public Brush 状态
        {
            get
            {
                if (计数 > 进度)
                    return Brushes.SeaGreen;
                if ((DateTime.Now - 完成时间).TotalDays >= 7)
                    return Brushes.IndianRed;
                return Brushes.Transparent;
            }
        }

        public void 版权字幕组()
        {
            var url = 番组?.版权链接;
            if (在线 && !string.IsNullOrEmpty(url))
            {
                if (url.Contains("bilibili"))
                    字幕组 = "哔哩哔哩";
                else if (url.Contains("tudou") || url.Contains("youku"))
                    字幕组 = "优土豆";
                else if (url.Contains("iqiyi"))
                    字幕组 = "爱奇艺";
                else if (url.Contains("letv"))
                    字幕组 = "乐视";
            }
        }
    }
}
