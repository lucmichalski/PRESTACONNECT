using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Prestashop
{
    partial class PsAEcStock
    {
        private int count_Supply;
        public int Count_Supply
        {
            get { return count_Supply; }
            set
            {
                count_Supply = value;
                SendPropertyChanged("Count_Supply");
            }
        }
    }
}
