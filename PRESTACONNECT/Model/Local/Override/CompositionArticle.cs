using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public partial class CompositionArticle
    {
        private bool _HasUpdated = false;
        public bool HasUpdated
        {
            get { return _HasUpdated; }
            set { _HasUpdated = value; SendPropertyChanged("HasUpdated"); }
        }

        public bool Updated
        {
            get
            {
                return _HasUpdated || ListPsAttributeGroupLang.Count(ag => ag.HasUpdated) > 0;
            }
        }

        public bool CanSyncCompositionArticle
        {
            get
            {
                return (ListPsAttributeGroupLang != null && ListPsAttributeGroupLang.Count > 0
                        && ListPsAttributeGroupLang.Count == this.CompositionArticleAttribute.Count);
            }
        }

        private Boolean isSelected;
        public Boolean IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                this.isSelected = value;
                SendPropertyChanged("IsSelected");
                SendPropertyChanged("Background");
            }
        }
        private string background = "White";
        public string Background
        {
            get
            {
                return (IsSelected) ? "DodgerBlue" : background;
            }
            set
            {
                background = value;
                SendPropertyChanged("Background");
            }
        }

        private Model.Sage.F_ARTICLE_Composition f_article_composition = null;
        public Model.Sage.F_ARTICLE_Composition F_ARTICLE_Composition
        {
            get
            {
                if (f_article_composition == null)
                {
                    f_article_composition = new Sage.F_ARTICLE_Composition();
                    if (this.ComArt_F_ARTICLE_SagId != 0)
                    {
                        Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Sage.F_ARTICLERepository();
                        if (F_ARTICLERepository.ExistArticle(this.ComArt_F_ARTICLE_SagId))
                        {
                            f_article_composition = F_ARTICLERepository.ReadComposition(this.ComArt_F_ARTICLE_SagId, this.ComArt_F_ARTENUMREF_SagId, this.ComArt_F_CONDITION_SagId);
                        }
                    }
                }
                return f_article_composition;
            }
        }
        private Model.Sage.F_ARTICLE_Light f_article_light = null;
        public Model.Sage.F_ARTICLE_Light F_ARTICLE_Light
        {
            get
            {
                if (f_article_light == null)
                {
                    f_article_light = new Sage.F_ARTICLE_Light();
                    if (this.ComArt_F_ARTICLE_SagId != 0)
                    {
                        Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Sage.F_ARTICLERepository();
                        if (F_ARTICLERepository.ExistArticle(this.ComArt_F_ARTICLE_SagId))
                        {
                            f_article_light = F_ARTICLERepository.ReadLight(this.ComArt_F_ARTICLE_SagId);
                        }
                    }
                }
                return f_article_light;
            }
        }

        private List<Model.Prestashop.PsAttributeGroupLang> listPsAttributeGroupLang;
        public List<Model.Prestashop.PsAttributeGroupLang> ListPsAttributeGroupLang
        {
            get { return listPsAttributeGroupLang; }
            set
            {
                listPsAttributeGroupLang = value;
                SendPropertyChanged("ListPsAttributeGroupLang");
            }
        }
        private Model.Prestashop.PsAttributeGroupLang selectedPsAttributeGroupLang;
        public Model.Prestashop.PsAttributeGroupLang SelectedPsAttributeGroupLang
        {
            get { return selectedPsAttributeGroupLang; }
            set
            {
                selectedPsAttributeGroupLang = value;
                SendPropertyChanged("SelectedPsAttributeGroupLang");
                SendPropertyChanged("CanEditAttribute");
            }
        }

        public bool CanEditAttribute
        {
            get
            {
                return SelectedPsAttributeGroupLang != null && SelectedPsAttributeGroupLang.IDAttributeGroup != 0;
            }
        }

        public void ReloadStringDeclinaison()
        {
            SendPropertyChanged("Declinaison");
        }

        private string declinaison = string.Empty;
        public string Declinaison
        {
            get
            {
                if (string.IsNullOrEmpty(declinaison) || (ListPsAttributeGroupLang != null && ListPsAttributeGroupLang.Count(ag => ag.HasUpdated) > 0))
                {
                    string r = string.Empty;
                    foreach (var item in ListPsAttributeGroupLang)
                    {
                        if (!string.IsNullOrEmpty(r))
                            r += " / ";
                        r += (!string.IsNullOrEmpty(item.Name)) ? item.Name : "#N/A#";
                        r += " : ";
                        r += (!string.IsNullOrEmpty(item.PsAttributeLang.Name)) ? item.PsAttributeLang.Name : "#N/A#";
                    }
                    declinaison = r;
                }
                return declinaison;
            }
        }

        public Model.Sage.F_ARTENUMREF EnumereF_ARTENUMREF
        {
            get
            {
                Model.Sage.F_ARTENUMREF r = new Sage.F_ARTENUMREF();
                if (this.ComArt_F_ARTENUMREF_SagId != null && ComArt_F_ARTENUMREF_SagId != 0)
                {
                    Model.Sage.F_ARTENUMREFRepository F_ARTENUMREFRepository = new Sage.F_ARTENUMREFRepository();
                    if (F_ARTENUMREFRepository.Exist(this.ComArt_F_ARTENUMREF_SagId.Value))
                    {
                        r = F_ARTENUMREFRepository.Read(this.ComArt_F_ARTENUMREF_SagId.Value);
                    }
                }
                return r;
            }
        }

        public Model.Sage.F_ARTGAMME EnumereGamme1
        {
            get
            {
                Model.Sage.F_ARTGAMME F_ARTGAMME1 = new Sage.F_ARTGAMME()
                {
                    AG_No = 0,
                    AG_Type = 0,
                };
                Model.Sage.F_ARTGAMMERepository F_ARTGAMMERepository = new Sage.F_ARTGAMMERepository();
                if (EnumereF_ARTENUMREF.AG_No1 != null && EnumereF_ARTENUMREF.AG_No1 != 0 && F_ARTGAMMERepository.Exist((int)EnumereF_ARTENUMREF.AG_No1, ABSTRACTION_SAGE.F_ARTGAMME.Obj._Enum_AG_Type.Gamme_1))
                {
                    F_ARTGAMME1 = F_ARTGAMMERepository.Read((int)EnumereF_ARTENUMREF.AG_No1, ABSTRACTION_SAGE.F_ARTGAMME.Obj._Enum_AG_Type.Gamme_1);
                }
                return F_ARTGAMME1;
            }
        }
        public Model.Sage.F_ARTGAMME EnumereGamme2
        {
            get
            {
                Model.Sage.F_ARTGAMME F_ARTGAMME2 = new Sage.F_ARTGAMME()
                {
                    AG_No = 0,
                    AG_Type = 1,
                };
                Model.Sage.F_ARTGAMMERepository F_ARTGAMMERepository = new Sage.F_ARTGAMMERepository();
                if (EnumereF_ARTENUMREF.AG_No2 != null && EnumereF_ARTENUMREF.AG_No2 != 0 && F_ARTGAMMERepository.Exist((int)EnumereF_ARTENUMREF.AG_No2, ABSTRACTION_SAGE.F_ARTGAMME.Obj._Enum_AG_Type.Gamme_2))
                {
                    F_ARTGAMME2 = F_ARTGAMMERepository.Read((int)EnumereF_ARTENUMREF.AG_No2, ABSTRACTION_SAGE.F_ARTGAMME.Obj._Enum_AG_Type.Gamme_2);
                }
                return F_ARTGAMME2;
            }
        }

        public Model.Sage.F_CONDITION EnumereF_CONDITION
        {
            get
            {
                Model.Sage.F_CONDITION r = new Sage.F_CONDITION();
                if (this.ComArt_F_CONDITION_SagId != null && ComArt_F_CONDITION_SagId != 0)
                {
                    Model.Sage.F_CONDITIONRepository F_CONDITIONRepository = new Sage.F_CONDITIONRepository();
                    if (F_CONDITIONRepository.ExistId(this.ComArt_F_CONDITION_SagId.Value))
                    {
                        r = F_CONDITIONRepository.ReadId(this.ComArt_F_CONDITION_SagId.Value);
                    }
                }
                return r;
            }
        }

        partial void OnComArt_ActiveChanged() { _HasUpdated = true; }
        partial void OnComArt_SyncChanged() { _HasUpdated = true; }
        partial void OnComArt_DefaultChanged() { _HasUpdated = true; }
        partial void OnComArt_QuantityChanged() { _HasUpdated = true; }

        private List<int> listImage = null;
        public List<int> ListImage
        {
            get
            {
                if (listImage == null)
                {
                    if (this.CompositionArticleImage != null)
                        listImage = this.CompositionArticleImage.Select(cai => cai.ImaArt_Id).ToList();
                    else
                        listImage = new List<int>();
                }

                return listImage;
            }
            set { listImage = value; SendPropertyChanged("ListImage"); }
        }

        #region Methods

        //public override string ToString()
        //{
        //    return EnumereF_CONDITION.EC_Enumere;
        //}

        #endregion
    }
}
