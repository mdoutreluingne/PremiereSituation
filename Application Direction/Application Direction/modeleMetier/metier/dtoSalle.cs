using System;
using System.Collections.Generic;
using System.Text;

namespace ModeleMetier.metier
{
    public class dtoSalle
    {
        #region Attributs
        private int _id;
        private dtoVille _dtoVille;
        private int _numero;
        private decimal _prix;
        private DateTime _heure_ouverture;
        private DateTime _heure_fermeture;
        private bool _archive;
        private dtoTheme _dtoTheme; 
        #endregion

        #region Constructeur
        public dtoSalle(int id, dtoVille dtoVille, int numero, decimal prix, DateTime heure_ouverture, DateTime heure_fermeture, bool archive, dtoTheme dtoTheme)
        {
            _id = id;
            _dtoVille = dtoVille;
            _numero = numero;
            _prix = prix;
            _heure_ouverture = heure_ouverture;
            _heure_fermeture = heure_fermeture;
            _archive = archive;
            _dtoTheme = dtoTheme;
        } 
        #endregion

        #region Accesseurs
        public int Id { get => _id; set => _id = value; }
        public int Numero { get => _numero; set => _numero = value; }
        public decimal Prix { get => _prix; set => _prix = value; }
        public DateTime Heure_ouverture { get => _heure_ouverture; set => _heure_ouverture = value; }
        public DateTime Heure_fermeture { get => _heure_fermeture; set => _heure_fermeture = value; }
        public bool Archive { get => _archive; set => _archive = value; }
        internal dtoVille DtoVille { get => _dtoVille; set => _dtoVille = value; }
        internal dtoTheme DtoTheme { get => _dtoTheme; set => _dtoTheme = value; } 
        #endregion
    }
}