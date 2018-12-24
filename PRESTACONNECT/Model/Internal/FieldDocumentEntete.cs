using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class FieldDocumentEntete : INotifyPropertyChanged
    {
        public static string[] FieldDocumentEnteteValuesText = { "", // 0
                                                                "Entête 1", // 1
                                                                "Entête 2", // 2
                                                                "Entête 3", // 3
                                                                "Entête 4", // 4
                                                                "Non défini", "Non défini", "Non défini", "Non défini", "Non défini"};

        #region Properties

        public Core.Parametres.FieldDocumentEntete _FieldDocumentEnteteValue;

        private string _LibellePersonalise = null;

        public int Marq
        {
            get { return (short)_FieldDocumentEnteteValue; }
        }

        public string Intitule
        {
            get
            {
                if (_LibellePersonalise == null)
                {
                    // lecture libellePersonalise
                    Model.Sage.P_PARAMETRECIAL P_PARAMETRECIAL = new Model.Sage.P_PARAMETRECIALRepository().Read();
                    if (P_PARAMETRECIAL != null)
                    {
						#if (SAGE_VERSION_16)
						switch (_FieldDocumentEnteteValue)
                        {
                            case Core.Parametres.FieldDocumentEntete.Entete1:
                            	_LibellePersonalise = "Entête 1";
                                break;
                            case Core.Parametres.FieldDocumentEntete.Entete2:
                            	_LibellePersonalise = "Entête 2";
                                break;
                            case Core.Parametres.FieldDocumentEntete.Entete3:
                            	_LibellePersonalise = "Entête 3";
                                break;
                            case Core.Parametres.FieldDocumentEntete.Entete4:
                            	_LibellePersonalise = "Entête 4";
                                break;
                        }
						#else
						switch (_FieldDocumentEnteteValue)
                        {
                            case Core.Parametres.FieldDocumentEntete.Entete1:
                                if (!string.IsNullOrWhiteSpace(P_PARAMETRECIAL.P_LibelleEntete1))
                                    _LibellePersonalise = P_PARAMETRECIAL.P_LibelleEntete1;
                                break;
                            case Core.Parametres.FieldDocumentEntete.Entete2:
                                if (!string.IsNullOrWhiteSpace(P_PARAMETRECIAL.P_LibelleEntete2))
                                    _LibellePersonalise = P_PARAMETRECIAL.P_LibelleEntete2;
                                break;
                            case Core.Parametres.FieldDocumentEntete.Entete3:
                                if (!string.IsNullOrWhiteSpace(P_PARAMETRECIAL.P_LibelleEntete3))
                                    _LibellePersonalise = P_PARAMETRECIAL.P_LibelleEntete3;
                                break;
                            case Core.Parametres.FieldDocumentEntete.Entete4:
                                if (!string.IsNullOrWhiteSpace(P_PARAMETRECIAL.P_LibelleEntete4))
                                    _LibellePersonalise = P_PARAMETRECIAL.P_LibelleEntete4;
                                break;
                        }
						#endif
                    }
                }

                if (!string.IsNullOrWhiteSpace(_LibellePersonalise))
                {
                    return _LibellePersonalise;
                }
                else
                {
                    switch (_FieldDocumentEnteteValue)
                    {
                        case Core.Parametres.FieldDocumentEntete.Entete1:
                        case Core.Parametres.FieldDocumentEntete.Entete2:
                        case Core.Parametres.FieldDocumentEntete.Entete3:
                        case Core.Parametres.FieldDocumentEntete.Entete4:
                            return FieldDocumentEnteteValuesText[Marq];
                        default:
                            return string.Empty;
                    }
                }
            }
        }

        #endregion
        #region Constructors

        public FieldDocumentEntete(Core.Parametres.FieldDocumentEntete FieldDocumentEnteteEnum)
        {
            _FieldDocumentEnteteValue = FieldDocumentEnteteEnum;
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
