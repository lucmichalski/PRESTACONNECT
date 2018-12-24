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

namespace PRESTACONNECT.View
{
    /// <summary>
    /// Logique d'interaction pour DeplacementCatalogue.xaml
    /// </summary>
    public partial class DeplacementCatalogue : Window
    {
        public DeplacementCatalogue(Model.Local.Catalog Catalog)
        {
            InitializeComponent();
            DataContext = new DeplacementCatalogueContext();
            DataContext.TargetCatalog = Catalog;
        }

        internal new DeplacementCatalogueContext DataContext
        {
            get { return (DeplacementCatalogueContext)base.DataContext; }
            private set
            {
                base.DataContext = value;
            }
        }

        private void Catalogs_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!DataContext.IsBusy)
            {
                DataContext.SelectedCatalog = ((TreeView)sender).SelectedItem as Model.Local.Catalog;
            }
        }

        private void ButtonMoveCatalog_Click(object sender, RoutedEventArgs e)
        {
            bool canmove = false;
            if (DataContext.TargetCatalog == null)
            {
                MessageBox.Show("Vous devez sélectionner un catalogue à déplacer !", "Déplacement catalogue", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (DataContext.SelectedCatalog != null)
            {
                if (DataContext.TargetCatalog.Cat_Parent == DataContext.SelectedCatalog.Cat_Id)
                {
                    MessageBox.Show("Le catalogue est déjà rattaché à ce parent !", "Déplacement catalogue", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (DataContext.TargetCatalog.Cat_Id == DataContext.SelectedCatalog.Cat_Id)
                {
                    MessageBox.Show("Vous devez sélectionner un catalogue parent différent de celui à déplacer !", "Déplacement catalogue", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (DataContext.TargetCatalog.Cat_Id == DataContext.SelectedCatalog.Cat_Parent)
                {
                    MessageBox.Show("Vous ne pouvez pas déplacer un catalogue dans son propre enfant !", "Déplacement catalogue", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (DataContext.TargetCatalog.ChildrenContainsCatalog(DataContext.SelectedCatalog.Cat_Id))
                {
                    MessageBox.Show("Vous ne pouvez pas déplacer un catalogue dans un de ses sous-enfants !", "Déplacement catalogue", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    canmove = true;
                }
            }
            else if (MessageBox.Show("Vous n'avez pas sélectionné de catalogue cible, déplacer vers la racine ?", "Déplacement catalogue", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                canmove = true;
            }

            if (canmove)
            {
                DataContext.MoveCatalog();
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
