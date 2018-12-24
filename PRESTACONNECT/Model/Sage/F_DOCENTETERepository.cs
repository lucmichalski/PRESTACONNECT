using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_DOCENTETERepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public static string _fields_light = " DO_Domaine, DO_Type, DO_Piece, DO_Date, DO_Tiers ";

        public List<F_DOCENTETE_Light> ListLight()
        {
            return DBSage.ExecuteQuery<F_DOCENTETE_Light>("SELECT " + _fields_light + " FROM F_DOCENTETE").ToList();
        }
        public List<F_DOCENTETE_Light> ListLight(short Domaine, short Type, int top = 0, bool orderbydatedesc = false, string filtreclient = null)
        {
            return DBSage.ExecuteQuery<F_DOCENTETE_Light>("SELECT " + ((top != 0) ? "TOP (" + top + ") " : string.Empty)
                + _fields_light
                + " FROM F_DOCENTETE WHERE DO_Domaine = " + Domaine
                + " AND DO_Type = " + Type
                + (!string.IsNullOrWhiteSpace(filtreclient) ? " AND DO_Tiers like '" + Core.Global.EscapeSqlString(filtreclient) + "' " : string.Empty)
                + (orderbydatedesc ? " ORDER BY DO_Date desc " : string.Empty)
                ).ToList();
        }

        public Boolean ExistDomaineTypePiece(short Domaine, short Type, String Piece)
        {
            if (this.DBSage.F_DOCENTETE.Count(Obj => Obj.DO_Domaine == Domaine && Obj.DO_Type == Type && Obj.DO_Piece.ToUpper() == Piece.ToUpper()) == 0)
                return false;
            else
                return true;
        }
        public F_DOCENTETE ReadDomaineTypePiece(short Domaine, short Type, String Piece)
        {
            return this.DBSage.F_DOCENTETE.FirstOrDefault(Obj => Obj.DO_Domaine == Domaine && Obj.DO_Type == Type && Obj.DO_Piece.ToUpper() == Piece.ToUpper());
        }

        public Boolean ExistWeb(String Web)
        {
            List<F_DOCENTETE> Return = (from Table in this.DBSage.F_DOCENTETE
                                        where Table.DO_NoWeb.ToUpper() == Web.ToUpper()
                                        orderby Table.cbMarq descending
                                        select Table).ToList();
            if (Core.Global.GetConfig().ConfigCommandeFiltreDate != null)
                Return = Return.Where(d => d.cbModification.Value >= Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.Date
                                            || d.DO_Date.Value >= Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.Date).ToList();
            return Return.Count > 0;
        }
        public List<F_DOCENTETE> ListWeb(String Web)
        {
            List<F_DOCENTETE> Return = (from Table in this.DBSage.F_DOCENTETE
                                        where Table.DO_NoWeb.ToUpper() == Web.ToUpper()
                                        orderby Table.cbMarq descending
                                        select Table).ToList();
            if (Core.Global.GetConfig().ConfigCommandeFiltreDate != null)
                Return = Return.Where(d => d.cbModification.Value >= Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.Date
                                            || d.DO_Date.Value >= Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.Date).ToList();
            return Return;
        }

        // <JG> 26/10/2012
        public Boolean Exist(int cbMarq)
        {
            return this.DBSage.F_DOCENTETE.Count(Obj => Obj.cbMarq == cbMarq) > 0;
        }
        public F_DOCENTETE Read(int cbMarq)
        {
            return this.DBSage.F_DOCENTETE.FirstOrDefault(Obj => Obj.cbMarq == cbMarq);
        }

        #region Module CartPreorder

        public List<DocPreorder> ListDocumentArticlePrecommande(String InformationLibre, String value, short Domaine, short Type)
        {
            string request = "SELECT F_DOCENTETE.DO_Piece, F_DOCENTETE.cbMarq, F_DOCLIGNE.AR_Ref FROM F_DOCENTETE, F_DOCLIGNE, F_ARTICLE "
                + " WHERE F_DOCENTETE.DO_Domaine = " + Domaine + " AND F_DOCENTETE.DO_Type = " + Type + " "
                + " AND F_DOCENTETE.DO_NoWeb <> '' "
                + " AND F_DOCENTETE.DO_Type = F_DOCLIGNE.DO_Type "
                + " AND F_DOCENTETE.DO_Domaine = F_DOCLIGNE.DO_Domaine "
                + " AND F_DOCENTETE.DO_Piece = F_DOCLIGNE.DO_Piece "
                + " AND F_DOCLIGNE.AR_Ref = F_ARTICLE.AR_Ref "
                + " AND UPPER(F_ARTICLE.[" + InformationLibre + "])='" + Core.Global.EscapeSqlString(value.ToUpper()) + "' ";

            return this.DBSage.ExecuteQuery<DocPreorder>(@request).ToList();
        }

        #endregion

        #region Module AECInvoiceHistory

        public List<Piece> ListPieceTiers(String CT_Num, ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Domaine Domaine, ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Type Type, DateTime? PeriodeDebut, DateTime? PeriodeFin)
        {
            string request =
                "SELECT F_DOCENTETE.DO_Piece, F_DOCENTETE.DO_Date, F_DOCENTETE.cbMarq, "
                    + "ROUND("
                    /* calcul total HT lignes */
                        + " (SELECT SUM(DL_MontantHT) "
                        + " FROM F_DOCLIGNE "
                        + " WHERE F_DOCLIGNE.DO_Domaine = F_DOCENTETE.DO_Domaine "
                        + " AND F_DOCLIGNE.DO_Type = F_DOCENTETE.DO_Type "
                        + " AND F_DOCLIGNE.DO_Piece = F_DOCENTETE.DO_Piece) "
                    /* calcul montant escompte HT  */
                        + " - (SELECT SUM(DL_MontantHT) * (DO_TxEscompte / 100) "
                        + " FROM F_DOCLIGNE "
                        + " WHERE F_DOCLIGNE.DO_Domaine = F_DOCENTETE.DO_Domaine "
                        + " AND F_DOCLIGNE.DO_Type = F_DOCENTETE.DO_Type "
                        + " AND F_DOCLIGNE.DO_Piece = F_DOCENTETE.DO_Piece "
                        + " AND F_DOCLIGNE.DL_Escompte = 0) "
                    + ", 2)"
                    /* calcul frais de port HT */
                    + " + (CASE DO_TypeLigneFrais WHEN 0 THEN DO_ValFrais ELSE (DO_ValFrais / (1 + (DO_Taxe1 / 100))) END) "
                + "AS TotalAmountTaxExcl, "
                + "  "
                    + "ROUND("
                    /* calcul montant taxes eco-taxe inclue / 2 = eco-taxe HT sinon eco-taxe TTC */
                        + "(SELECT SUM(CASE DL_TypeTaxe2 "
                            + " WHEN 2 THEN ((DL_MontantHT + (DL_Taxe2 * DL_Qte)) * (DL_Taxe1 / 100)) + ROUND(DL_Taxe2 * DL_Qte, 2) "
                            + " ELSE (DL_MontantHT * (DL_Taxe1 / 100)) + ROUND(DL_Taxe2 * DL_Qte, 2) END) "
                        + " FROM F_DOCLIGNE "
                        + " WHERE F_DOCLIGNE.DO_Domaine = F_DOCENTETE.DO_Domaine "
                        + " AND F_DOCLIGNE.DO_Type = F_DOCENTETE.DO_Type "
                        + " AND F_DOCLIGNE.DO_Piece = F_DOCENTETE.DO_Piece) "
                    /* calcul montant escompte TTC (eco-taxe exclue) */
                        + " - (SELECT SUM(DL_MontantHT * (DL_Taxe1 / 100)) * (DO_TxEscompte / 100) "
                        + " FROM F_DOCLIGNE "
                        + " WHERE F_DOCLIGNE.DO_Domaine = F_DOCENTETE.DO_Domaine "
                        + " AND F_DOCLIGNE.DO_Type = F_DOCENTETE.DO_Type "
                        + " AND F_DOCLIGNE.DO_Piece = F_DOCENTETE.DO_Piece "
                        + " AND F_DOCLIGNE.DL_Escompte = 0) "
                    + ", 2)"
                    /* calcul taxes sur frais de port */
                    + " + (CASE DO_TypeLigneFrais WHEN 0 THEN DO_ValFrais * (DO_Taxe1 / 100) ELSE DO_ValFrais - (DO_ValFrais / (1 + (DO_Taxe1 / 100))) END) "
                + "AS TotalTaxAmount "
                + "  "
                + " FROM F_DOCENTETE "
                + " WHERE F_DOCENTETE.DO_Domaine = " + (short)Domaine
                + " AND F_DOCENTETE.DO_Type = " + (short)Type
                + " AND F_DOCENTETE.DO_Tiers = '" + Core.Global.EscapeSqlString(CT_Num) + "' "
                + ((PeriodeDebut != null && PeriodeFin != null) ? " AND F_DOCENTETE.DO_DATE BETWEEN '" + PeriodeDebut.Value.ToShortDateString() + "' AND '" + PeriodeFin.Value.ToShortDateString() + "' " 
                    : (PeriodeDebut != null) ? " AND F_DOCENTETE.DO_DATE >= '" + PeriodeDebut.Value.ToShortDateString() + "' "
                    : (PeriodeFin != null) ? " AND F_DOCENTETE.DO_DATE <= '" + PeriodeFin.Value.ToShortDateString() + "' "
                    : string.Empty)
                + " ORDER BY F_DOCENTETE.DO_Date ";

            return this.DBSage.ExecuteQuery<Piece>(@request).ToList();
        }

        #endregion

        #region Informations libres

        public Boolean ExistDocumentInformationLibreText(String InformationLibre, short Domaine, short Type, String Piece)
        {
            IEnumerable<String> Result = this.DBSage.ExecuteQuery<String>(@"SELECT [" + InformationLibre + "] FROM F_DOCENTETE WHERE DO_Domaine = " + Domaine + " AND DO_Type = " + Type + " AND DO_Piece = '" + Piece + "'");
            if (Result != null && Result.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public String ReadDocumentInformationLibreText(String InformationLibre, short Domaine, short Type, String Piece)
        {
            String Return = string.Empty;
            IEnumerable<String> Result = this.DBSage.ExecuteQuery<String>(@"SELECT [" + InformationLibre + "] FROM F_DOCENTETE WHERE DO_Domaine = " + Domaine + " AND DO_Type = " + Type + " AND DO_Piece = '" + Piece + "'");
            foreach (String value in Result)
                Return = value;

            return Return;
        }
        #endregion
    }
}
