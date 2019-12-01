using ModeleMetier.metier;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ModeleMetier.modele
{
    public class daoArticleSalle:dao
    {
        private daoArticle _daoArticle;
        private daoSalle _daoSalle;
        public daoArticleSalle(dbal dbal, daoArticle daoArticle, daoSalle daoSalle)
            : base(dbal)
        {
            _daoArticle = daoArticle;
            _daoSalle = daoSalle;
        }

        public override void insert(object o)
        {
            throw new NotImplementedException();
        }
        public void insert(int article_id, int article_salle)
        {
            string request = "INSERT INTO article_salle VALUES("
                + article_id + "," + article_salle + ");";
            _dbal.command(request);
        }

        public void delete(int article_id)
        {
            string request = "DELETE FROM article_salle WHERE article_id = " + article_id + ";";
            _dbal.command(request);
        }
        public override object select(string elements, string join_where)
        {
            List<dtoArticleSalle> listArticleSalle = new List<dtoArticleSalle>();
            string request = "SELECT " + elements + " FROM article_salle " + join_where + ";";
            DataTableCollection table = _dbal.select(request);

            for (int i = 0; i < table[0].Rows.Count; i++)
            {
                int article_id = (int)table[0].Rows[i]["article_id"];
                int salle_id = (int)table[0].Rows[i]["salle_id"];
                List<dtoArticle> lesArticles = (List<dtoArticle>)_daoArticle.select("*", "WHERE id = " + article_id);
                List<dtoSalle> lesSalles = (List<dtoSalle>)_daoSalle.select("*", "WHERE id = " + salle_id);
                dtoArticle article = lesArticles[0];
                dtoSalle salle = lesSalles[0];
                listArticleSalle.Add(new dtoArticleSalle(article, salle));
            }
            return listArticleSalle;
        }
    }
}
