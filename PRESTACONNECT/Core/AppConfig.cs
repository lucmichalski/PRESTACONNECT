using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using PRESTACONNECT.Core.Parametres;

namespace PRESTACONNECT.Core
{
    internal sealed class AppConfig
    {
        #region Properties

        #region Keys

        #region Avancé

        private const string ConfigAppFoldersKey = "[CONFIGAPPFOLDERS]";

        #endregion

        #region General

        private const string ConfigRemiseModeKey = "[REMISEMODE]";
        private const string ConfigImageStorageModeKey = "[CONFIGIMAGESTORAGEMODE]";
        private const string ConfigAdminMailAddressKey = "[CONFIGTRANSFERTADMINMAILADDRESS]";

        private const string ConfigRemiseConflitKey = "[REMISECONFLIT]";

        #region Mode

        public const string ConfigBToBKey = "[CONFIGBTOB]";
        public const string ConfigBToCKey = "[CONFIGBTOC]";

        #endregion

        #region FTP

        private const string ConfigFTPActiveKey = "[IMAGE]";
        private const string ConfigFTPIPKey = "[FTPIP]";
        private const string ConfigFTPUserKey = "[FTPUSER]";
        private const string ConfigFTPPasswordKey = "[FTPPASSWORD]";
        private const string ConfigFTPSSLKey = "[FTPSSL]";

        #endregion

        #region Image

        public static String ConfigImage = "[IMAGE]";

        private static string ConfigImageSynchroPositionLegendeKey = "[CONFIGIMAGESYNCHROPOSITIONLEGENDE]";
        private static string ConfigLocalStorageModeKey = "[LOCALSTORAGEMODE]";
        private static string ConfigImageMiniatureHeightKey = "[IMAGEMINIATUREHEIGHT]";
        private static string ConfigImageMiniatureWidthKey = "[IMAGEMINIATUREWIDTH]";

        #endregion

        #region Reglement

        public static String ConfigReglement = "[CONFIGREGLEMENT]";

        #endregion

        #endregion

        #region Article

        private const string ConfigMajPoidsSynchroStockKey = "[MAJPOIDSSYNCHROSTOCK]";
        private const string ConfigLimiteStockConditionnementKey = "[LIMITESTOCKCONDITIONNEMENT]";
        private const string ConfigArticleFiltreDatePrixPrestashopKey = "[ARTICLEFILTREDATEPRIXPRESTASHOP]";

        private const string ConfigCombinationWithWeightConversionKey = "[COMBINATIONWITHWEIGHTCONVERSION]";
        private const string ConfigInformationLibreCoefficientConversionKey = "[INFORMATIONLIBRECOEFFICIENTCONVERSION]";

        private const string ConfigArticleEnSommeilKey = "[ARTICLESOMMEIL]";
        private const string ConfigArticleNonPublieSurLeWebKey = "[ARTICLEPUBLIEWEB]";

        private const string ConfigImportArticleStatutActifKey = "[IMPORTARTICLESTATUTACTIF]";
        private const string ConfigImportArticleRattachementParentsKey = "[IMPORTARTICLERATTACHEMENTPARENTS]";
        private const string ConfigImportArticleOnlyStandardProductsKey = "[IMPORTARTICLEONLYSTANDARDPRODUCTS]";

        private const string ConfigImportCatalogueAutoSelectionParentsKey = "[IMPORTCATALOGUEAUTOSELECTIONPARENTS]";
        private const string ConfigImportCatalogueAutoSelectionEnfantsKey = "[IMPORTCATALOGUEAUTOSELECTIONENFANTS]";

        private const string ConfigMarqueAutoStatistiqueActifKey = "[MARQUEAUTOSTATACTIF]";
        private const string ConfigMarqueAutoStatistiqueNameKey = "[MARQUEAUTOSTATNAME]";
        private const string ConfigMarqueAutoInfolibreActifKey = "[MARQUEAUTOINFOLIBREACTIF]";
        private const string ConfigMarqueAutoInfolibreNameKey = "[MARQUEAUTOINFOLIBRENAME]";

        private const string ConfigFournisseurAutoStatistiqueActifKey = "[FOURNISSAUTOSTATACTIF]";
        private const string ConfigFournisseurAutoStatistiqueNameKey = "[FOURNISSAUTOSTATNAME]";
        private const string ConfigFournisseurAutoInfolibreActifKey = "[FOURNISSAUTOINFOLIBREACTIF]";
        private const string ConfigFournisseurAutoInfolibreNameKey = "[FOURNISSAUTOINFOLIBRENAME]";

        private const string ConfigImportConditionnementActifKey = "[IMPORTCONDITIONNEMENTACTIF]";
        private const string ConfigConditionnementQuantiteToUPCKey = "[CONDITIONNEMENTQUANTITETOUPC]";

        private const string ConfigArticleContremarqueStockKey = "[ARTICLECONTREMARQUESTOCK]";
        private const string ConfigArticleInfolibrePackagingKey = "[ARTICLEINFOLIBREPACKAGING]";

        private const string ConfigDeleteCatalogProductAssociationKey = "[DELETECATALOGPRODUCTASSOCIATION]";

        private const string ConfigArticleDateDispoInfoLibreActifKey = "[ARTICLEDATEDISPOINFOLIBREACTIF]";
        private const string ConfigArticleDateDispoInfoLibreNameKey = "[ARTICLEDATEDISPOINFOLIBRENAME]";

        private const string ConfigArticleQuantiteMiniActifKey = "[ARTICLEQUANTITEMINIACTIF]";
        private const string ConfigArticleQuantiteMiniConditionnementKey = "[ARTICLEQUANTITEMINICONDITIONNEMENT]";
        private const string ConfigArticleQuantiteMiniUniteVenteKey = "[ARTICLEQUANTITEMINIUNITEVENTE]";

        private const string ConfigArticleTransfertInfosFournisseurActifKey = "[ARTICLETRANSFERTINFOSFOURNISSEURACTIF]";

        private const string ConfigArticleSuppressionAutoCaracteristiqueKey = "[ARTICLESUPPRESSIONAUTOCARACTERISTIQUE]";

        private const string ConfigArticleRedirectionCompositionActifKey = "[ARTICLEREDIRECTIONCOMPOSITIONACTIF]";

        private const string ConfigArticleStockNegatifZeroKey = "[ARTICLESTOCKNEGATIFZERO]";
        private const string ConfigArticleStockNegatifZeroParDepotKey = "[ARTICLESTOCKNEGATIFZEROPARDEPOT]";

        public const string ConfigArticleSpecificPriceLetBasePriceRuleKey = "[ARTICLESPECIFICPRICELETBASEPRICERULE]";

        #region Module AECStock

        private const string ConfigModuleAECStockActifKey = "[MODULEAECSTOCKACTIF]";

        #endregion

        #region Module AECAttributeList & AECAttributeStatut

        //private const string ConfigModuleAECAttributeListActifKey = "[MODULEAECATTRIBUTELISTACTIF]";
        //private const string ConfigModuleAECAttributeStatutActifKey = "[MODULEAECATTRIBUTESTATUTACTIF]";

		#endregion

		#region Module DWFProductGuiderates

		private const string ConfigModuleDWFProductGuideratesActifKey = "[MODULEDWFPRODUCTGUIDERATESACTIF]";

		#endregion

		#region Module DWFProductExtraFields

		private const string ConfigModuleDWFProductExtraFieldsActifKey = "[DWFPRODUCTEXTRAFIELDSACTIF]";

		#endregion		

		public static String ConfigArticleStock = "[ARTICLESTOCK]";
		public static String ConfigArticleContremarque = "[ARTICLECONTREMARQUE]";
		public static String ConfigArticleRupture = "[ARTICLERUPTURE]";

        public static String ConfigArticlePoidsType = "[ARTICLETYPE]";
        public static String ConfigArticlePoidsUnite = "[ARTICLEUNITE]";

        public static String ConfigArticleCatTarif = "[ARTICLECATTARIF]";

        public static String ConfigArticleTaxe = "[ARTICLETAXE]";

        private const string ConfigArticleCatComptableKey = "[ARTICLECATCOMPTABLE]";

        #endregion

        #region Taxe

        private const string ConfigTaxSageTVAKey = "[CONFIGTAXSAGETVA]";
        private const string ConfigTaxSageEcoKey = "[CONFIGTAXSAGEECO]";

        #endregion

        #region Client

        #region Création clients

        private const string ConfigClientFiltreCommandeKey = "[CLIENTFILTRECOMMANDE]";

        // <JG> 05/08/2013 gestion composition numérotation client
        private const string ConfigClientNumCompositionKey = "[CLIENTNUMCOMPOSITION]";
        private const string ConfigClientNumPrefixeKey = "[CLIENTPREFIXE]";
        private const string ConfigClientNumLongueurNomKey = "[CLIENTNUMLONGUEURNOM]";
        private const string ConfigClientNumTypeNomKey = "[CLIENTNUMTYPENOM]";
        private const string ConfigClientNumLongueurNumeroKey = "[CLIENTLONGUEURNUMEROCLIENT]";
        private const string ConfigClientNumTypeNumeroKey = "[CLIENTNUMTYPENUMERO]";
        private const string ConfigClientNumTypeCompteurKey = "[CLIENTNUMTYPECOMPTEUR]";
        private const string ConfigClientNumDebutCompteurKey = "[CLIENTNUMDEBUTCOMPTEUR]";

        private const string ConfigClientNumDepartementRemplacerCodeISOKey = "[CLIENTNUMDEPARTEMENTREMPLACERCODEISO]";

        private const string ConfigClientMultiMappageBtoBKey = "[CLIENTMULTIMAPPAGEBTOB]";

        private const string ConfigClientCiviliteActifKey = "[CLIENTCIVILITEACTIF]";

        private const string ConfigClientSocieteIntituleActifKey = "[CLIENTSOCIETEINTITULEACTIF]";
        private const string ConfigClientNIFActifKey = "[CLIENTNIFACTIF]";
        private const string ConfigClientInfosMajusculeActifKey = "[CLIENTINFOSMAJUSCULEACTIF]";

        // <JG> 08/11/2012
        public static String ConfigClientTypeLien = "[CLIENTTYPELIEN]";
        public enum ConfigClientTypeLienEnum { ComptesIndividuels, CompteCentralisateur };

        public static String ConfigClientCompteCentralisateur = "[CLIENTCENTRALISATEUR]";
        //END

        public static String ConfigClientCompteGeneral = "[CLIENTCOMPTEGENERAL]";
        public static String ConfigClientCompteComptable = "[CLIENTCOMPTECOMPTABLE]";
        public static String ConfigClientCategorieTarifaire = "[CLIENTCATEGORIETARIFAIRE]";

        public static String ConfigClientPeriodicite = "[CLIENTPERIODICITE]";
        public static String ConfigClientCodeRisque = "[CLIENTCODERISQUE]";
        public static String ConfigClientNombreLigne = "[CLIENTNOMBRELIGNE]";
        public static String ConfigClientSaut = "[CLIENTSAUT]";
        public static String ConfigClientLettrage = "[CLIENTLETTRAGE]";
        public static String ConfigClientPrioriteLivraison = "[CLIENTPRIORITELIVRAISON]";
        public static String ConfigClientCollaborateur = "[CLIENTCOLLABORATEUR]";
        public static String ConfigClientDepot = "[CLIENTDEPOT]";
        //public static String ConfigClientAnalytique = "[CLIENTANALYTIQUE]";
        public static String ConfigClientCodeAffaire = "[CLIENTCODEAFFAIRE]";
        public static String ConfigClientDevise = "[CLIENTDEVISE]";
        public static String ConfigClientLangue = "[CLIENTLANGUE]";

        public static String ConfigClientBLFacture = "[CLIENTBLFACTURE]";
        public static String ConfigClientLivraisonPartielle = "[CLIENTLIVRAISONPARTIELLE]";
        public static String ConfigClientValidationAutomatique = "[CLIENTVALIDATIONAUTOMATIQUE]";
        public static String ConfigClientRappel = "[CLIENTRAPPEL]";
        public static String ConfigClientPenalite = "[CLIENTPENALITE]";
        public static String ConfigClientSurveillance = "[CLIENTSURVEILLANCE]";


        public static String ConfigClientConditionLivraison = "[CLIENTCONDITIONLIVRAISON]";
        public static String ConfigClientModeExpedition = "[CLIENTMODEEXPEDITION]";
        // <JG> 07/09/2012
        public static String ConfigClientIntituleAdresse = "[CLIENTINTITULEADRESSE]";
        public enum ConfigClientIntituleAdresseEnum { CodePrestashop, NomPrenomPrestashop };

        #endregion

        #region TransfertClient

        // <JG> 07/09/2012 - 27/02/2013 déplacement clé cookie
        private const string ConfigTransfertPrestashopCookieKeyKey = "[CLIENTPRESTASHOPCOOKIEKEY]";

        private const string ConfigTransfertPriceCategoryAvailableKey = "[CONFIGTRANSFERTPRICECATEGORYAVAILABLE]";
        private const string ConfigTransfertMailAccountKey = "[CONFIGTRANSFERTMAILACCOUNT]";
        private const string ConfigTransfertMailAccountAlternativeKey = "[CONFIGTRANSFERTMAILACCOUNTALTERNATIVE]";
        private const string ConfigTransfertMailAccountContactService = "[CONFIGTRANSFERTMAILACCOUNTCONTACTSERVICE]";
        private const string ConfigTransfertRandomPasswordLengthKey = "[CONFIGTRANSFERTRANDOMPASSWORDLENGTH]";
        private const string ConfigTransfertRandomPasswordIncludeSpecialCharactersKey = "[CONFIGTRANSFERTRANDOMPASSWORDINCLUDESPECIALCHARACTERS]";

        private const string ConfigTransfertNotifyAccountAddressKey = "[CONFIGTRANSFERTNOTIFYACCOUNTADDRESS]";
        private const string ConfigTransfertNotifyAccountAdminCopyKey = "[CONFIGTRANSFERTNOTIFYACCOUNTADMINCOPY]";
        private const string ConfigTransfertNotifyAccountDeliveryMethodKey = "[CONFIGTRANSFERTNOTIFYACCOUNTDELIVERYMETHOD]";
        private const string ConfigTransfertNotifyAccountSageContactTypeKey = "[CONFIGTRANSFERTNOTIFYACCOUNTSAGECONTACTTYPE]";
        private const string ConfigTransfertNotifyAccountSageContactServiceKey = "[CONFIGTRANSFERTNOTIFYACCOUNTSAGECONTACTSERVICE]";

        private const string ConfigTransfertSageAddressSendKey = "[CONFIGTRANSFERTSAGEADDRESSSEND]";
        private const string ConfigTransfertLockPhoneNumberKey = "[CONFIGTRANSFERTLOCKPHONENUMBER]";
        private const string ConfigTransfertLockPhoneNumberReplaceEntryValueKey = "[CONFIGTRANSFERTLOCKPHONENUMBERREPLACEENTRYVALUE]";
        private const string ConfigTransfertAliasValueKey = "[CONFIGTRANSFERTALIASVALUE]";
        private const string ConfigTransfertLastNameValueKey = "[CONFIGTRANSFERTLASTNAMEVALUE]";
        private const string ConfigTransfertFirstNameValueKey = "[CONFIGTRANSFERTFIRSTNAMEVALUE]";
        private const string ConfigTransfertCompanyValueKey = "[CONFIGTRANSFERTCOMPANYVALUE]";
        private const string ConfigTransfertClientSeparateurIntituleKey = "[CONFIGTRANSFERTENTITLEDSEPARATOR]";

        private const string ConfigTransfertAccountActivationKey = "[CONFIGTRANSFERTACCOUNTACTIVATION]";
        private const string ConfigTransfertNewsLetterSuscribeKey = "[CONFIGTRANSFERTNEWSLETTERSUSCRIBE]";
        private const string ConfigTransfertOptInSuscribeKey = "[CONFIGTRANSFERTOPTINSUSCRIBE]";

        private const string ConfigTransfertIntegrateCustomerSynchronizationProcessKey = "[CONFIGTRANSFERTINTEGRATECUSTOMERSYNCHRONIZATIONPROCESS]";
        private const string ConfigTransfertSendAdminResultReportKey = "[CONFIGTRANSFERTSENDADMINRESULTREPORT]";
        private const string ConfigTransfertGenerateAccountFileKey = "[CONFIGTRANSFERTGENERATEACCOUNTFILE]";

        private const string ConfigRegexMailLevelKey = "[REGEXMAILLEVEL]";

        private const string ConfigTransfertNameIncludeNumbersKey = "[CONFIGTRANSFERTNAMEINCLUDEENUMBERS]";

        #endregion

        #region Statistiques et informations libres client

        private const string ConfigStatInfolibreClientActifKey = "[STATINFOLIBRECLIENTACTIF]";

        #endregion

        #region Module AECCollaborateur

        private const string ConfigModuleAECCollaborateurActifKey = "[MODULEAECCOLLABORATEURACTIF]";

        #endregion

        #region Module AECPaiement

        private const string ConfigModuleAECPaiementActifKey = "[MODULEAECPAIEMENTACTIF]";

        #endregion

        #region Module AECCustomerOutstanding

        private const string ConfigModuleAECCustomerOutstandingActifKey = "[MODULEAECCUSTOMEROUTSTANDINGACTIF]";

        #endregion

        #region Module AECCustomerInfo

        private const string ConfigModuleAECCustomerInfoActifKey = "[MODULEAECCUSTOMERINFOACTIF]";

        #endregion

        #region Module Portfolio Customer Employee

        public const string ConfigModulePortfolioCustomerEmployeeActifKey = "[MODULEPORTFOLIOCUSTOMEREMPLOYEEACTIF]";

