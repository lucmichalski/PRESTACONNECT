using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Core.Transfert
{
    public class TransfertFeature
    {
        private Core.Sync.SynchronisationArticle SynchronisationArticle = new Core.Sync.SynchronisationArticle();

        public void Exec(Int32 ArticleSend)
        {
            try
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                Model.Local.Article Article = ArticleRepository.ReadArticle(ArticleSend);
                Model.Prestashop.PsProductRepository ProductRepository = new Model.Prestashop.PsProductRepository();
                Model.Prestashop.PsProduct Product = new Model.Prestashop.PsProduct();
                // If the Article have a connection with Prestashop
                if (Article.Pre_Id != null)
                {
                    //Article.Art_Date = Article.Art_Date.AddMilliseconds(-Article.Art_Date.Millisecond);
                    if (ProductRepository.ExistId(Convert.ToUInt32(Article.Pre_Id.Value)))
                    {
                        Product = ProductRepository.ReadId(Convert.ToUInt32(Article.Pre_Id.Value));

                        Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
                        if (F_ARTICLERepository.ExistArticle(Article.Sag_Id))
                        {
                            SynchronisationArticle.ExecFeature(Article);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
    }
}
