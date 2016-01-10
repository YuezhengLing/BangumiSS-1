using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using SatrokiLibrary.Extend;

namespace BangumiSS
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += (s, e) =>
            {
                e.Exception.ShowException();
                e.Handled = true;
            };
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var view = new View.W主窗口();
            MainDispatcher = view.Dispatcher;
            view.Show();
        }

        public static System.Windows.Threading.Dispatcher MainDispatcher;
    }
}
