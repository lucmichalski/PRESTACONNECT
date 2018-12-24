using System;
using System.Collections.Generic;
using System.Linq;

namespace PRESTACONNECT.Model.Local
{
    public class OrderMacketplaceRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(OrderMacketplace Obj)
        {
            this.DBLocal.OrderMacketplace.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(OrderMacketplace Obj)
        {
            this.DBLocal.OrderMacketplace.DeleteOnSubmit(Obj);
            this.Save();
		}

		public bool Exist()
		{
			return (this.DBLocal.OrderMacketplace.Count() > 0);
		}

		public bool Exist(OrderMacketplace Obj)
		{
			return (this.DBLocal.OrderMacketplace.Where(obj => obj.Ord_MacketplaceId == Obj.Ord_MacketplaceId).Count() > 0);
		}

		public OrderMacketplace Read(OrderMacketplace Obj)
		{
			return this.DBLocal.OrderMacketplace.Where(obj => obj.Ord_MacketplaceId == Obj.Ord_MacketplaceId).FirstOrDefault();
		}

		public List<OrderMacketplace> List()
		{
			return this.DBLocal.OrderMacketplace.ToList();
		}
	}
}
