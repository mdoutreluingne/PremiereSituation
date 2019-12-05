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
            List<object> listPartie = new List<object>();
            string request = "SELECT " + elements + " FROM partie " + join_where + ";";
            DataTableCollection table = _dbal.select(request);
            return table;
        }
    }
}
