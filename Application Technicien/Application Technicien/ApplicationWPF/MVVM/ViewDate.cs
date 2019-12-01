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
            : base(main)
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
            set
            {
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
}
