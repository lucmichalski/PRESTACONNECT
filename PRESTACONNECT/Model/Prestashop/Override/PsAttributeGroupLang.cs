using System;
using System.Collections.Generic;
using System.Linq;

namespace PRESTACONNECT.Model.Prestashop
{
    partial class PsAttributeGroupLang
    {
        private bool _HasUpdated = false;
        public bool HasUpdated
        {
            get { return _HasUpdated; }
            set { _HasUpdated = value; SendPropertyChanged("HasUpdated"); }
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

        private int _IDArticle = 0;
        public int IDArticle
        {
            get { return _IDArticle; }
            set { _IDArticle = value; SendPropertyChanged("IDArticle"); }
        }
        private int _IDCompositionArticle = 0;
        public int IDCompositionArticle
        {
            get { return _IDCompositionArticle; }
            set { _IDCompositionArticle = value; SendPropertyChanged("IDCompositionArticle"); }
        }

        private string filterPsAttributeLang = string.Empty;
        public string FilterPsAttributeLang
        {
            get { return filterPsAttributeLang; }
            set
            {
                filterPsAttributeLang = value;
                if (string.IsNullOrEmpty(filterPsAttributeLang))
                    FilterAttributeValue();
                SendPropertyChanged("FilterPsAttributeLang");
                SendPropertyChanged("ListPsAttributeLang");
            }
        }

        public void FilterAttributeValue()
        {
            if (!string.IsNullOrEmpty(FilterPsAttributeLang))
            {
                //listPsAttributeLang = _tempListPsAttributeLang.Where(fv => fv.Name.Contains(FilterPsAttributeLang));
                //if(listPsAttributeLang.Count() == 0)
                    listPsAttributeLang = _tempListPsAttributeLang.Where(fv => fv.Name.ToLower().Contains(FilterPsAttributeLang.ToLower()));
            }
            else
                listPsAttributeLang = _tempListPsAttributeLang;

            SendPropertyChanged("ListPsAttributeLang");
        }

        private IEnumerable<PsAttributeLang> _tempListPsAttributeLang;
        private IEnumerable<PsAttributeLang> listPsAttributeLang;
        public IEnumerable<PsAttributeLang> ListPsAttributeLang
        {
            get
            {
                if (Core.Temp.PsAttributeLangRepository == null)
                {
                    Core.Temp.PsAttributeLangRepository = new Model.Prestashop.PsAttributeLangRepository();
                }
                if (_tempListPsAttributeLang == null)
                {
                    _tempListPsAttributeLang = Core.Temp.PsAttributeLangRepository.ListAttributeLang(IDAttributeGroup, Core.Global.Lang);
                    listPsAttributeLang = _tempListPsAttributeLang;
                }

                return listPsAttributeLang;
            }
            set
            {
                ListPsAttributeLang = value;
                SendPropertyChanged("ListPsAttributeLang");
            }
        }

        private Model.Local.CompositionArticleAttribute compositionArticleAttribute = null;
        public Model.Local.CompositionArticleAttribute CompositionArticleAttribute
        {
            get
            {
                if (compositionArticleAttribute == null)
                {
                    if (this.IDArticle != 0 && this.IDCompositionArticle != 0)
                    {
                        Model.Local.CompositionArticleAttributeRepository CompositionArticleAttributeRepository = new Local.CompositionArticleAttributeRepository();
                        if (CompositionArticleAttributeRepository.ExistAttributeGroupCompositionArticle((int)IDAttributeGroup, this.IDCompositionArticle))
                        {
                            compositionArticleAttribute = CompositionArticleAttributeRepository.ReadAttributeGroupCompositionArticle((int)IDAttributeGroup, this.IDCompositionArticle);
                        }
                    }
                }
                return compositionArticleAttribute;
            }
            set
            {
                compositionArticleAttribute = value;
                SendPropertyChanged("CompositionArticleAttribute");
            }
        }

        private Model.Prestashop.PsAttributeLang psAttributeLang;
        public Model.Prestashop.PsAttributeLang PsAttributeLang
        {
            get
            {
                if (Core.Temp.PsAttributeLangRepository == null)
                {
                    Core.Temp.PsAttributeLangRepository = new Model.Prestashop.PsAttributeLangRepository();
                }
                if (psAttributeLang == null)
                {
                    if (CompositionArticleAttribute != null && CompositionArticleAttribute.Caa_Attribute_PreId != 0
                        && Core.Temp.PsAttributeLangRepository.ExistAttributeLang((uint)CompositionArticleAttribute.Caa_Attribute_PreId, Core.Global.Lang))
                    {
                        psAttributeLang = Core.Temp.PsAttributeLangRepository.ReadAttributeLang((uint)CompositionArticleAttribute.Caa_Attribute_PreId, Core.Global.Lang);
                    }
                    else
                    {
                        psAttributeLang = new PsAttributeLang();
                    }
                }
                return psAttributeLang;
            }
            set
            {
                psAttributeLang = value;
                HasUpdated = true;
                compositionArticleAttribute = (value != null)
                    ? new Local.CompositionArticleAttribute()
                        {
                            Caa_AttributeGroup_PreId = (int)IDAttributeGroup,
                            Caa_Attribute_PreId = (int)value.IDAttribute,
                            Caa_ComArtId = this.IDCompositionArticle,
                        }
                    : null;
                SendPropertyChanged("PsAttributeLang");
            }
        }
    }
}
