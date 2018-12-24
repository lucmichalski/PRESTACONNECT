using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_DOCREGLRepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public Boolean ExistDomaineTypePiece(short Domaine, short Type, String Piece)
        {
            return this.DBSage.F_DOCREGL.Count(Obj => Obj.DO_Domaine == Domaine && Obj.DO_Type == Type && Obj.DO_Piece.ToUpper() == Piece.ToUpper()) > 0;
        }
        public F_DOCREGL ReadDomaineTypePiece(short Domaine, short Type, String Piece)
        {
            return this.DBSage.F_DOCREGL.FirstOrDefault(Obj => Obj.DO_Domaine == Domaine && Obj.DO_Type == Type && Obj.DO_Piece.ToUpper() == Piece.ToUpper());
        }

        public F_DOCREGL ReadDomaineTypePiece(
            ABSTRACTION_SAGE.F_DOCREGL.Obj._Enum_DO_Domaine Domaine,
            ABSTRACTION_SAGE.F_DOCREGL.Obj._Enum_DO_Type Type,
            String Piece, DateTime Date, string Libelle, Decimal Montant,
            ABSTRACTION_SAGE.F_DOCREGL.Obj._Enum_DR_TypeRegl TypeRegl)
        {
            return (from T in this.DBSage.F_DOCREGL
                    where (T.DO_Domaine == (short)Domaine
                        && T.DO_Type == (short)Type
                        && T.DO_Piece.ToUpper() == Piece.ToUpper()
                        && T.DR_Date == Date
                        && T.DR_Libelle == Libelle
                        && T.DR_Montant == Montant
                        && T.DR_TypeRegl == (short)TypeRegl)
                    orderby T.cbMarq descending
                    select T).FirstOrDefault();
        }

        public Boolean Exist(int cbMarq)
        {
            return this.DBSage.F_DOCREGL.Count(Obj => Obj.cbMarq == cbMarq) > 0;
        }

        public F_DOCREGL Read(int cbMarq)
        {
            return this.DBSage.F_DOCREGL.FirstOrDefault(Obj => Obj.cbMarq == cbMarq);
        }

        public decimal? MontantDomaineTypePiece(short Domaine, short Type, String Piece)
        {
            return (from Table in this.DBSage.F_DOCREGL
                    where Table.DO_Domaine == Domaine && Table.DO_Type == Type 
                    && Table.DO_Piece == Piece
                    select Table).Sum(r => r.DR_Montant);
        }
    }
}
