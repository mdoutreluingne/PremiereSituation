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

namespace WPFApplication
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(daoUtilisateur daoUtilisateur, daoVille daoVille, daoTheme daoTheme, daoSalle daoSalle, daoClient daoClient, daoReservation daoReservation, daoAvis daoAvis, daoPartie daoPartie)
        {
            InitializeComponent();
            viewModele _viewModele = new viewModele();
            DataContext = _viewModele; 

            #region Liste Salles
            List<dtoSalle> salles = (List<dtoSalle>)daoSalle.select("*", "Where archive = false ORDER BY id");
           //A CORRIGER !!!!  List<dtoAvis> avis = (List<dtoAvis>)daoAvis.select("salle_id, AVG(note)","Where salle_id = "+salles); 

            for (int i = 0; i < salles.Count; i++)
            {
                ColumnDefinition cw = new ColumnDefinition();
                grd_listSalle.ColumnDefinitions.Add(cw);

                //Ajout des labels 

                //ville
                Label lbl_Nom = new Label();
                lbl_Nom.Content = salles[i].DtoVille.Nom;
                lbl_Nom.VerticalAlignment = VerticalAlignment.Center;
                lbl_Nom.FontWeight = FontWeights.Bold;
                Grid.SetRow(lbl_Nom, 0);
                Grid.SetColumn(lbl_Nom, i + 1);
                grd_listSalle.Children.Add(lbl_Nom);
                

                //num salle 
                Label lbl_Num = new Label();
                lbl_Num.Content = "Salle n°"+ salles[i].Numero.ToString();
                lbl_Num.VerticalAlignment = VerticalAlignment.Center;
                lbl_Num.FontWeight = FontWeights.Bold;
                Grid.SetRow(lbl_Num, 1);
                Grid.SetColumn(lbl_Num, i + 1);
                grd_listSalle.Children.Add(lbl_Num);

                //theme
                Label lbl_theme = new Label();
                lbl_theme.Content = "Thème:\n "+ salles[i].DtoTheme.Nom;
                lbl_theme.VerticalAlignment = VerticalAlignment.Center;
                lbl_theme.FontWeight = FontWeights.Bold;
                Grid.SetRow(lbl_theme, 2);
                Grid.SetColumn(lbl_theme, i + 1);
                grd_listSalle.Children.Add(lbl_theme);

                //prix
                Label lbl_prix = new Label();
                lbl_prix.Content = "Prix: "+salles[i].Prix.ToString()+" €";
                lbl_prix.VerticalAlignment = VerticalAlignment.Center;
                lbl_prix.FontWeight = FontWeights.Bold;
                Grid.SetRow(lbl_prix, 3);
                Grid.SetColumn(lbl_prix, i + 1);                
                grd_listSalle.Children.Add(lbl_prix);
                
                //prix
                Label lbl_horaires = new Label();
                lbl_horaires.Content = "Ouverture: " + salles[i].Heure_ouverture.ToString() + "\n\nFermeture : " + salles[i].Heure_fermeture.ToString(); 
                lbl_horaires.VerticalAlignment = VerticalAlignment.Stretch;
                lbl_horaires.FontWeight = FontWeights.Bold;
                Grid.SetRow(lbl_horaires, 4);
                Grid.SetColumn(lbl_horaires, i + 1);
                grd_listSalle.Children.Add(lbl_horaires);

                //avis moyen 
                Label lbl_avis = new Label();
                lbl_avis.Content = "Avis Moyen : " + avis[i].Note.ToString(); 
                lbl_avis.VerticalAlignment = VerticalAlignment.Center;
                lbl_avis.FontWeight = FontWeights.Bold;
                Grid.SetRow(lbl_avis, 4);
                Grid.SetColumn(lbl_avis, i + 1);
                grd_listSalle.Children.Add(lbl_avis);
                

                /*Ajout des bordures
                Border brd = new Border();
                brd.BorderBrush = new SolidColorBrush(Colors.Black);
                brd.BorderThickness = new Thickness(0, 0, 0, 1);
                Grid.SetColumnSpan(brd, 17);
                Grid.SetRowSpan(brd, i + 2);
                grd_listSalle.Children.Add(brd);*/
                #endregion
            }
        }

  
    }
}