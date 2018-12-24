using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class DP_PORTEFEUILLEVENTE
    {
        //private String _CLI_Num = string.Empty;
        private Decimal _MontantPortefeuille = 0;

        //public String CLI_Num
        //{
        //    get
        //    {
        //        return this._CLI_Num;
        //    }
        //    set
        //    {
        //        this._CLI_Num = value;
        //    }
        //}

        public Decimal MontantPortefeuille
        {
            get
            {
                return this._MontantPortefeuille;
            }
            set
            {
                this._MontantPortefeuille = value;
            }
        }
    }
}
