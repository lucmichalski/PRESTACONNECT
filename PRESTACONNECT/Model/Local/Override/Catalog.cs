using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;

namespace PRESTACONNECT.Model.Local
{
    public partial class Catalog
    {
        #region attributs

        private string smallImagePath;
        private string smallImageTempPath;
        public string SmallImagePath
        {
            get
            {
                if (!string.IsNullOrEmpty(smallImagePath))
                {
                    return smallImagePath;
                }
                else
                {
                    Model.Local.CatalogImageRepository CatalogImageRepository = new CatalogImageRepository();
                    List<Model.Local.CatalogImage> list = CatalogImageRepository.ListCatalog(Cat_Id);
                    if (list.Count == 0 || string.IsNullOrEmpty(list[0].ImaCat_Image))
                        return @"..\Img\img.jpg";
                    else
                    {
                        if (string.IsNullOrEmpty(smallImageTempPath))
                            smallImageTempPath = System.IO.Path.Combine(Core.Global.GetConfig().Folders.Temp, Guid.NewGuid().ToString("N"));

                        if (!File.Exists(smallImageTempPath))
                            File.Copy(System.IO.Path.Combine(Core.Global.GetConfig().Folders.SmallCatalog, list[0].ImaCat_Image), smallImageTempPath);

                        return smallImageTempPath;
                    }
                }
            }
            set
            {
                smallImagePath = value;
                SendPropertyChanged("SmallImagePath");
            }
        }

        public void ReloadImage()
        {
            smallImagePath = string.Empty;
            SendPropertyChanged("SmallImagePath");
        }

        public bool HasUpdated { get; set; }

        public bool HasUpdatedOrChild
        {
            get { return HasUpdated || Catalog2.Count(c => c.HasUpdatedOrChild) > 0 || Catalog2.Count(c => c.HasNewImage) > 0; }
        }

        public bool HasNewImage
        {
            get { return !string.IsNullOrEmpty(smallImagePath); }
        }

        public bool Active
        {
            get { return Cat_Active; }
            set
            {
                Cat_Active = value;
                HasUpdated = true;
                SendPropertyChanged("Active");
            }
        }

        public string Name
        {
            get { return Cat_Name; }
            set
            {
                Cat_Name = Core.Global.RemovePurge(value, 128);
                HasUpdated = true;
                SendPropertyChanged("Name");
                //if (string.IsNullOrEmpty(LinkRewrite))
                LinkRewrite = value;
            }
        }
        public string Description
        {
            get { return Cat_Description; }
            set
            {
                Cat_Description = Core.Global.RemovePurge(value, 10000);
                HasUpdated = true;
                SendPropertyChanged("Description");
            }
        }
        public string MetaTitle
        {
            get { return Cat_MetaTitle; }
            set
            {
                Cat_MetaTitle = Core.Global.RemovePurge(value, 70);
                HasUpdated = true;
                SendPropertyChanged("MetaTitle");
            }
        }
        public string MetaDescription
        {
            get { return Cat_MetaDescription; }
            set
            {
                Cat_MetaDescription = Core.Global.RemovePurge(value, 160);
                HasUpdated = true;
                SendPropertyChanged("MetaDescription");
            }
        }
        public string MetaKeyword
        {
            get { return Cat_MetaKeyword; }
            set
            {
                Cat_MetaKeyword = Core.Global.RemovePurgeMeta(value, 255);
                HasUpdated = true;
                SendPropertyChanged("MetaKeyword");
            }
        }
        public string LinkRewrite
        {
            get { return Cat_LinkRewrite; }
            set
            {
                string rewriting = Core.Global.ReadLinkRewrite(value);
                if (rewriting != Cat_LinkRewrite)
                {
                    Cat_LinkRewrite = rewriting;
                    HasUpdated = true;
                    SendPropertyChanged("LinkRewrite");
                }
                rewriting = null;
            }
        }

