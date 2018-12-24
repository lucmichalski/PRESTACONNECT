using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PRESTACONNECT.Core;

namespace PRESTACONNECT.Contexts
{
    internal sealed class DeplacementCatalogueContext : Context
    {
        #region Properties

        private Model.Local.CatalogRepository CatalogRepository { get; set; }

        private Model.Local.Catalog targetCatalog;
        public Model.Local.Catalog TargetCatalog
        {
            get { return targetCatalog; }
            set
            {
                targetCatalog = value;
                OnPropertyChanged("TargetCatalog");
                OnPropertyChanged("ButtonMoveText");
            }
        }

        public String ButtonMoveText
        {
            get
            {
                return (TargetCatalog != null)
                    ? "Déplacer  \"" + TargetCatalog.Cat_Name + "\"  vers  \"" + ((SelectedCatalog == null) ? "Racine boutique" : SelectedCatalog.Cat_Name) + "\""
                    : "Aucun catalogue sélectionné pour le déplacement !";
            }
        }

        private ObservableCollection<Model.Local.Catalog> catalogs;
        public ObservableCollection<Model.Local.Catalog> Catalogs
        {
            get { return catalogs; }
            set
            {
                catalogs = value;
                OnPropertyChanged("Catalogs");
            }
        }

        private Model.Local.Catalog selectedCatalog;
        public Model.Local.Catalog SelectedCatalog
        {
            get { return selectedCatalog; }
            set
            {
                selectedCatalog = value;
                OnPropertyChanged("SelectedCatalog");
                OnPropertyChanged("ButtonMoveText");
            }
        }

        #endregion

        #region Constructors

        public DeplacementCatalogueContext()
            : base()
        {
            this.LoadCatalogs();
            OnPropertyChanged("ButtonMoveText");
        }

        public void LoadCatalogs()
        {
            IsBusy = true;
            LoadingStep = "Chargement des catalogues ...";
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate { Mouse.OverrideCursor = Cursors.Wait; }), null);

            CatalogRepository = new Model.Local.CatalogRepository();

            Catalogs = new ObservableCollection<Model.Local.Catalog>(CatalogRepository.ListParent(0));

            SelectedCatalog = null;

            LoadingStep = string.Empty;
            IsBusy = false;
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate { Mouse.OverrideCursor = null; }), null);
        }

        #endregion

        #region Methods

        public void MoveCatalog()
        {
            if (MessageBox.Show("Valider le déplacement de :\n\""
                + TargetCatalog.Cat_Name
                + "\"\n vers :\n\""
                + ((SelectedCatalog == null) ? "Racine boutique" : SelectedCatalog.Cat_Name) + "\"", "Déplacement catalogue", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                CatalogRepository.WriteParent(TargetCatalog.Cat_Id, ((SelectedCatalog == null) ? 0 : SelectedCatalog.Cat_Id));

                CatalogRepository = new Model.Local.CatalogRepository();
                TargetCatalog = CatalogRepository.ReadId(TargetCatalog.Cat_Id);
                TargetCatalog.Cat_Level = ((SelectedCatalog == null) ? 2 : SelectedCatalog.Cat_Level + 1);
                TargetCatalog.Cat_Date = DateTime.Now;

                // <JG> 07/10/2016 ajout recalcul des niveaux sur les enfants
                ChangeLevelChilds(TargetCatalog);

                CatalogRepository.Save();
            }
        }

        private void ChangeLevelChilds(Model.Local.Catalog Catalog)
        {
            if (Catalog.Catalog2 != null && Catalog.Catalog2.Count > 0)
            {
                foreach (Model.Local.Catalog child in Catalog.Catalog2)
                {
                    child.Cat_Level = Catalog.Cat_Level + 1;
                    ChangeLevelChilds(child);
                }
            }
        }

        #endregion
    }
}
