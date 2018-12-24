using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Core.Parametres
{
    public enum ImportSageFilterTypeSearchValue
    {
        DesignationContains = 0,
        DesignationBeginOrEndBy = 1,
        DesignationBeginBy = 2,
        DesignationEndBy = 3,
        ReferenceContains = 10,
        ReferenceBeginOrEndBy = 11,
        ReferenceBeginBy = 12,
        ReferenceEndBy = 13,
        ValueContains = 20,
        ValueBeginOrEndBy = 21,
        ValueBeginBy = 22,
        ValueEndBy = 23,
        ValueEquals = 24,
        ValueNotContains = 25,
        ValueNotEquals = 26,
    }
}
