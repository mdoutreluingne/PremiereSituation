using ModeleMetier.metier;
using ModeleMetier.modele;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using MessageBox = System.Windows.Forms.MessageBox;

namespace WPFApplication
{
    public class viewModele : INotifyPropertyChanged
    {
        #region base PropertyChanged
        private MainWindow _main;
        private static viewModele _instance = null;
        private static readonly object _padlock = new object();

        public List<viewModele> _lesViews;
        private daoAvis _daoavis;
        private daoPartie _daopartie;
        private daoTheme _daoTheme;
        private daoVille _daoVille;
        private dtoSalle _salle;
        private daoSalle _daoSalle;
        private List<dtoSalle> _lesSalles;
        private List<dtoAvis> _lesAvis;
        public event PropertyChangedEventHandler PropertyChanged;

        private ICommand _commandModifier;
        private bool _enableModification;

        private Visibility _isVisible;

        private ICommand _commandRetour;
        private bool _disableProperty;

        private ICommand _commandValider;
        private bool _validerModif;

        private ICommand _commandArchiver;
        private ICommand _commandStatitique;
        private ICommand _desarchiver;

        private ICommand _ajout;


        private int _Id;
        private string _NomVille;
        private int _NumSalle;
        private string _themeSalle;
        private decimal _prixSalle;
        private string _horaireOuverture;
        private string _horaireFermeture;


        public viewModele(daoAvis daoAvis, daoSalle daoSalle, dtoSalle laSalle, daoVille daoVille, daoPartie daopartie, daoTheme daoTheme, MainWindow mainWindow, List<dtoSalle> les_salles, List<dtoAvis> les_avis)
        {
            _main = mainWindow;
            _daoavis = daoAvis;
            _daopartie = daopartie;
            _daoTheme = daoTheme;
            _daoSalle = daoSalle;
            _daoVille = daoVille;
            _salle = laSalle;
            _lesSalles = les_salles;
            _lesAvis = les_avis;
            if (laSalle != null)
            {
                _Id = laSalle.Id;
                _NomVille = laSalle.DtoVille.Nom;
                _NumSalle = laSalle.Numero;
                _themeSalle = laSalle.DtoTheme.Nom.ToString();
                _prixSalle = laSalle.Prix;
                _horaireOuverture = laSalle.Heure_ouverture.ToShortTimeString();
                _horaireFermeture = laSalle.Heure_fermeture.ToShortTimeString();
            }
            IsVisible = Visibility.Hidden;
        }


        public string NomVille //gère les villes des salles 
        {
            get
            {
                return _NomVille;
            }
            set
            {
                _NomVille = value;
                OnPropertyChanged("NomVille");
            }
        }

        public int NumSalle //gère les numero des salles 
        {
            get
            {
                return _NumSalle;
            }
            set
            {
                _NumSalle = value;
                OnPropertyChanged("NumSalle");
            }
        }

        public string themeSalle  //gère les themes des salles 
        {
            get
            {
                return _themeSalle;
            }
            set
            {
                _themeSalle = value;
                OnPropertyChanged("themeSalle");
            }
        }

        public decimal prixSalle //gère les prix des salles 
        {
            get
            {
                return _prixSalle;
            }
            set
            {
                _prixSalle = value;
                OnPropertyChanged("prixSalle");
            }
        }

        public string horaireSalleOuverture //gère les heures d'ouverture des salles 
        {
            get
            {
                return _horaireOuverture;
            }
            set
            {
                _horaireOuverture = value;
                OnPropertyChanged("horaireSalleOuverture");
            }
        }

        public string horaireSalleFermeture//gère les heures de fermeture des salles 
        {
            get
            {
                return _horaireFermeture;
            }
            set
            {
                _horaireFermeture = value;
                OnPropertyChanged("horaireSalleFermeture");
            }
        }

    
        

