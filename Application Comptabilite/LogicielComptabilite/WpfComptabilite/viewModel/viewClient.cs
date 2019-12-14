using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CoucheModele.modele;
using CoucheModele.metier;
using System.Windows.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

namespace WpfComptabilite.viewModel
{
    public class viewClient : viewModelBase
    {
        #region Propriétés
        private dbal bdd;
        private daoClient unDaoClient;
        private daoVille unDaoVille;
        private daoTransaction unDaoTransac;
        private daoReservation unDaoReserv;
        private daoSalle unDaoSalle;
        private daoTheme unDaoTheme;
        private Client _clientActif;
        private Transaction _transactionActive;
        private string _modeAddCreditActif;
        private string _numCheque;
        private decimal _montant;
        private string _commentaire;
        private string _clientSaisie;
        private bool _isEnableLesClients = true;
        private bool _isEnableNom = false;
        private bool _isEnablePrenom = false;
        private bool _isEnableVille = false;
        private bool _isEnableTel = false;
        private bool _isEnableMail = false;
        private bool _isEnabledNumCheque = false;
        private bool _boutonVisible = false;
        private bool _autreBoutonVisible = true;
        private bool _lesVillesVisible = false;
        private bool _lesClientsVisible = true;
        private bool _isEnableGridUser = true;
        private object _soldes = 0;
        private ICommand addCommandClient;
        private ICommand archiveCommand;
        private ICommand desarchiveCommand;
        private ICommand detailHistoriqueCommand;
        private ICommand addCreditCommand;
        private ICommand modifyCommandClient;
        private ICommand viderLesChamps;
        private ICommand viderNom;
        private ICommand viderPrenom;
        private ICommand viderVille;
        private ICommand viderTel;
        private ICommand viderMail;
        private ICommand filterVille;
        private ICommand filterClient;


        private ObservableCollection<Client> _lesclients;
        private readonly ICollectionView collectionViewClient;

        private ObservableCollection<Ville> _lesvilles;
        private readonly ICollectionView collectionViewVille;

        private ObservableCollection<Transaction> _lesHistoriques;
        private readonly ICollectionView collectionViewHistoriques;
        #endregion


        #region Constructeur
        public viewClient(daoTransaction dt, daoVille dv, daoClient dc, dbal bdd, daoTheme dtheme, daoSalle ds, daoReservation dr)
        {
            unDaoTransac = dt;
            unDaoClient = dc;
            unDaoVille = dv;
            this.bdd = bdd;
            unDaoTheme = dtheme;
            unDaoSalle = ds;
            unDaoReserv = dr;
            _lesclients = new ObservableCollection<Client>(unDaoClient.selectAllClient());
            //_lesvilles = new ObservableCollection<Ville>(unDaoVille.selectAllVille());
            _lesHistoriques = new ObservableCollection<Transaction>();

            this.collectionViewVille = CollectionViewSource.GetDefaultView(this._lesvilles);

            this.collectionViewHistoriques = CollectionViewSource.GetDefaultView(this._lesHistoriques);
            if (this.collectionViewHistoriques == null) throw new NullReferenceException("collectionViewHistoriques");
            this.collectionViewHistoriques.CurrentChanged += new EventHandler(this.OnCollectionViewCurrentChanged);

            this.collectionViewClient = CollectionViewSource.GetDefaultView(this._lesclients);
            if (this.collectionViewClient == null) throw new NullReferenceException("collectionViewClient");
            this.collectionViewClient.CurrentChanged += new EventHandler(this.OnCollectionViewCurrentChanged);

            NumCheque = "Numéro de cheque";
            Commentaire = "Commentaire";
        } 
        #endregion

