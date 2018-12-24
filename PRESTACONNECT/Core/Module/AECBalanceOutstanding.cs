using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using PRESTACONNECT.Model.Internal;

namespace PRESTACONNECT.Core.Module
{
    public class AECBalanceOutstanding
    {
        public static Core.Parametres.P_BaseEncours ParametrePortefeuille = Core.Parametres.P_BaseEncours.NonDefini;

        List<String> logs = new List<string>();

        public void Exec(Model.Local.Customer Customer, out List<String> log_out)
        {
            //logs.Add("Client ps " + Customer.Pre_Id.ToString() + " / sage " + Customer.Sag_Id.ToString());
            try
            {
                //if (Core.Global.GetConfig().ModuleAECCustomerOutstandingActif && Core.Global.ExistAECCustomerOutstandingModule())
                {
                    //logs.Add("module encours actif");
                    Model.Sage.F_COMPTETRepository F_COMPTETRepository = new Model.Sage.F_COMPTETRepository();
                    Model.Prestashop.PsCustomerRepository PsCustomerRepository = new Model.Prestashop.PsCustomerRepository();

                    if (!F_COMPTETRepository.ExistId(Customer.Sag_Id))
                        logs.Add("BO10- Client Sage introuvable à partir du l'identifiant " + Customer.Sag_Id.ToString() + " !");
                    else if (!PsCustomerRepository.ExistCustomer((uint)Customer.Pre_Id))
                        logs.Add("BO11- Client PrestaShop introuvable à partir de l'identifiant " + Customer.Pre_Id.ToString() + " !");
                    else
                    {
                        //logs.Add("données client récupérées");
                        List<string> log = new List<string>();
                        try
                        {
                            DateTime DebutExo = new DateTime(1900, 01, 01);
                            DateTime FinExo = new DateTime(1900, 01, 01);
                            if (current_exercice(out DebutExo, out FinExo))
                            {
                                //logs.Add("Exercice " + DebutExo.ToShortDateString() + " / " + FinExo.Date.ToShortDateString());
                                if (ParametrePortefeuille == Core.Parametres.P_BaseEncours.NonDefini)
                                    read_param_portefeuille();

                                //logs.Add("Paramètre portefeuille " + ParametrePortefeuille.ToString());

                                Model.Sage.F_COMPTET F_COMPTET = F_COMPTETRepository.Read(Customer.Sag_Id);

                                Decimal EncoursAutorise = (F_COMPTET.CT_Encours != null) ? F_COMPTET.CT_Encours.Value : 0;
                                Decimal SoldeComptable, Portefeuille;
                                LoadControleEncours(F_COMPTET, DebutExo, FinExo, out SoldeComptable, out Portefeuille);


                                //logs.Add("client : " + F_COMPTET.CT_Num 
                                //+ " / type encours : " + F_COMPTET.EtatControleEncours.ToString()
                                //+ " / autorisé : " + EncoursAutorise
                                //+ " / solde comptable : " + SoldeComptable
                                //+ " / portefeuille : " + Portefeuille);


                                #region Saisie PrestaShop

                                Model.Prestashop.PsCustomer PsCustomer = PsCustomerRepository.ReadCustomer((uint)Customer.Pre_Id);
                                PsCustomer.OutstandingAllowAmount = EncoursAutorise;
                                PsCustomerRepository.Save();

                                Model.Prestashop.PsAECBalanceOutstandingRepository PsAECBalanceOutstandingRepository = new Model.Prestashop.PsAECBalanceOutstandingRepository();
                                Model.Prestashop.PsAEcBalanceOutstanding encours_client;
                                if (PsAECBalanceOutstandingRepository.ExistCustomer(PsCustomer.IDCustomer))
                                {
                                    encours_client = PsAECBalanceOutstandingRepository.ReadCustomer(PsCustomer.IDCustomer);
                                    encours_client.SageOutstanding = SoldeComptable;
                                    encours_client.SageWallet = Portefeuille;
                                    encours_client.OutstandingAllowAmount = EncoursAutorise;

                                    switch (F_COMPTET.EtatControleEncours)
                                    {
                                        case PRESTACONNECT.Model.Sage.P_CRISQUE._Enum_R_Type.Livraison:
                                            encours_client.Oversee = 0;
                                            encours_client.AccountLocked = 0;
                                            break;
                                        case PRESTACONNECT.Model.Sage.P_CRISQUE._Enum_R_Type.Surveillance:
                                            encours_client.Oversee = 1;
                                            encours_client.AccountLocked = 0;
                                            break;
                                        case PRESTACONNECT.Model.Sage.P_CRISQUE._Enum_R_Type.Blocage:
                                            encours_client.Oversee = 0;
                                            encours_client.AccountLocked = 1;
                                            break;
                                    }
                                    PsAECBalanceOutstandingRepository.Save();
                                }
                                else
                                {
                                    encours_client = new Model.Prestashop.PsAEcBalanceOutstanding()
                                    {
                                        IDCustomer = PsCustomer.IDCustomer,
                                        SageOutstanding = SoldeComptable,
                                        SageWallet = Portefeuille,
                                        OutstandingAllowAmount = EncoursAutorise,
                                    };

                                    switch (F_COMPTET.EtatControleEncours)
                                    {
                                        case PRESTACONNECT.Model.Sage.P_CRISQUE._Enum_R_Type.Livraison:
                                            encours_client.Oversee = 0;
                                            encours_client.AccountLocked = 0;
                                            break;
                                        case PRESTACONNECT.Model.Sage.P_CRISQUE._Enum_R_Type.Surveillance:
                                            encours_client.Oversee = 1;
                                            encours_client.AccountLocked = 0;
                                            break;
                                        case PRESTACONNECT.Model.Sage.P_CRISQUE._Enum_R_Type.Blocage:
                                            encours_client.Oversee = 0;
                                            encours_client.AccountLocked = 1;
                                            break;
                                    }
                                    PsAECBalanceOutstandingRepository.Add(encours_client);
                                }

                                #endregion

                            }
                        }
                        catch (Exception ex)
                        {
                            log.Add("BO02- Erreur calcul suivi comptable : " + ex.ToString());
                        }
                        if (log.Count > 0)
                            logs.AddRange(log);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("BO01- Une erreur est survenue : " + ex.ToString());
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

                //logs.Add(debutexo.Date.ToShortDateString() + " / " + finexo.Date.ToShortDateString());

                is_exo = (debutexo != datedefault && finexo != datedefault);

            }
            else
            {
                logs.Add("BO20- Impossible de récupérer les dates de début et/ou de fin de l'exercice courant !");
            }

            return is_exo;
        }

        private void read_param_portefeuille()
        {
            Model.Sage.P_PARAMETRECIALRepository P_PARAMETRECIALRepository = new Model.Sage.P_PARAMETRECIALRepository();
            if (P_PARAMETRECIALRepository.Exist())
            {
                Model.Sage.P_PARAMETRECIAL P_PARAMETRECIAL = P_PARAMETRECIALRepository.Read();

                switch (P_PARAMETRECIAL.P_BaseEncours)
                {
                    case (short)Core.Parametres.P_BaseEncours.SoldeComptable:
                        ParametrePortefeuille = Core.Parametres.P_BaseEncours.SoldeComptable;
                        break;
                    case (short)Core.Parametres.P_BaseEncours.SC_FA:
                        ParametrePortefeuille = Core.Parametres.P_BaseEncours.SC_FA;
                        break;
                    case (short)Core.Parametres.P_BaseEncours.SC_FA_BL:
                        ParametrePortefeuille = Core.Parametres.P_BaseEncours.SC_FA_BL;
                        break;
                    case (short)Core.Parametres.P_BaseEncours.SC_FA_BL_PL:
                        ParametrePortefeuille = Core.Parametres.P_BaseEncours.SC_FA_BL_PL;
                        break;
                    case (short)Core.Parametres.P_BaseEncours.SC_FA_BL_PL_BC:
                        ParametrePortefeuille = Core.Parametres.P_BaseEncours.SC_FA_BL_PL_BC;
                        break;
                }
            }
            else
            {
                logs.Add("BO21- Impossible de récupérer le paramètre de calcul de la base de l'encours !");
            }
        }

        public void LoadControleEncours(Model.Sage.F_COMPTET F_COMPTET, DateTime DebutExo, DateTime FinExo, out Decimal SoldeComptable, out Decimal Portefeuille)
        {
            Model.Sage.P_CRISQUE._Enum_R_Type EtatControle = Model.Sage.P_CRISQUE._Enum_R_Type.Livraison;

            Portefeuille = 0;
            SoldeComptable = 0;

            if (F_COMPTET.CT_ControlEnc == null)
            {
                // pas d'action = livraison
            }
            else if (F_COMPTET.CT_ControlEnc == (short)ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_CT_ControlEnc.Compte_Bloque)
            {
                EtatControle = Model.Sage.P_CRISQUE._Enum_R_Type.Blocage;
            }
            // si compte client en "controle automatique" ou "selon code risque"
            else
            {
                Model.Sage.P_CRISQUE P_CRISQUE;
                Model.Sage.P_CRISQUERepository P_CRISQUERepository = new Model.Sage.P_CRISQUERepository();
                if (P_CRISQUERepository.ExistCRisque(F_COMPTET.N_Risque.Value))
                {
                    P_CRISQUE = P_CRISQUERepository.ReadCRisque(F_COMPTET.N_Risque.Value);

                    if (P_CRISQUE.R_Type != null)
                    {
                        EtatControle = (Model.Sage.P_CRISQUE._Enum_R_Type)P_CRISQUE.R_Type;
                    }
                }
            }

            // envoi des informations sur l'encours du client

            // récupération du montant d'encours
            Model.Sage.DP_ENCOURSTIERSRepository DP_ENCOURSTIERSRepository = new Model.Sage.DP_ENCOURSTIERSRepository();
            Model.Sage.DP_ENCOURSTIERS DP_ENCOURSTIERS = DP_ENCOURSTIERSRepository.ReadTiers2(F_COMPTET.CT_Num, DebutExo, FinExo);
            SoldeComptable = DP_ENCOURSTIERS.SoldeComptable;

            Decimal LimiteTiers = 0;
            if (F_COMPTET.CT_Encours != null)
            {
                LimiteTiers = (Decimal)F_COMPTET.CT_Encours;
            }

            //Decimal DepassementMax = 0;
            //Decimal DepassementMini = 0;
            //if(P_CRISQUE.R_Min != null)
            //{
            //    DepassementMini = (Decimal)P_CRISQUE.R_Min;
            //}
            //if(P_CRISQUE.R_Max != null)
            //{
            //    DepassementMax = (Decimal)P_CRISQUE.R_Max;
            //}

            // récupération du portefeuille selon le paramétrage de Sage
            Model.Sage.DP_PORTEFEUILLEVENTERepository DP_PORTEFEUILLEVENTRepository = new Model.Sage.DP_PORTEFEUILLEVENTERepository();
            Model.Sage.DP_PORTEFEUILLEVENTE DP_PORTEFEUILLEVENTE = DP_PORTEFEUILLEVENTRepository.ReadTiers2(F_COMPTET.CT_Num, ParametrePortefeuille);
            Portefeuille = DP_PORTEFEUILLEVENTE.MontantPortefeuille;

            //solde = SoldeComptable + Portefeuille;
            //NouvelEncours = SoldeComptable + MontantDocument;
            //Depassement = NouvelEncours - LimiteTiers;

            F_COMPTET.EtatControleEncours = EtatControle;
        }
    }
}
