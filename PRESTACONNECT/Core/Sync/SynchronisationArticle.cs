using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PRESTACONNECT.Core.Parametres;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Core.Sync
{
    public class SynchronisationArticle
    {
        #region Variables calculs tarifs

        // Codes taxes Sage attribués sur la famille et non sur l'article
        //private bool isTaxeFamille = false;
        // montant éco-taxe HT pour calcul prix HT lorsque tarif Sage en TTC
        //public decimal ecotaxe_HT = 0;

        public List<String> log_chrono = new List<string>();

        #endregion

        public void Exec(Int32 ArticleSend, out uint pre_id)
        {
            pre_id = 0;
            try
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                Model.Local.Article Article = ArticleRepository.ReadArticle(ArticleSend);

                // If the catalog is sync with Prestashop
                if (Article.Catalog != null && Article.Catalog.Pre_Id != null && Article.Catalog.Pre_Id != 0
                    && new Model.Prestashop.PsCategoryRepository().ExistId((int)Article.Catalog.Pre_Id))
                {
                    Model.Prestashop.PsProductRepository ProductRepository = new Model.Prestashop.PsProductRepository();
                    Boolean isProduct = false;
                    Model.Prestashop.PsProduct Product = new Model.Prestashop.PsProduct();
                    // If the Article have a connection with Prestashop
                    if (Article.Pre_Id != null)
                    {
                        Article.Art_Date = Article.Art_Date.AddMilliseconds(-Article.Art_Date.Millisecond);
                        if (ProductRepository.ExistId(Convert.ToUInt32(Article.Pre_Id.Value)))
                        {
                            Product = ProductRepository.ReadId(Convert.ToUInt32(Article.Pre_Id.Value));
                            isProduct = true;
                            if (Product.DateUpd.Ticks > Article.Art_Date.Ticks)
                            {
                                this.ExecDistantToLocal(Product, Article, ArticleRepository);
                            }
                            else if (Product.DateUpd.Ticks < Article.Art_Date.Ticks)
                            {
                                this.ExecLocalToDistant(Article, Product, ArticleRepository, ProductRepository, isProduct);
                                this.ExecFeature(Article);
                                pre_id = Product.IDProduct;
                            }
                        }
                    }
                    // We need to sync Article with Product
                    else
                    {
                        this.ExecLocalToDistant(Article, Product, ArticleRepository, ProductRepository, isProduct);
                        this.ExecFeature(Article);
                        pre_id = Product.IDProduct;
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void ExecLocalToDistant(Model.Local.Article Article, Model.Prestashop.PsProduct Product, Model.Local.ArticleRepository ArticleRepository, Model.Prestashop.PsProductRepository ProductRepository, Boolean isProduct)
        {
            try
            {
                if (Article.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleComposition)
                {
                    if (Core.UpdateVersion.License.Option2)
                    {
                        Product.OnSale = Convert.ToByte(Article.Art_Solde.Value);
                        Product.IDCategoryDefault = Convert.ToUInt32(Article.Catalog.Pre_Id);
                        Product.EAn13 = Core.Global.RemovePurgeEAN(Article.Art_Ean13);
                        Product.Reference = Article.Art_Ref;
                        Product.DateUpd = Article.Art_Date;
                        Product.Active = Convert.ToByte(Article.Art_Active);
                        Product.IDShopDefault = Core.Global.CurrentShop.IDShop;
                        Product.Price = 0;
                        Product.Quantity = 0;

                        if (Article.Art_Manufacturer != 0 && (Core.Global.GetConfig().MarqueAutoInfolibreActif || Core.Global.GetConfig().MarqueAutoStatistiqueActif))
                            Product.IDManufacturer = (new Model.Prestashop.PsManufacturerRepository().ExistId(Article.Art_Manufacturer)) ? (uint?)Article.Art_Manufacturer : null;
                        if (Article.Art_Supplier != 0 && (Core.Global.GetConfig().FournisseurAutoInfolibreActif || Core.Global.GetConfig().FournisseurAutoStatistiqueActif))
                            Product.IDSupplier = (new Model.Prestashop.PsSupplierRepository().ExistId(Article.Art_Supplier)) ? (uint?)Article.Art_Supplier : null;

                        if (isProduct == false)         // si on est en création
                        {
                            Product.AvailableForOrder = 1;
                            Product.ShowPrice = 1;
                            Product.MinimalQuantity = 1;
                            this.ReadOutOfStock(Product);
                            Product.DateAdd = Product.DateUpd;
                            Product.CacheDefaultAttribute = 0;
                            Product.Unity = string.Empty;
                            Product.Upc = string.Empty;
                            Product.SupplierReference = string.Empty;
                            Product.Location = string.Empty;
                            Product.QuantityDiscount = 0;
                        	#if (PRESTASHOP_VERSION_172)
                            Product.State = 1;
							#endif
                            ProductRepository.Add(Product, Global.CurrentShop.IDShop);

                            Article.Pre_Id = (Int32)Product.IDProduct;
                            ArticleRepository.Save();
                            Core.Temp.SyncArticle_LaunchIndex = true;
                        }
                        else
                        {
                            ProductRepository.Save();
                        }
                        ProductRepository.WriteDate(Product.IDProduct);

                        this.ExecProductLang(Article, Product);
                        this.AssignCatalogProduct(Article, Product);
                        this.WriteStockAvailableProduct(Product);
                        this.ExecCompositionArticle(Article, Product, ProductRepository);
                        this.ExecShopProduct(Product);
                        this.DefineRedirection(Article);
                    }
                }
                else
                {
                    Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
                    if (F_ARTICLERepository.ExistArticle(Article.Sag_Id))
                    {
                        Model.Sage.F_ARTICLE F_ARTICLE = F_ARTICLERepository.ReadArticle(Article.Sag_Id);

                        Product.OnSale = Convert.ToByte(Article.Art_Solde.Value);
                        Product.IDCategoryDefault = Convert.ToUInt32(Article.Catalog.Pre_Id);
                        Product.EAn13 = Core.Global.RemovePurgeEAN(Article.Art_Ean13);
                        Product.Reference = Article.Art_Ref;
                        Product.DateUpd = Article.Art_Date;
                        Product.Active = Convert.ToByte(Article.Art_Active);
                        if (Article.Art_Pack == true)
                            Product.CacheIsPack = 1;

                        if (Article.Art_Manufacturer != 0 && (Core.Global.GetConfig().MarqueAutoInfolibreActif || Core.Global.GetConfig().MarqueAutoStatistiqueActif))
                            Product.IDManufacturer = (new Model.Prestashop.PsManufacturerRepository().ExistId(Article.Art_Manufacturer)) ? (uint?)Article.Art_Manufacturer : null;
                        if (Article.Art_Supplier != 0 && (Core.Global.GetConfig().FournisseurAutoInfolibreActif || Core.Global.GetConfig().FournisseurAutoStatistiqueActif))
                            Product.IDSupplier = (new Model.Prestashop.PsSupplierRepository().ExistId(Article.Art_Supplier)) ? (uint?)Article.Art_Supplier : null;

                        this.ReadWeight(F_ARTICLE, Product);
                        this.ReadQuantity(F_ARTICLE, Product);
                        int CatComptaArticle = Core.Global.GetConfig().ConfigArticleCatComptable;
                        Model.Sage.F_TAXE TaxeTVA = ReadTaxe(F_ARTICLE, Product, CatComptaArticle);
                        Model.Sage.F_TAXE TaxeEco = this.ReadEcoTaxe(F_ARTICLE, Product, TaxeTVA, CatComptaArticle);
                        // <JG> 03/06/2016
                        if (Article.Art_SyncPrice)
                            this.ReadPrice(F_ARTICLE, Product, TaxeTVA);
                        Product.IDShopDefault = Core.Global.CurrentShop.IDShop;

                        if (Core.Global.GetConfig().ArticleDateDispoInfoLibreActif)
                            this.ReadDateDispo(Product, F_ARTICLE);

                        if (isProduct == false)         // si on est en création
                        {
                            Product.AvailableForOrder = 1;
                            Product.ShowPrice = 1;
                            Product.MinimalQuantity = 1;
                            if (Core.Global.GetConfig().ArticleQuantiteMiniActif)
                                ReadMinimalQuantity(Product, F_ARTICLE);
                            this.ReadOutOfStock(Product);
                            Product.DateAdd = Product.DateUpd;
                            Product.CacheDefaultAttribute = 0;
                            Product.Unity = string.Empty;
                            Product.Upc = string.Empty;
                            Product.SupplierReference = string.Empty;
                            Product.Location = string.Empty;
                            Product.QuantityDiscount = 0;
                        	#if (PRESTASHOP_VERSION_172)
                            Product.State = 1;
							#endif
                            ProductRepository.Add(Product, Global.CurrentShop.IDShop);

                            Article.Pre_Id = (Int32)Product.IDProduct;
                            ArticleRepository.Save();
                            Core.Temp.SyncArticle_LaunchIndex = true;
                        }
                        else
                        {
                            if (Core.Global.GetConfig().ArticleQuantiteMiniActif)
                                ReadMinimalQuantity(Product, F_ARTICLE);
                            ProductRepository.Save();
                        }
                        ProductRepository.WriteDate(Product.IDProduct);

                        this.ExecAttribute(Article, Product, ProductRepository, TaxeTVA);
                        this.ExecConditioning(Article, Product, ProductRepository, TaxeTVA);

                        // <JG> 03/06/2016
                        if (Article.Art_SyncPrice)
                        {
                            List<string> log;
                            this.ExecSpecificPrice(F_ARTICLE, Product, Article, null, TaxeTVA, TaxeEco, out log);
                            if (log != null && log.Count > 0)
                                log_chrono.AddRange(log);
                        }

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
                            Int32 QuantityPack = 0;
                            // <AM> 24/10/2014 Si un des articles composant n'a pas de stock on sort de la fonction pour que l'article parent ai un stock à 0
                            Boolean QteZero = false;
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
                                                Int32 CurrentQuantity = this.ReadQuantityPack(F_ARTICLENOMENCLAT, F_NOMENCLAT.NO_Qte.Value);
                                                if (Core.Global.GetConfig().ArticleStockNegatifZero && CurrentQuantity < 0)
                                                    CurrentQuantity = 0;
                                                if (CurrentQuantity < QuantityPack)
                                                {
                                                    QuantityPack = CurrentQuantity;
                                                }
                                                else if (QuantityPack == 0)
                                                {
                                                    QuantityPack = CurrentQuantity;
                                                    // <AM> 14/08/2014 Si un des articles composant n'a pas de stock on sort de la fonction pour que l'article parent ai un stock à 0
                                                    if (CurrentQuantity == 0)
                                                    {
                                                        QteZero = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            // <AM> 24/10/2014 Si un des articles composant n'a pas de stock on sort de la fonction pour que l'article parent ai un stock à 0
                            if (QteZero)
                            {
                                Product.Quantity = 0;
                            }
                            else
                            {
                                Product.Quantity = QuantityPack;
                            }
                        }
                        #endregion

                        this.ExecProductLang(Article, Product);

                        this.AssignCatalogProduct(Article, Product);

                        this.WriteStockAvailableProduct(Product);

                        this.ExecShopProduct(Product);

						this.ExecDWFProductExtraField(Product, Article);
						
                        this.DefineRedirection(Article);

                        if (Article.Art_Supplier != 0 && (Core.Global.GetConfig().FournisseurAutoInfolibreActif || Core.Global.GetConfig().FournisseurAutoStatistiqueActif))
                            this.ExecSupplier(Product);

                        if (Core.Global.GetConfig().ArticleTransfertInfosFournisseurActif)
                        {
                            // gestion fournisseur depuis sage
                            this.ExecSupplierFull(Article, Product, F_ARTICLE, ProductRepository);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void ExecDistantToLocal(Model.Prestashop.PsProduct Product, Model.Local.Article Article, Model.Local.ArticleRepository ArticleRepository)
        {
            try
            {
                #region Recovery Data From ProductLang
                Model.Prestashop.PsProductLangRepository ProductLangRepository = new Model.Prestashop.PsProductLangRepository();
                Model.Prestashop.PsProductLang ProductLang = new Model.Prestashop.PsProductLang();
                if (ProductLangRepository.ExistProductLang((Int32)Product.IDProduct, Global.Lang, Global.CurrentShop.IDShop))
                {
                    ProductLang = ProductLangRepository.ReadProductLang((Int32)Product.IDProduct, Global.Lang, Global.CurrentShop.IDShop);
                    Article.Art_Name = ProductLang.Name;
                    Article.Art_Description = ProductLang.Description;
                    Article.Art_Description_Short = ProductLang.DescriptionShort;
                    Article.Art_LinkRewrite = ProductLang.LinkRewrite;
                    Article.Art_MetaTitle = ProductLang.MetaTitle;
                    Article.Art_MetaKeyword = ProductLang.MetaKeywords;
                    Article.Art_MetaDescription = ProductLang.MetaDescription;
                }
                Article.Art_Ref = Product.Reference;
                Article.Art_Ean13 = Product.EAn13;
                Article.Art_Solde = Convert.ToBoolean(Product.OnSale);
                Article.Art_Active = Convert.ToBoolean(Product.Active);
                Article.Art_Manufacturer = (Product.IDManufacturer != null) ? (int)Product.IDManufacturer : 0;
                Article.Art_Supplier = (Product.IDSupplier != null) ? (int)Product.IDSupplier : 0;

                Article.Art_Date = (Product.DateUpd != null && Product.DateUpd > new DateTime(1753, 1, 2)) ? Product.DateUpd : DateTime.Now.Date;
                ArticleRepository.Save();
                #endregion

                // <JG> 05/11/2015 regroup methods for recovery data
                Core.ImportPrestashop.ImportArticle form = new ImportPrestashop.ImportArticle();
                form.RecoveryChildData(Article, Product, ArticleRepository, true);
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        public void ExecProductLang(Model.Local.Article Article, Model.Prestashop.PsProduct Product)
        {
            Boolean isProductLang = false;
            Model.Prestashop.PsProductLangRepository ProductLangRepository = new Model.Prestashop.PsProductLangRepository();
            Model.Prestashop.PsProductLang ProductLang = new Model.Prestashop.PsProductLang();
            if (ProductLangRepository.ExistProductLang((Int32)Product.IDProduct, Global.Lang, Global.CurrentShop.IDShop))
            {
                ProductLang = ProductLangRepository.ReadProductLang((Int32)Product.IDProduct, Global.Lang, Global.CurrentShop.IDShop);
                isProductLang = true;
            }
            ProductLang.Description = Article.Art_Description;
            ProductLang.DescriptionShort = Article.Art_Description_Short;
            ProductLang.LinkRewrite = Core.Global.ReadLinkRewrite(Article.Art_LinkRewrite);
            ProductLang.MetaDescription = Core.Global.RemovePurge(Article.Art_MetaDescription, 160);
            ProductLang.MetaTitle = Core.Global.RemovePurge(Article.Art_MetaTitle, 70);
            ProductLang.MetaKeywords = Core.Global.RemovePurgeMeta(Article.Art_MetaKeyword, 255);
            ProductLang.Name = Core.Global.RemovePurge(Article.Art_Name, 128);
            if (isProductLang == false)
            {
                ProductLang.IDLang = (uint)Core.Global.Lang;
                ProductLang.IDProduct = Product.IDProduct;
                ProductLang.IDShop = Global.CurrentShop.IDShop;
                ProductLangRepository.Add(ProductLang);
            }
            else
            {
                ProductLangRepository.Save();
            }

            // <JG> 26/12/2012 ajout insertion autres langues actives si non renseignées
            try
            {
                Model.Prestashop.PsLangRepository PsLangRepository = new Model.Prestashop.PsLangRepository();
                foreach (Model.Prestashop.PsLang PsLang in PsLangRepository.ListActive(1, Global.CurrentShop.IDShop))
                {
                    if (!ProductLangRepository.ExistProductLang((int)Product.IDProduct, PsLang.IDLang, Global.CurrentShop.IDShop))
                    {
                        ProductLang = new Model.Prestashop.PsProductLang();
                        ProductLang.IDShop = Global.CurrentShop.IDShop;
                        ProductLang.IDProduct = Product.IDProduct;
                        ProductLang.IDLang = PsLang.IDLang;
                        ProductLang.Description = Article.Art_Description;
                        ProductLang.DescriptionShort = Article.Art_Description_Short;
                        ProductLang.LinkRewrite = Core.Global.ReadLinkRewrite(Article.Art_LinkRewrite);
                        ProductLang.MetaDescription = Core.Global.RemovePurge(Article.Art_MetaDescription, 160);
                        ProductLang.MetaTitle = Core.Global.RemovePurge(Article.Art_MetaTitle, 70);
                        ProductLang.MetaKeywords = Core.Global.RemovePurgeMeta(Article.Art_MetaKeyword, 255);
                        ProductLang.Name = Core.Global.RemovePurge(Article.Art_Name, 128);
                        ProductLangRepository.Add(ProductLang);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        public void ReadWeight(Model.Sage.F_ARTICLE F_ARTICLE, Model.Prestashop.PsProduct Product)
        {
            try
            {
                Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                Model.Local.Config Config = new Model.Local.Config();
                if (ConfigRepository.ExistName(Core.Global.ConfigArticlePoidsType))
                {
                    Config = ConfigRepository.ReadName(Core.Global.ConfigArticlePoidsType);
                    switch (Config.Con_Value)
                    {
                        case "1":
							#if (PRESTASHOP_VERSION_15)
                            Product.Weight = F_ARTICLE.AR_PoidsBrut.HasValue ? (float)F_ARTICLE.AR_PoidsBrut.Value : 0;
							#else
							Product.Weight = F_ARTICLE.AR_PoidsBrut.HasValue ? F_ARTICLE.AR_PoidsBrut.Value : 0;
							#endif
                            break;
                        case "2":
							#if (PRESTASHOP_VERSION_15)
                            Product.Weight = F_ARTICLE.AR_PoidsNet.HasValue ? (float)F_ARTICLE.AR_PoidsNet.Value : 0;
							#else
							Product.Weight = F_ARTICLE.AR_PoidsNet.HasValue ? F_ARTICLE.AR_PoidsNet.Value : 0;
							#endif
                            break;
                        default:
                            Product.Weight = 0;
                            break;
                    }
                }

                if (Product.Weight != 0)
                {
                    // <JG> 10/07/2014 transformation du poids Sage en kilogramme
                    switch (F_ARTICLE.AR_UnitePoids)
                    {
                        case (short)ABSTRACTION_SAGE.F_ARTICLE.Obj._Enum_AR_UnitePoids.Tonne:
                            Product.Weight *= 1000;
                            break;
                        case (short)ABSTRACTION_SAGE.F_ARTICLE.Obj._Enum_AR_UnitePoids.Quintal:
                            Product.Weight *= 100;
                            break;
                        case (short)ABSTRACTION_SAGE.F_ARTICLE.Obj._Enum_AR_UnitePoids.Kilogramme:
                            // no calc
                            break;
                        case (short)ABSTRACTION_SAGE.F_ARTICLE.Obj._Enum_AR_UnitePoids.Gramme:
                            Product.Weight /= 1000;
                            break;
                        case (short)ABSTRACTION_SAGE.F_ARTICLE.Obj._Enum_AR_UnitePoids.Milligramme:
                            Product.Weight /= 1000000;
                            break;
                        default:
                            break;
                    }

                    if (ConfigRepository.ExistName(Core.Global.ConfigArticlePoidsUnite))
                    {
                        Config = ConfigRepository.ReadName(Core.Global.ConfigArticlePoidsUnite);
                        switch (Config.Con_Value)
                        {
                            case "0":
                                Product.Weight /= 1000;
                                break;
                            case "1":
                                Product.Weight /= 100;
                                break;
                            case "2":
                                // no calc
                                break;
                            case "3":
                                Product.Weight *= 1000;
                                break;
                            case "4":
                                Product.Weight *= 1000000;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
        public void ReadWeight(Model.Sage.F_ARTICLE F_ARTICLE, Model.Prestashop.PsProductAttribute PsProductAttribute)
        {
            Model.Prestashop.PsProduct Product = new Model.Prestashop.PsProduct();
            ReadWeight(F_ARTICLE, Product);
            PsProductAttribute.Weight = Product.Weight;
        }

        #region Stocks

        private void ReadOutOfStock(Model.Prestashop.PsProduct Product)
        {
            try
            {
                Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                if (ConfigRepository.ExistName(Core.Global.ConfigArticleRupture))
                {
                    Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigArticleRupture);
                    switch (Config.Con_Value)
                    {
                        case "0":
                            Product.OutOfStock = 0;
                            break;
                        case "1":
                            Product.OutOfStock = 1;
                            break;
                        case "2":
                            Product.OutOfStock = 2;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        public void ReadQuantity(Model.Sage.F_ARTICLE F_ARTICLE, Model.Prestashop.PsProduct Product)
        {
            try
            {
                if (Core.Global.GetConfig().ModuleAECStockActif)
                {
                    Product.AEC_Stock = new Model.Prestashop.PsAEcStock();
                }

                Int32 Quantity = 0;
                Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                // if the F_ARTICLE is not Contremarque
                if (F_ARTICLE.AR_Contremarque == 0 || Core.Global.GetConfig().ArticleContremarqueStockActif)
                {
                    Model.Local.SupplyRepository SupplyRepository = new Model.Local.SupplyRepository();
                    List<Model.Local.Supply> ListSupply = SupplyRepository.ListActive(true);
                    Model.Sage.F_ARTSTOCKRepository F_ARTSTOCKRepository = new Model.Sage.F_ARTSTOCKRepository();

                    // if the ConfigArticleStock 
                    if (ConfigRepository.ExistName(Core.Global.ConfigArticleStock))
                    {
                        Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigArticleStock);
                        // foreach Supply, we add the stock at the quantity
                        foreach (Model.Local.Supply Supply in ListSupply)
                        {
                            if (F_ARTSTOCKRepository.ExistReferenceDepot(F_ARTICLE.AR_Ref, Supply.Sag_Id))
                            {
                                Model.Sage.F_ARTSTOCK F_ARTSTOCK = F_ARTSTOCKRepository.ReadReferenceDepot(F_ARTICLE.AR_Ref, Supply.Sag_Id);

                                Int32 QteSto = (Int32)((F_ARTSTOCK.AS_QteSto != null) ? F_ARTSTOCK.AS_QteSto.Value : 0);
                                Int32 QteCom = (Int32)((F_ARTSTOCK.AS_QteCom != null) ? F_ARTSTOCK.AS_QteCom.Value : 0);
                                Int32 QteRes = (Int32)((F_ARTSTOCK.AS_QteRes != null) ? F_ARTSTOCK.AS_QteRes.Value : 0);
                                Int32 QtePrepa = (Int32)((F_ARTSTOCK.AS_QtePrepa != null) ? F_ARTSTOCK.AS_QtePrepa.Value : 0);
                                Int32 QteAControler = (Int32)((F_ARTSTOCK.AS_QteAControler != null) ? F_ARTSTOCK.AS_QteAControler.Value : 0);

                                Int32 QteMini = (Int32)((F_ARTSTOCK.AS_QteMini != null) ? F_ARTSTOCK.AS_QteMini.Value : 0);
                                Int32 QteMaxi = (Int32)((F_ARTSTOCK.AS_QteMaxi != null) ? F_ARTSTOCK.AS_QteMaxi.Value : 0);

                                Int32 qty_temp = 0;

                                switch (Config.Con_Value)
                                {
                                    // stock à terme
                                    case "1":
                                        qty_temp = (Int32)(QteSto + QteCom - (QteRes + QtePrepa));
                                        break;
                                    // stock réel
                                    case "2":
                                        qty_temp = (Int32)QteSto;
                                        break;
                                    // stock disponible
                                    case "3":
                                        qty_temp = (Int32)(QteSto - (QtePrepa + QteAControler));
                                        break;
                                    // stock disponible avancé
                                    case "4":
                                        qty_temp = (Int32)(QteSto - (QteRes + QtePrepa + QteAControler));
                                        break;
                                }

                                // <JG> 05/10/2016
                                if (Core.Global.GetConfig().ArticleStockNegatifZeroParDepot && qty_temp < 0)
                                {
                                    qty_temp = 0;
                                }
                                Quantity += qty_temp;

                                if (Core.Global.GetConfig().ModuleAECStockActif)
                                {
                                    Product.AEC_Stock.Count_Supply += 1;
                                    Product.AEC_Stock.MinimalQuantity += QteMini;
                                    Product.AEC_Stock.MaximalQuantity += QteMaxi;
                                    Product.AEC_Stock.QuantityReal += QteSto;
                                    Product.AEC_Stock.QuantityFuture += (QteSto + QteCom - (QteRes + QtePrepa));
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (ConfigRepository.ExistName(Core.Global.ConfigArticleContremarque))
                    {
                        Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigArticleContremarque);
                        if (Core.Global.IsNumeric(Config.Con_Value))
                        {
                            Quantity = Convert.ToInt32(Config.Con_Value);
                        }
                    }
                }
                if (Core.Global.GetConfig().ArticleStockNegatifZero && Quantity < 0)
                    Quantity = 0;
                Product.Quantity = Quantity;
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
        public void ReadQuantity(Model.Sage.F_ARTICLE F_ARTICLE, Model.Prestashop.PsProductAttribute ProductAttribute)
        {
            Model.Prestashop.PsProduct Product = new Model.Prestashop.PsProduct();
            ReadQuantity(F_ARTICLE, Product);
            ProductAttribute.Quantity = Product.Quantity;
        }

        public Int32 ReadQuantityPack(Model.Sage.F_ARTICLE F_ARTICLE, Decimal QuantityPack)
        {
            Int32 Quantity = 0;
            try
            {
                Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                // if the F_ARTICLE is not Contremarque
                if (F_ARTICLE.AR_Contremarque == 0 || Core.Global.GetConfig().ArticleContremarqueStockActif)
                {
                    Model.Local.SupplyRepository SupplyRepository = new Model.Local.SupplyRepository();
                    List<Model.Local.Supply> ListSupply = SupplyRepository.ListActive(true);
                    Model.Sage.F_ARTSTOCKRepository F_ARTSTOCKRepository = new Model.Sage.F_ARTSTOCKRepository();

                    // if the ConfigArticleStock 
                    if (ConfigRepository.ExistName(Core.Global.ConfigArticleStock))
                    {
                        Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigArticleStock);
                        // foreach Supply, we add the stock at the quantity
                        foreach (Model.Local.Supply Supply in ListSupply)
                        {
                            if (F_ARTSTOCKRepository.ExistReferenceDepot(F_ARTICLE.AR_Ref, Supply.Sag_Id))
                            {
                                Model.Sage.F_ARTSTOCK F_ARTSTOCK = F_ARTSTOCKRepository.ReadReferenceDepot(F_ARTICLE.AR_Ref, Supply.Sag_Id);

                                Decimal QteSto = (F_ARTSTOCK.AS_QteSto != null) ? F_ARTSTOCK.AS_QteSto.Value : 0;
                                Decimal QteCom = (F_ARTSTOCK.AS_QteCom != null) ? F_ARTSTOCK.AS_QteCom.Value : 0;
                                Decimal QteRes = (F_ARTSTOCK.AS_QteRes != null) ? F_ARTSTOCK.AS_QteRes.Value : 0;
                                Decimal QtePrepa = (F_ARTSTOCK.AS_QtePrepa != null) ? F_ARTSTOCK.AS_QtePrepa.Value : 0;
                                Decimal QteAControler = (F_ARTSTOCK.AS_QteAControler != null) ? F_ARTSTOCK.AS_QteAControler.Value : 0;

                                Int32 qty_temp = 0;

                                // <JG> 05/10/2016 modification gestion division par la quantité nomenclature afin de gérer le cas où le diviseur est inférieur à 1
                                switch (Config.Con_Value)
                                {
                                    case "1":
                                        qty_temp = (Int32)((QteSto + QteCom - (QteRes + QtePrepa)) / QuantityPack);
                                        break;
                                    case "2":
                                        qty_temp = (Int32)(QteSto / QuantityPack);
                                        break;
                                    case "3":
                                        qty_temp = (Int32)((QteSto - (QtePrepa + QteAControler)) / QuantityPack);
                                        break;
                                    case "4":
                                        qty_temp = (Int32)((QteSto - (QteRes + QtePrepa + QteAControler)) / QuantityPack);
                                        break;
                                }

                                // <JG> 05/10/2016
                                if (Core.Global.GetConfig().ArticleStockNegatifZeroParDepot && qty_temp < 0)
                                {
                                    qty_temp = 0;
                                }
                                Quantity += qty_temp;
                            }
                        }
                    }
                }
                else
                {
                    if (ConfigRepository.ExistName(Core.Global.ConfigArticleContremarque))
                    {
                        Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigArticleContremarque);
                        if (Core.Global.IsNumeric(Config.Con_Value))
                        {
                            Quantity = (Int32)(Convert.ToInt32(Config.Con_Value) / QuantityPack);
                        }
                    }
                }
                if (QuantityPack > 0)
                {
                    // <JG> 09/06/2015 ajout arrondi inférieur forcé
                    //Quantity = (Int32)Math.Floor(Quantity / QuantityPack);
                    // <JG> 05/10/2016 modification gestion division par la quantité nomenclature afin de gérer le cas où le diviseur est inférieur à 1
                    Quantity = (Int32)Math.Floor((decimal)Quantity);
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
            return Quantity;
        }

        public void WriteStockAvailableProduct(Model.Prestashop.PsProduct Product)
        {
            Model.Prestashop.PsStockAvailableRepository PsStockAvailableRepository = new Model.Prestashop.PsStockAvailableRepository();
            Model.Prestashop.PsStockAvailable PsStockAvailable = new Model.Prestashop.PsStockAvailable();
            if (PsStockAvailableRepository.ExistProductAttributeShop(Product.IDProduct, 0, Global.CurrentShop.IDShop))
            {
                PsStockAvailable = PsStockAvailableRepository.ReadProductAttributeShop(Product.IDProduct, 0, Global.CurrentShop.IDShop);
                PsStockAvailable.Quantity = Product.Quantity;
                // <JG> 19/11/2014 correction ligne de stock ps>=1.6.0.9
                PsStockAvailable.IDShopGroup = 0;
                PsStockAvailableRepository.Save();
            }
            else
            {
                PsStockAvailable.IDProduct = Product.IDProduct;
                PsStockAvailable.IDShop = Global.CurrentShop.IDShop;
                // <JG> 19/11/2014 correction ligne de stock ps>=1.6.0.9
                PsStockAvailable.IDShopGroup = 0;
                PsStockAvailable.Quantity = Product.Quantity;
                PsStockAvailable.IDProductAttribute = 0;
                PsStockAvailable.OutOfStock = (byte)Product.OutOfStock;
                PsStockAvailableRepository.Add(PsStockAvailable);
            }

            if (Core.Global.GetConfig().ModuleAECStockActif && Core.Global.ExistAECStockModule())
            {
                WriteAdvancedStock(Product, PsStockAvailable.IDStockAvailable);
            }
        }
        public void WriteStockAvailableProductAttribute(Model.Prestashop.PsProduct PsProduct, Model.Prestashop.PsProductAttribute PsProductAttribute)
        {
            Model.Prestashop.PsStockAvailableRepository PsStockAvailableRepository = new Model.Prestashop.PsStockAvailableRepository();
            Model.Prestashop.PsStockAvailable PsStockAvailable = new Model.Prestashop.PsStockAvailable();
            if (PsStockAvailableRepository.ExistProductAttributeShop(PsProductAttribute.IDProduct, PsProductAttribute.IDProductAttribute, Global.CurrentShop.IDShop))
            {
                PsStockAvailable = PsStockAvailableRepository.ReadProductAttributeShop(PsProductAttribute.IDProduct, PsProductAttribute.IDProductAttribute, Global.CurrentShop.IDShop);
                PsStockAvailable.Quantity = PsProductAttribute.Quantity;
                // <JG> 19/11/2014 correction ligne de stock ps>=1.6.0.9
                PsStockAvailable.IDShopGroup = 0;
                PsStockAvailableRepository.Save();
            }
            else
            {
                PsStockAvailable.IDProduct = PsProductAttribute.IDProduct;
                PsStockAvailable.IDShop = Global.CurrentShop.IDShop;
                // <JG> 19/11/2014 correction ligne de stock ps>=1.6.0.9
                PsStockAvailable.IDShopGroup = 0;
                PsStockAvailable.Quantity = PsProductAttribute.Quantity;
                PsStockAvailable.IDProductAttribute = PsProductAttribute.IDProductAttribute;
                PsStockAvailable.OutOfStock = (byte)PsProduct.OutOfStock;
                PsStockAvailableRepository.Add(PsStockAvailable);
            }

            if (Core.Global.GetConfig().ModuleAECStockActif)
            {
                WriteAdvancedStock(PsProductAttribute, PsStockAvailable.IDStockAvailable);
            }
        }

        #endregion

        #region Tarifs/Remises

        public Model.Sage.F_TAXE ReadTaxe(Model.Sage.F_ARTICLE F_ARTICLE, Model.Prestashop.PsProduct Product, Int32 CatCompta)
        {
            Product.taxe_famillesage = false;
            Model.Sage.F_TAXE TaxeTVA = new Model.Sage.F_TAXE();
            try
            {
                Boolean isTaxe = false;
                if (CatCompta != 0)
                {
                    Model.Sage.F_ARTCOMPTARepository F_ARTCOMPTARepository = new Model.Sage.F_ARTCOMPTARepository();
                    if (F_ARTCOMPTARepository.ExistReferenceChampType(F_ARTICLE.AR_Ref, CatCompta, 0))
                    {
                        Model.Sage.F_ARTCOMPTA F_ARTCOMPTA = F_ARTCOMPTARepository.ReadReferenceChampType(F_ARTICLE.AR_Ref, CatCompta, 0);
                        Model.Sage.F_TAXERepository F_TAXERepository = new Model.Sage.F_TAXERepository();

                        switch (Core.Global.GetConfig().TaxSageTVA)
                        {
                            case TaxSage.CodeTaxe1:
                                if (F_ARTCOMPTA.ACP_ComptaCPT_Taxe1 != null && F_TAXERepository.ExistCode(F_ARTCOMPTA.ACP_ComptaCPT_Taxe1))
                                {
                                    TaxeTVA = F_TAXERepository.ReadCode(F_ARTCOMPTA.ACP_ComptaCPT_Taxe1);
                                    isTaxe = true;
                                }
                                break;
                            case TaxSage.CodeTaxe2:
                                if (F_ARTCOMPTA.ACP_ComptaCPT_Taxe2 != null && F_TAXERepository.ExistCode(F_ARTCOMPTA.ACP_ComptaCPT_Taxe2))
                                {
                                    TaxeTVA = F_TAXERepository.ReadCode(F_ARTCOMPTA.ACP_ComptaCPT_Taxe2);
                                    isTaxe = true;
                                }
                                break;
                            case TaxSage.CodeTaxe3:
                                if (F_ARTCOMPTA.ACP_ComptaCPT_Taxe3 != null && F_TAXERepository.ExistCode(F_ARTCOMPTA.ACP_ComptaCPT_Taxe3))
                                {
                                    TaxeTVA = F_TAXERepository.ReadCode(F_ARTCOMPTA.ACP_ComptaCPT_Taxe3);
                                    isTaxe = true;
                                }
                                break;
                            case TaxSage.Empty:
                            default:
                                break;
                        }
                    }
                    if (isTaxe == false)
                    {
                        Model.Sage.F_FAMCOMPTARepository F_FAMCOMPTARepository = new Model.Sage.F_FAMCOMPTARepository();
                        if (F_FAMCOMPTARepository.ExistFamilleChampType(F_ARTICLE.FA_CodeFamille, CatCompta, 0))
                        {
                            Model.Sage.F_FAMCOMPTA F_FAMCOMPTA = F_FAMCOMPTARepository.ReadFamilleChampType(F_ARTICLE.FA_CodeFamille, CatCompta, 0);
                            Product.taxe_famillesage = true;
                            Model.Sage.F_TAXERepository F_TAXERepository = new Model.Sage.F_TAXERepository();

                            switch (Core.Global.GetConfig().TaxSageTVA)
                            {
                                case TaxSage.CodeTaxe1:
                                    if (F_FAMCOMPTA.FCP_ComptaCPT_Taxe1 != null && F_TAXERepository.ExistCode(F_FAMCOMPTA.FCP_ComptaCPT_Taxe1))
                                    {
                                        TaxeTVA = F_TAXERepository.ReadCode(F_FAMCOMPTA.FCP_ComptaCPT_Taxe1);
                                    }
                                    break;
                                case TaxSage.CodeTaxe2:
                                    if (F_FAMCOMPTA.FCP_ComptaCPT_Taxe2 != null && F_TAXERepository.ExistCode(F_FAMCOMPTA.FCP_ComptaCPT_Taxe2))
                                    {
                                        TaxeTVA = F_TAXERepository.ReadCode(F_FAMCOMPTA.FCP_ComptaCPT_Taxe2);
                                    }
                                    break;
                                case TaxSage.CodeTaxe3:
                                    if (F_FAMCOMPTA.FCP_ComptaCPT_Taxe3 != null && F_TAXERepository.ExistCode(F_FAMCOMPTA.FCP_ComptaCPT_Taxe3))
                                    {
                                        TaxeTVA = F_TAXERepository.ReadCode(F_FAMCOMPTA.FCP_ComptaCPT_Taxe3);
                                    }
                                    break;
                                case TaxSage.Empty:
                                default:
                                    break;
                            }
                        }
                    }
                }
                Model.Local.TaxRepository TaxRepository = new Model.Local.TaxRepository();
                if (TaxRepository.ExistSage(TaxeTVA.cbMarq))
                {
                    Model.Local.Tax Tax = TaxRepository.ReadSage(TaxeTVA.cbMarq);
                    Product.IDTaxRulesGroup = Convert.ToUInt32(Tax.Pre_Id);
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
            return TaxeTVA;
        }
        public Model.Sage.F_TAXE ReadEcoTaxe(Model.Sage.F_ARTICLE F_ARTICLE, Model.Prestashop.PsProduct Product, Model.Sage.F_TAXE TaxeTVA, Int32 CatCompta)
        {
            Model.Sage.F_TAXE TaxeEco = new Model.Sage.F_TAXE();
            try
            {
                Product.EcOtAx = 0;
                Boolean isTaxe = false;
                if (CatCompta != 0)
                {
                    Model.Sage.F_ARTCOMPTARepository F_ARTCOMPTARepository = new Model.Sage.F_ARTCOMPTARepository();
                    if (F_ARTCOMPTARepository.ExistReferenceChampType(F_ARTICLE.AR_Ref, CatCompta, 0))
                    {
                        Model.Sage.F_ARTCOMPTA F_ARTCOMPTA = F_ARTCOMPTARepository.ReadReferenceChampType(F_ARTICLE.AR_Ref, CatCompta, 0);
                        Model.Sage.F_TAXERepository F_TAXERepository = new Model.Sage.F_TAXERepository();

                        switch (Core.Global.GetConfig().TaxSageEco)
                        {
                            case TaxSage.CodeTaxe1:
                                if (!string.IsNullOrEmpty(F_ARTCOMPTA.ACP_ComptaCPT_Taxe1) && F_TAXERepository.ExistCode(F_ARTCOMPTA.ACP_ComptaCPT_Taxe1))
                                {
                                    TaxeEco = F_TAXERepository.ReadCode(F_ARTCOMPTA.ACP_ComptaCPT_Taxe1);
                                    isTaxe = true;
                                }
                                break;
                            case TaxSage.CodeTaxe2:
                                if (!string.IsNullOrEmpty(F_ARTCOMPTA.ACP_ComptaCPT_Taxe2) && F_TAXERepository.ExistCode(F_ARTCOMPTA.ACP_ComptaCPT_Taxe2))
                                {
                                    TaxeEco = F_TAXERepository.ReadCode(F_ARTCOMPTA.ACP_ComptaCPT_Taxe2);
                                    isTaxe = true;
                                }
                                break;
                            case TaxSage.CodeTaxe3:
                                if (!string.IsNullOrEmpty(F_ARTCOMPTA.ACP_ComptaCPT_Taxe3) && F_TAXERepository.ExistCode(F_ARTCOMPTA.ACP_ComptaCPT_Taxe3))
                                {
                                    TaxeEco = F_TAXERepository.ReadCode(F_ARTCOMPTA.ACP_ComptaCPT_Taxe3);
                                    isTaxe = true;
                                }
                                break;
                            case TaxSage.Empty:
                            default:
                                break;
                        }
                    }
                    if (isTaxe == false && Product.taxe_famillesage)
                    {
                        Model.Sage.F_FAMCOMPTARepository F_FAMCOMPTARepository = new Model.Sage.F_FAMCOMPTARepository();
                        if (F_FAMCOMPTARepository.ExistFamilleChampType(F_ARTICLE.FA_CodeFamille, CatCompta, 0))
                        {
                            Model.Sage.F_FAMCOMPTA F_FAMCOMPTA = F_FAMCOMPTARepository.ReadFamilleChampType(F_ARTICLE.FA_CodeFamille, CatCompta, 0);
                            Model.Sage.F_TAXERepository F_TAXERepository = new Model.Sage.F_TAXERepository();

                            switch (Core.Global.GetConfig().TaxSageEco)
                            {
                                case TaxSage.CodeTaxe1:
                                    if (!string.IsNullOrEmpty(F_FAMCOMPTA.FCP_ComptaCPT_Taxe1) && F_TAXERepository.ExistCode(F_FAMCOMPTA.FCP_ComptaCPT_Taxe1))
                                    {
                                        TaxeEco = F_TAXERepository.ReadCode(F_FAMCOMPTA.FCP_ComptaCPT_Taxe1);
                                    }
                                    break;
                                case TaxSage.CodeTaxe2:
                                    if (!string.IsNullOrEmpty(F_FAMCOMPTA.FCP_ComptaCPT_Taxe2) && F_TAXERepository.ExistCode(F_FAMCOMPTA.FCP_ComptaCPT_Taxe2))
                                    {
                                        TaxeEco = F_TAXERepository.ReadCode(F_FAMCOMPTA.FCP_ComptaCPT_Taxe2);
                                    }
                                    break;
                                case TaxSage.CodeTaxe3:
                                    if (!string.IsNullOrEmpty(F_FAMCOMPTA.FCP_ComptaCPT_Taxe3) && F_TAXERepository.ExistCode(F_FAMCOMPTA.FCP_ComptaCPT_Taxe3))
                                    {
                                        TaxeEco = F_TAXERepository.ReadCode(F_FAMCOMPTA.FCP_ComptaCPT_Taxe3);
                                    }
                                    break;
                                case TaxSage.Empty:
                                default:
                                    break;
                            }
                        }
                    }
                    if (!String.IsNullOrEmpty(TaxeEco.TA_Code) && TaxeEco.TA_Taux != null)
                    {
                        switch (TaxeEco.TA_Type)
                        {
                            // <JG> 14/02/2017 correction gestion eco-taxe non perçue
                            // Dans Sage si non perçue est cochée, la base HT inclut le montant d'eco-taxe, mais le montant TTC n'inclut que la TVA sur l'eco-taxe et pas le montant HT lui-même
                            // produit à 100€ HT avec 1€ HT d'eco-part
                            // TOTAL HT = 100 
                            // BASE TVA = 101
                            // TOTAL TVA = 20.20
                            // TOTAL TTC 
                            // si percue == 121.20  // (TA_NP = 0 // traduire non "non perçue" du strucfic) //
                            // si non percue == 120.20 // (TA_NP = 1)
                            case (short)ABSTRACTION_SAGE.F_TAXE.Obj._Enum_TA_Type.TP_HT:
                                //if (TaxeTVA.TA_NP != null && TaxeTVA.TA_NP == 1)
                                {
                                    Product.ecotaxe_htsage = TaxeEco.TA_Taux.Value;
                                    Product.EcOtAx = TaxeEco.TA_Taux.Value;
                                }
                                //else
                                //{
                                //    Product.ecotaxe_htsage = TaxeEco.TA_Taux.Value;
                                //    Product.EcOtAx = TaxeEco.TA_Taux.Value * (1 + (TaxeTVA.TA_Taux.Value / 100));
                                //}
                                break;
                            case (short)ABSTRACTION_SAGE.F_TAXE.Obj._Enum_TA_Type.TP_TTC:
                                //if (TaxeTVA.TA_NP != null && TaxeTVA.TA_NP == 1)
                                {
                                    Product.EcOtAx = TaxeEco.TA_Taux.Value / (1 + (TaxeTVA.TA_Taux.Value / 100));
                                    Product.ecotaxe_htsage = TaxeEco.TA_Taux.Value / (1 + (TaxeTVA.TA_Taux.Value / 100));
                                }
                                //else
                                //{
                                //    Product.EcOtAx = TaxeEco.TA_Taux.Value;
                                //    Product.ecotaxe_htsage = TaxeEco.TA_Taux.Value / (1 + (TaxeTVA.TA_Taux.Value / 100));
                                //}
                                break;
                            case (short)ABSTRACTION_SAGE.F_TAXE.Obj._Enum_TA_Type.TP_Poids:
                                if (F_ARTICLE.AR_PoidsNet != null)
                                {
                                    Product.EcOtAx = TaxeEco.TA_Taux.Value * F_ARTICLE.AR_PoidsNet.Value;
                                    Product.ecotaxe_htsage = Product.EcOtAx / (1 + (TaxeTVA.TA_Taux.Value / 100));
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
            return TaxeEco;
        }

        // <JG> 18/02/2013 correction application taxe sur les produits
        public void ReadPrice(Model.Sage.F_ARTICLE F_ARTICLE, Model.Prestashop.PsProduct Product, Model.Sage.F_TAXE TaxeTVA)
        {
            try
            {
                Model.Local.TaxRepository TaxRepository = new Model.Local.TaxRepository();
                Model.Prestashop.PsTaxRepository PsTaxRepository = new Model.Prestashop.PsTaxRepository();
				Model.Prestashop.PsTaxRuleRepository PsTaxRuleRepository = new Model.Prestashop.PsTaxRuleRepository();

				Model.Prestashop.PsLangRepository PsLangRepository = new Model.Prestashop.PsLangRepository();
				Model.Prestashop.PsLang PsLang = PsLangRepository.ReadId(Core.Global.Lang);
				Model.Prestashop.PsCountryRepository PsCountryRepository = new Model.Prestashop.PsCountryRepository();
				
				Model.Prestashop.PsCountry PsCountry = (PsCountryRepository.ExistIsoCode(PsLang.IsoCode)? PsCountryRepository.ReadIsoCode(PsLang.IsoCode) :null);
				// Correction - prise en compte de la règle de taxe
				Model.Local.Tax Tax = TaxRepository.ReadSage(TaxeTVA.cbMarq);
				Model.Prestashop.PsTaxRule PsTaxRule = (Tax == null) ? null : PsTaxRuleRepository.ReadTaxesRulesGroupCountry(Tax.Pre_Id, (Int32)PsCountry.IDCountry);
				Model.Prestashop.PsTax PsTax = (Tax == null) ? null : PsTaxRepository.ReadTax((UInt32)PsTaxRule.IDTax);

                if (PsTaxRule != null)
                {
                    Product.IDTaxRulesGroup = (UInt32)Tax.Pre_Id;
                }

                Boolean isTTC = false;
                Model.Sage.F_ARTCLIENT F_ARTCLIENT = null;
                Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                if (ConfigRepository.ExistName(Core.Global.ConfigArticleCatTarif))
                {
                    Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigArticleCatTarif);

                    Model.Sage.F_ARTCLIENTRepository F_ARTCLIENTRepository = new Model.Sage.F_ARTCLIENTRepository();
                    if (F_ARTCLIENTRepository.ExistReferenceCategorie(F_ARTICLE.AR_Ref, Convert.ToInt32(Config.Con_Value)))
                    {
                        F_ARTCLIENT = F_ARTCLIENTRepository.ReadReferenceCategorie(F_ARTICLE.AR_Ref, Convert.ToInt32(Config.Con_Value));
                        isTTC = (F_ARTCLIENT.AC_PrixTTC == 1);
                    }
                }

                Product.Price = GetHTPrice(F_ARTICLE, F_ARTCLIENT, PsTax, Product);
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
        public void ReadPrice(Model.Sage.F_ARTICLE F_ARTICLE, Model.Prestashop.PsProductAttribute ProductAttribute, Model.Sage.F_TAXE TaxeTva)
        {
            Model.Prestashop.PsProduct Product = new Model.Prestashop.PsProduct();
            ReadPrice(F_ARTICLE, Product, TaxeTva);
            ProductAttribute.Price = Product.Price;
        }

        public Decimal GetHTPrice(Model.Sage.F_ARTICLE F_ARTICLE, Model.Sage.F_ARTCLIENT categorieArticle, Model.Prestashop.PsTax PsTax, Model.Prestashop.PsProduct Product)
        {
            decimal price = 0;

            if (categorieArticle != null)
            {
                //Recherche du prix HT de base sur la catégorie tarifaire de l'article
                price = (categorieArticle.AC_PrixVen != null) ? categorieArticle.AC_PrixVen.Value : 0;

                if (price == 0
                    && categorieArticle.AC_Coef != null && categorieArticle.AC_Coef.Value > 0
                    && F_ARTICLE.AR_PrixAch != null && F_ARTICLE.AR_PrixAch > 0)
                    price = (F_ARTICLE.AR_PrixAch.Value * categorieArticle.AC_Coef.Value);

                // gestion prix devise
                if (price == 0
                    && categorieArticle.AC_PrixDev != null && categorieArticle.AC_PrixDev != 0)
                {
                    price = categorieArticle.AC_PrixDev.Value;
                }

                if (price > 0 && categorieArticle.AC_PrixTTC == 1 && PsTax != null)
                    price = (price / (1 + (PsTax.Rate / 100))) - Product.ecotaxe_htsage;
            }

            //Recherche du prix HT de base sur la fiche article
            if (price == 0)
            {
                price = (F_ARTICLE.AR_PrixVen != null) ? F_ARTICLE.AR_PrixVen.Value : 0;

                if (price == 0
                     && F_ARTICLE.AR_Coef != null && F_ARTICLE.AR_Coef.Value > 0
                     && F_ARTICLE.AR_PrixAch != null && F_ARTICLE.AR_PrixAch > 0)
                    price = (F_ARTICLE.AR_PrixAch.Value * F_ARTICLE.AR_Coef.Value);

                if (price > 0 && F_ARTICLE.AR_PrixTTC == 1 && PsTax != null)
                    price = (price / (1 + (PsTax.Rate / 100))) - Product.ecotaxe_htsage;

            }
            return price;
        }

        // <JG> 19/02/2013 Ajout gestion recalcul HT si prix TTC
        public Dictionary<int, decimal> GetUnitPrices(Model.Sage.F_ARTICLE F_ARTICLE, Model.Local.Article Article, Model.Local.CompositionArticle CompositionArticle, string categorie, decimal defaultPrice, Boolean isTTC, Model.Prestashop.PsTax PsTax, decimal ecotaxe_HT)
        {
            Dictionary<int, decimal> enumeres = new Dictionary<int, decimal>();

            if (defaultPrice > 0)
                enumeres.Add(0, defaultPrice);

            #region Composition
            if (Article.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleComposition && CompositionArticle != null)
            {
                if (CompositionArticle.EnumereF_ARTENUMREF.cbMarq != 0)
                {
                    Model.Sage.F_TARIFGAMRepository F_TARIFGAMRepository = new Model.Sage.F_TARIFGAMRepository();
                    Model.Sage.F_TARIFGAM F_TARIFGAM = F_TARIFGAMRepository.ReadReferenceCategorieGamme1Gamme2(F_ARTICLE.AR_Ref, categorie,
                        CompositionArticle.EnumereGamme1.AG_No.Value, CompositionArticle.EnumereGamme2.AG_No.Value);

                    if (F_TARIFGAM != null && F_TARIFGAM.TG_Prix.HasValue)
                    {
                        if (isTTC && PsTax != null)
                            F_TARIFGAM.TG_Prix = (F_TARIFGAM.TG_Prix.Value / (1 + (PsTax.Rate / 100))) - ecotaxe_HT;
                    }
                    else if (defaultPrice > 0)
                        F_TARIFGAM = new Model.Sage.F_TARIFGAM()
                         {
                             AG_No1 = CompositionArticle.EnumereGamme1.AG_No.Value,
                             AG_No2 = CompositionArticle.EnumereGamme2.AG_No.Value,
                             TG_Prix = defaultPrice,
                         };

                    if (F_TARIFGAM != null && (F_TARIFGAM.TG_Prix.Value > 0 || defaultPrice > 0))
                        enumeres.Add(CompositionArticle.Pre_Id.Value, (F_TARIFGAM.TG_Prix.Value > 0) ? F_TARIFGAM.TG_Prix.Value : defaultPrice);
                }
                else if (CompositionArticle.EnumereF_CONDITION.cbMarq != 0)
                {
                    Decimal Price = 0;

                    Model.Sage.F_TARIFCOND F_TARIFCOND = null;
                    Model.Sage.F_TARIFCONDRepository F_TARIFCONDRepository = new Model.Sage.F_TARIFCONDRepository();
                    if (F_TARIFCONDRepository.ExistReferenceCategorieConditionnement(F_ARTICLE.AR_Ref, categorie, (int)CompositionArticle.EnumereF_CONDITION.CO_No))
                    {
                        F_TARIFCOND = F_TARIFCONDRepository.ReadReferenceCategorieConditionnement(F_ARTICLE.AR_Ref, categorie, (int)CompositionArticle.EnumereF_CONDITION.CO_No);
                        Price = F_TARIFCOND.TC_Prix.Value;
                    }
                    else if (CompositionArticle.EnumereF_CONDITION.EC_Quantite != null)
                    {
                        Price = (decimal)CompositionArticle.EnumereF_CONDITION.EC_Quantite * defaultPrice;
                        isTTC = false; // Product.Price est déjà HT
                    }

                    if (Price > 0 && isTTC && PsTax != null)
                        Price = (Price / (1 + (PsTax.Rate / 100))) - ecotaxe_HT;

                    if (CompositionArticle.Pre_Id.HasValue)
                        if (Price > 0 || defaultPrice > 0)
                            enumeres.Add(CompositionArticle.Pre_Id.Value, (Price > 0) ? Price : defaultPrice);
                }
            }
            #endregion
            else
            {
                #region Gammes
                if (F_ARTICLE.AR_Gamme1 != null && F_ARTICLE.AR_Gamme1 != 0)
                {
                    Model.Local.AttributeArticleRepository AttributeArticleRepository = new Model.Local.AttributeArticleRepository();
                    Model.Local.AttributeRepository AttributeRepository = new Model.Local.AttributeRepository();
                    Model.Sage.F_TARIFGAMRepository F_TARIFGAMRepository = new Model.Sage.F_TARIFGAMRepository();
                    Model.Sage.F_ARTGAMMERepository F_ARTGAMMERepository = new Model.Sage.F_ARTGAMMERepository();

                    //Récupération des énumérés de gamme mappés
                    List<Model.Local.AttributeArticle> declinaisons = new List<Model.Local.AttributeArticle>();

                    declinaisons.AddRange(AttributeArticleRepository.ListArticleSync(Article.Art_Id, true).Where(d => d.Pre_Id != null && d.Pre_Id != 0));

                    //foreach (Model.Local.AttributeArticle declinaison in AttributeArticleRepository.ListArticleSync(ArticleSend, true))
                    //    if (declinaison.Pre_Id != null && declinaison.Pre_Id > 0)
                    //        declinaisons.Add(declinaison);

                    //Récupération des gammes
                    List<Model.Sage.F_ARTGAMME> enumeresgamme1 = new List<Model.Sage.F_ARTGAMME>();
                    List<Model.Sage.F_ARTGAMME> enumeresgamme2 = new List<Model.Sage.F_ARTGAMME>();

                    //Récupération des tarifs par gammes
                    List<Model.Sage.F_TARIFGAM> tarifs = new List<Model.Sage.F_TARIFGAM>();

                    foreach (var conditionnement1 in F_ARTGAMMERepository.ListArticleType(F_ARTICLE.AR_Ref, 0))
                        enumeresgamme1.Add(conditionnement1);

                    //Parcours les énumérés de la 1ere gamme
                    foreach (var enumere1 in enumeresgamme1)
                    {
                        enumeresgamme2.Clear();

                        foreach (var conditionnement2 in F_ARTGAMMERepository.ListArticleType(F_ARTICLE.AR_Ref, 1))
                            enumeresgamme2.Add(conditionnement2);

                        if (enumeresgamme2.Count > 0)
                        {
                            //Parcours les énumérés de la 2eme gamme
                            foreach (var enumere2 in enumeresgamme2)
                            {
                                Model.Sage.F_TARIFGAM F_TARIFGAM = F_TARIFGAMRepository.ReadReferenceCategorieGamme1Gamme2(
                                    F_ARTICLE.AR_Ref, categorie, (int)enumere1.AG_No, (int)enumere2.AG_No);

                                //  <JG> 19/02/2013 Ajout gestion recalcul HT si prix TTC
                                if (F_TARIFGAM != null && F_TARIFGAM.TG_Prix.HasValue)
                                {
                                    if (isTTC && PsTax != null)
                                        F_TARIFGAM.TG_Prix = (F_TARIFGAM.TG_Prix.Value / (1 + (PsTax.Rate / 100))) - ecotaxe_HT;
                                    tarifs.Add(F_TARIFGAM);
                                }
                                else if (defaultPrice > 0)
                                    tarifs.Add(new Model.Sage.F_TARIFGAM()
                                    {
                                        AG_No1 = enumere1.AG_No,
                                        AG_No2 = enumere2.AG_No,
                                        TG_Prix = defaultPrice,
                                    });
                            }
                        }
                        else
                        {
                            Model.Sage.F_TARIFGAM F_TARIFGAM = F_TARIFGAMRepository.ReadReferenceCategorieGamme1Gamme2(
                                    F_ARTICLE.AR_Ref, categorie, (int)enumere1.AG_No, 0);

                            //  <JG> 19/02/2013 Ajout gestion recalcul HT si prix TTC
                            if (F_TARIFGAM != null && F_TARIFGAM.TG_Prix.HasValue)
                            {
                                if (isTTC && PsTax != null)
                                    F_TARIFGAM.TG_Prix = (F_TARIFGAM.TG_Prix.Value / (1 + (PsTax.Rate / 100))) - ecotaxe_HT;
                                tarifs.Add(F_TARIFGAM);
                            }
                            else if (defaultPrice > 0)
                                tarifs.Add(new Model.Sage.F_TARIFGAM()
                                {
                                    AG_No1 = enumere1.AG_No,
                                    AG_No2 = 0,
                                    TG_Prix = defaultPrice,
                                });
                        }
                    }

                    //Récupération des prix des énumérés de gamme
                    foreach (Model.Local.AttributeArticle declinaison in declinaisons)
                    {
                        Model.Sage.F_ARTENUMREF F_ARTENUMREF = new Model.Sage.F_ARTENUMREFRepository().Read(declinaison.Sag_Id);
                        if (F_ARTENUMREF != null)
                        {
                            Model.Sage.F_TARIFGAM F_TARIFGAM = tarifs.FirstOrDefault(result =>
                                result.AG_No1 == F_ARTENUMREF.AG_No1 && result.AG_No2 == F_ARTENUMREF.AG_No2);

                            if (F_TARIFGAM != null && (F_TARIFGAM.TG_Prix.Value > 0 || defaultPrice > 0))
                                enumeres.Add(declinaison.Pre_Id.Value, (F_TARIFGAM.TG_Prix.Value > 0) ? F_TARIFGAM.TG_Prix.Value : defaultPrice);
                        }
                    }
                }
                #endregion

                #region Conditionnements
                if (F_ARTICLE.AR_Condition != null && F_ARTICLE.AR_Condition != 0)
                {
                    Model.Local.ConditioningArticleRepository ConditioningArticleRepository = new Model.Local.ConditioningArticleRepository();
                    foreach (Model.Local.ConditioningArticle ConditioningArticle in ConditioningArticleRepository.ListArticle(Article.Art_Id))
                    {
                        Decimal Price = 0;

                        Model.Sage.F_TARIFCOND F_TARIFCOND = null;
                        Model.Sage.F_TARIFCONDRepository F_TARIFCONDRepository = new Model.Sage.F_TARIFCONDRepository();
                        if (F_TARIFCONDRepository.ExistReferenceCategorieConditionnement(F_ARTICLE.AR_Ref, categorie, (int)ConditioningArticle.EnumereF_CONDITION.CO_No))
                        {
                            F_TARIFCOND = F_TARIFCONDRepository.ReadReferenceCategorieConditionnement(F_ARTICLE.AR_Ref, categorie, (int)ConditioningArticle.EnumereF_CONDITION.CO_No);
                            Price = F_TARIFCOND.TC_Prix.Value;
                        }
                        else if (ConditioningArticle.EnumereF_CONDITION.EC_Quantite != null)
                        {
                            Price = (decimal)ConditioningArticle.EnumereF_CONDITION.EC_Quantite * defaultPrice;
                            isTTC = false; // Product.Price est déjà HT
                        }

                        if (Price > 0 && isTTC && PsTax != null)
                            Price = (Price / (1 + (PsTax.Rate / 100))) - ecotaxe_HT;

                        if (ConditioningArticle.Pre_Id.HasValue)
                            if (Price > 0 || defaultPrice > 0)
                                enumeres.Add(ConditioningArticle.Pre_Id.Value, (Price > 0) ? Price : defaultPrice);
                    }
                }
                #endregion
            }

            return enumeres;
        }

        public void ExecSpecificPrice(Model.Sage.F_ARTICLE F_ARTICLE, Model.Prestashop.PsProduct Product, Model.Local.Article Article, Model.Local.CompositionArticle CompositionArticle, Model.Sage.F_TAXE TaxeTVA, Model.Sage.F_TAXE TaxeEco, out List<String> log_chrono)
        {
            log_chrono = new List<string>();
            try
            {
                Int32 ArticleSend = Article.Art_Id;

                DateTime start = DateTime.UtcNow;
                DateTime inter = DateTime.UtcNow;
                if (Core.Global.GetConfig().ChronoSynchroStockPriceActif)
                    log_chrono.Add("201-ExecSpecificPrice;" + F_ARTICLE.AR_Ref + ";" + (DateTime.UtcNow - start).ToString());

                //Core.Temp.LoadListesClients();

                //if ((F_ARTICLE.AR_PrixVen.HasValue && F_ARTICLE.AR_PrixVen.Value > 0) ||
                //    (new Model.Prestashop.PsProductAttributeRepository().List(Product.IDProduct).Count(a => a.Price > 0) > 0))
                {
                    long precedent = 1;
                    decimal price = 0;
                    decimal netPrice = 0;
                    decimal precedentNetPrice = 0;
                    decimal remise = 0;
                    int remiseIndex = 0;
                    group_key groupKey = new group_key(0, 0, 0, 0, 0);
                    client_key clientKey = new client_key(0, 0, 0, 0, 0, 0);

                    Model.Local.GroupRepository GroupRepository = new Model.Local.GroupRepository();
                    Model.Local.CustomerRepository CustomerRepository = new Model.Local.CustomerRepository();
                    Model.Sage.F_ARTCLIENTRepository F_ARTCLIENTRepository = new Model.Sage.F_ARTCLIENTRepository();
                    Model.Sage.F_TARIFQTERepository F_TARIFQTERepository = new Model.Sage.F_TARIFQTERepository();
                    Model.Sage.F_COMPTETRepository F_COMPTETRepository = new Model.Sage.F_COMPTETRepository();
                    Model.Sage.F_FAMILLERepository F_FAMILLERepository = new Model.Sage.F_FAMILLERepository();
                    Model.Sage.F_FAMTARIFRepository F_FAMTARIFRepository = new Model.Sage.F_FAMTARIFRepository();
                    Model.Sage.F_FAMCLIENTRepository F_FAMCLIENTRepository = new Model.Sage.F_FAMCLIENTRepository();
                    Model.Sage.F_FAMTARIFQTERepository F_FAMTARIFQTERepository = new Model.Sage.F_FAMTARIFQTERepository();
                    Model.Prestashop.PsSpecificPriceRepository PsSpecificPriceRepository = new Model.Prestashop.PsSpecificPriceRepository();
                    Model.Prestashop.PsCustomerRepository PsCustomerRepository = new Model.Prestashop.PsCustomerRepository();
                    Model.Prestashop.PsGroupRepository PsGroupRepository = new Model.Prestashop.PsGroupRepository();
                    Model.Local.TaxRepository TaxRepository = new Model.Local.TaxRepository();
                    Model.Prestashop.PsTaxRepository PsTaxRepository = new Model.Prestashop.PsTaxRepository();
					Model.Prestashop.PsTaxRuleRepository PsTaxRuleRepository = new Model.Prestashop.PsTaxRuleRepository();
					Model.Prestashop.PsLangRepository PsLangRepository = new Model.Prestashop.PsLangRepository();
					Model.Prestashop.PsLang PsLang = PsLangRepository.ReadId(Core.Global.Lang);
					Model.Prestashop.PsCountryRepository PsCountryRepository = new Model.Prestashop.PsCountryRepository();

					List<client_key> prixSpecifiqueClients = new List<client_key>();
                    List<group_key> prixSpecifiqueGroupes = new List<group_key>();

                    //Va déterminer les clients qui ont un tarif d'exception pour l'article en cours
                    List<int> rateExceptionClients = new List<int>();
                    //Va déterminer les clients qui ont des remises de type prix net
                    List<int> netPriceDiscountClients = new List<int>();
                    //Liste les clients possédant une remise par famille
                    List<int> rateFamilleClients = new List<int>();
                    //Liste des catégories ayant une remise article
                    List<int> rateCategoryArticle = new List<int>();
                    //Liste les clients ayant une remise sur l'article
                    List<int> rateClientArticle = new List<int>();
                    //Liste de catégories avec prix net
                    List<int> netPriceCategory = new List<int>();
                    // Liste des clients en Hors-Remise
                    List<int> offDiscountClients = new List<int>();
                    // Liste des catégories en Hors-Remise
                    List<int> offDiscountCategoryArticle = new List<int>();
                    List<int> offDiscountCategoryFamille = new List<int>();
					
					if (Core.Global.GetConfig().ChronoSynchroStockPriceActif)
                        log_chrono.Add("202-InitVar;" + F_ARTICLE.AR_Ref + ";" + (DateTime.UtcNow - inter).ToString()); inter = DateTime.UtcNow;

					//Récupère les taxes de l'article
					//Model.Local.Tax Tax = TaxRepository.ReadSage(TaxeTVA.cbMarq);
					//Model.Prestashop.PsTax PsTax = (Tax == null) ? null : PsTaxRepository.ReadTax((UInt32)Tax.Pre_Id);
					Model.Prestashop.PsCountry PsCountry = (PsCountryRepository.ExistIsoCode(PsLang.IsoCode) ? PsCountryRepository.ReadIsoCode(PsLang.IsoCode) : null);
					// Correction - prise en compte de la règle de taxe
					Model.Local.Tax Tax = TaxRepository.ReadSage(TaxeTVA.cbMarq);
					Model.Prestashop.PsTaxRule PsTaxRule = (Tax == null) ? null : PsTaxRuleRepository.ReadTaxesRulesGroupCountry(Tax.Pre_Id, (Int32)PsCountry.IDCountry);
					Model.Prestashop.PsTax PsTax = (Tax == null || PsTaxRule == null) ? null : PsTaxRepository.ReadTax((UInt32)PsTaxRule.IDTax);

					if (Core.Global.GetConfig().ChronoSynchroStockPriceActif)
                        log_chrono.Add("203-ReadTax;" + F_ARTICLE.AR_Ref + ";" + (DateTime.UtcNow - inter).ToString()); inter = DateTime.UtcNow;

                    RemiseMode mode = Global.GetConfig().ModeRemise;

                    //PsSpecificPriceRepository.WriteAvailableForOrder(0, Product.IDProduct);

                    #region Déclaration variables

                    Model.Sage.F_COMPTET_Light F_COMPTET;
                    Model.Local.Customer Customer;
                    Model.Prestashop.idcustomer PsCustomer;
                    Model.Local.Customer Tiers;
                    Model.Prestashop.idcustomer PsTiers;
                    List<Centrale> centrales;

                    #endregion

                    #region Tarifs d'exceptions

                    //if (mode == RemiseMode.RemiseClient || mode == RemiseMode.RemiseArticle || mode == RemiseMode.RemiseCumulee || mode == RemiseMode.RemiseEnCascade)
                    {
                        List<string> ct_num = Core.Temp.ListF_COMPTET_Light.Select(c => c.CT_Num).ToList();
                        ct_num.AddRange(Core.Temp.ListF_COMPTET_Centrales.Select(c => c.CT_Num));
                        List<Model.Sage.F_ARTCLIENT> list_fartclient = F_ARTCLIENTRepository.List(F_ARTICLE.AR_Ref).Where(ca => ct_num.Contains(ca.CT_Num)).ToList();
                        foreach (Model.Sage.F_ARTCLIENT categorieArticle in list_fartclient)
                        {
                            if (categorieArticle != null)
                            {
                                centrales = new List<Centrale>();
                                F_COMPTET = Core.Temp.ListF_COMPTET_Light.FirstOrDefault(c => c.CT_Num == categorieArticle.CT_Num);
                                if (F_COMPTET == null)
                                    F_COMPTET = Core.Temp.ListF_COMPTET_Centrales.FirstOrDefault(c => c.CT_Num == categorieArticle.CT_Num);
                                Customer = (F_COMPTET == null) ? null : CustomerRepository.ReadSage(F_COMPTET.cbMarq);
                                PsCustomer = (Customer == null) ? null : Core.Temp.ListPrestashopCustomer.FirstOrDefault(c => c.id_customer == (uint)Customer.Pre_Id);

                                //Si le client est présent dans prestashop
                                if (F_COMPTET != null && Customer != null && PsCustomer != null)
                                    centrales.Add(new Centrale(F_COMPTET, Customer, PsCustomer));

                                //Récupére tous les tiers qui dépendent de la centrale d'achats et qui sont mappés
                                foreach (var customer in F_COMPTETRepository.ListCentrale(F_COMPTET.CT_Num))
                                {
                                    Tiers = (customer == null) ? null : CustomerRepository.ReadSage(customer.cbMarq);
                                    PsTiers = (Tiers == null || !PsCustomerRepository.ExistCustomer((uint)Tiers.Pre_Id)) ? null : new Model.Prestashop.idcustomer() { id_customer = (uint)Tiers.Pre_Id };

                                    if (customer != null && Tiers != null && PsTiers != null)
                                        centrales.Add(new Centrale(customer, Tiers, PsTiers, F_COMPTET));
                                }

                                foreach (var centrale in centrales)
                                {
                                    if (mode != RemiseMode.RemiseClient || centrale.Tiers.CT_Taux01 == 0)
                                    {
                                        precedent = 1;
                                        remise = 0;

                                        clientKey = new client_key(0, centrale.Tiers.cbMarq, centrale.cat_tarif, precedent);

                                        price = GetHTPrice(F_ARTICLE, categorieArticle, PsTax, Product);

                                        //Le client est répertorié comme ayant un tarif d'exception
                                        rateExceptionClients.Add(centrale.Tiers.cbMarq);

                                        //Récupération des prix des énumérés de gamme
                                        Dictionary<int, decimal> enumeres = GetUnitPrices(F_ARTICLE, Article, CompositionArticle,
                                            ((centrale.CentraleAchat != null) ? centrale.CentraleAchat.CT_Num : centrale.Tiers.CT_Num), price,
                                            ((categorieArticle.AC_PrixTTC == 1) ? true : false), PsTax, Product.ecotaxe_htsage);

                                        List<Model.Sage.F_TARIFQTE> remises = new List<Model.Sage.F_TARIFQTE>(
                                                F_TARIFQTERepository.ListReferenceCategorie(F_ARTICLE.AR_Ref,
                                                ((centrale.CentraleAchat != null) ? centrale.CentraleAchat.CT_Num : centrale.Tiers.CT_Num))
                                                .OrderBy(result => result.TQ_BorneSup));
                                        if (enumeres.Count == 0 && remises.Count != 0)
                                        {
                                            if (remises.Count(result => result.TQ_PrixNet.Value > 0) > 0)
                                            {
                                                //Parcours tous les prix net du client en cours
                                                for (int i = 0; i < remises.Count; i++)
                                                {
                                                    clientKey = new client_key(0, centrale.Tiers.cbMarq, centrale.cat_tarif, precedent);

                                                    Model.Sage.F_TARIFQTE remiseClient = remises[i];

                                                    if (remiseClient.TQ_PrixNet.Value != 0)
                                                    {
                                                        netPrice = remiseClient.TQ_PrixNet.Value;

                                                        if (netPrice > 0 && categorieArticle.AC_PrixTTC == 1 && PsTax != null)
                                                            netPrice = (netPrice / (1 + (PsTax.Rate / 100))) - Product.ecotaxe_htsage;

                                                        #region insert
                                                        if (!Contains(prixSpecifiqueClients, clientKey))
                                                        {
                                                            clientKey.PsSpecificPrice = new Model.Prestashop.PsSpecificPrice()
                                                            {
                                                                IDProduct = Product.IDProduct,
                                                                IDCurrency = 0,
                                                                IDCountry = 0,
                                                                IDGroup = 0,
                                                                Price = netPrice,
                                                                ReductionType = "percentage",
                                                                Reduction = 0,
                                                                FromQuantity = (ushort)precedent,
                                                                IDShop = Global.CurrentShop.IDShop,
                                                                IDShopGroup = Global.CurrentShop.IDShopGroup,
                                                                IDCart = 0,
                                                                IDCustomer = Convert.ToUInt32(centrale.Customer.Pre_Id),
                                                                IDProductAttribute = 0,
                                                                IDSpecificPriceRule = 0,
                                                            };
                                                            prixSpecifiqueClients.Add(clientKey);
                                                            //Identifie le client comme ayant une remise par prix net
                                                            if (!netPriceDiscountClients.Contains(centrale.Tiers.cbMarq))
                                                                netPriceDiscountClients.Add(centrale.Tiers.cbMarq);
                                                        }
                                                        #endregion

                                                    }

                                                    if (remiseClient.TQ_BorneSup != null && remiseClient.TQ_BorneSup != 0)
                                                        precedent = Convert.ToInt64(remiseClient.TQ_BorneSup.Value) + 1;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (enumeres.Count == 0 && remises.Count == 0)
                                            {
                                                if (F_ARTICLE.F_ARTGAMME == null || F_ARTICLE.F_ARTGAMME.Count == 0)
                                                    enumeres.Add(0, 0);
                                                else
                                                {
                                                    if (Article.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleComposition && CompositionArticle != null)
                                                    {
                                                        if (CompositionArticle.Pre_Id != null)
                                                            enumeres.Add((int)CompositionArticle.Pre_Id, 0);
                                                    }
                                                    else
                                                    {
                                                        //Récupération des énumérés de gamme mappés
                                                        Model.Local.AttributeArticleRepository AttributeArticleRepository = new Model.Local.AttributeArticleRepository();
                                                        List<Model.Local.AttributeArticle> declinaisons = new List<Model.Local.AttributeArticle>();
                                                        foreach (Model.Local.AttributeArticle declinaison in AttributeArticleRepository.ListArticleSync(ArticleSend, true))
                                                            if (declinaison.Pre_Id != null && declinaison.Pre_Id > 0)
                                                                enumeres.Add((int)declinaison.Pre_Id, 0);
                                                    }
                                                }
                                            }
                                            foreach (var enumere in enumeres)
                                            {
                                                precedent = 1;
                                                remise = 0;

                                                //Uniquement remise simple en pourcentage
                                                if (categorieArticle.AC_Remise > 0)
                                                {
                                                    clientKey = new client_key(enumere.Key, centrale.Tiers.cbMarq, centrale.cat_tarif, precedent);

                                                    #region insert
                                                    if (!Contains(prixSpecifiqueClients, clientKey))
                                                    {
                                                        clientKey.PsSpecificPrice = new Model.Prestashop.PsSpecificPrice()
                                                        {
                                                            IDProduct = Product.IDProduct,
                                                            IDCurrency = 0,
                                                            IDCountry = 0,
                                                            IDGroup = 0,
                                                            Price = enumere.Value,
                                                            ReductionType = "percentage",
                                                            Reduction = categorieArticle.AC_Remise.Value / 100,
                                                            FromQuantity = (ushort)precedent,
                                                            IDShop = Global.CurrentShop.IDShop,
                                                            IDShopGroup = Global.CurrentShop.IDShopGroup,
                                                            IDCart = 0,
                                                            IDCustomer = Convert.ToUInt32(centrale.Customer.Pre_Id),
                                                            IDProductAttribute = Convert.ToUInt32(enumere.Key),
                                                            IDSpecificPriceRule = 0,
                                                        };
                                                        clientKey.remise_articleclient = clientKey.PsSpecificPrice.Reduction;
                                                        prixSpecifiqueClients.Add(clientKey);
                                                    }
                                                    #endregion

                                                    rateClientArticle.Add(centrale.Tiers.cbMarq);
                                                }
                                                else
                                                {
                                                    /*List<Model.Sage.F_TARIFQTE>*/
                                                    remises = new List<Model.Sage.F_TARIFQTE>(
                                                        F_TARIFQTERepository.ListReferenceCategorie(F_ARTICLE.AR_Ref,
                                                        ((centrale.CentraleAchat != null) ? centrale.CentraleAchat.CT_Num : centrale.Tiers.CT_Num))
                                                        .OrderBy(result => result.TQ_BorneSup));

                                                    //Remises par quantité en pourcentage
                                                    if (remises.Count(result => result.TQ_Remise01REM_Valeur.Value > 0) > 0)
                                                    {
                                                        remiseIndex = 0;

                                                        //Parcours toutes les remises du client en cours
                                                        for (int i = 0; i < remises.Count; i++)
                                                        {
                                                            clientKey = new client_key(enumere.Key, centrale.Tiers.cbMarq, centrale.cat_tarif, precedent);

                                                            Model.Sage.F_TARIFQTE remiseClient = remises[i];
                                                            remise = remiseClient.TQ_Remise01REM_Valeur.Value;

                                                            if (remiseClient.TQ_Remise01REM_Type != 1)
                                                                remise = 0;

                                                            //MODIF JG 17/06/2013
                                                            if ((remise > 0) || (remise == 0 && remiseIndex > 0 && remises[i - 1].TQ_Remise01REM_Valeur.Value > 0)
                                                                || (remise == 0 && enumere.Value != Product.Price))
                                                            {
                                                                if (remiseClient.TQ_Remise01REM_Type == 1)
                                                                {
                                                                    Decimal CurrentPriceArticle = enumere.Value * ((100 - remiseClient.TQ_Remise01REM_Valeur.Value) / 100);
                                                                    if (remiseClient.TQ_Remise02REM_Valeur != null && remiseClient.TQ_Remise02REM_Valeur > 0 && remiseClient.TQ_Remise02REM_Type == 1)
                                                                    {
                                                                        Decimal SecondPriceArticle = CurrentPriceArticle * ((100 - remiseClient.TQ_Remise02REM_Valeur.Value) / 100);
                                                                        remise = 100 - (SecondPriceArticle / enumere.Value) * 100;
                                                                        if (remiseClient.TQ_Remise03REM_Valeur != null && remiseClient.TQ_Remise03REM_Valeur > 0 && remiseClient.TQ_Remise02REM_Type == 1)
                                                                        {
                                                                            Decimal ThirdPriceArticle = SecondPriceArticle * ((100 - remiseClient.TQ_Remise03REM_Valeur.Value) / 100);
                                                                            remise = 100 - (ThirdPriceArticle / enumere.Value) * 100;
                                                                        }
                                                                    }
                                                                }

                                                                #region insert
                                                                if (!Contains(prixSpecifiqueClients, clientKey))
                                                                {
                                                                    clientKey.PsSpecificPrice = new Model.Prestashop.PsSpecificPrice()
                                                                    {
                                                                        IDProduct = Product.IDProduct,
                                                                        IDCurrency = 0,
                                                                        IDCountry = 0,
                                                                        IDGroup = 0,
                                                                        Price = enumere.Value,
                                                                        ReductionType = "percentage",
                                                                        Reduction = remise / 100,
                                                                        FromQuantity = (ushort)precedent,
                                                                        IDShop = Global.CurrentShop.IDShop,
                                                                        IDShopGroup = Global.CurrentShop.IDShopGroup,
                                                                        IDCart = 0,
                                                                        IDCustomer = Convert.ToUInt32(centrale.Customer.Pre_Id),
                                                                        IDProductAttribute = Convert.ToUInt32(enumere.Key),
                                                                        IDSpecificPriceRule = 0,
                                                                    };
                                                                    clientKey.remise_articleclient = clientKey.PsSpecificPrice.Reduction;
                                                                    prixSpecifiqueClients.Add(clientKey);
                                                                }
                                                                else if (mode == RemiseMode.RemiseCumulee || mode == RemiseMode.RemiseCumuleeCatFamille)
                                                                {
                                                                    client_key c = Read(prixSpecifiqueClients, clientKey);
                                                                    c.PsSpecificPrice.Reduction += remise / 100;
                                                                }
                                                                else if (mode == RemiseMode.RemiseEnCascade || mode == RemiseMode.RemiseCumuleeCatFamille)
                                                                {
                                                                    client_key c = Read(prixSpecifiqueClients, clientKey);
                                                                    if (c.PsSpecificPrice.Price != 0)
                                                                        c.PsSpecificPrice.Reduction = ((100 - (((
                                                                            (c.PsSpecificPrice.Price * (1 - c.PsSpecificPrice.Reduction))
                                                                            * (1 - (remise / 100)))
                                                                            * 100) / c.PsSpecificPrice.Price)) / 100);
                                                                    else
                                                                        c.PsSpecificPrice.Reduction = ((100 - ((
                                                                            (1 - c.PsSpecificPrice.Reduction)
                                                                            * (1 - (remise / 100)))
                                                                            * 100)) / 100);
                                                                }
                                                                #endregion

                                                                rateClientArticle.Add(centrale.Tiers.cbMarq);

                                                                remiseIndex = i;
                                                            }

                                                            if (remiseClient.TQ_BorneSup != null && remiseClient.TQ_BorneSup != 0)
                                                                precedent = Convert.ToInt64(remiseClient.TQ_BorneSup.Value) + 1;
                                                        }
                                                    }
                                                    //Prix net par quantité
                                                    else if (remises.Count(result => result.TQ_PrixNet.Value > 0) > 0)
                                                    {
                                                        //Parcours toutes les prix net du client en cours
                                                        for (int i = 0; i < remises.Count; i++)
                                                        {
                                                            clientKey = new client_key(enumere.Key, centrale.Tiers.cbMarq, centrale.cat_tarif, precedent);

                                                            Model.Sage.F_TARIFQTE remiseClient = remises[i];

                                                            if (remiseClient.TQ_PrixNet.Value == 0)
                                                                netPrice = enumere.Value;
                                                            else
                                                            {
                                                                netPrice = remiseClient.TQ_PrixNet.Value;

                                                                if (netPrice > 0 && categorieArticle.AC_PrixTTC == 1 && PsTax != null)
                                                                    netPrice = (netPrice / (1 + (PsTax.Rate / 100))) - Product.ecotaxe_htsage;
                                                            }

                                                            #region insert
                                                            if (!Contains(prixSpecifiqueClients, clientKey))
                                                            {
                                                                clientKey.PsSpecificPrice = new Model.Prestashop.PsSpecificPrice()
                                                                {
                                                                    IDProduct = Product.IDProduct,
                                                                    IDCurrency = 0,
                                                                    IDCountry = 0,
                                                                    IDGroup = 0,
                                                                    Price = netPrice,
                                                                    ReductionType = "percentage",
                                                                    Reduction = 0,
                                                                    FromQuantity = (ushort)precedent,
                                                                    IDShop = Global.CurrentShop.IDShop,
                                                                    IDShopGroup = Global.CurrentShop.IDShopGroup,
                                                                    IDCart = 0,
                                                                    IDCustomer = Convert.ToUInt32(centrale.Customer.Pre_Id),
                                                                    IDProductAttribute = Convert.ToUInt32(enumere.Key),
                                                                    IDSpecificPriceRule = 0,
                                                                };
                                                                prixSpecifiqueClients.Add(clientKey);

                                                                //Identifie le client comme ayant une remise par prix net
                                                                if (!netPriceDiscountClients.Contains(centrale.Tiers.cbMarq))
                                                                    netPriceDiscountClients.Add(centrale.Tiers.cbMarq);
                                                            }
                                                            #endregion

                                                            if (remiseClient.TQ_BorneSup != null && remiseClient.TQ_BorneSup != 0)
                                                                precedent = Convert.ToInt64(remiseClient.TQ_BorneSup.Value) + 1;
                                                        }
                                                    }
                                                    // <JG> 22/02/2013 correction insertion tarif d'exception énuméré/client sans remise
                                                    else if (enumere.Value > 0)
                                                    {
                                                        clientKey = new client_key(enumere.Key, centrale.Tiers.cbMarq, centrale.cat_tarif, precedent);
                                                        clientKey.horsremise = categorieArticle.AC_TypeRem == (short)ABSTRACTION_SAGE.F_ARTCLIENT.Obj._Enum_AC_TypeRem.Hors_Remise;
                                                        if (!offDiscountClients.Contains(centrale.Tiers.cbMarq))
                                                            offDiscountClients.Add(centrale.Tiers.cbMarq);

                                                        #region insert
                                                        if (!Contains(prixSpecifiqueClients, clientKey))
                                                        {
                                                            clientKey.PsSpecificPrice = new Model.Prestashop.PsSpecificPrice()
                                                            {
                                                                IDProduct = Product.IDProduct,
                                                                IDCurrency = 0,
                                                                IDCountry = 0,
                                                                IDGroup = 0,
                                                                Price = enumere.Value,
                                                                ReductionType = "percentage",
                                                                Reduction = 0,
                                                                FromQuantity = (ushort)precedent,
                                                                IDShop = Global.CurrentShop.IDShop,
                                                                IDShopGroup = Global.CurrentShop.IDShopGroup,
                                                                IDCart = 0,
                                                                IDCustomer = Convert.ToUInt32(centrale.Customer.Pre_Id),
                                                                IDProductAttribute = Convert.ToUInt32(enumere.Key),
                                                                IDSpecificPriceRule = 0,
                                                            };
                                                            prixSpecifiqueClients.Add(clientKey);
                                                        }
                                                        #endregion
                                                    }
                                                }

                                                if (mode == RemiseMode.RemiseArticle || mode == RemiseMode.RemiseCumulee || mode == RemiseMode.RemiseEnCascade || mode == RemiseMode.RemiseCumuleeCatFamille || mode == RemiseMode.RemiseEnCascadeCatFamille)
                                                {
                                                    precedent = 1;
                                                    remise = 0;

                                                    /*List<Model.Sage.F_TARIFQTE>*/
                                                    remises = new List<Model.Sage.F_TARIFQTE>(
                                                        F_TARIFQTERepository.ListReferenceCategorie(F_ARTICLE.AR_Ref, String.Format("a{0}", centrale.cat_tarif.ToString("00")))
                                                        .OrderBy(result => result.TQ_BorneSup));

                                                    if (remises.Count(result => result.TQ_Remise01REM_Valeur.Value > 0) > 0)
                                                    {
                                                        remiseIndex = 0;

                                                        //Parcours toutes les remises définies pour la catégorie tarifaire mappée et pour l'article en cours
                                                        for (int i = 0; i < remises.Count; i++)
                                                        {
                                                            Model.Sage.F_TARIFQTE remiseArticle = remises[i];
                                                            remise = remiseArticle.TQ_Remise01REM_Valeur.Value;

                                                            if (remiseArticle.TQ_Remise01REM_Type != 1)
                                                                remise = 0;

                                                            //MODIF JG 17/06/2013
                                                            if ((remise > 0) || (remise == 0 && remiseIndex > 0 && remises[i - 1].TQ_Remise01REM_Valeur.Value > 0)
                                                                || (remise == 0 && enumere.Value != Product.Price))
                                                            {
                                                                clientKey = new client_key(enumere.Key, centrale.Tiers.cbMarq, centrale.cat_tarif, precedent);

                                                                Model.Prestashop.PsSpecificPrice palier = null;
                                                                foreach (var prixSpecifiqueClient in prixSpecifiqueClients.Where(ck => ck.product_attribute == enumere.Key && ck.tiers_cbMarq == centrale.Tiers.cbMarq))
                                                                {
                                                                    if (prixSpecifiqueClient.PsSpecificPrice.FromQuantity <= precedent
                                                                            && (palier == null || palier.FromQuantity <= prixSpecifiqueClient.PsSpecificPrice.FromQuantity))
                                                                        palier = prixSpecifiqueClient.PsSpecificPrice;
                                                                }

                                                                #region insert
                                                                if (palier != null && !Contains(prixSpecifiqueClients, clientKey))
                                                                {
                                                                    clientKey.PsSpecificPrice = new Model.Prestashop.PsSpecificPrice()
                                                                    {
                                                                        IDProduct = Product.IDProduct,
                                                                        IDCurrency = 0,
                                                                        IDCountry = 0,
                                                                        IDGroup = 0,
                                                                        Price = palier.Price,
                                                                        ReductionType = "percentage",
                                                                        Reduction = palier.Reduction,
                                                                        FromQuantity = (uint)precedent,
                                                                        IDShop = Global.CurrentShop.IDShop,
                                                                        IDShopGroup = Global.CurrentShop.IDShopGroup,
                                                                        IDCart = 0,
                                                                        IDCustomer = Convert.ToUInt32(centrale.Customer.Pre_Id),
                                                                        IDProductAttribute = Convert.ToUInt32(enumere.Key),
                                                                        IDSpecificPriceRule = 0,
                                                                    };
                                                                    clientKey.remise_articlecategorie = clientKey.PsSpecificPrice.Reduction;
                                                                    prixSpecifiqueClients.Add(clientKey);
                                                                }
                                                                #endregion

                                                                remiseIndex = i;
                                                            }

                                                            if (remiseArticle.TQ_BorneSup != null && remiseArticle.TQ_BorneSup != 0)
                                                                precedent = Convert.ToInt64(remiseArticle.TQ_BorneSup.Value) + 1;
                                                        }
                                                    }
                                                    else if (remises.Count(result => result.TQ_PrixNet.Value > 0) > 0)
                                                    {
                                                        //Parcours tous les prix net du client en cours
                                                        for (int i = 0; i < remises.Count; i++)
                                                        {
                                                            clientKey = new client_key(enumere.Key, centrale.Tiers.cbMarq, centrale.cat_tarif, precedent);

                                                            Model.Sage.F_TARIFQTE remiseClient = remises[i];

                                                            #region insert
                                                            if (!Contains(prixSpecifiqueClients, clientKey))
                                                            {
                                                                clientKey.PsSpecificPrice = new Model.Prestashop.PsSpecificPrice()
                                                                {
                                                                    IDProduct = Product.IDProduct,
                                                                    IDCurrency = 0,
                                                                    IDCountry = 0,
                                                                    IDGroup = 0,
                                                                    Price = enumere.Value,
                                                                    ReductionType = "percentage",
                                                                    Reduction = categorieArticle.AC_Remise.Value / 100,
                                                                    FromQuantity = (ushort)precedent,
                                                                    IDShop = Global.CurrentShop.IDShop,
                                                                    IDShopGroup = Global.CurrentShop.IDShopGroup,
                                                                    IDCart = 0,
                                                                    IDCustomer = Convert.ToUInt32(centrale.Customer.Pre_Id),
                                                                    IDProductAttribute = Convert.ToUInt32(enumere.Key),
                                                                    IDSpecificPriceRule = 0,
                                                                };
                                                                clientKey.remise_articlecategorie = clientKey.PsSpecificPrice.Reduction;
                                                                prixSpecifiqueClients.Add(clientKey);
                                                            }
                                                            #endregion

                                                            if (remiseClient.TQ_BorneSup != null && remiseClient.TQ_BorneSup != 0)
                                                                precedent = Convert.ToInt64(remiseClient.TQ_BorneSup.Value) + 1;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            F_COMPTET = null;
                            Customer = null;
                            PsCustomer = null;
                            Tiers = null;
                            PsTiers = null;
                            centrales = null;
                        }
                    }

                    #endregion
                    if (Core.Global.GetConfig().ChronoSynchroStockPriceActif)
                        log_chrono.Add("210-TarifException;" + F_ARTICLE.AR_Ref + ";" + (DateTime.UtcNow - inter).ToString()); inter = DateTime.UtcNow;

                    #region Remises article par catégorie tarifaire

                    //if (mode == RemiseMode.RemiseArticle || mode == RemiseMode.RemiseCumulee || mode == RemiseMode.RemiseEnCascade)
                    {
                        //Parcours toutes les catégories tarifaires mappées
                        Model.Sage.F_ARTCLIENT categorieArticle;
                        foreach (Model.Local.Group group in GroupRepository.List())
                        {
                            categorieArticle = null;
                            if (group.Grp_CatTarifId.HasValue && group.Grp_CatTarifId.Value > 0
                                && PsGroupRepository.ExistGroup(group.Grp_Pre_Id))
                            {
                                //Récupération de la tarification article
                                categorieArticle = F_ARTCLIENTRepository.ReadReferenceCategorie(
                                    F_ARTICLE.AR_Ref, group.Grp_CatTarifId.Value);

                                if (categorieArticle != null)
                                {
                                    precedent = 1;
                                    remise = 0;

                                    groupKey = new group_key(0, group.Grp_Pre_Id, group.Grp_CatTarifId, precedent, Product.Price);

                                    price = GetHTPrice(F_ARTICLE, categorieArticle, PsTax, Product);

                                    //Récupération des prix des énumérés de gamme
                                    Dictionary<int, decimal> enumeres = GetUnitPrices(F_ARTICLE, Article, CompositionArticle,
                                            String.Format("a{0}", Convert.ToInt16(group.Grp_CatTarifId).ToString("00")), price,
                                            ((categorieArticle.AC_PrixTTC == 1) ? true : false), PsTax, Product.ecotaxe_htsage);

                                    if (enumeres.Count == 0 && price == 0)
                                        enumeres.Add(0, 0);

                                    foreach (var enumere in enumeres)
                                    {
                                        precedent = 1;
                                        remise = 0;
                                        decimal base_price = enumere.Value;
                                        if (enumere.Key == 0)
                                        {
                                            base_price = Product.Price;
                                        }
                                        else
                                        {
                                            Model.Prestashop.PsProductAttributeRepository PsProductAttributeRepository = new Model.Prestashop.PsProductAttributeRepository();
                                            if (PsProductAttributeRepository.ExistProductAttribute((uint)enumere.Key))
                                            {
                                                Model.Prestashop.PsProductAttribute PsProductAttribute = PsProductAttributeRepository.ReadProductAttribute((uint)enumere.Key);
                                                base_price = Product.Price + PsProductAttribute.Price;
                                            }
                                        }

                                        //Uniquement remise simple en pourcentage
                                        if (categorieArticle.AC_Remise > 0)
                                        {
                                            groupKey = new group_key(enumere.Key, group.Grp_Pre_Id, group.Grp_CatTarifId, precedent, base_price);
                                            #region insert
                                            groupKey.PsSpecificPrice = new Model.Prestashop.PsSpecificPrice()
                                            {
                                                IDProduct = Product.IDProduct,
                                                IDCurrency = 0,
                                                IDCountry = 0,
                                                IDGroup = (ushort)group.Grp_Pre_Id,
                                                Price = enumere.Value,
                                                ReductionType = "percentage",
                                                Reduction = categorieArticle.AC_Remise.Value / 100,
                                                FromQuantity = (ushort)precedent,
                                                IDShop = Global.CurrentShop.IDShop,
                                                IDShopGroup = Global.CurrentShop.IDShopGroup,
                                                IDCart = 0,
                                                IDCustomer = 0,
                                                IDProductAttribute = Convert.ToUInt32(enumere.Key),
                                                IDSpecificPriceRule = 0,
                                            };
                                            groupKey.remise_articlecategorie = groupKey.PsSpecificPrice.Reduction;
                                            if (mode == RemiseMode.RemiseArticle)
                                                prixSpecifiqueGroupes.Add(groupKey);
                                            else if (mode == RemiseMode.RemiseCumulee || mode == RemiseMode.RemiseEnCascade || mode == RemiseMode.RemiseCumuleeCatFamille || mode == RemiseMode.RemiseEnCascadeCatFamille)
                                            {
                                                if (!Contains(prixSpecifiqueGroupes, groupKey))
                                                    prixSpecifiqueGroupes.Add(groupKey);
                                                else if (mode == RemiseMode.RemiseCumulee || mode == RemiseMode.RemiseCumuleeCatFamille)
                                                {
                                                    group_key g = Read(prixSpecifiqueGroupes, groupKey);
                                                    g.PsSpecificPrice.Reduction += categorieArticle.AC_Remise.Value / 100;
                                                }
                                                else if (mode == RemiseMode.RemiseEnCascade || mode == RemiseMode.RemiseEnCascadeCatFamille)
                                                {
                                                    group_key g = Read(prixSpecifiqueGroupes, groupKey);
                                                    if (g.PsSpecificPrice.Price != 0)
                                                        g.PsSpecificPrice.Reduction = ((100 - (((
                                                            (g.PsSpecificPrice.Price * (1 - g.PsSpecificPrice.Reduction))
                                                            * (1 - (categorieArticle.AC_Remise.Value / 100)))
                                                            * 100) / g.PsSpecificPrice.Price)) / 100);
                                                    else
                                                        g.PsSpecificPrice.Reduction = ((100 - ((
                                                            (1 - g.PsSpecificPrice.Reduction)
                                                            * (1 - (categorieArticle.AC_Remise.Value / 100)))
                                                            * 100)) / 100);
                                                }
                                            }
                                            #endregion

                                            rateCategoryArticle.Add(group.Grp_CatTarifId.Value);
                                        }
                                        else
                                        {
                                            List<Model.Sage.F_TARIFQTE> remises = new List<Model.Sage.F_TARIFQTE>(
                                                F_TARIFQTERepository.ListReferenceCategorie(F_ARTICLE.AR_Ref, String.Format("a{0}", Convert.ToInt16(group.Grp_CatTarifId).ToString("00")))
                                                .OrderBy(result => result.TQ_BorneSup));

                                            //Remises par quantité en pourcentage
                                            if (remises.Count(result => result.TQ_Remise01REM_Valeur.Value > 0) > 0)
                                            {
                                                remiseIndex = 0;

                                                //Parcours toutes les remises définies pour la catégorie tarifaire mappée et pour l'article en cours
                                                for (int i = 0; i < remises.Count; i++)
                                                {
                                                    Model.Sage.F_TARIFQTE remiseArticle = remises[i];
                                                    remise = remiseArticle.TQ_Remise01REM_Valeur.Value;

                                                    if (remiseArticle.TQ_Remise01REM_Type != 1)
                                                        remise = 0;

                                                    //MODIF JG 17/06/2013
                                                    if ((remise > 0) || (remise == 0 && remiseIndex > 0 && remises[i - 1].TQ_Remise01REM_Valeur.Value > 0)
                                                        || (remise == 0 && enumere.Value != Product.Price))
                                                    {
                                                        if (remiseArticle.TQ_Remise01REM_Type == 1)
                                                        {
                                                            Decimal CurrentPriceArticle = enumere.Value * ((100 - remiseArticle.TQ_Remise01REM_Valeur.Value) / 100);
                                                            if (remiseArticle.TQ_Remise02REM_Valeur != null && remiseArticle.TQ_Remise02REM_Valeur > 0 && remiseArticle.TQ_Remise02REM_Type == 1)
                                                            {
                                                                Decimal SecondPriceArticle = CurrentPriceArticle * ((100 - remiseArticle.TQ_Remise02REM_Valeur.Value) / 100);
                                                                remise = 100 - (SecondPriceArticle / enumere.Value) * 100;
                                                                if (remiseArticle.TQ_Remise03REM_Valeur != null && remiseArticle.TQ_Remise03REM_Valeur > 0 && remiseArticle.TQ_Remise02REM_Type == 1)
                                                                {
                                                                    Decimal ThirdPriceArticle = SecondPriceArticle * ((100 - remiseArticle.TQ_Remise03REM_Valeur.Value) / 100);
                                                                    remise = 100 - (ThirdPriceArticle / enumere.Value) * 100;
                                                                }
                                                            }
                                                        }

                                                        groupKey = new group_key(enumere.Key, group.Grp_Pre_Id, group.Grp_CatTarifId, precedent, base_price);
                                                        #region insert
                                                        groupKey.PsSpecificPrice = new Model.Prestashop.PsSpecificPrice()
                                                        {
                                                            IDProduct = Product.IDProduct,
                                                            IDCurrency = 0,
                                                            IDCountry = 0,
                                                            IDGroup = (ushort)group.Grp_Pre_Id,
                                                            Price = enumere.Value,
                                                            ReductionType = "percentage",
                                                            Reduction = remise / 100,
                                                            FromQuantity = (ushort)precedent,
                                                            IDShop = Global.CurrentShop.IDShop,
                                                            IDShopGroup = Global.CurrentShop.IDShopGroup,
                                                            IDCart = 0,
                                                            IDCustomer = 0,
                                                            IDProductAttribute = Convert.ToUInt32(enumere.Key),
                                                            IDSpecificPriceRule = 0,
                                                        };
                                                        groupKey.remise_articlecategorie = groupKey.PsSpecificPrice.Reduction;
                                                        if (mode == RemiseMode.RemiseArticle)
                                                        {
                                                            if (Contains(prixSpecifiqueGroupes, groupKey))
                                                            {
                                                                group_key g = Read(prixSpecifiqueGroupes, groupKey);
                                                                g.PsSpecificPrice.Reduction = remise / 100;
                                                            }
                                                            else
                                                                prixSpecifiqueGroupes.Add(groupKey);
                                                        }
                                                        else if (mode == RemiseMode.RemiseCumulee || mode == RemiseMode.RemiseEnCascade || mode == RemiseMode.RemiseCumuleeCatFamille || mode == RemiseMode.RemiseEnCascadeCatFamille)
                                                        {
                                                            if (!Contains(prixSpecifiqueGroupes, groupKey))
                                                                prixSpecifiqueGroupes.Add(groupKey);
                                                            else if (mode == RemiseMode.RemiseCumulee || mode == RemiseMode.RemiseCumuleeCatFamille)
                                                            {
                                                                group_key g = Read(prixSpecifiqueGroupes, groupKey);
                                                                g.PsSpecificPrice.Reduction += remise / 100;
                                                            }
                                                            else if (mode == RemiseMode.RemiseEnCascade || mode == RemiseMode.RemiseEnCascadeCatFamille)
                                                            {
                                                                group_key g = Read(prixSpecifiqueGroupes, groupKey);
                                                                if (g.PsSpecificPrice.Price != 0)
                                                                    g.PsSpecificPrice.Reduction = ((100 - (((
                                                                        (g.PsSpecificPrice.Price * (1 - g.PsSpecificPrice.Reduction))
                                                                        * (1 - (remise / 100)))
                                                                        * 100) / g.PsSpecificPrice.Price)) / 100);
                                                                else
                                                                    g.PsSpecificPrice.Reduction = ((100 - (((
                                                                        (1 - g.PsSpecificPrice.Reduction))
                                                                        * (1 - (remise / 100)))
                                                                        * 100)) / 100);
                                                            }
                                                        }
                                                        #endregion

                                                        rateCategoryArticle.Add(group.Grp_CatTarifId.Value);

                                                        remiseIndex = i;
                                                    }

                                                    if (remiseArticle.TQ_BorneSup != null && remiseArticle.TQ_BorneSup != 0)
                                                        precedent = Convert.ToInt64(remiseArticle.TQ_BorneSup.Value) + 1;
                                                }
                                            }
                                            //Prix net par quantité
                                            else if (remises.Count(result => result.TQ_PrixNet.Value > 0) > 0)
                                            {
                                                precedentNetPrice = 0;
                                                remiseIndex = 0;

                                                //Parcours toutes les prix net de la catégorie tarifaire en cours
                                                for (int i = 0; i < remises.Count; i++)
                                                {
                                                    groupKey = new group_key(enumere.Key, group.Grp_Pre_Id, group.Grp_CatTarifId, precedent, base_price);

                                                    Model.Sage.F_TARIFQTE remiseArticle = remises[i];

                                                    if (remiseArticle.TQ_PrixNet.Value == 0)
                                                        netPrice = enumere.Value;
                                                    else
                                                    {
                                                        netPrice = remiseArticle.TQ_PrixNet.Value;

                                                        if (netPrice > 0 && categorieArticle.AC_PrixTTC == 1 && PsTax != null)
                                                            netPrice = (netPrice / (1 + (PsTax.Rate / 100))) - Product.ecotaxe_htsage;
                                                    }

                                                    precedentNetPrice = 0;

                                                    //Recherche du prix net précédent
                                                    if (remiseIndex > 0)
                                                    {
                                                        Model.Sage.F_TARIFQTE precedentRemiseArticle = remises[i - 1];

                                                        if (precedentRemiseArticle.TQ_PrixNet.Value == 0)
                                                            precedentNetPrice = enumere.Value;
                                                        else
                                                        {
                                                            precedentNetPrice = precedentRemiseArticle.TQ_PrixNet.Value;

                                                            if (precedentNetPrice > 0 && categorieArticle.AC_PrixTTC == 1 && PsTax != null)
                                                                precedentNetPrice = precedentNetPrice / (1 + (PsTax.Rate / 100)) - Product.ecotaxe_htsage;
                                                        }
                                                    }

                                                    if ((netPrice != enumere.Value || netPrice != Product.Price) || (remiseIndex > 0 && precedentNetPrice != enumere.Value))
                                                    {
                                                        #region insert
                                                        if (!Contains(prixSpecifiqueGroupes, groupKey))
                                                        {
                                                            groupKey.PsSpecificPrice = new Model.Prestashop.PsSpecificPrice()
                                                            {
                                                                IDProduct = Product.IDProduct,
                                                                IDCurrency = 0,
                                                                IDCountry = 0,
                                                                IDGroup = (ushort)group.Grp_Pre_Id,
                                                                Price = netPrice,
                                                                ReductionType = "percentage",
                                                                Reduction = 0,
                                                                FromQuantity = (ushort)precedent,
                                                                IDShop = Global.CurrentShop.IDShop,
                                                                IDShopGroup = Global.CurrentShop.IDShopGroup,
                                                                IDCart = 0,
                                                                IDCustomer = 0,
                                                                IDProductAttribute = Convert.ToUInt32(enumere.Key),
                                                                IDSpecificPriceRule = 0,
                                                            };
                                                            prixSpecifiqueGroupes.Add(groupKey);
                                                            //Identifie la catégorie comme ayant des prix nets
                                                            if (!netPriceCategory.Contains(group.Grp_CatTarifId.Value))
                                                                netPriceCategory.Add(group.Grp_CatTarifId.Value);
                                                        }
                                                        #endregion

                                                        remiseIndex = i;
                                                    }

                                                    if (remiseArticle.TQ_BorneSup != null && remiseArticle.TQ_BorneSup != 0)
                                                        precedent = Convert.ToInt64(remiseArticle.TQ_BorneSup.Value) + 1;
                                                }
                                            }
                                            // <JG> 22/02/2013 correction insertion tarif énuméré/cat_tarif sans remise
                                            else if (enumere.Value > 0)
                                            {
                                                groupKey = new group_key(enumere.Key, group.Grp_Pre_Id, group.Grp_CatTarifId, precedent, base_price);
                                                groupKey.horsremise_article = categorieArticle.AC_TypeRem == (short)ABSTRACTION_SAGE.F_ARTCLIENT.Obj._Enum_AC_TypeRem.Hors_Remise;
                                                if (groupKey.horsremise_article && !offDiscountCategoryArticle.Contains(group.Grp_CatTarifId.Value))
                                                    offDiscountCategoryArticle.Add(group.Grp_CatTarifId.Value);
                                                #region insert
                                                if (!Contains(prixSpecifiqueGroupes, groupKey))
                                                {
                                                    groupKey.PsSpecificPrice = new Model.Prestashop.PsSpecificPrice()
                                                    {
                                                        IDProduct = Product.IDProduct,
                                                        IDCurrency = 0,
                                                        IDCountry = 0,
                                                        IDGroup = (ushort)group.Grp_Pre_Id,
                                                        Price = enumere.Value,
                                                        ReductionType = "percentage",
                                                        Reduction = 0,
                                                        FromQuantity = (ushort)precedent,
                                                        IDShop = Global.CurrentShop.IDShop,
                                                        IDShopGroup = Global.CurrentShop.IDShopGroup,
                                                        IDCart = 0,
                                                        IDCustomer = 0,
                                                        IDProductAttribute = Convert.ToUInt32(enumere.Key),
                                                        IDSpecificPriceRule = 0,
                                                    };
                                                    prixSpecifiqueGroupes.Add(groupKey);
                                                }
                                                #endregion
                                            }
                                        }
                                    }
                                }
                                // si pas de prix par catégorie tarifaire mais que prix fiche article différent de la catégorie tarifaire par défaut
                                else
                                {
                                    decimal price_cat = GetHTPrice(F_ARTICLE, null, PsTax, Product);
                                    if (price_cat != Product.Price)
                                    {
                                        groupKey = new group_key(0, group.Grp_Pre_Id, group.Grp_CatTarifId, precedent, Product.Price);
                                        #region insert
                                        if (!Contains(prixSpecifiqueGroupes, groupKey))
                                        {
                                            groupKey.PsSpecificPrice = new Model.Prestashop.PsSpecificPrice()
                                            {
                                                IDProduct = Product.IDProduct,
                                                IDCurrency = 0,
                                                IDCountry = 0,
                                                IDGroup = (ushort)group.Grp_Pre_Id,
                                                Price = price_cat,
                                                ReductionType = "percentage",
                                                Reduction = 0,
                                                FromQuantity = (ushort)precedent,
                                                IDShop = Global.CurrentShop.IDShop,
                                                IDShopGroup = Global.CurrentShop.IDShopGroup,
                                                IDCart = 0,
                                                IDCustomer = 0,
                                                IDProductAttribute = 0,
                                                IDSpecificPriceRule = 0,
                                            };
                                            prixSpecifiqueGroupes.Add(groupKey);
                                        }
                                        #endregion
                                    }
                                }
                            }
                        }
                        categorieArticle = null;
                    }

                    #endregion
                    if (Core.Global.GetConfig().ChronoSynchroStockPriceActif)
                        log_chrono.Add("211-CategorieTarifaire;" + F_ARTICLE.AR_Ref + ";" + (DateTime.UtcNow - inter).ToString()); inter = DateTime.UtcNow;

                    #region Remises client

                    if (mode == RemiseMode.RemiseClient || mode == RemiseMode.RemiseCumulee || mode == RemiseMode.RemiseEnCascade
                        || mode == RemiseMode.RemiseCumuleeCatFamille || mode == RemiseMode.RemiseEnCascadeCatFamille)
                    {
                        DateTime inter2 = DateTime.UtcNow;
                        Model.Sage.F_ARTCLIENT categorieArticle;
                        Model.Sage.F_ARTCLIENT categorieClient;
                        //Parcourir tous les clients mappés qui ont une remise et l'applique sur l'article en cours
                        foreach (Model.Local.Customer LocalCustomer in Core.Temp.ListLocalCustomer_Remise)
                        {
                            centrales = new List<Centrale>();

                            F_COMPTET = (LocalCustomer == null) ? null : Core.Temp.ListF_COMPTET_Light.FirstOrDefault(c => c.cbMarq == LocalCustomer.Sag_Id);
                            PsCustomer = (LocalCustomer == null) ? null : Core.Temp.ListPrestashopCustomer.FirstOrDefault(p => p.id_customer == (uint)LocalCustomer.Pre_Id);
                            //PsCustomer = (LocalCustomer == null || !PsCustomerRepository.ExistCustomer((uint)LocalCustomer.Pre_Id)) ? null : new Model.Prestashop.idcustomer() { id_customer = (uint)LocalCustomer.Pre_Id };

                            //Si le client est présent dans prestashop
                            if (F_COMPTET != null && LocalCustomer != null && PsCustomer != null && F_COMPTET.CT_Taux01.Value > 0)
                                centrales.Add(new Centrale(F_COMPTET, LocalCustomer, PsCustomer));

                            //Récupére tous les tiers qui dépendent de la centrale d'achats et qui sont mappés
                            if (F_COMPTET != null && !string.IsNullOrEmpty(F_COMPTET.CT_Num) && F_COMPTET.CT_Taux01.Value > 0)
                                foreach (Model.Sage.F_COMPTET_Light F_COMPTET_Light in F_COMPTETRepository.ListCentrale(F_COMPTET.CT_Num))
                                {
                                    Tiers = (F_COMPTET_Light == null) ? null : CustomerRepository.ReadSage(F_COMPTET_Light.cbMarq);
                                    PsTiers = (Tiers == null) ? null : Core.Temp.ListPrestashopCustomer.FirstOrDefault(p => p.id_customer == (uint)Tiers.Pre_Id);

                                    if (F_COMPTET_Light != null && Tiers != null && PsTiers != null)
                                        centrales.Add(new Centrale(F_COMPTET_Light, Tiers, PsTiers, F_COMPTET));
                                }

                            #region PLI
                            foreach (var centrale in centrales)
                            {
                                precedent = 1;
                                remise = 0;

                                // ajout récupération des prix et remises de la catégorie tarifaire si absence de tarif d'exception
                                if (mode == RemiseMode.RemiseCumulee || mode == RemiseMode.RemiseEnCascade || mode == RemiseMode.RemiseCumuleeCatFamille || mode == RemiseMode.RemiseEnCascadeCatFamille)
                                    if (!rateExceptionClients.Contains(centrale.Tiers.cbMarq)
                                        && !netPriceDiscountClients.Contains(centrale.Tiers.cbMarq)
                                        && !netPriceCategory.Contains(centrale.Tiers.N_CatTarif.Value)
                                        && prixSpecifiqueGroupes.Count(gk => gk.cattarif_cbMarq == centrale.Tiers.N_CatTarif.Value) > 0)
                                    {
                                        foreach (group_key gk in prixSpecifiqueGroupes.Where(gk => gk.cattarif_cbMarq == centrale.Tiers.N_CatTarif.Value))
                                        {
                                            clientKey = new client_key(
                                                gk.product_attribute,
                                                centrale.Tiers.cbMarq,
                                                centrale.cat_tarif,
                                                gk.pallierqte,
                                                gk.remise_articlecategorie,
                                                gk.remise_famillecategorie
                                            );
                                            #region insert
                                            if (!Contains(prixSpecifiqueClients, clientKey))
                                            {
                                                clientKey.PsSpecificPrice = new Model.Prestashop.PsSpecificPrice()
                                                {
                                                    IDProduct = Product.IDProduct,
                                                    IDCurrency = 0,
                                                    IDCountry = 0,
                                                    IDGroup = 0,
                                                    Price = gk.PsSpecificPrice.Price,
                                                    ReductionType = "percentage",
                                                    Reduction = gk.PsSpecificPrice.Reduction,
                                                    FromQuantity = gk.PsSpecificPrice.FromQuantity,
                                                    IDShop = Global.CurrentShop.IDShop,
                                                    IDShopGroup = Global.CurrentShop.IDShopGroup,
                                                    IDCart = 0,
                                                    IDCustomer = (UInt32)centrale.Customer.Pre_Id,
                                                    IDProductAttribute = gk.PsSpecificPrice.IDProductAttribute,
                                                    IDSpecificPriceRule = 0,
                                                };
                                                clientKey.remise_articlecategorie = clientKey.PsSpecificPrice.Reduction;
                                                prixSpecifiqueClients.Add(clientKey);
                                            }
                                            #endregion
                                        }
                                    }

                                //Recherche du prix HT de base sur le tarif d'exception du client
                                price = 0;
                                categorieArticle = F_ARTCLIENTRepository.Read(F_ARTICLE.AR_Ref, centrale.Tiers.CT_Num);

                                if (categorieArticle != null)
                                {
                                    if (categorieArticle.AC_PrixVen != null)
                                        price = categorieArticle.AC_PrixVen.Value;

                                    if (price == 0
                                        && categorieArticle.AC_Coef != null && categorieArticle.AC_Coef.Value > 0
                                        && F_ARTICLE.AR_PrixAch != null && F_ARTICLE.AR_PrixAch > 0)
                                        price = (F_ARTICLE.AR_PrixAch.Value * categorieArticle.AC_Coef.Value);

                                    if (price > 0 && categorieArticle.AC_PrixTTC == 1 && PsTax != null)
                                        price = (price / (1 + (PsTax.Rate / 100))) - Product.ecotaxe_htsage;
                                }

                                //Recherche du prix HT de base sur la catégorie tarifaire correspondant à celle de la fiche client
                                if (price == 0)
                                {
                                    //Récupération de la catégorie tarifaire de la fiche client
                                    categorieClient = F_ARTCLIENTRepository.ReadReferenceCategorie(
                                    F_ARTICLE.AR_Ref, ((centrale.CentraleAchat != null) ? centrale.CentraleAchat.N_CatTarif.Value : centrale.Tiers.N_CatTarif.Value));

                                    price = GetHTPrice(F_ARTICLE, categorieClient, PsTax, Product);
                                }

                                decimal taux = ((centrale.CentraleAchat != null) ?
                                    centrale.CentraleAchat.CT_Taux01.Value : centrale.Tiers.CT_Taux01.Value);

                                clientKey = new client_key(0, centrale.Tiers.cbMarq, centrale.cat_tarif, precedent);
                                if (!netPriceDiscountClients.Contains(centrale.Tiers.cbMarq)
                                        && !netPriceCategory.Contains(centrale.Tiers.N_CatTarif.Value))
                                    #region insert
                                    if (!Contains(prixSpecifiqueClients, clientKey))
                                    {
                                        clientKey.PsSpecificPrice = new Model.Prestashop.PsSpecificPrice()
                                        {
                                            IDProduct = Product.IDProduct,
                                            IDCurrency = 0,
                                            IDCountry = 0,
                                            IDGroup = 0,
                                            Price = price,
                                            ReductionType = "percentage",
                                            Reduction = 0,
                                            FromQuantity = 1,
                                            IDShop = Global.CurrentShop.IDShop,
                                            IDShopGroup = Global.CurrentShop.IDShopGroup,
                                            IDCart = 0,
                                            IDCustomer = (UInt32)centrale.Customer.Pre_Id,
                                            IDProductAttribute = 0,
                                            IDSpecificPriceRule = 0,
                                        };
                                        prixSpecifiqueClients.Add(clientKey);
                                    }
                                    #endregion

                                if (mode == RemiseMode.RemiseClient)
                                {
                                    //Pour chaque tarif d'exception déjà existant, y affecter la remise client si non prix net
                                    if (!netPriceDiscountClients.Contains(centrale.Tiers.cbMarq)
                                        && !netPriceCategory.Contains(centrale.Tiers.N_CatTarif.Value))
                                    {
                                        foreach (var prixSpecifiqueClient in prixSpecifiqueClients.Where(ck => ck.tiers_cbMarq == centrale.Tiers.cbMarq))
                                        {
                                            prixSpecifiqueClient.PsSpecificPrice.Reduction = taux / 100;
                                            prixSpecifiqueClient.remise_client = clientKey.PsSpecificPrice.Reduction;
                                        }
                                    }
                                }
                                else if (mode == RemiseMode.RemiseCumulee || mode == RemiseMode.RemiseEnCascade || mode == RemiseMode.RemiseCumuleeCatFamille || mode == RemiseMode.RemiseEnCascadeCatFamille)
                                {
                                    if (!netPriceDiscountClients.Contains(centrale.Tiers.cbMarq)
                                            && !netPriceCategory.Contains(centrale.Tiers.N_CatTarif.Value))
                                    {
                                        foreach (var prixSpecifiqueClient in prixSpecifiqueClients.Where(ck => ck.tiers_cbMarq == centrale.Tiers.cbMarq))
                                        {
                                            prixSpecifiqueClient.remise_client = taux / 100;
                                            if (mode == RemiseMode.RemiseCumulee || mode == RemiseMode.RemiseCumuleeCatFamille)
                                            {
                                                prixSpecifiqueClient.PsSpecificPrice.Reduction += taux / 100;
                                            }
                                            else if (mode == RemiseMode.RemiseEnCascade || mode == RemiseMode.RemiseEnCascadeCatFamille)
                                            {
                                                ////1 calcul le prix remisé
                                                //decimal prix = remisesClient[clientKey].Price * (1 - remisesClient[clientKey].Reduction);

                                                ////2 applique la nouvelle remise
                                                //prix = prix * (1 - (F_COMPTET.CT_Taux01.Value / 100));

                                                ////3 à combien la remise est elle au final ?
                                                //remisesClient[clientKey].Reduction = ((100 - ((prix * 100) / remisesClient[clientKey].Price)) / 100); 

                                                if (prixSpecifiqueClient.PsSpecificPrice.Price > 0)
                                                    prixSpecifiqueClient.PsSpecificPrice.Reduction = ((100 - (((
                                                        (prixSpecifiqueClient.PsSpecificPrice.Price * (1 - prixSpecifiqueClient.PsSpecificPrice.Reduction))
                                                        * (1 - (taux / 100)))
                                                        * 100) / prixSpecifiqueClient.PsSpecificPrice.Price)) / 100);
                                                else
                                                    prixSpecifiqueClient.PsSpecificPrice.Reduction = ((100 - ((
                                                        (1 - prixSpecifiqueClient.PsSpecificPrice.Reduction)
                                                        * (1 - (taux / 100)))
                                                        * 100)) / 100);
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                        centrales = null;
                        F_COMPTET = null;
                        PsCustomer = null;
                        Tiers = null;
                        PsTiers = null;
                        categorieArticle = null;
                        categorieClient = null;
                    }

                    #endregion
                    if (Core.Global.GetConfig().ChronoSynchroStockPriceActif)
                        log_chrono.Add("212-RemisesClient;" + F_ARTICLE.AR_Ref + ";" + (DateTime.UtcNow - inter).ToString()); inter = DateTime.UtcNow;

                    #region Remises famille par client

                    if (mode == RemiseMode.RemiseFamille || mode == RemiseMode.RemiseCumulee || mode == RemiseMode.RemiseEnCascade
                        || mode == RemiseMode.RemiseCumuleeCatFamille || mode == RemiseMode.RemiseEnCascadeCatFamille)
                    {
                        Model.Sage.F_ARTCLIENT categorieArticle;
                        Model.Sage.F_ARTCLIENT categorieClient;
                        Model.Sage.F_FAMTARIF categorieFamille;

                        //Récupération de la famille de l'article
                        Model.Sage.F_FAMILLE F_FAMILLE = F_FAMILLERepository.Read(F_ARTICLE);

                        List<Model.Sage.F_COMPTET_Light> ListClientRemiseOrTarifException = new List<Model.Sage.F_COMPTET_Light>();

                        if (mode == RemiseMode.RemiseEnCascade || mode == RemiseMode.RemiseCumulee || mode == RemiseMode.RemiseCumuleeCatFamille || mode == RemiseMode.RemiseEnCascadeCatFamille)
                            ListClientRemiseOrTarifException = new Model.Sage.F_COMPTETRepository().ListClientRemiseTarifException(F_ARTICLE.AR_Ref);
                        ListClientRemiseOrTarifException = ListClientRemiseOrTarifException.Where(c => (Core.Temp.ListF_COMPTET_Light.Count(s => s.CT_Num == c.CT_Num) > 0)).ToList();

                        // Pour la gestion du client et de la centrale d'achat. 
                        IEnumerable<Model.Sage.F_FAMCLIENT> ListeFamilleClient = F_FAMCLIENTRepository.List(F_FAMILLE.FA_CodeFamille).Where(fa => Core.Temp.ListF_COMPTET_Light.Count(c => c.CT_Num == fa.CT_Num || c.CT_NumCentrale == fa.CT_Num) > 0);

                        #region Parcours tous les tarifs client de la famille en cours
                        foreach (Model.Sage.F_FAMCLIENT clientFamille in ListeFamilleClient)
                        {
                            centrales = new List<Centrale>();
                            price = 0;

                            F_COMPTET = Core.Temp.ListF_COMPTET_Light.FirstOrDefault(c => c.CT_Num == clientFamille.CT_Num);
                            if (F_COMPTET == null)
                                F_COMPTET = Core.Temp.ListF_COMPTET_Centrales.FirstOrDefault(c => c.CT_Num == clientFamille.CT_Num);
                            Customer = (F_COMPTET == null) ? null : CustomerRepository.ReadSage(F_COMPTET.cbMarq);
                            PsCustomer = (Customer == null) ? null : Core.Temp.ListPrestashopCustomer.FirstOrDefault(p => p.id_customer == (uint)Customer.Pre_Id);

                            //Si le client est présent dans prestashop
                            if (F_COMPTET != null && Customer != null && PsCustomer != null)
                                centrales.Add(new Centrale(F_COMPTET, Customer, PsCustomer));

                            //Récupère tous les tiers mappés qui dépendent de la centrale d'achat
                            if (F_COMPTET != null && !string.IsNullOrEmpty(F_COMPTET.CT_Num))
                                foreach (var customer in F_COMPTETRepository.ListCentrale(F_COMPTET.CT_Num))
                                {
                                    Tiers = (customer == null) ? null : CustomerRepository.ReadSage(customer.cbMarq);
                                    PsTiers = (Tiers == null) ? null : Core.Temp.ListPrestashopCustomer.FirstOrDefault(p => p.id_customer == (uint)Tiers.Pre_Id);

                                    if (customer != null && Tiers != null && PsTiers != null)
                                        centrales.Add(new Centrale(customer, Tiers, PsTiers, F_COMPTET));
                                }

                            if (mode == RemiseMode.RemiseEnCascade || mode == RemiseMode.RemiseCumulee || mode == RemiseMode.RemiseCumuleeCatFamille || mode == RemiseMode.RemiseEnCascadeCatFamille)
                                if (ListClientRemiseOrTarifException.Count(c => c.CT_Num == clientFamille.CT_Num) == 1)
                                    ListClientRemiseOrTarifException.Remove(ListClientRemiseOrTarifException.First(result => result.CT_Num == clientFamille.CT_Num));

                            if (clientFamille.FC_Remise != null && clientFamille.FC_Remise > 0)
                            {
                                foreach (var centrale in centrales)
                                {
                                    precedent = 1;
                                    remise = 0;

                                    rateFamilleClients.Add(centrale.Tiers.cbMarq);

                                    // ajout récupération des prix et remises de la catégorie tarifaire si absence de tarif d'exception
                                    if (mode == RemiseMode.RemiseCumulee || mode == RemiseMode.RemiseEnCascade || mode == RemiseMode.RemiseCumuleeCatFamille || mode == RemiseMode.RemiseEnCascadeCatFamille)
                                        if (!rateExceptionClients.Contains(centrale.Tiers.cbMarq)
                                            && !netPriceDiscountClients.Contains(centrale.Tiers.cbMarq)
                                            && !netPriceCategory.Contains(centrale.Tiers.N_CatTarif.Value)
                                            && prixSpecifiqueGroupes.Count(gk => gk.cattarif_cbMarq == centrale.Tiers.N_CatTarif.Value) > 0)
                                        {
                                            foreach (group_key gk in prixSpecifiqueGroupes.Where(gk => gk.cattarif_cbMarq == centrale.Tiers.N_CatTarif.Value))
                                            {
                                                clientKey = new client_key(
                                                    gk.product_attribute,
                                                    centrale.Tiers.cbMarq,
                                                    centrale.cat_tarif,
                                                    gk.pallierqte,
                                                    gk.remise_articlecategorie,
                                                    gk.remise_famillecategorie
                                                );
                                                if (!Contains(prixSpecifiqueClients, clientKey))
                                                {
                                                    clientKey.PsSpecificPrice = new Model.Prestashop.PsSpecificPrice()
                                                    {
                                                        IDProduct = Product.IDProduct,
                                                        IDCurrency = 0,
                                                        IDCountry = 0,
                                                        IDGroup = 0,
                                                        Price = gk.PsSpecificPrice.Price,
                                                        ReductionType = "percentage",
                                                        Reduction = gk.PsSpecificPrice.Reduction,
                                                        FromQuantity = gk.PsSpecificPrice.FromQuantity,
                                                        IDShop = Global.CurrentShop.IDShop,
                                                        IDShopGroup = Global.CurrentShop.IDShopGroup,
                                                        IDCart = 0,
                                                        IDCustomer = (UInt32)centrale.Customer.Pre_Id,
                                                        IDProductAttribute = gk.PsSpecificPrice.IDProductAttribute,
                                                        IDSpecificPriceRule = 0,
                                                    };
                                                    prixSpecifiqueClients.Add(clientKey);

                                                    rateFamilleClients.Add(centrale.Tiers.cbMarq);
                                                }
                                            }
                                        }

                                    //Uniquement remise simple en pourcentage
                                    if (!netPriceDiscountClients.Contains(centrale.Tiers.cbMarq)
                                        && !netPriceCategory.Contains(centrale.Tiers.N_CatTarif.Value)
                                        && (mode == RemiseMode.RemiseCumulee || mode == RemiseMode.RemiseEnCascade || mode == RemiseMode.RemiseCumuleeCatFamille || mode == RemiseMode.RemiseEnCascadeCatFamille))
                                    {
                                        foreach (var prixSpecifiqueClient in prixSpecifiqueClients.Where(ck => ck.tiers_cbMarq == centrale.Tiers.cbMarq))
                                        {
                                            prixSpecifiqueClient.remise_familleclient = clientFamille.FC_Remise.Value / 100;
                                            if (mode == RemiseMode.RemiseCumulee || mode == RemiseMode.RemiseCumuleeCatFamille)
                                            {
                                                prixSpecifiqueClient.PsSpecificPrice.Reduction += clientFamille.FC_Remise.Value / 100;
                                            }
                                            else if (mode == RemiseMode.RemiseEnCascade || mode == RemiseMode.RemiseEnCascadeCatFamille)
                                            {
                                                if (prixSpecifiqueClient.PsSpecificPrice.Price > 0)
                                                    prixSpecifiqueClient.PsSpecificPrice.Reduction = ((100 - (((
                                                        (prixSpecifiqueClient.PsSpecificPrice.Price * (1 - prixSpecifiqueClient.PsSpecificPrice.Reduction))
                                                        * (1 - (clientFamille.FC_Remise.Value / 100)))
                                                        * 100) / prixSpecifiqueClient.PsSpecificPrice.Price)) / 100);
                                                else
                                                    prixSpecifiqueClient.PsSpecificPrice.Reduction = ((100 - ((
                                                        (1 - prixSpecifiqueClient.PsSpecificPrice.Reduction)
                                                        * (1 - (clientFamille.FC_Remise.Value / 100)))
                                                        * 100)) / 100);
                                            }
                                        }
                                    }

                                    clientKey = new client_key(0, centrale.Tiers.cbMarq, centrale.cat_tarif, precedent);

                                    if (!Contains(prixSpecifiqueClients, clientKey) && !netPriceDiscountClients.Contains(centrale.Tiers.cbMarq)
                                            && !netPriceCategory.Contains(centrale.Tiers.N_CatTarif.Value))
                                    {
                                        //Recherche du prix HT de base sur le tarif d'exception du client
                                        categorieArticle = F_ARTCLIENTRepository.Read(F_ARTICLE.AR_Ref,
                                            ((centrale.CentraleAchat != null) ? centrale.CentraleAchat.CT_Num : centrale.Tiers.CT_Num));

                                        if (categorieArticle != null)
                                        {
                                            price = categorieArticle.AC_PrixVen.Value;

                                            if (price == 0 && categorieArticle.AC_Coef.Value > 0 && F_ARTICLE.AR_PrixAch > 0)
                                                price = (F_ARTICLE.AR_PrixAch.Value * categorieArticle.AC_Coef.Value);

                                            if (price > 0 && categorieArticle.AC_PrixTTC == 1 && PsTax != null)
                                                price = (categorieArticle.AC_PrixVen.Value / (1 + (PsTax.Rate / 100))) - Product.ecotaxe_htsage;
                                        }

                                        //Recherche du prix HT de base sur la catégorie tarifaire correspondant à celle de la fiche client
                                        if (price == 0)
                                        {
                                            //Récupération de la catégorie tarifaire de la fiche client
                                            categorieClient = F_ARTCLIENTRepository.ReadReferenceCategorie(
                                            F_ARTICLE.AR_Ref, ((centrale.CentraleAchat != null) ? centrale.CentraleAchat.N_CatTarif.Value : centrale.Tiers.N_CatTarif.Value));

                                            price = GetHTPrice(F_ARTICLE, categorieClient, PsTax, Product);
                                        }

                                        clientKey.PsSpecificPrice = new Model.Prestashop.PsSpecificPrice()
                                        {
                                            IDProduct = Product.IDProduct,
                                            IDCurrency = 0,
                                            IDCountry = 0,
                                            IDGroup = 0,
                                            Price = price,
                                            ReductionType = "percentage",
                                            Reduction = clientFamille.FC_Remise.Value / 100,
                                            FromQuantity = (ushort)precedent,
                                            IDShop = Global.CurrentShop.IDShop,
                                            IDShopGroup = Global.CurrentShop.IDShopGroup,
                                            IDCart = 0,
                                            IDCustomer = (UInt32)centrale.Customer.Pre_Id,
                                            IDProductAttribute = 0,
                                            IDSpecificPriceRule = 0,
                                        };
                                        clientKey.remise_familleclient = clientKey.PsSpecificPrice.Reduction;
                                        prixSpecifiqueClients.Add(clientKey);
                                    }
                                }
                            }
                        }
                        #endregion

                        #region récupération remises famille/cat-tarif sur le client
                        if (mode == RemiseMode.RemiseEnCascadeCatFamille || mode == RemiseMode.RemiseCumuleeCatFamille)
                        {
                            foreach (Model.Sage.F_COMPTET_Light client in ListClientRemiseOrTarifException)
                            {
                                precedent = 1;
                                remise = 0;

                                Customer = (client == null) ? null : CustomerRepository.ReadSage(client.cbMarq);
                                PsCustomer = (Customer == null) ? null : Core.Temp.ListPrestashopCustomer.FirstOrDefault(p => p.id_customer == (uint)Customer.Pre_Id);

                                //Si le client est présent dans prestashop
                                if (client != null && Customer != null && PsCustomer != null)
                                {
                                    //Récupération de la tarification de la famille pour la catégorie tarifaire du client
                                    categorieFamille = F_FAMTARIFRepository.ReadReferenceCategorie(F_FAMILLE.FA_CodeFamille, client.N_CatTarif.Value);

                                    if (categorieFamille != null)
                                    {
                                        clientKey = new client_key(0, client.cbMarq, client.N_CatTarif.Value, precedent);

                                        #region remise simple en pourcentage de la catégorie tarifaire
                                        if (categorieFamille.FT_Remise > 0)
                                        {
                                            foreach (var prixSpecifiqueClient in prixSpecifiqueClients.Where(ck => ck.tiers_cbMarq == client.cbMarq))
                                            {
                                                prixSpecifiqueClient.remise_famillecategorie = categorieFamille.FT_Remise.Value / 100;
                                                if (mode == RemiseMode.RemiseCumuleeCatFamille)
                                                    prixSpecifiqueClient.PsSpecificPrice.Reduction += categorieFamille.FT_Remise.Value / 100;
                                                else if (mode == RemiseMode.RemiseEnCascadeCatFamille)
                                                {
                                                    if (prixSpecifiqueClient.PsSpecificPrice.Price > 0)
                                                        prixSpecifiqueClient.PsSpecificPrice.Reduction = ((100 - (((
                                                            (prixSpecifiqueClient.PsSpecificPrice.Price * (1 - prixSpecifiqueClient.PsSpecificPrice.Reduction))
                                                            * (1 - (categorieFamille.FT_Remise.Value / 100)))
                                                            * 100) / prixSpecifiqueClient.PsSpecificPrice.Price)) / 100);
                                                    else
                                                        prixSpecifiqueClient.PsSpecificPrice.Reduction = ((100 - ((
                                                            (1 - prixSpecifiqueClient.PsSpecificPrice.Reduction)
                                                            * (1 - (categorieFamille.FT_Remise.Value / 100)))
                                                            * 100)) / 100);
                                                }
                                            }
                                        }
                                        #endregion
                                        #region remises par quantité en pourcentage de la catégorie tarifaire
                                        else
                                        {
                                            //Recherche du prix HT de base sur le tarif d'exception du client
                                            categorieArticle = F_ARTCLIENTRepository.Read(F_ARTICLE.AR_Ref, client.CT_Num);

                                            if (categorieArticle != null)
                                            {
                                                price = categorieArticle.AC_PrixVen.Value;

                                                if (price == 0 && categorieArticle.AC_Coef.Value > 0 && F_ARTICLE.AR_PrixAch > 0)
                                                    price = (F_ARTICLE.AR_PrixAch.Value * categorieArticle.AC_Coef.Value);

                                                if (price > 0 && categorieArticle.AC_PrixTTC == 1 && PsTax != null)
                                                    price = (price / (1 + (PsTax.Rate / 100))) - Product.ecotaxe_htsage;
                                            }

                                            categorieClient = null;
                                            //Recherche du prix HT de base sur la catégorie tarifaire correspondant à celle de la fiche client
                                            if (price == 0 && client != null)
                                            {
                                                //Récupération de la catégorie tarifaire de la fiche client
                                                categorieClient = F_ARTCLIENTRepository.ReadReferenceCategorie(
                                                F_ARTICLE.AR_Ref, client.N_CatTarif.Value);
                                            }
                                            price = GetHTPrice(F_ARTICLE, categorieClient, PsTax, Product);


                                            List<Model.Sage.F_FAMTARIFQTE> remises = new List<Model.Sage.F_FAMTARIFQTE>(
                                                F_FAMTARIFQTERepository.ListReferenceCategorieType(F_FAMILLE.FA_CodeFamille, String.Format("a{0}", Convert.ToInt16(client.N_CatTarif).ToString("00")))
                                                .OrderBy(result => result.FQ_BorneSup));

                                            if (remises.Count(result => result.FQ_Remise01REM_Valeur.Value > 0) > 0)
                                            {
                                                remiseIndex = 0;

                                                //Parcours toutes les remises définies pour la catégorie tarifaire mappée et pour l'article en cours
                                                for (int i = 0; i < remises.Count; i++)
                                                {
                                                    Model.Sage.F_FAMTARIFQTE remiseArticle = remises[i];
                                                    remise = remiseArticle.FQ_Remise01REM_Valeur.Value;

                                                    if (remiseArticle.FQ_Remise01REM_Type != 1)
                                                        remise = 0;

                                                    if ((remise > 0) || (remise == 0 && remiseIndex > 0 && remises[i - 1].FQ_Remise01REM_Valeur.Value > 0))
                                                    {
                                                        clientKey = new client_key(0, client.cbMarq, client.N_CatTarif.Value, precedent);

                                                        Model.Prestashop.PsSpecificPrice palier = null;
                                                        foreach (var prixSpecifiqueClient in prixSpecifiqueClients.Where(ck => ck.product_attribute == clientKey.product_attribute && ck.tiers_cbMarq == clientKey.tiers_cbMarq))
                                                            if (prixSpecifiqueClient.PsSpecificPrice.FromQuantity <= precedent &&
                                                                (palier == null || palier.FromQuantity <= prixSpecifiqueClient.PsSpecificPrice.FromQuantity))
                                                                palier = prixSpecifiqueClient.PsSpecificPrice;

                                                        #region insert
                                                        if (palier != null && !Contains(prixSpecifiqueClients, clientKey))
                                                        {
                                                            clientKey.PsSpecificPrice = new Model.Prestashop.PsSpecificPrice()
                                                            {
                                                                IDProduct = Product.IDProduct,
                                                                IDCurrency = 0,
                                                                IDCountry = 0,
                                                                IDGroup = 0,
                                                                Price = palier.Price,
                                                                ReductionType = "percentage",
                                                                Reduction = palier.Reduction,
                                                                FromQuantity = (uint)precedent,
                                                                IDShop = Global.CurrentShop.IDShop,
                                                                IDShopGroup = Global.CurrentShop.IDShopGroup,
                                                                IDCart = 0,
                                                                IDCustomer = (uint)Customer.Pre_Id,
                                                                IDProductAttribute = 0,
                                                                IDSpecificPriceRule = 0,
                                                            };
                                                            clientKey.remise_famillecategorie = clientKey.PsSpecificPrice.Reduction;
                                                            prixSpecifiqueClients.Add(clientKey);
                                                        }
                                                        #endregion

                                                        remiseIndex = i;
                                                    }

                                                    if (remiseArticle.FQ_BorneSup != null && remiseArticle.FQ_BorneSup != 0)
                                                        precedent = Convert.ToInt64(remiseArticle.FQ_BorneSup.Value) + 1;
                                                }
                                            }

                                            precedent = 1;
                                            remise = 0;

                                            List<client_key> pallier_qte_client_avec_remise_famille = new List<client_key>();

                                            if (remises.Count(result => result.FQ_Remise01REM_Valeur.Value > 0) > 0)
                                            {
                                                remiseIndex = 0;

                                                //Parcours toutes les remises définies pour la catégorie tarifaire mappée et pour l'article en cours
                                                for (int i = 0; i < remises.Count; i++)
                                                {
                                                    Model.Sage.F_FAMTARIFQTE remiseArticle = remises[i];
                                                    remise = remiseArticle.FQ_Remise01REM_Valeur.Value;

                                                    if (remiseArticle.FQ_Remise01REM_Type != 1)
                                                        remise = 0;

                                                    if ((remise > 0) || (remise == 0 && remiseIndex > 0 && remises[i - 1].FQ_Remise01REM_Valeur.Value > 0))
                                                    {
                                                        clientKey = new client_key(0, client.cbMarq, client.N_CatTarif.Value, precedent);

                                                        if (remiseArticle.FQ_Remise01REM_Type == 1)
                                                        {
                                                            Decimal CurrentPriceArticle = price * ((100 - remiseArticle.FQ_Remise01REM_Valeur.Value) / 100);
                                                            if (remiseArticle.FQ_Remise02REM_Valeur != null && remiseArticle.FQ_Remise02REM_Valeur > 0 && remiseArticle.FQ_Remise02REM_Type == 1)
                                                            {
                                                                Decimal SecondPriceArticle = CurrentPriceArticle * ((100 - remiseArticle.FQ_Remise02REM_Valeur.Value) / 100);
                                                                remise = 100 - (SecondPriceArticle / price) * 100;
                                                                if (remiseArticle.FQ_Remise03REM_Valeur != null && remiseArticle.FQ_Remise03REM_Valeur > 0 && remiseArticle.FQ_Remise02REM_Type == 1)
                                                                {
                                                                    Decimal ThirdPriceArticle = SecondPriceArticle * ((100 - remiseArticle.FQ_Remise03REM_Valeur.Value) / 100);
                                                                    remise = 100 - (ThirdPriceArticle / price) * 100;
                                                                }
                                                            }
                                                        }

                                                        foreach (client_key c in prixSpecifiqueClients.Where(ck => ck.tiers_cbMarq == clientKey.tiers_cbMarq
                                                            && ck.PsSpecificPrice.FromQuantity <= precedent))
                                                        {
                                                            if (!pallier_qte_client_avec_remise_famille.Contains(c))
                                                            {
                                                                if (mode == RemiseMode.RemiseCumuleeCatFamille)
                                                                {
                                                                    c.PsSpecificPrice.Reduction += remise / 100;
                                                                }
                                                                else if (mode == RemiseMode.RemiseEnCascadeCatFamille)
                                                                {
                                                                    if (c.PsSpecificPrice.Price != 0)
                                                                        c.PsSpecificPrice.Reduction = ((100 - (((
                                                                            (c.PsSpecificPrice.Price * (1 - c.PsSpecificPrice.Reduction))
                                                                            * (1 - (remise / 100)))
                                                                            * 100) / c.PsSpecificPrice.Price)) / 100);
                                                                    else
                                                                        c.PsSpecificPrice.Reduction = ((100 - ((
                                                                            (1 - c.PsSpecificPrice.Reduction)
                                                                            * (1 - (remise / 100)))
                                                                            * 100)) / 100);
                                                                }
                                                                pallier_qte_client_avec_remise_famille.Add(c);
                                                            }
                                                        }

                                                        // si dernier pallier application aux palliers existants restants
                                                        if (i == remises.Count - 1)
                                                            foreach (client_key c in prixSpecifiqueClients.Where(ck => ck.tiers_cbMarq == clientKey.tiers_cbMarq
                                                                && ck.PsSpecificPrice.FromQuantity >= precedent
                                                                && pallier_qte_client_avec_remise_famille.Count(p => p == ck) == 0))
                                                            {
                                                                if (!pallier_qte_client_avec_remise_famille.Contains(c))
                                                                {
                                                                    if (mode == RemiseMode.RemiseCumuleeCatFamille)
                                                                    {
                                                                        c.PsSpecificPrice.Reduction += remise / 100;
                                                                    }
                                                                    else if (mode == RemiseMode.RemiseEnCascadeCatFamille)
                                                                    {
                                                                        if (c.PsSpecificPrice.Price != 0)
                                                                            c.PsSpecificPrice.Reduction = ((100 - (((
                                                                                (c.PsSpecificPrice.Price * (1 - c.PsSpecificPrice.Reduction))
                                                                                * (1 - (remise / 100)))
                                                                                * 100) / c.PsSpecificPrice.Price)) / 100);
                                                                        else
                                                                            c.PsSpecificPrice.Reduction = ((100 - ((
                                                                                (1 - c.PsSpecificPrice.Reduction)
                                                                                * (1 - (remise / 100)))
                                                                                * 100)) / 100);
                                                                    }
                                                                    pallier_qte_client_avec_remise_famille.Add(c);
                                                                }
                                                            }

                                                        remiseIndex = i;
                                                    }

                                                    if (remiseArticle.FQ_BorneSup != null && remiseArticle.FQ_BorneSup != 0)
                                                        precedent = Convert.ToInt64(remiseArticle.FQ_BorneSup.Value) + 1;
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                }
                            }
                        }
                        #endregion

                        centrales = null;
                        F_COMPTET = null;
                        Customer = null;
                        Tiers = null;
                        PsTiers = null;
                        categorieArticle = null;
                        categorieClient = null;
                        categorieFamille = null;
                        F_FAMILLE = null;
                        ListClientRemiseOrTarifException = null;
                    }

                    #endregion
                    if (Core.Global.GetConfig().ChronoSynchroStockPriceActif)
                        log_chrono.Add("213-RemisesFamilleClient;" + F_ARTICLE.AR_Ref + ";" + (DateTime.UtcNow - inter).ToString()); inter = DateTime.UtcNow;

                    #region Remises famille par catégorie tarifaire

                    if (mode == RemiseMode.RemiseFamille || mode == RemiseMode.RemiseCumuleeCatFamille || mode == RemiseMode.RemiseEnCascadeCatFamille)
                    {
                        //Récupération de la famille de l'article
                        Model.Sage.F_FAMILLE F_FAMILLE = F_FAMILLERepository.Read(F_ARTICLE);

                        //Parcours toutes les catégories tarifaires mappées
                        Model.Sage.F_FAMTARIF categorieFamille;
                        Model.Sage.F_ARTCLIENT categorieArticle;
                        foreach (Model.Local.Group group in GroupRepository.List())
                        {
                            categorieFamille = null;
                            categorieArticle = null;
                            if (group.Grp_CatTarifId.HasValue && group.Grp_CatTarifId.Value > 0
                                && PsGroupRepository.ExistGroup(group.Grp_Pre_Id))
                            {
                                //Récupération de la tarification de la famille
                                categorieFamille = F_FAMTARIFRepository.ReadReferenceCategorie(
                                    F_FAMILLE.FA_CodeFamille, group.Grp_CatTarifId.Value);

                                if (categorieFamille != null)
                                {
                                    precedent = 1;
                                    remise = 0;

                                    //Recherche du prix HT de base sur la catégorie tarifaire de l'article
                                    categorieArticle = F_ARTCLIENTRepository.ReadReferenceCategorie(
                                        F_ARTICLE.AR_Ref, group.Grp_CatTarifId.Value);

                                    price = GetHTPrice(F_ARTICLE, categorieArticle, PsTax, Product);

                                    if (categorieFamille.FT_TypeRem == (short)ABSTRACTION_SAGE.F_ARTCLIENT.Obj._Enum_AC_TypeRem.Hors_Remise)
                                    {
                                        if (!offDiscountCategoryFamille.Contains(group.Grp_CatTarifId.Value))
                                            offDiscountCategoryFamille.Add(group.Grp_CatTarifId.Value);
                                    }
                                    #region remise simple en pourcentage
                                    else if (categorieFamille.FT_Remise != null && categorieFamille.FT_Remise > 0)
                                    {
                                        #region insert
                                        groupKey = new group_key(0, group.Grp_Pre_Id, group.Grp_CatTarifId, precedent, Product.Price);
                                        groupKey.PsSpecificPrice = new Model.Prestashop.PsSpecificPrice()
                                        {
                                            IDProduct = Product.IDProduct,
                                            IDCurrency = 0,
                                            IDCountry = 0,
                                            IDGroup = (ushort)group.Grp_Pre_Id,
                                            Price = price,
                                            ReductionType = "percentage",
                                            Reduction = categorieFamille.FT_Remise.Value / 100,
                                            FromQuantity = (ushort)precedent,
                                            IDShop = Global.CurrentShop.IDShop,
                                            IDShopGroup = Global.CurrentShop.IDShopGroup,
                                            IDCart = 0,
                                            IDCustomer = 0,
                                            IDProductAttribute = 0,
                                            IDSpecificPriceRule = 0,
                                        };
                                        groupKey.remise_famillecategorie = groupKey.PsSpecificPrice.Reduction;

                                        if (mode == RemiseMode.RemiseFamille)
                                        {
                                            if (!Contains(prixSpecifiqueGroupes, groupKey))
                                                prixSpecifiqueGroupes.Add(groupKey);
                                        }
                                        else if (mode == RemiseMode.RemiseCumuleeCatFamille || mode == RemiseMode.RemiseEnCascadeCatFamille)
                                        {
                                            if (!Contains(prixSpecifiqueGroupes, groupKey))
                                            {
                                                prixSpecifiqueGroupes.Add(groupKey);
                                            }
                                            else if (mode == RemiseMode.RemiseCumuleeCatFamille || mode == RemiseMode.RemiseEnCascadeCatFamille)
                                            {
                                                foreach (group_key g in prixSpecifiqueGroupes.Where(gk => gk.prestashop_group == groupKey.prestashop_group && gk.cattarif_cbMarq == groupKey.cattarif_cbMarq))
                                                {
                                                    g.remise_famillecategorie = groupKey.remise_famillecategorie;
                                                    if (mode == RemiseMode.RemiseCumuleeCatFamille)
                                                    {
                                                        g.PsSpecificPrice.Reduction += categorieFamille.FT_Remise.Value / 100;
                                                    }
                                                    else if (mode == RemiseMode.RemiseEnCascadeCatFamille)
                                                    {
                                                        if (g.PsSpecificPrice.Price != 0)
                                                            g.PsSpecificPrice.Reduction = ((100 - (((
                                                                (g.PsSpecificPrice.Price * (1 - g.PsSpecificPrice.Reduction))
                                                                * (1 - (categorieFamille.FT_Remise.Value / 100)))
                                                                * 100) / g.PsSpecificPrice.Price)) / 100);
                                                        else
                                                            g.PsSpecificPrice.Reduction = ((100 - ((
                                                                (1 - g.PsSpecificPrice.Reduction)
                                                                * (1 - (categorieFamille.FT_Remise.Value / 100)))
                                                                * 100)) / 100);
                                                    }
                                                }
                                                //foreach (client_key c in prixSpecifiqueClients.Where(ck => rateFamilleClients.Count(f => f == ck.tiers_cbMarq) == 0))
                                                //{
                                                //    if (mode == RemiseMode.RemiseCumulee)
                                                //    {
                                                //        c.PsSpecificPrice.Reduction += categorieFamille.FT_Remise.Value / 100;
                                                //    }
                                                //    else if (mode == RemiseMode.RemiseEnCascade)
                                                //    {
                                                //        c.PsSpecificPrice.Reduction = ((100 - (((
                                                //            (c.PsSpecificPrice.Price * (1 - c.PsSpecificPrice.Reduction))
                                                //            * (1 - (categorieFamille.FT_Remise.Value / 100)))
                                                //            * 100) / c.PsSpecificPrice.Price)) / 100);
                                                //    }
                                                //}
                                            }
                                        }
                                        #endregion
                                    }
                                    #endregion
                                    #region remises par quantité en pourcentage
                                    else
                                    {
                                        List<Model.Sage.F_FAMTARIFQTE> remises = new List<Model.Sage.F_FAMTARIFQTE>(
                                            F_FAMTARIFQTERepository.ListReferenceCategorieType(F_FAMILLE.FA_CodeFamille, String.Format("a{0}", Convert.ToInt16(group.Grp_CatTarifId).ToString("00")))
                                            .OrderBy(result => result.FQ_BorneSup));

                                        if (remises.Count(result => result.FQ_Remise01REM_Valeur.Value > 0) > 0)
                                        {
                                            remiseIndex = 0;

                                            //Parcours toutes les remises définit pour la catégorie tarifaire mappé et pour l'article en cours
                                            for (int i = 0; i < remises.Count; i++)
                                            {
                                                Model.Sage.F_FAMTARIFQTE remiseArticle = remises[i];
                                                remise = remiseArticle.FQ_Remise01REM_Valeur.Value;

                                                if (remiseArticle.FQ_Remise01REM_Type != 1)
                                                    remise = 0;

                                                if ((remise > 0) || (remise == 0 && remiseIndex > 0 && remises[i - 1].FQ_Remise01REM_Valeur.Value > 0))
                                                {
                                                    groupKey = new group_key(0, group.Grp_Pre_Id, group.Grp_CatTarifId, precedent, Product.Price);

                                                    if (categorieArticle != null)
                                                    {
                                                        //Récupération de la tarification article
                                                        categorieArticle = F_ARTCLIENTRepository.ReadReferenceCategorie(F_ARTICLE.AR_Ref, group.Grp_CatTarifId.Value);

                                                        if (categorieArticle != null)
                                                        {
                                                            Model.Prestashop.PsSpecificPrice palier = null;
                                                            foreach (var prixSpecifiqueGroupe in prixSpecifiqueGroupes.Where(gk => gk.prestashop_group == groupKey.prestashop_group && gk.cattarif_cbMarq == groupKey.cattarif_cbMarq))
                                                                if (prixSpecifiqueGroupe.PsSpecificPrice.FromQuantity <= precedent
                                                                    && (palier == null || palier.FromQuantity <= prixSpecifiqueGroupe.PsSpecificPrice.FromQuantity))
                                                                    palier = prixSpecifiqueGroupe.PsSpecificPrice;

                                                            #region insert
                                                            if (palier != null && !Contains(prixSpecifiqueGroupes, groupKey))
                                                            {
                                                                groupKey.PsSpecificPrice = new Model.Prestashop.PsSpecificPrice()
                                                                {
                                                                    IDProduct = Product.IDProduct,
                                                                    IDCurrency = 0,
                                                                    IDCountry = 0,
                                                                    IDGroup = (ushort)group.Grp_Pre_Id,
                                                                    Price = palier.Price,
                                                                    ReductionType = "percentage",
                                                                    Reduction = palier.Reduction,
                                                                    FromQuantity = (uint)precedent,
                                                                    IDShop = Global.CurrentShop.IDShop,
                                                                    IDShopGroup = Global.CurrentShop.IDShopGroup,
                                                                    IDCart = 0,
                                                                    IDCustomer = 0,
                                                                    IDProductAttribute = 0,
                                                                    IDSpecificPriceRule = 0,
                                                                };
                                                                groupKey.remise_articlecategorie = groupKey.PsSpecificPrice.Reduction;
                                                                prixSpecifiqueGroupes.Add(groupKey);
                                                            }
                                                            #endregion
                                                        }
                                                    }

                                                    remiseIndex = i;
                                                }

                                                if (remiseArticle.FQ_BorneSup != null && remiseArticle.FQ_BorneSup != 0)
                                                    precedent = Convert.ToInt64(remiseArticle.FQ_BorneSup.Value) + 1;
                                            }
                                        }

                                        precedent = 1;
                                        remise = 0;

                                        List<group_key> pallier_qte_group_avec_remise_famille = new List<group_key>();

                                        if (remises.Count(result => result.FQ_Remise01REM_Valeur.Value > 0) > 0)
                                        {
                                            remiseIndex = 0;

                                            //Parcours toutes les remises définit pour la catégorie tarifaire mappé et pour l'article en cours
                                            for (int i = 0; i < remises.Count; i++)
                                            {
                                                Model.Sage.F_FAMTARIFQTE remiseArticle = remises[i];
                                                remise = remiseArticle.FQ_Remise01REM_Valeur.Value;

                                                if (remiseArticle.FQ_Remise01REM_Type != 1)
                                                    remise = 0;

                                                if ((remise > 0) || (remise == 0 && remiseIndex > 0 && remises[i - 1].FQ_Remise01REM_Valeur.Value > 0))
                                                {
                                                    groupKey = new group_key(0, group.Grp_Pre_Id, group.Grp_CatTarifId, precedent, Product.Price);

                                                    if (remiseArticle.FQ_Remise01REM_Type == 1)
                                                    {
                                                        Decimal CurrentPriceArticle = price * ((100 - remiseArticle.FQ_Remise01REM_Valeur.Value) / 100);
                                                        if (remiseArticle.FQ_Remise02REM_Valeur != null && remiseArticle.FQ_Remise02REM_Valeur > 0 && remiseArticle.FQ_Remise02REM_Type == 1)
                                                        {
                                                            Decimal SecondPriceArticle = CurrentPriceArticle * ((100 - remiseArticle.FQ_Remise02REM_Valeur.Value) / 100);
                                                            remise = 100 - (SecondPriceArticle / price) * 100;
                                                            if (remiseArticle.FQ_Remise03REM_Valeur != null && remiseArticle.FQ_Remise03REM_Valeur > 0 && remiseArticle.FQ_Remise02REM_Type == 1)
                                                            {
                                                                Decimal ThirdPriceArticle = SecondPriceArticle * ((100 - remiseArticle.FQ_Remise03REM_Valeur.Value) / 100);
                                                                remise = 100 - (ThirdPriceArticle / price) * 100;
                                                            }
                                                        }
                                                    }

                                                    #region insert
                                                    groupKey.PsSpecificPrice = new Model.Prestashop.PsSpecificPrice()
                                                    {
                                                        IDProduct = Product.IDProduct,
                                                        IDCurrency = 0,
                                                        IDCountry = 0,
                                                        IDGroup = (ushort)group.Grp_Pre_Id,
                                                        Price = price,
                                                        ReductionType = "percentage",
                                                        Reduction = remise / 100,
                                                        FromQuantity = (ushort)precedent,
                                                        IDShop = Global.CurrentShop.IDShop,
                                                        IDShopGroup = Global.CurrentShop.IDShopGroup,
                                                        IDCart = 0,
                                                        IDCustomer = 0,
                                                        IDProductAttribute = 0,
                                                        IDSpecificPriceRule = 0,
                                                    };
                                                    groupKey.remise_famillecategorie = groupKey.PsSpecificPrice.Reduction;

                                                    if (mode == RemiseMode.RemiseFamille)
                                                    {
                                                        if (Contains(prixSpecifiqueGroupes, groupKey))
                                                        {
                                                            foreach (group_key g in prixSpecifiqueGroupes.Where(gk => gk.prestashop_group == groupKey.prestashop_group && gk.cattarif_cbMarq == groupKey.cattarif_cbMarq && gk.PsSpecificPrice.FromQuantity <= precedent))
                                                            {
                                                                if (!pallier_qte_group_avec_remise_famille.Contains(g))
                                                                {
                                                                    g.remise_famillecategorie = remise / 100;
                                                                    g.PsSpecificPrice.Reduction = remise / 100;
                                                                    pallier_qte_group_avec_remise_famille.Add(g);
                                                                }
                                                            }
                                                        }
                                                        else
                                                            prixSpecifiqueGroupes.Add(groupKey);
                                                    }
                                                    else if (mode == RemiseMode.RemiseCumuleeCatFamille || mode == RemiseMode.RemiseEnCascadeCatFamille)
                                                    {
                                                        foreach (group_key g in prixSpecifiqueGroupes.Where(gk => gk.prestashop_group == groupKey.prestashop_group
                                                            && gk.cattarif_cbMarq == groupKey.cattarif_cbMarq
                                                            && gk.PsSpecificPrice.FromQuantity <= precedent))
                                                        {
                                                            if (!pallier_qte_group_avec_remise_famille.Contains(g))
                                                            {
                                                                g.remise_famillecategorie = remise / 100;
                                                                if (mode == RemiseMode.RemiseCumuleeCatFamille)
                                                                {
                                                                    g.PsSpecificPrice.Reduction += remise / 100;
                                                                }
                                                                else if (mode == RemiseMode.RemiseEnCascadeCatFamille)
                                                                {
                                                                    if (g.PsSpecificPrice.Price != 0)
                                                                        g.PsSpecificPrice.Reduction = ((100 - (((
                                                                            (g.PsSpecificPrice.Price * (1 - g.PsSpecificPrice.Reduction))
                                                                            * (1 - (remise / 100)))
                                                                            * 100) / g.PsSpecificPrice.Price)) / 100);
                                                                    else
                                                                        g.PsSpecificPrice.Reduction = ((100 - ((
                                                                            (1 - g.PsSpecificPrice.Reduction)
                                                                            * (1 - (remise / 100)))
                                                                            * 100)) / 100);
                                                                }
                                                                pallier_qte_group_avec_remise_famille.Add(g);
                                                            }
                                                        }

                                                        // si dernier pallier application aux palliers existants restants
                                                        if (i == remises.Count - 1)
                                                            foreach (group_key g in prixSpecifiqueGroupes.Where(gk => gk.prestashop_group == groupKey.prestashop_group
                                                                && gk.cattarif_cbMarq == groupKey.cattarif_cbMarq
                                                                && gk.PsSpecificPrice.FromQuantity >= precedent
                                                                && pallier_qte_group_avec_remise_famille.Count(p => p == gk) == 0))
                                                            {
                                                                if (!pallier_qte_group_avec_remise_famille.Contains(g))
                                                                {
                                                                    g.remise_famillecategorie = remise / 100;
                                                                    if (mode == RemiseMode.RemiseCumuleeCatFamille)
                                                                    {
                                                                        g.PsSpecificPrice.Reduction += remise / 100;
                                                                    }
                                                                    else if (mode == RemiseMode.RemiseEnCascadeCatFamille)
                                                                    {
                                                                        if (g.PsSpecificPrice.Price != 0)
                                                                            g.PsSpecificPrice.Reduction = ((100 - (((
                                                                                (g.PsSpecificPrice.Price * (1 - g.PsSpecificPrice.Reduction))
                                                                                * (1 - (remise / 100)))
                                                                                * 100) / g.PsSpecificPrice.Price)) / 100);
                                                                        else
                                                                            g.PsSpecificPrice.Reduction = ((100 - ((
                                                                                (1 - g.PsSpecificPrice.Reduction)
                                                                                * (1 - (remise / 100)))
                                                                                * 100)) / 100);
                                                                    }
                                                                    pallier_qte_group_avec_remise_famille.Add(g);
                                                                }
                                                            }
                                                    }
                                                    #endregion

                                                    remiseIndex = i;
                                                }

                                                if (remiseArticle.FQ_BorneSup != null && remiseArticle.FQ_BorneSup != 0)
                                                    precedent = Convert.ToInt64(remiseArticle.FQ_BorneSup.Value) + 1;
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                        categorieFamille = null;
                        categorieArticle = null;
                        F_FAMILLE = null;
                    }

                    #endregion
                    if (Core.Global.GetConfig().ChronoSynchroStockPriceActif)
                        log_chrono.Add("214-RemisesFamilleCategorieTarifaire;" + F_ARTICLE.AR_Ref + ";" + (DateTime.UtcNow - inter).ToString()); inter = DateTime.UtcNow;

                    #region gestion conflit de remise
                    if (mode == RemiseMode.RemiseEnCascade || mode == RemiseMode.RemiseCumulee || mode == RemiseMode.RemiseCumuleeCatFamille || mode == RemiseMode.RemiseEnCascadeCatFamille)
                    {
                        if (Core.Global.GetConfig().ConflitRemise != RemiseConflit.CumulCascade)
                        {
                            foreach (client_key ck in prixSpecifiqueClients)
                            {
                                if ((mode == RemiseMode.RemiseEnCascade || mode == RemiseMode.RemiseCumulee)
                                    && (ck.horsremise || (offDiscountCategoryArticle.Contains(ck.cattarif_cbMarq.Value) && !rateExceptionClients.Contains(ck.tiers_cbMarq))))
                                {
                                    ck.PsSpecificPrice.Reduction = 0;
                                }
                                else if ((mode == RemiseMode.RemiseEnCascadeCatFamille
                                        || mode == RemiseMode.RemiseCumuleeCatFamille)
                                    && (ck.horsremise
                                        || ((offDiscountCategoryArticle.Contains(ck.cattarif_cbMarq.Value) || offDiscountCategoryFamille.Contains(ck.cattarif_cbMarq.Value))
                                            && !rateExceptionClients.Contains(ck.tiers_cbMarq))))
                                {
                                    ck.PsSpecificPrice.Reduction = 0;
                                }
                                else if (ck.conflitderemise())
                                {
                                    switch (Core.Global.GetConfig().ConflitRemise)
                                    {
                                        case RemiseConflit.MeilleureRemise:
                                            ck.PsSpecificPrice.Reduction = ck.meilleureremise();
                                            break;

                                        case RemiseConflit.PrioriteArticleFamilleClient:
                                            ck.PsSpecificPrice.Reduction = ck.prioritearticlefamilleclient();
                                            break;

                                        case RemiseConflit.CumulCascade:
                                        default:
                                            break;
                                    }
                                }
                            }
                            foreach (group_key gk in prixSpecifiqueGroupes)
                            {
                                if ((mode == RemiseMode.RemiseEnCascadeCatFamille
                                        || mode == RemiseMode.RemiseCumuleeCatFamille)
                                    && (gk.horsremise_article
                                        || offDiscountCategoryFamille.Contains(gk.cattarif_cbMarq.Value)))
                                {
                                    gk.PsSpecificPrice.Reduction = 0;
                                }
                                else if (gk.conflitderemise())
                                {
                                    switch (Core.Global.GetConfig().ConflitRemise)
                                    {
                                        case RemiseConflit.MeilleureRemise:
                                            gk.PsSpecificPrice.Reduction = gk.meilleureremise();
                                            break;

                                        case RemiseConflit.PrioriteArticleFamilleClient:
                                            gk.PsSpecificPrice.Reduction = gk.prioritearticlefamille();
                                            break;

                                        case RemiseConflit.CumulCascade:
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                        if (Core.Global.GetConfig().ChronoSynchroStockPriceActif)
                            log_chrono.Add("215-GestionConflitRemise;" + F_ARTICLE.AR_Ref + ";" + (DateTime.UtcNow - inter).ToString()); inter = DateTime.UtcNow;
                    }
                    #endregion

                    #region Suppression occurences en trop
                    bool delete = false;

                    // <JG> 23/02/2013 suppression des tarifications par déclinaisons si toutes identiques au tarif "Toutes les déclinaisons"
                    #region
                    do
                    {
                        delete = false;
                        List<group_key> remove_keys = new List<group_key>();
                        foreach (group_key gk in prixSpecifiqueGroupes)
                        {
                            if (delete =
                                (gk.PsSpecificPrice.IDProductAttribute != 0
                                && prixSpecifiqueGroupes.Count(p => p.PsSpecificPrice.IDProductAttribute == 0 && p.PsSpecificPrice.FromQuantity == gk.PsSpecificPrice.FromQuantity && p.PsSpecificPrice.IDCustomer == gk.PsSpecificPrice.IDCustomer && p.PsSpecificPrice.IDGroup == gk.PsSpecificPrice.IDGroup) == 1
                                && prixSpecifiqueGroupes.Count(result =>
                                    result.PsSpecificPrice.IDProductAttribute != 0 &&
                                    result.PsSpecificPrice.FromQuantity == gk.PsSpecificPrice.FromQuantity &&
                                    result.PsSpecificPrice.IDCustomer == gk.PsSpecificPrice.IDCustomer &&
                                    result.PsSpecificPrice.IDGroup == gk.PsSpecificPrice.IDGroup)
                                == (prixSpecifiqueGroupes.Count(result =>
                                    result.PsSpecificPrice.IDProductAttribute != 0 &&
                                    result.PsSpecificPrice.FromQuantity == gk.PsSpecificPrice.FromQuantity &&
                                    result.PsSpecificPrice.IDCustomer == gk.PsSpecificPrice.IDCustomer &&
                                    result.PsSpecificPrice.IDGroup == gk.PsSpecificPrice.IDGroup &&
                                    result.PsSpecificPrice.Price == (prixSpecifiqueGroupes.FirstOrDefault(p => p.PsSpecificPrice.IDProductAttribute == 0
                                                                                                && p.PsSpecificPrice.FromQuantity == gk.PsSpecificPrice.FromQuantity
                                                                                                && p.PsSpecificPrice.IDCustomer == gk.PsSpecificPrice.IDCustomer
                                                                                                && p.PsSpecificPrice.IDGroup == gk.PsSpecificPrice.IDGroup).PsSpecificPrice.Price) &&
                                    result.PsSpecificPrice.Reduction == (prixSpecifiqueGroupes.FirstOrDefault(p => p.PsSpecificPrice.IDProductAttribute == 0
                                                                                                && p.PsSpecificPrice.FromQuantity == gk.PsSpecificPrice.FromQuantity
                                                                                                && p.PsSpecificPrice.IDCustomer == gk.PsSpecificPrice.IDCustomer
                                                                                                && p.PsSpecificPrice.IDGroup == gk.PsSpecificPrice.IDGroup).PsSpecificPrice.Reduction)))))
                            {
                                if (!remove_keys.Contains(gk))
                                    remove_keys.Add(gk);
                                //prixSpecifiqueGroupes.Remove(PsSpecificPrice.Key);
                            }
                        }
                        foreach (group_key key in remove_keys)
                            prixSpecifiqueGroupes.Remove(key);
                    } while (delete);
                    #endregion

                    //Supprime tous les prix spécifiques "Toutes les déclinaisons" des groupes qui ont des déclinaisons
                    #region
                    do
                    {
                        delete = false;

                        foreach (group_key gk in prixSpecifiqueGroupes)
                        {
                            if (delete = (gk.PsSpecificPrice.IDProductAttribute == 0 && prixSpecifiqueGroupes.Count(result =>
                                result.PsSpecificPrice.IDProductAttribute != 0 &&
                                result.PsSpecificPrice.FromQuantity == gk.PsSpecificPrice.FromQuantity &&
                                result.PsSpecificPrice.IDCustomer == gk.PsSpecificPrice.IDCustomer &&
                                result.PsSpecificPrice.IDGroup == gk.PsSpecificPrice.IDGroup) > 0))
                            {
                                prixSpecifiqueGroupes.Remove(gk);
                                break;
                            }
                        }
                    } while (delete);
                    #endregion

                    // <JG> 22/02/2013 suppression des tarifications par déclinaisons si toutes identiques au tarif "Toutes les déclinaisons"
                    #region
                    do
                    {
                        delete = false;
                        List<client_key> remove_keys = new List<client_key>();
                        foreach (client_key ck in prixSpecifiqueClients)
                        {
                            if (delete =
                                (ck.PsSpecificPrice.IDProductAttribute != 0
                                && prixSpecifiqueClients.Count(p => p.PsSpecificPrice.IDProductAttribute == 0 &&
                                    p.PsSpecificPrice.FromQuantity == ck.PsSpecificPrice.FromQuantity &&
                                    p.PsSpecificPrice.IDCustomer == ck.PsSpecificPrice.IDCustomer) == 1
                                && prixSpecifiqueClients.Count(result =>
                                    result.PsSpecificPrice.IDProductAttribute != 0 &&
                                    result.PsSpecificPrice.FromQuantity == ck.PsSpecificPrice.FromQuantity &&
                                    result.PsSpecificPrice.IDCustomer == ck.PsSpecificPrice.IDCustomer)
                                == (prixSpecifiqueClients.Count(result =>
                                    result.PsSpecificPrice.IDProductAttribute != 0 &&
                                    result.PsSpecificPrice.FromQuantity == ck.PsSpecificPrice.FromQuantity &&
                                    result.PsSpecificPrice.IDCustomer == ck.PsSpecificPrice.IDCustomer &&
                                    result.PsSpecificPrice.Price == (prixSpecifiqueClients.FirstOrDefault(p => p.PsSpecificPrice.IDProductAttribute == 0 && p.PsSpecificPrice.FromQuantity == ck.PsSpecificPrice.FromQuantity && p.PsSpecificPrice.IDCustomer == ck.PsSpecificPrice.IDCustomer).PsSpecificPrice.Price) &&
                                    result.PsSpecificPrice.Reduction == (prixSpecifiqueClients.FirstOrDefault(p => p.PsSpecificPrice.IDProductAttribute == 0 && p.PsSpecificPrice.FromQuantity == ck.PsSpecificPrice.FromQuantity && p.PsSpecificPrice.IDCustomer == ck.PsSpecificPrice.IDCustomer).PsSpecificPrice.Reduction)))
                                && ((ck.PsSpecificPrice.IDProductAttribute != 0
                                    && prixSpecifiqueGroupes.Count(result => result.PsSpecificPrice.IDProductAttribute == 0) > 0)
                                    || (ck.PsSpecificPrice.IDProductAttribute == 0
                                        && prixSpecifiqueGroupes.Count(result => result.PsSpecificPrice.IDProductAttribute != 0) > 0))))
                            {
                                if (!remove_keys.Contains(ck))
                                    remove_keys.Add(ck);
                                //prixSpecifiqueClients.Remove(PsSpecificPrice.Key);
                            }
                        }
                        foreach (var key in remove_keys)
                            prixSpecifiqueClients.Remove(key);
                    } while (delete);
                    #endregion

                    //Supprime tous les prix spécifiques "Toutes les déclinaisons" des clients qui ont des déclinaisons
                    #region
                    do
                    {
                        delete = false;

                        foreach (client_key ck in prixSpecifiqueClients)
                        {
                            if (delete = (ck.PsSpecificPrice.IDProductAttribute == 0 && prixSpecifiqueClients.Count(result =>
                                result.PsSpecificPrice.IDProductAttribute != 0 &&
                                result.PsSpecificPrice.FromQuantity == ck.PsSpecificPrice.FromQuantity &&
                                result.PsSpecificPrice.IDCustomer == ck.PsSpecificPrice.IDCustomer) > 0))
                            {
                                prixSpecifiqueClients.Remove(ck);
                                break;
                            }
                        }
                    } while (delete);
                    #endregion

                    // <JG> 21/03/2013 ajout suppression des remises avec prix et remise à zero 
                    #region
                    do
                    {
                        delete = false;
                        foreach (client_key ck in prixSpecifiqueClients)
                        {
                            if (delete = (ck.PsSpecificPrice.Price == 0 && ck.PsSpecificPrice.Reduction == 0
                                && prixSpecifiqueClients.Count(result => result.PsSpecificPrice.IDProduct == ck.PsSpecificPrice.IDProduct
                                                                && result.PsSpecificPrice.IDCustomer == ck.PsSpecificPrice.IDCustomer
                                                                && result.PsSpecificPrice.IDGroup == ck.PsSpecificPrice.IDGroup
                                                                && result.PsSpecificPrice.IDProductAttribute == ck.PsSpecificPrice.IDProductAttribute) == 1))
                            {
                                prixSpecifiqueClients.Remove(ck);
                                break;
                            }
                        }
                    }
                    while (delete);

                    do
                    {
                        delete = false;
                        foreach (group_key gk in prixSpecifiqueGroupes)
                        {
                            if (delete = (gk.PsSpecificPrice.Price == 0 && gk.PsSpecificPrice.Reduction == 0
                                && prixSpecifiqueGroupes.Count(result => result.PsSpecificPrice.IDProduct == gk.PsSpecificPrice.IDProduct
                                                                && result.PsSpecificPrice.IDGroup == gk.PsSpecificPrice.IDGroup
                                                                && result.PsSpecificPrice.IDProductAttribute == gk.PsSpecificPrice.IDProductAttribute) == 1))
                            {
                                prixSpecifiqueGroupes.Remove(gk);
                                break;
                            }
                        }
                    }
                    while (delete);
                    #endregion

                    // <JG> 09/06/2014 ajout suppression des lignes unique par client ou groupe sans remises et avec le même prix que la fiche produit
                    #region

                    do
                    {
                        delete = false;
                        foreach (client_key ck in prixSpecifiqueClients)
                        {
                            if (delete = (ck.PsSpecificPrice.Price == Product.Price && ck.PsSpecificPrice.Reduction == 0
                                && prixSpecifiqueClients.Count(result => result.PsSpecificPrice.IDProduct == ck.PsSpecificPrice.IDProduct
                                                                && result.PsSpecificPrice.IDCustomer == ck.PsSpecificPrice.IDCustomer
                                                                && result.PsSpecificPrice.IDGroup == ck.PsSpecificPrice.IDGroup
                                                                && result.PsSpecificPrice.IDProductAttribute == ck.PsSpecificPrice.IDProductAttribute) == 1))
                            {
                                prixSpecifiqueClients.Remove(ck);
                                break;
                            }
                        }
                    }
                    while (delete);

                    do
                    {
                        delete = false;
                        foreach (group_key gk in prixSpecifiqueGroupes)
                        {
                            if (delete = (gk.PsSpecificPrice.Price == Product.Price && gk.PsSpecificPrice.Reduction == 0
                                && prixSpecifiqueGroupes.Count(result => result.PsSpecificPrice.IDProduct == gk.PsSpecificPrice.IDProduct
                                                                && result.PsSpecificPrice.IDGroup == gk.PsSpecificPrice.IDGroup
                                                                && result.PsSpecificPrice.IDProductAttribute == gk.PsSpecificPrice.IDProductAttribute) == 1))
                            {
                                prixSpecifiqueGroupes.Remove(gk);
                                break;
                            }
                        }
                    }
                    while (delete);

                    #endregion

                    // <JG> 16/09/2016 ajout suppression des lignes unique par client ou groupe sans remises et avec le même prix que la déclinaison
                    #region

                    //do
                    //{
                    //    delete = false;
                    //    foreach (client_key ck in prixSpecifiqueClients)
                    //    {
                    //        if (delete = (ck.PsSpecificPrice.Price == ck.enumere_price_basic && ck.PsSpecificPrice.Reduction == 0
                    //            && prixSpecifiqueClients.Count(result => result.PsSpecificPrice.IDProduct == ck.PsSpecificPrice.IDProduct
                    //                                            && result.PsSpecificPrice.IDCustomer == ck.PsSpecificPrice.IDCustomer
                    //                                            && result.PsSpecificPrice.IDGroup == ck.PsSpecificPrice.IDGroup
                    //                                            && result.PsSpecificPrice.IDProductAttribute == ck.PsSpecificPrice.IDProductAttribute) == 1))
                    //        {
                    //            prixSpecifiqueClients.Remove(ck);
                    //            break;
                    //        }
                    //    }
                    //}
                    //while (delete);

                    do
                    {
                        delete = false;
                        foreach (group_key gk in prixSpecifiqueGroupes)
                        {
                            if (delete = (gk.PsSpecificPrice.Price == gk.enumere_price_basic && gk.PsSpecificPrice.Reduction == 0
                                && prixSpecifiqueGroupes.Count(result => result.PsSpecificPrice.IDProduct == gk.PsSpecificPrice.IDProduct
                                                                && result.PsSpecificPrice.IDGroup == gk.PsSpecificPrice.IDGroup
                                                                && result.PsSpecificPrice.IDProductAttribute == gk.PsSpecificPrice.IDProductAttribute) == 1))
                            {
                                prixSpecifiqueGroupes.Remove(gk);
                                break;
                            }
                        }
                    }
                    while (delete);

                    #endregion

                    #endregion

                    // <JG> 18/03/2014 ajout détection multi-boutique ou non
                    uint ID_Shop = 0;
                    uint ID_ShopGroup = 0;

                    Model.Prestashop.PsShopGroupRepository PsShopGroupRepository = new Model.Prestashop.PsShopGroupRepository();
                    Model.Prestashop.PsShopRepository PsShopRepository = new Model.Prestashop.PsShopRepository();
                    if (PsShopGroupRepository.Count() > 1 || PsShopRepository.Count() > 1)
                    {
                        ID_Shop = Core.Global.CurrentShop.IDShop;
                        ID_ShopGroup = Core.Global.CurrentShop.IDShopGroup;
                    }
                    if (Core.Global.GetConfig().ChronoSynchroStockPriceActif)
                        log_chrono.Add("220-ControleDonnees;" + F_ARTICLE.AR_Ref + ";" + (DateTime.UtcNow - inter).ToString()); inter = DateTime.UtcNow;

                    //Supprime tous les prix spécifiques de l'article
                    if (Article.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleComposition && CompositionArticle != null)
                    {
                        PsSpecificPriceRepository.DeleteFromProductAttribute(Product.IDProduct, CompositionArticle.Pre_Id.Value);
                    }
                    else
                    {
                        PsSpecificPriceRepository.DeleteFromProduct(Product.IDProduct);
                    }
                    if (Core.Global.GetConfig().ChronoSynchroStockPriceActif)
                        log_chrono.Add("221-PsSpecificPrice.DeleteFromProduct;" + F_ARTICLE.AR_Ref + ";" + (DateTime.UtcNow - inter).ToString()); inter = DateTime.UtcNow;

                    #region Envoi Prestashop
                    // <JG> ajout ordre de tri selon Groupe/Client/Declinaison/Qte
                    foreach (client_key ck in prixSpecifiqueClients)
                    {
                        if (ck.PsSpecificPrice.Price == 0)
                            ck.PsSpecificPrice.Price = -1;
                        ck.PsSpecificPrice.IDShop = ID_Shop;
                        ck.PsSpecificPrice.IDShopGroup = ID_ShopGroup;
                        if (Article.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleComposition && CompositionArticle != null)
                        {
                            ck.PsSpecificPrice.IDProduct = (uint)Article.Pre_Id.Value;
                            ck.PsSpecificPrice.IDProductAttribute = (uint)CompositionArticle.Pre_Id.Value;
                        }
                        if (Core.Global.GetConfig().ArticleSpecificPriceLetBasePriceRule)
                            if (ck.PsSpecificPrice.Price == Product.Price
                                && ck.PsSpecificPrice.Reduction > 0
                                && ck.PsSpecificPrice.ReductionType == Model.Prestashop.PsSpecificPrice._ReductionType_Percentage)
                            {
                                ck.PsSpecificPrice.Price = -1;
								#if (PRESTASHOP_VERSION_161 || PRESTASHOP_VERSION_172)
								ck.PsSpecificPrice.ReductionTax = 1;
								#endif
							}
					}
                    if (Core.Global.GetConfig().ChronoSynchroStockPriceActif)
                        log_chrono.Add("230-TriCustomerPrice;" + F_ARTICLE.AR_Ref + ";" + (DateTime.UtcNow - inter).ToString()); inter = DateTime.UtcNow;

                    PsSpecificPriceRepository.AddList((from t in prixSpecifiqueClients
                                                       orderby t.PsSpecificPrice.IDGroup, t.PsSpecificPrice.IDCustomer, t.PsSpecificPrice.IDProductAttribute, t.PsSpecificPrice.FromQuantity
                                                       select t).Select(t => t.PsSpecificPrice));
                    if (Core.Global.GetConfig().ChronoSynchroStockPriceActif)
                        log_chrono.Add("231-PsSpecificPriceAddCustomerPrice;" + F_ARTICLE.AR_Ref + ";" + (DateTime.UtcNow - inter).ToString()); inter = DateTime.UtcNow;

                    foreach (group_key gk in prixSpecifiqueGroupes)
                    {
                        if (gk.PsSpecificPrice.Price == 0)
                            gk.PsSpecificPrice.Price = -1;
                        gk.PsSpecificPrice.IDShop = ID_Shop;
                        gk.PsSpecificPrice.IDShopGroup = ID_ShopGroup;
                        if (Article.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleComposition && CompositionArticle != null)
                        {
                            gk.PsSpecificPrice.IDProduct = (uint)Article.Pre_Id.Value;
                            gk.PsSpecificPrice.IDProductAttribute = (uint)CompositionArticle.Pre_Id.Value;
                        }
                        if (Core.Global.GetConfig().ArticleSpecificPriceLetBasePriceRule)
                            if (gk.PsSpecificPrice.Price == Product.Price
                                && gk.PsSpecificPrice.Reduction > 0
                                && gk.PsSpecificPrice.ReductionType == Model.Prestashop.PsSpecificPrice._ReductionType_Percentage)
                            {
                                gk.PsSpecificPrice.Price = -1;
								#if (PRESTASHOP_VERSION_161 || PRESTASHOP_VERSION_172)
								gk.PsSpecificPrice.ReductionTax = 1;
								#endif
                            }
                    }
                    if (Core.Global.GetConfig().ChronoSynchroStockPriceActif)
                        log_chrono.Add("240-TriGroupPrice;" + F_ARTICLE.AR_Ref + ";" + (DateTime.UtcNow - inter).ToString()); inter = DateTime.UtcNow;

                    PsSpecificPriceRepository.AddList((from t in prixSpecifiqueGroupes
                                                       orderby t.PsSpecificPrice.IDGroup, t.PsSpecificPrice.IDCustomer, t.PsSpecificPrice.IDProductAttribute, t.PsSpecificPrice.FromQuantity
                                                       select t).Select(t => t.PsSpecificPrice));
                    if (Core.Global.GetConfig().ChronoSynchroStockPriceActif)
                        log_chrono.Add("241-PsSpecificPriceAddGroupPrice;" + F_ARTICLE.AR_Ref + ";" + (DateTime.UtcNow - inter).ToString()); inter = DateTime.UtcNow;

                    PsSpecificPriceRepository.WriteReductionType(Product);
                    PsSpecificPriceRepository.WriteDateToDate(Product);

                    if (Core.Global.GetConfig().ChronoSynchroStockPriceActif)
                        log_chrono.Add("250-ValidationDonneesPrestaShop;" + F_ARTICLE.AR_Ref + ";" + (DateTime.UtcNow - inter).ToString()); inter = DateTime.UtcNow;
                    #endregion

                    #region Dispose
                    PsShopGroupRepository = null;
                    PsShopRepository = null;
                    PsSpecificPriceRepository = null;

                    Tax = null;
                    PsTax = null;

                    groupKey = null;
                    clientKey = null;

                    GroupRepository = null;
                    CustomerRepository = null;
                    F_ARTCLIENTRepository = null;
                    F_TARIFQTERepository = null;
                    F_COMPTETRepository = null;
                    F_FAMILLERepository = null;
                    F_FAMTARIFRepository = null;
                    F_FAMCLIENTRepository = null;
                    F_FAMTARIFQTERepository = null;
                    PsSpecificPriceRepository = null;
                    PsCustomerRepository = null;
                    PsGroupRepository = null;
                    TaxRepository = null;
                    PsTaxRepository = null;
                    prixSpecifiqueClients = null;
                    prixSpecifiqueGroupes = null;
                    rateExceptionClients = null;
                    netPriceDiscountClients = null;
                    netPriceCategory = null;
                    rateFamilleClients = null;

                    #endregion

                }
                if (Core.Global.GetConfig().ChronoSynchroStockPriceActif)
                    log_chrono.Add("299-EndExecSpecificPrice;" + F_ARTICLE.AR_Ref + ";" + (DateTime.UtcNow - start).ToString());
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private bool Contains(List<client_key> list, client_key clientKey)
        {
            return list.Count(
                ck => ck.product_attribute == clientKey.product_attribute
                && ck.tiers_cbMarq == clientKey.tiers_cbMarq
                && ck.pallierqte == clientKey.pallierqte) > 0;
        }
        private client_key Read(List<client_key> list, client_key clientKey)
        {
            return list.FirstOrDefault(
                ck => ck.product_attribute == clientKey.product_attribute
                && ck.tiers_cbMarq == clientKey.tiers_cbMarq
                && ck.pallierqte == clientKey.pallierqte);
        }

        private bool Contains(List<group_key> list, group_key groupKey)
        {
            return list.Count(
                gk => gk.product_attribute == groupKey.product_attribute
                && gk.prestashop_group == groupKey.prestashop_group
                && gk.cattarif_cbMarq == groupKey.cattarif_cbMarq
                && gk.pallierqte == groupKey.pallierqte) > 0;
        }
        private group_key Read(List<group_key> list, group_key groupKey)
        {
            return list.FirstOrDefault(
                gk => gk.product_attribute == groupKey.product_attribute
                && gk.prestashop_group == groupKey.prestashop_group
                && gk.cattarif_cbMarq == groupKey.cattarif_cbMarq
                && gk.pallierqte == groupKey.pallierqte);
        }

        #endregion

        private void AssignCatalogProduct(Model.Local.Article Article, Model.Prestashop.PsProduct Product)
        {
            try
            {
                Model.Local.ArticleCatalogRepository ArticleCatalogRepository = new Model.Local.ArticleCatalogRepository();
                Model.Local.CatalogRepository CatalogRepository = new Model.Local.CatalogRepository();
                Model.Prestashop.PsCategoryProductRepository PsCategoryProductRepository = new Model.Prestashop.PsCategoryProductRepository();
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

                if (Core.Global.GetConfig().DeleteCatalogProductAssociation)
                {
                    // suppressions des associations inexistantes dans PC
                    while (ListPsCategoryProduct.Count(cp => ListLocal.Count(ac => ac.Catalog.Pre_Id == cp.IDCategory) == 0) > 0)
                    {
                        Model.Prestashop.PsCategoryProduct target = ListPsCategoryProduct.FirstOrDefault(cp => ListLocal.Count(ac => ac.Catalog.Pre_Id == cp.IDCategory) == 0);
                        PsCategoryProductRepository.Delete(target);
                        ListPsCategoryProduct.Remove(target);
                    };
                }

                ListLocal = ListLocal.Where(ac => ListPsCategoryProduct.Count(cp => cp.IDCategory == ac.Catalog.Pre_Id) == 0).ToList();
                foreach (Model.Local.ArticleCatalog ArticleCatalog in ListLocal)
                {
                    uint IDCategory = (UInt32)ArticleCatalog.Catalog.Pre_Id;
                    PsCategoryProductRepository.Add(new Model.Prestashop.PsCategoryProduct()
                    {
                        IDProduct = Product.IDProduct,
                        IDCategory = IDCategory,
                        Position = Model.Prestashop.PsCategoryProductRepository.NextPositionProductCatalog(IDCategory),
                    });
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        internal void ExecFeature(Model.Local.Article Article)
        {
            try
            {
                Model.Local.CharacteristicRepository CharacteristicRepository = new Model.Local.CharacteristicRepository();
                List<Model.Local.Characteristic> ListCharacteristic = CharacteristicRepository.ListArticle(Article.Art_Id);

                Model.Prestashop.PsFeatureValueRepository PsFeatureValueRepository = new Model.Prestashop.PsFeatureValueRepository();
                Model.Prestashop.PsFeatureValue PsFeatureValue;

                Model.Prestashop.PsFeatureValueLangRepository PsFeatureValueLangRepository = new Model.Prestashop.PsFeatureValueLangRepository();
                Model.Prestashop.PsFeatureValueLang PsFeatureValueLang;

                Model.Prestashop.PsFeatureProductRepository PsFeatureProductRepository = new Model.Prestashop.PsFeatureProductRepository();
                Model.Prestashop.PsFeatureProduct PsFeatureProduct;

				Model.Prestashop.PsFeatureRepository PsFeatureRepository = new Model.Prestashop.PsFeatureRepository();

				Boolean isFeatureValue = false;
                Boolean isFeatureValueLang = false;
                Boolean isFeatureProduct = false;

                // <JG> 23/08/2017 ajout gestion des filtres sur transfert automatique
                if (Core.Temp.TaskTransfertFeatureFilter != null)
                    ListCharacteristic = ListCharacteristic.Where(c => Core.Temp.TaskTransfertFeatureFilter.Contains((uint)c.Cha_IdFeature)).ToList();

				foreach (Model.Local.Characteristic Characteristic in ListCharacteristic)
				{
					if (Characteristic.Pre_Id == null || PsFeatureRepository.Exist(Convert.ToUInt32(Characteristic.Cha_IdFeature)))
					{
						PsFeatureValue = new Model.Prestashop.PsFeatureValue();
						PsFeatureValueLang = new Model.Prestashop.PsFeatureValueLang();
						PsFeatureProduct = new Model.Prestashop.PsFeatureProduct();
						isFeatureValue = false;
						isFeatureValueLang = false;
						isFeatureProduct = false;

						// identification valeur
						if (Characteristic.Pre_Id == null && Characteristic.Cha_Custom == false)
						{
							Characteristic.Pre_Id = Core.ImportSage.ImportStatInfoLibreArticle.CreateFeatureValue(Characteristic.Cha_Value, Characteristic.Cha_IdFeature);
							CharacteristicRepository.Save();
						}
						if (Characteristic.Pre_Id != null)
						{
							if (PsFeatureValueRepository.ExistFeatureValue(Convert.ToUInt32(Characteristic.Pre_Id)))
							{
								PsFeatureValue = PsFeatureValueRepository.ReadFeatureValue(Convert.ToUInt32(Characteristic.Pre_Id));
								isFeatureValue = true;
								if (PsFeatureValueLangRepository.ExistFeatureValueLang(Convert.ToUInt32(Characteristic.Pre_Id), Core.Global.Lang))
								{
									PsFeatureValueLang = PsFeatureValueLangRepository.ReadFeatureValueLang(Convert.ToUInt32(Characteristic.Pre_Id), Core.Global.Lang);
									isFeatureValueLang = true;
								}
							}
						}
						// identification lien caractéristique produit
						if (PsFeatureProductRepository.ExistFeatureProduct(Convert.ToUInt32(Characteristic.Cha_IdFeature), Convert.ToUInt32(Article.Pre_Id)))
						{
							PsFeatureProduct = PsFeatureProductRepository.ReadFeatureProduct(Convert.ToUInt32(Characteristic.Cha_IdFeature), Convert.ToUInt32(Article.Pre_Id));
							isFeatureProduct = true;
						}

						PsFeatureValue.Custom = Convert.ToByte(Characteristic.Cha_Custom);
						PsFeatureValue.IDFeature = Convert.ToUInt32(Characteristic.Cha_IdFeature);
						if (isFeatureValue == true)
						{
							PsFeatureValueRepository.Save();
						}
						else
						{
							PsFeatureValueRepository.Add(PsFeatureValue);
							Characteristic.Pre_Id = Convert.ToInt32(PsFeatureValue.IDFeatureValue);
							CharacteristicRepository.Save();
						}

						PsFeatureValueLang.IDLang = Core.Global.Lang;
						PsFeatureValueLang.IDFeatureValue = PsFeatureValue.IDFeatureValue;
						PsFeatureValueLang.Value = Characteristic.Cha_Value;
						if (isFeatureValueLang == true)
						{
							PsFeatureValueLangRepository.Save();
						}
						else
						{
							PsFeatureValueLangRepository.Add(PsFeatureValueLang);
						}

						// <JG> 21/05/2013 gestion insertion multi-langue pour la valeur de caracteristique
						foreach (Model.Prestashop.PsLang lang in new Model.Prestashop.PsLangRepository().ListActive(1, Core.Global.CurrentShop.IDShop))
							if (lang.IDLang != Core.Global.Lang
									&& !PsFeatureValueLangRepository.ExistFeatureValueLang(PsFeatureValue.IDFeatureValue, lang.IDLang))
								PsFeatureValueLangRepository.Add(new Model.Prestashop.PsFeatureValueLang()
								{
									IDLang = lang.IDLang,
									IDFeatureValue = PsFeatureValue.IDFeatureValue,
									Value = Characteristic.Cha_Value
								});

						PsFeatureProduct.IDFeatureValue = PsFeatureValue.IDFeatureValue;
						PsFeatureProduct.IDProduct = Convert.ToUInt32(Article.Pre_Id);
						PsFeatureProduct.IDFeature = Convert.ToUInt32(Characteristic.Cha_IdFeature);
						if (isFeatureProduct == true)
						{
							PsFeatureProductRepository.Save();
						}
						else
						{
							PsFeatureProductRepository.Add(PsFeatureProduct);
						}
					}
				}
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        #region Gamme/Déclinaisons

        public void ExecAttribute(Model.Local.Article Article, Model.Prestashop.PsProduct PsProduct, Model.Prestashop.PsProductRepository PsProductRepository, Model.Sage.F_TAXE TaxeTVA)
        {
            try
            {
                Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
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
                            this.ReadQuantityAttribute(PsProductAttribute, F_ARTICLE, AttributeArticle.EnumereGamme1, AttributeArticle.EnumereGamme2);
                            CumulStockGammes += PsProductAttribute.Quantity;

                            if (Article.Art_SyncPrice)
                                this.ReadPriceAttribute(PsProduct, PsProductAttribute, F_ARTICLE, AttributeArticle.EnumereGamme1, AttributeArticle.EnumereGamme2, TaxeTVA);

                            this.ReadWeightAttribute(PsProduct, PsProductAttribute, F_ARTICLE, AttributeArticle.EnumereGamme1);
                            this.ReadRefEANAttribute(PsProductAttribute, F_ARTICLE, AttributeArticle.EnumereGamme1, AttributeArticle.EnumereGamme2);
                            //PsProductAttribute.Weight = PsProduct.Weight;

                            PsProductAttribute.DefaultOn = (AttributeArticle.AttArt_Default) ? (byte)1 : 
								#if (PRESTASHOP_VERSION_161 || PRESTASHOP_VERSION_172)
								(byte?)null;
								#else
								(byte)0;
								#endif

                            // update others ps_product_attribute_lines
                            if (AttributeArticle.AttArt_Default)
                                PsProductAttributeRepository.EraseDefault(PsProductAttribute.IDProduct, PsProductAttribute.IDProductAttribute);

                            if (isProductAttribute == true)
                            {
                                PsProductAttributeRepository.Save();
                                this.ExecShopProductAttribute(PsProductAttribute);
                            }
                            else
                            {
                                PsProductAttribute.IDProduct = (UInt32)Article.Pre_Id;
                                //HARD CODE
                                PsProductAttribute.MinimalQuantity = 1;

                                PsProductAttributeRepository.Add(PsProductAttribute, Global.CurrentShop.IDShop);
                                // We need to update AttributeArticle
                                AttributeArticle.Pre_Id = (Int32)PsProductAttribute.IDProductAttribute;
                                AttributeArticleRepository.Save();
                            }
                            if (AttributeArticle.AttArt_Default)
                            {
                                PsProduct.CacheDefaultAttribute = PsProductAttribute.IDProductAttribute;
                                PsProductRepository.Save();
                            }

                            #region Lien déclinaison - énuméré de gamme
                            //We need to update productattributecombination too
                            // <JG> 26/10/2012 correction insertion AttributeFirst.Pre_Id sur PsProductAttributeCombination
                            Model.Prestashop.PsProductAttributeCombinationRepository PsProductAttributeCombinationRepository = new Model.Prestashop.PsProductAttributeCombinationRepository();
                            // <JG> 06/11/2012 Correction synchronisation nouveaux attributs
                            if (PsProductAttributeCombinationRepository.ExistAttributeProductAttribute((UInt32)AttributeArticle.Att_IdFirst, PsProductAttribute.IDProductAttribute) == false)
                            {
                                Model.Prestashop.PsProductAttributeCombination PsProductAttributeCombination = new Model.Prestashop.PsProductAttributeCombination();
                                PsProductAttributeCombination.IDAttribute = (UInt32)AttributeArticle.Att_IdFirst;
                                PsProductAttributeCombination.IDProductAttribute = PsProductAttribute.IDProductAttribute;
                                PsProductAttributeCombinationRepository.Add(PsProductAttributeCombination);
                            }
                            if (AttributeArticle.Att_IdSecond != null)
                            {
                                if (PsProductAttributeCombinationRepository.ExistAttributeProductAttribute((UInt32)AttributeArticle.Att_IdSecond, PsProductAttribute.IDProductAttribute) == false)
                                {
                                    Model.Prestashop.PsProductAttributeCombination PsProductAttributeCombination = new Model.Prestashop.PsProductAttributeCombination();
                                    PsProductAttributeCombination.IDAttribute = (UInt32)AttributeArticle.Att_IdSecond;
                                    PsProductAttributeCombination.IDProductAttribute = PsProductAttribute.IDProductAttribute;
                                    PsProductAttributeCombinationRepository.Add(PsProductAttributeCombination);
                                }
                            }
                            #endregion

                            WriteStockAvailableProductAttribute(PsProduct, PsProductAttribute);
                        }
                    }
                    if (PsProduct.Quantity != CumulStockGammes)
                    {
                        PsProduct.Quantity = CumulStockGammes;
                        WriteStockAvailableProduct(PsProduct);
                        PsProductRepository.Save();
                    }
                    PsProductAttributeRepository.WriteDate(PsProduct.IDProduct);
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        public void ReadQuantityAttribute(Model.Prestashop.PsProductAttribute PsProductAttribute, Model.Sage.F_ARTICLE F_ARTICLE, Model.Sage.F_ARTGAMME EnumereGamme1, Model.Sage.F_ARTGAMME EnumereGamme2)
        {
            try
            {
                if (Core.Global.GetConfig().ModuleAECStockActif)
                {
                    PsProductAttribute.AEC_Stock = new Model.Prestashop.PsAEcStock();
                }

                Int32 Quantity = 0;
                Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                // if the F_ARTICLE is not Contremarque
                if (F_ARTICLE.AR_Contremarque == 0 || Core.Global.GetConfig().ArticleContremarqueStockActif)
                {
                    Model.Local.SupplyRepository SupplyRepository = new Model.Local.SupplyRepository();
                    List<Model.Local.Supply> ListSupply = SupplyRepository.ListActive(true);

                    Model.Sage.F_GAMSTOCKRepository F_GAMSTOCKRepository = new Model.Sage.F_GAMSTOCKRepository();
                    Model.Sage.F_GAMSTOCK F_GAMSTOCK;
                    Boolean isQuantity;
                    // if the ConfigArticleStock 
                    if (ConfigRepository.ExistName(Core.Global.ConfigArticleStock))
                    {
                        Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigArticleStock);
                        // foreach Supply, we add the stock at the quantity
                        foreach (Model.Local.Supply Supply in ListSupply)
                        {
                            F_GAMSTOCK = new Model.Sage.F_GAMSTOCK();
                            isQuantity = false;

                            if (F_GAMSTOCKRepository.ExistReferenceGamme1Gamme2Depot(F_ARTICLE.AR_Ref, EnumereGamme1.AG_No.Value, EnumereGamme2.AG_No.Value, Supply.Sag_Id))
                            {
                                isQuantity = true;
                                F_GAMSTOCK = F_GAMSTOCKRepository.ReadReferenceGamme1Gamme2Depot(F_ARTICLE.AR_Ref, EnumereGamme1.AG_No.Value, EnumereGamme2.AG_No.Value, Supply.Sag_Id);
                            }

                            if (isQuantity == true)
                            {
                                Int32 QteSto = (Int32)((F_GAMSTOCK.GS_QteSto != null) ? F_GAMSTOCK.GS_QteSto.Value : 0);
                                Int32 QteCom = (Int32)((F_GAMSTOCK.GS_QteCom != null) ? F_GAMSTOCK.GS_QteCom.Value : 0);
                                Int32 QteRes = (Int32)((F_GAMSTOCK.GS_QteRes != null) ? F_GAMSTOCK.GS_QteRes.Value : 0);
                                Int32 QtePrepa = (Int32)((F_GAMSTOCK.GS_QtePrepa != null) ? F_GAMSTOCK.GS_QtePrepa.Value : 0);
                                Int32 QteAControler = (Int32)((F_GAMSTOCK.GS_QteAControler != null) ? F_GAMSTOCK.GS_QteAControler.Value : 0);

                                Int32 QteMini = (Int32)((F_GAMSTOCK.GS_QteMini != null) ? F_GAMSTOCK.GS_QteMini.Value : 0);
                                Int32 QteMaxi = (Int32)((F_GAMSTOCK.GS_QteMaxi != null) ? F_GAMSTOCK.GS_QteMaxi.Value : 0);

                                Int32 qty_temp = 0;

                                switch (Config.Con_Value)
                                {
                                    // stock à terme
                                    case "1":
                                        qty_temp = (Int32)(QteSto + QteCom - (QteRes + QtePrepa));
                                        break;
                                    // stock réel
                                    case "2":
                                        qty_temp = (Int32)QteSto;
                                        break;
                                    // stock disponible
                                    case "3":
                                        qty_temp = (Int32)(QteSto - (QtePrepa + QteAControler));
                                        break;
                                    // stock disponible avancé
                                    case "4":
                                        qty_temp = (Int32)(QteSto - (QteRes + QtePrepa + QteAControler));
                                        break;
                                }

                                // <JG> 05/10/2016
                                if (Core.Global.GetConfig().ArticleStockNegatifZeroParDepot && qty_temp < 0)
                                {
                                    qty_temp = 0;
                                }
                                Quantity += qty_temp;

                                if (Core.Global.GetConfig().ModuleAECStockActif)
                                {
                                    PsProductAttribute.AEC_Stock.Count_Supply += 1;
                                    PsProductAttribute.AEC_Stock.MinimalQuantity += QteMini;
                                    PsProductAttribute.AEC_Stock.MaximalQuantity += QteMaxi;
                                    PsProductAttribute.AEC_Stock.QuantityReal += QteSto;
                                    PsProductAttribute.AEC_Stock.QuantityFuture += (QteSto + QteCom - (QteRes + QtePrepa));
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (ConfigRepository.ExistName(Core.Global.ConfigArticleContremarque))
                    {
                        Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigArticleContremarque);
                        if (Core.Global.IsNumeric(Config.Con_Value))
                        {
                            Quantity = Convert.ToInt32(Config.Con_Value);
                        }
                    }
                }
                if (Core.Global.GetConfig().ArticleStockNegatifZero && Quantity < 0)
                    Quantity = 0;
                PsProductAttribute.Quantity = Quantity;
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        public void ReadPriceAttribute(Model.Prestashop.PsProduct Product, Model.Prestashop.PsProductAttribute PsProductAttribute,
            Model.Sage.F_ARTICLE F_ARTICLE, Model.Sage.F_ARTGAMME EnumereGamme1, Model.Sage.F_ARTGAMME EnumereGamme2, Model.Sage.F_TAXE TaxeTVA)
        {
            try
            {
                Decimal Price = 0;
                Boolean isTTC = false;
                Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                if (ConfigRepository.ExistName(Core.Global.ConfigArticleCatTarif))
                {
                    Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigArticleCatTarif);

                    Model.Sage.F_ARTCLIENTRepository F_ARTCLIENTRepository = new Model.Sage.F_ARTCLIENTRepository();
                    if (F_ARTCLIENTRepository.ExistReferenceCategorie(F_ARTICLE.AR_Ref, Convert.ToInt32(Config.Con_Value)))
                    {
                        Model.Sage.F_ARTCLIENT F_ARTCLIENT = F_ARTCLIENTRepository.ReadReferenceCategorie(F_ARTICLE.AR_Ref, Convert.ToInt32(Config.Con_Value));
                        isTTC = (F_ARTCLIENT.AC_PrixTTC == 1);
                    }

                    if (Core.Global.IsNumeric(Config.Con_Value))
                    {
                        String CategorieTarifaire = "a";
                        Int32 Categorie = Convert.ToInt32(Config.Con_Value);
                        if (Categorie < 10)
                        {
                            CategorieTarifaire += "0";
                        }
                        CategorieTarifaire += Categorie.ToString();
                        Model.Sage.F_TARIFGAMRepository F_TARIFGAMRepository = new Model.Sage.F_TARIFGAMRepository();
                        Model.Sage.F_TARIFGAM F_TARIFGAM = new Model.Sage.F_TARIFGAM();

                        if (F_TARIFGAMRepository.ExistReferenceCategorieGamme1Gamme2(F_ARTICLE.AR_Ref, CategorieTarifaire, (int)EnumereGamme1.AG_No, (int)EnumereGamme2.AG_No))
                        {
                            F_TARIFGAM = F_TARIFGAMRepository.ReadReferenceCategorieGamme1Gamme2(F_ARTICLE.AR_Ref, CategorieTarifaire, (int)EnumereGamme1.AG_No, (int)EnumereGamme2.AG_No);
                            Price = F_TARIFGAM.TG_Prix.Value;
                        }

                        if (Price > 0 && isTTC == true)
                        {
                            Model.Local.TaxRepository TaxRepository = new Model.Local.TaxRepository();
                            if (TaxRepository.ExistSage(TaxeTVA.cbMarq))
                            {
                                Model.Local.Tax Tax = TaxRepository.ReadSage(TaxeTVA.cbMarq);
                                Model.Prestashop.PsTaxRepository PsTaxRepository = new Model.Prestashop.PsTaxRepository();
                                if (PsTaxRepository.ExistTaxe((UInt32)Tax.Pre_Id))
                                {
                                    Model.Prestashop.PsTax PsTax = PsTaxRepository.ReadTax((UInt32)Tax.Pre_Id);
                                    Price = (Price / (1 + (PsTax.Rate / 100))) - Product.ecotaxe_htsage;
                                }
                            }
                        }
                    }
                }
                if (Price > 0)
                {
                    PsProductAttribute.Price = Price - Product.Price;
                }
                else
                {
                    PsProductAttribute.Price = Price;
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        public void ReadRefEANAttribute(Model.Prestashop.PsProductAttribute PsProductAttribute, Model.Sage.F_ARTICLE F_ARTICLE, Model.Sage.F_ARTGAMME EnumereGamme1, Model.Sage.F_ARTGAMME EnumereGamme2)
        {
            try
            {
                String Reference = string.Empty;
                String EAN13 = string.Empty;

                Model.Sage.F_ARTENUMREFRepository F_ARTENUMREFRepository = new Model.Sage.F_ARTENUMREFRepository();
                Model.Sage.F_ARTENUMREF F_ARTENUMREF = new Model.Sage.F_ARTENUMREF();

                if (F_ARTENUMREFRepository.ExistReferenceGamme1Gamme2(F_ARTICLE.AR_Ref, (int)EnumereGamme1.AG_No, (int)EnumereGamme2.AG_No))
                {
                    F_ARTENUMREF = F_ARTENUMREFRepository.ReadReferenceGamme1Gamme2(F_ARTICLE.AR_Ref, (int)EnumereGamme1.AG_No, (int)EnumereGamme2.AG_No);
                    Reference = F_ARTENUMREF.AE_Ref;
                    EAN13 = F_ARTENUMREF.AE_CodeBarre;
                }

                PsProductAttribute.EAn13 = Core.Global.RemovePurgeEAN(EAN13);
                PsProductAttribute.Reference = Reference;
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        public void ReadWeightAttribute(Model.Prestashop.PsProduct Product, Model.Prestashop.PsProductAttribute PsProductAttribute, Model.Sage.F_ARTICLE F_ARTICLE, Model.Sage.F_ARTGAMME EnumereGamme1)
        {
            Decimal Weight = 0;
            Decimal Coefficient = 1;
            try
            {
                Boolean ReplaceDot;
                #region Coefficient
                if (Core.Global.GetConfig().CombinationWithWeightConversion.Contains((int)F_ARTICLE.AR_Gamme1))
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(Core.Global.GetConfig().InformationLibreCoefficientConversion))
                        {
                            Model.Sage.F_ARTICLERepository F_ARTICLERespository = new Model.Sage.F_ARTICLERepository();
                            Model.Sage.cbSysLibreRepository.CB_Type cbtype = new Model.Sage.cbSysLibreRepository().ReadTypeInformationLibre(Core.Global.GetConfig().InformationLibreCoefficientConversion, Model.Sage.cbSysLibreRepository.CB_File.F_ARTICLE);
                            switch (cbtype)
                            {
                                case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageValeur:
                                case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageMontant:
                                    if (F_ARTICLERespository.ExistArticleInformationLibreNumerique(Core.Global.GetConfig().InformationLibreCoefficientConversion, F_ARTICLE.AR_Ref))
                                    {
                                        decimal? result = F_ARTICLERespository.ReadArticleInformationLibreNumerique(Core.Global.GetConfig().InformationLibreCoefficientConversion, F_ARTICLE.AR_Ref);
                                        if (result != null && result != 0)
                                            Coefficient = (decimal)result;
                                    }
                                    break;
                                case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageText:
                                case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageTable:
                                    if (F_ARTICLERespository.ExistArticleInformationLibreText(Core.Global.GetConfig().InformationLibreCoefficientConversion, F_ARTICLE.AR_Ref))
                                    {
                                        string result = F_ARTICLERespository.ReadArticleInformationLibreText(Core.Global.GetConfig().InformationLibreCoefficientConversion, F_ARTICLE.AR_Ref);
                                        if (!String.IsNullOrWhiteSpace(result) && Core.Global.IsNumeric(result.Trim(), out ReplaceDot))
                                        {
                                            Coefficient = Decimal.Parse((ReplaceDot) ? result.Trim().Replace('.', ',') : result.Trim());
                                        }
                                    }
                                    break;
                                case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.Deleted:
                                case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageSmallDate:
                                case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageDate:
                                default:
                                    break;
                            }


                        }
                    }
                    catch (Exception ex) { Core.Error.SendMailError("[ReadWeightAttribute]" + ex.ToString()); }
                }
                #endregion

                //if (isDoubleGamme && Core.Global.GetConfig().CombinationWithWeightConversion.Contains(CombinationSecond.Com_Id))
                //{
                //    if (Core.Global.IsNumeric(AttributeSecond.Att_Name.Trim(), out ReplaceDot))
                //    {

                //    }
                //}
                //else  
                if (Core.Global.GetConfig().CombinationWithWeightConversion.Contains((int)F_ARTICLE.AR_Gamme1))
                {
                    if (Core.Global.IsNumeric(EnumereGamme1.EG_Enumere.Trim(), out ReplaceDot))
                    {
                        Weight = Decimal.Parse((ReplaceDot) ? EnumereGamme1.EG_Enumere.Trim().Replace('.', ',') : EnumereGamme1.EG_Enumere.Trim());
                    }
                }
            }
            catch (Exception ex) { Core.Error.SendMailError("[ReadWeightAttribute]" + ex.ToString()); }
            if (Weight != 0 || Core.Global.GetConfig().CombinationWithWeightConversion.Contains((int)F_ARTICLE.AR_Gamme1))
				#if (PRESTASHOP_VERSION_15)
                PsProductAttribute.Weight = (float)(Weight * Coefficient);
				#else
				PsProductAttribute.Weight = (Weight * Coefficient);
				#endif
            // <JG> 12/01/2015 attention gestion augmentation-réduction de poids sur la déclinaison
            if (PsProductAttribute.Weight != 0 && Product.Weight != 0)
            {
                PsProductAttribute.Weight = PsProductAttribute.Weight - Product.Weight;
            }
        }

        #endregion

        #region Conditionnement/Déclinaisons

        public void ExecConditioning(Model.Local.Article Article, Model.Prestashop.PsProduct PsProduct, Model.Prestashop.PsProductRepository PsProductRepository, Model.Sage.F_TAXE TaxeTVA)
        {
            try
            {
                Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
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

                            this.ReadQuantityConditioning(PsProductAttribute, F_ARTICLE, ConditioningArticle.EnumereF_CONDITION);
                            
                            if (Core.Global.GetConfig().LimiteStockConditionnement && PsProductAttribute.Quantity >= stockmaxunity)
                            {
                                stockmaxunity = PsProductAttribute.Quantity;
                                PsProduct.Quantity = stockmaxunity;
                                PsProductRepository.Save();
                            }

                            // <JG> 03/06/2016
                            if (Article.Art_SyncPrice)
                                this.ReadPriceConditioning(PsProduct, PsProductAttribute, F_ARTICLE, ConditioningArticle.EnumereF_CONDITION, TaxeTVA);

                            this.ReadWeightConditioning(PsProduct, PsProductAttribute, ConditioningArticle.EnumereF_CONDITION);
                            //this.ReadRefEANAttribute(PsProductAttribute, F_ARTICLE, Conditioning);

                            PsProductAttribute.DefaultOn = (ConditioningArticle.ConArt_Default) ? (byte)1 : 
								#if (PRESTASHOP_VERSION_161 || PRESTASHOP_VERSION_172)
								(byte?)null;
								#else
								(byte)0;
								#endif

                            // update others ps_product_attribute_lines
                            if (ConditioningArticle.ConArt_Default)
                                PsProductAttributeRepository.EraseDefault(PsProductAttribute.IDProduct, PsProductAttribute.IDProductAttribute);

                            if (isProductAttribute == true)
                            {
                                PsProductAttributeRepository.Save();
                                this.ExecShopProductAttribute(PsProductAttribute);
                            }
                            else
                            {
                                PsProductAttribute.IDProduct = (UInt32)Article.Pre_Id;
                                //HARD CODE
                                PsProductAttribute.MinimalQuantity = 1;
                                PsProductAttributeRepository.Add(PsProductAttribute, Global.CurrentShop.IDShop);
                                // We need to update AttributeArticle
                                ConditioningArticle.Pre_Id = (Int32)PsProductAttribute.IDProductAttribute;
                                ConditioningArticleRepository.Save();
                            }

                            if (ConditioningArticle.ConArt_Default)
                            {
                                PsProduct.CacheDefaultAttribute = PsProductAttribute.IDProductAttribute;
                                PsProductRepository.Save();
                            }

                            #region lien déclinaison - énuméré de conditionnnement
                            //We need to update productattributecombination too
                            // <JG> 26/10/2012 correction insertion AttributeFirst.Pre_Id sur PsProductAttributeCombination
                            Model.Prestashop.PsProductAttributeCombinationRepository PsProductAttributeCombinationRepository = new Model.Prestashop.PsProductAttributeCombinationRepository();
                            // <JG> 06/11/2012 Correction synchronisation nouveaux attributs
                            if (PsProductAttributeCombinationRepository.ExistAttributeProductAttribute((UInt32)ConditioningArticle.Con_Id, PsProductAttribute.IDProductAttribute) == false)
                            {
                                Model.Prestashop.PsProductAttributeCombination PsProductAttributeCombination = new Model.Prestashop.PsProductAttributeCombination();
                                PsProductAttributeCombination.IDAttribute = (UInt32)ConditioningArticle.Con_Id;
                                PsProductAttributeCombination.IDProductAttribute = PsProductAttribute.IDProductAttribute;
                                PsProductAttributeCombinationRepository.Add(PsProductAttributeCombination);
                            }
                            #endregion

                            WriteStockAvailableProductAttribute(PsProduct, PsProductAttribute);
                        }
                    }
                    PsProductAttributeRepository.WriteDate(PsProduct.IDProduct);
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        public void ReadQuantityConditioning(Model.Prestashop.PsProductAttribute PsProductAttribute, Model.Sage.F_ARTICLE F_ARTICLE, Model.Sage.F_CONDITION EnumereF_CONDITION)
        {
            try
            {
                if (Core.Global.GetConfig().ModuleAECStockActif)
                {
                    PsProductAttribute.AEC_Stock = new Model.Prestashop.PsAEcStock();
                }

                if (EnumereF_CONDITION != null && EnumereF_CONDITION.EC_Quantite != null && EnumereF_CONDITION.EC_Quantite != 0)
                {
                    decimal ec_qte = EnumereF_CONDITION.EC_Quantite.Value;
                    Int32 Quantity = 0;

                    Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                    // if the F_ARTICLE is not Contremarque
                    if (F_ARTICLE.AR_Contremarque == 0 || Core.Global.GetConfig().ArticleContremarqueStockActif)
                    {


                        Model.Local.SupplyRepository SupplyRepository = new Model.Local.SupplyRepository();
                        List<Model.Local.Supply> ListSupply = SupplyRepository.ListActive(true);
                        Model.Sage.F_ARTSTOCKRepository F_ARTSTOCKRepository = new Model.Sage.F_ARTSTOCKRepository();

                        // if the ConfigArticleStock 
                        if (ConfigRepository.ExistName(Core.Global.ConfigArticleStock))
                        {
                            Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigArticleStock);
                            // foreach Supply, we add the stock at the quantity
                            Model.Sage.F_ARTSTOCK F_ARTSTOCK;
                            foreach (Model.Local.Supply Supply in ListSupply)
                            {
                                if (F_ARTSTOCKRepository.ExistReferenceDepot(F_ARTICLE.AR_Ref, Supply.Sag_Id))
                                {
                                    F_ARTSTOCK = F_ARTSTOCKRepository.ReadReferenceDepot(F_ARTICLE.AR_Ref, Supply.Sag_Id);

                                    Decimal QteSto = ((F_ARTSTOCK.AS_QteSto != null) ? F_ARTSTOCK.AS_QteSto.Value : 0) / ec_qte;
                                    Decimal QteCom = ((F_ARTSTOCK.AS_QteCom != null) ? F_ARTSTOCK.AS_QteCom.Value : 0) / ec_qte;
                                    Decimal QteRes = ((F_ARTSTOCK.AS_QteRes != null) ? F_ARTSTOCK.AS_QteRes.Value : 0) / ec_qte;
                                    Decimal QtePrepa = ((F_ARTSTOCK.AS_QtePrepa != null) ? F_ARTSTOCK.AS_QtePrepa.Value : 0) / ec_qte;
                                    Decimal QteAControler = ((F_ARTSTOCK.AS_QteAControler != null) ? F_ARTSTOCK.AS_QteAControler.Value : 0) / ec_qte;

                                    Decimal QteMini = ((F_ARTSTOCK.AS_QteMini != null) ? F_ARTSTOCK.AS_QteMini.Value : 0) / ec_qte;
                                    Decimal QteMaxi = ((F_ARTSTOCK.AS_QteMaxi != null) ? F_ARTSTOCK.AS_QteMaxi.Value : 0) / ec_qte;

                                    Int32 qty_temp = 0;

                                    switch (Config.Con_Value)
                                    {
                                        // stock à terme
                                        case "1":
                                            qty_temp = (Int32)(QteSto + QteCom - (QteRes + QtePrepa));
                                            break;
                                        // stock réel
                                        case "2":
                                            qty_temp = (Int32)QteSto;
                                            break;
                                        // stock disponible
                                        case "3":
                                            qty_temp = (Int32)(QteSto - (QtePrepa + QteAControler));
                                            break;
                                        // stock disponible avancé
                                        case "4":
                                            qty_temp = (Int32)(QteSto - (QteRes + QtePrepa + QteAControler));
                                            break;
                                    }

                                    // <JG> 05/10/2016
                                    if (Core.Global.GetConfig().ArticleStockNegatifZeroParDepot && qty_temp < 0)
                                    {
                                        qty_temp = 0;
                                    }
                                    Quantity += qty_temp;

                                    if (Core.Global.GetConfig().ModuleAECStockActif)
                                    {
                                        PsProductAttribute.AEC_Stock.Count_Supply += 1;
                                        PsProductAttribute.AEC_Stock.MinimalQuantity += (Int32)(QteMini);
                                        PsProductAttribute.AEC_Stock.MaximalQuantity += (Int32)(QteMaxi);
                                        PsProductAttribute.AEC_Stock.QuantityReal += (Int32)(QteSto);
                                        PsProductAttribute.AEC_Stock.QuantityFuture += (Int32)(QteSto + QteCom - (QteRes + QtePrepa));
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (ConfigRepository.ExistName(Core.Global.ConfigArticleContremarque))
                        {
                            Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigArticleContremarque);
                            if (Core.Global.IsNumeric(Config.Con_Value))
                            {
                                Quantity = (Int32)Math.Floor(Convert.ToInt32(Config.Con_Value) / ec_qte);
                            }
                        }
                    }

                    if (Core.Global.GetConfig().ModuleAECStockActif)
                    {
                        PsProductAttribute.AEC_Stock.MinimalQuantity = (Int32)Math.Floor((decimal)PsProductAttribute.AEC_Stock.MinimalQuantity);
                        PsProductAttribute.AEC_Stock.MaximalQuantity = (Int32)Math.Floor((decimal)PsProductAttribute.AEC_Stock.MaximalQuantity);
                        PsProductAttribute.AEC_Stock.QuantityReal = (Int32)Math.Floor((decimal)PsProductAttribute.AEC_Stock.QuantityReal);
                        PsProductAttribute.AEC_Stock.QuantityFuture = (Int32)Math.Floor((decimal)PsProductAttribute.AEC_Stock.QuantityFuture);
                    }

                    if (Core.Global.GetConfig().ArticleStockNegatifZero && Quantity < 0)
                        Quantity = 0;

                    PsProductAttribute.Quantity = Quantity;
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        public void ReadPriceConditioning(Model.Prestashop.PsProduct Product, Model.Prestashop.PsProductAttribute PsProductAttribute, Model.Sage.F_ARTICLE F_ARTICLE,
            Model.Sage.F_CONDITION EnumereF_CONDITION, Model.Sage.F_TAXE TaxeTVA)
        {
            try
            {
                Decimal Price = 0;
                String Reference = string.Empty;
                String EAN13 = string.Empty;
                String UPC = string.Empty;
                Boolean isTTC = false;

                Model.Sage.F_TARIFCOND F_TARIFCOND = null;

                Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                if (ConfigRepository.ExistName(Core.Global.ConfigArticleCatTarif))
                {
                    Model.Local.Config Config = ConfigRepository.ReadName(Core.Global.ConfigArticleCatTarif);
                    int cat_tarif_default = int.Parse(Config.Con_Value);

                    Model.Sage.F_ARTCLIENTRepository F_ARTCLIENTRepository = new Model.Sage.F_ARTCLIENTRepository();
                    if (F_ARTCLIENTRepository.ExistReferenceCategorie(F_ARTICLE.AR_Ref, cat_tarif_default))
                    {
                        Model.Sage.F_ARTCLIENT F_ARTCLIENT = F_ARTCLIENTRepository.ReadReferenceCategorie(F_ARTICLE.AR_Ref, cat_tarif_default);
                        isTTC = (F_ARTCLIENT.AC_PrixTTC == 1);
                    }

                    String CategorieTarifaire = "a";
                    if (cat_tarif_default < 10)
                        CategorieTarifaire += "0";
                    CategorieTarifaire += cat_tarif_default.ToString();

                    Model.Sage.F_TARIFCONDRepository F_TARIFCONDRepository = new Model.Sage.F_TARIFCONDRepository();
                    if (F_TARIFCONDRepository.ExistReferenceCategorieConditionnement(F_ARTICLE.AR_Ref, CategorieTarifaire, (int)EnumereF_CONDITION.CO_No))
                    {
                        F_TARIFCOND = F_TARIFCONDRepository.ReadReferenceCategorieConditionnement(F_ARTICLE.AR_Ref, CategorieTarifaire, (int)EnumereF_CONDITION.CO_No);
                        Price = F_TARIFCOND.TC_Prix.Value;
                    }
                }

                if (F_TARIFCOND == null && EnumereF_CONDITION != null && EnumereF_CONDITION.EC_Quantite != null)
                {
                    Price = (decimal)EnumereF_CONDITION.EC_Quantite * Product.Price;
                    isTTC = false; // Product.Price est déjà HT
                }
                Reference = EnumereF_CONDITION.CO_Ref;
                EAN13 = EnumereF_CONDITION.CO_CodeBarre;
                UPC = (EnumereF_CONDITION.EC_Quantite != null && Core.Global.GetConfig().ArticleConditionnementQuantiteToUPC)
                    ? EnumereF_CONDITION.EC_Quantite.Value.ToString("0.####").Replace(',', '.')
                    : string.Empty;

                if (Price > 0 && isTTC == true)
                {
                    Model.Local.TaxRepository TaxRepository = new Model.Local.TaxRepository();
                    if (TaxRepository.ExistSage(TaxeTVA.cbMarq))
                    {
                        Model.Local.Tax Tax = TaxRepository.ReadSage(TaxeTVA.cbMarq);
                        Model.Prestashop.PsTaxRepository PsTaxRepository = new Model.Prestashop.PsTaxRepository();
                        if (PsTaxRepository.ExistTaxe((UInt32)Tax.Pre_Id))
                        {
                            Model.Prestashop.PsTax PsTax = PsTaxRepository.ReadTax((UInt32)Tax.Pre_Id);
                            Price = (Price / (1 + (PsTax.Rate / 100))) - Product.ecotaxe_htsage;
                        }
                    }
                }

                if (Price > 0)
                {
                    PsProductAttribute.Price = Price - Product.Price;
                }
                else
                {
                    PsProductAttribute.Price = Price;
                }
                PsProductAttribute.EAn13 = Core.Global.RemovePurgeEAN(EAN13);
                PsProductAttribute.Reference = Reference;
                PsProductAttribute.Upc = UPC;
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        public void ReadWeightConditioning(Model.Prestashop.PsProduct Product, Model.Prestashop.PsProductAttribute PsProductAttribute, Model.Sage.F_CONDITION F_CONDITION)
        {
            PsProductAttribute.Weight = 0;
            try
            {
                if (F_CONDITION != null && F_CONDITION.EC_Quantite != null && F_CONDITION.EC_Quantite != 0)
                {
                    // <JG> 12/01/2015 ajout gestion augmentation-reduction du poids
                    if (Product.Weight != 0)
						#if (PRESTASHOP_VERSION_15)
                        PsProductAttribute.Weight = (Product.Weight * (float)F_CONDITION.EC_Quantite.Value) - Product.Weight;
						#else
						PsProductAttribute.Weight = (Product.Weight * F_CONDITION.EC_Quantite.Value) - Product.Weight;
						#endif
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
        #endregion

        #region CompositionArticle

        public void ExecCompositionArticle(Model.Local.Article Article, Model.Prestashop.PsProduct Product, Model.Prestashop.PsProductRepository PsProductRepository)
        {
            Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
            Model.Local.CompositionArticleRepository CompositionArticleRepository = new Model.Local.CompositionArticleRepository();
            List<Model.Local.CompositionArticle> ListCompositionArticle = CompositionArticleRepository.ListArticle(Article.Art_Id);
            foreach (var item in ListCompositionArticle)
            {
                // test si les valeurs d'attributs sont renseignées pour chaque groupe
                if (Article.CompositionArticleAttributeGroup.Count == item.CompositionArticleAttribute.Count)
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

                            if (Core.Global.GetConfig().ArticleDateDispoInfoLibreActif)
                                this.ReadDateDispo(PsProductAttributeComposition, F_ARTICLE);

                            if (Core.Global.GetConfig().ArticleQuantiteMiniActif)
                                ReadMinimalQuantity(PsProductAttributeComposition, F_ARTICLE);

                            int CatComptaArticle = Core.Global.GetConfig().ConfigArticleCatComptable;
                            Model.Sage.F_TAXE TaxeTVA = ReadTaxe(F_ARTICLE, Product, CatComptaArticle);

                            Model.Prestashop.PsProduct temp_product = new Model.Prestashop.PsProduct();
                            temp_product.taxe_famillesage = Product.taxe_famillesage;

                            // temp_product sert pour conserver l'information "ecoraxeht_sage" pour la fonction SpecificPrice
                            Model.Sage.F_TAXE TaxeEco = this.ReadEcoTaxe(F_ARTICLE, temp_product, TaxeTVA, CatComptaArticle);
                            PsProductAttributeComposition.EcOtAx = temp_product.EcOtAx;

                            if (F_ARTICLE_Composition.F_CONDITION_SagId == null && F_ARTICLE_Composition.F_ARTENUMREF_SagId == null)
                            {
                                this.ReadWeight(F_ARTICLE, PsProductAttributeComposition);
                                this.ReadQuantity(F_ARTICLE, PsProductAttributeComposition);

                                // <JG> 03/06/2016
                                if (Article.Art_SyncPrice)
                                    this.ReadPrice(F_ARTICLE, PsProductAttributeComposition, TaxeTVA);

                                //PsProductAttributeComposition.Reference = F_ARTICLE.AR_Ref;
                                //PsProductAttributeComposition.EAn13 = Core.Global.RemovePurgeEAN(F_ARTICLE.AR_CodeBarre);
                            }
                            else if (F_ARTICLE_Composition.F_ARTENUMREF_SagId != null)
                            {
                                Model.Prestashop.PsProduct temp_product_f_artenumref = new Model.Prestashop.PsProduct();
                                // lecture des valeurs pour F_ARTICLE
                                this.ReadWeight(F_ARTICLE, temp_product_f_artenumref);
                                this.ReadQuantity(F_ARTICLE, temp_product_f_artenumref);

                                // <JG> 03/06/2016
                                if (Article.Art_SyncPrice)
                                    this.ReadPrice(F_ARTICLE, temp_product_f_artenumref, TaxeTVA);

                                // transformation en fonction de f_artenumref
                                this.ReadWeightAttribute(temp_product_f_artenumref, PsProductAttributeComposition, F_ARTICLE, item.EnumereGamme1);
                                PsProductAttributeComposition.Weight += temp_product_f_artenumref.Weight;
                                this.ReadQuantityAttribute(PsProductAttributeComposition, F_ARTICLE, item.EnumereGamme1, item.EnumereGamme2);
                                this.ReadRefEANAttribute(PsProductAttributeComposition, F_ARTICLE, item.EnumereGamme1, item.EnumereGamme2);

                                // <JG> 03/06/2016
                                if (Article.Art_SyncPrice)
                                    this.ReadPriceAttribute(temp_product_f_artenumref, PsProductAttributeComposition, F_ARTICLE, item.EnumereGamme1, item.EnumereGamme2, TaxeTVA);

                                PsProductAttributeComposition.Price += temp_product_f_artenumref.Price;
                            }
                            else if (F_ARTICLE_Composition.F_CONDITION_SagId != null)
                            {
                                Model.Prestashop.PsProduct temp_product_f_condition = new Model.Prestashop.PsProduct();
                                // lecture des valeurs pour F_ARTICLE
                                this.ReadWeight(F_ARTICLE, temp_product_f_condition);
                                this.ReadQuantity(F_ARTICLE, temp_product_f_condition);

                                // <JG> 03/06/2016
                                if (Article.Art_SyncPrice)
                                    this.ReadPrice(F_ARTICLE, temp_product_f_condition, TaxeTVA);

                                // transformation en fonction de F_CONDITION
                                this.ReadWeightConditioning(temp_product_f_condition, PsProductAttributeComposition, item.EnumereF_CONDITION);
                                PsProductAttributeComposition.Weight += temp_product_f_condition.Weight;
                                this.ReadQuantityConditioning(PsProductAttributeComposition, F_ARTICLE, item.EnumereF_CONDITION);

                                // <JG> 03/06/2016
                                if (Article.Art_SyncPrice)
                                    this.ReadPriceConditioning(temp_product_f_condition, PsProductAttributeComposition, F_ARTICLE, item.EnumereF_CONDITION, TaxeTVA);

                                PsProductAttributeComposition.Price += temp_product_f_condition.Price;
                            }
                            // déplacer afin de renseigner les informations si absence de données sur la gamme ou le conditionnement.
                            if (string.IsNullOrEmpty(PsProductAttributeComposition.Reference))
                                PsProductAttributeComposition.Reference = F_ARTICLE.AR_Ref;
                            if (string.IsNullOrEmpty(PsProductAttributeComposition.EAn13))
                                PsProductAttributeComposition.EAn13 = Core.Global.RemovePurgeEAN(F_ARTICLE.AR_CodeBarre);

                            PsProductAttributeComposition.DefaultOn = (item.ComArt_Default) ? (byte)1 : 
								#if (PRESTASHOP_VERSION_161 || PRESTASHOP_VERSION_172)
								(byte?)null;
								#else
								(byte)0;
								#endif

                            // update others ps_product_attribute_lines
                            if (item.ComArt_Default)
                                PsProductAttributeRepository.EraseDefault(PsProductAttributeComposition.IDProduct, PsProductAttributeComposition.IDProductAttribute);

                            if (isProductAttribute == true)
                            {
                                PsProductAttributeRepository.Save();
                                this.ExecShopProductAttribute(PsProductAttributeComposition);
                            }
                            else
                            {
                                PsProductAttributeComposition.IDProduct = Product.IDProduct;
                                PsProductAttributeComposition.MinimalQuantity = 1;
                                if (Core.Global.GetConfig().ArticleQuantiteMiniActif)
                                    ReadMinimalQuantity(PsProductAttributeComposition, F_ARTICLE);
                                PsProductAttributeRepository.Add(PsProductAttributeComposition, Global.CurrentShop.IDShop);

                                item.Pre_Id = (Int32)PsProductAttributeComposition.IDProductAttribute;
                                CompositionArticleRepository.Save();
                            }

                            if (item.ComArt_Default)
                            {
                                Product.CacheDefaultAttribute = PsProductAttributeComposition.IDProductAttribute;
                                PsProductRepository.Save();
                            }

                            #region lien déclinaison - attributs
                            Model.Prestashop.PsProductAttributeCombinationRepository PsProductAttributeCombinationRepository = new Model.Prestashop.PsProductAttributeCombinationRepository();
                            Model.Local.CompositionArticleAttributeRepository CompositionArticleAttributeRepository = new Model.Local.CompositionArticleAttributeRepository();
                            PsProductAttributeCombinationRepository.DeleteAll(PsProductAttributeCombinationRepository.ListProductAttribute(PsProductAttributeComposition.IDProductAttribute));
                            foreach (Model.Local.CompositionArticleAttribute CompositionArticleAttribute in CompositionArticleAttributeRepository.ListCompositionArticle(item.ComArt_Id))
                            {
                                if (PsProductAttributeCombinationRepository.ExistAttributeProductAttribute((UInt32)CompositionArticleAttribute.Caa_Attribute_PreId, PsProductAttributeComposition.IDProductAttribute) == false)
                                {
                                    Model.Prestashop.PsProductAttributeCombination PsProductAttributeCombination = new Model.Prestashop.PsProductAttributeCombination();
                                    PsProductAttributeCombination.IDAttribute = (UInt32)CompositionArticleAttribute.Caa_Attribute_PreId;
                                    PsProductAttributeCombination.IDProductAttribute = PsProductAttributeComposition.IDProductAttribute;
                                    PsProductAttributeCombinationRepository.Add(PsProductAttributeCombination);
                                }
                            }
                            #endregion

                            WriteStockAvailableComposition(Product, PsProductAttributeComposition);

                            // prix spécifiques
                            // <JG> 03/06/2016
                            if (Article.Art_SyncPrice)
                            {
                                temp_product.Price = PsProductAttributeComposition.Price;
                                temp_product.IDProduct = Product.IDProduct;
                                List<string> log;
                                // suppression des tarifs toutes déclinaisons en cas de saisie manuelle dans PrestaShop ou de transformation d'article
                                new Model.Prestashop.PsSpecificPriceRepository().DeleteFromProductAttribute(Product.IDProduct, 0);
                                this.ExecSpecificPrice(F_ARTICLE, temp_product, Article, item, TaxeTVA, TaxeEco, out log);
                                if (log != null && log.Count > 0)
                                    log_chrono.AddRange(log);
                            }
                        }
                    }
                    catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
            }
            new Model.Prestashop.PsProductAttributeRepository().WriteDate(Product.IDProduct);
        }

        public void WriteStockAvailableComposition(Model.Prestashop.PsProduct Product, Model.Prestashop.PsProductAttribute PsProductAttributeComposition)
        {
            Model.Prestashop.PsStockAvailableRepository PsStockAvailableRepository = new Model.Prestashop.PsStockAvailableRepository();
            Model.Prestashop.PsStockAvailable PsStockAvailable = new Model.Prestashop.PsStockAvailable();
            if (PsStockAvailableRepository.ExistProductAttributeShop(Product.IDProduct, 0, Global.CurrentShop.IDShop))
            {
                PsStockAvailable = PsStockAvailableRepository.ReadProductAttributeShop(Product.IDProduct, 0, Global.CurrentShop.IDShop);
                PsStockAvailable.Quantity += PsProductAttributeComposition.Quantity;
                // <JG> 19/11/2014 correction ligne de stock ps>=1.6.0.9
                PsStockAvailable.IDShopGroup = 0;
                PsStockAvailableRepository.Save();
            }
            else
            {
                PsStockAvailable.IDProduct = Product.IDProduct;
                PsStockAvailable.IDShop = Global.CurrentShop.IDShop;
                // <JG> 19/11/2014 correction ligne de stock ps>=1.6.0.9
                PsStockAvailable.IDShopGroup = 0;
                PsStockAvailable.Quantity += PsProductAttributeComposition.Quantity;
                PsStockAvailable.IDProductAttribute = 0;
                PsStockAvailable.OutOfStock = (byte)Product.OutOfStock;
                PsStockAvailableRepository.Add(PsStockAvailable);
            }

            if (Core.Global.GetConfig().ModuleAECStockActif && Core.Global.ExistAECStockModule())
            {
                WriteAdvancedStock(Product, PsStockAvailable.IDStockAvailable);
            }

            if (PsStockAvailableRepository.ExistProductAttributeShop(PsProductAttributeComposition.IDProduct, PsProductAttributeComposition.IDProductAttribute, Global.CurrentShop.IDShop))
            {
                PsStockAvailable = PsStockAvailableRepository.ReadProductAttributeShop(PsProductAttributeComposition.IDProduct, PsProductAttributeComposition.IDProductAttribute, Global.CurrentShop.IDShop);
                PsStockAvailable.Quantity = PsProductAttributeComposition.Quantity;
                // <JG> 19/11/2014 correction ligne de stock ps>=1.6.0.9
                PsStockAvailable.IDShopGroup = 0;
                PsStockAvailableRepository.Save();
            }
            else
            {
                PsStockAvailable = new Model.Prestashop.PsStockAvailable();
                PsStockAvailable.IDProduct = PsProductAttributeComposition.IDProduct;
                PsStockAvailable.IDShop = Global.CurrentShop.IDShop;
                // <JG> 19/11/2014 correction ligne de stock ps>=1.6.0.9
                PsStockAvailable.IDShopGroup = 0;
                PsStockAvailable.Quantity = PsProductAttributeComposition.Quantity;
                PsStockAvailable.IDProductAttribute = PsProductAttributeComposition.IDProductAttribute;
                PsStockAvailable.OutOfStock = (byte)Product.OutOfStock;
                PsStockAvailableRepository.Add(PsStockAvailable);
            }
            if (Core.Global.GetConfig().ModuleAECStockActif)
            {
                WriteAdvancedStock(PsProductAttributeComposition, PsStockAvailable.IDStockAvailable);
            }
        }

        #endregion

        #region maj boutique

        // <JG> 17/12/2012 Correction mis-à-jour informations par boutique
        public void ExecShopProduct(Model.Prestashop.PsProduct PsProduct)
        {
            Model.Prestashop.PsProductShopRepository PsProductShopRepository = new Model.Prestashop.PsProductShopRepository();
            //Si le produit n'existe pas dans la boutique, il est rajouté.
            if (!PsProductShopRepository.ExistProductShop(PsProduct.IDProduct, Core.Global.CurrentShop.IDShop))
            {
                PsProductShopRepository.Add(new Model.Prestashop.PsProductShop()
                {
                    Active = PsProduct.Active,
                    AdditionalShippingCost = PsProduct.AdditionalShippingCost,
                    AdvancedStockManagement = PsProduct.AdvancedStockManagement,
                    AvailableDate = PsProduct.AvailableDate,
                    AvailableForOrder = PsProduct.AvailableForOrder,
                    CacheDefaultAttribute = PsProduct.CacheDefaultAttribute,
                    Customizable = PsProduct.Customizable,
                    DateAdd = PsProduct.DateAdd,
                    DateUpd = PsProduct.DateUpd,
                    EcOtAx = PsProduct.EcOtAx,
                    IDCategoryDefault = PsProduct.IDCategoryDefault,
                    IDProduct = PsProduct.IDProduct,
                    IDShop = Core.Global.CurrentShop.IDShop,
                    IDTaxRulesGroup = PsProduct.IDTaxRulesGroup,
                    Indexed = PsProduct.Indexed,
                    MinimalQuantity = PsProduct.MinimalQuantity,
                    OnlineOnly = PsProduct.OnlineOnly,
                    OnSale = PsProduct.OnSale,
                    Price = PsProduct.Price,
                    ShowPrice = PsProduct.ShowPrice,
                    TextFields = PsProduct.TextFields,
                    UnitPriceRatio = PsProduct.UnitPriceRatio,
                    Unity = PsProduct.Unity,
                    UploadAbleFiles = PsProduct.UploadAbleFiles,
                    WholesalePrice = PsProduct.WholesalePrice,
					#if (PRESTASHOP_VERSION_161 || PRESTASHOP_VERSION_172)
                    PackStockType = PsProduct.PackStockType,
					#endif
                });
            }
            else
            {
                Model.Prestashop.PsProductShop PsProductShop = PsProductShopRepository.ReadProductShop(PsProduct.IDProduct, Core.Global.CurrentShop.IDShop);
                PsProductShop.Active = PsProduct.Active;
                PsProductShop.AdditionalShippingCost = PsProduct.AdditionalShippingCost;
                PsProductShop.AdvancedStockManagement = PsProduct.AdvancedStockManagement;
                PsProductShop.AvailableDate = PsProduct.AvailableDate;
                PsProductShop.AvailableForOrder = PsProduct.AvailableForOrder;
                PsProductShop.CacheDefaultAttribute = PsProduct.CacheDefaultAttribute;
                PsProductShop.Customizable = PsProduct.Customizable;
                PsProductShop.DateAdd = PsProduct.DateAdd;
                PsProductShop.DateUpd = PsProduct.DateUpd;
                PsProductShop.EcOtAx = PsProduct.EcOtAx;
                PsProductShop.IDCategoryDefault = PsProduct.IDCategoryDefault;
                PsProductShop.IDTaxRulesGroup = PsProduct.IDTaxRulesGroup;
                PsProductShop.Indexed = PsProduct.Indexed;
                PsProductShop.MinimalQuantity = PsProduct.MinimalQuantity;
                PsProductShop.OnlineOnly = PsProduct.OnlineOnly;
                PsProductShop.OnSale = PsProduct.OnSale;
                PsProductShop.Price = PsProduct.Price;
                PsProductShop.ShowPrice = PsProduct.ShowPrice;
                PsProductShop.TextFields = PsProduct.TextFields;
                PsProductShop.UnitPriceRatio = PsProduct.UnitPriceRatio;
                PsProductShop.Unity = PsProduct.Unity;
                PsProductShop.UploadAbleFiles = PsProduct.UploadAbleFiles;
                PsProductShop.WholesalePrice = PsProduct.WholesalePrice;
				#if (PRESTASHOP_VERSION_161 || PRESTASHOP_VERSION_172)
                PsProductShop.PackStockType = PsProduct.PackStockType;
				#endif
                PsProductShopRepository.Save();
            }
            PsProductShopRepository.WriteDate(PsProduct.IDProduct);
        }

        public void ExecShopProductAttribute(Model.Prestashop.PsProductAttribute PsProductAttribute)
        {
            Model.Prestashop.PsProductAttributeShopRepository PsProductAttributeShopRepository = new Model.Prestashop.PsProductAttributeShopRepository();
            if (!PsProductAttributeShopRepository.ExistPsProductAttributeShop(PsProductAttribute.IDProductAttribute, Global.CurrentShop.IDShop))
            {
                PsProductAttributeShopRepository.Add(new Model.Prestashop.PsProductAttributeShop()
                {
                    AvailableDate = PsProductAttribute.AvailableDate,
                    DefaultOn = PsProductAttribute.DefaultOn,
                    EcOtAx = PsProductAttribute.EcOtAx,
                    IDProductAttribute = PsProductAttribute.IDProductAttribute,
                    IDShop = Global.CurrentShop.IDShop,
                    MinimalQuantity = PsProductAttribute.MinimalQuantity,
                    Price = PsProductAttribute.Price,
                    UnitPriceImpact = PsProductAttribute.UnitPriceImpact,
                    Weight = PsProductAttribute.Weight,
                    WholesalePrice = PsProductAttribute.WholesalePrice,
					#if (PRESTASHOP_VERSION_161 || PRESTASHOP_VERSION_172)
                    IDProduct = PsProductAttribute.IDProduct,
					#endif
                });
            }
            else
            {
                Model.Prestashop.PsProductAttributeShop PsProductAttributeShop = PsProductAttributeShopRepository.ReadPsProductAttributeShop(PsProductAttribute.IDProductAttribute, Global.CurrentShop.IDShop);
                PsProductAttributeShop.AvailableDate = PsProductAttribute.AvailableDate;
                PsProductAttributeShop.DefaultOn = PsProductAttribute.DefaultOn;
                PsProductAttributeShop.EcOtAx = PsProductAttribute.EcOtAx;
                PsProductAttributeShop.MinimalQuantity = PsProductAttribute.MinimalQuantity;
                PsProductAttributeShop.Price = PsProductAttribute.Price;
                PsProductAttributeShop.UnitPriceImpact = PsProductAttribute.UnitPriceImpact;
                PsProductAttributeShop.Weight = PsProductAttribute.Weight;
                PsProductAttributeShop.WholesalePrice = PsProductAttribute.WholesalePrice;
				#if (PRESTASHOP_VERSION_161 || PRESTASHOP_VERSION_172)
                PsProductAttributeShop.IDProduct = PsProductAttributeShop.IDProduct;
				#endif
                PsProductAttributeShopRepository.Save();
            }
            PsProductAttributeShopRepository.WriteDate(PsProductAttribute.IDProductAttribute);
        }

        #endregion

        #region Redirection

        public void DefineRedirection(Model.Local.Article Article)
        {
            try
            {
                using (MySqlConnection Connection = new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString))
                {
                    try
                    {
                        Connection.Open();
                        using (MySqlCommand Command = Connection.CreateCommand())
                        {
                            int? redirected_product = 0;
                            if (Article.Art_RedirectProduct != 0)
                            {
                                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                                if (ArticleRepository.ExistArticle(Article.Art_RedirectProduct))
                                    redirected_product = ArticleRepository.ReadArticle(Article.Art_RedirectProduct).Pre_Id;
                                if (redirected_product == null)
                                    redirected_product = 0;
                            }

                            Command.CommandText = "update ps_product " +
                                " set redirect_type = '" + ((!String.IsNullOrWhiteSpace(Article.Art_RedirectType)) ? Article.Art_RedirectType : "") + "' " +
								#if (PRESTASHOP_VERSION_172)
                                ", id_type_redirected = " + redirected_product + " " +
								#else
                                ", id_product_redirected = " + redirected_product + " " +
								#endif
                                " where id_product = " + Article.Pre_Id + ";";

                            Command.ExecuteNonQuery();
                            Command.CommandText = "update ps_product_shop " +
                                " set redirect_type = '" + ((!String.IsNullOrWhiteSpace(Article.Art_RedirectType)) ? Article.Art_RedirectType : "") + "' " +
								#if (PRESTASHOP_VERSION_172)
                                ", id_type_redirected = " + redirected_product + " " +
								#else
                                ", id_product_redirected = " + redirected_product + " " +
								#endif
                                " where id_product = " + Article.Pre_Id + " and id_shop = " + Core.Global.CurrentShop.IDShop + ";";

                            Command.ExecuteNonQuery();
                        }
                    }
                    finally
                    {
                        Connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        #endregion

        #region Fournisseur

        private void ExecSupplier(Model.Prestashop.PsProduct Product)
        {
            try
            {
                Model.Prestashop.PsProductSupplierRepository PsProductSupplierRepository = new Model.Prestashop.PsProductSupplierRepository();
                if (Product.IDSupplier == 0)
                {
                    if (PsProductSupplierRepository.ExistProduct(Product.IDProduct))
                    {
                        PsProductSupplierRepository.DeleteAll(PsProductSupplierRepository.ListProduct(Product.IDProduct));
                    }
                }
                else if (Product.IDSupplier != null)
                {
                    if (!PsProductSupplierRepository.ExistProductSupplier(Product.IDProduct, (uint)Product.IDSupplier))
                    {
                        PsProductSupplierRepository.Add(new Model.Prestashop.PsProductSupplier()
                        {
                            IDCurrency = 0,
                            IDProduct = Product.IDProduct,
                            IDProductAttribute = 0,
                            IDSupplier = (uint)Product.IDSupplier,
                            ProductSupplierPriceTe = 0,
                            ProductSupplierReference = string.Empty,
                        });
                    }
                }
            }
            catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
        }

        private void ExecSupplierFull(Model.Local.Article Article, Model.Prestashop.PsProduct Product, Model.Sage.F_ARTICLE F_ARTICLE, Model.Prestashop.PsProductRepository PsProductRepository)
        {
            try
            {
                if (F_ARTICLE.F_ARTFOURNISS != null && F_ARTICLE.F_ARTFOURNISS.Count > 0)
                {
                    Model.Local.SupplierRepository SupplierRepository = new Model.Local.SupplierRepository();
                    Model.Prestashop.PsSupplierRepository PsSupplierRepository = new Model.Prestashop.PsSupplierRepository();

                    foreach (Model.Sage.F_ARTFOURNISS F_ARTFOURNISS in F_ARTICLE.F_ARTFOURNISS)
                    {
                        if (F_ARTFOURNISS.F_COMPTET != null && SupplierRepository.ExistSag_Id(F_ARTFOURNISS.F_COMPTET.cbMarq))
                        {
                            Model.Local.Supplier Supplier = SupplierRepository.ReadSag_Id(F_ARTFOURNISS.F_COMPTET.cbMarq);
                            if (Supplier.Pre_Id != null && PsSupplierRepository.ExistId((int)Supplier.Pre_Id))
                            {
                                Model.Prestashop.PsProductSupplierRepository PsProductSupplierRepository = new Model.Prestashop.PsProductSupplierRepository();

                                if (!PsProductSupplierRepository.ExistProductSupplier(Product.IDProduct, (uint)Supplier.Pre_Id))
                                {
                                    PsProductSupplierRepository.Add(new Model.Prestashop.PsProductSupplier()
                                    {
                                        IDCurrency = 0,
                                        IDProduct = Product.IDProduct,
                                        IDProductAttribute = 0,
                                        IDSupplier = (uint)Supplier.Pre_Id,
                                        ProductSupplierPriceTe = 0,
                                        ProductSupplierReference = F_ARTFOURNISS.AF_RefFourniss,
                                    });
                                }
                                else
                                {
                                    Model.Prestashop.PsProductSupplier PsProductSupplier = PsProductSupplierRepository.ReadProductSupplier(Product.IDProduct, (uint)Supplier.Pre_Id);
                                    PsProductSupplier.ProductSupplierReference = F_ARTFOURNISS.AF_RefFourniss;
                                    PsProductSupplierRepository.Save();
                                }

                                if (F_ARTFOURNISS.AF_Principal != null && F_ARTFOURNISS.AF_Principal == 1)
                                {
                                    Product.IDSupplier = (uint)Supplier.Pre_Id;
                                    Product.SupplierReference = F_ARTFOURNISS.AF_RefFourniss;
                                    PsProductRepository.Save();
                                }

                                if (F_ARTICLE.AR_Gamme1 != null && F_ARTICLE.AR_Gamme1 != 0)
                                {
                                    Model.Sage.F_TARIFGAMRepository F_TARIFGAMRepository = new Model.Sage.F_TARIFGAMRepository();
                                    if (F_TARIFGAMRepository.ExistReferenceFournisseur(F_ARTICLE.AR_Ref, F_ARTFOURNISS.CT_Num))
                                    {
                                        Model.Local.AttributeArticleRepository AttributeArticleRepository = new Model.Local.AttributeArticleRepository();
                                        List<Model.Local.AttributeArticle> ListLocal = AttributeArticleRepository.ListArticle(Article.Art_Id);
                                        ListLocal = ListLocal.Where(at => at.Pre_Id != null).ToList();

                                        Model.Prestashop.PsProductAttributeRepository PsProductAttributeRepository = new Model.Prestashop.PsProductAttributeRepository();
                                        List<Model.Prestashop.PsProductAttribute> ListPrestaShop = PsProductAttributeRepository.List(Product.IDProduct);
                                        ListLocal = ListLocal.Where(at => ListPrestaShop.Count(pa => pa.IDProductAttribute == (uint)at.Pre_Id) > 0).ToList();

                                        foreach (Model.Local.AttributeArticle AttributeArticle in ListLocal)
                                        {
                                            Model.Sage.F_ARTENUMREF F_ARTENUMREF = AttributeArticle.EnumereF_ARTENUMREF;
                                            if (F_ARTENUMREF.AG_No1 != null && F_ARTENUMREF.AG_No1 != 0)
                                            {
                                                int AG_1 = F_ARTENUMREF.AG_No1.Value;
                                                int AG_2 = (F_ARTENUMREF.AG_No2 != null) ? F_ARTENUMREF.AG_No2.Value : 0;

                                                if (F_TARIFGAMRepository.ExistReferenceCategorieGamme1Gamme2(F_ARTICLE.AR_Ref, F_ARTFOURNISS.CT_Num, AG_1, AG_2))
                                                {
                                                    Model.Sage.F_TARIFGAM F_TARIFGAM = F_TARIFGAMRepository.ReadReferenceCategorieGamme1Gamme2(F_ARTICLE.AR_Ref, F_ARTFOURNISS.CT_Num, AG_1, AG_2);

                                                    if (!PsProductSupplierRepository.ExistProductAttributeSupplier(Product.IDProduct, (uint)Supplier.Pre_Id, (uint)AttributeArticle.Pre_Id))
                                                    {
                                                        PsProductSupplierRepository.Add(new Model.Prestashop.PsProductSupplier()
                                                        {
                                                            IDCurrency = 0,
                                                            IDProduct = Product.IDProduct,
                                                            IDProductAttribute = (uint)AttributeArticle.Pre_Id,
                                                            IDSupplier = (uint)Supplier.Pre_Id,
                                                            ProductSupplierPriceTe = 0,
                                                            ProductSupplierReference = F_TARIFGAM.TG_Ref,
                                                        });
                                                    }
                                                    else
                                                    {
                                                        Model.Prestashop.PsProductSupplier PsProductSupplier = PsProductSupplierRepository.ReadProductAttributeSupplier(Product.IDProduct, (uint)Supplier.Pre_Id, (uint)AttributeArticle.Pre_Id);
                                                        PsProductSupplier.ProductSupplierReference = F_TARIFGAM.TG_Ref;
                                                        PsProductSupplierRepository.Save();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
        }

        #endregion

        #region Stock avancé

        public void WriteAdvancedStock(Model.Prestashop.PsProduct Product, uint IDStockAvailable)
        {
            Model.Prestashop.PsAEcStock PsAecStock;
            Model.Prestashop.PsAECStockRepository PsAECStockRepository = new Model.Prestashop.PsAECStockRepository();
            if (PsAECStockRepository.Exist(IDStockAvailable))
            {
                PsAecStock = PsAECStockRepository.Read(IDStockAvailable);
            }
            else
            {
                PsAecStock = new Model.Prestashop.PsAEcStock();
                PsAecStock.IDStockAvailable = IDStockAvailable;
                PsAECStockRepository.Add(PsAecStock);
            }
            if (Product.AEC_Stock.Count_Supply > 0)
            {
                PsAecStock.MinimalQuantity = Product.AEC_Stock.MinimalQuantity / Product.AEC_Stock.Count_Supply;
                PsAecStock.MaximalQuantity = Product.AEC_Stock.MaximalQuantity / Product.AEC_Stock.Count_Supply;
            }
            PsAecStock.QuantityReal = Product.AEC_Stock.QuantityReal;
            PsAecStock.QuantityFuture = Product.AEC_Stock.QuantityFuture;
            PsAECStockRepository.Save();
        }

        public void WriteAdvancedStock(Model.Prestashop.PsProductAttribute PsProductAttribute, uint IDStockAvailable)
        {
            Model.Prestashop.PsAEcStock PsAecStock;
            Model.Prestashop.PsAECStockRepository PsAECStockRepository = new Model.Prestashop.PsAECStockRepository();
            if (PsAECStockRepository.Exist(IDStockAvailable))
            {
                PsAecStock = PsAECStockRepository.Read(IDStockAvailable);
            }
            else
            {
                PsAecStock = new Model.Prestashop.PsAEcStock();
                PsAecStock.IDStockAvailable = IDStockAvailable;
                PsAECStockRepository.Add(PsAecStock);
            }
            if (PsProductAttribute.AEC_Stock.Count_Supply > 0)
            {
                PsAecStock.MinimalQuantity = PsProductAttribute.AEC_Stock.MinimalQuantity / PsProductAttribute.AEC_Stock.Count_Supply;
                PsAecStock.MaximalQuantity = PsProductAttribute.AEC_Stock.MaximalQuantity / PsProductAttribute.AEC_Stock.Count_Supply;
            }
            PsAecStock.QuantityReal = PsProductAttribute.AEC_Stock.QuantityReal;
            PsAecStock.QuantityFuture = PsProductAttribute.AEC_Stock.QuantityFuture;
            PsAECStockRepository.Save();
        }

        #endregion

        #region Date dispo

        public void ReadDateDispo(Model.Prestashop.PsProduct Product, Model.Sage.F_ARTICLE F_ARTICLE)
        {
            try
            {
                if (Core.Global.GetConfig().ArticleDateDispoInfoLibreActif)
                {
                    if (!String.IsNullOrEmpty(Core.Global.GetConfig().ArticleDateDispoInfoLibreName))
                    {
                        DateTime? datedispo = null;
                        Model.Sage.F_ARTICLERepository F_ARTICLERespository = new Model.Sage.F_ARTICLERepository();
                        Model.Sage.cbSysLibreRepository.CB_Type cbtype = new Model.Sage.cbSysLibreRepository().ReadTypeInformationLibre(Core.Global.GetConfig().ArticleDateDispoInfoLibreName, Model.Sage.cbSysLibreRepository.CB_File.F_ARTICLE);
                        switch (cbtype)
                        {
                            case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageSmallDate:
                            case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageDate:
                                if (F_ARTICLERespository.ExistArticleInformationLibreDate(Core.Global.GetConfig().ArticleDateDispoInfoLibreName, F_ARTICLE.AR_Ref))
                                {
                                    datedispo = F_ARTICLERespository.ReadArticleInformationLibreDate(Core.Global.GetConfig().ArticleDateDispoInfoLibreName, F_ARTICLE.AR_Ref);
                                }
                                break;
                            case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageText:
                            case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageTable:
                                if (F_ARTICLERespository.ExistArticleInformationLibreText(Core.Global.GetConfig().ArticleDateDispoInfoLibreName, F_ARTICLE.AR_Ref))
                                {
                                    string result = F_ARTICLERespository.ReadArticleInformationLibreText(Core.Global.GetConfig().ArticleDateDispoInfoLibreName, F_ARTICLE.AR_Ref);
                                    if (!String.IsNullOrWhiteSpace(result) && Core.Global.IsDate(result.Trim()))
                                    {
                                        datedispo = DateTime.Parse(result.Trim());
                                    }
                                }
                                break;
                            case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.Deleted:
                            case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageValeur:
                            case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageMontant:
                            default:
                                break;
                        }

                        if (datedispo != null)
                            Product.AvailableDate = datedispo.Value;
                    }

                }
            }
            catch (Exception ex) { Core.Error.SendMailError("[ReadDateDispo]" + ex.ToString()); }
        }
        public void ReadDateDispo(Model.Prestashop.PsProductAttribute ProductAttribute, Model.Sage.F_ARTICLE F_ARTICLE)
        {
            Model.Prestashop.PsProduct Product = new Model.Prestashop.PsProduct();
            ReadDateDispo(Product, F_ARTICLE);
            if (Product.AvailableDate > new DateTime(1753, 01, 01))
            {
                ProductAttribute.AvailableDate = Product.AvailableDate;
            }
        }

        public void ReadMinimalQuantity(Model.Prestashop.PsProduct Product, Model.Sage.F_ARTICLE F_ARTICLE)
        {
            try
            {
                if (Core.Global.GetConfig().ArticleQuantiteMiniActif)
                {
                    if (Core.Global.GetConfig().ArticleQuantiteMiniConditionnement && F_ARTICLE.AR_Condition != null)
                    {
                        Model.Sage.P_CONDITIONNEMENTRepository P_CONDITIONNEMENTRepository = new Model.Sage.P_CONDITIONNEMENTRepository();
                        Model.Sage.P_CONDITIONNEMENT P_CONDITIONNEMENT = P_CONDITIONNEMENTRepository.ReadConditionnement(F_ARTICLE.AR_Condition.Value);
                        if (P_CONDITIONNEMENT != null
                            && !string.IsNullOrWhiteSpace(P_CONDITIONNEMENT.P_Conditionnement)
                            && Core.Global.IsNumericSimple(P_CONDITIONNEMENT.P_Conditionnement.Trim()))
                        {
                            int qty = int.Parse(P_CONDITIONNEMENT.P_Conditionnement.Trim());
                            Product.MinimalQuantity = (uint)qty;
                        }
                    }
                    else if (Core.Global.GetConfig().ArticleQuantiteMiniUniteVente && F_ARTICLE.AR_UniteVen != null)
                    {
                        Model.Sage.P_UNITERepository P_UNITERepository = new Model.Sage.P_UNITERepository();
                        Model.Sage.P_UNITE P_UNITE = P_UNITERepository.ReadUnite(F_ARTICLE.AR_UniteVen.Value);
                        if (P_UNITE != null
                            && !string.IsNullOrWhiteSpace(P_UNITE.U_Intitule)
                            && Core.Global.IsNumericSimple(P_UNITE.U_Intitule.Trim()))
                        {
                            int qty = int.Parse(P_UNITE.U_Intitule.Trim());
                            Product.MinimalQuantity = (uint)qty;
                        }
                    }
                }
            }
            catch (Exception ex) { Core.Error.SendMailError("[ReadMinimalQuantity]" + ex.ToString()); }
        }
        public void ReadMinimalQuantity(Model.Prestashop.PsProductAttribute ProductAttribute, Model.Sage.F_ARTICLE F_ARTICLE)
        {
            Model.Prestashop.PsProduct Product = new Model.Prestashop.PsProduct();
            ReadMinimalQuantity(Product, F_ARTICLE);
            if (Product.MinimalQuantity >= 0)
            {
                ProductAttribute.MinimalQuantity = Product.MinimalQuantity;
            }
        }

		#endregion

		#region DWFProductExtraField
		public void ExecDWFProductExtraField(Model.Prestashop.PsProduct Product, Model.Local.Article Article)
		{
			if (Core.Global.GetConfig().ModuleDWFProductGuideratesActif || Core.Global.GetConfig().ModuleDWFProductExtraFieldsActif)
			{
				List<Model.Local.ArticleAdditionalField> ListInfoPrestaConnect = new Model.Local.ArticleAdditionalFieldRepository().ListArticle(Core.Temp.Article);
				List<Model.Local.ArticleAdditionalField> ListResultAdditionnalFieldArticle = new List<Model.Local.ArticleAdditionalField>();
				if (Core.Global.GetConfig().ModuleDWFProductGuideratesActif)
				{
					List<Model.Prestashop.PsDWFProductGuiderates> ListeRates = new Model.Prestashop.PsDWFProductGuideratesRepository().ListActive();
					foreach (Model.Prestashop.PsDWFProductGuiderates rate in ListeRates)
					{
						string FieldValue1 = "";
						string FieldValue2 = "";
						int Id = 0;
						if (ListInfoPrestaConnect.Where(obj => obj.FieldName == rate.Name).Count() > 0)
						{
							Model.Local.ArticleAdditionalField info = ListInfoPrestaConnect.Where(obj => obj.FieldName == rate.Name).FirstOrDefault();
							FieldValue1 = info.FieldValue;
							FieldValue2 = info.FieldValue2;
							Id = (int)rate.IdDWFProductGuiderates;
						}
						ListResultAdditionnalFieldArticle.Add(new Model.Local.ArticleAdditionalField()
						{
							PsId = Id,
							Art_id = Core.Temp.Article,
							FieldName = rate.Name,
							FieldValue = FieldValue1,
							FieldValue2 = FieldValue2,
							Type = Model.Local.ArticleAdditionalField.ArticleAdditionalFieldType.Rate,
						});
					}
				}
				if (Core.Global.GetConfig().ModuleDWFProductExtraFieldsActif)
				{
					List<Model.Prestashop.PsDWFProductExtraField> ListExtraField = new Model.Prestashop.PsDWFProductExtraFieldRepository().ListActive();
					foreach (Model.Prestashop.PsDWFProductExtraField extraField in ListExtraField)
					{
						string FieldValue1 = "";
						string FieldValue2 = "";
						int Id = 0;
						if (ListInfoPrestaConnect.Where(obj => obj.FieldName == extraField.FieldName).Count() > 0)
						{
							Model.Local.ArticleAdditionalField info = ListInfoPrestaConnect.Where(obj => obj.FieldName == extraField.FieldName).FirstOrDefault();
							FieldValue1 = info.FieldValue;
							FieldValue2 = info.FieldValue2;
							Id = (int)extraField.IdDWFProductExtraFields;
						}
						ListResultAdditionnalFieldArticle.Add(new Model.Local.ArticleAdditionalField()
						{
							PsId = Id,
							Art_id = Core.Temp.Article,
							FieldName = extraField.FieldName,
							FieldValue = FieldValue1,
							FieldValue2 = FieldValue2,
							Type = Model.Local.ArticleAdditionalField.AffectType(extraField.Type),
						});
					}
				}

				Model.Prestashop.PsProductExtraFieldRepository psProductExtraFieldRepository = new Model.Prestashop.PsProductExtraFieldRepository();
				Model.Prestashop.PsProductExtraField psProductExtraField = new Model.Prestashop.PsProductExtraField();
				Model.Prestashop.PsProductExtraFieldShopRepository psProductExtraFieldShopRepository = new Model.Prestashop.PsProductExtraFieldShopRepository();
				Model.Prestashop.PsProductExtraFieldShop psProductExtraFieldShop = new Model.Prestashop.PsProductExtraFieldShop();
				Model.Prestashop.PsProductExtraFieldLangRepository psProductExtraFieldLangRepository = new Model.Prestashop.PsProductExtraFieldLangRepository();
				Model.Prestashop.PsProductExtraFieldLang psProductExtraFieldLang = new Model.Prestashop.PsProductExtraFieldLang();
				if (ListResultAdditionnalFieldArticle.Where(obj=> obj.Type != Model.Local.ArticleAdditionalField.ArticleAdditionalFieldType.Rate).Count() > 0)
				{
					// Partie PsProductGuideRate
					psProductExtraField.IdProduct = (uint)Article.Pre_Id;
					psProductExtraField.IdShopDefault = Core.Global.CurrentShop.IDShop;
					psProductExtraFieldRepository.Add(psProductExtraField);

					// partie PsProductGuideRateShop
					psProductExtraFieldShop.IdProductExtraField = (uint)psProductExtraField.IdProductExtraField;
					psProductExtraFieldShop.IdShop = Core.Global.CurrentShop.IDShop;
					psProductExtraFieldShopRepository.Add(psProductExtraFieldShop);

					// Partie PsProductGuideRateLang
					foreach (Model.Prestashop.PsLang PsLang in new Model.Prestashop.PsLangRepository().ListActive(1, Core.Global.CurrentShop.IDShop))
					{
						psProductExtraFieldLang.IdProductExtraField = (uint)psProductExtraField.IdProductExtraField;
						psProductExtraFieldLang.IdShop = Core.Global.CurrentShop.IDShop;
						psProductExtraFieldLang.IdLang = PsLang.IDLang;
						psProductExtraFieldLangRepository.Add(psProductExtraFieldLang);
					}
				}

				foreach(Model.Local.ArticleAdditionalField field in ListResultAdditionnalFieldArticle)
				{
					if (field.Type == Model.Local.ArticleAdditionalField.ArticleAdditionalFieldType.Rate)
					{
						// Partie PsProductGuideRate
						Model.Prestashop.PsProductGuideRateRepository psProductGuideRateRepository = new Model.Prestashop.PsProductGuideRateRepository();
						Model.Prestashop.PsProductGuideRate psProductGuideRate = new Model.Prestashop.PsProductGuideRate();
						psProductGuideRate.IdDwfproductguiderates = (uint)field.PsId;
						psProductGuideRate.IdProduct = (uint)Article.Pre_Id;
						psProductGuideRate.IdShopDefault = Core.Global.CurrentShop.IDShop;
						psProductGuideRate.Rate = field.FieldValue;
						psProductGuideRateRepository.Add(psProductGuideRate);

						// partie PsProductGuideRateShop
						Model.Prestashop.PsProductGuideRateShopRepository psProductGuideRateShopRepository = new Model.Prestashop.PsProductGuideRateShopRepository();
						Model.Prestashop.PsProductGuideRateShop psProductGuideRateShop = new Model.Prestashop.PsProductGuideRateShop();
						psProductGuideRateShop.IdProductGuideRate = (uint)psProductGuideRate.IdProductGuideRate;
						psProductGuideRateShop.IdShop = Core.Global.CurrentShop.IDShop;
						psProductGuideRateShopRepository.Add(psProductGuideRateShop);

						// Partie PsProductGuideRateLang
						foreach (Model.Prestashop.PsLang PsLang in new Model.Prestashop.PsLangRepository().ListActive(1, Core.Global.CurrentShop.IDShop))
						{
							Model.Prestashop.PsProductGuideRateLangRepository psProductGuideRateLangRepository = new Model.Prestashop.PsProductGuideRateLangRepository();
							Model.Prestashop.PsProductGuideRateLang psProductGuideRateLang = new Model.Prestashop.PsProductGuideRateLang();
							psProductGuideRateLang.IdProductGuideRate = (uint)psProductGuideRate.IdProductGuideRate;
							psProductGuideRateLang.IdShop = Core.Global.CurrentShop.IDShop;
							psProductGuideRateLang.IdLang = PsLang.IDLang;
							psProductGuideRateLang.Comment = field.FieldValue2;
							psProductGuideRateLangRepository.Add(psProductGuideRateLang);
						}
					}
					else
					{
						psProductExtraFieldLangRepository.UpdateExtraField(psProductExtraFieldLang, field.FieldName, field.FieldValue);
						psProductExtraFieldRepository.UpdateExtraField(psProductExtraField, field.FieldName, field.FieldValue);
						psProductExtraFieldShopRepository.UpdateExtraField(psProductExtraFieldShop, field.FieldName, field.FieldValue);
					}
				}
			}
		}
		#endregion
	}

	public class client_key
    {
        // default 0 = Product price
        public int product_attribute = 0;
        public int tiers_cbMarq = 0;
        public long pallierqte = 0;

        public decimal enumere_price_basic = 0;

        public int prestashop_group = 0;
        public int? cattarif_cbMarq = 0;

        public decimal remise_articleclient = 0;
        public decimal remise_articlecategorie = 0;
        public decimal remise_familleclient = 0;
        public decimal remise_famillecategorie = 0;
        public decimal remise_client = 0;

        public bool horsremise = false;

        public bool conflitderemise()
        {
            bool remise = false;
            bool conflit = false;

            if (remise_articleclient != 0)
                remise = true;

            if (remise_articlecategorie != 0)
                if (remise)
                    conflit = true;
                else
                    remise = true;

            if (remise_client != 0)
                if (remise)
                    conflit = true;
                else
                    remise = true;

            if (remise_familleclient != 0)
                if (remise)
                    conflit = true;
                else
                    remise = true;

            if (remise_famillecategorie != 0)
                if (remise)
                    conflit = true;
                else
                    remise = true;

            return conflit;
        }

        public decimal meilleureremise()
        {
            decimal meilleure_remise = 0;

            if (remise_articleclient > meilleure_remise)
                meilleure_remise = remise_articleclient;

            if (remise_articlecategorie > meilleure_remise)
                meilleure_remise = remise_articlecategorie;

            if (remise_client > meilleure_remise)
                meilleure_remise = remise_client;

            if (remise_familleclient > meilleure_remise)
                meilleure_remise = remise_familleclient;

            if (remise_famillecategorie > meilleure_remise)
                meilleure_remise = remise_famillecategorie;

            return meilleure_remise;
        }

        public decimal prioritearticlefamilleclient()
        {
            decimal priorite_remise = 0;

            if (remise_articleclient != 0)
                priorite_remise = remise_articleclient;
            else if (remise_articlecategorie != 0)
                priorite_remise = remise_articlecategorie;
            else if (remise_familleclient != 0)
                priorite_remise = remise_familleclient;
            else if (remise_famillecategorie != 0)
                priorite_remise = remise_famillecategorie;
            else if (remise_client != 0)
                priorite_remise = remise_client;

            return priorite_remise;
        }

        public Model.Prestashop.PsSpecificPrice PsSpecificPrice = new Model.Prestashop.PsSpecificPrice();

        public client_key(int productattribute, int tiers_cbmarq, int? tiers_cattarif, long pallier)
        {
            product_attribute = productattribute;
            tiers_cbMarq = tiers_cbmarq;
            cattarif_cbMarq = tiers_cattarif;
            pallierqte = pallier;
        }
        public client_key(int productattribute, int tiers_cbmarq, int? tiers_cattarif, long pallier, decimal remise_articlecat, decimal remise_famillecat)
        {
            product_attribute = productattribute;
            tiers_cbMarq = tiers_cbmarq;
            cattarif_cbMarq = tiers_cattarif;
            pallierqte = pallier;
            remise_articlecategorie = remise_articlecat;
            remise_famillecategorie = remise_famillecat;
        }

        public override string ToString()
        {
            return "Produit/Déclinaison : " + PsSpecificPrice.IDProduct + "-" + product_attribute + " / Client : " + tiers_cbMarq + " / PallierQte : " + pallierqte + " / Prix : " + PsSpecificPrice.Price + " / Remise : " + PsSpecificPrice.Reduction;
        }
    }
    public class group_key
    {
        public int product_attribute = 0;
        public int prestashop_group = 0;
        public int? cattarif_cbMarq = 0;
        public long pallierqte = 0;

        public decimal enumere_price_basic = 0;

        public decimal remise_articlecategorie = 0;
        public decimal remise_famillecategorie = 0;

        public bool horsremise_article = false;
        public bool horsremise_famille = false;

        public bool conflitderemise()
        {
            bool remise = false;
            bool conflit = false;

            if (remise_articlecategorie != 0)
                if (remise)
                    conflit = true;
                else
                    remise = true;

            if (remise_famillecategorie != 0)
                if (remise)
                    conflit = true;
                else
                    remise = true;

            return conflit;
        }

        public decimal meilleureremise()
        {
            decimal meilleure_remise = 0;

            if (remise_articlecategorie > meilleure_remise)
                meilleure_remise = remise_articlecategorie;

            if (remise_famillecategorie > meilleure_remise)
                meilleure_remise = remise_famillecategorie;

            return meilleure_remise;
        }

        public decimal prioritearticlefamille()
        {
            decimal priorite_remise = 0;

            if (remise_articlecategorie != 0)
                priorite_remise = remise_articlecategorie;
            else if (remise_famillecategorie != 0)
                priorite_remise = remise_famillecategorie;

            return priorite_remise;
        }

        public Model.Prestashop.PsSpecificPrice PsSpecificPrice = new Model.Prestashop.PsSpecificPrice();

        public group_key(int productattribute, int group_pre_id, int? group_cattarif, long pallier, decimal enumerepricebasic)
        {
            product_attribute = productattribute;
            prestashop_group = group_pre_id;
            cattarif_cbMarq = group_cattarif;
            pallierqte = pallier;
            enumere_price_basic = enumerepricebasic;
        }

        public override string ToString()
        {
            return "Produit/Déclinaison : " + PsSpecificPrice.IDProduct + "-" + product_attribute + " / Groupe : " + prestashop_group + " / CatTarif : " + cattarif_cbMarq + " / PallierQte : " + pallierqte + " / Base/Prix : " + enumere_price_basic + " " + PsSpecificPrice.Price + " / Remise : " + PsSpecificPrice.Reduction;
        }
    }
}
