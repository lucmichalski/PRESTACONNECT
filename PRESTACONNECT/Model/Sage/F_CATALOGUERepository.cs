using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public class F_CATALOGUERepository
    {
        private DataClassesSageDataContext DBSage = new DataClassesSageDataContext();

        public Boolean ExistParent(Int32 Parent)
        {
            if (this.DBSage.F_CATALOGUE.Count(Obj => Obj.CL_No == Parent) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Boolean ExistCatalogue(Int32 Catalog)
        {
            if (this.DBSage.F_CATALOGUE.Count(Obj => Obj.cbMarq == Catalog) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //public Boolean ExistIntituleNiveau(String Intitule, short Niveau)
        //{
        //    if (this.DBSage.F_CATALOGUE.Count(Obj => Obj.CL_Intitule.ToUpper() == Intitule.ToUpper() && Obj.CL_Niveau == Niveau) == 0)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}

        public Boolean ExistNumero(Int32 Numero)
        {
            if (this.DBSage.F_CATALOGUE.Count(Obj => Obj.CL_No == Numero) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public F_CATALOGUE ReadParent(Int32 Parent)
        {
            return this.DBSage.F_CATALOGUE.FirstOrDefault(Obj => Obj.CL_No == Parent);
        }

        //public F_CATALOGUE ReadIntituleNiveau(String Intitule, short Niveau)
        //{
        //    return this.DBSage.F_CATALOGUE.FirstOrDefault(Obj => Obj.CL_Intitule.ToUpper() == Intitule.ToUpper() && Obj.CL_Niveau == Niveau);
        //}
        public IEnumerable<F_CATALOGUE> ReadIntituleNiveau(String Intitule, short Niveau)
        {
            var query = from catalog in DBSage.F_CATALOGUE
                        where ((catalog.CL_Intitule.ToUpper() == Intitule.ToUpper()) && (catalog.CL_Niveau == Niveau))
                        select catalog;

            return query;
        }

        public F_CATALOGUE ReadCatalogue(Int32 Id)
        {
            return this.DBSage.F_CATALOGUE.FirstOrDefault(Obj => Obj.cbMarq == Id);
        }

        public F_CATALOGUE ReadNumero(Int32 Numero)
        {
            return this.DBSage.F_CATALOGUE.FirstOrDefault(Obj => Obj.CL_No == Numero);
        }

        public List<F_CATALOGUE> ListOrderByNiveauIntitule()
        {
            System.Linq.IQueryable<F_CATALOGUE> Return = from Table in this.DBSage.F_CATALOGUE
                                                         orderby Table.CL_Niveau ascending, Table.CL_Intitule ascending
                                                         select Table;
            return Return.ToList();
        }

        public List<Int32> ListIdOrderByNiveauIntitule()
        {
            System.Linq.IQueryable<Int32> Return = from Table in this.DBSage.F_CATALOGUE
                                                   orderby Table.CL_Niveau ascending, Table.CL_Intitule ascending
                                                   select Table.cbMarq;
            return Return.ToList();
        }

        public List<F_CATALOGUE> List()
        {
            return this.DBSage.F_CATALOGUE.ToList().OrderBy(c => c.ComboText).ToList();
        }

        public List<F_CATALOGUE> RootList()
        {
            return this.DBSage.F_CATALOGUE.Where(result => result.CL_Niveau == 0).ToList();
        }

        public List<F_CATALOGUE> ListParent(Int32 Parent)
        {
            System.Linq.IQueryable<F_CATALOGUE> Return = from Table in this.DBSage.F_CATALOGUE
                                                         where Table.CL_NoParent == Parent
                                                         select Table;
            return Return.ToList();
        }
    }
}
