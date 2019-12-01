using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ModeleMetier.metier;

namespace ApplicationWPF.MVVM
{
    public class ViewPlanning : ViewModele
    {
        private ViewPlanning _origin;
        private dtoReservation _reservation;
        private System.Windows.Visibility _visibilite;
        private ICommand _icommand;
        public ViewPlanning(ViewPlanning origin, dtoReservation reservation, MainWindow main)
            : base(main)
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
            ViewDate.Instance(null, null, null, null).Visibilite = System.Windows.Visibility.Hidden;
            ViewReservation.Instance(_mainWindow, null, null, null, null, null, null, System.Windows.Visibility.Visible).LoadReservation = _reservation;

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
