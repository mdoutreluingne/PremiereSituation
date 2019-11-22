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

namespace WpfComptabilite
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static private dbal bdd = new dbal("admin", "admin", 3306, "172.31.135.1", "bdd_escape_game");
        //static private dbal bdd = new dbal("root", "", 3306, "127.0.0.1", "bdd_escape_game");
        static private daoVille theDaoVille = new daoVille(bdd);
        static private daoTheme theDaoTheme = new daoTheme(bdd);
        static private daoSalle theDaoSalle = new daoSalle(bdd, theDaoVille, theDaoTheme);
        static private daoClient theDaoClient = new daoClient(bdd, theDaoVille);
        static private daoReservation theDaoReserv = new daoReservation(bdd, theDaoClient, theDaoSalle);
        static private daoTransaction theDaoTransac = new daoTransaction(bdd, theDaoClient, theDaoReserv);
        private viewClient vc;
        public MainWindow()
        {
            InitializeComponent();
            vc = new viewClient(theDaoTransac, theDaoVille, theDaoClient, bdd, theDaoTheme, theDaoSalle, theDaoReserv);
            principale.DataContext = vc;
            cmb_ville.SelectedIndex = -1;
        }

        //private void cmb_ville_PreviewTextInput(object sender, TextCompositionEventArgs e)
        //{
        //    cmb_ville.MaxDropDownHeight = 50;
        //    cmb_ville.ItemsSource = vc.Lesvilles.Where(p => p.Nom.Contains(e.Text)).ToList();
        //    cmb_ville.IsDropDownOpen = true;
        //}
    }
}
