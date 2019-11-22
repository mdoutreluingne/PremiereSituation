using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using CoucheModele.metier;

namespace CoucheModele.modele
{
    public class daoClient
    {
        private dbal dbal;
        private daoVille uneVille;

        public daoClient(dbal dbal, daoVille unDaoVille)
        {
            this.dbal = dbal;
            this.uneVille = unDaoVille;
        }
        public void insert(Client client)
        {
            this.dbal.execRequete("INSERT INTO client (nom, prenom, ville_id, tel, mail, archive) VALUES ('" + client.Nom + "', '" + client.Prenom + "', " + client.Ville_id.Id + ", '" + client.Tel + "', '" + client.Mail + "', " + client.Archive + ");");
        }
        public void delete(Client client)
        {
            this.dbal.execRequete("DELETE FROM client WHERE id = " + client.Id + ";");
        }
        public void archiver(Client client)
        {
            this.dbal.execRequete("UPDATE client SET archive = 1 WHERE id = " + client.Id + ";");
        }
        public void desarchiver(Client client)
        {
            this.dbal.execRequete("UPDATE client SET archive = 0 WHERE id = " + client.Id + ";");
        }
        public void selectNom(Client client)
        {
            this.dbal.execRequete("SELECT nom FROM client WHERE id = " + client.Id + ";");
        }

        public List<Client> selectAllClient()
        {

            List<Client> lesClients = new List<Client>();
            DataTable table = this.dbal.selectAll("SELECT * FROM client WHERE archive = 0 ORDER BY nom ASC");

            for (int i = 0; i < table.Rows.Count; i++)
            {
                int id = (int)table.Rows[i]["id"];
                string nom = table.Rows[i]["nom"].ToString();
                string prenom = table.Rows[i]["prenom"].ToString();
                Ville v = uneVille.selectById((int)table.Rows[i]["ville_id"]);
                string tel = table.Rows[i]["tel"].ToString();
                string mail = table.Rows[i]["mail"].ToString();
                bool archive = (bool)table.Rows[i]["archive"];
                lesClients.Add(new Client(id, nom, prenom, v, tel, mail, archive));

            }
            return lesClients;
        }
        public Client selectById(int id)
        {
            DataRow datarow = this.dbal.SelectById("client", id);
            Ville v = uneVille.selectById((int)datarow["ville_id"]);
            return new Client((int)(datarow["id"]), (string)datarow["nom"], (string)datarow["prenom"], v, (string)datarow["tel"], (string)datarow["mail"], (bool)datarow["archive"]);
        }
    }
}
