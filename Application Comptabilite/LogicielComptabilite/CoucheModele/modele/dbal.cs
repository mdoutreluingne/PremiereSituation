using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace CoucheModele.modele
{
    public class dbal
    {
        MySqlConnection connection;

        public dbal(string user, string pass, int port, string server, string bdd)
        {
            connection = new MySqlConnection("SERVER=" + server + "; PORT=" + port + "; DATABASE=" + bdd + "; UID=" + user + "; PASSWORD=" + pass + " "); //Initialise la connexion
        }
        public bool OpenConnexions()
        {

            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {

                Console.Write(ex.Message);
                return false;
            }

        }
        public bool CloseConnexions()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {

                Console.Write(ex.Message);
                return false;
            }
        }

        public void execRequete(string query)
        {
            if (this.OpenConnexions() == true)
            {
                MySqlCommand req = new MySqlCommand(query, connection);

                req.ExecuteNonQuery(); //execute la requete sql

                this.CloseConnexions(); //ferme la connection

            }
        }

        public DataTable selectMontant(string sql)
        {
            DataSet dataSet = new DataSet();

            this.connection.Open();

            MySqlDataAdapter adaptater = new MySqlDataAdapter(sql, connection);

            adaptater.Fill(dataSet);

            this.connection.Close();

            DataTable table = dataSet.Tables[0];

            return table;
        }
        public DataTable selectAll(string sql)
        {
            DataSet dataSet = new DataSet();

            this.connection.Open();

            MySqlDataAdapter adaptater = new MySqlDataAdapter(sql, connection);

            adaptater.Fill(dataSet);

            this.connection.Close();

            DataTable table = dataSet.Tables[0];

            return table;
        }

        public DataSet Requete(string query)
        {
            DataSet dataset = new DataSet();
            //Open connection
            if (this.OpenConnexions() == true)
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                adapter.Fill(dataset);
                CloseConnexions();
            }
            return dataset;
        }
        public DataRow SelectById(string table, object id)
        {
            string query = "SELECT * FROM " + table + " where id='" + id + "'";
            DataSet dataset = Requete(query);

            return dataset.Tables[0].Rows[0];
        }
        public DataTable SelectByNom(string table, string champs)
        {
            string query = "SELECT * FROM " + table + " where nom='" + champs + "'";
            DataSet dataset = Requete(query);

            return dataset.Tables[0];
        }
        public DataRow SelectByIdReservationClient(string table, int id)
        {
            string query = "SELECT * FROM " + table + " where client_id='" + id + "'";
            DataSet dataset = Requete(query);

            return dataset.Tables[0].Rows[0];
        }

    }
}
