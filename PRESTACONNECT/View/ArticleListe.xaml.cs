using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using PRESTACONNECT.Core;

namespace PRESTACONNECT
{
    //<YH> 22/08/2012
    public partial class ArticleListe : Window
    {
        private List<Model.Local.Catalog> ListCatalog;
        private List<Model.Local.Article> ListArticle;
        private List<Int32> ArrayCatalog;
        private List<Model.Local.Catalog> ListCatalogDisplay;


        public ArticleListe()
        {
            this.InitializeComponent();

            // Insérez le code requis pour la création d’objet sous ce point.
        }

        private void ButtonNewArticle_Click(object sender, RoutedEventArgs e)
        {
            PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
            Loading.Show();
            PRESTACONNECT.Article Article = new Article();
            Article.Show();
            Loading.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.ArrayCatalog = new List<int>();
            this.ListCatalogDisplay = new List<Model.Local.Catalog>();
            Model.Local.CatalogRepository CatalogRepository = new Model.Local.CatalogRepository();
            CheckBoxSearchOnlyCatalog.IsChecked = Global.GetConfig().ArticleUniquementEnStock;
            this.ListCatalog = CatalogRepository.ListOrderByLevel();
            Boolean AddCatalog = false;
            String Add = "";
            foreach (Model.Local.Catalog Catalog in this.ListCatalog)
            {
                AddCatalog = false;
                Add = "";
                if (this.ArrayCatalog.Contains(Catalog.Cat_Id) == false)
                {
                    if (Catalog.Cat_Parent != 0)
                    {
                        if (this.ArrayCatalog.Contains(Catalog.Cat_Parent) == false)
                        {
                            AddCatalog = true;
                        }
                    }
                    else
                    {
                        AddCatalog = true;
                    }
                }
                if (AddCatalog == true)
                {
                    Catalog.Cat_IdWithParent = Catalog.Cat_Id.ToString();
                    this.ArrayCatalog.Add(Catalog.Cat_Id);
                    this.ListCatalogDisplay.Add(Catalog);
                    Add += Catalog.Cat_Id;
                    List<Model.Local.Catalog> ListParent = CatalogRepository.ListParent(Catalog.Cat_Id);
                    if (ListParent.Count > 0)
                    {
                        this.LoadCatalogParent(ListParent, Add);
                    }
                }
            }
            this.DataGridCatalogue.ItemsSource = this.ListCatalogDisplay;
        }

        private void LoadCatalogParent(List<Model.Local.Catalog> ListParent, String Add)
        {
            Model.Local.CatalogRepository CatalogRepository = new Model.Local.CatalogRepository();
            foreach (Model.Local.Catalog Catalog in ListParent)
            {
                if (this.ArrayCatalog.Contains(Catalog.Cat_Id) == false)
                {
                    this.ArrayCatalog.Add(Catalog.Cat_Id);
                    Catalog.Cat_IdWithParent = Add + " - " + Catalog.Cat_Id;
                    this.ListCatalogDisplay.Add(Catalog);
                    List<Model.Local.Catalog> ListNewParent = CatalogRepository.ListParent(Catalog.Cat_Id);
                    if (ListParent.Count > 0)
                    {
                        this.LoadCatalogParent(ListNewParent, Add + " - " + Catalog.Cat_Id);
                    }

                }
            }
        }

