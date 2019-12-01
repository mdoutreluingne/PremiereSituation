using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using ModeleMetier.metier;
using ModeleMetier.modele;

namespace ApplicationWPF.MVVM
{
    public sealed class ViewReservation : ViewModele
    {
        public bool Nouveau = true;
        private static ViewReservation _instance = null;
        private static readonly object _padlock = new object();
        private ViewPlanning _viewPlanning;

        private daoClient _daoClient;
        private daoVille _daoVille;
        private daoTransaction _daoTransaction;
        private Visibility _visibilite;
        private ObservableCollection<dtoSalle> _les_salles;
        private ObservableCollection<dtoClient> _les_clients;
        private ObservableCollection<dtoVille> _les_villes;
        private ObservableCollection<string> _les_heures;
        private readonly ICollectionView collectionViewSalles;
        private readonly ICollectionView collectionViewHeures;

        private bool _enablePayement;
        private int _id;
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

        private ICommand _commandFocusCommentaire;
        private ICommand _commandLostCommentaire;

        private ICommand _commandAnnulerResa;
        private ICommand _commandValiderResa;
        #endregion
        ViewReservation(MainWindow main, daoClient daoClient, daoVille daoVille, daoTransaction daoTransaction, List<dtoSalle> les_salles, List<string> les_heures, ViewPlanning viewPlanning)
            : base(main)
        {
            _viewPlanning = viewPlanning;
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
        public static ViewReservation Instance(MainWindow main, daoClient daoClient, daoVille daoVille, daoTransaction daoTransaction, List<dtoSalle> les_salles, List<string> les_heures, ViewPlanning viewPlanning, Visibility visibility)
        {
            lock (_padlock)
            {
                if (_instance == null)
                {
                    _instance = new ViewReservation(main, daoClient, daoVille, daoTransaction, les_salles, les_heures, viewPlanning);
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
        public bool EnablePayement
        {
            get
            {
                return _enablePayement;
            }
            set
            {
                _enablePayement = value;
                OnPropertyChanged("EnablePayement");
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
                    Nouveau = false;
                    _id = value.Id;
                    Salle = value.DtoSalle;
                    this.collectionViewSalles.MoveCurrentToPosition(value.DtoSalle.Numero - 1);
                    _client = value.Client;
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
                    if (((List<dtoTransaction>)_daoTransaction.select("*", "WHERE reservation_id = " + value.Id)).Count >= 1)
                    {
                        EnablePayement = false;
                    }
                    else
                    {
                        EnablePayement = true;
                    }
                }
                else
                {
                    EnablePayement = true;
                    Nouveau = true;
                    _id = -1;
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
                if (value != null)
                {
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
            else
            {
                LesClients = new ObservableCollection<dtoClient>((List<dtoClient>)_daoClient.select("*", "WHERE nom LIKE '%" + Nom + "%'"));
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
        public ICommand AnnulerResa
        {
            get
            {
                if (this._commandAnnulerResa == null)
                    this._commandAnnulerResa = new RelayCommand(() => this.annuler_reservation(), () => true);

                return this._commandAnnulerResa;
            }
        }
        public void annuler_reservation()
        {
            MessageBoxResult mr = MessageBox.Show("Êtes vous sûr de vouloir abandonner la réservation ?", "Abandon", MessageBoxButton.YesNo);
            if (mr == MessageBoxResult.Yes)
            {
                ViewReservation.Instance(null, null, null, null, null, null, null, Visibility.Hidden);
                ViewReservation.Instance(null, null, null, null, null, null, null, Visibility.Hidden).SelectClient = null;
                ViewDate.Instance(null, null, null, null).Visibilite = Visibility.Visible;
                _viewPlanning.Visibilite = Visibility.Visible;
            }
        }

        public ICommand ValiderResa
        {
            get
            {
                if (this._commandValiderResa == null)
                    this._commandValiderResa = new RelayCommand(() => this.valider_reservation(), () => true);

                return this._commandValiderResa;
            }
        }

        public void valider_reservation()
        {
            if (_client != null)
            {
                dtoReservation reservation = new dtoReservation(_id, _date, _commentaire, _nombreJoueur, _client, _salle);
                Visibilite = Visibility.Hidden;
                ViewObjet.Instance(null, null, null, null, null, null, reservation).Visibilite = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un client", "Reservation Invalide", MessageBoxButton.OK);
            }
        }
    }
}
