using System;
using System.Collections.Generic;
using System.Text;

namespace CoucheModele.metier
{
    public class Client
    {
        private int id;
        private string nom;
        private string prenom;
        private Ville ville_id;
        private string tel;
        private string mail;
        private bool archive;

        public Client()
        {
            Id = 0;
            Nom = "";
            Prenom = "";
            Ville_id = null;
            Tel = "";
            Mail = "";
            Archive = false;
        }
        public Client(int unId, string unNom, string unPrenom, Ville uneVille, string unTel, string unMail, bool uneArchive)
        {
            Id = unId;
            Nom = unNom;
            Prenom = unPrenom;
            Ville_id = uneVille;
            Tel = unTel;
            Mail = unMail;
            Archive = uneArchive;
        }

        public int Id { get => id; set => id = value; }
        public string Nom { get => nom; set => nom = value; }
        public string Prenom { get => prenom; set => prenom = value; }
        public string Tel { get => tel; set => tel = value; }
        public string Mail { get => mail; set => mail = value; }
        public bool Archive { get => archive; set => archive = value; }
        public Ville Ville_id { get => ville_id; set => ville_id = value; }

        public override string ToString()
        {
            return Nom + " " + Prenom + " - " + Mail;
        }
    }
}
