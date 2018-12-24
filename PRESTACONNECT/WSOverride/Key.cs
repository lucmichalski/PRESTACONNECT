using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.WSKEY
{
    public partial class Key : INotifyPropertyChanged
    {
        public bool ExtranetOnly
        {
            get
            {
                return !this.Option1 && !this.Option2 && this.Option3;
            }
        }
    }
}
