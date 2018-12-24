using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsCategoryGroupRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsCategoryGroup Obj)
        {
            MySqlConnection Connection = new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString);
            Connection.Open();
            MySqlCommand Command = Connection.CreateCommand();
            Command.CommandText = "insert into ps_category_group (id_category, id_group) values (" + Obj.IDCategory + ", " + Obj.IDGroup + ")";
            Command.ExecuteNonQuery();
            Connection.Close();
        }

        public void Delete(PsCategoryGroup Obj)
        {
            this.DBPrestashop.PsCategoryGroup.DeleteOnSubmit(Obj);
            this.Save();
        }

        public List<PsCategoryGroup> List()
        {
            System.Linq.IQueryable<PsCategoryGroup> Return = from Table in this.DBPrestashop.PsCategoryGroup
                                                        select Table;
            return Return.ToList();
        }

        public List<PsCategoryGroup> ListCategory(Int32 Category)
        {
            System.Linq.IQueryable<PsCategoryGroup> Return = from Table in this.DBPrestashop.PsCategoryGroup
                                                             where Table.IDCategory == Category
                                                             select Table;
            return Return.ToList();
        }

        public Boolean ExistCategoryGroup(Int32 Category, Int32 Group)
        {
            if (this.DBPrestashop.PsCategoryGroup.Count(Obj => Obj.IDCategory == Category && Obj.IDGroup == Group) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsCategoryGroup ReadCategoryGroup(Int32 Category, Int32 Group)
        {
            return this.DBPrestashop.PsCategoryGroup.FirstOrDefault(Obj => Obj.IDCategory == Category && Obj.IDGroup == Group);
        }
    }
}
