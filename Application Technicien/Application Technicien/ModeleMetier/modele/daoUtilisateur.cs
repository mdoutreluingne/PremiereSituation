using ModeleMetier.metier;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ModeleMetier.modele
{
    public class daoUtilisateur : dao
    {
        private daoClient _daoClient;
        public daoUtilisateur(dbal dbal, daoClient daoClient)
            : base(dbal)
        {
            _daoClient = daoClient;
        }

        public override void insert(object o)
        {
            throw new NotImplementedException();
        }

        public void update(string login, string password)
        {
            string request = "UPDATE utilisateur SET mdp = '" + password + "' WHERE login = '" + login + "'";
            _dbal.command(request);
        }

        public override object select(string elements, string join_where)
        {
            List<dtoUtilisateur> listUtilisateur = new List<dtoUtilisateur>();
            string request = "SELECT " + elements + " FROM utilisateur " + join_where + ";";
            DataTableCollection table = _dbal.select(request);

            for (int i = 0; i < table[0].Rows.Count; i++)
            {
                string login = (string)table[0].Rows[i]["login"];
                string mdp = (string)table[0].Rows[i]["mdp"];
                string role = (string)table[0].Rows[i]["roles"];
                object client_id = (object)table[0].Rows[i]["client_id"];

                dtoClient client = null;
                
                if (client_id.GetType() != typeof(DBNull))
                {
                    List<dtoClient> lesClients = (List<dtoClient>)_daoClient.select("*", "WHERE id = " + client_id);
                    client = lesClients[0];
                }

                listUtilisateur.Add(new dtoUtilisateur(login, mdp, role, client));
            }
            return listUtilisateur;
        }
    }
}
