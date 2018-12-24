using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PRESTACONNECT.Core
{
    public static class Task
    {
        #region Arguments

        public enum Tasks
        {
            ExecSQL,

            SynchroStock,
            SynchroStockPrice,
            SynchroPrice,
            SynchroClient,
            SynchroArticle,
            SynchroGamme,
            SynchroConditionnement,
            SynchroCatalogue,
            SynchroFournisseur,
            SynchroDocument,
            SynchroDocumentSageJour,
            SynchroDocumentSageSemaine,
            SynchroDocumentSageMois,
            SynchroStatutCommande,
            SynchroStatutCommandeNJours,
            SynchroGlobale,

            SynchroImageMedia,
            GestionStatutArticle,

            SynchroGroupCatTarif,
            SynchroEncoursClient,
            SynchroGroupCodeRisque,
            SynchroPortfolioCustomerEmployee,

            RelanceAnnulationCommandePrestashop,

            SynchronisationPaiementDiffere,
            SynchroPaiementJour,
            SynchroPaiementSemaine,
            SynchroPaiementMois,

            ImportSageCatalogue,
            ImportSageArticle,
            ImportCompositionGammes,
            ImportImage,
            ImportDocument,
            ImportSageMedia,
            ImportSageComplet,
            TransfertSageClient,

            ImportSageStatInfolibreArticle,
            ImportSageStatInfolibreClient,
            ImportSageCatalogueInfolibre,

            TransfertAttribute,
            TransfertFeature,

            ImportPrestashopCaracteristiqueArticle,

            ExportFacture,
            ExportFactureSemaine,
            ExportFactureMois,
            ExportFactureAnnee,

            ExportCollaborateur,
            ExportPayement,
            ExportInfosClient,

            ExportAECRepresentative,
            ExportAECBalance,
        }

        public const String TaskExecSQL = "ExecSQL";

        public const String TaskSynchroStock = "SynchroStock";
        public const String TaskSynchroStockPrice = "SynchroStockPrice";
        public const String TaskSynchroPrice = "SynchroPrice";
        public const String TaskSynchroClient = "SynchroClient";
        public const String TaskSynchroArticle = "SynchroArticle";
        public const String TaskSynchroGamme = "SynchroGamme";
        public const String TaskSynchroConditionnement = "SynchroConditionnement";
        public const String TaskSynchroCatalogue = "SynchroCatalogue";
        public const String TaskSynchroFournisseur = "SynchroFournisseur";
        public const String TaskSynchroDocument = "SynchroDocument";
        public const String TaskSynchroDocumentSageJour = "DocumentSageJour";
        public const String TaskSynchroDocumentSageSemaine = "DocumentSageSemaine";
        public const String TaskSynchroDocumentSageMois = "DocumentSageMois";
        public const String TaskSynchroStatutCommande = "SynchroStatutCommande";
        public const String TaskSynchroStatutCommandeNJours = "SynchroStatutCommandeNJours";
        public const String TaskSynchroGlobale = "SynchroGlobale";

        public const String TaskSynchroImageMedia = "SynchroImageMedia";
        public const String TaskRelanceAnnulationCommandePrestashop = "RelanceAnnulationCommandePrestashop";
        public const String TaskGestionStatutArticle = "GestionStatutArticle";

        public const String TaskSynchronisationPaiementDiffere = "SynchronisationPaiementDiffere";
        public const String TaskSynchroPaiementJour = "SynchroPaiementJour";
        public const String TaskSynchroPaiementSemaine = "SynchroPaiementSemaine";
        public const String TaskSynchroPaiementMois = "SynchroPaiementMois";

        public const String TaskSynchroGroupCatTarif = "SynchroGroupCatTarif";
        public const String TaskSynchroEncoursClient = "SynchroEncoursClient";
        public const String TaskSynchroGroupCodeRisque = "SynchroGroupCodeRisque";
        public const String TaskSynchroPortfolioCustomerEmployee = "SynchroPortfolioCustomerEmployee";

        public const String TaskImportSageCatalogue = "ImportSageCatalogue";
        public const String TaskImportSageArticle = "ImportSageArticle";
        public const String TaskImportCompositionGammes = "ImportCompositionGammes";
        public const String TaskImportImage = "ImportImage";
        public const String TaskImportDocument = "ImportDocument";
        public const String TaskImportSageMedia = "ImportSageMedia";
        public const String TaskImportSageComplet = "ImportSageComplet";
        public const String TaskTransfertSageClient = "TransfertSageClient";

        public const String TaskImportSageStatInfolibreArticle = "ImportSageStatInfolibreArticle";
        public const String TaskImportSageStatInfolibreClient = "ImportSageStatInfolibreClient";
        public const String TaskImportSageCatalogueInfolibre = "ImportSageCatalogueInfolibre";

        public const String TaskTransfertAttribute = "TransfertAttribute";
        public const String TaskTransfertFeature = "TransfertFeature";

        public const String TaskImportPrestashopCaracteristiqueArticle = "ImportPrestashopCaracteristiqueArticle";

        public const String TaskExportFacture = "ExportFacture";
        public const String TaskExportFactureSemaine = "ExportFactureSemaine";
        public const String TaskExportFactureMois = "ExportFactureMois";
        public const String TaskExportFactureAnnee = "ExportFactureAnnee";

        public const String TaskExportCollaborateur = "ExportCollaborateur";
        public const String TaskExportPayement = "ExportPayement";
        public const String TaskExportInfosClient = "ExportInfosClient";

        public const String TaskExportAECBalance = "ExportAECBalance";
        public const String TaskExportAECRepresentative = "ExportAECRepresentative";

        #endregion

        #region SQL

        public static void ExecSQL()
        {
            if (Directory.Exists(Core.Global.SQLPath))
            {
                String[] ArrayFile = System.IO.Directory.GetFiles(Core.Global.SQLPath);
                foreach (String file in ArrayFile)
                {
                    if (file.EndsWith(".sql"))
                    {
                        StreamReader Reader = new StreamReader(file);
                        String TxtSQL = Reader.ReadToEnd().Replace("\n", " ").Replace("\r", " ").Replace("\t", " ");
                        Model.Local.DataClassesLocalDataContext DBLocal = new Model.Local.DataClassesLocalDataContext();
                        DBLocal.ExecuteCommand(TxtSQL);
                    }
                }
            }
        }

        #endregion

        #region Sync

        public static void ExecArgsSynchroGlobale()
        {
            ExecArgsArticle();
            ExecArgsDocument();
        }

        public static void ExecArgsStockPrice()
        {
            TransfertStockPrice form = new TransfertStockPrice();
            form.ShowDialog();
        }

        // <JG> 04/12/2012 Synchro stock seule
        public static void ExecArgsStock()
        {
            TransfertStock form = new TransfertStock();
            form.ShowDialog();
        }

        public static void ExecArgsClient(DateTime? filtre = null)
        {
            // update temporaire du filtre date en fonction de l'argument en paramètre
            if (filtre != null)
                Core.Global.GetConfig().UpdateConfigCommandeFiltreDate(filtre, true);

            if (Core.Global.GetConfig().ConfigBToC)
            {
                PRESTACONNECT.SynchronisationClient SynchronisationClient = new SynchronisationClient();
                SynchronisationClient.ShowDialog();
            }
            PRESTACONNECT.SynchronisationLivraison SynchronisationLivraison = new SynchronisationLivraison();
            SynchronisationLivraison.ShowDialog();
        }

        public static void ExecArgsArticle()
        {
            ExecArgsFournisseur();
            ExecArgsCatalogue();
            ExecArgsGamme();
            ExecArgsConditionnement();

            //Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
            //List<Int32> ListArticle = ArticleRepository.ListIdSync(true);

            //foreach (Int32 Article in ListArticle)
            //{
            //    Core.Sync.SynchronisationArticle Sync = new Core.Sync.SynchronisationArticle();
            //    Sync.Exec(Article);
            //}

            SynchronisationArticle formsync = new SynchronisationArticle();
            formsync.ShowDialog();

            // <JG> 16/08/2013 ajout cron update product index
            if (Core.Temp.SyncArticle_LaunchIndex)
                Core.Global.LaunchCron(Core.Global.GetConfig().ConfigCronSynchroArticleURL);
            Core.Temp.SyncArticle_LaunchIndex = false;

            ExecArgsSynchroImageMedia();

            // désactivation
            //ExecArgsStockPrice();
        }

        public static void ExecArgsSynchroImageMedia()
        {
            if (Core.Global.GetConfig().ConfigFTPActive)
            {
                if (!string.IsNullOrEmpty(Core.Global.GetConfig().ConfigFTPIP)
                    && !string.IsNullOrEmpty(Core.Global.GetConfig().ConfigFTPUser)
                    && !string.IsNullOrEmpty(Core.Global.GetConfig().ConfigFTPPassword))
                {
                    Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                    List<Int32> ListArticle = ArticleRepository.ListIdSync(true);

                    TransfertArticleImage Sync = new TransfertArticleImage(ListArticle);
                    Sync.ShowDialog();
                    TransfertArticleDocument SyncArticleDocument = new TransfertArticleDocument(ListArticle);
                    SyncArticleDocument.ShowDialog();
                }
            }
        }

        public static void ExecArgsGamme()
        {
            // <JG> 18/09/2012 modification synchronisation des énumérés de gamme
            //Model.Local.AttributeGroupRepository CombinationRepository = new Model.Local.AttributeGroupRepository();
            //List<Int32> ListCombination = CombinationRepository.ListIdSync(true);
            //foreach (Int32 Combination in ListCombination)
            //{
            //    Core.Sync.SynchronisationGamme SynchronisationGamme = new Core.Sync.SynchronisationGamme();
            //    SynchronisationGamme.Exec(Combination);

            //    List<Int32> ListAttribute = new List<Int32>();
            //    Model.Local.AttributeRepository AttributeRepository = new Model.Local.AttributeRepository();
            //    ListAttribute = AttributeRepository.ListIDCombination(Combination);
            //    foreach (Int32 Attribute in ListAttribute)
            //    {
            //        SynchronisationGamme.ExecAttribute(Attribute);
            //    }
            //}
        }

        public static void ExecArgsConditionnement()
        {
            // <JG> 18/09/2012 modification synchronisation des énumérés de gamme
            //Model.Local.ConditioningGroupRepository ConditioningGroupRepository = new Model.Local.ConditioningGroupRepository();
            //List<Int32> ListConditioningGroup = ConditioningGroupRepository.ListId();
            //foreach (Int32 ConditioningGroup in ListConditioningGroup)
            //{
            //    Core.Sync.SynchronisationConditionnement SynchronisationConditionnement = new Core.Sync.SynchronisationConditionnement();
            //    SynchronisationConditionnement.Exec(ConditioningGroup);

            //    List<Int32> ListConditionning = new List<Int32>();
            //    Model.Local.ConditioningRepository ConditioningRepository = new Model.Local.ConditioningRepository();
            //    ListConditionning = ConditioningRepository.ListIDConditioningGroup(ConditioningGroup);
            //    foreach (Int32 Conditionning in ListConditionning)
            //    {
            //        SynchronisationConditionnement.ExecConditioning(Conditionning);
            //    }
            //}
        }

        public static void ExecArgsFournisseur()
        {
            SynchronisationFournisseur SyncFournisseur = new SynchronisationFournisseur();
            SyncFournisseur.ShowDialog();
        }

        public static void ExecArgsCatalogue()
        {
            Model.Local.CatalogRepository CatalogRepository = new Model.Local.CatalogRepository();
            List<Int32> ListCatalog = CatalogRepository.ListIdSyncOrderByLevel(true);

            foreach (Int32 Catalog in ListCatalog)
            {
                Core.Sync.SynchronisationCatalogue Sync = new Core.Sync.SynchronisationCatalogue();
                Sync.Exec(Catalog);
            }

            if (Core.Temp.SyncCatalogue_ClearSmartyCache)
                Core.Global.LaunchAlternetis_ClearSmartyCache();
            if (Core.Temp.SyncCatalogue_RegenerateTree)
                Core.Global.LaunchAlternetis_RegenerateCategoryTree();

            Core.Temp.SyncCatalogue_ClearSmartyCache = false;
            Core.Temp.SyncCatalogue_RegenerateTree = false;
        }

        public static void ExecArgsDocument()
        {
            ExecArgsDocumentSage();
            ExecArgsStatutCommande();
        }

        public static void ExecArgsDocumentSage(DateTime? filtre = null)
        {
            ExecArgsClient(filtre);

            PRESTACONNECT.SynchronisationCommande Sync = new SynchronisationCommande(filtre);
            Sync.ShowDialog();
        }

        public static void ExecArgsStatutCommande()
        {
            PRESTACONNECT.SynchronisationStatutCommande SyncState = new SynchronisationStatutCommande();
            SyncState.ShowDialog();
        }

        public static void ExecArgsStatutCommandeNJours()
        {
            PRESTACONNECT.SynchronisationStatutCommande SyncState = new SynchronisationStatutCommande(Core.Global.GetConfig().CommandeStatutJoursAutomate);
            SyncState.ShowDialog();
        }

        public static void ExecArgsRelanceAnnulationCommandePrestashop()
        {
            Model.Prestashop.PsOrdersRepository PsOrdersRepository = new Model.Prestashop.PsOrdersRepository();
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            Model.Local.OrderMailRepository OrderMailRepository = new Model.Local.OrderMailRepository();

            AppConfig config = Global.GetConfig();

            Model.Local.Config RelanceConfig = new Model.Local.Config();
            Model.Local.Config AnnulationConfig = new Model.Local.Config();
            String[] ArrayRelanceConfig = null;
            String[] ArrayAnnulationConfig = null;

            RelanceConfig = ConfigRepository.ReadName(Core.Global.ConfigCommandeRelance);
            AnnulationConfig = ConfigRepository.ReadName(Core.Global.ConfigCommandeAnnulation);

            if (RelanceConfig != null)
                ArrayRelanceConfig = RelanceConfig.Con_Value.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);

            if (AnnulationConfig != null)
                ArrayAnnulationConfig = AnnulationConfig.Con_Value.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);


            int relanceCount = 0;
            if (config.DureeJourAvantPremiereRelance > 0)
            {
                relanceCount = 1;
                if (config.DureeJourApresPremiereRelance > 0)
                {
                    relanceCount = 2;
                    if (config.DureeJourApresDeuxiemeRelance > 0)
                        relanceCount = 3;
                }
            }

            if (ArrayRelanceConfig.Length > 0 || ArrayAnnulationConfig.Length > 0)
                foreach (var order in PsOrdersRepository.List(Global.CurrentShop.IDShop))
                {
                    Model.Prestashop.PsOrderHistoryRepository PsOrderHistoryRepository = new Model.Prestashop.PsOrderHistoryRepository();
                    List<Model.Prestashop.PsOrderHistory> ListPsOrderHistory = PsOrderHistoryRepository.ListOrder(order.IDOrder);

                    foreach (var state in ArrayRelanceConfig)
                    {
                        uint stateId = Convert.ToUInt32(state);
                        int statecount = ListPsOrderHistory.Count(result => result.IDOrderState == stateId);
                        Model.Prestashop.PsOrderHistory last = ListPsOrderHistory
                            .OrderByDescending(result => result.DateAdd).FirstOrDefault();
                        int mailId = (statecount == 1) ? 19 : (statecount == 2) ? 28 : 29;

                        if (last != null && last.IDOrderState == stateId)
                        {
                            if (((statecount == 1) && (config.DureeJourAvantPremiereRelance > 0) && ((DateTime.Now.Date - last.DateAdd.Date).TotalDays >= config.DureeJourAvantPremiereRelance)) ||
                                ((statecount == 2) && (config.DureeJourApresPremiereRelance > 0) && ((DateTime.Now.Date - last.DateAdd.Date).TotalDays >= config.DureeJourApresPremiereRelance)) ||
                                ((statecount == 3) && (config.DureeJourApresDeuxiemeRelance > 0) && ((DateTime.Now.Date - last.DateAdd.Date).TotalDays >= config.DureeJourApresDeuxiemeRelance)))
                            {
                                Model.Local.OrderMail OrderMail = OrderMailRepository.ReadType(mailId);
                                if (OrderMail != null && OrderMail.OrdMai_Active)
                                {
                                    PRESTACONNECT.Core.Sync.SynchronisationCommande.SendMail(mailId, order);

                                    Model.Prestashop.PsOrderHistory PsOrderHistory = new Model.Prestashop.PsOrderHistory();

                                    PsOrderHistory.IDOrder = Convert.ToUInt32(order.IDOrder);
                                    PsOrderHistory.IDOrderState = Convert.ToUInt32(stateId);
                                    PsOrderHistory.DateAdd = DateTime.Now;
                                    PsOrderHistoryRepository.Add(PsOrderHistory);
                                }
                            }
                        }
                    }

                    foreach (var state in ArrayAnnulationConfig)
                    {
                        uint stateId = Convert.ToUInt32(state);
                        int statecount = ListPsOrderHistory.Count(result => result.IDOrderState == stateId);
                        Model.Prestashop.PsOrderHistory last = ListPsOrderHistory
                            .OrderByDescending(result => result.DateAdd).FirstOrDefault();
                        int mailId = 32;

                        if (last != null && last.IDOrderState == stateId)
                        {
                            if ((statecount == relanceCount + 1) && (config.DureeJourAnnulationApresDerniereRelance > 0) && ((DateTime.Now.Date - last.DateAdd.Date).TotalDays >= config.DureeJourAnnulationApresDerniereRelance))
                            {
                                Model.Local.OrderMail OrderMail = OrderMailRepository.ReadType(mailId);
                                if (OrderMail != null && OrderMail.OrdMai_Active)
                                {
                                    PRESTACONNECT.Core.Sync.SynchronisationCommande.SendMail(mailId, order);

                                    Model.Prestashop.PsOrderHistory PsOrderHistory = new Model.Prestashop.PsOrderHistory();

                                    PsOrderHistory.IDOrder = Convert.ToUInt32(order.IDOrder);
                                    PsOrderHistory.IDOrderState = Convert.ToUInt32(6);
                                    PsOrderHistory.DateAdd = DateTime.Now;
                                    PsOrderHistoryRepository.Add(PsOrderHistory);
                                }
                            }
                        }
                    }
                }
        }

        public static void ExecArgsGestionStatutArticle()
        {
            try
            {
                GestionStatutArticle formsynchro = new GestionStatutArticle();
                formsynchro.ShowDialog();
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[GESTION STATUT ARTICLE]" + ex.ToString());
            }
        }

        public static void ExecArgsSynchronisationPaiementDiffere(DateTime? filtre = null)
        {
            try
            {
                // update temporaire du filtre date en fonction de l'argument en paramètre
                if (filtre != null)
                    Core.Global.GetConfig().UpdateConfigCommandeFiltreDate(filtre, true);

                SynchronisationPaiement form = new SynchronisationPaiement();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[SYNCHRONISATION REGLEMENTS]" + ex.ToString());
            }
        }

        public static void ExecArgsSynchroGroupCatTarif()
        {
            try
            {
                SynchroGroupCatTarif formsynchro = new SynchroGroupCatTarif();
                formsynchro.ShowDialog();
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[SynchroGroupCatTarif] " + ex.ToString());
            }
        }

        public static void ExecArgsSynchroGroupCodeRisque()
        {
            try
            {
                SynchroGroupCRisque formsynchro = new SynchroGroupCRisque();
                formsynchro.ShowDialog();
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[SynchroGroupCRisque] " + ex.ToString());
            }
        }

        public static void ExecArgsSynchroPortfolio()
        {
            try
            {
                SynchroPortfolioCustomerEmployee formsynchro = new SynchroPortfolioCustomerEmployee();
                formsynchro.ShowDialog();
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[SynchroPortfolioCustomerEmployee] " + ex.ToString());
            }
        }

        public static void ExecArgsSynchroEncoursClient()
        {
            PRESTACONNECT.AECCustomerOutstanding form = new PRESTACONNECT.AECCustomerOutstanding();
            form.ShowDialog();
        }

        #endregion

        #region Import Sage

        public static void ExecArgsTransfertSageClient()
        {
            List<string> logs = new List<string>();

            if (Core.Global.GetConfig().TransfertPriceCategoryAvailable.Count < 1)
                logs.Add("TC00- Aucune catégorie tarifaire n'est transférable");
            else
            {
                List<int> ListMappage = new Model.Local.CustomerRepository().List().Select(c => c.Sag_Id).ToList();
                List<int> ListIdClient = new List<int>();
                Model.Sage.F_COMPTETRepository F_COMPTETRepository = new Model.Sage.F_COMPTETRepository();
                List<Model.Sage.F_COMPTET> ListF_COMPTET = F_COMPTETRepository.ListTypeSommeil(0, 0);
                ListF_COMPTET = ListF_COMPTET.Where(c => c.CT_EMail != "" || c.F_LIVRAISON.Count(a => a.LI_EMail != "") > 0).ToList();

                List<Model.Sage.F_COMPTET> temp = new List<Model.Sage.F_COMPTET>();

                Model.Local.GroupRepository GroupRepository = new Model.Local.GroupRepository();
                foreach (Int32 CatTarifMarq in Core.Global.GetConfig().TransfertPriceCategoryAvailable)
                    if (GroupRepository.CatTarifSageMonoGroupe(CatTarifMarq))
                        temp.AddRange(ListF_COMPTET.Where(c => c.N_CatTarif == CatTarifMarq));

                ListIdClient = (from Table in temp
                                where !ListMappage.Contains(Table.cbMarq)
                                select Table.cbMarq).ToList();

                // <JG> 15/01/2015 activation utilisation formulaire existant
                TransfertSageClient Form = new TransfertSageClient(ListIdClient);
                Form.ShowDialog();

                //foreach (int IdClient in ListIdClient)
                //{
                //    Core.ImportSage.TransfertSageClient Sync = new Core.ImportSage.TransfertSageClient();
                //    List<string> log;
                //    Sync.Exec(IdClient, out log, false);
                //    if (log.Count > 0)
                //    {
                //        log.Add(Core.Log.LogLineSeparator);
                //        logs.AddRange(log);
                //    }
                //}
            }

            if (logs.Count > 0)
            {
                Core.Log.SendLog(logs, Log.LogIdentifier.TransfertClient);
            }
        }

        public static void ExecArgsImportSageCatalogue()
        {
            try
            {
                List<string> logs = new List<string>();

                Model.Sage.F_CATALOGUERepository F_CATALOGUERepository = new Model.Sage.F_CATALOGUERepository();
                List<Int32> ListF_CATALOGUE = F_CATALOGUERepository.ListIdOrderByNiveauIntitule();

                foreach (Int32 SageCatalogue in ListF_CATALOGUE)
                {
                    Core.ImportSage.ImportCatalogue Sync = new Core.ImportSage.ImportCatalogue();
                    List<string> log;
                    Sync.Exec(SageCatalogue, out log);
                    if (log.Count > 0)
                    {
                        log.Add(Core.Log.LogLineSeparator);
                        logs.AddRange(log);
                    }
                }

                if (logs.Count > 0)
                {
                    Core.Log.SendLog(logs, Log.LogIdentifier.ImportAutoCatalog);
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[IMPORT AUTO CATALOGUE]" + ex.ToString());
            }
        }

        public static void ExecArgsImportSageArticle()
        {
            try
            {
                List<string> logs = new List<string>();

                Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
                List<Model.Sage.F_ARTICLE_Import> ListF_ARTICLE = F_ARTICLERepository.ListInCatalog();
                Core.Temp.ListArticleLocal = new Model.Local.ArticleRepository().ListSageId();
                Core.Temp.ListArticleLocal.AddRange(new Model.Local.CompositionArticleRepository().ListF_ARTICLESageId());

                Boolean Sommeil = Core.Global.GetConfig().ArticleEnSommeil;
                Boolean NonPublie = Core.Global.GetConfig().ArticleNonPublieSurLeWeb;
                Boolean OnlyStandardProducts = Core.Global.GetConfig().ImportArticleOnlyStandardProducts;

                ListF_ARTICLE = ListF_ARTICLE.AsParallel().Where(a => !a.Exist
                            && (Sommeil || a.AR_Sommeil == 0)
                            && (NonPublie || a.AR_Publie == 1)
                            && (!OnlyStandardProducts || ((a.AR_Gamme1 == null || a.AR_Gamme1 == 0) && (a.AR_Gamme2 == null || a.AR_Gamme2 == 0) && (a.AR_Condition == null || a.AR_Condition == 0)))).AsParallel().ToList();

                // <JG> 02/11/2015 ajout filtres d'import par exclusion
                ListF_ARTICLE = Core.Tools.FiltreImportSage.ImportSageFilter(ListF_ARTICLE);

                ImportSageArticle import = new ImportSageArticle(ListF_ARTICLE.AsParallel().Select(a => a.cbMarq).AsParallel().ToList(), null);
                import.ShowDialog();
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[IMPORT AUTO ARTICLE]" + ex.ToString());
            }
        }

        public static void ExecArgsImportCompositionGammes()
        {
            try
            {
                ImportCompositionGammes form = new ImportCompositionGammes();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[IMPORT AUTO COMPOSITION GAMMES]" + ex.ToString());
            }
        }

        public static void ExecArgsImportImage()
        {
            try
            {
                List<string> logs = new List<string>();

                if (!string.IsNullOrWhiteSpace(Core.Global.GetConfig().AutomaticImportFolderPicture))
                {
                    if (System.IO.Directory.Exists(Core.Global.GetConfig().AutomaticImportFolderPicture))
                    {
                        if (Core.Global.GetConfig().ImportImageRemoveDeletedFiles)
                        {
                            SuppressionImage delete = new SuppressionImage(true);
                            delete.ShowDialog();
                        }

                        // parcours des données Sage
                        if (Core.Global.GetConfig().ImportImageUseSageDatas)
                        {
                            Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
                            List<Model.Sage.F_ARTICLE_Photo> ListPhoto = F_ARTICLERepository.ListPhoto();

                            Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                            List<int> ListSageLocal = ArticleRepository.ListSageId();

                            ListPhoto = ListPhoto.Where(ap => ListSageLocal.Contains(ap.cbMarq)).ToList();

                            foreach (Model.Sage.F_ARTICLE_Photo F_ARTICLE_Photo in ListPhoto)
                            {
                                string file = null;
                                if (!string.IsNullOrWhiteSpace(F_ARTICLE_Photo.AC_Photo))
                                {
                                    file = System.IO.Path.Combine(Core.Global.GetConfig().AutomaticImportFolderPicture, F_ARTICLE_Photo.AC_Photo);
                                }
                                else if (!string.IsNullOrWhiteSpace(F_ARTICLE_Photo.AR_Photo))
                                {
                                    file = System.IO.Path.Combine(Core.Global.GetConfig().AutomaticImportFolderPicture, F_ARTICLE_Photo.AR_Photo);
                                }
                                if (file != null && System.IO.File.Exists(file))
                                {
                                    Model.Local.Article Article = ArticleRepository.ReadSag_Id(F_ARTICLE_Photo.cbMarq);
                                    String extension = Path.GetExtension(file).ToLower();
                                    if (Core.Img.imageExtensions.Contains(extension))
                                    {
                                        Core.ImportSage.ImportArticleImage import = new Core.ImportSage.ImportArticleImage();
                                        if (import.Exec(file, Article.Art_Id))
                                        {
                                            if (import.logs != null && import.logs.Count > 0)
                                                logs.AddRange(import.logs);
                                            else
                                                logs.Add("II10- Import de l'image " + file + " pour l'article [ " + F_ARTICLE_Photo.AR_Ref + " ]");
                                        }
                                    }
                                }
                            }
                        }
                        // fonction par défaut = parcours dossier local ou réseau
                        else
                        {
                            String[] Files = System.IO.Directory.GetFiles(Core.Global.GetConfig().AutomaticImportFolderPicture);
                            foreach (String File in Files)
                            {
                                try
                                {
                                    String extension = Path.GetExtension(File).ToLower();
                                    if (Core.Img.imageExtensions.Contains(extension))
                                    {
                                        String ValuesImg = Path.GetFileNameWithoutExtension(File);
                                        String AR_Ref = ValuesImg;

                                        int position, Declination;
                                        int Article = Core.Global.SearchReference(ValuesImg, out AR_Ref, out position, out Declination);
                                        if (Article != 0)
                                        {
                                            Core.ImportSage.ImportArticleImage import = new Core.ImportSage.ImportArticleImage();
                                            if (import.Exec(File, Article, position, Declination))
                                            {
                                                if (import.logs != null && import.logs.Count > 0)
                                                    logs.AddRange(import.logs);
                                                else
                                                    logs.Add("II10- Import de l'image " + File + " pour l'article [ " + AR_Ref + " ]");
                                            }
                                        }

                                        if (Core.Global.GetConfig().ImportImageSearchReferenceClient)
                                        {
                                            List<int> listID = Core.Global.SearchListReference(ValuesImg);
                                            if (listID != null && listID.Count > 0)
                                            {
                                                int cpt = 0;
                                                foreach (int id_article_local in listID)
                                                {
                                                    Core.ImportSage.ImportArticleImage import = new Core.ImportSage.ImportArticleImage();
                                                    if (import.Exec(File, id_article_local))
                                                    {
                                                        cpt++;
                                                        if (import.logs != null && import.logs.Count > 0)
                                                            logs.AddRange(import.logs);
                                                    }
                                                }
                                                logs.Add("II10- Affectation image " + File + " à " + cpt + " article(s) ");
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logs.Add("II20- Erreur import image : " + ex.ToString());
                                    Core.Error.SendMailError("[II20] " + ex.ToString());
                                }
                            }
                        }
                    }
                    else
                        logs.Add("II02- Répertoire d'image inexistant ou inaccessible");
                }
                else
                    logs.Add("II01- Répertoire d'image non renseigné");

                if (logs.Count > 0)
                {
                    Core.Log.SendLog(logs, Log.LogIdentifier.ImportAutoImage);
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[IMPORT AUTO IMAGE]" + ex.ToString());
            }
        }

        public static void ExecArgsImportDocument()
        {
            try
            {
                List<string> logs = new List<string>();

                if (!string.IsNullOrWhiteSpace(Core.Global.GetConfig().AutomaticImportFolderDocument))
                {
                    if (System.IO.Directory.Exists(Core.Global.GetConfig().AutomaticImportFolderDocument))
                    {
                        String[] Files = System.IO.Directory.GetFiles(Core.Global.GetConfig().AutomaticImportFolderDocument);
                        foreach (String File in Files)
                        {
                            try
                            {
                                int Article = 0;
                                string extension = Path.GetExtension(File).ToLower();
                                string[] ValuesParamFile = Path.GetFileNameWithoutExtension(File).Split('\\');
                                string filename = ValuesParamFile[ValuesParamFile.Length - 1];

                                Model.Local.MediaAssignmentRuleRepository MediaAssignmentRuleRepository = new Model.Local.MediaAssignmentRuleRepository();
                                List<Model.Local.MediaAssignmentRule> list = MediaAssignmentRuleRepository.List();
                                if (list.Count(r => filename.EndsWith(r.SuffixText)) > 0)
                                {
                                    foreach (Model.Local.MediaAssignmentRule mediarule in list.Where(r => filename.EndsWith(r.SuffixText)))
                                    {
                                        if (filename.EndsWith(mediarule.SuffixText))
                                        {
                                            string ref_art;
                                            int position, AttributeArticle;
                                            Article = Core.Global.SearchReference(filename.Substring(0, filename.Length - mediarule.SuffixText.Length), out ref_art, out position, out AttributeArticle);
                                            if (Article != 0)
                                            {
                                                switch (mediarule.Rule)
                                                {
                                                    case (short)Core.Parametres.MediaRule.AsAttachment:

                                                        Core.ImportSage.ImportArticleDocument import = new Core.ImportSage.ImportArticleDocument();
                                                        if (import.Exec(File, Article, (!string.IsNullOrEmpty(mediarule.AssignName) ? mediarule.AssignName : filename), null, null))
                                                            logs.Add("ID11- Import du document " + File + " pour l'article [ " + ref_art + " ]");
                                                        break;
                                                    case (short)Core.Parametres.MediaRule.AsPicture:
                                                        if (Core.Img.imageExtensions.Contains(extension))
                                                        {
                                                            Core.ImportSage.ImportArticleImage ImportImage = new Core.ImportSage.ImportArticleImage();
                                                            if (ImportImage.Exec(File, Article, position, AttributeArticle))
                                                                logs.Add("ID12- Import du document " + File + " pour l'article [ " + ref_art + " ]");
                                                        }
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    int position, AttributeArticle;
                                    Article = Core.Global.SearchReference(filename, out position, out AttributeArticle);
                                    if (Article != 0)
                                    {
                                        if (Core.Img.imageExtensions.Contains(extension))
                                        {
                                            // Unactive import
                                            //Core.ImportSage.ImportArticleImage ImportImage = new Core.ImportSage.ImportArticleImage();
                                            //if (ImportImage.Exec(File, Article, position))
                                            //    logs.Add("ID13- Import de document pour l'article [ " + filename + " ]");
                                        }
                                        else
                                        {
                                            Core.ImportSage.ImportArticleDocument import = new Core.ImportSage.ImportArticleDocument();
                                            if (import.Exec(File, Article))
                                                logs.Add("ID10- Import du document " + File + " pour l'article [ " + filename + " ]");
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                logs.Add("ID20- Erreur import document : " + ex.ToString());
                                Core.Error.SendMailError("[ID20] " + ex.ToString());
                            }
                        }
                    }
                    else
                        logs.Add("ID02- Répertoire de document inexistant ou inaccessible");
                }
                else
                    logs.Add("ID01- Répertoire de document non renseigné");

                if (logs.Count > 0)
                {
                    Core.Log.SendLog(logs, Log.LogIdentifier.ImportAutoDocument);
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[IMPORT AUTO DOCUMENT] " + ex.ToString());
            }
        }

        public static void ExecArgsImportSageMedia()
        {
            try
            {
                List<string> logs = new List<string>();
                if (!string.IsNullOrWhiteSpace(Core.Global.GetConfig().AutomaticImportFolderMedia)
                    && System.IO.Directory.Exists(Core.Global.GetConfig().AutomaticImportFolderMedia))
                {
                    string DirDoc = Core.Global.GetConfig().AutomaticImportFolderMedia;
                    List<int> ListArticles = new Model.Local.ArticleRepository().ListId();
                    foreach (int IdArticle in ListArticles)
                    {
                        try
                        {
                            Model.Sage.F_ARTICLEMEDIARepository F_ARTICLEMEDIARepository = new Model.Sage.F_ARTICLEMEDIARepository();
                            Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                            Model.Local.Article Article = new Model.Local.Article();
                            if (ArticleRepository.ExistArticle(IdArticle))
                            {
                                Article = ArticleRepository.ReadArticle(IdArticle);

                                // <JG> 24/03/2015 ajout option suppression auto 
                                if (Core.Global.GetConfig().ImportMediaAutoDeleteAttachment)
                                {
                                    Model.Local.AttachmentRepository AttachmentRepository = new Model.Local.AttachmentRepository();
                                    if (AttachmentRepository.ExistArticle(IdArticle))
                                    {
                                        List<Model.Local.Attachment> ListArticle = AttachmentRepository.ListArticle(IdArticle);
                                        ListArticle = ListArticle.Where(at => at.Sag_Id != null).ToList();

                                        foreach (Model.Local.Attachment Attachment in ListArticle)
                                        {
                                            if (!F_ARTICLEMEDIARepository.Exist(Attachment.Sag_Id.Value))
                                            {
                                                if (System.IO.File.Exists(System.IO.Path.Combine(Core.Global.GetConfig().Folders.RootAttachment, Attachment.Att_File)))
                                                    File.Delete(System.IO.Path.Combine(Core.Global.GetConfig().Folders.RootAttachment, Attachment.Att_File));

                                                if (Attachment.Pre_Id != null && Attachment.Pre_Id > 0)
                                                {
                                                    // Suppression de l'occurence du document sur prestashop 
                                                    Model.Prestashop.PsAttachmentRepository psAttachmentRepository = new Model.Prestashop.PsAttachmentRepository();
                                                    Model.Prestashop.PsAttachmentLangRepository psAttachmentLangRepository = new Model.Prestashop.PsAttachmentLangRepository();
                                                    Model.Prestashop.PsProductAttachmentRepository psProductAttachmentRepository = new Model.Prestashop.PsProductAttachmentRepository();

                                                    Model.Prestashop.PsAttachment psAttachment = psAttachmentRepository.ReadAttachment(Convert.ToUInt32(Attachment.Pre_Id.Value));

                                                    string distant_file = string.Empty;
                                                    if (psAttachment != null)
                                                    {
                                                        distant_file = psAttachment.File;
                                                        psProductAttachmentRepository.Delete(psProductAttachmentRepository.ListAttachment(psAttachment.IDAttachment));
                                                        psAttachmentLangRepository.Delete(psAttachmentLangRepository.ListAttachment(psAttachment.IDAttachment));
                                                        psAttachmentRepository.Delete(psAttachment);
                                                    }

                                                    if (Core.Global.GetConfig().ConfigFTPActive)
                                                    {
                                                        String FTP = Core.Global.GetConfig().ConfigFTPIP;
                                                        String User = Core.Global.GetConfig().ConfigFTPUser;
                                                        String Password = Core.Global.GetConfig().ConfigFTPPassword;

                                                        string ftpfullpath = FTP + "/download/" + distant_file;

                                                        if (Core.Ftp.ExistFile(ftpfullpath, User, Password))
                                                        {
                                                            try
                                                            {
                                                                System.Net.FtpWebRequest request = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(ftpfullpath);
                                                                request.Credentials = new System.Net.NetworkCredential(User, Password);
                                                                request.Method = System.Net.WebRequestMethods.Ftp.DeleteFile;
                                                                request.UseBinary = true;
                                                                request.UsePassive = true;
                                                                request.KeepAlive = false;
                                                                request.EnableSsl = Core.Global.GetConfig().ConfigFTPSSL;

                                                                System.Net.FtpWebResponse response = (System.Net.FtpWebResponse)request.GetResponse();
                                                                response.Close();
                                                            }
                                                            catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
                                                        }
                                                    }
                                                }

                                                AttachmentRepository.Delete(Attachment);
                                                logs.Add("IM90- Suppression du média " + Attachment.Att_FileName + " pour l'article [ " + Article.Art_Ref + " ]");
                                            }
                                        }
                                    }
                                }

                                if (F_ARTICLEMEDIARepository.ExistReference(Article.Art_Ref))
                                {
                                    foreach (Model.Sage.F_ARTICLEMEDIA F_ARTICLEMEDIA in F_ARTICLEMEDIARepository.ListReference(Article.Art_Ref))
                                    {
                                        try
                                        {
                                            String File = (System.IO.File.Exists(F_ARTICLEMEDIA.ME_Fichier))
                                                        ? F_ARTICLEMEDIA.ME_Fichier
                                                        : Path.Combine(DirDoc, F_ARTICLEMEDIA.ME_Fichier.Substring(2));
                                            if (System.IO.File.Exists(File))
                                            {
                                                string extension = Path.GetExtension(File).ToLower();
                                                string filename = Path.GetFileNameWithoutExtension(File);
                                                Model.Local.MediaAssignmentRuleRepository MediaAssignmentRuleRepository = new Model.Local.MediaAssignmentRuleRepository();
                                                List<Model.Local.MediaAssignmentRule> list = MediaAssignmentRuleRepository.List();
                                                if (list.Count(r => filename.EndsWith(r.SuffixText)) > 0)
                                                {
                                                    foreach (Model.Local.MediaAssignmentRule mediarule in list.Where(r => filename.EndsWith(r.SuffixText)))
                                                    {
                                                        if (filename.EndsWith(mediarule.SuffixText))
                                                        {
                                                            switch (mediarule.Rule)
                                                            {
                                                                case (short)Core.Parametres.MediaRule.AsAttachment:
                                                                    Core.ImportSage.ImportArticleDocument import = new Core.ImportSage.ImportArticleDocument();
                                                                    if (import.Exec(File, Article.Art_Id, (!string.IsNullOrEmpty(mediarule.AssignName) ? mediarule.AssignName : F_ARTICLEMEDIA.ME_Commentaire), null, F_ARTICLEMEDIA.cbMarq))
                                                                        logs.Add("IM11- Import du média " + File + " pour l'article [ " + Article.Art_Ref + " ]");
                                                                    break;
                                                                case (short)Core.Parametres.MediaRule.AsPicture:
                                                                    if (Core.Img.imageExtensions.Contains(extension))
                                                                    {
                                                                        int position, AttributeArticle;
                                                                        Core.Global.SearchReference(filename, out position, out AttributeArticle);
                                                                        Core.ImportSage.ImportArticleImage ImportImage = new Core.ImportSage.ImportArticleImage();
                                                                        if (ImportImage.Exec(File, Article.Art_Id, position, AttributeArticle))
                                                                            logs.Add("IM12- Import du média " + File + " pour l'article [ " + Article.Art_Ref + " ]");
                                                                    }
                                                                    break;
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (Core.Img.imageExtensions.Contains(extension))
                                                    {
                                                        if (Core.Global.GetConfig().ImportMediaIncludePictures)
                                                        {
                                                            int position, AttributeArticle;
                                                            Core.Global.SearchReference(filename, out position, out AttributeArticle);
                                                            Core.ImportSage.ImportArticleImage ImportImage = new Core.ImportSage.ImportArticleImage();
                                                            if (ImportImage.Exec(File, Article.Art_Id, position, AttributeArticle))
                                                                logs.Add("IM13- Import du média " + File + " en tant qu'image pour l'article [ " + filename + " ]");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Core.ImportSage.ImportArticleDocument Sync = new Core.ImportSage.ImportArticleDocument();
                                                        if (Sync.Exec(File, Article.Art_Id, F_ARTICLEMEDIA.ME_Commentaire, null, F_ARTICLEMEDIA.cbMarq))
                                                            logs.Add("IM10- Import du média " + File + " pour l'article [ " + Article.Art_Ref + " ]");
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            logs.Add("IM20- Erreur import média : " + ex.ToString());
                                            Core.Error.SendMailError("[IM20] " + ex.ToString());
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logs.Add("IM21- Erreur import média : " + ex.ToString());
                        }
                    }
                }
                else
                    logs.Add("IM01- Répertoire multimédia de Sage introuvable ou inaccessible");

                if (logs.Count > 0)
                {
                    Core.Log.SendLog(logs, Log.LogIdentifier.ImportAutoSageMedia);
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[IMPORT AUTO SAGE MEDIA] " + ex.ToString());
            }
        }

        public static void ExecArgsImportSageComplet()
        {
            ExecArgsImportSageCatalogue();
            ExecArgsImportSageArticle();
            ExecArgsImportImage();
            ExecArgsImportDocument();
            ExecArgsImportSageMedia();
        }

        public static void ExecArgsImportSageStatInfolibreArticle(bool updatedateactive = true)
        {
            try
            {
                ImportSageStatInfoLibre formimport = new ImportSageStatInfoLibre(new Model.Local.ArticleRepository().ListId(), updatedateactive);
                formimport.ShowDialog();
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[IMPORT STATS INFOS-LIBRES ARTICLE]" + ex.ToString());
            }
        }
        public static void ExecArgsImportSageStatInfolibreArticleFiltres(String Arg)
        {
            try
            {
                String[] values = Arg.Split(':');
                if (values.Count() > 1)
                {
                    Core.Temp.TaskImportStatInfoLibreFilter = values[1].Split('#').ToList();

                    if (Core.Temp.TaskImportStatInfoLibreFilter.Count > 0)
                    {
                        bool args_valid = true;
                        List<string> Logs = new List<string>();
                        Model.Local.InformationLibreRepository InformationLibreRepository = new Model.Local.InformationLibreRepository();
                        foreach (string name in Core.Temp.TaskImportStatInfoLibreFilter)
                        {
                            if (!InformationLibreRepository.ExistInfoLibre(name)
                                && !InformationLibreRepository.ExistInfoLibreArgSyntax(name))
                            {
                                args_valid = false;
                                if (Logs.Count == 0)
                                    Logs.Add("Les paramètres de filtre des informations libres sont incorrects, les valeurs suivantes sont erronées :");

                                Logs.Add("- " + name);
                            }
                        }
                        if (args_valid)
                            Core.Task.ExecArgsImportSageStatInfolibreArticle(false);
                        else
                            Core.Log.SendLog(Logs, Core.Log.LogIdentifier.ImportAutoStatInfoLibreArticle);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[IMPORT INFOS-LIBRES ARTICLE AVEC FILTRE]" + ex.ToString());
            }
        }

        public static void ExecArgsTransfertSageStatInfolibreClient()
        {
            try
            {
                List<string> logs = new List<string>();
                List<Model.Local.Customer> ListCustomer = new Model.Local.CustomerRepository().List();
                foreach (Model.Local.Customer Customer in ListCustomer)
                {
                    try
                    {
                        if (new Model.Sage.F_COMPTETRepository().ExistId(Customer.Sag_Id)
                            && new Model.Prestashop.PsCustomerRepository().ExistCustomer((uint)Customer.Pre_Id))
                        {
                            try
                            {
                                Core.Transfert.TransfertStatInfoLibreClient transfert = new Core.Transfert.TransfertStatInfoLibreClient();
                                transfert.Exec(Customer.Sag_Id);
                            }
                            catch (Exception ex)
                            {
                                logs.Add("ISCC20- Erreur transfert caractéristique client : " + ex.ToString());
                                Core.Error.SendMailError("[ISCC20] " + ex.ToString());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logs.Add("ISCC21- Erreur transfert caractéristique client : " + ex.ToString());
                    }
                }

                if (logs.Count > 0)
                {
                    Core.Log.SendLog(logs, Log.LogIdentifier.ImportAutoSageMedia);
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[TRANSFERT STATS INFOS-LIBRES CLIENT]" + ex.ToString());
            }
        }

        public static void ExecArgsImportSageCatalogueInfolibre()
        {
            try
            {
                List<string> logs = new List<string>();
                List<Model.Local.Article> ListArticle = new Model.Local.ArticleRepository().List();

                Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
                Model.Sage.F_ARTICLE F_ARTICLE = new Model.Sage.F_ARTICLE();

                foreach (Model.Local.Article Article in ListArticle)
                {
                    try
                    {
                        F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
                        F_ARTICLE = F_ARTICLERepository.ReadArticle(Article.Sag_Id);

                        try
                        {
                            Core.ImportSage.ImportArticle Sync = new Core.ImportSage.ImportArticle();
                            Sync.ImportCatalogueInfoLibre(F_ARTICLE, Article);
                        }
                        catch (Exception ex)
                        {
                            logs.Add("ICIL20- Erreur import catalogue info libre : " + ex.ToString());
                            Core.Error.SendMailError("[ICIL20] " + ex.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        logs.Add("ICIL21- Erreur import catalogue info libre : " + ex.ToString());
                    }
                }

                if (logs.Count > 0)
                {
                    Core.Log.SendLog(logs, Log.LogIdentifier.ImportAutoSageMedia);
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[IMPORT CATALOGUE INFO LIBRE]" + ex.ToString());
            }
        }

        public static void ExecArgsTransfertAttribute()
        {
            try
            {
                TransfertAttribute form = new TransfertAttribute();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[TRANSFERT DECLINAISONS ARTICLE]" + ex.ToString());
            }
        }

        public static void ExecArgsTransfertFeature()
        {
            try
            {
                TransfertFeature form = new TransfertFeature();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[TRANSFERT CARACTERISTIQUES ARTICLE]" + ex.ToString());
            }
        }

        public static void ExecArgsTransfertFeatureFilters(String Arg)
        {
            try
            {
                String[] values = Arg.Split(':');
                if (values.Count() > 1)
                {
                    Core.Temp.TaskTransfertFeatureFilter = new List<uint>();
                    foreach (string value in values[1].Split('#'))
                    {
                        uint id_feature;
                        if (uint.TryParse(value, out id_feature))
                        {
                            Core.Temp.TaskTransfertFeatureFilter.Add(id_feature);
                        }
                    }

                    if (Core.Temp.TaskTransfertFeatureFilter.Count > 0)
                    {
                        bool args_valid = true;
                        List<string> Logs = new List<string>();
                        Model.Prestashop.PsFeatureRepository PsFeatureRepository = new Model.Prestashop.PsFeatureRepository();
                        foreach (uint id_feature in Core.Temp.TaskTransfertFeatureFilter)
                        {
                            if (!PsFeatureRepository.Exist(id_feature))
                            {
                                args_valid = false;
                                if (Logs.Count == 0)
                                    Logs.Add("Les paramètres de filtre des caractéristiques sont incorrects, les valeurs suivantes sont erronées :");

                                Logs.Add("- " + id_feature);
                            }
                        }
                        if (args_valid)
                            Core.Task.ExecArgsTransfertFeature();
                        else
                            Core.Log.SendLog(Logs, Core.Log.LogIdentifier.TransfertPrestashopCaracteristique);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[TRANSFERT CARACTERISTIQUES ARTICLE AVEC FILTRES]" + ex.ToString());
            }
        }

        #endregion

        #region Import PrestaShop

        public static void ExecArgsImportPrestashopCaracteristiqueArticle()
        {
            try
            {
                List<string> logs = new List<string>();
                Model.Prestashop.PsProductRepository PsProductRepository = new Model.Prestashop.PsProductRepository();
                Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();

                List<Model.Prestashop.ProductLight> ListProduct = PsProductRepository.ListLight();
                foreach (Model.Prestashop.ProductLight p in ListProduct)
                {
                    try
                    {
                        if (ArticleRepository.ExistPre_Id((int)p.id_product))
                        {
                            try
                            {
                                Core.ImportPrestashop.ImportArticle Sync = new Core.ImportPrestashop.ImportArticle();
                                Sync.ImportCharacteristic(PsProductRepository.ReadId(p.id_product), ArticleRepository.ReadPre_Id((int)p.id_product), true);
                            }
                            catch (Exception ex)
                            {
                                logs.Add("IPCA20- Erreur import caractéristiques article : " + ex.ToString());
                                Core.Error.SendMailError("[IPCA20] " + ex.ToString());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logs.Add("IPCA21- Erreur import caractéristiques article : " + ex.ToString());
                    }
                }

                if (logs.Count > 0)
                {
                    Core.Log.SendLog(logs, Log.LogIdentifier.ImportPrestashopCaracteristiqueArticle);
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[IMPORT PRESTASHOP CARACTERISTIQUES ARTICLES]" + ex.ToString());
            }
        }

        #endregion

        #region Export Facture

        public static void ExecArgsExportFacture(DateTime? start = null, DateTime? end = null)
        {
            try
            {
                if (!Core.Global.ExistAECInvoiceHistoryModule())
                    Core.Log.WriteLog("IH01- Envoi de l'historique de documents impossible : Module Prestashop AEC_InvoiceHistory introuvable.", true);
                else if (!Core.Global.GetConfig().ModuleAECInvoiceHistoryActif)
                    Core.Log.WriteLog("IH02- Envoi de l'historique de documents impossible : Module PrestaConnect non activé.", true);
                else if (!System.IO.File.Exists(System.IO.Path.Combine(Core.Global.GetConfig().Folders.RootReport, "AEC_Invoice.rpt")))
                    Core.Log.WriteLog("IH03- Envoi de l'historique de documents impossible : Modèle d'impression Crystal Report introuvable.", true);
                else
                {
                    AECInvoiceHistory AECInvoiceHistory = new PRESTACONNECT.AECInvoiceHistory(start, end);
                    AECInvoiceHistory.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[EXPORT FACTURE PDF]" + ex.ToString());
            }
        }

        #endregion

        #region Export Collaborateur

        public static void ExecArgsExportCollaborateur()
        {
            try
            {
                if (Core.Global.GetConfig().ModuleAECCollaborateurActif && Core.Global.ExistAECCollaborateurModule())
                {
                    AECCustomerCollaborateur formexport = new AECCustomerCollaborateur();
                    formexport.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[EXPORT COLLABORATEUR]" + ex.ToString());
            }
        }

        #endregion

        #region Export Payement

        public static void ExecArgsExportPayement()
        {
            try
            {
                if (Core.Global.GetConfig().ModuleAECPaiementActif && Core.Global.ExistAECPaiementModule())
                {
                    AECCustomerPayement formexport = new AECCustomerPayement();
                    formexport.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[EXPORT PAIEMENT]" + ex.ToString());
            }
        }
        #endregion

        #region Export Infos Client
        public static void ExecArgsExportInfosClient()
        {
            try
            {
                if (Core.Global.GetConfig().ModuleAECCustomerInfoActif)
                {
                    CustomerInfo formexport = new CustomerInfo();
                    formexport.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[EXPORT INFOS CLIENT]" + ex.ToString());
            }
        }
        #endregion

        #region Modules AEC

        public static void ExecArgsExportAECRepresentative()
        {
            if (Core.Global.ExistAECRepresentativeModule())
            {
                AECRepresentative form = new AECRepresentative();
                form.ShowDialog();
                AECRepresentativeCustomer form2 = new AECRepresentativeCustomer();
                form2.ShowDialog();
            }
            else
            {
                Core.Log.WriteLog("Module AECRepresentative non trouvé sur votre PrestaShop !", true);
            }
        }

        public static void ExecArgsExportAECBalance()
        {
            if (Core.Global.ExistAECBalanceModule())
            {
                AECBalanceOutstanding form = new AECBalanceOutstanding();
                form.ShowDialog();
                AECBalanceAccounting form2 = new AECBalanceAccounting();
                form2.ShowDialog();
            }
            else
            {
                Core.Log.WriteLog("Module AECBalance non trouvé sur votre PrestaShop !", true);
            }
        }

        #endregion
    }
}
