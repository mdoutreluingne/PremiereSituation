using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using CoucheModele.metier;

namespace CoucheModele.modele
{
    public class daoTransaction
    {
        private dbal dbal;
        private daoClient unDaoClient;
        private daoReservation unDaoReservation;

        public daoTransaction(dbal dbal, daoClient unDaoClient, daoReservation unDaoReservation)
        {
            this.dbal = dbal;
            this.unDaoClient = unDaoClient;
            this.unDaoReservation = unDaoReservation;
        }
        public void insert(Transaction transaction)
        {
            this.dbal.execRequete("INSERT INTO transaction (date, montant, type, numero, commentaire, client_id) VALUES ('" + transaction.Date.ToString("yyyy-MM-dd HH:mm") + "', " + transaction.Montant + ", '" + transaction.Type + "', '" + transaction.Numero + "', '" + transaction.Commentaire + "', " + transaction.Client_id.Id + ");");
        }
        public void delete(Transaction transaction)
        {
            this.dbal.execRequete("DELETE FROM transaction WHERE id = " + transaction.Id + ";");
        }
        public object soldes_client(int idClient)
        {
            object soldes = 0;
            DataTable table = this.dbal.selectMontant("SELECT SUM(montant) as soldes FROM transaction WHERE client_id = " + idClient + ";");
            soldes = table.Rows[0]["soldes"];
            return soldes;
        }
        public List<Transaction> selectAllHistorique(int idClient)
        {
            List<Transaction> lesHistoriques = new List<Transaction>();
            DataTable table = this.dbal.selectAll("SELECT * FROM transaction WHERE client_id = " + idClient + " ORDER BY date DESC;");

            for (int i = 0; i < table.Rows.Count; i++)
            {
                object id = table.Rows[i]["id"];
                DateTime date = (DateTime)table.Rows[i]["date"];
                decimal montant = Convert.ToDecimal(table.Rows[i]["montant"]);
                string type = table.Rows[i]["type"].ToString();
                object num = table.Rows[i]["numero"];
                object com = table.Rows[i]["commentaire"];
                object reserv = table.Rows[i]["reservation_id"];
                Reservation r = new Reservation();
                if (reserv.GetType() != typeof(DBNull)) //Si reservation_id n'est pas null
                {
                     r = unDaoReservation.selectById(reserv);
                }

                Client c = unDaoClient.selectById((int)table.Rows[i]["client_id"]);
                lesHistoriques.Add(new Transaction(id, date, montant, type, num.ToString(), com.ToString(), r, c));

            }
            return lesHistoriques;

        }

    }
}
