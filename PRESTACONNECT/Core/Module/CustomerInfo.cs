using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Core.Module
{
    public class CustomerInfo
    {
        public void Exec(Model.Sage.F_COMPTET F_COMPTET, Model.Prestashop.PsCustomer PsCustomer)
        {
            try
            {
                if (Core.Global.ExistCustomerInfoModule())
                {
                    Model.Prestashop.PsCustomerInfoRepository PsCustomerInfoRepository = new Model.Prestashop.PsCustomerInfoRepository();
                    if (PsCustomerInfoRepository.ExistCustomer(PsCustomer.IDCustomer))
                    {
                        Model.Prestashop.PsCustomerInfo PsCustomerInfo = PsCustomerInfoRepository.ReadCustomer(PsCustomer.IDCustomer);
                        if(string.IsNullOrWhiteSpace(PsCustomerInfo.CustomerNumber))
                        {
                            PsCustomerInfo.CustomerNumber = F_COMPTET.CT_Num;
                            PsCustomerInfoRepository.Save();
                        }
                    }
                    else
                    {
                        PsCustomerInfoRepository.Add(new Model.Prestashop.PsCustomerInfo()
                        {
                            IDCustomer = PsCustomer.IDCustomer,
                            CustomerNumber = F_COMPTET.CT_Num,
                            IDStoreCustomer = 0,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
    }
}
