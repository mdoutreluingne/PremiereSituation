using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using CoucheModele.metier;
using CoucheModele.modele;

namespace WpfComptabilite.viewModel
{
    class viewDesarchive : viewModelBase
    {
        private dbal bdd;
        private daoClient LeDaoClient;
        private daoVille LeDaoVille;
        private bool _labelVisible = false;
        private ICommand dearchiveCommandClient;
        private Client _clientActifDesarchiver;

        private ObservableCollection<Client> _lesclientsDesarchiver;
        private readonly ICollectionView collectionViewDesarchive;

        public viewDesarchive(daoVille daov, daoClient daoc, dbal bdd)
        {
            this.bdd = bdd;
            LeDaoVille = daov;
            LeDaoClient = daoc;

            _lesclientsDesarchiver = new ObservableCollection<Client>(LeDaoClient.selectAllClientDesarchiver());
            this.collectionViewDesarchive = CollectionViewSource.GetDefaultView(this._lesclientsDesarchiver);
            if (this.collectionViewDesarchive == null) throw new NullReferenceException("collectionViewDesarchive");
            this.collectionViewDesarchive.CurrentChanged += new EventHandler(this.OnCollectionViewCurrentChanged);
        }
        public ObservableCollection<Client> LesclientsDesarchiver 
        {
            get { return _lesclientsDesarchiver; } 
        }

        public Client ClientActifDesarchiver 
        { 
            get => _clientActifDesarchiver; 
            set => _clientActifDesarchiver = value; 
        }
        public bool LabelVisible
        {
            get => _labelVisible;
            set
            {
                _labelVisible = value;
                OnPropertyChanged("LabelVisible");
            }
        }
        public ICommand DearchiveCommandClient 
        {
            get
            {
                if (this.dearchiveCommandClient == null)
                    this.dearchiveCommandClient = new RelayCommand(() => this.desarchiverLeClient(), () => true);

                return this.dearchiveCommandClient;
            }
        }
        public void desarchiverLeClient()
        {
            LeDaoClient.desarchiver(ClientActifDesarchiver);
            this.collectionViewDesarchive.Refresh();
            LabelVisible = true;
        }

        private void OnCollectionViewCurrentChanged(object sender, EventArgs e)
        {
            if (this.collectionViewDesarchive.CurrentItem != null)
            {
                ClientActifDesarchiver = this.collectionViewDesarchive.CurrentItem as Client;
            }

        }
    }
}
