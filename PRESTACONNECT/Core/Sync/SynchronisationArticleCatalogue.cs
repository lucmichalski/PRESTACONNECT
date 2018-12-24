using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PRESTACONNECT.Core.Parametres;

namespace PRESTACONNECT.Core.Sync
{
    public class SynchronisationArticleCatalogue
    {
        public void Exec(Int32 ArticleSend)
        {
            try
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                Model.Local.Article Article = ArticleRepository.ReadArticle(ArticleSend);

                // If the catalog is sync with Prestashop
                //if (Article.Catalog.Pre_Id != null || Article.Catalog.Pre_Id != 0)
                {
                    Model.Prestashop.PsProductRepository ProductRepository = new Model.Prestashop.PsProductRepository();
                    Boolean isProduct = false;
                    // If the Article have a connection with Prestashop
                    if (Article.Pre_Id != null
                        && ProductRepository.ExistId(Convert.ToUInt32(Article.Pre_Id.Value)))
                    {
                        Model.Prestashop.PsProduct Product = ProductRepository.ReadId(Convert.ToUInt32(Article.Pre_Id.Value));
                        isProduct = true;
                        if (Product.DateUpd.Ticks > Article.Art_Date.Ticks)
                        {
                            this.ExecDistantToLocal(Product, Article, ArticleRepository);
                        }
                        else if (Product.DateUpd.Ticks < Article.Art_Date.Ticks)
                        {
                            this.ExecLocalToDistant(Article, Product, ArticleRepository, ProductRepository, isProduct);
                        }
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
                if (isProduct)
                {
                    Model.Local.CatalogRepository CatalogRepository = new Model.Local.CatalogRepository();
                    Model.Local.Catalog Catalog = CatalogRepository.ReadId(Article.Cat_Id);

                    if (Catalog.Pre_Id != null && new Model.Prestashop.PsCategoryRepository().ExistId((int)Catalog.Pre_Id))
                    {
                        Product.IDCategoryDefault = Convert.ToUInt32(Catalog.Pre_Id);

                        ProductRepository.Save();

                        this.AssignCatalogProduct(Article, Product, ArticleRepository);

                        // <JG> 17/12/2012
                        this.ExecShopProduct(Product);
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

                #region Recovery Data From CategoryProduct
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
                #endregion
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void AssignCatalogProduct(Model.Local.Article Article, Model.Prestashop.PsProduct Product, Model.Local.ArticleRepository ArticleRepository)
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

        public void ExecShopProduct(Model.Prestashop.PsProduct PsProduct)
        {
            Model.Prestashop.PsProductShopRepository PsProductShopRepository = new Model.Prestashop.PsProductShopRepository();
            //Si le produit n'existe pas dans la boutique, il est rajouté.
            if (!PsProductShopRepository.ExistProductShop(PsProduct.IDProduct, Core.Global.CurrentShop.IDShop))
            {
                //PsProductShopRepository.Add(new Model.Prestashop.PsProductShop()
                //{
                //    Active = PsProduct.Active,
                //    AdditionalShippingCost = PsProduct.AdditionalShippingCost,
                //    AdvancedStockManagement = PsProduct.AdvancedStockManagement,
                //    AvailableDate = PsProduct.AvailableDate,
                //    AvailableForOrder = PsProduct.AvailableForOrder,
                //    CacheDefaultAttribute = PsProduct.CacheDefaultAttribute,
                //    Customizable = PsProduct.Customizable,
                //    DateAdd = PsProduct.DateAdd,
                //    DateUpd = PsProduct.DateUpd,
                //    EcOtAx = PsProduct.EcOtAx,
                //    IDCategoryDefault = PsProduct.IDCategoryDefault,
                //    IDProduct = PsProduct.IDProduct,
                //    IDShop = Core.Global.CurrentShop.IDShop,
                //    IDTaxRulesGroup = PsProduct.IDTaxRulesGroup,
                //    Indexed = PsProduct.Indexed,
                //    MinimalQuantity = PsProduct.MinimalQuantity,
                //    OnlineOnly = PsProduct.OnlineOnly,
                //    OnSale = PsProduct.OnSale,
                //    Price = PsProduct.Price,
                //    ShowPrice = PsProduct.ShowPrice,
                //    TextFields = PsProduct.TextFields,
                //    UnitPriceRatio = PsProduct.UnitPriceRatio,
                //    Unity = PsProduct.Unity,
                //    UploadAbleFiles = PsProduct.UploadAbleFiles,
                //    WholesalePrice = PsProduct.WholesalePrice,
                //});
            }
            else
            {
                Model.Prestashop.PsProductShop PsProductShop = PsProductShopRepository.ReadProductShop(PsProduct.IDProduct, Core.Global.CurrentShop.IDShop);
                //PsProductShop.Active = PsProduct.Active;
                //PsProductShop.AdditionalShippingCost = PsProduct.AdditionalShippingCost;
                //PsProductShop.AdvancedStockManagement = PsProduct.AdvancedStockManagement;
                //PsProductShop.AvailableDate = PsProduct.AvailableDate;
                //PsProductShop.AvailableForOrder = PsProduct.AvailableForOrder;
                //PsProductShop.CacheDefaultAttribute = PsProduct.CacheDefaultAttribute;
                //PsProductShop.Customizable = PsProduct.Customizable;
                //PsProductShop.DateAdd = PsProduct.DateAdd;
                //PsProductShop.DateUpd = PsProduct.DateUpd;
                //PsProductShop.EcOtAx = PsProduct.EcOtAx;
                PsProductShop.IDCategoryDefault = PsProduct.IDCategoryDefault;
                //PsProductShop.IDTaxRulesGroup = PsProduct.IDTaxRulesGroup;
                //PsProductShop.Indexed = PsProduct.Indexed;
                //PsProductShop.MinimalQuantity = PsProduct.MinimalQuantity;
                //PsProductShop.OnlineOnly = PsProduct.OnlineOnly;
                //PsProductShop.OnSale = PsProduct.OnSale;
                //PsProductShop.Price = PsProduct.Price;
                //PsProductShop.ShowPrice = PsProduct.ShowPrice;
                //PsProductShop.TextFields = PsProduct.TextFields;
                //PsProductShop.UnitPriceRatio = PsProduct.UnitPriceRatio;
                //PsProductShop.Unity = PsProduct.Unity;
                //PsProductShop.UploadAbleFiles = PsProduct.UploadAbleFiles;
                //PsProductShop.WholesalePrice = PsProduct.WholesalePrice;
                PsProductShopRepository.Save();
            }
        }
    }
}
