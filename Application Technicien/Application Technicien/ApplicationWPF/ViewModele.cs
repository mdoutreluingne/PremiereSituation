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

    public class ViewEntete : ViewModele
    {
        private static ViewEntete _instance = null;
        private static readonly object _padlock = new object();
        private ViewPlanning _viewPlanning;

        private ICommand _icommandGoPlanning;
        ViewEntete(MainWindow main, ViewPlanning viewPlanning)
            :base(main)
        {
            _viewPlanning = viewPlanning;
        }

        //singleton
        public static ViewEntete Instance(MainWindow main, ViewPlanning viewPlanning)
        {
            lock (_padlock)
            {
                if (_instance == null)
                {
                    _instance = new ViewEntete(main, viewPlanning);
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
            if (_viewPlanning.Visibilite == Visibility.Hidden)
            {
                MessageBoxResult mr = MessageBox.Show("Êtes vous sûr de vouloir abandonner la réservation ?", "Abandon", MessageBoxButton.YesNo);
                if (mr == MessageBoxResult.Yes)
                {
                    ViewReservation.Instance(null, null, null, null, null, null, null, Visibility.Hidden);
                    ViewReservation.Instance(null, null, null, null, null, null, null, Visibility.Hidden).SelectClient = null;
                    ViewObjet.Instance(null, null, null, null, null, null, null).Visibilite = Visibility.Hidden;
                    ViewDate.Instance(null,null, null, null).Visibilite = Visibility.Visible;
                    _viewPlanning.Visibilite = Visibility.Visible;
                }
            }
        }


    }

    public class ViewDate : ViewModele
    {
        private static ViewDate _instance = null;
        private static readonly object _padlock = new object();

        private DateTime _date;
        private List<dtoSalle> _salle;
        private daoReservation _daoReservation;
        private daoTransaction _daoTransaction;
        private ICommand _icommandPreview;
        private ICommand _icommandNext;
        private System.Windows.Visibility _visibilite;

        ViewDate(List<dtoSalle> salle, daoReservation daoReservation, daoTransaction daoTransaction, MainWindow main)
            :base(main)
        {
            _date = DateTime.Today;
            _salle = salle;
            _daoReservation = daoReservation;
            _daoTransaction = daoTransaction;

        }

        //singleton
        public static ViewDate Instance(List<dtoSalle> salle, daoReservation daoReservation, daoTransaction daoTransaction, MainWindow main)
        {
            lock (_padlock)
            {
                if (_instance == null)
                {
                    _instance = new ViewDate(salle, daoReservation, daoTransaction, main);
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
                _mainWindow.loadPlanning(_salle, _daoReservation, _daoTransaction);
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

    public class ViewPlanning : ViewModele
    {
        private ViewPlanning _origin;
        private dtoReservation _reservation;
        private System.Windows.Visibility _visibilite;
        private ICommand _icommand;
        public ViewPlanning(ViewPlanning origin, dtoReservation reservation, MainWindow main)
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
            ViewDate.Instance(null, null, null, null).Visibilite = System.Windows.Visibility.Hidden;
            ViewReservation.Instance(_mainWindow, null, null, null, null, null, null, System.Windows.Visibility.Visible).LoadReservation = _reservation;

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

    public sealed class ViewReservation : ViewModele
    {
        public bool Nouveau = true;
        private static ViewReservation _instance = null;
        private static readonly object _padlock = new object();
        private ViewPlanning _viewPlanning;

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
        ViewReservation(MainWindow main, daoClient daoClient,daoVille daoVille, daoTransaction daoTransaction, List<dtoSalle> les_salles, List<string> les_heures, ViewPlanning viewPlanning)
            :base(main)
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
                ViewObjet.Instance(null,null, null, null, null, null, reservation).Visibilite = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un client", "Reservation Invalide", MessageBoxButton.OK);
            }
        }
    }

    public class ViewObjet : ViewModele
    {
        private static ViewObjet _instance = null;
        private static readonly object _padlock = new object();
        private daoTransaction _daoTransaction;
        private daoReservation _daoReservation;
        private daoArticle _daoArticle;
        private daoObstacle _daoObstacle;

        public Visibility _visibilite;
        public Visibility _visibiliteAjout;
        public Visibility _visibiliteModif;
        public Visibility _visibiliteDelete;
        public Visibility _visibiliteSupPaye;

        private ViewPlanning _viewPlanning;
        private dtoSalle _salle;
        private dtoArticle _articleSelect;
        private dtoObstacle _obstacleSelect;
        private dtoReservation _reservation;
        private bool _enableArticle;
        private bool _enableObstacle;
        private bool _enablePayement;
        private string _passeur;

        private bool _nouveau;
        private string _nomObstacle;
        private int _positionObstacle;
        private int _quantiteObstacle;
        private decimal _prixObstacle;
        private string _commentaireObstacle;
        private decimal _prix;
        private decimal _solde;
        private ObservableCollection<dtoArticle> _les_articles;
        private ObservableCollection<dtoObstacle> _les_obstacles;
        private ICollectionView collectionViewArticles;
        private ICollectionView collectionViewObstacles;
        private ICommand _commandAnnulerObstacle;
        private ICommand _commandPlacerObstacle;
        private ICommand _commandModifierObstacle;
        private ICommand _commandDeleteObstacle;
        private ICommand _commandPrecedent;
        private ICommand _commandFinResa;
        private ICommand _commandSupprimerReservation;
        private ICommand _commandPayerReservation;

        ViewObjet(MainWindow main,daoTransaction daoTransaction, daoReservation daoReservation, daoArticle daoArticle, daoObstacle daoObstacle, ViewPlanning viewPlanning)
            : base(main)
        {
            _daoTransaction = daoTransaction;
            _daoReservation = daoReservation;
            _daoArticle = daoArticle;
            _daoObstacle = daoObstacle;
            _viewPlanning = viewPlanning;
            Visibilite = Visibility.Hidden;
            VisibiliteAjout = Visibility.Hidden;
            VisibiliteModif = Visibility.Hidden;
            VisibiliteDelete = Visibility.Hidden;
            VisibiliteSupPaye = Visibility.Hidden;
            EnableArticle = true;
            EnableObstacle = true;
            _les_obstacles = new ObservableCollection<dtoObstacle>();
            _les_articles = new ObservableCollection<dtoArticle>();
        }

        //singleton
        public static ViewObjet Instance(MainWindow main,daoTransaction daoTransaction, daoReservation daoReservation, daoArticle daoArticle, daoObstacle daoObstacle, ViewPlanning viewPlanning,dtoReservation dtoReservation)
        {
            lock (_padlock)
            {
                if (_instance == null)
                {
                    _instance = new ViewObjet(main,daoTransaction, daoReservation,daoArticle, daoObstacle, viewPlanning);
                }
                if (dtoReservation != null)
                {
                    _instance._nouveau = ViewReservation.Instance(null, null, null, null, null, null, null, Visibility.Hidden).Nouveau;
                    _instance.Reservation = dtoReservation;
                    _instance.Salle = dtoReservation.DtoSalle;
                    string joinWhere = " JOIN article_salle ON article.id = article_salle.article_id"
                        + " JOIN salle ON article_salle.salle_id = salle.id"
                        + " WHERE salle.id = "+ _instance.Salle.DtoTheme.Id + " AND archive = 0";
                    _instance._les_articles = new ObservableCollection<dtoArticle>((List<dtoArticle>)_instance._daoArticle.select("*", joinWhere​));
                    _instance.LesArticles = null;
                    _instance.Solde = ViewReservation.Instance(null, null, null, null, null, null, null, Visibility.Hidden).Solde;
                    if (!_instance._nouveau)
                    {
                        _instance.VisibiliteSupPaye = Visibility.Visible;
                        if (((List<dtoTransaction>)_instance._daoTransaction.select("*","WHERE reservation_id = " + _instance.Reservation.Id)).Count >= 1)
                        {
                            _instance.EnablePayement = false;
                            _instance.EnableArticle = false;
                        }
                        else
                        {
                            _instance.EnablePayement = true;
                            _instance.EnableArticle = true;
                        }
                        _instance._les_obstacles = new ObservableCollection<dtoObstacle>((List<dtoObstacle>)_instance._daoObstacle.select("*", "WHERE reservation_id = " + dtoReservation.Id));
                        _instance.LesObstacles = null;
                    }
                    else
                    {
                        _instance.EnablePayement = true;
                        _instance.EnableArticle = true;
                        _instance.VisibiliteSupPaye = Visibility.Hidden;
                        _instance._les_obstacles.Clear();
                    }

                    _instance.collectionViewArticles = CollectionViewSource.GetDefaultView(_instance._les_articles);
                    if (_instance.collectionViewArticles == null) throw new NullReferenceException("collectionView");
                    _instance.collectionViewArticles.CurrentChanged += new EventHandler(_instance.OnCollectionViewCurrentChanged);

                    _instance.collectionViewObstacles = CollectionViewSource.GetDefaultView(_instance._les_obstacles);
                    if (_instance.collectionViewObstacles == null) throw new NullReferenceException("collectionView");
                    _instance.collectionViewObstacles.CurrentChanged += new EventHandler(_instance.OnCollectionViewCurrentChanged);
                    _instance.Prix = 0;
                }
                return _instance;
            }
        }

        private void OnCollectionViewCurrentChanged(object sender, EventArgs e)
        {
            if (((ICollectionView)sender).CurrentItem != null)
            {
                if (this.collectionViewArticles.CurrentItem != null && ((ICollectionView)sender).CurrentItem.GetType() == typeof(dtoArticle))
                {
                    ArticleSelect = this.collectionViewArticles.CurrentItem as dtoArticle;
                }

                if (this.collectionViewObstacles.CurrentItem != null && ((ICollectionView)sender).CurrentItem.GetType() == typeof(dtoObstacle))
                {
                    ObstacleSelect = this.collectionViewObstacles.CurrentItem as dtoObstacle;
                }
            }
        }
        public  ObservableCollection<dtoArticle> LesArticles
        {
            get
            {
                return _les_articles;
            }
            set
            {
                OnPropertyChanged("LesArticles");
            }
        }

        public ObservableCollection<dtoObstacle> LesObstacles
        {
            get
            {
                return _les_obstacles;
            }
            set
            {
                OnPropertyChanged("LesObstacles");
            }
        }

        public dtoArticle ArticleSelect
        {
            get => _articleSelect;
            set
            {
                _passeur = "Article";
                VisibiliteAjout = Visibility.Visible;
                _articleSelect = value;
                NomObstacle = value.Libelle;
                PrixObstacle = value.Montant;
                QuantiteObstacle = 1;
                PositionObstacle = 1;
                CommentaireObstacle = "";
                OnPropertyChanged("ArticleSelect");
            }
        }
        public dtoObstacle ObstacleSelect
        {
            get => _obstacleSelect;
            set
            {
                _obstacleSelect = value;
                VisibiliteModif = Visibility.Visible;
                VisibiliteDelete = Visibility.Visible;
                OnPropertyChanged("ObstacleSelect");
            }
        }
        public Visibility Visibilite
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
        public Visibility VisibiliteAjout
        {
            get
            {
                return _visibiliteAjout;
            }
            set
            {
                _visibiliteAjout = value;
                EnableArticle = false;
                EnableObstacle = false;
                OnPropertyChanged("VisibiliteAjout");
            }
        }
        public Visibility VisibiliteModif
        {
            get
            {
                return _visibiliteModif;
            }
            set
            {
                _visibiliteModif = value;
                OnPropertyChanged("VisibiliteModif");
            }
        }
        public Visibility VisibiliteDelete
        {
            get
            {
                return _visibiliteDelete;
            }
            set
            {
                _visibiliteDelete = value;
                OnPropertyChanged("VisibiliteDelete");
            }
        }
        public Visibility VisibiliteSupPaye
        {
            get
            {
                return _visibiliteSupPaye;
            }
            set
            {
                _visibiliteSupPaye = value;
                OnPropertyChanged("VisibiliteSupPaye");
            }
        }

        public bool EnableArticle
        {
            get
            {
                return _enableArticle;
            }
            set
            {
                _enableArticle = value;
                OnPropertyChanged("EnableArticle");
            }
        }
        public bool EnableObstacle
        {
            get
            {
                return _enableObstacle;
            }
            set
            {
                _enableObstacle = value;
                OnPropertyChanged("EnableObstacle");
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
        public dtoReservation Reservation
        {
            get
            {
                return _reservation;
            }
            set
            {
                _reservation = value;
            }
        }
        public dtoSalle Salle
        {
            get
            {
                return _salle;
            }
            set
            {
                _salle = value;
            }
        }
        public string NomObstacle
        {
            get
            {
                return _nomObstacle;
            }
             set
            {
                if (value != "")
                {
                    _nomObstacle = value;
                }
                else
                {
                    _nomObstacle = "NOM";
                }
                OnPropertyChanged("NomObstacle");
            }
        }
        public int PositionObstacle
        {
            get
            {
                return _positionObstacle;
            }
            set
            {
                _positionObstacle = value;
                OnPropertyChanged("PositionObstacle");
            }
        }
        public int QuantiteObstacle
        {
            get
            {
                return _quantiteObstacle;
            }
            set
            {
                _quantiteObstacle = value;
                OnPropertyChanged("QuantiteObstacle");
            }
        }
        public decimal PrixObstacle
        {
            get
            {
                return _prixObstacle;
            }
            set
            {
                _prixObstacle = value;
                OnPropertyChanged("PrixObstacle");
            }
        }
        public decimal Prix
        {
            get
            {
                return _prix;
            }
            set
            {
                _prix = 0;
                _prix = _salle.Prix;
                foreach (var o in _les_obstacles)
                {
                    _prix += o.Qte * o.DtoArticle.Montant;
                }
                OnPropertyChanged("Prix");
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
        public string CommentaireObstacle
        {
            get
            {
                return _commentaireObstacle;
            }
            set
            {
                if (value != "")
                {
                    _commentaireObstacle = value;
                }
                else
                {
                    _commentaireObstacle = "COMMENTAIRE";
                }
                OnPropertyChanged("CommentaireObstacle");
            }
        }
        public ICommand AnnulerObstacle
        {
            get
            {
                if (this._commandAnnulerObstacle == null)
                    this._commandAnnulerObstacle = new RelayCommand(() => this.annulerObstacle(), () => true);

                return this._commandAnnulerObstacle;
            }
        }
        public ICommand PlacerObstacle
        {
            get
            {
                if (this._commandPlacerObstacle == null)
                    this._commandPlacerObstacle = new RelayCommand(() => this.placerObstacle(), () => true);

                return this._commandPlacerObstacle;
            }
        }
        public ICommand ModifierObstacle
        {
            get
            {
                if (this._commandModifierObstacle == null)
                    this._commandModifierObstacle = new RelayCommand(() => this.modifierObstacle(), () => true);

                return this._commandModifierObstacle;
            }
        }
        public ICommand DeleteObstacle
        {
            get
            {
                if (this._commandDeleteObstacle == null)
                    this._commandDeleteObstacle = new RelayCommand(() => this.deleteObstacle(), () => true);

                return this._commandDeleteObstacle;
            }
        }
        public ICommand Precedent
        {
            get
            {
                if (this._commandPrecedent == null)
                    this._commandPrecedent = new RelayCommand(() => this.precedent(), () => true);

                return this._commandPrecedent;
            }
        }
        public ICommand FinReservation
        {
            get
            {
                if (this._commandFinResa == null)
                    this._commandFinResa = new RelayCommand(() => this.finResa(), () => true);

                return this._commandFinResa;
            }
        }
        public ICommand SupprimerReservation
        {
            get
            {
                if (this._commandSupprimerReservation == null)
                    this._commandSupprimerReservation = new RelayCommand(() => this.supprimerReservation(), () => true);

                return this._commandSupprimerReservation;
            }
        }
        public ICommand PayerReservation
        {
            get
            {
                if (this._commandPayerReservation == null)
                    this._commandPayerReservation = new RelayCommand(() => this.payerReservation(), () => true);

                return this._commandPayerReservation;
            }
        }
        public void placerObstacle()
        {
            dtoObstacle existe = null;
            dtoObstacle obstacle = null;
            if (_passeur == "Article")
            {
                obstacle = new dtoObstacle(-1, _positionObstacle, _commentaireObstacle, _quantiteObstacle, _articleSelect, _reservation);
            }
            else
            {
                obstacle = new dtoObstacle(-1, _positionObstacle, _commentaireObstacle, _quantiteObstacle, _obstacleSelect.DtoArticle, _reservation);
            }
            foreach (var o in _les_obstacles)
            {
                if (o.DtoArticle.Libelle == obstacle.DtoArticle.Libelle)
                {
                    existe = o;
                }
            }
            if (existe == null)
            {
                _les_obstacles.Add(obstacle);
                LesObstacles = null;
            }
            else
            {
                _les_obstacles.Remove(existe);
                _les_obstacles.Add(obstacle);
                LesObstacles = null;
            }
            Prix = 0;
            VisibiliteAjout = Visibility.Hidden;
            EnableArticle = true;
            EnableObstacle = true;
        }
        public void annulerObstacle()
        {
            MessageBoxResult mr = MessageBox.Show("Êtes vous sûr de vouloir abandonner l'ajout de l'objet ?", "Abandon", MessageBoxButton.YesNo);
            if (mr == MessageBoxResult.Yes)
            {
                VisibiliteAjout = Visibility.Hidden;
                EnableArticle = true;
                EnableObstacle = true;
            }
        }
        public void modifierObstacle()
        {
            _passeur = "Obstacle";
            VisibiliteAjout = Visibility.Visible;
            NomObstacle = _obstacleSelect.DtoArticle.Libelle;
            PrixObstacle = _obstacleSelect.DtoArticle.Montant;
            QuantiteObstacle = _obstacleSelect.Qte;
            PositionObstacle = _obstacleSelect.Position;
            CommentaireObstacle = _obstacleSelect.Commentaire;
        }
        public void deleteObstacle()
        {
            MessageBoxResult mr = MessageBox.Show("Êtes vous sûr de vouloir supprimer l'obstacle ?", "Suppression", MessageBoxButton.YesNo);
            if (mr == MessageBoxResult.Yes)
            {
                _les_obstacles.Remove(_obstacleSelect);
                LesObstacles = null;
                if (_les_obstacles.Count == 0)
                {
                    VisibiliteDelete = Visibility.Hidden;
                    VisibiliteModif = Visibility.Hidden;
                }
            }
        }
        public void precedent()
        {
            Visibilite = Visibility.Hidden;
            ViewReservation.Instance(null, null, null, null, null, null, null, System.Windows.Visibility.Visible);
        }
        public void finResa()
        {
            string obstaclesString = "";
            foreach (var o in _les_obstacles)
            {
                obstaclesString += "    - " + o + "\n";
            }
            string message = "===== Résumé de la réservation ====="
                + "\nClient : " + _reservation.Client
                + "\nSalle : " + _reservation.DtoSalle.Numero
                + "\nDate : " + _reservation.Date
                + "\nNombre de joueurs : " + _reservation.NbJoueur
                + "\nCommentaire : " + _reservation.Commentaire
                + "\nObstacles : \n" + obstaclesString
                + "\nPrix Total : " + _prix
                + "\nSolde Restant : " + (_solde - _prix);
            if (_solde - _prix <= 0)
            {
                message += "\n\n /!\\ Solde non suffisant, de l'argent doit être ajouté au préalable !"
                        + "\nVoulez vous quand même sauvegarder la réservation ?";
            }
            else
            {
                message += "\n\nVoulez vous valider la réservation ?";
            }
            MessageBoxResult mr =  MessageBox.Show(message, "Valider la réservation", MessageBoxButton.YesNo);
            if (mr == MessageBoxResult.Yes)
            {
                if (!_nouveau)
                {
                    _daoReservation.update(_reservation, "WHERE id = " + _reservation.Id);
                    _daoObstacle.delete("WHERE reservation_id = " + _reservation.Id);
                }
                else
                {
                    _daoReservation.insert(_reservation);
                }
                foreach (var o in _les_obstacles)
                {
                    _daoObstacle.insert(o);
                }
                ViewDate.Instance(null, null, null, null).DateSelect = DateTime.Today;
                _les_obstacles.Clear();
                Visibilite = Visibility.Hidden;
                _viewPlanning.Visibilite = Visibility.Visible;
                ViewDate.Instance(null, null, null, null).Visibilite = Visibility.Visible;
                ViewReservation.Instance(null, null, null, null, null, null, null, Visibility.Hidden).SelectClient = null;
            }
        }
        public void supprimerReservation()
        {
            MessageBoxResult mr = MessageBox.Show("Êtes vous sûr de vouloir supprimer la réservation ?", "Suppression", MessageBoxButton.YesNo);
            if (mr == MessageBoxResult.Yes)
            {
                _daoReservation.delete("WHERE id = " + _reservation.Id);
                _daoObstacle.delete("WHERE reservation_id = " + _reservation.Id);
                ViewDate.Instance(null, null, null, null).DateSelect = DateTime.Today;
                _les_obstacles.Clear();
                Visibilite = Visibility.Hidden;
                _viewPlanning.Visibilite = Visibility.Visible;
                ViewDate.Instance(null, null, null, null).Visibilite = Visibility.Visible;
                ViewReservation.Instance(null, null, null, null, null, null, null, Visibility.Hidden).SelectClient = null;
            }
        }
        public void payerReservation()
        {
            MessageBoxResult mr = MessageBox.Show("Êtes vous sûr de vouloir payer la réservation ?", "Suppression", MessageBoxButton.YesNo);
            if (mr == MessageBoxResult.Yes)
            {
                if (_solde - _prix <= 0)
                {
                    MessageBox.Show("Crédit insuffisant !", "Erreur crédit", MessageBoxButton.OK);
                }
                else
                {
                    _daoTransaction.insert(_reservation, Prix);
                    ViewDate.Instance(null, null, null, null).DateSelect = DateTime.Today;
                    _les_obstacles.Clear();
                    Visibilite = Visibility.Hidden;
                    _viewPlanning.Visibilite = Visibility.Visible;
                    ViewDate.Instance(null, null, null, null).Visibilite = Visibility.Visible;
                    ViewReservation.Instance(null, null, null, null, null, null, null, Visibility.Hidden).SelectClient = null;
                }
            }
        }

    }

    public class ViewArticle : ViewModele
    {
        #region Attributs
        private static ViewArticle _instance = null;
        private static readonly object _padlock = new object();

        private daoArticle _daoArticle;
        private daoSalle _daoSalle;
        private daoArticleSalle _daoArticleSalle;
        private ObservableCollection<dtoArticle> _les_articles;
        private ObservableCollection<dtoSalle> _les_salles;
        private ObservableCollection<dtoSalle> _les_salles_select;

        private dtoArticle _articleSelect;
        private dtoSalle _laSalleSelect;
        private bool _chargementSalle;
        private bool _chargementArticle;
        private bool _enableArchive;
        private string _libelle;
        private decimal _prix;

        private Visibility _visibiliteAjout;
        private Visibility _visibiliteDelete;

        private ICommand _commandAjoutArticle;
        private ICommand _commandDeleteSalle;
        private ICommand _commandArchiverArticle;
        private ICommand _commandAjouterArticle;

        private ICollectionView collectionViewArticles;
        private ICollectionView collectionViewSalles;
        private ICollectionView collectionViewSallesSelect;
        #endregion

        ViewArticle(MainWindow main, daoArticle daoArticle, daoSalle daoSalle, List<dtoSalle> salle, daoArticleSalle daoArticleSalle)
            : base(main)
        {
            _daoArticle = daoArticle;
            _daoSalle = daoSalle;
            _daoArticleSalle = daoArticleSalle;
            _les_articles = new ObservableCollection<dtoArticle>();
            _les_salles = new ObservableCollection<dtoSalle>(salle);
            _les_salles_select = new ObservableCollection<dtoSalle>();

            _chargementSalle = false;
            _chargementArticle = false;

            _visibiliteAjout = Visibility.Hidden;
            _visibiliteDelete = Visibility.Hidden;

            collectionViewSalles = CollectionViewSource.GetDefaultView(_les_salles);
            if (collectionViewSalles == null) throw new NullReferenceException("collectionView");
            collectionViewSalles.CurrentChanged += new EventHandler(OnCollectionViewCurrentChanged);

            collectionViewSallesSelect = CollectionViewSource.GetDefaultView(_les_salles_select);
            if (collectionViewSallesSelect == null) throw new NullReferenceException("collectionView");
            collectionViewSallesSelect.CurrentChanged += new EventHandler(OnCollectionViewCurrentChanged);
        }

        //singleton
        public static ViewArticle Instance(MainWindow main, daoArticle daoArticle, daoSalle daoSalle, List<dtoSalle> salle, daoArticleSalle daoArticleSalle)
        {
            lock (_padlock)
            {
                if (_instance == null)
                {
                    _instance = new ViewArticle(main, daoArticle, daoSalle, salle, daoArticleSalle);
                }
                string inSql = "";
                foreach (dtoSalle s in _instance._les_salles)
                {
                    inSql += s.Numero + ",";
                }
                inSql = inSql.Substring(0, inSql.Length - 1);
                string joinWhere = "JOIN article_salle on article.id = article_salle.article_id"
                    + " WHERE article_salle.salle_id IN(" + inSql + ") AND archive = 0"
                    + " GROUP BY id;";
                _instance._les_articles = new ObservableCollection<dtoArticle>((List<dtoArticle>)_instance._daoArticle.select("*", joinWhere​));
                _instance.LesArticles = null;

                _instance.collectionViewArticles = CollectionViewSource.GetDefaultView(_instance._les_articles);
                if (_instance.collectionViewArticles == null) throw new NullReferenceException("collectionView");
                _instance.collectionViewArticles.CurrentChanged += new EventHandler(_instance.OnCollectionViewCurrentChanged);

                return _instance;
            }
        }

        private void OnCollectionViewCurrentChanged(object sender, EventArgs e)
        {
            if (((ICollectionView)sender).CurrentItem != null)
            {
                if (this.collectionViewArticles.CurrentItem != null && ((ICollectionView)sender).CurrentItem.GetType() == typeof(dtoArticle))
                {
                    ArticleSelect = this.collectionViewArticles.CurrentItem as dtoArticle;
                }
                if (this.collectionViewSalles.CurrentItem != null && ((ListCollectionView)sender).Count == _les_salles.Count)
                {
                    SelectSalle = this.collectionViewSalles.CurrentItem as dtoSalle;
                }
                if (this.collectionViewSallesSelect.CurrentItem != null && ((ListCollectionView)sender).Count == _les_salles_select.Count)
                {
                    if (_les_salles_select.Count > 0)
                    {
                        VisibiliteDelete = Visibility.Visible;
                        LaSalleSelect = this.collectionViewSallesSelect.CurrentItem as dtoSalle;
                    }
                }
            }
        }

        public ObservableCollection<dtoArticle> LesArticles
        {
            get
            {
                return _les_articles;
            }
            set
            {
                OnPropertyChanged("LesArticles");
            }
        }
        public ObservableCollection<dtoSalle> LesSalles
        {
            get
            {
                return _les_salles;
            }
            set
            {
                OnPropertyChanged("LesSalles");
            }
        }

        public ObservableCollection<dtoSalle> SallesSelect
        {
            get
            {
                return _les_salles_select;
            }
            set
            {
                OnPropertyChanged("SallesSelect");
            }
        }

        public dtoSalle SelectSalle
        {
            set
            {
                if (_chargementSalle && !_les_salles_select.Contains(value))
                {
                    _les_salles_select.Add(value);
                    SallesSelect = null;
                }
                else
                {
                    _chargementSalle = true;
                }
            }
        }
        public dtoArticle ArticleSelect
        {
            get
            {
                return _articleSelect;
            }
            set
            {
                if (_chargementArticle)
                {
                    _articleSelect = value;
                    Libelle = value.Libelle;
                    Prix = value.Montant;
                    string inSql = "";
                    foreach (dtoSalle s in _les_salles)
                    {
                        inSql += s.Numero + ",";
                    }
                    inSql = inSql.Substring(0, inSql.Length - 1);
                    string joinWhere = "JOIN article_salle ON salle.id = article_salle.salle_id"
                        + " WHERE article_salle.article_id = " + value.Id
                        + " AND salle.id IN (" + inSql + ") GROUP BY salle.id";
                    _les_salles_select.Clear();
                    List<dtoSalle> salles = (List<dtoSalle>)_daoSalle.select("*", joinWhere​);
                    foreach (dtoSalle s in salles)
                    {
                        foreach (dtoSalle sa in _les_salles)
                        {
                            if (sa.Numero == s.Numero)
                            {
                                _les_salles_select.Add(sa);
                            }
                        }
                    }

                    EnableArchive = true;
                    VisibiliteAjout = Visibility.Visible;
                }
                else
                {
                    _chargementArticle = true;
                }
            }
        }
        public dtoSalle LaSalleSelect
        {
            get
            {
                return _laSalleSelect;
            }
            set
            {
                _laSalleSelect = value;
            }
        }

        public string Libelle
        {
            get
            {
                return _libelle;
            }
            set
            {
                _libelle = value;
                OnPropertyChanged("Libelle");
            }
        }
        public decimal Prix
        {
            get
            {
                return _prix;
            }
            set
            {
                _prix = value;
                OnPropertyChanged("Prix");
            }
        }

        public bool EnableArchive
        {
            get
            {
                return _enableArchive;
            }
            set
            {
                _enableArchive = value;
                OnPropertyChanged("EnableArchive");
            }
        }

        public Visibility VisibiliteDelete
        {
            get
            {
                return _visibiliteDelete;
            }
            set
            {
                _visibiliteDelete = value;
                OnPropertyChanged("VisibiliteDelete");
            }
        }
        public Visibility VisibiliteAjout
        {
            get
            {
                return _visibiliteAjout;
            }
            set
            {
                _visibiliteAjout = value;
                OnPropertyChanged("VisibiliteAjout");
            }
        }

        public ICommand AjoutArticle
        {
            get
            {
                if (this._commandAjoutArticle == null)
                    this._commandAjoutArticle = new RelayCommand(() => this.ajoutArticle(), () => true);

                return this._commandAjoutArticle;
            }
        }
        public ICommand DeleteSalle
        {
            get
            {
                if (this._commandDeleteSalle == null)
                    this._commandDeleteSalle = new RelayCommand(() => this.deleteSalle(), () => true);

                return this._commandDeleteSalle;
            }
        }

        public ICommand ArchiverArticle
        {
            get
            {
                if (this._commandArchiverArticle == null)
                    this._commandArchiverArticle = new RelayCommand(() => this.archiverArticle(), () => true);

                return this._commandArchiverArticle;
            }
        }

        public ICommand AjouterArticle
        {
            get
            {
                if (this._commandAjouterArticle == null)
                    this._commandAjouterArticle = new RelayCommand(() => this.ajouterArticle(), () => true);

                return this._commandAjouterArticle;
            }
        }

        public void ajoutArticle()
        {
            Libelle = "";
            Prix = 0;
            _les_salles_select = new ObservableCollection<dtoSalle>();
            SelectSalle = null;
            EnableArchive = false;
            VisibiliteAjout = Visibility.Visible;
        }
        public void deleteSalle()
        {
            _les_salles_select.Remove(_laSalleSelect);
            if (_les_salles_select.Count == 0)
            {
                VisibiliteDelete = Visibility.Hidden;
            }
        }
        public void archiverArticle()
        {
            MessageBoxResult mr = MessageBox.Show("Êtes vous sûr de vouloir archiver l'article?", "Archivage", MessageBoxButton.YesNo);
            if (mr == MessageBoxResult.Yes)
            {
                _daoArticle.archive(ArticleSelect);
                _les_articles.Remove(ArticleSelect);
                LesArticles = null;
                VisibiliteAjout = Visibility.Hidden;
            }
        }
        public void ajouterArticle()
        {
            MessageBoxResult mr = MessageBox.Show("Êtes vous sûr de vouloir ajouter l'article?", "Archivage", MessageBoxButton.YesNo);
            if (mr == MessageBoxResult.Yes)
            {
                if (EnableArchive == true)
                {
                    dtoArticle article = new dtoArticle(ArticleSelect.Id, Libelle, Prix, false);
                    _daoArticle.update(article);
                    _daoArticleSalle.delete(article.Id);
                    for (int i = 0; i < _les_salles_select.Count; i++)
                    {
                        _daoArticleSalle.insert(article.Id, _les_salles_select[i].Id);
                    }
                    _les_articles.Remove(ArticleSelect);
                    _les_articles.Add(article);
                    VisibiliteAjout = Visibility.Hidden;
                    LesArticles = null;
                }
                else
                {
                    int id = 0;
                    List<dtoArticle> article_id = (List<dtoArticle>)_daoArticle.select("*", "ORDER BY id DESC LIMIT 1");
                    id = article_id[0].Id + 1;
                    dtoArticle article = new dtoArticle(id, Libelle, Prix, false);
                    for (int i = 1; i < _les_salles_select.Count; i++)
                    {
                        _daoArticleSalle.insert(id, _les_salles_select[i].Id);
                    }
                    _daoArticle.insert(article);
                    _les_articles.Add(article);
                    VisibiliteAjout = Visibility.Hidden;
                    LesArticles = null;
                }
            }
        }
    }
}
