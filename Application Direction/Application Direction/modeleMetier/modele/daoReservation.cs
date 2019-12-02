using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ModeleMetier.metier;

namespace ModeleMetier.modele
{
    public class daoReservation : dao
    {
        private daoClient _daoClient;
        private daoSalle _daoSalle;

        public daoReservation(dbal dbal, daoClient daoClient, daoSalle daoSalle)
            : base(dbal)
        {
            _daoClient = daoClient;
            _daoSalle = daoSalle;
        }

        public override void insert(object o)
        {
            throw new NotImplementedException();
        }

        public override object select(string elements, string join_where)
        {
            List<dtoReservation> listReservation = new List<dtoReservation>();
            string request = "SELECT " + elements + " FROM reservation " + join_where + ";";
            DataTableCollection table = _dbal.select(request);

            for (int i = 0; i < table[0].Rows.Count; i++)
            {
                int id = (int)table[0].Rows[i]["id"];
                DateTime date = Convert.ToDateTime(table[0].Rows[i]["date"].ToString());
                string commentaire = (string)table[0].Rows[i]["commentaire"];
                int nbJoueur = (int)table[0].Rows[i]["nbJoueur"];
                int client_id = (int)table[0].Rows[i]["client_id"];
                int salle_id = (int)table[0].Rows[i]["salle_id"];

                List<dtoClient> lesClients = (List<dtoClient>)_daoClient.select("*", "WHERE id = " + client_id);
                List<dtoSalle> lesSalles = (List<dtoSalle>)_daoSalle.select("*", "WHERE id = " + salle_id);
                dtoClient client = lesClients[0];
                dtoSalle salle = lesSalles[0];

                listReservation.Add(new dtoReservation(id, date, commentaire, nbJoueur, client, salle));
            }

            return listReservation;
        }
    }
}
