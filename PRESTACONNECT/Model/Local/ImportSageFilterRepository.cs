using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class ImportSageFilterRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(ImportSageFilter obj)
        {
            this.DBLocal.ImportSageFilter.InsertOnSubmit(obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(ImportSageFilter obj)
        {
            this.DBLocal.ImportSageFilter.DeleteOnSubmit(obj);
            this.Save();
        }

        public Boolean ExistValue(string value)
        {
            return this.DBLocal.ImportSageFilter.Count(t => t.Imp_Value == value) > 0;
        }

        public ImportSageFilter ReadValue(string value)
        {
            return this.DBLocal.ImportSageFilter.FirstOrDefault(t => t.Imp_Value == value);
        }

        public List<ImportSageFilter> List()
        {
            return (from Table in this.DBLocal.ImportSageFilter
                    select Table).ToList();
        }

        public List<ImportSageFilter> ListActive()
        {
            return (from Table in this.DBLocal.ImportSageFilter
                    where Table.Imp_Active
                    select Table).ToList();
        }
    }
}
