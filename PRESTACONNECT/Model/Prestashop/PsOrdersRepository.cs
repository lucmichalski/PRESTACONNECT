using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsOrdersRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        private static string ps_fields_short = " O.id_order, O.current_state ";
        private static string ps_fields_payment = " O.id_order, O.reference ";
        private static string ps_fields_resume = " O.id_order, O.reference, O.current_state, O.id_customer, C.lastname, C.firstname, O.total_paid_tax_incl, O.total_paid_tax_excl, O.payment, OS.name as order_state_name, O.date_add ";
        private static string ps_fields_resume_order = "O.date_add DESC,  O.id_order, O.reference, O.current_state, O.id_customer, C.lastname, C.firstname, O.total_paid_tax_incl, O.total_paid_tax_excl, O.payment, OS.name ";
        private static string ps_fields_address = " O.id_order, O.id_cart, O.id_shop, O.id_customer, O.id_address_invoice, O.id_address_delivery ";


        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public List<PsOrders> List(UInt32 IDShop)
        {
            List<PsOrders> Return = (from Table in this.DBPrestashop.PsOrders
                                     where Table.IDShop == IDShop
                                     select Table).ToList();
            if (Core.Global.GetConfig().ConfigCommandeFiltreDate != null)
                Return = Return.Where(c => c.DateAdd >= Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.Date).ToList();
            return Return;
        }

        public List<PsOrders> ListTopOrderByDateAdd(Int32 Top, UInt32 IDShop)
        {
            List<PsOrders> Return = (from Table in this.DBPrestashop.PsOrders
                                     where Table.IDShop == IDShop
                                     orderby Table.DateAdd descending
                                     select Table).Take(Top).ToList();
            if (Core.Global.GetConfig().ConfigCommandeFiltreDate != null)
                Return = Return.Where(c => c.DateAdd >= Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.Date).ToList();
            return Return.ToList();
        }

        public List<ps_orders_resume> ListTopOrderResumeByDateAdd(Int32 Top, UInt32 Lang, UInt32 IDShop)
        {
            return DBPrestashop.ExecuteQuery<ps_orders_resume>(
                "SELECT " +
                ps_fields_resume +
                " FROM ps_orders as O " +
                " LEFT JOIN ps_customer as C ON O.id_customer = C.id_customer " +
                " LEFT JOIN ps_order_state_lang as OS ON O.current_state = OS.id_order_state " +
                " WHERE O.id_shop = {0} AND OS.id_lang = {1} " +
                ((Core.Global.GetConfig().ConfigCommandeFiltreDate != null) ? " AND O.date_add >= '"
                    + Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.ToString("yyyy/MM/dd") + "'" : "") +
                " ORDER BY " + ps_fields_resume_order +
                " LIMIT {2} ", IDShop, Lang, Top).ToList();
        }

        public Boolean ExistOrder(Int32 Order)
        {
            return this.DBPrestashop.PsOrders.Count(Obj => Obj.IDOrder == Order) > 0;
        }

        public PsOrders ReadOrder(Int32 Order)
        {
            return this.DBPrestashop.PsOrders.FirstOrDefault(Obj => Obj.IDOrder == Order);
        }

        //public Boolean ExistCustomer(Int32 Customer)
        //{
        //    return this.DBPrestashop.PsOrders.Count(Obj => Obj.IDCustomer == Customer) > 0;
        //}
        //public PsOrders ReadCustomer(Int32 Customer)
        //{
        //    return this.DBPrestashop.PsOrders.FirstOrDefault(Obj => Obj.IDCustomer == Customer);
        //}

        public Boolean ExistCustomer(UInt32 IDShop, UInt32 Customer)
        {
            return DBPrestashop.ExecuteQuery<order_address>(
                "SELECT DISTINCT " + ps_fields_address +
                " FROM ps_orders O " +
                " WHERE O.id_shop = {0} " +
                " AND O.id_customer = " + Customer, IDShop).Count() > 0;
        }
        public order_address ReadCustomer(UInt32 IDShop, UInt32 Customer)
        {
            return DBPrestashop.ExecuteQuery<order_address>(
                "SELECT DISTINCT " + ps_fields_address +
                " FROM ps_orders O " +
                " WHERE O.id_shop = {0} " +
                " AND O.id_customer = " + Customer, IDShop).FirstOrDefault();
        }

        public void Delete(PsOrders Obj)
        {
            this.DBPrestashop.PsOrders.DeleteOnSubmit(Obj);
            this.Save();
        }

        public List<idorder> ListID(UInt32 IDShop)
        {
            return DBPrestashop.ExecuteQuery<idorder>(
                "SELECT DISTINCT " +
                ps_fields_short +
                " FROM ps_orders O " +
                " WHERE O.id_shop = {0} " +
                ((Core.Global.GetConfig().ConfigCommandeFiltreDate != null) ? " AND O.date_add >= '"
                    + Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.ToString("yyyy/MM/dd") + "'" : "") +
                " ", IDShop).ToList();
        }

        public List<order_payment> ListIDPayment(UInt32 IDShop)
        {
            return DBPrestashop.ExecuteQuery<order_payment>(
                "SELECT DISTINCT " +
                ps_fields_payment +
                " FROM ps_orders O " +
                " WHERE O.id_shop = {0} " +
                ((Core.Global.GetConfig().ConfigCommandeFiltreDate != null) ? " AND O.date_add >= '"
                    + Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.ToString("yyyy/MM/dd") + "'" : "") +
                " ", IDShop).ToList();
        }

        public List<idorder> ListID(UInt32 IDShop, DateTime date)
        {
            return DBPrestashop.ExecuteQuery<idorder>(
                "SELECT DISTINCT " +
                ps_fields_short +
                " FROM ps_orders O " +
                " WHERE O.id_shop = {0} " +
                ((Core.Global.GetConfig().ConfigCommandeFiltreDate != null) ? " AND O.date_add >= '"
                    + Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.ToString("yyyy/MM/dd") + "'" : "") +
                " AND O.date_add >= '" + date.ToString("yyyy/MM/dd") + "'" +
                " ", IDShop).ToList();
        }

        public List<order_address> ListAddress(UInt32 IDShop, UInt32 Customer)
        {
            return DBPrestashop.ExecuteQuery<order_address>(
                "SELECT DISTINCT " +
                ps_fields_address +
                " FROM ps_orders O " +
                " WHERE O.id_shop = {0} " +
                ((Core.Global.GetConfig().ConfigCommandeFiltreDate != null) ? " AND O.date_add >= '"
                    + Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.ToString("yyyy/MM/dd") + "'" : "") +
                " ", IDShop).ToList();
        }
    }
}
