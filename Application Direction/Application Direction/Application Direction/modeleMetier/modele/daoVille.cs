using ModeleMetier.metier;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ModeleMetier.modele
{
    public class daoVille : dao
    {
        private daoVille _daoVille;
        public daoVille(dbal dbal)
            : base(dbal)
        {

        }

        public override void insert(object o)
        {
            throw new NotImplementedException();
        }

        public override object select(string elements, string join_where)
        {
            List<dtoVille> listVille = new List<dtoVille>();
            string request = "SELECT " + elements + " FROM ville " + join_where + ";";
            DataTableCollection table = _dbal.select(request);

            for (int i = 0; i < table[0].Rows.Count; i++)
            {
                int id = (int)table[0].Rows[i]["id"];
                string nom = (string)table[0].Rows[i]["nom"];
                listVille.Add(new dtoVille(id, nom));
            }
            return listVille;
        }

        public override object update(string elementss, string join_where)
        {
            throw new NotImplementedException();
        }
    }
}