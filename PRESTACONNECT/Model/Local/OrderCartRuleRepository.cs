using System;
using System.Collections.Generic;
using System.Linq;

namespace PRESTACONNECT.Model.Local
{
    public class OrderCartRuleRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(OrderCartRule Obj)
        {
            this.DBLocal.OrderCartRule.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(OrderCartRule Obj)
        {
            this.DBLocal.OrderCartRule.DeleteOnSubmit(Obj);
            this.Save();
		}

		public bool Exist()
		{
			return (this.DBLocal.OrderCartRule.Count() > 0);
		}

		public bool Exist(int Pre_id)
		{
			return (this.DBLocal.OrderCartRule.Where(obj => obj.Pre_id == Pre_id).Count() > 0);
		}

		public bool Exist(OrderCartRule Obj)
		{
			return (this.DBLocal.OrderCartRule.Where(obj => obj.Pre_id == Obj.Pre_id).Count() > 0);
		}

		public OrderCartRule Read(int Pre_id)
		{
			return this.DBLocal.OrderCartRule.Where(obj => obj.Pre_id == Pre_id).FirstOrDefault();
		}

		public OrderCartRule Read(OrderCartRule Obj)
		{
			return this.DBLocal.OrderCartRule.Where(obj => obj.Pre_id == Obj.Pre_id).FirstOrDefault();
		}

		public List<OrderCartRule> List()
		{
			return this.DBLocal.OrderCartRule.ToList();
		}
	}
}
