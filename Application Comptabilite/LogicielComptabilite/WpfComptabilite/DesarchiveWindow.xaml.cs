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
using CoucheModele.modele;
using CoucheModele.metier;
using WpfComptabilite.viewModel;

namespace WpfComptabilite
{
    /// <summary>
    /// Logique d'interaction pour DesarchiveWindow.xaml
    /// </summary>
    public partial class DesarchiveWindow : Window
    {
        //static private dbal bdd = new dbal("admin", "admin", 3306, "172.31.135.1", "bdd_escape_game");
        static private dbal bdd = new dbal("root", "", 3306, "127.0.0.1", "bdd_escape_game");
        static private daoVille theDaoVille = new daoVille(bdd);
        static private daoClient theDaoClient = new daoClient(bdd, theDaoVille);
        private viewDesarchive vd;
        public DesarchiveWindow()
        {
            InitializeComponent();
            vd = new viewDesarchive(theDaoVille, theDaoClient, bdd);
            secondaire.DataContext = vd;
        }
    }
}
