using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Core.Sync
{
    public class SynchronisationCatalogue
    {
        public void Exec(Int32 CatalogSend)
        {
            try
            {
                Model.Local.CatalogRepository CatalogRepository = new Model.Local.CatalogRepository();
                Model.Local.Catalog Catalog = CatalogRepository.ReadId(CatalogSend);
                // if the catalog have a parent
                Boolean isSync = false;
                uint IdParent = 0;
                if (Catalog.Cat_Parent != 0)
                {
                    if (CatalogRepository.ExistId(Catalog.Cat_Parent))
                    {
                        Model.Local.Catalog CatalogParent = CatalogRepository.ReadId(Catalog.Cat_Parent);
                        if (CatalogParent.Pre_Id != null && CatalogParent.Pre_Id != 0)
                        {
                            IdParent = (uint)CatalogParent.Pre_Id.Value;
                            isSync = true;
                        }
                    }
                }
                // default IdParent is defined on shop in Prestashop
                else
                {
                    isSync = true;
                    IdParent = Global.CurrentShop.IDCategory;
                }


                if (isSync == true)
                {
                    Boolean isCategory = false;
                    Model.Prestashop.PsCategory Category = new Model.Prestashop.PsCategory();
                    Model.Prestashop.PsCategoryRepository CategoryRepository = new Model.Prestashop.PsCategoryRepository();
                    // If the Catalog have a connection with Prestashop
                    if (Catalog.Pre_Id != null)
                    {
                        Catalog.Cat_Date = Catalog.Cat_Date.AddMilliseconds(-Catalog.Cat_Date.Millisecond);
                        if (CategoryRepository.ExistId(Catalog.Pre_Id.Value))
                        {
                            Category = CategoryRepository.ReadId((UInt32)Catalog.Pre_Id.Value);
                            isCategory = true;

                            if (Category.DateUpd.Ticks > Catalog.Cat_Date.Ticks)
                                this.ExecDistantLocal(Category, Catalog, CatalogRepository);
                            else if (Category.DateUpd.Ticks < Catalog.Cat_Date.Ticks)
                                this.ExecLocalDistant(Catalog, Category, CatalogRepository, CategoryRepository, isCategory, IdParent);
                            else
                                ExecGroupsLocalDistant(Category);
                        }
                    }
                    // We need to sync Catalog with Category
                    else
                    {
                        this.ExecLocalDistant(Catalog, Category, CatalogRepository, CategoryRepository, isCategory, IdParent);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void ExecLocalDistant(Model.Local.Catalog Catalog, Model.Prestashop.PsCategory Category, Model.Local.CatalogRepository CatalogRepository, Model.Prestashop.PsCategoryRepository CategoryRepository, Boolean isCategory, uint IdParent)
        {
            try
            {
                bool flag_move = (Category.IDParent != IdParent);

                Core.Temp.SyncCatalogue_ClearSmartyCache = true;
                Core.Temp.SyncCatalogue_RegenerateTree = true;
                // Assign data from Catalog to Category
                //Category.Position = (uint)Catalog.Cat_Position;
                Category.Active = Convert.ToByte(Catalog.Cat_Active);
                Category.LevelDepth = (Byte)Catalog.Cat_Level;
                Category.IDParent = IdParent;
                Category.DateUpd = Catalog.Cat_Date;
                // use cron for function "regenerateEntireNtree"
                //Category.NLeft = CategoryRepository.ReadId(IdParent).NLeft;
                //Category.NRight = CategoryRepository.ReadId(IdParent).NRight;
                Category.IDShopDefault = Global.CurrentShop.IDShop;
                if (isCategory == false)
                {
                    Category.DateAdd = Category.DateUpd;
                    Category.IsRootCategory = Convert.ToSByte(false);
                    Category.Position = 0;
                    CategoryRepository.Add(Category, Global.CurrentShop.IDShop);
                    // We assign the CategoryId to Catalog
                    Catalog.Pre_Id = (Int32)Category.IDCategory;
                    CatalogRepository.Save();
                }
                else
                {
                    CategoryRepository.Save();

                    if (flag_move)
                    {
                        Model.Prestashop.PsCategoryShopRepository PsCategoryShopRepository = new Model.Prestashop.PsCategoryShopRepository();
                        if (PsCategoryShopRepository.ExistCategoryShop(Category.IDCategory, Core.Global.CurrentShop.IDShop))
                        {
                            Model.Prestashop.PsCategoryShop PsCategoryShop = PsCategoryShopRepository.ReadCategoryShop(Category.IDCategory, Core.Global.CurrentShop.IDShop);
                            PsCategoryShop.Position = new Model.Prestashop.PsCategoryRepository().ReadNextPosition(1, Category, (uint)Core.Global.CurrentShop.IDShop);
                            PsCategoryShopRepository.Save();

                            Category.Position = PsCategoryShop.Position;
                            CategoryRepository.Save();
                        }
                    }
                }

                // We need to update CategoryLang too
                Boolean isCategoryLang = false;
                Model.Prestashop.PsCategoryLangRepository CategoryLangRepository = new Model.Prestashop.PsCategoryLangRepository();
                Model.Prestashop.PsCategoryLang CategoryLang = new Model.Prestashop.PsCategoryLang();
                if (CategoryLangRepository.ExistCategoryLang((Int32)Category.IDCategory, Core.Global.Lang, Global.CurrentShop.IDShop))
                {
                    CategoryLang = CategoryLangRepository.ReadCategoryLang((Int32)Category.IDCategory, Core.Global.Lang, Global.CurrentShop.IDShop);
                    isCategoryLang = true;
                }
                CategoryLang.Name = Core.Global.RemovePurge(Catalog.Cat_Name, 128);
                CategoryLang.Description = Catalog.Cat_Description;
                CategoryLang.LinkRewrite = Core.Global.ReadLinkRewrite(Catalog.Cat_LinkRewrite);
                CategoryLang.MetaTitle = Core.Global.RemovePurge(Catalog.Cat_MetaTitle, 70);
                CategoryLang.MetaDescription = Core.Global.RemovePurge(Catalog.Cat_MetaDescription, 160);
                CategoryLang.MetaKeywords = Core.Global.RemovePurgeMeta(Catalog.Cat_MetaKeyword, 255);

                if (isCategoryLang == false)
                {
                    CategoryLang.IDShop = Global.CurrentShop.IDShop;
                    CategoryLang.IDLang = (uint)Core.Global.Lang;
                    CategoryLang.IDCategory = Category.IDCategory;
                    CategoryLangRepository.Add(CategoryLang);
                }
                else
                {
                    CategoryLangRepository.Save();
                }


                // <JG> 26/12/2012 ajout insertion autres langues actives si non renseignées
                try
                {
                    Model.Prestashop.PsLangRepository PsLangRepository = new Model.Prestashop.PsLangRepository();
                    foreach (Model.Prestashop.PsLang PsLang in PsLangRepository.ListActive(1, Global.CurrentShop.IDShop))
                    {
                        if (!CategoryLangRepository.ExistCategoryLang((int)Category.IDCategory, PsLang.IDLang, Global.CurrentShop.IDShop))
                        {
                            CategoryLang = new Model.Prestashop.PsCategoryLang();
                            CategoryLang.IDShop = Global.CurrentShop.IDShop;
                            CategoryLang.IDCategory = Category.IDCategory;
                            CategoryLang.IDLang = PsLang.IDLang;
                            CategoryLang.Name = Core.Global.RemovePurge(Catalog.Cat_Name, 128);
                            CategoryLang.Description = Catalog.Cat_Description;
                            CategoryLang.LinkRewrite = Core.Global.ReadLinkRewrite(Catalog.Cat_LinkRewrite);
                            CategoryLang.MetaTitle = Core.Global.RemovePurge(Catalog.Cat_MetaTitle, 70);
                            CategoryLang.MetaDescription = Core.Global.RemovePurge(Catalog.Cat_MetaDescription, 160);
                            CategoryLang.MetaKeywords = Core.Global.RemovePurgeMeta(Catalog.Cat_MetaKeyword, 255);
                            CategoryLangRepository.Add(CategoryLang);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Core.Error.SendMailError(ex.ToString());
                }

                this.ExecGroupsLocalDistant(Category);

                // We need to send pictures
                this.ExecLocalDistantImage(Catalog, Category);
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        // 19/09/2012
        private void ExecGroupsLocalDistant(Model.Prestashop.PsCategory Category)
        {
            try
            {
                // We need to update CategoryGroup too
                Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                Model.Local.GroupRepository GroupRepository = new Model.Local.GroupRepository();
                Model.Prestashop.PsGroupRepository PsGroupRepository = new Model.Prestashop.PsGroupRepository();

                foreach (var group in GroupRepository.List())
                    if (group.Grp_ShowCatalog && PsGroupRepository.ExistGroup(Convert.ToInt32(group.Grp_Pre_Id)))
                    {
                        Model.Prestashop.PsGroup Group = PsGroupRepository.ReadGroup(Convert.ToInt32(group.Grp_Pre_Id));

                        Model.Prestashop.PsCategoryGroupRepository CategoryGroupRepository = new Model.Prestashop.PsCategoryGroupRepository();
                        if (CategoryGroupRepository.ExistCategoryGroup((Int32)Category.IDCategory, (Int32)Group.IDGroup) == false)
                        {
                            Model.Prestashop.PsCategoryGroup CategoryGroup = new Model.Prestashop.PsCategoryGroup();
                            CategoryGroup.IDCategory = Category.IDCategory;
                            CategoryGroup.IDGroup = Group.IDGroup;
                            CategoryGroupRepository.Add(CategoryGroup);
                            Core.Temp.SyncCatalogue_ClearSmartyCache = true;
                            Core.Temp.SyncCatalogue_RegenerateTree = true;
                        }
                    }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void ExecLocalDistantImage(Model.Local.Catalog Catalog, Model.Prestashop.PsCategory PsCategory)
        {
            // We need to send pictures
            Model.Local.CatalogImageRepository CatalogImageRepository = new Model.Local.CatalogImageRepository();
            Model.Local.CatalogImage CatalogImage = CatalogImageRepository.ReadCatalog(Catalog.Cat_Id, true);

            if (CatalogImage != null)
            {
                Model.Prestashop.PsImageTypeRepository PsImageTypeRepository = new Model.Prestashop.PsImageTypeRepository();
                List<Model.Prestashop.PsImageType> ListPsImageType = PsImageTypeRepository.ListCategorie(1);
                if (Core.Global.GetConfig().ConfigFTPActive)
                {
                    String FTP = Core.Global.GetConfig().ConfigFTPIP;
                    String User = Core.Global.GetConfig().ConfigFTPUser;
                    String Password = Core.Global.GetConfig().ConfigFTPPassword;

                    string extension = System.IO.Path.GetExtension(CatalogImage.ImaCat_Image);

                    String PathImgTmp = System.IO.Path.Combine(Global.GetConfig().Folders.TempCatalog, String.Format("{0}" + extension, CatalogImage.ImaCat_Id));
                    if (System.IO.File.Exists(PathImgTmp))
                    {
                        string ftpfullpath = FTP + "/img/c/" + PsCategory.IDCategory + ".jpg";
                        System.Net.FtpWebRequest ftp = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(ftpfullpath);
                        ftp.Credentials = new System.Net.NetworkCredential(User, Password);
                        //userid and password for the ftp server to given  

                        ftp.UseBinary = true;
                        ftp.UsePassive = true;
                        ftp.EnableSsl = Core.Global.GetConfig().ConfigFTPSSL;
                        ftp.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
                        System.IO.FileStream fs = System.IO.File.OpenRead(PathImgTmp);
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        fs.Close();
                        System.IO.Stream ftpstream = ftp.GetRequestStream();
                        ftpstream.Write(buffer, 0, buffer.Length);
                        ftpstream.Close();
                        ftp.Abort();

                        foreach (Model.Prestashop.PsImageType PsImageType in ListPsImageType)
                        {
                            String PathImg = System.IO.Path.Combine(Global.GetConfig().Folders.RootCatalog, String.Format("{0}-{1}" + extension, CatalogImage.ImaCat_Id, PsImageType.Name));
                            if (System.IO.File.Exists(PathImg))
                            {
                                ftpfullpath = FTP + "/img/c/" + PsCategory.IDCategory + "-" + PsImageType.Name + ".jpg";
                                ftp = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(ftpfullpath);
                                ftp.Credentials = new System.Net.NetworkCredential(User, Password);
                                //userid and password for the ftp server to given  

                                ftp.UseBinary = true;
                                ftp.UsePassive = true;
                                ftp.EnableSsl = Core.Global.GetConfig().ConfigFTPSSL;
                                ftp.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
                                fs = System.IO.File.OpenRead(PathImg);
                                buffer = new byte[fs.Length];
                                fs.Read(buffer, 0, buffer.Length);
                                fs.Close();
                                ftpstream = ftp.GetRequestStream();
                                ftpstream.Write(buffer, 0, buffer.Length);
                                ftpstream.Close();
                                ftp.Abort();
                            }
                        }
                        Core.Temp.SyncCatalogue_ClearSmartyCache = true;
                        Core.Temp.SyncCatalogue_RegenerateTree = true;
                    }
                }
            }
        }

        private void ExecDistantLocal(Model.Prestashop.PsCategory Category, Model.Local.Catalog Catalog, Model.Local.CatalogRepository CatalogRepository)
        {
            try
            {
                // Recovery Data From CatalogLang
                Model.Prestashop.PsCategoryLangRepository CategoryLangRepository = new Model.Prestashop.PsCategoryLangRepository();
                Model.Prestashop.PsCategoryLang CategoryLang = new Model.Prestashop.PsCategoryLang();
                if (CategoryLangRepository.ExistCategoryLang((Int32)Category.IDCategory, Core.Global.Lang, Global.CurrentShop.IDShop))
                {
                    CategoryLang = CategoryLangRepository.ReadCategoryLang((Int32)Category.IDCategory, Core.Global.Lang, Global.CurrentShop.IDShop);
                    Catalog.Cat_Name = CategoryLang.Name;
                    Catalog.Cat_Description = CategoryLang.Description;
                    Catalog.Cat_MetaTitle = CategoryLang.MetaTitle;
                    Catalog.Cat_MetaKeyword = CategoryLang.MetaKeywords;
                    Catalog.Cat_MetaDescription = CategoryLang.MetaDescription;
                    Catalog.Cat_LinkRewrite = CategoryLang.LinkRewrite;

                }

                bool find_parent = false;
                if (Category.IDParent != Core.Global.CurrentShop.IDCategory)
                {
                    if (!CatalogRepository.ExistPre_Id((int)Category.IDParent))
                    {
                        // todo import catalogue depuis prestashop
                        Core.ImportPrestashop.ImportCatalogue ImportCatalogue = new ImportPrestashop.ImportCatalogue();
                        ImportCatalogue.Exec((int)Category.IDParent);
                        find_parent = CatalogRepository.ExistPre_Id((int)Category.IDParent);
                    }
                    else
                    {
                        find_parent = true;
                    }
                    if (find_parent)
                    {
                        Catalog.Cat_Level = Category.LevelDepth;
                    }
                }
                else
                {
                    Catalog.Cat_Level = Category.LevelDepth;
                }

                //Catalog.Cat_Position = (Int32)Category.Position;
                Catalog.Cat_Active = Convert.ToBoolean(Category.Active);
                Catalog.Cat_Date = (Category.DateUpd != null && Category.DateUpd > new DateTime(1753, 1, 2)) ? Category.DateUpd : DateTime.Now.Date;
                CatalogRepository.Save();

                int parent = (Category.IDParent != Core.Global.CurrentShop.IDCategory)
                    ? CatalogRepository.ReadPre_Id((int)Category.IDParent).Cat_Id
                    : 0;
                CatalogRepository.WriteParent(Catalog.Cat_Id, parent);
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
    }
}
