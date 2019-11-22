using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using CoucheModele.metier;

namespace CoucheModele.modele
{
    public class daoVille
    {
        private dbal dbal;

        public daoVille(dbal dbal)
        {
            this.dbal = dbal;
        }
        public void insert(Ville ville)
        {
            this.dbal.execRequete("INSERT INTO ville (id, nom) VALUES (" + ville.Id + ", '" + ville.Nom + "');");
        }
        public void delete(Ville ville)
        {
            this.dbal.execRequete("DELETE FROM ville WHERE id = " + ville.Id + ";");
        }
        public Ville selectById(int clientVille)
        {

            string select = "SELECT * FROM ville WHERE id = " + clientVille + ";";
            DataTable pays = this.dbal.selectAll(select);
            Ville v = new Ville((int)pays.Rows[0]["id"], (string)pays.Rows[0]["nom"]);
            return v;

        }
        public List<Ville> selectAllVille()
        {

            List<Ville> lesVilles = new List<Ville>();
            DataTable table = this.dbal.selectAll("SELECT * FROM ville ORDER BY nom ASC");

            for (int i = 0; i < table.Rows.Count; i++)
            {
                int id = (int)table.Rows[i]["id"];
                string nom = table.Rows[i]["nom"].ToString();
                lesVilles.Add(new Ville(id, nom));

            }
            return lesVilles;
        }
    }
}
