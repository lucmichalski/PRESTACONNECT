using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsAddressRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public static string _fields_light = " A.id_address, A.id_customer ";
        
        public List<PsAddress_Light> ListWithOrder()
        {
            return DBPrestashop.ExecuteQuery<PsAddress_Light>(
                "SELECT DISTINCT " + _fields_light +
                " FROM ps_address A " +
                " WHERE A.id_customer <> 0 " +
                " AND (A.id_address in (SELECT DISTINCT O.id_address_delivery FROM ps_orders O WHERE O.id_shop = {0}" +
                        ((Core.Global.GetConfig().ConfigCommandeFiltreDate != null) ? " AND O.date_add >= '"
                            + Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.ToString("yyyy/MM/dd") + "'" : "") + ") " +
                    //" OR A.id_address in (SELECT DISTINCT O.id_address_invoice FROM ps_orders O WHERE O.id_shop = {0}" +
                    //    ((Core.Global.GetConfig().ConfigCommandeFiltreDate != null) ? " AND O.date_add >= '"
                    //        + Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.ToString("yyyy/MM/dd") + "'" : "") + ")"
                    ")" +
                " ", Core.Global.CurrentShop.IDShop).ToList();
        }

        public List<PsAddress_Light> List()
        {
            string request = 
                "SELECT DISTINCT " + _fields_light +
                " FROM ps_address A " +
                " WHERE A.Active = 1 " +
                " AND A.id_customer <> 0 " +
                " AND ((A.deleted = 0 " +
                    " AND A.id_address in (SELECT DISTINCT O.id_address_delivery FROM ps_orders O WHERE O.id_shop = " + Core.Global.CurrentShop.IDShop + " " +
                        ((Core.Global.GetConfig().ConfigCommandeFiltreDate != null) ? " AND O.date_add >= '"
                            + Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.ToString("yyyy/MM/dd") + "'" : "") + ") " +
                    //" OR A.id_address in (SELECT DISTINCT O.id_address_invoice FROM ps_orders O WHERE O.id_shop = {0}" +
                    //    ((Core.Global.GetConfig().ConfigCommandeFiltreDate != null) ? " AND O.date_add >= '"
                //        + Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.ToString("yyyy/MM/dd") + "'" : "") + ")"
                    ")" +
                    ((Core.Global.GetConfig().ConfigCommandeFiltreDate != null)
                        ? " OR (A.date_add >= '" + Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.ToString("yyyy/MM/dd") + "'"
                            + " OR A.date_upd >= '" + Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.ToString("yyyy/MM/dd") + "')"
                        : "") +
                    ")" +
                " ";

            return DBPrestashop.ExecuteQuery<PsAddress_Light>(request).ToList();
        }

        public List<idaddress> ListCustomer(uint IDCustomer)
        {
            return DBPrestashop.ExecuteQuery<idaddress>(
                "SELECT DISTINCT A.id_address " +
                " FROM ps_address A " +
                " WHERE A.Active = 1 " +
                " AND A.id_customer =  " + IDCustomer + 
                " AND (A.deleted = 0 " +
                    " OR A.id_address in (SELECT DISTINCT O.id_address_delivery FROM ps_orders O WHERE O.id_shop = {0}" +
                        ((Core.Global.GetConfig().ConfigCommandeFiltreDate != null) ? " AND O.date_add >= '"
                            + Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.ToString("yyyy/MM/dd") + "'" : "") + ") " +
                    //" OR A.id_address in (SELECT DISTINCT O.id_address_invoice FROM ps_orders O WHERE O.id_shop = {0}" +
                    //    ((Core.Global.GetConfig().ConfigCommandeFiltreDate != null) ? " AND O.date_add >= '"
                //        + Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.ToString("yyyy/MM/dd") + "'" : "") + ")"
                    ")" +
                " ", Core.Global.CurrentShop.IDShop).ToList();
        }

        public Boolean ExistAddress(UInt32 Id)
        {
            if (this.DBPrestashop.PsAddress.Count(Obj => Obj.IDAddress == Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsAddress ReadAddress(UInt32 Id)
        {
            return this.DBPrestashop.PsAddress.FirstOrDefault(Obj => Obj.IDAddress == Id);
        }

        // <JG> 06/09/2012 ajout création d'adresse de Sage vers Prestashop
        public void Add(PsAddress NouvelleAdresse)
        {
            this.DBPrestashop.PsAddress.InsertOnSubmit(NouvelleAdresse);
            this.DBPrestashop.SubmitChanges();
        }

        public Boolean ExistCustomer(UInt32 IDCustomer, Byte Active, Byte Deleted)
        {
            return this.DBPrestashop.PsAddress.Count(Obj => Obj.IDCustomer == IDCustomer && Obj.Active == Active && Obj.Deleted == Deleted) > 0;
        }

        public PsAddress ReadCustomer(UInt32 IDCustomer, Byte Active, Byte Deleted)
        {
            return this.DBPrestashop.PsAddress.FirstOrDefault(Obj => Obj.IDCustomer == IDCustomer && Obj.Active == Active && Obj.Deleted == Deleted);
        }
    }
}
