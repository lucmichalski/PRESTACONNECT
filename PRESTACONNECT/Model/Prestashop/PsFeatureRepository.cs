using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsFeatureRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsFeature Feature)
        {
            this.DBPrestashop.PsFeature.InsertOnSubmit(Feature);
            this.Save();
        }

        public Boolean Exist(UInt32 Feature)
        {
            return this.DBPrestashop.PsFeature.Count(f => f.IDFeature == Feature) > 0;
        }

        public PsFeature Read(UInt32 Feature)
        {
            return this.DBPrestashop.PsFeature.FirstOrDefault(f => f.IDFeature == Feature);
        }

        public List<PsFeature> List()
        {
            return this.DBPrestashop.PsFeature.ToList();
        }

        public static UInt32 NextPosition()
        {
            DataClassesPrestashop DBPs
                = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));
            if (DBPs.PsFeature.Count() > 0)
            {
                return DBPs.PsFeature.ToList().Max(cp => cp.Position) + 1;
            }
            else
                return 1;
        }
    }
}
