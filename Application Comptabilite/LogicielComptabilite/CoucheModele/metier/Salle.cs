using System;
using System.Collections.Generic;
using System.Text;

namespace CoucheModele.metier
{
    public class Salle
    {
        private int id;
        private Ville ville_id;
        private int numero;
        private decimal prix;
        private TimeSpan heure_ouverture;
        private TimeSpan heure_fermeture;
        private bool archive;
        private Theme theme_id;

        public Salle(int id, Ville ville_id, int numero, decimal prix, TimeSpan heure_ouverture, TimeSpan heure_fermeture, bool archive, Theme theme_id)
        {
            this.id = id;
            this.ville_id = ville_id;
            this.numero = numero;
            this.prix = prix;
            this.heure_ouverture = heure_ouverture;
            this.heure_fermeture = heure_fermeture;
            this.archive = archive;
            this.theme_id = theme_id;
        }

        public int Id { get => id; set => id = value; }
        public Ville Ville_id { get => ville_id; set => ville_id = value; }
        public int Numero { get => numero; set => numero = value; }
        public decimal Prix { get => prix; set => prix = value; }
        public TimeSpan Heure_ouverture { get => heure_ouverture; set => heure_ouverture = value; }
        public TimeSpan Heure_fermeture { get => heure_fermeture; set => heure_fermeture = value; }
        public bool Archive { get => archive; set => archive = value; }
        internal Theme Theme_id { get => theme_id; set => theme_id = value; }
    }
}
