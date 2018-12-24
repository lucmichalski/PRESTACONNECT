using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class OrderPaymentRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(OrderPayment Obj)
        {
            this.DBLocal.OrderPayment.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(OrderPayment Obj)
        {
            this.DBLocal.OrderPayment.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistPayment(Int32 Payment)
        {
            return this.DBLocal.OrderPayment.Count(Obj => Obj.Pre_Id_Order_Payment == Payment) > 0;
        }

        public OrderPayment ReadPayment(Int32 Payment)
        {
            return this.DBLocal.OrderPayment.FirstOrDefault(Obj => Obj.Pre_Id_Order_Payment == Payment);
        }

        public Boolean ExistOrder(Int32 Order)
        {
            return this.DBLocal.OrderPayment.Count(o => o.Pre_Id_Order == Order) > 0;
        }

        public Int32 CountOrder(Int32 Order)
        {
            return this.DBLocal.OrderPayment.Count(o => o.Pre_Id_Order == Order);
        }
        
        public List<OrderPayment> ListOrder(Int32 Order)
        {
            System.Linq.IQueryable<OrderPayment> Return = from Table in this.DBLocal.OrderPayment
                                                          where Table.Pre_Id_Order == Order
                                                          select Table;
            return Return.ToList();
        }
    }
}
