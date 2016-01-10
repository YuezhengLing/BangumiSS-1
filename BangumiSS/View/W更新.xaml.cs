using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace BangumiSS.View
{
    /// <summary>
    /// W更新.xaml 的交互逻辑
    /// </summary>
    public partial class W更新 : Window
    {
        public W更新(ViewModel.VM更新 vm = null)
        {
            InitializeComponent();
            DataContext = vm ?? new ViewModel.VM更新(null);
        }

        private void 全选_Click(object sender, RoutedEventArgs e)
        {
            LB_全部.SelectAll();
        }

        private void 清除_Click(object sender, RoutedEventArgs e)
        {
            LB_全部.SelectedItems.Clear();
        }
    }

    public class CustomListBox : ListBox
    {
        public CustomListBox()
        {
            this.SelectionChanged += CustomListBox_SelectionChanged;
        }

        void CustomListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedItemsList = this.SelectedItems;
        }

        #region SelectedItemsList
        public IList SelectedItemsList
        {
            get { return (IList)GetValue(SelectedItemsListProperty); }
            set { SetValue(SelectedItemsListProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsListProperty =
                DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(CustomListBox), new PropertyMetadata(null));
        #endregion
    }
}
