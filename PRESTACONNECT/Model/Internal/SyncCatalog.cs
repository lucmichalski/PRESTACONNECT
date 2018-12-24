using PRESTACONNECT.Core;
using System.IO;
using System;

namespace PRESTACONNECT.Model.Internal
{
    public sealed class SyncCatalog : HierarchicalCatalog<SyncCatalog>
    {
        #region Properties

        private string smallImagePath;
        private string smallImageTempPath;
        public string SmallImagePath
        {
            get
            {
                if (!string.IsNullOrEmpty(smallImagePath))
                    return smallImagePath;
                else if (!ExistToLocal || (Local.CatalogImage.Count == 0) || string.IsNullOrEmpty(Local.CatalogImage[0].ImaCat_Image))
                    return @"..\Img\img.jpg";
                else
                {
                    if (string.IsNullOrEmpty(smallImageTempPath))
                        smallImageTempPath = System.IO.Path.Combine(Global.GetConfig().Folders.Temp, Guid.NewGuid().ToString("N"));

                    if (!File.Exists(smallImageTempPath))
                        File.Copy(System.IO.Path.Combine(Global.GetConfig().Folders.SmallCatalog, Local.CatalogImage[0].ImaCat_Image), smallImageTempPath);

                    return smallImageTempPath;
                }
            }
            set
            {
                smallImagePath = value;
                OnPropertyChanged("SmallImagePath");
            }
        }

        public bool HasUpdated { get; set; }
        public bool HasNewImage
        {
            get { return !string.IsNullOrEmpty(smallImagePath); }
        }

        public string Name
        {
            get { return ExistToLocal ? Local.Cat_Name : string.Empty; }
            set
            {
                if (ExistToLocal)
                    Local.Cat_Name = Global.RemovePurge(value);

                HasUpdated = true;
                OnPropertyChanged("Name");
            }
        }
        public string Description
        {
            get { return ExistToLocal ? Local.Cat_Description : string.Empty; }
            set
            {
                if (ExistToLocal)
                    Local.Cat_Description = Global.RemovePurge(value);

                HasUpdated = true;
                OnPropertyChanged("Description");
            }
        }
        public string MetaTitle
        {
            get { return ExistToLocal ? Local.Cat_MetaTitle : string.Empty; }
            set
            {
                if (ExistToLocal)
                    Local.Cat_MetaTitle = Global.RemovePurge(value);

                HasUpdated = true;
                OnPropertyChanged("MetaTitle");

                LinkRewrite = value;
            }
        }
        public string MetaDescription
        {
            get { return ExistToLocal ? Local.Cat_MetaDescription : string.Empty; }
            set
            {
                if (ExistToLocal)
                    Local.Cat_MetaDescription = Global.RemovePurge(value);

                HasUpdated = true;
                OnPropertyChanged("MetaDescription");
            }
        }
        public string MetaKeyword
        {
            get { return ExistToLocal ? Local.Cat_MetaKeyword : string.Empty; }
            set
            {
                if (ExistToLocal)
                    Local.Cat_MetaKeyword = Global.RemovePurge(value);

                HasUpdated = true;
                OnPropertyChanged("MetaKeyword");
            }
        }
        public string LinkRewrite
        {
            get { return ExistToLocal ? Local.Cat_LinkRewrite : string.Empty; }
            set
            {
                if (ExistToLocal)
                {
                    string rewriting = Global.RemovePurge(Global.ReadLinkRewrite(Global.RemoveDiacritics(value)).ToLower());
                    Local.Cat_LinkRewrite = (rewriting.Length > 128) ? rewriting.Substring(0, 128) : rewriting;
                }

                HasUpdated = true;
                OnPropertyChanged("LinkRewrite");
            }
        }

        private Model.Sage.F_CATALOGUE defaultCatalog;
        public Model.Sage.F_CATALOGUE DefaultCatalog
        {
            get { return defaultCatalog; }
            set
            {
                defaultCatalog = value;
                Local.Sag_Id = (defaultCatalog == null) ? 0 : defaultCatalog.cbMarq;
                OnPropertyChanged("SelectedDefaultCatalog");
            }
        }

        // <JG> 04/02/2013 Ajout attributs de gestion de possibilité de suppression du catalogue dans Prestashop
        private Boolean toDeleteFromPrestashop = false;
        public Boolean ToDeleteFromPrestashop
        {
            get { return toDeleteFromPrestashop; }
            set
            {
                toDeleteFromPrestashop = value;
                OnPropertyChanged("ToDeleteFromPrestashop");
                CanDelete = (ToDeleteFromPrestashop && CanRemovePrestashop);
            }
        }

        private Boolean canDelete = false;
        public Boolean CanDelete
        {
            get { return canDelete; }
            set
            {
                canDelete = value;
                OnPropertyChanged("CanDelete");
            }
        }

        #endregion
        #region Constructors

        public SyncCatalog(Model.Local.Catalog localParent)
            : base(null, localParent)
        {
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return Name.ToString();
        }

        #endregion
    }
}
