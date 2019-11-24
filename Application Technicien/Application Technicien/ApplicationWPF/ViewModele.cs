using ModeleMetier.metier;
using ModeleMetier.modele;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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

    public class viewDate : ViewModele
    {
        private static viewDate _instance = null;
        private static readonly object _padlock = new object();

        private DateTime _date;
        private List<dtoSalle> _salle;
        private daoReservation _daoReservation;
        private ICommand _icommandPreview;
        private ICommand _icommandNext;
        private System.Windows.Visibility _visibilite;

        viewDate(List<dtoSalle> salle, daoReservation daoReservation, MainWindow main)
            :base(main)
        {
            _date = DateTime.Today;
            _salle = salle;
            _daoReservation = daoReservation;

        }

        //singleton
        public static viewDate Instance(List<dtoSalle> salle, daoReservation daoReservation, MainWindow main)
        {
            lock (_padlock)
            {
                if (_instance == null)
                {
                    _instance = new viewDate(salle, daoReservation, main);
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

    public class viewPlanning : ViewModele
    {
        private viewPlanning _origin;
        private dtoReservation _reservation;
        private System.Windows.Visibility _visibilite;
        private ICommand _icommand;
        public viewPlanning(viewPlanning origin, dtoReservation reservation, MainWindow main)
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
            viewDate.Instance(null, null, null).Visibilite = System.Windows.Visibility.Hidden;
            viewReservation.Instance(_mainWindow, null,  System.Windows.Visibility.Visible).LoadReservation = _reservation;

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

    public sealed class viewReservation : ViewModele
    {
        private static viewReservation _instance = null;
        private static readonly object _padlock = new object();

        private System.Windows.Visibility _visibilite;
        private List<dtoSalle> _les_salles;

        private dtoSalle _salle;
        private string _nom;
        private string _prenom;
        private string _ville;
        private string _mail;
        private string _telephone;

        viewReservation(MainWindow main, List<dtoSalle> les_salles)
            :base(main)
        {
            _les_salles = les_salles;
        }

        //singleton
        public static viewReservation Instance(MainWindow main, List<dtoSalle> les_salles, System.Windows.Visibility visibility)
        {
            lock (_padlock)
            {
                if (_instance == null)
                {
                    _instance = new viewReservation(main, les_salles);
                }
                _instance.Visibilite = visibility;
                return _instance;
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

        public List<dtoSalle> LesSalles
        {
            get
            {
                return _les_salles;
            }
        }

        public dtoReservation LoadReservation
        {
            set
            {
                if (value != null)
                {
                    Salle = value.DtoSalle;
                    Nom = value.Client.Nom;
                    Prenom = value.Client.Prenom;
                    Ville = value.Client.DtoVille.Nom;
                    Mail = value.Client.Mail;
                    Telephone = value.Client.Tel;
                }
                else
                {
                    Nom = "NOM";
                    Prenom = "PRENOM";
                    Ville = "VILLE";
                    Mail = "MAIL";
                    Telephone = "TELEPHONE";
                }
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
    }
}
