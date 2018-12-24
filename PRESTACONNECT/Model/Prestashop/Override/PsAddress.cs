using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Prestashop
{
    public partial class PsAddress
    {
        #region Methods

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }

        #endregion
    }

    public class PsAddress_Light
    {
        public uint id_address = 0;
        public uint id_customer = 0;
    }

    public class idaddress
    {
        public uint id_address = 0;
    }
}
