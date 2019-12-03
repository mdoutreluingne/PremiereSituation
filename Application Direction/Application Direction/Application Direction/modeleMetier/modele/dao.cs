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
        /// <summary>
        /// Select d'un element
        /// </summary>
        /// <param name="elements"> ce qu'il y a avant le "From"</param>
        /// <param name="join_where">Join + where </param>
        /// <returns></returns>
        public abstract object select(string elements, string join_where);
        public abstract object update(string elementss, string join_where); 
        
    }
}
