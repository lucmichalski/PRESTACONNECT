using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public partial class AttributeArticle
    {
        public bool _HasUpdated = false;

        public Model.Sage.F_ARTENUMREF EnumereF_ARTENUMREF
        {
            get
            {
                Model.Sage.F_ARTENUMREF r = new Sage.F_ARTENUMREF();
                if (this.Sag_Id != 0)
                {
                    Model.Sage.F_ARTENUMREFRepository F_ARTENUMREFRepository = new Sage.F_ARTENUMREFRepository();
                    if (F_ARTENUMREFRepository.Exist(this.Sag_Id))
                    {
                        r = F_ARTENUMREFRepository.Read(this.Sag_Id);
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


        private Model.Prestashop.PsAttributeLang psattributelang1 = null;
        public Model.Prestashop.PsAttributeLang AttributeLang1
        {
            get
            {
                if (psattributelang1 == null)
                {
                    psattributelang1 = new Prestashop.PsAttributeLang() { Name = "Non trouvé / Erreur" };
                    if (this.Att_IdFirst != 0 && Core.Temp.ListPsAttributeLang.Count(a => a.IDAttribute == this.Att_IdFirst) > 0)
                    {
                        psattributelang1 = Core.Temp.ListPsAttributeLang.FirstOrDefault(a => a.IDAttribute == this.Att_IdFirst);
                    }
                }
                return psattributelang1;
            }
        }
        private Model.Prestashop.PsAttributeLang psattributelang2 = null;
        public Model.Prestashop.PsAttributeLang AttributeLang2
        {
            get
            {
                if (psattributelang2 == null)
                {
                    psattributelang2 = new Prestashop.PsAttributeLang() { Name = "Non trouvé / Erreur" };
                    if (this.Att_IdSecond != 0 && Core.Temp.ListPsAttributeLang.Count(a => a.IDAttribute == this.Att_IdSecond) > 0)
                    {
                        psattributelang2 = Core.Temp.ListPsAttributeLang.FirstOrDefault(a => a.IDAttribute == this.Att_IdSecond);
                    }
                }
                return psattributelang2;
            }
        }

        partial void OnAttArt_SyncChanged() { _HasUpdated = true; }
        partial void OnAttArt_DefaultChanged() { _HasUpdated = true; }
        partial void OnAttArt_ActiveChanged() { _HasUpdated = true; }

        private List<int> listImage = null;
        public List<int> ListImage
        {
            get
            {
                if (listImage == null)
                {
                    if (this.AttributeArticleImage != null)
                        listImage = this.AttributeArticleImage.Select(ati => ati.ImaArt_Id).ToList();
                    else
                        listImage = new List<int>();
                }     

                return listImage;
            }
            set { listImage = value; SendPropertyChanged("ListImage"); }
        }

        #region Methods

        public override string ToString()
        {
            return EnumereGamme1 + " " + EnumereGamme2;
        }

        #endregion
    }
}
