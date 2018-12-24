using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Core.ImportSage
{
    public class ImportFournisseur
    {
        public void Exec(Int32 F_COMPTETSend)
        {
            try
            {
                Model.Local.SupplierRepository SupplierRepository = new Model.Local.SupplierRepository();
                if (SupplierRepository.ExistSag_Id(F_COMPTETSend) == false)
                {
                    Model.Sage.F_COMPTETRepository F_COMPTETRepository = new Model.Sage.F_COMPTETRepository();
                    Model.Sage.F_COMPTET F_COMPTET = F_COMPTETRepository.Read(F_COMPTETSend);
                    Model.Local.Supplier Supplier = new Model.Local.Supplier()
                    {
                        Sup_Active = true,
                        Sup_Description = F_COMPTET.CT_Intitule,
                        Sup_MetaDescription = F_COMPTET.CT_Intitule,
                        Sup_MetaKeyword = F_COMPTET.CT_Intitule,
                        Sup_Name = F_COMPTET.CT_Intitule,
                        Sup_MetaTitle = F_COMPTET.CT_Intitule,
                        Sup_Date = DateTime.Now,
                        Sup_Sync = true,
                        Sag_Id = F_COMPTET.cbMarq
                    };
                    SupplierRepository.Add(Supplier);
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

    }
}
