using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Core.Transfert
{
    public class TransfertStock
    {
        private Core.Sync.SynchronisationArticle SynchronisationArticle = new Core.Sync.SynchronisationArticle();

        public void Exec(Int32 ArticleSend)
        {
            try
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                Model.Local.Article Article = ArticleRepository.ReadArticle(ArticleSend);
                Model.Prestashop.PsProductRepository PsProductRepository = new Model.Prestashop.PsProductRepository();
                Model.Prestashop.PsProduct Product = new Model.Prestashop.PsProduct();
                // If the Article have a connection with Prestashop
                if (Article.Pre_Id != null)
                {
                    //Article.Art_Date = Article.Art_Date.AddMilliseconds(-Article.Art_Date.Millisecond);
                    if (PsProductRepository.ExistId(Convert.ToUInt32(Article.Pre_Id.Value)))
                    {
                        Product = PsProductRepository.ReadId(Convert.ToUInt32(Article.Pre_Id.Value));
                        this.ExecLocalToDistant(Article, Product, PsProductRepository);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void ExecLocalToDistant(Model.Local.Article Article, Model.Prestashop.PsProduct Product, Model.Prestashop.PsProductRepository PsProductRepository)
        {
            try
            {
                if (Article.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleComposition)
                {
                    if (Core.UpdateVersion.License.Option2)
                    {
                        Product.Quantity = 0;
                        SynchronisationArticle.WriteStockAvailableProduct(Product);
                        ExecCompositionArticle(Article, Product);
                    }
                }
                else
                {
                    Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
                    if (F_ARTICLERepository.ExistArticle(Article.Sag_Id))
                    {
                        Model.Sage.F_ARTICLE F_ARTICLE = F_ARTICLERepository.ReadArticle(Article.Sag_Id);
                        SynchronisationArticle.ReadQuantity(F_ARTICLE, Product);

                        #region Stock pack limité par composés
                        if (Article.Art_Pack == true || F_ARTICLE.AR_Nomencl == (short)ABSTRACTION_SAGE.F_ARTICLE.Obj._Enum_AR_Nomencl.Commerciale_Composant
								|| F_ARTICLE.AR_Nomencl == (short)ABSTRACTION_SAGE.F_ARTICLE.Obj._Enum_AR_Nomencl.Commerciale_Compose) // pour que les articles en nomenclature soient pris en compte
						{
                            Model.Sage.F_NOMENCLATRepository F_NOMENCLATRepository = new Model.Sage.F_NOMENCLATRepository();
                            List<Model.Sage.F_NOMENCLAT> ListF_NOMENCLAT = F_NOMENCLATRepository.ListRef(F_ARTICLE.AR_Ref);
                            Model.Sage.F_ARTICLE F_ARTICLENOMENCLAT;
                            Int32 QuantityPack = 0;
                            foreach (Model.Sage.F_NOMENCLAT F_NOMENCLAT in ListF_NOMENCLAT)
                            {
                                if (F_ARTICLERepository.ExistReference(F_NOMENCLAT.NO_RefDet))
                                {
                                    F_ARTICLENOMENCLAT = F_ARTICLERepository.ReadReference(F_NOMENCLAT.NO_RefDet);
									if (F_ARTICLENOMENCLAT.AR_SuiviStock != (short)ABSTRACTION_SAGE.F_ARTICLE.Obj._Enum_AR_SuiviStock.Aucun)	// pour ne pas prendre en compte les articles non suivi en stock
									{
										Int32 CurrentQuantity = SynchronisationArticle.ReadQuantityPack(F_ARTICLENOMENCLAT, F_NOMENCLAT.NO_Qte.Value);
										if (Core.Global.GetConfig().ArticleStockNegatifZero && CurrentQuantity < 0)
											CurrentQuantity = 0;
										if (CurrentQuantity < QuantityPack)
										{
											QuantityPack = CurrentQuantity;
											// <AM> 14/08/2014 Si un des articles composant n'a pas de stock on sort de la fonction pour que l'article parent ai un stock à 0
											if (CurrentQuantity == 0)
											{
												break;
											}
										}
										else if (QuantityPack == 0)
										{
											QuantityPack = CurrentQuantity;
											// <AM> 14/08/2014 Si un des articles composant n'a pas de stock on sort de la fonction pour que l'article parent ai un stock à 0
											if (CurrentQuantity == 0)
											{
												break;
											}
										}
									}
                                }
                            }
                            Product.Quantity = QuantityPack;
                        }
                        #endregion

                        // <JG> 29/10/2012 ajout synchronisation des stocks seule
                        //SynchronisationArticle.ReadEcoTaxe(F_ARTICLE, Product);
                        //SynchronisationArticle.ReadPrice(F_ARTICLE, Product);

                        if (Core.Global.GetConfig().MajPoidsSynchroStock)
                            SynchronisationArticle.ReadWeight(F_ARTICLE, Product);

                        if (Core.Global.GetConfig().ArticleDateDispoInfoLibreActif)
                            SynchronisationArticle.ReadDateDispo(Product, F_ARTICLE);

                        PsProductRepository.Save();
                        PsProductRepository.WriteDate(Product.IDProduct);

                        this.ExecAttribute(Article, Product, PsProductRepository);
                        this.ExecConditioning(Article, Product, PsProductRepository);

                        //SynchronisationArticle.ExecSpecificPrice(F_ARTICLE, Product);

                        // <JG> 17/12/2012
                        SynchronisationArticle.ExecShopProduct(Product);

                        SynchronisationArticle.WriteStockAvailableProduct(Product);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void ExecAttribute(Model.Local.Article Article, Model.Prestashop.PsProduct PsProduct, Model.Prestashop.PsProductRepository PsProductRepository)
        {
            try
            {
                Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
                if (F_ARTICLERepository.ExistArticle(Article.Sag_Id))
                {
                    Model.Sage.F_ARTICLE F_ARTICLE = F_ARTICLERepository.ReadArticle(Article.Sag_Id);

                    Model.Local.AttributeGroupRepository AttributeGroupRepository = new Model.Local.AttributeGroupRepository();
                    Model.Prestashop.PsAttributeGroupRepository PsAttributeGroupRepository = new Model.Prestashop.PsAttributeGroupRepository();

                    Boolean isProductAttribute;

                    if (F_ARTICLE.AR_Gamme1 != null && F_ARTICLE.AR_Gamme1 != 0
                        && AttributeGroupRepository.ExistSage((int)F_ARTICLE.AR_Gamme1)
                        && PsAttributeGroupRepository.ExistAttributeGroup((uint)AttributeGroupRepository.ReadSage((int)F_ARTICLE.AR_Gamme1).Pre_Id)
                        && ((F_ARTICLE.AR_Gamme2 == null || F_ARTICLE.AR_Gamme2 == 0)
                            || (AttributeGroupRepository.ExistSage((int)F_ARTICLE.AR_Gamme2)
                                && PsAttributeGroupRepository.ExistAttributeGroup((uint)AttributeGroupRepository.ReadSage((int)F_ARTICLE.AR_Gamme2).Pre_Id))))
                    {

                        Model.Local.AttributeArticleRepository AttributeArticleRepository = new Model.Local.AttributeArticleRepository();
                        List<Model.Local.AttributeArticle> ListAttributeArticle = AttributeArticleRepository.ListArticleSync(Article.Art_Id, true);

                        Model.Prestashop.PsProductAttributeRepository PsProductAttributeRepository = new Model.Prestashop.PsProductAttributeRepository();
                        Model.Prestashop.PsProductAttribute PsProductAttribute;

                        int CumulStockGammes = 0;
                        foreach (Model.Local.AttributeArticle AttributeArticle in ListAttributeArticle)
                        {
                            if (AttributeArticle.EnumereF_ARTENUMREF != null)
                            {
                                PsProductAttribute = new Model.Prestashop.PsProductAttribute();
                                isProductAttribute = false;

                                if (AttributeArticle.Pre_Id != null && AttributeArticle.Pre_Id != 0)
                                {
                                    if (PsProductAttributeRepository.ExistProductAttribute((UInt32)AttributeArticle.Pre_Id))
                                    {
                                        PsProductAttribute = PsProductAttributeRepository.ReadProductAttribute((UInt32)AttributeArticle.Pre_Id);
                                        isProductAttribute = true;
                                    }
                                }

                                if (isProductAttribute == true)
                                {
                                    if (Core.Global.GetConfig().MajPoidsSynchroStock)
                                        SynchronisationArticle.ReadWeightAttribute(PsProduct, PsProductAttribute, F_ARTICLE, AttributeArticle.EnumereGamme1);

                                    SynchronisationArticle.ReadQuantityAttribute(PsProductAttribute, F_ARTICLE, AttributeArticle.EnumereGamme1, AttributeArticle.EnumereGamme2);
                                    CumulStockGammes += PsProductAttribute.Quantity;

                                    PsProductAttributeRepository.Save();
                                    SynchronisationArticle.ExecShopProductAttribute(PsProductAttribute);

                                    SynchronisationArticle.WriteStockAvailableProductAttribute(PsProduct, PsProductAttribute);
                                }
                            }
                        }
                        if (PsProduct.Quantity != CumulStockGammes)
                        {
                            PsProduct.Quantity = CumulStockGammes;
                            PsProductRepository.Save();
                        }
                        PsProductAttributeRepository.WriteDate(PsProduct.IDProduct);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void ExecConditioning(Model.Local.Article Article, Model.Prestashop.PsProduct PsProduct, Model.Prestashop.PsProductRepository PsProductRepository)
        {
            try
            {
                Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
                if (F_ARTICLERepository.ExistArticle(Article.Sag_Id))
                {
                    Model.Sage.F_ARTICLE F_ARTICLE = F_ARTICLERepository.ReadArticle(Article.Sag_Id);

                    Model.Local.ConditioningGroupRepository ConditioningGroupRepository = new Model.Local.ConditioningGroupRepository();
                    Model.Prestashop.PsAttributeGroupRepository PsAttributeGroupRepository = new Model.Prestashop.PsAttributeGroupRepository();

                    Boolean isProductAttribute;

                    if (F_ARTICLE.AR_Condition != null && F_ARTICLE.AR_Condition != 0
                         && ConditioningGroupRepository.ExistSage((int)F_ARTICLE.AR_Condition)
                         && PsAttributeGroupRepository.ExistAttributeGroup((uint)ConditioningGroupRepository.ReadSage((int)F_ARTICLE.AR_Condition).Pre_Id))
                    {
                        Model.Local.ConditioningArticleRepository ConditioningArticleRepository = new Model.Local.ConditioningArticleRepository();
                        List<Model.Local.ConditioningArticle> ListConditioningArticle = ConditioningArticleRepository.ListArticleSync(Article.Art_Id, true);

                        Model.Prestashop.PsProductAttributeRepository PsProductAttributeRepository = new Model.Prestashop.PsProductAttributeRepository();
                        Model.Prestashop.PsProductAttribute PsProductAttribute;
                        int stockmaxunity = 0;
                        foreach (Model.Local.ConditioningArticle ConditioningArticle in ListConditioningArticle)
                        {
                            if (ConditioningArticle.EnumereF_CONDITION != null)
                            {
                                PsProductAttribute = new Model.Prestashop.PsProductAttribute();
                                isProductAttribute = false;
                                if (ConditioningArticle.Pre_Id != null && ConditioningArticle.Pre_Id != 0)
                                {
                                    if (PsProductAttributeRepository.ExistProductAttribute((UInt32)ConditioningArticle.Pre_Id))
                                    {
                                        PsProductAttribute = PsProductAttributeRepository.ReadProductAttribute((UInt32)ConditioningArticle.Pre_Id);
                                        isProductAttribute = true;
                                    }
                                }

                                if (Core.Global.GetConfig().MajPoidsSynchroStock)
                                    SynchronisationArticle.ReadWeightConditioning(PsProduct, PsProductAttribute, ConditioningArticle.EnumereF_CONDITION);

                                SynchronisationArticle.ReadQuantityConditioning(PsProductAttribute, F_ARTICLE, ConditioningArticle.EnumereF_CONDITION);

                                if (Core.Global.GetConfig().LimiteStockConditionnement && PsProductAttribute.Quantity >= stockmaxunity)
                                {
                                    stockmaxunity = PsProductAttribute.Quantity;
                                    PsProduct.Quantity = stockmaxunity;
                                    PsProductRepository.Save();
                                }

                                //this.ReadPriceConditioning(PsProduct, PsProductAttribute, F_ARTICLE, ConditioningArticle, TAXE_1);

                                //PsProductAttribute.DefaultOn = (ConditioningArticle.ConArt_Default) ? (byte)1 : (byte)0;
                                if (isProductAttribute == true)
                                {
                                    PsProductAttributeRepository.Save();
                                    SynchronisationArticle.ExecShopProductAttribute(PsProductAttribute);

                                    SynchronisationArticle.WriteStockAvailableProductAttribute(PsProduct, PsProductAttribute);
                                }
                            }
                        }
                        PsProductAttributeRepository.WriteDate(PsProduct.IDProduct);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void ExecCompositionArticle(Model.Local.Article Article, Model.Prestashop.PsProduct Product)
        {
            Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
            Model.Local.CompositionArticleRepository CompositionArticleRepository = new Model.Local.CompositionArticleRepository();
            List<Model.Local.CompositionArticle> ListCompositionArticle = CompositionArticleRepository.ListArticle(Article.Art_Id);
            foreach (var item in ListCompositionArticle)
            {
                try
                {
                    Model.Sage.F_ARTICLE_Composition F_ARTICLE_Composition = F_ARTICLERepository.ReadComposition(item.ComArt_F_ARTICLE_SagId, item.ComArt_F_ARTENUMREF_SagId, item.ComArt_F_CONDITION_SagId);
                    if (F_ARTICLE_Composition != null)
                    {
                        Model.Sage.F_ARTICLE F_ARTICLE = F_ARTICLERepository.ReadArticle(item.ComArt_F_ARTICLE_SagId);
                        Model.Prestashop.PsProductAttributeRepository PsProductAttributeRepository = new Model.Prestashop.PsProductAttributeRepository();
                        Model.Prestashop.PsProductAttribute PsProductAttributeComposition = new Model.Prestashop.PsProductAttribute();
                        Boolean isProductAttribute = false;
                        if (item.Pre_Id != null && PsProductAttributeRepository.ExistProductAttribute((uint)item.Pre_Id))
                        {
                            PsProductAttributeComposition = PsProductAttributeRepository.ReadProductAttribute((uint)item.Pre_Id);
                            if (PsProductAttributeComposition != null)
                                isProductAttribute = true;
                        }

                        if (isProductAttribute == true)
                        {
                            int CatComptaArticle = Core.Global.GetConfig().ConfigArticleCatComptable;
                            Model.Sage.F_TAXE TaxeTVA = SynchronisationArticle.ReadTaxe(F_ARTICLE, Product, CatComptaArticle);

                            Model.Prestashop.PsProduct temp_product = new Model.Prestashop.PsProduct();
                            temp_product.taxe_famillesage = Product.taxe_famillesage;

                            // temp_product sert pour conserver l'information "ecoraxeht_sage" pour la fonction SpecificPrice
                            Model.Sage.F_TAXE TaxeEco = SynchronisationArticle.ReadEcoTaxe(F_ARTICLE, temp_product, TaxeTVA, CatComptaArticle);
                            PsProductAttributeComposition.EcOtAx = temp_product.EcOtAx;
                            
                            if (F_ARTICLE_Composition.F_CONDITION_SagId == null && F_ARTICLE_Composition.F_ARTENUMREF_SagId == null)
                            {
                                if (Core.Global.GetConfig().MajPoidsSynchroStock)
                                    SynchronisationArticle.ReadWeight(F_ARTICLE, PsProductAttributeComposition);

                                SynchronisationArticle.ReadQuantity(F_ARTICLE, PsProductAttributeComposition);

                                //SynchronisationArticle.ReadPrice(F_ARTICLE, PsProductAttributeComposition, TaxeTVA);

                                //List<string> log;
                                //this.ExecSpecificPrice(F_ARTICLE, Product, Article.Art_Id, TaxeTVA, TaxeEco, out log);
                                //if (log != null && log.Count > 0)
                                //    log_chrono.AddRange(log);
                            }
                            else if (F_ARTICLE_Composition.F_ARTENUMREF_SagId != null)
                            {
                                Model.Prestashop.PsProduct temp_product_f_artenumref = new Model.Prestashop.PsProduct();
                                // lecture des valeurs pour F_ARTICLE
                                if (Core.Global.GetConfig().MajPoidsSynchroStock)
                                    SynchronisationArticle.ReadWeight(F_ARTICLE, temp_product_f_artenumref);

                                SynchronisationArticle.ReadQuantity(F_ARTICLE, temp_product_f_artenumref);
                                //SynchronisationArticle.ReadPrice(F_ARTICLE, temp_product_f_artenumref, TaxeTVA);

                                // transformation en fonction de f_artenumref
                                if (Core.Global.GetConfig().MajPoidsSynchroStock)
                                {
                                    SynchronisationArticle.ReadWeightAttribute(temp_product_f_artenumref, PsProductAttributeComposition, F_ARTICLE, item.EnumereGamme1);
                                    PsProductAttributeComposition.Weight += temp_product_f_artenumref.Weight;
                                }
                                SynchronisationArticle.ReadQuantityAttribute(PsProductAttributeComposition, F_ARTICLE, item.EnumereGamme1, item.EnumereGamme2);
                                //SynchronisationArticle.ReadRefEANAttribute(PsProductAttributeComposition, F_ARTICLE, item.EnumereGamme1, item.EnumereGamme2);
                                //SynchronisationArticle.ReadPriceAttribute(temp_product_f_artenumref, PsProductAttributeComposition, F_ARTICLE, item.EnumereGamme1, item.EnumereGamme2, TaxeTVA);
                                //PsProductAttributeComposition.Price += temp_product_f_artenumref.Price;
                            }
                            else if (F_ARTICLE_Composition.F_CONDITION_SagId != null)
                            {
                                Model.Prestashop.PsProduct temp_product_f_condition = new Model.Prestashop.PsProduct();
                                // lecture des valeurs pour F_ARTICLE
                                if (Core.Global.GetConfig().MajPoidsSynchroStock)
                                    SynchronisationArticle.ReadWeight(F_ARTICLE, temp_product_f_condition);

                                SynchronisationArticle.ReadQuantity(F_ARTICLE, temp_product_f_condition);
                                //SynchronisationArticle.ReadPrice(F_ARTICLE, temp_product_f_condition, TaxeTVA);

                                // transformation en fonction de F_CONDITION
                                if (Core.Global.GetConfig().MajPoidsSynchroStock)
                                {
                                    SynchronisationArticle.ReadWeightConditioning(temp_product_f_condition, PsProductAttributeComposition, item.EnumereF_CONDITION);
                                    PsProductAttributeComposition.Weight += temp_product_f_condition.Weight;
                                }
                                SynchronisationArticle.ReadQuantityConditioning(PsProductAttributeComposition, F_ARTICLE, item.EnumereF_CONDITION);
                                //SynchronisationArticle.ReadPriceConditioning(temp_product_f_condition, PsProductAttributeComposition, F_ARTICLE, item.EnumereF_CONDITION, TaxeTVA);
                                //PsProductAttributeComposition.Price += temp_product_f_condition.Price;
                            }

                            //PsProductAttributeComposition.DefaultOn = (item.ComArt_Default) ? (byte)1 : (byte)0;
                            PsProductAttributeRepository.Save();
                            SynchronisationArticle.ExecShopProductAttribute(PsProductAttributeComposition);

                            SynchronisationArticle.WriteStockAvailableComposition(Product, PsProductAttributeComposition);
							
                            // prix spécifiques
                            //temp_product.Price = PsProductAttributeComposition.Price;
                            //temp_product.IDProduct = Product.IDProduct;
                            //List<string> log;
                            //this.ExecSpecificPrice(F_ARTICLE, temp_product, Article, item, TaxeTVA, TaxeEco, out log);
                            //if (log != null && log.Count > 0)
                            //    log_chrono.AddRange(log);
                        }
                    }
                }
                catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
            }
        }
    }
}
