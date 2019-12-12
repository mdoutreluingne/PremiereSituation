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
        private viewModele _viewModele;
        private daoSalle _daoSalle;
        private daoAvis _daoAvis;
        private daoVille _daoVille;
        private daoPartie _daoPartie;
        private daoTheme _daoTheme;

        public MainWindow(daoSalle daoSalle, daoAvis daoAvis, daoVille daoVille, daoPartie daoPartie, daoTheme daoTheme )
        {
            InitializeComponent();

       

            _daoSalle = daoSalle;
            _daoAvis = daoAvis;
            _daoVille = daoVille;
            _daoPartie = daoPartie;
            _daoTheme = daoTheme;

            #region Liste Salles
            List<dtoSalle> salles = (List<dtoSalle>)_daoSalle.select("*", "Where archive = false ORDER BY id");
            List<dtoAvis> avis = new List<dtoAvis>();
            foreach (dtoSalle s in salles)
            {
                avis.Add(((List<dtoAvis>)_daoAvis.select("id,  AVG(note) as 'note', date, commentaire, salle_id, client_id", "Where salle_id = " + s.Id))[0]);
            }
            _viewModele = new viewModele(_daoAvis, _daoSalle, null, _daoVille, _daoPartie, _daoTheme, this, salles, avis); 
            loadPage(salles, avis);
            DataContext = _viewModele;

        }

        public void loadPage(List<dtoSalle> salles, List<dtoAvis> avis)
        {
            List<viewModele> lesViews = new List<viewModele>();
            for (int i = 0; i < salles.Count + 1; i++)
            {
                ColumnDefinition cw = new ColumnDefinition();
                grd_listSalle.ColumnDefinitions.Add(cw);
            }

            for (int i = 0; i < salles.Count; i++)
            {
                viewModele viewModele = new viewModele(_daoAvis, _daoSalle, salles[i], _daoVille, _daoPartie, _daoTheme, this, salles, avis);
                lesViews.Add(viewModele);
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
                txt_Nom.SetBinding(TextBox.TextProperty, texte_Nom);
                grd_listSalle.Children.Add(txt_Nom);

                Button bouton_ville = new Button();
                bouton_ville.Content = "Statistiques";
                bouton_ville.Margin = new Thickness(0, 70, 0, 0);
                bouton_ville.Height = 25;
                bouton_ville.Width = 200;
                Grid.SetRow(bouton_ville, 1);
                Grid.SetColumn(bouton_ville, i + 1);

                Binding bind_btn_ville = new Binding("Statistiques");
                bind_btn_ville.Source = viewModele;
                bouton_ville.SetBinding(Button.CommandProperty, bind_btn_ville);

                grd_listSalle.Children.Add(bouton_ville);


                //num salle 
                Label lbl_Num = new Label();
                lbl_Num.Content = "Ville :";

                TextBox txt_Num = new TextBox();
                txt_Num.Text = "Salle n°" + salles[i].Numero.ToString();
                txt_Num.VerticalAlignment = VerticalAlignment.Center;
                txt_Num.FontWeight = FontWeights.Bold;
                txt_Num.Width = 200;
                Grid.SetRow(txt_Num, 2);
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
                txt_theme.Text = salles[i].DtoTheme.Nom;
                txt_theme.VerticalAlignment = VerticalAlignment.Center;
                txt_theme.FontWeight = FontWeights.Bold;
                txt_theme.Width = 200;
                Grid.SetRow(txt_theme, 3);
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
                txt_prix.Width = 200;
                Grid.SetRow(txt_prix, 4);
                Grid.SetColumn(txt_prix, i + 1);

                Binding bind_prix = new Binding("EnableModification");
                bind_prix.Source = viewModele;
                txt_prix.SetBinding(TextBox.IsEnabledProperty, bind_prix);

                Binding texte_prix = new Binding("prixSalle");
                texte_prix.Source = viewModele;
                txt_prix.SetBinding(TextBox.TextProperty, texte_prix);


                grd_listSalle.Children.Add(txt_prix);

                //horaires
                TextBox txt_ouverture = new TextBox();
                txt_ouverture.Text = salles[i].Heure_ouverture.ToString();
                txt_ouverture.VerticalAlignment = VerticalAlignment.Stretch;
                txt_ouverture.FontWeight = FontWeights.Bold;
                txt_ouverture.Width = 200;
                Grid.SetRow(txt_ouverture, 5);
                Grid.SetColumn(txt_ouverture, i + 1);

                Binding bind_enableOuverture = new Binding("EnableModification");
                bind_enableOuverture.Source = viewModele;
                txt_ouverture.SetBinding(TextBox.IsEnabledProperty, bind_enableOuverture);

                Binding heureOuverture = new Binding("horaireSalleOuverture");
                heureOuverture.Source = viewModele;
                txt_ouverture.SetBinding(TextBox.TextProperty, heureOuverture);

                grd_listSalle.Children.Add(txt_ouverture);

                TextBox txt_fermeture = new TextBox();
                txt_fermeture.Text = salles[i].Heure_fermeture.ToString();
                txt_fermeture.VerticalAlignment = VerticalAlignment.Stretch;
                txt_fermeture.FontWeight = FontWeights.Bold;
                txt_fermeture.Width = 200;
                Grid.SetRow(txt_fermeture, 6);
                Grid.SetColumn(txt_fermeture, i + 1);

                Binding bind_enableFermeture = new Binding("EnableModification");
                bind_enableFermeture.Source = viewModele;
                txt_fermeture.SetBinding(TextBox.IsEnabledProperty, bind_enableFermeture);

                Binding bind_horairesFermeture = new Binding("horaireSalleFermeture");
                bind_horairesFermeture.Source = viewModele;
                txt_fermeture.SetBinding(TextBox.TextProperty, bind_horairesFermeture);

                grd_listSalle.Children.Add(txt_fermeture);

                //avis moyen 
                Button btn_avis = new Button();
                btn_avis.Tag = salles[i].Id;
                btn_avis.Content = "Avis Moyen : " + Math.Round(avis[i].Note, 2);
                btn_avis.VerticalAlignment = VerticalAlignment.Center;
                btn_avis.FontWeight = FontWeights.Bold;
                btn_avis.Click += btn_avis_Click;
                btn_avis.Width = 200;
                Grid.SetRow(btn_avis, 7);
                Grid.SetColumn(btn_avis, i + 1);
                grd_listSalle.Children.Add(btn_avis);

                //Bouton archiver salle 

                Button btn_suppr = new Button();
                btn_suppr.Content = "ARCHIVER";
                btn_suppr.FontWeight = FontWeights.Bold;
                btn_suppr.Width = 175;

                Grid.SetRow(btn_suppr, 8);
                Grid.SetColumn(btn_suppr, i + 1);
                grd_listSalle.Children.Add(btn_suppr);

                Binding bind_archive = new Binding("EnableModification");
                bind_archive.Source = viewModele;
                btn_suppr.SetBinding(TextBox.IsEnabledProperty, bind_archive);

                Binding bind_btn = new Binding("Archiver");
                bind_btn.Source = viewModele;
                btn_suppr.SetBinding(Button.CommandProperty, bind_btn);

                /*Ajout des bordures
                Border brd = new Border();
                brd.BorderBrush = new SolidColorBrush(Colors.Black);
                brd.BorderThickness = new Thickness(0, 0, 0, 1);
                Grid.SetColumnSpan(brd, 17);
                Grid.SetRowSpan(brd, i + 2);
                grd_listSalle.Children.Add(brd);*/
                #endregion
            }
            _viewModele.compteLesView(lesViews);
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