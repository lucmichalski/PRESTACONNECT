using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsAttributeGroupLangRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsAttributeGroupLang Obj)
        {
            this.DBPrestashop.PsAttributeGroupLang.InsertOnSubmit(Obj);
            this.Save();
        }

        public void Delete(PsAttributeGroupLang Obj)
        {
            this.DBPrestashop.PsAttributeGroupLang.DeleteOnSubmit(Obj);
            this.Save();
        }

        public List<PsAttributeGroupLang> List()
        {
            System.Linq.IQueryable<PsAttributeGroupLang> Return = from Table in this.DBPrestashop.PsAttributeGroupLang
                                                                  orderby Table.IDAttributeGroup, Table.IDLang
                                                                  select Table;
            return Return.ToList();
        }

        public List<PsAttributeGroupLang> List(uint idLang)
        {
            System.Linq.IQueryable<PsAttributeGroupLang> Return = from Table in this.DBPrestashop.PsAttributeGroupLang
                                                                  where Table.IDLang == idLang
                                                                  orderby Table.Name
                                                                  select Table;
            return Return.ToList();
        }

        public Boolean ExistAttributeGroupLang(UInt32 AttributeGroup, UInt32 Lang)
        {
            if (this.DBPrestashop.PsAttributeGroupLang.Count(Obj => Obj.IDAttributeGroup == AttributeGroup && Obj.IDLang == Lang) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsAttributeGroupLang ReadAttributeGroupLang(UInt32 AttributeGroup, UInt32 Lang)
        {
            return this.DBPrestashop.PsAttributeGroupLang.FirstOrDefault(Obj => Obj.IDAttributeGroup == AttributeGroup && Obj.IDLang == Lang);
        }

        public Boolean ExistNameLang(String Name, uint Lang)
        {
            return this.DBPrestashop.PsAttributeGroupLang.Count(Obj => Obj.Name == Name && Obj.IDLang == Lang) > 0;
        }
        public PsAttributeGroupLang ReadNameLang(String Name, uint Lang)
        {
            return this.DBPrestashop.PsAttributeGroupLang.FirstOrDefault(Obj => Obj.Name == Name && Obj.IDLang == Lang);
        }
    }
}
