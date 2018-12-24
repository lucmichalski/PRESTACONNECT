using System.Windows;
using System.Windows.Controls;
using PRESTACONNECT.Contexts;
using PRESTACONNECT.Model.Internal;

namespace PRESTACONNECT
{
    public partial class Catalogue : Window
    {
        #region Properties

        internal new CatalogueContext DataContext
        {
            get { return (CatalogueContext)base.DataContext; }
            private set
            {
                base.DataContext = value;
            }
        }

        #endregion
        #region Constructors

        public Catalogue()
        {
            InitializeComponent();
            DataContext = new CatalogueContext();

            if (Core.Temp.Current != System.Windows.WindowState.Minimized)
                this.WindowState = Core.Temp.Current;
        }

        #endregion
        #region Event methods

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext.Load();
        }

        private void Catalogs_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!DataContext.IsBusy)
            {
                DataContext.SelectedCatalog = ((TreeView)sender).SelectedItem as Model.Local.Catalog;

                if (DataContext.SelectedCatalog != null)
                {
                    // <JG> 04/02/2013 Lecture de la possibilité de supprimer ou non le catalogue local
                    DataContext.SelectedCatalog.CanDelete = DataContext.SelectedCatalog.CanRemoveLocal;

                    if (DataContext.SelectedCatalog.IsRemovedFromPrestashop())
                    {
                        MessageBoxResult Result = MessageBox.Show("Le catalogue n'existe plus dans Prestashop.\nVoulez-vous le recréer ?",
                            "Catalogue", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);

                        if (Result == MessageBoxResult.Yes)
                            DataContext.SelectedCatalog.PrepareToCreateInPrestashop();
                    }
                }
            }
        }

        private void CatalogSync_Checked(object sender, RoutedEventArgs e)
        {
            DataContext.ChangeToSyncCatalogs(((CheckBox)sender).DataContext as Model.Local.Catalog, true);
        }
        private void CatalogSync_Unchecked(object sender, RoutedEventArgs e)
        {
            DataContext.ChangeToSyncCatalogs(((CheckBox)sender).DataContext as Model.Local.Catalog, false);
        }

        private void ButtonCatalogueImage_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext.SelectedCatalog != null)
            {
                Microsoft.Win32.OpenFileDialog OFD = new Microsoft.Win32.OpenFileDialog();
                OFD.Filter = "Images (*.jpg, *.png, *.gif) | *.jpg;*.png;*.gif";
                OFD.Multiselect = false;

                bool? result = OFD.ShowDialog();

                if (result.HasValue && result.Value)
                    DataContext.SelectedCatalog.SmallImagePath = OFD.FileName;
            }
        }
        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            //MessageBoxResult Result = MessageBoxResult.Yes;

            //if (DataContext.HasUpdated())
            //    Result = MessageBox.Show("Confirmez vous l'enregistrement des modifications apportées aux catalogues ?",
            //        "Catalogue", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            //if (Result == MessageBoxResult.Yes)
            if (DataContext.UpdateAll())
            {
                MessageBox.Show("Données catalogue enregistrées !", "PrestaConnect", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            //DataContext.LoadCatalogs();
        }
        private void ButtonSync_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Result = MessageBoxResult.Yes;

            if (DataContext.HasUpdated())
                Result = MessageBox.Show("Confirmez vous l'enregistrement des modifications apportées aux catalogues ?",
                    "Catalogue", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

            if (Result == MessageBoxResult.Yes)
            {
                DataContext.UpdateAll();
                Synchroniser();

                DataContext.LoadCatalogs();
            }
        }
        private void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Result = MessageBoxResult.No;

            if (DataContext.SelectedCatalog != null)
                Result = MessageBox.Show(string.Format("Voulez-vous créer un sous catalogue de '{0}' ?", DataContext.SelectedCatalog.Name),
                    "Catalogue", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.None);

            DataContext.CreateCatalog((Result == MessageBoxResult.No) ? null : DataContext.SelectedCatalog);


        }
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Result = MessageBoxResult.No;

            // <JG> 04/02/2013 évolution du messagebox en cas de suppression Prestashop
            if (DataContext.SelectedCatalog != null && DataContext.SelectedCatalog.Catalog2.Count == 0)
                Result = MessageBox.Show(string.Format("Confirmez vous la suppression du catalogue '{0}' "
                    + ((DataContext.SelectedCatalog.ToDeleteFromPrestashop) ? "dans Prestaconnect ET Prestashop " : "") + "?", DataContext.SelectedCatalog.Name),
                    "Catalogue", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            if (Result == MessageBoxResult.Yes)
                DataContext.DeleteCatalog(DataContext.SelectedCatalog);
        }

        #endregion

        #region Methods

        private void Synchroniser()
        {
            DataContext.Sync();
        }

        private void ButtonMoveCatalogInTree_Click(object sender, RoutedEventArgs e)
        {
            PRESTACONNECT.View.DeplacementCatalogue Form = new View.DeplacementCatalogue(DataContext.SelectedCatalog);
            if (Form.ShowDialog() == true)
                DataContext.LoadCatalogs();
        }

        #endregion
    }
}