using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class RemiseConflit : INotifyPropertyChanged
    {
        public static string[] RemiseConflitText = { "Cumul ou Cascade",
                                                     "Meilleure remise uniquement",
                                                     "Non défini", "Non défini", "Non défini", "Non défini", 
                                                     "Non défini", "Non défini", "Non défini", "Non défini",
                                                     "Priorité Article / Famille / Client",
                                                     "Non défini", 
                                                     "Non défini", "Non défini", "Non défini", "Non défini", 
                                                     "Non défini", "Non défini", "Non défini", "Non défini" };

        #region Properties

        public Core.Parametres.RemiseConflit _RemiseConflit;

        public int Marq
        {
            get { return (short)_RemiseConflit; }
        }

        public string Intitule
        {
            get
            {
                switch (_RemiseConflit)
                {
                    case Core.Parametres.RemiseConflit.CumulCascade:
                    case Core.Parametres.RemiseConflit.MeilleureRemise:
                    case Core.Parametres.RemiseConflit.PrioriteArticleFamilleClient:
                        return RemiseConflitText[Marq];
                    default:
                        return string.Empty;
                }
            }
        }

        #endregion
        #region Constructors

        public RemiseConflit(Core.Parametres.RemiseConflit RemiseConflitEnum)
        {
            _RemiseConflit = RemiseConflitEnum;
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