        #endregion

        #endregion

        #region Adresse

        public const string ConfigClientAdresseTelephonePositionFixeKey = "[CLIENTADRESSETELEPHONEPOSITIONFIXE]";

        #endregion
		
        #region Commandes

        #region Filtres PrestaShop

        private const string ConfigCommandeFiltreDateKey = "[COMMANDEFILTREDATE]";

        #endregion

        #region Entete document Sage

        public static String ConfigCommandeDepot = "[COMMANDEDEPOT]";
        public static String ConfigCommandeSouche = "[COMMANDESOUCHE]";
        public static String ConfigCommandeStatut = "[COMMANDESTATUT]";

        public const string ConfigCommandeUpdateAdresseFacturationKey = "[COMMANDEUPDATEADRESSEFACTURATION]";
        public const string ConfigCommandeInsertFacturationEnteteKey = "[COMMANDEINSERTFACTURATIONENTETE]";

        public const string ConfigCommandeReferencePrestaShopKey = "[COMMANDEREFERENCEPRESTASHOP]";

        #endregion

        #region Gestion statuts Prestashop

        public static String ConfigCommandeSynchro = "[CONFIGCOMMANDESYNCHRO]";
        public static String ConfigCommandePayment = "[CONFIGCOMMANDESPAYMENT]";
        public static String ConfigCommandeRelance = "[CONFIGCOMMANDESRELANCE]";
        public static String ConfigCommandeAnnulation = "[CONFIGCOMMANDESANNULATION]";

        public const string ConfigCommandeNumeroFactureSageForceActifKey = "[COMMANDENUMEROFACTURESAGEFORCEACTIF]";
        public const string ConfigCommandeStatutJoursAutomateKey = "[COMMANDESTATUTJOURSAUTOMATE]";

        #endregion

        #region Tracking

        public const string ConfigCommandeTrackingEnteteActifKey = "[CONFIGCOMMANDETRACKINGENTETEACTIFKEY]";
        public const string ConfigCommandeTrackingInfolibreActifKey = "[CONFIGCOMMANDETRACKINGINFOLIBREACTIFKEY]";
        public const string ConfigCommandeTrackingEnteteKey = "[CONFIGCOMMANDETRACKINGENTETEKEY]";
        public const string ConfigCommandeTrackingInfolibreKey = "[CONFIGCOMMANDETRACKINGINFOLIBREKEY]";

        #endregion

        #region Mise à jour statuts

        public const String ConfigCommandeDEKey = "[CONFIGCOMMANDEDE]";
        public const String ConfigCommandeBCKey = "[CONFIGCOMMANDEBC]";
        public const String ConfigCommandePLKey = "[CONFIGCOMMANDEPL]";
        public const String ConfigCommandeBLKey = "[CONFIGCOMMANDEBL]";
        public const String ConfigCommandeFAKey = "[CONFIGCOMMANDEFA]";
        public const String ConfigCommandeFCKey = "[CONFIGCOMMANDEFC]";

        #endregion

        private const string ConfigLigneRemiseModeKey = "[CONFIGLIGNEREMISEMODE]";

		#region Frais de port

		private const string ConfigLigneFraisPortKey = "[CONFIGLIGNEFRAISPORT]";
		private const string ConfigLigneArticlePortKey = "[CONFIGLIGNEARTICLEPORT]";

		#endregion

		#region Date de livraison

		private const string ConfigDateLivraisonMode = "[CONFIGDATELIVRAISON]";
		private const string ConfigDateLivraisonJours = "[CONFIGDATELIVRAISONJOURS]";

		#endregion

		#region Bons de réductions

		private const string ConfigCommandeArticleReductionKey = "[CONFIGCOMMANDEARTICLEREDUCTION]";

        #endregion

        #region Commentaires

        private const string ConfigCommentaireBoutiqueActifKey = "[COMMENTAIREBOUTIQUEACTIF]";
        private const string ConfigCommentaireBoutiqueTexteKey = "[COMMENTAIREBOUTIQUETEXTE]";
        private const string ConfigCommentaireNumeroActifKey = "[COMMENTAIRENUMEROACTIF]";
        private const string ConfigCommentaireNumeroTexteKey = "[COMMENTAIRENUMEROTEXTE]";
        private const string ConfigCommentaireReferencePaiementActifKey = "[COMMENTAIREREFERENCEPAIEMENTACTIF]";
        private const string ConfigCommentaireReferencePaiementTexteKey = "[COMMENTAIREREFERENCEPAIEMENTTEXTE]";
        private const string ConfigCommentaireDateActifKey = "[COMMENTAIREDATEACTIF]";
        private const string ConfigCommentaireDateTexteKey = "[COMMENTAIREDATETEXTE]";

        private const string ConfigCommentaireClientActifKey = "[COMMENTAIRECLIENTACTIF]";
        private const string ConfigCommentaireClientTexteKey = "[COMMENTAIRECLIENTTEXTE]";
        private const string ConfigCommentaireAdresseFacturationActifKey = "[COMMENTAIREADRESSEFACTURATIONACTIF]";
        private const string ConfigCommentaireAdresseFacturationTexteKey = "[COMMENTAIREADRESSEFACTURATIONTEXTE]";
        private const string ConfigCommentaireAdresseLivraisonActifKey = "[COMMENTAIREADRESSELIVRAISONACTIF]";
        private const string ConfigCommentaireAdresseLivraisonTexteKey = "[COMMENTAIREADRESSELIVRAISONTEXTE]";

        private const string ConfigCommentaireLibre1ActifKey = "[COMMENTAIRELIBRE1ACTIF]";
        private const string ConfigCommentaireLibre1TexteKey = "[COMMENTAIRELIBRE1TEXTE]";
        private const string ConfigCommentaireLibre2ActifKey = "[COMMENTAIRELIBRE2ACTIF]";
        private const string ConfigCommentaireLibre2TexteKey = "[COMMENTAIRELIBRE2TEXTE]";
        private const string ConfigCommentaireLibre3ActifKey = "[COMMENTAIRELIBRE3ACTIF]";
        private const string ConfigCommentaireLibre3TexteKey = "[COMMENTAIRELIBRE3TEXTE]";

        private const string ConfigCommentaireDebutDocumentKey = "[COMMENTAIREDEBUTDOCUMENT]";
        private const string ConfigCommentaireFinDocumentKey = "[COMMENTAIREFINDOCUMENT]";

        #endregion

        #region Relance

        private const string ConfigDureeJourAvantPremiereRelanceKey = "[RELANCEDUREE1]";
        private const string ConfigDureeJourApresPremiereRelanceKey = "[RELANCEDUREE2]";
        private const string ConfigDureeJourApresDeuxiemeRelanceKey = "[RELANCEDUREE3]";
        private const string ConfigDureeJourAnnulationApresDerniereRelanceKey = "[COMMANDEANNULATION]";

        #endregion

        #region Module OleaPromo

        private const string ConfigModuleOleaPromoActifKey = "[MODULEOLEAPROMOACTIF]";
        private const string ConfigModuleOleaSuffixeGratuitKey = "[MODULEOLEASUFFIXEGRATUIT]";

        #endregion

        #region Module Preorder

        private const string ConfigModulePreorderActifKey = "[MODULEPREORDERACTIF]";
        private const string ConfigModulePreorderInfolibreNameKey = "[MODULEPREORDERINFOLIBRENAME]";
        private const string ConfigModulePreorderInfolibreValueKey = "[MODULEPREORDERINFOLIBREVALUE]";
        private const string ConfigModulePreorderPrestashopProductKey = "[MODULEPREORDERPRESTASHOPPRODUCT]";
        private const string ConfigModulePreorderPrestashopOrderStateKey = "[MODULEPREORDERPRESTASHOPORDERSTATE]";

        private const string ConfigModulePreorderInfolibreEnteteNameKey = "[MODULEPREORDERINFOLIBREENTETENAME]";
        private const string ConfigModulePreorderInfolibreEnteteValueKey = "[MODULEPREORDERINFOLIBREENTETEVALUE]";

        #endregion

        #region Module AECInvoiceHistory

        private const string ConfigModuleAECInvoiceHistoryActifKey = "[MODULEAECINVOICEHISTORYACTIF]";
        private const string ConfigModuleAECInvoiceHistoryInfoLibreClientSendMailKey = "[MODULEAECINVOICEHISTORYINFOLIBRECLIENTSENDMAIL]";
        private const string ConfigModuleAECInvoiceHistoryInfolibreClientSendMailValueKey = "[MODULEAECINVOICEHISTORYINFOLIBRECLIENTSENDMAILVALUE]";

        private const string ConfigModuleAECInvoiceHistoryArchivePDFActiveKey = "[MODULEAECINVOICEHISTORYARCHIVEPDFACTIVE]";
        private const string ConfigModuleAECInvoiceHistoryArchivePDFFolderKey = "[MODULEAECINVOICEHISTORYARCHIVEPDFFOLDER]";

        #endregion

        #region Module SoColissimo

        private const string ConfigModuleSoColissimoDeliveryActiveKey = "[ModuleSoColissimoDeliveryActive]";
        private const string ConfigModuleSoColissimoInfolibreTypePointActiveKey = "[ModuleSoColissimoInfolibreTypePointActive]";
        private const string ConfigModuleSoColissimoInfolibreEnteteTypePointNameKey = "[ModuleSoColissimoInfolibreEnteteTypePointName]";
        private const string ConfigModuleSoColissimoInfolibreDestinataireActiveKey = "[ModuleSoColissimoInfolibreDestinataireActive]";
        private const string ConfigModuleSoColissimoInfolibreEnteteDestinataireNameKey = "[ModuleSoColissimoInfolibreEnteteDestinataireName]";
        private const string ConfigModuleSoColissimoReplacePhoneActiveKey = "[ModuleSoColissimoReplacePhoneActive]";
        private const string ConfigModuleSoColissimoReplaceAddressNameActiveKey = "[ModuleSoColissimoReplaceAddressNameActive]";

        #endregion

        private const string ConfigCommandeArticlePackagingKey = "[COMMANDEARTICLEPACKAGING]";

        #endregion

        #region Reglement

        private const string ConfigSyncReglementActifKey = "[CONFIGREGLEMENT]";
        private const string ConfigModeReglementEcheancierActifKey = "[MODEREGLEMENTECHEANCIERACTIF]";
        private const string ConfigReglementLibellePartielActifKey = "[REGLEMENTLIBELLEPARTIELACTIF]";

        #endregion

        #region Mail

        #region Identification

        public const string ConfigMailActiveKey = "[CONFIGMAILACTIVE]";
        public const string ConfigMailUserKey = "[CONFIGMAILUSER]";
        public const string ConfigMailPasswordKey = "[CONFIGMAILPASSWORD]";
        public const string ConfigMailSMTPKey = "[CONFIGMAILSMTP]";
        public const string ConfigMailPortKey = "[CONFIGMAILPORT]";
        public const string ConfigMailSSLKey = "[CONFIGMAILSSL]";

        #endregion

        #region Paramètres contenu

        public const string MailOrderId = "[ORDERID]";
        public const string MailOrderFirstName = "[ORDERFIRSTNAME]";
        public const string MailOrderLastName = "[ORDERLASTNAME]";
        public const string MailOrderAddress = "[ORDERADDRESS]";
        public const string MailOrderPostCode = "[ORDERPOSTCODE]";
        public const string MailOrderCity = "[ORDERCITY]";
        public const string MailOrderTotalPaidTTC = "[ORDERTOTALPAIDTTC]";
        public const string MailOrderTotalPaidHT = "[ORDERTOTALPAIDHT]";
        public const string MailOrderDate = "[ORDERDATE]";

        public const string MailAccountLastName = "[CUSTOMERLASTNAME]";
        public const string MailAccountFirstName = "[CUSTOMERFIRSTNAME]";
        public const string MailAccountPassword = "[CUSTOMERPASSWORD]";

        #endregion

        #endregion

        #region Import

        private const string ConfigAutomaticImportFolderPictureKey = "[CONFIGAUTOMATICIMPORTFOLDERPICTURE]";
        private const string ConfigAutomaticImportFolderDocumentKey = "[CONFIGAUTOMATICIMPORTFOLDERDOCUMENT]";
        private const string ConfigAutomaticImportFolderMediaKey = "[CONFIGAUTOMATICIMPORTFOLDERMEDIA]";
        private const string ConfigImportMediaIncludePicturesKey = "[IMPORTMEDIAINCLUDEPICTURES]";
        private const string ConfigImportMediaAutoDeleteAttachmentKey = "[IMPORTMEDIAAUTODELETEATTACHMENT]";
        private const string ConfigImportImageReplaceFilesKey = "[IMPORTIMAGEREPLACEFILES]";
        private const string ConfigImportImageRemoveDeletedFilesKey = "[IMPORTIMAGEREMOVEDELETEDFILES]";

        private const string ConfigImportImageSearchReferenceClientKey = "[IMPORTIMAGESEARCHREFERENCECLIENT]";
        private const string ConfigImportImageUseSageDatasKey = "[IMPORTIMAGEUSESAGEDATAS]";

        #endregion

        #region Outils

        private const string ConfigCronSynchroArticleURLKey = "[CRONSYNCHROARTICLEURL]";

        private const string ConfigRefreshTempCustomerListDisabledKey = "[REFRESHTEMPCUSTOMERLISTDISABLED]";

        private const string ConfigUnlockProcessorCoreKey = "[UNLOCKPROCESSORCORE]";
        private const string ConfigAllocatedProcessorCoreKey = "[ALLOCATEDPROCESSORCORE]";

        private const string ConfigCrystalForceConnectionInfoOnSubReportsKey = "[CRYSTALFORCECONNECTIONINFOONSUBREPORTS]";

        #endregion

        #region Interface/UI

        private const string UIProductFilterActiveDefaultKey = "[UIPRODUCTFILTERACTIVEDEFAULT]";
        private const string UIProductUpdateValidationDisabledKey = "[UIPRODUCTUPDATEVALIDATIONDISABLED]";

        private const string UIMaximizeWindowKey = "[UIMAXIMIZEWINDOW]";

        private const string UISleepTimeWYSIWYGKey = "[UISLEEPTIMEWYSIWYG]";
        private const string UIDisabledWYSIWYGKey = "[UIDISABLEDWYSIWYG]";

        private const string UIIE11EmulationModeDisabledKey = "[UIIE11EMULATIONMODEDISABLED]";

        // <JG> 03/06/2016
        private const string ListeArticleColonneIDVisibleKey = "[LISTEARTICLECOLONNEIDVISIBLE]";
        private const string ListeArticleColonneTypeVisibleKey = "[LISTEARTICLECOLONNETYPEVISIBLE]";
        private const string ListeArticleColonneNomVisibleKey = "[LISTEARTICLECOLONNENOMVISIBLE]";
        private const string ListeArticleColonneReferenceVisibleKey = "[LISTEARTICLECOLONNEREFERENCEVISIBLE]";
        private const string ListeArticleColonneEANVisibleKey = "[LISTEARTICLECOLONNEEANVISIBLE]";
        private const string ListeArticleColonneActifVisibleKey = "[LISTEARTICLECOLONNEACTIFVISIBLE]";
        private const string ListeArticleColonneSyncVisibleKey = "[LISTEARTICLECOLONNESYNCVISIBLE]";
        private const string ListeArticleColonneSyncPriceVisibleKey = "[LISTEARTICLECOLONNESYNCPRICEVISIBLE]";
        private const string ListeArticleColonneDateVisibleKey = "[LISTEARTICLECOLONNEDATEVISIBLE]";

        #endregion

        #region ReimporSage

        private const string ReimportUpdateMainCatalogKey = "[REIMPORTUPDATEMAINCATALOG]";
        private const string ReimportLinkParentsKey = "[REIMPORTLINKPARENTS]";
        private const string ReimportDeleteLinkOldMainKey = "[REIMPORTDELETELINKOLDMAIN]";
        private const string ReimportDeleteLinkOldSecondaryKey = "[REIMPORTDELETELINKOLDSECONDARY]";

        private const string ReimportUpdateProductNameKey = "[REIMPORTUPDATEPRODUCTNAME]";
        private const string ReimportUpdateDescriptionShortKey = "[REIMPORTUPDATEDESCRIPTIONSHORT]";
        private const string ReimportUpdateDescriptionKey = "[REIMPORTUPDATEDESCRIPTION]";
        private const string ReimportUpdateMetaTitleKey = "[REIMPORTUPDATEMETATITLE]";
        private const string ReimportUpdateMetaDescriptionKey = "[REIMPORTUPDATEMETADESCRIPTION]";
        private const string ReimportUpdateMetaKeywordsKey = "[REIMPORTUPDATEMETAKEYWORDS]";
        private const string ReimportUpdateURLKey = "[REIMPORTUPDATEURL]";
        private const string ReimportUpdateEANKey = "[REIMPORTUPDATEEAN]";
        private const string ReimportUpdateActiveKey = "[REIMPORTUPDATEACTIVE]";

        private const string ReimportUpdateCharacteristicKey = "[REIMPORTUPDATECHARACTERISTIC]";
        private const string ReimportUpdateAttributeKey = "[REIMPORTUPDATEATTRIBUTE]";
        private const string ReimportUpdateConditioningKey = "[REIMPORTUPDATECONDITIONING]";

        private const string ReimportUpdateDateActiveKey = "[REIMPORTUPDATEDATEACTIVE]";

        #endregion

        #region Chronos

        private const string ChronoSynchroStockPriceActifKey = "[CHRONOSYNCHROSTOCKPRICEACTIF]";

        #endregion

