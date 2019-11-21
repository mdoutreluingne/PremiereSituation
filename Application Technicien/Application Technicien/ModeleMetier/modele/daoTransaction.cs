using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ModeleMetier.metier;

namespace ModeleMetier.modele
{
    public class daoTransaction:dao
    {

        private daoClient _daoClient;
        private daoReservation _daoReservation;
        public daoTransaction(dbal dbal, daoClient daoClient, daoReservation daoReservation)
            : base(dbal)
        {
            _daoClient = daoClient;
            _daoReservation = daoReservation;
        }
        public override void insert(object o)
        {
            throw new NotImplementedException();
        }

        public override object select(string elements, string join_where)
        {
            List<dtoTransaction> listTransaction = new List<dtoTransaction>();
            string request = "SELECT " + elements + " FROM transaction " + join_where + ";";
            DataTableCollection table = _dbal.select(request);

            for (int i = 0; i < table[0].Rows.Count; i++)
            {
                object g = table[0].Rows[i];
                int id = (int)table[0].Rows[i]["id"];
                DateTime date = Convert.ToDateTime(table[0].Rows[i]["date"].ToString());
                decimal montant = (decimal)table[0].Rows[i]["montant"];
                string type = (string)table[0].Rows[i]["type"];
                object numero = table[0].Rows[i]["numero"];
                object commentaire = table[0].Rows[i]["commentaire"];
                object reservation_id = table[0].Rows[i]["reservation_id"];
                int client_id = (int)table[0].Rows[i]["client_id"];

                List<dtoClient> lesClients = (List<dtoClient>)_daoClient.select("*", "WHERE id = " + client_id);
                dtoClient client = lesClients[0];
                if (reservation_id.GetType() != typeof(DBNull))
                {
                    List<dtoReservation> lesReservations = (List<dtoReservation>)_daoReservation.select("*", "WHERE id = " + reservation_id);
                    dtoReservation reservation = lesReservations[0];
                    listTransaction.Add(new dtoTransaction(id, date, montant, type, numero.ToString(), commentaire.ToString(), reservation, client));
                }
                else
                {
                    listTransaction.Add(new dtoTransaction(id, date, montant, type, numero.ToString(), commentaire.ToString(), client));
                }
            }
            return listTransaction;
        }
    }
}
