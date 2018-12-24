using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_COMPTETRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public static string _fields_light = " CT_Num, CT_Intitule, CT_Type, N_CatTarif, N_CatCompta, CT_EMail, CT_NumCentrale, CT_Taux01, CT_Sommeil, cbMarq ";
        public static string _fields_btob = " CT_Num, CT_Intitule, CT_EMail, cbMarq ";

        public List<F_COMPTET_Light> ListLight()
        {
            return DBSage.ExecuteQuery<F_COMPTET_Light>("SELECT " + _fields_light + " FROM F_COMPTET").ToList();
        }
        public List<F_COMPTET_Light> ListLight(short Type)
        {
            return DBSage.ExecuteQuery<F_COMPTET_Light>("SELECT " + _fields_light
                + " FROM F_COMPTET "
                + " WHERE CT_Type = " + Type).ToList();
        }
        public List<F_COMPTET_Light> ListLight(short Type, short Sommeil)
        {
            return DBSage.ExecuteQuery<F_COMPTET_Light>("SELECT " + _fields_light
                + " FROM F_COMPTET "
                + " WHERE CT_Type = " + Type
                + " AND CT_Sommeil = " + Sommeil).ToList();
        }
        public List<F_COMPTET_BtoB> ListBtoB(short Type)
        {
            return DBSage.ExecuteQuery<F_COMPTET_BtoB>("SELECT " + _fields_btob
                + " FROM F_COMPTET "
                + " WHERE CT_Type = " + Type).ToList();
        }

        public List<F_COMPTET_Light> ListClientRemiseTarifException(String ReferenceArticle)
        {
            return DBSage.ExecuteQuery<F_COMPTET_Light>("SELECT " + _fields_light
                + " FROM F_COMPTET T "
                + " WHERE (T.CT_Taux01 IS NOT NULL AND T.CT_Taux01 > 0) "
                + " OR T.CT_Num IN (SELECT A.CT_NUM FROM F_ARTCLIENT A WHERE A.AR_Ref = '{0}') ", ReferenceArticle).ToList();
        }

        public List<F_COMPTET_Light> ListCentrale(string numeroCentrale)
        {
            return DBSage.ExecuteQuery<F_COMPTET_Light>("SELECT " + _fields_light
                + " FROM F_COMPTET "
                + " WHERE CT_NumCentrale = '" + numeroCentrale + "' ").ToList();
        }

        public F_COMPTET Read(Int32 marqueur)
        {
            return this.DBSage.F_COMPTET.FirstOrDefault(Obj => Obj.cbMarq == marqueur);
        }

        public F_COMPTET Read(string numero)
        {
            return this.DBSage.F_COMPTET.FirstOrDefault(Obj => Obj.CT_Num == numero.ToUpper());
        }

        public Boolean ExistId(Int32 marqueur)
        {
            if (this.DBSage.F_COMPTET.Count(Obj => Obj.cbMarq == marqueur) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public List<F_COMPTET> List()
        {
            return this.DBSage.F_COMPTET.ToList();
        }

        public List<F_COMPTET> ListType(short Type)
        {
            System.Linq.IQueryable<F_COMPTET> Return = from Table in this.DBSage.F_COMPTET
                                                       where Table.CT_Type == Type
                                                       select Table;
            return Return.ToList();
        }

        public List<F_COMPTET> ListClientsAvecRemises()
        {
            System.Linq.IQueryable<F_COMPTET> Return = from Table in this.DBSage.F_COMPTET
                                                       where Table.CT_Type == 0 && Table.CT_Taux01 > 0
                                                       select Table;
            return Return.ToList();
        }

        public List<Int32> ListIdType(short Type)
        {
            System.Linq.IQueryable<Int32> Return = from Table in this.DBSage.F_COMPTET
                                                   where Table.CT_Type == Type
                                                   select Table.cbMarq;
            return Return.ToList();
        }
        public List<Int32> ListIdTypeSommeil(short Type, short Sommeil)
        {
            System.Linq.IQueryable<Int32> Return = from Table in this.DBSage.F_COMPTET
                                                   where Table.CT_Type == Type && Table.CT_Sommeil == Sommeil
                                                   select Table.cbMarq;
            return Return.ToList();
        }

        public List<F_COMPTET> ListTypeSommeil(short Type, short Sommeil)
        {
            System.Linq.IQueryable<F_COMPTET> Return = from Table in this.DBSage.F_COMPTET
                                                       where Table.CT_Type == Type && Table.CT_Sommeil == Sommeil
                                                       orderby Table.CT_Num
                                                       select Table;
            return Return.ToList();
        }

        public List<F_COMPTET> ListTypeSommeilStartWithNum(short Type, short Sommeil, String Num)
        {
            IQueryable<F_COMPTET> Return = from Table in this.DBSage.F_COMPTET
                                           where Table.CT_Type == Type && Table.CT_Sommeil == Sommeil && Table.CT_Num.ToUpper().StartsWith(Num.ToUpper())
                                           select Table;
            return Return.ToList();
        }

        public Boolean ExistComptet(String Comptet)
        {
            if (this.DBSage.F_COMPTET.Count(Obj => Obj.CT_Num.ToUpper() == Comptet) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public F_COMPTET ReadComptet(String Comptet)
        {
            return this.DBSage.F_COMPTET.FirstOrDefault(Obj => Obj.CT_Num.ToUpper() == Comptet);
        }

        public Boolean ExistPrefixeClient(String PrefixeClient)
        {
            return this.DBSage.F_COMPTET.Count(Obj => Obj.CT_Num.ToUpper().StartsWith(PrefixeClient)) > 0;
        }

        public String ReadMaxCT_NumPrefixe(String PrefixeClient)
        {
            return this.DBSage.F_COMPTET.Where(Obj => Obj.CT_Num.ToUpper().StartsWith(PrefixeClient)).OrderByDescending(Obj => Obj.CT_Num).First().CT_Num;
        }

        #region Informations libres

        public Boolean ExistArticleInformationLibreText(String InformationLibre, String CT_Num)
        {
            IEnumerable<String> Result = this.DBSage.ExecuteQuery<String>(@"select [" + InformationLibre + "] from F_COMPTET where CT_Num = '" + CT_Num + "'");
            if (Result != null && Result.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public String ReadArticleInformationLibreText(String InformationLibre, String CT_Num)
        {
            String Return = "";
            IEnumerable<String> Result = this.DBSage.ExecuteQuery<String>(@"select [" + InformationLibre + "] from F_COMPTET where CT_Num = '" + CT_Num + "'");
            foreach (String value in Result)
                Return = value;

            return Return;
        }

        public Boolean ExistArticleInformationLibreNumerique(String InformationLibre, String CT_Num)
        {
            IEnumerable<Decimal?> Result = this.DBSage.ExecuteQuery<Decimal?>(@"select [" + InformationLibre + "] from F_COMPTET where CT_Num = '" + CT_Num + "'");
            if (Result != null && Result.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public Decimal? ReadArticleInformationLibreNumerique(String InformationLibre, String CT_Num)
        {
            Decimal? Return = null;
            IEnumerable<Decimal?> Result = this.DBSage.ExecuteQuery<Decimal?>(@"select [" + InformationLibre + "] from F_COMPTET where CT_Num = '" + CT_Num + "'");
            foreach (Decimal? value in Result)
                Return = value;

            return Return;
        }

        public Boolean ExistArticleInformationLibreDate(String InformationLibre, String CT_Num)
        {
            IEnumerable<DateTime?> Result = this.DBSage.ExecuteQuery<DateTime?>(@"select [" + InformationLibre + "] from F_COMPTET where CT_Num = '" + CT_Num + "'");
            if (Result != null && Result.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public DateTime? ReadArticleInformationLibreDate(String InformationLibre, String CT_Num)
        {
            DateTime? Return = null;
            IEnumerable<DateTime?> Result = this.DBSage.ExecuteQuery<DateTime?>(@"select [" + InformationLibre + "] from F_COMPTET where CT_Num = '" + CT_Num + "'");
            foreach (DateTime? value in Result)
                Return = value;

            return Return;
        }

        #endregion
    }
}
