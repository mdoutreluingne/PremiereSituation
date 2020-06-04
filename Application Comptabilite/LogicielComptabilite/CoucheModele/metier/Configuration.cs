using System;
using System.Collections.Generic;
using System.Text;

namespace CoucheModele.metier
{
    public class Configuration
    {
        private int id;
        private int nb_commentaire;
        private int nb_avis;
        private double note_min;
        private double seuil_vert_total_credit;
        private double seuil_orange_total_credit;
        private double seuil_rouge_total_credit;
        private double seuil_vert_total_achat;
        private double seuil_orange_total_achat;
        private double seuil_rouge_total_achat;
        private double seuil_vert_total_depense;
        private double seuil_orange_total_depense;
        private double seuil_rouge_total_depense;

        public Configuration(int id, int nb_commentaire, int nb_avis, double note_min, double seuil_vert_total_credit, double seuil_orange_total_credit, double seuil_rouge_total_credit, double seuil_vert_total_achat, double seuil_orange_total_achat, double seuil_rouge_total_achat, double seuil_vert_total_depense, double seuil_orange_total_depense, double seuil_rouge_total_depense)
        {
            this.Id = id;
            this.Nb_commentaire = nb_commentaire;
            this.Nb_avis = nb_avis;
            this.Note_min = note_min;
            this.Seuil_vert_total_credit = seuil_vert_total_credit;
            this.Seuil_orange_total_credit = seuil_orange_total_credit;
            this.Seuil_rouge_total_credit = seuil_rouge_total_credit;
            this.Seuil_vert_total_achat = seuil_vert_total_achat;
            this.Seuil_orange_total_achat = seuil_orange_total_achat;
            this.Seuil_rouge_total_achat = seuil_rouge_total_achat;
            this.Seuil_vert_total_depense = seuil_vert_total_depense;
            this.Seuil_orange_total_depense = seuil_orange_total_depense;
            this.Seuil_rouge_total_depense = seuil_rouge_total_depense;
        }

        public int Id { get => id; set => id = value; }
        public int Nb_commentaire { get => nb_commentaire; set => nb_commentaire = value; }
        public int Nb_avis { get => nb_avis; set => nb_avis = value; }
        public double Note_min { get => note_min; set => note_min = value; }
        public double Seuil_vert_total_credit { get => seuil_vert_total_credit; set => seuil_vert_total_credit = value; }
        public double Seuil_orange_total_credit { get => seuil_orange_total_credit; set => seuil_orange_total_credit = value; }
        public double Seuil_rouge_total_credit { get => seuil_rouge_total_credit; set => seuil_rouge_total_credit = value; }
        public double Seuil_vert_total_achat { get => seuil_vert_total_achat; set => seuil_vert_total_achat = value; }
        public double Seuil_orange_total_achat { get => seuil_orange_total_achat; set => seuil_orange_total_achat = value; }
        public double Seuil_rouge_total_achat { get => seuil_rouge_total_achat; set => seuil_rouge_total_achat = value; }
        public double Seuil_vert_total_depense { get => seuil_vert_total_depense; set => seuil_vert_total_depense = value; }
        public double Seuil_orange_total_depense { get => seuil_orange_total_depense; set => seuil_orange_total_depense = value; }
        public double Seuil_rouge_total_depense { get => seuil_rouge_total_depense; set => seuil_rouge_total_depense = value; }
    }
}
