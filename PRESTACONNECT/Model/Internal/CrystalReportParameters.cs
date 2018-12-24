using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Internal
{
    public class CrystalReportParameters
    {
        public enum _Type { SelectionFormula, ParameterField }

        public _Type ParameterType;
        public string ParameterName;
        public string ParameterValue;
    }
}
