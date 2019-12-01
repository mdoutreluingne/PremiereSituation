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
        private ObservableCollection<dtoArticle> _les_articles;
        private ObservableCollection<dtoArticle> _les_articles_archives;
        private ObservableCollection<dtoSalle> _les_salles;
        private ObservableCollection<dtoSalle> _les_salles_select;

        private dtoArticle _articleSelect;
        private dtoArticle _articleArchiveSelect;
        private dtoSalle _laSalleSelect;
        private bool _chargementSalle;
        private bool _chargementArticle;
        private bool _enableArchive;
        private string _libelle;
        private decimal _prix;

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

                return _instance;
            }
        }

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
        public ICommand Desarchive
        {
            get
            {
                if (this._commandDesarchive == null)
                    this._commandDesarchive = new RelayCommand(() => this.desarchive(), () => true);

                return this._commandDesarchive;
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
                _daoArticle.archive(ArticleSelect, 1);
                _les_articles_archives.Add(ArticleSelect);
                _les_articles.Remove(ArticleSelect);
                LesArticles = null;
                LesArticlesArchive = null;
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
    }
}
