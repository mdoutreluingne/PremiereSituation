using ModeleMetier.metier;
using ModeleMetier.modele;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace ApplicationWPF
{
    public class ViewModele : INotifyPropertyChanged
    {
        protected MainWindow _mainWindow;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string caller = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(caller));
            }
        }

        public ViewModele(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }
    }

    public class viewEntete : ViewModele
    {
        private static viewEntete _instance = null;
        private static readonly object _padlock = new object();
        private viewPlanning _viewPlanning;

        private ICommand _icommandGoPlanning;
        viewEntete(MainWindow main, viewPlanning viewPlanning)
            :base(main)
        {
            _viewPlanning = viewPlanning;
        }

        //singleton
        public static viewEntete Instance(MainWindow main, viewPlanning viewPlanning)
        {
            lock (_padlock)
            {
                if (_instance == null)
                {
                    _instance = new viewEntete(main, viewPlanning);
                }
                return _instance;
            }
        }

        public ICommand GoPlanning
        {
            get
            {
                if (this._icommandGoPlanning == null)
                    this._icommandGoPlanning = new RelayCommand(() => this.goPlanning(), () => true);

                return this._icommandGoPlanning;
            }
        }

        public void goPlanning()
        {
            MessageBoxResult mr =  MessageBox.Show("Etes vous sûr de vouloir abandonner la réservation ?", "Abandon", MessageBoxButton.YesNo);
            if (mr == MessageBoxResult.Yes)
            {
                viewReservation.Instance(null, null, null, null, null, null, System.Windows.Visibility.Hidden);
                viewDate.Instance(null, null, null).Visibilite = System.Windows.Visibility.Visible;
                _viewPlanning.Visibilite = System.Windows.Visibility.Visible;
            }
        }


    }

    public class viewDate : ViewModele
    {
        private static viewDate _instance = null;
        private static readonly object _padlock = new object();

        private DateTime _date;
        private List<dtoSalle> _salle;
        private daoReservation _daoReservation;
        private ICommand _icommandPreview;
        private ICommand _icommandNext;
        private System.Windows.Visibility _visibilite;

        viewDate(List<dtoSalle> salle, daoReservation daoReservation, MainWindow main)
            :base(main)
        {
            _date = DateTime.Today;
            _salle = salle;
            _daoReservation = daoReservation;

        }

        //singleton
        public static viewDate Instance(List<dtoSalle> salle, daoReservation daoReservation, MainWindow main)
        {
            lock (_padlock)
            {
                if (_instance == null)
                {
                    _instance = new viewDate(salle, daoReservation, main);
                }
                return _instance;
            }
        }

        public DateTime DateSelect
        {
            get { return _date; }
            set {
                _date = value;
                _mainWindow.grd_planning.Children.Clear();
                _mainWindow.loadPlanning(_salle, _daoReservation);
                OnPropertyChanged("DateSelect");
            }
        }

        public ICommand SelectPreviewDate
        {
            get
            {
                if (this._icommandPreview == null)
                    this._icommandPreview = new RelayCommand(() => this.goPreviewDate(), () => true);

                return this._icommandPreview;
            }
        }

        public ICommand SelectNextDate
        {
            get
            {
                if (this._icommandNext == null)
                    this._icommandNext = new RelayCommand(() => this.goNextDate(), () => true);

                return this._icommandNext;
            }
        }

        protected void goPreviewDate()
        {
            DateSelect = Convert.ToDateTime(DateSelect).AddDays(-1);
        }

        protected void goNextDate()
        {
            DateSelect = Convert.ToDateTime(DateSelect).AddDays(1);
        }

        public System.Windows.Visibility Visibilite
        {
            get
            {
                return _visibilite;
            }
            set
            {
                _visibilite = value;
                OnPropertyChanged("Visibilite");
            }
        }
    }

    public class viewPlanning : ViewModele
    {
        private viewPlanning _origin;
        private dtoReservation _reservation;
        private System.Windows.Visibility _visibilite;
        private ICommand _icommand;
        public viewPlanning(viewPlanning origin, dtoReservation reservation, MainWindow main)
            :base(main)
        {
            _origin = origin;
            _reservation = reservation;
        }

        public ICommand selectReservationCommand
        {
            get
            {
                if (this._icommand == null)
                    this._icommand = new RelayCommand(() => this.goReservation(), () => true);

                return this._icommand;
            }
        }

        protected void goReservation()
        {
            _origin.Visibilite = System.Windows.Visibility.Hidden;
            viewDate.Instance(null, null, null).Visibilite = System.Windows.Visibility.Hidden;
            viewReservation.Instance(_mainWindow, null, null, null, null, null, System.Windows.Visibility.Visible).LoadReservation = _reservation;

        }
        public System.Windows.Visibility Visibilite
        {
            get
            {
                return _visibilite;
            }
            set
            {
                _visibilite = value;
                OnPropertyChanged("Visibilite");
            }
        }
    }

    public sealed class viewReservation : ViewModele
    {
        private static viewReservation _instance = null;
        private static readonly object _padlock = new object();

        private daoClient _daoClient;
        private daoVille _daoVille;
        private daoTransaction _daoTransaction;
        private System.Windows.Visibility _visibilite;
        private ObservableCollection<dtoSalle> _les_salles;
        private ObservableCollection<dtoClient> _les_clients;
        private ObservableCollection<dtoVille> _les_villes;
        private ObservableCollection<string> _les_heures;
        private readonly ICollectionView collectionViewSalles;
        private readonly ICollectionView collectionViewHeures;

        private dtoSalle _salle;
        private dtoClient _client;
        private string _nom;
        private string _prenom;
        private string _ville;
        private string _mail;
        private string _telephone;
        private DateTime _date;
        private string _heure;
        private decimal _solde;
        private int _nombreJoueur;
        private string _commentaire;

        /// <summary>
        /// Les attributs Icommands
        /// </summary>
        #region Attributs Icommand
        private ICommand _commandFocusNom;
        private ICommand _commandLostNom;

        private ICommand _commandFocusPrenom;
        private ICommand _commandLostPrenom;

        private ICommand _commandFocusVille;
        private ICommand _commandLostVille;

        private ICommand _commandFocusMail;
        private ICommand _commandLostMail;

        private ICommand _commandFocusTelephone;
        private ICommand _commandLostTelephone;

        private ICommand _commandFocusCommentaire;
        private ICommand _commandLostCommentaire;
        #endregion
        viewReservation(MainWindow main, daoClient daoClient,daoVille daoVille, daoTransaction daoTransaction, List<dtoSalle> les_salles, List<string> les_heures)
            :base(main)
        {
            _daoClient = daoClient;
            _daoVille = daoVille;
            _daoTransaction = daoTransaction;
            _les_salles = new ObservableCollection<dtoSalle>(les_salles);
            _les_clients = new ObservableCollection<dtoClient>();
            _les_heures = new ObservableCollection<string>(les_heures);
            this.collectionViewSalles = CollectionViewSource.GetDefaultView(this._les_salles);
            if (this.collectionViewSalles == null) throw new NullReferenceException("collectionView");
            this.collectionViewSalles.CurrentChanged += new EventHandler(this.OnCollectionViewCurrentChanged);

            this.collectionViewHeures = CollectionViewSource.GetDefaultView(this._les_heures);
            if (this.collectionViewHeures == null) throw new NullReferenceException("collectionView");
            this.collectionViewHeures.CurrentChanged += new EventHandler(this.OnCollectionViewCurrentChanged);

        }

        //singleton
        public static viewReservation Instance(MainWindow main, daoClient daoClient, daoVille daoVille, daoTransaction daoTransaction, List<dtoSalle> les_salles, List<string> les_heures, System.Windows.Visibility visibility)
        {
            lock (_padlock)
            {
                if (_instance == null)
                {
                    _instance = new viewReservation(main, daoClient, daoVille, daoTransaction, les_salles, les_heures);
                }
                _instance.Visibilite = visibility;
                return _instance;
            }
        }
        private void OnCollectionViewCurrentChanged(object sender, EventArgs e) 
        {
            if (this.collectionViewSalles.CurrentItem != null)
            {
                Salle = this.collectionViewSalles.CurrentItem as dtoSalle;
            }

            if (this.collectionViewHeures.CurrentItem != null)
            {
                Heure = this.collectionViewHeures.CurrentItem as string;
            }
        }

        public System.Windows.Visibility Visibilite
        {
            get
            {
                return _visibilite;
            }
            set
            {
                _visibilite = value;
                OnPropertyChanged("Visibilite");
            }
        }
        public ObservableCollection<dtoClient> LesClients
        {
            get
            {
                return _les_clients;
            }
            set
            {
                _les_clients = value;
                OnPropertyChanged("LesClients");
            }
        }
        public ObservableCollection<dtoSalle> LesSalles
        {
            get
            {
                return _les_salles;
            }
        }
        public ObservableCollection<dtoVille> LesVilles
        {
            get
            {
                return _les_villes;
            }
            set
            {
                _les_villes = value;
                OnPropertyChanged("LesVilles");
            }
        }
        public ObservableCollection<string> LesHeures
        {
            get
            {
                return _les_heures;
            }
        }
        public dtoReservation LoadReservation
        {
            set
            {
                if (value.Id != 0)
                {
                    Salle = value.DtoSalle;
                    this.collectionViewSalles.MoveCurrentToPosition(value.DtoSalle.Numero - 1);
                    Nom = value.Client.Nom;
                    Prenom = value.Client.Prenom;
                    Ville = value.Client.DtoVille.Nom;
                    Mail = value.Client.Mail;
                    Telephone = value.Client.Tel;
                    Date = value.Date;
                    Heure = value.Date.TimeOfDay.ToString();
                    this.collectionViewHeures.MoveCurrentTo(value.Date.TimeOfDay.ToString());
                    List<dtoTransaction> le_solde = (List<dtoTransaction>)_daoTransaction.select("id, date, SUM(montant) as montant, type, numero, commentaire, reservation_id, client_id", "WHERE client_id = " + value.Client.Id);
                    Solde = le_solde[0].Montant;
                    NombreJoueur = value.NbJoueur;
                    Commentaire = value.Commentaire;
                }
                else
                {
                    Salle = value.DtoSalle;
                    this.collectionViewSalles.MoveCurrentToPosition(value.DtoSalle.Numero - 1);
                    Date = value.Date;
                    Heure = value.Date.TimeOfDay.ToString();
                    this.collectionViewHeures.MoveCurrentTo(value.Date.TimeOfDay.ToString());
                    Nom = "NOM";
                    Prenom = "PRENOM";
                    Ville = "VILLE";
                    Mail = "MAIL";
                    Telephone = "TELEPHONE";
                    Solde = 0;
                    NombreJoueur = value.NbJoueur;
                    Commentaire = "COMMENTAIRE";
                }
            }
        }
        public dtoClient SelectClient
        {
            get
            {
                return null;
            }
            set
            {
                _client = value;
                Nom = value.Nom;
                Prenom = value.Prenom;
                Ville = value.DtoVille.Nom;
                Mail = value.Mail;
                Telephone = value.Tel;
                List<dtoTransaction> le_solde = (List<dtoTransaction>)_daoTransaction.select("id, date, SUM(montant) as 'montant', type, numero, commentaire, reservation_id, client_id", "WHERE client_id = " + value.Id);
                Solde = le_solde[0].Montant;
                OnPropertyChanged("SelectClient");
            }
        }


        /// <summary>
        /// Le contenue des textBoxs 
        /// </summary>
        #region TextBoxs
        public dtoSalle Salle
        {
            get
            {
                return _salle;
            }
            set
            {
                _salle = value;
                OnPropertyChanged("Salle");
            }
        }
        public string Nom
        {
            get
            {
                return _nom;
            }
            set
            {
                _nom = value;
                OnPropertyChanged("Nom");
            }
        }
        public string Prenom
        {
            get
            {
                return _prenom;
            }
            set
            {
                _prenom = value;
                OnPropertyChanged("Prenom");
            }
        }
        public string Ville
        {
            get
            {
                return _ville;
            }
            set
            {
                _ville = value;
                OnPropertyChanged("Ville");
            }
        }
        public string Mail
        {
            get
            {
                return _mail;
            }
            set
            {
                _mail = value;
                OnPropertyChanged("Mail");
            }
        }
        public string Telephone
        {
            get
            {
                return _telephone;
            }
            set
            {
                _telephone = value;
                OnPropertyChanged("Telephone");
            }
        }
        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged("Date");
            }
        }
        public string Heure
        {
            get
            {
                return _heure;
            }
            set
            {
                _heure = value;
                OnPropertyChanged("Heure");
            }
        }
        public decimal Solde
        {
            get
            {
                return _solde;
            }
            set
            {
                _solde = value;
                OnPropertyChanged("Solde");
            }
        }
        public int NombreJoueur
        {
            get
            {
                return _nombreJoueur;
            }
            set
            {
                _nombreJoueur = value;
                OnPropertyChanged("NombreJoueur");
            }
        }
        public string Commentaire
        {
            get
            {
                return _commentaire;
            }
            set
            {
                _commentaire = value;
                OnPropertyChanged("Commentaire");
            }
        } 
        #endregion

        /// <summary>
        /// Focus des TextBoxs
        /// </summary>
        #region Focus
        // --- Les Commandes--- //
        public ICommand FocusNom
        {
            get
            {
                if (this._commandFocusNom == null)
                    this._commandFocusNom = new RelayCommand(() => this.focus_nom(), () => true);

                return this._commandFocusNom;
            }
        }
        public ICommand FocusPrenom
        {
            get
            {
                if (this._commandFocusPrenom == null)
                    this._commandFocusPrenom = new RelayCommand(() => this.focus_prenom(), () => true);

                return this._commandFocusPrenom;
            }
        }
        public ICommand FocusVille
        {
            get
            {
                if (this._commandFocusVille == null)
                    this._commandFocusVille = new RelayCommand(() => this.focus_ville(), () => true);

                return this._commandFocusVille;
            }
        }
        public ICommand FocusMail
        {
            get
            {
                if (this._commandFocusMail == null)
                    this._commandFocusMail = new RelayCommand(() => this.focus_mail(), () => true);

                return this._commandFocusMail;
            }
        }
        public ICommand FocusTelephone
        {
            get
            {
                if (this._commandFocusTelephone == null)
                    this._commandFocusTelephone = new RelayCommand(() => this.focus_telephone(), () => true);

                return this._commandFocusTelephone;
            }
        }
        public ICommand FocusCommentaire
        {
            get
            {
                if (this._commandFocusCommentaire == null)
                    this._commandFocusCommentaire = new RelayCommand(() => this.focus_commentaire(), () => true);

                return this._commandFocusCommentaire;
            }
        }
        // --- --- --- //

        // --- Les Méthodes --- //
        public void focus_nom()
        {
            if (Nom == "NOM")
            {
                Nom = "";
            };
        }
        public void focus_prenom()
        {
            if (Prenom == "PRENOM")
            {
                Prenom = "";
            };
        }
        public void focus_ville()
        {
            if (Ville == "VILLE")
            {
                Ville = "";
            };
        }
        public void focus_mail()
        {
            if (Mail == "MAIL")
            {
                Mail = "";
            };
        }
        public void focus_telephone()
        {
            if (Telephone == "TELEPHONE")
            {
                Telephone = "";
            };
        }
        public void focus_commentaire()
        {
            if (Commentaire == "COMMENTAIRE")
            {
                Commentaire = "";
            };
        }
        // --- --- --- //
        #endregion

        /// <summary>
        /// lostFocus des TextBoxs
        /// </summary>
        #region LostFocus
        // --- Les Commandes--- //
        public ICommand LostFocusNom
        {
            get
            {
                if (this._commandLostNom == null)
                    this._commandLostNom = new RelayCommand(() => this.lostfocus_nom(), () => true);

                return this._commandLostNom;
            }
        }
        public ICommand LostFocusPrenom
        {
            get
            {
                if (this._commandLostPrenom == null)
                    this._commandLostPrenom = new RelayCommand(() => this.lostfocus_prenom(), () => true);

                return this._commandLostPrenom;
            }
        }
        public ICommand LostFocusVille
        {
            get
            {
                if (this._commandLostVille == null)
                    this._commandLostVille = new RelayCommand(() => this.lostfocus_ville(), () => true);

                return this._commandLostVille;
            }
        }
        public ICommand LostFocusMail
        {
            get
            {
                if (this._commandLostMail == null)
                    this._commandLostMail = new RelayCommand(() => this.lostfocus_mail(), () => true);

                return this._commandLostMail;
            }
        }
        public ICommand LostFocusTelephone
        {
            get
            {
                if (this._commandLostTelephone == null)
                    this._commandLostTelephone = new RelayCommand(() => this.lostfocus_telephone(), () => true);

                return this._commandLostTelephone;
            }
        }
        public ICommand LostFocusCommentaire
        {
            get
            {
                if (this._commandLostCommentaire == null)
                    this._commandLostCommentaire = new RelayCommand(() => this.lostfocus_commentaire(), () => true);

                return this._commandLostCommentaire;
            }
        }
        // --- --- --- //

        // --- Les Méthodes --- //
        public void lostfocus_nom()
        {
            if (Nom == "")
            {
                Nom = "NOM";
            }
            else if (Nom.Length >= 3)
            {
                LesClients = new ObservableCollection<dtoClient>((List<dtoClient>)_daoClient.select("*", "WHERE nom LIKE '%" + Nom + "%'"));
            }
        }
        public void lostfocus_prenom()
        {
            if (Prenom == "")
            {
                Prenom = "PRENOM";
            }
        }
        public void lostfocus_ville()
        {
            if (Ville == "")
            {
                Ville = "VILLE";
            }
            else if (Ville.Length >= 3)
            {
                LesVilles = new ObservableCollection<dtoVille>((List<dtoVille>)_daoVille.select("*", "WHERE nom LIKE '" + Ville + "%'"));
            }
        }
        public void lostfocus_mail()
        {
            if (Mail == "")
            {
                Mail = "MAIL";
            }
        }
        public void lostfocus_telephone()
        {
            if (Telephone == "")
            {
                Telephone = "TELEPHONE";
            }
        }
        public void lostfocus_commentaire()
        {
            if (Commentaire == "")
            {
                Commentaire = "COMMENTAIRE";
            }
        }
        // --- --- --- //
        #endregion
    }
}
