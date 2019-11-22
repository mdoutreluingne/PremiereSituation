using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using CoucheModele.metier;

namespace CoucheModele.modele
{
    public class daoReservation
    {
        private dbal dbal;
        private daoClient unDaoClient;
        private daoSalle unDaoSalle;

        public daoReservation(dbal dbal, daoClient dc, daoSalle ds)
        {
            this.dbal = dbal;
            unDaoClient = dc;
            unDaoSalle = ds;
        }
        public Reservation selectById(object id)
        {
            DataRow datarow = this.dbal.SelectById("reservation", id);
            Client unClient = unDaoClient.selectById((int)datarow["client_id"]);
            Salle uneSalle = unDaoSalle.selectById((int)datarow["salle_id"]);
            return new Reservation((int)(datarow["id"]), (DateTime)datarow["date"], (string)datarow["commentaire"], (int)datarow["nbJoueur"], unClient, uneSalle);
        }

    }
}
