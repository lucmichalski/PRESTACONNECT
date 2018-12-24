using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using PRESTACONNECT.Contexts;
using PRESTACONNECT.Model.Internal;

namespace PRESTACONNECT
{
    public partial class Article : Window
    {
        #region Properties

        internal new ArticleContext DataContext
        {
            get { return (ArticleContext)base.DataContext; }
            private set
            {
                base.DataContext = value;
            }
        }

        internal int SelectedTabItemIndex
        {
            get { return (int)GetValue(SelectedTabItemIndexProperty); }
            set { SetValue(SelectedTabItemIndexProperty, value); }
        }
        internal static readonly DependencyProperty SelectedTabItemIndexProperty =
            DependencyProperty.Register("SelectedTabItemIndex", typeof(int), typeof(Article));

        #endregion
        #region Constructors

        public Article(Model.Local.Article local)
        {
            DataContext = new ArticleContext(local);
            DataContext.LoadCatalogs();

            this.InitializeComponent();

            if (Core.Global.GetConfig().UIDisabledWYSIWYG)
            {
                this.TabItemWYSIWYGResume.Visibility = System.Windows.Visibility.Collapsed;
                this.TabItemWYSIWYGDescription.Visibility = System.Windows.Visibility.Collapsed;
                this.ButtonReloadDescription.Visibility = System.Windows.Visibility.Collapsed;
                this.TabItemHTMLEditResume.IsSelected = true;
                this.TabItemHTMLEditDescription.IsSelected = true;
                this.buttonInsertHTMLResume.Visibility = System.Windows.Visibility.Hidden;
                this.buttonLoadHTMLResume.Visibility = System.Windows.Visibility.Hidden;
                this.buttonInsertHTMLDescription.Visibility = System.Windows.Visibility.Hidden;
                this.buttonLoadHTMLDescription.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                this.TinyMceResume.CreateEditor();
                this.TinyMceDescription.CreateEditor();

                this.TabItemDescription.IsSelected = true;
            }

            //this.LoadComboBoxCharacteristic();
            this.LoadComponent();

            if (Core.Temp.Current != System.Windows.WindowState.Minimized)
                this.WindowState = Core.Temp.Current;
        }

