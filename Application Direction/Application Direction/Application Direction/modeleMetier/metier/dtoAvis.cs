using System;
using System.Collections.Generic;
using System.Text;

namespace ModeleMetier.metier
{
    public class dtoAvis
    {
        #region Attributs
        private int _id;
        private double _note;
        private DateTime _date;
        private string _commentaire;
        private dtoSalle _salle;
        private dtoClient _client;
        #endregion

        #region Constructeur
        public dtoAvis(int id, double note, DateTime date, string commentaire, dtoSalle salle, dtoClient client)
        {
            _id = id;
            _note = note;
            _date = date;
            _commentaire = commentaire;
            _salle = salle;
            _client = client;
        }

        #endregion

        #region Accesseurs
        public int Id { get => _id; set => _id = value; }
        public double Note { get => _note; set => _note = value; }
        public DateTime Date { get => _date; set => _date = value; }
        public string Commentaire { get => _commentaire; set => _commentaire = value; }
        public dtoSalle Salle { get => _salle; set => _salle = value; }
        public dtoClient Client { get => _client; set => _client = value; } 
        #endregion
    }
}
