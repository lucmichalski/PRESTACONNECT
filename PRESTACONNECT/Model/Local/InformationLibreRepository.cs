using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class InformationLibreRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(InformationLibre Obj)
        {
            this.DBLocal.InformationLibre.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(InformationLibre Obj)
        {
            this.DBLocal.InformationLibre.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistInfoLibre(String InfoLibre)
        {
            return this.DBLocal.InformationLibre.Count(Obj => Obj.Sag_InfoLibre == InfoLibre) > 0;
        }
        public Boolean ExistInfoLibreArgSyntax(String InfoLibre)
        {
            return this.DBLocal.InformationLibre.Count(Obj => Obj.Sag_InfoLibre.Replace(" ", "_") == InfoLibre) > 0;
        }

        public InformationLibre ReadInfoLibre(String InfoLibre)
        {
            return this.DBLocal.InformationLibre.FirstOrDefault(Obj => Obj.Sag_InfoLibre == InfoLibre);
        }

        public Boolean ExistFeature(UInt32 Feature)
        {
            return this.DBLocal.InformationLibre.Count(Obj => Obj.Cha_Id == Feature) > 0;
        }

        public InformationLibre ReadFeature(UInt32 Feature)
        {
            return this.DBLocal.InformationLibre.FirstOrDefault(Obj => Obj.Cha_Id == Feature);
        }

        public List<InformationLibre> List()
        {
            return this.DBLocal.InformationLibre.ToList();
        }

        public List<InformationLibre> ListSync()
        {
            return this.DBLocal.InformationLibre.Where(i => i.Inf_Mode != (short)Core.Parametres.InformationLibreValeursMode.NonTransferees && i.Cha_Id != 0).ToList();
        }
    }
}
