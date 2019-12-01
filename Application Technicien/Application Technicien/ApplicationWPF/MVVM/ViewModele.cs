using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationWPF.MVVM
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
}
