using ModeleMetier.metier;
using ModeleMetier.modele;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
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


        private int _Id;
        private string _NomVille;
        private int _NumSalle;
        private string _themeSalle;
        private decimal _prixSalle;
        private string _horaireOuverture;
        private string _horaireFermeture;


        public viewModele(daoAvis daoAvis,daoSalle daoSalle, dtoSalle laSalle, daoVille daoVille, daoPartie daopartie, daoTheme daoTheme, MainWindow mainWindow, List<dtoSalle> les_salles, List<dtoAvis> les_avis)
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
            if(laSalle != null)
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

     
        public string NomVille
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

        public int NumSalle
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

        public string themeSalle
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

        public decimal prixSalle
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

        public string horaireSalleOuverture
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

        public string horaireSalleFermeture
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

        public Visibility IsVisible
        {

            get { return _isVisible; }
            set
            {
                _isVisible = value;
                OnPropertyChanged("IsVisible");
            }
        }
        public ICommand Modifier
        {
            get
            {
                if (this._commandModifier == null)
                    this._commandModifier = new RelayCommand(() => this.modifier(), () => true);


                return this._commandModifier;
            }

        }

        public ICommand Desarchiver
        {
            get
            {
                if (this._desarchiver == null)
                    this._desarchiver = new RelayCommand(() => this.desarchiver(), () => true);


                return this._desarchiver; 
            }

        }

        public ICommand Retour
        {
            get
            {
                if (this._commandRetour == null)
                    this._commandRetour = new RelayCommand(() => this.retour(), () => true);


                return this._commandRetour;
            }
        }

        public ICommand Valider
        {
            get
            {
                if (this._commandValider == null)
                    this._commandValider = new RelayCommand(() => this.valider(), () => true);


                return this._commandValider;
            }
        }

        public ICommand Archiver
        {
            get
            {
                 if (this._commandArchiver == null)
                    this._commandArchiver = new RelayCommand(() => this.archive(), () => true);


                return this._commandArchiver;
            }
        }

        public ICommand Statistiques
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
