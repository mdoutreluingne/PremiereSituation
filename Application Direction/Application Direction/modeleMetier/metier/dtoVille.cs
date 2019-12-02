using System;
using System.Collections.Generic;
using System.Text;

namespace ModeleMetier.metier
{
    public class dtoVille
    {
        #region Attributs
        private int _id;
        private string _nom; 
        #endregion

        #region Constructeur
        public dtoVille(int id, string nom)
        {
            _id = id;
            _nom = nom;
        } 
        #endregion

        #region Accesseurs
        public int Id { get => _id; set => _id = value; }
        public string Nom { get => _nom; set => _nom = value; } 
        #endregion
    }
}