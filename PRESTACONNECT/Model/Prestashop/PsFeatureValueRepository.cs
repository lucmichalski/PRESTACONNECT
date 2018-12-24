using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsFeatureValueRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsFeatureValue Obj)
        {
            this.DBPrestashop.PsFeatureValue.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Delete(PsFeatureValue Obj)
        {
            this.DBPrestashop.PsFeatureValue.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistFeatureValue(UInt32 FeatureValue)
        {
            if (this.DBPrestashop.PsFeatureValue.Count(Obj => Obj.IDFeatureValue == FeatureValue) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsFeatureValue ReadFeatureValue(UInt32 FeatureValue)
        {
            return this.DBPrestashop.PsFeatureValue.FirstOrDefault(Obj => Obj.IDFeatureValue == FeatureValue);
        }

        public List<PsFeatureValue> ListFeature(UInt32 Feature)
        {
            System.Linq.IQueryable<PsFeatureValue> Return;
            Return = from Table in this.DBPrestashop.PsFeatureValue
                     where Table.IDFeature == Feature
                     select Table;
            return Return.ToList();
        }

        public List<PsFeatureValue> ListFeatureCustom(UInt32 Feature, byte custom)
        {
            System.Linq.IQueryable<PsFeatureValue> Return;
            Return = from Table in this.DBPrestashop.PsFeatureValue
                     where Table.IDFeature == Feature && Table.Custom == custom
                     select Table;
            return Return.ToList();
        }
    }
}