        #region Cron/Scripts

        private const string CronArticleURLKey = "[CRONARTICLEURL]";
        private const string CronArticleBaliseKey = "[CRONARTICLEBALISE]";
        private const string CronArticleTimeoutKey = "[CRONARTICLETIMEOUT]";

		private const string CronCommandeURLKey = "[CRONCOMMANDEURL]";
		private const string CronCommandeBaliseKey = "[CRONCOMMANDEBALISE]";
		private const string CronCommandeTimeoutKey = "[CRONCOMMANDETIMEOUT]";

		#endregion

        #endregion

        #region Get/Set

        #region Avancé

        public AppFolders Folders { get; private set; }

        #endregion

        #region General

        public string AdminMailAddress { get; private set; }

        public bool ConfigBToB { get; private set; }
        public bool ConfigBToC { get; private set; }

        public RemiseMode ModeRemise { get; private set; }
        public RemiseConflit ConflitRemise { get; private set; }

        public bool ConfigFTPActive { get; private set; }
        public string ConfigFTPIP { get; private set; }
        public string ConfigFTPUser { get; private set; }
        public string ConfigFTPPassword { get; private set; }
        public bool ConfigFTPSSL { get; private set; }

        #region Images

        public ImageStorageMode ConfigImageStorageMode { get; private set; }
        public LocalStorageMode ConfigLocalStorageMode { get; private set; }
        public int ConfigImageMiniatureHeight { get; private set; }
        public int ConfigImageMiniatureWidth { get; private set; }
        public bool ConfigImageSynchroPositionLegende { get; private set; }

        #endregion

        #endregion

        #region Article

        public bool MajPoidsSynchroStock { get; private set; }
        public bool LimiteStockConditionnement { get; private set; }
        public bool ArticleFiltreDatePrixPrestashop { get; private set; }

        public List<int> CombinationWithWeightConversion { get; private set; }
        public string InformationLibreCoefficientConversion { get; private set; }

        public bool ArticleEnSommeil { get; private set; }
        public bool ArticleNonPublieSurLeWeb { get; private set; }

        public bool ImportArticleStatutActif { get; private set; }
        public bool ImportArticleRattachementParents { get; private set; }
        public bool ImportArticleOnlyStandardProducts { get; private set; }

        public bool ImportCatalogueAutoSelectionParents { get; private set; }
        public bool ImportCatalogueAutoSelectionEnfants { get; private set; }

        public bool MarqueAutoStatistiqueActif { get; private set; }
        public string MarqueAutoStatistiqueName { get; private set; }
        public bool MarqueAutoInfolibreActif { get; private set; }
        public string MarqueAutoInfolibreName { get; private set; }

        public bool FournisseurAutoStatistiqueActif { get; private set; }
        public string FournisseurAutoStatistiqueName { get; private set; }
        public bool FournisseurAutoInfolibreActif { get; private set; }
        public string FournisseurAutoInfolibreName { get; private set; }

        public bool ArticleImportConditionnementActif { get; private set; }
        public bool ArticleConditionnementQuantiteToUPC { get; private set; }

        public bool ArticleContremarqueStockActif { get; private set; }
        public string ArticleInfolibrePackaging { get; private set; }

        public bool DeleteCatalogProductAssociation { get; private set; }

        public bool ArticleDateDispoInfoLibreActif { get; private set; }
        public string ArticleDateDispoInfoLibreName { get; private set; }

        public bool ArticleQuantiteMiniActif { get; private set; }
        public bool ArticleQuantiteMiniConditionnement { get; private set; }
        public bool ArticleQuantiteMiniUniteVente { get; private set; }

        public bool ArticleTransfertInfosFournisseurActif { get; private set; }

        public bool ArticleSuppressionAutoCaracteristique { get; private set; }

        public bool ArticleRedirectionCompositionActif { get; private set; }

        public bool ArticleStockNegatifZero { get; private set; }
        public bool ArticleStockNegatifZeroParDepot { get; private set; }

        public bool ArticleSpecificPriceLetBasePriceRule { get; private set; }

        #region Module AECStock

        public bool ModuleAECStockActif { get; private set; }

        #endregion

        #region Module AECAttributeList & AECAttributeStatut

        public bool ModuleAECAttributeListActif { get; private set; }
        public bool ModuleAECAttributeStatutActif { get; private set; }

        #endregion

        public int ConfigArticleCatComptable { get; private set; }

		#region Module DWFProductGuiderates

		public bool ModuleDWFProductGuideratesActif { get; private set; }

		#endregion

		#region Module DWFProductExtraFields

		public bool ModuleDWFProductExtraFieldsActif { get; private set; }

		#endregion

        #endregion

        #region Taxes

        public TaxSage TaxSageTVA { get; private set; }
        public TaxSage TaxSageEco { get; private set; }

        #endregion

        #region Client

        #region Création clients

        public Boolean ConfigClientFiltreCommande { get; private set; }

        public string ConfigClientNumComposition { get; private set; }
        public string ConfigClientNumPrefixe { get; private set; }
        public int ConfigClientNumLongueurNom { get; private set; }
        public NameNumComponent ConfigClientNumTypeNom { get; private set; }
        public int ConfigClientNumLongueurNumero { get; private set; }
        public NumberNumComponent ConfigClientNumTypeNumero { get; private set; }
        public CounterType ConfigClientNumTypeCompteur { get; private set; }
        public int ConfigClientNumDebutCompteur { get; private set; }

        public bool ConfigClientNumDepartementRemplacerCodeISO { get; private set; }

        public bool ConfigClientMultiMappageBtoB { get; private set; }

        public bool ConfigClientCiviliteActif { get; private set; }

        public bool ConfigClientSocieteIntituleActif { get; private set; }
        public bool ConfigClientNIFActif { get; private set; }
        public bool ConfigClientInfosMajusculeActif { get; private set; }

        #endregion

        #region TransfertClient

        public string TransfertPrestashopCookieKey { get; private set; }

        public List<int> TransfertPriceCategoryAvailable { get; private set; }
        public MailAccountIdentification TransfertMailAccountIdentification { get; private set; }
        public bool TransfertMailAccountAlternative { get; private set; }
        public int TransfertMailAccountContactService { get; private set; }
        public int TransfertRandomPasswordLength { get; private set; }
        public bool TransfertRandomPasswordIncludeSpecialCharacters { get; private set; }

        public MailNotification TransfertNotifyAccountAddress { get; private set; }
        public bool TransfertNotifyAccountAdminCopy { get; private set; }
        public DeliveryMethod TransfertNotifyAccountDeliveryMethod { get; private set; }
        public List<int> TransfertNotifyAccountSageContactType { get; private set; }
        public List<int> TransfertNotifyAccountSageContactService { get; private set; }

        public List<int> TransfertSageAddressSend { get; private set; }
        public List<int> TransfertLockPhoneNumber { get; private set; }
        public string TransfertLockPhoneNumberReplaceEntryValue { get; private set; }
        public AliasValue TransfertAliasValue { get; private set; }
        public LastNameValue TransfertLastNameValue { get; private set; }
        public FirstNameValue TransfertFirstNameValue { get; private set; }
        public CompanyValue TransfertCompanyValue { get; private set; }
        public string TransfertClientSeparateurIntitule { get; private set; }

        public bool TransfertAccountActivation { get; private set; }
        public bool TransfertNewsLetterSuscribe { get; private set; }
        public bool TransfertOptInSuscribe { get; private set; }

        public bool TransfertIntegrateCustomerSynchronizationProcess { get; private set; }
        public bool TransfertSendAdminResultReport { get; private set; }
        public bool TransfertGenerateAccountFile { get; private set; }

        public RegexMail RegexMailLevel { get; private set; }

        public bool TransfertNameIncludeNumbers { get; private set; }

        #endregion

        #region Statistiques et informations libres client

        public bool StatInfolibreClientActif { get; private set; }

        #endregion

        #region Module AECCollaborateur

        public bool ModuleAECCollaborateurActif { get; private set; }

        #endregion
        #region Module AECPaiement

        public bool ModuleAECPaiementActif { get; private set; }

        #endregion
        #region Module AECCustomerOutstanding

        public bool ModuleAECCustomerOutstandingActif { get; private set; }

        #endregion
        #region Module AECCustomerInfo

        public bool ModuleAECCustomerInfoActif { get; private set; }

        #endregion

        #region Module Portfolio Customer Employee

        public bool ModulePortfolioCustomerEmployeeActif { get; private set; }

        #endregion

        #endregion

        #region Adresses

        public bool ConfigClientAdresseTelephonePositionFixe { get; private set; }

        #endregion

        #region Commandes

        #region Filtres Prestashop

        public DateTime? ConfigCommandeFiltreDate { get; private set; }

        #endregion

        #region Statuts

        public int ConfigCommandeDE { get; private set; }
        public int ConfigCommandeBC { get; private set; }
        public int ConfigCommandePL { get; private set; }
        public int ConfigCommandeBL { get; private set; }
        public int ConfigCommandeFA { get; private set; }
        public int ConfigCommandeFC { get; private set; }

        #endregion

        public LigneRemiseMode LigneRemiseMode { get; private set; }

        public bool CommandeUpdateAdresseFacturation { get; private set; }
        public bool CommandeInsertFacturationEntete { get; private set; }

        public bool CommandeReferencePrestaShop { get; private set; }

        public bool CommandeNumeroFactureSageForceActif { get; private set; }

        public int CommandeStatutJoursAutomate { get; private set; }

        public bool CommandeTrackingEnteteActif { get; private set; }
        public bool CommandeTrackingInfolibreActif { get; private set; }
        public FieldDocumentEntete CommandeTrackingEntete { get; private set; }
        public string CommandeTrackingInfolibre { get; private set; }

		#region Frais de port

		public bool LigneFraisPort { get; private set; }
		public string LigneArticlePort { get; private set; }

		#endregion

		#region Date de livraison

		public DateLivraisonMode DateLivraisonMode { get; private set; }
		public int DateLivraisonJours { get; private set; }

		#endregion

		#region Bons de réductions

		public string CommandeArticleReduction { get; private set; }

        #endregion

        #region Commentaires

        public bool CommentaireBoutiqueActif { get; private set; }
        public string CommentaireBoutiqueTexte { get; private set; }
        public bool CommentaireNumeroActif { get; private set; }
        public string CommentaireNumeroTexte { get; private set; }
        public bool CommentaireReferencePaiementActif { get; private set; }
        public string CommentaireReferencePaiementTexte { get; private set; }
        public bool CommentaireDateActif { get; private set; }
        public string CommentaireDateTexte { get; private set; }

        public bool CommentaireClientActif { get; private set; }
        public string CommentaireClientTexte { get; private set; }
        public bool CommentaireAdresseFacturationActif { get; private set; }
        public string CommentaireAdresseFacturationTexte { get; private set; }
        public bool CommentaireAdresseLivraisonActif { get; private set; }
        public string CommentaireAdresseLivraisonTexte { get; private set; }

        public bool CommentaireLibre1Actif { get; private set; }
        public string CommentaireLibre1Texte { get; private set; }
        public bool CommentaireLibre2Actif { get; private set; }
        public string CommentaireLibre2Texte { get; private set; }
        public bool CommentaireLibre3Actif { get; private set; }
        public string CommentaireLibre3Texte { get; private set; }

        public bool CommentaireDebutDocument { get; private set; }
        public bool CommentaireFinDocument { get; private set; }

        #endregion

        #region Relance / Annulation

        public int DureeJourAvantPremiereRelance { get; private set; }
        public int DureeJourApresPremiereRelance { get; private set; }
        public int DureeJourApresDeuxiemeRelance { get; private set; }
        public int DureeJourAnnulationApresDerniereRelance { get; private set; }

        #endregion

        #region Module OleaPromo

        public bool ModuleOleaPromoActif { get; private set; }
        public string ModuleOleaSuffixeGratuit { get; private set; }

        #endregion

        #region Module Preorder

        public bool ModulePreorderActif { get; private set; }
        public string ModulePreorderInfolibreName { get; private set; }
        public string ModulePreorderInfolibreValue { get; private set; }
        public int ModulePreorderPrestashopProduct { get; private set; }
        public int ModulePreorderPrestashopOrderState { get; private set; }

        public string ModulePreorderInfolibreEnteteName { get; private set; }
        public string ModulePreorderInfolibreEnteteValue { get; private set; }

        #endregion

        #region Module AECInvoiceHistory

        public bool ModuleAECInvoiceHistoryActif { get; private set; }
        public string ModuleAECInvoiceHistoryInfoLibreClientSendMail { get; private set; }
        public string ModuleAECInvoiceHistoryInfoLibreClientSendMailValue { get; private set; }

        public bool ModuleAECInvoiceHistoryArchivePDFActive { get; private set; }
        public string ModuleAECInvoiceHistoryArchivePDFFolder { get; private set; }

        #endregion

        #region Module SoColissimo

        public bool ModuleSoColissimoDeliveryActive { get; private set; }

        public bool ModuleSoColissimoInfolibreTypePointActive { get; private set; }
        public string ModuleSoColissimoInfolibreEnteteTypePointName { get; private set; }

        public bool ModuleSoColissimoInfolibreDestinataireActive { get; private set; }
        public string ModuleSoColissimoInfolibreEnteteDestinataireName { get; private set; }

        public bool ModuleSoColissimoReplacePhoneActive { get; private set; }
        public bool ModuleSoColissimoReplaceAddressNameActive { get; private set; }

        #endregion

        public string CommandeArticlePackaging { get; private set; }

        #endregion

        #region Reglement

        public bool SyncReglementActif { get; private set; }
        public bool ModeReglementEcheancierActif { get; private set; }
        public bool ReglementLibellePartielActif { get; private set; }

        #endregion

        #region Mail

        #region Identification

        public bool ConfigMailActive { get; private set; }
        public string ConfigMailUser { get; private set; }
        public string ConfigMailPassword { get; private set; }
        public string ConfigMailSMTP { get; private set; }
        public int ConfigMailPort { get; private set; }
        public bool ConfigMailSSL { get; private set; }

        #endregion

        #endregion

        #region Import

        public string AutomaticImportFolderPicture { get; private set; }
        public string AutomaticImportFolderDocument { get; private set; }
        public string AutomaticImportFolderMedia { get; private set; }
        public bool ImportMediaIncludePictures { get; private set; }
        public bool ImportMediaAutoDeleteAttachment { get; private set; }
        public bool ImportImageReplaceFiles { get; private set; }
        public bool ImportImageRemoveDeletedFiles { get; private set; }

        public bool ImportImageSearchReferenceClient { get; private set; }

        public bool ImportImageUseSageDatas { get; private set; }

        #endregion

        #region Outils

        public string ConfigCronSynchroArticleURL { get; private set; }

        public bool ConfigRefreshTempCustomerListDisabled { get; private set; }

        public bool ConfigUnlockProcessorCore { get; private set; }
        public int ConfigAllocatedProcessorCore { get; private set; }

        public bool CrystalForceConnectionInfoOnSubReports { get; private set; }

        #endregion

        #region Interface/UI

        public ProductFilterActiveDefault UIProductFilterActiveDefault { get; private set; }
        public bool UIProductUpdateValidationDisabled { get; private set; }

        public bool UIMaximizeWindow { get; private set; }

        public int UISleepTimeWYSIWYG { get; private set; }
        public bool UIDisabledWYSIWYG { get; private set; }

        public bool UIIE11EmulationModeDisabled { get; private set; }


        // <JG> 03/06/2016
        public bool ListeArticleColonneIDVisible { get; private set; }
        public bool ListeArticleColonneTypeVisible { get; private set; }
        public bool ListeArticleColonneNomVisible { get; private set; }
        public bool ListeArticleColonneReferenceVisible { get; private set; }
        public bool ListeArticleColonneEANVisible { get; private set; }
        public bool ListeArticleColonneActifVisible { get; private set; }
        public bool ListeArticleColonneSyncVisible { get; private set; }
        public bool ListeArticleColonneSyncPriceVisible { get; private set; }
        public bool ListeArticleColonneDateVisible { get; private set; }

        #endregion

        #region ReimporSage

        public bool ReimportUpdateMainCatalog { get; private set; }
        public bool ReimportLinkParents { get; private set; }
        public bool ReimportDeleteLinkOldMain { get; private set; }
        public bool ReimportDeleteLinkOldSecondary { get; private set; }

        public bool ReimportUpdateProductName { get; private set; }
        public bool ReimportUpdateDescriptionShort { get; private set; }
        public bool ReimportUpdateDescription { get; private set; }
        public bool ReimportUpdateMetaTitle { get; private set; }
        public bool ReimportUpdateMetaDescription { get; private set; }
        public bool ReimportUpdateMetaKeywords { get; private set; }
        public bool ReimportUpdateURL { get; private set; }
        public bool ReimportUpdateEAN { get; private set; }
        public bool ReimportUpdateActive { get; private set; }

        public bool ReimportUpdateCharacteristic { get; private set; }
        public bool ReimportUpdateAttribute { get; private set; }
        public bool ReimportUpdateConditioning { get; private set; }

        public bool ReimportUpdateDateActive { get; private set; }

        #endregion

        #region Chronos

        public bool ChronoSynchroStockPriceActif { get; private set; }

        #endregion

        #region Cron/Scripts

        public string CronArticleURL { get; private set; }
        public string CronArticleBalise { get; private set; }
        public int CronArticleTimeout { get; private set; }

		public string CronCommandeURL { get; private set; }
		public string CronCommandeBalise { get; private set; }
		public int CronCommandeTimeout { get; private set; }

		#endregion

        #endregion

        #endregion

        #region Constructors

