using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using CoucheModele.metier;

namespace CoucheModele.modele
{
    public class daoConfiguration
    {
        private dbal dbal;

        public daoConfiguration(dbal dbal)
        {
            this.dbal = dbal;
        }

        public Configuration seuil_total_credit()
        {
            object seuil = 0;
            Configuration uneConfig;
            DataTable table = this.dbal.selectMontant("SELECT * FROM configuration;");
            uneConfig = new Configuration(Convert.ToInt32(table.Rows[0]["id"]), Convert.ToInt32(table.Rows[0]["nb_commentaire"]), Convert.ToInt32(table.Rows[0]["nb_avis"]), Convert.ToInt32(table.Rows[0]["note_min"]), Convert.ToDouble(table.Rows[0]["seuil_vert_total_credit"]), Convert.ToDouble(table.Rows[0]["seuil_orange_total_credit"]), Convert.ToDouble(table.Rows[0]["seuil_rouge_total_credit"]), Convert.ToDouble(table.Rows[0]["seuil_vert_total_achat"]), Convert.ToDouble(table.Rows[0]["seuil_orange_total_achat"]), Convert.ToDouble(table.Rows[0]["seuil_rouge_total_achat"]), Convert.ToDouble(table.Rows[0]["seuil_vert_total_depense"]), Convert.ToDouble(table.Rows[0]["seuil_orange_total_depense"]), Convert.ToDouble(table.Rows[0]["seuil_rouge_total_depense"]));
            return uneConfig;
        }

        public Configuration seuil_total_achat()
        {
            object seuil = 0;
            Configuration uneConfig;
            DataTable table = this.dbal.selectMontant("SELECT * FROM configuration;");
            uneConfig = new Configuration(Convert.ToInt32(table.Rows[0]["id"]), Convert.ToInt32(table.Rows[0]["nb_commentaire"]), Convert.ToInt32(table.Rows[0]["nb_avis"]), Convert.ToInt32(table.Rows[0]["note_min"]), Convert.ToDouble(table.Rows[0]["seuil_vert_total_credit"]), Convert.ToDouble(table.Rows[0]["seuil_orange_total_credit"]), Convert.ToDouble(table.Rows[0]["seuil_rouge_total_credit"]), Convert.ToDouble(table.Rows[0]["seuil_vert_total_achat"]), Convert.ToDouble(table.Rows[0]["seuil_orange_total_achat"]), Convert.ToDouble(table.Rows[0]["seuil_rouge_total_achat"]), Convert.ToDouble(table.Rows[0]["seuil_vert_total_depense"]), Convert.ToDouble(table.Rows[0]["seuil_orange_total_depense"]), Convert.ToDouble(table.Rows[0]["seuil_rouge_total_depense"]));
            return uneConfig;
        }

        public Configuration seuil_total_depense()
        {
            object seuil = 0;
            Configuration uneConfig;
            DataTable table = this.dbal.selectMontant("SELECT * FROM configuration;");
            uneConfig = new Configuration(Convert.ToInt32(table.Rows[0]["id"]), Convert.ToInt32(table.Rows[0]["nb_commentaire"]), Convert.ToInt32(table.Rows[0]["nb_avis"]), Convert.ToInt32(table.Rows[0]["note_min"]), Convert.ToDouble(table.Rows[0]["seuil_vert_total_credit"]), Convert.ToDouble(table.Rows[0]["seuil_orange_total_credit"]), Convert.ToDouble(table.Rows[0]["seuil_rouge_total_credit"]), Convert.ToDouble(table.Rows[0]["seuil_vert_total_achat"]), Convert.ToDouble(table.Rows[0]["seuil_orange_total_achat"]), Convert.ToDouble(table.Rows[0]["seuil_rouge_total_achat"]), Convert.ToDouble(table.Rows[0]["seuil_vert_total_depense"]), Convert.ToDouble(table.Rows[0]["seuil_orange_total_depense"]), Convert.ToDouble(table.Rows[0]["seuil_rouge_total_depense"]));
            return uneConfig;
        }
    }
}
