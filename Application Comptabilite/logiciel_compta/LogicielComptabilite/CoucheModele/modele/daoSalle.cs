using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using CoucheModele.metier;

namespace CoucheModele.modele
{
    public class daoSalle
    {
        private dbal dbal;
        private daoVille unDaoVille;
        private daoTheme unDaoTheme;

        public daoSalle(dbal dbal, daoVille dv, daoTheme dt)
        {
            this.dbal = dbal;
            unDaoVille = dv;
            unDaoTheme = dt;
        }
        public Salle selectById(int id)
        {
            DataRow datarow = this.dbal.SelectById("salle", id);
            Ville uneville = unDaoVille.selectById((int)datarow["ville_id"]);
            Theme untheme = unDaoTheme.selectById((int)datarow["theme_id"]);
            return new Salle((int)(datarow["id"]), uneville, (int)(datarow["numero"]), (decimal)(datarow["prix"]), (TimeSpan)datarow["heure_ouverture"], (TimeSpan)datarow["heure_fermeture"], (bool)(datarow["archive"]), untheme);
        }
    }
}