        public AppConfig()
        {
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config config = null;

            #region Folders
            config = ConfigRepository.ReadName(ConfigAppFoldersKey);

            if (config == null)
            {
                config = new Model.Local.Config()
                {
                    Con_Name = ConfigAppFoldersKey,
                    Con_Value = Path.Combine(Path.GetDirectoryName(Application.ResourceAssembly.Location), "Espace"),
                };

                ConfigRepository.Add(config);
            }

            Folders = new AppFolders(config.Con_Value);
            #endregion

            ConfigBToB = GetValue(ConfigBToBKey, true);
            ConfigBToC = GetValue(ConfigBToCKey, false);

            ConfigFTPActive = GetValue<bool>(ConfigFTPActiveKey);
            ConfigFTPIP = GetValue(ConfigFTPIPKey, "ftp://");
            ConfigFTPUser = GetValue<string>(ConfigFTPUserKey);
            ConfigFTPPassword = GetValue<string>(ConfigFTPPasswordKey);
            ConfigFTPSSL = GetValue<bool>(ConfigFTPSSLKey);

            //Autres
            ArticleEnSommeil = GetValue<bool>(ConfigArticleEnSommeilKey);
            ArticleNonPublieSurLeWeb = GetValue<bool>(ConfigArticleNonPublieSurLeWebKey);
            DureeJourAvantPremiereRelance = GetValue<int>(ConfigDureeJourAvantPremiereRelanceKey);
            DureeJourApresPremiereRelance = GetValue<int>(ConfigDureeJourApresPremiereRelanceKey);
            DureeJourApresDeuxiemeRelance = GetValue<int>(ConfigDureeJourApresDeuxiemeRelanceKey);
            DureeJourAnnulationApresDerniereRelance = GetValue<int>(ConfigDureeJourAnnulationApresDerniereRelanceKey);

            ModeRemise = GetValue<RemiseMode>(ConfigRemiseModeKey);
            ConflitRemise = (RemiseConflit)GetValue(ConfigRemiseConflitKey, (short)RemiseConflit.PrioriteArticleFamilleClient);

            ConfigLocalStorageMode = GetValue<LocalStorageMode>(ConfigLocalStorageModeKey);
            ConfigImageSynchroPositionLegende = GetValue<bool>(ConfigImageSynchroPositionLegendeKey);
            ConfigImageMiniatureHeight = GetValue(ConfigImageMiniatureHeightKey, 45);
            ConfigImageMiniatureWidth = GetValue(ConfigImageMiniatureWidthKey, 45);

            AdminMailAddress = GetValue<string>(ConfigAdminMailAddressKey).Trim();

            #region Articles

            LimiteStockConditionnement = GetValue(ConfigLimiteStockConditionnementKey, true);

            ConfigArticleCatComptable = GetValue<int>(ConfigArticleCatComptableKey);
            ArticleStockNegatifZero = GetValue(ConfigArticleStockNegatifZeroKey, true);
            ArticleStockNegatifZeroParDepot = GetValue<bool>(ConfigArticleStockNegatifZeroParDepotKey);

            ArticleSpecificPriceLetBasePriceRule = GetValue(ConfigArticleSpecificPriceLetBasePriceRuleKey, true);

            #endregion

            #region Transfert Client
            TransfertPrestashopCookieKey = GetValue<string>(ConfigTransfertPrestashopCookieKeyKey).Trim();

            TransfertPriceCategoryAvailable = GetListValue(ConfigTransfertPriceCategoryAvailableKey);
            TransfertMailAccountIdentification = GetValue<MailAccountIdentification>(ConfigTransfertMailAccountKey);
            TransfertMailAccountAlternative = GetValue<bool>(ConfigTransfertMailAccountAlternativeKey);
            TransfertMailAccountContactService = GetValue<int>(ConfigTransfertMailAccountContactService);
            TransfertRandomPasswordLength = GetValue<int>(ConfigTransfertRandomPasswordLengthKey);
            TransfertRandomPasswordIncludeSpecialCharacters = GetValue<bool>(ConfigTransfertRandomPasswordIncludeSpecialCharactersKey);

            TransfertNotifyAccountAddress = GetValue<MailNotification>(ConfigTransfertNotifyAccountAddressKey);
            TransfertNotifyAccountAdminCopy = GetValue<bool>(ConfigTransfertNotifyAccountAdminCopyKey);
            TransfertNotifyAccountDeliveryMethod = GetValue<DeliveryMethod>(ConfigTransfertNotifyAccountDeliveryMethodKey);
            TransfertNotifyAccountSageContactType = GetListValue(ConfigTransfertNotifyAccountSageContactTypeKey);
            TransfertNotifyAccountSageContactService = GetListValue(ConfigTransfertNotifyAccountSageContactServiceKey);

            TransfertSageAddressSend = GetListValue(ConfigTransfertSageAddressSendKey);
            TransfertLockPhoneNumber = GetListValue(ConfigTransfertLockPhoneNumberKey);
            TransfertLockPhoneNumberReplaceEntryValue = GetValue<string>(ConfigTransfertLockPhoneNumberReplaceEntryValueKey);
            TransfertAliasValue = GetValue<AliasValue>(ConfigTransfertAliasValueKey);
            TransfertLastNameValue = GetValue<LastNameValue>(ConfigTransfertLastNameValueKey);
            TransfertFirstNameValue = GetValue<FirstNameValue>(ConfigTransfertFirstNameValueKey);
            TransfertCompanyValue = GetValue<CompanyValue>(ConfigTransfertCompanyValueKey);
            TransfertClientSeparateurIntitule = GetValue<string>(ConfigTransfertClientSeparateurIntituleKey);

            TransfertAccountActivation = GetValue<bool>(ConfigTransfertAccountActivationKey);
            TransfertNewsLetterSuscribe = GetValue<bool>(ConfigTransfertNewsLetterSuscribeKey);
            TransfertOptInSuscribe = GetValue<bool>(ConfigTransfertOptInSuscribeKey);

            TransfertIntegrateCustomerSynchronizationProcess = GetValue<bool>(ConfigTransfertIntegrateCustomerSynchronizationProcessKey);
            TransfertSendAdminResultReport = GetValue<bool>(ConfigTransfertSendAdminResultReportKey);
            TransfertGenerateAccountFile = GetValue<bool>(ConfigTransfertGenerateAccountFileKey);

            RegexMailLevel = GetValue<RegexMail>(ConfigRegexMailLevelKey);

            TransfertNameIncludeNumbers = GetValue(ConfigTransfertNameIncludeNumbersKey, false);

            #endregion

            ImportArticleStatutActif = GetValue(ConfigImportArticleStatutActifKey, true);
            ImportArticleRattachementParents = GetValue(ConfigImportArticleRattachementParentsKey, false);
            ImportArticleOnlyStandardProducts = GetValue<bool>(ConfigImportArticleOnlyStandardProductsKey);

            ImportCatalogueAutoSelectionParents = GetValue(ConfigImportCatalogueAutoSelectionParentsKey, true);
            ImportCatalogueAutoSelectionEnfants = GetValue(ConfigImportCatalogueAutoSelectionEnfantsKey, true);

            #region Import

            AutomaticImportFolderPicture = GetValue<string>(ConfigAutomaticImportFolderPictureKey);
            AutomaticImportFolderDocument = GetValue<string>(ConfigAutomaticImportFolderDocumentKey);
            AutomaticImportFolderMedia = GetValue(ConfigAutomaticImportFolderMediaKey, GetDefaultSageMediaFolder());
            ImportMediaIncludePictures = GetValue<bool>(ConfigImportMediaIncludePicturesKey);
            ImportMediaAutoDeleteAttachment = GetValue<bool>(ConfigImportMediaAutoDeleteAttachmentKey);
            ImportImageReplaceFiles = GetValue<bool>(ConfigImportImageReplaceFilesKey);
            ImportImageRemoveDeletedFiles = GetValue<bool>(ConfigImportImageRemoveDeletedFilesKey);

            ImportImageSearchReferenceClient = GetValue<bool>(ConfigImportImageSearchReferenceClientKey);
            ImportImageUseSageDatas = GetValue<bool>(ConfigImportImageUseSageDatasKey);

            #endregion

            ConfigImageStorageMode = GetValue<ImageStorageMode>(ConfigImageStorageModeKey);

            LigneRemiseMode = GetValue<LigneRemiseMode>(ConfigLigneRemiseModeKey);

			LigneFraisPort = GetValue<bool>(ConfigLigneFraisPortKey);
			LigneArticlePort = GetValue<string>(ConfigLigneArticlePortKey);

			DateLivraisonMode = GetValue<DateLivraisonMode>(ConfigDateLivraisonMode);
			DateLivraisonJours = GetValue<int>(ConfigDateLivraisonJours);

			CommandeArticleReduction = GetValue<string>(ConfigCommandeArticleReductionKey);

            CommandeReferencePrestaShop = GetValue<bool>(ConfigCommandeReferencePrestaShopKey);
            CommandeNumeroFactureSageForceActif = GetValue<bool>(ConfigCommandeNumeroFactureSageForceActifKey);
            CommandeStatutJoursAutomate = GetValue(ConfigCommandeStatutJoursAutomateKey, 7);
            ConfigCommandeFiltreDate = GetValue(ConfigCommandeFiltreDateKey);

            #region Tracking
            CommandeTrackingEnteteActif = GetValue<bool>(ConfigCommandeTrackingEnteteActifKey);
            CommandeTrackingInfolibreActif = GetValue<bool>(ConfigCommandeTrackingInfolibreActifKey);
            CommandeTrackingEntete = GetValue<FieldDocumentEntete>(ConfigCommandeTrackingEnteteKey);
            CommandeTrackingInfolibre = GetValue<string>(ConfigCommandeTrackingInfolibreKey);
            #endregion

            #region statuts
            ConfigCommandeDE = GetValue(ConfigCommandeDEKey, 0);
            ConfigCommandeBC = GetValue(ConfigCommandeBCKey, 0);
            ConfigCommandePL = GetValue(ConfigCommandePLKey, 0);
            ConfigCommandeBL = GetValue(ConfigCommandeBLKey, 0);
            ConfigCommandeFA = GetValue(ConfigCommandeFAKey, 0);
            ConfigCommandeFC = GetValue(ConfigCommandeFCKey, 0);
            #endregion

            ConfigClientFiltreCommande = GetValue<bool>(ConfigClientFiltreCommandeKey);

            MajPoidsSynchroStock = GetValue<bool>(ConfigMajPoidsSynchroStockKey);
            ArticleFiltreDatePrixPrestashop = GetValue<bool>(ConfigArticleFiltreDatePrixPrestashopKey);

            CombinationWithWeightConversion = GetListValue(ConfigCombinationWithWeightConversionKey);
            InformationLibreCoefficientConversion = GetValue<string>(ConfigInformationLibreCoefficientConversionKey);

            #region Création client / Numérotation

            ConfigClientNumComposition = GetValue<string>(ConfigClientNumCompositionKey);
            ConfigClientNumPrefixe = GetValue<string>(ConfigClientNumPrefixeKey);
            ConfigClientNumLongueurNom = GetValue<int>(ConfigClientNumLongueurNomKey);
            ConfigClientNumTypeNom = GetValue<NameNumComponent>(ConfigClientNumTypeNomKey);
            ConfigClientNumLongueurNumero = GetValue<int>(ConfigClientNumLongueurNumeroKey);
            ConfigClientNumTypeNumero = GetValue<NumberNumComponent>(ConfigClientNumTypeNumeroKey);
            ConfigClientNumTypeCompteur = GetValue<CounterType>(ConfigClientNumTypeCompteurKey);
            ConfigClientNumDebutCompteur = GetValue<int>(ConfigClientNumDebutCompteurKey);

            ConfigClientNumDepartementRemplacerCodeISO = GetValue<bool>(ConfigClientNumDepartementRemplacerCodeISOKey);

            ConfigClientMultiMappageBtoB = GetValue<bool>(ConfigClientMultiMappageBtoBKey);

            ConfigClientCiviliteActif = GetValue<bool>(ConfigClientCiviliteActifKey);

            ConfigClientSocieteIntituleActif = GetValue<bool>(ConfigClientSocieteIntituleActifKey);
            ConfigClientNIFActif = GetValue<bool>(ConfigClientNIFActifKey);
            ConfigClientInfosMajusculeActif = GetValue<bool>(ConfigClientInfosMajusculeActifKey);

            #endregion

            #region Adresses

            ConfigClientAdresseTelephonePositionFixe = GetValue<bool>(ConfigClientAdresseTelephonePositionFixeKey);

            #endregion

            ConfigCronSynchroArticleURL = GetValue<string>(ConfigCronSynchroArticleURLKey);

            #region Marque produit automatique

            MarqueAutoStatistiqueActif = GetValue<bool>(ConfigMarqueAutoStatistiqueActifKey);
            MarqueAutoStatistiqueName = GetValue<string>(ConfigMarqueAutoStatistiqueNameKey);
            MarqueAutoInfolibreActif = GetValue<bool>(ConfigMarqueAutoInfolibreActifKey);
            MarqueAutoInfolibreName = GetValue<string>(ConfigMarqueAutoInfolibreNameKey);

            #endregion

            #region Fournisseur produit automatique

            FournisseurAutoStatistiqueActif = GetValue<bool>(ConfigFournisseurAutoStatistiqueActifKey);
            FournisseurAutoStatistiqueName = GetValue<string>(ConfigFournisseurAutoStatistiqueNameKey);
            FournisseurAutoInfolibreActif = GetValue<bool>(ConfigFournisseurAutoInfolibreActifKey);
            FournisseurAutoInfolibreName = GetValue<string>(ConfigFournisseurAutoInfolibreNameKey);

            #endregion

            StatInfolibreClientActif = GetValue<bool>(ConfigStatInfolibreClientActifKey);

            ArticleImportConditionnementActif = GetValue<bool>(ConfigImportConditionnementActifKey);
            ArticleConditionnementQuantiteToUPC = GetValue<bool>(ConfigConditionnementQuantiteToUPCKey);

            ArticleContremarqueStockActif = GetValue<bool>(ConfigArticleContremarqueStockKey);

            ArticleInfolibrePackaging = GetValue<string>(ConfigArticleInfolibrePackagingKey);

            CommandeArticlePackaging = GetValue<string>(ConfigCommandeArticlePackagingKey);

            DeleteCatalogProductAssociation = GetValue(ConfigDeleteCatalogProductAssociationKey, true);

            ArticleDateDispoInfoLibreActif = GetValue<bool>(ConfigArticleDateDispoInfoLibreActifKey);
            ArticleDateDispoInfoLibreName = GetValue<string>(ConfigArticleDateDispoInfoLibreNameKey);

            ArticleQuantiteMiniActif = GetValue<bool>(ConfigArticleQuantiteMiniActifKey);
            ArticleQuantiteMiniConditionnement = GetValue<bool>(ConfigArticleQuantiteMiniConditionnementKey);
            ArticleQuantiteMiniUniteVente = GetValue<bool>(ConfigArticleQuantiteMiniUniteVenteKey);

            ArticleTransfertInfosFournisseurActif = GetValue<bool>(ConfigArticleTransfertInfosFournisseurActifKey);

            ArticleSuppressionAutoCaracteristique = GetValue<bool>(ConfigArticleSuppressionAutoCaracteristiqueKey);

            ArticleRedirectionCompositionActif = GetValue<bool>(ConfigArticleRedirectionCompositionActifKey);

            CommandeUpdateAdresseFacturation = GetValue<bool>(ConfigCommandeUpdateAdresseFacturationKey);
            CommandeInsertFacturationEntete = GetValue<bool>(ConfigCommandeInsertFacturationEnteteKey);

            #region Taxes

            TaxSageTVA = (Core.Global.IsInteger(GetValue<string>(ConfigTaxSageTVAKey)) ? GetValue<TaxSage>(ConfigTaxSageTVAKey) : TaxSage.CodeTaxe1);
            TaxSageEco = (Core.Global.IsInteger(GetValue<string>(ConfigTaxSageEcoKey)) ? GetValue<TaxSage>(ConfigTaxSageEcoKey) : TaxSage.CodeTaxe2);

            #endregion

            #region Module OleaPromo

            ModuleOleaPromoActif = GetValue<bool>(ConfigModuleOleaPromoActifKey);
            ModuleOleaSuffixeGratuit = GetValue<string>(ConfigModuleOleaSuffixeGratuitKey);

            #endregion

            #region Outils

            ConfigRefreshTempCustomerListDisabled = GetValue<bool>(ConfigRefreshTempCustomerListDisabledKey);

            ConfigUnlockProcessorCore = GetValue<bool>(ConfigUnlockProcessorCoreKey);
            ConfigAllocatedProcessorCore = GetValue(ConfigAllocatedProcessorCoreKey, ((System.Environment.ProcessorCount <= 4) ? System.Environment.ProcessorCount : 4));

            CrystalForceConnectionInfoOnSubReports = GetValue(ConfigCrystalForceConnectionInfoOnSubReportsKey, true);

            #endregion

            #region Interface/UI

            UIProductFilterActiveDefault = GetValue<ProductFilterActiveDefault>(UIProductFilterActiveDefaultKey);
            UIProductUpdateValidationDisabled = GetValue<bool>(UIProductUpdateValidationDisabledKey);

            UIMaximizeWindow = GetValue<bool>(UIMaximizeWindowKey);

            UISleepTimeWYSIWYG = GetValue(UISleepTimeWYSIWYGKey, 1000);
            UIDisabledWYSIWYG = GetValue<bool>(UIDisabledWYSIWYGKey);

            UIIE11EmulationModeDisabled = GetValue<bool>(UIIE11EmulationModeDisabledKey);

            // <JG> 03/06/2016
            ListeArticleColonneIDVisible = GetValue(ListeArticleColonneIDVisibleKey, true);
            ListeArticleColonneTypeVisible = GetValue(ListeArticleColonneTypeVisibleKey, true);
            ListeArticleColonneNomVisible = GetValue(ListeArticleColonneNomVisibleKey, true);
            ListeArticleColonneReferenceVisible = GetValue(ListeArticleColonneReferenceVisibleKey, true);
            ListeArticleColonneEANVisible = GetValue(ListeArticleColonneEANVisibleKey, true);
            ListeArticleColonneActifVisible = GetValue(ListeArticleColonneActifVisibleKey, true);
            ListeArticleColonneSyncVisible = GetValue(ListeArticleColonneSyncVisibleKey, true);
            ListeArticleColonneSyncPriceVisible = GetValue(ListeArticleColonneSyncPriceVisibleKey, true);
            ListeArticleColonneDateVisible = GetValue(ListeArticleColonneDateVisibleKey, true);

            #endregion

            #region Commentaires commandes

            CommentaireBoutiqueActif = GetValue<bool>(ConfigCommentaireBoutiqueActifKey);
            CommentaireBoutiqueTexte = GetValue<string>(ConfigCommentaireBoutiqueTexteKey);
            CommentaireNumeroActif = GetValue<bool>(ConfigCommentaireNumeroActifKey);
            CommentaireNumeroTexte = GetValue<string>(ConfigCommentaireNumeroTexteKey);
            CommentaireReferencePaiementActif = GetValue<bool>(ConfigCommentaireReferencePaiementActifKey);
            CommentaireReferencePaiementTexte = GetValue<string>(ConfigCommentaireReferencePaiementTexteKey);
            CommentaireDateActif = GetValue<bool>(ConfigCommentaireDateActifKey);
            CommentaireDateTexte = GetValue<string>(ConfigCommentaireDateTexteKey);

            CommentaireClientActif = GetValue(ConfigCommentaireClientActifKey, true);
            CommentaireClientTexte = GetValue(ConfigCommentaireClientTexteKey, "Message du client pour cette commande en glossaire");
            CommentaireAdresseFacturationActif = GetValue(ConfigCommentaireAdresseFacturationActifKey, true);
            CommentaireAdresseFacturationTexte = GetValue(ConfigCommentaireAdresseFacturationTexteKey, "Informations complémentaires adresse facturation en glossaire");
            CommentaireAdresseLivraisonActif = GetValue(ConfigCommentaireAdresseLivraisonActifKey, true);
            CommentaireAdresseLivraisonTexte = GetValue(ConfigCommentaireAdresseLivraisonTexteKey, "Informations complémentaires adresse livraison en glossaire");

            CommentaireLibre1Actif = GetValue<bool>(ConfigCommentaireLibre1ActifKey);
            CommentaireLibre1Texte = GetValue<string>(ConfigCommentaireLibre1TexteKey);
            CommentaireLibre2Actif = GetValue<bool>(ConfigCommentaireLibre2ActifKey);
            CommentaireLibre2Texte = GetValue<string>(ConfigCommentaireLibre2TexteKey);
            CommentaireLibre3Actif = GetValue<bool>(ConfigCommentaireLibre3ActifKey);
            CommentaireLibre3Texte = GetValue<string>(ConfigCommentaireLibre3TexteKey);

            CommentaireDebutDocument = GetValue(ConfigCommentaireDebutDocumentKey, true);
            CommentaireFinDocument = GetValue(ConfigCommentaireFinDocumentKey, false);

            #endregion

            #region Reglement

            SyncReglementActif = GetValue<bool>(ConfigSyncReglementActifKey);
            ModeReglementEcheancierActif = GetValue<bool>(ConfigModeReglementEcheancierActifKey);
            ReglementLibellePartielActif = GetValue<bool>(ConfigReglementLibellePartielActifKey);

            #endregion

            #region Mail

            ConfigMailActive = GetValue<bool>(ConfigMailActiveKey);
            ConfigMailUser = GetValue<string>(ConfigMailUserKey);
            ConfigMailPassword = GetValue<string>(ConfigMailPasswordKey);
            ConfigMailSMTP = GetValue<string>(ConfigMailSMTPKey);
            ConfigMailPort = (Core.Global.IsInteger(GetValue<string>(ConfigMailPortKey)) ? GetValue<int>(ConfigMailPortKey) : 25);
            ConfigMailSSL = GetValue<bool>(ConfigMailSSLKey);

            #endregion

            #region Module Preorder

            ModulePreorderActif = GetValue<bool>(ConfigModulePreorderActifKey);
            ModulePreorderInfolibreName = GetValue<string>(ConfigModulePreorderInfolibreNameKey);
            ModulePreorderInfolibreValue = GetValue<string>(ConfigModulePreorderInfolibreValueKey);
            ModulePreorderPrestashopProduct = GetValue<int>(ConfigModulePreorderPrestashopProductKey);
            ModulePreorderPrestashopOrderState = GetValue<int>(ConfigModulePreorderPrestashopOrderStateKey);

            ModulePreorderInfolibreEnteteName = GetValue<string>(ConfigModulePreorderInfolibreEnteteNameKey);
            ModulePreorderInfolibreEnteteValue = GetValue<string>(ConfigModulePreorderInfolibreEnteteValueKey);

            #endregion

            #region Module AECInvoiceHistory

            ModuleAECInvoiceHistoryActif = GetValue<bool>(ConfigModuleAECInvoiceHistoryActifKey);
            ModuleAECInvoiceHistoryInfoLibreClientSendMail = GetValue<string>(ConfigModuleAECInvoiceHistoryInfoLibreClientSendMailKey);
            ModuleAECInvoiceHistoryInfoLibreClientSendMailValue = GetValue<string>(ConfigModuleAECInvoiceHistoryInfolibreClientSendMailValueKey);

            ModuleAECInvoiceHistoryArchivePDFActive = GetValue<bool>(ConfigModuleAECInvoiceHistoryArchivePDFActiveKey);
            ModuleAECInvoiceHistoryArchivePDFFolder = GetValue<string>(ConfigModuleAECInvoiceHistoryArchivePDFFolderKey);

            #endregion

            #region Module AECStock

            ModuleAECStockActif = GetValue<bool>(ConfigModuleAECStockActifKey);

            #endregion

            #region Module AECCollaborateur

            ModuleAECCollaborateurActif = GetValue<bool>(ConfigModuleAECCollaborateurActifKey);

            #endregion

            #region Module AECPaiement

            ModuleAECPaiementActif = GetValue<bool>(ConfigModuleAECPaiementActifKey);

            #endregion

            #region Module AECCustomerOutstanding

            ModuleAECCustomerOutstandingActif = GetValue<bool>(ConfigModuleAECCustomerOutstandingActifKey);

            #endregion

            #region Module AECCustomerInfo

            ModuleAECCustomerInfoActif = GetValue<bool>(ConfigModuleAECCustomerInfoActifKey);

            #endregion

            #region Module Portfolio Customer Employee

            ModulePortfolioCustomerEmployeeActif = GetValue<bool>(ConfigModulePortfolioCustomerEmployeeActifKey);

            #endregion

            #region Module AECAttributeList & AECAttributeStatut

            //ModuleAECAttributeListActif = GetValue<bool>(ConfigModuleAECAttributeListActifKey);
            //ModuleAECAttributeStatutActif = GetValue<bool>(ConfigModuleAECAttributeStatutActifKey);

            #endregion

            #region Module SoColissimo

            ModuleSoColissimoDeliveryActive = GetValue<bool>(ConfigModuleSoColissimoDeliveryActiveKey);

            ModuleSoColissimoInfolibreTypePointActive = GetValue<bool>(ConfigModuleSoColissimoInfolibreTypePointActiveKey);
            ModuleSoColissimoInfolibreEnteteTypePointName = GetValue<string>(ConfigModuleSoColissimoInfolibreEnteteTypePointNameKey);

            ModuleSoColissimoInfolibreDestinataireActive = GetValue<bool>(ConfigModuleSoColissimoInfolibreDestinataireActiveKey);
            ModuleSoColissimoInfolibreEnteteDestinataireName = GetValue<string>(ConfigModuleSoColissimoInfolibreEnteteDestinataireNameKey);

            ModuleSoColissimoReplacePhoneActive = GetValue<bool>(ConfigModuleSoColissimoReplacePhoneActiveKey);
            ModuleSoColissimoReplaceAddressNameActive = GetValue<bool>(ConfigModuleSoColissimoReplaceAddressNameActiveKey);

            #endregion

			#region Module DWFProductGuiderates

			ModuleDWFProductGuideratesActif = GetValue<bool>(ConfigModuleDWFProductGuideratesActifKey);

			#endregion

			#region Module DWFProductGuiderates

			ModuleDWFProductExtraFieldsActif = GetValue<bool>(ConfigModuleDWFProductExtraFieldsActifKey);

            #endregion

            #region ReimporSage

            ReimportUpdateMainCatalog = GetValue<bool>(ReimportUpdateMainCatalogKey);
            ReimportLinkParents = GetValue<bool>(ReimportLinkParentsKey);
            ReimportDeleteLinkOldMain = GetValue<bool>(ReimportDeleteLinkOldMainKey);
            ReimportDeleteLinkOldSecondary = GetValue<bool>(ReimportDeleteLinkOldSecondaryKey);

            ReimportUpdateProductName = GetValue<bool>(ReimportUpdateProductNameKey);
            ReimportUpdateDescriptionShort = GetValue<bool>(ReimportUpdateDescriptionShortKey);
            ReimportUpdateDescription = GetValue<bool>(ReimportUpdateDescriptionKey);
            ReimportUpdateMetaTitle = GetValue<bool>(ReimportUpdateMetaTitleKey);
            ReimportUpdateMetaDescription = GetValue<bool>(ReimportUpdateMetaDescriptionKey);
            ReimportUpdateMetaKeywords = GetValue<bool>(ReimportUpdateMetaKeywordsKey);
            ReimportUpdateURL = GetValue<bool>(ReimportUpdateURLKey);
            ReimportUpdateEAN = GetValue<bool>(ReimportUpdateEANKey);
            ReimportUpdateActive = GetValue<bool>(ReimportUpdateActiveKey);

            ReimportUpdateCharacteristic = GetValue<bool>(ReimportUpdateCharacteristicKey);
            ReimportUpdateAttribute = GetValue<bool>(ReimportUpdateAttributeKey);
            ReimportUpdateConditioning = GetValue<bool>(ReimportUpdateConditioningKey);

            ReimportUpdateDateActive = GetValue(ReimportUpdateDateActiveKey, true);

            #endregion

            #region Chronos

            ChronoSynchroStockPriceActif = GetValue<bool>(ChronoSynchroStockPriceActifKey);

            #endregion

            #region Cron/Scripts

            CronArticleURL = GetValue<string>(CronArticleURLKey);
            CronArticleBalise = GetValue<string>(CronArticleBaliseKey);
            CronArticleTimeout = GetValue(CronArticleTimeoutKey, 100);

			CronCommandeURL = GetValue<string>(CronCommandeURLKey);
			CronCommandeBalise = GetValue<string>(CronCommandeBaliseKey);
			CronCommandeTimeout = GetValue(CronCommandeTimeoutKey, 100);

            #endregion
        }

