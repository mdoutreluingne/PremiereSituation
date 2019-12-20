using System;
using System.Collections.Generic;
using System.Text;

namespace ModeleMetier.metier
{
    public class dtoUtilisateur
    {
        #region Attributs
        private string _login;
        private string _mdp;
        private string _role;
        private dtoClient _client;
        #endregion

        #region Constructeur
        public dtoUtilisateur(string login, string mdp, string role, dtoClient client)
        {
            _login = login;
            _mdp = mdp;
            _role = role;
            _client = client;
        }
        #endregion

        #region Accesseurs
        public string Login { get => _login; set => _login = value; }
        public string Mdp { get => _mdp; set => _mdp = value; }
        public string Role { get => _role; set => _role = value; }
        public dtoClient Client { get => _client; set => _client = value; }
        #endregion
    }
}
