using System;
using System.Linq;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Core.ImportPrestashop
{
    public class ImportArticle
    {
        public void Exec(Int32 ProductSend)
        {
            try
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                if (ArticleRepository.ExistPre_Id(ProductSend) == false)
                {
                    Model.Prestashop.PsProductLangRepository PsProductLangRepository = new Model.Prestashop.PsProductLangRepository();
                    if (PsProductLangRepository.ExistProductLang(ProductSend, Global.Lang, Global.CurrentShop.IDShop))
                    {
                        Model.Prestashop.PsProductRepository PsProductRepository = new Model.Prestashop.PsProductRepository();
                        Model.Prestashop.PsProduct PsProduct = PsProductRepository.ReadId(Convert.ToUInt32(ProductSend));
                        Model.Prestashop.PsProductLang PsProductLang = PsProductLangRepository.ReadProductLang(ProductSend, Global.Lang, Global.CurrentShop.IDShop);

                        Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
						string product_reference = PsProduct.Reference;
						product_reference = product_reference.Replace(" ", "_");
						if (F_ARTICLERepository.ExistReference(product_reference))
                        {
                            Model.Sage.F_ARTICLE F_ARTICLE = F_ARTICLERepository.ReadReference(product_reference);
                            Model.Local.Article Article = new Model.Local.Article()
                            {
                                Art_Name = PsProductLang.Name,
                                Art_Description = PsProductLang.Description,
                                Art_Description_Short = PsProductLang.DescriptionShort,
                                Art_LinkRewrite = PsProductLang.LinkRewrite,
                                Art_MetaTitle = PsProductLang.MetaTitle,
                                Art_MetaKeyword = PsProductLang.MetaKeywords,
                                Art_MetaDescription = PsProductLang.MetaDescription,
                                Art_Ref = PsProduct.Reference,
                                Art_Ean13 = PsProduct.EAn13,
                                Art_Pack = PsProduct.CacheIsPack == 1,
                                Art_Solde = Convert.ToBoolean(PsProduct.OnSale),
                                Art_Active = Convert.ToBoolean(PsProduct.Active),
                                Art_Sync = true,
                                Art_SyncPrice = true,
                                Art_Date = (PsProduct.DateUpd != null && PsProduct.DateUpd > new DateTime(1753, 1, 2)) ? PsProduct.DateUpd : DateTime.Now.Date,
                                Sag_Id = F_ARTICLE.cbMarq,
                                Pre_Id = Convert.ToInt32(PsProduct.IDProduct),
                                Cat_Id = this.ReadCatalog(PsProduct.IDCategoryDefault),
                                Art_RedirectType = new Model.Internal.RedirectType(Core.Parametres.RedirectType.NoRedirect404).Page,
                                Art_RedirectProduct = 0,
                                Art_Manufacturer = (PsProduct.IDManufacturer != null) ? (int)PsProduct.IDManufacturer : 0,
                                Art_Supplier = (PsProduct.IDSupplier != null) ? (int)PsProduct.IDSupplier : 0,
                            };

                            if (Article.Cat_Id == 0)
                            {
                                foreach (Model.Prestashop.PsCategoryProduct PsCategoryProduct in new Model.Prestashop.PsCategoryProductRepository().ListProduct(PsProduct.IDProduct))
                                {
                                    Article.Cat_Id = this.ReadCatalog(PsCategoryProduct.IDCategory);
                                    if (Article.Cat_Id != 0)
                                    {
                                        break;
                                    }
                                }
                            }

                            if (PsProduct.CacheIsPack == 1)
                            {
                                Article.Art_Pack = true;
                            }
                            if (Article.Cat_Id != 0)
                            {
                                ArticleRepository.Add(Article);
                                RecoveryChildData(Article, PsProduct, ArticleRepository, false);
                            }
                        }
                    }
                }
                else
                {
                    Model.Local.Article Article = ArticleRepository.ReadPre_Id(ProductSend);
                    Model.Prestashop.PsProductRepository PsProductRepository = new Model.Prestashop.PsProductRepository();
                    Model.Prestashop.PsProduct PsProduct = PsProductRepository.ReadId(Convert.ToUInt32(ProductSend));
                    RecoveryChildData(Article, PsProduct, ArticleRepository, true);
                }

            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        public void RecoveryChildData(Model.Local.Article Article, Model.Prestashop.PsProduct PsProduct, Model.Local.ArticleRepository ArticleRepository, Boolean ExistLocal)
        {
            ReadRedirection(Article);
            AssociateCatalogue(PsProduct, Article, ArticleRepository);
            RecoveryDataProductAttribute(PsProduct);
            ImportCharacteristic(PsProduct, Article, true);
        }

        private Int32 ReadCatalog(uint? IdPsCategory)
        {
            Int32 Catalog = 0;
            try
            {
                Model.Local.CatalogRepository CatalogRepository = new Model.Local.CatalogRepository();
                if (IdPsCategory != null && CatalogRepository.ExistPre_Id((int)IdPsCategory))
                {
                    Catalog = CatalogRepository.ReadPre_Id((int)IdPsCategory).Cat_Id;
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
            return Catalog;
        }

        private void AssociateCatalogue(Model.Prestashop.PsProduct Product, Model.Local.Article Article, Model.Local.ArticleRepository ArticleRepository)
        {
            //Model.Prestashop.PsCategoryProductRepository PsCategoryProductRepository = new Model.Prestashop.PsCategoryProductRepository();
            //Model.Local.CatalogRepository CatalogRepository = new Model.Local.CatalogRepository();
            //Model.Local.Catalog Catalog;
            //Model.Local.ArticleCatalogRepository ArticleCatalogRepository = new Model.Local.ArticleCatalogRepository();

            //List<Model.Prestashop.PsCategoryProduct> ListPsCategoryProduct = PsCategoryProductRepository.ListProduct(PsProduct.IDProduct);
            //foreach (Model.Prestashop.PsCategoryProduct PsCategoryProduct in ListPsCategoryProduct)
            //{
            //    if (CatalogRepository.ExistPre_Id((Int32)PsCategoryProduct.IDCategory))
            //    {
            //        Catalog = CatalogRepository.ReadPre_Id((Int32)PsCategoryProduct.IDCategory);
            //        if (ArticleCatalogRepository.ExistArticleCatalog(Article.Art_Id, Catalog.Cat_Id) == false)
            //        {
            //            Model.Local.ArticleCatalog ArticleCatalog = new Model.Local.ArticleCatalog()
            //            {
            //                Art_Id = Article.Art_Id,
            //                Cat_Id = Catalog.Cat_Id
            //            };
            //            ArticleCatalogRepository.Add(ArticleCatalog);
            //        }
            //    }
            //}
            Model.Prestashop.PsCategoryProductRepository PsCategoryProductRepository = new Model.Prestashop.PsCategoryProductRepository();
            Model.Local.CatalogRepository CatalogRepository = new Model.Local.CatalogRepository();
            Model.Local.ArticleCatalogRepository ArticleCatalogRepository = new Model.Local.ArticleCatalogRepository();
            Model.Prestashop.PsCategoryRepository PsCategoryRepository = new Model.Prestashop.PsCategoryRepository();

            // filtrage des catalogues existants dans PC et PS
            List<Model.Local.CatalogLight> ListLocalCatalog = CatalogRepository.ListLight();
            List<uint> ListPrestashopCategory = PsCategoryRepository.ListIdOrderByLevelDepth(Core.Global.CurrentShop.IDShop, PsCategoryRepository.ReadId(Core.Global.CurrentShop.IDCategory).LevelDepth);
            ListLocalCatalog = ListLocalCatalog.Where(lc => ListPrestashopCategory.Count(pc => pc == (uint)lc.Pre_Id) > 0).ToList();

            // filtrage des associations PS par rapport aux catégories PS existantes en tant que catalogue PC
            List<Model.Prestashop.PsCategoryProduct> ListPsCategoryProduct = PsCategoryProductRepository.ListProduct(Product.IDProduct);
            ListPsCategoryProduct = ListPsCategoryProduct.Where(cp => ListLocalCatalog.Count(lc => lc.Pre_Id == cp.IDCategory) > 0).ToList();

            // filtrage des associations PC par rapport aux catégories ayant un ID PS
            List<Model.Local.ArticleCatalog> ListLocal = ArticleCatalogRepository.ListArticle(Article.Art_Id);
            ListLocal = ListLocal.Where(ac => ac.Catalog.Pre_Id != null).ToList();

            // tant que les associations en local contiennent des associations non présentes dans Prestashop
            if (Core.Global.GetConfig().DeleteCatalogProductAssociation)
            {
                // suppression des occurences inexistantes dans PS
                while (ListLocal.Count(ac => ListPsCategoryProduct.Count(cp => cp.IDCategory == ac.Catalog.Pre_Id) == 0) > 0)
                {
                    Model.Local.ArticleCatalog target = ListLocal.FirstOrDefault(ac => ListPsCategoryProduct.Count(cp => cp.IDCategory == ac.Catalog.Pre_Id) == 0);
                    ArticleCatalogRepository.Delete(target);
                    ListLocal.Remove(target);
                };
            }

            // récupération catégorie principale si catalogue existant dans Prestaconnect
            if (Product.IDCategoryDefault != null && Product.IDCategoryDefault != 0
                && ListLocalCatalog.Count(lc => lc.Pre_Id == (int)Product.IDCategoryDefault) > 0)
            {
                ArticleRepository = new Model.Local.ArticleRepository();
                Article = ArticleRepository.ReadArticle(Article.Art_Id);

                Article.Cat_Id = ListLocalCatalog.FirstOrDefault(lc => lc.Pre_Id == (int)Product.IDCategoryDefault).Cat_Id;
                ArticleRepository.Save();

                if (ListLocal.Count(ac => ac.Art_Id == Article.Art_Id && ac.Cat_Id == Article.Cat_Id) == 0)
                {
                    ArticleCatalogRepository.Add(new Model.Local.ArticleCatalog()
                    {
                        Art_Id = Article.Art_Id,
                        Cat_Id = Article.Cat_Id
                    });
                }
            }

            // filtre des associations Prestashop par rapport à celles déjà présentes dans Prestaconnect puis ajout
            ListLocal = ArticleCatalogRepository.ListArticle(Article.Art_Id);
            ListLocal = ListLocal.Where(ac => ac.Catalog.Pre_Id != null).ToList();
            ListPsCategoryProduct = ListPsCategoryProduct.Where(cp => ListLocal.Count(ac => ac.Catalog.Pre_Id == (Int32)cp.IDCategory) == 0).ToList();
            foreach (Model.Prestashop.PsCategoryProduct PsCategoryProduct in ListPsCategoryProduct)
            {
                ArticleCatalogRepository.Add(new Model.Local.ArticleCatalog()
                {
                    Art_Id = Article.Art_Id,
                    Cat_Id = ListLocalCatalog.FirstOrDefault(lc => lc.Pre_Id == (Int32)PsCategoryProduct.IDCategory).Cat_Id
                });
            }
        }

        private void RecoveryDataProductAttribute(Model.Prestashop.PsProduct Product)
        {
            Model.Prestashop.PsProductAttributeRepository PsProductAttributeRepository = new Model.Prestashop.PsProductAttributeRepository();
            Model.Prestashop.PsProductAttributeShopRepository PsProductAttributeShopRepository = new Model.Prestashop.PsProductAttributeShopRepository();
            Model.Prestashop.PsProductAttributeImageRepository PsProductAttributeImageRepository = new Model.Prestashop.PsProductAttributeImageRepository();

            Model.Local.AttributeArticleRepository AttributeArticleRepository = new Model.Local.AttributeArticleRepository();
            Model.Local.ConditioningArticleRepository ConditioningArticleRepository = new Model.Local.ConditioningArticleRepository();
            Model.Local.CompositionArticleRepository CompositionArticleRepository = new Model.Local.CompositionArticleRepository();

            Model.Local.AttributeArticleImageRepository AttributeArticleImageRepository = new Model.Local.AttributeArticleImageRepository();
            Model.Local.CompositionArticleImageRepository CompositionArticleImageRepository = new Model.Local.CompositionArticleImageRepository();
            Model.Local.ArticleImageRepository ArticleImageRepository = new Model.Local.ArticleImageRepository();

            Model.Local.AttributeArticle AttributeArticle;
            Model.Local.ConditioningArticle ConditioningArticle;
            Model.Local.CompositionArticle CompositionArticle;
            Model.Local.ArticleImage ArticleImage;

            List<Model.Prestashop.PsProductAttribute> ListPsProductAttribute = PsProductAttributeRepository.List(Product.IDProduct);
            //parcours déclinaisons PrestaShop
            foreach (Model.Prestashop.PsProductAttribute PsProductAttribute in ListPsProductAttribute)
            {
                // test si déclinaison en gamme dans PrestaConnect
                if (AttributeArticleRepository.ExistPrestashop((int)PsProductAttribute.IDProductAttribute))
                {
                    AttributeArticle = AttributeArticleRepository.ReadPrestashop((int)PsProductAttribute.IDProductAttribute);
                    Model.Prestashop.PsProductAttributeShop PsProductAttributeShop = (PsProductAttributeShopRepository.ExistPsProductAttributeShop(PsProductAttribute.IDProductAttribute, Core.Global.CurrentShop.IDShop))
                        ? PsProductAttributeShopRepository.ReadPsProductAttributeShop(PsProductAttribute.IDProductAttribute, Core.Global.CurrentShop.IDShop)
                        : null;
                    bool defaut = (PsProductAttributeShop != null) ? PsProductAttributeShop.DefaultOn == 1 : PsProductAttribute.DefaultOn == 1;
                    if (AttributeArticle.AttArt_Default != defaut)
                    {
                        AttributeArticle.AttArt_Default = defaut;
                        AttributeArticleRepository.Save();
                        if (defaut)
                        {
                            List<Model.Local.AttributeArticle> ListAttributeArticle = AttributeArticleRepository.ListArticle(AttributeArticle.Art_Id);
                            if (ListAttributeArticle.Count(i => i.AttArt_Default == true && i.AttArt_Id != AttributeArticle.AttArt_Id) > 0)
                                foreach (Model.Local.AttributeArticle AttributeArticleDefault in ListAttributeArticle.Where(i => i.AttArt_Default == true && i.AttArt_Id != AttributeArticle.Art_Id))
                                {
                                    AttributeArticleDefault.AttArt_Default = false;
                                    AttributeArticleRepository.Save();
                                }
                        }
                    }

                    // attribution images gammes
                    if (PsProductAttributeImageRepository.ExistProductAttribute(PsProductAttribute.IDProductAttribute))
                    {
                        List<Model.Prestashop.PsProductAttributeImage> ListPsProductAttributeImage = PsProductAttributeImageRepository.ListProductAttribute(PsProductAttribute.IDProductAttribute);
                        // étape 1 attachement dans PrestaConnect des images affectées a la déclinaison PrestaShop
                        foreach (Model.Prestashop.PsProductAttributeImage PsProductAttributeImage in ListPsProductAttributeImage)
                        {
                            // si l'image existe dans PrestaConnect
                            if (ArticleImageRepository.ExistPre_Id((int)PsProductAttributeImage.IDImage))
                            {
                                ArticleImage = ArticleImageRepository.ReadPrestaShop((int)PsProductAttributeImage.IDImage);
                                if (!AttributeArticleImageRepository.ExistAttributeArticleImage(AttributeArticle.AttArt_Id, ArticleImage.ImaArt_Id))
                                {
                                    AttributeArticleImageRepository.Add(new Model.Local.AttributeArticleImage()
                                    {
                                        AttArt_Id = AttributeArticle.AttArt_Id,
                                        ImaArt_Id = ArticleImage.ImaArt_Id,
                                    });
                                }
                            }
                        }
                        // étape 2 détachement
                        List<uint> list_prestashop = ListPsProductAttributeImage.Select(pai => pai.IDImage).ToList();
                        foreach (Model.Local.AttributeArticleImage AttributeArticleImage in AttributeArticleImageRepository.ListAttributeArticle(AttributeArticle.AttArt_Id))
                        {
                            if (AttributeArticleImage.ArticleImage.Pre_Id != null
                                && !list_prestashop.Contains((uint)AttributeArticleImage.ArticleImage.Pre_Id.Value))
                            {
                                AttributeArticleImageRepository.Delete(AttributeArticleImage);
                            }
                        }
                    }
                    else if (AttributeArticleImageRepository.ExistAttributeArticle(AttributeArticle.AttArt_Id))
                    {
                        // absence de lien gamme déclinaison dans Prestashop donc suppression des liens dans PrestaConnect
                        AttributeArticleImageRepository.DeleteAll(AttributeArticleImageRepository.ListAttributeArticle(AttributeArticle.AttArt_Id));
                    }
                }
                // test si déclinaison en conditionnement dans PrestaConnect
                else if (ConditioningArticleRepository.ExistPrestashop((int)PsProductAttribute.IDProductAttribute))
                {
                    ConditioningArticle = ConditioningArticleRepository.ReadPrestashop((int)PsProductAttribute.IDProductAttribute);
                    Model.Prestashop.PsProductAttributeShop PsProductAttributeShop = (PsProductAttributeShopRepository.ExistPsProductAttributeShop(PsProductAttribute.IDProductAttribute, Core.Global.CurrentShop.IDShop))
                        ? PsProductAttributeShopRepository.ReadPsProductAttributeShop(PsProductAttribute.IDProductAttribute, Core.Global.CurrentShop.IDShop)
                        : null;
                    bool defaut = (PsProductAttributeShop != null) ? PsProductAttributeShop.DefaultOn == 1 : PsProductAttribute.DefaultOn == 1;
                    if (ConditioningArticle.ConArt_Default != defaut)
                    {
                        ConditioningArticle.ConArt_Default = defaut;
                        ConditioningArticleRepository.Save();
                        if (defaut)
                        {
                            List<Model.Local.ConditioningArticle> ListConditioningArticle = ConditioningArticleRepository.ListArticle(ConditioningArticle.Art_Id);
                            if (ListConditioningArticle.Count(i => i.ConArt_Default == true && i.ConArt_Id != ConditioningArticle.ConArt_Id) > 0)
                                foreach (Model.Local.ConditioningArticle ConditioningArticleDefault in ListConditioningArticle.Where(i => i.ConArt_Default == true && i.ConArt_Id != ConditioningArticle.Art_Id))
                                {
                                    ConditioningArticleDefault.ConArt_Default = false;
                                    ConditioningArticleRepository.Save();
                                }
                        }
                    }
                }
                // test si déclinaison composition dans PrestaConnect
                else if (CompositionArticleRepository.ExistPrestaShop((int)PsProductAttribute.IDProductAttribute))
                {
                    CompositionArticle = CompositionArticleRepository.ReadPrestaShop((int)PsProductAttribute.IDProductAttribute);
                    Model.Prestashop.PsProductAttributeShop PsProductAttributeShop = (PsProductAttributeShopRepository.ExistPsProductAttributeShop(PsProductAttribute.IDProductAttribute, Core.Global.CurrentShop.IDShop))
                        ? PsProductAttributeShopRepository.ReadPsProductAttributeShop(PsProductAttribute.IDProductAttribute, Core.Global.CurrentShop.IDShop)
                        : null;
                    bool defaut = (PsProductAttributeShop != null) ? PsProductAttributeShop.DefaultOn == 1 : PsProductAttribute.DefaultOn == 1;
                    if (CompositionArticle.ComArt_Default != defaut)
                    {
                        CompositionArticle.ComArt_Default = defaut;
                        CompositionArticleRepository.Save();
                        if (defaut)
                        {
                            List<Model.Local.CompositionArticle> ListCompositionArticle = CompositionArticleRepository.ListArticle(CompositionArticle.ComArt_ArtId);
                            if (ListCompositionArticle.Count(i => i.ComArt_Default == true && i.ComArt_Id != CompositionArticle.ComArt_Id) > 0)
                                foreach (Model.Local.CompositionArticle CompositionArticleDefault in ListCompositionArticle.Where(i => i.ComArt_Default == true && i.ComArt_Id != CompositionArticle.ComArt_ArtId))
                                {
                                    CompositionArticleDefault.ComArt_Default = false;
                                    ConditioningArticleRepository.Save();
                                }
                        }
                    }

                    // attribution images gammes
                    if (PsProductAttributeImageRepository.ExistProductAttribute(PsProductAttribute.IDProductAttribute))
                    {
                        List<Model.Prestashop.PsProductAttributeImage> ListPsProductAttributeImage = PsProductAttributeImageRepository.ListProductAttribute(PsProductAttribute.IDProductAttribute);
                        // étape 1 attachement dans PrestaConnect des images affectées a la déclinaison PrestaShop
                        foreach (Model.Prestashop.PsProductAttributeImage PsProductAttributeImage in ListPsProductAttributeImage)
                        {
                            // si l'image existe dans PrestaConnect
                            if (ArticleImageRepository.ExistPre_Id((int)PsProductAttributeImage.IDImage))
                            {
                                ArticleImage = ArticleImageRepository.ReadPrestaShop((int)PsProductAttributeImage.IDImage);
                                if (!CompositionArticleImageRepository.ExistCompositionArticleImage(CompositionArticle.ComArt_Id, ArticleImage.ImaArt_Id))
                                {
                                    CompositionArticleImageRepository.Add(new Model.Local.CompositionArticleImage()
                                    {
                                        ComArt_Id = CompositionArticle.ComArt_Id,
                                        ImaArt_Id = ArticleImage.ImaArt_Id,
                                    });
                                }
                            }
                        }
                        // étape 2 détachement
                        List<uint> list_prestashop = ListPsProductAttributeImage.Select(pai => pai.IDImage).ToList();
                        foreach (Model.Local.CompositionArticleImage CompositionArticleImage in CompositionArticleImageRepository.ListCompositionArticle(CompositionArticle.ComArt_Id))
                        {
                            if (CompositionArticleImage.ArticleImage.Pre_Id != null
                                && !list_prestashop.Contains((uint)CompositionArticleImage.ArticleImage.Pre_Id.Value))
                            {
                                CompositionArticleImageRepository.Delete(CompositionArticleImage);
                            }
                        }
                    }
                    else if (CompositionArticleImageRepository.ExistCompositionArticle(CompositionArticle.ComArt_Id))
                    {
                        // absence de lien gamme déclinaison dans Prestashop donc suppression des liens dans PrestaConnect
                        CompositionArticleImageRepository.DeleteAll(CompositionArticleImageRepository.ListCompositionArticle(CompositionArticle.ComArt_Id));
                    }
                }
            }
        }
        
        public void ImportCharacteristic(Model.Prestashop.PsProduct PsProduct, Model.Local.Article Article, bool ArticleExists)
        {
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config Config = ConfigRepository.ReadName(Global.ConfigLang);

            if (Config != null && Global.IsNumeric(Config.Con_Value))
            {
                Model.Prestashop.PsFeatureProductRepository PsFeatureProductRepository = new Model.Prestashop.PsFeatureProductRepository();
                Model.Prestashop.PsFeatureValueRepository PsFeatureValueRepository = new Model.Prestashop.PsFeatureValueRepository();
                Model.Prestashop.PsFeatureValueLangRepository PsFeatureValueLangRepository = new Model.Prestashop.PsFeatureValueLangRepository();
                Model.Local.CharacteristicRepository CharacteristicRepository = new Model.Local.CharacteristicRepository();

                foreach (Model.Prestashop.PsFeatureProduct FeatureProduct in PsFeatureProductRepository.List(PsProduct.IDProduct))
                {
                    Model.Prestashop.PsFeatureValue PsFeatureValue = PsFeatureValueRepository.ReadFeatureValue(FeatureProduct.IDFeatureValue);
                    Model.Prestashop.PsFeatureValueLang PsFeatureValueLang = PsFeatureValueLangRepository.ReadFeatureValueLang(FeatureProduct.IDFeatureValue, Convert.ToUInt32(Config.Con_Value));

                    Model.Local.Characteristic Characteristic = null;

                    if (ArticleExists)
                        Characteristic = CharacteristicRepository.ReadFeatureArticle((int)FeatureProduct.IDFeature, Article.Art_Id);

                    if (!ArticleExists || Characteristic == null)
                    {
                        Characteristic = new Model.Local.Characteristic()
                        {
                            Art_Id = Article.Art_Id,
                            Cha_Custom = Convert.ToBoolean(PsFeatureValue.Custom),
                            Cha_IdFeature = (int)FeatureProduct.IDFeature,
                            Cha_Value = PsFeatureValueLang.Value,
                            Pre_Id = (int)FeatureProduct.IDFeatureValue,
                        };

                        CharacteristicRepository.Add(Characteristic);
                    }
                    else
                    {
                        Characteristic.Cha_Custom = Convert.ToBoolean(PsFeatureValue.Custom);
                        Characteristic.Cha_Value = PsFeatureValueLang.Value;
                        Characteristic.Pre_Id = (int)FeatureProduct.IDFeatureValue;

                        CharacteristicRepository.Save();
                    }
                }
            }
        }

        private void ReadRedirection(Model.Local.Article Article)
        {
            try
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                Article = ArticleRepository.ReadArticle(Article.Art_Id);
                Model.Prestashop.DataClassesPrestashop DBPrestashop = new Model.Prestashop.DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));
                List<Model.Prestashop.Product_Redirection> list = DBPrestashop.ExecuteQuery<Model.Prestashop.Product_Redirection>
					("select redirect_type, " +
					#if (PRESTASHOP_VERSION_172)
					"id_type_redirected " +
					#else
					"id_product_redirected " + 
					#endif
					" from ps_product_shop where id_product = " + Article.Pre_Id + " and id_shop = " + Core.Global.CurrentShop.IDShop + " ").ToList();

                if (list != null && list.Count == 1)
                {
                    Model.Prestashop.Product_Redirection values = list.FirstOrDefault();
                    Article.Art_RedirectType = values.redirect_type;
					#if (PRESTASHOP_VERSION_172)
					Article.Art_RedirectProduct = (ArticleRepository.ExistPre_Id((int)values.id_type_redirected)) ? ArticleRepository.ReadPre_Id((int)values.id_type_redirected).Art_Id : 0;
					#else
					Article.Art_RedirectProduct = (ArticleRepository.ExistPre_Id((int)values.id_product_redirected)) ? ArticleRepository.ReadPre_Id((int)values.id_product_redirected).Art_Id : 0;
					#endif
					ArticleRepository.Save();
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
    }
}
