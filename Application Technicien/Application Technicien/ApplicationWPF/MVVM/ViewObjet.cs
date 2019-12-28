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
    public class ViewObjet : ViewModele
    {
        #region Attributs
        private static ViewObjet _instance = null;
        private static readonly object _padlock = new object();

        private daoTransaction _daoTransaction;
        private daoReservation _daoReservation;
        private daoArticle _daoArticle;
        private daoObstacle _daoObstacle;
        private dtoSalle _salle;
        private dtoArticle _articleSelect;
        private dtoObstacle _obstacleSelect;
        private dtoReservation _reservation;

        private bool _enableArticle;
        private bool _enableObstacle;
        private bool _enablePayement;
        private bool _nouveau;
        private string _passeur;
        private string _commentaireObstacle;
        private string _nomObstacle;
        private int _positionObstacle;
        private int _quantiteObstacle;
        private decimal _prixObstacle;
        private decimal _prix;
        private decimal _solde;

        private ViewPlanning _viewPlanning;

        private ObservableCollection<dtoArticle> _les_articles;
        private ObservableCollection<dtoObstacle> _les_obstacles;

        public Visibility _visibilite;
        public Visibility _visibiliteAjout;
        public Visibility _visibiliteModif;
        public Visibility _visibiliteDelete;
        public Visibility _visibiliteSupPaye;

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
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeurs
        /// </summary>
        /// <param name="main"></param>
        /// <param name="daoTransaction"></param>
        /// <param name="daoReservation"></param>
        /// <param name="daoArticle"></param>
        /// <param name="daoObstacle"></param>
        /// <param name="viewPlanning"></param>
        ViewObjet(MainWindow main, daoTransaction daoTransaction, daoReservation daoReservation, daoArticle daoArticle, daoObstacle daoObstacle, ViewPlanning viewPlanning)
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

        /// <summary>
        /// Singleton
        /// </summary>
        /// <param name="main"></param>
        /// <param name="daoTransaction"></param>
        /// <param name="daoReservation"></param>
        /// <param name="daoArticle"></param>
        /// <param name="daoObstacle"></param>
        /// <param name="viewPlanning"></param>
        /// <param name="dtoReservation"></param>
        /// <returns></returns>
        public static ViewObjet Instance(MainWindow main, daoTransaction daoTransaction, daoReservation daoReservation, daoArticle daoArticle, daoObstacle daoObstacle, ViewPlanning viewPlanning, dtoReservation dtoReservation)
        {
            lock (_padlock)
            {
                //--- Si le constructeur n'éxiste pas ---//
                if (_instance == null)
                {
                    _instance = new ViewObjet(main, daoTransaction, daoReservation, daoArticle, daoObstacle, viewPlanning);
                }
                //--- ----- --//
                if (dtoReservation != null)
                {
                    _instance._nouveau = ViewReservation.Instance(null, null, null, null, null, null, null, Visibility.Hidden).Nouveau;
                    _instance.Reservation = dtoReservation;
                    _instance.Salle = dtoReservation.DtoSalle;
                    string joinWhere = " JOIN article_salle ON article.id = article_salle.article_id"
                        + " JOIN salle ON article_salle.salle_id = salle.id"
                        + " WHERE salle.id = " + _instance.Salle.DtoTheme.Id + " AND article.archive = 0";
                    _instance._les_articles = new ObservableCollection<dtoArticle>((List<dtoArticle>)_instance._daoArticle.select("*", joinWhere​));
                    _instance.LesArticles = null;
                    _instance.Solde = ViewReservation.Instance(null, null, null, null, null, null, null, Visibility.Hidden).Solde;
                    if (!_instance._nouveau)
                    {
                        _instance.VisibiliteSupPaye = Visibility.Visible;
                        if (((List<dtoTransaction>)_instance._daoTransaction.select("*", "WHERE reservation_id = " + _instance.Reservation.Id)).Count >= 1)
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

        #endregion

        #region Accesseurs
        /// <summary>
        /// La liste des articles
        /// </summary>
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

        /// <summary>
        /// La liste des obstacles
        /// </summary>
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

        /// <summary>
        /// L'article séléctionné dans la liste
        /// </summary>
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

        /// <summary>
        /// L'objet séléctionné dans la liste
        /// </summary>
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

        /// <summary>
        /// Visibilite de la page
        /// </summary>
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

        /// <summary>
        /// Visibilite du bouton d'ajout
        /// </summary>
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

        /// <summary>
        /// Visibilite du bouton de modification
        /// </summary>
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

        /// <summary>
        /// Visibilite du bouton de suppression
        /// </summary>
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

        /// <summary>
        /// Visibilite du bouton de payement
        /// </summary>
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

        /// <summary>
        /// Autorisation de l'utilisation de la liste d'article
        /// </summary>
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

        /// <summary>
        /// Autorisation de l'utilisation de la liste d'obstacle
        /// </summary>
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

        /// <summary>
        /// Autorisation de l'utilisation du bouton de payement
        /// </summary>
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

        /// <summary>
        /// La réservation actuelle
        /// </summary>
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

        /// <summary>
        /// La salle actuelle
        /// </summary>
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

        /// <summary>
        /// Nom de l'obstacle
        /// </summary>
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

        /// <summary>
        /// Poisition de l'obstacle
        /// </summary>
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

        /// <summary>
        /// Quantité de l'obstacle
        /// </summary>
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

        /// <summary>
        /// Prix de l'obstacle
        /// </summary>
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

        /// <summary>
        /// Prix total à payer
        /// </summary>
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

        /// <summary>
        /// Solde du client
        /// </summary>
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

        /// <summary>
        /// Commentaire de l'obstacle
        /// </summary>
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

        /// <summary>
        /// Commande pour annuler un obstacle
        /// </summary>
        public ICommand AnnulerObstacle
        {
            get
            {
                if (this._commandAnnulerObstacle == null)
                    this._commandAnnulerObstacle = new RelayCommand(() => this.annulerObstacle(), () => true);

                return this._commandAnnulerObstacle;
            }
        }

        /// <summary>
        /// Commande pour placer l'obstacle
        /// </summary>
        public ICommand PlacerObstacle
        {
            get
            {
                if (this._commandPlacerObstacle == null)
                    this._commandPlacerObstacle = new RelayCommand(() => this.placerObstacle(), () => true);

                return this._commandPlacerObstacle;
            }
        }

        /// <summary>
        /// Commande pour modifier un obstacle
        /// </summary>
        public ICommand ModifierObstacle
        {
            get
            {
                if (this._commandModifierObstacle == null)
                    this._commandModifierObstacle = new RelayCommand(() => this.modifierObstacle(), () => true);

                return this._commandModifierObstacle;
            }
        }

        /// <summary>
        /// Commande pour supprimer un obstacle
        /// </summary>
        public ICommand DeleteObstacle
        {
            get
            {
                if (this._commandDeleteObstacle == null)
                    this._commandDeleteObstacle = new RelayCommand(() => this.deleteObstacle(), () => true);

                return this._commandDeleteObstacle;
            }
        }

        /// <summary>
        /// Commande pour retourner à la réservation
        /// </summary>
        public ICommand Precedent
        {
            get
            {
                if (this._commandPrecedent == null)
                    this._commandPrecedent = new RelayCommand(() => this.precedent(), () => true);

                return this._commandPrecedent;
            }
        }

        /// <summary>
        /// Commande pour valider la réservation
        /// </summary>
        public ICommand FinReservation
        {
            get
            {
                if (this._commandFinResa == null)
                    this._commandFinResa = new RelayCommand(() => this.finResa(false), () => true);

                return this._commandFinResa;
            }
        }

        /// <summary>
        /// Commande pour supprimer la réservation
        /// </summary>
        public ICommand SupprimerReservation
        {
            get
            {
                if (this._commandSupprimerReservation == null)
                    this._commandSupprimerReservation = new RelayCommand(() => this.supprimerReservation(), () => true);

                return this._commandSupprimerReservation;
            }
        }

        /// <summary>
        /// Commande pour payer la réservation
        /// </summary>
        public ICommand PayerReservation
        {
            get
            {
                if (this._commandPayerReservation == null)
                    this._commandPayerReservation = new RelayCommand(() => this.payerReservation(), () => true);

                return this._commandPayerReservation;
            }
        }
        #endregion

        #region Méthodes
        /// <summary>
        /// Lors du changement de séléction d'une collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Placer l'obstacle
        /// </summary>
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

        /// <summary>
        /// Annuler l'obstacle
        /// </summary>
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

        /// <summary>
        /// Modifier l'obstacle
        /// </summary>
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

        /// <summary>
        /// Supprimer l'obstacle
        /// </summary>
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

        /// <summary>
        /// Retour vers la réservation
        /// </summary>
        public void precedent()
        {
            Visibilite = Visibility.Hidden;
            ViewReservation.Instance(null, null, null, null, null, null, null, System.Windows.Visibility.Visible);
        }

        /// <summary>
        /// Création de la réservation
        /// </summary>
        public void finResa(bool bypass)
        {
            MessageBoxResult mr = MessageBoxResult.Yes;
            if (!bypass)
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
                mr = MessageBox.Show(message, "Valider la réservation", MessageBoxButton.YesNo);
            }
            if (mr == MessageBoxResult.Yes || bypass)
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
                if (!bypass)
                {
                    ViewDate.Instance(null, null, null, null).DateSelect = DateTime.Today;
                    _les_obstacles.Clear();
                    Visibilite = Visibility.Hidden;
                    _viewPlanning.Visibilite = Visibility.Visible;
                    ViewDate.Instance(null, null, null, null).Visibilite = Visibility.Visible;
                    ViewReservation.Instance(null, null, null, null, null, null, null, Visibility.Hidden).SelectClient = null;
                }
            }
        }

        /// <summary>
        /// Suppression de la réservation
        /// </summary>
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

        /// <summary>
        /// Payement de la réservation
        /// </summary>
        public void payerReservation()
        {
            finResa(true);
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
        #endregion
    }
}
