using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsGroupRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        private bool ExistInShop(UInt32 IDGroup, UInt32 IDShop)
        {
            return (DBPrestashop.PsGroupShop.FirstOrDefault(
                result => result.IDShop == IDShop && result.IDGroup == IDGroup) != null);
        }

        public Boolean ExistGroup(Int32 Id)
        {
            if (this.DBPrestashop.PsGroup.Count(Obj => Obj.IDGroup == Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsGroup ReadGroup(Int32 Id)
        {
            return this.DBPrestashop.PsGroup.FirstOrDefault(Obj => Obj.IDGroup == Id);
        }

        public List<PsGroup> List(UInt32 IDShop)
        {
            return DBPrestashop.ExecuteQuery<PsGroup>(
                "SELECT DISTINCT G.id_group, G.reduction, G.price_display_method, G.show_prices, G.date_add, G.date_upd FROM ps_group G " +
                " INNER JOIN ps_group_shop GS ON GS.id_group = G.id_group " +
                " WHERE GS.id_shop = {0} " +
                " ", IDShop).ToList();
        }
    }
}
