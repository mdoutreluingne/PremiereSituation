using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using CoucheModele.metier;

namespace CoucheModele.modele
{
    public class daoTheme
    {
        private dbal dbal;

        public daoTheme(dbal dbal)
        {
            this.dbal = dbal;
        }
        public Theme selectById(int id)
        {
            DataRow datarow = this.dbal.SelectById("theme", id);
            return new Theme((int)(datarow["id"]), (string)datarow["nom"]);
        }
    }
}
