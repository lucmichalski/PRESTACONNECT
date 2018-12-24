using System;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class NumberNumComponent : INotifyPropertyChanged
    {
        public static string[] NumberNumComponentText = { "Identifiant du client sur Prestashop",
                                                 "Compteur paramétrable",
                                                 "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini" };

        #region Properties

        public Core.Parametres.NumberNumComponent _NumberNumComponent;

        public int Marq
        {
            get { return (short)_NumberNumComponent; }
        }

        public string Intitule
        {
            get
            {
                switch (_NumberNumComponent)
                {
                    case Core.Parametres.NumberNumComponent.IdPrestashop:
                    case Core.Parametres.NumberNumComponent.Counter:
                        return NumberNumComponentText[Marq];
                    default:
                        return string.Empty;
                }
            }
        }

        public Boolean CanDefineCounter
        {
            get
            {
                switch (_NumberNumComponent)
                {
                    case Core.Parametres.NumberNumComponent.IdPrestashop:
                    default:
                        return false;

                    case Core.Parametres.NumberNumComponent.Counter:
                        return true;
                }
            }
        }

        #endregion
        #region Constructors

        public NumberNumComponent(Core.Parametres.NumberNumComponent NumberNumComponentEnum)
        {
            _NumberNumComponent = NumberNumComponentEnum;
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
