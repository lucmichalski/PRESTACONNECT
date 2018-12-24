using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsConfigurationRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsConfiguration Obj)
        {
            this.DBPrestashop.PsConfiguration.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Delete(PsConfiguration Obj)
        {
            this.DBPrestashop.PsConfiguration.DeleteOnSubmit(Obj);
            this.Save();
        }

        public List<PsConfiguration> List()
        {
            System.Linq.IQueryable<PsConfiguration> Return = from Table in this.DBPrestashop.PsConfiguration
                                                             select Table;
            return Return.ToList();
        }

        public Boolean ExistName(String Name)
        {
            if (this.DBPrestashop.PsConfiguration.Count(Obj => Obj.Name.ToUpper() == Name.ToUpper()) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsConfiguration ReadName(String Name)
        {
            return this.DBPrestashop.PsConfiguration.FirstOrDefault(Obj => Obj.Name.ToUpper() == Name.ToUpper());
        }

        public Boolean ExistNameShop(String Name)
        {
            //return (this.DBPrestashop.PsConfiguration.Count(Obj => Obj.Name.ToUpper() == Name.ToUpper() && Obj.IDShop != null && Obj.IDShop == Core.Global.CurrentShop.IDShop) > 0);

            if (this.DBPrestashop.PsConfiguration.Count(Obj => Obj.Name.ToUpper() == Name.ToUpper() && Obj.IDShop != null && Core.Global.CurrentShop != null && Obj.IDShop == Core.Global.CurrentShop.IDShop) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public PsConfiguration ReadNameShop(String Name)
        {
            return this.DBPrestashop.PsConfiguration.FirstOrDefault(Obj => Obj.Name.ToUpper() == Name.ToUpper() && Obj.IDShop != null && Core.Global.CurrentShop != null && Obj.IDShop == Core.Global.CurrentShop.IDShop);
        }
    }
}
