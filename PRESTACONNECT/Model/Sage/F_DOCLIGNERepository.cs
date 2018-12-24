using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_DOCLIGNERepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public static string _fields_light = " DO_Domaine, DO_Type, DO_Piece, DL_PieceBC, DL_PieceBL, AR_Ref ";

        public List<F_DOCLIGNE_Light> ListLight()
        {
            return DBSage.ExecuteQuery<F_DOCLIGNE_Light>("SELECT " + _fields_light + " FROM F_DOCLIGNE").ToList();
        }
        public List<F_DOCLIGNE_Light> ListLight(short Domaine, short Type, short Type2, short Type3, short Type4)
        {
            return DBSage.ExecuteQuery<F_DOCLIGNE_Light>("SELECT " + _fields_light
                + " FROM F_DOCLIGNE WHERE DO_Domaine = " + Domaine
                + " AND DO_Type IN (" + Type + ", " + Type2 + ", " + Type3 + ", " + Type4 + ") ").ToList();
        }

        public Boolean ExistDomaineTypePieceValorise(short Domaine, short Type, String Piece)
        {
            if (this.DBSage.F_DOCLIGNE.Count(Obj => Obj.DO_Domaine == Domaine && Obj.DO_Type == Type && Obj.DO_Piece.ToUpper() == Piece.ToUpper() && Obj.DL_Valorise == 1) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public List<F_DOCLIGNE> ListDomaineTypePieceValorise(short Domaine, short Type, String Piece)
        {
            return (from T in DBSage.F_DOCLIGNE
                    where T.DO_Domaine == Domaine && T.DO_Type == Type && T.DO_Piece == Piece && T.DL_Valorise == 1
                    select T).ToList();
        }

        public decimal? MontantDomaineTypePieceValorise(short Domaine, short Type, String Piece)
        {
            return (from T in DBSage.F_DOCLIGNE
                    where T.DO_Domaine == Domaine && T.DO_Type == Type && T.DO_Piece == Piece && T.DL_Valorise == 1
                    select T).Sum(l => l.DL_MontantTTC);
        }
    }
}
