using System;
using System.Collections.Generic;
using System.Linq;

namespace PRESTACONNECT.Model.Local
{
    public class CountryRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(Country Obj)
        {
            this.DBLocal.Country.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(Country Obj)
        {
            this.DBLocal.Country.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistCountry(UInt32 PreId)
        {
            return this.DBLocal.Country.Count(Obj => Obj.Pre_IdCountry == PreId) == 1;
        }
        public Country ReadCountry(UInt32 PreId)
        {
            return this.DBLocal.Country.FirstOrDefault(Obj => Obj.Pre_IdCountry == PreId);
        }
        
        public List<Country> List()
        {
            return this.DBLocal.Country.ToList();
        }

    }
}
