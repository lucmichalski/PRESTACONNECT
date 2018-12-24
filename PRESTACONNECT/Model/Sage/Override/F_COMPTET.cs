using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace PRESTACONNECT.Model.Sage
{
    public partial class F_COMPTET
    {
        public enum _Fields { ComboText };
        public string ComboText
        {
            get
            {
                return cbMarq + " - " + CT_Num + " " + CT_Intitule;
            }
        }

        public string NumIntitule
        {
            get
            {
                return CT_Num + " " + CT_Intitule;
            }
        }

        private Model.Sage.P_CRISQUE._Enum_R_Type etatControleEncours = P_CRISQUE._Enum_R_Type.Livraison;
        public Model.Sage.P_CRISQUE._Enum_R_Type EtatControleEncours
        {
            get { return this.etatControleEncours; }
            set
            {
                this.etatControleEncours = value;
                SendPropertyChanged("EtatControleEncours");
            }
        }

        public bool CanImportSupplier
        {
            get
            {
                return (Core.Temp.ListSupplier != null && Core.Temp.ListSupplier.Count(s => s.Sag_Id == cbMarq) == 0);
            }
        }

        public bool IsImportedSupplier
        {
            get
            {
                return (Core.Temp.ListSupplier != null && Core.Temp.ListSupplier.Count(s => s.Sag_Id == cbMarq) == 1);
            }
        }

        private bool checkToImport = false;
        public bool CheckToImport
        {
            get { return checkToImport; }
            set { checkToImport = value; SendPropertyChanged("CheckToImport"); }
        }

        #region Methods

        public override string ToString()
        {
            return CT_Intitule;
        }

        #endregion
    }

    public class F_COMPTET_Light
    {
        public string CT_Num = string.Empty;
        public string CT_Intitule = string.Empty;
        public short? CT_Type = 0;
        public short? N_CatTarif = 0;
        public short? N_CatCompta = 0;
        public string CT_EMail = string.Empty;
        public string CT_NumCentrale = string.Empty;
        public decimal? CT_Taux01 = 0;
        public short? CT_Sommeil = 0;
        public int cbMarq = 0;

        public override string ToString()
        {
            return CT_Num + " - " + CT_Intitule;
        }

        public string ComboText()
        {
            return cbMarq + " - " + CT_Num + " " + CT_Intitule;
        }

        public IQueryable<F_LIVRAISON> F_LIVRAISON_Principale()
        {
            return new Model.Sage.F_LIVRAISONRepository().ListComptetPrincipale(CT_Num, 1);
        }
        public IQueryable<F_LIVRAISON> F_LIVRAISON()
        {
            return new Model.Sage.F_LIVRAISONRepository().ListComptet(CT_Num);
        }
    }
    public class F_COMPTET_BtoB
    {
        public string CT_Num = string.Empty;
        public string CT_Intitule = string.Empty;
        public string CT_EMail = string.Empty;
        public int cbMarq = 0;

        public override string ToString()
        {
            return CT_Num + " - " + CT_Intitule;
        }

        public IQueryable<F_LIVRAISON> F_LIVRAISON()
        {
            return new Model.Sage.F_LIVRAISONRepository().ListComptet(CT_Num);
        }
    }
}
