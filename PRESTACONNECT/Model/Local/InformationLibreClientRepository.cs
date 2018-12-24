using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class InformationLibreClientRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(InformationLibreClient Obj)
        {
            this.DBLocal.InformationLibreClient.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(InformationLibreClient Obj)
        {
            this.DBLocal.InformationLibreClient.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistInfoLibre(String InfoLibreClient)
        {
            return this.DBLocal.InformationLibreClient.Count(Obj => Obj.Sag_InfoLibreClient == InfoLibreClient) > 0;
        }

        public InformationLibreClient ReadInfoLibre(String InfoLibreClient)
        {
            return this.DBLocal.InformationLibreClient.FirstOrDefault(Obj => Obj.Sag_InfoLibreClient == InfoLibreClient);
        }

        public Boolean ExistCustomerFeature(UInt32 CustomerFeature)
        {
            return this.DBLocal.InformationLibreClient.Count(Obj => Obj.Cha_Id == CustomerFeature) > 0;
        }

        public InformationLibreClient ReadCustomerFeature(UInt32 CustomerFeature)
        {
            return this.DBLocal.InformationLibreClient.FirstOrDefault(Obj => Obj.Cha_Id == CustomerFeature);
        }

        public List<InformationLibreClient> List()
        {
            return this.DBLocal.InformationLibreClient.ToList();
        }

        public List<InformationLibreClient> ListSync()
        {
            return this.DBLocal.InformationLibreClient.Where(i => i.Inf_Mode != (short)Core.Parametres.InformationLibreValeursMode.NonTransferees && i.Cha_Id != 0).ToList();
        }
    }
}
