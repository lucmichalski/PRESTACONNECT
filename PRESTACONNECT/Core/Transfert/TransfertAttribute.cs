using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Core.Transfert
{
    public class TransfertAttribute
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
                        this.ExecLocalToDistant(Article, Product, ProductRepository);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void ExecLocalToDistant(Model.Local.Article Article, Model.Prestashop.PsProduct Product, Model.Prestashop.PsProductRepository ProductRepository)
        {
            try
            {
                if (Article.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleComposition)
                {
                    if (Core.UpdateVersion.License.Option2)
                    {
                        Product.Quantity = 0;
                        Product.Price = 0;
                        SynchronisationArticle.WriteStockAvailableProduct(Product);
                        SynchronisationArticle.ExecCompositionArticle(Article, Product, ProductRepository);
                    }
                }
                else
                {
                    Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
                    if (F_ARTICLERepository.ExistArticle(Article.Sag_Id))
                    {
                        Model.Sage.F_ARTICLE F_ARTICLE = F_ARTICLERepository.ReadArticle(Article.Sag_Id);

                        if ((F_ARTICLE.AR_Gamme1 != null && F_ARTICLE.AR_Gamme1 != 0) || (F_ARTICLE.AR_Condition != null && F_ARTICLE.AR_Condition != 0))
                        {
                            int CatComptaArticle = Core.Global.GetConfig().ConfigArticleCatComptable;
                            Model.Sage.F_TAXE TaxeTVA = SynchronisationArticle.ReadTaxe(F_ARTICLE, Product, CatComptaArticle);
                            Model.Sage.F_TAXE TaxeEco = SynchronisationArticle.ReadEcoTaxe(F_ARTICLE, Product, TaxeTVA, CatComptaArticle);
                            SynchronisationArticle.ReadPrice(F_ARTICLE, Product, TaxeTVA);


                            // <JG> 19/02/2013 déplacement pour correction problème prix spécifiques lors de la création de l'article
                            SynchronisationArticle.ExecAttribute(Article, Product, ProductRepository, TaxeTVA);
                            SynchronisationArticle.ExecConditioning(Article, Product, ProductRepository, TaxeTVA);

                            //List<string> log;
                            //SynchronisationArticle.ExecSpecificPrice(F_ARTICLE, Product, Article.Art_Id, TaxeTVA, TaxeEco, out log);
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
