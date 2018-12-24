using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class DP_ENCOURSTIERSRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public DP_ENCOURSTIERS ReadTiers(String CT_Num, DateTime DebutExo, DateTime FinExo)
        {
            string DebutExoStringSql = "'" + DebutExo.ToString("dd/MM/yyyy") + "'";
            string FinExoStringSql = "'" + FinExo.ToString("dd/MM/yyyy") + "'";
            
            Model.Sage.DP_ENCOURSTIERS DP_ENCOURSTIERS = new DP_ENCOURSTIERS();
            String RequestVirtualEncours =
                "SELECT ("
                // calcul écritures comptables débit
                + "(CASE WHEN (SELECT SUM([EC_Montant]) FROM [F_ECRITUREC] "
                + "WHERE CT_NUM = '" + CT_Num + "' "
                + "AND EC_SENS = 0 "
                + "AND JM_DATE >= " + DebutExoStringSql + " "
                + "AND JM_DATE <= " + FinExoStringSql + ") IS NULL "
                + "THEN 0 "
                + "ELSE (SELECT SUM([EC_Montant]) FROM [F_ECRITUREC] "
                + "WHERE CT_NUM = '" + CT_Num + "' "
                + "AND EC_SENS = 0 "
                + "AND JM_DATE >= " + DebutExoStringSql + " "
                + "AND JM_DATE <= " + FinExoStringSql + ") "
                + "END) "
                // calcul écritures comptables crédit
                + "- (CASE WHEN (SELECT SUM([EC_Montant]) FROM [F_ECRITUREC] "
                + "WHERE CT_NUM = '" + CT_Num + "' "
                + "AND EC_SENS = 1 "
                + "AND JM_DATE >= " + DebutExoStringSql + " "
                + "AND JM_DATE <= " + FinExoStringSql + ") IS NULL "
                + "THEN 0 "
                + "ELSE (SELECT SUM([EC_Montant]) FROM [F_ECRITUREC] "
                + "WHERE CT_NUM = '" + CT_Num + "' "
                + "AND EC_SENS = 1 "
                + "AND JM_DATE >= " + DebutExoStringSql + " "
                + "AND JM_DATE <= " + FinExoStringSql + ") "
                + "END) "
                + ") AS SoldeComptable";
            
            List<Model.Sage.DP_ENCOURSTIERS> col_encours = this.DBSage.ExecuteQuery<Model.Sage.DP_ENCOURSTIERS>(RequestVirtualEncours).ToList();

            if (col_encours != null && col_encours.Count() > 0)
            {
                DP_ENCOURSTIERS = col_encours.First();
            }
            return DP_ENCOURSTIERS;
        }


        public DP_ENCOURSTIERS ReadTiers2(String CT_Num, DateTime DebutExo, DateTime FinExo)
        {
            string DebutExoStringSql = "'" + DebutExo.ToString("dd/MM/yyyy") + "'";
            string FinExoStringSql = "'" + FinExo.ToString("dd/MM/yyyy") + "'";

            Model.Sage.DP_ENCOURSTIERS DP_ENCOURSTIERS = new DP_ENCOURSTIERS();
            String RequestVirtualEncours =
                "SELECT ("
                // calcul écritures comptables débit
                + "(CASE WHEN ((SELECT count([EC_Montant]) FROM [F_ECRITUREC] "
                + "WHERE CT_NUM = '" + CT_Num + "' "
                + "AND EC_SENS = 0 "
                + "AND JM_DATE >= " + DebutExoStringSql + " "
                + "AND JM_DATE <= " + FinExoStringSql + ") = 0) "
                + "THEN 0 "
                + "ELSE (SELECT SUM([EC_Montant]) FROM [F_ECRITUREC] "
                + "WHERE CT_NUM = '" + CT_Num + "' "
                + "AND EC_SENS = 0 "
                + "AND JM_DATE >= " + DebutExoStringSql + " "
                + "AND JM_DATE <= " + FinExoStringSql + ") "
                + "END) "
                // calcul écritures comptables crédit
                + "- (CASE WHEN ((SELECT count([EC_Montant]) FROM [F_ECRITUREC] "
                + "WHERE CT_NUM = '" + CT_Num + "' "
                + "AND EC_SENS = 1 "
                + "AND JM_DATE >= " + DebutExoStringSql + " "
                + "AND JM_DATE <= " + FinExoStringSql + ") = 0) "
                + "THEN 0 "
                + "ELSE (SELECT SUM([EC_Montant]) FROM [F_ECRITUREC] "
                + "WHERE CT_NUM = '" + CT_Num + "' "
                + "AND EC_SENS = 1 "
                + "AND JM_DATE >= " + DebutExoStringSql + " "
                + "AND JM_DATE <= " + FinExoStringSql + ") "
                + "END) "
                + ") AS SoldeComptable";

            List<Model.Sage.DP_ENCOURSTIERS> col_encours = this.DBSage.ExecuteQuery<Model.Sage.DP_ENCOURSTIERS>(RequestVirtualEncours).ToList();

            if (col_encours != null && col_encours.Count() > 0)
            {
                DP_ENCOURSTIERS = col_encours.First();
            }
            return DP_ENCOURSTIERS;
        }
    }
}
