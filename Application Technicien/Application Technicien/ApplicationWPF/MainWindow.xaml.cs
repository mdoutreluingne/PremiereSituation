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
        private static ViewEntete _viewEntete;
        private static ViewDate _viewDate;
        private static ViewReservation _viewReservation;
        private static ViewPlanning _viewPlanning;
        private static ViewObjet _viewObjet;
        
        public MainWindow(daoUtilisateur daoUtilisateur, daoArticle daoArticle, daoVille daoVille, daoTheme daoTheme, daoSalle daoSalle, daoClient daoClient, daoReservation daoReservation, daoTransaction daoTransaction, daoObstacle daoObstacle, daoArticleSalle daoArticleSalle)
        {
            InitializeComponent();
            //On donne l'utilisateur qui se connecte
            List<dtoUtilisateur> lesUser = (List<dtoUtilisateur>)daoUtilisateur.select("*", "");
            dtoUtilisateur user = lesUser[2];

            //On récupère la ville de l'utilisateur
            string nomVille = user.Login.Substring(0, user.Login.IndexOf('-'));
            List<dtoVille> lesVille = (List<dtoVille>)daoVille.select("*", "WHERE nom LIKE '" + nomVille + "%'");
            dtoVille ville = lesVille[0];

            //On récupère les salles actives de la ville
            List<dtoSalle> salle = (List<dtoSalle>)daoSalle.select("*", "WHERE ville_id = " + ville.Id + " AND archive = false");

            //MVVM
            _viewPlanning = new ViewPlanning(null, null, this);
            _viewDate = ViewDate.Instance(salle, daoReservation, this);
            _viewReservation = ViewReservation.Instance(this, daoClient, daoVille, daoTransaction, salle, horaires, _viewPlanning, Visibility.Hidden);
            _viewEntete = ViewEntete.Instance(this, _viewPlanning);
            _viewObjet = ViewObjet.Instance(this, daoReservation,daoArticle, _viewPlanning, null);
            grd_entete.DataContext = _viewEntete;
            grd_planning.DataContext = _viewPlanning;
            grd_date.DataContext = _viewDate;
            grd_reservation.DataContext = _viewReservation;
            grd_objet.DataContext = _viewObjet;

            //On charge le planning
            loadColumnRow(salle);
            loadPlanning(salle, daoReservation);
        }
        public void loadColumnRow(List<dtoSalle> salle)
        {
            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(50);
            grd_planning.RowDefinitions.Add(row);
            for (int i = 0; i < 17; i++)
            {
                ColumnDefinition column = new ColumnDefinition();
                grd_planning.ColumnDefinitions.Add(column);
            }
            for (int i = 0; i < salle.Count; i++)
            {
                RowDefinition rw = new RowDefinition();
                rw.Height = new GridLength(75);
                grd_planning.RowDefinitions.Add(rw);
            }
        }
        public void loadPlanning(List<dtoSalle> salle, daoReservation daoReservation)
        {
            #region Horaires
            Label label = new Label();
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Center;
            label.Content = "Salles";
            label.FontSize = 14;
            label.FontWeight = FontWeights.Bold;
            label.Width = 47;
            Grid.SetColumn(label, 0);
            Grid.SetRow(label,  0);
            grd_planning.Children.Add(label);

            for (int i = 0; i < 16; i++)
            {
                Label lbl = new Label();
                lbl.HorizontalAlignment = HorizontalAlignment.Center;
                lbl.VerticalAlignment = VerticalAlignment.Center;
                lbl.Content = horaires[i];
                lbl.FontSize = 14;
                lbl.FontWeight = FontWeights.Bold;
                lbl.Width = 47;
                Grid.SetColumn(lbl, i + 1);
                Grid.SetRow(lbl, 0);
                grd_planning.Children.Add(lbl);
            }

            Border border = new Border();
            border.BorderBrush = new SolidColorBrush(Colors.Black);
            border.BorderThickness = new Thickness(0, 0, 0, 1);
            Grid.SetColumnSpan(border, 17);
            Grid.SetRow(border, 0);
            grd_planning.Children.Add(border);

            for (int i = 0; i < 16; i++)
            {
                Border brd = new Border();
                brd.BorderBrush = new SolidColorBrush(Colors.Black);
                brd.BorderThickness = new Thickness(1, 0, 0, 0);
                Grid.SetColumn(brd, i + 1);
                Grid.SetRowSpan(brd, 10);
                grd_planning.Children.Add(brd);
            }
            #endregion

            #region Reservations
            for (int i = 0; i < salle.Count; i++)
            {
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
                int ouverture = horaires.IndexOf(salle[i].Heure_ouverture.TimeOfDay.ToString());
                int fermeture = horaires.IndexOf(salle[i].Heure_fermeture.TimeOfDay.ToString());
                for (int j = 0; j < 16; j++)
                {
                    //Heures où la salle est fermée
                    if (j < ouverture || j > fermeture)
                    {
                        Rectangle rectangle = new Rectangle();
                        rectangle.Fill = new SolidColorBrush(Colors.LightGray);
                        rectangle.Margin = new Thickness(2, 2, 2, 2);
                        Grid.SetColumn(rectangle, j + 1);
                        Grid.SetRow(rectangle, i + 1);
                        grd_planning.Children.Add(rectangle);
                    }
                    //Heures sans réservation
                    else if(_viewDate.DateSelect >= DateTime.Today)
                    {

                        Button bouton = new Button();
                        bouton.Background = new SolidColorBrush(Colors.White);
                        bouton.BorderBrush = null;
                        bouton.Margin = new Thickness(2, 2, 2, 2);
                        bouton.Cursor = Cursors.Hand;

                        string la_date = DateTime.Now.ToShortDateString() + " " + horaires[j];
                        ViewPlanning viewPlanning = new ViewPlanning(_viewPlanning, new dtoReservation(0, Convert.ToDateTime(la_date), null, 1, null, salle[i]), this);
                        Binding bind = new Binding("selectReservationCommand");
                        bind.Source = viewPlanning;
                        bouton.SetBinding(Button.CommandProperty, bind);

                        Grid.SetColumn(bouton, j + 1);
                        Grid.SetRow(bouton, i + 1);
                        grd_planning.Children.Add(bouton);
                    }
                }

                //Ajout des réservations
                string date = _viewDate.DateSelect.ToShortDateString();
                string[] jourMoisAn = date.Split('/');
                date = jourMoisAn[2] + "-" + jourMoisAn[1] + "-" + jourMoisAn[0];
                string joinWhere = "WHERE reservation.date LIKE '%" + date + "%' AND salle_id = " + i;
                List<dtoReservation> reservations = (List<dtoReservation>)daoReservation.select("*", joinWhere);
                for (int k = 0; k < reservations.Count; k++)
                {
                    int heure = horaires.IndexOf(reservations[k].Date.TimeOfDay.ToString());
                    Color c = Color.FromRgb(116, 172, 147);
                    Button bouton = new Button();
                    bouton.Background =  new SolidColorBrush(c);
                    bouton.BorderBrush = null;
                    bouton.Margin = new Thickness(5, 5, 5, 45);
                    bouton.Cursor = Cursors.Hand;

                    //Data binding
                    ViewPlanning viewPlanning = new ViewPlanning(_viewPlanning ,reservations[k], this);
                    Binding bind = new Binding("selectReservationCommand");
                    bind.Source = viewPlanning;
                    bouton.SetBinding(Button.CommandProperty, bind);

                    Grid.SetColumn(bouton, heure + 1);
                    Grid.SetRow(bouton, i);
                    if (_viewDate.DateSelect >= DateTime.Today)
                    {
                        var bouton_free = grd_planning.Children
                            .Cast<UIElement>()
                            .First(e => Grid.GetRow(e) == i && Grid.GetColumn(e) == heure + 1);
                        grd_planning.Children.Remove(bouton_free);
                    }
                    grd_planning.Children.Add(bouton);
                }
            } 
            #endregion
        }
    }
}
