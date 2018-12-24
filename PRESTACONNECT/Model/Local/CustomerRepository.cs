using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class CustomerRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(Customer Obj)
        {
            this.DBLocal.Customer.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(Customer Obj)
        {
            this.DBLocal.Customer.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistPrestashop(Int32 Prestashop)
        {
            if (this.DBLocal.Customer.Count(Obj => Obj.Pre_Id == Prestashop) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Boolean ExistSage(Int32 Sage)
        {
            return this.DBLocal.Customer.Count(Obj => Obj.Sag_Id == Sage) > 0;
        }

        public Customer ReadPrestashop(Int32 Prestashop)
        {
            return this.DBLocal.Customer.FirstOrDefault(Obj => Obj.Pre_Id == Prestashop);
        }
        public Customer ReadSage(Int32 Sage)
        {
            return this.DBLocal.Customer.FirstOrDefault(Obj => Obj.Sag_Id == Sage);
        }


        public Boolean ExistPrestashopSage(Int32 Prestashop, Int32 Sage)
        {
            if (this.DBLocal.Customer.Count(Obj => Obj.Pre_Id == Prestashop && Obj.Sag_Id == Sage) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Customer ReadPrestashopSage(Int32 Prestashop, Int32 Sage)
        {
            return this.DBLocal.Customer.FirstOrDefault(Obj => Obj.Pre_Id == Prestashop && Obj.Sag_Id == Sage);
        }

        public List<Customer> List()
        {
            System.Linq.IQueryable<Customer> Return = from Table in this.DBLocal.Customer
                                                    select Table;
            return Return.ToList();
        }
        public IQueryable<Customer> ListQueryable()
        {
            return this.DBLocal.Customer;
        }
    }
}