        #endregion

        #region Methods

        #region Generic

        private TValue GetValue<TValue>(string key)
        {
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config config = null;

            config = ConfigRepository.ReadName(key);

            TValue value = default(TValue);

            if (config == null)
            {
                config = new Model.Local.Config();
                config.Con_Name = key;

                if (typeof(TValue).IsEnum)
                    config.Con_Value = Convert.ToInt32(value).ToString();
                else
                    config.Con_Value = (value != null) ? value.ToString() : string.Empty;

                ConfigRepository.Add(config);
            }
            else if (typeof(TValue).IsEnum)
                value = (TValue)Convert.ChangeType(config.Con_Value, typeof(Int32));
            else
                value = (TValue)Convert.ChangeType(config.Con_Value, typeof(TValue));

            // empêche la lecture d'une chaîne nulle
            if (value == null)
                value = GetValue<TValue>(key);

            return value;
        }
        private int GetValue(string key, int defaut)
        {
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config config = null;

            config = ConfigRepository.ReadName(key);
            int value = defaut;
            if (config == null)
            {
                config = new Model.Local.Config();
                config.Con_Name = key;
                config.Con_Value = value.ToString();
                ConfigRepository.Add(config);
            }
            else
                int.TryParse(config.Con_Value, out value);

            return value;
        }
        private string GetValue(string key, string defaut)
        {
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config config = null;

            config = ConfigRepository.ReadName(key);
            string value = defaut;
            if (config == null)
            {
                config = new Model.Local.Config();
                config.Con_Name = key;
                config.Con_Value = value.ToString();
                ConfigRepository.Add(config);
            }
            else
                value = config.Con_Value;

            return value;
        }
        private bool GetValue(string key, bool defaut)
        {
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config config = null;

            config = ConfigRepository.ReadName(key);
            bool value = defaut;
            if (config == null)
            {
                config = new Model.Local.Config();
                config.Con_Name = key;
                config.Con_Value = value.ToString();
                ConfigRepository.Add(config);
            }
            else
                bool.TryParse(config.Con_Value, out value);

            return value;
        }

        private List<int> GetListValue(string key)
        {

            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config config = null;

            config = ConfigRepository.ReadName(key);

            List<int> list = new List<int>();

            if (config == null)
            {
                config = new Model.Local.Config();
                config.Con_Name = key;
                config.Con_Value = string.Empty;
                ConfigRepository.Add(config);
            }
            else
            {
                foreach (string s in config.Con_Value.Split((new String[] { "#" }), StringSplitOptions.RemoveEmptyEntries))
                {
                    int v;
                    if (int.TryParse(s, out v))
                        list.Add(v);
                }
            }
            return list;
        }
        private DateTime? GetValue(string key)
        {
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.Config config = null;

            config = ConfigRepository.ReadName(key);

            DateTime? value = null;

            if (config == null)
            {
                config = new Model.Local.Config();
                config.Con_Name = key;

                config.Con_Value = (value != null) ? value.Value.ToShortDateString() : string.Empty;

                ConfigRepository.Add(config);
            }
            else
                value = (!String.IsNullOrEmpty(config.Con_Value) && Core.Global.IsDate(config.Con_Value)) ? (DateTime?)DateTime.Parse(config.Con_Value) : null;

            return value;
        }

        private TValue SetValue<TValue>(string key, TValue value)
        {
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();

            Model.Local.Config config = ConfigRepository.ReadName(key);

            if (typeof(TValue).IsEnum)
                config.Con_Value = Convert.ToInt32(value).ToString();
            else
                config.Con_Value = value.ToString();

            ConfigRepository.Save();

            return value;
        }
        private List<int> SetListValue(string key, List<int> value)
        {
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();

            Model.Local.Config config = ConfigRepository.ReadName(key);
            config.Con_Value = string.Empty;

            if (value.Count > 0)
                foreach (int item in value)
                    config.Con_Value += "#" + item.ToString();

            ConfigRepository.Save();

            return value;
        }

        #endregion

        #region Avancé

        public void UpdateFolders(string dossier)
        {
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();

            Model.Local.Config config = ConfigRepository.ReadName(ConfigAppFoldersKey);
            config.Con_Value = dossier;

            ConfigRepository.Save();

            Folders = new AppFolders(dossier);
        }

        #endregion

        #region General

        public void UpdateModeRemise(RemiseMode modeRemise)
        {
            ModeRemise = SetValue<RemiseMode>(ConfigRemiseModeKey, modeRemise);
        }

        public void UpdateConflitRemise(RemiseConflit conflit)
        {
            ConflitRemise = SetValue<RemiseConflit>(ConfigRemiseConflitKey, conflit);
        }

        public void UpdateAdminMailAddress(string AdminMail)
        {
            AdminMailAddress = SetValue<string>(ConfigAdminMailAddressKey, AdminMail);
        }

        public void UpdateConfigBtoCBtoB(bool btoc, bool btob)
        {
            ConfigBToC = SetValue<bool>(ConfigBToCKey, btoc);
            ConfigBToB = SetValue<bool>(ConfigBToBKey, btob);
        }

        public void UpdateFTPActive(bool ftpactive)
        {
            ConfigFTPActive = SetValue(ConfigFTPActiveKey, ftpactive);
        }
        public void UpdateFTPIP(string ftp)
        {
            ConfigFTPIP = SetValue(ConfigFTPIPKey, ftp);
        }
        public void UpdateFTPUser(string user)
        {
            ConfigFTPUser = SetValue<string>(ConfigFTPUserKey, user);
        }
        public void UpdateFTPPass(string pass)
        {
            ConfigFTPPassword = SetValue<string>(ConfigFTPPasswordKey, pass);
        }
        public void UpdateFTPSSL(bool ssl)
        {
            ConfigFTPSSL = SetValue<bool>(ConfigFTPSSLKey, ssl);
        }

        #region Images

        public void UpdateConfigImageStorageMode(ImageStorageMode StorageMode)
        {
            ConfigImageStorageMode = SetValue<ImageStorageMode>(ConfigImageStorageModeKey, StorageMode);
        }

        public void UpdateConfigLocalStorageMode(LocalStorageMode StorageMode)
        {
            ConfigLocalStorageMode = SetValue<LocalStorageMode>(ConfigLocalStorageModeKey, StorageMode);
        }

        public void UpdateConfigImageSynchroPositionLegende(bool ImageSynchroPositionLegende)
        {
            ConfigImageSynchroPositionLegende = SetValue<bool>(ConfigImageSynchroPositionLegendeKey, ImageSynchroPositionLegende);
        }

