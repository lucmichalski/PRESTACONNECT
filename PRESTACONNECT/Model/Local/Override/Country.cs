using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Local
{
    public partial class Country
    {
        #region Properties

        private Model.Prestashop.PsCountryLang psCountryLang = null;
        public Model.Prestashop.PsCountryLang PsCountryLang
        {
            get
            {
                if (psCountryLang == null)
                    psCountryLang = Core.Temp.ListPsCountryLang.FirstOrDefault(cl => cl.IDCountry == Pre_IdCountry);
                if (psCountryLang == null)
                {
                    Model.Prestashop.PsCountryLangRepository PsCountryLangRepository = new Prestashop.PsCountryLangRepository();
                    if(PsCountryLangRepository.ExistCountryLang((uint)Pre_IdCountry, (uint)Core.Global.Lang))
                        psCountryLang = PsCountryLangRepository.ReadCountryLang((uint)Pre_IdCountry, (uint)Core.Global.Lang);
                    else
                        psCountryLang = new Prestashop.PsCountryLang();
                }
                return psCountryLang;
            }
        }

        public Model.Internal.CategorieComptable Sage_CatCompta
        {
            get { return Core.Temp.ListCategorieComptable.FirstOrDefault(cc => cc.SageMarq == Sag_CatCompta); }
            set
            {
                if (value != null)
                    Sag_CatCompta = value.SageMarq;
                else
                    Sag_CatCompta = 0;
                OnPropertyChanged("Sage_CatCompta");
            }
        }

        public Model.Internal.CategorieComptable Sage_CatComptaPro
        {
            get { return Core.Temp.ListCategorieComptable.FirstOrDefault(cc => cc.SageMarq == Sag_CatComptaPro); }
            set
            {
                if (value != null)
                    Sag_CatComptaPro = value.SageMarq;
                else
                    Sag_CatComptaPro = 0;
                OnPropertyChanged("Sage_CatComptaPro");
            }
        }

        #endregion

        #region Events

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return (PsCountryLang != null) ? PsCountryLang.Name : string.Empty;
        }

        #endregion
    }
}
