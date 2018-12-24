using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Local
{
    public class CatalogRepository
    {
        private DataClassesLocalDataContext DBLocal = new DataClassesLocalDataContext();

        public void Add(Catalog Obj)
        {
            this.DBLocal.Catalog.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Save()
        {
            this.DBLocal.SubmitChanges();
        }

        public void Delete(Catalog Obj)
        {
            this.DBLocal.Catalog.DeleteOnSubmit(Obj);
            this.Save();
        }

        public Boolean ExistId(Int32 Id)
        {
            if (this.DBLocal.Catalog.Count(Obj => Obj.Cat_Id == Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Boolean ExistNameLevelParent(String Name, Int32 Level, Int32 Parent)
        {
            return this.DBLocal.Catalog.Count(Obj => Obj.Cat_Name == Name && Obj.Cat_Level == Level && Obj.Cat_Parent == Parent) > 0;
        }

        public Catalog ReadNameLevelParent(String Name, Int32 Level, Int32 Parent)
        {
            return this.DBLocal.Catalog.FirstOrDefault(c => c.Cat_Name == Name && c.Cat_Level == Level && c.Cat_Parent == Parent);
        }

        public Boolean ExistParent(Int32 Parent)
        {
            return this.DBLocal.Catalog.Count(result => result.Cat_Parent == Parent && result.Cat_Id != Parent) > 0;
        }

        public Catalog ReadId(Int32 Id)
        {
            return this.DBLocal.Catalog.FirstOrDefault(Obj => Obj.Cat_Id == Id);
        }

        public Boolean ExistSag_Id(Int32 Sag_Id)
        {
            if (this.DBLocal.Catalog.Count(Obj => Obj.Sag_Id == Sag_Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Boolean ExistPre_Id(Int32 Pre_Id)
        {
            if (this.DBLocal.Catalog.Count(Obj => Obj.Pre_Id == Pre_Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Boolean ExistSag_IdSync(Int32 Sag_Id, Boolean Sync)
        {
            if (this.DBLocal.Catalog.Count(Obj => Obj.Sag_Id == Sag_Id && Obj.Cat_Sync == Sync) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Catalog ReadSag_Id(Int32 Sag_Id)
        {
            return this.DBLocal.Catalog.FirstOrDefault(Obj => Obj.Sag_Id == Sag_Id);
        }

        public Catalog ReadPre_Id(Int32 Pre_Id)
        {
            return this.DBLocal.Catalog.FirstOrDefault(Obj => Obj.Pre_Id == Pre_Id);
        }

        public List<Catalog> List()
        {
            IQueryable<Catalog> Return = from Table in this.DBLocal.Catalog
                                         select Table;
            return Return.ToList().OrderBy(c => c.ComboText).ToList();
        }

        public List<CatalogLight> ListLight()
        {
            List<CatalogLight> Return = (from Table in this.DBLocal.Catalog
                                         where Table.Pre_Id != null
                                         select new CatalogLight { Cat_Id = Table.Cat_Id, Pre_Id = (int)Table.Pre_Id }).ToList();
            return Return;
        }

        public List<Catalog> ListOrderByLevel()
        {
            IQueryable<Catalog> Return = from Table in this.DBLocal.Catalog
                                         orderby Table.Cat_Level ascending
                                         select Table;
            return Return.ToList();
        }

        public List<Catalog> ListSyncOrderByLevel(Boolean Sync)
        {
            IQueryable<Catalog> Return = from Table in this.DBLocal.Catalog
                                         where Table.Cat_Sync == Sync
                                         orderby Table.Cat_Level ascending
                                         select Table;
            return Return.ToList();
        }

        public List<Int32> ListIdSyncOrderByLevel(Boolean Sync)
        {
            IQueryable<Int32> Return = from Table in this.DBLocal.Catalog
                                       where Table.Cat_Sync == Sync
                                       orderby Table.Cat_Level ascending
                                       select Table.Cat_Id;
            return Return.ToList();
        }
        public List<Int32> ListId()
        {
            IQueryable<Int32> Return = from Table in this.DBLocal.Catalog
                                       select Table.Cat_Id;
            return Return.ToList();
        }
        public List<Int32> ListId(Boolean Sync)
        {
            IQueryable<Int32> Return = from Table in this.DBLocal.Catalog
                                       where Table.Cat_Sync == Sync
                                       select Table.Cat_Id;
            return Return.ToList();
        }

        public List<Catalog> ListParent(Int32 Parent)
        {
            IQueryable<Catalog> Return = from Table in this.DBLocal.Catalog
                                         where Table.Cat_Parent == Parent && Table.Cat_Id != Parent
                                         select Table;
            return Return.ToList();
        }

        public List<Catalog> RootList()
        {
            return this.DBLocal.Catalog.Where(result => result.Cat_Parent == 0 && result.Cat_Id != 0).ToList();
        }

        public List<Catalog> ListSageId(Int32 Sag_Id)
        {
            return this.DBLocal.Catalog.Where(r => r.Sag_Id == Sag_Id).ToList();
        }
        
        public void WriteParent(Int32 Catalog, Int32 Parent)
        {
            this.DBLocal.ExecuteQuery<int>("UPDATE [Catalog] SET Cat_Parent = {0} WHERE Cat_Id = {1}", Parent, Catalog);
        }
    }
}
