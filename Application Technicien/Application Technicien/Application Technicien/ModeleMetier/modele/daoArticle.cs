using ModeleMetier.metier;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ModeleMetier.modele
{
    public class daoArticle : dao
    {
        public daoArticle(dbal dbal)
            : base(dbal)
        {

        }

        public override void insert(object o)
        {
            throw new NotImplementedException();
        }

        public override object select(string elements, string join_where)
        {
            List<dtoArticle> listArticle = new List<dtoArticle>();
            string request = "SELECT " + elements + " FROM article " + join_where + ";";
            DataTableCollection table = _dbal.select(request);

            for (int i = 0; i < table[0].Rows.Count; i++)
            {
                int id = (int)table[0].Rows[i]["id"];
                string libelle = (string)table[0].Rows[i]["libelle"];
                decimal montant = (decimal)table[0].Rows[i]["montant"];
                bool archive = (bool)table[0].Rows[i]["archive"];
                listArticle.Add(new dtoArticle(id, libelle, montant, archive));
            }
            return listArticle;
        }
    }
}
