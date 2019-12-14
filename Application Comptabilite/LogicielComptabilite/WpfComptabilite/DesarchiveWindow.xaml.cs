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
using System.Configuration;

namespace WpfComptabilite
{
    /// <summary>
    /// Logique d'interaction pour DesarchiveWindow.xaml
    /// </summary>
    public partial class DesarchiveWindow : Window
    {
        static private dbal bdd;
        static private daoVille theDaoVille;
        static private daoClient theDaoClient;
        private viewDesarchive vd;
        public DesarchiveWindow()
        {
            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["BddVm"]))
            {
                string[] recup = ConfigurationSettings.AppSettings["BddVm"].Split(',');
                bdd = new dbal(recup[0], recup[1], Convert.ToInt32(recup[2]), recup[3], recup[4]);
            }

            theDaoVille = new daoVille(bdd);
            theDaoClient = new daoClient(bdd, theDaoVille);

            InitializeComponent();
            vd = new viewDesarchive(theDaoVille, theDaoClient, bdd);
            secondaire.DataContext = vd;
        }
    }
}
