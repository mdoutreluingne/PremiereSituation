using System;
using System.Collections.Generic;
using System.Text;

namespace ModeleMetier.metier
{
    public class dtoObstacle
    {
        #region Attributs
        private int _id;
        private int _position;
        private string _commentaire;
        private int _qte;
        private dtoArticle _dtoArticle;
        private dtoReservation _dtoReservation; 
        #endregion

        public dtoObstacle(int id, int position, string commentaire, int qte, dtoArticle dtoArticle, dtoReservation dtoReservation)
        {
            _id = id;
            _position = position;
            _commentaire = commentaire;
            _qte = qte;
            _dtoArticle = dtoArticle;
            _dtoReservation = dtoReservation;
        }

        #region Accesseurs
        public int Id { get => _id; set => _id = value; }
        public int Position { get => _position; set => _position = value; }
        public string Commentaire { get => _commentaire; set => _commentaire = value; }
        public int Qte { get => _qte; set => _qte = value; }
        public dtoArticle DtoArticle { get => _dtoArticle; set => _dtoArticle = value; }
        public dtoReservation DtoReservation { get => _dtoReservation; set => _dtoReservation = value; }
        #endregion

        public override string ToString()
        {
            return _dtoArticle.Libelle + " |Position : " + _position + " |Qantite : "  + _qte;
        }
    }
}
