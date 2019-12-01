using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ApplicationWPF.MVVM
{
    public class ViewEntete : ViewModele
    {
        private static ViewEntete _instance = null;
        private static readonly object _padlock = new object();
        private ViewPlanning _viewPlanning;

        private ICommand _icommandGoPlanning;
        private ICommand _icommandGoGestionArticle;
        ViewEntete(MainWindow main, ViewPlanning viewPlanning)
            : base(main)
        {
            _viewPlanning = viewPlanning;
        }

        //singleton
        public static ViewEntete Instance(MainWindow main, ViewPlanning viewPlanning)
        {
            lock (_padlock)
            {
                if (_instance == null)
                {
                    _instance = new ViewEntete(main, viewPlanning);
                }
                return _instance;
            }
        }

        public ICommand GoPlanning
        {
            get
            {
                if (this._icommandGoPlanning == null)
                    this._icommandGoPlanning = new RelayCommand(() => this.goPlanning(), () => true);

                return this._icommandGoPlanning;
            }
        }
        public ICommand GoGestionArticle
        {
            get
            {
                if (this._icommandGoGestionArticle == null)
                    this._icommandGoGestionArticle = new RelayCommand(() => this.goArticle(), () => true);

                return this._icommandGoGestionArticle;
            }
        }

        public void goPlanning()
        {
            if (_viewPlanning.Visibilite == Visibility.Hidden)
            {
                MessageBoxResult mr = MessageBox.Show("Êtes vous sûr de vouloir retourner au planning ?", "Planning", MessageBoxButton.YesNo);
                if (mr == MessageBoxResult.Yes)
                {
                    ViewReservation.Instance(null, null, null, null, null, null, null, Visibility.Hidden);
                    ViewReservation.Instance(null, null, null, null, null, null, null, Visibility.Hidden).SelectClient = null;
                    ViewObjet.Instance(null, null, null, null, null, null, null).Visibilite = Visibility.Hidden;
                    ViewArticle.Instance(null, null, null, null, null).Visibilite = Visibility.Hidden;
                    ViewDate.Instance(null, null, null, null).Visibilite = Visibility.Visible;
                    _viewPlanning.Visibilite = Visibility.Visible;
                }
            }
        }
        public void goArticle()
        {
            ViewArticle viewA = ViewArticle.Instance(null, null, null, null, null);
            if (viewA.Visibilite == Visibility.Hidden)
            {
                MessageBoxResult mr = MessageBox.Show("Êtes vous sûr de vouloir rejoindre le menu de gestion des articles ?", "Gestion des articles", MessageBoxButton.YesNo);
                if (mr == MessageBoxResult.Yes)
                {
                    ViewReservation.Instance(null, null, null, null, null, null, null, Visibility.Hidden);
                    ViewObjet.Instance(null, null, null, null, null, null, null).Visibilite = Visibility.Hidden;
                    ViewDate.Instance(null, null, null, null).Visibilite = Visibility.Hidden;
                    _viewPlanning.Visibilite = Visibility.Hidden;
                    viewA.Visibilite = Visibility.Visible;
                }
            }
        }
    }
}
