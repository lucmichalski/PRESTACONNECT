using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Core
{
    public static class Temp
    {
        public enum _action_information_synchro { debut, fin, refresh }

        #region Temp Local PrestaConnect

        private static int article = 0;
        public static int Article
        {
            get { return article; }
            set { article = value; }
        }

        public static Boolean insert_productattribute = false;

        private static List<int> listFeatureLocked = new List<int>();
        public static List<int> ListFeatureLocked
        {
            get { return listFeatureLocked; }
            set { listFeatureLocked = value; }
        }
        private static List<string> listManufacturerLocked = new List<string>();
        public static List<string> ListManufacturerLocked
        {
            get { return listManufacturerLocked; }
            set { listManufacturerLocked = value; }
        }
        private static List<string> listSupplierLocked = new List<string>();
        public static List<string> ListSupplierLocked
        {
            get { return listSupplierLocked; }
            set { listSupplierLocked = value; }
        }
        private static List<uint> listAttributeGroupLocked = new List<uint>();
        public static List<uint> ListAttributeGroupLocked
        {
            get { return listAttributeGroupLocked; }
            set { listAttributeGroupLocked = value; }
        }

        private static List<int> listArticlesLocal = new List<int>();
        public static List<int> ListArticleLocal
        {
            get { return listArticlesLocal; }
            set { listArticlesLocal = value; }
        }

        private static List<int> listCatalogLocal = new List<int>();
        public static List<int> ListCatalogLocal
        {
            get { return listCatalogLocal; }
            set { listCatalogLocal = value; }
        }

        private static List<Model.Local.Customer> listLocalCustomer = new List<Model.Local.Customer>();
        public static List<Model.Local.Customer> ListLocalCustomer
        {
            get { return listLocalCustomer; }
            set { listLocalCustomer = value; }
        }

        private static List<Model.Local.Customer> listLocalCustomer_Remise = new List<Model.Local.Customer>();
        public static List<Model.Local.Customer> ListLocalCustomer_Remise
        {
            get { return listLocalCustomer_Remise; }
            set { listLocalCustomer_Remise = value; }
        }

        private static List<Model.Local.Address> listLocalAddress = new List<Model.Local.Address>();
        public static List<Model.Local.Address> ListLocalAddress
        {
            get { return listLocalAddress; }
            set { listLocalAddress = value; }
        }

        private static List<Model.Internal.CategorieComptable> listCategorieComptable = new List<Model.Internal.CategorieComptable>();
        public static List<Model.Internal.CategorieComptable> ListCategorieComptable
        {
            get { return listCategorieComptable; }
            set { listCategorieComptable = value; }
        }

        private static List<Model.Local.Supplier> listSupplier = new List<Model.Local.Supplier>();
        public static List<Model.Local.Supplier> ListSupplier
        {
            get { return listSupplier; }
            set { listSupplier = value; }
        }

        #endregion

        #region Temp composition

        public static Model.Sage.F_TAXE selected_taxe_composition = null;
        public static string reference_sage_composition = string.Empty;
        public static string designation_composition = string.Empty;
        public static int selectedcatalog_composition = 0;

        #endregion

        #region Temp PrestaShop

        private static List<Model.Prestashop.idcustomer> listPrestashopCustomer = new List<Model.Prestashop.idcustomer>();
        public static List<Model.Prestashop.idcustomer> ListPrestashopCustomer
        {
            get { return listPrestashopCustomer; }
            set { listPrestashopCustomer = value; }
        }
        private static List<Model.Prestashop.btoc_customer> listPrestashopCustomerBtoC = new List<Model.Prestashop.btoc_customer>();
        public static List<Model.Prestashop.btoc_customer> ListPrestashopCustomerBtoC
        {
            get { return listPrestashopCustomerBtoC; }
            set { listPrestashopCustomerBtoC = value; }
        }
        private static List<Model.Prestashop.btob_customer> listPrestashopCustomerBtoB = new List<Model.Prestashop.btob_customer>();
        public static List<Model.Prestashop.btob_customer> ListPrestashopCustomerBtoB
        {
            get { return listPrestashopCustomerBtoB; }
            set { listPrestashopCustomerBtoB = value; }
        }

        private static List<Model.Prestashop.ProductUpdate> listPrestashopProducts = new List<Model.Prestashop.ProductUpdate>();
        public static List<Model.Prestashop.ProductUpdate> ListPrestashopProducts
        {
            get { return listPrestashopProducts; }
            set { listPrestashopProducts = value; }
        }

        private static List<Model.Prestashop.PsCountryLang> listPsCountryLang = new List<Model.Prestashop.PsCountryLang>();
        public static List<Model.Prestashop.PsCountryLang> ListPsCountryLang
        {
            get { return listPsCountryLang; }
            set { listPsCountryLang = value; }
        }

        public static Model.Prestashop.PsFeatureValueLangRepository PsFeatureValueLangRepository;
        public static Model.Prestashop.PsAttributeLangRepository PsAttributeLangRepository;

        //private static IEnumerable<Model.Prestashop.PsFeatureValueLang> listPsFeatureValueLang = null;
        //public static IEnumerable<Model.Prestashop.PsFeatureValueLang> ListPsFeatureValueLang
        //{
        //    get { return listPsFeatureValueLang; }
        //    set { listPsFeatureValueLang = value; }
        //}

        private static List<Model.Prestashop.PsAttributeLang> listPsAttributeLang = null;
        public static List<Model.Prestashop.PsAttributeLang> ListPsAttributeLang
        {
            get { return listPsAttributeLang; }
            set { listPsAttributeLang = value; }
        }

        private static List<Model.Prestashop.PsOrderStateLang> listPsOrderStateLang = new List<Model.Prestashop.PsOrderStateLang>();
        public static List<Model.Prestashop.PsOrderStateLang> ListPsOrderStateLang
        {
            get { return listPsOrderStateLang; }
            set { listPsOrderStateLang = value; }
        }

        private static List<Model.Prestashop.ProductUpdate> listProductUpdate = new List<Model.Prestashop.ProductUpdate>();
        public static List<Model.Prestashop.ProductUpdate> ListProductUpdate
        {
            get { return listProductUpdate; }
            set { listProductUpdate = value; }
        }

        #endregion

        #region Temp Sage

        private static List<Model.Sage.F_CATALOGUE> listF_CATALOGUE = new List<Model.Sage.F_CATALOGUE>();
        public static List<Model.Sage.F_CATALOGUE> ListF_CATALOGUE
        {
            get { return listF_CATALOGUE; }
            set { listF_CATALOGUE = value; }
        }

        private static List<Model.Sage.F_COMPTET_Light> listF_COMPTET_Light = new List<Model.Sage.F_COMPTET_Light>();
        public static List<Model.Sage.F_COMPTET_Light> ListF_COMPTET_Light
        {
            get { return listF_COMPTET_Light; }
            set { listF_COMPTET_Light = value; }
        }

        private static List<Model.Sage.F_COMPTET_BtoB> listF_COMPTET_BtoB = new List<Model.Sage.F_COMPTET_BtoB>();
        public static List<Model.Sage.F_COMPTET_BtoB> ListF_COMPTET_BtoB
        {
            get { return listF_COMPTET_BtoB; }
            set { listF_COMPTET_BtoB = value; }
        }

        private static List<Model.Sage.F_COMPTET_Light> listF_COMPTET_Centrales = new List<Model.Sage.F_COMPTET_Light>();
        public static List<Model.Sage.F_COMPTET_Light> ListF_COMPTET_Centrales
        {
            get { return listF_COMPTET_Centrales; }
            set { listF_COMPTET_Centrales = value; }
        }

        private static List<Model.Sage.F_CONDITION> listF_CONDITION = new List<Model.Sage.F_CONDITION>();
        public static List<Model.Sage.F_CONDITION> ListF_CONDITION
        {
            get { return listF_CONDITION; }
            set { listF_CONDITION = value; }
        }

        private static List<Model.Sage.P_CONDITIONNEMENT> listP_CONDITIONNEMENT = new List<Model.Sage.P_CONDITIONNEMENT>();
        public static List<Model.Sage.P_CONDITIONNEMENT> ListP_CONDITIONNEMENT
        {
            get { return listP_CONDITIONNEMENT; }
            set { listP_CONDITIONNEMENT = value; }
        }

        #endregion

        #region TempSynchro

        private static List<uint> listAddressOnCurrentSync = new List<uint>();
        public static List<uint> ListAddressOnCurrentSync
        {
            get { return listAddressOnCurrentSync; }
            set { listAddressOnCurrentSync = value; }
        }

        #endregion

        #region TempFilters

        public static String ModuleAECInvoiceHistory_Numero = null;
        public static String ModuleAECInvoiceHistory_Intitule = null;

        public static List<string> TaskImportStatInfoLibreFilter = null;
        public static List<uint> TaskTransfertFeatureFilter = null;

        #endregion

        #region TempFunction

        public static Boolean UnlockProcessorCore = false;

        public static Boolean UpdateDateActive = false;

        #endregion

        /// <summary>
        /// Permet d'indiquer si Prestaconnect doit lancer le nettoyage du cache Smarty à la fin de la synchro catalogue
        /// </summary>
        public static Boolean SyncCatalogue_ClearSmartyCache = false;
        /// <summary>
        /// Permet d'indiquer si Prestaconnect doit lancer la régénération de l'arborescence des catégories à la fin de la synchro catalogue
        /// </summary>
        public static Boolean SyncCatalogue_RegenerateTree = false;
        /// <summary>
        /// Permet d'indique si Prestaconnect doit lancer la réindexation des produits à la fin de la synchro article
        /// </summary>
        public static Boolean SyncArticle_LaunchIndex = false;

        public static System.Windows.WindowState Current;

        #region Method

        public static void LoadListesClients()
        {
            // chargement listes clients si non chargé
            // attention ces listes doivent être vidées en fin de synchro et non à la fin du traitement article
            bool filtrage = false;
            if (Core.Temp.ListLocalCustomer.Count == 0)
            {
                Core.Temp.ListLocalCustomer = new Model.Local.CustomerRepository().List();
                filtrage = true;
            }

            if (Core.Temp.ListF_COMPTET_Light.Count == 0)
            {
                Core.Temp.ListF_COMPTET_Light = new Model.Sage.F_COMPTETRepository().ListLight((short)ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_CT_Type.Client);
                List<string> ctnum_centrales = Core.Temp.ListF_COMPTET_Light.Select(c => c.CT_NumCentrale).Distinct().Where(ctnum => !string.IsNullOrWhiteSpace(ctnum)).ToList();
                Core.Temp.ListF_COMPTET_Centrales = Core.Temp.ListF_COMPTET_Light.Where(s => ctnum_centrales.Contains(s.CT_Num)).ToList();
                filtrage = true;
            }

            if (filtrage)
            {
                List<int> sag_id = Core.Temp.ListLocalCustomer.AsParallel().Select(l => l.Sag_Id).ToList();
                Core.Temp.ListF_COMPTET_Light = Core.Temp.ListF_COMPTET_Light.AsParallel().Where(c => sag_id.Contains(c.cbMarq)).ToList();
            }

            if (Core.Temp.ListPrestashopCustomer.Count == 0)
            {
                Core.Temp.ListPrestashopCustomer = new Model.Prestashop.PsCustomerRepository().ListIDActiveFull(1, Core.Global.CurrentShop.IDShop);
                filtrage = true;
            }

            if (filtrage)
            {
                List<int> sag_id = Core.Temp.ListF_COMPTET_Light.AsParallel().Select(s => s.cbMarq).ToList();
                Core.Temp.ListLocalCustomer = Core.Temp.ListLocalCustomer.AsParallel().Where(l =>  sag_id.Contains(l.Sag_Id)).ToList();
                List<uint> pre_id = Core.Temp.ListLocalCustomer.AsParallel().Select(l => (uint)l.Pre_Id).ToList();
                Core.Temp.ListPrestashopCustomer = Core.Temp.ListPrestashopCustomer.AsParallel().Where(p => pre_id.Contains(p.id_customer)).ToList();
            }

            if (Core.Temp.ListLocalCustomer.Count > 0 && Core.Temp.ListF_COMPTET_Light.Count > 0
                && (ListLocalCustomer_Remise == null || ListLocalCustomer_Remise.Count == 0))
            {
                foreach (Model.Sage.F_COMPTET_Light centrale in ListF_COMPTET_Centrales)
                    Core.Temp.ListLocalCustomer_Remise.Add(new Model.Local.Customer()
                    {
                        Sag_Id = centrale.cbMarq,
                        Pre_Id = 0,
                    });
                List<Model.Sage.F_COMPTET_Light> listremiseclient = Core.Temp.ListF_COMPTET_Light.AsParallel().Where(s => s.CT_Taux01 != null && s.CT_Taux01 > 0).ToList();
                List<int> sag_id = listremiseclient.AsParallel().Select(s => s.cbMarq).ToList();
                Core.Temp.ListLocalCustomer_Remise.AddRange(Core.Temp.ListLocalCustomer.AsParallel().Where(l => sag_id.Contains(l.Sag_Id)));
                
            }

        }

        public static void InitListesClients()
        {
            Core.Temp.ListLocalCustomer = new List<Model.Local.Customer>();
            Core.Temp.ListF_COMPTET_Light = new List<Model.Sage.F_COMPTET_Light>();
            Core.Temp.ListPrestashopCustomer = new List<Model.Prestashop.idcustomer>();
        }

        public static void LoadF_COMPTET_BtoBIfEmpty()
        {
            if (Core.Temp.ListF_COMPTET_BtoB == null || Core.Temp.ListF_COMPTET_BtoB.Count == 0)
                Core.Temp.ListF_COMPTET_BtoB = new Model.Sage.F_COMPTETRepository().ListBtoB((short)ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_CT_Type.Client);
        }

        #endregion
    }
}
