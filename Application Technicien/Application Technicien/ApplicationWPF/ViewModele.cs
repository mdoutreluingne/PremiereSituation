using ModeleMetier.metier;
using ModeleMetier.modele;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationWPF
{
    class ViewModele : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string caller = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(caller));
            }
        }
    }

    class viewDate : ViewModele
    {
        private MainWindow _mainWindow;
        private DateTime _date;
        private List<dtoSalle> _salle;
        private dtoVille _ville;
        private daoReservation _daoReservation;

        public viewDate(MainWindow mainWindow, List<dtoSalle> salle, daoReservation daoReservation)
        {
            _date = DateTime.Today;
            _mainWindow = mainWindow;
            _salle = salle;
            _daoReservation = daoReservation;
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
    }
}
