using PRESTACONNECT.Core;

namespace PRESTACONNECT.Model.Internal
{
    public sealed class ImportCatalog : HierarchicalCatalog<ImportCatalog>
    {
        #region Properties

        //private bool toImport;
        //public bool ToImport
        //{
        //    get { return toImport; }
        //    set
        //    {
        //        toImport = value;
        //        OnPropertyChanged("ToImport");
        //    }
        //}

        public string Name
        {
            get { return ExistToLocal ? Local.Cat_Name : string.Empty; }
        }

        //private ImportCatalog parent;
        //public ImportCatalog Parent
        //{
        //    get { return parent; }
        //    set { parent = value; OnPropertyChanged("Parent"); }
        //}

        //private bool checkChild;
        //public bool CheckChild
        //{
        //    get { return checkChild; }
        //    set
        //    {
        //        checkChild = value;
        //        OnPropertyChanged("CheckChild");
        //    }
        //}

        #endregion
        #region Constructors

        //public ImportCatalog(Model.Sage.F_CATALOGUE F_CATALOGUE, Model.Local.Catalog local, Model.Internal.ImportCatalog parent)
        //    : base(F_CATALOGUE, local)
        //{
        //    CheckChild = true;
        //    Parent = parent;
        //    //ToImport = !ExistToLocal;
        //    ToImport = false;
        //}
        public ImportCatalog(Model.Local.Catalog localParent)
            : base(null, localParent)
        {
            //ToImport = !ExistToLocal;
            //ToImport = false;
        }

        #endregion

        #region Methods
        
        //public override string ToString()
        //{
        //    return ((Local != null) ? Local.Cat_Name.ToString() : ((Sage != null) ? Sage.CL_Intitule.ToString() : string.Empty));
        //}

        #endregion
    }
}
