using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsEmployeeRepository
    {
   private static string ps_fields_es = "E.id_employee, E.id_profile, E.id_lang, E.lastname, E.firstname, E.email, E.passwd, E.last_passwd_gen, E.stats_date_from, E.stats_date_to, E.stats_compare_from, E.stats_compare_to, E.stats_compare_option, E.preselect_date_range, E.bo_color, E.bo_theme, E.bo_css, E.default_tab, E.bo_width, E.bo_menu, E.active, E.optin, E.id_last_order, E.id_last_customer_message, E.id_last_customer";

        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsEmployee Obj)
        {
            this.DBPrestashop.PsEmployee.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Delete(PsEmployee Obj)
        {
            this.DBPrestashop.PsEmployee.DeleteOnSubmit(Obj);
            this.Save();
        }

        public void DeleteAll(IEnumerable<PsEmployee> list)
        {
            this.DBPrestashop.PsEmployee.DeleteAllOnSubmit(list);
            this.Save();
        }

        public Boolean Exist(UInt32 Employee)
        {
            return (this.DBPrestashop.PsEmployee.Count(Obj => Obj.IDEmployee == Employee) == 1);
        }

        public PsEmployee Read(UInt32 Employee)
        {
            return this.DBPrestashop.PsEmployee.FirstOrDefault(Obj => Obj.IDEmployee == Employee);
        }

        public List<PsEmployee> List()
        {
            return DBPrestashop.ExecuteQuery<PsEmployee>(
                "SELECT " + ps_fields_es +
                " FROM ps_employee AS E " +
                " NATURAL JOIN ps_employee_shop " +
                " WHERE ps_employee_shop.id_shop = " + Core.Global.CurrentShop.IDShop
                ).ToList();
        }
        public List<PsEmployee> List(uint Profile)
        {
            return DBPrestashop.ExecuteQuery<PsEmployee>(
                "SELECT " + ps_fields_es +
                " FROM ps_employee AS E " +
                " NATURAL JOIN ps_employee_shop " +
                " WHERE ps_employee_shop.id_shop = " + Core.Global.CurrentShop.IDShop + 
                " AND E.id_profile = " + Profile
                ).ToList();
        }
    }
}