        public ObservableCollection<Client> Lesclients
        {
            get { return this._lesclients; }
            set 
            {
                _lesclients = value;
                OnPropertyChanged("Lesclients");
            }
        }
        public ObservableCollection<Ville> Lesvilles
        {
            get { return _lesvilles; }
            set
            {
                _lesvilles = value;
                OnPropertyChanged("Lesvilles");
            }
        }
        public string SelectVille //Ville sélectionner
        {
            get { return null; }
            set
            {
                Ville_id = value;
                OnPropertyChanged("SelectVille");
            }
        }
        public string SelectClient //Client sélectionner
        {
            get { return _clientSaisie; }
            set
            {
                _clientSaisie = value;
                OnPropertyChanged("SelectClient");
            }
        }
        public Client ClientActif
        {
            get 
            {
                IsEnableGridUser = true; //Réactive la grid après avoir archiver puis sélectionner un client
                return _clientActif; 
            }
            set
            {
                if (value != null)
                {
                    _clientActif = value;
                    OnPropertyChanged("Nom");
                    OnPropertyChanged("Prenom");
                    OnPropertyChanged("Ville_id");
                    OnPropertyChanged("Tel");
                    OnPropertyChanged("Mail");
                    OnPropertyChanged("Soldes");
                    OnPropertyChanged("LesHistoriques");
                    OnPropertyChanged("BoutonVisible");
                    OnPropertyChanged("AutreBoutonVisible");
                    OnPropertyChanged("ArchiveCommand");
                    OnPropertyChanged("IsEnableNom");
                    OnPropertyChanged("IsEnablePrenom");
                    OnPropertyChanged("IsEnableVille");
                    OnPropertyChanged("IsEnableTel");
                    OnPropertyChanged("IsEnableMail");
                    OnPropertyChanged("IsEnableLesClients");
                    OnPropertyChanged("Lesclients");
                    OnPropertyChanged("SelectVille");
                    OnPropertyChanged("LesVillesVisible");

                }
            }
        }
        public Transaction TransactionActive //Ce fait par le SelectItem du xaml
        {
            get => _transactionActive;
            set => _transactionActive = value;
        }
        public string ModeAddCreditActif //Ce fait par le SelectItem du xaml
        { 
            get => _modeAddCreditActif;
            set 
            {
                //Active / désactive la textebox numero chèque
                if (value == "Chèque")
                {
                    IsEnabledNumCheque = true;
                    
                }
                else
                {
                    IsEnabledNumCheque = false;
                }
                _modeAddCreditActif = value;
                OnPropertyChanged("ModeAddCreditActif");

            } 
        }
        public string Nom
        {
            get
            {
               return _clientActif.Nom;
            }
            set
            {
                if (_clientActif.Nom != value)
                {
                    _clientActif.Nom = value;
                    OnPropertyChanged("Nom");
                }
            }
        }
        public string Prenom
        {
            get => _clientActif.Prenom;
            set
            {
                if (_clientActif.Prenom != value)
                {
                    _clientActif.Prenom = value;
                    OnPropertyChanged("Prenom");
                }
            }
        }
        public string Ville_id //Ville dans la textbox
        {
            get => _clientActif.Ville_id.Nom;
            set
            {
                if (_clientActif.Ville_id.Nom != value)
                {
                    Ville v = _clientActif.Ville_id;
                    _clientActif.Ville_id.Nom = value;
                    OnPropertyChanged("Ville_id");
                }
            }
        }
        public string Tel
        {
            get => _clientActif.Tel;
            set
            {
                if (_clientActif.Tel != value)
                {
                    _clientActif.Tel = value;
                    OnPropertyChanged("Tel");
                }
            }
        }
        public string Mail
        {
            get => _clientActif.Mail;
            set
            {
                if (_clientActif.Mail != value)
                {
                    _clientActif.Mail = value;
                    OnPropertyChanged("Mail");
                }
            }
        }
        public object Soldes 
        {
            get 
            {
                _soldes = unDaoTransac.soldes_client(_clientActif.Id);
                return _soldes; 
            }
            set
            {
                if (_soldes != value)
                {
                    _soldes = value;
                    OnPropertyChanged("Soldes");
                }
            }

        }
        public string NumCheque 
        { 
            get => _numCheque;
            set
            {
                _numCheque = value;
                OnPropertyChanged("NumCheque");
            } 
        }
        public decimal Montant 
        { 
            get => _montant;
            set
            {
                _montant = value;
                OnPropertyChanged("Montant");
            }
        }
        public string Commentaire 
        { 
            get => _commentaire;
            set
            {
                _commentaire = value;
                OnPropertyChanged("Commentaire");
            } 
        }
        #region IsEnable
        public bool IsEnableLesClients
        {
            get => _isEnableLesClients;
            set
            {
                _isEnableLesClients = value;
                OnPropertyChanged("IsEnableLesClients");
            }
        }
        public bool IsEnableNom
        {
            get => _isEnableNom;
            set
            {
                _isEnableNom = value;
                OnPropertyChanged("IsEnableNom");
            }
        }
        public bool IsEnablePrenom
        {
            get => _isEnablePrenom;
            set
            {
                _isEnablePrenom = value;
                OnPropertyChanged("IsEnablePrenom");
            }
        }
        public bool IsEnableVille
        {
            get => _isEnableVille;
            set
            {
                _isEnableVille = value;
                OnPropertyChanged("IsEnableVille");
            }
        }
        public bool IsEnableTel
        {
            get => _isEnableTel;
            set
            {
                _isEnableTel = value;
                OnPropertyChanged("IsEnableTel");
            }
        }
        public bool IsEnableMail
        {
            get => _isEnableMail;
            set
            {
                _isEnableMail = value;
                OnPropertyChanged("IsEnableMail");
            }

        }
        public bool IsEnabledNumCheque 
        { 
            get => _isEnabledNumCheque;
            set
            {
                _isEnabledNumCheque = value;
                OnPropertyChanged("IsEnabledNumCheque");
            }
        }

