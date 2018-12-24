using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PRESTACONNECT.Model.Prestashop
{
    public class PsCategoryRepository
    {
        private DataClassesPrestashop DBPrestashop = new DataClassesPrestashop(new MySqlConnection(Properties.Settings.Default.PRESTASHOPConnectionString));

        public Boolean ExistChild(Int32 Category)
        {
            return this.DBPrestashop.PsCategory.Count(c => c.IDParent == Category) > 0;
        }

        public void Save()
        {
            this.DBPrestashop.SubmitChanges();
        }

        public void Add(PsCategory Obj, UInt32 IDShop)
        {
            this.DBPrestashop.PsCategory.InsertOnSubmit(Obj);
            this.Save();

            //Si la catégorie n'existe pas dans la boutique, elle est rajoutée.
            // <JG> 22/01/2014 correction insertion position par défaut dans la boutique
            Model.Prestashop.PsCategoryShopRepository PsCategoryShopRepository = new PsCategoryShopRepository();
            if (!PsCategoryShopRepository.ExistCategoryShop(Obj.IDCategory, IDShop))
            {
                Model.Prestashop.PsCategoryShop PsCategoryShop = new PsCategoryShop()
                {
                    IDCategory = Convert.ToInt32(Obj.IDCategory),
                    IDShop = Convert.ToInt32(IDShop),
                    Position = ReadNextPosition(1, Obj, IDShop),
                };
                PsCategoryShopRepository.Add(PsCategoryShop);
                Obj.Position = PsCategoryShop.Position;
                this.Save();
            }
        }

        public UInt32 ReadNextPosition(UInt32 Position, Model.Prestashop.PsCategory PsCategory, UInt32 IDShop)
        {
            if (PsCategory != null)
            {
                // si présence d'enfant dans la catégorie parente (donc catégories "soeurs" de celle à insérer)
                if(this.DBPrestashop.PsCategory.Count(c => c.LevelDepth == PsCategory.LevelDepth && c.IDParent == PsCategory.IDParent) > 0)
                {
                    List<uint> List_Children = this.DBPrestashop.PsCategory.Where(c => c.LevelDepth == PsCategory.LevelDepth && c.IDParent == PsCategory.IDParent).Select(c => c.IDCategory).ToList();
                    List<PsCategoryShop> Listpscs = this.DBPrestashop.PsCategoryShop.Where(cs => cs.IDShop == IDShop).ToList();
                    Listpscs = Listpscs.Where(cs => List_Children.Count(c => c == cs.IDCategory) == 1).ToList();
                    List<uint> List_position = Listpscs.Select(cs => cs.Position).ToList();

                    while (List_position.Contains(Position))
                    {
                        Position += 1;
                    }
                }
            }
            return Position;
        }

        public void Delete(PsCategory Obj)
        {
            this.DBPrestashop.PsCategory.DeleteOnSubmit(Obj);
            this.Save();
        }

        public List<UInt32> ListIdOrderByLevelDepth(UInt32 IDShop, UInt32 ShopRootLevel)
        {
            //    System.Linq.IQueryable<UInt32> Return = from Table in this.DBPrestashop.PsCategory
            //                                            orderby Table.LevelDepth ascending
            //                                            select Table.IDCategory;
            //    return Return.ToList();

            //String request = "select distinct (ps_category.id_category) from ((((ps_category join ps_category ps_category_1 on((ps_category.id_parent = ps_category_1.id_category))) join ps_category_lang on((ps_category.id_category = ps_category_lang.id_category))) join ps_category_lang ps_category_lang_1 on((ps_category_1.id_category = ps_category_lang_1.id_category)))) where ((ps_category_lang_1.id_lang = 2) and (ps_category_lang.id_lang = 2)) order by ps_category_1.id_category, ps_category.position";

            String request = "SELECT DISTINCT C.id_category, C.level_depth, C.position " +
                " FROM ps_category C " +
                " INNER JOIN ps_category_shop CS ON CS.id_category = C.id_category " +
                " WHERE CS.id_shop = " + IDShop + " " +
                " AND C.level_depth > " + ShopRootLevel + " " +
                " ORDER BY C.level_depth, C.position, C.id_category ";


            List<uint> categories = new List<uint>();

            categories = (DBPrestashop.ExecuteQuery<PsCategory>(request)).Select(c => c.IDCategory).ToList();

            //List<uint> categories = new List<uint>();

            //foreach (var category in DBPrestashop.ExecuteQuery<PsCategory>(
            //    "SELECT DISTINCT C.id_category " +
            //    " FROM ps_category C " +
            //    " INNER JOIN ps_category_shop CS ON CS.id_category = C.id_category " +
            //    " WHERE CS.id_shop = {0} AND C.level_depth > 0 " +
            //    " ORDER BY C.level_depth " +
            //    " ", IDShop))
            //    categories.Add(category.IDCategory);

            return categories;
        }

        public Boolean ExistPositionLevelParent(UInt32 Position, Byte Level, UInt32 Parent)
        {
            if (this.DBPrestashop.PsCategory.Count(Obj => Obj.Position == Position
                && Obj.LevelDepth == Level
                && Obj.IDParent == Parent) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public PsCategory ReadPositionLevel_Depth(UInt32 Position, Byte Level)
        {
            return this.DBPrestashop.PsCategory.FirstOrDefault(Obj => Obj.Position == Position && Obj.LevelDepth == Level);
        }

        public Boolean ExistId(Int32 Id)
        {
            if (this.DBPrestashop.PsCategory.Count(Obj => Obj.IDCategory == Id) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public PsCategory ReadId(UInt32 Id)
        {
            return this.DBPrestashop.PsCategory.FirstOrDefault(Obj => Obj.IDCategory == Id);
        }
    }
}
