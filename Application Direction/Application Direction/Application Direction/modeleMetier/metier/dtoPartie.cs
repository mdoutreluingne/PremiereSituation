using System;
using System.Collections.Generic;
using System.Text;


namespace ModeleMetier.metier
{
    public class dtoPartie
    {
        #region Attributs
        private dtoReservation _resa;
        private DateTime _temps;
        #endregion

        #region Constructeur
        public dtoPartie(dtoReservation resa, DateTime temps)
        {
            _resa = resa;
            _temps = temps;
        }
        #endregion

        #region Accesseurs
        public dtoReservation Resa { get => _resa; set => _resa = value; }
        public DateTime Temps { get => _temps; set => _temps = value; }
        #endregion
    }
}
