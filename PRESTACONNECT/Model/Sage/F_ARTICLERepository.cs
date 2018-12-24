using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_ARTICLERepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public static string _fields_light = " AR_Ref, AR_Design, FA_CodeFamille, AR_Gamme1, AR_Gamme2, AR_Condition, AR_SuiviStock, AR_Sommeil, AR_Publie, cbMarq ";
        public static string _fields_import = " AR_Ref, AR_Design, FA_CodeFamille, AR_Gamme1, AR_Gamme2, AR_Condition, AR_SuiviStock, AR_Sommeil, AR_Publie, AR_CodeBarre, CL_No1, CL_No2, CL_No3, CL_No4, cbMarq, "
                + "(SELECT TOP(1) F_ARTFOURNISS.AF_RefFourniss FROM F_ARTFOURNISS WHERE F_ARTFOURNISS.AR_Ref = F_ARTICLE.AR_Ref ORDER BY AF_Principal DESC) as 'AF_Ref'";
        public static string _fields_composition = "SELECT "
                + " F_ARTICLE.AR_Ref, AR_Design, "
                + " CL_No1, (Select CL_Intitule FROM F_CATALOGUE WHERE CL_No = CL_No1) as Catalogue1, (Select cbMarq FROM F_CATALOGUE WHERE CL_No = CL_No1) as Catalogue1_SagId, "
                + " CL_No2, (Select CL_Intitule FROM F_CATALOGUE WHERE CL_No = CL_No2) as Catalogue2, (Select cbMarq FROM F_CATALOGUE WHERE CL_No = CL_No2) as Catalogue2_SagId, "
                + " CL_No3, (Select CL_Intitule FROM F_CATALOGUE WHERE CL_No = CL_No3) as Catalogue3, (Select cbMarq FROM F_CATALOGUE WHERE CL_No = CL_No3) as Catalogue3_SagId, "
                + " CL_No4, (Select CL_Intitule FROM F_CATALOGUE WHERE CL_No = CL_No4) as Catalogue4, (Select cbMarq FROM F_CATALOGUE WHERE CL_No = CL_No4) as Catalogue4_SagId, "
                + " FA_CodeFamille, "
                + " (SELECT FA_Intitule FROM F_FAMILLE WHERE F_FAMILLE.FA_CodeFamille = F_ARTICLE.FA_CodeFamille AND FA_Type = 0) as FA_Intitule, "
                + " AR_Gamme1, "
                + " (SELECT G_Intitule FROM P_GAMME WHERE cbMarq = F_ARTICLE.AR_Gamme1) as Gamme1, "
                + " AR_Gamme2, "
                + " (SELECT G_Intitule FROM P_GAMME WHERE cbMarq = F_ARTICLE.AR_Gamme2) as Gamme2, "
                + " AR_Condition, "
                + " (SELECT P_Conditionnement FROM P_CONDITIONNEMENT WHERE cbMarq = F_ARTICLE.AR_Condition) as TypeConditionnement, "
                + " AR_SuiviStock, AR_Sommeil, AR_Publie, F_ARTICLE.cbMarq, "
                + " F_ARTENUMREF.cbMarq as F_ARTENUMREF_SagId, "
                + " F_ARTENUMREF.AE_Ref, "
                + " F_ARTENUMREF.AE_Sommeil, "
                + " (SELECT EG_Enumere FROM F_ARTGAMME WHERE AG_No = F_ARTENUMREF.AG_No1) as EnumereGamme1, "
                + " (SELECT EG_Enumere FROM F_ARTGAMME WHERE AG_No = F_ARTENUMREF.AG_No2) as EnumereGamme2, "
                + " F_CONDITION.cbMarq as F_CONDITION_SagId, "
                + " F_CONDITION.EC_Enumere, "
                + " F_CONDITION.EC_Quantite, "
                + " F_CONDITION.CO_Ref, "
	            + " (SELECT ACP_ComptaCPT_Taxe1 FROM F_ARTCOMPTA WHERE F_ARTCOMPTA.AR_Ref = F_ARTICLE.AR_Ref AND ACP_Champ = " + Core.Global.GetConfig().ConfigArticleCatComptable + " AND ACP_Type = 0) as TA_Code1, "
                + " (SELECT ACP_ComptaCPT_Taxe2 FROM F_ARTCOMPTA WHERE F_ARTCOMPTA.AR_Ref = F_ARTICLE.AR_Ref AND ACP_Champ = " + Core.Global.GetConfig().ConfigArticleCatComptable + " AND ACP_Type = 0) as TA_Code2, "
                + " (SELECT ACP_ComptaCPT_Taxe3 FROM F_ARTCOMPTA WHERE F_ARTCOMPTA.AR_Ref = F_ARTICLE.AR_Ref AND ACP_Champ = " + Core.Global.GetConfig().ConfigArticleCatComptable + " AND ACP_Type = 0) as TA_Code3, "
                + " (SELECT FCP_ComptaCPT_Taxe1 FROM F_FAMCOMPTA WHERE F_FAMCOMPTA.FA_CodeFamille = F_ARTICLE.FA_CodeFamille AND FCP_Champ = " + Core.Global.GetConfig().ConfigArticleCatComptable + " AND FCP_Type = 0) as TA_CodeFamille1, "
                + " (SELECT FCP_ComptaCPT_Taxe2 FROM F_FAMCOMPTA WHERE F_FAMCOMPTA.FA_CodeFamille = F_ARTICLE.FA_CodeFamille AND FCP_Champ = " + Core.Global.GetConfig().ConfigArticleCatComptable + " AND FCP_Type = 0) as TA_CodeFamille2, "
                + " (SELECT FCP_ComptaCPT_Taxe3 FROM F_FAMCOMPTA WHERE F_FAMCOMPTA.FA_CodeFamille = F_ARTICLE.FA_CodeFamille AND FCP_Champ = " + Core.Global.GetConfig().ConfigArticleCatComptable + " AND FCP_Type = 0) as TA_CodeFamille3 "
                + " FROM F_ARTICLE "
                + " LEFT JOIN F_ARTENUMREF ON F_ARTICLE.AR_Ref = F_ARTENUMREF.AR_Ref "
                + " LEFT JOIN F_CONDITION ON F_CONDITION.AR_Ref = F_ARTICLE.AR_Ref ";

        public static string _fields_photo = (Core.Global.GetConfig().ImportImageSearchReferenceClient) ?
            @"SELECT A.cbMarq, A.AR_Ref, REPLACE(A.AR_Photo, '.\Multimedia\','') as AR_Photo,
                    C.AC_RefClient, REPLACE(M.AR_Photo, '.\Multimedia\','') as AC_Photo
                FROM F_ARTICLE A
                    INNER JOIN F_ARTCLIENT C on a.AR_Ref = C.AR_Ref
                    LEFT JOIN F_ARTICLE M on C.AC_RefClient = M.AR_Ref
                WHERE ((A.AR_Photo IS NOT NULL AND A.AR_Photo <> '') OR (M.AR_Photo IS NOT NULL AND M.AR_Photo <> '')) 
                    AND (C.AC_RefClient = '' OR AC_RefClient IN (SELECT F.AR_Ref FROM F_ARTICLE F))
                GROUP BY A.cbMarq, A.AR_Ref, A.AR_Photo, AC_RefClient, M.AR_Photo
                ORDER BY AR_Ref, AC_Photo desc;"
            : @"SELECT A.cbMarq, A.AR_Ref, REPLACE(A.AR_Photo, '.\Multimedia\','') as AR_Photo 
                FROM F_ARTICLE A
                WHERE (A.AR_Photo IS NOT NULL AND A.AR_Photo <> '')";

        public F_ARTICLE_Light ReadLight(int cbMarq)
        {
            return DBSage.ExecuteQuery<F_ARTICLE_Light>("SELECT " + _fields_light
                + " FROM F_ARTICLE "
                + " WHERE cbMarq = " + cbMarq).FirstOrDefault();
        }

        public List<F_ARTICLE_Photo> ListPhoto()
        {
            return DBSage.ExecuteQuery<F_ARTICLE_Photo>(_fields_photo).ToList();
        }

        public List<F_ARTICLE_Composition> ListComposition(string ar_ref)
        {
            return DBSage.ExecuteQuery<F_ARTICLE_Composition>(_fields_composition + " WHERE F_ARTICLE.AR_Ref = '" + ar_ref + "' ").ToList();
        }
        public F_ARTICLE_Composition ReadComposition(int F_ARTICLE_cbMarq, int? F_ARTENUMREF_cbMarq, int? F_CONDITION_cbMarq)
        {
            return DBSage.ExecuteQuery<F_ARTICLE_Composition>(_fields_composition + " WHERE F_ARTICLE.cbMarq = " + F_ARTICLE_cbMarq
                + ((F_ARTENUMREF_cbMarq != null) ? " AND F_ARTENUMREF.cbMarq = " +  F_ARTENUMREF_cbMarq.Value : string.Empty)
                + ((F_CONDITION_cbMarq != null) ? " AND F_CONDITION.cbMarq = " + F_CONDITION_cbMarq.Value : string.Empty)).FirstOrDefault();
        }

        public List<F_ARTICLE_Light> ListLight()
        {
            return DBSage.ExecuteQuery<F_ARTICLE_Light>("SELECT " + _fields_light + " FROM F_ARTICLE ORDER BY AR_Ref").ToList();
        }
        public List<F_ARTICLE_Light> ListLightHorsStock()
        {
            return DBSage.ExecuteQuery<F_ARTICLE_Light>("SELECT " + _fields_light
                + " FROM F_ARTICLE "
                + " WHERE AR_SuiviStock = " + (short)ABSTRACTION_SAGE.F_ARTICLE.Obj._Enum_AR_SuiviStock.Aucun
                + " AND AR_Sommeil = " + (short)ABSTRACTION_SAGE.F_ARTICLE.Obj._Enum_Boolean.Non
                + " ORDER BY AR_Ref ").ToList();
        }

        public List<F_ARTICLE_Light> ListLightNomenclature()
        {
            return DBSage.ExecuteQuery<F_ARTICLE_Light>("SELECT " + _fields_light
                + " FROM F_ARTICLE "
                + " WHERE AR_Sommeil = " + (short)ABSTRACTION_SAGE.F_ARTICLE.Obj._Enum_Boolean.Non
                + " AND AR_Nomencl = " + (short)ABSTRACTION_SAGE.F_ARTICLE.Obj._Enum_AR_Nomencl.Article_Lie
                + " ORDER BY AR_Ref ").ToList();
        }

        public List<F_ARTICLE_Import> ListInCatalog()
        {
            return DBSage.ExecuteQuery<F_ARTICLE_Import>("SELECT " + _fields_import
                + " FROM F_ARTICLE "
                + " WHERE (CL_No1 IS NOT NULL AND CL_No1 > 0) "
                + " OR (CL_No2 IS NOT NULL AND CL_No2 > 0) "
                + " OR (CL_No3 IS NOT NULL AND CL_No3 > 0) "
                + " OR (CL_No4 IS NOT NULL AND CL_No4 > 0) "
                + " ORDER BY AR_Ref ").ToList();
        }

        public List<F_ARTICLE_Import> ListWithoutCatalog()
        {
            return DBSage.ExecuteQuery<F_ARTICLE_Import>("SELECT " + _fields_import
                + " FROM F_ARTICLE "
                + " WHERE (CL_No1 IS NULL OR CL_No1 = 0) "
                + " AND (CL_No2 IS NULL OR CL_No2 = 0) "
                + " AND (CL_No3 IS NULL OR CL_No3 = 0) "
                + " AND (CL_No4 IS NULL OR CL_No4 = 0) "
                + " ORDER BY AR_Ref ").ToList();
        }

        public List<F_ARTICLE_Import> ListFullCatalogue(Int32 Catalogue)
        {
            return DBSage.ExecuteQuery<F_ARTICLE_Import>("SELECT " + _fields_import
                + " FROM F_ARTICLE "
                + " WHERE (CL_No1 = " + Catalogue + ") "
                + " OR (CL_No2 = " + Catalogue + ") "
                + " OR (CL_No3 = " + Catalogue + ") "
                + " OR (CL_No4 = " + Catalogue + ") "
                + " ORDER BY AR_Ref ").ToList();
        }

        public List<F_ARTICLE_Composition> ListComposition()
        {
            return DBSage.ExecuteQuery<F_ARTICLE_Composition>(_fields_composition
                //+ " WHERE (CL_No1 IS NOT NULL AND CL_No1 > 0) "
                //+ " OR (CL_No2 IS NOT NULL AND CL_No2 > 0) "
                //+ " OR (CL_No3 IS NOT NULL AND CL_No3 > 0) "
                //+ " OR (CL_No4 IS NOT NULL AND CL_No4 > 0) "
                + " ORDER BY AR_Ref ").ToList();
        }

        private List<F_ARTICLE> ListHorsStock()
        {
            return (from Table in this.DBSage.F_ARTICLE
                    where Table.AR_SuiviStock == (short)ABSTRACTION_SAGE.F_ARTICLE.Obj._Enum_AR_SuiviStock.Aucun
                        && Table.AR_Sommeil == (short)ABSTRACTION_SAGE.F_ARTICLE.Obj._Enum_Boolean.Non
                    select Table).ToList();
        }

        public Boolean ExistArticle(Int32 Id)
        {
            if (this.DBSage.F_ARTICLE.Count(Obj => Obj.cbMarq == Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Boolean ExistReference(String Reference)
        {
            if (this.DBSage.F_ARTICLE.Count(Obj => Obj.AR_Ref.ToUpper() == Reference.ToUpper()) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public F_ARTICLE ReadReference(String Reference)
        {
            return this.DBSage.F_ARTICLE.FirstOrDefault(Obj => Obj.AR_Ref.ToUpper() == Reference.ToUpper());
        }

        public F_ARTICLE ReadArticle(Int32 Id)
        {
            return this.DBSage.F_ARTICLE.FirstOrDefault(Obj => Obj.cbMarq == Id);
        }


        public List<F_ARTICLE> List()
        {
            return this.DBSage.F_ARTICLE.OrderBy(a => a.AR_Ref).ToList();
        }

        #region Informations libres

        public Boolean ExistArticleInformationLibreText(String InformationLibre, String Ref)
        {
            return ExistArticleInformationLibreText(InformationLibre, Ref, true);
        }
        public Boolean ExistArticleInformationLibreText(String InformationLibre, String Ref, bool acceptnull)
        {
            List<String> Result = this.DBSage.ExecuteQuery<String>(@"select [" + InformationLibre + "] from F_ARTICLE where ar_ref = '" + Ref + "'").ToList();
            if (Result != null && Result.Count > 0)
            {
                return (acceptnull) ? true : Result.First() != null;
            }
            else
            {
                return false;
            }
        }
        public String ReadArticleInformationLibreText(String InformationLibre, String Ref)
        {
            return ReadArticleInformationLibreText(InformationLibre, Ref, true);
        }
        public String ReadArticleInformationLibreText(String InformationLibre, String Ref, bool acceptnull)
        {
            String Return = string.Empty;
            List<String> Result = this.DBSage.ExecuteQuery<String>(@"select [" + InformationLibre + "] from F_ARTICLE where ar_ref = '" + Ref + "'").ToList();
            foreach (String value in Result)
                Return = value;

            if (!acceptnull && Return == null)
                Return = string.Empty;

            return Return;
        }

        public Boolean ExistArticleInformationLibreNumerique(String InformationLibre, String Ref)
        {
            IEnumerable<Decimal?> Result = this.DBSage.ExecuteQuery<Decimal?>(@"select [" + InformationLibre + "] from F_ARTICLE where ar_ref = '" + Ref + "'");
            if (Result != null && Result.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public Decimal? ReadArticleInformationLibreNumerique(String InformationLibre, String Ref)
        {
            Decimal? Return = null;
            IEnumerable<Decimal?> Result = this.DBSage.ExecuteQuery<Decimal?>(@"select [" + InformationLibre + "] from F_ARTICLE where ar_ref = '" + Ref + "'");
            foreach (Decimal? value in Result)
                Return = value;

            return Return;
        }

        public Boolean ExistArticleInformationLibreDate(String InformationLibre, String Ref)
        {
            IEnumerable<DateTime?> Result = this.DBSage.ExecuteQuery<DateTime?>(@"select [" + InformationLibre + "] from F_ARTICLE where ar_ref = '" + Ref + "'");
            if (Result != null && Result.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public DateTime? ReadArticleInformationLibreDate(String InformationLibre, String Ref)
        {
            DateTime? Return = null;
            IEnumerable<DateTime?> Result = this.DBSage.ExecuteQuery<DateTime?>(@"select [" + InformationLibre + "] from F_ARTICLE where ar_ref = '" + Ref + "'");
            foreach (DateTime? value in Result)
                Return = value;

            return Return;
        }

        #endregion

        public List<F_ARTICLE> ListCatCompta(Int32 Champ, short Type, string CodeTaxe)
        {
            return (from Table in this.DBSage.F_ARTICLE
                    where Table.AR_SuiviStock == (short)ABSTRACTION_SAGE.F_ARTICLE.Obj._Enum_AR_SuiviStock.Aucun
                        && (Table.F_ARTCOMPTA.Count(Obj => Obj.ACP_Champ == Champ && Obj.ACP_Type == Type && Obj.ACP_ComptaCPT_Taxe1 == CodeTaxe) > 0
                            || ((Table.F_ARTCOMPTA == null || Table.F_ARTCOMPTA.Count(a => a.ACP_Champ == Champ && a.ACP_Type == Type && a.ACP_ComptaCPT_Taxe1 == CodeTaxe) == 0)
                                && (Table.F_FAMILLE != null && Table.F_FAMILLE.F_FAMCOMPTA != null && Table.F_FAMILLE.F_FAMCOMPTA.Count > 0
                                    && Table.F_FAMILLE.F_FAMCOMPTA.Count(f => f.FCP_Champ == Champ && f.FCP_Type == Type && f.FCP_ComptaCPT_Taxe1 == CodeTaxe) > 0)))
                    orderby Table.AR_Ref
                    select Table).ToList();
        }

        public List<Int32> ListId()
        {
            return (from T in this.DBSage.F_ARTICLE
                    select T.cbMarq).ToList();
        }
    }
}
