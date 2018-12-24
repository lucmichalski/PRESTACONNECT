using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;

namespace PRESTACONNECT.Model.Internal
{
    public class RemiseMode : INotifyPropertyChanged
    {
        public static string[] RemiseModeText = { "Remise client",
                                                 "Remise article",
                                                 "Remise famille",
                                                 "Remise cumulée avec catégorie/famille",
                                                 "Remise en cascade avec catégorie/famille",
                                                 "Remise cumulée",
                                                 "Remise en cascade",
                                                 "Non défini", "Non défini", "Non défini", "Non défini" };

        #region Properties

        public Core.Parametres.RemiseMode _RemiseMode;

        public int Marq
        {
            get { return (short)_RemiseMode; }
        }

        public string Intitule
        {
            get
            {
                switch (_RemiseMode)
                {
                    case Core.Parametres.RemiseMode.RemiseClient:
                    case Core.Parametres.RemiseMode.RemiseArticle:
                    case Core.Parametres.RemiseMode.RemiseFamille:
                    case Core.Parametres.RemiseMode.RemiseCumulee:
                    case Core.Parametres.RemiseMode.RemiseEnCascade:
                    case Core.Parametres.RemiseMode.RemiseCumuleeCatFamille:
                    case Core.Parametres.RemiseMode.RemiseEnCascadeCatFamille:
                        return RemiseModeText[Marq];

                    default:
                        return string.Empty;
                }
            }
        }

        public Visibility ShowConflit
        {
            get
            {
                switch (_RemiseMode)
                {
                    case PRESTACONNECT.Core.Parametres.RemiseMode.RemiseClient:
                    case PRESTACONNECT.Core.Parametres.RemiseMode.RemiseArticle:
                    case PRESTACONNECT.Core.Parametres.RemiseMode.RemiseFamille:
                    default:
                        return Visibility.Hidden;

                    case PRESTACONNECT.Core.Parametres.RemiseMode.RemiseCumuleeCatFamille:
                    case PRESTACONNECT.Core.Parametres.RemiseMode.RemiseEnCascadeCatFamille:
                    case PRESTACONNECT.Core.Parametres.RemiseMode.RemiseCumulee:
                    case PRESTACONNECT.Core.Parametres.RemiseMode.RemiseEnCascade:
                        return Visibility.Visible;
                }
            }
        }

        #endregion
        #region Constructors

        public RemiseMode(Core.Parametres.RemiseMode RemiseModeEnum)
        {
            _RemiseMode = RemiseModeEnum;
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
