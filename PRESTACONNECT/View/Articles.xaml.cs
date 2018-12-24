using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PRESTACONNECT.Contexts;
using PRESTACONNECT.Model.Internal;

namespace PRESTACONNECT
{
    public partial class Articles : Window
    {
        //Point p = new Point();

        #region Properties

        internal new ArticlesContext DataContext
        {
            get { return (ArticlesContext)base.DataContext; }
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

        #endregion
        #region Constructors

        public Articles()
        {
            DataContext = new ArticlesContext();
            InitializeComponent();

            if (Core.Temp.Current != System.Windows.WindowState.Minimized)
                this.WindowState = Core.Temp.Current;
        }

        #endregion
        #region Event methods

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext.Load();
            DataContext.LoadCatalogs();
            SearchItem.Focus();
        }

        private void Catalogs_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DataContext.SelectedCatalog = ((TreeView)sender).SelectedItem as Model.Local.Catalog;
            DataContext.LoadSelectedCatalogItems();
        }

        private void ToActiveSelectedCatalogItems_Click(object sender, RoutedEventArgs e)
        {
            DataContext.AllToActive();
        }

        // <JG> 21/03/2013 ajout gestion activation/désactivation synchronisation
        private void ToSyncSelectedCatalogItems_Click(object sender, RoutedEventArgs e)
        {
            DataContext.AllToSync();
        }
        private void ToSyncPriceSelectedCatalogItems_Click(object sender, RoutedEventArgs e)
        {
            DataContext.AllToSyncPrice();
        }

        private void Items_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ShowItem(DataContext.SelectedItem.Art_Id);
            //p = e.GetPosition(DataGridArticles);
        }

        private void DataGridArticles_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                ShowItem(DataContext.SelectedItem.Art_Id);
        }

        private void SearchItems_Click(object sender, RoutedEventArgs e)
        {
            DataContext.LoadSelectedCatalogItems();
            SearchItem.Focus();
            SearchItem.SelectAll();
        }
        private void ButtonNewArticle_Click(object sender, RoutedEventArgs e)
        {
            //ShowItem(null);
        }

        #endregion
        #region Methods

        private void ShowItem(int article)
        {
            PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
            Loading.Show();
            PRESTACONNECT.Article Article = new Article(new Model.Local.ArticleRepository().ReadArticle(article));
            Loading.Close();
            Article.ShowDialog();
            #region Refresh ligne article
            // rafraichissement des données affichées pour l'article par rapport à la base de données
            // sans perte du focus nécessaire à la continuité de saisie au clavier (accessibilité)
            Model.Local.Article temp = new Model.Local.ArticleRepository().ReadArticle(article);
            if (temp != null)
            {
                Model.Local.Article select = DataContext.SelectedCatalogItems.FirstOrDefault(a => a.Art_Id == article);
                select.Art_Name = temp.Art_Name;
                select.Art_Ean13 = temp.Art_Ean13;
                select.Art_Sync = temp.Art_Sync;
                select.Art_SyncPrice = temp.Art_SyncPrice;
                select.Art_Active = temp.Art_Active;
                select.Art_Date = temp.Art_Date;
                select.Art_Type = temp.Art_Type;
                select.Pre_Id = temp.Pre_Id;
                if (temp.prestashopProduct != null)
                {
                    select.RefreshPrestashopProductData();
                }
                if (temp.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleComposition)
                {
                    select.TypeArticle = Model.Local.Article.enum_TypeArticle.defaut;
                    if (temp.CompositionArticle != null)
                    {
                        List<int> list_composition = temp.CompositionArticle.Select(ca => ca.ComArt_F_ARTICLE_SagId).ToList();
                        foreach (Model.Local.Article composition in DataContext.SelectedCatalogItems.Where(a => list_composition.Contains(a.Sag_Id)))
                        {
                            Model.Local.Article temp_compo = new Model.Local.ArticleRepository().ReadArticle(composition.Art_Id);
                            composition.Art_Sync = temp_compo.Art_Sync;
                            composition.Art_SyncPrice = temp_compo.Art_SyncPrice;
                            composition.Art_Active = temp_compo.Art_Active;
                            composition.Art_Date = temp_compo.Art_Date;
                        }
                    }
                }
            }
            else
            {
                DataContext.LoadSelectedCatalogItems();
            }
            #endregion
        }

        #endregion

        #region Sync

        private void ButtonSync_Click(object sender, RoutedEventArgs e)
        {
            DataContext.SyncArticle();
        }
        private void ButtonTransfertStockPrice_Click(object sender, RoutedEventArgs e)
        {
            DataContext.TransfertStockPrice();
        }

        // <JG> 29/10/2012 ajout synchronisation des stocks seuls
        private void ButtonTransfertStock_Click(object sender, RoutedEventArgs e)
        {
            DataContext.TransfertStock();
        }

        private void ButtonTransfertImages_Click(object sender, RoutedEventArgs e)
        {
            DataContext.TransfertImages();
        }

        private void ButtonSyncArticleCatalogue_Click(object sender, RoutedEventArgs e)
        {
            DataContext.SyncArticleCatalogue();
        }

        #endregion

        #region CheckState

        private void CheckArticleSync_Click(object sender, RoutedEventArgs e)
        {
            DataContext.SelectedItemToSync();
        }

        private void CheckArticleActive_Click(object sender, RoutedEventArgs e)
        {
            DataContext.SelectedItemToActive();
        }

        private void CheckArticleSyncPrice_Click(object sender, RoutedEventArgs e)
        {
            DataContext.SelectedItemToSyncPrice();
        }

        #endregion

        private void ButtonImportSageInformationLibre_Click(object sender, RoutedEventArgs e)
        {
            DataContext.ImportSageInformationLibreValeurs();
        }

        private void ButtonImportCatalogueInformationLibre_Click(object sender, RoutedEventArgs e)
        {
            DataContext.ImportCatalogueInfoLibre();
        }

        private void ButtonImportConditioningArticle_Click(object sender, RoutedEventArgs e)
        {
            DataContext.ImportConditioningArticle();
        }

        private void ButtonReimportSage_Click(object sender, RoutedEventArgs e)
        {
            DataContext.ReimportSage();
        }

        private void ButtonCancelDate_Click(object sender, RoutedEventArgs e)
        {
            DataContext.FiltreDateStart = null;
            DataContext.FiltreDateEnd = null;
        }

        private void SearchItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SearchItems_Click(sender, e);
        }

        private void ButtonSyncAttribute_Click(object sender, RoutedEventArgs e)
        {
            DataContext.TransfertAttribute();
        }

        private void ButtonAddArticleComposition_Click(object sender, RoutedEventArgs e)
        {
            DataContext.CreateArticleComposition();
        }

        private void ButtonTransfertPack_Click(object sender, RoutedEventArgs e)
        {
            DataContext.TransfertPack();
        }

        private void Expander_MouseEnter(object sender, MouseEventArgs e)
        {
            //if (!ExpanderAdvancedFilters.IsExpanded)
            //{
            //    ExpanderAdvancedFilters.IsExpanded = true;
            //}
        }

        private void ExpanderAdvancedFilters_MouseLeave(object sender, MouseEventArgs e)
        {
            //if (ExpanderAdvancedFilters.IsExpanded)
            //{
            //    ExpanderAdvancedFilters.IsExpanded = false;
            //}
        }

        private void ButtonTransfertFeature_Click(object sender, RoutedEventArgs e)
        {
            DataContext.TransfertFeature();
        }

        private void ButtonGestionStatutArticle_Click(object sender, RoutedEventArgs e)
        {
            DataContext.GestionStatutArticle();
        }
    }
}
