using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public abstract class HierarchicalCatalog<TCatalog> : INotifyPropertyChanged
    {
        #region Properties

        protected Model.Sage.F_CATALOGUE Sage { get; private set; }
        public Model.Local.Catalog Local { get; private set; }

        private ObservableCollection<TCatalog> children;
        public ObservableCollection<TCatalog> Children
        {
            get { return children; }
            private set
            {
                children = value;
                OnPropertyChanged("Children");
            }
        }

        public int Id
        {
            get { return ExistToLocal ? Local.Cat_Id : 0; }
        }
        public int SageId
        {
            get { return Sage.cbMarq; }
        }
        public int SageNumero
        {
            get { return Sage.CL_No.HasValue ? Sage.CL_No.Value : 0; }
        }

        public bool ExistToLocal
        {
            get { return (Local != null); }
        }
        public bool ToActive
        {
            get { return (ExistToLocal ? Local.Cat_Active : false); }
            set
            {
                if (ExistToLocal)
                    Local.Cat_Active = value;

                OnPropertyChanged("ToActive");
            }
        }
        public bool ToSync
        {
            get { return (ExistToLocal ? Local.Cat_Sync : false); }
            set
            {
                if (ExistToLocal)
                    Local.Cat_Sync = value;

                OnPropertyChanged("ToSync");
            }
        }
        public bool IsSync
        {
            get { return (ExistToLocal && Local.Pre_Id.HasValue && Local.Pre_Id.Value > 0); }
        }
        public string Intitule
        {
            get { return Sage.CL_Intitule; }
            set
            {
                Sage.CL_Intitule = value;
                OnPropertyChanged("Intitule");
            }
        }

        // <JG> 04/02/2013 Ajout de propriétés de contrôle pour suppression
        public bool ExistInPrestashop
        {
            get
            {
                return (Local != null
                   && Local.Pre_Id != null
                   && Local.Pre_Id != 0
                   && new Model.Prestashop.PsCategoryRepository().ExistId((int)Local.Pre_Id));
            }
        }
        public bool CanRemoveLocal
        {
            get
            {
                return (ExistToLocal
                        && !ExistInPrestashop
                        && !HasLocalChild
                        && !IsDefaultLocal);
            }
        }
        public bool CanRemovePrestashop
        {
            get
            {
                return (ExistToLocal
                        && !HasLocalChild
                        && !IsDefaultLocal
                        && ExistInPrestashop
                        && !HasPrestashopChild
                        && !IsDefaultPrestashop
                        && !HasProductPrestashop);
            }
        }
        public bool HasLocalChild
        {
            get { return new Model.Local.CatalogRepository().ExistParent(Local.Cat_Id); }
        }
        public bool IsDefaultLocal
        {
            get { return new Model.Local.ArticleRepository().ExistDefaultCatalog(Local.Cat_Id); }
        }
        public bool HasPrestashopChild
        {
            get { return new Model.Prestashop.PsCategoryRepository().ExistChild((int)Local.Pre_Id); }
        }
        public bool IsDefaultPrestashop
        {
            get { return new Model.Prestashop.PsProductRepository().ExistDefaultCatalog((int)Local.Pre_Id); }
        }
        public bool HasProductPrestashop
        {
            get { return new Model.Prestashop.PsCategoryProductRepository().ExistCategory((uint)Local.Pre_Id); }
        }

        #endregion
        #region Constructors

        public HierarchicalCatalog(Model.Sage.F_CATALOGUE sageParent, Model.Local.Catalog localParent)
        {
            Children = new ObservableCollection<TCatalog>();

            Sage = sageParent;
            Local = localParent;
        }

        #endregion
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
        #region Event methods

        public bool IsRemovedFromPrestashop()
        {
            if (!ExistToLocal || !Local.Pre_Id.HasValue || Local.Pre_Id.Value == 0)
                return false;
            else
                return !(new Model.Prestashop.PsCategoryRepository().ExistId(Local.Pre_Id.Value));
        }
        public void PrepareToCreateInPrestashop()
        {
            Local.Pre_Id = null;
        }

        public override string ToString()
        {
            return ((Local != null) ? Local.Cat_Name.ToString() : ((Sage != null) ? Sage.CL_Intitule.ToString() : string.Empty));
        }

        #endregion
    }
}
