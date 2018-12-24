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
    internal sealed class CatalogueContext : Context
    {
        #region Properties

        private bool ChangeParentToSyncIsBusy { get; set; }
        private Model.Local.CatalogRepository CatalogRepository { get; set; }

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
                OnPropertyChanged("CatalogIsSelected");
            }
        }

        public bool CatalogIsSelected
        {
            get { return SelectedCatalog != null; }
        }

        public List<Model.Sage.F_CATALOGUE> ListF_CATALOGUE
        {
            get { return Core.Temp.ListF_CATALOGUE; }
        }

        private Boolean syncAll = true;
        public Boolean SyncAll
        {
            get { return syncAll; }
            set { syncAll = value; OnPropertyChanged("SyncAll"); }
        }
        private Boolean syncSelected = false;
        public Boolean SyncSelected
        {
            get { return syncSelected; }
            set { syncSelected = value; OnPropertyChanged("SyncSelected"); }
        }

        #endregion

        #region Constructors

        public CatalogueContext()
            : base()
        {
            this.LoadCatalogs();
        }

        public void LoadCatalogs()
        {
            IsBusy = true;
            LoadingStep = "Chargement des catalogues ...";
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate { Mouse.OverrideCursor = Cursors.Wait; }), null);

            CatalogRepository = new Model.Local.CatalogRepository();

            Catalogs = new ObservableCollection<Model.Local.Catalog>(CatalogRepository.ListParent(0));

            Core.Temp.ListF_CATALOGUE = new List<Model.Sage.F_CATALOGUE>();
            Core.Temp.ListF_CATALOGUE.Add(new Model.Sage.F_CATALOGUE() { CL_Intitule = "Aucun", });
            Core.Temp.ListF_CATALOGUE.AddRange(new Model.Sage.F_CATALOGUERepository().List());
            OnPropertyChanged("ListF_CATALOGUE");

            SelectedCatalog = null;

            SyncAll = true;

            LoadingStep = string.Empty;
            IsBusy = false;
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate { Mouse.OverrideCursor = null; }), null);
        }

        #endregion

        #region Methods

        public bool HasUpdated()
        {
            return Catalogs.Count(c => c.HasUpdatedOrChild) > 0;
        }

        private void ChangeChildrenToSyncCatalogs(Model.Local.Catalog catalog, bool toSync)
        {
            catalog.Cat_Sync = toSync;

            foreach (var child in catalog.Catalog2)
                ChangeChildrenToSyncCatalogs(child, toSync);
        }
        private void ChangeParentToSyncCatalogs(Model.Local.Catalog catalog, bool toSync)
        {
            catalog.Cat_Sync = toSync;

            foreach (var parent in Catalogs)
            {
                if (toSync)
                {
                    foreach (var child in parent.Catalog2)
                        if (child.Cat_Id.Equals(catalog.Cat_Id))
                            ChangeParentToSyncCatalogs(parent, toSync);
                }
                else
                {
                    bool find = false;
                    bool atLeastOne = false;

                    foreach (var child in parent.Catalog2)
                        if (child.Cat_Id.Equals(catalog.Cat_Id))
                        {
                            find = true;
                            break;
                        }

                    if (find)
                    {
                        foreach (var child in parent.Catalog2)
                            if (child.Cat_Sync)
                            {
                                atLeastOne = true;
                                break;
                            }

                        if (!atLeastOne)
                            foreach (var child in parent.Catalog2)
                                if (child.Cat_Id.Equals(catalog.Cat_Id))
                                    ChangeParentToSyncCatalogs(parent, toSync);
                    }
                }
            }
        }

        public void ChangeToSyncCatalogs(Model.Local.Catalog catalog, bool toSync)
        {
            if (!ChangeParentToSyncIsBusy)
                ChangeChildrenToSyncCatalogs(catalog, toSync);

            ChangeParentToSyncIsBusy = true;
            ChangeParentToSyncCatalogs(catalog, toSync);
            ChangeParentToSyncIsBusy = false;
        }
        public bool UpdateAll()
        {
            bool result = false;
            if (!IsBusy)
            {
                result = UpdateCatalogs(Catalogs.ToList());
                if (result)
                    CatalogRepository.Save();
            }
            return result;
        }

        private Boolean UpdateCatalogs(List<Model.Local.Catalog> Catalogs)
        {
            Boolean updated = false;
            DateTime now = DateTime.Now;
            foreach (var catalog in Catalogs)
            {
                if (catalog.HasLocalChild && catalog.HasUpdatedOrChild)
                    if (UpdateCatalogs(catalog.Catalog2.ToList()))
                        updated = true;
                if (catalog.HasNewImage)
                {
                    try
                    {
                        Model.Local.CatalogImageRepository catalogImageRepository = new Model.Local.CatalogImageRepository();

                        bool isNew = false;
                        Model.Local.CatalogImage image = catalogImageRepository.ReadCatalogImageByCatalog(catalog.Cat_Id);

                        if (isNew = (image == null))
                        {
                            image = new Model.Local.CatalogImage()
                            {
                                Cat_Id = catalog.Cat_Id,
                                ImaCat_Image = string.Empty,
                            };
                            catalogImageRepository.Add(image);
                        }
                        String extension = System.IO.Path.GetExtension(catalog.SmallImagePath).ToLower();

                        string uri = catalog.SmallImagePath.Replace("File:///", "").Replace("file:///", "").Replace("File://", "\\\\").Replace("file://", "\\\\").Replace("/", "\\");
                        string tempFile = System.IO.Path.Combine(Global.GetConfig().Folders.TempCatalog, String.Format("{0}" + extension, image.ImaCat_Id));
                        System.IO.File.Copy(catalog.SmallImagePath, tempFile, true);

                        Model.Prestashop.PsImageTypeRepository PsImageTypeRepository = new Model.Prestashop.PsImageTypeRepository();
                        List<Model.Prestashop.PsImageType> ListPsImageType = PsImageTypeRepository.ListCategorie(1);

                        System.Drawing.Image img = System.Drawing.Image.FromFile(tempFile);

                        foreach (Model.Prestashop.PsImageType PsImageType in ListPsImageType)
                        {
                            Core.Img.resizeImage(img, Convert.ToInt32(PsImageType.Width), Convert.ToInt32(PsImageType.Height),
                                System.IO.Path.Combine(Global.GetConfig().Folders.RootCatalog, String.Format("{0}-{1}" + extension, image.ImaCat_Id, PsImageType.Name)));
                        }

                        Core.Img.resizeImage(img, Core.Global.GetConfig().ConfigImageMiniatureWidth, Core.Global.GetConfig().ConfigImageMiniatureHeight,
                                System.IO.Path.Combine(Global.GetConfig().Folders.SmallCatalog, String.Format("{0}" + extension, image.ImaCat_Id)));

                        img.Dispose();

                        if (isNew || image.ImaCat_Image != String.Format("{0}" + extension, image.ImaCat_Id))
                        {
                            image.ImaCat_Image = String.Format("{0}" + extension, image.ImaCat_Id);
                            catalogImageRepository.Save();
                        }
                    }
                    catch (Exception ex)
                    {
                        Core.Error.SendMailError(ex.ToString());
                    }
                }

                if (catalog.HasUpdated || catalog.HasNewImage)
                {
                    catalog.Cat_Date = now;
                    catalog.HasUpdated = false;
                    catalog.ReloadImage();
                    updated = true;
                }
            }
            return updated;
        }

        internal void Sync()
        {
            PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
            Loading.Show();
            if (SyncSelected && SelectedCatalog != null)
            {
                List<int> selection = Catalogs.Where(c => c.ChildrenContainsCatalog(SelectedCatalog.Cat_Id)).Select(c => c.Cat_Id).ToList();
                selection.Add(SelectedCatalog.Cat_Id);
                SynchronisationCatalogue Sync = new SynchronisationCatalogue(selection);
                Sync.ShowDialog();
            }
            else
            {
                SynchronisationCatalogue Sync = new SynchronisationCatalogue();
                Sync.ShowDialog();
            }

            Loading.Close();
        }

        public void CreateCatalog(Model.Local.Catalog parent)
        {
            Model.Local.Catalog local = new Model.Local.Catalog()
            {
                Name = "Nouveau catalogue",
                Cat_Level = (parent == null) ? 2 : parent.Cat_Level + 1,
                Cat_Parent = (parent == null) ? 0 : parent.Cat_Id,
                Cat_Date = DateTime.Now,
                Sag_Id = 0,
                Pre_Id = null,
            };
            CatalogRepository.Add(local);

            this.LoadCatalogs();
        }
        public void DeleteCatalog(Model.Local.Catalog catalog)
        {
            Model.Local.ArticleCatalogRepository ArticleCatalogRepository =
                new Model.Local.ArticleCatalogRepository();

            //Suppression de tous les liens articles
            foreach (var articleCatalog in ArticleCatalogRepository.ListCatalog(catalog.Cat_Id))
                ArticleCatalogRepository.Delete(articleCatalog);

            //Suppression image
            Model.Local.CatalogImageRepository CatalogImageRepository = new Model.Local.CatalogImageRepository();
            if (catalog.CatalogImage != null && catalog.CatalogImage.Count > 0)
            {
                foreach (Model.Local.CatalogImage catalogimage in CatalogImageRepository.ListCatalog(catalog.Cat_Id))
                {
                    catalogimage.EraseFiles();
                    CatalogImageRepository.Delete(catalogimage);
                }
            }

            CatalogRepository.Delete(catalog);

            // <JG> 04/02/2013 ajout de la tentative de suppression du catalogue Prestashop
            try
            {
                if (catalog.Pre_Id != null)
                {
                    Model.Prestashop.PsCategoryRepository PsCategoryRepository = new Model.Prestashop.PsCategoryRepository();
                    if (PsCategoryRepository.ExistId((int)catalog.Pre_Id))
                    {
                        PsCategoryRepository.Delete(PsCategoryRepository.ReadId((UInt32)catalog.Pre_Id));

                        Core.Global.LaunchAlternetis_RegenerateCategoryTree();
                        Core.Global.LaunchAlternetis_ClearSmartyCache();
                    }
                }
            }
            catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }

            this.LoadCatalogs();
        }

        #endregion
    }
}
