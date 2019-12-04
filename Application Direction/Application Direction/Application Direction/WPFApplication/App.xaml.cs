using ModeleMetier.modele;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WPFApplication
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void App_Startup(object sender, StartupEventArgs e)
        {
            MainWindow wnd = new MainWindow();
            wnd.WindowState = WindowState.Maximized;
            wnd.Show();
            
        }
    }
}
