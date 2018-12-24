using System;
using System.ComponentModel;
using System.Linq;
using PRESTACONNECT.Core;
using System.Collections.ObjectModel;
using System.Windows;

namespace PRESTACONNECT.Model.Internal
{
    public class SyncArticle : INotifyPropertyChanged
    {
        #region Properties

        #region Redirection

        public System.Windows.Visibility CanDefineRedirection
        {
            get { return (ExistToLocal && !Local.Art_Active) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden; }
        }

        private ObservableCollection<RedirectType> listRedirectType;
        public ObservableCollection<RedirectType> ListRedirectType
        {
            get { return listRedirectType; }
            set
            {
                listRedirectType = value;
                OnPropertyChanged("ListRedirectType");
            }
        }

        public RedirectType selectedRedirectType;
        public RedirectType SelectedRedirectType
        {
            get { return selectedRedirectType; }
            set
            {
                selectedRedirectType = value;
                OnPropertyChanged("SelectedRedirectType");

                if ((selectedRedirectType == null && !String.IsNullOrEmpty(Local.Art_RedirectType)) || (selectedRedirectType != null && selectedRedirectType.Page != Local.Art_RedirectType))
                    HasUpdated = true;

                Local.Art_RedirectType = (selectedRedirectType != null) ? selectedRedirectType.Page : string.Empty;

                if (selectedRedirectType.CanDefineRedirectProduct)
                    ListRedirectProduct = (ListRedirectProduct == null) ? new ObservableCollection<Model.Local.Article>(new Model.Local.ArticleRepository().ListSyncActive(true, true)) : ListRedirectProduct;
                else
                    ListRedirectProduct = null;
            }
        }

        private ObservableCollection<Model.Local.Article> listRedirectProduct;
        public ObservableCollection<Model.Local.Article> ListRedirectProduct
        {
            get { return listRedirectProduct; }
            set
            {
                listRedirectProduct = value;
                OnPropertyChanged("ListRedirectProduct");
                if (listRedirectProduct != null && listRedirectProduct.Count(a => a.Art_Id == Local.Art_RedirectProduct) > 0)
                    SelectedRedirectProduct = listRedirectProduct.First(a => a.Art_Id == Local.Art_RedirectProduct);
                else
                    SelectedRedirectProduct = null;
            }
        }

        private Model.Local.Article selectedRedirectProduct;
        public Model.Local.Article SelectedRedirectProduct
        {
            get { return selectedRedirectProduct; }
            set
            {
                selectedRedirectProduct = value;
                OnPropertyChanged("SelectedRedirectProduct");

                if ((selectedRedirectProduct == null && Local.Art_RedirectProduct != 0) || (selectedRedirectProduct != null && selectedRedirectProduct.Art_Id != Local.Art_RedirectProduct))
                    HasUpdated = true;

                Local.Art_RedirectProduct = (selectedRedirectProduct != null) ? selectedRedirectProduct.Art_Id : 0;
            }
        }

        #endregion

        public Model.Local.Article Local { get; private set; }
        public Model.Sage.F_ARTICLE Sage { get; set; }

        public int Id
        {
            get { return ExistToLocal ? Local.Art_Id : 0; }
        }

