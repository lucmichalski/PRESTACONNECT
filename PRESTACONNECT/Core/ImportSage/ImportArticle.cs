using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Core.ImportSage
{
    public class ImportArticle
    {
        List<String> log;

        public void Exec(Int32 F_ARTICLESend, Int32 CatId, out List<String> log_out)
        {
            log = new List<string>();
            try
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                if (ArticleRepository.ExistSag_Id(F_ARTICLESend) == false)
                {
                    Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
                    Model.Sage.F_ARTICLE F_ARTICLE = F_ARTICLERepository.ReadArticle(F_ARTICLESend);

                    string dft = Core.Global.RemovePurge(F_ARTICLE.AR_Design, 255);
                    Model.Local.Article Article = new Model.Local.Article()
                    {
                        Art_Name = dft,
                        Art_Description = F_ARTICLE.AR_Design,
                        Art_Description_Short = F_ARTICLE.AR_Design,
                        Art_MetaTitle = dft,
                        Art_MetaDescription = dft,
                        Art_MetaKeyword = Core.Global.RemovePurgeMeta(dft, 255),
                        Art_LinkRewrite = Core.Global.ReadLinkRewrite(dft),
                        Art_Ref = F_ARTICLE.AR_Ref,
                        Art_Active = Core.Global.GetConfig().ImportArticleStatutActif,
                        Art_Date = DateTime.Now,
                        Art_Solde = false,
                        Art_Sync = true,
                        Art_SyncPrice = true,
                        Sag_Id = F_ARTICLE.cbMarq,
                        Cat_Id = CatId,
                        Art_RedirectType = new Model.Internal.RedirectType(Core.Parametres.RedirectType.NoRedirect404).Page,
                        Art_RedirectProduct = 0,
                        Art_Pack = (F_ARTICLE.AR_Nomencl == (short)ABSTRACTION_SAGE.F_ARTICLE.Obj._Enum_AR_Nomencl.Commerciale_Compose
                                    && (F_ARTICLE.AR_Gamme1 == null || F_ARTICLE.AR_Gamme1 == 0)
                                    && (F_ARTICLE.AR_Condition == null || F_ARTICLE.AR_Condition == 0)),
                        Art_Ean13 = Core.Global.RemovePurgeEAN(F_ARTICLE.AR_CodeBarre),
                    };

                    // <JG> 08/06/2013 evolution système d'import depuis Sage
                    if (CatId == 0)
                        Article.Cat_Id = ReadCatalog(F_ARTICLE);

                    if (Article.Cat_Id != 0)
                    {
                        ArticleRepository.Add(Article);

                        log.Add("IA10- Import de l'article Sage [ " + F_ARTICLE.AR_Ref + " - " + F_ARTICLE.AR_Design + " ]");

                        try
                        {
                            this.AssignCatalog(CatId, Article.Cat_Id, Article, Core.Global.GetConfig().ImportArticleRattachementParents);
                        }
                        catch (Exception ex)
                        {
                            log.Add("IA02- Erreur d'assignation des catalogues pour l'article : [ " + F_ARTICLE.AR_Ref + " - " + F_ARTICLE.AR_Design + " ]" + ex.ToString());
                        }

                        if (F_ARTICLE.AR_Gamme1 != null && F_ARTICLE.AR_Gamme1 != 0)
                        {
                            this.ExecAttribute(F_ARTICLE, Article);
                        }

                        if (Core.Global.GetConfig().ArticleImportConditionnementActif && F_ARTICLE.AR_Condition != null && F_ARTICLE.AR_Condition != 0)
                        {
                            this.ExecConditioning(F_ARTICLE, Article);
                        }

                        ImportStatInfoLibreArticle importfeature = new ImportStatInfoLibreArticle();
                        importfeature.Exec(Article.Art_Id);

                        ImportCatalogueInfoLibre(F_ARTICLE, Article);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("IA01- Une erreur est survenue : " + ex.ToString());
            }
            finally
            {
                log_out = log;
            }
        }

        public void ExecReimport(Int32 ArticleSend, out List<String> log_out)
        {
            log = new List<string>();
            try
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                if (ArticleRepository.ExistArticle(ArticleSend))
                {
                    Model.Local.Article Article = ArticleRepository.ReadArticle(ArticleSend);
                    int sag_id = Article.Sag_Id;
                    Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
                    if (!F_ARTICLERepository.ExistArticle(Article.Sag_Id)
                        && Article.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleComposition
                        && Article.CompositionArticle != null && Article.CompositionArticle.Count > 0)
                    {
                        sag_id = (from Table in Article.CompositionArticle
                                  orderby Table.ComArt_Default descending
                                  select Table.ComArt_F_ARTICLE_SagId).FirstOrDefault();
                    }

                    if (F_ARTICLERepository.ExistArticle(sag_id))
                    {
                        Model.Sage.F_ARTICLE F_ARTICLE = F_ARTICLERepository.ReadArticle(sag_id);

                        Boolean isUpdated = false;

                        if (Core.Global.GetConfig().ReimportUpdateMainCatalog)
                        {
                            int Cat_Id = ReadCatalog(F_ARTICLE);

                            if (Cat_Id != 0 && Article.Cat_Id != Cat_Id)
                            {
                                int old_cat = Article.Cat_Id;

                                Article.Cat_Id = Cat_Id;
                                isUpdated = true;

                                Model.Local.ArticleCatalogRepository ArticleCatalogRepository = new Model.Local.ArticleCatalogRepository();
                                if (Core.Global.GetConfig().ReimportDeleteLinkOldMain)
                                {
                                    if (ArticleCatalogRepository.ExistArticleCatalog(Article.Art_Id, old_cat))
                                    {
                                        ArticleCatalogRepository.Delete(ArticleCatalogRepository.ReadArticleCatalog(Article.Art_Id, old_cat));
                                    }
                                }
                                if (Core.Global.GetConfig().ReimportDeleteLinkOldSecondary)
                                {
                                    foreach (Model.Local.ArticleCatalog ArticleCatalog in ArticleCatalogRepository.ListArticle(Article.Art_Id))
                                    {
                                        if (ArticleCatalog.Cat_Id != old_cat)
                                            ArticleCatalogRepository.Delete(ArticleCatalog);
                                    }
                                }

                                try
                                {
                                    this.AssignCatalog(0, Article.Cat_Id, Article, Core.Global.GetConfig().ReimportLinkParents);
                                }
                                catch (Exception ex)
                                {
                                    log.Add("RA02- Erreur d'assignation des catalogues pour l'article : [ " + F_ARTICLE.AR_Ref + " - " + F_ARTICLE.AR_Design + " ]" + ex.ToString());
                                }
                            }
                        }

                        string dft = Core.Global.RemovePurge(F_ARTICLE.AR_Design, 255);
                        if (Core.Global.GetConfig().ReimportUpdateProductName && Article.Art_Name != dft)
                        {
                            Article.Art_Name = dft;
                            isUpdated = true;
                        }
                        if (Core.Global.GetConfig().ReimportUpdateDescriptionShort && Article.Art_Description_Short != F_ARTICLE.AR_Design)
                        {
                            Article.Art_Description_Short = F_ARTICLE.AR_Design;
                            isUpdated = true;
                        }
                        if (Core.Global.GetConfig().ReimportUpdateDescription && Article.Art_Description != F_ARTICLE.AR_Design)
                        {
                            Article.Art_Description = F_ARTICLE.AR_Design;
                            isUpdated = true;
                        }
                        if (Core.Global.GetConfig().ReimportUpdateMetaTitle && Article.Art_MetaTitle != dft)
                        {
                            Article.Art_MetaTitle = dft;
                            isUpdated = true;
                        }
                        if (Core.Global.GetConfig().ReimportUpdateMetaDescription && Article.Art_MetaDescription != dft)
                        {
                            Article.Art_MetaDescription = dft;
                            isUpdated = true;
                        }
                        if (Core.Global.GetConfig().ReimportUpdateMetaKeywords && Article.Art_MetaKeyword != Core.Global.RemovePurgeMeta(dft, 255))
                        {
                            Article.Art_MetaKeyword = Core.Global.RemovePurgeMeta(dft, 255);
                            isUpdated = true;
                        }
                        if (Core.Global.GetConfig().ReimportUpdateURL)
                        {
                            String rewriting = Core.Global.ReadLinkRewrite(dft);
                            if (Article.Art_LinkRewrite != rewriting)
                            {
                                Article.Art_LinkRewrite = rewriting;
                                isUpdated = true;
                            }
                        }
                        if (Core.Global.GetConfig().ReimportUpdateActive)
                        {
                            Boolean Sommeil = Core.Global.GetConfig().ArticleEnSommeil;
                            Boolean NonPublie = Core.Global.GetConfig().ArticleNonPublieSurLeWeb;
                            // si on affiche pas les sommeil et que article en sommeil = True
                            // si on affiche pas les non publie et que article non publie = True
                            // si affiche sommeil ou non publie = False
                            Boolean mettreinactif = ((!Sommeil && F_ARTICLE.AR_Sommeil == 1)
                                            || (!NonPublie && F_ARTICLE.AR_Publie == 0)
                                            || Core.Tools.FiltreImportSage.ImportSageFilterExclude(F_ARTICLE));
                            bool active = !mettreinactif;
                            if (Article.Art_Active != active)
                            {
                                Article.Art_Active = active;
                                isUpdated = true;
                            }
                        }

                        if (Article.TypeArticle != Model.Local.Article.enum_TypeArticle.ArticleComposition)
                        {
                            if (Core.Global.GetConfig().ReimportUpdateEAN)
                            {
                                string ean = Core.Global.RemovePurgeEAN(F_ARTICLE.AR_CodeBarre);
                                if (Article.Art_Ean13 != ean)
                                {
                                    Article.Art_Ean13 = ean;
                                    isUpdated = true;
                                }
                            }
                            if (Core.Global.GetConfig().ReimportUpdateAttribute)
                            {
                                if (F_ARTICLE.AR_Gamme1 != null && F_ARTICLE.AR_Gamme1 != 0)
                                {
                                    if (this.ExecAttribute(F_ARTICLE, Article))
                                        isUpdated = true;
                                }
                            }
                            if (Core.Global.GetConfig().ReimportUpdateConditioning)
                            {
                                if (Core.Global.GetConfig().ArticleImportConditionnementActif && F_ARTICLE.AR_Condition != null && F_ARTICLE.AR_Condition != 0)
                                {
                                    if (this.ExecConditioning(F_ARTICLE, Article))
                                        isUpdated = true;
                                }
                            }
                        }

                        if (isUpdated)
                        {
                            if (Core.Global.GetConfig().ReimportUpdateDateActive)
                                Article.Art_Date = DateTime.Now;

                            ArticleRepository.Save();
                        }

                        if (Core.Global.GetConfig().ReimportUpdateCharacteristic)
                        {
                            Core.Temp.UpdateDateActive = Core.Global.GetConfig().ReimportUpdateDateActive;
                            ImportStatInfoLibreArticle importfeature = new ImportStatInfoLibreArticle();
                            importfeature.Exec(Article.Art_Id);
                        }
                    }
                    else
                    {
                        Article.Art_Active = false;
                        Article.Art_Sync = false;
                        ArticleRepository.Save();
                        log.Add("RA10- Article Sage introuvable désactivation complète de l'article [ " + Article.Art_Ref + " - " + Article.Art_Name + " ]");
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("RA01- Une erreur est survenue : " + ex.ToString());
            }
            finally
            {
                log_out = log;
            }
        }

        private Int32 ReadCatalog(Int32 Numero)
        {
            Int32 Return = 0;
            try
            {
                Model.Sage.F_CATALOGUERepository F_CATALOGUERepository = new Model.Sage.F_CATALOGUERepository();
                if (F_CATALOGUERepository.ExistNumero(Numero))
                {
                    Model.Sage.F_CATALOGUE F_CATALOGUE = F_CATALOGUERepository.ReadNumero(Numero);
                    Model.Local.CatalogRepository CatalogRepository = new Model.Local.CatalogRepository();
                    if (CatalogRepository.ExistSag_Id(F_CATALOGUE.cbMarq))
                    {
                        Model.Local.Catalog Catalog = CatalogRepository.ReadSag_Id(F_CATALOGUE.cbMarq);
                        Return = Catalog.Cat_Id;
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
            return Return;
        }

        public Int32 ReadCatalog(Model.Sage.F_ARTICLE F_ARTICLE)
        {
            Int32 Return = 0;
            try
            {
                Int32 CL_No = 0;
                if (F_ARTICLE.CL_No4 != null && F_ARTICLE.CL_No4 != 0)
                    CL_No = (int)F_ARTICLE.CL_No4;
                if (CL_No == 0 && F_ARTICLE.CL_No3 != null && F_ARTICLE.CL_No3 != 0)
                    CL_No = (int)F_ARTICLE.CL_No3;
                if (CL_No == 0 && F_ARTICLE.CL_No2 != null && F_ARTICLE.CL_No2 != 0)
                    CL_No = (int)F_ARTICLE.CL_No2;
                if (CL_No == 0 && F_ARTICLE.CL_No1 != null && F_ARTICLE.CL_No1 != 0)
                    CL_No = (int)F_ARTICLE.CL_No1;


                Model.Sage.F_CATALOGUERepository F_CATALOGUERepository = new Model.Sage.F_CATALOGUERepository();
                if (CL_No != 0 && F_CATALOGUERepository.ExistNumero(CL_No))
                {
                    Model.Sage.F_CATALOGUE F_CATALOGUE = F_CATALOGUERepository.ReadNumero(CL_No);
                    Model.Local.CatalogRepository CatalogRepository = new Model.Local.CatalogRepository();
                    if (CatalogRepository.ExistSag_Id(F_CATALOGUE.cbMarq))
                    {
                        Model.Local.Catalog Catalog = CatalogRepository.ReadSag_Id(F_CATALOGUE.cbMarq);
                        Return = Catalog.Cat_Id;
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
            return Return;
        }

        public Boolean ExecAttribute(Model.Sage.F_ARTICLE F_ARTICLE, Model.Local.Article Article, Boolean OnlyAttributeStatut = false)
        {
            Boolean isUpdated = false;
            try
            {
                if (F_ARTICLE.AR_Gamme1 != null && F_ARTICLE.AR_Gamme1 != 0)
                    CreateAttributeGroup(F_ARTICLE.AR_Gamme1.Value);
                if (F_ARTICLE.AR_Gamme2 != null && F_ARTICLE.AR_Gamme2 != 0)
                    CreateAttributeGroup(F_ARTICLE.AR_Gamme2.Value);

                Model.Sage.F_ARTENUMREFRepository F_ARTENUMREFRepository = new Model.Sage.F_ARTENUMREFRepository();
                Model.Sage.F_ARTGAMMERepository F_ARTGAMMERepository = new Model.Sage.F_ARTGAMMERepository();

                Model.Local.AttributeArticleRepository AttributeArticleRepository = new Model.Local.AttributeArticleRepository();
                Boolean IsDefault = !AttributeArticleRepository.ExistArticleDefault(Article.Art_Id, true);

                List<Model.Local.AttributeArticle> listLocal = AttributeArticleRepository.ListArticle(Article.Art_Id);
                List<Int32> listLocalSag_Id = listLocal.Select(a => a.Sag_Id).ToList();

                List<Model.Sage.F_ARTENUMREF> list_F_ARTENUMREF = F_ARTENUMREFRepository.ListReference(F_ARTICLE.AR_Ref);
                List<Int32> listcbMarq = list_F_ARTENUMREF.Select(s => s.cbMarq).ToList();

                Boolean ExistAECAttributeStatut = Core.Global.ExistAECAttributeStatut();

                // 09/12/2016 si le module de désactivation des gammes est installé
                if (ExistAECAttributeStatut)
                {
                    // Identifie les déclinaisons locales qui ne sont plus dans Sage car supprimées
                    List<Model.Local.AttributeArticle> list_disable = listLocal.Where(attart => !listcbMarq.Contains(attart.Sag_Id)).ToList();
                    //// Identifie les déclinaisons locales qui sont en sommeil dans Sage ou ne sont plus "à importer/activer"
                    //if (list_disable == null)
                    //    list_disable = new List<Model.Local.AttributeArticle>();
                    //list_disable.AddRange(listLocal.Where(attart => list_F_ARTENUMREF_toimport.Count(g => g.cbMarq == attart.Sag_Id) == 0));

                    // désactivation déclinaisons qui n'existe plus dans Sage
                    foreach (Model.Local.AttributeArticle t in list_disable)
                    {
                        t.AttArt_Active = false;
                        //t.AttArt_Sync = false;
                    }
                    AttributeArticleRepository.Save();

                    if (OnlyAttributeStatut)
                    {
                        // filtrage de F_ARTENUMREF pour obtenir ceux déjà importés uniquement
                        list_F_ARTENUMREF = list_F_ARTENUMREF.Where(g => listLocalSag_Id.Contains(g.cbMarq)).ToList();
                    }
                }
                else
                {
                    // filtrage de F_ARTENUMREF pour obtenir ceux non en sommeil et non déjà importés
					#if (SAGE_VERSION_16 || SAGE_VERSION_17)
					list_F_ARTENUMREF = list_F_ARTENUMREF.Where(g => !listLocalSag_Id.Contains(g.cbMarq)).ToList();
					#else
					list_F_ARTENUMREF = list_F_ARTENUMREF.Where(g => g.AE_Sommeil != null && g.AE_Sommeil == 0
                        && !listLocalSag_Id.Contains(g.cbMarq)).ToList();
					#endif
                    // application des filtres d'import sur les références Gamme
                    list_F_ARTENUMREF = Core.Tools.FiltreImportSage.ImportSageFilter(list_F_ARTENUMREF);
                }


                bool importsagefilterexclude = false;
                foreach (Model.Sage.F_ARTENUMREF F_ARTENUMREF in list_F_ARTENUMREF)
                {
                    importsagefilterexclude = false;
                    Model.Sage.F_ARTGAMME F_ARTGAMME1 = null;
                    Model.Sage.F_ARTGAMME F_ARTGAMME2 = null;
                    if (F_ARTENUMREF.AG_No1 != null && F_ARTENUMREF.AG_No1 != 0 && F_ARTGAMMERepository.Exist((int)F_ARTENUMREF.AG_No1, ABSTRACTION_SAGE.F_ARTGAMME.Obj._Enum_AG_Type.Gamme_1))
                    {
                        F_ARTGAMME1 = F_ARTGAMMERepository.Read((int)F_ARTENUMREF.AG_No1, ABSTRACTION_SAGE.F_ARTGAMME.Obj._Enum_AG_Type.Gamme_1);
                        importsagefilterexclude = Core.Tools.FiltreImportSage.ImportSageFilterExclude(F_ARTGAMME1);
                    }
                    if (!importsagefilterexclude)
                    {
                        if (F_ARTENUMREF.AG_No2 != null && F_ARTENUMREF.AG_No2 != 0 && F_ARTGAMMERepository.Exist((int)F_ARTENUMREF.AG_No2, ABSTRACTION_SAGE.F_ARTGAMME.Obj._Enum_AG_Type.Gamme_2))
                        {
                            F_ARTGAMME2 = F_ARTGAMMERepository.Read((int)F_ARTENUMREF.AG_No2, ABSTRACTION_SAGE.F_ARTGAMME.Obj._Enum_AG_Type.Gamme_2);
                            importsagefilterexclude = Core.Tools.FiltreImportSage.ImportSageFilterExclude(F_ARTGAMME2);
                        }
                    }

                    // si la déclinaison n'existe pas
                    if (!OnlyAttributeStatut && !listLocalSag_Id.Contains(F_ARTENUMREF.cbMarq))
                    {
                        // et qu'elle n'est pas exclue par les filtres
                        if (!importsagefilterexclude)
                        {
                            if (F_ARTGAMME1 != null)
                            {
                                UInt32 FirstAttribute = CreateAttribute(F_ARTGAMME1);
                                UInt32 SecondAttribute = 0;
                                if (F_ARTGAMME2 != null)
                                {
                                    SecondAttribute = CreateAttribute(F_ARTGAMME2);
                                }

                                AttributeArticleRepository.Add(new Model.Local.AttributeArticle()
                                {
                                    Art_Id = Article.Art_Id,
                                    Att_IdFirst = (int)FirstAttribute,
                                    Att_IdSecond = ((SecondAttribute != 0) ? (int?)SecondAttribute : null),
                                    AttArt_Default = IsDefault,
                                    AttArt_Sync = true,
                                    AttArt_Active = true,
                                    Sag_Id = F_ARTENUMREF.cbMarq,
                                });

                                if (IsDefault)
                                    IsDefault = false;

                                isUpdated = true;
                                if (log != null)
                                    log.Add("IA11- Import déclinaison gamme depuis Sage [ " + F_ARTGAMME1.EG_Enumere + " ]" + ((F_ARTGAMME2 != null) ? " / [ " + F_ARTGAMME2.EG_Enumere + " ]" : string.Empty));

                            }
                        }
                    }
                    // si la déclinaison existe
                    else if (listLocalSag_Id.Contains(F_ARTENUMREF.cbMarq))
                    {
						#if (SAGE_VERSION_16 || SAGE_VERSION_17)
						bool isenable = !importsagefilterexclude
                            && !Core.Tools.FiltreImportSage.ImportSageFilterExclude(F_ARTENUMREF);
						#else
						bool isenable = !importsagefilterexclude
                            && F_ARTENUMREF.AE_Sommeil != null && F_ARTENUMREF.AE_Sommeil == 0
                            && !Core.Tools.FiltreImportSage.ImportSageFilterExclude(F_ARTENUMREF);
						#endif

                        // on l'active ou la désactive en fonction de l'état sommeil et des filtres d'exclusion
                        Model.Local.AttributeArticle AttributeArticle = listLocal.FirstOrDefault(a => a.Sag_Id == F_ARTENUMREF.cbMarq);
                        if (AttributeArticle != null
                            && AttributeArticle.AttArt_Id != 0
                            && AttributeArticle.AttArt_Active != isenable)
                        {
                            AttributeArticle.AttArt_Active = isenable;
                            AttributeArticleRepository.Save();
                            if (log != null)
                                log.Add((OnlyAttributeStatut ? "SA51- " : "IA51- ")
                                    + (isenable ? "Activation" : "Désactivation")
                                    + " déclinaison gamme depuis Sage [ " + F_ARTGAMME1.EG_Enumere + " ]"
                                    + ((F_ARTGAMME2 != null) ? " / [ " + F_ARTGAMME2.EG_Enumere + " ]" : string.Empty));
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
            return isUpdated;
        }

        public UInt32 CreateAttributeGroup(Int32 AR_Gamme)
        {
            UInt32 r = 0;
            try
            {
                Model.Local.AttributeGroupRepository AttributeGroupRepository = new Model.Local.AttributeGroupRepository();
                if (AttributeGroupRepository.ExistSage(AR_Gamme))
                {
                    r = (uint)AttributeGroupRepository.ReadSage(AR_Gamme).Pre_Id;
                }
                else if (AttributeGroupRepository.ExistSage(AR_Gamme) == false)
                {
                    Model.Sage.P_GAMMERepository P_GAMMERepository = new Model.Sage.P_GAMMERepository();
                    Model.Sage.P_GAMME P_GAMME = P_GAMMERepository.ReadGamme(AR_Gamme);
                    if (!string.IsNullOrWhiteSpace(P_GAMME.G_Intitule))
                    {
                        Model.Prestashop.PsAttributeGroupRepository PsAttributeGroupRepository = new Model.Prestashop.PsAttributeGroupRepository();
                        Model.Prestashop.PsAttributeGroupLangRepository PsAttributeGroupLangRepository = new Model.Prestashop.PsAttributeGroupLangRepository();

                        Model.Prestashop.PsAttributeGroup PsAttributeGroup;
                        Model.Prestashop.PsAttributeGroupLang PsAttributeGroupLang;

                        if (PsAttributeGroupLangRepository.ExistNameLang(P_GAMME.G_Intitule, Core.Global.Lang))
                        {
                            PsAttributeGroupLang = PsAttributeGroupLangRepository.ReadNameLang(P_GAMME.G_Intitule, Core.Global.Lang);
                            PsAttributeGroup = PsAttributeGroupRepository.ReadAttributeGroup(PsAttributeGroupLang.IDAttributeGroup);
                            r = PsAttributeGroup.IDAttributeGroup;

                            AttributeGroupRepository.Add(new Model.Local.AttributeGroup()
                            {
                                Sag_Id = P_GAMME.cbMarq,
                                Pre_Id = (int)PsAttributeGroup.IDAttributeGroup,
                            });
                        }
                        else
                        {
                            PsAttributeGroup = new Model.Prestashop.PsAttributeGroup()
                            {
                        		#if (PRESTASHOP_VERSION_172)
								GroupType = "select",
								#endif
                                IsColorGroup = 0,
                                Position = PsAttributeGroupRepository.NextPosition(),
                            };
                            PsAttributeGroupRepository.Add(PsAttributeGroup, Core.Global.CurrentShop.IDShop);
                            r = PsAttributeGroup.IDAttributeGroup;

                            foreach (Model.Prestashop.PsLang PsLang in new Model.Prestashop.PsLangRepository().ListActive(1, Global.CurrentShop.IDShop))
                                if (!PsAttributeGroupLangRepository.ExistAttributeGroupLang(PsAttributeGroup.IDAttributeGroup, PsLang.IDLang))
                                {
                                    PsAttributeGroupLang = new Model.Prestashop.PsAttributeGroupLang();
                                    PsAttributeGroupLang.IDAttributeGroup = PsAttributeGroup.IDAttributeGroup;
                                    PsAttributeGroupLang.IDLang = PsLang.IDLang;
                                    PsAttributeGroupLang.Name = P_GAMME.G_Intitule;
                                    PsAttributeGroupLang.PublicName = P_GAMME.G_Intitule;
                                    PsAttributeGroupLangRepository.Add(PsAttributeGroupLang);
                                }

                            AttributeGroupRepository.Add(new Model.Local.AttributeGroup()
                            {
                                Sag_Id = P_GAMME.cbMarq,
                                Pre_Id = (int)PsAttributeGroup.IDAttributeGroup,
                            });

                            if (log != null)
                                log.Add("IA20- Création d'attribut pour la gamme produit Sage [ " + P_GAMME.G_Intitule + " ]");
                        }

                    }
                }
            }
            catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
            return r;
        }

        public UInt32 CreateAttribute(Model.Sage.F_ARTGAMME F_ARTGAMME)
        {
            UInt32 r = 0;
            UInt32 PsAttributeGroup = 0;

            if (F_ARTGAMME.AG_Type == (short)ABSTRACTION_SAGE.F_ARTGAMME.Obj._Enum_AG_Type.Gamme_1
                && F_ARTGAMME.F_ARTICLE != null && F_ARTGAMME.F_ARTICLE.AR_Gamme1 != null && F_ARTGAMME.F_ARTICLE.AR_Gamme1 != 0)
                PsAttributeGroup = CreateAttributeGroup(F_ARTGAMME.F_ARTICLE.AR_Gamme1.Value);
            if (F_ARTGAMME.AG_Type == (short)ABSTRACTION_SAGE.F_ARTGAMME.Obj._Enum_AG_Type.Gamme_2
                && F_ARTGAMME.F_ARTICLE != null && F_ARTGAMME.F_ARTICLE.AR_Gamme2 != null && F_ARTGAMME.F_ARTICLE.AR_Gamme2 != 0)
                PsAttributeGroup = CreateAttributeGroup(F_ARTGAMME.F_ARTICLE.AR_Gamme2.Value);

            if (PsAttributeGroup != 0)
                try
                {
                    Model.Local.AttributeRepository AttributeRepository = new Model.Local.AttributeRepository();
                    if (!AttributeRepository.ExistSag_Id(F_ARTGAMME.cbMarq))
                    {
                        Model.Prestashop.PsAttributeLangRepository PsAttributeLangRepository = new Model.Prestashop.PsAttributeLangRepository();
                        if (PsAttributeLangRepository.ExistAttributeLang(F_ARTGAMME.EG_Enumere, Core.Global.Lang, PsAttributeGroup))
                        {
                            Model.Prestashop.PsAttributeLang PsAttributeLang = PsAttributeLangRepository.ReadAttributeLang(F_ARTGAMME.EG_Enumere, Core.Global.Lang, PsAttributeGroup);
                            r = PsAttributeLang.IDAttribute;

                            new Model.Local.AttributeRepository().Add(new Model.Local.Attribute()
                            {
                                Sag_Id = F_ARTGAMME.cbMarq,
                                Pre_Id = (int)PsAttributeLang.IDAttribute,
                            });
                        }
                        else
                        {
                            Model.Prestashop.PsAttribute PsAttribute = new Model.Prestashop.PsAttribute()
                            {
                                Color = string.Empty,
                                IDAttributeGroup = (uint)PsAttributeGroup,
                                Position = new Model.Prestashop.PsAttributeRepository().NextPosition(),
                            };
                            new Model.Prestashop.PsAttributeRepository().Add(PsAttribute, Core.Global.CurrentShop.IDShop);
                            r = PsAttribute.IDAttribute;

                            foreach (Model.Prestashop.PsLang lang in new Model.Prestashop.PsLangRepository().ListActive(1, Core.Global.CurrentShop.IDShop))
                                new Model.Prestashop.PsAttributeLangRepository().Add(new Model.Prestashop.PsAttributeLang()
                                {
                                    IDLang = lang.IDLang,
                                    IDAttribute = PsAttribute.IDAttribute,
                                    Name = F_ARTGAMME.EG_Enumere
                                });

                            if (log != null)
                                log.Add("IA21- Création valeur d'attribut pour énuméré de gamme Sage [ " + F_ARTGAMME.EG_Enumere + " ]");

                            new Model.Local.AttributeRepository().Add(new Model.Local.Attribute()
                            {
                                Sag_Id = F_ARTGAMME.cbMarq,
                                Pre_Id = (int)PsAttribute.IDAttribute,
                            });
                        }
                    }
                    else
                    {
                        Model.Local.Attribute Attribute = AttributeRepository.ReadSag_Id(F_ARTGAMME.cbMarq);
                        Model.Prestashop.PsAttributeRepository PsAttributeRepository = new Model.Prestashop.PsAttributeRepository();
                        if (PsAttributeRepository.Exist((uint)Attribute.Pre_Id))
                        {
                            r = (uint)Attribute.Pre_Id;
                        }
                        else
                        {
                            AttributeRepository.Delete(Attribute);
                            CreateAttribute(F_ARTGAMME);
                        }
                    }
                }
                catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
            return r;
        }

        public UInt32 CreateConditioningGroup(Int32 AR_Condition)
        {
            UInt32 r = 0;
            try
            {
                Model.Local.ConditioningGroupRepository ConditioningGroupRepository = new Model.Local.ConditioningGroupRepository();
                if (ConditioningGroupRepository.ExistSage(AR_Condition))
                {
                    r = (uint)ConditioningGroupRepository.ReadSage(AR_Condition).Pre_Id;
                }
                else if (ConditioningGroupRepository.ExistSage(AR_Condition) == false)
                {
                    Model.Sage.P_CONDITIONNEMENTRepository P_CONDITIONNEMENTRepository = new Model.Sage.P_CONDITIONNEMENTRepository();
                    Model.Sage.P_CONDITIONNEMENT P_CONDITIONNEMENT = P_CONDITIONNEMENTRepository.ReadConditionnement(AR_Condition);
                    if (!string.IsNullOrWhiteSpace(P_CONDITIONNEMENT.P_Conditionnement))
                    {
                        Model.Prestashop.PsAttributeGroupRepository PsAttributeGroupRepository = new Model.Prestashop.PsAttributeGroupRepository();
                        Model.Prestashop.PsAttributeGroupLangRepository PsAttributeGroupLangRepository = new Model.Prestashop.PsAttributeGroupLangRepository();

                        Model.Prestashop.PsAttributeGroup PsAttributeGroup;
                        Model.Prestashop.PsAttributeGroupLang PsAttributeGroupLang;

                        if (PsAttributeGroupLangRepository.ExistNameLang(P_CONDITIONNEMENT.P_Conditionnement, Core.Global.Lang))
                        {
                            PsAttributeGroupLang = PsAttributeGroupLangRepository.ReadNameLang(P_CONDITIONNEMENT.P_Conditionnement, Core.Global.Lang);
                            PsAttributeGroup = PsAttributeGroupRepository.ReadAttributeGroup(PsAttributeGroupLang.IDAttributeGroup);
                            r = PsAttributeGroup.IDAttributeGroup;

                            ConditioningGroupRepository.Add(new Model.Local.ConditioningGroup()
                            {
                                Sag_Id = P_CONDITIONNEMENT.cbMarq,
                                Pre_Id = (int)PsAttributeGroup.IDAttributeGroup,
                            });
                        }
                        else
                        {
                            PsAttributeGroup = new Model.Prestashop.PsAttributeGroup()
                            {
                        		#if (PRESTASHOP_VERSION_172)
								GroupType = "select",
								#endif
                                IsColorGroup = 0,
                                Position = PsAttributeGroupRepository.NextPosition(),
                            };
                            PsAttributeGroupRepository.Add(PsAttributeGroup, Core.Global.CurrentShop.IDShop);
                            r = PsAttributeGroup.IDAttributeGroup;

                            foreach (Model.Prestashop.PsLang PsLang in new Model.Prestashop.PsLangRepository().ListActive(1, Global.CurrentShop.IDShop))
                                if (!PsAttributeGroupLangRepository.ExistAttributeGroupLang(PsAttributeGroup.IDAttributeGroup, PsLang.IDLang))
                                {
                                    PsAttributeGroupLang = new Model.Prestashop.PsAttributeGroupLang();
                                    PsAttributeGroupLang.IDAttributeGroup = PsAttributeGroup.IDAttributeGroup;
                                    PsAttributeGroupLang.IDLang = PsLang.IDLang;
                                    PsAttributeGroupLang.Name = P_CONDITIONNEMENT.P_Conditionnement;
                                    PsAttributeGroupLang.PublicName = P_CONDITIONNEMENT.P_Conditionnement;
                                    PsAttributeGroupLangRepository.Add(PsAttributeGroupLang);
                                }

                            ConditioningGroupRepository.Add(new Model.Local.ConditioningGroup()
                            {
                                Sag_Id = P_CONDITIONNEMENT.cbMarq,
                                Pre_Id = (int)PsAttributeGroup.IDAttributeGroup,
                            });

                            if (log != null)
                                log.Add("IA30- Création d'attribut pour le conditionnement Sage [ " + P_CONDITIONNEMENT.P_Conditionnement + " ]");
                        }

                    }
                }
            }
            catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
            return r;
        }

        public UInt32 CreateConditioning(Model.Sage.F_CONDITION F_CONDITION)
        {
            UInt32 r = 0;
            UInt32 PsAttributeGroup = 0;

            if (F_CONDITION.F_ARTICLE != null && F_CONDITION.F_ARTICLE.AR_Condition != null && F_CONDITION.F_ARTICLE.AR_Condition != 0)
                PsAttributeGroup = CreateConditioningGroup(F_CONDITION.F_ARTICLE.AR_Condition.Value);

            if (PsAttributeGroup != 0)
                try
                {
                    Model.Local.ConditioningRepository ConditioningRepository = new Model.Local.ConditioningRepository();
                    if (!ConditioningRepository.ExistSag_Id(F_CONDITION.cbMarq))
                    {
                        Model.Prestashop.PsAttributeLangRepository PsAttributeLangRepository = new Model.Prestashop.PsAttributeLangRepository();
                        if (PsAttributeLangRepository.ExistAttributeLang(F_CONDITION.EC_Enumere, Core.Global.Lang, PsAttributeGroup))
                        {
                            Model.Prestashop.PsAttributeLang PsAttributeLang = PsAttributeLangRepository.ReadAttributeLang(F_CONDITION.EC_Enumere, Core.Global.Lang, PsAttributeGroup);
                            r = PsAttributeLang.IDAttribute;

                            new Model.Local.ConditioningRepository().Add(new Model.Local.Conditioning()
                            {
                                Sag_Id = F_CONDITION.cbMarq,
                                Pre_Id = (int)PsAttributeLang.IDAttribute,
                            });
                        }
                        else
                        {
                            Model.Prestashop.PsAttribute PsAttribute = new Model.Prestashop.PsAttribute()
                            {
                                Color = string.Empty,
                                IDAttributeGroup = (uint)PsAttributeGroup,
                                Position = new Model.Prestashop.PsAttributeRepository().NextPosition(),
                            };
                            new Model.Prestashop.PsAttributeRepository().Add(PsAttribute, Core.Global.CurrentShop.IDShop);
                            r = PsAttribute.IDAttribute;

                            foreach (Model.Prestashop.PsLang lang in new Model.Prestashop.PsLangRepository().ListActive(1, Core.Global.CurrentShop.IDShop))
                                new Model.Prestashop.PsAttributeLangRepository().Add(new Model.Prestashop.PsAttributeLang()
                                {
                                    IDLang = lang.IDLang,
                                    IDAttribute = PsAttribute.IDAttribute,
                                    Name = F_CONDITION.EC_Enumere,
                                });

                            if (log != null)
                                log.Add("IA31- Création valeur d'attribut pour énuméré de conditionnement Sage [ " + F_CONDITION.EC_Enumere + " ]");

                            new Model.Local.ConditioningRepository().Add(new Model.Local.Conditioning()
                            {
                                Sag_Id = F_CONDITION.cbMarq,
                                Pre_Id = (int)PsAttribute.IDAttribute,
                            });
                        }
                    }
                    else
                    {
                        Model.Local.Conditioning Conditioning = ConditioningRepository.ReadSag_Id(F_CONDITION.cbMarq);
                        Model.Prestashop.PsAttributeRepository PsAttributeRepository = new Model.Prestashop.PsAttributeRepository();
                        if (PsAttributeRepository.Exist((uint)Conditioning.Pre_Id))
                        {
                            r = (uint)Conditioning.Pre_Id;
                        }
                        else
                        {
                            ConditioningRepository.Delete(Conditioning);
                            CreateConditioning(F_CONDITION);
                        }
                    }
                }
                catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
            return r;
        }

        public Boolean ExecConditioning(Model.Sage.F_ARTICLE F_ARTICLE, Model.Local.Article Article)
        {
            Boolean isUpdated = false;
            try
            {
                if (F_ARTICLE.AR_Condition != null && F_ARTICLE.AR_Condition != 0)
                    CreateConditioningGroup(F_ARTICLE.AR_Condition.Value);

                Model.Sage.F_CONDITIONRepository F_CONDITIONRepository = new Model.Sage.F_CONDITIONRepository();

                List<Model.Sage.F_CONDITION> ListF_CONDITION = F_CONDITIONRepository.ListArticle(F_ARTICLE.AR_Ref);

                Model.Local.ConditioningArticleRepository ConditioningArticleRepository = new Model.Local.ConditioningArticleRepository();

                // si besoin filtrage sur les conditionnements déjà importés - ne rédéfinira pas la déclinaison par défaut
                //List<Model.Local.ConditioningArticle> listLocal = ConditioningArticleRepository.ListArticle(Article.Art_Id);
                //list = list.Where(g => listLocal.Count(l => l.Sag_Id == g.cbMarq) == 0).ToList();

                ListF_CONDITION = Core.Tools.FiltreImportSage.ImportSageFilter(ListF_CONDITION);

                Int32 IDPrincipal = 0;
                if (ListF_CONDITION.Count(c => c.CO_Principal == 1) == 1)
                    IDPrincipal = (int)ListF_CONDITION.FirstOrDefault(c => c.CO_Principal == 1).CO_No;

                foreach (Model.Sage.F_CONDITION F_CONDITION in ListF_CONDITION)
                {
                    try
                    {
                        if (IDPrincipal == 0)
                            IDPrincipal = F_CONDITION.CO_No.Value;

                        uint IDConditioning = CreateConditioning(F_CONDITION);

                        if (!ConditioningArticleRepository.ExistSag_Id(F_CONDITION.cbMarq))
                        {
                            Model.Local.ConditioningArticle ConditioningArticle = new Model.Local.ConditioningArticle()
                            {
                                ConArt_Sync = true,
                                Con_Id = (int)IDConditioning,
                                Art_Id = Article.Art_Id,
                                ConArt_Default = (F_CONDITION.CO_No == IDPrincipal),
                                Sag_Id = F_CONDITION.cbMarq,
                            };
                            ConditioningArticleRepository.Add(ConditioningArticle);

                            isUpdated = true;
                            if (log != null)
                                log.Add("IA41- Import déclinaison conditionnement depuis Sage [ " + F_CONDITION.EC_Enumere + " ]");
                        }
                        else if (ConditioningArticleRepository.ExistSag_Id(F_CONDITION.cbMarq))
                        {
                            Model.Local.ConditioningArticle ConditioningArticle = ConditioningArticleRepository.ReadSag_Id(F_CONDITION.cbMarq);
                            if (ConditioningArticle.ConArt_Default != (F_CONDITION.CO_No == IDPrincipal))
                            {
                                ConditioningArticle.ConArt_Default = (F_CONDITION.CO_No == IDPrincipal);
                                ConditioningArticleRepository.Save();
                                isUpdated = true;
                            }
                        }
                    }
                    catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
            return isUpdated;
        }

        public void AssignCatalog(Int32 SelectedCatalog, Int32 IdLocal, Model.Local.Article Article, Boolean LienCataloguesParents)
        {
            try
            {
                // Si SelectedCatalog == 0 : système d'affectation auto des catalogues
                Model.Local.CatalogRepository CatalogRepository = new Model.Local.CatalogRepository();
                if (CatalogRepository.ExistId(IdLocal))
                {
                    Model.Local.Catalog Catalog = CatalogRepository.ReadId(IdLocal);
                    Model.Local.ArticleCatalogRepository ArticleCatalogRepository = new Model.Local.ArticleCatalogRepository();
                    if (SelectedCatalog == 0 && Catalog.Sag_Id != 0)
                    {
                        foreach (Model.Local.Catalog LocalCatalog in CatalogRepository.ListSageId((int)Catalog.Sag_Id))
                        {
                            if (ArticleCatalogRepository.ExistArticleCatalog(Article.Art_Id, LocalCatalog.Cat_Id) == false)
                            {
                                ArticleCatalogRepository.Add(new Model.Local.ArticleCatalog()
                                {
                                    Art_Id = Article.Art_Id,
                                    Cat_Id = LocalCatalog.Cat_Id
                                }
                                );
                            }
                            if (LienCataloguesParents)
                                AssignCatalogSecondaire(Article, LocalCatalog);
                        }
                    }
                    else
                    {
                        if (ArticleCatalogRepository.ExistArticleCatalog(Article.Art_Id, Catalog.Cat_Id) == false)
                        {
                            ArticleCatalogRepository.Add(new Model.Local.ArticleCatalog()
                            {
                                Art_Id = Article.Art_Id,
                                Cat_Id = Catalog.Cat_Id
                            }
                            );
                            if (LienCataloguesParents)
                                AssignCatalogSecondaire(Article, Catalog);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
        private void AssignCatalogSecondaire(Model.Local.Article Article, Model.Local.Catalog Catalog)
        {
            try
            {
                Model.Local.CatalogRepository CatalogRepository = new Model.Local.CatalogRepository();
                if (Catalog.Cat_Parent != 0 && CatalogRepository.ExistId(Catalog.Cat_Parent))
                {
                    Model.Local.Catalog Parent = CatalogRepository.ReadId(Catalog.Cat_Parent);
                    Model.Local.ArticleCatalogRepository ArticleCatalogRepository = new Model.Local.ArticleCatalogRepository();
                    if (ArticleCatalogRepository.ExistArticleCatalog(Article.Art_Id, Parent.Cat_Id) == false)
                    {
                        ArticleCatalogRepository.Add(new Model.Local.ArticleCatalog()
                        {
                            Art_Id = Article.Art_Id,
                            Cat_Id = Parent.Cat_Id
                        });
                    }
                    AssignCatalogSecondaire(Article, Parent);
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[IMPORT SAGE] Affectation catalogues secondaires" + ex.ToString());
            }
        }

        #region Import Catalogue Info Libre

        public void ImportCatalogueInfoLibre(Model.Sage.F_ARTICLE F_ARTICLE, Model.Local.Article Article)
        {
            Model.Local.CatalogRepository CatalogRepository = new Model.Local.CatalogRepository();

            Model.Local.InformationLibreArticleRepository InformationLibreArticleRepository = new Model.Local.InformationLibreArticleRepository();
            Model.Local.InformationLibreArticle InformationLibreArticleParent = new Model.Local.InformationLibreArticle();

            Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();

            Model.Local.Catalog Parent;

            foreach (Model.Local.InformationLibreArticle InformationLibreArticle in InformationLibreArticleRepository.List())
            {
                CatalogRepository = new Model.Local.CatalogRepository();
                Parent = null;

                // si création de catalogue enfant d'un existant
                if (InformationLibreArticle.Inf_Catalogue == 2)
                {
                    if (CatalogRepository.ExistNameLevelParent(InformationLibreArticle.Inf_Parent, InformationLibreArticle.Inf_Catalogue, 0))
                    {
                        Parent = CatalogRepository.ReadNameLevelParent(InformationLibreArticle.Inf_Parent, InformationLibreArticle.Inf_Catalogue, 0);
                    }
                }
                // sinon si création d'un catalogue enfant de celui défini dans une autre information libre
                else if (InformationLibreArticle.Inf_Catalogue == 3)
                {
                    if (InformationLibreArticleRepository.ExistInfoLibreLevel(InformationLibreArticle.Inf_Parent, InformationLibreArticle.Inf_Catalogue - 1))
                    {
                        InformationLibreArticleParent = InformationLibreArticleRepository.ReadInfoLibre(InformationLibreArticle.Inf_Parent, InformationLibreArticle.Inf_Catalogue - 1);
                        String CatParent = Core.Global.RemovePurge(F_ARTICLERepository.ReadArticleInformationLibreText(InformationLibreArticleParent.Sag_InfoLibreArticle, F_ARTICLE.AR_Ref), 128);

                        // récupération du catalogue niveau 1
                        if (CatalogRepository.ExistNameLevelParent(InformationLibreArticleParent.Inf_Parent, InformationLibreArticle.Inf_Catalogue - 1, 0))
                        {
                            Parent = CatalogRepository.ReadNameLevelParent(InformationLibreArticleParent.Inf_Parent, InformationLibreArticle.Inf_Catalogue - 1, 0);
                        }
                        // récupération du catalogue niveau 2
                        if (CatalogRepository.ExistNameLevelParent(CatParent, InformationLibreArticle.Inf_Catalogue, Parent.Cat_Id))
                        {
                            Parent = CatalogRepository.ReadNameLevelParent(CatParent, InformationLibreArticle.Inf_Catalogue, Parent.Cat_Id);
                        }
                    }
                }
                if (Parent != null)
                {
                    CreateCatalogueInfoLibre(InformationLibreArticle, F_ARTICLE, Article, InformationLibreArticle.Inf_Catalogue + 1, Parent.Cat_Id);
                }
            }
        }

        private void CreateCatalogueInfoLibre(Model.Local.InformationLibreArticle InformationLibreArticle, Model.Sage.F_ARTICLE F_ARTICLE, Model.Local.Article Article, Int32 Level, Int32 Parent)
        {
            try
            {
                Model.Local.CatalogRepository CatalogRepository = new Model.Local.CatalogRepository();
                Model.Local.Catalog Catalog = new Model.Local.Catalog();

                Model.Sage.cbSysLibreRepository cbSysLibreRepository = new Model.Sage.cbSysLibreRepository();
                Model.Sage.cbSysLibre cbSysLibre = new Model.Sage.cbSysLibre();

                Model.Sage.P_INTSTATARTRepository P_INTSTATARTRepository = new Model.Sage.P_INTSTATARTRepository();
                Model.Sage.P_INTSTATART P_INTSTATART = new Model.Sage.P_INTSTATART();

                Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();

                if ((cbSysLibreRepository.ExistInformationLibre(InformationLibreArticle.Sag_InfoLibreArticle, Model.Sage.cbSysLibreRepository.CB_File.F_ARTICLE)
                        && InformationLibreArticle.Inf_IsStat == false)
                    || (P_INTSTATARTRepository.ExistStatArt(InformationLibreArticle.Sag_InfoLibreArticle)
                        && InformationLibreArticle.Inf_IsStat == true))
                {
                    if ((InformationLibreArticle.Inf_IsStat == false
                        && !String.IsNullOrEmpty(F_ARTICLERepository.ReadArticleInformationLibreText(InformationLibreArticle.Sag_InfoLibreArticle, F_ARTICLE.AR_Ref)))
                        || InformationLibreArticle.Inf_IsStat == true)
                    {
                        Catalog = new Model.Local.Catalog();
                        if (InformationLibreArticle.Inf_IsStat == true)
                        {
                            int caseSwitch = (int)InformationLibreArticle.Inf_Stat;
                            switch (caseSwitch)
                            {
                                case 1:
                                    if (F_ARTICLE.AR_Stat01 != null)
                                    {
                                        Catalog.Cat_Name = F_ARTICLE.AR_Stat01.ToString();
                                    }
                                    break;
                                case 2:
                                    if (F_ARTICLE.AR_Stat02 != null)
                                    {
                                        Catalog.Cat_Name = F_ARTICLE.AR_Stat02.ToString();
                                    }
                                    break;
                                case 3:
                                    if (F_ARTICLE.AR_Stat03 != null)
                                    {
                                        Catalog.Cat_Name = F_ARTICLE.AR_Stat03.ToString();
                                    }
                                    break;
                                case 4:
                                    if (F_ARTICLE.AR_Stat04 != null)
                                    {
                                        Catalog.Cat_Name = F_ARTICLE.AR_Stat04.ToString();
                                    }
                                    break;
                                case 5:
                                    if (F_ARTICLE.AR_Stat05 != null)
                                    {
                                        Catalog.Cat_Name = F_ARTICLE.AR_Stat05.ToString();
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            Catalog.Cat_Name = F_ARTICLERepository.ReadArticleInformationLibreText(InformationLibreArticle.Sag_InfoLibreArticle, F_ARTICLE.AR_Ref);
                        }

                        Catalog.Cat_Name = Core.Global.RemovePurge(Catalog.Cat_Name, 128);

                        Catalog.Cat_Description = Catalog.Cat_Name;
                        Catalog.Cat_Level = Level;
                        Catalog.Cat_Parent = Parent;
                        Catalog.Cat_MetaTitle = Core.Global.RemovePurge(Catalog.Cat_Name, 70);
                        Catalog.Cat_MetaDescription = Core.Global.RemovePurge(Catalog.Cat_Name, 160);
                        Catalog.Cat_MetaKeyword = Core.Global.RemovePurgeMeta(Catalog.Cat_Name, 255);
                        Catalog.Pre_Id = null;
                        //Catalog.Sag_Id = null;
                        Catalog.Cat_LinkRewrite = Core.Global.ReadLinkRewrite(Catalog.Cat_MetaTitle);

                        Catalog.Cat_Active = true;
                        Catalog.Cat_Sync = true;
                        Catalog.Cat_Date = DateTime.Now;

                        if (!String.IsNullOrEmpty(Catalog.Cat_Name))
                        {
                            if (CatalogRepository.ExistNameLevelParent(Catalog.Cat_Name, Catalog.Cat_Level, Catalog.Cat_Parent) == false)
                            {
                                CatalogRepository.Add(Catalog);
                            }
                            else
                            {
                                Catalog = CatalogRepository.ReadNameLevelParent(Catalog.Cat_Name, Catalog.Cat_Level, Catalog.Cat_Parent);
                            }

                            Core.ImportSage.ImportArticle ImportArticle = new ImportSage.ImportArticle();
                            ImportArticle.AssignCatalog(Catalog.Cat_Id, Catalog.Cat_Id, Article, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        #endregion
    }
}
