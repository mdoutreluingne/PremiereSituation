using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ModeleMetier.metier;
using ModeleMetier.modele;

namespace ApplicationWPF.MVVM
{
    public class ViewDate : ViewModele
    {
        #region Attributs
        private static ViewDate _instance = null;
        private static readonly object _padlock = new object();

        private daoReservation _daoReservation;
        private daoTransaction _daoTransaction;

        private DateTime _date;
        private List<dtoSalle> _salle;

        private ICommand _icommandPreview;
        private ICommand _icommandNext;
        private System.Windows.Visibility _visibilite;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="salle"></param>
        /// <param name="daoReservation"></param>
        /// <param name="daoTransaction"></param>
        /// <param name="main"></param>
        ViewDate(List<dtoSalle> salle, daoReservation daoReservation, daoTransaction daoTransaction, MainWindow main)
           : base(main)
        {
            _date = DateTime.Today;
            _salle = salle;
            _daoReservation = daoReservation;
            _daoTransaction = daoTransaction;

        }

        /// <summary>
        /// Singleton
        /// </summary>
        /// <param name="salle"></param>
        /// <param name="daoReservation"></param>
        /// <param name="daoTransaction"></param>
        /// <param name="main"></param>
        /// <returns></returns>
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
        #endregion

        #region Accesseurs
        /// <summary>
        /// La valeur de la date du datepicker
        /// </summary>
        public DateTime DateSelect
        {
            get { return _date; }
            set
            {
                _date = value;
                _mainWindow.grd_planning.Children.Clear();
                _mainWindow.loadPlanning(_salle, _daoReservation, _daoTransaction);
                OnPropertyChanged("DateSelect");
            }
        }

        /// <summary>
        /// Visibilite de la grid
        /// </summary>
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

        /// <summary>
        /// Commande pour séléctionner la date précédente
        /// </summary>
        public ICommand SelectPreviewDate
        {
            get
            {
                if (this._icommandPreview == null)
                    this._icommandPreview = new RelayCommand(() => this.goPreviewDate(), () => true);

                return this._icommandPreview;
            }
        }

        /// <summary>
        /// Commande pour séléctionner la date suivante
        /// </summary>
        public ICommand SelectNextDate
        {
            get
            {
                if (this._icommandNext == null)
                    this._icommandNext = new RelayCommand(() => this.goNextDate(), () => true);

                return this._icommandNext;
            }
        }
        #endregion

        #region Méthodes
        /// <summary>
        /// Chargement du jour précédent
        /// </summary>
        protected void goPreviewDate()
        {
            DateSelect = Convert.ToDateTime(DateSelect).AddDays(-1);
        }

        /// <summary>
        /// Chargement du jour suivant
        /// </summary>
        protected void goNextDate()
        {
            DateSelect = Convert.ToDateTime(DateSelect).AddDays(1);
        } 
        #endregion
    }
}
