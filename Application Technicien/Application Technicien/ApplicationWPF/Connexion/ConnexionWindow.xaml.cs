using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
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
using ApplicationWPF;
using ModeleMetier.metier;
using ModeleMetier.modele;

namespace WpfComptabilite
{
    /// <summary>
    /// Logique d'interaction pour ConnexionWindow.xaml
    /// </summary>
    public partial class ConnexionWindow : Window
    {
        static private dbal bdd;
        static private daoArticle daoArticle;
        static private daoArticleSalle daoArticleSalle;
        static private daoClient daoClient;
        static private daoObstacle daoObstacle;
        static private daoReservation daoReservation;
        static private daoSalle daoSalle;
        static private daoTheme daoTheme;
        static private daoTransaction daoTransaction;
        static private daoUtilisateur daoUtilisateur;
        static private daoVille daoVille;
        static private object lesUsers;

        public ConnexionWindow()
        {
            if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["bdd"]))
            {
                string[] recup = ConfigurationSettings.AppSettings["bdd"].Split(',');
                bdd = new dbal(recup[0], recup[1], recup[2], recup[3]);
            }
            daoArticle = new daoArticle(bdd);
            daoTheme = new daoTheme(bdd);
            daoVille = new daoVille(bdd);
            daoSalle = new daoSalle(bdd, daoVille, daoTheme);
            daoArticleSalle = new daoArticleSalle(bdd, daoArticle, daoSalle);
            daoClient = new daoClient(bdd, daoVille);
            daoReservation = new daoReservation(bdd, daoClient, daoSalle);
            daoTransaction = new daoTransaction(bdd, daoClient, daoReservation);
            daoObstacle = new daoObstacle(bdd, daoReservation, daoArticle);
            daoUtilisateur = new daoUtilisateur(bdd, new daoClient(bdd, new daoVille(bdd)));
            lesUsers = daoUtilisateur.select("*","");

            InitializeComponent();
            cmb_ville.SelectedIndex = 0;
            txt_login.Focus();
        }
        
        private void connecter_Click(object sender, RoutedEventArgs e)
        {  
            string login = cmb_ville.Text + txt_login.Text;
            bool erreur_login = false;
            bool erreur_pass = false;

            foreach (dtoUtilisateur user in (List<dtoUtilisateur>)lesUsers)
            {
                if (login.Contains(user.Login) == true)
                {
                    string source = pass.Password; //Password saisie
                    bool connect = false;

                    using (MD5 md5Hash = MD5.Create())
                    {
                        string hash = user.Mdp;

                        if (VerifyMd5Hash(md5Hash, source, hash)) //Test si les 2 passwords cryptés sont identiques
                        {
                            connect = true;
                        }
                        else
                        {
                            connect = false;
                        }
                    }

                    if (connect == true) //Si les password sont identiques
                    {
                        MainWindow logicielCompta = new MainWindow(daoUtilisateur, daoArticle, daoVille, daoTheme, daoSalle, daoClient, daoReservation, daoTransaction, daoObstacle, daoArticleSalle, user);
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
        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void btn_modif_pass_Click(object sender, RoutedEventArgs e)
        {
            lbl_new_mdp.Visibility = Visibility.Visible;
            pass_new.Visibility = Visibility.Visible;
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
            bool comfirm_pass = false;
            MD5 md5Hash = MD5.Create();
            string AncienPasswordCrypte = GetMd5Hash(md5Hash, pass.Password);
            string NouveauPasswordCrypte = GetMd5Hash(md5Hash, pass_new.Password);
            string NouveauPasswordComfirmCrypte = GetMd5Hash(md5Hash, pass_confirm.Password);

            if (txt_login.Text != "" || pass.Password != "")
            {
                foreach (dtoUtilisateur user in (List<dtoUtilisateur>)lesUsers) //Parcourt les utilisateurs de la bdd
                {
                    if (login.Contains(user.Login) == true)
                    {
                        if (AncienPasswordCrypte.Contains(user.Mdp) == true)
                        {    
                            if (NouveauPasswordCrypte == NouveauPasswordComfirmCrypte)
                            {
                                daoUtilisateur.update(login, NouveauPasswordCrypte);
                                lbl_new_mdp.Visibility = Visibility.Hidden;
                                pass_new.Visibility = Visibility.Hidden;
                                lbl_comfirm_pass.Visibility = Visibility.Hidden;
                                pass_confirm.Visibility = Visibility.Hidden;
                                btn_connecter.Visibility = Visibility.Visible;
                                btn_update.Visibility = Visibility.Hidden;
                                erreur_login = false;
                                lesUsers = daoUtilisateur.select("*", ""); //Rechargement des utilisateurs après modification mdp
                                break;
                            }
                            else
                            {
                                erreur_login = false;
                                erreur_pass = false;
                                comfirm_pass = true;
                                break;
                            }

                        }
                        else
                        {
                            erreur_login = false;
                            comfirm_pass = false;
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
                MessageBox.Show("Mot de passe incorrect", "Erreur de connexion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (comfirm_pass == true)
            {
                MessageBox.Show("Les mots de passe saisie ne sont pas identiques", "Erreur de connexion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
