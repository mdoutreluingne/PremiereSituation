using System;
using System.Collections.Generic;
using System.Text;

namespace CoucheModele.metier
{
    public class Theme
    {
        private int id;
        private string nom;

        public Theme()
        {
            Id = 0;
            Nom = "";
        }
        public Theme(int unId, string unNom)
        {
            Id = unId;
            Nom = unNom;
        }

        public int Id { get => id; set => id = value; }
        public string Nom { get => nom; set => nom = value; }
    }
}
