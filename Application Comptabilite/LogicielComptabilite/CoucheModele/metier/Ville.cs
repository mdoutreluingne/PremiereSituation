using System;
using System.Collections.Generic;
using System.Text;

namespace CoucheModele.metier
{
    public class Ville
    {
        private int id;
        private string nom;

        public Ville()
        {
            Id = 0;
            Nom = "";
        }
        public Ville(int unId, string uneVille)
        {
            Id = unId;
            Nom = uneVille;
        }

        public int Id { get => id; set => id = value; }
        public string Nom { get => nom; set => nom = value; }

        public override string ToString()
        {
            return Nom;
        }

    }
}