        private void DataGridButtonArticleUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataGridArticle.SelectedItem != null)
            {
                Model.Local.Article ArticleSend = this.DataGridArticle.SelectedItem as Model.Local.Article;
                PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
                Loading.Show();
                PRESTACONNECT.Article Article = new Article(ArticleSend);
                Article.Show();
                Loading.Close();
            }
        }

        #region Search
        private void ButtonSearchArticle_Click(object sender, RoutedEventArgs e)
        {
            this.SearchArticle();
        }

        private void SearchArticle()
        {
            Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
            List<Model.Local.Article> ListArticleDisplay = new List<Model.Local.Article>();
            Model.Local.Catalog Catalog = this.DataGridCatalogue.SelectedItem as Model.Local.Catalog;
            if (Catalog != null && this.CheckBoxSearchOnlyCatalog.IsChecked == true)
            {
                this.ListArticle = ArticleRepository.ListCatalog(Catalog.Cat_Id);
                Model.Local.ArticleCatalogRepository ArticleCatalogRepository = new Model.Local.ArticleCatalogRepository();
                List<Model.Local.ArticleCatalog> ListArticleCatalog = ArticleCatalogRepository.ListCatalog(Catalog.Cat_Id);
                foreach (Model.Local.ArticleCatalog ArticleCatalog in ListArticleCatalog)
                {
                    Boolean Add = true;
                    foreach (Model.Local.Article Article in ListArticle)
                    {
                        if (Article.Art_Id == ArticleCatalog.Art_Id)
                        {
                            Add = false;
                            break;
                        }
                    }
                    if (Add == true)
                    {
                        this.ListArticle.Add(ArticleCatalog.Article);
                    }
                }
            }
            else
            {
                this.ListArticle = ArticleRepository.List();
            }

            Boolean isArticle;
            foreach (Model.Local.Article Article in this.ListArticle)
            {
                isArticle = true;
                if (this.TextBoxSearchArticleName.Text != "" && this.TextBoxSearchArticleName.Text.Replace("%", "") != "")
                {
                    if (this.TextBoxSearchArticleName.Text.Contains("%"))
                    {
                        if (Article.Art_Name.ToUpper().Contains(this.TextBoxSearchArticleName.Text.Replace("%", "").ToUpper()) == false)
                        {
                            isArticle = false;
                        }
                    }
                    else
                    {
                        if (Article.Art_Name.ToUpper().StartsWith(this.TextBoxSearchArticleName.Text.ToUpper()) == false)
                        {
                            isArticle = false;
                        }
                    }
                }
                if (this.TextBoxSearchArticleEan13.Text != "" && this.TextBoxSearchArticleEan13.Text.Replace("%", "") != "")
                {
                    if (Article.Art_Ean13 != null)
                    {
                        if (this.TextBoxSearchArticleEan13.Text.Contains("%"))
                        {
                            if (Article.Art_Ean13.ToUpper().Contains(this.TextBoxSearchArticleEan13.Text.Replace("%", "").ToUpper()) == false)
                            {
                                isArticle = false;
                            }
                        }
                        else
                        {
                            if (Article.Art_Ean13.ToUpper().StartsWith(this.TextBoxSearchArticleEan13.Text.ToUpper()) == false)
                            {
                                isArticle = false;
                            }
                        }
                    }
                    else
                    {
                        isArticle = false;
                    }
                }
                if (this.TextBoxSearchArticleRef.Text != "" && this.TextBoxSearchArticleRef.Text.Replace("%", "") != "")
                {
                    if (this.TextBoxSearchArticleRef.Text.Contains("%"))
                    {
                        if (Article.Art_Ref.ToUpper().Contains(this.TextBoxSearchArticleRef.Text.Replace("%", "").ToUpper()) == false)
                        {
                            isArticle = false;
                        }
                    }
                    else
                    {
                        if (Article.Art_Ref.ToUpper().StartsWith(this.TextBoxSearchArticleRef.Text.ToUpper()) == false)
                        {
                            isArticle = false;
                        }
                    }
                }

                if (Article.Art_Sync != this.CheckBoxSearchArticleSync.IsChecked)
                {
                    isArticle = false;
                }
                if (Article.Art_Active != this.CheckBoxSearchArticleActif.IsChecked)
                {
                    isArticle = false;
                }
                if (isArticle == true)
                {
                    ListArticleDisplay.Add(Article);
                }
            }
            this.DataGridArticle.ItemsSource = ListArticleDisplay;
        }

        private void TextBoxSearchCatalog_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<Model.Local.Catalog> ListCatalogDisplaySearch = new List<Model.Local.Catalog>();
            if (this.TextBoxSearchCatalog.Text != "")
            {
                if (this.TextBoxSearchCatalog.Text.Replace("%", "") != "")
                {
                    foreach (Model.Local.Catalog Catalog in this.ListCatalogDisplay)
                    {
                        if (this.TextBoxSearchCatalog.Text.StartsWith("%"))
                        {
                            if (Catalog.Cat_Name.ToUpper().Contains(this.TextBoxSearchCatalog.Text.Replace("%", "").ToUpper()))
                            {
                                ListCatalogDisplaySearch.Add(Catalog);
                            }
                        }
                        else if (this.TextBoxSearchCatalog.Text.EndsWith("%"))
                        {
                            if (Catalog.Cat_Name.ToUpper().EndsWith(this.TextBoxSearchCatalog.Text.Replace("%", "").ToUpper()))
                            {
                                ListCatalogDisplaySearch.Add(Catalog);
                            }
                        }
                        else
                        {
                            if (Catalog.Cat_Name.ToUpper().StartsWith(this.TextBoxSearchCatalog.Text.ToUpper()))
                            {
                                ListCatalogDisplaySearch.Add(Catalog);
                            }
                        }
                    }
                    this.DataGridCatalogue.ItemsSource = ListCatalogDisplaySearch;
                }
                else
                {
                    this.DataGridCatalogue.ItemsSource = this.ListCatalogDisplay;
                }
            }
            else
            {
                this.DataGridCatalogue.ItemsSource = this.ListCatalogDisplay;
            }
        }
        #endregion

        private void DataGridCatalogue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Model.Local.Catalog Catalog = this.DataGridCatalogue.SelectedItem as Model.Local.Catalog;
            if (Catalog != null && this.CheckBoxSearchOnlyCatalog.IsChecked == true)
            {
                this.SearchArticle();
            }
        }

        private void DataGridArticle_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.DataGridArticle.SelectedItem != null)
            {
                Model.Local.Article ArticleSend = this.DataGridArticle.SelectedItem as Model.Local.Article;
                PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
                Loading.Show();
                PRESTACONNECT.Article Article = new Article(ArticleSend);
                Article.Show();
                Loading.Close();
            }
        }

        private void CheckBoxSearchOnlyCatalog_Checked(object sender, RoutedEventArgs e)
        {
            Global.GetConfig().UpdateArticleUniquementEnStock(true);
        }
        private void CheckBoxSearchOnlyCatalog_Unchecked(object sender, RoutedEventArgs e)
        {
            Global.GetConfig().UpdateArticleUniquementEnStock(false);
        }
    }
}