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

        public List<viewModele> _lesViews; 
        private daoAvis _daoavis;
        private dtoSalle _salle;
        private daoSalle _daoSalle;
        private List<dtoSalle> _lesSalles; 
        public event PropertyChangedEventHandler PropertyChanged;

        private ICommand _commandModifier;
        private bool _enableModification;

        private Visibility _isVisible;

        private ICommand _commandRetour;
        private bool _disableProperty;

        private ICommand _commandValider;
        private bool _validerModif;

        private string _NomVille;
        private string _NumSalle;
        private string _themeSalle;
        private string _prixSalle;
        private string _horaireSalle;


        public viewModele(daoAvis daoAvis,daoSalle daoSalle, dtoSalle laSalle)
        {
            _daoavis = daoAvis;
            _daoSalle = daoSalle;
            _salle = laSalle;
            if(laSalle != null)
            {
                _NomVille = laSalle.DtoVille.Nom;
                _NumSalle = laSalle.Numero.ToString();
                _themeSalle = laSalle.DtoTheme.Nom.ToString();
                _prixSalle = laSalle.Prix.ToString();
                _horaireSalle =laSalle.Heure_ouverture.TimeOfDay.ToString()+ "\n\nHoraire de fermeture :\n" + laSalle.Heure_fermeture.TimeOfDay.ToString(); 
            }
        IsVisible = Visibility.Hidden;
        }

     
        public string NomVille
        {
            get 
            {
                return _NomVille; 
            }
            set
            {
                _NomVille = value;
                OnPropertyChanged("NomVille");
            }
        }

        public string NumSalle
        {
            get
            {
                return "Salle n°" + _NumSalle;
            }
            set
            {
                _NumSalle = value;
                OnPropertyChanged("NumSalle");
            }
        }

        public string themeSalle
        {
            get
            {
                return "Thème :\n" + _themeSalle;
            }
            set
            {
                _themeSalle = value;
                OnPropertyChanged("themeSalle");
            }
        }

        public string prixSalle
        {
            get
            {
                return "Prix :\n "+_prixSalle+"€";
            }
            set
            {
                _prixSalle = value;
                OnPropertyChanged("prixSalle");
            }
        }

        public string horaireSalle
        {
            get
            {
                return "Horaire d'ouverture :\n" + _horaireSalle;
            }
            set
            {
                _horaireSalle = value;
                OnPropertyChanged("horaireSalle");
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
                _enableModification = true; 
                foreach (viewModele v in _lesViews)
                {
                    v._lesViews = new List<viewModele>();
                    v.EnableModification = true; 
                }              
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
                
                foreach (dtoSalle s in _lesSalles)
                {
                    //pour chaque salle -> update
                    _daoSalle.update(s);

                }

            }
            else
            {
               
            }
            DisableModification = true;
            IsVisible = Visibility.Hidden;
            EnableModification = false;
        }

        public void compteLesView(List<viewModele> uneListe)
        {
            _lesViews = uneListe; 
        }
        #endregion

    }
}
