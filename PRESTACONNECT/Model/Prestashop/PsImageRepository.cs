using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsImageRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        private bool ExistInShop(UInt32 IDImage, UInt32 IDShop)
        {
            return (DBPrestashop.PsImageShop.FirstOrDefault(
                result => result.IDShop == IDShop && result.IDImage == IDImage) != null);
        }

        private List<PsImageShop> ListInShop(UInt32 IDImage)
        {
            System.Linq.IQueryable<PsImageShop> Return = from Table in this.DBPrestashop.PsImageShop
                                                         where Table.IDImage == IDImage
                                                         select Table;
            return Return.ToList();
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsImage Obj, UInt32 IDShop)
        {
            this.DBPrestashop.PsImage.InsertOnSubmit(Obj);
            this.Save();

            //Si l'image n'existe pas dans la boutique, elle est rajoutée.
            if (!ExistInShop(Obj.IDImage, IDShop))
            {
                DBPrestashop.PsImageShop.InsertOnSubmit(new PsImageShop()
                {
                    IDImage = Obj.IDImage,
                    IDShop = IDShop,
					#if (PRESTASHOP_VERSION_161 || PRESTASHOP_VERSION_172)
					IDProduct = Obj.IDProduct,
                    Cover = Obj.Cover,
					#elif (PRESTASHOP_VERSION_160)
                    Cover = (sbyte)Obj.Cover,
					#elif (PREStASHOP_VERSION_15)
                    Cover = Obj.Cover,
					#endif
                });
                DBPrestashop.SubmitChanges();
            }
        }

        public void Delete(PsImage Obj)
        {
            //suppression de l'image de toutes les boutiques
            DBPrestashop.PsImageShop.DeleteAllOnSubmit(ListInShop(Obj.IDImage));

            this.DBPrestashop.PsImage.DeleteOnSubmit(Obj);
            this.Save();
        }

        public List<UInt32> ListId(UInt32 IDShop)
        {
            return DBPrestashop.ExecuteQuery<idimage>(
                "SELECT IM.id_image FROM ps_image IM " +
                " INNER JOIN ps_image_shop IMS ON IMS.id_image = IM.id_image " +
                " WHERE IMS.id_shop = {0} " +
                " ", IDShop).Select(im => im.ID_Image).ToList();
        }

        public List<PsImage> ListProduct(UInt32 Product)
        {
            return this.DBPrestashop.PsImage.Where(im => im.IDProduct == Product).ToList();
        }

        public Boolean ExistProductPosition(UInt32 Product, UInt32 Position)
        {
            return this.DBPrestashop.PsImage.Count(im => im.IDProduct == Product && im.Position == Position) > 0;
        }

        public PsImage ReadProductPosition(UInt32 Product, UInt32 Position)
        {
            return this.DBPrestashop.PsImage.FirstOrDefault(im => im.IDProduct == Product && im.Position == Position);
        }

        public Boolean ExistProductCover(UInt32 Product, Byte Cover)
        {
            return this.DBPrestashop.PsImage.Count(im => im.IDProduct == Product && im.Cover == Cover) > 0;
        }

        public static ushort NextPositionProductImage(UInt32 Product)
        {
            DataClassesPrestashop DBPs
                = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));
            if (DBPs.PsImage.Count(im => im.IDProduct == Product) > 0)
            {
                return (ushort)(DBPs.PsImage.Where(im => im.IDProduct == Product).ToList().Max(cp => cp.Position) + 1);
            }
            else
                return 1;
        }

        public Boolean ExistImage(UInt32 Image)
        {
            return this.DBPrestashop.PsImage.Count(im => im.IDImage == Image) > 0;
        }

        public PsImage ReadImage(UInt32 Image)
        {
            return this.DBPrestashop.PsImage.FirstOrDefault(im => im.IDImage == Image);
        }

        public Boolean ExistProduct(UInt32 Product)
        {
            return this.DBPrestashop.PsImage.Count(im => im.IDProduct == Product ) > 0;
        }
    }

    class idimage
    {
        public uint ID_Image = 0;
    }
}
