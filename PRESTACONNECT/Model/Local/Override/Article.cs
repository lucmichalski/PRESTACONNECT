using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Local
{
    public partial class Article
    {
        public enum enum_TypeArticle
        {
            ArticleSimple = 0,
            ArticleComposition = 1,
            ArticleMonoGamme = 2,
            ArticleMultiGammes = 3,
            ArticleConditionnement = 4,
            defaut = 99,
        }

        private enum_TypeArticle typeArticle = enum_TypeArticle.defaut;
        public enum_TypeArticle TypeArticle
        {
            get
            {
                if (typeArticle == enum_TypeArticle.defaut)
                {
                    if (Art_Type == 1)
                    {
                        typeArticle = enum_TypeArticle.ArticleComposition;
                    }
                    else
                    {
                        typeArticle = enum_TypeArticle.ArticleSimple;
                        Model.Sage.F_ARTICLE_Light light = sageArticleLight;
                        if (light != null)
                        {
                            if (light.AR_Gamme1 != null && light.AR_Gamme1 != 0 && light.AR_Gamme2 != null && light.AR_Gamme2 != 0)
                            {
                                typeArticle = enum_TypeArticle.ArticleMultiGammes;
                            }
                            else if (light.AR_Gamme1 != null && light.AR_Gamme1 != 0)
                            {
                                typeArticle = enum_TypeArticle.ArticleMonoGamme;
                            }
                            else if (light.AR_Condition != null && light.AR_Condition != 0)
                            {
                                typeArticle = enum_TypeArticle.ArticleConditionnement;
                            }
                        }
                    }
                }
                return typeArticle;
            }
            set
            {
                if (value == enum_TypeArticle.defaut)
                {
                    typeArticle = enum_TypeArticle.defaut;
                    SendPropertyChanged("TypeArticle");
                    SendPropertyChanged("ImageTypeArticle");
                    SendPropertyChanged("TypeArticleString");
                    SendPropertyChanged("CanSync");
                    SendPropertyChanged("CanSyncPrice");
                }
            }
        }

        public Model.Sage.F_ARTICLE_Light sageArticleLight
        {
            get
            {
                Model.Sage.F_ARTICLERepository art_repo = new Sage.F_ARTICLERepository();
                if (art_repo.ExistArticle(this.Sag_Id))
                {
                    return art_repo.ReadLight(this.Sag_Id);
                }
                else
                {
                    return null;
                }
            }
        }

        public String ComboText
        {
            get
            {
                return this.Art_Id + " - " + this.Art_Ref + " - " + this.Art_Name;
            }
        }

        private string smallImageTempPath = "/PRESTACONNECT;component/Resources/prestaconnect-name-mini.png";
        public string SmallImageTempPath
        {
            get
            {
                if (smallImageTempPath == "/PRESTACONNECT;component/Resources/prestaconnect-name-mini.png"
                    && this.ArticleImage != null && this.ArticleImage.Count > 0)
                {
                    if (this.ArticleImage.Count(img => img.ImaArt_Default) > 0)
                        smallImageTempPath = this.ArticleImage.First(img => img.ImaArt_Default).SmallImageTempPath;
                    else
                        smallImageTempPath = this.ArticleImage.First().SmallImageTempPath;

                    // si l'image de base est introuvable
                    if (!System.IO.File.Exists(smallImageTempPath))
                        smallImageTempPath = "/PRESTACONNECT;component/Resources/file_broken_small.png";
                }
                return smallImageTempPath;
            }
        }

        private string imageTypeArticle = "/PRESTACONNECT;component/Resources/product.png";
        public string ImageTypeArticle
        {
            get
            {
                switch (TypeArticle)
                {
                    case enum_TypeArticle.ArticleComposition:
                        imageTypeArticle = "/PRESTACONNECT;component/Resources/components.png";
                        break;
                    case enum_TypeArticle.ArticleMonoGamme:
                        imageTypeArticle = "/PRESTACONNECT;component/Resources/color-swatch.png";
                        break;
                    case enum_TypeArticle.ArticleMultiGammes:
                        imageTypeArticle = "/PRESTACONNECT;component/Resources/pantone.png";
                        break;
                    case enum_TypeArticle.ArticleConditionnement:
                        imageTypeArticle = "/PRESTACONNECT;component/Resources/bottles.png";
                        break;
                    case enum_TypeArticle.ArticleSimple:
                    case enum_TypeArticle.defaut:
                    default:
                        //no modification of default value : "/PRESTACONNECT;component/Resources/product.png"
                        break;
                }
                return imageTypeArticle;
            }
        }
        public string TypeArticleString
        {
            get
            {
                string temp = string.Empty;
                switch (TypeArticle)
                {
                    case enum_TypeArticle.ArticleComposition:
                        temp = "Article de composition";
                        break;
                    case enum_TypeArticle.ArticleMonoGamme:
                        temp = "Article mono gamme";
                        break;
                    case enum_TypeArticle.ArticleMultiGammes:
                        temp = "Article à gammes multiples";
                        break;
                    case enum_TypeArticle.ArticleConditionnement:
                        temp = "Article en conditionnement";
                        break;
                    case enum_TypeArticle.ArticleSimple:
                    case enum_TypeArticle.defaut:
                    default:
                        temp = "Article simple";
                        break;
                }
                return temp;
            }
        }

        public Model.Prestashop.ProductUpdate prestashopProduct
        {
            get
            {
                return (this.Pre_Id != null && Core.Temp.ListProductUpdate != null && Core.Temp.ListProductUpdate.Count > 0) ? 
					Core.Temp.ListProductUpdate.FirstOrDefault(p => p.id_product == Pre_Id) : null;
            }
        }
        //public string DateForeColor
        //{
        //    get
        //    {
        //        if (prestashopProduct != null && prestashopProduct.date_upd != null
        //            && prestashopProduct.date_upd.Value >= Art_Date)
        //            return "#FF0072F8";
        //        else if (prestashopProduct != null && prestashopProduct.date_upd != null
        //                && prestashopProduct.date_upd.Value <= Art_Date)
        //            return "#FFF85F00";
        //        else
        //            return "#FF000000";
        //    }
        //}
        //public string DateSelectedForeColor
        //{
        //    get
        //    {
        //        if (prestashopProduct != null && prestashopProduct.date_upd != null
        //            && prestashopProduct.date_upd.Value >= Art_Date)
        //            return "#FFFFFFFF";
        //        else if (prestashopProduct != null && prestashopProduct.date_upd != null
        //                && prestashopProduct.date_upd.Value <= Art_Date)
        //            return "#FFFFFFFF";
        //        else
        //            return "#FFFFFFFF";
        //    }
        //}
        public void RefreshPrestashopProductData()
        {
            SendPropertyChanged("prestashopProduct");
            SendPropertyChanged("DateForeColor");
        }

        // bloque le statut "à synchroniser" si article de composition sans données
        public bool CanSync
        {
            get
            {
                return (CompositionComplete
                    //(TypeArticle == enum_TypeArticle.ArticleComposition
                    //    && this.CompositionArticle != null && this.CompositionArticle.Count > 0 && this.CompositionArticle.Count(ca => ca.CompositionArticleAttribute.Count == 0) == 0
                    //    && this.CompositionArticleAttributeGroup != null && this.CompositionArticleAttributeGroup.Count > 0
                    //    )
                    || TypeArticle != enum_TypeArticle.ArticleComposition);
            }
        }

        public bool CanSyncPrice
        {
            get
            {
                return (CanSync && Art_Sync);
            }
        }

        partial void OnArt_SyncChanged() { SendPropertyChanged("CanSyncPrice"); }

        public bool CompositionComplete
        {
            get
            {
                return (TypeArticle == enum_TypeArticle.ArticleComposition
                        && this.CompositionArticleAttributeGroup != null
                        && this.CompositionArticleAttributeGroup.Count > 0
                        && this.CompositionArticle != null
                        && this.CompositionArticle.Count > 0
                        && this.CompositionArticle.Count(ca => ca.CompositionArticleAttribute.Count == this.CompositionArticleAttributeGroup.Count) == this.CompositionArticle.Count
                        );
            }
        }

        //public Model.Internal.RedirectType redirectType
        //{
        //    get
        //    {
        //        return new Internal.RedirectType(this.Art_RedirectType);
        //    }
        //    set
        //    {
        //        this.Art_RedirectType = value._RedirectType.ToString();
        //        this.SendPropertyChanged("redirectType");
        //    }
        //}

        #region Methods

        public override string ToString()
        {
            return Art_Name;
        }

        #endregion
    }

    public class Article_Light : INotifyPropertyChanged
    {
        public int _Art_Id = 0;
        public int Art_Id
        {
            get { return _Art_Id; }
            set { _Art_Id = value; OnPropertyChanged("Art_Id"); }
        }
        public int _Sag_Id = 0;
        public int Sag_Id
        {
            get { return _Sag_Id; }
            set { _Sag_Id = value; OnPropertyChanged("Sag_Id"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Article_Progress : INotifyPropertyChanged
    {
        public int _Art_Id = 0;
        public int Art_Id
        {
            get { return _Art_Id; }
            set { _Art_Id = value; OnPropertyChanged("Art_Id"); }
        }
        public string _Art_Ref = string.Empty;
        public string Art_Ref
        {
            get { return _Art_Ref; }
            set { _Art_Ref = value; OnPropertyChanged("Art_Ref"); }
        }
        public string _Art_Name = string.Empty;
        public string Art_Name
        {
            get { return _Art_Name; }
            set { _Art_Name = value; OnPropertyChanged("Art_Name"); }
        }

        public string _Comment = string.Empty;
        public string Comment
        {
            get { return _Comment; }
            set { _Comment = (value != null) ? value : string.Empty; OnPropertyChanged("Comment"); OnPropertyChanged("StringProgress"); }
        }

        public string StringProgress
        {
            get { return (!string.IsNullOrWhiteSpace(Art_Ref) ? Art_Ref + " - " + Art_Name : Art_Name) + (!string.IsNullOrWhiteSpace(Comment) ? " : " + Comment : string.Empty); }
            set { }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
