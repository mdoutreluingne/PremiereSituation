using System;
using System.Collections.Generic;
using System.Text;

namespace ModeleMetier.modele
{
    public abstract class dao
    {
        protected dbal _dbal;

        protected dao(dbal dbal)
        {
            _dbal = dbal;
        }
        public abstract void insert(object o);
        public abstract object select(string elements, string join_where);
    }
}
