using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public partial class cbSysLibre
    {
        public static string[] CB_TypeText = { 
            "Supprimée", // 0
            "Non défini", // 1
            "Non défini", // 2
            "Date", // 3
            "Non défini", "Non défini", "Non défini", // 4 - 5 - 6
            "Valeur", // 7
            "Non défini", // 8
            "Texte", // 9
            "Non défini", "Non défini", "Non défini", "Non défini", // 10 - 11 - 12 - 13
            "Date\\heure", // 14
            "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", // 15 - 16 - 17 - 18 - 19
            "Montant", // 20
            "Non défini", // 21
            "Liste", // 22
            "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini" };

        public string CB_TypeString
        {
            get
            {
                switch ((cbSysLibreRepository.CB_Type)this.CB_Type)
                {
                    case cbSysLibreRepository.CB_Type.SageDate:
                    case cbSysLibreRepository.CB_Type.SageMontant:
                    case cbSysLibreRepository.CB_Type.SageSmallDate:
                    case cbSysLibreRepository.CB_Type.SageTable:
                    case cbSysLibreRepository.CB_Type.SageText:
                    case cbSysLibreRepository.CB_Type.SageValeur:
                        return CB_TypeText[this.CB_Type];
                    case cbSysLibreRepository.CB_Type.Deleted:
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        #region Methods

        public override string ToString()
        {
            return CB_Name;
        }

        #endregion
    }
}
