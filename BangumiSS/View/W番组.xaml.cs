using System.Windows;
using System.Windows.Input;
using System.Windows.Data;
using System;
using System.Globalization;

namespace BangumiSS.View
{
    /// <summary>
    /// W番组.xaml 的交互逻辑
    /// </summary>
    public partial class W番组 : Window
    {
        public W番组(ViewModel.VM番组 vm)
        {
            InitializeComponent();
            if (vm == null)
                vm = new ViewModel.VM番组();
            vm.Close = this.Close;
            DataContext = vm;
        }

        private void B_取消_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TB_编号.Focus();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                B_确定.Focus();
        }
    }

    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dt = (DateTime)value;
            var str = "yyyy/MM/dd ";
            if (dt.TimeOfDay.TotalMinutes > 0)
                str += "HH:mm";
            return ((DateTime)value).ToString(str);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dt = new DateTime();
            var str = value.ToString();
            if (!DateTime.TryParse(str, out dt))
            {
                var parts = str.Split(' ');
                dt = DateTime.Parse(parts[0]);

                if (parts.Length > 1 && !string.IsNullOrEmpty(parts[1]))
                {
                    var temp = parts[1].Trim().Split('.', ':', ',', ' ');
                    int d = 0, h = 0, m = 0;
                    switch (temp.Length)
                    {
                        case 1: h = int.Parse(temp[0]); break;
                        case 2: h = int.Parse(temp[0]); m = int.Parse(temp[1]); break;
                        case 3: d = int.Parse(temp[0]); h = int.Parse(temp[1]); m = int.Parse(temp[2]); break;
                    }
                    if (h >= 24)
                    { h = h - 24; d = d + 1; }
                    var ts = new TimeSpan(d, h, m, 0);
                    dt += ts;
                }
            }
            return dt;
        }
    }
}
