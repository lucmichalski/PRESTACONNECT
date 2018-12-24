using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class ImportSageFilterTypeSearchValue : INotifyPropertyChanged
    {
        public static string[] ImportSageFilterTypeSearchValueText = { "Intitulé contenant", // 0
                                                 "Intitulé commençant ou finissant par", // 1
                                                 "Intitulé commençant par", // 2
                                                 "Intitulé finissant par", // 3
                                                 "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", // 4-5-6-7-8-9
                                                 "Référence contenant", // 10
                                                 "Référence commençant ou finissant par", // 11
                                                 "Référence commençant par", // 12
                                                 "Référence finissant par", // 13
                                                 "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", // 14-15-16-17-18-19
                                                 "Valeur contenant", // 20
                                                 "Valeur commençant ou finissant par", // 21
                                                 "Valeur commençant par", // 22
                                                 "Valeur finissant par", // 23
                                                 "Valeur égale à", // 24
                                                 "Valeur ne contenant pas", // 25
                                                 "Valeur différente de", // 26
                                                 "Non défini", "Non défini", "Non défini", // 27-28-29
                                                                     };

        #region Properties

        public Core.Parametres.ImportSageFilterTypeSearchValue _ImportSageFilterTypeSearchValue;

        public int Marq
        {
            get { return (short)_ImportSageFilterTypeSearchValue; }
        }

        public string Intitule
        {
            get
            {
                switch (_ImportSageFilterTypeSearchValue)
                {
                    case Core.Parametres.ImportSageFilterTypeSearchValue.DesignationContains:
                    case Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginOrEndBy:
                    case Core.Parametres.ImportSageFilterTypeSearchValue.DesignationBeginBy:
                    case Core.Parametres.ImportSageFilterTypeSearchValue.DesignationEndBy:
                    case Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceContains:
                    case Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginOrEndBy:
                    case Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginBy:
                    case Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceEndBy:
                    case Core.Parametres.ImportSageFilterTypeSearchValue.ValueContains:
                    case Core.Parametres.ImportSageFilterTypeSearchValue.ValueBeginOrEndBy:
                    case Core.Parametres.ImportSageFilterTypeSearchValue.ValueBeginBy:
                    case Core.Parametres.ImportSageFilterTypeSearchValue.ValueEndBy:
                    case Core.Parametres.ImportSageFilterTypeSearchValue.ValueEquals:
                    case Core.Parametres.ImportSageFilterTypeSearchValue.ValueNotContains:
                    case Core.Parametres.ImportSageFilterTypeSearchValue.ValueNotEquals:
                        return ImportSageFilterTypeSearchValueText[Marq];
                    default:
                        return string.Empty;
                }
            }
        }

        public bool ValueUppercaseOnly
        {
            get
            {
                switch (_ImportSageFilterTypeSearchValue)
                {
                    case Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceContains:
                    case Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginOrEndBy:
                    case Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceBeginBy:
                    case Core.Parametres.ImportSageFilterTypeSearchValue.ReferenceEndBy:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public bool EnabledInfolibre
        {
            get
            {
                switch (_ImportSageFilterTypeSearchValue)
                {
                    case Core.Parametres.ImportSageFilterTypeSearchValue.ValueContains:
                    case Core.Parametres.ImportSageFilterTypeSearchValue.ValueBeginOrEndBy:
                    case Core.Parametres.ImportSageFilterTypeSearchValue.ValueBeginBy:
                    case Core.Parametres.ImportSageFilterTypeSearchValue.ValueEndBy:
                    case Core.Parametres.ImportSageFilterTypeSearchValue.ValueEquals:
                    case Core.Parametres.ImportSageFilterTypeSearchValue.ValueNotContains:
                    case Core.Parametres.ImportSageFilterTypeSearchValue.ValueNotEquals:
                        return true;
                    default:
                        return false;
                }
            }
        }

        #endregion
        #region Constructors

        public ImportSageFilterTypeSearchValue(Core.Parametres.ImportSageFilterTypeSearchValue ImportSageFilterTypeSearchValueEnum)
        {
            _ImportSageFilterTypeSearchValue = ImportSageFilterTypeSearchValueEnum;
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
