using System;
using System.Collections.Generic;
using System.Text;

namespace ModeleMetier.metier
{
    public class dtoArticle
    {
        #region Attributs
        private int _id;
        private string _libelle;
        private decimal _montant;
        private bool _archive; 
        #endregion

        #region Constructeur
        public dtoArticle(int id, string libelle, decimal montant, bool archive)
        {
            _id = id;
            _libelle = libelle;
            _montant = montant;
            _archive = archive;
        } 
        #endregion

        #region Acccesseurs
        public int Id { get => _id; set => _id = value; }
        public string Libelle { get => _libelle; set => _libelle = value; }
        public decimal Montant { get => _montant; set => _montant = value; }
        public bool Archive { get => _archive; set => _archive = value; }
        #endregion

        #region Méthodes
        public override string ToString()
        {
            return _libelle.ToUpper();
        } 
        #endregion
    }
}