        public Model.Sage.F_CATALOGUE DefaultCatalog
        {
            get
            {
                if (Core.Temp.ListF_CATALOGUE.Count(c => c.cbMarq == this.Sag_Id) == 1)
                {
                    return Core.Temp.ListF_CATALOGUE.First(c => c.cbMarq == this.Sag_Id);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                int sage = (value == null) ? 0 : value.cbMarq;
                if (sage != Sag_Id)
                {
                    Sag_Id = sage;
                    HasUpdated = true;
                    this.SendPropertyChanged("DefaultCatalog");
                }
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
                SendPropertyChanged("ToDeleteFromPrestashop");
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
                SendPropertyChanged("CanDelete");
            }
        }

        #endregion

        #region attributs 2

        public bool IsSync
        {
            get { return Pre_Id.HasValue && Pre_Id.Value > 0; }
        }

        // <JG> 04/02/2013 Ajout de propriétés de contrôle pour suppression
        public bool ExistInPrestashop
        {
            get
            {
                return (Pre_Id != null
                   && Pre_Id != 0
                   && new Model.Prestashop.PsCategoryRepository().ExistId((int)Pre_Id));
            }
        }
        public bool CanRemoveLocal
        {
            get
            {
                return (!ExistInPrestashop
                        && !HasLocalChild
                        && !IsDefaultLocal);
            }
        }
        public bool CanRemovePrestashop
        {
            get
            {
                return (!HasLocalChild
                        && !IsDefaultLocal
                        && ExistInPrestashop
                        && !HasPrestashopChild
                        && !IsDefaultPrestashop
                        && !HasProductPrestashop);
            }
        }
        public bool HasLocalChild
        {
            get { return new Model.Local.CatalogRepository().ExistParent(Cat_Id); }
        }
        public bool IsDefaultLocal
        {
            get { return new Model.Local.ArticleRepository().ExistDefaultCatalog(Cat_Id); }
        }
        public bool HasPrestashopChild
        {
            get { return new Model.Prestashop.PsCategoryRepository().ExistChild((int)Pre_Id); }
        }
        public bool IsDefaultPrestashop
        {
            get { return new Model.Prestashop.PsProductRepository().ExistDefaultCatalog((int)Pre_Id); }
        }
        public bool HasProductPrestashop
        {
            get { return new Model.Prestashop.PsCategoryProductRepository().ExistCategory((uint)Pre_Id); }
        }

        public bool IsRemovedFromPrestashop()
        {
            if (!Pre_Id.HasValue || Pre_Id.Value == 0)
                return false;
            else
                return !(new Model.Prestashop.PsCategoryRepository().ExistId(Pre_Id.Value));
        }
        public void PrepareToCreateInPrestashop()
        {
            Pre_Id = null;
        }

        #endregion

        private String m_Cat_IdWithParent;
        public String Cat_IdWithParent
        {
            get
            {
                return this.m_Cat_IdWithParent;
            }
            set
            {
                this.m_Cat_IdWithParent = value;
            }
        }

        private Boolean m_Cat_AssociateArticle;
        public Boolean Cat_AssociateArticle
        {
            get
            {
                return this.m_Cat_AssociateArticle;
            }
            set
            {
                this.m_Cat_AssociateArticle = value;
            }
        }

        public String ComboText
        {
            get
            {
                string t = this.Cat_Name;
                Model.Local.Catalog p = this;
                while (p.Catalog1 != null)
                {
                    p = p.Catalog1;
                    t = p.Cat_Name + " >> " + t;
                }
                return t;
            }
        }

        public Dictionary<int, int> GetParentsByIDLevel
        {
            get
            {
                Dictionary<int, int> parents = new Dictionary<int, int>();
                Model.Local.Catalog p = this;
                while (p.Catalog1 != null)
                {
                    p = p.Catalog1;
                    parents.Add(p.Cat_Id, p.Cat_Level);
                }
                return parents;
            }
        }

        public Visibility SyncVisibility
        {
            get
            {
                if (Cat_Sync)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }

        #region Methods

        public override string ToString()
        {
            return ComboText;
        }

        public Boolean ChildrenContainsCatalog(List<Int32> childs)
        {
            return Catalog2.Count(c => childs.Count(cd => cd == c.Cat_Id) > 0) > 0
                || Catalog2.ToList().Count(c => c.ChildrenContainsCatalog(childs)) > 0;
        }

        public Boolean ChildrenContainsCatalog(Int32 child)
        {
            return Catalog2.Count(c => c.Cat_Id == child) > 0
                || Catalog2.ToList().Count(c => c.ChildrenContainsCatalog(child)) > 0;
        }

        #endregion

        public IQueryable<Model.Local.Catalog> SortChildren
        {
            get { return Catalog2.OrderBy(c => c.Cat_Name).AsQueryable(); }
        }
    }

    public class CatalogLight
    {
        public int Cat_Id;
        public int Pre_Id;
    }
}
