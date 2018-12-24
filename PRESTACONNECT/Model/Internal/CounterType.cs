using System;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class CounterType : INotifyPropertyChanged
    {
        public static string[] CounterTypeText = { "Incrémentiel",
                                                 "Décrémentiel",
                                                 "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini" };

        #region Properties

        public Core.Parametres.CounterType _CounterType;

        public int Marq
        {
            get { return (short)_CounterType; }
        }

        public string Intitule
        {
            get
            {
                switch (_CounterType)
                {
                    case Core.Parametres.CounterType.Incremental:
                    case Core.Parametres.CounterType.Decremental:
                        return CounterTypeText[Marq];
                    default:
                        return string.Empty;
                }
            }
        }

        #endregion
        #region Constructors

        public CounterType(Core.Parametres.CounterType CounterTypeEnum)
        {
            _CounterType = CounterTypeEnum;
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
