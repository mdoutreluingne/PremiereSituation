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
                    ViewReservation.Instance(null, null, null, null, null, null, null, System.Windows.Visibility.Hidden);
                    ViewObjet.Instance(null, null, null, null, null).Visibilite = Visibility.Hidden;
                    ViewDate.Instance(null, null, null).Visibilite = System.Windows.Visibility.Visible;
                    _viewPlanning.Visibilite = System.Windows.Visibility.Visible;
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
        private ICommand _icommandPreview;
        private ICommand _icommandNext;
        private System.Windows.Visibility _visibilite;

        ViewDate(List<dtoSalle> salle, daoReservation daoReservation, MainWindow main)
            :base(main)
        {
            _date = DateTime.Today;
            _salle = salle;
            _daoReservation = daoReservation;

        }

        //singleton
        public static ViewDate Instance(List<dtoSalle> salle, daoReservation daoReservation, MainWindow main)
        {
            lock (_padlock)
            {
                if (_instance == null)
                {
                    _instance = new ViewDate(salle, daoReservation, main);
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
            ViewDate.Instance(null, null, null).Visibilite = System.Windows.Visibility.Hidden;
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
        public static ViewReservation Instance(MainWindow main, daoClient daoClient, daoVille daoVille, daoTransaction daoTransaction, List<dtoSalle> les_salles, List<string> les_heures, ViewPlanning viewPlanning, System.Windows.Visibility visibility)
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
            else if (Nom.Length >= 3)
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
                ViewReservation.Instance(null, null, null, null, null, null, null, System.Windows.Visibility.Hidden);
                ViewDate.Instance(null, null, null).Visibilite = System.Windows.Visibility.Visible;
                _viewPlanning.Visibilite = System.Windows.Visibility.Visible;
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
                dtoReservation reservation = new dtoReservation(-1, _date, _commentaire, _nombreJoueur, _client, _salle);
                Visibilite = Visibility.Hidden;
                ViewObjet.Instance(null, null, null, null, reservation).Visibilite = Visibility.Visible;
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
        public daoReservation _daoReservation;
        public daoArticle _daoArticle;
        public Visibility _visibilite;
        public Visibility _visibiliteAjout;
        public Visibility _visibiliteModif;
        public Visibility _visibiliteDelete;

        private ViewPlanning _viewPlanning;
        private dtoSalle _salle;
        private dtoArticle _articleSelect;
        private dtoObstacle _obstacleSelect;
        private dtoReservation _reservation;
        private bool _enableArticle;
        private bool _enableObstacle;
        private string _passeur;

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

        ViewObjet(MainWindow main, daoReservation daoReservation, daoArticle daoArticle, ViewPlanning viewPlanning)
            : base(main)
        {
            _daoReservation = daoReservation;
            _daoArticle = daoArticle;
            _viewPlanning = viewPlanning;
            Visibilite = Visibility.Hidden;
            VisibiliteAjout = Visibility.Hidden;
            VisibiliteModif = Visibility.Hidden;
            VisibiliteDelete = Visibility.Hidden;
            EnableArticle = true;
            EnableObstacle = true;
            _les_obstacles = new ObservableCollection<dtoObstacle>();
            _les_articles = new ObservableCollection<dtoArticle>();
        }

        //singleton
        public static ViewObjet Instance(MainWindow main, daoReservation daoReservation, daoArticle daoArticle, ViewPlanning viewPlanning,dtoReservation dtoReservation)
        {
            lock (_padlock)
            {
                if (_instance == null)
                {
                    _instance = new ViewObjet(main, daoReservation,daoArticle, viewPlanning);
                }
                if (dtoReservation != null)
                {
                    _instance.Reservation = dtoReservation;
                    _instance.Salle = dtoReservation.DtoSalle;
                    string joinWhere = " JOIN article_salle ON article.id = article_salle.article_id"
                        + " JOIN salle ON article_salle.salle_id = salle.id"
                        + " WHERE salle.id = "+ _instance.Salle.DtoTheme.Id;
                    _instance._les_articles = new ObservableCollection<dtoArticle>((List<dtoArticle>)_instance._daoArticle.select("*", joinWhere​));
                    _instance.LesArticles = null;
                    _instance.Prix = 0;
                    _instance.Solde = ViewReservation.Instance(null, null, null, null, null, null, null, Visibility.Hidden).Solde;
                    _instance.collectionViewArticles = CollectionViewSource.GetDefaultView(_instance._les_articles);
                    if (_instance.collectionViewArticles == null) throw new NullReferenceException("collectionView");
                    _instance.collectionViewArticles.CurrentChanged += new EventHandler(_instance.OnCollectionViewCurrentChanged);

                    _instance.collectionViewObstacles = CollectionViewSource.GetDefaultView(_instance._les_obstacles);
                    if (_instance.collectionViewObstacles == null) throw new NullReferenceException("collectionView");
                    _instance.collectionViewObstacles.CurrentChanged += new EventHandler(_instance.OnCollectionViewCurrentChanged);
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
                + "\nSolde Restant : " + (_solde - _prix)
                + "\n\nVoulez vous valider la réservation ?";
            MessageBoxResult mr =  MessageBox.Show(message, "Valider la réservation", MessageBoxButton.YesNo);
            if (mr == MessageBoxResult.Yes)
            {
                _daoReservation.insert(_reservation);
            }
        }
    }
}
