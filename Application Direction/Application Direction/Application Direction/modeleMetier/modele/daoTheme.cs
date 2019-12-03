using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ModeleMetier.metier;

namespace ModeleMetier.modele
{
    public class daoTheme : dao
    {
        public daoTheme(dbal dbal)
            : base(dbal)
        {

        }

        public override void insert(object o)
        {
            throw new NotImplementedException();
        }

        public override object select(string elements, string join_where)
        {
            List<dtoTheme> listTheme = new List<dtoTheme>();
            string request = "SELECT " + elements + " FROM theme " + join_where + ";";
            DataTableCollection table = _dbal.select(request);

            for (int i = 0; i < table[0].Rows.Count; i++)
            {
                int id = (int)table[0].Rows[i]["id"];
                string nom = (string)table[0].Rows[i]["nom"];
                listTheme.Add(new dtoTheme(id, nom));
            }
            return listTheme;
        }

        public override object update(string elementss, string join_where)
        {
            throw new NotImplementedException();
        }
    }
}