        public void UpdateConfigImageMiniatureHeight(int Height)
        {
            ConfigImageMiniatureHeight = SetValue<int>(ConfigImageMiniatureHeightKey, Height);
        }

        public void UpdateConfigImageMiniatureWidth(int Width)
        {
            ConfigImageMiniatureWidth = SetValue<int>(ConfigImageMiniatureWidthKey, Width);
        }

        #endregion

        #endregion

        #region Article

        public void UpdateMajPoidsSynchroStock(bool updateWeigth)
        {
            MajPoidsSynchroStock = SetValue<bool>(ConfigMajPoidsSynchroStockKey, updateWeigth);
        }

        public void UpdateArticleFiltreDatePrixPrestashop(bool filtredateprix)
        {
            ArticleFiltreDatePrixPrestashop = SetValue<bool>(ConfigArticleFiltreDatePrixPrestashopKey, filtredateprix);
        }

        public void UpdateCombinationWithWeightConversion(List<int> ListCombination)
        {
            CombinationWithWeightConversion = SetListValue(ConfigCombinationWithWeightConversionKey, ListCombination);
        }
        public void UpdateInformationLibreCoefficientConversion(string InfolibreCoefficient)
        {
            InformationLibreCoefficientConversion = SetValue<string>(ConfigInformationLibreCoefficientConversionKey, InfolibreCoefficient);
        }

        public void UpdateArticleNonPublieSurLeWeb(bool publier)
        {
            ArticleNonPublieSurLeWeb = SetValue<bool>(ConfigArticleNonPublieSurLeWebKey, publier);
        }
        public void UpdateArticleEnSommeil(bool enSommeil)
        {
            ArticleEnSommeil = SetValue<bool>(ConfigArticleEnSommeilKey, enSommeil);
        }

        public void UpdateImportArticleStatutActif(bool statut)
        {
            ImportArticleStatutActif = SetValue<bool>(ConfigImportArticleStatutActifKey, statut);
        }
        public void UpdateImportArticleRattachementParents(bool rattachement)
        {
            ImportArticleRattachementParents = SetValue<bool>(ConfigImportArticleRattachementParentsKey, rattachement);
        }
        public void UpdateImportArticleOnlyStandardProducts(bool onlystandard)
        {
            ImportArticleOnlyStandardProducts = SetValue<bool>(ConfigImportArticleOnlyStandardProductsKey, onlystandard);
        }

        public void UpdateImportCatalogueAutoSelectionParents(bool autoselectparents)
        {
            ImportCatalogueAutoSelectionParents = SetValue<bool>(ConfigImportCatalogueAutoSelectionParentsKey, autoselectparents);
        }
        public void UpdateImportCatalogueAutoSelectionEnfants(bool autoselectenfants)
        {
            ImportCatalogueAutoSelectionEnfants = SetValue<bool>(ConfigImportCatalogueAutoSelectionEnfantsKey, autoselectenfants);
        }

        public void UpdateMarqueAutoStatistiqueActif(bool MarqueStatActif)
        {
            MarqueAutoStatistiqueActif = SetValue<bool>(ConfigMarqueAutoStatistiqueActifKey, MarqueStatActif);
        }
        public void UpdateMarqueAutoStatistiqueName(string MarqueStatName)
        {
            MarqueAutoStatistiqueName = SetValue<string>(ConfigMarqueAutoStatistiqueNameKey, MarqueStatName);
        }
        public void UpdateMarqueAutoInfolibreActif(bool MarqueInfolibreActif)
        {
            MarqueAutoInfolibreActif = SetValue<bool>(ConfigMarqueAutoInfolibreActifKey, MarqueInfolibreActif);
        }
        public void UpdateMarqueAutoInfolibreName(string MarqueInfolibreName)
        {
            MarqueAutoInfolibreName = SetValue<string>(ConfigMarqueAutoInfolibreNameKey, MarqueInfolibreName);
        }

        public void UpdateFournisseurAutoStatistiqueActif(bool FournisseurStatActif)
        {
            FournisseurAutoStatistiqueActif = SetValue<bool>(ConfigFournisseurAutoStatistiqueActifKey, FournisseurStatActif);
        }
        public void UpdateFournisseurAutoStatistiqueName(string FournisseurStatName)
        {
            FournisseurAutoStatistiqueName = SetValue<string>(ConfigFournisseurAutoStatistiqueNameKey, FournisseurStatName);
        }
        public void UpdateFournisseurAutoInfolibreActif(bool FournisseurInfolibreActif)
        {
            FournisseurAutoInfolibreActif = SetValue<bool>(ConfigFournisseurAutoInfolibreActifKey, FournisseurInfolibreActif);
        }
        public void UpdateFournisseurAutoInfolibreName(string FournisseurInfolibreName)
        {
            FournisseurAutoInfolibreName = SetValue<string>(ConfigFournisseurAutoInfolibreNameKey, FournisseurInfolibreName);
        }

        public void UpdateImportConditionnementActif(bool ConditionnementActif)
        {
            ArticleImportConditionnementActif = SetValue<bool>(ConfigImportConditionnementActifKey, ConditionnementActif);
        }
        public void UpdateConditionnementQuantiteToUPC(bool QuantiteToUPC)
        {
            ArticleConditionnementQuantiteToUPC = SetValue<bool>(ConfigConditionnementQuantiteToUPCKey, QuantiteToUPC);
        }
        public void UpdateLimiteStockConditionnement(bool LimiteConditionnement)
        {
            LimiteStockConditionnement = SetValue<bool>(ConfigLimiteStockConditionnementKey, LimiteConditionnement);
        }

        public void UpdateArticleContremarqueStockActif(bool StockContremarqueActif)
        {
            ArticleContremarqueStockActif = SetValue<bool>(ConfigArticleContremarqueStockKey, StockContremarqueActif);
        }
        public void UpdateArticleInfolibrePackaging(string InfolibrePackaging)
        {
            ArticleInfolibrePackaging = SetValue<string>(ConfigArticleInfolibrePackagingKey, InfolibrePackaging);
        }
        public void UpdateDeleteCatalogProductAssociation(bool DeleteAssociation)
        {
            DeleteCatalogProductAssociation = SetValue<bool>(ConfigDeleteCatalogProductAssociationKey, DeleteAssociation);
        }

        public void UpdateArticleDateDispoInfoLibreActif(bool DateDispoInfoLibreActif)
        {
            ArticleDateDispoInfoLibreActif = SetValue<bool>(ConfigArticleDateDispoInfoLibreActifKey, DateDispoInfoLibreActif);
        }
        public void UpdateArticleDateDispoInfoLibreName(string DateDispoInfoLibreName)
        {
            ArticleDateDispoInfoLibreName = SetValue<string>(ConfigArticleDateDispoInfoLibreNameKey, DateDispoInfoLibreName);
        }

        public void UpdateArticleQuantiteMiniActif(bool QuantiteMiniActif)
        {
            ArticleQuantiteMiniActif = SetValue<bool>(ConfigArticleQuantiteMiniActifKey, QuantiteMiniActif);
        }
        public void UpdateArticleQuantiteMiniConditionnement(bool QuantiteMiniConditionnement)
        {
            ArticleQuantiteMiniConditionnement = SetValue<bool>(ConfigArticleQuantiteMiniConditionnementKey, QuantiteMiniConditionnement);
        }
        public void UpdateArticleQuantiteMiniUniteVente(bool QuantiteMiniUniteVente)
        {
            ArticleQuantiteMiniUniteVente = SetValue<bool>(ConfigArticleQuantiteMiniUniteVenteKey, QuantiteMiniUniteVente);
        }

        public void UpdateArticleTransfertInfosFournisseurActif(bool InfosFournisseurActif)
        {
            ArticleTransfertInfosFournisseurActif = SetValue<bool>(ConfigArticleTransfertInfosFournisseurActifKey, InfosFournisseurActif);
        }

        public void UpdateArticleSuppressionAutoCaracteristique(bool SuppressionCaracteristique)
        {
            ArticleSuppressionAutoCaracteristique = SetValue<bool>(ConfigArticleSuppressionAutoCaracteristiqueKey, SuppressionCaracteristique);
        }

        public void UpdateArticleRedirectionCompositionActif(bool RedirectionComposition)
        {
            ArticleRedirectionCompositionActif = SetValue<bool>(ConfigArticleRedirectionCompositionActifKey, RedirectionComposition);
        }

        public void UpdateArticleStockNegatifZero(bool stocknegatif)
        {
            ArticleStockNegatifZero = SetValue<bool>(ConfigArticleStockNegatifZeroKey, stocknegatif);
        }
        public void UpdateArticleStockNegatifZeroParDepot(bool stocknegatifpardepot)
        {
            ArticleStockNegatifZeroParDepot = SetValue<bool>(ConfigArticleStockNegatifZeroParDepotKey, stocknegatifpardepot);
        }

        public void UpdateArticleSpecificPriceLetBasePriceRule(bool regleprixbase)
        {
            ArticleSpecificPriceLetBasePriceRule = SetValue<bool>(ConfigArticleSpecificPriceLetBasePriceRuleKey, regleprixbase);
        }

        #region Module AECStock

        public void UpdateModuleAECStockActif(bool AECStockActif)
        {
            ModuleAECStockActif = SetValue<bool>(ConfigModuleAECStockActifKey, AECStockActif);
        }

        #endregion

        #region Module AECAttributeList & AECAttributeStatut

        //public void UpdateAECAttributeListActif(bool AECAttributeListActif)
        //{
        //    ModuleAECAttributeListActif = SetValue<bool>(ConfigModuleAECAttributeListActifKey, AECAttributeListActif);
        //}
        //public void UpdateModuleAECAttributeStatutActif(bool AECAttributeStatutActif)
        //{
        //    ModuleAECAttributeStatutActif = SetValue<bool>(ConfigModuleAECAttributeStatutActifKey, AECAttributeStatutActif);
        //}
        #endregion

        public void UpdateConfigArticleCatComptable(int catcompta)
        {
            ConfigArticleCatComptable = SetValue<int>(ConfigArticleCatComptableKey, catcompta);
        }

		#region Module DWFProductGuiderates

		public void UpdateModuleDWFProductGuideratesActif(bool DWFProductGuideratesActif)
		{
			ModuleDWFProductGuideratesActif = SetValue<bool>(ConfigModuleDWFProductGuideratesActifKey, DWFProductGuideratesActif);
		}

		#endregion

		#region Module DWFProductExtraFields

		public void UpdateModuleDWFProductExtraFieldsActif(bool DWFProductExtraFieldsActif)
		{
			ModuleDWFProductExtraFieldsActif = SetValue<bool>(ConfigModuleDWFProductExtraFieldsActifKey, DWFProductExtraFieldsActif);
		}

		#endregion

        #endregion

        #region Taxes

        public void UpdateTaxSageTVA(TaxSage TaxSageTVAEnum)
        {
            TaxSageTVA = SetValue(ConfigTaxSageTVAKey, TaxSageTVAEnum);
        }

        public void UpdateTaxSageEco(TaxSage TaxSageEcoEnum)
        {
            TaxSageEco = SetValue(ConfigTaxSageEcoKey, TaxSageEcoEnum);
        }

        #endregion

        #region Client

        #region Création clients

        public void UpdateConfigClientFiltreCommande(bool ClientFiltreCommande)
        {
            ConfigClientFiltreCommande = SetValue<bool>(ConfigClientFiltreCommandeKey, ClientFiltreCommande);
        }

        public void UpdateConfigClientNumComposition(string Composition)
        {
            ConfigClientNumComposition = SetValue<string>(ConfigClientNumCompositionKey, Composition);
        }
        public void UpdateConfigClientNumPrefixe(string Prefixe)
        {
            ConfigClientNumPrefixe = SetValue<string>(ConfigClientNumPrefixeKey, Prefixe);
        }
        public void UpdateConfigClientNumLongueurNom(int LongueurNom)
        {
            ConfigClientNumLongueurNom = SetValue<int>(ConfigClientNumLongueurNomKey, LongueurNom);
        }
        public void UpdateConfigClientNumTypeNom(NameNumComponent TypeNom)
        {
            ConfigClientNumTypeNom = SetValue<NameNumComponent>(ConfigClientNumTypeNomKey, TypeNom);
        }
        public void UpdateConfigClientNumLongueurNumero(int LongueurNumero)
        {
            ConfigClientNumLongueurNumero = SetValue<int>(ConfigClientNumLongueurNumeroKey, LongueurNumero);
        }
        public void UpdateConfigClientNumTypeNumero(NumberNumComponent TypeNumero)
        {
            ConfigClientNumTypeNumero = SetValue<NumberNumComponent>(ConfigClientNumTypeNumeroKey, TypeNumero);
        }
        public void UpdateConfigClientNumTypeCompteur(CounterType TypeCompteur)
        {
            ConfigClientNumTypeCompteur = SetValue<CounterType>(ConfigClientNumTypeCompteurKey, TypeCompteur);
        }
        public void UpdateConfigClientNumDebutCompteur(int DebutCompteur)
        {
            ConfigClientNumDebutCompteur = SetValue<int>(ConfigClientNumDebutCompteurKey, DebutCompteur);
        }

        public void UpdateClientNumDepartementRemplacerCodeISO(bool DepartementRemplacerCodeISO)
        {
            ConfigClientNumDepartementRemplacerCodeISO = SetValue<bool>(ConfigClientNumDepartementRemplacerCodeISOKey, DepartementRemplacerCodeISO);
        }

        public void UpdateClientMultiMappageBtoB(bool MultiMappage)
        {
            ConfigClientMultiMappageBtoB = SetValue<bool>(ConfigClientMultiMappageBtoBKey, MultiMappage);
        }

        public void UpdateConfigClientCiviliteActif(bool Civilite)
        {
            ConfigClientCiviliteActif = SetValue<bool>(ConfigClientCiviliteActifKey, Civilite);
        }

        public void UpdateConfigClientSocieteIntituleActif(bool Societe)
        {
            ConfigClientSocieteIntituleActif = SetValue<bool>(ConfigClientSocieteIntituleActifKey, Societe);
        }
        public void UpdateConfigClientNIFActif(bool NIFActif)
        {
            ConfigClientNIFActif = SetValue<bool>(ConfigClientNIFActifKey, NIFActif);
        }
        public void UpdateConfigClientInfosMajusculeActif(bool InfosMajusculeActif)
        {
            ConfigClientInfosMajusculeActif = SetValue<bool>(ConfigClientInfosMajusculeActifKey, InfosMajusculeActif);
        }

        #endregion

        #region TransfertClient

        public void UpdateTransfertPrestashopCookieKey(string PrestashopCookieKey)
        {
            TransfertPrestashopCookieKey = SetValue<string>(ConfigTransfertPrestashopCookieKeyKey, PrestashopCookieKey);
        }

        public void UpdateTransfertPriceCategoryAvailable(List<int> PriceCategoryAvailable)
        {
            TransfertPriceCategoryAvailable = SetListValue(ConfigTransfertPriceCategoryAvailableKey, PriceCategoryAvailable);
        }
        public void UpdateTransfertMailAccountIdentification(MailAccountIdentification MailAccountIdentification)
        {
            TransfertMailAccountIdentification = SetValue<MailAccountIdentification>(ConfigTransfertMailAccountKey, MailAccountIdentification);
        }
        public void UpdateTransfertMailAccountAlternative(bool MailAccountAlternative)
        {
            TransfertMailAccountAlternative = SetValue<bool>(ConfigTransfertMailAccountAlternativeKey, MailAccountAlternative);
        }
        public void UpdateTransfertMailAccountContactService(int MailAccountContactService)
        {
            TransfertMailAccountContactService = SetValue<int>(ConfigTransfertMailAccountContactService, MailAccountContactService);
        }
        public void UpdateTransfertRandomPasswordLength(int RandomPasswordLength)
        {
            TransfertRandomPasswordLength = SetValue<int>(ConfigTransfertRandomPasswordLengthKey, RandomPasswordLength);
        }
        public void UpdateTransfertRandomPasswordIncludeSpecialCharacters(bool RandomPasswordIncludeSpecialCharacters)
        {
            TransfertRandomPasswordIncludeSpecialCharacters = SetValue<bool>(ConfigTransfertRandomPasswordIncludeSpecialCharactersKey, RandomPasswordIncludeSpecialCharacters);
        }

        public void UpdateTransfertNotifyAccountAddress(MailNotification NotifyAccountAddress)
        {
            TransfertNotifyAccountAddress = SetValue<MailNotification>(ConfigTransfertNotifyAccountAddressKey, NotifyAccountAddress);
        }
        public void UpdateTransfertNotifyAccountAdminCopy(bool NotifyAccountAdminCopy)
        {
            TransfertNotifyAccountAdminCopy = SetValue<bool>(ConfigTransfertNotifyAccountAdminCopyKey, NotifyAccountAdminCopy);
        }
        public void UpdateTransfertNotifyAccountDeliveryMethod(DeliveryMethod NotifyAccountDeliveryMethod)
        {
            TransfertNotifyAccountDeliveryMethod = SetValue<DeliveryMethod>(ConfigTransfertNotifyAccountDeliveryMethodKey, NotifyAccountDeliveryMethod);
        }
        public void UpdateTransfertNotifyAccountSageContactType(List<int> NotifyAccountSageContactType)
        {
            TransfertNotifyAccountSageContactType = SetListValue(ConfigTransfertNotifyAccountSageContactTypeKey, NotifyAccountSageContactType);
        }
        public void UpdateTransfertNotifyAccountSageContactService(List<int> NotifyAccountSageContactService)
        {
            TransfertNotifyAccountSageContactService = SetListValue(ConfigTransfertNotifyAccountSageContactServiceKey, NotifyAccountSageContactService);
        }

