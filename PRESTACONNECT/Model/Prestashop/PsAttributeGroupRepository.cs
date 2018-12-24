using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsAttributeGroupRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        private bool ExistInShop(UInt32 IDAttributeGroup, UInt32 IDShop)
        {
            return (DBPrestashop.PsAttributeGroupShop.FirstOrDefault(
                result => result.IDShop == IDShop && result.IDAttributeGroup == IDAttributeGroup) != null);
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsAttributeGroup Obj, UInt32 IDShop)
        {
            if (Obj.Position == 0)
            {
                List<PsAttributeGroup> groups = List(IDShop);
                Obj.Position = (groups.Count == 0) ? 1 : groups.Max(result => result.Position) + 1;
            }

            this.DBPrestashop.PsAttributeGroup.InsertOnSubmit(Obj);
            this.Save();

            //Si le groupe n'existe pas dans la boutique, il est rajouté.
            if (!ExistInShop(Obj.IDAttributeGroup, IDShop))
            {
                DBPrestashop.PsAttributeGroupShop.InsertOnSubmit(new PsAttributeGroupShop()
                {
                    IDAttributeGroup = Obj.IDAttributeGroup,
                    IDShop = IDShop,
                });
                DBPrestashop.SubmitChanges();
            }
        }

        public List<PsAttributeGroup> List(UInt32 IDShop)
        {
            //System.Linq.IQueryable<PsAttributeGroup> Return = from Table in this.DBPrestashop.PsAttributeGroup
            //                                                  select Table;
            //return Return.ToList();

            List<PsAttributeGroup> attributes = new List<PsAttributeGroup>(
                DBPrestashop.ExecuteQuery<PsAttributeGroup>(
                "SELECT DISTINCT A.id_attribute_group, A.is_color_group, A.position  FROM ps_attribute_group A " +
                " INNER JOIN ps_attribute_group_shop ATS ON ATS.id_attribute_group = A.id_attribute_group " +
                " WHERE ATS.id_shop = {0} " +
                " ", IDShop));

            return attributes;
        }

        public Boolean ExistAttributeGroup(uint AttributeGroup)
        {
            if (this.DBPrestashop.PsAttributeGroup.Count(Obj => Obj.IDAttributeGroup == AttributeGroup) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsAttributeGroup ReadAttributeGroup(uint AttributeGroup)
        {
            return this.DBPrestashop.PsAttributeGroup.FirstOrDefault(Obj => Obj.IDAttributeGroup == AttributeGroup);
        }

        public ushort NextPosition()
        {
            DataClassesPrestashop DBPs = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));
            if (DBPs.PsAttributeGroup.Count() > 0)
            {
                return (ushort)(DBPs.PsAttributeGroup.ToList().Max(ag => ag.Position) + 1);
            }
            else
                return 1;
        }
    }
}
