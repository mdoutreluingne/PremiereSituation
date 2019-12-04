using ModeleMetier.metier;
using ModeleMetier.modele;
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
using JR.Utils.GUI.Forms;

namespace WPFApplication
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private dbal _dbal;
        private daoUtilisateur _daoUtilisateur;
        private daoAvis _daoAvis;
        private daoPartie _daoPartie;
        private daoClient _daoClient;
        private daoReservation _daoReservation;
        private daoSalle _daoSalle;
        private daoTheme _daoTheme;
        private daoVille _daoVille;
        public MainWindow()
        {
            // _dbal = new dbal("172.31.135.1","admin","bdd_escape_game","admin");
            _dbal = new dbal("127.0.0.1", "root", "bdd_escape_game", "");
            _daoUtilisateur = new daoUtilisateur(_dbal);
            _daoVille = new daoVille(_dbal);
            _daoTheme = new daoTheme(_dbal);
            _daoSalle = new daoSalle(_dbal, _daoVille, _daoTheme);
            _daoClient = new daoClient(_dbal, _daoVille);
            _daoReservation = new daoReservation(_dbal, _daoClient, _daoSalle);
            _daoAvis = new daoAvis(_dbal, _daoSalle, _daoClient);
            _daoPartie = new daoPartie(_dbal, _daoReservation);

            List<viewModele> _lesViews = new List<viewModele>(); 
            InitializeComponent();
           
          //dans le code alexis --> xaml.cs -> l.209 -> exemple de ce qu'il y à faire. 

            #region Liste Salles
            List<dtoSalle> salles = (List<dtoSalle>)_daoSalle.select("*", "Where archive = false ORDER BY id");

            viewModele _viewModele = new viewModele(_daoAvis,_daoSalle,null);                     
            

            List<dtoAvis> avis = new List<dtoAvis>();
            foreach (dtoSalle s in salles)
            {
                avis.Add(((List<dtoAvis>)_daoAvis.select("id,  AVG(note) as 'note', date, commentaire, salle_id, client_id", "Where salle_id = " + s.Id))[0]);
            }
            
            for (int i = 0; i < salles.Count; i++)
            {

                viewModele viewModele = new viewModele(_daoAvis, _daoSalle, salles[i]);
                _lesViews.Add(viewModele); 


                ColumnDefinition cw = new ColumnDefinition();
                grd_listSalle.ColumnDefinitions.Add(cw);

                //Ajout des labels 

                //ville
                TextBox txt_Nom = new TextBox();
                txt_Nom.Text = salles[i].DtoVille.Nom;
                txt_Nom.VerticalAlignment = VerticalAlignment.Center;
                txt_Nom.FontWeight = FontWeights.Bold;
                txt_Nom.IsEnabled = false;
                txt_Nom.Width = 200;
                Grid.SetRow(txt_Nom, 0);
                Grid.SetColumn(txt_Nom, i + 1);
                
                Binding bind_Nom = new Binding("EnableModification");
                bind_Nom.Source = viewModele;
                txt_Nom.SetBinding(TextBox.IsEnabledProperty, bind_Nom);

                Binding texte_Nom = new Binding("NomVille");
                texte_Nom.Source = viewModele;
                txt_Nom.SetBinding(TextBox.TextProperty,texte_Nom);

                grd_listSalle.Children.Add(txt_Nom);

                //num salle 
                TextBox txt_Num = new TextBox();
                txt_Num.Text = "Salle n°" + salles[i].Numero.ToString();
                txt_Num.VerticalAlignment = VerticalAlignment.Center;
                txt_Num.FontWeight = FontWeights.Bold;
                Grid.SetRow(txt_Num, 1);
                Grid.SetColumn(txt_Num, i + 1);

                Binding bind_Num = new Binding("EnableModification");
                bind_Num.Source = viewModele;
                txt_Num.SetBinding(TextBox.IsEnabledProperty, bind_Num);

                Binding texte_Num = new Binding("NumSalle");
                texte_Num.Source = viewModele;
                txt_Num.SetBinding(TextBox.TextProperty, texte_Num);

                grd_listSalle.Children.Add(txt_Num);

                //theme
                TextBox txt_theme = new TextBox();
                txt_theme.Text = "Thème:\n " + salles[i].DtoTheme.Nom;
                txt_theme.VerticalAlignment = VerticalAlignment.Center;
                txt_theme.FontWeight = FontWeights.Bold;
                Grid.SetRow(txt_theme, 2);
                Grid.SetColumn(txt_theme, i + 1);

                Binding bind_theme = new Binding("EnableModification");
                bind_theme.Source = viewModele;
                txt_theme.SetBinding(TextBox.IsEnabledProperty, bind_Num);

                Binding texte_theme = new Binding("themeSalle");
                texte_theme.Source = viewModele;
                txt_theme.SetBinding(TextBox.TextProperty, texte_theme);

                grd_listSalle.Children.Add(txt_theme);

                //prix
                TextBox txt_prix = new TextBox();
                txt_prix.Text = "Prix: " + salles[i].Prix.ToString() + " €";
                txt_prix.VerticalAlignment = VerticalAlignment.Center;
                txt_prix.FontWeight = FontWeights.Bold;
                Grid.SetRow(txt_prix, 3);
                Grid.SetColumn(txt_prix, i + 1);

                Binding bind_prix = new Binding("EnableModification");
                bind_prix.Source = viewModele;
                txt_prix.SetBinding(TextBox.IsEnabledProperty, bind_prix);

                Binding texte_prix = new Binding("prixSalle");
                texte_prix.Source = viewModele;
                txt_prix.SetBinding(TextBox.TextProperty, texte_prix);


                grd_listSalle.Children.Add(txt_prix);

                //horaires
                TextBox txt_horaires = new TextBox();
                txt_horaires.Text = "Ouverture: " + salles[i].Heure_ouverture.ToString() + "\n\nFermeture : " + salles[i].Heure_fermeture.ToString();
                txt_horaires.VerticalAlignment = VerticalAlignment.Stretch;
                txt_horaires.FontWeight = FontWeights.Bold;
                Grid.SetRow(txt_horaires, 4);
                Grid.SetColumn(txt_horaires, i + 1);

                Binding bind_horaires = new Binding("EnableModification");
                bind_horaires.Source = viewModele;
                txt_horaires.SetBinding(TextBox.IsEnabledProperty, bind_horaires);

                Binding texte_horaires = new Binding("horaireSalle");
                texte_horaires.Source = viewModele;
                txt_horaires.SetBinding(TextBox.TextProperty, texte_horaires);

                grd_listSalle.Children.Add(txt_horaires);

                //avis moyen 
                Button btn_avis = new Button();
                btn_avis.Tag = salles[i].Id;
                btn_avis.Content = "Avis Moyen : " + Math.Round(avis[i].Note, 2);
                btn_avis.VerticalAlignment = VerticalAlignment.Center;
                btn_avis.FontWeight = FontWeights.Bold;
                btn_avis.Click += btn_avis_Click;
                Grid.SetRow(btn_avis, 5);
                Grid.SetColumn(btn_avis, i + 1);
                grd_listSalle.Children.Add(btn_avis);

                //Bouton archiver salle 

                Button btn_suppr = new Button();
                btn_suppr.Content = "ARCHIVER";
                btn_suppr.FontWeight = FontWeights.Bold;
                btn_suppr.Margin = new Thickness(20, 20, 20, 20);

                Grid.SetRow(btn_suppr, 6);
                Grid.SetColumn(btn_suppr, i + 1);
                grd_listSalle.Children.Add(btn_suppr);

                Binding bind_archive = new Binding("EnableModification");
                bind_archive.Source = viewModele;
                btn_suppr.SetBinding(TextBox.IsEnabledProperty, bind_archive);

                /*Ajout des bordures
                Border brd = new Border();
                brd.BorderBrush = new SolidColorBrush(Colors.Black);
                brd.BorderThickness = new Thickness(0, 0, 0, 1);
                Grid.SetColumnSpan(brd, 17);
                Grid.SetRowSpan(brd, i + 2);
                grd_listSalle.Children.Add(brd);*/


                #endregion
            }
            _viewModele.compteLesView(_lesViews);
            DataContext = _viewModele;

        }

        private void btn_avis_Click(object sender, RoutedEventArgs e)
        {
            Button el_verfificator = (Button)sender;
            List<dtoAvis> lesavis = ((List<dtoAvis>)_daoAvis.select("*", "WHERE salle_id =" + el_verfificator.Tag));
            string message = "";
            foreach (dtoAvis avis in lesavis)
            {
                message += "---------------\nClient : " + avis.Client.Nom + " " + avis.Client.Prenom;
                message += "\nDate : " + avis.Date;
                message += "\nNote : " + avis.Note;
                message += "\nCommentaire : " + avis.Commentaire + "\n---------------\n\n";
            }
          FlexibleMessageBox.Show(message);
        }
    }
}