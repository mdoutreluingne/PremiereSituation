using ModeleMetier.metier;
using ModeleMetier.modele;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    class viewModele : INotifyPropertyChanged
    {
        #region base PropertyChanged
        private static viewModele _instance = null;
        private static readonly object _padlock = new object();

        private daoAvis _daoavis;
        private dtoSalle _salle;
        private daoSalle _daoSalle;
        public event PropertyChangedEventHandler PropertyChanged;

        private ICommand _commandModifier;
        private bool _enableModification;

        private Visibility _isVisible;

        private ICommand _commandRetour;
        private bool _disableProperty;

        private ICommand _commandValider;
        private bool _validerModif; 
        private viewModele(daoAvis daoAvis)
        {
            _daoavis = daoAvis;

            IsVisible = Visibility.Hidden;
        }

        public static viewModele Instance(daoAvis daoAvis)
        {
            lock (_padlock)
            {
                if (_instance == null)
                {
                    _instance = new viewModele(daoAvis);
                }
                return _instance;
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
            DisableModification = true;
            IsVisible = Visibility.Hidden;
            EnableModification = false; 

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
               List<dtoSalle> lesalles = ((List<dtoSalle>)_daoSalle.)
            }
            else
            {
               
            }
            DisableModification = true;
            IsVisible = Visibility.Hidden;
            EnableModification = false;

        }

        #endregion


    }
}
