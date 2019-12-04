using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ModeleMetier.metier;

namespace ModeleMetier.modele
{
    public class daoSalle : dao
    {
        private daoVille _daoVille;
        private daoTheme _daoTheme;
        public daoSalle(dbal dbal, daoVille daoVille, daoTheme daoTheme)
            : base(dbal)
        {
            _daoVille = daoVille;
            _daoTheme = daoTheme;
        }

        public override void insert(object o)
        {
            throw new NotImplementedException();
        }

        public override object select(string elements, string join_where)
        {
            List<dtoSalle> listSalle = new List<dtoSalle>();
            string request = "SELECT " + elements + " FROM salle " + join_where + ";";
            DataTableCollection table = _dbal.select(request);

            for (int i = 0; i < table[0].Rows.Count; i++)
            {
                object a = table[0].Rows[i]["heure_ouverture"];
                int id = (int)table[0].Rows[i]["id"];
                int ville_id = (int)table[0].Rows[i]["ville_id"];
                int numero = (int)table[0].Rows[i]["numero"];
                decimal prix = (decimal)table[0].Rows[i]["prix"];

                DateTime heure_ouverture = Convert.ToDateTime(table[0].Rows[i]["heure_ouverture"].ToString());
                DateTime heure_fermeture = Convert.ToDateTime(table[0].Rows[i]["heure_fermeture"].ToString());
                bool archive = (bool)table[0].Rows[i]["archive"];
                int theme_id = (int)table[0].Rows[i]["theme_id"];

                List<dtoTheme> lesThemes = (List<dtoTheme>)_daoTheme.select("*", "WHERE id = " + theme_id);
                List<dtoVille> lesVilles = (List<dtoVille>)_daoVille.select("*", "WHERE id = " + ville_id);
                dtoTheme theme = lesThemes[0];
                dtoVille ville = lesVilles[0];

                listSalle.Add(new dtoSalle(id, ville, numero, prix, heure_ouverture, heure_fermeture, archive, theme));
            }
            return listSalle;
        }

       public override object update(object o)
        {
            dtoSalle Salle =(dtoSalle)o;


            // List<dtoVille> lesVilles = (List<dtoVille>)_daoVille.select("*", "WHERE nom = " + ville_id);
               
            string request = "Update salle(ville_id,numero,prix,heure_ouverture,heure_fermeture,archive,theme_id) " +
                "Values" + 
                Salle.DtoVille.Nom + "," + Salle.Numero.ToString() + "," + Salle.DtoTheme.ToString() + "," + Salle.Prix.ToString() + "," + 
                Salle.Heure_ouverture.ToString() + "," + Salle.Heure_fermeture.ToString()+ "," + Salle.Archive.ToString()+ ";";

            

            return Salle; 
        }
    }
}
