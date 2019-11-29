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
using CoucheModele.metier;
using CoucheModele.modele;

namespace WpfComptabilite
{
    /// <summary>
    /// Logique d'interaction pour ConnexionWindow.xaml
    /// </summary>
    public partial class ConnexionWindow : Window
    {
        //static private dbal bdd = new dbal("admin", "admin", 3306, "172.31.135.1", "bdd_escape_game");
        //static private dbal bdd = new dbal("admin", "admin", 3306, "172.31.135.2", "bdd_escape_game");
        static private dbal bdd = new dbal("root", "", 3306, "127.0.0.1", "bdd_escape_game");
        static private daoUtilisateur unDaoUtilisateur = new daoUtilisateur(bdd);

        public ConnexionWindow()
        {
            InitializeComponent();
            cmb_ville.SelectedIndex = 0;
            txt_login.Focus();
        }
        //PERSONNE NE TOUCHE AU CODE
        private void connecter_Click(object sender, RoutedEventArgs e)
        {
            List<Utilisateur> lesUsers = new List<Utilisateur>(unDaoUtilisateur.selectAllClient());
            string login = cmb_ville.Text + txt_login.Text;

            foreach (Utilisateur user in lesUsers)
            {
                if (login.Contains(user.Login) == true)
                {
                    if (pass.Password.Contains(user.Mdp) == true)
                    {
                        MainWindow logicielCompta = new MainWindow();
                        logicielCompta.Show(); //Ouvre la fenetre
                        this.Close(); //Fermeture de la fenêtre actuel
                    }
                    else
                    {
                        MessageBox.Show("Mot de passe incorrect", "Erreur de connexion", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Login incorrect", "Erreur de connexion", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
        

    }
}
