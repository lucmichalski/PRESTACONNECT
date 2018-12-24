using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Core.Import
{
    public class ImportCatalogueInfoLibre
    {
        public void Exec(Int32 ArticleSend)
        {
            try
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                Model.Local.Article Article = ArticleRepository.ReadArticle(ArticleSend);

                Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
                Model.Sage.F_ARTICLE F_ARTICLE = new Model.Sage.F_ARTICLE();
                F_ARTICLE = F_ARTICLERepository.ReadArticle(Article.Sag_Id);

                Core.ImportSage.ImportArticle ImportArticle = new ImportSage.ImportArticle();
                ImportArticle.ImportCatalogueInfoLibre(F_ARTICLE, Article);

                //Model.Local.CatalogRepository CatalogRepository = new Model.Local.CatalogRepository();

                //Model.Local.InformationLibreArticleRepository InformationLibreArticleRepository = new Model.Local.InformationLibreArticleRepository();
                //Model.Local.InformationLibreArticle InformationLibreArticleCatalogueParent = new Model.Local.InformationLibreArticle();

                //Model.Local.Catalog Parent;

                //foreach (Model.Local.InformationLibreArticle InformationLibreArticle in InformationLibreArticleRepository.List())
                //{
                //    CatalogRepository = new Model.Local.CatalogRepository();

                //    if (InformationLibreArticle.Inf_Catalogue == 2)
                //    {
                //        if (CatalogRepository.ExistParent(InformationLibreArticle.Inf_Parent, InformationLibreArticle.Inf_Catalogue))
                //        {
                //            CreateCatalogueInfoLibre(InformationLibreArticle, F_ARTICLE, Article, InformationLibreArticle.Inf_Catalogue + 1, CatalogRepository.ReadParent(InformationLibreArticle.Inf_Parent, InformationLibreArticle.Inf_Catalogue).Cat_Id);
                //        }
                //    }
                //    else
                //    {
                //        if (InformationLibreArticleRepository.ExistInfoLibreLevel(InformationLibreArticle.Inf_Parent, 2))
                //        {
                //            InformationLibreArticleCatalogueParent = InformationLibreArticleRepository.ReadInfoLibre(InformationLibreArticle.Inf_Parent, (int)InformationLibreArticle.Inf_Catalogue - 1);
                //            if (CatalogRepository.ExistName(F_ARTICLERepository.ReadArticleInformationLibreText(InformationLibreArticleCatalogueParent.Sag_InfoLibreArticle, F_ARTICLE.AR_Ref)))
                //            {
                //                Parent = CatalogRepository.ReadParent(F_ARTICLERepository.ReadArticleInformationLibreText(InformationLibreArticleCatalogueParent.Sag_InfoLibreArticle, F_ARTICLE.AR_Ref), 2);

                //                if (Parent != null)
                //                {
                //                    CreateCatalogueInfoLibre(InformationLibreArticle, F_ARTICLE, Article, 3, Parent.Cat_Id);
                //                }
                //            }
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        //private void CreateCatalogueInfoLibre(Model.Local.InformationLibreArticle InformationLibreArticle, Model.Sage.F_ARTICLE F_ARTICLE, Model.Local.Article Article, Int32 Level, Int32 Parent)
        //{
        //    try
        //    {
        //        Model.Local.CatalogRepository CatalogRepository = new Model.Local.CatalogRepository();
        //        Model.Local.Catalog Catalog = new Model.Local.Catalog();

        //        Model.Sage.cbSysLibreRepository cbSysLibreRepository = new Model.Sage.cbSysLibreRepository();
        //        Model.Sage.cbSysLibre cbSysLibre = new Model.Sage.cbSysLibre();

        //        Model.Sage.P_INTSTATARTRepository P_INTSTATARTRepository = new Model.Sage.P_INTSTATARTRepository();
        //        Model.Sage.P_INTSTATART P_INTSTATART = new Model.Sage.P_INTSTATART();

        //        Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();

        //        if ((cbSysLibreRepository.ExistInformationLibre(InformationLibreArticle.Sag_InfoLibreArticle, Model.Sage.cbSysLibreRepository.CB_File.F_ARTICLE) 
        //                && InformationLibreArticle.Inf_IsStat == false) 
        //            || (P_INTSTATARTRepository.ExistStatArt(InformationLibreArticle.Sag_InfoLibreArticle) 
        //                && InformationLibreArticle.Inf_IsStat == true))
        //        {
        //            if ((InformationLibreArticle.Inf_IsStat == false 
        //                && !String.IsNullOrEmpty(F_ARTICLERepository.ReadArticleInformationLibreText(InformationLibreArticle.Sag_InfoLibreArticle, F_ARTICLE.AR_Ref)))
        //                || InformationLibreArticle.Inf_IsStat == true)
        //            {
        //                Catalog = new Model.Local.Catalog();
        //                if (InformationLibreArticle.Inf_IsStat == true)
        //                {
        //                    int caseSwitch = (int)InformationLibreArticle.Inf_Stat;
        //                    switch (caseSwitch)
        //                    {
        //                        case 1:
        //                            if (F_ARTICLE.AR_Stat01 != null)
        //                            {
        //                                Catalog.Cat_Name = F_ARTICLE.AR_Stat01.ToString();
        //                            }
        //                            break;
        //                        case 2:
        //                            if (F_ARTICLE.AR_Stat02 != null)
        //                            {
        //                                Catalog.Cat_Name = F_ARTICLE.AR_Stat02.ToString();
        //                            }
        //                            break;
        //                        case 3:
        //                            if (F_ARTICLE.AR_Stat03 != null)
        //                            {
        //                                Catalog.Cat_Name = F_ARTICLE.AR_Stat03.ToString();
        //                            }
        //                            break;
        //                        case 4:
        //                            if (F_ARTICLE.AR_Stat04 != null)
        //                            {
        //                                Catalog.Cat_Name = F_ARTICLE.AR_Stat04.ToString();
        //                            }
        //                            break;
        //                        case 5:
        //                            if (F_ARTICLE.AR_Stat05 != null)
        //                            {
        //                                Catalog.Cat_Name = F_ARTICLE.AR_Stat05.ToString();
        //                            }
        //                            break;
        //                    }
        //                }
        //                else
        //                {
        //                    Catalog.Cat_Name = F_ARTICLERepository.ReadArticleInformationLibreText(InformationLibreArticle.Sag_InfoLibreArticle, F_ARTICLE.AR_Ref);
        //                }
        //                Catalog.Cat_Description = Catalog.Cat_Name;
        //                Catalog.Cat_Level = Level;
        //                Catalog.Cat_Parent = Parent;
        //                Catalog.Cat_MetaTitle = Catalog.Cat_Name;
        //                Catalog.Cat_MetaDescription = Catalog.Cat_Name;
        //                Catalog.Cat_MetaKeyword = Catalog.Cat_Name;
        //                Catalog.Pre_Id = null;
        //                //Catalog.Sag_Id = null;

        //                String Value = Catalog.Cat_MetaTitle;
        //                Value = Core.Global.RemoveDiacritics(Value);
        //                Value = Core.Global.ReadLinkRewrite(Value);
        //                if (Value.Length > 128) { Catalog.Cat_LinkRewrite = Value.ToLower().Substring(0, 128); } else { Catalog.Cat_LinkRewrite = Value.ToLower(); };

        //                Catalog.Cat_LinkRewrite = Value;

        //                Catalog.Cat_Active = true;
        //                Catalog.Cat_Sync = true;
        //                Catalog.Cat_Date = DateTime.Now;

        //                if (!String.IsNullOrEmpty(Catalog.Cat_Name))
        //                {
        //                    if (CatalogRepository.ExistNameLevel(Catalog.Cat_Name, InformationLibreArticle.Inf_Catalogue) == false)
        //                    {
        //                        CatalogRepository.Add(Catalog);
        //                    }
        //                    else
        //                    {
        //                        Catalog = CatalogRepository.ReadParent(Catalog.Cat_Name, InformationLibreArticle.Inf_Catalogue);
        //                    }

        //                    Core.ImportSage.ImportArticle ImportArticle = new ImportSage.ImportArticle();
        //                    ImportArticle.AssignCatalog(Catalog.Cat_Id, Catalog.Cat_Id, Article, false);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Core.Error.SendMailError(ex.ToString());
        //    }
        //}
    }
}
