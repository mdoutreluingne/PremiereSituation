using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace ModeleMetier.modele
{
    public class dbal
    {
        #region Attributs
        private MySqlConnection connection;
        private DataSet dataSet;
        #endregion

        #region Constructeur
        public dbal(string server, string uid, string database, string password)
        {
            this.connection = new MySqlConnection("SERVER=" + server + ";DATABASE=" + database + ";UID=" + uid + ";PASSWORD=" + password);
        }
        #endregion

        #region Méthodes
        public bool openConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException e)
            {
                Console.WriteLine("[ERREUR]: " + e.Message);
                return false;
            }
        }

        public bool closeConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException e)
            {
                Console.WriteLine("[ERREUR]: " + e.Message);
                return false;
            }
        }

        public void command(string request)
        {
            this.connection.Open();
            MySqlCommand query = new MySqlCommand(request, connection);
            query.ExecuteNonQuery();
            this.connection.Close();
        }

        public DataTableCollection select(string request)
        {
            DataSet dataSet = new DataSet();
            this.connection.Open();
            MySqlDataAdapter adaptater = new MySqlDataAdapter(request, connection);
            adaptater.Fill(dataSet);
            this.connection.Close();
            DataTableCollection table = dataSet.Tables;
            return table;
        }
        #endregion
    }
}