        #endregion
        #region Event methods

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            DataContext.Load();
            if (Core.Global.GetConfig().UIDisabledWYSIWYG)
            {
                this.TextBoxInsertHTMLResume.Text = DataContext.Item.DescriptionShort;
                this.TextBoxInsertHTMLDescription.Text = DataContext.Item.Description;
                this.IsEnabled = true;
            }
            else if (IsLoaded)
            {
                Task.Factory.StartNew(delegate
                {
                    Thread.Sleep(Core.Global.GetConfig().UISleepTimeWYSIWYG);

                    Application.Current.Dispatcher.BeginInvoke(
                        new Action(delegate
                    {
                        this.TinyMceResume.HtmlContent = DataContext.Item.DescriptionShort;
                        this.TinyMceDescription.HtmlContent = DataContext.Item.Description;
                        if (DataContext.Item.Description != this.TinyMceDescription.HtmlContent)
                            DataContext.Item.Description = this.TinyMceDescription.HtmlContent;
                        if (DataContext.Item.DescriptionShort != this.TinyMceResume.HtmlContent)
                            DataContext.Item.DescriptionShort = this.TinyMceResume.HtmlContent;
                        DataContext.Item.HasUpdated = false;
                        this.IsEnabled = true;
                    }), null);

                });
            }
        }

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SelectedTabItemIndex = ((TabControl)sender).SelectedIndex;
        }
        private void Catalogs_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DataContext.SelectedCatalog = ((TreeView)sender).SelectedItem as AssociateCatalog;
        }

        private void CatalogAssociate_Checked(object sender, RoutedEventArgs e)
        {
            DataContext.ChangeToAssociateCatalogs(((CheckBox)sender).DataContext as AssociateCatalog, true);
        }
        private void CatalogAssociate_Unchecked(object sender, RoutedEventArgs e)
        {
            DataContext.ChangeToAssociateCatalogs(((CheckBox)sender).DataContext as AssociateCatalog, false);
        }

        private void CatalogDefault_Click(object sender, RoutedEventArgs e)
        {
            DataContext.DefaultCatalog(DataContext.SelectedCatalog);
        }

        #endregion

        // <JG> 28/05/2012
        private String ImageFileName = string.Empty;

        private void LoadComponent()
        {
            this.LoadArticleImage();
            this.LoadArticleAttachement();

            this.TabItemImage.IsEnabled = Core.Global.GetConfig().ConfigFTPActive;
            this.TabItemDocument.IsEnabled = Core.Global.GetConfig().ConfigFTPActive;

            if (DataContext.Item.Sage != null
                && DataContext.Item.Sage.AR_Gamme1 != null
                && DataContext.Item.Sage.AR_Gamme1 != 0)
            {
                this.TabItemGamme.IsEnabled = true;

                if (new Model.Local.AttributeGroupRepository().ExistSage((int)DataContext.Item.Sage.AR_Gamme1))
                    this.LabelAttributeGroup1.Content = new Model.Sage.P_GAMMERepository().ReadGamme((int)DataContext.Item.Sage.AR_Gamme1).G_Intitule;

                if (DataContext.Item.Sage.AR_Gamme2 != null
                    && DataContext.Item.Sage.AR_Gamme2 != 0)
                {
                    if (new Model.Local.AttributeGroupRepository().ExistSage((int)DataContext.Item.Sage.AR_Gamme2))
                        this.LabelAttributeGroup2.Content = new Model.Sage.P_GAMMERepository().ReadGamme((int)DataContext.Item.Sage.AR_Gamme2).G_Intitule;
                }
                else
                {
                    this.LabelAttributeGroup2.Content = "Pas de gamme 2";
                    // masquer colonne enuméré 2
                }
            }
            else
            {
                this.TabItemGamme.IsEnabled = false;
            }

            this.ButtonImportConditioningArticle.IsEnabled = Core.Global.GetConfig().ArticleImportConditionnementActif;

            if (DataContext.Item.Sage != null
                && DataContext.Item.Sage.AR_Condition != null
                && DataContext.Item.Sage.AR_Condition != 0)
            {
                this.TabItemConditionnement.IsEnabled = true;

                if (new Model.Local.ConditioningGroupRepository().ExistSage((int)DataContext.Item.Sage.AR_Condition))
                    this.LabelConditioningGroup.Content = new Model.Sage.P_CONDITIONNEMENTRepository().ReadConditionnement((int)DataContext.Item.Sage.AR_Condition).P_Conditionnement;
            }
            else
            {
                this.TabItemConditionnement.IsEnabled = false;
            }

            // <JG> ajout article composition
            if (DataContext.Item.Local.Art_Id == 0)
            {
                this.TabItemImage.IsEnabled = false;
                this.TabItemDocument.IsEnabled = false;
                this.TabItemCaracteristique.IsEnabled = false;
                this.TabItemComposition.IsEnabled = false;
                this.ButtonSync.IsEnabled = false;
            }
            else
            {
                this.TabItemCaracteristique.IsEnabled = true;
                this.TabItemComposition.IsEnabled = (DataContext.Item.Local.Art_Type == (short)Model.Local.Article.enum_TypeArticle.ArticleComposition
                    && Core.UpdateVersion.License.Option2);
            }

			this.TabItemModuleDWF.IsEnabled = Core.Global.GetConfig().ModuleDWFProductGuideratesActif || Core.Global.GetConfig().ModuleDWFProductExtraFieldsActif;
		}

        #region Button
        private void ButtonSync_Click(object sender, RoutedEventArgs e)
        {
            Boolean sync = true;
            ApplyChanges();
            if (DataContext.Item.Updated)
                if (Core.Global.GetConfig().UIProductUpdateValidationDisabled
                        || MessageBox.Show("Vos modifications vont être enregistrées, continuer ?", "PrestaConnect", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    this.Write();
                else sync = false;

            if (sync)
            {
                PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
                Loading.Show();
                SynchronisationFournisseur SyncFournisseur = new SynchronisationFournisseur();
                SyncFournisseur.ShowDialog();

                List<Int32> catalogues = new Model.Local.ArticleCatalogRepository().ListCataloguesArticle(DataContext.Item.Id);

                SynchronisationCatalogue SyncCatalogue = new SynchronisationCatalogue(catalogues);
                SyncCatalogue.ShowDialog();

                //SynchronisationGamme SyncGamme = new SynchronisationGamme();
                //SyncGamme.ShowDialog();

                //SynchronisationGammeEnumere SyncGammeEnumere = new SynchronisationGammeEnumere(DataContext.Item.Id);
                //SyncGammeEnumere.ShowDialog();

                //SynchronisationConditionnement SyncConditionnement = new SynchronisationConditionnement();
                //SyncConditionnement.ShowDialog();

                //SynchronisationConditionnementEnumere SyncConditionnementEnumere = new SynchronisationConditionnementEnumere(DataContext.Item.Id);
                //SyncConditionnementEnumere.ShowDialog();

                SynchronisationArticle Sync = new SynchronisationArticle(DataContext.Item.Id);
                Sync.ShowDialog();

                if (Core.Global.GetConfig().ConfigFTPActive)
                {
                    if (!string.IsNullOrEmpty(Core.Global.GetConfig().ConfigFTPIP)
                        && !string.IsNullOrEmpty(Core.Global.GetConfig().ConfigFTPUser)
                        && !string.IsNullOrEmpty(Core.Global.GetConfig().ConfigFTPPassword))
                    {
                        TransfertArticleImage SyncArticleImage = new TransfertArticleImage(DataContext.Item.Id);
                        SyncArticleImage.ShowDialog();
                        TransfertArticleDocument SyncArticleDocument = new TransfertArticleDocument(DataContext.Item.Id);
                        SyncArticleDocument.ShowDialog();
                    }
                }
                Loading.Close();

                this.InitContext();
            }
        }

        private void ButtonSyncStockPrice_Click(object sender, RoutedEventArgs e)
        {
            PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
            Loading.Show();
            TransfertStockPrice SyncStockPrice = new TransfertStockPrice();
            SyncStockPrice.ShowDialog();
            Loading.Close();
        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            Boolean write = false;
            ApplyChanges();
            if (DataContext.Item.Local.Art_Id != 0)
            {
                if (DataContext.Item.Updated)
                    if (Core.Global.GetConfig().UIProductUpdateValidationDisabled
                        || MessageBox.Show("Enregistrer vos modifications ?", "PrestaConnect", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        write = true;

                if (write)
                {
                    Write();
                    System.Windows.MessageBox.Show("Article mis à jour avec succès", "Article", MessageBoxButton.OK);
                    if (DataContext.Item.Local.Art_Type == (short)Model.Local.Article.enum_TypeArticle.ArticleComposition)
                    {
                        this.InitContext();
                    }
                }
            }
            else if (DataContext.Item.Local.Art_Type == (short)Model.Local.Article.enum_TypeArticle.ArticleComposition)
            {
                if (DataContext.Item.Local.Cat_Id == 0)
                {
                    MessageBox.Show("Veuillez définir un catalogue par défaut !", "Catalogue par défaut", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                else if (MessageBox.Show("Valider la création de l'article de composition : " + DataContext.Item.Name + " ? ",
                    "Création composition", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Write();
                    DataContext = new ArticleContext(DataContext.Item.Local);
                    DataContext.LoadCatalogs();
                    LoadComponent();
                }
            }
        }

        private void InitContext()
        {
            Model.Local.Article local = new Model.Local.ArticleRepository().ReadArticle(DataContext.Item.Local.Art_Id);
            if (local.Art_Date > DataContext.Item.Local.Art_Date || local.Pre_Id != DataContext.Item.Local.Pre_Id)
            {
                DataContext = new ArticleContext(local);
                DataContext.LoadCatalogs();
                LoadComponent();
            }
        }

        public void ApplyChanges()
        {
            if (Core.Global.GetConfig().UIDisabledWYSIWYG)
            {
                if (!string.IsNullOrWhiteSpace(this.TextBoxInsertHTMLResume.Text)
                    && DataContext.Item.DescriptionShort != this.TextBoxInsertHTMLResume.Text)
                    DataContext.Item.HasUpdated = true;
                if (!string.IsNullOrWhiteSpace(this.TextBoxInsertHTMLDescription.Text)
                    && DataContext.Item.Description != this.TextBoxInsertHTMLDescription.Text)
                    DataContext.Item.HasUpdated = true;
            }
            else
            {
                // descriptions
                if (!string.IsNullOrWhiteSpace(this.TinyMceDescription.HtmlContent)
                    && DataContext.Item.Description != this.TinyMceDescription.HtmlContent
                    && DataContext.Item.Description != this.TinyMceDescription.HtmlContentNoP)
                    DataContext.Item.HasUpdated = true;
                //DataContext.Item.Description = this.TinyMceDescription.HtmlContent;
                if (!string.IsNullOrWhiteSpace(this.TinyMceResume.HtmlContent)
                    && DataContext.Item.DescriptionShort != this.TinyMceResume.HtmlContent
                    && DataContext.Item.DescriptionShort != this.TinyMceResume.HtmlContentNoP)
                    DataContext.Item.HasUpdated = true;
                //DataContext.Item.DescriptionShort = this.TinyMceResume.HtmlContent;
            }

            // déclinaisons de gammes
            if (DataContext.ListAttributeArticle.Count(a => a._HasUpdated) > 0)
                DataContext.Item.HasUpdatedAttribute = true;

            // déclinaisons de conditionnement
            if (DataContext.ListConditioningArticle.Count(c => c._HasUpdated) > 0)
                DataContext.Item.HasUpdatedConditioning = true;

            // déclinaisons composition
            if (DataContext.ListCompositionArticle.Count(c => c.Updated) > 0)
                DataContext.Item.HasUpdatedCompositionArticle = true;

            if (DataContext.ListFeature.Count(f => f.HasUpdated) > 0)
                DataContext.Item.HasUpdatedCharacteristic = true;
        }

        private void Write()
        {
            if (Core.Global.GetConfig().UIDisabledWYSIWYG)
            {
                if (!string.IsNullOrWhiteSpace(this.TextBoxInsertHTMLResume.Text) && DataContext.Item.DescriptionShort != this.TextBoxInsertHTMLResume.Text)
                    DataContext.Item.DescriptionShort = this.TextBoxInsertHTMLResume.Text;
                if (!string.IsNullOrWhiteSpace(this.TextBoxInsertHTMLDescription.Text) && DataContext.Item.Description != this.TextBoxInsertHTMLDescription.Text)
                    DataContext.Item.Description = this.TextBoxInsertHTMLDescription.Text;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(this.TinyMceDescription.HtmlContent) && DataContext.Item.Description != this.TinyMceDescription.HtmlContent)
                    DataContext.Item.Description = this.TinyMceDescription.HtmlContent;
                if (!string.IsNullOrWhiteSpace(this.TinyMceResume.HtmlContent) && DataContext.Item.DescriptionShort != this.TinyMceResume.HtmlContent)
                    DataContext.Item.DescriptionShort = this.TinyMceResume.HtmlContent;
            }

            DataContext.Update();

            this.WriteArticleCatalog(DataContext.Item.Local);

            this.WriteImage(DataContext.Item.Local);
            this.WriteAttachment(DataContext.Item.Local);

            this.LoadArticleImage();
            this.LoadArticleAttachement();
        }

        private void WriteArticleCatalog(Model.Local.Article Article)
        {
            Model.Local.ArticleCatalogRepository ArticleCatalogRepository = new Model.Local.ArticleCatalogRepository();
            foreach (AssociateCatalog Catalog in DataContext.GetCatalogs())
            {
                if (Catalog.ToAssociate)
                {
                    if (ArticleCatalogRepository.ExistArticleCatalog(Article.Art_Id, Catalog.Id) == false)
                    {
                        Model.Local.ArticleCatalog ArticleCatalog = new Model.Local.ArticleCatalog()
                        {
                            Art_Id = Article.Art_Id,
                            Cat_Id = Catalog.Id
                        };
                        ArticleCatalogRepository.Add(ArticleCatalog);
                        DataContext.UpdateDate();
                    }
                }
                else
                {
                    if (ArticleCatalogRepository.ExistArticleCatalog(Article.Art_Id, Catalog.Id))
                    {
                        Model.Local.ArticleCatalog ArticleCatalog = ArticleCatalogRepository.ReadArticleCatalog(Article.Art_Id, Catalog.Id);
                        ArticleCatalogRepository.Delete(ArticleCatalog);
                        DataContext.UpdateDate();
                    }
                }
            }
        }

        private void WriteImage(Model.Local.Article Article)
        {
            int IdArticleImage = 0;
            try
            {
                if (this.ImageArticleImage.Source.ToString().Contains("pack://application:") == false && this.ImageFileName != "")
                {
                    String extension = Path.GetExtension(this.ImageFileName).ToLower();

                    if (Core.Img.imageExtensions.Contains(extension))
                    {
                        Model.Local.ArticleImageRepository ArticleImageRepository = new Model.Local.ArticleImageRepository();
                        Model.Local.ArticleImage ArticleImage = new Model.Local.ArticleImage();

                        Int32 Position = numericUpDownImagePosition.Value;
                        while (ArticleImageRepository.ExistArticlePosition(Article.Art_Id, Position))
                        {
                            Position++;
                        }
                        ArticleImage.ImaArt_Position = Position;

                        ArticleImage.ImaArt_Default = (DataGridImage.ItemsSource == null || ((List<Model.Local.ArticleImage>)DataGridImage.ItemsSource).Count == 0 || checkBoxImageDefaut.IsChecked == true);
                        ArticleImage.ImaArt_Name = (!string.IsNullOrWhiteSpace(this.TextBoxArticleImageLegend.Text)) ? this.TextBoxArticleImageLegend.Text : DataContext.Item.Name;
                        ArticleImage.Art_Id = Article.Art_Id;
                        ArticleImage.ImaArt_Image = string.Empty;
                        // <JG> 28/05/2012
                        ArticleImage.ImaArt_DateAdd = DateTime.Now;
                        ArticleImage.ImaArt_SourceFile = this.ImageFileName;
                        //-----END
                        ArticleImageRepository.Add(ArticleImage);
                        //ArticleImage.ImaArt_Image = System.IO.Path.Combine(Global.GetConfig().Folders.SmallArticle, String.Format("{0}.jpg", ArticleImage.ImaArt_Id));
                        ArticleImage.ImaArt_Image = String.Format("{0}" + extension, ArticleImage.ImaArt_Id);
                        ArticleImageRepository.Save();
                        IdArticleImage = ArticleImage.ImaArt_Id;

                        if (ArticleImage.ImaArt_Default == true)
                        {
                            List<Model.Local.ArticleImage> ListArticleImageDefault = ArticleImageRepository.ListArticle(Article.Art_Id);
                            if (ListArticleImageDefault.Count(i => i.ImaArt_Default == true && i.ImaArt_Id != ArticleImage.ImaArt_Id) > 0)
                                foreach (Model.Local.ArticleImage ArticleImageDefault in ListArticleImageDefault.Where(i => i.ImaArt_Default == true && i.ImaArt_Id != ArticleImage.ImaArt_Id))
                                {
                                    ArticleImageDefault.ImaArt_Default = false;
                                    ArticleImageRepository.Save();
                                }
                        }

                        string uri = this.ImageArticleImage.Source.ToString()
                            .Replace("File:///", "").Replace("file:///", "").Replace("File://", "\\\\").Replace("file://", "\\\\").Replace("/", "\\");

                        System.IO.File.Copy(uri, ArticleImage.TempFileName);

                        Model.Prestashop.PsImageTypeRepository PsImageTypeRepository = new Model.Prestashop.PsImageTypeRepository();
                        List<Model.Prestashop.PsImageType> ListPsImageType = PsImageTypeRepository.ListProduct(1);
                        System.Drawing.Image img = System.Drawing.Image.FromFile(ArticleImage.TempFileName);

                        foreach (Model.Prestashop.PsImageType PsImageType in ListPsImageType)
                            Core.Img.resizeImage(img, Convert.ToInt32(PsImageType.Width), Convert.ToInt32(PsImageType.Height),
                                ArticleImage.FileName(PsImageType.Name));

                        Core.Img.resizeImage(img, Core.Global.GetConfig().ConfigImageMiniatureWidth, Core.Global.GetConfig().ConfigImageMiniatureHeight,
                                ArticleImage.SmallFileName);

                        img.Dispose();

                        this.TextBoxArticleImageLegend.Text = string.Empty;
                        this.numericUpDownImagePosition.Value = 0;
                        this.checkBoxImageDefaut.IsChecked = false;
                        BitmapImage Img = new BitmapImage(new Uri("pack://application:,,,/PRESTACONNECT;component/Img/img.jpg", UriKind.RelativeOrAbsolute));
                        this.ImageArticleImage.Source = Img;
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[IMPORT IMAGE ARTICLE]<br />" + ex.ToString());
                if (ex.ToString().Contains("System.UnauthorizedAccessException") && IdArticleImage != 0)
                {
                    Model.Local.ArticleImageRepository ArticleImageRepository = new Model.Local.ArticleImageRepository();
                    ArticleImageRepository.Delete(ArticleImageRepository.ReadArticleImage(IdArticleImage));
                }
            }
        }

        private void WriteAttachment(Model.Local.Article Article)
        {
            if (this.TextBoxArticleAttachmentFileName.Text.ToString().Contains("pack://application:") == false && this.TextBoxArticleAttachmentFileName.Text.ToString() != "")
            {

                Model.Local.AttachmentRepository AttachmentRepository = new Model.Local.AttachmentRepository();
                Model.Local.Attachment Attachment = new Model.Local.Attachment();

                Attachment.Att_File = GetRandomFileName();
                Attachment.Att_Name = this.TextBoxArticleAttachmentName.Text;
                String[] ArrayFileName = this.TextBoxArticleAttachmentFileName.Text.Split('\\');
                Attachment.Att_FileName = ArrayFileName[ArrayFileName.Length - 1];
                Attachment.Att_Description = this.TextBoxArticleAttachmentDescription.Text;
                String[] ArrayMime = this.TextBoxArticleAttachmentFileName.Text.Split('.');
                String Mime = ArrayMime[ArrayMime.Length - 1];
                switch (Mime)
                {
                    case "pdf":
                        Attachment.Att_Mime = "application/pdf";
                        break;
                    default:
                        Attachment.Att_Mime = "application/pdf";
                        break;

                }
                Attachment.Art_Id = DataContext.Item.Id;
                AttachmentRepository.Add(Attachment);

                String Dir = Core.Global.GetConfig().Folders.RootAttachment + Attachment.Att_File;
                String Uri = this.TextBoxArticleAttachmentFileName.Text.ToString();
                Uri = Uri.Replace("File:///", "");
                Uri = Uri.Replace("file:///", "");
                Uri = Uri.Replace("File://", "\\\\");
                Uri = Uri.Replace("file://", "\\\\");
                Uri = Uri.Replace("/", "\\");
                System.IO.File.Copy(Uri, Dir);

                this.TextBoxArticleAttachmentFileName.Text = "";
                this.TextBoxArticleAttachmentName.Text = "";
                this.TextBoxArticleAttachmentDescription.Text = "";
            }
        }

        #endregion

        #region Characteristic

        private void ButtonImportSageInformationLibre_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext.ImportSageInformationLibre())
            {
                DataContext = new ArticleContext(new Model.Local.ArticleRepository().ReadArticle(this.DataContext.Item.Id));
                DataContext.LoadCatalogs();
            }
        }

        #endregion

        #region Image
        private void ButtonArticleImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog OFD = new Microsoft.Win32.OpenFileDialog();
            // <JG> 11/05/2012
            OFD.Filter = "Images (*.jpg, *.png, *.gif) | *.jpg;*.png;*.gif";
            OFD.Multiselect = false;

            if (OFD.ShowDialog() == true)
            {
                Model.Local.ArticleImageRepository ArticleImageRepository = new Model.Local.ArticleImageRepository();
                if (ArticleImageRepository.ExistArticleFile(DataContext.Item.Id, OFD.SafeFileName.Trim()) == false)
                {
                    BitmapImage Img = new BitmapImage(new Uri(OFD.FileName, UriKind.RelativeOrAbsolute));
                    this.ImageArticleImage.Source = Img;
                    // <JG> 28/05/2012
                    this.ImageFileName = OFD.SafeFileName.Trim();
                    DataContext.Item.HasSelectedImage = true;
                }
                else
                {
                    MessageBox.Show("Le fichier " + OFD.SafeFileName.Trim() + " a déjà été importé pour l'article " + DataContext.Item.Local.Art_Ref + " !",
                        "Image déjà importée", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
            OFD = null;
        }

        private void DataGridImageButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Model.Local.ArticleImage ArticleImage = this.DataGridImage.SelectedItem as Model.Local.ArticleImage;
                if (MessageBox.Show("Supprimer l'image " + ArticleImage.ImaArt_Name + " ?", "Suppression", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Model.Local.ArticleRepository articleRepository = new Model.Local.ArticleRepository();
                    Model.Local.Article article = articleRepository.ReadArticle(ArticleImage.Art_Id.Value);

                    if (article != null)
                    {
                        Core.Tools.SuppressionImage delete = new Core.Tools.SuppressionImage();
                        List<string> log;
                        delete.Exec(ArticleImage, article, out log);

                        this.LoadArticleImage();
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void LoadArticleImage()
        {
            Model.Local.ArticleImageRepository ArticleImageRepository = new Model.Local.ArticleImageRepository();

            this.DataGridImage.ItemsSource = ArticleImageRepository.ListArticle(DataContext.Item.Id);
        }
        #endregion

        #region Attachment
        private void ButtonArticleAttachment_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog OFD = new Microsoft.Win32.OpenFileDialog();
            OFD.Filter = "Document (*.pdf) | *.pdf;";
            OFD.Multiselect = false;

            if (OFD.ShowDialog() == true)
            {
                this.TextBoxArticleAttachmentFileName.Text = OFD.FileName;
                DataContext.Item.HasSelectedDocument = true;
            }
        }

        private void DataGridAttachmentButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Model.Local.Attachment Attachment = this.DataGridAttachment.SelectedItem as Model.Local.Attachment;
                if (MessageBox.Show("Supprimer le document " + Attachment.Att_Name + " ?", "Suppression document", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Model.Local.AttachmentRepository AttachmentRepository = new Model.Local.AttachmentRepository();
                    if (AttachmentRepository.ExistAttachment(Attachment.Att_Id))
                    {
                        Model.Local.Attachment AttachmentDelete = AttachmentRepository.ReadAttachment(Attachment.Att_Id);
                        AttachmentRepository.Delete(AttachmentDelete);

                        // Suppression du fichier local
                        File.Delete(System.IO.Path.Combine(Core.Global.GetConfig().Folders.RootAttachment, Attachment.Att_File));

                        if (Attachment.Pre_Id != null && Attachment.Pre_Id > 0)
                        {
                            // Suppression de l'occurence du document sur prestashop 
                            Model.Prestashop.PsAttachmentRepository psAttachmentRepository = new Model.Prestashop.PsAttachmentRepository();
                            Model.Prestashop.PsAttachmentLangRepository psAttachmentLangRepository = new Model.Prestashop.PsAttachmentLangRepository();
                            Model.Prestashop.PsProductAttachmentRepository psProductAttachmentRepository = new Model.Prestashop.PsProductAttachmentRepository();

                            Model.Prestashop.PsAttachment psAttachment = psAttachmentRepository.ReadAttachment(Convert.ToUInt32(Attachment.Pre_Id.Value));

                            string distant_file = "";
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

                                System.Net.FtpWebRequest request = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(ftpfullpath);
                                request.Credentials = new System.Net.NetworkCredential(User, Password);
                                request.Method = WebRequestMethods.Ftp.DeleteFile;
                                request.UseBinary = true;
                                request.UsePassive = true;
                                request.KeepAlive = false;

                                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                                response.Close();
                            }
                        }

                        this.LoadArticleAttachement();
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void LoadArticleAttachement()
        {
            this.DataGridAttachment.Items.Refresh();
            Model.Local.AttachmentRepository AttachmentRepository = new Model.Local.AttachmentRepository();

            this.DataGridAttachment.ItemsSource = AttachmentRepository.ListArticle(DataContext.Item.Id);
            this.DataGridAttachment.Items.Refresh();
        }

        private string GetRandomFileName()
        {
            String File = Core.Global.GetRandomHexNumber(40).ToLower();
            if (System.IO.File.Exists(Core.Global.GetConfig().Folders.RootAttachment + File)
                || new Model.Prestashop.PsAttachmentRepository().ExistFile(File))
            {
                return this.GetRandomFileName();
            }
            else
            {
                return File;
            }
        }
        #endregion

        #region Gamme

        private void ButtonGammeColor1_Click(object sender, RoutedEventArgs e)
        {
            //ColorDialog colorDialog = new ColorDialog();
            //colorDialog.Owner = this;
            //if ((bool)colorDialog.ShowDialog())
            //{
            //    this.RectangleGammeColor1.Fill = new SolidColorBrush(colorDialog.SelectedColor);
            //    if (IsLoadedAttribute)
            //        DataContext.Item.HasUpdatedAttribute = true;
            //}
        }

        private void ButtonGammeColor2_Click(object sender, RoutedEventArgs e)
        {
            //ColorDialog colorDialog = new ColorDialog();
            //colorDialog.Owner = this;
            //if ((bool)colorDialog.ShowDialog())
            //{
            //    this.RectangleGammeColor2.Fill = new SolidColorBrush(colorDialog.SelectedColor);
            //    if (IsLoadedAttribute)
            //        DataContext.Item.HasUpdatedAttribute = true;
            //}
        }

        #endregion

        private void ButtonImportConditioningArticle_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Êtes-vous sûr de vouloir importer les conditionnements depuis Sage ?", "Import conditionnements", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                DataContext.ImportConditioningArticle();
                if (new Model.Local.ConditioningGroupRepository().ExistSage((int)DataContext.Item.Sage.AR_Condition))
                    this.LabelConditioningGroup.Content = new Model.Sage.P_CONDITIONNEMENTRepository().ReadConditionnement((int)DataContext.Item.Sage.AR_Condition).P_Conditionnement;
            }
        }

        private void ButtonImportAttributeArticle_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Êtes-vous sûr de vouloir importer les gammes depuis Sage ?", "Import gammes", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                DataContext.ImportAttributeArticle();
                if (new Model.Local.AttributeGroupRepository().ExistSage((int)DataContext.Item.Sage.AR_Gamme1))
                    this.LabelAttributeGroup1.Content = new Model.Sage.P_GAMMERepository().ReadGamme((int)DataContext.Item.Sage.AR_Gamme1).G_Intitule;

                if (DataContext.Item.Sage.AR_Gamme2 != null && DataContext.Item.Sage.AR_Gamme2 != 0)
                {
                    if (new Model.Local.AttributeGroupRepository().ExistSage((int)DataContext.Item.Sage.AR_Gamme2))
                        this.LabelAttributeGroup2.Content = new Model.Sage.P_GAMMERepository().ReadGamme((int)DataContext.Item.Sage.AR_Gamme2).G_Intitule;
                }
            }
        }

        private void DataGridFeatureButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            DataContext.EraseFeatureProduct();
        }

        private void ButtonFilterPsFeatureValueLang_Click(object sender, RoutedEventArgs e)
        {
            DataContext.FilterFeatureValue();
        }

        private void DataGridFeature_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext.SelectedFeature != null && DataContext.SelectedFeature.PsFeatureValueLang != null)
            {
                ListBoxFeatureValue.ScrollIntoView(DataContext.SelectedFeature.PsFeatureValueLang);
                //DataGridFeatureValue.ScrollIntoView(DataContext.SelectedFeature.PsFeatureValueLang);
            }
        }

        private void ButtonReloadDescription_Click(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
            {
                this.TinyMceResume.HtmlContent = DataContext.Item.DescriptionShort;
                this.TinyMceDescription.HtmlContent = DataContext.Item.Description;
            }
        }

        private void buttonAjoutImage_Click(object sender, RoutedEventArgs e)
        {
            WriteImage(DataContext.Item.Local);
            LoadArticleImage();
        }

        private void buttonAddCompositionAttributeGroup_Click(object sender, RoutedEventArgs e)
        {
            DataContext.AddCompositionArticleAttributeGroup();
        }

        private void buttonRemoveCompositionAttributeGroup_Click(object sender, RoutedEventArgs e)
        {
            DataContext.RemoveCompositionArticleAttributeGroup();
        }

        private void listBoxPsAttributeGroup_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DataContext.AddCompositionArticleAttributeGroup();
        }

        private void listBoxCompositionAttributeGroup_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DataContext.RemoveCompositionArticleAttributeGroup();
        }

        private void ButtonSearchCompositionArticle_Click(object sender, RoutedEventArgs e)
        {
            var default_cursors = this.Cursor;
            this.Cursor = System.Windows.Input.Cursors.Wait;
            DataContext.SearchArticleComposition();
            this.Cursor = default_cursors;
        }

        private void ToAddResultCompositionArticle_Click(object sender, RoutedEventArgs e)
        {
            DataContext.CheckAddAllArticleComposition();
        }

        private void CheckBoxAttachToAttributeArticle_Checked(object sender, RoutedEventArgs e)
        {
            DataContext.AttachImageToAttributeArticle();
        }

        private void ButtonAddResultComposition_Click(object sender, RoutedEventArgs e)
        {
            var default_cursors = this.Cursor;
            this.Cursor = System.Windows.Input.Cursors.Wait;
            DataContext.AddCompositionArticle();
            this.Cursor = default_cursors;
        }

        private void DeleteCompositionArticle_Click(object sender, RoutedEventArgs e)
        {
            DataContext.DeleteCompositionArticle();
        }

        private void DeleteAllCompositionArticle_Click(object sender, RoutedEventArgs e)
        {
            DataContext.DeleteAllCompositionArticle();
        }

        private void buttonCreatePsAttributeGroup_Click(object sender, RoutedEventArgs e)
        {
            DataContext.AddPsAttributeGroup();
        }

        private void ButtonFilterPsAttributeLang_Click(object sender, RoutedEventArgs e)
        {
            DataContext.FilterAttributeValue();
        }

        private void ButtonAddNewAttribute_Click(object sender, RoutedEventArgs e)
        {
            var default_cursors = this.Cursor;
            this.Cursor = System.Windows.Input.Cursors.Wait;
            DataContext.AddPsAttribute();
            this.Cursor = default_cursors;
        }

        private void dataGridCompositionArticleAttribute_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext.SelectedCompositionArticle != null
                && DataContext.SelectedCompositionArticle.SelectedPsAttributeGroupLang != null
                && DataContext.SelectedCompositionArticle.SelectedPsAttributeGroupLang.PsAttributeLang != null
                && ListboxSelectedCompositionArticleListPsAttributeLang != null)
            {
                ListboxSelectedCompositionArticleListPsAttributeLang.ScrollIntoView(DataContext.SelectedCompositionArticle.SelectedPsAttributeGroupLang.PsAttributeLang);
            }
        }

        private void ListboxSelectedCompositionArticleListPsAttributeLang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext.SelectedCompositionArticle != null)
            {
                if (DataContext.SelectedCompositionArticle.SelectedPsAttributeGroupLang != null)
                {
                    if (DataContext.SelectedCompositionArticle.SelectedPsAttributeGroupLang.PsAttributeLang.Name != null)
                    {
                        if (DataContext.ListAttachPsAttributeGroupLang.Count == 1
                            && DataContext.ListCompositionArticle.Where(ca => ca.ComArt_Id != DataContext.SelectedCompositionArticle.ComArt_Id)
                                .Count(ca => ca.ListPsAttributeGroupLang.First(ag => ag.IDAttributeGroup == DataContext.SelectedCompositionArticle.SelectedPsAttributeGroupLang.IDAttributeGroup)
                                    .PsAttributeLang.IDAttribute == DataContext.SelectedCompositionArticle.SelectedPsAttributeGroupLang.PsAttributeLang.IDAttribute) > 0)
                        {
                            MessageBox.Show("Une composition utilise déjà l'attribut : " + DataContext.SelectedCompositionArticle.SelectedPsAttributeGroupLang.PsAttributeLang.Name, "", MessageBoxButton.OK, MessageBoxImage.Error);
                            DataContext.SelectedCompositionArticle.SelectedPsAttributeGroupLang.PsAttributeLang = new Model.Prestashop.PsAttributeLang();
                        }
                        else if (DataContext.ListAttachPsAttributeGroupLang.Count > 1
                            && DataContext.ListCompositionArticle.Count(ca => ca.IsSelected) > 1)
                        {
                            if (MessageBox.Show("Appliquer la valeur : " + DataContext.SelectedCompositionArticle.SelectedPsAttributeGroupLang.PsAttributeLang.Name + "\naux " + DataContext.ListCompositionArticle.Count(ca => ca.IsSelected && ca.ComArt_Id != DataContext.SelectedCompositionArticle.ComArt_Id) + " autres compositions sélectionnées ?",
                                "Saisie multiple", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                foreach (Model.Local.CompositionArticle compo in DataContext.ListCompositionArticle.Where(ca => ca.IsSelected))
                                {
                                    compo.ListPsAttributeGroupLang.First(ag => ag.IDAttributeGroup ==
                                        DataContext.SelectedCompositionArticle.SelectedPsAttributeGroupLang.IDAttributeGroup).PsAttributeLang
                                        = compo.ListPsAttributeGroupLang.First(ag => ag.IDAttributeGroup ==
                                        DataContext.SelectedCompositionArticle.SelectedPsAttributeGroupLang.IDAttributeGroup).ListPsAttributeLang.First(a => a.IDAttribute == DataContext.SelectedCompositionArticle.SelectedPsAttributeGroupLang.PsAttributeLang.IDAttribute);
                                    compo.ReloadStringDeclinaison();
                                }
                            }
                        }
                    }
                }
                DataContext.SelectedCompositionArticle.ReloadStringDeclinaison();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ApplyChanges();
            if (DataContext.Item.HasUpdatedSync && !DataContext.Item.HasUpdated)
                DataContext.Update();
            if (DataContext.Item.Updated && MessageBox.Show("En fermant la fenêtre vos modifications ne seront pas enregistrées, continuer ?", "PrestaConnect", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                e.Cancel = true;
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext.DeleteArticle())
                this.Close();
        }

        private void buttonInitSearch_Click(object sender, RoutedEventArgs e)
        {
            DataContext.InitSearchArticleComposition();
        }

        private void buttonTransform_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext.TransformArticle())
                this.InitContext();
        }

        private void CreateAttributeByGamme1_Click(object sender, RoutedEventArgs e)
        {
            var default_cursors = this.Cursor;
            this.Cursor = System.Windows.Input.Cursors.Wait;
            DataContext.CreateAttributeByGamme1();
            this.Cursor = default_cursors;
        }

        private void CreateAttributeByGamme2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CreateAttributeByConditioning_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBoxAttachToCompositionArticle_Checked(object sender, RoutedEventArgs e)
        {
            DataContext.AttachImageToCompositionArticle();
        }

        private void FastSelectionComposition_Click(object sender, RoutedEventArgs e)
        {
            DataContext.AutoSelectComposition();
        }

        private void DeleteAllAttributeArticle_Click(object sender, RoutedEventArgs e)
        {
            DataContext.DeleteAllAttributeArticle();
        }

        private void DeleteAttributeArticle_Click(object sender, RoutedEventArgs e)
        {
            DataContext.DeleteAttributeArticle();
        }

        private void DeleteAllConditioningArticle_Click(object sender, RoutedEventArgs e)
        {
            DataContext.DeleteAllConditioningArticle();
        }

        private void DeleteConditioningArticle_Click(object sender, RoutedEventArgs e)
        {
            DataContext.DeleteConditioningArticle();
        }

        private void dataGridCompositionArticleImage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext.SelectedCompositionArticle != null && DataContext.SelectedCompositionArticle.CompositionArticleImage != null)
            {
                ListBoxArticleImageComposition.ScrollIntoView(DataContext.ListArticleImageComposition.FirstOrDefault(ci => ci.AttachedToCompositionArticle));
            }
        }

        #region WYSIWYG
        private void TextBoxInsertHTMLResume_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.TextBoxInsertHTMLResume.Text))
            {
                this.ViewWebBrowserResume.NavigateToString(this.TextBoxInsertHTMLResume.Text);
            }
            else
            {
                this.ViewWebBrowserResume.NavigateToString("<html></html>");
            }
        }

        private void buttonLoadHTMLResume_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext.Item != null)
            {
                this.TextBoxInsertHTMLResume.Text = this.TinyMceResume.HtmlContent;
            }
        }

        private void buttonInsertHTMLResume_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext.Item != null)
                if (this.TextBoxInsertHTMLResume.Text != string.Empty)
                {
                    this.TinyMceResume.HtmlContent = this.TextBoxInsertHTMLResume.Text;
                }
        }

        private void buttonLoadDbHTMLResume_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext.Item != null)
            {
                try
                {
                    this.TextBoxInsertHTMLResume.Text = DataContext.Item.DescriptionShort;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erreur chargement", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void buttonInsertDbHTMLResume_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext.Item != null)
            {
                try
                {

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erreur sauvegarde", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void TextBoxInsertHTMLDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.TextBoxInsertHTMLDescription.Text))
            {
                this.ViewWebBrowserDescription.NavigateToString(this.TextBoxInsertHTMLDescription.Text);
            }
            else
            {
                this.ViewWebBrowserDescription.NavigateToString("<html></html>");
            }
        }

        private void buttonLoadHTMLDescription_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext.Item != null)
            {
                this.TextBoxInsertHTMLDescription.Text = this.TinyMceDescription.HtmlContent;
            }
        }

        private void buttonInsertHTMLDescription_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext.Item != null)
                if (this.TextBoxInsertHTMLDescription.Text != string.Empty)
                {
                    this.TinyMceDescription.HtmlContent = this.TextBoxInsertHTMLDescription.Text;
                }
        }

        private void buttonLoadDbHTMLDescription_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext.Item != null)
            {
                try
                {
                    this.TextBoxInsertHTMLDescription.Text = DataContext.Item.Description;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erreur chargement", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void buttonInsertDbHTMLDescription_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext.Item != null)
            {
                try
                {

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erreur sauvegarde", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
		#endregion

		private void ButtonSubmitAdditionnalField_Click(object sender, RoutedEventArgs e)
		{
			Model.Local.ArticleAdditionalFieldRepository ArticleAdditionalFieldRepository = new Model.Local.ArticleAdditionalFieldRepository();
			if (DataContext.SelectedResultAdditionnalFieldArticle != null)
			{
				if (ArticleAdditionalFieldRepository.ExistFieldName(DataContext.Item.Local.Art_Id, DataContext.SelectedResultAdditionnalFieldArticle.FieldName))
				{
					ArticleAdditionalFieldRepository.Save(DataContext.SelectedResultAdditionnalFieldArticle);
				}
				else
				{
					ArticleAdditionalFieldRepository.Add(DataContext.SelectedResultAdditionnalFieldArticle);
				}
				try
				{
					DataContext.UpdateDate();
					DataContext.LoadAdditionnalFieldArticle();
					DataContext.SelectedResultAdditionnalFieldArticle = null;
				}
				catch (Exception ex)
				{
					Core.Log.WriteLog(ex.Message);
				}
			}
		}
	}
}