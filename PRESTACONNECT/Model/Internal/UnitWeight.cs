using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class UnitWeight : INotifyPropertyChanged
    {
        public static string[] UnitWeightText = { "Tonne",
                                                 "Quintal",
                                                 "Kilogramme",
                                                 "Gramme",
                                                 "Milligramme",
                                                 "Non défini", "Non défini", "Non défini", "Non défini", "Non défini" };

        #region Properties

        public Core.Parametres.UnitWeight _UnitWeight;

        public int Marq
        {
            get { return (short)_UnitWeight; }
        }

        public string Intitule
        {
            get
            {
                switch (_UnitWeight)
                {
                    case Core.Parametres.UnitWeight.Tonne:
                    case Core.Parametres.UnitWeight.Quintal:
                    case Core.Parametres.UnitWeight.Kilogramme:
                    case Core.Parametres.UnitWeight.Gramme:
                    case Core.Parametres.UnitWeight.Milligramme:
                        return UnitWeightText[Marq];
                    default:
                        return string.Empty;
                }
            }
        }

        #endregion
        #region Constructors

        public UnitWeight(Core.Parametres.UnitWeight UnitWeightEnum)
        {
            _UnitWeight = UnitWeightEnum;
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
