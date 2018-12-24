using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using PRESTACONNECT.Contexts;
using PRESTACONNECT.Core;
using PRESTACONNECT.View;
using PRESTACONNECT.Model.Internal;
using System.IO;

namespace PRESTACONNECT
{
    public partial class Import : Window
    {
        #region Properties

        internal new ImportContext DataContext
        {
            get { return (ImportContext)base.DataContext; }
            private set
            {
                if (value != null)
                {
                }

                base.DataContext = value;

                if (value != null)
                {
                }
            }
        }

        //Boolean IsLoad = false;

        #endregion
        #region Event methods

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext.Load();
        }

        private void Catalogs_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DataContext.SelectedCatalog = ((TreeView)sender).SelectedItem as Model.Sage.F_CATALOGUE;
            DataContext.LoadSelectedCatalogItems();
        }

        private void CatalogSync_Checked(object sender, RoutedEventArgs e)
        {
            DataContext.ChangeToImportCatalogs(((CheckBox)sender).DataContext as Model.Sage.F_CATALOGUE, true);
        }
        private void CatalogSync_Unchecked(object sender, RoutedEventArgs e)
        {
            DataContext.ChangeToImportCatalogs(((CheckBox)sender).DataContext as Model.Sage.F_CATALOGUE, false);
        }

        private void ToImportSelectedCatalogItems_Click(object sender, RoutedEventArgs e)
        {
            DataContext.CheckAllArticlesToImport();
        }
        private void Advanced_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationAvancee dialog = new ConfigurationAvancee();
            dialog.Owner = this;

            dialog.ShowDialog();
        }

        #endregion

        public Import()
        {
            try
            {
                DataContext = new ImportContext();
                this.InitializeComponent();
                this.LoadButtonImageDocument();
                this.LoadImportSage();

                if (Core.Temp.Current != System.Windows.WindowState.Minimized)
                    this.WindowState = Core.Temp.Current;
                //this.IsLoad = true;
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
            // Insérez le code requis pour la création d’objet sous ce point.
        }

        private void LoadButtonImageDocument()
        {
            Boolean flag = false;
            Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
            if (ConfigRepository.ExistName(Core.AppConfig.ConfigImage))
            {
                if (ConfigRepository.ReadName(Core.AppConfig.ConfigImage).Con_Value.ToString() == "True")
                {
                    flag = true;
                }
            }

            this.ButtonImportPrestashopImage.IsEnabled = flag;
            this.ButtonImportPrestashopDocumentJoin.IsEnabled = flag;

            this.TabItemImportMedia.IsEnabled = flag;
            this.TabItemImportDocumentsRules.IsEnabled = flag;

            this.ButtonClearPsImage.IsEnabled = flag;
        }

        #region ImportSage

        private void LoadImportSage()
        {
            DataContext.LoadCatalogs();
        }

        #region Catalogue

        private void ButtonImportSageCatalog_Click(object sender, RoutedEventArgs e)
        {
            List<int> catalogs = new List<int>();

            foreach (var catalog in DataContext.GetToImportCatalogs())
                catalogs.Add(catalog.cbMarq);

            if (catalogs.Count > 0
                && MessageBox.Show("Voulez-vous importer les " + catalogs.Count + " catalogues Sage sélectionnés ?",
                    "Import catalogues Sage", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ImportSageCatalogue Sync = new ImportSageCatalogue(catalogs);
                Sync.ShowDialog();

                DataContext.LoadCatalogs();
            }
        }

        private void ButtonRefreshImportSageCatalogue_Click(object sender, RoutedEventArgs e)
        {
            DataContext.LoadCatalogs();
        }
        #endregion

        #region Article

        private void ButtonImportSageArticle_Click(object sender, RoutedEventArgs e)
        {
            if (this.radioButtonImportSageCatalogueManuel.IsChecked == true && DataContext.SelectedLocalCatalog == null)
            {
                MessageBox.Show("Vous n'avez pas sélectionné de catalogue par défaut !",
                    "Import Sage impossible", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                List<int> items = new List<int>();
                items = DataContext.GetToImportSelectedCatalogItems();

                if (items.Count > 0
                    && MessageBox.Show("Voulez-vous importer les " + items.Count + " articles Sage sélectionnés" 
                    + ((this.radioButtonImportSageCatalogueManuel.IsChecked == true) ? " dans le catalogue " + DataContext.SelectedLocalCatalog.Cat_Name : "") + " ?",
                        "Import articles Sage", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    ImportSageArticle Sync = new ImportSageArticle(items, (this.radioButtonImportSageCatalogueManuel.IsChecked == true) ? DataContext.SelectedLocalCatalog : null);
                    Sync.ShowDialog();

                    System.Threading.Thread.Sleep(1000);

                    DataContext.LoadSelectedCatalogItems();
                }
            }
        }

        #endregion

        private void ButtonImportSageSupplier_Click(object sender, RoutedEventArgs e)
        {
            ImportSageFournisseur Sync = new ImportSageFournisseur();
            Sync.ShowDialog();
        }

        private void ButtonImportSageCombination_Click(object sender, RoutedEventArgs e)
        {
            //ImportSageGamme Sync = new ImportSageGamme();
            //Sync.ShowDialog();
        }
        
        private void ButtonImportSageImage_Click(object sender, RoutedEventArgs e)
        {
            if (this.TextBoxImportSageImageDir.Text != "")
            {
                if (System.IO.Directory.Exists(this.TextBoxImportSageImageDir.Text))
                {
                    if (Core.Global.GetConfig().ImportImageRemoveDeletedFiles)
                    {
                        SuppressionImage delete = new SuppressionImage(true);
                        delete.ShowDialog();
                    }

                    ImportSageArticleImage Sync = new ImportSageArticleImage(this.TextBoxImportSageImageDir.Text);
                    Sync.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Le répertoire d'image n'existe pas ou n'est pas accessible !", "Import images", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Le répertoire d'image n'est pas renseigné !", "Import images", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private void ButtonImportSageImageRepertoire_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog FBD = new System.Windows.Forms.FolderBrowserDialog();
            if (FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.TextBoxImportSageImageDir.Text = FBD.SelectedPath.ToString();
            }
        }
        private void ButtonSavePictureFolder_Click(object sender, RoutedEventArgs e)
        {
            DataContext.SavePictureFolder();
        }

        private void ButtonImportSageDocumentJoint_Click(object sender, RoutedEventArgs e)
        {
            if (this.TextBoxImportSageDocumentJointDir.Text != "")
            {
                if (System.IO.Directory.Exists(this.TextBoxImportSageDocumentJointDir.Text))
                {
                    ImportSageArticleDocument Sync = new ImportSageArticleDocument(this.TextBoxImportSageDocumentJointDir.Text);
                    Sync.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Le répertoire des documents n'existe pas ou n'est pas accessible !", "Import documents", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Le répertoire des documents n'est pas renseigné !", "Import documents", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private void ButtonImportSageDocumentRepertoire_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog FBD = new System.Windows.Forms.FolderBrowserDialog();
            if (FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.TextBoxImportSageDocumentJointDir.Text = FBD.SelectedPath.ToString();
            }
        }
        private void ButtonSaveDocumentFolder_Click(object sender, RoutedEventArgs e)
        {
            DataContext.SaveDocumentFolder();
        }

        private void ButtonImportSageMedia_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.TextBoxImportSageMediaDir.Text))
            {
                if (System.IO.Directory.Exists(this.TextBoxImportSageMediaDir.Text))
                {
                    ImportSageArticleMedia Sync = new ImportSageArticleMedia(this.TextBoxImportSageMediaDir.Text);
                    Sync.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Le répertoire multimédia de Sage est introuvable ou inaccessible !", "Import Sage", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Le répertoire des fichiers multimédias n'est pas renseigné !", "Import Sage", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private void ButtonImportSageMediaRepertoire_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog FBD = new System.Windows.Forms.FolderBrowserDialog();
            if (FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.TextBoxImportSageMediaDir.Text = FBD.SelectedPath.ToString();
            }
        }
        private void ButtonSaveMediaFolder_Click(object sender, RoutedEventArgs e)
        {
            DataContext.SaveMediaFolder();
        }

        private void ButtonAddMediaRule_Click(object sender, RoutedEventArgs e)
        {
            DataContext.AddMediaAssignmentRule();
        }
        private void ButtonDeleteRule_Click(object sender, RoutedEventArgs e)
        {
            DataContext.DeleteMediaAssignmentRule();
        }

        #endregion

        #region ImportPrestashop

        private void ButtonImportPrestashopCatalog_Click(object sender, RoutedEventArgs e)
        {
            ImportPrestashopCatalogue Sync = new ImportPrestashopCatalogue();
            Sync.ShowDialog();
        }

        private void ButtonImportPrestashopArticle_Click(object sender, RoutedEventArgs e)
        {
            ImportPrestashopArticle Sync = new ImportPrestashopArticle(DataContext.ForceImportProduct);
            Sync.ShowDialog();
        }

        private void ButtonImportPrestashopImage_Click(object sender, RoutedEventArgs e)
        {
            ImportPrestashopArticleImage Sync = new ImportPrestashopArticleImage(DataContext.ForceImportImage);
            Sync.ShowDialog();
        }

        private void ButtonImportPrestashopDocumentJoint_Click(object sender, RoutedEventArgs e)
        {
            ImportPrestashopArticleDocument Sync = new ImportPrestashopArticleDocument();
            Sync.ShowDialog();
        }

        #endregion

        #region outils

        private void ButtonClearIdPsProduct_Click(object sender, RoutedEventArgs e)
        {
            DataContext.ControleMappageArticles((this.CheckBoxControleArticleActivation.IsChecked == true) ? true : false);
        }

        private void ButtonClearPsImage_Click(object sender, RoutedEventArgs e)
        {
            DataContext.ClearPsImage();
        }

        private void buttonClearSmartyCache_Click(object sender, RoutedEventArgs e)
        {
            Core.Global.LaunchAlternetis_ClearSmartyCache();
        }

        private void buttonControlURLRewrite_Click(object sender, RoutedEventArgs e)
        {
            DataContext.ControlURLRewrite(this.Ck_RewritePrestashop.IsChecked.Value);
        }

        #endregion

        public void LoadSelectedCatalogItems()
        {
            DataContext.LoadSelectedCatalogItems();
        }

        private void textBoxImportFilter_GotFocus(object sender, RoutedEventArgs e)
        {
            //this.buttonSearch.IsDefault = true;
        }

        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            DataContext.LoadSelectedCatalogItems();
        }

        private void ButtonDeleteImportFilter_Click(object sender, RoutedEventArgs e)
        {
            DataContext.DeleteImportSageFilter();
        }

        private void ButtonAddImportSageFilter_Click(object sender, RoutedEventArgs e)
        {
            DataContext.AddImportSageFilter();
        }

        private void buttonLienConditionnement_Click(object sender, RoutedEventArgs e)
        {
            DataContext.OpenLienConditionnement();
        }

        private void buttonImportCompositionsGammes_Click(object sender, RoutedEventArgs e)
        {
            ImportCompositionGammes form = new ImportCompositionGammes();
            form.ShowDialog();
        }

        private void buttonListPrestashop_Click(object sender, RoutedEventArgs e)
        {
            ImportPrestashopArticleManuel form = new ImportPrestashopArticleManuel();
            form.ShowDialog();
        }
	}
}