using System;
using System.Collections.Generic;
using System.Text;

namespace ModeleMetier.metier
{
    public class dtoClient
    {
        #region Attributs
        private int _id;
        private string _nom;
        private string _prenom;
        private dtoVille _dtoVille;
        private string _tel;
        private string _mail;
        private bool _archive;
        #endregion

        #region Constructeurs
        public dtoClient(int id, string nom, string prenom, dtoVille dtoVille, string tel, string mail, bool archive)
        {
            _id = id;
            _nom = nom;
            _prenom = prenom;
            _dtoVille = dtoVille;
            _tel = tel;
            _mail = mail;
            _archive = archive;
        }
        #endregion

        #region Accesseurs
        public int Id { get => _id; set => _id = value; }
        public string Nom { get => _nom; set => _nom = value; }
        public string Prenom { get => _prenom; set => _prenom = value; }
        public string Tel { get => _tel; set => _tel = value; }
        public string Mail { get => _mail; set => _mail = value; }
        public bool Archive { get => _archive; set => _archive = value; }
        internal dtoVille DtoVille { get => _dtoVille; set => _dtoVille = value; } 
        #endregion
    }
}