using System;
using System.Collections.Generic;

namespace PRESTACONNECT.Core.ImportPrestashop
{
    public class ImportCatalogue
    {
        private Model.Local.Catalog GetCatalogue(Model.Prestashop.PsCategory category, Model.Prestashop.PsCategoryLang lang, int sageId)
        {
            Model.Local.Catalog result = new Model.Local.Catalog()
            {
                Cat_Name = lang.Name,
                Cat_Description = lang.Description,
                Cat_Level = category.LevelDepth,
                Cat_MetaTitle = lang.MetaTitle,
                Cat_MetaKeyword = lang.MetaKeywords,
                Cat_Active = Convert.ToBoolean(category.Active),
                Cat_MetaDescription = lang.MetaDescription,
                Cat_LinkRewrite = lang.LinkRewrite,
                Cat_Sync = true,
                Cat_Date = (category.DateUpd != null && category.DateUpd > new DateTime(1753, 1, 2)) ? category.DateUpd : DateTime.Now.Date,
                Sag_Id = sageId,
                Pre_Id = Convert.ToInt32(category.IDCategory)
            };

            return result;
        }

        //public void Exec(Int32 CategorySend)
        //{
        //    try
        //    {
        //        if (CategorySend != 1)
        //        {
        //            CatalogRepository CatalogRepository = new CatalogRepository();
        //            if (CatalogRepository.ExistPre_Id(CategorySend) == false)
        //            {
        //                PsCategoryLangRepository PsCategoryLangRepository = new PsCategoryLangRepository();
        //                if (PsCategoryLangRepository.ExistCategoryLang(CategorySend, Core.Global.Lang))
        //                {
        //                    PsCategoryRepository PsCategoryRepository = new PsCategoryRepository();
        //                    PsCategory PsCategory = PsCategoryRepository.ReadId(CategorySend);
        //                    PsCategoryLang PsCategoryLang = PsCategoryLangRepository.ReadCategoryLang(CategorySend, Core.Global.Lang);


        //                    F_CATALOGUERepository F_CATALOGUERepository = new F_CATALOGUERepository();
        //                    if (F_CATALOGUERepository.ExistIntituleNiveau(PsCategoryLang.Name, Convert.ToInt16(PsCategory.LevelDepth - 1)))
        //                    {
        //                        F_CATALOGUE F_CATALOGUE = F_CATALOGUERepository.ReadIntituleNiveau(PsCategoryLang.Name, Convert.ToInt16(PsCategory.LevelDepth - 1));
        //                        Catalog Catalog = new Catalog()
        //                        {
        //                            Cat_Name = PsCategoryLang.Name,
        //                            Cat_Description = PsCategoryLang.Description,
        //                            Cat_Level = PsCategory.LevelDepth,
        //                            Cat_MetaTitle = PsCategoryLang.MetaTitle,
        //                            Cat_MetaKeyword = PsCategoryLang.MetaKeywords,
        //                            Cat_Active = Convert.ToBoolean(PsCategory.Active),
        //                            Cat_MetaDescription = PsCategoryLang.MetaDescription,
        //                            Cat_LinkRewrite = PsCategoryLang.LinkRewrite,
        //                            Cat_Sync = true,
        //                            Cat_Date = PsCategory.DateUpd,
        //                            Sag_Id = F_CATALOGUE.cbMarq,
        //                            Pre_Id = Convert.ToInt32(PsCategory.IDCategory)
        //                        };

        //                        if (PsCategory.IDParent != 0 && PsCategory.IDParent != 1)
        //                        {
        //                            if (PsCategoryRepository.ExistId(Convert.ToInt32(PsCategory.IDParent)))
        //                            {
        //                                PsCategory PsCategoryParent = PsCategoryRepository.ReadId(Convert.ToInt32(PsCategory.IDParent));
        //                                if (CatalogRepository.ExistPre_Id(Convert.ToInt32(PsCategoryParent.IDCategory)))
        //                                {
        //                                    Catalog CatalogParent = CatalogRepository.ReadPre_Id(Convert.ToInt32(PsCategoryParent.IDCategory));
        //                                    Catalog.Cat_Parent = CatalogParent.Cat_Id;
        //                                }
        //                            }
        //                        }
        //                        CatalogRepository.Add(Catalog);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Core.Error.SendMailError(ex.ToString());
        //    }
        //}


        //public void Exec(Int32 CategorySend)
        //{
        //    try
        //    {
        //        if (CategorySend != 1)
        //        {
        //            CatalogRepository catalogs = new CatalogRepository();

        //            //Vérifie si le catalogue PS est référence dans PC
        //            if (!catalogs.ExistPre_Id(CategorySend))
        //            {
        //                PsCategoryLangRepository langs = new PsCategoryLangRepository();

        //                //Vérifie que le catalogue existe bien dans la langue définit dans PC
        //                if (langs.ExistCategoryLang(CategorySend, Global.Lang,Global.CurrentShop.IDShop))
        //                {
        //                    PsCategoryRepository categories = new PsCategoryRepository();

