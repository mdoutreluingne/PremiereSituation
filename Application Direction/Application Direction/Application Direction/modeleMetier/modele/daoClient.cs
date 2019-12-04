using ModeleMetier.metier;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ModeleMetier.modele
{
    public class daoClient : dao
    {
        private daoVille _daoVille;
        public daoClient(dbal dbal, daoVille daoVille)
            : base(dbal)
        {
            _daoVille = daoVille;
        }

        public override void insert(object o)
        {
            throw new NotImplementedException();
        }

        public override object select(string elements, string join_where)
        {
            List<dtoClient> listClient = new List<dtoClient>();
            string request = "SELECT " + elements + " FROM client " + join_where + ";";
            DataTableCollection table = _dbal.select(request);
            for (int i = 0; i < table[0].Rows.Count; i++)
            {
                int id = (int)table[0].Rows[i]["id"];
                string nom = (string)table[0].Rows[i]["nom"];
                string prenom = (string)table[0].Rows[i]["prenom"];
                int ville_id = (int)table[0].Rows[i]["ville_id"];
                string tel = (string)table[0].Rows[i]["tel"];
                string mail = (string)table[0].Rows[i]["mail"];
                bool archive = (bool)table[0].Rows[i]["archive"];
                List<dtoVille> les_villes = (List<dtoVille>)_daoVille.select("*", "WHERE id = " + ville_id);
                dtoVille ville = les_villes[0];
                listClient.Add(new dtoClient(id, nom, prenom, ville, tel, mail, archive));
            }
            return listClient;

        }

        public override object update(object o)
        {
            throw new NotImplementedException();
        }
    }
}
