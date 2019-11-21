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

namespace ApplicationWPF
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> horaires = new List<string>()
        {
            "00:30:00", "02:00:00", "03:30:00", "05:00:00", "06:30:00", "08:00:00", "09:30:00", "11:00:00",
            "12:30:00","14:00:00", "15:30:00", "17:00:00", "18:30:00", "20:00:00", "21:30:00", "23:00:00"
        };
        public MainWindow(daoUtilisateur daoUtilisateur, daoArticle daoArticle, daoVille daoVille, daoTheme daoTheme, daoSalle daoSalle, daoClient daoClient, daoReservation daoReservation, daoTransaction daoTransaction, daoObstacle daoObstacle, daoArticleSalle daoArticleSalle)
        {
            InitializeComponent();
            #region Récupération de l'utilisateur
            //On donne l'utilisateur qui se connecte
            List<dtoUtilisateur> lesUser = (List<dtoUtilisateur>)daoUtilisateur.select("*", "");
            dtoUtilisateur user = lesUser[2];

            //On récupère la ville de l'utilisateur
            string nomVille = user.Login.Substring(0, user.Login.IndexOf('-'));
            List<dtoVille> lesVille = (List<dtoVille>)daoVille.select("*", "WHERE nom LIKE '" + nomVille + "%'");
            dtoVille ville = lesVille[0];
            #endregion

            #region Planning dynamique
            //On récupère les salles actives de la ville
            List<dtoSalle> salle = (List<dtoSalle>)daoSalle.select("*", "WHERE ville_id = " + ville.Id + " AND archive = false");
            for (int i = 0; i < salle.Count; i++)
            {
                RowDefinition rw = new RowDefinition();
                rw.Height = new GridLength(75);
                grd_planning.RowDefinitions.Add(rw);

                //Ajout du label avec le nom de la salle
                Label lbl = new Label();
                lbl.Content = "Salle n° " + salle[i].Numero.ToString();
                lbl.VerticalAlignment = VerticalAlignment.Center;
                lbl.FontWeight = FontWeights.Bold;
                Grid.SetColumn(lbl, 0);
                Grid.SetRow(lbl, i + 1);
                grd_planning.Children.Add(lbl);
                //--- --- ---

                //Ajout des bordures
                Border brd = new Border();
                brd.BorderBrush = new SolidColorBrush(Colors.Black);
                brd.BorderThickness = new Thickness(0, 0, 0, 1);
                Grid.SetColumnSpan(brd, 17);
                Grid.SetRowSpan(brd, i + 2);
                grd_planning.Children.Add(brd);

                //Ajout des heures où la salle est fermé
                int ouverture  = horaires.IndexOf(salle[i].Heure_ouverture.TimeOfDay.ToString());
                int fermeture = horaires.IndexOf(salle[i].Heure_fermeture.TimeOfDay.ToString());
                for (int j = 0; j < 16; j++)
                {
                    if (j < ouverture || j > fermeture)
                    {
                        Rectangle rectangle = new Rectangle();
                        rectangle.Fill = new SolidColorBrush(Colors.LightGray);
                        rectangle.Margin = new Thickness(2, 2, 2, 2) ;
                        Grid.SetColumn(rectangle, j + 1);
                        Grid.SetRow(rectangle, i + 1);
                        grd_planning.Children.Add(rectangle);
                    }
                    else
                    {
                        Rectangle rectangle = new Rectangle();
                        rectangle.Fill = new SolidColorBrush(Colors.White);
                        rectangle.Margin = new Thickness(2, 2, 2, 2);
                        Grid.SetColumn(rectangle, j + 1);
                        Grid.SetRow(rectangle, i + 1);
                        grd_planning.Children.Add(rectangle);
                    }
                }

                //Ajout des réservations
                string joinWhere = "WHERE reservation.date LIKE '%2019-02-16%' AND salle_id = " + i;
                List<dtoReservation> reservations = (List<dtoReservation>)daoReservation.select("*", joinWhere);
                for (int k = 0; k < reservations.Count; k++)
                {
                    int heure = horaires.IndexOf(reservations[k].Date.TimeOfDay.ToString());
                    Rectangle rectangle = new Rectangle();
                    Color c = Color.FromRgb(116, 172, 147);
                    rectangle.Fill = new SolidColorBrush(c);
                    rectangle.Margin = new Thickness(5, 5, 5, 50);
                    Grid.SetColumn(rectangle, heure + 1);
                    Grid.SetRow(rectangle, i);
                    var rectangle_free =  grd_planning.Children
                        .Cast<UIElement>()
                        .First(e => Grid.GetRow(e) == i && Grid.GetColumn(e) == heure + 1);
                    grd_planning.Children.Remove(rectangle_free);
                    grd_planning.Children.Add(rectangle);
                }
            }
            #endregion
        }
    }
}
