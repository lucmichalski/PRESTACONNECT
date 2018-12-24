using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsAttributeRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        private bool ExistInShop(UInt32 IDAttribute, UInt32 IDShop)
        {
            return (DBPrestashop.PsAttributeShop.FirstOrDefault(
                result => result.IDShop == IDShop && result.IDAttribute == IDAttribute) != null);
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsAttribute Obj, UInt32 IDShop)
        {
            this.DBPrestashop.PsAttribute.InsertOnSubmit(Obj);
            this.Save();

            //Si l'énuméré n'existe pas dans la boutique, il est rajouté.
            if (!ExistInShop(Obj.IDAttribute, IDShop))
            {
                DBPrestashop.PsAttributeShop.InsertOnSubmit(new PsAttributeShop()
                {
                    IDAttribute = Obj.IDAttribute,
                    IDShop = IDShop,
                });
                DBPrestashop.SubmitChanges();
            }
        }

		// PS 1.5 Only
        public List<PsAttribute> List(UInt32 IDAttributeGroup)
        {
            //System.Linq.IQueryable<PsAttribute> Return = from Table in this.DBPrestashop.PsAttribute
            //                                             where Table.IDAttributeGroup == IDAttributeGroup
            //                                             select Table;
            //return Return.ToList();

            List<PsAttribute> attributes = new List<PsAttribute>(
                from Table in this.DBPrestashop.PsAttribute
                where Table.IDAttributeGroup == IDAttributeGroup
                select Table);

            return attributes;
        }

        public Boolean ExistAttributeGroupAttribute(UInt32 AttributeGroup, UInt32 Attribute)
        {
            if (this.DBPrestashop.PsAttribute.Count(Obj => Obj.IDAttributeGroup == AttributeGroup && Obj.IDAttribute == Attribute) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsAttribute ReadAttributeGroupAttribute(UInt32 AttributeGroup, UInt32 Attribute)
        {
            return this.DBPrestashop.PsAttribute.FirstOrDefault(Obj => Obj.IDAttributeGroup == AttributeGroup && Obj.IDAttribute == Attribute);
        }


        public Boolean Exist(UInt32 Attribute)
        {
            return this.DBPrestashop.PsAttribute.Count(a => a.IDAttribute == Attribute) > 0;
        }
        public PsAttribute Read(UInt32 Attribute)
        {
            return this.DBPrestashop.PsAttribute.First(a => a.IDAttribute == Attribute);
        }

        public ushort NextPosition()
        {
            DataClassesPrestashop DBPs = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));
            if (DBPs.PsAttribute.Count() > 0)
            {
                return (ushort)(DBPs.PsAttribute.ToList().Max(ag => ag.Position) + 1);
            }
            else
                return 1;
        }
    }
}
