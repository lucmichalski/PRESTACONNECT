using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class CharacteristicRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(Characteristic Obj)
        {
            this.DBLocal.Characteristic.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(Characteristic Obj)
        {
            this.DBLocal.Characteristic.DeleteOnSubmit(Obj);
            this.Save();
        }
        public void DeleteAll(List<Characteristic> Obj)
        {
            this.DBLocal.Characteristic.DeleteAllOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistId(Int32 Id)
        {
            if (this.DBLocal.Characteristic.Count(Obj => Obj.Cha_Id == Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Characteristic ReadId(Int32 Id)
        {
            return this.DBLocal.Characteristic.FirstOrDefault(Obj => Obj.Cha_Id == Id);
        }

        public Boolean ExistPre_Id(Int32 Pre_Id)
        {
            if (this.DBLocal.Characteristic.Count(Obj => Obj.Pre_Id == Pre_Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Characteristic ReadPre_Id(Int32 Pre_Id)
        {
            return this.DBLocal.Characteristic.FirstOrDefault(Obj => Obj.Pre_Id == Pre_Id);
        }

        public Boolean ExistFeatureArticle(Int32 Feature, Int32 Article)
        {
            if (this.DBLocal.Characteristic.Count(Obj => Obj.Cha_IdFeature == Feature && Obj.Art_Id == Article) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Characteristic ReadFeatureArticle(Int32 Feature, Int32 Article)
        {
            return this.DBLocal.Characteristic.FirstOrDefault(Obj => Obj.Cha_IdFeature == Feature && Obj.Art_Id == Article);
        }

        public List<Characteristic> ListArticle(Int32 Article)
        {
            System.Linq.IQueryable<Characteristic> Return = from Table in this.DBLocal.Characteristic
                                                            where Table.Art_Id == Article
                                                            select Table;
            return Return.ToList();
        }
    }
}
