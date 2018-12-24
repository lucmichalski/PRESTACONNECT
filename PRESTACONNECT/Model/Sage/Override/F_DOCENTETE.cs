using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public partial class F_DOCENTETE
    {
    }

    public class DocPreorder
    {
        public string DO_Piece;
        public int cbMarq;
        public string AR_Ref;
    }
    public class Piece
    {
        public string DO_Piece;
        public DateTime? DO_Date;
        public int cbMarq;
        public decimal? TotalAmountTaxExcl;
        public decimal? TotalTaxAmount;
    }

    public class F_DOCENTETE_Light
    {
        public short DO_Domaine;
        public short DO_Type;
        public string DO_Piece;
        public DateTime? DO_Date;
        public string DO_Tiers;

        public override string ToString()
        {
            return (DO_Date != null) 
            ? DO_Date.Value.ToShortDateString() + " / " +  DO_Tiers + " / " + DO_Piece
            : DO_Tiers + " / " + DO_Piece;
        }
    }
}
