using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsImageTypeRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public List<PsImageType> ListProduct(SByte Product)
        {
           IQueryable<PsImageType> Return = from Table in this.DBPrestashop.PsImageType
                                                         where Table.Products == Product
                                                         select Table;
            return Return.ToList();
        }

        public List<PsImageType> ListCategorie(SByte Categorie)
        {
            IQueryable<PsImageType> Return = from Table in this.DBPrestashop.PsImageType
                                             where Table.Categories == Categorie
                                             select Table;
            return Return.ToList();
        }
    }
}
