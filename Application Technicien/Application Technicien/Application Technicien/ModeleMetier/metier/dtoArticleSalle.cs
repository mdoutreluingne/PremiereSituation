using System;
using System.Collections.Generic;
using System.Text;

namespace ModeleMetier.metier
{
    public class dtoArticleSalle
    {
        private dtoArticle _article;
        private dtoSalle _salle;

        #region Constructeur
        public dtoArticleSalle(dtoArticle article, dtoSalle salle)
        {
            _article = article;
            _salle = salle;
        }
        #endregion

        #region Acccesseurs
        public dtoArticle Article { get => _article; set => _article = value; }
        public dtoSalle Salle { get => _salle; set => _salle = value; }
        #endregion
    }
}
