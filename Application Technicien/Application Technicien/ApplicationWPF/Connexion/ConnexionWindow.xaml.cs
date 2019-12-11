using System;
using System.Collections.Generic;
using System.Configuration;
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
using ModeleMetier.metier;
using ModeleMetier.modele;

namespace ApplicationWPF.Connexion
{
    /// <summary>
    /// Logique d'interaction pour ConnexionWindow.xaml
    /// </summary>
    public partial class ConnexionWindow : Window
    {
        static private dbal _dbal;
        static private daoUtilisateur _daoUtilisateur;
        static private daoArticle _daoArticle;
        static private daoVille _daoVille;
        static private daoTheme _daoTheme;
        static private daoSalle _daoSalle;
        static private daoClient _daoClient;
        static private daoReservation _daoReservation;
        static private daoTransaction _daoTransaction;
        static private daoObstacle _daoObstacle;
        static private daoArticleSalle _daoArticleSalle;
        static private List<dtoUtilisateur> lesUsers;

        public ConnexionWindow()
        {
            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["bdd"]))
            {
                string[] configBdd = ConfigurationSettings.AppSettings["bdd"].Split(',');
                _dbal = new dbal(configBdd[0], configBdd[1], configBdd[2], configBdd[3]);
                _daoUtilisateur = new daoUtilisateur(_dbal);
                _daoArticle = new daoArticle(_dbal);
                _daoVille = new daoVille(_dbal);
                _daoTheme = new daoTheme(_dbal);
                _daoSalle = new daoSalle(_dbal, _daoVille, _daoTheme);
                _daoClient = new daoClient(_dbal, _daoVille);
                _daoReservation = new daoReservation(_dbal, _daoClient, _daoSalle);
                _daoTransaction = new daoTransaction(_dbal, _daoClient, _daoReservation);
                _daoObstacle = new daoObstacle(_dbal, _daoReservation, _daoArticle);
                lesUsers = (List<dtoUtilisateur>)_daoUtilisateur.select("*", "");
            }
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

            foreach (dtoUtilisateur user in lesUsers)
            {
                if (login.Contains(user.Login) == true)
                {
                    if (pass.Password.Contains(user.Mdp) == true)
                    {
                        MainWindow logiciel = new MainWindow(_daoUtilisateur, _daoArticle, _daoVille, _daoTheme, _daoSalle, _daoClient, _daoReservation, _daoTransaction, _daoObstacle, _daoArticleSalle, user);
                        logiciel.WindowState = WindowState.Maximized;
                        logiciel.Show(); //Ouvre la fenetre
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
                foreach (dtoUtilisateur user in lesUsers)
                {
                    if (login.Contains(user.Login) == true)
                    {
                        if (pass.Password == pass_confirm.Password)
                        {
                            _daoUtilisateur.update(pass.Password, login);
                            lbl_comfirm_pass.Visibility = Visibility.Hidden;
                            pass_confirm.Visibility = Visibility.Hidden;
                            btn_connecter.Visibility = Visibility.Visible;
                            btn_update.Visibility = Visibility.Hidden;
                            erreur_login = false;
                            lesUsers = (List<dtoUtilisateur>)_daoUtilisateur.select("*", ""); //Rechargement des utilisateurs après modification mdp
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
