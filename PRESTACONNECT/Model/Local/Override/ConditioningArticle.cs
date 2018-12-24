using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public partial class ConditioningArticle
    {
        public bool _HasUpdated = false;

        public Model.Sage.F_CONDITION EnumereF_CONDITION
        {
            get
            {
                Model.Sage.F_CONDITION r = new Sage.F_CONDITION();
                if (this.Sag_Id != 0)
                {
                    Model.Sage.F_CONDITIONRepository F_CONDITIONRepository = new Sage.F_CONDITIONRepository();
                    if (F_CONDITIONRepository.ExistId(this.Sag_Id))
                    {
                        r = F_CONDITIONRepository.ReadId(this.Sag_Id);
                    }
                }
                return r;
            }
        }

        //public Model.Prestashop.PsAttributeLang EnumerePsAttributeLang
        //{
        //    get
        //    {
        //        Model.Prestashop.PsAttributeLang r = new Prestashop.PsAttributeLang();
        //        if (this.Con_Id != 0)
        //        {
        //            Model.Prestashop.PsAttributeLangRepository PsAttributeLangRepository = new Prestashop.PsAttributeLangRepository();
        //            if (PsAttributeLangRepository.ExistAttributeLang((uint)this.Con_Id, Core.Global.Lang))
        //            {
        //                r = PsAttributeLangRepository.ReadAttributeLang((uint)this.Con_Id, Core.Global.Lang);
        //            }
        //        }
        //        return r;
        //    }
        //}
        public Model.Prestashop.PsAttributeLang AttributeLang
        {
            get
            {
                Model.Prestashop.PsAttributeLang PsAttributeLang = new Prestashop.PsAttributeLang();
                Model.Prestashop.PsAttributeLangRepository PsAttributeLangRepository = new Prestashop.PsAttributeLangRepository();
                if (PsAttributeLangRepository.ExistAttributeLang((uint)this.Con_Id, Core.Global.Lang))
                    PsAttributeLang = PsAttributeLangRepository.ReadAttributeLang((uint)this.Con_Id, Core.Global.Lang);
                return PsAttributeLang;
            }
        }

        private Model.Sage.F_CONDITION f_conditionold = null;
        public Model.Sage.F_CONDITION EnumereF_CONDITIONSageOld
        {
            get
            {
                if (f_conditionold == null)
                {
                    f_conditionold = new Sage.F_CONDITION() { EC_Enumere = "Non trouvé" };
                    if (this.Sag_Id != 0 && Core.Temp.ListF_CONDITION.Count(a => a.cbMarq == this.Sag_Id) > 0)
                    {
                        f_conditionold = Core.Temp.ListF_CONDITION.FirstOrDefault(a => a.cbMarq == this.Sag_Id);
                    }
                }
                return f_conditionold;
            }
        }
        public string Lien
        {
            get
            {
                return (EnumereF_CONDITIONSageOld.CO_No.HasValue && EnumereF_CONDITIONSageOld.CO_No.Value != 0
                    && PsAttributeLang.IDAttribute != 0) ? "O" : "N";
            }
        }

        private Model.Prestashop.PsAttributeLang psattributelang = null;
        public Model.Prestashop.PsAttributeLang PsAttributeLang
        {
            get
            {
                if (psattributelang == null)
                {
                    psattributelang = new Prestashop.PsAttributeLang() { Name = "Non trouvée" };
                    if (this.Con_Id != 0 && Core.Temp.ListPsAttributeLang.Count(a => a.IDAttribute == this.Con_Id) > 0)
                    {
                        psattributelang = Core.Temp.ListPsAttributeLang.FirstOrDefault(a => a.IDAttribute == this.Con_Id);
                    }
                }
                return psattributelang;
            }
        }

        private Model.Sage.F_CONDITION f_conditionnew = null;
        public Model.Sage.F_CONDITION EnumereF_CONDITIONSageNew
        {
            get
            {
                f_conditionnew = new Sage.F_CONDITION() { EC_Enumere = "/" };
                if (CanReplace)
                {
                    f_conditionnew.EC_Enumere = "Non trouvé";
                    if (Core.Temp.ListF_CONDITION.Count(a => a.AR_Ref == this.Article.Art_Ref && a.EC_Enumere == PsAttributeLang.Name) > 0)
                    {
                        f_conditionnew = Core.Temp.ListF_CONDITION.FirstOrDefault(a => a.AR_Ref == this.Article.Art_Ref && a.EC_Enumere == PsAttributeLang.Name);
                    }
                }
                return f_conditionnew;
            }
        }
        private bool? canReplace = null;
        public bool CanReplace
        {
            get
            {
                if (canReplace == null)
                {
                    canReplace = ((!EnumereF_CONDITIONSageOld.CO_No.HasValue || EnumereF_CONDITIONSageOld.CO_No.Value == 0)
                    && PsAttributeLang.IDAttribute != 0) ? true : false;
                }
                return canReplace.Value;
            }
        }
        private string intituleconditionnement = string.Empty;
        public string IntituleConditionnement
        {
            get
            {
                if (string.IsNullOrEmpty(intituleconditionnement) && Core.Temp.ListP_CONDITIONNEMENT.Count(c => c.cbMarq == this.Article.sageArticleLight.AR_Condition.Value) > 0)
                {
                    intituleconditionnement = Core.Temp.ListP_CONDITIONNEMENT.FirstOrDefault(c => c.cbMarq == this.Article.sageArticleLight.AR_Condition.Value).P_Conditionnement;
                }
                return intituleconditionnement;
            }
        }

        private Boolean replace = false;
        public Boolean Replace
        {
            get { return replace; }
            set { replace = value; SendPropertyChanged("Replace"); }
        }

        partial void OnConArt_SyncChanged() { _HasUpdated = true; }
        partial void OnConArt_DefaultChanged() { _HasUpdated = true; }

        #region Methods

        public override string ToString()
        {
            return EnumereF_CONDITION.EC_Enumere;
        }

        #endregion
    }
}

