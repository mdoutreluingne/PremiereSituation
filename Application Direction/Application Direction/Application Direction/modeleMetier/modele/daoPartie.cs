using ModeleMetier.metier;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ModeleMetier.modele
{
    public class daoPartie : dao
    {
        private daoReservation _daoReservation;
        public daoPartie(dbal dbal, daoReservation daoReservation)
            : base(dbal)
        {
            _daoReservation = daoReservation;
        }

        public override void insert(object o)
        {
            throw new NotImplementedException();
        }

        public override object select(string elements, string join_where)
        {
            List<dtoPartie> listPartie = new List<dtoPartie>();
            string request = "SELECT " + elements + " FROM partie " + join_where + ";";
            DataTableCollection table = _dbal.select(request);

            for (int i = 0; i < table[0].Rows.Count; i++)
            {
                int reservation_id = (int)table[0].Rows[i]["reservation_id"];
                DateTime temps = Convert.ToDateTime(table[0].Rows[i]["temps"].ToString());
                List<dtoReservation> les_reservations = (List<dtoReservation>)_daoReservation.select("*", "WHERE id = " +reservation_id);
                dtoReservation reservation = les_reservations[0];
                listPartie.Add(new dtoPartie(reservation, temps));
            }
            return listPartie;
        }
    }
}
