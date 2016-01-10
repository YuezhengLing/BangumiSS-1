namespace BangumiSS.Model
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class 番组 : SatrokiLibrary.MVVM.ViewModelBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public 番组()
        {
            资源 = new ObservableCollection<资源>();
        }

        [Key]
        public int BId { get; set; }

        private string _档期;
        [Required]
        [StringLength(6)]
        public string 档期
        {
            get { return _档期; }
            set { SetProperty(ref _档期, value); }
        }

        private DateTime _首播;
        [Column(TypeName = "datetime")]
        public DateTime 首播
        {
            get { return _首播; }
            set
            {
                if (_首播 != value)
                {
                    _首播 = value;
                    档期 = value.ToString("yyyyMM");
                    星期 = value.ToString("dddd");
                    OnPropertyChanged(nameof(首播));
                }
            }
        }

        private string _星期;
        [Required]
        [StringLength(3)]
        public string 星期
        {
            get { return _星期; }
            set { SetProperty(ref _星期, value); }
        }

        private string _译名;
        [Required]
        [StringLength(256)]
        public string 译名
        {
            get { return _译名; }
            set { SetProperty(ref _译名, value); }
        }

        private string _原名;
        [StringLength(256)]
        public string 原名
        {
            get { return _原名; }
            set { SetProperty(ref _原名, value); }
        }

        private string _原作;
        [StringLength(256)]
        public string 原作
        {
            get { return _原作; }
            set { SetProperty(ref _原作, value); }
        }

        private string _动画制作;
        [StringLength(256)]
        public string 动画制作
        {
            get { return _动画制作; }
            set { SetProperty(ref _动画制作, value); }
        }

        private string _官网;
        [StringLength(256)]
        public string 官网
        {
            get { return _官网; }
            set { SetProperty(ref _官网, value); }
        }

        private string _Bangumi编号;
        [StringLength(20)]
        public string Bangumi编号
        {
            get { return _Bangumi编号; }
            set { SetProperty(ref _Bangumi编号, value); }
        }

        private int? _话数;
        public int? 话数
        {
            get { return _话数; }
            set { SetProperty(ref _话数, value); }
        }

        private string _OP;
        [StringLength(20)]
        public string OP
        {
            get { return _OP; }
            set { SetProperty(ref _OP, value); }
        }

        private string _ED;
        [StringLength(20)]
        public string ED
        {
            get { return _ED; }
            set { SetProperty(ref _ED, value); }
        }

        private string _音乐信息;
        [StringLength(1024)]
        public string 音乐信息
        {
            get { return _音乐信息; }
            set { SetProperty(ref _音乐信息, value); }
        }

        private string _版权链接;
        [StringLength(256)]
        public string 版权链接
        {
            get { return _版权链接; }
            set { SetProperty(ref _版权链接, value); }
        }

        private bool _完结;
        public bool 完结
        {
            get { return _完结; }
            set { SetProperty(ref _完结, value); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<资源> 资源 { get; set; }
    }
}
