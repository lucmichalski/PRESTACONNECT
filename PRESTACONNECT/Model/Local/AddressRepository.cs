using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class AddressRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(Address Obj)
        {
            this.DBLocal.Address.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(Address Obj)
        {
            this.DBLocal.Address.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistPrestashop(Int32 Prestashop)
        {
            if (this.DBLocal.Address.Count(Obj => Obj.Pre_Id == Prestashop) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Address ReadSage(Int32 Sage)
        {
            return this.DBLocal.Address.FirstOrDefault(Obj => Obj.Sag_Id == Sage);
        }

        public Boolean ExistSage(Int32 Sage)
        {
            return this.DBLocal.Address.Count(Obj => Obj.Sag_Id == Sage) == 1;
        }

        public Address ReadPrestashop(Int32 Prestashop)
        {
            return this.DBLocal.Address.FirstOrDefault(Obj => Obj.Pre_Id == Prestashop);
        }

        public List<Address> List()
        {
            System.Linq.IQueryable<Address> Return = from Table in this.DBLocal.Address
                                                      select Table;
            return Return.ToList();
        }
    }
}
