using System;
using System.Collections.Generic;
using System.Text;

namespace CoucheModele.metier
{
    public class Reservation
    {
        private int id;
        private DateTime date;
        private string commentaire;
        private int nbJoueur;
        private Client client_id;
        private Salle salle_id;

        public Reservation()
        {
            id = 0;
            date = new DateTime();
            commentaire = "";
            nbJoueur = 0;
            client_id = null;
            salle_id = null;

        }
        public Reservation(int unId, DateTime uneDate, string unCommentaire, int unNbJoueur, Client clientId, Salle salleId)
        {
            id = unId;
            date = uneDate;
            commentaire = unCommentaire;
            nbJoueur = unNbJoueur;
            client_id = clientId;
            salle_id = salleId;

        }

        public int Id { get => id; set => id = value; }
        public DateTime Date { get => date; set => date = value; }
        public string Commentaire { get => commentaire; set => commentaire = value; }
        public int NbJoueur { get => nbJoueur; set => nbJoueur = value; }
        public Client Client_id { get => client_id; set => client_id = value; }
        public Salle Salle_id { get => salle_id; set => salle_id = value; }
    }
}
