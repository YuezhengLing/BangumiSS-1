using BangumiSS.Model;
using System.Windows;
using System.Windows.Input;

namespace BangumiSS.View
{
    /// <summary>
    /// W新番.xaml 的交互逻辑
    /// </summary>
    public partial class W新番 : Window
    {
        public W新番(ViewModel.VM新番 vm)
        {
            InitializeComponent();
            if (vm == null)
                vm = new ViewModel.VM新番();
            vm.Close = this.Close;
            this.DataContext = vm;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                B_确定.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CB_名称.Focus();
        }
    }
}
