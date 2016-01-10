using System.Windows;
using System.Windows.Input;

namespace BangumiSS.View
{
    /// <summary>
    /// W主窗口.xaml 的交互逻辑
    /// </summary>
    public partial class W主窗口 : Window
    {
        public W主窗口(ViewModel.VM主窗口 vm = null)
        {
            InitializeComponent();
            vm = vm ?? new ViewModel.VM主窗口();
            DataContext = vm;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                B_搜索.Focus();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
