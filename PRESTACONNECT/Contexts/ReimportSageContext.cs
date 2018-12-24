using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using PRESTACONNECT.Model.Internal;

namespace PRESTACONNECT.Contexts
{
    internal sealed class ReimportSageContext : Context
    {

        #region Properties

        private List<Int32> listArticles = new List<int>();
        public List<Int32> ListArticles
        {
            get { return listArticles; }
            set
            {
                listArticles = value;
                NombreArticleString = (listArticles != null) ? listArticles.Count + " articles sélectionnés" : "Aucun article sélectionné";
                OnPropertyChanged("ListArticles");
            }
        }

        private string nombreArticleString = string.Empty;
        public string NombreArticleString
        {
            get { return nombreArticleString; }
            set
            {
                nombreArticleString = value;
                OnPropertyChanged("NombreArticleString");
            }
        }

        public bool UpdateMainCatalog
        {
            get { return Core.Global.GetConfig().ReimportUpdateMainCatalog; }
            set
            {
                Core.Global.GetConfig().UpdateReimportUpdateMainCatalog(value);
                OnPropertyChanged("UpdateMainCatalog");
            }
        }

        public bool LinkParents
        {
            get { return Core.Global.GetConfig().ReimportLinkParents; }
            set
            {
                Core.Global.GetConfig().UpdateReimportLinkParents(value);
                OnPropertyChanged("LinkParents");
            }
        }

        public bool DeleteLinkOldMain
        {
            get { return Core.Global.GetConfig().ReimportDeleteLinkOldMain; }
            set
            {
                Core.Global.GetConfig().UpdateReimportDeleteLinkOldMain(value);
                OnPropertyChanged("DeleteLinkOldMain");
            }
        }

        public bool DeleteLinkOldSecondary
        {
            get { return Core.Global.GetConfig().ReimportDeleteLinkOldSecondary; }
            set
            {
                Core.Global.GetConfig().UpdateReimportDeleteLinkOldSecondary(value);
                OnPropertyChanged("DeleteLinkOldSecondary");
            }
        }

        public bool UpdateProductName
        {
            get { return Core.Global.GetConfig().ReimportUpdateProductName; }
            set
            {
                Core.Global.GetConfig().UpdateReimportUpdateProductName(value);
                OnPropertyChanged("UpdateProductName");
            }
        }

        public bool UpdateDescriptionShort
        {
            get { return Core.Global.GetConfig().ReimportUpdateDescriptionShort; }
            set
            {
                Core.Global.GetConfig().UpdateReimportUpdateDescriptionShort(value);
                OnPropertyChanged("UpdateDescriptionShort");
            }
        }

        public bool UpdateDescription
        {
            get { return Core.Global.GetConfig().ReimportUpdateDescription; }
            set
            {
                Core.Global.GetConfig().UpdateReimportUpdateDescription(value);
                OnPropertyChanged("UpdateDescription");
            }
        }

        public bool UpdateMetaTitle
        {
            get { return Core.Global.GetConfig().ReimportUpdateMetaTitle; }
            set
            {
                Core.Global.GetConfig().UpdateReimportUpdateMetaTitle(value);
                OnPropertyChanged("UpdateMetaTitle");
            }
        }

        public bool UpdateMetaDescription
        {
            get { return Core.Global.GetConfig().ReimportUpdateMetaDescription; }
            set
            {
                Core.Global.GetConfig().UpdateReimportUpdateMetaDescription(value);
                OnPropertyChanged("UpdateMetaDescription");
            }
        }

        public bool UpdateMetaKeywords
        {
            get { return Core.Global.GetConfig().ReimportUpdateMetaKeywords; }
            set
            {
                Core.Global.GetConfig().UpdateReimportUpdateMetaKeywords(value);
                OnPropertyChanged("UpdateMetaKeywords");
            }
        }

        public bool UpdateURL
        {
            get { return Core.Global.GetConfig().ReimportUpdateURL; }
            set
            {
                Core.Global.GetConfig().UpdateReimportUpdateURL(value);
                UpdateURLString = (value) ? "À ne pas modifier si vos produits sont déjà référencés !!" : string.Empty;
                OnPropertyChanged("UpdateURL");
            }
        }

        private string updateURLString = string.Empty;
        public string UpdateURLString
        {
            get { return updateURLString; }
            set
            {
                updateURLString = value;
                OnPropertyChanged("UpdateURLString");
            }
        }

        public bool UpdateEAN
        {
            get { return Core.Global.GetConfig().ReimportUpdateEAN; }
            set
            {
                Core.Global.GetConfig().UpdateReimportUpdateEAN(value);
                OnPropertyChanged("UpdateEAN");
            }
        }

        public bool UpdateActive
        {
            get { return Core.Global.GetConfig().ReimportUpdateActive; }
            set
            {
                Core.Global.GetConfig().UpdateReimportUpdateActive(value);
                OnPropertyChanged("UpdateActive");
            }
        }

        public bool UpdateCharacteristic
        {
            get { return Core.Global.GetConfig().ReimportUpdateCharacteristic; }
            set
            {
                Core.Global.GetConfig().UpdateReimportUpdateCharacteristic(value);
                OnPropertyChanged("UpdateCharacteristic");
            }
        }

        public bool UpdateAttribute
        {
            get { return Core.Global.GetConfig().ReimportUpdateAttribute; }
            set
            {
                Core.Global.GetConfig().UpdateReimportUpdateAttribute(value);
                OnPropertyChanged("UpdateAttribute");
            }
        }

        public bool UpdateConditioning
        {
            get { return Core.Global.GetConfig().ReimportUpdateConditioning; }
            set
            {
                Core.Global.GetConfig().UpdateReimportUpdateConditioning(value);
                OnPropertyChanged("UpdateConditioning");
            }
        }

        public Boolean ConditioningActive
        {
            get { return Core.Global.GetConfig().ArticleImportConditionnementActif; }
        }

        public bool UpdateDateActive
        {
            get { return Core.Global.GetConfig().ReimportUpdateDateActive; }
            set
            {
                if (value ||
                    (!value && new PRESTACONNECT.View.PrestaMessage("Attention la désactivation de cette option peut entraîner des pertes de données entre PrestaConnect et PrestaShop"
                         + " pour les traitements utilisant la date de mise-à-jour comme déclencheur !"
                         + "\n\n"
                         + "Valider la désactivation de cette option ?",
                    "Désactivation mise-à-jour date de modification article", MessageBoxButton.YesNo, MessageBoxImage.Warning, 600).ShowDialog() == true))
                {
                    Core.Global.GetConfig().UpdateReimportUpdateDateActive(value);
                }
                OnPropertyChanged("UpdateDateActive");
            }
        }

        #endregion

        #region Constructors

        public ReimportSageContext(List<Int32> Articles)
            : base()
        {
            ListArticles = Articles;

            // paramètres auto-chargés-sauvés depuis AppConfig

            // Cette valeur doit toujours être à true par défaut avant un réimport
            if (!Core.Global.GetConfig().ReimportUpdateDateActive)
                Core.Global.GetConfig().UpdateReimportUpdateDateActive(true);                
        }

        #endregion

        #region Methods

        public void LaunchReimportSage()
        {
            if (MessageBox.Show("Lancer la réimportation des données depuis Sage ?", "Réimport Sage",
                    MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
                Loading.Show();

                ReimportArticle ReimportArticle = new ReimportArticle(ListArticles);
                ReimportArticle.ShowDialog();

                Loading.Close();
            }
        }

        #endregion
    }
}
