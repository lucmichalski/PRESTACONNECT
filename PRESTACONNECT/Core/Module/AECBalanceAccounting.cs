using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using PRESTACONNECT.Model.Internal;

namespace PRESTACONNECT.Core.Module
{
    public class AECBalanceAccounting
    {
        List<String> logs = new List<string>();

        public void Exec(Model.Local.Customer Customer, out List<String> log_out)
        {
            try
            {
                DateTime DebutExo = new DateTime(1900, 01, 01);
                DateTime FinExo = new DateTime(1900, 01, 01);
                //if (Core.Global.GetConfig().ModuleAECCustomerOutstandingActif && Core.Global.ExistAECCustomerOutstandingModule())
                {
                    Model.Sage.F_COMPTETRepository F_COMPTETRepository = new Model.Sage.F_COMPTETRepository();
                    Model.Prestashop.PsCustomerRepository PsCustomerRepository = new Model.Prestashop.PsCustomerRepository();

                    if (!F_COMPTETRepository.ExistId(Customer.Sag_Id))
                        logs.Add("BA10- Client Sage introuvable à partir du l'identifiant " + Customer.Sag_Id.ToString() + " !");
                    else if (!PsCustomerRepository.ExistCustomer((uint)Customer.Pre_Id))
                        logs.Add("BA11- Client PrestaShop introuvable à partir de l'identifiant " + Customer.Pre_Id.ToString() + " !");
                    else
                    {
                        List<string> log = new List<string>();
                        if (current_exercice(out DebutExo, out FinExo))
                        {
                            Model.Sage.F_COMPTET F_COMPTET = F_COMPTETRepository.Read(Customer.Sag_Id);

                            SearchBalance(F_COMPTET, (uint)Customer.Pre_Id, DebutExo, FinExo);
                            
                        }
                        if (log.Count > 0)
                            logs.AddRange(log);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("BA01- Une erreur est survenue : " + ex.ToString());
            }
            finally
            {
                log_out = logs;
            }
        }

        private Boolean current_exercice(out DateTime debutexo, out DateTime finexo)
        {
            bool is_exo = false;

            debutexo = new DateTime(1900, 01, 01);
            finexo = new DateTime(1900, 01, 01);

            DateTime today = DateTime.Now.Date, datedefault = new DateTime(1900, 01, 01);

            Model.Sage.P_DOSSIERRepository P_DOSSIER_Repository = new Model.Sage.P_DOSSIERRepository();

            if (P_DOSSIER_Repository.Exist())
            {
                Model.Sage.P_DOSSIER P_DOSSIER = P_DOSSIER_Repository.Read();

                // date de début du dernier exercice
                if (P_DOSSIER.D_DebutExo05 != null && P_DOSSIER.D_DebutExo05 > debutexo && P_DOSSIER.D_DebutExo05 <= today
                    && P_DOSSIER.D_FinExo05 != null && P_DOSSIER.D_FinExo05 > finexo && P_DOSSIER.D_FinExo05 >= today)
                {
                    debutexo = P_DOSSIER.D_DebutExo05.Value;
                    finexo = P_DOSSIER.D_FinExo05.Value;
                }
                if (P_DOSSIER.D_DebutExo04 != null && P_DOSSIER.D_DebutExo04 > debutexo && P_DOSSIER.D_DebutExo04 <= today
                    && P_DOSSIER.D_FinExo04 != null && P_DOSSIER.D_FinExo04 > finexo && P_DOSSIER.D_FinExo04 >= today)
                {
                    debutexo = P_DOSSIER.D_DebutExo04.Value;
                    finexo = P_DOSSIER.D_FinExo04.Value;
                }
                if (P_DOSSIER.D_DebutExo03 != null && P_DOSSIER.D_DebutExo03 > debutexo && P_DOSSIER.D_DebutExo03 <= today
                    && P_DOSSIER.D_FinExo03 != null && P_DOSSIER.D_FinExo03 > finexo && P_DOSSIER.D_FinExo03 >= today)
                {
                    debutexo = P_DOSSIER.D_DebutExo03.Value;
                    finexo = P_DOSSIER.D_FinExo03.Value;
                }
                if (P_DOSSIER.D_DebutExo02 != null && P_DOSSIER.D_DebutExo02 > debutexo && P_DOSSIER.D_DebutExo02 <= today
                    && P_DOSSIER.D_FinExo02 != null && P_DOSSIER.D_FinExo02 > finexo && P_DOSSIER.D_FinExo02 >= today)
                {
                    debutexo = P_DOSSIER.D_DebutExo02.Value;
                    finexo = P_DOSSIER.D_FinExo02.Value;
                }
                if (P_DOSSIER.D_DebutExo01 != null && P_DOSSIER.D_DebutExo01 > debutexo && P_DOSSIER.D_DebutExo01 <= today
                    && P_DOSSIER.D_FinExo01 != null && P_DOSSIER.D_FinExo01 > finexo && P_DOSSIER.D_FinExo01 >= today)
                {
                    debutexo = P_DOSSIER.D_DebutExo01.Value;
                    finexo = P_DOSSIER.D_FinExo01.Value;
                }
                //logs.Add("exercice pris en compte : " + debutexo.Date.ToShortDateString() + " / " + finexo.Date.ToShortDateString());

                is_exo = (debutexo != datedefault && finexo != datedefault);

            }
            else
            {
                logs.Add("BA20- Impossible de récupérer les dates de début et/ou de fin de l'exercice courant !");
            }

            return is_exo;
        }
        
        public void SearchBalance(Model.Sage.F_COMPTET F_COMPTET, uint PsCustomer, DateTime DebutExo, DateTime FinExo)
        {
            try
            {
                Model.Prestashop.PsAECBalanceAccountingRepository PsAECBalanceAccountingRepository = new Model.Prestashop.PsAECBalanceAccountingRepository();
                PsAECBalanceAccountingRepository.Delete(PsAECBalanceAccountingRepository.ListCustomer(PsCustomer));

                Model.Sage.F_ECRITURECRepository F_ECRITURECRepository = new Model.Sage.F_ECRITURECRepository();
                List<Model.Sage.F_ECRITUREC> ListF_ECRITUREC = F_ECRITURECRepository.ListTiers(F_COMPTET.CT_Num, DebutExo, FinExo);
                
                List<Model.Prestashop.PsAEcBalanceAccounting> temp = new List<Model.Prestashop.PsAEcBalanceAccounting>();
                foreach(Model.Sage.F_ECRITUREC F_ECRITUREC in ListF_ECRITUREC)
                {
                    temp.Add(new Model.Prestashop.PsAEcBalanceAccounting()
                    {
                        IDCustomer = PsCustomer,
                        DateAdd = F_ECRITUREC.EC_Date.Value,
                        DateTerm = F_ECRITUREC.EC_Echeance.Value,
                        CreditAmount = (F_ECRITUREC.EC_Sens == (short)ABSTRACTION_SAGE.F_ECRITUREC.Obj._Enum_EC_Sens.Credit && F_ECRITUREC.EC_Montant.HasValue) ? F_ECRITUREC.EC_Montant.Value : 0,
                        DebitAmount = (F_ECRITUREC.EC_Sens == (short)ABSTRACTION_SAGE.F_ECRITUREC.Obj._Enum_EC_Sens.Debit && F_ECRITUREC.EC_Montant.HasValue) ? F_ECRITUREC.EC_Montant.Value : 0,
                        Description = F_ECRITUREC.EC_Intitule,
                        InvoiceNumber = F_ECRITUREC.EC_RefPiece,
                        Lettering = (F_ECRITUREC.EC_Lettre == (short)ABSTRACTION_SAGE.F_ECRITUREC.Obj._Enum_Boolean.Oui) ? F_ECRITUREC.EC_Lettrage : string.Empty,
                    });
                }
                PsAECBalanceAccountingRepository.Add(temp);
            }
            catch (Exception ex)
            {
                logs.Add("BA30- Erreur lecture écritures comptables" + ex.ToString());
            }
        }
    }
}
