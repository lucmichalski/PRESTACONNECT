using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsCarrierRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public List<PsCarrier> ListActive(Byte Active, UInt32 IDShop)
        {
            //System.Linq.IQueryable<PsCarrier> Return = from Table in this.DBPrestashop.PsCarrier
            //                                       where Table.Active == Active
            //                                       select Table;
            //return Return.ToList();
            return DBPrestashop.ExecuteQuery<PsCarrier>(
                "SELECT DISTINCT C.id_carrier, C.id_reference, C.id_tax_rules_group, C.name, " +
                " C.url, C.active, C.deleted, C.shipping_handling, " +
                " C.range_behavior, C.is_module, C.is_free, C.shipping_external, " +
                " C.need_range, C.external_module_name, C.shipping_method, C.position, " +
                " C.max_width, C.max_height, C.max_depth, C.max_weight, " +
                " C.grade FROM ps_carrier C " +
                " INNER JOIN ps_carrier_shop CS ON CS.id_carrier = C.id_carrier " +
                " WHERE C.active = {0} AND CS.id_shop = {1} AND C.deleted = 0 " +
                " ", Active, IDShop).ToList();
        }

        public PsCarrier ReadCarrier(UInt32 Carrier)
        {
            return this.DBPrestashop.PsCarrier.FirstOrDefault(Obj => Obj.IDCarrier == Carrier);
        }

        public List<PsCarrier> ListIDReference(uint IDReference)
        {
            return (from Table in this.DBPrestashop.PsCarrier
                    where Table.IDReference == IDReference
                    orderby Table.IDCarrier descending
                    select Table).ToList();
        }
    }
}
