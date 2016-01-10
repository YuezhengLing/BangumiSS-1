using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BangumiSS.ViewModel;

namespace BangumiSS.View
{
    /// <summary>
    /// W音乐.xaml 的交互逻辑
    /// </summary>
    public partial class W音乐 : Window
    {
        public W音乐(VM音乐 vm)
        {
            InitializeComponent();
            this.vm = vm;
            DataContext = vm;
        }

        private VM音乐 vm;

        private void Window_Closed(object sender, EventArgs e)
        {
            vm?.Save();
        }
    }
}
