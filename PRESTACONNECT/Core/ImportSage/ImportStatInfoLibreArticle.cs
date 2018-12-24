using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Core.ImportSage
{
    public class ImportStatInfoLibreArticle
    {
        public void Exec(Int32 ArticleSend)
        {
            try
            {
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                if (ArticleRepository.ExistArticle(ArticleSend))
                {
                    Model.Local.Article Article = ArticleRepository.ReadArticle(ArticleSend);
                    Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();

                    string AR_Ref = Article.Art_Ref;

                    // ajout gestion lecture des informations pour une composition
                    if (!F_ARTICLERepository.ExistReference(Article.Art_Ref)
                        && Article.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleComposition
                        && Article.CompositionArticle != null && Article.CompositionArticle.Count > 0)
                    {
                        int sag_id = (from Table in Article.CompositionArticle
                                      orderby Table.ComArt_Default descending
                                      select Table.ComArt_F_ARTICLE_SagId).FirstOrDefault();
                        Model.Sage.F_ARTICLE_Light light = F_ARTICLERepository.ReadLight(sag_id);
                        if (light != null && !string.IsNullOrWhiteSpace(light.AR_Ref))
                        {
                            AR_Ref = light.AR_Ref;
                        }
                    }

                    if (F_ARTICLERepository.ExistReference(AR_Ref))
                    {
                        if (ImportValues(Article, AR_Ref))
                        {
                            if (Core.Temp.UpdateDateActive)
                                Article.Art_Date = DateTime.Now;

                            ArticleRepository.Save();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        public Boolean ImportValues(Model.Local.Article Article, string AR_Ref)
        {
            Boolean HasValue = false;
            try
            {
                Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();

                #region Informations libre Article

                Model.Local.InformationLibreRepository InformationLibreRepository = new Model.Local.InformationLibreRepository();
                foreach (Model.Local.InformationLibre InformationLibre in InformationLibreRepository.ListSync())
                {
                    // <JG> 21/08/2017 ajout filtre info libre pour automates
                    if (Core.Temp.TaskImportStatInfoLibreFilter == null || Core.Temp.TaskImportStatInfoLibreFilter.Contains(Core.Global.EscapeArgumentSyntax(InformationLibre.Sag_InfoLibre)))
                    {
                        Model.Sage.cbSysLibreRepository.CB_Type TypeInfoLibre = new Model.Sage.cbSysLibreRepository().ReadTypeInformationLibre(InformationLibre.Sag_InfoLibre, Model.Sage.cbSysLibreRepository.CB_File.F_ARTICLE);

                        switch (TypeInfoLibre)
                        {
                            case Model.Sage.cbSysLibreRepository.CB_Type.SageText:
                            case Model.Sage.cbSysLibreRepository.CB_Type.SageTable:
                                #region text
                                if (F_ARTICLERepository.ExistArticleInformationLibreText(InformationLibre.Sag_InfoLibre, AR_Ref))
                                {
                                    if (!string.IsNullOrWhiteSpace(Core.Global.SageValueReplacement(F_ARTICLERepository.ReadArticleInformationLibreText(InformationLibre.Sag_InfoLibre, AR_Ref))))
                                    {
                                        Model.Local.CharacteristicRepository CharacteristicRepository = new Model.Local.CharacteristicRepository();
                                        Model.Local.Characteristic ArticleCharacteristic;
                                        if (CharacteristicRepository.ExistFeatureArticle(InformationLibre.Cha_Id, Article.Art_Id))
                                        {
                                            ArticleCharacteristic = CharacteristicRepository.ReadFeatureArticle(InformationLibre.Cha_Id, Article.Art_Id);
                                            string temp_text = ArticleCharacteristic.Cha_Value;
                                            int? temp_id = ArticleCharacteristic.Pre_Id;
                                            ArticleCharacteristic.Cha_Value = Core.Global.SageValueReplacement(F_ARTICLERepository.ReadArticleInformationLibreText(InformationLibre.Sag_InfoLibre, AR_Ref)); ;
                                            switch (InformationLibre.Inf_Mode)
                                            {
                                                case (short)Core.Parametres.InformationLibreValeursMode.Predefinies:
                                                    ArticleCharacteristic.Cha_Custom = false;
                                                    ArticleCharacteristic.Pre_Id = CreateFeatureValue(ArticleCharacteristic.Cha_Value, InformationLibre.Cha_Id);
                                                    break;

                                                case (short)Core.Parametres.InformationLibreValeursMode.Personnalisees:
                                                    ArticleCharacteristic.Cha_Custom = true;
                                                    break;
                                            }
                                            CharacteristicRepository.Save();
                                            if (temp_text != ArticleCharacteristic.Cha_Value || temp_id != ArticleCharacteristic.Pre_Id)
                                                HasValue = true;
                                        }
                                        else
                                        {
                                            HasValue = true;
                                            ArticleCharacteristic = new Model.Local.Characteristic()
                                            {
                                                Art_Id = Article.Art_Id,
                                                Cha_IdFeature = (int)InformationLibre.Cha_Id,
                                                Cha_Value = Core.Global.SageValueReplacement(F_ARTICLERepository.ReadArticleInformationLibreText(InformationLibre.Sag_InfoLibre, AR_Ref)),
                                            };
                                            switch (InformationLibre.Inf_Mode)
                                            {
                                                case (short)Core.Parametres.InformationLibreValeursMode.Predefinies:
                                                    ArticleCharacteristic.Cha_Custom = false;
                                                    ArticleCharacteristic.Pre_Id = CreateFeatureValue(ArticleCharacteristic.Cha_Value, InformationLibre.Cha_Id);
                                                    break;

                                                case (short)Core.Parametres.InformationLibreValeursMode.Personnalisees:
                                                    ArticleCharacteristic.Cha_Custom = true;
                                                    break;
                                            }
                                            CharacteristicRepository.Add(ArticleCharacteristic);
                                        }
                                    }
                                    else if (Core.Global.GetConfig().ArticleSuppressionAutoCaracteristique)
                                    {
                                        DeleteFeatureProduct(InformationLibre, Article);
                                    }
                                }
                                #endregion
                                break;

                            case Model.Sage.cbSysLibreRepository.CB_Type.SageValeur:
                            case Model.Sage.cbSysLibreRepository.CB_Type.SageMontant:
                                #region decimal
                                if (F_ARTICLERepository.ExistArticleInformationLibreNumerique(InformationLibre.Sag_InfoLibre, AR_Ref))
                                {
                                    if (F_ARTICLERepository.ReadArticleInformationLibreNumerique(InformationLibre.Sag_InfoLibre, AR_Ref) != null)
                                    {
                                        Model.Local.CharacteristicRepository CharacteristicRepository = new Model.Local.CharacteristicRepository();
                                        Model.Local.Characteristic ArticleCharacteristic;
                                        if (CharacteristicRepository.ExistFeatureArticle(InformationLibre.Cha_Id, Article.Art_Id))
                                        {
                                            ArticleCharacteristic = CharacteristicRepository.ReadFeatureArticle(InformationLibre.Cha_Id, Article.Art_Id);
                                            string temp_text = ArticleCharacteristic.Cha_Value;
                                            int? temp_id = ArticleCharacteristic.Pre_Id;
                                            ArticleCharacteristic.Cha_Value = F_ARTICLERepository.ReadArticleInformationLibreNumerique(InformationLibre.Sag_InfoLibre, AR_Ref).Value
                                                .ToString((TypeInfoLibre == Model.Sage.cbSysLibreRepository.CB_Type.SageMontant) ? "0.00####" : "0.######");
                                            switch (InformationLibre.Inf_Mode)
                                            {
                                                case (short)Core.Parametres.InformationLibreValeursMode.Predefinies:
                                                    ArticleCharacteristic.Cha_Custom = false;
                                                    ArticleCharacteristic.Pre_Id = CreateFeatureValue(ArticleCharacteristic.Cha_Value, InformationLibre.Cha_Id);
                                                    break;

                                                case (short)Core.Parametres.InformationLibreValeursMode.Personnalisees:
                                                    ArticleCharacteristic.Cha_Custom = true;
                                                    break;
                                            }
                                            CharacteristicRepository.Save();
                                            if (temp_text != ArticleCharacteristic.Cha_Value || temp_id != ArticleCharacteristic.Pre_Id)
                                                HasValue = true;
                                        }
                                        else
                                        {
                                            HasValue = true;
                                            ArticleCharacteristic = new Model.Local.Characteristic()
                                            {
                                                Art_Id = Article.Art_Id,
                                                Cha_IdFeature = (int)InformationLibre.Cha_Id,
                                                Cha_Value = F_ARTICLERepository.ReadArticleInformationLibreNumerique(InformationLibre.Sag_InfoLibre, AR_Ref).Value
                                                .ToString((TypeInfoLibre == Model.Sage.cbSysLibreRepository.CB_Type.SageMontant) ? "0.00####" : "0.######"),
                                            };
                                            switch (InformationLibre.Inf_Mode)
                                            {
                                                case (short)Core.Parametres.InformationLibreValeursMode.Predefinies:
                                                    ArticleCharacteristic.Cha_Custom = false;
                                                    ArticleCharacteristic.Pre_Id = CreateFeatureValue(ArticleCharacteristic.Cha_Value, InformationLibre.Cha_Id);
                                                    break;

                                                case (short)Core.Parametres.InformationLibreValeursMode.Personnalisees:
                                                    ArticleCharacteristic.Cha_Custom = true;
                                                    break;
                                            }
                                            CharacteristicRepository.Add(ArticleCharacteristic);
                                        }
                                    }
                                    else if (Core.Global.GetConfig().ArticleSuppressionAutoCaracteristique)
                                    {
                                        DeleteFeatureProduct(InformationLibre, Article);
                                    }
                                }
                                #endregion
                                break;

                            case Model.Sage.cbSysLibreRepository.CB_Type.SageDate:
                            case Model.Sage.cbSysLibreRepository.CB_Type.SageSmallDate:
                                #region datetime
                                if (F_ARTICLERepository.ExistArticleInformationLibreDate(InformationLibre.Sag_InfoLibre, AR_Ref))
                                {
                                    if (F_ARTICLERepository.ReadArticleInformationLibreDate(InformationLibre.Sag_InfoLibre, AR_Ref) != null)
                                    {
                                        Model.Local.CharacteristicRepository CharacteristicRepository = new Model.Local.CharacteristicRepository();
                                        Model.Local.Characteristic ArticleCharacteristic;
                                        if (CharacteristicRepository.ExistFeatureArticle(InformationLibre.Cha_Id, Article.Art_Id))
                                        {
                                            ArticleCharacteristic = CharacteristicRepository.ReadFeatureArticle(InformationLibre.Cha_Id, Article.Art_Id);
                                            string temp_text = ArticleCharacteristic.Cha_Value;
                                            int? temp_id = ArticleCharacteristic.Pre_Id;
                                            ArticleCharacteristic.Cha_Value = F_ARTICLERepository.ReadArticleInformationLibreDate(InformationLibre.Sag_InfoLibre, AR_Ref).ToString();
                                            switch (InformationLibre.Inf_Mode)
                                            {
                                                case (short)Core.Parametres.InformationLibreValeursMode.Predefinies:
                                                    ArticleCharacteristic.Cha_Custom = false;
                                                    ArticleCharacteristic.Pre_Id = CreateFeatureValue(ArticleCharacteristic.Cha_Value, InformationLibre.Cha_Id);
                                                    break;

                                                case (short)Core.Parametres.InformationLibreValeursMode.Personnalisees:
                                                    ArticleCharacteristic.Cha_Custom = true;
                                                    break;
                                            }
                                            CharacteristicRepository.Save();
                                            if (temp_text != ArticleCharacteristic.Cha_Value || temp_id != ArticleCharacteristic.Pre_Id)
                                                HasValue = true;
                                        }
                                        else
                                        {
                                            ArticleCharacteristic = new Model.Local.Characteristic()
                                            {
                                                Art_Id = Article.Art_Id,
                                                Cha_IdFeature = (int)InformationLibre.Cha_Id,
                                                Cha_Value = F_ARTICLERepository.ReadArticleInformationLibreDate(InformationLibre.Sag_InfoLibre, AR_Ref).ToString(),
                                            };
                                            switch (InformationLibre.Inf_Mode)
                                            {
                                                case (short)Core.Parametres.InformationLibreValeursMode.Predefinies:
                                                    ArticleCharacteristic.Cha_Custom = false;
                                                    ArticleCharacteristic.Pre_Id = CreateFeatureValue(ArticleCharacteristic.Cha_Value, InformationLibre.Cha_Id);
                                                    break;

                                                case (short)Core.Parametres.InformationLibreValeursMode.Personnalisees:
                                                    ArticleCharacteristic.Cha_Custom = true;
                                                    break;
                                            }
                                            CharacteristicRepository.Add(ArticleCharacteristic);
                                        }
                                    }
                                    else if (Core.Global.GetConfig().ArticleSuppressionAutoCaracteristique)
                                    {
                                        DeleteFeatureProduct(InformationLibre, Article);
                                    }
                                }
                                #endregion
                                break;

                            case Model.Sage.cbSysLibreRepository.CB_Type.Deleted:
                            default:
                                break;
                        }
                    }
                }

                #endregion

                // traitement uniquement si absence de filtre sur l'automate
                if (Core.Temp.TaskImportStatInfoLibreFilter == null)
                {
                    #region Statistiques Article

                    Model.Local.StatistiqueArticleRepository StatistiqueArticleRepository = new Model.Local.StatistiqueArticleRepository();
                    foreach (Model.Local.StatistiqueArticle StatistiqueArticle in StatistiqueArticleRepository.ListSync())
                    {
                        Model.Sage.P_INTSTATARTRepository P_INTSTATARTRepository = new Model.Sage.P_INTSTATARTRepository();
                        if (P_INTSTATARTRepository.ExistStatArt(StatistiqueArticle.Sag_StatArt))
                        {
                            Model.Sage.P_INTSTATART P_INTSTATART = P_INTSTATARTRepository.ReadStatArt(StatistiqueArticle.Sag_StatArt);
                            Model.Sage.F_ARTICLE F_ARTICLE = F_ARTICLERepository.ReadReference(AR_Ref);
                            String stat_value = null;
                            switch (P_INTSTATART.cbMarq)
                            {
                                case 1:
                                    stat_value = F_ARTICLE.AR_Stat01;
                                    break;
                                case 2:
                                    stat_value = F_ARTICLE.AR_Stat02;
                                    break;
                                case 3:
                                    stat_value = F_ARTICLE.AR_Stat03;
                                    break;
                                case 4:
                                    stat_value = F_ARTICLE.AR_Stat04;
                                    break;
                                case 5:
                                    stat_value = F_ARTICLE.AR_Stat05;
                                    break;
                            }
                            if (!String.IsNullOrWhiteSpace(stat_value))
                            {
                                Model.Local.CharacteristicRepository CharacteristicRepository = new Model.Local.CharacteristicRepository();
                                Model.Local.Characteristic ArticleCharacteristic;
                                if (CharacteristicRepository.ExistFeatureArticle(StatistiqueArticle.Cha_Id, Article.Art_Id))
                                {
                                    ArticleCharacteristic = CharacteristicRepository.ReadFeatureArticle(StatistiqueArticle.Cha_Id, Article.Art_Id);
                                    string temp_text = ArticleCharacteristic.Cha_Value;
                                    int? temp_id = ArticleCharacteristic.Pre_Id;
                                    ArticleCharacteristic.Cha_Value = Core.Global.SageValueReplacement(stat_value);
                                    switch (StatistiqueArticle.Inf_Mode)
                                    {
                                        case (short)PRESTACONNECT.Core.Parametres.InformationLibreValeursMode.Predefinies:
                                            ArticleCharacteristic.Cha_Custom = false;
                                            ArticleCharacteristic.Pre_Id = CreateFeatureValue(ArticleCharacteristic.Cha_Value, StatistiqueArticle.Cha_Id);
                                            break;

                                        case (short)PRESTACONNECT.Core.Parametres.InformationLibreValeursMode.Personnalisees:
                                            ArticleCharacteristic.Cha_Custom = true;
                                            break;
                                    }
                                    CharacteristicRepository.Save();
                                    if (temp_text != ArticleCharacteristic.Cha_Value || temp_id != ArticleCharacteristic.Pre_Id)
                                        HasValue = true;
                                }
                                else
                                {
                                    HasValue = true;
                                    ArticleCharacteristic = new Model.Local.Characteristic()
                                    {
                                        Art_Id = Article.Art_Id,
                                        Cha_IdFeature = (int)StatistiqueArticle.Cha_Id,
                                        Cha_Value = Core.Global.SageValueReplacement(stat_value),
                                    };
                                    switch (StatistiqueArticle.Inf_Mode)
                                    {
                                        case (short)PRESTACONNECT.Core.Parametres.InformationLibreValeursMode.Predefinies:
                                            ArticleCharacteristic.Cha_Custom = false;
                                            ArticleCharacteristic.Pre_Id = CreateFeatureValue(ArticleCharacteristic.Cha_Value, StatistiqueArticle.Cha_Id);
                                            break;

                                        case (short)PRESTACONNECT.Core.Parametres.InformationLibreValeursMode.Personnalisees:
                                            ArticleCharacteristic.Cha_Custom = true;
                                            break;
                                    }
                                    CharacteristicRepository.Add(ArticleCharacteristic);
                                }
                            }
                        }
                    }

                    #endregion
                }

                #region Marque/Fabricant

                if (Core.Global.GetConfig().MarqueAutoStatistiqueActif && Core.Temp.TaskImportStatInfoLibreFilter == null)
                {
                    Model.Sage.P_INTSTATARTRepository P_INTSTATARTRepository = new Model.Sage.P_INTSTATARTRepository();
                    if (P_INTSTATARTRepository.ExistStatArt(Core.Global.GetConfig().MarqueAutoStatistiqueName))
                    {
                        Model.Sage.P_INTSTATART P_INTSTATART = P_INTSTATARTRepository.ReadStatArt(Core.Global.GetConfig().MarqueAutoStatistiqueName);
                        Model.Sage.F_ARTICLE F_ARTICLE = F_ARTICLERepository.ReadReference(AR_Ref);
                        String stat_value = null;
                        switch (P_INTSTATART.cbMarq)
                        {
                            case 1:
                                stat_value = F_ARTICLE.AR_Stat01;
                                break;
                            case 2:
                                stat_value = F_ARTICLE.AR_Stat02;
                                break;
                            case 3:
                                stat_value = F_ARTICLE.AR_Stat03;
                                break;
                            case 4:
                                stat_value = F_ARTICLE.AR_Stat04;
                                break;
                            case 5:
                                stat_value = F_ARTICLE.AR_Stat05;
                                break;
                        }
                        if (!String.IsNullOrWhiteSpace(stat_value))
                        {
                            int temp_id = Article.Art_Manufacturer;
                            Article.Art_Manufacturer = CreateManufacturer(Core.Global.SageValueReplacement(stat_value));
                            if (temp_id != Article.Art_Manufacturer)
                                HasValue = true;
                        }
                    }
                }
                else if (Core.Global.GetConfig().MarqueAutoInfolibreActif)
                {
                    Model.Sage.cbSysLibreRepository.CB_Type TypeInfoLibre = new Model.Sage.cbSysLibreRepository().ReadTypeInformationLibre(Core.Global.GetConfig().MarqueAutoInfolibreName, Model.Sage.cbSysLibreRepository.CB_File.F_ARTICLE);

                    switch (TypeInfoLibre)
                    {
                        case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageText:
                        case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageTable:
                            #region text
                            if (F_ARTICLERepository.ExistArticleInformationLibreText(Core.Global.GetConfig().MarqueAutoInfolibreName, AR_Ref)
                                && !string.IsNullOrWhiteSpace(Core.Global.SageValueReplacement(F_ARTICLERepository.ReadArticleInformationLibreText(Core.Global.GetConfig().MarqueAutoInfolibreName, AR_Ref))))
                            {
                                int temp_id = Article.Art_Manufacturer;
                                Article.Art_Manufacturer = CreateManufacturer(Core.Global.SageValueReplacement(F_ARTICLERepository.ReadArticleInformationLibreText(Core.Global.GetConfig().MarqueAutoInfolibreName, AR_Ref)));
                                if (temp_id != Article.Art_Manufacturer)
                                    HasValue = true;
                            }
                            #endregion
                            break;

                        default:
                            break;
                    }
                }

                #endregion

                #region Fournisseur

                if (Core.Global.GetConfig().FournisseurAutoStatistiqueActif && Core.Temp.TaskImportStatInfoLibreFilter == null)
                {
                    Model.Sage.P_INTSTATARTRepository P_INTSTATARTRepository = new Model.Sage.P_INTSTATARTRepository();
                    if (P_INTSTATARTRepository.ExistStatArt(Core.Global.GetConfig().FournisseurAutoStatistiqueName))
                    {
                        Model.Sage.P_INTSTATART P_INTSTATART = P_INTSTATARTRepository.ReadStatArt(Core.Global.GetConfig().FournisseurAutoStatistiqueName);
                        Model.Sage.F_ARTICLE F_ARTICLE = F_ARTICLERepository.ReadReference(AR_Ref);
                        String stat_value = null;
                        switch (P_INTSTATART.cbMarq)
                        {
                            case 1:
                                stat_value = F_ARTICLE.AR_Stat01;
                                break;
                            case 2:
                                stat_value = F_ARTICLE.AR_Stat02;
                                break;
                            case 3:
                                stat_value = F_ARTICLE.AR_Stat03;
                                break;
                            case 4:
                                stat_value = F_ARTICLE.AR_Stat04;
                                break;
                            case 5:
                                stat_value = F_ARTICLE.AR_Stat05;
                                break;
                        }
                        if (!String.IsNullOrWhiteSpace(stat_value))
                        {
                            int temp_id = Article.Art_Supplier;
                            Article.Art_Supplier = CreateSupplier(Core.Global.SageValueReplacement(stat_value));
                            if (temp_id != Article.Art_Supplier)
                                HasValue = true;
                        }
                    }
                }
                else if (Core.Global.GetConfig().FournisseurAutoInfolibreActif)
                {
                    Model.Sage.cbSysLibreRepository.CB_Type TypeInfoLibre = new Model.Sage.cbSysLibreRepository().ReadTypeInformationLibre(Core.Global.GetConfig().FournisseurAutoInfolibreName, Model.Sage.cbSysLibreRepository.CB_File.F_ARTICLE);

                    switch (TypeInfoLibre)
                    {
                        case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageText:
                        case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageTable:
                            #region text
                            if (F_ARTICLERepository.ExistArticleInformationLibreText(Core.Global.GetConfig().FournisseurAutoInfolibreName, AR_Ref)
                                && !string.IsNullOrWhiteSpace(Core.Global.SageValueReplacement(F_ARTICLERepository.ReadArticleInformationLibreText(Core.Global.GetConfig().FournisseurAutoInfolibreName, AR_Ref))))
                            {
                                int temp_id = Article.Art_Supplier;
                                Article.Art_Supplier = CreateSupplier(Core.Global.SageValueReplacement(F_ARTICLERepository.ReadArticleInformationLibreText(Core.Global.GetConfig().FournisseurAutoInfolibreName, AR_Ref)));
                                if (temp_id != Article.Art_Supplier)
                                    HasValue = true;
                            }
                            #endregion
                            break;

                        default:
                            break;
                    }
                }

                #endregion

                if (Core.Temp.TaskImportStatInfoLibreFilter == null)
                {
                    #region Information Sage

                    Model.Local.InformationArticleRepository InformationArticleRepository = new Model.Local.InformationArticleRepository();
                    foreach (Model.Local.InformationArticle InformationArticle in InformationArticleRepository.ListSync())
                    {
                        {
                            Model.Internal.SageInfoArticle SageInfoArticle = new Model.Internal.SageInfoArticle((Parametres.SageInfoArticle)InformationArticle.Sag_InfoArt);
                            Model.Sage.F_ARTICLE F_ARTICLE = F_ARTICLERepository.ReadReference(AR_Ref);
                            String stat_value = null;
                            switch (SageInfoArticle._SageInfoArticle)
                            {
                                case PRESTACONNECT.Core.Parametres.SageInfoArticle.Substitut:
                                    stat_value = F_ARTICLE.AR_Substitut;
                                    break;
                                case PRESTACONNECT.Core.Parametres.SageInfoArticle.FamilleCode:
                                    stat_value = F_ARTICLE.FA_CodeFamille;
                                    break;
                                case PRESTACONNECT.Core.Parametres.SageInfoArticle.FamilleIntitule:
                                    if (F_ARTICLE.F_FAMILLE != null && !string.IsNullOrEmpty(F_ARTICLE.F_FAMILLE.FA_Intitule))
                                        stat_value = F_ARTICLE.F_FAMILLE.FA_Intitule;
                                    break;
                                case PRESTACONNECT.Core.Parametres.SageInfoArticle.Garantie:
                                    if (F_ARTICLE.AR_Garantie != null)
                                        stat_value = F_ARTICLE.AR_Garantie.Value.ToString();
                                    break;
                                case PRESTACONNECT.Core.Parametres.SageInfoArticle.Pays:
                                    stat_value = F_ARTICLE.AR_Pays;
                                    break;
                                case PRESTACONNECT.Core.Parametres.SageInfoArticle.PoidsNet:
                                    if (F_ARTICLE.AR_PoidsNet != null && F_ARTICLE.AR_PoidsNet != 0)
                                        stat_value = F_ARTICLE.AR_PoidsNet.ToString();
                                    break;
                                case PRESTACONNECT.Core.Parametres.SageInfoArticle.PoidsBrut:
                                    if (F_ARTICLE.AR_PoidsBrut != null && F_ARTICLE.AR_PoidsBrut != 0)
                                        stat_value = F_ARTICLE.AR_PoidsBrut.ToString();
                                    break;
                            }
                            if (!String.IsNullOrWhiteSpace(stat_value))
                            {
                                Model.Local.CharacteristicRepository CharacteristicRepository = new Model.Local.CharacteristicRepository();
                                Model.Local.Characteristic ArticleCharacteristic;
                                if (CharacteristicRepository.ExistFeatureArticle(InformationArticle.Cha_Id, Article.Art_Id))
                                {
                                    ArticleCharacteristic = CharacteristicRepository.ReadFeatureArticle(InformationArticle.Cha_Id, Article.Art_Id);
                                    string temp_text = ArticleCharacteristic.Cha_Value;
                                    int? temp_id = ArticleCharacteristic.Pre_Id;
                                    ArticleCharacteristic.Cha_Value = Core.Global.SageValueReplacement(stat_value);
                                    switch (InformationArticle.Inf_Mode)
                                    {
                                        case (short)PRESTACONNECT.Core.Parametres.InformationLibreValeursMode.Predefinies:
                                            ArticleCharacteristic.Cha_Custom = false;
                                            ArticleCharacteristic.Pre_Id = CreateFeatureValue(ArticleCharacteristic.Cha_Value, InformationArticle.Cha_Id);
                                            break;

                                        case (short)PRESTACONNECT.Core.Parametres.InformationLibreValeursMode.Personnalisees:
                                            ArticleCharacteristic.Cha_Custom = true;
                                            break;
                                    }
                                    CharacteristicRepository.Save();
                                    if (temp_text != ArticleCharacteristic.Cha_Value || temp_id != ArticleCharacteristic.Pre_Id)
                                        HasValue = true;
                                }
                                else
                                {
                                    HasValue = true;
                                    ArticleCharacteristic = new Model.Local.Characteristic()
                                    {
                                        Art_Id = Article.Art_Id,
                                        Cha_IdFeature = (int)InformationArticle.Cha_Id,
                                        Cha_Value = Core.Global.SageValueReplacement(stat_value),
                                    };
                                    switch (InformationArticle.Inf_Mode)
                                    {
                                        case (short)PRESTACONNECT.Core.Parametres.InformationLibreValeursMode.Predefinies:
                                            ArticleCharacteristic.Cha_Custom = false;
                                            ArticleCharacteristic.Pre_Id = CreateFeatureValue(ArticleCharacteristic.Cha_Value, InformationArticle.Cha_Id);
                                            break;

                                        case (short)PRESTACONNECT.Core.Parametres.InformationLibreValeursMode.Personnalisees:
                                            ArticleCharacteristic.Cha_Custom = true;
                                            break;
                                    }
                                    CharacteristicRepository.Add(ArticleCharacteristic);
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

            return HasValue;
        }

        internal static Int32? CreateFeatureValue(String InfoLibreValue, Int32 Feature)
        {
            Int32? r = null;
            try
            {
                lock (Core.Temp.ListFeatureLocked)
                {
                    // ajout de la gestion d'une liste de locked feature au moment de la création pr éviter les doublons //
                    while (Core.Temp.ListFeatureLocked.Contains(Feature))
                        System.Threading.Thread.Sleep(20);

                    Core.Temp.ListFeatureLocked.Add(Feature);

                    Model.Prestashop.PsFeatureValueLangRepository PsFeatureValueLangRepository = new Model.Prestashop.PsFeatureValueLangRepository();
                    if (PsFeatureValueLangRepository.ExistFeatureValueLang(InfoLibreValue, Core.Global.Lang, (UInt32)Feature))
                    {
                        r = (int)PsFeatureValueLangRepository.ReadFeatureValueLang(InfoLibreValue, Core.Global.Lang, (UInt32)Feature).IDFeatureValue;
                    }
                    else
                    {
                        Model.Prestashop.PsFeatureValue PsFeatureValue = new Model.Prestashop.PsFeatureValue()
                        {
                            Custom = 0,
                            IDFeature = (uint)Feature
                        };
                        new Model.Prestashop.PsFeatureValueRepository().Add(PsFeatureValue);
                        r = (int)PsFeatureValue.IDFeatureValue;
                        foreach (Model.Prestashop.PsLang lang in new Model.Prestashop.PsLangRepository().ListActive(1, Core.Global.CurrentShop.IDShop))
                            new Model.Prestashop.PsFeatureValueLangRepository().Add(new Model.Prestashop.PsFeatureValueLang()
                            {
                                IDLang = lang.IDLang,
                                IDFeatureValue = PsFeatureValue.IDFeatureValue,
                                Value = InfoLibreValue
                            });
                    }

                    Core.Temp.ListFeatureLocked.Remove(Feature);
                }
            }
            catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
            return r;
        }

        private Int32 CreateManufacturer(String SageValue)
        {
            Int32 r = 0;
            try
            {
                lock (Core.Temp.ListManufacturerLocked)
                {
                    while (Core.Temp.ListManufacturerLocked.Contains(SageValue))
                        System.Threading.Thread.Sleep(20);

                    Core.Temp.ListManufacturerLocked.Add(SageValue);

                    Model.Prestashop.PsManufacturerRepository PsManufacturerRepository = new Model.Prestashop.PsManufacturerRepository();
                    if (PsManufacturerRepository.Exist(SageValue))
                    {
                        r = (int)PsManufacturerRepository.Read(SageValue).IDManufacturer;
                    }
                    else
                    {
                        Model.Prestashop.PsManufacturer PsManufacturer = new Model.Prestashop.PsManufacturer()
                        {
                            Name = SageValue,
                            DateAdd = DateTime.Now,
                            DateUpd = DateTime.Now,
                            Active = 1,
                        };
                        new Model.Prestashop.PsManufacturerRepository().Add(PsManufacturer, Core.Global.CurrentShop.IDShop);
                        r = (int)PsManufacturer.IDManufacturer;
                        foreach (Model.Prestashop.PsLang lang in new Model.Prestashop.PsLangRepository().ListActive(1, Core.Global.CurrentShop.IDShop))
                            new Model.Prestashop.PsManufacturerLangRepository().Add(new Model.Prestashop.PsManufacturerLang()
                            {
                                IDLang = lang.IDLang,
                                IDManufacturer = PsManufacturer.IDManufacturer,
                                Description = SageValue,
                                ShortDescription = SageValue,
                                MetaDescription = SageValue,
                                MetaKeywords = SageValue,
                                MetaTitle = SageValue,
                            });
                    }
                    Core.Temp.ListManufacturerLocked.Remove(SageValue);
                }
            }
            catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
            return r;
        }

        private Int32 CreateSupplier(String SageValue)
        {
            Int32 r = 0;
            try
            {
                lock (Core.Temp.ListSupplierLocked)
                {
                    while (Core.Temp.ListSupplierLocked.Contains(SageValue))
                        System.Threading.Thread.Sleep(20);

                    Core.Temp.ListSupplierLocked.Add(SageValue);

                    Model.Prestashop.PsSupplierRepository PsSupplierRepository = new Model.Prestashop.PsSupplierRepository();
                    if (PsSupplierRepository.Exist(SageValue))
                    {
                        r = (int)PsSupplierRepository.Read(SageValue).IDSupplier;
                    }
                    else
                    {
                        Model.Prestashop.PsSupplier PsSupplier = new Model.Prestashop.PsSupplier()
                        {
                            Name = SageValue,
                            DateAdd = DateTime.Now,
                            DateUpd = DateTime.Now,
                            Active = 1,
                        };
                        new Model.Prestashop.PsSupplierRepository().Add(PsSupplier, Core.Global.CurrentShop.IDShop);
                        r = (int)PsSupplier.IDSupplier;
                        foreach (Model.Prestashop.PsLang lang in new Model.Prestashop.PsLangRepository().ListActive(1, Core.Global.CurrentShop.IDShop))
                            new Model.Prestashop.PsSupplierLangRepository().Add(new Model.Prestashop.PsSupplierLang()
                            {
                                IDLang = lang.IDLang,
                                IDSupplier = PsSupplier.IDSupplier,
                                Description = SageValue,
                                MetaDescription = SageValue,
                                MetaKeywords = SageValue,
                                MetaTitle = SageValue,
                            });
                    }
                    Core.Temp.ListSupplierLocked.Remove(SageValue);
                }
            }
            catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
            return r;
        }

        private void DeleteFeatureProduct(Model.Local.InformationLibre InformationLibre, Model.Local.Article Article)
        {
            try
            {
                Model.Local.CharacteristicRepository CharacteristicRepository = new Model.Local.CharacteristicRepository();
                if (CharacteristicRepository.ExistFeatureArticle(InformationLibre.Cha_Id, Article.Art_Id))
                {
                    Model.Local.Characteristic ArticleCharacteristic = CharacteristicRepository.ReadFeatureArticle(InformationLibre.Cha_Id, Article.Art_Id);
                    //delete prestashop
                    if (Article.Pre_Id != null)
                    {
                        Model.Prestashop.PsFeatureProductRepository PsFeatureProductRepository = new Model.Prestashop.PsFeatureProductRepository();
                        if (PsFeatureProductRepository.ExistFeatureProduct((uint)ArticleCharacteristic.Cha_IdFeature, (uint)Article.Pre_Id))
                        {
                            Model.Prestashop.PsFeatureProduct PsFeatureProduct = PsFeatureProductRepository.ReadFeatureProduct((uint)ArticleCharacteristic.Cha_IdFeature, (uint)Article.Pre_Id);
                            PsFeatureProductRepository.Delete(PsFeatureProduct);

                            Model.Prestashop.PsFeatureValueRepository PsFeatureValueRepository = new Model.Prestashop.PsFeatureValueRepository();
                            if (PsFeatureValueRepository.ExistFeatureValue(PsFeatureProduct.IDFeatureValue))
                            {
                                Model.Prestashop.PsFeatureValue PsFeatureValue = PsFeatureValueRepository.ReadFeatureValue(PsFeatureProduct.IDFeatureValue);
                                //delete all datas if featurevalue is custom
                                if (PsFeatureValue.Custom == 1)
                                {
                                    Model.Prestashop.PsFeatureValueLangRepository PsFeatureValueLangRepository = new Model.Prestashop.PsFeatureValueLangRepository();
                                    PsFeatureValueLangRepository.DeleteAll(PsFeatureValueLangRepository.ListFeatureValue(PsFeatureValue.IDFeatureValue));

                                    PsFeatureValueRepository.Delete(PsFeatureValue);
                                }
                            }
                        }
                    }
                    //delete local
                    CharacteristicRepository.Delete(ArticleCharacteristic);
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
    }
}