        public void UpdateTransfertSageAddressSend(List<int> SageAddressSend)
        {
            TransfertSageAddressSend = SetListValue(ConfigTransfertSageAddressSendKey, SageAddressSend);
        }
        public void UpdateTransfertLockPhoneNumber(List<int> LockPhoneNumber)
        {
            TransfertLockPhoneNumber = SetListValue(ConfigTransfertLockPhoneNumberKey, LockPhoneNumber);
        }
        public void UpdateTransfertLockPhoneNumberReplaceEntryValue(string LockPhoneNumberReplaceEntryValue)
        {
            TransfertLockPhoneNumberReplaceEntryValue = SetValue<string>(ConfigTransfertLockPhoneNumberReplaceEntryValueKey, LockPhoneNumberReplaceEntryValue);
        }
        public void UpdateTransfertAliasValue(AliasValue AliasValue)
        {
            TransfertAliasValue = SetValue<AliasValue>(ConfigTransfertAliasValueKey, AliasValue);
        }
        public void UpdateTransfertLastNameValue(LastNameValue LastNameValue)
        {
            TransfertLastNameValue = SetValue<LastNameValue>(ConfigTransfertLastNameValueKey, LastNameValue);
        }
        public void UpdateTransfertFirstNameValue(FirstNameValue FirstNameValue)
        {
            TransfertFirstNameValue = SetValue<FirstNameValue>(ConfigTransfertFirstNameValueKey, FirstNameValue);
        }
        public void UpdateTransfertCompanyValue(CompanyValue CompanyValue)
        {
            TransfertCompanyValue = SetValue<CompanyValue>(ConfigTransfertCompanyValueKey, CompanyValue);
        }
        public void UpdateTransfertClientSeparateurIntitule(string ClientSeparateurIntitule)
        {
            TransfertClientSeparateurIntitule = SetValue<string>(ConfigTransfertClientSeparateurIntituleKey, ClientSeparateurIntitule);
        }

        public void UpdateTransfertAccountActivation(bool AccountActivation)
        {
            TransfertAccountActivation = SetValue<bool>(ConfigTransfertAccountActivationKey, AccountActivation);
        }
        public void UpdateTransfertNewsLetterSuscribe(bool NewsLetterSuscribe)
        {
            TransfertNewsLetterSuscribe = SetValue<bool>(ConfigTransfertNewsLetterSuscribeKey, NewsLetterSuscribe);
        }
        public void UpdateTransfertOptInSuscribe(bool OptInSuscribe)
        {
            TransfertOptInSuscribe = SetValue<bool>(ConfigTransfertOptInSuscribeKey, OptInSuscribe);
        }

        public void UpdateTransfertIntegrateCustomerSynchronizationProcess(bool IntegrateCustomerSynchronizationProcess)
        {
            TransfertIntegrateCustomerSynchronizationProcess = SetValue<bool>(ConfigTransfertIntegrateCustomerSynchronizationProcessKey, IntegrateCustomerSynchronizationProcess);
        }
        public void UpdateTransfertSendAdminResultReport(bool SendAdminResultReport)
        {
            TransfertSendAdminResultReport = SetValue<bool>(ConfigTransfertSendAdminResultReportKey, SendAdminResultReport);
        }
        public void UpdateTransfertGenerateAccountFile(bool GenerateAccountFile)
        {
            TransfertGenerateAccountFile = SetValue<bool>(ConfigTransfertGenerateAccountFileKey, GenerateAccountFile);
        }

        public void UpdateRegexMailLevel(RegexMail RegexMailEnum)
        {
            RegexMailLevel = SetValue<RegexMail>(ConfigRegexMailLevelKey, RegexMailEnum);
        }

        public void UpdateTransfertNameIncludeNumbers(bool NameIncludeNumbers)
        {
            TransfertNameIncludeNumbers = SetValue<bool>(ConfigTransfertNameIncludeNumbersKey, NameIncludeNumbers);
        }

        #endregion

        #region Statistiques et informations libres Client

        public void UpdateStatInfolibreClientActif(bool statinfolibreactif)
        {
            StatInfolibreClientActif = SetValue<bool>(ConfigStatInfolibreClientActifKey, statinfolibreactif);
        }

        #endregion

        #region Module AECCollaborateur

        public void UpdateModuleAECCollaborateurActif(bool AECCollaborateurActif)
        {
            ModuleAECCollaborateurActif = SetValue<bool>(ConfigModuleAECCollaborateurActifKey, AECCollaborateurActif);
        }

        #endregion

        #region Module AECPaiement

        public void UpdateModuleAECPaiementActif(bool AECPaiementActif)
        {
            ModuleAECPaiementActif = SetValue<bool>(ConfigModuleAECPaiementActifKey, AECPaiementActif);
        }

        #endregion

        #region Module AECCustomerOutstanding

        public void UpdateModuleAECCustomerOutstandingActif(bool AECCustomerOutstandingActif)
        {
            ModuleAECCustomerOutstandingActif = SetValue<bool>(ConfigModuleAECCustomerOutstandingActifKey, AECCustomerOutstandingActif);
        }

        #endregion

        #region Module AECCustomerInfo

        public void UpdateModuleAECCustomerInfoActif(bool AECCustomerInfoActif)
        {
            ModuleAECCustomerInfoActif = SetValue<bool>(ConfigModuleAECCustomerInfoActifKey, AECCustomerInfoActif);
        }

        #endregion

        #region Module Portfolio Customer Employee

        public void UpdateModulePortfolioCustomerEmployeeActif(bool PortfolioActif)
        {
            ModulePortfolioCustomerEmployeeActif = SetValue<bool>(ConfigModulePortfolioCustomerEmployeeActifKey, PortfolioActif);
        }

        #endregion

        #endregion

        #region Adresses

        public void UpdateConfigClientAdresseTelephonePositionFixe(bool positionfixe)
        {
            ConfigClientAdresseTelephonePositionFixe = SetValue<bool>(ConfigClientAdresseTelephonePositionFixeKey, positionfixe);
        }

        #endregion
		
        #region Commandes

        public void UpdateConfigCommandeFiltreDate(DateTime? Date, bool isTemp = false)
        {
            // paramètre temporaire pour application du filtre date uniquement sur l'instance et les synchronisations en cours 
            if (isTemp)
                ConfigCommandeFiltreDate = Date;
            // écriture en base
            else
                ConfigCommandeFiltreDate = SetValue<DateTime?>(ConfigCommandeFiltreDateKey, Date);
        }

        #region Statuts

        public void UpdateConfigCommandeStatuts(int DE, int BC, int PL, int BL, int FA, int FC)
        {
            ConfigCommandeDE = SetValue<int>(ConfigCommandeDEKey, DE);
            ConfigCommandeBC = SetValue<int>(ConfigCommandeBCKey, BC);
            ConfigCommandePL = SetValue<int>(ConfigCommandePLKey, PL);
            ConfigCommandeBL = SetValue<int>(ConfigCommandeBLKey, BL);
            ConfigCommandeFA = SetValue<int>(ConfigCommandeFAKey, FA);
            ConfigCommandeFC = SetValue<int>(ConfigCommandeFCKey, FC);
        }

        public bool HasPrestaShopStateToChange()
        {
            return (ConfigCommandeDE != 0 || ConfigCommandeBC != 0 || ConfigCommandePL != 0 || ConfigCommandeBL != 0 || ConfigCommandeFA != 0 || ConfigCommandeFC != 0);
        }

        public void UpdateCommandeStatutJoursAutomate(int jours)
        {
            CommandeStatutJoursAutomate = SetValue<int>(ConfigCommandeStatutJoursAutomateKey, jours);
        }

        #endregion

        #region Tracking
        public void UpdateCommandeTrackingEnteteActif(bool UpdateTrackingEnteteActif)
        {
            CommandeTrackingEnteteActif = SetValue<bool>(ConfigCommandeTrackingEnteteActifKey, UpdateTrackingEnteteActif);
        }
        public void UpdateCommandeTrackingInfolibreActif(bool UpdateTrackingInfolibreActif)
        {
            CommandeTrackingInfolibreActif = SetValue<bool>(ConfigCommandeTrackingInfolibreActifKey, UpdateTrackingInfolibreActif);
        }
        public void UpdateCommandeTrackingEntete(FieldDocumentEntete TrackingEntete)
        {
            CommandeTrackingEntete = SetValue<FieldDocumentEntete>(ConfigCommandeTrackingEnteteKey, TrackingEntete);
        }
        public void UpdateCommandeTrackingInfolibre(string TrackingInfolibre)
        {
            CommandeTrackingInfolibre = SetValue<string>(ConfigCommandeTrackingInfolibreKey, TrackingInfolibre);
        }

        #endregion

        public void UpdateConfigLigneRemiseMode(LigneRemiseMode LigneRemise)
        {
            LigneRemiseMode = SetValue<LigneRemiseMode>(ConfigLigneRemiseModeKey, LigneRemise);
        }

        public void UpdateCommandeUpdateAdresseFacturation(bool UpdateAdresseFacturation)
        {
            CommandeUpdateAdresseFacturation = SetValue<bool>(ConfigCommandeUpdateAdresseFacturationKey, UpdateAdresseFacturation);
        }
        public void UpdateCommandeInsertFacturationEntete(bool InsertFacturationEntete)
        {
            CommandeInsertFacturationEntete = SetValue<bool>(ConfigCommandeInsertFacturationEnteteKey, InsertFacturationEntete);
        }

        public void UpdateCommandeReferencePrestashop(bool insertreferenceprestashop)
        {
            CommandeReferencePrestaShop = SetValue<bool>(ConfigCommandeReferencePrestaShopKey, insertreferenceprestashop);
        }
        public void UpdateCommandeNumeroFactureSageForceActif(bool NumeroFactureForce)
        {
            CommandeNumeroFactureSageForceActif = SetValue<bool>(ConfigCommandeNumeroFactureSageForceActifKey, NumeroFactureForce);
        }

        #region Frais de port

        public void UpdateConfigLigneFraisPort(bool LignePort)
        {
            LigneFraisPort = SetValue<bool>(ConfigLigneFraisPortKey, LignePort);
        }
        public void UpdateConfigLigneArticlePort(string ArticlePort)
        {
            LigneArticlePort = SetValue<string>(ConfigLigneArticlePortKey, ArticlePort);
        }

		#endregion

		#region Date de livraison

		public void UpdateConfigDateLivraisonMode(DateLivraisonMode dateLivraisonMode)
		{
			DateLivraisonMode = SetValue(ConfigDateLivraisonMode, dateLivraisonMode);
		}
		public void UpdateConfigDateLivraisonJours(int dateLivraisonJours)
		{
			DateLivraisonJours = SetValue(ConfigDateLivraisonJours, dateLivraisonJours);
		}

		#endregion

		#region Bons de réductions

		public void UpdateCommandeArticleReduction(string article)
        {
            CommandeArticleReduction = SetValue<string>(ConfigCommandeArticleReductionKey, article);
        }

        #endregion

        #region Commentaires

        public void UpdateConfigCommentaireBoutiqueActif(bool BoutiqueActif)
        {
            CommentaireBoutiqueActif = SetValue<bool>(ConfigCommentaireBoutiqueActifKey, BoutiqueActif);
        }
        public void UpdateConfigCommentaireBoutiqueTexte(string BoutiqueTexte)
        {
            CommentaireBoutiqueTexte = SetValue<string>(ConfigCommentaireBoutiqueTexteKey, BoutiqueTexte);
        }
        public void UpdateConfigCommentaireNumeroActif(bool NumeroActif)
        {
            CommentaireNumeroActif = SetValue<bool>(ConfigCommentaireNumeroActifKey, NumeroActif);
        }
        public void UpdateConfigCommentaireNumeroTexte(string NumeroTexte)
        {
            CommentaireNumeroTexte = SetValue<string>(ConfigCommentaireNumeroTexteKey, NumeroTexte);
        }
        public void UpdateConfigCommentaireReferencePaiementActif(bool RefPaiementActif)
        {
            CommentaireReferencePaiementActif = SetValue<bool>(ConfigCommentaireReferencePaiementActifKey, RefPaiementActif);
        }
        public void UpdateConfigCommentaireReferencePaiementTexte(string RefPaiementTexte)
        {
            CommentaireReferencePaiementTexte = SetValue<string>(ConfigCommentaireReferencePaiementTexteKey, RefPaiementTexte);
        }
        public void UpdateConfigCommentaireDateActif(bool DateActif)
        {
            CommentaireDateActif = SetValue<bool>(ConfigCommentaireDateActifKey, DateActif);
        }
        public void UpdateConfigCommentaireDateTexte(string DateTexte)
        {
            CommentaireDateTexte = SetValue<string>(ConfigCommentaireDateTexteKey, DateTexte);
        }

        public void UpdateConfigCommentaireClientActif(bool ClientActif)
        {
            CommentaireClientActif = SetValue<bool>(ConfigCommentaireClientActifKey, ClientActif);
        }
        public void UpdateConfigCommentaireClientTexte(string ClientTexte)
        {
            CommentaireClientTexte = SetValue<string>(ConfigCommentaireClientTexteKey, ClientTexte);
        }
        public void UpdateConfigCommentaireAdresseFacturationActif(bool AdresseFacturationActif)
        {
            CommentaireAdresseFacturationActif = SetValue<bool>(ConfigCommentaireAdresseFacturationActifKey, AdresseFacturationActif);
        }
        public void UpdateConfigCommentaireAdresseFacturationTexte(string AdresseFacturationTexte)
        {
            CommentaireAdresseFacturationTexte = SetValue<string>(ConfigCommentaireAdresseFacturationTexteKey, AdresseFacturationTexte);
        }
        public void UpdateConfigCommentaireAdresseLivraisonActif(bool AdresseLivraisonActif)
        {
            CommentaireAdresseLivraisonActif = SetValue<bool>(ConfigCommentaireAdresseLivraisonActifKey, AdresseLivraisonActif);
        }
        public void UpdateConfigCommentaireAdresseLivraisonTexte(string AdresseLivraisonTexte)
        {
            CommentaireAdresseLivraisonTexte = SetValue<string>(ConfigCommentaireAdresseLivraisonTexteKey, AdresseLivraisonTexte);
        }

        public void UpdateConfigCommentaireLibre1Actif(bool libre1)
        {
            CommentaireLibre1Actif = SetValue<bool>(ConfigCommentaireLibre1ActifKey, libre1);
        }
        public void UpdateConfigCommentaireLibre1Texte(string textelibre1)
        {
            CommentaireLibre1Texte = SetValue<string>(ConfigCommentaireLibre1TexteKey, textelibre1);
        }
        public void UpdateConfigCommentaireLibre2Actif(bool libre2)
        {
            CommentaireLibre2Actif = SetValue<bool>(ConfigCommentaireLibre2ActifKey, libre2);
        }
        public void UpdateConfigCommentaireLibre2Texte(string textelibre2)
        {
            CommentaireLibre2Texte = SetValue<string>(ConfigCommentaireLibre2TexteKey, textelibre2);
        }
        public void UpdateConfigCommentaireLibre3Actif(bool libre3)
        {
            CommentaireLibre3Actif = SetValue<bool>(ConfigCommentaireLibre3ActifKey, libre3);
        }
        public void UpdateConfigCommentaireLibre3Texte(string textelibre3)
        {
            CommentaireLibre3Texte = SetValue<string>(ConfigCommentaireLibre3TexteKey, textelibre3);
        }


        public void UpdateConfigCommentaireDebutDocument(bool debut)
        {
            CommentaireDebutDocument = SetValue<bool>(ConfigCommentaireDebutDocumentKey, debut);
        }
        public void UpdateConfigCommentaireFinDocument(bool fin)
        {
            CommentaireFinDocument = SetValue<bool>(ConfigCommentaireFinDocumentKey, fin);
        }

        #endregion

        public void UpdatePeriodiciteRelances(int premiereRelance, int deuxiemeRelance, int troisiemeRelance)
        {
            DureeJourAvantPremiereRelance = SetValue<int>(ConfigDureeJourAvantPremiereRelanceKey, premiereRelance);
            DureeJourApresPremiereRelance = SetValue<int>(ConfigDureeJourApresPremiereRelanceKey, deuxiemeRelance);
            DureeJourApresDeuxiemeRelance = SetValue<int>(ConfigDureeJourApresDeuxiemeRelanceKey, troisiemeRelance);
        }
        public void UpdateAnnulation(int annulation)
        {
            DureeJourAnnulationApresDerniereRelance = SetValue<int>(ConfigDureeJourAnnulationApresDerniereRelanceKey, annulation);
        }

        #region Module OleaPromo

        public void UpdateModuleOleaPromoActif(bool OleaPromoActif)
        {
            ModuleOleaPromoActif = SetValue<bool>(ConfigModuleOleaPromoActifKey, OleaPromoActif);
        }
        public void UpdateModuleOleaSuffixeGratuit(string OleaSuffixeGratuit)
        {
            ModuleOleaSuffixeGratuit = SetValue<string>(ConfigModuleOleaSuffixeGratuitKey, OleaSuffixeGratuit);

        }
        #endregion

        #region Module Preorder