        public bool IsEnableGridUser 
        { 
            get => _isEnableGridUser;
            set
            {
                _isEnableGridUser = value;
                OnPropertyChanged("IsEnableGridUser");
            }
        }
        #endregion

        public ObservableCollection<Transaction> LesHistoriques 
        {
            get
            {
                //Sélectionne toutes les transactions du client sélectionner
                _lesHistoriques = new ObservableCollection<Transaction>(unDaoTransac.selectAllHistorique(_clientActif.Id)); 
                return this._lesHistoriques;
            }
            set
            {
                _lesHistoriques = value;
                OnPropertyChanged("LesHistoriques");
            }
             
        }
        public bool BoutonVisible
        {
            get => _boutonVisible;
            set 
            {
                _boutonVisible = value;
                OnPropertyChanged("BoutonVisible");
            }
        }
        public bool AutreBoutonVisible
        {
            get => _autreBoutonVisible;
            set {
                _autreBoutonVisible = value;
                OnPropertyChanged("AutreBoutonVisible");
            }
        }
        public bool LesVillesVisible
        {
            get => _lesVillesVisible;
            set
            {
                _lesVillesVisible = value;
                OnPropertyChanged("LesVillesVisible");
            }
        }
        public bool LesClientsVisible 
        { 
            get => _lesClientsVisible;
            set
            {
                _lesClientsVisible = value;
                OnPropertyChanged("LesClientsVisible");
            }
        }
        public ICommand AddCommandClient 
        {
            get
            {
                if (this.addCommandClient == null)
                    this.addCommandClient = new RelayCommand(() => this.ajouterClient(), () => true);

                return this.addCommandClient;
            }
        }

        public ICommand ArchiveCommand 
        {
            get
            {
                if (this.archiveCommand == null)
                    this.archiveCommand = new RelayCommand(() => this.archiverClient(), () => true);

                return this.archiveCommand;
            }
        }

        public ICommand DesarchiveCommand
        {
            get
            {
                if (this.desarchiveCommand == null)
                    this.desarchiveCommand = new RelayCommand(() => this.FenetreDesarchiver(), () => true);

                return this.desarchiveCommand;
            }
        }
        public ICommand DetailHistoriqueCommand 
        {
            get
            {
                if (this.detailHistoriqueCommand == null)
                    this.detailHistoriqueCommand = new RelayCommand(() => this.detailHistorique(), () => true);

                return this.detailHistoriqueCommand;
            }
        }
        public ICommand AddCreditCommand 
        {
            get
            {
                if (this.addCreditCommand == null)
                    this.addCreditCommand = new RelayCommand(() => this.addCredit(), () => true);

                return this.addCreditCommand;
            }
        }
        public ICommand ModifyCommandClient
        {
            get
            {
                if (this.modifyCommandClient == null)
                    this.modifyCommandClient = new RelayCommand(() => this.updateClient(), () => true);

                return this.modifyCommandClient;
            }
        }
        public ICommand ViderLesChamps 
        {
            get
            {
                if (this.viderLesChamps == null)
                    this.viderLesChamps = new RelayCommand(() => this.viderDesChamps(), () => true);

                return this.viderLesChamps;
            }
        }

        public ICommand ViderNom
        {
            get
            {
                if (this.viderNom == null)
                    this.viderNom = new RelayCommand(() => this.viderChampsNom(), () => true);

                return this.viderNom;
            }
        }
        public ICommand ViderPrenom
        {
            get
            {
                if (this.viderPrenom == null)
                    this.viderPrenom = new RelayCommand(() => this.viderChampsPrenom(), () => true);

                return this.viderPrenom;
            }
        }
        public ICommand ViderVille
        {
            get
            {
                if (this.viderVille == null)
                    this.viderVille = new RelayCommand(() => this.viderChampsVille(), () => true);

                return this.viderVille;
            }
        }
        public ICommand ViderTel
        {
            get
            {
                if (this.viderTel == null)
                    this.viderTel = new RelayCommand(() => this.viderChampsTel(), () => true);

                return this.viderTel;
            }
        }
        public ICommand ViderMail
        {
            get
            {
                if (this.viderMail == null)
                    this.viderMail = new RelayCommand(() => this.viderChampsMail(), () => true);

                return this.viderMail;
            }
        }

