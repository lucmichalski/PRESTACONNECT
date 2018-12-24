using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Prestashop
{
    public partial class PsEmployee
    {
        #region Methods

        public string EmployeeName
        {
            get { return FirstName + " " + LastName; }
        }


        public override string ToString()
        {
            return FirstName + " " + LastName;
        }

        #endregion
    }
}
