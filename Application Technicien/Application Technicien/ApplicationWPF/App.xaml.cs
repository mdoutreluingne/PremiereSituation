using ModeleMetier.modele;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ApplicationWPF
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private dbal _dbal;
        private daoUtilisateur _daoUtilisateur;
        private daoArticle _daoArticle;
        private daoArticleSalle _daoArticleSalle;
        private daoClient _daoClient;
        private daoObstacle _daoObstacle;
        private daoReservation _daoReservation;
        private daoSalle _daoSalle;
        private daoTheme _daoTheme;
        private daoTransaction _daoTransaction;
        private daoVille _daoVille;

        public void App_Startup(object sender, StartupEventArgs e)
        {
            //_dbal = new dbal("172.31.135.1","admin","bdd_escape_game","admin");
            //_dbal = new dbal("172.31.135.2","admin","bdd_escape_game","admin");
            _dbal = new dbal("127.0.0.1", "root", "bdd_escape_game", "");
            _daoUtilisateur = new daoUtilisateur(_dbal);
            _daoArticle = new daoArticle(_dbal);
            _daoVille = new daoVille(_dbal);
            _daoTheme = new daoTheme(_dbal);
            _daoSalle = new daoSalle(_dbal, _daoVille, _daoTheme);
            _daoClient = new daoClient(_dbal, _daoVille);
            _daoReservation = new daoReservation(_dbal, _daoClient, _daoSalle);
            _daoTransaction = new daoTransaction(_dbal, _daoClient, _daoReservation);
            _daoObstacle = new daoObstacle(_dbal, _daoReservation, _daoArticle);
            _daoArticleSalle = new daoArticleSalle(_dbal, _daoArticle, _daoSalle);
            MainWindow wnd = new MainWindow(_daoUtilisateur, _daoArticle, _daoVille, _daoTheme, _daoSalle, _daoClient, _daoReservation, _daoTransaction, _daoObstacle, _daoArticleSalle);
            wnd.WindowState = WindowState.Maximized;
            wnd.Show();
        }
    }
}