        public ICommand FilterVille
        {
            get
            {
                if (this.filterVille == null)
                    this.filterVille = new RelayCommand(() => this.autocomplete_ville(), () => true);

                return this.filterVille;
            }
        }

        public ICommand FilterClient
        {
            get
            {
                if (this.filterClient == null)
                    this.filterClient = new RelayCommand(() => this.autocomplete_client(), () => true);

                return this.filterClient;
            }
        }      

        #region Méthodes
        public void ajouterClient()
        {
            List<string> lesMails = new List<string>(unDaoClient.selectMail());
            bool valide = false;

            //Cherche si un mail est déjà éxistant
            foreach (string s in lesMails)
            {
                if (Mail.Contains(s) == true)
                {
                    valide = true;
                    break;
                }
            }

            if (valide == true) //Erreur si mail existe déjà
            {
                MessageBox.Show("Le client existe déjà", "Erreur lors d'ajout d'un client", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else 
            {
                if (String.IsNullOrEmpty(Nom) == true || String.IsNullOrEmpty(Prenom) == true || String.IsNullOrEmpty(Ville_id) == true || String.IsNullOrEmpty(Tel) == true || String.IsNullOrEmpty(Mail) == true)
                {
                    MessageBox.Show("Certains champs sont vides", "Erreur lors d'ajout d'un client", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    ClientActif.Nom = Nom[0].ToString().ToUpper() + Nom.Substring(1).ToLower(); // Met la première lettre en majuscule
                    ClientActif.Prenom = Prenom[0].ToString().ToUpper() + Prenom.Substring(1).ToLower(); // Met la première lettre en majuscule

                    if (ClientActif.Nom == "Nom" || ClientActif.Prenom == "Prenom" || ClientActif.Tel == "Téléphone" || ClientActif.Ville_id.Nom == "Ville" || ClientActif.Mail == "Email")
                    {
                        MessageBox.Show("Saisir les informations du client à ajouter", "Erreur lors d'ajout d'un client", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        //AJout du client
                        _clientActif.Ville_id.Id = unDaoVille.selectByNom(_clientActif.Ville_id.Nom); //Récupère l'id du pays saisie
                        unDaoClient.insert(ClientActif);
                        ClientActif = unDaoClient.selectByNom(ClientActif.Nom); //Recupère l'id du client après ajout dans la bdd
                        Lesclients.Add(ClientActif);
                        this.collectionViewClient.Refresh();
                        this.collectionViewClient.MoveCurrentTo(ClientActif);

                        //Ajout de la transaction par default
                        DateTime date = DateTime.Now;
                        Client c = unDaoClient.selectById(_clientActif.Id);
                        _transactionActive = new Transaction(null, date, 0, "Carte bancaire", null, "Creation du compte", null, c);
                        _lesHistoriques.Add(_transactionActive);
                        unDaoTransac.insert(_transactionActive);
                        this.collectionViewHistoriques.Refresh();

                        //Désactive les champs
                        IsEnableNom = false;
                        IsEnablePrenom = false;
                        IsEnableVille = false;
                        IsEnableTel = false;
                        IsEnableMail = false;
                        IsEnableLesClients = true;
                        LesVillesVisible = false;
                        LesClientsVisible = true;
                        IsEnableGridUser = true;
                    }
                    
                }
            }
            
        }
        public void updateClient()
        {
            _clientActif.Ville_id.Id = unDaoVille.selectByNom(ClientActif.Ville_id.Nom); //Récupère l'id du pays saisie
            unDaoClient.update(ClientActif);
            this.collectionViewClient.MoveCurrentTo(ClientActif);
            
            _lesclients = new ObservableCollection<Client>(unDaoClient.selectAllClient());
            this.collectionViewClient.Refresh();

            IsEnableNom = false;
            IsEnablePrenom = false;
            IsEnableVille = false;
            IsEnableTel = false;
            IsEnableMail = false;
            IsEnableLesClients = true;
            BoutonVisible = false;
            AutreBoutonVisible = true;
            LesVillesVisible = false;
            LesClientsVisible = true;
            SelectClient = string.Empty;

        }
        public void archiverClient()
        {
            unDaoClient.archiver(ClientActif);
            this.collectionViewClient.Refresh();
            this.collectionViewClient.MoveCurrentTo(null);
            SelectClient = string.Empty;
            IsEnableGridUser = false;

            //Met les champs vide
            ClientActif = new Client();
            TransactionActive = new Transaction();
            this.collectionViewClient.MoveCurrentTo(null);
            this.collectionViewClient.Refresh();
            this.collectionViewHistoriques.MoveCurrentTo(null);
            this.collectionViewHistoriques.Refresh();

        }
        public void FenetreDesarchiver()
        {
            DesarchiveWindow secondwindows = new DesarchiveWindow();
            secondwindows.Show();            
        }
        public void detailHistorique()
        {
            if (_transactionActive != null)
            {
                if (_transactionActive.Type == "Chèque") 
                {   
                    MessageBox.Show("Type : " + _transactionActive.Type + "\n\nDate : " + _transactionActive.Date.ToString() + "\n\nNuméro de chèque : " + _transactionActive.Numero + "\n\nMontant : " + _transactionActive.Montant + "\n\nCommentaire : " + _transactionActive.Commentaire, "Détail de la transaction", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show("Type : " + _transactionActive.Type + "\n\nDate : " + _transactionActive.Date.ToString() + "\n\nMontant : " + _transactionActive.Montant + "\n\nCommentaire : " + _transactionActive.Commentaire, "Détail de la transaction", MessageBoxButton.OK);
                }
                
            }
            else
            {
                MessageBox.Show("Sélectionner une transation", "Détail de la transaction", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        public void addCredit()
        {
            
            if (ModeAddCreditActif != null)
            {
                DateTime date = DateTime.Now;
                Client c = unDaoClient.selectById(_clientActif.Id);
                _transactionActive = new Transaction(null, date, Montant, _modeAddCreditActif, NumCheque, Commentaire, null, c);
                _lesHistoriques.Add(_transactionActive);
                unDaoTransac.insert(_transactionActive);
                OnPropertyChanged("Soldes");
            }
            else
            {
                MessageBox.Show("Sélectionner un mode de paiement", "Erreur lors d'ajout de crédit", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
        }
        public void autocomplete_ville()
        {
            if (Ville_id.Length >= 3)
            {
                Lesvilles = new ObservableCollection<Ville>(unDaoVille.selectFilter("*", "WHERE nom LIKE '" + Ville_id + "%'"));
            }
        }
        public void autocomplete_client()
        {
            Lesclients = new ObservableCollection<Client>(unDaoClient.selectFilter("*", "WHERE nom LIKE '" + _clientSaisie + "%' AND archive = 0 ORDER BY nom ASC;"));
        }
        private void OnCollectionViewCurrentChanged(object sender, EventArgs e)
        {
            if (this.collectionViewClient.CurrentItem != null)
            {
                ClientActif = this.collectionViewClient.CurrentItem as Client;
            }
            if (this.collectionViewHistoriques.CurrentItem != null)
            {
                TransactionActive = this.collectionViewHistoriques.CurrentItem as Transaction;
            }

        }
        #endregion

        #region Vidage des champs
        public void viderDesChamps()
        {
            // Vide tous les champs
            IsEnableNom = true;
            IsEnablePrenom = true;
            IsEnableVille = true;
            IsEnableTel = true;
            IsEnableMail = true;
            IsEnableLesClients = false;
            LesVillesVisible = true;
            LesClientsVisible = false;
            IsEnableGridUser = false;

            ClientActif = new Client();
            TransactionActive = new Transaction();
            this.collectionViewClient.MoveCurrentTo(null);
            this.collectionViewClient.Refresh();
            this.collectionViewHistoriques.MoveCurrentTo(null);
            this.collectionViewHistoriques.Refresh();

            Nom = "Nom";
            Prenom = "Prenom";
            Ville_id = "Ville";
            Tel = "Téléphone";
            Mail = "Email";
        }
        public void viderChampsNom()
        {
            IsEnableNom = true;
            BoutonVisible = true;
            AutreBoutonVisible = false;
            IsEnableLesClients = false;
            LesClientsVisible = false;
        }
        public void viderChampsPrenom()
        {
            IsEnablePrenom = true;
            BoutonVisible = true;
            AutreBoutonVisible = false;
            IsEnableLesClients = false;
            LesClientsVisible = false;
        }
        public void viderChampsVille()
        {
            IsEnableVille = true;
            BoutonVisible = true;
            AutreBoutonVisible = false;
            IsEnableLesClients = false;
            LesVillesVisible = true;
            LesClientsVisible = false;
        }
        public void viderChampsTel()
        {
            IsEnableTel = true;
            BoutonVisible = true;
            AutreBoutonVisible = false;
            IsEnableLesClients = false;
            LesClientsVisible = false;
        }
        public void viderChampsMail()
        {
            IsEnableMail = true;
            BoutonVisible = true;
            AutreBoutonVisible = false;
            IsEnableLesClients = false;
            LesClientsVisible = false;
        }
        #endregion
        
        
    }
}
