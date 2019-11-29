using System;
using System.Collections.Generic;
using System.Text;

namespace CoucheModele.metier
{
    public class Utilisateur
    {
        private string login;
        private string mdp;
        private string role;

        public Utilisateur()
        {
            login = "";
            mdp = "";
            role = "";
        }
        public Utilisateur(string unlogin, string unmdp, string unrole)
        {
            login = unlogin;
            mdp = unmdp;
            role = unrole;
        }

        public string Login { get => login; set => login = value; }
        public string Mdp { get => mdp; set => mdp = value; }
        public string Role { get => role; set => role = value; }
    }
}