        public void UpdateModulePreorderActif(bool PreorderActif)
        {
            ModulePreorderActif = SetValue<bool>(ConfigModulePreorderActifKey, PreorderActif);
        }
        public void UpdateModulePreorderInfolibreName(string InfolibreName)
        {
            ModulePreorderInfolibreName = SetValue<string>(ConfigModulePreorderInfolibreNameKey, InfolibreName);
        }
        public void UpdateModulePreorderInfolibreValue(string InfolibreValue)
        {
            ModulePreorderInfolibreValue = SetValue<string>(ConfigModulePreorderInfolibreValueKey, InfolibreValue);
        }
        public void UpdateModulePreorderPrestashopProduct(int Product)
        {
            ModulePreorderPrestashopProduct = SetValue<int>(ConfigModulePreorderPrestashopProductKey, Product);
        }
        public void UpdateModulePreorderPrestashopOrderState(int State)
        {
            ModulePreorderPrestashopOrderState = SetValue<int>(ConfigModulePreorderPrestashopOrderStateKey, State);
        }

        public void UpdateModulePreorderInfolibreEnteteName(string InfolibreName)
        {
            ModulePreorderInfolibreEnteteName = SetValue<string>(ConfigModulePreorderInfolibreEnteteNameKey, InfolibreName);
        }
        public void UpdateModulePreorderInfolibreEnteteValue(string InfolibreValue)
        {
            ModulePreorderInfolibreEnteteValue = SetValue<string>(ConfigModulePreorderInfolibreEnteteValueKey, InfolibreValue);
        }

        #endregion

        #region Module AECInvoiceHistory

        public void UpdateModuleAECInvoiceHistoryActif(bool AECInvoiceHistoryActif)
        {
            ModuleAECInvoiceHistoryActif = SetValue<bool>(ConfigModuleAECInvoiceHistoryActifKey, AECInvoiceHistoryActif);
        }
        public void UpdateModuleAECInvoiceHistoryInfoLibreClientSendMail(string InfoLibre)
        {
            ModuleAECInvoiceHistoryInfoLibreClientSendMail = SetValue<string>(ConfigModuleAECInvoiceHistoryInfoLibreClientSendMailKey, InfoLibre);
        }
        public void UpdateModuleAECInvoiceHistoryInfoLibreClientSendMailValue(string value)
        {
            ModuleAECInvoiceHistoryInfoLibreClientSendMailValue = SetValue(ConfigModuleAECInvoiceHistoryInfolibreClientSendMailValueKey, value);
        }

        public void UpdateModuleAECInvoiceHistoryArchivePDFActive(bool archive)
        {
            ModuleAECInvoiceHistoryArchivePDFActive = SetValue<bool>(ConfigModuleAECInvoiceHistoryArchivePDFActiveKey, archive);
        }
        public void UpdateModuleAECInvoiceHistoryArchivePDFFolder(string folder)
        {
            ModuleAECInvoiceHistoryArchivePDFFolder = SetValue<string>(ConfigModuleAECInvoiceHistoryArchivePDFFolderKey, folder);
        }

        #endregion

        public void UpdateCommandeArticlePackaging(string article)
        {
            CommandeArticlePackaging = SetValue<string>(ConfigCommandeArticlePackagingKey, article);
        }

        #region Module SoColissimo

        public void UpdateModuleSoColissimoDeliveryActive(bool active)
        {
            ModuleSoColissimoDeliveryActive = SetValue<bool>(ConfigModuleSoColissimoDeliveryActiveKey, active);
        }

        public void UpdateModuleSoColissimoInfolibreTypePointActive(bool active)
        {
            ModuleSoColissimoInfolibreTypePointActive = SetValue<bool>(ConfigModuleSoColissimoInfolibreTypePointActiveKey, active);
        }
        public void UpdateModuleSoColissimoInfolibreEnteteTypePointName(string InfolibreName)
        {
            ModuleSoColissimoInfolibreEnteteTypePointName = SetValue<string>(ConfigModuleSoColissimoInfolibreEnteteTypePointNameKey, InfolibreName);
        }

        public void UpdateModuleSoColissimoInfolibreDestinataireActive(bool active)
        {
            ModuleSoColissimoInfolibreDestinataireActive = SetValue<bool>(ConfigModuleSoColissimoInfolibreDestinataireActiveKey, active);
        }
        public void UpdateModuleSoColissimoInfolibreEnteteDestinataireName(string InfolibreName)
        {
            ModuleSoColissimoInfolibreEnteteDestinataireName = SetValue<string>(ConfigModuleSoColissimoInfolibreEnteteDestinataireNameKey, InfolibreName);
        }

        public void UpdateModuleSoColissimoReplacePhoneActive(bool active)
        {
            ModuleSoColissimoReplacePhoneActive = SetValue<bool>(ConfigModuleSoColissimoReplacePhoneActiveKey, active);
        }
        public void UpdateModuleSoColissimoReplaceAddressNameActive(bool active)
        {
            ModuleSoColissimoReplaceAddressNameActive = SetValue<bool>(ConfigModuleSoColissimoReplaceAddressNameActiveKey, active);
        }

        #endregion

        #endregion

        #region Reglement

        public void UpdateSyncReglementActif(bool reglementactif)
        {
            SyncReglementActif = SetValue<bool>(ConfigSyncReglementActifKey, reglementactif);
        }

        public void UpdateModeReglementEcheancierActif(bool echeancieractif)
        {
            ModeReglementEcheancierActif = SetValue<bool>(ConfigModeReglementEcheancierActifKey, echeancieractif);
        }

        public void UpdateReglementLibellePartielActif(bool libellepartiel)
        {
            ReglementLibellePartielActif = SetValue<bool>(ConfigReglementLibellePartielActifKey, libellepartiel);
        }

        #endregion

        #region Mail

        #region Identification

        public void UpdateConfigMailActive(bool MailActive)
        {
            ConfigMailActive = SetValue<bool>(ConfigMailActiveKey, MailActive);
        }
        public void UpdateConfigMailUser(string MailUser)
        {
            ConfigMailUser = SetValue<string>(ConfigMailUserKey, MailUser);
        }
        public void UpdateConfigMailPassword(string MailPassword)
        {
            ConfigMailPassword = SetValue<string>(ConfigMailPasswordKey, MailPassword);
        }
        public void UpdateConfigMailSMTP(string MailSMTP)
        {
            ConfigMailSMTP = SetValue<string>(ConfigMailSMTPKey, MailSMTP);
        }
        public void UpdateConfigMailPort(int MailPort)
        {
            ConfigMailPort = SetValue<int>(ConfigMailPortKey, MailPort);
        }
        public void UpdateConfigMailSSL(bool MailSSL)
        {
            ConfigMailSSL = SetValue<bool>(ConfigMailSSLKey, MailSSL);
        }

        #endregion

        #endregion

        #region Import

        public void UpdateConfigAutomaticImportFolderPicture(string ImportFolderPicture)
        {
            AutomaticImportFolderPicture = SetValue<string>(ConfigAutomaticImportFolderPictureKey, ImportFolderPicture);
        }
        public void UpdateConfigAutomaticImportFolderDocument(string ImportFolderDocument)
        {
            AutomaticImportFolderDocument = SetValue<string>(ConfigAutomaticImportFolderDocumentKey, ImportFolderDocument);
        }
        public void UpdateConfigAutomaticImportFolderMedia(string ImportFolderMedia)
        {
            AutomaticImportFolderMedia = SetValue<string>(ConfigAutomaticImportFolderMediaKey, ImportFolderMedia);
        }
        public void UpdateConfigImportMediaIncludePictures(bool MediaIncludePictures)
        {
            ImportMediaIncludePictures = SetValue<bool>(ConfigImportMediaIncludePicturesKey, MediaIncludePictures);
        }
        public void UpdateConfigImportMediaAutoDeleteAttachment(bool MediaAutoDeleteAttachment)
        {
            ImportMediaAutoDeleteAttachment = SetValue<bool>(ConfigImportMediaAutoDeleteAttachmentKey, MediaAutoDeleteAttachment);
        }
        public void UpdateConfigImportImageReplaceFiles(bool ImageReplaceFiles)
        {
            ImportImageReplaceFiles = SetValue<bool>(ConfigImportImageReplaceFilesKey, ImageReplaceFiles);
        }
        public void UpdateConfigImportImageRemoveDeletedFiles(bool ImageRemoveDeletedFiles)
        {
            ImportImageRemoveDeletedFiles = SetValue<bool>(ConfigImportImageRemoveDeletedFilesKey, ImageRemoveDeletedFiles);
        }

        public void UpdateConfigImportImageSearchReferenceClient(bool includereferenceclient)
        {
            ImportImageSearchReferenceClient = SetValue<bool>(ConfigImportImageSearchReferenceClientKey, includereferenceclient);
        }

        public void UpdateConfigImportImageUseSageDatas(bool usesagedatas)
        {
            ImportImageUseSageDatas = SetValue<bool>(ConfigImportImageUseSageDatasKey, usesagedatas);
        }

        #endregion

        #region Outils

        public void UpdateConfigCronSynchroArticleURL(string CronURL)
        {
            ConfigCronSynchroArticleURL = SetValue<string>(ConfigCronSynchroArticleURLKey, CronURL);
        }

        public void UpdateConfigRefreshTempCustomerListDisabled(bool refreshdisabled)
        {
            ConfigRefreshTempCustomerListDisabled = SetValue<bool>(ConfigRefreshTempCustomerListDisabledKey, refreshdisabled);
        }

        public void UpdateConfigUnlockProcessorCore(bool unlockcores)
        {
            ConfigUnlockProcessorCore = SetValue<bool>(ConfigUnlockProcessorCoreKey, unlockcores);
        }

        public void UpdateConfigAllocatedProcessorCore(int cores)
        {
            ConfigAllocatedProcessorCore = SetValue<int>(ConfigAllocatedProcessorCoreKey, cores);
        }

        public void UpdateCrystalForceConnectionInfoOnSubReports(bool forcesubreport)
        {
            CrystalForceConnectionInfoOnSubReports = SetValue<bool>(ConfigCrystalForceConnectionInfoOnSubReportsKey, forcesubreport);
        }

        #endregion

        #region Interface/UI

        public void UpdateUIProductFilterActiveDefault(ProductFilterActiveDefault Filter)
        {
            UIProductFilterActiveDefault = SetValue<ProductFilterActiveDefault>(UIProductFilterActiveDefaultKey, Filter);
        }
        public void UpdateUIProductUpdateValidationDisabled(bool updatevalidation)
        {
            UIProductUpdateValidationDisabled = SetValue<bool>(UIProductUpdateValidationDisabledKey, updatevalidation);
        }

        public void UpdateUIMaximizeWindow(bool maximize)
        {
            UIMaximizeWindow = SetValue<bool>(UIMaximizeWindowKey, maximize);
        }

        public void UpdateUISleepTimeWYSIWYG(int SleepTime)
        {
            UISleepTimeWYSIWYG = SetValue<int>(UISleepTimeWYSIWYGKey, SleepTime);
        }
        public void UpdateUIDisabledWYSIWYG(bool disable)
        {
            UIDisabledWYSIWYG = SetValue<bool>(UIDisabledWYSIWYGKey, disable);
        }

        public void UpdateUIIE11EmulationModeDisabled(bool ie11emulationdisabled)
        {
            UIIE11EmulationModeDisabled = SetValue<bool>(UIIE11EmulationModeDisabledKey, ie11emulationdisabled);
        }

        public void UpdateListeArticleColonneIDVisible(bool visible)
        {
            ListeArticleColonneIDVisible = SetValue<bool>(ListeArticleColonneIDVisibleKey, visible);
        }
        public void UpdateListeArticleColonneTypeVisible(bool visible)
        {
            ListeArticleColonneTypeVisible = SetValue<bool>(ListeArticleColonneTypeVisibleKey, visible);
        }
        public void UpdateListeArticleColonneNomVisible(bool visible)
        {
            ListeArticleColonneNomVisible = SetValue<bool>(ListeArticleColonneNomVisibleKey, visible);
        }
        public void UpdateListeArticleColonneReferenceVisible(bool visible)
        {
            ListeArticleColonneReferenceVisible = SetValue<bool>(ListeArticleColonneReferenceVisibleKey, visible);
        }
        public void UpdateListeArticleColonneEANVisible(bool visible)
        {
            ListeArticleColonneEANVisible = SetValue<bool>(ListeArticleColonneEANVisibleKey, visible);
        }
        public void UpdateListeArticleColonneActifVisible(bool visible)
        {
            ListeArticleColonneActifVisible = SetValue<bool>(ListeArticleColonneActifVisibleKey, visible);
        }
        public void UpdateListeArticleColonneSyncVisible(bool visible)
        {
            ListeArticleColonneSyncVisible = SetValue<bool>(ListeArticleColonneSyncVisibleKey, visible);
        }
        public void UpdateListeArticleColonneSyncPriceVisible(bool visible)
        {
            ListeArticleColonneSyncPriceVisible = SetValue<bool>(ListeArticleColonneSyncPriceVisibleKey, visible);
        }
        public void UpdateListeArticleColonneDateVisible(bool visible)
        {
            ListeArticleColonneDateVisible = SetValue<bool>(ListeArticleColonneDateVisibleKey, visible);
        }

        #endregion

        #region ReimporSage

        public void UpdateReimportUpdateMainCatalog(bool value)
        {
            ReimportUpdateMainCatalog = SetValue<bool>(ReimportUpdateMainCatalogKey, value);
        }
        public void UpdateReimportLinkParents(bool value)
        {
            ReimportLinkParents = SetValue<bool>(ReimportLinkParentsKey, value);
        }
        public void UpdateReimportDeleteLinkOldMain(bool value)
        {
            ReimportDeleteLinkOldMain = SetValue<bool>(ReimportDeleteLinkOldMainKey, value);
        }
        public void UpdateReimportDeleteLinkOldSecondary(bool value)
        {
            ReimportDeleteLinkOldSecondary = SetValue<bool>(ReimportDeleteLinkOldSecondaryKey, value);
        }

        public void UpdateReimportUpdateProductName(bool value)
        {
            ReimportUpdateProductName = SetValue<bool>(ReimportUpdateProductNameKey, value);
        }
        public void UpdateReimportUpdateDescriptionShort(bool value)
        {
            ReimportUpdateDescriptionShort = SetValue<bool>(ReimportUpdateDescriptionShortKey, value);
        }
        public void UpdateReimportUpdateDescription(bool value)
        {
            ReimportUpdateDescription = SetValue<bool>(ReimportUpdateDescriptionKey, value);
        }
        public void UpdateReimportUpdateMetaTitle(bool value)
        {
            ReimportUpdateMetaTitle = SetValue<bool>(ReimportUpdateMetaTitleKey, value);
        }
        public void UpdateReimportUpdateMetaDescription(bool value)
        {
            ReimportUpdateMetaDescription = SetValue<bool>(ReimportUpdateMetaDescriptionKey, value);
        }
        public void UpdateReimportUpdateMetaKeywords(bool value)
        {
            ReimportUpdateMetaKeywords = SetValue<bool>(ReimportUpdateMetaKeywordsKey, value);
        }
        public void UpdateReimportUpdateURL(bool value)
        {
            ReimportUpdateURL = SetValue<bool>(ReimportUpdateURLKey, value);
        }
        public void UpdateReimportUpdateEAN(bool value)
        {
            ReimportUpdateEAN = SetValue<bool>(ReimportUpdateEANKey, value);
        }
        public void UpdateReimportUpdateActive(bool value)
        {
            ReimportUpdateActive = SetValue<bool>(ReimportUpdateActiveKey, value);
        }

        public void UpdateReimportUpdateCharacteristic(bool value)
        {
            ReimportUpdateCharacteristic = SetValue<bool>(ReimportUpdateCharacteristicKey, value);
        }
        public void UpdateReimportUpdateAttribute(bool value)
        {
            ReimportUpdateAttribute = SetValue<bool>(ReimportUpdateAttributeKey, value);
        }
        public void UpdateReimportUpdateConditioning(bool value)
        {
            ReimportUpdateConditioning = SetValue<bool>(ReimportUpdateConditioningKey, value);
        }

        public void UpdateReimportUpdateDateActive(bool value)
        {
            ReimportUpdateDateActive = SetValue<bool>(ReimportUpdateDateActiveKey, value);
        }

        #endregion

        #region Chronos

        public void UpdateChronoSynchroStockPriceActif(bool ChronoStockPrice)
        {
            ChronoSynchroStockPriceActif = SetValue<bool>(ChronoSynchroStockPriceActifKey, ChronoStockPrice);
        }

        #endregion

        #region Cron/Scripts

        public void UpdateCronArticleURL(string url)
        {
            CronArticleURL = SetValue<string>(CronArticleURLKey, url);
        }
        public void UpdateCronArticleBalise(string balise)
        {
            CronArticleBalise = SetValue<string>(CronArticleBaliseKey, balise);
        }
        public void UpdateCronArticleTimeout(int timeout)
        {
            CronArticleTimeout = SetValue<int>(CronArticleTimeoutKey, timeout);
		}
		public void UpdateCronCommandeURL(string url)
		{
			CronCommandeURL = SetValue<string>(CronCommandeURLKey, url);
		}
		public void UpdateCronCommandeBalise(string balise)
		{
			CronCommandeBalise = SetValue<string>(CronCommandeBaliseKey, balise);
		}
		public void UpdateCronCommandeTimeout(int timeout)
		{
			CronCommandeTimeout = SetValue<int>(CronCommandeTimeoutKey, timeout);
		}

        #endregion

        #endregion

        #region Default

        private string GetDefaultSageMediaFolder()
        {
            string f = string.Empty;
            String rep = "Users\\Public\\Documents\\Sage\\Sage Entreprise";
            string windrive = Path.GetPathRoot(Environment.SystemDirectory); // C:\
            rep = Path.Combine(windrive, rep);
            if (Directory.Exists(rep))
                f = rep;
            return f;
        }

        #endregion
    }
}