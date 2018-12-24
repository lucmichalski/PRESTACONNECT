using System;

namespace PRESTACONNECT.Model.Prestashop
{
    public partial class PsSpecificPrice
    {
        public const String _ReductionType_Amount = "amount";
        public const String _ReductionType_Percentage = "percentage";

        private String _ReductionType;

        public String ReductionType
        {
            get
            {
                return this._ReductionType;
            }
            set
            {
                this._ReductionType = value;
            }
        }
    }
}
