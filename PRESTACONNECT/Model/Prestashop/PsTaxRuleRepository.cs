using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsTaxRuleRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public Boolean ExistTaxeRulesGroupCountry(Int32 TaxeRulesGroup, Int32 Country)
        {
            if (this.DBPrestashop.PsTaxRule.Count(Obj => Obj.IDTaxRulesGroup == TaxeRulesGroup && Obj.IDCountry == Country) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsTaxRule ReadTaxesRulesGroupCountry(Int32 TaxeRulesGroup, Int32 Country)
        {
            return this.DBPrestashop.PsTaxRule.FirstOrDefault(Obj => Obj.IDTaxRulesGroup == TaxeRulesGroup && Obj.IDCountry == Country);
        }

        public List<int> ListTaxRulesGroup(Int32 IdTax)
        {
            return this.DBPrestashop.PsTaxRule.Where(t => t.IDTax == IdTax).Select(t => t.IDTaxRulesGroup).Distinct().ToList();
        }
    }
}
