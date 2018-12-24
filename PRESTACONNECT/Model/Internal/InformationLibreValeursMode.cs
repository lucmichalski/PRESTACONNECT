using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class InformationLibreValeursMode : INotifyPropertyChanged
    {
        public static string[] InformationLibreValeursModeText = { "Non transférées vers Prestashop",
                                                 "Transférées en tant que valeurs prédéfinies",
                                                 "Transférées en tant que valeurs personnalisées",
                                                 "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini" };

        #region Properties

        public Core.Parametres.InformationLibreValeursMode _InformationLibreValeursMode;

        public int Marq
        {
            get { return (short)_InformationLibreValeursMode; }
        }

        public string Intitule
        {
            get
            {
                switch (_InformationLibreValeursMode)
                {
                    case Core.Parametres.InformationLibreValeursMode.NonTransferees:
                    case Core.Parametres.InformationLibreValeursMode.Predefinies:
                    case Core.Parametres.InformationLibreValeursMode.Personnalisees:
                        return InformationLibreValeursModeText[Marq];
                    default:
                        return string.Empty;
                }
            }
        }

        #endregion
        #region Constructors

        public InformationLibreValeursMode(Core.Parametres.InformationLibreValeursMode InformationLibreValeursModeEnum)
        {
            _InformationLibreValeursMode = InformationLibreValeursModeEnum;
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
