using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class DP_ENCOURSTIERS
    {
        //private String _TI_Num = string.Empty;
        private Decimal _SoldeComptable = 0;

        //public String TI_Num
        //{
        //    get
        //    {
        //        return this._TI_Num;
        //    }
        //    set
        //    {
        //        this._TI_Num = value;
        //    }
        //}

        public Decimal SoldeComptable
        {
            get
            {
                return this._SoldeComptable;
            }
            set
            {
                this._SoldeComptable = value;
            }
        }
    }
}
