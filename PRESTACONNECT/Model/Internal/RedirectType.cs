using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class RedirectType : INotifyPropertyChanged
    {
        public static string[] RedirectTypeName = { "Pas de redirection",
                                                 "Redirection permanente",
                                                 "Redirection temporaire",
                                                 "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini" };
        public static string[] RedirectTypeDescription = { "Not found : le produit n'existe pas et il n'y a pas de redirection",
                                                 "Moved permanently : la page produit redirige définitivement vers une autre page",
                                                 "Moved temporarily : la page produit redirige temporairement vers une autre page",
                                                 "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini" };
        public static string[] RedirectPage = { "404",
                                                 "301",
                                                 "302",
                                                 "", "", "", "", "", "", "" };

        #region Properties

        public Core.Parametres.RedirectType _RedirectType;

        public int Marq
        {
            get { return (short)_RedirectType; }
        }

        public string Intitule
        {
            get
            {
                switch (_RedirectType)
                {
                    case Core.Parametres.RedirectType.NoRedirect404:
                    case Core.Parametres.RedirectType.RedirectPermanently301:
                    case Core.Parametres.RedirectType.RedirectTemporarily302:
                        return RedirectTypeName[Marq] + " (" + Page + ")";

                    default:
                        return string.Empty;
                }
            }
        }
        public string Description
        {
            get
            {
                switch (_RedirectType)
                {
                    case Core.Parametres.RedirectType.NoRedirect404:
                    case Core.Parametres.RedirectType.RedirectPermanently301:
                    case Core.Parametres.RedirectType.RedirectTemporarily302:
                        return Page + " " + RedirectTypeDescription[Marq];

                    default:
                        return string.Empty;
                }
            }
        }

        public string Page
        {
            get
            {
                switch (_RedirectType)
                {
                    case Core.Parametres.RedirectType.NoRedirect404:
                    case Core.Parametres.RedirectType.RedirectPermanently301:
                    case Core.Parametres.RedirectType.RedirectTemporarily302:
                        return RedirectPage[Marq];

                    default:
                        return string.Empty;
                }
            }
        }
        
        public Boolean CanDefineRedirectProduct
        {
            get
            {
                switch (_RedirectType)
                {
                    case Core.Parametres.RedirectType.NoRedirect404:
                    default:
                        return false;

                    case Core.Parametres.RedirectType.RedirectPermanently301:
                    case Core.Parametres.RedirectType.RedirectTemporarily302:
                        return true;
                }
            }
        }

        #endregion
        #region Constructors

        public RedirectType(Core.Parametres.RedirectType RedirectTypeEnum)
        {
            _RedirectType = RedirectTypeEnum;
        }

        public RedirectType(String RedirectTypeValue)
        {
            _RedirectType = Core.Parametres.RedirectType.NoRedirect404;
            if (Core.Global.IsInteger(RedirectTypeValue))
            {
                int value = int.Parse(RedirectTypeValue);
                switch (value)
                {
                    case (int)Core.Parametres.RedirectType.NoRedirect404:
                    case (int)Core.Parametres.RedirectType.RedirectPermanently301:
                    case (int)Core.Parametres.RedirectType.RedirectTemporarily302:
                        _RedirectType = (Core.Parametres.RedirectType)value;
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
