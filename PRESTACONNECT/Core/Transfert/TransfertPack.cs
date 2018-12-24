using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Core.Transfert
{
    public class TransfertPack
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
                        this.ExecLocalToDistant(Article, ArticleRepository, Product, ProductRepository);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void ExecLocalToDistant(Model.Local.Article Article, Model.Local.ArticleRepository ArticleRepository, Model.Prestashop.PsProduct Product, Model.Prestashop.PsProductRepository ProductRepository)
        {
            try
            {
                Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
                if (F_ARTICLERepository.ExistArticle(Article.Sag_Id))
                {
                    Model.Sage.F_ARTICLE F_ARTICLE = F_ARTICLERepository.ReadArticle(Article.Sag_Id);
                    Model.Prestashop.PsPackRepository PsPackRepository = new Model.Prestashop.PsPackRepository();
                    List<Model.Prestashop.PsPack> ListPsPack = PsPackRepository.ListProductPack(Product.IDProduct);
                    foreach (Model.Prestashop.PsPack PsPack in ListPsPack)
                    {
                        PsPackRepository.Delete(PsPack);
                    }
                    #region Pack/nomenclature
                    if (Article.Art_Pack == true)
                    {
                        Model.Sage.F_NOMENCLATRepository F_NOMENCLATRepository = new Model.Sage.F_NOMENCLATRepository();
                        List<Model.Sage.F_NOMENCLAT> ListF_NOMENCLAT = F_NOMENCLATRepository.ListRef(F_ARTICLE.AR_Ref);
                        Model.Sage.F_ARTICLE F_ARTICLENOMENCLAT;
                        Model.Local.Article ArticleNomenclat;
                        Model.Prestashop.PsPack PsPackAdd;
                        foreach (Model.Sage.F_NOMENCLAT F_NOMENCLAT in ListF_NOMENCLAT)
                        {
                            if (F_ARTICLERepository.ExistReference(F_NOMENCLAT.NO_RefDet))
                            {
                                F_ARTICLENOMENCLAT = F_ARTICLERepository.ReadReference(F_NOMENCLAT.NO_RefDet);
                                if (ArticleRepository.ExistSag_Id(F_ARTICLENOMENCLAT.cbMarq)
										&& F_ARTICLENOMENCLAT.AR_SuiviStock != (short)ABSTRACTION_SAGE.F_ARTICLE.Obj._Enum_AR_SuiviStock.Aucun)
										// pour ne pas prendre en compte les articles non suivi en stock
								{
									ArticleNomenclat = ArticleRepository.ReadSag_Id(F_ARTICLENOMENCLAT.cbMarq);
                                    if (ArticleNomenclat.Pre_Id != null && ArticleNomenclat.Pre_Id.Value != 0)
                                    {
                                        if (ProductRepository.ExistId((UInt32)ArticleNomenclat.Pre_Id.Value))
                                        {
                                            PsPackAdd = new Model.Prestashop.PsPack()
                                            {
                                                IDProductPack = Product.IDProduct,
                                                IDProductItem = (UInt32)ArticleNomenclat.Pre_Id.Value,
                                                Quantity = (UInt32)F_NOMENCLAT.NO_Qte.Value
                                            };
                                            PsPackRepository.Add(PsPackAdd);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

    }
}
