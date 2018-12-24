using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class DP_PORTEFEUILLEVENTERepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public DP_PORTEFEUILLEVENTE ReadTiers(String CT_Num, Core.Parametres.P_BaseEncours p_baseencours)
        {
            Model.Sage.DP_PORTEFEUILLEVENTE DP_PORTEFEUILLEVENTE = new DP_PORTEFEUILLEVENTE();

            string DO_Type_DOCLIGNE = string.Empty;
            int DO_Type_DOCREGL = 6;
            switch (p_baseencours)
            {
                default:
                case PRESTACONNECT.Core.Parametres.P_BaseEncours.NonDefini:
                case PRESTACONNECT.Core.Parametres.P_BaseEncours.SoldeComptable:
                    break;
                case PRESTACONNECT.Core.Parametres.P_BaseEncours.SC_FA:
                    DO_Type_DOCLIGNE = "6";
                    break;
                case PRESTACONNECT.Core.Parametres.P_BaseEncours.SC_FA_BL:
                    DO_Type_DOCLIGNE = "3, 6";
                    DO_Type_DOCREGL = 3;
                    break;
                case PRESTACONNECT.Core.Parametres.P_BaseEncours.SC_FA_BL_PL:
                    DO_Type_DOCLIGNE = "2, 3, 6";
                    DO_Type_DOCREGL = 2;
                    break;
                case PRESTACONNECT.Core.Parametres.P_BaseEncours.SC_FA_BL_PL_BC:
                    DO_Type_DOCLIGNE = "1, 2, 3, 6";
                    DO_Type_DOCREGL = 1;
                    break;
            }

            if (p_baseencours != Core.Parametres.P_BaseEncours.NonDefini && p_baseencours != Core.Parametres.P_BaseEncours.SoldeComptable)
            {
                String RequestVirtualEncours =
                    "SELECT ("
                    + "(CASE WHEN (SELECT SUM(DL_MONTANTTTC) FROM [F_DOCLIGNE] WHERE CT_Num = '" + CT_Num + "' AND DO_TYPE in (" + DO_Type_DOCLIGNE + ") AND DL_Valorise = 1) IS NULL "
                    + "THEN 0 "
                    + "ELSE (SELECT SUM(DL_MONTANTTTC) FROM [F_DOCLIGNE] WHERE CT_Num = '" + CT_Num + "' AND DO_TYPE in (" + DO_Type_DOCLIGNE + ") AND DL_Valorise = 1) END) "
                    + "- (CASE WHEN (SELECT SUM(DL_MONTANTTTC) FROM [F_DOCLIGNE] WHERE CT_Num = '" + CT_Num + "' AND DO_TYPE in (4, 5) AND DL_Valorise = 1) IS NULL "
                    + "THEN 0 "
                    + "ELSE (SELECT SUM(DL_MONTANTTTC) FROM [F_DOCLIGNE] WHERE CT_Num = '" + CT_Num + "' AND DO_TYPE in (4, 5) AND DL_Valorise = 1) END) "

                    + "- (CASE WHEN (SELECT SUM(DR_MONTANT) FROM [F_DOCREGL] WHERE DO_PIECE in "
                    + "(SELECT DO_PIECE FROM [F_DOCENTETE] WHERE DO_Tiers = '" + CT_Num + "') AND DO_Type <= 6 AND DO_Type >= " + DO_Type_DOCREGL + ") IS NULL "
                    + "THEN 0 "
                    + "ELSE (SELECT SUM(DR_MONTANT) FROM [F_DOCREGL] WHERE DO_PIECE in "
                    + "(SELECT DO_PIECE FROM [F_DOCENTETE] WHERE DO_Tiers = '" + CT_Num + "') AND DO_Type <= 6 AND DO_Type >= " + DO_Type_DOCREGL + ") END) "
                    + ") AS MontantPortefeuille";


                List<Model.Sage.DP_PORTEFEUILLEVENTE> col_encours = this.DBSage.ExecuteQuery<Model.Sage.DP_PORTEFEUILLEVENTE>(RequestVirtualEncours).ToList();

                if (col_encours != null && col_encours.Count() > 0)
                {
                    DP_PORTEFEUILLEVENTE = col_encours.First();
                }
            }
            return DP_PORTEFEUILLEVENTE;
        }

        public DP_PORTEFEUILLEVENTE ReadTiers2(String CT_Num, Core.Parametres.P_BaseEncours p_baseencours)
        {
            Model.Sage.DP_PORTEFEUILLEVENTE DP_PORTEFEUILLEVENTE = new DP_PORTEFEUILLEVENTE();

            string DO_Type_DOCLIGNE = string.Empty;
            int DO_Type_DOCREGL = 6;
            switch (p_baseencours)
            {
                default:
                case PRESTACONNECT.Core.Parametres.P_BaseEncours.NonDefini:
                case PRESTACONNECT.Core.Parametres.P_BaseEncours.SoldeComptable:
                    break;
                case PRESTACONNECT.Core.Parametres.P_BaseEncours.SC_FA:
                    DO_Type_DOCLIGNE = "6";
                    break;
                case PRESTACONNECT.Core.Parametres.P_BaseEncours.SC_FA_BL:
                    DO_Type_DOCLIGNE = "3, 6";
                    DO_Type_DOCREGL = 3;
                    break;
                case PRESTACONNECT.Core.Parametres.P_BaseEncours.SC_FA_BL_PL:
                    DO_Type_DOCLIGNE = "2, 3, 6";
                    DO_Type_DOCREGL = 2;
                    break;
                case PRESTACONNECT.Core.Parametres.P_BaseEncours.SC_FA_BL_PL_BC:
                    DO_Type_DOCLIGNE = "1, 2, 3, 6";
                    DO_Type_DOCREGL = 1;
                    break;
            }

            if (p_baseencours != Core.Parametres.P_BaseEncours.NonDefini && p_baseencours != Core.Parametres.P_BaseEncours.SoldeComptable)
            {
                String RequestVirtualEncours =
                    "SELECT ("
                    + "(CASE WHEN (SELECT count(DL_MONTANTTTC) FROM [F_DOCLIGNE] WHERE CT_Num = '" + CT_Num + "' AND DO_TYPE in (" + DO_Type_DOCLIGNE + ") AND DL_Valorise = 1) = 0 "
                    + "THEN 0 "
                    + "ELSE (SELECT SUM(DL_MONTANTTTC) FROM [F_DOCLIGNE] WHERE CT_Num = '" + CT_Num + "' AND DO_TYPE in (" + DO_Type_DOCLIGNE + ") AND DL_Valorise = 1) END) "
                    + "- (CASE WHEN (SELECT count(DL_MONTANTTTC) FROM [F_DOCLIGNE] WHERE CT_Num = '" + CT_Num + "' AND DO_TYPE in (4, 5) AND DL_Valorise = 1) = 0 "
                    + "THEN 0 "
                    + "ELSE (SELECT SUM(DL_MONTANTTTC) FROM [F_DOCLIGNE] WHERE CT_Num = '" + CT_Num + "' AND DO_TYPE in (4, 5) AND DL_Valorise = 1) END) "

                    + "- (CASE WHEN (SELECT count(DR_MONTANT) FROM [F_DOCREGL] WHERE DO_PIECE in "
                    + "(SELECT DO_PIECE FROM [F_DOCENTETE] WHERE DO_Tiers = '" + CT_Num + "') AND DO_Type <= 6 AND DO_Type >= " + DO_Type_DOCREGL + ") = 0 "
                    + "THEN 0 "
                    + "ELSE (SELECT SUM(DR_MONTANT) FROM [F_DOCREGL] WHERE DO_PIECE in "
                    + "(SELECT DO_PIECE FROM [F_DOCENTETE] WHERE DO_Tiers = '" + CT_Num + "') AND DO_Type <= 6 AND DO_Type >= " + DO_Type_DOCREGL + ") END) "
                    + ") AS MontantPortefeuille";


                List<Model.Sage.DP_PORTEFEUILLEVENTE> col_encours = this.DBSage.ExecuteQuery<Model.Sage.DP_PORTEFEUILLEVENTE>(RequestVirtualEncours).ToList();

                if (col_encours != null && col_encours.Count() > 0)
                {
                    DP_PORTEFEUILLEVENTE = col_encours.First();
                }
            }
            return DP_PORTEFEUILLEVENTE;
        }
    }
}
