using System.Linq;


namespace PRESTACONNECT.Model.Internal
{
    public sealed class AssociateCatalog : HierarchicalCatalog<AssociateCatalog>
    {
        #region Properties

        public Model.Local.Article Item { get; set; }

        public bool ToAssociate
        {
            get { return Local.Cat_AssociateArticle; }
            set
            {
                Local.Cat_AssociateArticle = value;
                OnPropertyChanged("ToAssociate");
                OnPropertyChanged("IsDeployed");
            }
        }
        public bool IsDefault
        {
            get { return Local.Cat_Id.Equals(Item.Cat_Id); }
            set { OnPropertyChanged("IsDefault"); OnPropertyChanged("CanUnselect"); }
        }
        public bool CanUnselect
        {
            get { return !IsDefault; }
        }

        public string Name
        {
            get { return ExistToLocal ? Local.Cat_Name : string.Empty; }
        }

        public bool IsDeployed
        {
            get
            {
                return (Children.Count(c => c.ToAssociate || c.IsDeployed) > 0);
            }
        }

        #endregion
        #region Constructors

        public AssociateCatalog(Model.Local.Article item, Model.Local.Catalog localParent)
            : base(null, localParent)
        {
            Item = item;
        }

        #endregion
    }
}
