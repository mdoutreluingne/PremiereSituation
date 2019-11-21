using System;
using System.Collections.Generic;
using System.Text;

namespace ModeleMetier.metier
{
    public class dtoTransaction
    {
        #region Attributs
        private int _id;
        private DateTime _date;
        private decimal _montant;
        private string _type;
        private string _numero;
        private string _commentaire;
        private dtoReservation _dtoReservation;
        private dtoClient _dtoClient; 
        #endregion

        #region Constructeur
        public dtoTransaction(int id, DateTime date, decimal montant, string type, string numero, string commentaire, dtoReservation dtoReservation, dtoClient dtoClient)
        {
            _id = id;
            _date = date;
            _montant = montant;
            _type = type;
            _numero = numero;
            _commentaire = commentaire;
            _dtoReservation = dtoReservation;
            _dtoClient = dtoClient;
        }

        public dtoTransaction(int id, DateTime date, decimal montant, string type, string numero, string commentaire, dtoClient dtoClient)
        {
            _id = id;
            _date = date;
            _montant = montant;
            _type = type;
            _numero = numero;
            _commentaire = commentaire;
            _dtoClient = dtoClient;
        }
        #endregion

        #region Accesseurs
        public int Id { get => _id; set => _id = value; }
        public DateTime Date { get => _date; set => _date = value; }
        public decimal Montant { get => _montant; set => _montant = value; }
        public string Type { get => _type; set => _type = value; }
        public string Numero { get => _numero; set => _numero = value; }
        public string Commentaire { get => _commentaire; set => _commentaire = value; }
        internal dtoReservation DtoReservation { get => _dtoReservation; set => _dtoReservation = value; }
        internal dtoClient DtoClient { get => _dtoClient; set => _dtoClient = value; } 
        #endregion
    }
}
