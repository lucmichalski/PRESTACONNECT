using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace PRESTACONNECT
{
    /// <summary>
    /// Logique d'interaction pour Fournisseur.xaml
    /// </summary>
    public partial class Fournisseur : Window
    {
        private Boolean isValid = true;

        public Fournisseur()
        {
            this.InitializeComponent();
            this.LoadComboBoxFournisseur();
            // Insérez le code requis pour la création d’objet sous ce point.

            if (Core.Temp.Current != System.Windows.WindowState.Minimized)
                this.WindowState = Core.Temp.Current;
        }

        private void LoadComponent()
        {
            this.TextBoxDescription.Text = string.Empty;
            this.TextBoxMetaDescription.Text = string.Empty;
            this.TextBoxMetaKeyword.Text = string.Empty;
            this.TextBoxName.Text = string.Empty;
            this.TextBoxTitle.Text = string.Empty;
            this.RadioButtonNotSync.IsChecked = true;
            this.RadioButtonSync.IsChecked = false;
            this.CheckBoxActive.IsChecked = false;
        }

        private void LoadComboBoxFournisseur()
        {
            Core.Temp.ListSupplier = new Model.Local.SupplierRepository().List();

            //this.ListBoxFournisseur.Items.Clear();
            Model.Sage.F_COMPTETRepository F_COMPTETRepository = new Model.Sage.F_COMPTETRepository();
            List<Model.Sage.F_COMPTET> List = F_COMPTETRepository.ListTypeSommeil(1, 0);
            List = List.Where(frs => frs.F_ARTFOURNISS != null && frs.F_ARTFOURNISS.Count > 0).ToList();
            this.ListBoxFournisseur.ItemsSource = List;

            //foreach (Model.Sage.F_COMPTET F_COMPTET in List)
            //{
            //    this.ListBoxFournisseur.Items.Add(F_COMPTET.cbMarq + " - " + F_COMPTET.CT_Intitule);
            //}
        }

        #region Button
        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (this.ListBoxFournisseur.SelectedItem != null)
            {
                Model.Local.SupplierRepository SupplierRepository = new Model.Local.SupplierRepository();
                if (SupplierRepository.ExistSag_Id(((Model.Sage.F_COMPTET)this.ListBoxFournisseur.SelectedItem).cbMarq))
                {
                    Model.Local.Supplier Supplier = SupplierRepository.ReadSag_Id(((Model.Sage.F_COMPTET)this.ListBoxFournisseur.SelectedItem).cbMarq);

                    Supplier.Sup_Name = Core.Global.RemovePurge(this.TextBoxName.Text, 128);
                    Supplier.Sup_Description = Core.Global.RemovePurge(this.TextBoxDescription.Text, 10000);
                    Supplier.Sup_MetaTitle = Core.Global.RemovePurge(this.TextBoxTitle.Text, 70);
                    Supplier.Sup_MetaDescription = Core.Global.RemovePurge(this.TextBoxDescription.Text, 160);
                    Supplier.Sup_MetaKeyword = Core.Global.RemovePurgeMeta(this.TextBoxMetaKeyword.Text, 255);
                    Supplier.Sup_Active = this.CheckBoxActive.IsChecked.Value;
                    Supplier.Sup_Sync = this.RadioButtonSync.IsChecked.Value;
                    Supplier.Sup_Date = DateTime.Now;

                    SupplierRepository.Save();
                    MessageBox.Show("Fournisseur mis à jour avec succès", "Fournisseur", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show("Fournisseur non valide !", "Fournisseur", MessageBoxButton.OK);
                }
            }
        }

        private void ButtonSync_Click(object sender, RoutedEventArgs e)
        {
            if (this.isValid == false)
            {
                MessageBoxResult Result = MessageBox.Show("Voulez vous enregistrer votre fournisseur en cours ?", "Fournisseur", MessageBoxButton.YesNo);
                if (Result == MessageBoxResult.Yes)
                {
                    this.ButtonSubmit_Click(sender, e);
                }
            }
            PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
            Loading.Show();
            SynchronisationFournisseur Sync = new SynchronisationFournisseur();
            Sync.ShowDialog();
            Loading.Close();
        }
        #endregion

        private void ListBoxFournisseur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ListBoxFournisseur.SelectedItem != null)
            {
                if (this.ListBoxFournisseur.SelectedItem is Model.Sage.F_COMPTET)
                {
                    this.isValid = false;
                    Model.Local.SupplierRepository SupplierRepository = new Model.Local.SupplierRepository();
                    Model.Local.Supplier Supplier = new Model.Local.Supplier();
                    if (SupplierRepository.ExistSag_Id(((Model.Sage.F_COMPTET)this.ListBoxFournisseur.SelectedItem).cbMarq))
                    {
                        this.groupBoxSupplierDetail.IsEnabled = true;
                        Supplier = SupplierRepository.ReadSag_Id(((Model.Sage.F_COMPTET)this.ListBoxFournisseur.SelectedItem).cbMarq);
                        this.TextBoxName.Text = Supplier.Sup_Name;
                        this.TextBoxDescription.Text = Supplier.Sup_Description;
                        this.TextBoxTitle.Text = Supplier.Sup_MetaTitle;
                        this.TextBoxMetaKeyword.Text = Supplier.Sup_MetaKeyword;
                        this.TextBoxMetaDescription.Text = Supplier.Sup_MetaDescription;
                        this.CheckBoxActive.IsChecked = Supplier.Sup_Active;
                        this.RadioButtonSync.IsChecked = Supplier.Sup_Sync;

                        if (Supplier.Pre_Id != null)
                        {
                            Model.Prestashop.PsSupplierRepository PsSupplierRepository = new Model.Prestashop.PsSupplierRepository();
                            if (PsSupplierRepository.ExistId(Convert.ToInt32(Supplier.Pre_Id)) == false)
                            {
                                MessageBoxResult Result = MessageBox.Show("Le fournisseur Prestashop n'existe plus. Voulez-vous le recréer ?", "Fournisseur", MessageBoxButton.YesNo);
                                if (Result == MessageBoxResult.Yes)
                                {
                                    Supplier.Pre_Id = null;
                                    SupplierRepository.Save();
                                }
                            }
                        }
                    }
                    else
                    {
                        this.LoadComponent();
                        this.groupBoxSupplierDetail.IsEnabled = false;
                    }
                }
                else
                {
                    this.LoadComponent();
                    this.groupBoxSupplierDetail.IsEnabled = false;
                }
            }
            else
            {
                this.LoadComponent();
                this.groupBoxSupplierDetail.IsEnabled = false;
            }
        }

        private void buttonImportSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (this.ListBoxFournisseur.ItemsSource != null)
            {
                List<Model.Sage.F_COMPTET> List = (List<Model.Sage.F_COMPTET>)this.ListBoxFournisseur.ItemsSource;
                foreach (Model.Sage.F_COMPTET item in List.Where(frs => frs.CheckToImport))
                {
                    CreateSupplier(item);
                }
                this.LoadComboBoxFournisseur();
            }
        }

        private void CreateSupplier(Model.Sage.F_COMPTET F_COMPTET)
        {
            if (F_COMPTET != null && !string.IsNullOrWhiteSpace(F_COMPTET.CT_Intitule))
            {
                Model.Local.SupplierRepository SupplierRepository = new Model.Local.SupplierRepository();
                if (!SupplierRepository.ExistSag_Id(F_COMPTET.cbMarq))
                {
                    Model.Local.Supplier Supplier = new Model.Local.Supplier();
                    //Supplier = SupplierRepository.ReadSag_Id(F_COMPTET.cbMarq);
                    Supplier.Sag_Id = F_COMPTET.cbMarq;
                    Supplier.Sup_Name = Core.Global.RemovePurge(F_COMPTET.CT_Intitule, 128);
                    Supplier.Sup_Description = Supplier.Sup_Name;
                    Supplier.Sup_MetaTitle = Core.Global.RemovePurge(Supplier.Sup_Name, 70);
                    Supplier.Sup_MetaDescription = Core.Global.RemovePurge(Supplier.Sup_Name, 160);
                    Supplier.Sup_MetaKeyword = Core.Global.RemovePurgeMeta(Supplier.Sup_Name, 255);
                    Supplier.Sup_Active = true;
                    Supplier.Sup_Date = DateTime.Now;
                    Supplier.Sup_Sync = true;

                    SupplierRepository.Add(Supplier);
                }
            }
        }

        private void buttonAllCheckImportSupplier_Click(object sender, RoutedEventArgs e)
        {
            List<Model.Sage.F_COMPTET> List = (List<Model.Sage.F_COMPTET>)this.ListBoxFournisseur.ItemsSource;
            foreach (Model.Sage.F_COMPTET item in List.Where(frs => !frs.IsImportedSupplier))
            {
                item.CheckToImport = true;
            }
        }
    }
}