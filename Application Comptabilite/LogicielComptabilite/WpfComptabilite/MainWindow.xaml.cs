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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CoucheModele.modele;
using CoucheModele.metier;
using WpfComptabilite.viewModel;
using System.Configuration;

namespace WpfComptabilite
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //static private dbal bdd = new dbal("admin", "admin", 3306, "172.31.135.1", "bdd_escape_game");
        //static private dbal bdd = new dbal("admin", "admin", 3306, "172.31.135.2", "bdd_escape_game");
        //static private dbal bdd = new dbal("admin", "admin", 3306, "127.0.0.1", "bdd_escape_game");
        static private dbal bdd;
        static private daoVille theDaoVille;
        static private daoTheme theDaoTheme;
        static private daoSalle theDaoSalle;
        static private daoClient theDaoClient;
        static private daoReservation theDaoReserv;
        static private daoTransaction theDaoTransac;
        private viewClient vc;
        public MainWindow()
        {
            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["BddVm"]))
            {
                string[] recup = ConfigurationSettings.AppSettings["BddVm"].Split(',');
                bdd = new dbal(recup[0], recup[1], Convert.ToInt32(recup[2]), recup[3], recup[4]);
            }
            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["Localhost"]))
            {
                string[] recup = ConfigurationSettings.AppSettings["Localhost"].Split(',');
                bdd = new dbal(recup[0], recup[1], Convert.ToInt32(recup[2]), recup[3], recup[4]);
            }

            theDaoVille = new daoVille(bdd);
            theDaoTheme = new daoTheme(bdd);
            theDaoSalle = new daoSalle(bdd, theDaoVille, theDaoTheme);
            theDaoClient = new daoClient(bdd, theDaoVille);
            theDaoReserv = new daoReservation(bdd, theDaoClient, theDaoSalle);
            theDaoTransac = new daoTransaction(bdd, theDaoClient, theDaoReserv);

            InitializeComponent();
            vc = new viewClient(theDaoTransac, theDaoVille, theDaoClient, bdd, theDaoTheme, theDaoSalle, theDaoReserv);
            principale.DataContext = vc;
        }

    }
}
