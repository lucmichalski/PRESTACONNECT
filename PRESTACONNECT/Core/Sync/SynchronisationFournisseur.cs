using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Core.Sync
{
    public class SynchronisationFournisseur
    {
        // Void Exec : Sync Supplier
        public void Exec(Int32 LocalSupplierSend)
        {
            try
            {
                Model.Local.SupplierRepository LocalSupplierRepository = new Model.Local.SupplierRepository();
                Model.Local.Supplier LocalSupplier = LocalSupplierRepository.ReadId(LocalSupplierSend);
                Model.Prestashop.PsSupplierRepository SupplierRepository = new Model.Prestashop.PsSupplierRepository();
                Boolean isSupplier = false;
                Model.Prestashop.PsSupplier Supplier = new Model.Prestashop.PsSupplier();

                // If the LocalSupplier have a connection with Prestashop
                if (LocalSupplier.Pre_Id != null)
                {
                    LocalSupplier.Sup_Date = LocalSupplier.Sup_Date.AddMilliseconds(-LocalSupplier.Sup_Date.Millisecond);
                    if (SupplierRepository.ExistId(LocalSupplier.Pre_Id.Value))
                    {
                        Supplier = SupplierRepository.ReadId(LocalSupplier.Pre_Id.Value);
                        isSupplier = true;
                        if (Supplier.DateUpd.Ticks > LocalSupplier.Sup_Date.Ticks)
                        {
                            this.ExecDistantLocal(Supplier, LocalSupplier, LocalSupplierRepository);
                        }
                        else if (Supplier.DateUpd.Ticks < LocalSupplier.Sup_Date.Ticks)
                        {
                            this.ExecLocalDistant(LocalSupplier, Supplier, LocalSupplierRepository, SupplierRepository, isSupplier);
                        }
                    }
                }
                // We need to sync LocalSupplier with Prestashop
                else
                {
                    this.ExecLocalDistant(LocalSupplier, Supplier, LocalSupplierRepository, SupplierRepository, isSupplier);
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void ExecLocalDistant(Model.Local.Supplier LocalSupplier, Model.Prestashop.PsSupplier Supplier, Model.Local.SupplierRepository LocalSupplierRepository, Model.Prestashop.PsSupplierRepository SupplierRepository, Boolean isSupplier)
        {
            try
            {
                // Assign data from LocalSupplier to Supplier
                Supplier.Name = LocalSupplier.Sup_Name;
                Supplier.DateUpd = LocalSupplier.Sup_Date;
                Supplier.Active = Convert.ToSByte(LocalSupplier.Sup_Active);
                // If the LocalSupplier have not a connection with Prestashop, We need to add Supplier
                if (isSupplier == false)
                {
                    Supplier.DateAdd = Supplier.DateUpd;
                    SupplierRepository.Add(Supplier, Global.CurrentShop.IDShop);
                    // We assign the SupplierId to LocalSupplier
                    LocalSupplier.Pre_Id = (Int32)Supplier.IDSupplier;
                    LocalSupplierRepository.Save();
                }
                // else we need to update Supplier
                else
                {
                    SupplierRepository.Save();
                }

                // We need to update SupplierLang too
                Boolean isSupplierLang = false;
                Model.Prestashop.PsSupplierLangRepository SupplierLangRepository = new Model.Prestashop.PsSupplierLangRepository();
                Model.Prestashop.PsSupplierLang SupplierLang = new Model.Prestashop.PsSupplierLang();
                if (SupplierLangRepository.ExistSupplierLang((Int32)Supplier.IDSupplier, Core.Global.Lang))
                {
                    SupplierLang = SupplierLangRepository.ReadSupplierLang((Int32)Supplier.IDSupplier, Core.Global.Lang);
                    isSupplierLang = true;
                }
                SupplierLang.Description = LocalSupplier.Sup_Description;
                SupplierLang.MetaTitle = LocalSupplier.Sup_MetaTitle;
                SupplierLang.MetaDescription = LocalSupplier.Sup_MetaDescription;
                SupplierLang.MetaKeywords = LocalSupplier.Sup_MetaKeyword;
                // If the SupplierLang doesn't exist, we need to add him
                if (isSupplierLang == false)
                {
                    SupplierLang.IDLang = (uint)Core.Global.Lang;
                    SupplierLang.IDSupplier = Supplier.IDSupplier;
                    SupplierLangRepository.Add(SupplierLang);
                }
                // else we need to update him
                else
                {
                    SupplierLangRepository.Save();
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void ExecDistantLocal(Model.Prestashop.PsSupplier Supplier, Model.Local.Supplier LocalSupplier, Model.Local.SupplierRepository LocalSupplierRepository)
        {
            try
            {
                // Recovery Data From SupplierLang
                Model.Prestashop.PsSupplierLangRepository SupplierLangRepository = new Model.Prestashop.PsSupplierLangRepository();
                Model.Prestashop.PsSupplierLang SupplierLang = new Model.Prestashop.PsSupplierLang();
                if (SupplierLangRepository.ExistSupplierLang((Int32)Supplier.IDSupplier, Core.Global.Lang))
                {
                    SupplierLang = SupplierLangRepository.ReadSupplierLang((Int32)Supplier.IDSupplier, Core.Global.Lang);
                    LocalSupplier.Sup_Description = SupplierLang.Description;
                    LocalSupplier.Sup_MetaTitle = SupplierLang.MetaTitle;
                    LocalSupplier.Sup_MetaKeyword = SupplierLang.MetaKeywords;
                    LocalSupplier.Sup_MetaDescription = SupplierLang.MetaDescription;

                }
                LocalSupplier.Sup_Name = Supplier.Name;
                LocalSupplier.Sup_Active = Convert.ToBoolean(Supplier.Active);
                LocalSupplier.Sup_Date = (Supplier.DateUpd != null && Supplier.DateUpd > new DateTime(1753, 1, 2)) ? Supplier.DateUpd : DateTime.Now.Date;
                LocalSupplierRepository.Save();
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
    }
}
