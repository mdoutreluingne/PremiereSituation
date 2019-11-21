using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ModeleMetier.metier;

namespace ModeleMetier.modele
{
    public class daoObstacle : dao
    {
        private daoReservation _daoReservation;
        private daoArticle _daoArticle;

        public daoObstacle(dbal dbal, daoReservation daoReservation, daoArticle daoArticle)
            : base(dbal)
        {
            _daoReservation = daoReservation;
            _daoArticle = daoArticle;
        }
        public override void insert(object o)
        {
            throw new NotImplementedException();
        }

        public override object select(string elements, string join_where)
        {
            List<dtoObstacle> listObstacle = new List<dtoObstacle>();
            string request = "SELECT " + elements + " FROM obstacle " + join_where + ";";
            DataTableCollection table = _dbal.select(request);
            for (int i = 0; i < table[0].Rows.Count; i++)
            {
                int id = (int)table[0].Rows[i]["id"];
                int position = (int)table[0].Rows[i]["position"];
                string commentaire = (string)table[0].Rows[i]["commentaire"];
                int qte = (int)table[0].Rows[i]["qte"];
                int article_id = (int)table[0].Rows[i]["article_id"];
                int reservation_id = (int)table[0].Rows[i]["reservation_id"];

                List<dtoArticle> les_articles = (List<dtoArticle>)_daoArticle.select("*", "WHERE id = " + article_id);
                List<dtoReservation> les_reservations = (List<dtoReservation>)_daoReservation.select("*", "WHERE id = " + reservation_id);

                dtoArticle article = les_articles[0];
                dtoReservation reservation = les_reservations[0];

                listObstacle.Add(new dtoObstacle(id, position, commentaire, qte, article, reservation));
            }
            return listObstacle;
        }
    }
}