        public void OnPropertyChanged([CallerMemberName]string caller = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(caller));
            }
        }

        public Visibility IsVisible //permet de recuperer et modifié la valeure de la visibilité d'un évènement
        {

            get { return _isVisible; }
            set
            {
                _isVisible = value;
                OnPropertyChanged("IsVisible");
            }
        }
        public ICommand Modifier //permet de gérer les différents évènements prévus lors des modifications en faisant appel à la méthode modifier
        {
            get
            {
                if (this._commandModifier == null)
                    this._commandModifier = new RelayCommand(() => this.modifier(), () => true);


                return this._commandModifier;
            }

        }

        public ICommand Desarchiver //permet de gérer le bouton permettant de désarchiver une salle dans l'application  en faisant appel à la méthode desarchiver
        {
            get
            {
                if (this._desarchiver == null)
                    this._desarchiver = new RelayCommand(() => this.desarchiver(), () => true);


                return this._desarchiver; 
            }

        }

        public ICommand Retour //permet le retour en arrière dans l'application en faisant appel à la méthode retour 
        {
            get
            {
                if (this._commandRetour == null)
                    this._commandRetour = new RelayCommand(() => this.retour(), () => true);


                return this._commandRetour;
            }
        }

        public ICommand Ajout
        {
            get
            {
                if (this._ajout == null)
                    this._ajout = new RelayCommand(() => this.ajout(), () => true);


                return this._ajout;
            }
        }
        public ICommand Valider //permet de valider les modifications en faisant appel à la méthode valider 
        {
            get
            {
                if (this._commandValider == null)
                    this._commandValider = new RelayCommand(() => this.valider(), () => true);


                return this._commandValider;
            }
        }

        public ICommand Archiver //permet d'archiver une salle en cliquant sur un bouton  en faisant appel à la méthode archive
        {
            get
            {
                 if (this._commandArchiver == null)
                    this._commandArchiver = new RelayCommand(() => this.archive(), () => true);


                return this._commandArchiver;
            }
        }

        public ICommand Statistiques //permet d'afficher les statistiques d'une salle en cliquant sur un bouton en faisant appel à la méthode statistique
        {
            get
            {
                if (this._commandStatitique == null)
                    this._commandStatitique = new RelayCommand(() => this.statistiques(), () => true);


                return this._commandStatitique;
            }
        }

        public void statistiques()
        {
            DataTableCollection table =  (DataTableCollection)_daopartie.select("MIN(temps) as temps, nom, prenom", "JOIN reservation ON reservation.id = reservation_id "
             + "JOIN salle ON salle.id = reservation.salle_id "
             + "JOIN client ON client.id = reservation.client_id "
             + "WHERE salle.id =" + _salle.Id);
            string message = "Temps minimum : " + table[0].Rows[0]["temps"];
            message += "\nRecord par : " + table[0].Rows[0]["nom"] + " " + table[0].Rows[0]["prenom"];

            table = (DataTableCollection)_daopartie.select("MAX(temps) as temps", "JOIN reservation ON reservation.id = reservation_id "
             + "JOIN salle ON salle.id = reservation.salle_id "
             + "JOIN client ON client.id = reservation.client_id "
             + "WHERE salle.id = " + _salle.Id);
            message += "\nTemps max : " + table[0].Rows[0]["temps"];

            //moyenne

            MessageBox.Show(message, "Statistiques", MessageBoxButtons.OK);

        }

        public bool DisableModification
        {
            get
            {
                return _disableProperty;
            }
            set
            {
                _disableProperty = value;
                OnPropertyChanged("DisableModification");
            }
        }

        public bool EnableModification
        {
            get
            {
                return _enableModification;
            }
            set
            {
                _enableModification = value; 
                foreach (viewModele v in _lesViews)
                {
                    v._lesViews = new List<viewModele>();
                    v.EnableModification = value; 
                }              
                OnPropertyChanged("EnableModification");
            }
        }
        #endregion

        #region Méthodes
        private void modifier()
        {          
            EnableModification = true;
            IsVisible = Visibility.Visible;

        }

        private void retour()
        {
            EnableModification = false;
            IsVisible = Visibility.Hidden;
            List<dtoSalle> salles = (List<dtoSalle>)_daoSalle.select("*", "Where archive = false ORDER BY id");
            _main.grd_listSalle.Children.Clear();
            _main.grd_listSalle.ColumnDefinitions.Clear();
            _main.loadPage(salles, _lesAvis);
        }

    private void desarchiver()
        {
            List<dtoSalle> salles = (List<dtoSalle>)_daoSalle.select("*", "Where archive = true ORDER BY id");
            _main.grd_listSalle.Children.Clear();
            _main.grd_listSalle.ColumnDefinitions.Clear();
            _main.loadPage(salles, _lesAvis);
        }

        private void valider()
        {
            //List<dtoAvis> lesavis = ((List<dtoAvis>)_daoAvis.select("*", "WHERE salle_id =" + el_verfificator.Tag));
            string message = "Voulez-vous validez vos changements?";
            string title = "Validation";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);

            if (result == DialogResult.Yes)
            {
                //pour chaque salle -> update
                foreach (viewModele vm in _lesViews)
                {

                    List<dtoVille> les_villes = (List<dtoVille>)_daoVille.select("*", "WHERE nom = '" + vm._NomVille + "'");
                    List<dtoTheme> les_themes = (List<dtoTheme>)_daoTheme.select("*", "WHERE nom = '" + vm.themeSalle + "'");
                    if (les_villes.Count > 0 && les_themes.Count > 0)
                    {
                        dtoSalle salle = new dtoSalle(-1, les_villes[0], vm.NumSalle, vm.prixSalle, Convert.ToDateTime(vm.horaireSalleOuverture), Convert.ToDateTime(vm.horaireSalleFermeture), false, les_themes[0]);
                        _daoSalle.update(salle, vm._Id);
                    }
                    else
                    {
                        MessageBox.Show("Une des villes ou un des thèmes entrées n'existe pas !", "Erreur de saisie", MessageBoxButtons.OK);
                    }
                }
            }
            else
            {
               
            }
            DisableModification = true;
            IsVisible = Visibility.Hidden;
            EnableModification = false;
        }

        public void ajout()
        {
            string message = "Voulez-vous ajouter une salle ?";
            string title = "Validation";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            if(result == DialogResult.Yes)
            {
                Window addSalle = new Window();

                Label lbl_Ville = new Label();
                Label lbl_numero = new Label();
                Label lbl_prix = new Label();               
                Label lbl_heure_ouverture = new Label();
                Label lbl_fermeture = new Label();
                Label lbl_archive = new Label();
                Label lbl_theme = new Label();

                TextBox txt_ville_id = new TextBox();
                TextBox txt_numero = new TextBox();
                TextBox txt_prix = new TextBox();
                TextBox txt_heure_ouverture = new TextBox();
                TextBox txt_heure_fermeture = new TextBox();
                TextBox txt_archive = new TextBox();
                TextBox txt_theme = new TextBox();



                addSalle.Content = lbl_Ville.Text = "Dans quelle ville ?: ";
                addSalle.Content = addSalle.Content.ToString() + txt_ville_id.Enabled;



                addSalle.Show(); 

            }



        }



        public void compteLesView(List<viewModele> uneListe)
        {
            _lesViews = uneListe; 
        }

        public void archive()
        {
            if(!_lesSalles[0].Archive)
            {
                string message = "Voulez-vous validez l'archivage?";
                string title = "Validation";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(message, title, buttons);
                if (result == DialogResult.Yes)
                {
                    _daoSalle.archive(_Id, 1);
                    List<dtoSalle> salles = (List<dtoSalle>)_daoSalle.select("*", "Where archive = false ORDER BY id");
                    _main.grd_listSalle.Children.Clear();
                    _main.grd_listSalle.ColumnDefinitions.Clear();
                    _main.loadPage(salles, _lesAvis);

                }
                DisableModification = true;
                IsVisible = Visibility.Hidden;
                EnableModification = false;
            }
            else
            {
                string message = "Voulez-vous désarchiver?";
                string title = "Validation";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(message, title, buttons);
                if (result == DialogResult.Yes)
                {
                    _daoSalle.archive(_Id, 0);
                    List<dtoSalle> salles = (List<dtoSalle>)_daoSalle.select("*", "Where archive = true ORDER BY id");
                    _main.grd_listSalle.Children.Clear();
                    _main.grd_listSalle.ColumnDefinitions.Clear();
                    _main.loadPage(salles, _lesAvis);

                }
                DisableModification = true;
                IsVisible = Visibility.Hidden;
                EnableModification = false;
            }
        }

 

        #endregion

    }
}
