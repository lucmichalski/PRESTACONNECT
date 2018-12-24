using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class SettlementRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(Settlement Obj)
        {
            this.DBLocal.Settlement.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(Settlement Obj)
        {
            this.DBLocal.Settlement.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistId(Int32 Id)
        {
            if (this.DBLocal.Settlement.Count(Obj => Obj.Set_Id == Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Settlement ReadId(Int32 Id)
        {
            return this.DBLocal.Settlement.FirstOrDefault(Obj => Obj.Set_Id == Id);
        }

        public Boolean ExistPayment(String Payment)
        {
            if (this.DBLocal.Settlement.Count(Obj => Obj.Set_PaymentMethod.ToUpper() == Payment.ToUpper()) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Settlement ReadPayment(String Payment)
        {
            return this.DBLocal.Settlement.FirstOrDefault(Obj => Obj.Set_PaymentMethod.ToUpper() == Payment.ToUpper());
        }

        public Boolean ExistPaymentPartiel(String Payment)
        {
            return this.DBLocal.Settlement.ToList().Count(r => Payment.StartsWith(r.Set_PaymentMethod)) > 0;
        }
        public Settlement ReadPaymentPartiel(String Payment)
        {
            return this.DBLocal.Settlement.ToList().FirstOrDefault(r => Payment.StartsWith(r.Set_PaymentMethod));
        }

        public List<Settlement> List()
        {
            System.Linq.IQueryable<Settlement> Return = from Table in this.DBLocal.Settlement
                                                      select Table;
            return Return.ToList();
        }
    }
}