        //                    //Récupération du catalogue dans PS
        //                    PsCategory category = categories.ReadId(CategorySend);
        //                    PsCategoryLang lang = langs.ReadCategoryLang(CategorySend, Global.Lang, Global.CurrentShop.IDShop);

        //                    //Récupération de tous les catalogues S suceptible de correspondre au catalogue PS
        //                    List<F_CATALOGUE> correspondances = new List<F_CATALOGUE>(
        //                        new F_CATALOGUERepository().ReadIntituleNiveau(lang.Name, Convert.ToInt16(category.LevelDepth - 1)));

        //                    if (correspondances.Count > 0)
        //                    {
        //                        PsCategory parentPrestashop = null;

        //                        //Récupération du parent dans PS
        //                        //Le catalogue 1 correspond à au noeud racine "Accueil" dans PS (il n'est ni utilisable, ni supprimable, ni exportable)
        //                        if (category.IDParent != 0 && category.IDParent != 1)
        //                            parentPrestashop = categories.ReadId(Convert.ToInt32(category.IDParent));

        //                        Catalog nouveauCatalogue = null;

        //                        if (correspondances.Count == 1)
        //                            nouveauCatalogue = GetCatalogue(category, lang, correspondances[0].cbMarq);
        //                        else if (correspondances.Count > 1)
        //                            foreach (var correspondanceSage in correspondances)
        //                                if (correspondanceSage.CL_NoParent.HasValue)
        //                                {
        //                                    //Récupération de la référence au parent de la correspondance en cours dans PC
        //                                    Catalog parentCorrespondance = catalogs.ReadSag_Id(correspondanceSage.CL_NoParent.Value);

        //                                    //Comparaison de l'ID de la réf PS du parent de la correspondance avec l'id du parent PS.
        //                                    if (parentCorrespondance != null)
        //                                        if (parentCorrespondance.Pre_Id == parentPrestashop.IDCategory)
        //                                        {
        //                                            nouveauCatalogue = GetCatalogue(category, lang, correspondanceSage.cbMarq);
        //                                            break;
        //                                        }
        //                                }

        //                        if (nouveauCatalogue != null && parentPrestashop != null)
        //                        {
        //                            //Si l'identifiant PS du parent existe dans PC
        //                            if (catalogs.ExistPre_Id(Convert.ToInt32(parentPrestashop.IDCategory)))
        //                                nouveauCatalogue.Cat_Parent = catalogs.ReadPre_Id(Convert.ToInt32(parentPrestashop.IDCategory)).Sag_Id;
        //                        }

        //                        catalogs.Add(nouveauCatalogue);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Core.Error.SendMailError(ex.ToString());
        //    }
        //}


        public void Exec(Int32 CategorySend)
        {
            try
            {
                if (CategorySend != Core.Global.CurrentShop.IDCategory)
                {
                    Model.Local.CatalogRepository CatalogRepository = new Model.Local.CatalogRepository();

                    //Vérifie si le catalogue PS est référence dans PC
                    if (!CatalogRepository.ExistPre_Id(CategorySend))
                    {
                        Model.Prestashop.PsCategoryLangRepository PsCategoryLangRepository = new Model.Prestashop.PsCategoryLangRepository();

                        //Vérifie que le catalogue existe bien dans la langue définit dans PC
                        if (PsCategoryLangRepository.ExistCategoryLang(CategorySend, Global.Lang, Core.Global.CurrentShop.IDShop))
                        {
                            Model.Prestashop.PsCategoryRepository PsCategoryRepository = new Model.Prestashop.PsCategoryRepository();

                            //Récupération du catalogue dans PS
                            Model.Prestashop.PsCategory PsCategory = PsCategoryRepository.ReadId((UInt32)CategorySend);
                            Model.Prestashop.PsCategoryLang PsCategoryLang = PsCategoryLangRepository.ReadCategoryLang(CategorySend, Global.Lang, Core.Global.CurrentShop.IDShop);

                            Model.Prestashop.PsCategory parentPrestashop = null;

                            //Récupération du parent dans PS
                            //Le catalogue 1 correspond à au noeud racine "Accueil" dans PS (il n'est ni utilisable, ni supprimable, ni exportable)
                            if (PsCategory.IDParent != 0 && PsCategory.IDParent != 1 && PsCategory.IDParent != Core.Global.CurrentShop.IDShop)
                                parentPrestashop = PsCategoryRepository.ReadId(PsCategory.IDParent);

                            Model.Local.Catalog nouveauCatalogue = GetCatalogue(PsCategory, PsCategoryLang, 0);

                            if (nouveauCatalogue != null && parentPrestashop != null)
                            {
                                //Si l'identifiant PS du parent existe dans PC
                                if (CatalogRepository.ExistPre_Id(Convert.ToInt32(parentPrestashop.IDCategory)))
                                    nouveauCatalogue.Cat_Parent = CatalogRepository.ReadPre_Id(Convert.ToInt32(parentPrestashop.IDCategory)).Cat_Id;
                            }

                            CatalogRepository.Add(nouveauCatalogue);
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
