using System;
using System.Collections.Generic;
using System.Text;

namespace ModeleMetier.metier
{
    public class dtoReservation
    {
        #region Attributs
        private int _id;
        private DateTime _date;
        private string _commentaire;
        private int _nbJoueur;
        private dtoClient _client;
        private dtoSalle _dtoSalle;
        #endregion

        #region Constructeur
        public dtoReservation() { }
        public dtoReservation(int id, DateTime date, string commentaire, int nbJoueur, dtoClient client, dtoSalle dtoSalle)
        {
            _id = id;
            _date = date;
            _commentaire = commentaire;
            _nbJoueur = nbJoueur;
            _client = client;
            _dtoSalle = dtoSalle;
        } 
        #endregion

        #region Accesseurs
        public int Id { get => _id; set => _id = value; }
        public DateTime Date { get => _date; set => _date = value; }
        public string Commentaire { get => _commentaire; set => _commentaire = value; }
        public int NbJoueur { get => _nbJoueur; set => _nbJoueur = value; }
        internal dtoClient Client { get => _client; set => _client = value; }
        internal dtoSalle DtoSalle { get => _dtoSalle; set => _dtoSalle = value; } 
        #endregion
    }
}
