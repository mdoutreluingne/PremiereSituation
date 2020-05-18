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
    public class ViewArticle : ViewModele
    {
        #region Attributs
        private static ViewArticle _instance = null;
        private static readonly object _padlock = new object();

        private daoArticle _daoArticle;
        private daoSalle _daoSalle;
        private daoArticleSalle _daoArticleSalle;

        private dtoArticle _articleSelect;
        private dtoArticle _articleArchiveSelect;
        private dtoSalle _laSalleSelect;
        private bool _chargementSalle;
        private bool _chargementArticle;
        private bool _enableArchive;
        private string _libelle;
        private decimal _prix;

        private ObservableCollection<dtoArticle> _les_articles;
        private ObservableCollection<dtoArticle> _les_articles_archives;
        private ObservableCollection<dtoSalle> _les_salles;
        private ObservableCollection<dtoSalle> _les_salles_select;

        private Visibility _visibilite;
        private Visibility _visibiliteAjout;
        private Visibility _visibiliteDelete;

        private ICommand _commandAjoutArticle;
        private ICommand _commandDeleteSalle;
        private ICommand _commandArchiverArticle;
        private ICommand _commandAjouterArticle;
        private ICommand _commandDesarchive;

        private ICollectionView collectionViewArticles;
        private ICollectionView collectionViewArticlesArchives;
        private ICollectionView collectionViewSalles;
        private ICollectionView collectionViewSallesSelect;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="main"></param>
        /// <param name="daoArticle"></param>
        /// <param name="daoSalle"></param>
        /// <param name="salle"></param>
        /// <param name="daoArticleSalle"></param>
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

            _visibilite = Visibility.Hidden;
            _visibiliteAjout = Visibility.Hidden;
            _visibiliteDelete = Visibility.Hidden;

            collectionViewSalles = CollectionViewSource.GetDefaultView(_les_salles);
            if (collectionViewSalles == null) throw new NullReferenceException("collectionView");
            collectionViewSalles.CurrentChanged += new EventHandler(OnCollectionViewCurrentChanged);

            collectionViewSallesSelect = CollectionViewSource.GetDefaultView(_les_salles_select);
            if (collectionViewSallesSelect == null) throw new NullReferenceException("collectionView");
            collectionViewSallesSelect.CurrentChanged += new EventHandler(OnCollectionViewCurrentChanged);
        }

        /// <summary>
        /// Singleton
        /// </summary>
        /// <param name="main"></param>
        /// <param name="daoArticle"></param>
        /// <param name="daoSalle"></param>
        /// <param name="salle"></param>
        /// <param name="daoArticleSalle"></param>
        /// <returns></returns>
        public static ViewArticle Instance(MainWindow main, daoArticle daoArticle, daoSalle daoSalle, List<dtoSalle> salle, daoArticleSalle daoArticleSalle)
        {
            lock (_padlock)
            {
                //--- Création du constructeur ---//
                if (_instance == null)
                {
                    _instance = new ViewArticle(main, daoArticle, daoSalle, salle, daoArticleSalle);
                }
                //--- ----- ---//

                //--- Séléction des articles ---//
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

                joinWhere = "JOIN article_salle on article.id = article_salle.article_id"
                    + " WHERE article_salle.salle_id IN(" + inSql + ") AND archive = 1"
                    + " GROUP BY id;";
                _instance._les_articles_archives = new ObservableCollection<dtoArticle>((List<dtoArticle>)_instance._daoArticle.select("*", joinWhere​));
                _instance.LesArticlesArchive = null;

                _instance.collectionViewArticles = CollectionViewSource.GetDefaultView(_instance._les_articles);
                if (_instance.collectionViewArticles == null) throw new NullReferenceException("collectionView");
                _instance.collectionViewArticles.CurrentChanged += new EventHandler(_instance.OnCollectionViewCurrentChanged);

                _instance.collectionViewArticlesArchives = CollectionViewSource.GetDefaultView(_instance._les_articles_archives);
                if (_instance.collectionViewArticlesArchives == null) throw new NullReferenceException("collectionView");
                _instance.collectionViewArticlesArchives.CurrentChanged += new EventHandler(_instance.OnCollectionViewCurrentChanged);
                //--- ----- ---//

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
        /// La liste des articles archivés
        /// </summary>
        public ObservableCollection<dtoArticle> LesArticlesArchive
        {
            get
            {
                return _les_articles_archives;
            }
            set
            {
                OnPropertyChanged("LesArticlesArchive");
            }
        }

        /// <summary>
        /// La liste des salles disponibles
        /// </summary>
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

        /// <summary>
        /// La liste des salles séléctionnés
        /// </summary>
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

        /// <summary>
        /// La salle séléctionné dans la combobox
        /// </summary>
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

        /// <summary>
        /// L'article séléctionné dans la liste
        /// </summary>
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

        /// <summary>
        /// L'article archivé séléctionné dans la liste
        /// </summary>
        public dtoArticle ArticleArchiveSelect
        {
            get
            {
                return _articleArchiveSelect;
            }
            set
            {
                _articleArchiveSelect = value;
            }
        }

        /// <summary>
        /// La salle séléctionné dans la liste
        /// </summary>
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

        /// <summary>
        /// Le libelle de l'article
        /// </summary>
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

        /// <summary>
        /// Le prix de l'article
        /// </summary>
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

        /// <summary>
        /// Si l'article peut être archivé
        /// </summary>
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

        /// <summary>
        /// Visibilité de la page
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
        /// Visibilité du bouton supprimer (poubelle)
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
                OnPropertyChanged("VisibiliteAjout");
            }
        }

        /// <summary>
        /// Commande du bouton d'ajout (+)
        /// </summary>
        public ICommand AjoutArticle
        {
            get
            {
                if (this._commandAjoutArticle == null)
                    this._commandAjoutArticle = new RelayCommand(() => this.ajoutArticle(), () => true);

                return this._commandAjoutArticle;
            }
        }

        /// <summary>
        /// Commande du bouton de suppression (poubelle)
        /// </summary>
        public ICommand DeleteSalle
        {
            get
            {
                if (this._commandDeleteSalle == null)
                    this._commandDeleteSalle = new RelayCommand(() => this.deleteSalle(), () => true);

                return this._commandDeleteSalle;
            }
        }

        /// <summary>
        /// Commande du bouton archiver
        /// </summary>
        public ICommand ArchiverArticle
        {
            get
            {
                if (this._commandArchiverArticle == null)
                    this._commandArchiverArticle = new RelayCommand(() => this.archiverArticle(), () => true);

                return this._commandArchiverArticle;
            }
        }

        /// <summary>
        /// Commande du bouton 
        /// </summary>
        public ICommand AjouterArticle
        {
            get
            {
                if (this._commandAjouterArticle == null)
                    this._commandAjouterArticle = new RelayCommand(() => this.ajouterArticle(), () => true);

                return this._commandAjouterArticle;
            }
        }

        /// <summary>
        /// Commande de désarchivage
        /// </summary>
        public ICommand Desarchive
        {
            get
            {
                if (this._commandDesarchive == null)
                    this._commandDesarchive = new RelayCommand(() => this.desarchive(), () => true);

                return this._commandDesarchive;
            }
        }
        #endregion

        #region Méthodes
        /// <summary>
        /// Selection d'un objet dans une des collections
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCollectionViewCurrentChanged(object sender, EventArgs e)
        {
            if (((ICollectionView)sender).CurrentItem != null)
            {
                if (this.collectionViewArticles.CurrentItem != null && ((ListCollectionView)sender).Count == _les_articles.Count)
                {
                    ArticleSelect = this.collectionViewArticles.CurrentItem as dtoArticle;
                }
                if (this.collectionViewArticlesArchives.CurrentItem != null && ((ListCollectionView)sender).Count == _les_articles_archives.Count)
                {
                    ArticleArchiveSelect = this.collectionViewArticlesArchives.CurrentItem as dtoArticle;
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

        /// <summary>
        /// Clique sur ajouter un article (+)
        /// </summary>
        public void ajoutArticle()
        {
            Libelle = "";
            Prix = 0;
            _les_salles_select = new ObservableCollection<dtoSalle>();
            SelectSalle = null;
            EnableArchive = false;
            VisibiliteAjout = Visibility.Visible;
        }

        /// <summary>
        /// Supression d'une salle d'un article (icon poubelle)
        /// </summary>
        public void deleteSalle()
        {
            _les_salles_select.Remove(_laSalleSelect);
            if (_les_salles_select.Count == 0)
            {
                VisibiliteDelete = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Archivage d'un article (bouton)
        /// </summary>
        public void archiverArticle()
        {
            MessageBoxResult mr = MessageBox.Show("Êtes vous sûr de vouloir archiver l'article?", "Archivage", MessageBoxButton.YesNo);
            if (mr == MessageBoxResult.Yes)
            {
                _daoArticle.archive(ArticleSelect, 1);
                _les_articles_archives.Add(ArticleSelect);
                _les_articles.Remove(ArticleSelect);
                LesArticles = null;
                LesArticlesArchive = null;
                VisibiliteAjout = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Ajout d'un article dans la BDD (bouton)
        /// </summary>
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
                    _daoArticle.insert(article);
                    for (int i = 1; i < _les_salles_select.Count; i++)
                    {
                        _daoArticleSalle.insert(id, _les_salles_select[i].Id);
                    }
                    _les_articles.Add(article);
                    VisibiliteAjout = Visibility.Hidden;
                    LesArticles = null;
                }
            }
        }

        /// <summary>
        /// Désarchivage d'un article (-)
        /// </summary>
        public void desarchive()
        {
            MessageBoxResult mr = MessageBox.Show("Êtes vous sûr de vouloir désarchiver l'article?", "Archivage", MessageBoxButton.YesNo);
            if (mr == MessageBoxResult.Yes)
            {
                _daoArticle.archive(ArticleArchiveSelect, 0);
                _les_articles.Add(ArticleArchiveSelect);
                _les_articles_archives.Remove(ArticleArchiveSelect);
                LesArticlesArchive = null;
                LesArticles = null;
            }
        } 
        #endregion
    }
}
