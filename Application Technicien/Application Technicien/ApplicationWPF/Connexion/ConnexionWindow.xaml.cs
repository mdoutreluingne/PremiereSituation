﻿using System;
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
using ModeleMetier.metier;
using ModeleMetier.modele;

namespace ApplicationWPF.Connexion
{
    /// <summary>
    /// Logique d'interaction pour ConnexionWindow.xaml
    /// </summary>
    public partial class ConnexionWindow : Window
    {
        //_dbal = new dbal("172.31.135.1","admin","bdd_escape_game","admin");
        //_dbal = new dbal("172.31.135.2","admin","bdd_escape_game","admin");
        static private dbal _dbal = new dbal("127.0.0.1", "root", "bdd_escape_game", "");
        static private daoUtilisateur _daoUtilisateur = new daoUtilisateur(_dbal);
        static private daoArticle _daoArticle = new daoArticle(_dbal);
        static private daoVille _daoVille = new daoVille(_dbal);
        static private daoTheme _daoTheme = new daoTheme(_dbal);
        static private daoSalle _daoSalle = new daoSalle(_dbal, _daoVille, _daoTheme);
        static private daoClient _daoClient = new daoClient(_dbal, _daoVille);
        static private daoReservation _daoReservation = new daoReservation(_dbal, _daoClient, _daoSalle);
        static private daoTransaction _daoTransaction = new daoTransaction(_dbal, _daoClient, _daoReservation);
        static private daoObstacle _daoObstacle = new daoObstacle(_dbal, _daoReservation, _daoArticle);
        static private daoArticleSalle _daoArticleSalle = new daoArticleSalle(_dbal, _daoArticle, _daoSalle);
        static private List<dtoUtilisateur> lesUsers = (List<dtoUtilisateur>)_daoUtilisateur.select("*","");

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