        public bool ExistToLocal
        {
            get { return (Local != null && Local.Art_Id != 0); }
        }
        public bool IsSync
        {
            get { return (ExistToLocal && Local.Pre_Id.HasValue && Local.Pre_Id.Value > 0); }
        }
        public bool ToSync
        {
            get { return (ExistToLocal ? Local.Art_Sync : false); }
            set
            {
                if (ExistToLocal)
                    Local.Art_Sync = value;
                HasUpdatedSync = true;
                OnPropertyChanged("ToSync");
            }
        }
        public bool ToSyncPrice
        {
            get { return (ExistToLocal ? Local.Art_SyncPrice : false); }
            set
            {
                if (ExistToLocal)
                    Local.Art_SyncPrice = value;
                if (!Local.Art_Sync)
                    Local.Art_SyncPrice = false;
                HasUpdatedSync = true;
                OnPropertyChanged("ToSyncPrice");
            }
        }
        public bool ToActive
        {
            get { return ExistToLocal ? Local.Art_Active : false; }
            set
            {
                if (ExistToLocal)
                    Local.Art_Active = value;
                HasUpdated = true;
                OnPropertyChanged("ToActive");
                OnPropertyChanged("CanDefineRedirection");
            }
        }
        public bool IsOnSale
        {
            get { return (ExistToLocal ? Local.Art_Solde.Value : false); }
            set
            {
                if (ExistToLocal)
                    Local.Art_Solde = value;
                HasUpdated = true;
                OnPropertyChanged("IsOnSale");
            }
        }

        // <JG> 11/07/2013 correction gestion update
        public bool HasUpdated { get; set; }
        public bool HasUpdatedSync { get; set; }
        public bool HasUpdatedCharacteristic { get; set; }
        public bool HasUpdatedAttribute { get; set; }
        public bool HasUpdatedConditioning { get; set; }
        public bool HasSelectedImage { get; set; }
        public bool HasSelectedDocument { get; set; }

        private bool hasUpdatedCompositionArticleAttributeGroup = false;
        public bool HasUpdatedCompositionArticleAttributeGroup
        {
            get { return hasUpdatedCompositionArticleAttributeGroup; }
            set { hasUpdatedCompositionArticleAttributeGroup = value; OnPropertyChanged("HasUpdatedCompositionArticleAttributeGroup"); OnPropertyChanged("CanDefineCompositionArticle"); }
        }
        private bool hasUpdatedCompositionArticle = false;
        public bool HasUpdatedCompositionArticle
        {
            get { return hasUpdatedCompositionArticle; }
            set { hasUpdatedCompositionArticle = value; OnPropertyChanged("HasUpdatedCompositionArticle"); OnPropertyChanged("CanDefineCompositionArticleAttributeGroup"); }
        }

        public bool CanDefineCompositionArticleAttributeGroup
        {
            get
            {
                return (Local.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleComposition
                        && !HasUpdatedCompositionArticle);
            }
        }
        public void SendCanDefinePropertyChanged()
        {
            OnPropertyChanged("CanDefineCompositionArticleAttributeGroup");
            OnPropertyChanged("CanDefineCompositionArticle");
        }
        public bool CanDefineCompositionArticle
        {
            get
            {
                return (Local.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleComposition
                        && (Local.CompositionArticleAttributeGroup != null && Local.CompositionArticleAttributeGroup.Count > 0)
                        && !HasUpdatedCompositionArticleAttributeGroup);
            }
        }

        public bool Updated
        {
            get
            {
                return (HasUpdated
                    || HasUpdatedCharacteristic
                    || HasUpdatedAttribute
                    || HasUpdatedConditioning
                    || HasSelectedImage
                    || HasSelectedDocument
                    || HasUpdatedCompositionArticleAttributeGroup
                    || HasUpdatedCompositionArticle);
            }
        }

