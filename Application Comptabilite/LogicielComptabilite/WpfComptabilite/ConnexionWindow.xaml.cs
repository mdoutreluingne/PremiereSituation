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
        static private List<Utilisateur> lesUsers = new List<Utilisateur>(unDaoUtilisateur.selectAllClient());

        public ConnexionWindow()
        {
            InitializeComponent();
            cmb_ville.SelectedIndex = 0;
            txt_login.Focus();
        }
        //PERSONNE NE TOUCHE AU CODE
        private void connecter_Click(object sender, RoutedEventArgs e)
        {  
            string login = cmb_ville.Text + txt_login.Text;
            bool erreur_login = false;
            bool erreur_pass = false;

            foreach (Utilisateur user in lesUsers)
            {
                if (login.Contains(user.Login) == true)
                {
                    if (pass.Password.Contains(user.Mdp) == true)
                    {
                        MainWindow logicielCompta = new MainWindow();
                        logicielCompta.Show(); //Ouvre la fenetre
                        this.Close(); //Fermeture de la fenêtre actuel
                        erreur_login = false;
                        erreur_pass = false;
                        break;
                    }
                    else
                    {
                        erreur_login = false;
                        erreur_pass = true;
                        break;
                    }
                }
                else
                {
                    erreur_login = true;
                }
            }

            //Gestion cas d'erreur
            if (erreur_login == true)
            {
                MessageBox.Show("Login incorrect", "Erreur de connexion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (erreur_pass == true)
            {
                MessageBox.Show("Mot de passe incorrect", "Erreur de connexion", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btn_modif_pass_Click(object sender, RoutedEventArgs e)
        {
            lbl_comfirm_pass.Visibility = Visibility.Visible;
            pass_confirm.Visibility = Visibility.Visible;
            btn_connecter.Visibility = Visibility.Hidden;
            btn_update.Visibility = Visibility.Visible;

        }

        private void btn_update_Click(object sender, RoutedEventArgs e)
        {
            string login = cmb_ville.Text + txt_login.Text;
            bool erreur_login = false;
            bool erreur_pass = false;

            if (txt_login.Text != "" || pass.Password != "")
            {
                foreach (Utilisateur user in lesUsers)
                {
                    if (login.Contains(user.Login) == true)
                    {
                        if (pass.Password == pass_confirm.Password)
                        {
                            unDaoUtilisateur.update("UPDATE utilisateur SET mdp = '" + pass.Password + "' WHERE login = '" + login + "'");
                            lbl_comfirm_pass.Visibility = Visibility.Hidden;
                            pass_confirm.Visibility = Visibility.Hidden;
                            btn_connecter.Visibility = Visibility.Visible;
                            btn_update.Visibility = Visibility.Hidden;
                            erreur_login = false;
                            lesUsers = new List<Utilisateur>(unDaoUtilisateur.selectAllClient()); //Rechargement des utilisateurs après modification mdp
                            break;

                        }
                        else
                        {
                            erreur_login = false;
                            erreur_pass = true;
                            break;
                        }
                    }
                    else
                    {
                        erreur_login = true;
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez renseigner les champs", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //Gestion cas d'erreur
            if (erreur_login == true)
            {
                MessageBox.Show("Login incorrect", "Erreur de connexion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (erreur_pass == true)
            {
                MessageBox.Show("Les mots de passe saisie ne sont pas identiques", "Erreur de connexion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
