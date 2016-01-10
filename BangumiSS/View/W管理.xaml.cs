using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BangumiSS.View
{
    /// <summary>
    /// W管理.xaml 的交互逻辑
    /// </summary>
    public partial class W管理 : Window
    {
        public W管理(ViewModel.VM管理 vm = null)
        {
            InitializeComponent();
            this.DataContext = vm ?? new ViewModel.VM管理();
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is GridViewColumnHeader)
            {
                var clickedColumn = (e.OriginalSource as GridViewColumnHeader).Column;
                if (clickedColumn != null)
                {
                    string bindingProperty = clickedColumn.Header.ToString();
                    var sdc = LV_番组.Items.SortDescriptions;
                    var sortDirection = ListSortDirection.Ascending;
                    if (sdc.Count > 0)
                    {
                        SortDescription sd = sdc[0];
                        sortDirection = (ListSortDirection)((((int)sd.Direction) + 1) % 2);
                        sdc.Clear();
                    }
                    sdc.Add(new SortDescription(bindingProperty, sortDirection));
                }
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                B_搜索.Focus();
        }

        private void LV_番组_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Link;
            e.Handled = true;
        }

        private void LV_番组_Drop(object sender, DragEventArgs e)
        {
            var uri = e.Data.GetData(DataFormats.Text).ToString();
            uri = uri.Substring(uri.LastIndexOf('/') + 1);
            int t;
            if (int.TryParse(uri, out t))
                (DataContext as ViewModel.VM管理).DropBgm.Execute(uri);
        }
    }
}
