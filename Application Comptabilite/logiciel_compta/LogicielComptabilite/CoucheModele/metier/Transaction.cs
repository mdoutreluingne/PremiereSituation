using System;
using System.Collections.Generic;
using System.Text;

namespace CoucheModele.metier
{
    public class Transaction
    {
        private object id;
        private DateTime date;
        private decimal montant;
        private string type;
        private string numero;
        private string commentaire;
        private Reservation reservation_id;
        private Client client_id;

        public Transaction()
        {
            Id = 0;
            Date = new DateTime();
            Montant = 0;
            Type = "";
            Numero = "";
            Commentaire = "";
            Reservation_id = new Reservation();
            Client_id = new Client();
        }
        public Transaction(object unId, DateTime uneDate, decimal unMontant, string unType, string unNum, string unCommentaire, Reservation uneReservation, Client unCLient)
        {
            Id = unId;
            Date = uneDate;
            Montant = unMontant;
            Type = unType;
            Numero = unNum;
            Commentaire = unCommentaire;
            Reservation_id = uneReservation;
            Client_id = unCLient;
        }
        public Transaction(object unId, DateTime uneDate, decimal unMontant, string unType, string unNum, string unCommentaire, Client unCLient)
        {
            Id = unId;
            Date = uneDate;
            Montant = unMontant;
            Type = unType;
            Numero = unNum;
            Commentaire = unCommentaire;
            Reservation_id = new Reservation();
            Client_id = unCLient;
        }

        public object Id { get => id; set => id = value; }
        public DateTime Date { get => date; set => date = value; }
        public decimal Montant { get => montant; set => montant = value; }
        public string Type { get => type; set => type = value; }
        public string Numero { get => numero; set => numero = value; }
        public string Commentaire { get => commentaire; set => commentaire = value; }
        public Reservation Reservation_id { get => reservation_id; set => reservation_id = value; }
        public Client Client_id { get => client_id; set => client_id = value; }
        public override string ToString()
        {
            return Date + "                                         " + Montant;
        }
    }
}
