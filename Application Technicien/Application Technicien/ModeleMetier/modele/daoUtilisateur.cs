using ModeleMetier.metier;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ModeleMetier.modele
{
    public class daoUtilisateur : dao
    {
        public daoUtilisateur(dbal dbal)
            : base(dbal)
        {

        }

        public override void insert(object o)
        {
            throw new NotImplementedException();
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
                string role = (string)table[0].Rows[i]["role"];
                listUtilisateur.Add(new dtoUtilisateur(login, mdp, role));
            }
            return listUtilisateur;
        }
    }
}
