using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Core.Transfert
{
    public class TransfertStatInfoLibreClient
    {
        public void Exec(Int32 SageClientSend)
        {
            try
            {
                if (Core.Global.GetConfig().StatInfolibreClientActif)
                {
                    Model.Local.CustomerRepository CustomerRepository = new Model.Local.CustomerRepository();
                    if (CustomerRepository.ExistSage(SageClientSend))
                    {
                        Model.Local.Customer Customer = CustomerRepository.ReadSage(SageClientSend);
                        Model.Sage.F_COMPTETRepository F_COMPTETRepository = new Model.Sage.F_COMPTETRepository();
                        if (F_COMPTETRepository.ExistId(Customer.Sag_Id))
                        {
                            Model.Sage.F_COMPTET F_COMPTET = F_COMPTETRepository.Read(Customer.Sag_Id);

                            #region Informations libre client

                            Model.Local.InformationLibreClientRepository InformationLibreClientRepository = new Model.Local.InformationLibreClientRepository();
                            foreach (Model.Local.InformationLibreClient InformationLibreClient in InformationLibreClientRepository.ListSync())
                            {
                                Model.Sage.cbSysLibreRepository.CB_Type TypeInfoLibre = new Model.Sage.cbSysLibreRepository().ReadTypeInformationLibre(InformationLibreClient.Sag_InfoLibreClient, Model.Sage.cbSysLibreRepository.CB_File.F_COMPTET);
                                UInt32 IDCustomerFeatureValue = 0;
                                switch (TypeInfoLibre)
                                {
                                    case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageText:
                                    case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageTable:
                                        #region text
                                        if (F_COMPTETRepository.ExistArticleInformationLibreText(InformationLibreClient.Sag_InfoLibreClient, F_COMPTET.CT_Num)
                                            && !string.IsNullOrWhiteSpace(Core.Global.SageValueReplacement(F_COMPTETRepository.ReadArticleInformationLibreText(InformationLibreClient.Sag_InfoLibreClient, F_COMPTET.CT_Num))))
                                        {
                                            IDCustomerFeatureValue = CreateCustomerFeatureValue(Core.Global.SageValueReplacement(F_COMPTETRepository.ReadArticleInformationLibreText(InformationLibreClient.Sag_InfoLibreClient, F_COMPTET.CT_Num)), (uint)InformationLibreClient.Cha_Id, InformationLibreClient.Inf_Mode);
                                        }
                                        AssociateCustomerFeatureValueWithCustomer((uint)InformationLibreClient.Cha_Id, (uint)Customer.Pre_Id, IDCustomerFeatureValue);
                                        #endregion
                                        break;

                                    case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageValeur:
                                    case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageMontant:
                                        #region decimal
                                        if (F_COMPTETRepository.ExistArticleInformationLibreNumerique(InformationLibreClient.Sag_InfoLibreClient, F_COMPTET.CT_Num)
                                            && F_COMPTETRepository.ReadArticleInformationLibreNumerique(InformationLibreClient.Sag_InfoLibreClient, F_COMPTET.CT_Num) != null)
                                        {
                                            IDCustomerFeatureValue = CreateCustomerFeatureValue(Core.Global.SageValueReplacement(F_COMPTETRepository.ReadArticleInformationLibreNumerique(InformationLibreClient.Sag_InfoLibreClient, F_COMPTET.CT_Num).ToString()), (uint)InformationLibreClient.Cha_Id, InformationLibreClient.Inf_Mode);
                                        }
                                        AssociateCustomerFeatureValueWithCustomer((uint)InformationLibreClient.Cha_Id, (uint)Customer.Pre_Id, IDCustomerFeatureValue);
                                        #endregion
                                        break;

                                    case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageDate:
                                    case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageSmallDate:
                                        #region datetime
                                        if (F_COMPTETRepository.ExistArticleInformationLibreDate(InformationLibreClient.Sag_InfoLibreClient, F_COMPTET.CT_Num)
                                            && F_COMPTETRepository.ReadArticleInformationLibreDate(InformationLibreClient.Sag_InfoLibreClient, F_COMPTET.CT_Num) != null)
                                        {
                                            IDCustomerFeatureValue = CreateCustomerFeatureValue(Core.Global.SageValueReplacement(F_COMPTETRepository.ReadArticleInformationLibreDate(InformationLibreClient.Sag_InfoLibreClient, F_COMPTET.CT_Num).ToString()), (uint)InformationLibreClient.Cha_Id, InformationLibreClient.Inf_Mode);
                                        }
                                        AssociateCustomerFeatureValueWithCustomer((uint)InformationLibreClient.Cha_Id, (uint)Customer.Pre_Id, IDCustomerFeatureValue);
                                        #endregion
                                        break;

                                    case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.Deleted:
                                    default:
                                        break;
                                }
                            }

                            #endregion

                            #region Statistiques client

                            Model.Local.StatistiqueClientRepository StatistiqueClientRepository = new Model.Local.StatistiqueClientRepository();
                            foreach (Model.Local.StatistiqueClient StatistiqueClient in StatistiqueClientRepository.ListSync())
                            {
                                Model.Sage.P_STATISTIQUERepository P_STATISTIQUERepository = new Model.Sage.P_STATISTIQUERepository();
                                if (P_STATISTIQUERepository.ExistStatClient(StatistiqueClient.Sag_StatClient))
                                {
                                    Model.Sage.P_STATISTIQUE P_STATISTIQUE = P_STATISTIQUERepository.ReadStatClient(StatistiqueClient.Sag_StatClient);
                                    String stat_value = null;
                                    switch (P_STATISTIQUE.cbMarq)
                                    {
                                        case 1:
                                            stat_value = F_COMPTET.CT_Statistique01;
                                            break;
                                        case 2:
                                            stat_value = F_COMPTET.CT_Statistique02;
                                            break;
                                        case 3:
                                            stat_value = F_COMPTET.CT_Statistique03;
                                            break;
                                        case 4:
                                            stat_value = F_COMPTET.CT_Statistique04;
                                            break;
                                        case 5:
                                            stat_value = F_COMPTET.CT_Statistique05;
                                            break;
                                        case 6:
                                            stat_value = F_COMPTET.CT_Statistique06;
                                            break;
                                        case 7:
                                            stat_value = F_COMPTET.CT_Statistique07;
                                            break;
                                        case 8:
                                            stat_value = F_COMPTET.CT_Statistique08;
                                            break;
                                        case 9:
                                            stat_value = F_COMPTET.CT_Statistique09;
                                            break;
                                        case 10:
                                            stat_value = F_COMPTET.CT_Statistique10;
                                            break;
                                    }
                                    UInt32 IDCustomerFeatureValue = 0;
                                    if (!String.IsNullOrWhiteSpace(stat_value))
                                    {
                                        IDCustomerFeatureValue = CreateCustomerFeatureValue(Core.Global.SageValueReplacement(stat_value), (uint)StatistiqueClient.Cha_Id, StatistiqueClient.Inf_Mode);
                                    }
                                    AssociateCustomerFeatureValueWithCustomer((uint)StatistiqueClient.Cha_Id, (uint)Customer.Pre_Id, IDCustomerFeatureValue);
                                }
                            }

                            #endregion

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private UInt32 CreateCustomerFeatureValue(String InfoLibreValue, UInt32 CustomerFeature, short ModeTransfert)
        {
            UInt32 r = 0;
            try
            {
                Model.Prestashop.PsCustomerFeatureValueLangRepository PsCustomerFeatureValueLangRepository = new Model.Prestashop.PsCustomerFeatureValueLangRepository();
                if (PsCustomerFeatureValueLangRepository.ExistCustomerFeatureValueLang(InfoLibreValue, Core.Global.Lang))
                {
                    r = PsCustomerFeatureValueLangRepository.ReadCustomerFeatureValueLang(InfoLibreValue, Core.Global.Lang).IDCustomerFeatureValue;
                }
                else
                {
                    Model.Prestashop.PsCustomerFeatureValue PsCustomerFeatureValue = new Model.Prestashop.PsCustomerFeatureValue()
                    {
                        Custom = (ModeTransfert == (short)Core.Parametres.InformationLibreValeursMode.Personnalisees) ? (byte)1 : (byte)0,
                        IDCustomerFeature = CustomerFeature
                    };
                    new Model.Prestashop.PsCustomerFeatureValueRepository().Add(PsCustomerFeatureValue);
                    r = PsCustomerFeatureValue.IDCustomerFeatureValue;
                    foreach (Model.Prestashop.PsLang lang in new Model.Prestashop.PsLangRepository().ListActive(1, Core.Global.CurrentShop.IDShop))
                        new Model.Prestashop.PsCustomerFeatureValueLangRepository().Add(new Model.Prestashop.PsCustomerFeatureValueLang()
                        {
                            IDLang = lang.IDLang,
                            IDCustomerFeatureValue = PsCustomerFeatureValue.IDCustomerFeatureValue,
                            Value = InfoLibreValue
                        });
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
            return r;
        }

        private void AssociateCustomerFeatureValueWithCustomer(UInt32 CustomerFeature, UInt32 Customer, UInt32 CustomerFeatureValue)
        {
            try
            {
                Model.Prestashop.PsCustomerFeatureCustomerRepository PsCustomerFeatureCustomerRepository = new Model.Prestashop.PsCustomerFeatureCustomerRepository();
                Model.Prestashop.PsCustomerFeatureCustomer PsCustomerFeatureCustomer = new Model.Prestashop.PsCustomerFeatureCustomer();

                Boolean isFeatureProduct = false;

                if (PsCustomerFeatureCustomerRepository.ExistCustomerFeatureCustomer(CustomerFeature, Customer))
                {
                    PsCustomerFeatureCustomer = PsCustomerFeatureCustomerRepository.ReadCustomerFeatureCustomer(CustomerFeature, Customer);
                    isFeatureProduct = true;
                }
                if (CustomerFeatureValue != 0)
                {
                    PsCustomerFeatureCustomer.IDCustomerFeatureValue = CustomerFeatureValue;
                    PsCustomerFeatureCustomer.IDCustomer = Customer;
                    PsCustomerFeatureCustomer.IDCustomerFeature = CustomerFeature;

                    if (isFeatureProduct)
                        PsCustomerFeatureCustomerRepository.Save();
                    else
                        PsCustomerFeatureCustomerRepository.Add(PsCustomerFeatureCustomer);
                }
                else if (isFeatureProduct)
                    PsCustomerFeatureCustomerRepository.Delete(PsCustomerFeatureCustomer);
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
    }
}
