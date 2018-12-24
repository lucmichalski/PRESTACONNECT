using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using PRESTACONNECT.Model.Internal;

namespace PRESTACONNECT.Core.Module
{
    public class AECCustomerOutstanding
    {

        public static DateTime DebutExo = new DateTime(1900, 01, 01);
        public static DateTime FinExo = new DateTime(1900, 01, 01);
        public static Core.Parametres.P_BaseEncours ParametrePortefeuille = Core.Parametres.P_BaseEncours.NonDefini;

        List<String> logs;

        public void Exec(Model.Local.Customer Customer, out List<String> log_out)
        {
            logs = new List<string>();
            //logs.Add("Client ps " + Customer.Pre_Id.ToString() + " / sage " + Customer.Sag_Id.ToString());
            try
            {
                if (Core.Global.GetConfig().ModuleAECCustomerOutstandingActif && Core.Global.ExistAECCustomerOutstandingModule())
                {
                    //logs.Add("module encours actif");
                    Model.Sage.F_COMPTETRepository F_COMPTETRepository = new Model.Sage.F_COMPTETRepository();
                    Model.Prestashop.PsCustomerRepository PsCustomerRepository = new Model.Prestashop.PsCustomerRepository();

                    if (!F_COMPTETRepository.ExistId(Customer.Sag_Id))
                        logs.Add("TEC10- Client Sage introuvable à partir du l'identifiant " + Customer.Sag_Id.ToString() + " !");
                    else if (!PsCustomerRepository.ExistCustomer((uint)Customer.Pre_Id))
                        logs.Add("TEC11- Client PrestaShop introuvable à partir de l'identifiant " + Customer.Pre_Id.ToString() + " !");
                    else
                    {
                        //logs.Add("données client récupérées");
                        List<string> log = new List<string>();
                        if (current_exercice(out DebutExo, out FinExo))
                        {
                            //logs.Add("Exercice " + DebutExo.ToShortDateString() + " / " + FinExo.Date.ToShortDateString());
                            if (ParametrePortefeuille == Core.Parametres.P_BaseEncours.NonDefini)
                                read_param_portefeuille();

                            //logs.Add("Paramètre portefeuille " + ParametrePortefeuille.ToString());

                            Model.Sage.F_COMPTET F_COMPTET = F_COMPTETRepository.Read(Customer.Sag_Id);

                            Decimal EncoursAutorise = 0;
                            Decimal EncoursUtilise;
                            LoadControleEncours(F_COMPTET, out EncoursUtilise);


                            //logs.Add("client : " + F_COMPTET.CT_Num 
                            //+ " / type encours " + F_COMPTET.EtatControleEncours.ToString() 
                            //+ " / autorisé " + EncoursAutorise
                            //+ " / utilisé " + EncoursUtilise);

                            switch (F_COMPTET.EtatControleEncours)
                            {
                                case PRESTACONNECT.Model.Sage.P_CRISQUE._Enum_R_Type.Livraison:
                                    EncoursAutorise = (F_COMPTET.CT_Encours != null) ? F_COMPTET.CT_Encours.Value : 0;
                                    break;
                                case PRESTACONNECT.Model.Sage.P_CRISQUE._Enum_R_Type.Surveillance:
                                case PRESTACONNECT.Model.Sage.P_CRISQUE._Enum_R_Type.Blocage:
                                    if (F_COMPTET.CT_ControlEnc == (short)ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_CT_ControlEnc.Compte_Bloque)
                                        EncoursAutorise = 0;
                                    else
                                        EncoursAutorise = (F_COMPTET.CT_Encours != null) ? F_COMPTET.CT_Encours.Value : 0;
                                    break;
                            }

                            #region Saisie PrestaShop

                            Model.Prestashop.PsCustomer PsCustomer = PsCustomerRepository.ReadCustomer((uint)Customer.Pre_Id);
                            PsCustomer.OutstandingAllowAmount = EncoursAutorise;
                            PsCustomerRepository.Save();

                            Model.Prestashop.PsAECCustomerOutstandingRepository PsAECCustomerOutstandingRepository = new Model.Prestashop.PsAECCustomerOutstandingRepository();
                            Model.Prestashop.PsAEcCustomerOutstanding encours_client;
                            if (PsAECCustomerOutstandingRepository.ExistCustomer(PsCustomer.IDCustomer))
                            {
                                encours_client = PsAECCustomerOutstandingRepository.ReadCustomer(PsCustomer.IDCustomer);
                                encours_client.EncoursActuelSage = EncoursUtilise;
                                PsAECCustomerOutstandingRepository.Save();
                            }
                            else
                            {
                                PsAECCustomerOutstandingRepository.Add(new Model.Prestashop.PsAEcCustomerOutstanding()
                                {
                                    IDCustomer = PsCustomer.IDCustomer,
                                    EncoursActuelSage = EncoursUtilise,
                                });
                                if (log != null && log.Count > 0)
                                    logs.AddRange(log);
                            }

                            #endregion

                        }
                        if (log.Count > 0)
                            logs.AddRange(log);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("TEC01- Une erreur est survenue : " + ex.ToString());
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
                logs.Add("TEC20- Impossible de récupérer les dates de début et/ou de fin de l'exercice courant !");
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
                logs.Add("TEC21- Impossible de récupérer le paramètre de calcul de la base de l'encours !");
            }
        }

        public void LoadControleEncours(Model.Sage.F_COMPTET F_COMPTET, out Decimal Encours)
        {
            Model.Sage.P_CRISQUE._Enum_R_Type EtatControle = Model.Sage.P_CRISQUE._Enum_R_Type.Livraison;
            Decimal Portefeuille = 0,
                    SoldeComptable = 0;

            Encours = 0;

            if (F_COMPTET.CT_ControlEnc == null)
            {
                // pas d'action = livraison
            }
            else if (F_COMPTET.CT_ControlEnc == (short)ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_CT_ControlEnc.Compte_Bloque)
            {
                EtatControle = Model.Sage.P_CRISQUE._Enum_R_Type.Blocage;
            }
            // si compte client en "controle automatique" ou "selon code risque" alors
            else
            {
                Model.Sage.P_CRISQUE P_CRISQUE;
                Model.Sage.P_CRISQUERepository P_CRISQUERepository = new Model.Sage.P_CRISQUERepository();
                if (P_CRISQUERepository.ExistCRisque(F_COMPTET.N_Risque.Value))
                {
                    P_CRISQUE = P_CRISQUERepository.ReadCRisque(F_COMPTET.N_Risque.Value);

                    if (P_CRISQUE.R_Type != null)
                    {
                        switch (P_CRISQUE.R_Type)
                        {
                            case (short)Model.Sage.P_CRISQUE._Enum_R_Type.Livraison:
                                // aucun blocage
                                break;

                            case (short)Model.Sage.P_CRISQUE._Enum_R_Type.Surveillance:
                            case (short)Model.Sage.P_CRISQUE._Enum_R_Type.Blocage:

                                // récupération du montant d'encours
                                Model.Sage.DP_ENCOURSTIERSRepository DP_ENCOURSTIERSRepository = new Model.Sage.DP_ENCOURSTIERSRepository();
                                Model.Sage.DP_ENCOURSTIERS DP_ENCOURSTIERS = DP_ENCOURSTIERSRepository.ReadTiers(F_COMPTET.CT_Num, DebutExo, FinExo);
                                Decimal LimiteTiers = 0;
                                //Decimal DepassementMax = 0;
                                //Decimal DepassementMini = 0;
                                if (F_COMPTET.CT_Encours != null)
                                {
                                    LimiteTiers = (Decimal)F_COMPTET.CT_Encours;
                                }
                                //if(P_CRISQUE.R_Min != null)
                                //{
                                //    DepassementMini = (Decimal)P_CRISQUE.R_Min;
                                //}
                                //if(P_CRISQUE.R_Max != null)
                                //{
                                //    DepassementMax = (Decimal)P_CRISQUE.R_Max;
                                //}
                                Model.Sage.DP_PORTEFEUILLEVENTERepository DP_PORTEFEUILLEVENTRepository = new Model.Sage.DP_PORTEFEUILLEVENTERepository();

                                Model.Sage.DP_PORTEFEUILLEVENTE DP_PORTEFEUILLEVENTE = DP_PORTEFEUILLEVENTRepository.ReadTiers(F_COMPTET.CT_Num, ParametrePortefeuille);
                                Portefeuille = DP_PORTEFEUILLEVENTE.MontantPortefeuille;

                                SoldeComptable = DP_ENCOURSTIERS.SoldeComptable;

                                Encours = SoldeComptable + Portefeuille;
                                //NouvelEncours = SoldeComptable + MontantDocument;
                                //Depassement = NouvelEncours - LimiteTiers;
                                if (Encours >= LimiteTiers)
                                {
                                    if (P_CRISQUE.R_Type == (short)Model.Sage.P_CRISQUE._Enum_R_Type.Surveillance)
                                    {
                                        EtatControle = Model.Sage.P_CRISQUE._Enum_R_Type.Surveillance;
                                    }
                                    else if (P_CRISQUE.R_Type == (short)Model.Sage.P_CRISQUE._Enum_R_Type.Blocage)
                                    {
                                        EtatControle = Model.Sage.P_CRISQUE._Enum_R_Type.Blocage;
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            F_COMPTET.EtatControleEncours = EtatControle;
        }
    }
}
