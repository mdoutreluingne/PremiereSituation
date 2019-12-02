using ModeleMetier.modele;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WPFApplication
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private dbal _dbal;
        private daoUtilisateur _daoUtilisateur;
        private daoAvis _daoAvis;
        private daoPartie _daoPartie;
        private daoClient _daoClient;
        private daoReservation _daoReservation;
        private daoSalle _daoSalle;
        private daoTheme _daoTheme;
        private daoVille _daoVille;

        public void App_Startup(object sender, StartupEventArgs e)
        {
           // _dbal = new dbal("172.31.135.1","admin","bdd_escape_game","admin");
            _dbal = new dbal("127.0.0.1", "root", "bdd_escape_game", "");
            _daoUtilisateur = new daoUtilisateur(_dbal);
            _daoVille = new daoVille(_dbal);
            _daoTheme = new daoTheme(_dbal);
            _daoSalle = new daoSalle(_dbal, _daoVille, _daoTheme);
            _daoClient = new daoClient(_dbal, _daoVille);
            _daoReservation = new daoReservation(_dbal, _daoClient, _daoSalle);
            _daoAvis = new daoAvis(_dbal, _daoSalle, _daoClient);
            _daoPartie = new daoPartie(_dbal, _daoReservation);
            MainWindow wnd = new MainWindow(_daoUtilisateur, _daoVille, _daoTheme, _daoSalle, _daoClient, _daoReservation, _daoAvis, _daoPartie);
            wnd.WindowState = WindowState.Maximized;
            wnd.Show();
        }
    }
}
