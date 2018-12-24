using System;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsModuleRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public Boolean ExistModule(UInt32 Id, UInt32 IDShop)
        {
            //if (this.DBPrestashop.PsModule.Count(Obj => Obj.IDModule == Id) == 0)
            //{
            //    return false;
            //}
            //else
            //{
            //    return true;
            //}

            return (DBPrestashop.ExecuteQuery<PsModule>(
                "SELECT DISTINCT M.id_module, M.name, M.active, M.version FROM ps_module M " +
                " INNER JOIN ps_module_shop MS ON M.id_module = MS.id_module " +
                " WHERE M.id_module = {0} AND MS.id_shop = {1} " +
                " ", Id, IDShop).FirstOrDefault() != null);
        }

        public PsModule ReadModule(UInt32 Id, UInt32 IDShop)
        {
            //return this.DBPrestashop.PsModule.FirstOrDefault(Obj => Obj.IDModule == Id);

            return DBPrestashop.ExecuteQuery<PsModule>(
                "SELECT DISTINCT M.id_module, M.name, M.active, M.version FROM ps_module M " +
                " INNER JOIN ps_module_shop MS ON M.id_module = MS.id_module " +
                " WHERE M.id_module = {0} AND MS.id_shop = {1} " +
                " ", Id, IDShop).FirstOrDefault();
        }
    }
}
