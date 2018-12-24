using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class OrderMailRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(OrderMail Obj)
        {
            this.DBLocal.OrderMail.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(OrderMail Obj)
        {
            this.DBLocal.OrderMail.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistType(Int32 Type)
        {
            if (this.DBLocal.OrderMail.Count(Obj => Obj.OrdMai_Type == Type) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public OrderMail ReadType(Int32 Type)
        {
            return this.DBLocal.OrderMail.FirstOrDefault(Obj => Obj.OrdMai_Type == Type);
        }

        public List<OrderMail> List()
        {
            System.Linq.IQueryable<OrderMail> Return = from Table in this.DBLocal.OrderMail
                                                   select Table;
            return Return.ToList();
        }
    }
}
