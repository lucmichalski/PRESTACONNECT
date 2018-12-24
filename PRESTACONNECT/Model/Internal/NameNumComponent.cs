using System;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class NameNumComponent : INotifyPropertyChanged
    {
        public static string[] NameNumComponentText = { "Nom du client uniquement",
                                                 "Nom de la société si renseigné sinon nom du client",
                                                 "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini" };

        #region Properties

        public Core.Parametres.NameNumComponent _NameNumComponent;

        public int Marq
        {
            get { return (short)_NameNumComponent; }
        }

        public string Intitule
        {
            get
            {
                switch (_NameNumComponent)
                {
                    case Core.Parametres.NameNumComponent.NameOnly:
                    case Core.Parametres.NameNumComponent.CompanyBeforeName:
                        return NameNumComponentText[Marq];
                    default:
                        return string.Empty;
                }
            }
        }

        #endregion
        #region Constructors

        public NameNumComponent(Core.Parametres.NameNumComponent NameNumComponentEnum)
        {
            _NameNumComponent = NameNumComponentEnum;
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
