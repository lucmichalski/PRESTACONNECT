using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsCustomerRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        private static string ps_fields = "C.id_customer, C.id_shop_group, C.id_shop, C.id_gender, " +
        "C.id_default_group, C.id_risk, C.company, " +
        "C.siret, C.ape, C.firstname, C.lastname, " +
        "C.email, C.passwd, C.last_passwd_gen, C.birthday, " +
        "C.newsletter, C.ip_registration_newsletter, C.newsletter_date_add, C.optin, " +
        "C.website, C.outstanding_allow_amount, C.show_public_prices, C.max_payment_days, " +
        "C.secure_key, C.note, C.active, C.is_guest, " +
        "C.deleted, C.date_add, C.date_upd";

        private static string ps_fields_short = "C.id_customer ";
        private static string ps_fields_btob = "C.id_customer, C.firstname, C.lastname, C.email, C.company ";
        private static string ps_fields_btoc = "C.id_customer, C.firstname, C.lastname, C.email, C.date_add ";
        private static string ps_fields_btoc_order = "C.date_add DESC, C.id_customer, C.firstname, C.lastname, C.email ";

        /*
         PS154
        C.id_customer, C.id_shop_group, C.id_shop, C.id_gender, 
        C.id_default_group, C.id_lang, C.id_risk, C.company, 
        C.siret, C.ape, C.firstname, C.lastname, 
        C.email, C.passwd, C.last_passwd_gen, C.birthday, 
        C.newsletter, C.ip_registration_newsletter, C.newsletter_date_add, C.optin, 
        C.website, C.outstanding_allow_amount, C.show_public_prices, C.max_payment_days, 
        C.secure_key, C.note, C.active, C.is_guest, 
        C.deleted, C.date_add, C.date_upd

         PS152 / PS153
        C.id_customer, C.id_shop_group, C.id_shop, C.id_gender, 
        C.id_default_group, C.id_risk, C.company, 
        C.siret, C.ape, C.firstname, C.lastname, 
        C.email, C.passwd, C.last_passwd_gen, C.birthday, 
        C.newsletter, C.ip_registration_newsletter, C.newsletter_date_add, C.optin, 
        C.website, C.outstanding_allow_amount, C.show_public_prices, C.max_payment_days, 
        C.secure_key, C.note, C.active, C.is_guest, 
        C.deleted, C.date_add, C.date_upd

            */

        public List<btoc_customer> ListTopActiveOrderByDateAddWithOrder(Int32 Top, byte Active, UInt32 IDShop)
        {
            //System.Linq.IQueryable<PsCustomer> Return = (from Table in this.DBPrestashop.PsCustomer
            //                                             where Table.Active == Active
            //                                             orderby Table.DateAdd descending
            //                                             select Table).Take(Top);

            //return Return.ToList();
            return DBPrestashop.ExecuteQuery<btoc_customer>(
                "SELECT DISTINCT " +
                ps_fields_btoc +
                " FROM ps_customer C " +
                " INNER JOIN ps_orders O ON C.id_customer = O.id_customer " +
                " WHERE C.active = {0} AND C.id_shop = {1} " +
                ((Core.Global.GetConfig().ConfigCommandeFiltreDate != null) ? " AND O.date_add >= '"
                    + Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.ToString("yyyy/MM/dd") + "'" : "") +
                " ORDER BY " + ps_fields_btoc_order + 
                " LIMIT {2} ", Active, IDShop, Top).ToList();
        }

        public List<btoc_customer> ListTopActiveOrderByDateAdd(Int32 Top, byte Active, UInt32 IDShop)
        {
            return DBPrestashop.ExecuteQuery<btoc_customer>(
                "SELECT DISTINCT " +
                ps_fields_btoc +
                " FROM ps_customer C " +
                " LEFT JOIN ps_orders O ON C.id_customer = O.id_customer " +
                " WHERE C.active = {0} AND C.id_shop = {1} " +
                ((Core.Global.GetConfig().ConfigCommandeFiltreDate != null)
                    ? " AND (C.date_add >= '" + Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.ToString("yyyy/MM/dd") + "' "
                        + " OR O.date_add >= '" + Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.ToString("yyyy/MM/dd") + "' ) "
                    : "") +
                " ORDER BY date_add DESC " +
                " LIMIT {2} ", Active, IDShop, Top).ToList();
        }

        public List<idcustomer> ListIDActiveWithOrder(Byte Active, UInt32 IDShop)
        {
            //System.Linq.IQueryable<PsCustomer> Return = from Table in this.DBPrestashop.PsCustomer
            //                                            where Table.Active == Active
            //                                            select Table;
            //return Return.ToList();

            return DBPrestashop.ExecuteQuery<idcustomer>(
                "SELECT DISTINCT " +
                ps_fields_short +
                " FROM ps_customer C " +
                " INNER JOIN ps_orders O ON O.id_customer = C.id_customer " +
                " WHERE C.active = {0} AND C.id_shop = {1} " +
                ((Core.Global.GetConfig().ConfigCommandeFiltreDate != null) ? " AND O.date_add >= '"
                    + Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.ToString("yyyy/MM/dd") + "'" : "") +
                " ", Active, IDShop).ToList();
        }

        public List<idcustomer> ListIDActive(Byte Active, UInt32 IDShop)
        {
            return DBPrestashop.ExecuteQuery<idcustomer>(
                "SELECT DISTINCT " +
                ps_fields_short +
                " FROM ps_customer C " +
                " LEFT JOIN ps_orders O ON O.id_customer = C.id_customer " +
                " WHERE C.active = {0} AND C.id_shop = {1} " +
                ((Core.Global.GetConfig().ConfigCommandeFiltreDate != null)
                    ? " AND (C.date_add >= '" + Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.ToString("yyyy/MM/dd") + "' "
                        + " OR O.date_add >= '" + Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.ToString("yyyy/MM/dd") + "' ) "
                    : "") +
                " ", Active, IDShop).ToList();
        }
        public List<idcustomer> ListIDActiveFull(Byte Active, UInt32 IDShop)
        {
            return DBPrestashop.ExecuteQuery<idcustomer>(
                "SELECT DISTINCT " +
                ps_fields_short +
                " FROM ps_customer C " +
                " WHERE C.active = {0} AND C.id_shop = {1} " +
                " ", Active, IDShop).AsParallel().ToList();
        }

        public List<btob_customer> ListFullDate(UInt32 IDShop)
        {
            return DBPrestashop.ExecuteQuery<btob_customer>(
                "SELECT DISTINCT " +
                ps_fields_btob +
                " FROM ps_customer C " +
                " LEFT JOIN ps_orders O ON O.id_customer = C.id_customer " +
                " WHERE C.id_shop = {0} " +
                ((Core.Global.GetConfig().ConfigCommandeFiltreDate != null)
                    ? " AND (C.date_add >= '" + Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.ToString("yyyy/MM/dd") + "' "
                        + " OR O.date_add >= '" + Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.ToString("yyyy/MM/dd") + "' ) "
                    : "") +
                " ", IDShop).ToList();
        }

        public List<btob_customer> ListFull(UInt32 IDShop)
        {
            return DBPrestashop.ExecuteQuery<btob_customer>(
                "SELECT DISTINCT " +
                ps_fields_btob +
                " FROM ps_customer C " +
                " WHERE C.id_shop = {0} " +
                " ", IDShop).ToList();
        }

        public Boolean ExistCustomer(UInt32 Id)
        {
            return this.DBPrestashop.PsCustomer.Count(Obj => Obj.IDCustomer == Id) > 0;
        }

        public Boolean ExistCustomerWithOrder(UInt32 Id)
        {
            //TODO Voir si l'equivalent d'un executescalar() est possible ou utiliser un executequery().
            return (DBPrestashop.ExecuteQuery<PsCustomer>(
                "SELECT DISTINCT " +
                ps_fields +
                " FROM ps_customer C " +
                " INNER JOIN ps_orders O ON C.id_customer = O.id_customer " +
                " WHERE C.id_customer = {0} " +
                ((Core.Global.GetConfig().ConfigCommandeFiltreDate != null) ? " AND O.date_add >= '"
                    + Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.ToString("yyyy/MM/dd") + "'" : "") +
                " ", Id).FirstOrDefault() != null);
        }

        public PsCustomer ReadCustomerWithOrder(UInt32 Id)
        {
            return DBPrestashop.ExecuteQuery<PsCustomer>(
                "SELECT DISTINCT " +
                ps_fields +
                " FROM ps_customer C " +
                " INNER JOIN ps_orders O ON C.id_customer = O.id_customer " +
                " WHERE C.id_customer = {0} " +
                ((Core.Global.GetConfig().ConfigCommandeFiltreDate != null) ? " AND O.date_add >= '"
                    + Core.Global.GetConfig().ConfigCommandeFiltreDate.Value.ToString("yyyy/MM/dd") + "'" : "") +
                " ", Id).FirstOrDefault();
        }

        public PsCustomer ReadCustomer(UInt32 Id)
        {
            return this.DBPrestashop.PsCustomer.FirstOrDefault(Obj => Obj.IDCustomer == Id);
        }

        public void Add(PsCustomer NouveauCompte)
        {
            this.DBPrestashop.PsCustomer.InsertOnSubmit(NouveauCompte);
            this.Save();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public Boolean ExistMail(String Email, UInt32 IDShop)
        {
            return this.DBPrestashop.PsCustomer.Count(cus => cus.Email.ToUpper() == Email.ToUpper() && cus.IDShop == IDShop) > 0;
        }
    }

    public class idcustomer
    {
        public uint id_customer = 0;
    }
}
