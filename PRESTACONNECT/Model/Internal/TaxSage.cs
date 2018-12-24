using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class TaxSage : INotifyPropertyChanged
    {
        public static string[] TaxSageName = { "Aucun",
                                                 "Code taxe 1",
                                                 "Code taxe 2",
                                                 "Code taxe 3",
                                                 "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini" };
        
        #region Properties

        public Core.Parametres.TaxSage _TaxSage;

        public int Marq
        {
            get { return (short)_TaxSage; }
        }

        public string Intitule
        {
            get
            {
                switch (_TaxSage)
                {
                    case Core.Parametres.TaxSage.Empty:
                    case Core.Parametres.TaxSage.CodeTaxe1:
                    case Core.Parametres.TaxSage.CodeTaxe2:
                    case Core.Parametres.TaxSage.CodeTaxe3:
                        return TaxSageName[Marq];

                    default:
                        return string.Empty;
                }
            }
        }

        #endregion
        #region Constructors

        public TaxSage(Core.Parametres.TaxSage TaxSageEnum)
        {
            _TaxSage = TaxSageEnum;
        }

        public TaxSage(String TaxSageValue)
        {
            _TaxSage = Core.Parametres.TaxSage.Empty;
            if (Core.Global.IsInteger(TaxSageValue))
            {
                int value = int.Parse(TaxSageValue);
                switch (value)
                {
                    case (int)Core.Parametres.TaxSage.CodeTaxe1:
                    case (int)Core.Parametres.TaxSage.CodeTaxe2:
                    case (int)Core.Parametres.TaxSage.CodeTaxe3:
                        _TaxSage = (Core.Parametres.TaxSage)value;
                        break;
                    default:
                        //noaction
                        break;
                }
            }
        }

        #endregion
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return Intitule;
        }

        #endregion
    }
}