        public string Name
        {
            get { return Local.Art_Name; }
            set
            {
                Local.Art_Name = Core.Global.RemovePurge(value, 128);
                HasUpdated = true;
                OnPropertyChanged("Name");
            }
        }
        public string MetaTitle
        {
            get { return Local.Art_MetaTitle; }
            set
            {
                Local.Art_MetaTitle = Core.Global.RemovePurge(value, 70);
                HasUpdated = true;
                OnPropertyChanged("MetaTitle");
                if (string.IsNullOrEmpty(LinkRewrite) || Local.Pre_Id == null)
                    LinkRewrite = value;
            }
        }
        public string MetaDescription
        {
            get { return Local.Art_MetaDescription; }
            set
            {
                Local.Art_MetaDescription = Core.Global.RemovePurge(value, 160);
                HasUpdated = true;
                OnPropertyChanged("MetaDescription");
            }
        }
        public string MetaKeyword
        {
            get { return Local.Art_MetaKeyword; }
            set
            {
                Local.Art_MetaKeyword = Core.Global.RemovePurgeMeta(value, 255);
                HasUpdated = true;
                OnPropertyChanged("MetaKeyword");
            }
        }
        public string LinkRewrite
        {
            get { return Local.Art_LinkRewrite; }
            set
            {
                string rewriting = Global.ReadLinkRewrite(value);
                if (rewriting != Local.Art_LinkRewrite)
                {
                    Local.Art_LinkRewrite = rewriting;
                    HasUpdated = true;
                    OnPropertyChanged("LinkRewrite");
                }
                rewriting = null;
            }
        }
        public string DescriptionShort
        {
            get { return Local.Art_Description_Short; }
            set
            {
                Local.Art_Description_Short = value;
                HasUpdated = true;
                OnPropertyChanged("DescriptionShort");
            }
        }
        public string Description
        {
            get { return Local.Art_Description; }
            set
            {
                Local.Art_Description = value;
                HasUpdated = true;
                OnPropertyChanged("Description");
            }
        }
        public string EAN
        {
            get { return Local.Art_Ean13; }
            set
            {
                string s = Core.Global.RemovePurgeEAN(value);
                if (s != Local.Art_Ean13)
                {
                    Local.Art_Ean13 = s;
                    HasUpdated = true;
                    OnPropertyChanged("EAN");
                }
                s = null;
            }
        }
        public string Reference
        {
            get { return Local.Art_Ref; }
            set
            {
                OnPropertyChanged("Reference");
            }
        }
        public bool IsPack
        {
            get { return Local.Art_Pack; }
            set
            {
                OnPropertyChanged("IsPack");
            }
        }

        public Boolean CanDelete
        {
            get
            {
                return (Local.Art_Id != 0 && (Local.Pre_Id == null || (Local.TypeArticle != Model.Local.Article.enum_TypeArticle.ArticleComposition && Sage == null)));
            }
        }
        public Visibility CanDeleteVisibility
        {
            get
            {
                if (CanDelete)
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }
        }
        public Boolean CanTransform
        {
            get
            {
                return (Local.Art_Id != 0 && Local.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleSimple);
            }
        }
        public Visibility CanTransformVisibility
        {
            get
            {
                if (CanTransform)
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }
        }
        #endregion
        #region Constructors

        public SyncArticle(Model.Local.Article local, Model.Sage.F_ARTICLE sage)
        {
            Local = local;
            Sage = sage;

            OnPropertyChanged("CanDelete");

            ListRedirectType = new ObservableCollection<RedirectType>();
            foreach (Core.Parametres.RedirectType valeur in Enum.GetValues(typeof(Core.Parametres.RedirectType)))
                ListRedirectType.Add(new RedirectType(valeur));
            if (!String.IsNullOrWhiteSpace(Local.Art_RedirectType) && Core.Global.IsInteger(Local.Art_RedirectType) && ListRedirectType.Count(r => r.Page == Local.Art_RedirectType) == 1)
                SelectedRedirectType = ListRedirectType.First(r => r.Page == Local.Art_RedirectType);
        }

        #endregion
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        //public void RefreshTiny()
        //{
        //    OnPropertyChanged("Description");
        //}

        #endregion
        #region Methods

        public bool IsRemovedFromPrestashop()
        {
            if (!ExistToLocal || !Local.Pre_Id.HasValue || Local.Pre_Id.Value == 0)
                return false;
            else
                return !(new Model.Prestashop.PsProductRepository().ExistId((uint)Local.Pre_Id.Value));
        }

        public override string ToString()
        {
            return Name.ToString();
        }

        #endregion
    }
}
