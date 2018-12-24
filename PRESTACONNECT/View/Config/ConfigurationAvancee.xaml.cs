using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using PRESTACONNECT.Contexts;

namespace PRESTACONNECT.View
{
    public partial class ConfigurationAvancee : Window
    {
        #region Properties

        internal new ConfigurationAvanceeContext DataContext
        {
            get { return (ConfigurationAvanceeContext)base.DataContext; }
            private set
            {
                if (value != null)
                {
                    value.Saved -= new EventHandler(DataContext_Saved);
                }

                base.DataContext = value;

                if (value != null)
                {
                    value.Saved += new EventHandler(DataContext_Saved);
                }
            }
        }

        #endregion
        #region Constructors

        public ConfigurationAvancee()
        {
            DataContext = new ConfigurationAvanceeContext();
            InitializeComponent();
        }

        #endregion
        #region Event methods

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext.Load();

            AppRootFolder.Focus();
            AppRootFolder.SelectAll();
        }

        private void DataContext_Saved(object sender, EventArgs e)
        {
            DialogResult = true;
        }

        private void DossierRacineParcourir_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            dialog.Description = "Choississez l'emplacement des données de l'application :";

            if (Directory.Exists(DataContext.AppRootFolder))
                dialog.SelectedPath = DataContext.AppRootFolder;
            else
                dialog.RootFolder = Environment.SpecialFolder.MyComputer;

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AppRootFolder.Text = dialog.SelectedPath;
                AppRootFolder.Focus();
                AppRootFolder.SelectAll();
            }

        }
        private void Appliquer_Click(object sender, RoutedEventArgs e)
        {
            DataContext.ApplyChanges();
        }

        #endregion
    }
}
