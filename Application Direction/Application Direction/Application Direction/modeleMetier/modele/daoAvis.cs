using ModeleMetier.metier;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ModeleMetier.modele
{
    public class daoAvis : dao
    {
        private daoSalle _daoSalle;
        private daoClient _daoClient;
        public daoAvis(dbal dbal, daoSalle daoSalle, daoClient daoClient)
            : base(dbal)
        {
            _daoSalle = daoSalle;
            _daoClient = daoClient; 
        }

        public override void insert(object o)
        {
            throw new NotImplementedException();
        }

        public override object select(string elements, string join_where)
        {
            List<dtoAvis> listAvis = new List<dtoAvis>();
            string request = "SELECT " + elements + " FROM avis " + join_where + ";";
            DataTableCollection table = _dbal.select(request);

            for (int i = 0; i < table[0].Rows.Count; i++)
            {
                int id = (int)table[0].Rows[i]["id"];
                double note = (double)table[0].Rows[i]["note"];
                DateTime date = Convert.ToDateTime(table[0].Rows[i]["date"].ToString());
                string commentaire = (string)table[0].Rows[i]["commentaire"];
                int salle_id = (int)table[0].Rows[i]["salle_id"];
                int client_id = (int)table[0].Rows[i]["client_id"];

                List<dtoSalle> les_salles = (List<dtoSalle>)_daoSalle.select("*", "WHERE id = " + salle_id);
                List<dtoClient> les_clients = (List<dtoClient>)_daoClient.select("*", "WHERE id = " + client_id);
                dtoSalle salle = les_salles[0];
                dtoClient client = les_clients[0];
                listAvis.Add(new dtoAvis(id, note, date, commentaire, salle, client));
            }
            return listAvis;
        }

        public override object update(string elementss, string join_where)
        {
            throw new NotImplementedException();
        }
    }
   
}
