using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Core.Tools
{
    public static class FiltreImportSage
    {
        public static bool ImportSageFilterExclude(Model.Sage.F_ARTICLE F_ARTICLE)
        {
            bool r = false;
            foreach (Model.Local.ImportSageFilter ImportSageFilter in new Model.Local.ImportSageFilterRepository().ListActive())
            {
                switch (ImportSageFilter.Imp_TargetData)
                {
                    case (int)Core.Parametres.ImportSageFilterTargetData.InformationLibreArticle:
                        Model.Sage.cbSysLibreRepository cbSysLibreRepository = new Model.Sage.cbSysLibreRepository();
                        if (cbSysLibreRepository.ExistInformationLibre(ImportSageFilter.Sag_Infolibre, Model.Sage.cbSysLibreRepository.CB_File.F_ARTICLE))
                        {
                            Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();

                            switch (ImportSageFilter.Imp_TypeSearchValue)
                            {
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueContains:
                                    r = F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, F_ARTICLE.AR_Ref)
                                            && F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, F_ARTICLE.AR_Ref, false).ToLower().Contains(ImportSageFilter.Imp_Value.ToLower());
                                    break;
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueBeginOrEndBy:
                                    r = F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, F_ARTICLE.AR_Ref)
                                            && (!F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, F_ARTICLE.AR_Ref, false).ToLower().StartsWith(ImportSageFilter.Imp_Value.ToLower())
                                                || !F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, F_ARTICLE.AR_Ref, false).ToLower().EndsWith(ImportSageFilter.Imp_Value.ToLower()));
                                    break;
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueBeginBy:
                                    r = F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, F_ARTICLE.AR_Ref)
                                            && F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, F_ARTICLE.AR_Ref, false).ToLower().StartsWith(ImportSageFilter.Imp_Value.ToLower());
                                    break;
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueEndBy:
                                    r = F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, F_ARTICLE.AR_Ref)
                                            && F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, F_ARTICLE.AR_Ref, false).ToLower().EndsWith(ImportSageFilter.Imp_Value.ToLower());
                                    break;
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueEquals:
                                    r = F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, F_ARTICLE.AR_Ref)
                                            && F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, F_ARTICLE.AR_Ref, false).ToLower() == ImportSageFilter.Imp_Value.ToLower();
                                    break;
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueNotContains:
                                    r = !F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, F_ARTICLE.AR_Ref, false)
                                            || !F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, F_ARTICLE.AR_Ref, false).ToLower().Contains(ImportSageFilter.Imp_Value.ToLower());
                                    break;
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueNotEquals:
                                    r = !F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, F_ARTICLE.AR_Ref, false)
                                            || F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, F_ARTICLE.AR_Ref, false).ToLower() != ImportSageFilter.Imp_Value.ToLower();
                                    break;

                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.DesignationContains:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginOrEndBy:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginBy:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.DesignationEndBy:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceContains:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginOrEndBy:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginBy:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceEndBy:
                                default:
                                    // no action
                                    break;
                            }
                        }
                        break;


                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleGammeConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleGamme:
                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Article:
                        switch (ImportSageFilter.Imp_TypeSearchValue)
                        {
                            // filtre sur l'intitulé
                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationContains:
                                r = (F_ARTICLE.AR_Design.ToLower().Contains(ImportSageFilter.Imp_Value.ToLower()));
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginOrEndBy:
                                r = (F_ARTICLE.AR_Design.ToLower().StartsWith(ImportSageFilter.Imp_Value.ToLower())
                                    || F_ARTICLE.AR_Design.ToLower().EndsWith(ImportSageFilter.Imp_Value.ToLower()));
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginBy:
                                r = (F_ARTICLE.AR_Design.ToLower().StartsWith(ImportSageFilter.Imp_Value.ToLower()));
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationEndBy:
                                r = (F_ARTICLE.AR_Design.ToLower().EndsWith(ImportSageFilter.Imp_Value.ToLower()));
                                break;

                            // filtre sur la référence
                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceContains:
                                r = (F_ARTICLE.AR_Ref.Contains(ImportSageFilter.Imp_Value));
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginOrEndBy:
                                r = (F_ARTICLE.AR_Ref.StartsWith(ImportSageFilter.Imp_Value)
                                    || F_ARTICLE.AR_Ref.EndsWith(ImportSageFilter.Imp_Value));
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginBy:
                                r = (F_ARTICLE.AR_Ref.StartsWith(ImportSageFilter.Imp_Value));
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceEndBy:
                                r = (F_ARTICLE.AR_Ref.EndsWith(ImportSageFilter.Imp_Value));
                                break;
                        }
                        break;

                    case (int)Core.Parametres.ImportSageFilterTargetData.GammeConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Gamme:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Conditionnement:
                    default:
                        // no action
                        break;
                }
                if (r)
                    break;
            }
            return r;
        }
        public static bool ImportSageFilterExclude(Model.Sage.F_ARTGAMME F_ARTGAMME)
        {
            bool r = false;
            foreach (Model.Local.ImportSageFilter ImportSageFilter in new Model.Local.ImportSageFilterRepository().ListActive())
            {
                switch (ImportSageFilter.Imp_TargetData)
                {
                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleGammeConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleGamme:
                    case (int)Core.Parametres.ImportSageFilterTargetData.GammeConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Gamme:
                        switch (ImportSageFilter.Imp_TypeSearchValue)
                        {
                            // filtre sur l'intitulé
                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationContains:
                                r = (F_ARTGAMME.EG_Enumere != null && F_ARTGAMME.EG_Enumere.ToLower().Contains(ImportSageFilter.Imp_Value.ToLower()));
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginOrEndBy:
                                r = (F_ARTGAMME.EG_Enumere != null && (F_ARTGAMME.EG_Enumere.ToLower().StartsWith(ImportSageFilter.Imp_Value.ToLower())
                                    || F_ARTGAMME.EG_Enumere.ToLower().EndsWith(ImportSageFilter.Imp_Value.ToLower())));
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginBy:
                                r = (F_ARTGAMME.EG_Enumere != null && F_ARTGAMME.EG_Enumere.ToLower().StartsWith(ImportSageFilter.Imp_Value.ToLower()));
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationEndBy:
                                r = (F_ARTGAMME.EG_Enumere != null && F_ARTGAMME.EG_Enumere.ToLower().EndsWith(ImportSageFilter.Imp_Value.ToLower()));
                                break;
                        }
                        break;

                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Article:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Conditionnement:
                    default:
                        // no action
                        break;
                }
                if (r)
                    break;
            }
            return r;
        }
        public static bool ImportSageFilterExclude(Model.Sage.F_ARTENUMREF F_ARTENUMREF)
        {
            bool r = false;
            foreach (Model.Local.ImportSageFilter ImportSageFilter in new Model.Local.ImportSageFilterRepository().ListActive())
            {
                switch (ImportSageFilter.Imp_TargetData)
                {
                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleGammeConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleGamme:
                    case (int)Core.Parametres.ImportSageFilterTargetData.GammeConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Gamme:
                        switch (ImportSageFilter.Imp_TypeSearchValue)
                        {
                            // filtre sur l'intitulé
                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceContains:
                                r = (F_ARTENUMREF.AE_Ref != null && F_ARTENUMREF.AE_Ref.ToLower().Contains(ImportSageFilter.Imp_Value.ToLower()));
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginOrEndBy:
                                r = (F_ARTENUMREF.AE_Ref != null && (F_ARTENUMREF.AE_Ref.ToLower().StartsWith(ImportSageFilter.Imp_Value.ToLower())
                                    || F_ARTENUMREF.AE_Ref.ToLower().EndsWith(ImportSageFilter.Imp_Value.ToLower())));
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginBy:
                                r = (F_ARTENUMREF.AE_Ref != null && F_ARTENUMREF.AE_Ref.ToLower().StartsWith(ImportSageFilter.Imp_Value.ToLower()));
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceEndBy:
                                r = (F_ARTENUMREF.AE_Ref != null && F_ARTENUMREF.AE_Ref.ToLower().EndsWith(ImportSageFilter.Imp_Value.ToLower()));
                                break;
                        }
                        break;

                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Article:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Conditionnement:
                    default:
                        // no action
                        break;
                }
                if (r)
                    break;
            }
            return r;
        }

        public static List<Model.Sage.F_ARTICLE_Import> ImportSageFilter(List<Model.Sage.F_ARTICLE_Import> articles)
        {
            foreach (Model.Local.ImportSageFilter ImportSageFilter in new Model.Local.ImportSageFilterRepository().ListActive())
            {
                switch (ImportSageFilter.Imp_TargetData)
                {
                    case (int)Core.Parametres.ImportSageFilterTargetData.InformationLibreArticle:
                        Model.Sage.cbSysLibreRepository cbSysLibreRepository = new Model.Sage.cbSysLibreRepository();
                        if (cbSysLibreRepository.ExistInformationLibre(ImportSageFilter.Sag_Infolibre, Model.Sage.cbSysLibreRepository.CB_File.F_ARTICLE))
                        {
                            Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();

                            switch (ImportSageFilter.Imp_TypeSearchValue)
                            {
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueContains:
                                    articles = articles.Where(a => !F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref)
                                                            || !F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false).ToLower().Contains(ImportSageFilter.Imp_Value.ToLower())
                                                        ).ToList();
                                    break;
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueBeginOrEndBy:
                                    articles = articles.Where(a => !F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref)
                                                            || (!F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false).ToLower().StartsWith(ImportSageFilter.Imp_Value.ToLower())
                                                                && !F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false).ToLower().EndsWith(ImportSageFilter.Imp_Value.ToLower()))
                                                        ).ToList();
                                    break;
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueBeginBy:
                                    articles = articles.Where(a => !F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref)
                                                            || !F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false).ToLower().StartsWith(ImportSageFilter.Imp_Value.ToLower())
                                                        ).ToList();
                                    break;
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueEndBy:
                                    articles = articles.Where(a => !F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref)
                                                            || !F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false).ToLower().EndsWith(ImportSageFilter.Imp_Value.ToLower())
                                                        ).ToList();
                                    break;
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueEquals:
                                    articles = articles.Where(a => !F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref)
                                                            || F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false).ToLower() != ImportSageFilter.Imp_Value.ToLower()
                                                        ).ToList();
                                    break;
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueNotContains:
                                    articles = articles.Where(a => F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false)
                                                            && F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false).ToLower().Contains(ImportSageFilter.Imp_Value.ToLower())
                                                        ).ToList();
                                    break;
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueNotEquals:
                                    articles = articles.Where(a => F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false)
                                                            && F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false).ToLower() == ImportSageFilter.Imp_Value.ToLower()
                                                        ).ToList();
                                    break;

                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.DesignationContains:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginOrEndBy:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginBy:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.DesignationEndBy:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceContains:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginOrEndBy:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginBy:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceEndBy:
                                default:
                                    // no action
                                    break;
                            }
                        }
                        break;


                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleGammeConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleGamme:
                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Article:
                        switch (ImportSageFilter.Imp_TypeSearchValue)
                        {
                            // filtre sur l'intitulé
                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationContains:
                                articles = articles.Where(a => !a.AR_Design.ToLower().Contains(ImportSageFilter.Imp_Value.ToLower())).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginOrEndBy:
                                articles = articles.Where(a => !a.AR_Design.ToLower().StartsWith(ImportSageFilter.Imp_Value.ToLower()) && !a.AR_Design.EndsWith(ImportSageFilter.Imp_Value.ToLower())).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginBy:
                                articles = articles.Where(a => !a.AR_Design.ToLower().StartsWith(ImportSageFilter.Imp_Value.ToLower())).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationEndBy:
                                articles = articles.Where(a => !a.AR_Design.ToLower().EndsWith(ImportSageFilter.Imp_Value.ToLower())).ToList();
                                break;

                            // filtre sur la référence
                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceContains:
                                articles = articles.Where(a => !a.AR_Ref.Contains(ImportSageFilter.Imp_Value)).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginOrEndBy:
                                articles = articles.Where(a => !a.AR_Ref.StartsWith(ImportSageFilter.Imp_Value) && !a.AR_Ref.EndsWith(ImportSageFilter.Imp_Value)).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginBy:
                                articles = articles.Where(a => !a.AR_Ref.StartsWith(ImportSageFilter.Imp_Value)).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceEndBy:
                                articles = articles.Where(a => !a.AR_Ref.EndsWith(ImportSageFilter.Imp_Value)).ToList();
                                break;
                        }
                        break;

                    case (int)Core.Parametres.ImportSageFilterTargetData.GammeConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Gamme:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Conditionnement:
                    default:
                        // no action
                        break;
                }
            }
            return articles;
        }
        public static List<Model.Sage.F_ARTICLE_Light> ImportSageFilter(List<Model.Sage.F_ARTICLE_Light> articles)
        {
            foreach (Model.Local.ImportSageFilter ImportSageFilter in new Model.Local.ImportSageFilterRepository().ListActive())
            {
                switch (ImportSageFilter.Imp_TargetData)
                {
                    case (int)Core.Parametres.ImportSageFilterTargetData.InformationLibreArticle:
                        Model.Sage.cbSysLibreRepository cbSysLibreRepository = new Model.Sage.cbSysLibreRepository();
                        if (cbSysLibreRepository.ExistInformationLibre(ImportSageFilter.Sag_Infolibre, Model.Sage.cbSysLibreRepository.CB_File.F_ARTICLE))
                        {
                            Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();

                            switch (ImportSageFilter.Imp_TypeSearchValue)
                            {
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueContains:
                                    articles = articles.Where(a => !F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref)
                                                            || !F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false).ToLower().Contains(ImportSageFilter.Imp_Value.ToLower())
                                                        ).ToList();
                                    break;
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueBeginOrEndBy:
                                    articles = articles.Where(a => !F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref)
                                                            || (!F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false).ToLower().StartsWith(ImportSageFilter.Imp_Value.ToLower())
                                                                && !F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false).ToLower().EndsWith(ImportSageFilter.Imp_Value.ToLower()))
                                                        ).ToList();
                                    break;
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueBeginBy:
                                    articles = articles.Where(a => !F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref)
                                                            || !F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false).ToLower().StartsWith(ImportSageFilter.Imp_Value.ToLower())
                                                        ).ToList();
                                    break;
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueEndBy:
                                    articles = articles.Where(a => !F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref)
                                                            || !F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false).ToLower().EndsWith(ImportSageFilter.Imp_Value.ToLower())
                                                        ).ToList();
                                    break;
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueEquals:
                                    articles = articles.Where(a => !F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref)
                                                            || F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false).ToLower() != ImportSageFilter.Imp_Value.ToLower()
                                                        ).ToList();
                                    break;
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueNotContains:
                                    articles = articles.Where(a => F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false)
                                                            && F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false).ToLower().Contains(ImportSageFilter.Imp_Value.ToLower())
                                                        ).ToList();
                                    break;
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueNotEquals:
                                    articles = articles.Where(a => F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false)
                                                            && F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false).ToLower() == ImportSageFilter.Imp_Value.ToLower()
                                                        ).ToList();
                                    break;

                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.DesignationContains:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginOrEndBy:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginBy:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.DesignationEndBy:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceContains:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginOrEndBy:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginBy:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceEndBy:
                                default:
                                    // no action
                                    break;
                            }
                        }
                        break;


                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleGammeConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleGamme:
                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Article:
                        switch (ImportSageFilter.Imp_TypeSearchValue)
                        {
                            // filtre sur l'intitulé
                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationContains:
                                articles = articles.Where(a => !a.AR_Design.ToLower().Contains(ImportSageFilter.Imp_Value.ToLower())).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginOrEndBy:
                                articles = articles.Where(a => !a.AR_Design.ToLower().StartsWith(ImportSageFilter.Imp_Value.ToLower()) && !a.AR_Design.EndsWith(ImportSageFilter.Imp_Value.ToLower())).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginBy:
                                articles = articles.Where(a => !a.AR_Design.ToLower().StartsWith(ImportSageFilter.Imp_Value.ToLower())).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationEndBy:
                                articles = articles.Where(a => !a.AR_Design.ToLower().EndsWith(ImportSageFilter.Imp_Value.ToLower())).ToList();
                                break;

                            // filtre sur la référence
                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceContains:
                                articles = articles.Where(a => !a.AR_Ref.Contains(ImportSageFilter.Imp_Value)).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginOrEndBy:
                                articles = articles.Where(a => !a.AR_Ref.StartsWith(ImportSageFilter.Imp_Value) && !a.AR_Ref.EndsWith(ImportSageFilter.Imp_Value)).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginBy:
                                articles = articles.Where(a => !a.AR_Ref.StartsWith(ImportSageFilter.Imp_Value)).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceEndBy:
                                articles = articles.Where(a => !a.AR_Ref.EndsWith(ImportSageFilter.Imp_Value)).ToList();
                                break;
                        }
                        break;

                    case (int)Core.Parametres.ImportSageFilterTargetData.GammeConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Gamme:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Conditionnement:
                    default:
                        // no action
                        break;
                }
            }
            return articles;
        }

        public static List<Model.Sage.F_ARTGAMME> ImportSageFilter(List<Model.Sage.F_ARTGAMME> enumeres_gammes)
        {
            foreach (Model.Local.ImportSageFilter ImportSageFilter in new Model.Local.ImportSageFilterRepository().ListActive())
            {
                switch (ImportSageFilter.Imp_TargetData)
                {
                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleGammeConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleGamme:
                    case (int)Core.Parametres.ImportSageFilterTargetData.GammeConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Gamme:
                        switch (ImportSageFilter.Imp_TypeSearchValue)
                        {
                            // filtre sur l'intitulé
                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationContains:
                                enumeres_gammes = enumeres_gammes.Where(g => g.EG_Enumere != null && !g.EG_Enumere.ToLower().Contains(ImportSageFilter.Imp_Value.ToLower())).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginOrEndBy:
                                enumeres_gammes = enumeres_gammes.Where(g => g.EG_Enumere != null && !g.EG_Enumere.ToLower().StartsWith(ImportSageFilter.Imp_Value.ToLower()) && !g.EG_Enumere.EndsWith(ImportSageFilter.Imp_Value.ToLower())).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginBy:
                                enumeres_gammes = enumeres_gammes.Where(g => g.EG_Enumere != null && !g.EG_Enumere.ToLower().StartsWith(ImportSageFilter.Imp_Value.ToLower())).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationEndBy:
                                enumeres_gammes = enumeres_gammes.Where(g => g.EG_Enumere != null && !g.EG_Enumere.ToLower().EndsWith(ImportSageFilter.Imp_Value.ToLower())).ToList();
                                break;
                        }
                        break;

                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Article:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Conditionnement:
                    default:
                        // no action
                        break;
                }
            }
            return enumeres_gammes;
        }
        public static List<Model.Sage.F_ARTENUMREF> ImportSageFilter(List<Model.Sage.F_ARTENUMREF> enumeres_gammes)
        {
            foreach (Model.Local.ImportSageFilter ImportSageFilter in new Model.Local.ImportSageFilterRepository().ListActive())
            {
                switch (ImportSageFilter.Imp_TargetData)
                {
                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleGammeConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleGamme:
                    case (int)Core.Parametres.ImportSageFilterTargetData.GammeConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Gamme:
                        switch (ImportSageFilter.Imp_TypeSearchValue)
                        {
                            // filtre sur la référence
                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceContains:
                                enumeres_gammes = enumeres_gammes.Where(g => string.IsNullOrWhiteSpace(g.AE_Ref) || !g.AE_Ref.Contains(ImportSageFilter.Imp_Value)).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginOrEndBy:
                                enumeres_gammes = enumeres_gammes.Where(g => string.IsNullOrWhiteSpace(g.AE_Ref) || (!g.AE_Ref.StartsWith(ImportSageFilter.Imp_Value) && !g.AE_Ref.EndsWith(ImportSageFilter.Imp_Value))).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginBy:
                                enumeres_gammes = enumeres_gammes.Where(g => string.IsNullOrWhiteSpace(g.AE_Ref) || !g.AE_Ref.StartsWith(ImportSageFilter.Imp_Value)).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceEndBy:
                                enumeres_gammes = enumeres_gammes.Where(g => string.IsNullOrWhiteSpace(g.AE_Ref) || !g.AE_Ref.EndsWith(ImportSageFilter.Imp_Value)).ToList();
                                break;
                        }
                        break;

                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Article:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Conditionnement:
                    default:
                        // no action
                        break;
                }
            }
            return enumeres_gammes;
        }

        public static List<Model.Sage.F_CONDITION> ImportSageFilter(List<Model.Sage.F_CONDITION> enumeres_conditionnement)
        {
            foreach (Model.Local.ImportSageFilter ImportSageFilter in new Model.Local.ImportSageFilterRepository().ListActive())
            {
                switch (ImportSageFilter.Imp_TargetData)
                {
                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleGammeConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.GammeConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Conditionnement:
                        switch (ImportSageFilter.Imp_TypeSearchValue)
                        {
                            // filtre sur l'intitulé
                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationContains:
                                enumeres_conditionnement = enumeres_conditionnement.Where(c => !c.EC_Enumere.ToLower().Contains(ImportSageFilter.Imp_Value.ToLower())).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginOrEndBy:
                                enumeres_conditionnement = enumeres_conditionnement.Where(c => !c.EC_Enumere.ToLower().StartsWith(ImportSageFilter.Imp_Value.ToLower()) && !c.EC_Enumere.EndsWith(ImportSageFilter.Imp_Value.ToLower())).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginBy:
                                enumeres_conditionnement = enumeres_conditionnement.Where(c => !c.EC_Enumere.ToLower().StartsWith(ImportSageFilter.Imp_Value.ToLower())).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationEndBy:
                                enumeres_conditionnement = enumeres_conditionnement.Where(c => !c.EC_Enumere.ToLower().EndsWith(ImportSageFilter.Imp_Value.ToLower())).ToList();
                                break;

                            // filtre sur la référence
                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceContains:
                                enumeres_conditionnement = enumeres_conditionnement.Where(c => string.IsNullOrWhiteSpace(c.CO_Ref) || !c.CO_Ref.Contains(ImportSageFilter.Imp_Value)).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginOrEndBy:
                                enumeres_conditionnement = enumeres_conditionnement.Where(c => string.IsNullOrWhiteSpace(c.CO_Ref) || (!c.CO_Ref.StartsWith(ImportSageFilter.Imp_Value) && !c.CO_Ref.EndsWith(ImportSageFilter.Imp_Value))).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginBy:
                                enumeres_conditionnement = enumeres_conditionnement.Where(c => string.IsNullOrWhiteSpace(c.CO_Ref) || !c.CO_Ref.StartsWith(ImportSageFilter.Imp_Value)).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceEndBy:
                                enumeres_conditionnement = enumeres_conditionnement.Where(c => string.IsNullOrWhiteSpace(c.CO_Ref) || !c.CO_Ref.EndsWith(ImportSageFilter.Imp_Value)).ToList();
                                break;
                        }
                        break;

                    case (int)Core.Parametres.ImportSageFilterTargetData.Article:
                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleGamme:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Gamme:
                    default:
                        // no action
                        break;
                }
            }
            return enumeres_conditionnement;
        }

        public static List<Model.Sage.F_ARTICLE_Composition> ImportSageFilter(List<Model.Sage.F_ARTICLE_Composition> articles)
        {
            foreach (Model.Local.ImportSageFilter ImportSageFilter in new Model.Local.ImportSageFilterRepository().ListActive())
            {
                switch (ImportSageFilter.Imp_TargetData)
                {
                    case (int)Core.Parametres.ImportSageFilterTargetData.InformationLibreArticle:
                        Model.Sage.cbSysLibreRepository cbSysLibreRepository = new Model.Sage.cbSysLibreRepository();
                        if (cbSysLibreRepository.ExistInformationLibre(ImportSageFilter.Sag_Infolibre, Model.Sage.cbSysLibreRepository.CB_File.F_ARTICLE))
                        {
                            Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();

                            switch (ImportSageFilter.Imp_TypeSearchValue)
                            {
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueContains:
                                    articles = articles.Where(a => !F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref)
                                                            || !F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false).ToLower().Contains(ImportSageFilter.Imp_Value.ToLower())
                                                        ).ToList();
                                    break;
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueBeginOrEndBy:
                                    articles = articles.Where(a => !F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref)
                                                            || (!F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false).ToLower().StartsWith(ImportSageFilter.Imp_Value.ToLower())
                                                                && !F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false).ToLower().EndsWith(ImportSageFilter.Imp_Value.ToLower()))
                                                        ).ToList();
                                    break;
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueBeginBy:
                                    articles = articles.Where(a => !F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref)
                                                            || !F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false).ToLower().StartsWith(ImportSageFilter.Imp_Value.ToLower())
                                                        ).ToList();
                                    break;
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueEndBy:
                                    articles = articles.Where(a => !F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref)
                                                            || !F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false).ToLower().EndsWith(ImportSageFilter.Imp_Value.ToLower())
                                                        ).ToList();
                                    break;
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueEquals:
                                    articles = articles.Where(a => !F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref)
                                                            || F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false).ToLower() != ImportSageFilter.Imp_Value.ToLower()
                                                        ).ToList();
                                    break;
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueNotContains:
                                    articles = articles.Where(a => F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false)
                                                            && F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false).ToLower().Contains(ImportSageFilter.Imp_Value.ToLower())
                                                        ).ToList();
                                    break;
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ValueNotEquals:
                                    articles = articles.Where(a => F_ARTICLERepository.ExistArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false)
                                                            && F_ARTICLERepository.ReadArticleInformationLibreText(ImportSageFilter.Sag_Infolibre, a.AR_Ref, false).ToLower() == ImportSageFilter.Imp_Value.ToLower()
                                                        ).ToList();
                                    break;

                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.DesignationContains:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginOrEndBy:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginBy:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.DesignationEndBy:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceContains:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginOrEndBy:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginBy:
                                case (int)PRESTACONNECT.Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceEndBy:
                                default:
                                    // no action
                                    break;
                            }
                        }
                        break;


                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleGammeConditionnement:
                        #region ArticleGammeConditionnement
                        switch (ImportSageFilter.Imp_TypeSearchValue)
                        {
                            // filtre sur l'intitulé
                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationContains:
                                articles = articles.Where(a => !a.AR_Design.Contains(ImportSageFilter.Imp_Value)
                                    && !a.EnumereGamme1.Contains(ImportSageFilter.Imp_Value)
                                    && !a.EnumereGamme2.Contains(ImportSageFilter.Imp_Value)
                                    && !a.EC_Enumere.Contains(ImportSageFilter.Imp_Value)).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginOrEndBy:
                                articles = articles.Where(a => !a.AR_Design.StartsWith(ImportSageFilter.Imp_Value) && !a.AR_Design.EndsWith(ImportSageFilter.Imp_Value)
                                    && !a.EnumereGamme1.StartsWith(ImportSageFilter.Imp_Value) && !a.EnumereGamme1.EndsWith(ImportSageFilter.Imp_Value)
                                    && !a.EnumereGamme2.StartsWith(ImportSageFilter.Imp_Value) && !a.EnumereGamme2.EndsWith(ImportSageFilter.Imp_Value)
                                    && !a.EC_Enumere.StartsWith(ImportSageFilter.Imp_Value) && !a.EC_Enumere.EndsWith(ImportSageFilter.Imp_Value)).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginBy:
                                articles = articles.Where(a => !a.AR_Design.StartsWith(ImportSageFilter.Imp_Value)
                                    && !a.EnumereGamme1.StartsWith(ImportSageFilter.Imp_Value)
                                    && !a.EnumereGamme2.StartsWith(ImportSageFilter.Imp_Value)
                                    && !a.EC_Enumere.StartsWith(ImportSageFilter.Imp_Value)).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationEndBy:
                                articles = articles.Where(a => !a.AR_Design.EndsWith(ImportSageFilter.Imp_Value)
                                    && !a.EnumereGamme1.EndsWith(ImportSageFilter.Imp_Value)
                                    && !a.EnumereGamme2.EndsWith(ImportSageFilter.Imp_Value)
                                    && !a.EC_Enumere.EndsWith(ImportSageFilter.Imp_Value)).ToList();
                                break;

                            // filtre sur la référence
                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceContains:
                                articles = articles.Where(a => !a.AR_Ref.Contains(ImportSageFilter.Imp_Value)
                                    && !a.AE_Ref.Contains(ImportSageFilter.Imp_Value)
                                    && !a.CO_Ref.Contains(ImportSageFilter.Imp_Value)).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginOrEndBy:
                                articles = articles.Where(a => !a.AR_Ref.StartsWith(ImportSageFilter.Imp_Value) && !a.AR_Ref.EndsWith(ImportSageFilter.Imp_Value)
                                    && !a.AE_Ref.StartsWith(ImportSageFilter.Imp_Value) && !a.AE_Ref.EndsWith(ImportSageFilter.Imp_Value)
                                    && !a.CO_Ref.StartsWith(ImportSageFilter.Imp_Value) && !a.CO_Ref.EndsWith(ImportSageFilter.Imp_Value)).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginBy:
                                articles = articles.Where(a => !a.AR_Ref.StartsWith(ImportSageFilter.Imp_Value)
                                    && !a.AE_Ref.StartsWith(ImportSageFilter.Imp_Value)
                                    && !a.CO_Ref.StartsWith(ImportSageFilter.Imp_Value)).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceEndBy:
                                articles = articles.Where(a => !a.AR_Ref.EndsWith(ImportSageFilter.Imp_Value)
                                    && !a.AE_Ref.EndsWith(ImportSageFilter.Imp_Value)
                                    && !a.CO_Ref.EndsWith(ImportSageFilter.Imp_Value)).ToList();
                                break;
                        }
                        break;
                        #endregion

                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleGamme:
                    case (int)Core.Parametres.ImportSageFilterTargetData.ArticleConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Article:
                        switch (ImportSageFilter.Imp_TypeSearchValue)
                        {
                            // filtre sur l'intitulé
                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationContains:
                                articles = articles.Where(a => !a.AR_Design.Contains(ImportSageFilter.Imp_Value)).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginOrEndBy:
                                articles = articles.Where(a => !a.AR_Design.StartsWith(ImportSageFilter.Imp_Value) && !a.AR_Design.EndsWith(ImportSageFilter.Imp_Value)).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginBy:
                                articles = articles.Where(a => !a.AR_Design.StartsWith(ImportSageFilter.Imp_Value)).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.DesignationEndBy:
                                articles = articles.Where(a => !a.AR_Design.EndsWith(ImportSageFilter.Imp_Value)).ToList();
                                break;

                            // filtre sur la référence
                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceContains:
                                articles = articles.Where(a => !a.AR_Ref.Contains(ImportSageFilter.Imp_Value)).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginOrEndBy:
                                articles = articles.Where(a => !a.AR_Ref.StartsWith(ImportSageFilter.Imp_Value) && !a.AR_Ref.EndsWith(ImportSageFilter.Imp_Value)).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginBy:
                                articles = articles.Where(a => !a.AR_Ref.StartsWith(ImportSageFilter.Imp_Value)).ToList();
                                break;

                            case (int)Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceEndBy:
                                articles = articles.Where(a => !a.AR_Ref.EndsWith(ImportSageFilter.Imp_Value)).ToList();
                                break;
                        }
                        break;

                    case (int)Core.Parametres.ImportSageFilterTargetData.GammeConditionnement:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Gamme:
                    case (int)Core.Parametres.ImportSageFilterTargetData.Conditionnement:
                    default:
                        // no action
                        break;
                }
            }
            return articles;
        }
    }
}
