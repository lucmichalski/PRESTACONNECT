using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class OrderRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(Order Obj)
        {
            this.DBLocal.Order.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(Order Obj)
        {
            this.DBLocal.Order.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistPrestashop(Int32 Prestashop)
        {
            if (this.DBLocal.Order.Count(Obj => Obj.Pre_Id == Prestashop) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Order ReadPrestashop(Int32 Prestashop)
        {
            return this.DBLocal.Order.FirstOrDefault(Obj => Obj.Pre_Id == Prestashop);
        }


        public Boolean ExistSage(Int32 Sage)
        {
            if (this.DBLocal.Order.Count(Obj => Obj.Sag_Id == Sage) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Order ReadSage(Int32 Sage)
        {
            return this.DBLocal.Order.FirstOrDefault(Obj => Obj.Sag_Id == Sage);
        }

        public List<Order> List()
        {
            System.Linq.IQueryable<Order> Return = from Table in this.DBLocal.Order
                                                    select Table;
            return Return.ToList();
        }

        public List<int> ListPrestaShop()
        {
            return (from Table in this.DBLocal.Order
                    select Table.Pre_Id).ToList();
        }
    }
}
