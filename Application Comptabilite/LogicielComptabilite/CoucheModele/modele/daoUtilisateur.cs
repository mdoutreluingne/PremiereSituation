using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using CoucheModele.metier;

namespace CoucheModele.modele
{
    public class daoUtilisateur
    {
        private dbal dbal;
        public daoUtilisateur(dbal dbal)
        {
            this.dbal = dbal;
        }
        public void update(string requete)
        {
            this.dbal.execRequete(requete);
        }
        public List<Utilisateur> selectAllClient()
        {
            List<Utilisateur> lesUsers = new List<Utilisateur>();
            DataTable table = this.dbal.selectAll("SELECT * FROM utilisateur");

            for (int i = 0; i < table.Rows.Count; i++)
            {
                string login = table.Rows[i]["login"].ToString();
                string mdp = table.Rows[i]["mdp"].ToString();
                string role = table.Rows[i]["role"].ToString();
                lesUsers.Add(new Utilisateur(login, mdp, role));

            }
            return lesUsers;
        }
    }
}
