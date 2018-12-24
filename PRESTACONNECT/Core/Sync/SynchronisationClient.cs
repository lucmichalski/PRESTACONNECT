using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Core.Sync
{
    public class SynchronisationClient
    {
        public void Exec(ABSTRACTION_SAGE.ALTERNETIS.Connexion Connexion, UInt32 IDCustomer)
        {
            try
            {
                Model.Prestashop.PsCustomerRepository PsCustomerRepository = new Model.Prestashop.PsCustomerRepository();
                if (PsCustomerRepository.ExistCustomer(IDCustomer))
                {
                    Model.Prestashop.PsCustomer PsCustomer = PsCustomerRepository.ReadCustomer(IDCustomer);
                    Model.Local.CustomerRepository CustomerRepository = new Model.Local.CustomerRepository();
                    if (CustomerRepository.ExistPrestashop(Convert.ToInt32(PsCustomer.IDCustomer)) == false)
                    {
                        this.ExecDistantToLocal(PsCustomer, Connexion, CustomerRepository);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void ExecDistantToLocal(Model.Prestashop.PsCustomer PsCustomer, ABSTRACTION_SAGE.ALTERNETIS.Connexion Connexion, Model.Local.CustomerRepository CustomerRepository)
        {
            try
            {
                Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                Model.Local.Config Config = new Model.Local.Config();

                ABSTRACTION_SAGE.F_COMPTET.Obj F_COMPTET = new ABSTRACTION_SAGE.F_COMPTET.Obj();

                Boolean IsCentralisation = false;
                if (ConfigRepository.ExistName(Core.Global.ConfigClientTypeLien))
                {
                    Config = ConfigRepository.ReadName(Core.Global.ConfigClientTypeLien);
                    IsCentralisation = Config.Con_Value == Core.Global.ConfigClientTypeLienEnum.CompteCentralisateur.ToString();
                }

                if (IsCentralisation)
                {
                    //TODO ajouter gestion d'un compte client centralisé par groupe de client !!! attention filtre catégorie tarifaire !!!
                    #region centralisation clients
                    if (ConfigRepository.ExistName(Core.Global.ConfigClientCompteCentralisateur))
                    {
                        Config = ConfigRepository.ReadName(Core.Global.ConfigClientCompteCentralisateur);
                        if (Core.Global.IsInteger(Config.Con_Value))
                        {
                            Int32 cbMarqCentralisateur = Int32.Parse(Config.Con_Value);
                            Model.Sage.F_COMPTETRepository F_COMPTETRepositoryCentralisateur = new Model.Sage.F_COMPTETRepository();
                            Model.Sage.F_COMPTET F_COMPTETCentralisateur;
                            if (F_COMPTETRepositoryCentralisateur.ExistId(cbMarqCentralisateur))
                            {
                                F_COMPTETCentralisateur = F_COMPTETRepositoryCentralisateur.Read(cbMarqCentralisateur);
                                if (F_COMPTETCentralisateur.CT_Sommeil == 0)
                                {
                                    // création du lien dans la base Prestaconnect
                                    Model.Local.Customer Customer = new Model.Local.Customer();
                                    Customer.Pre_Id = Convert.ToInt32(PsCustomer.IDCustomer);
                                    Customer.Sag_Id = F_COMPTETCentralisateur.cbMarq;
                                    CustomerRepository.Add(Customer);
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Insertion comptes individuels
                    Boolean Valid;

                    // <JG> 05/08/2013 à 07/08/2013 modification gestion numérotation client
                    Model.Prestashop.PsAddress PsAddress;
                    F_COMPTET.CT_Num = GenerateSageNumber(PsCustomer, out Valid, out PsAddress);

                    if (Valid)
                    {
                        F_COMPTET.CT_Type = ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_CT_Type.Client;

                        String Intitule = (Core.Global.GetConfig().ConfigClientSocieteIntituleActif && !string.IsNullOrEmpty(PsCustomer.Company))
                            ? PsCustomer.Company
                            : PsCustomer.LastName + " " + PsCustomer.FirstName;
                        String Contact = (Core.Global.GetConfig().ConfigClientSocieteIntituleActif && !string.IsNullOrEmpty(PsCustomer.Company))
                            ? PsCustomer.LastName + " " + PsCustomer.FirstName
                            : (!string.IsNullOrEmpty(PsCustomer.Company) ? PsCustomer.Company : string.Empty);

                        F_COMPTET.CT_Intitule = (Intitule.Length > 35) ? Intitule.Substring(0, 35) : Intitule;
                        F_COMPTET.CT_Classement = (Intitule.Length > 17) ? Intitule.Substring(0, 17) : Intitule;

                        F_COMPTET.CT_Contact = (Contact.Length > 35) ? Contact.Substring(0, 35) : Contact;

                        if (PsCustomer.SireT != null)
                            F_COMPTET.CT_Siret = (PsCustomer.SireT.Length > 15) ? PsCustomer.SireT.Substring(0, 15) : PsCustomer.SireT;
                        if (PsCustomer.Ape != null)
                            F_COMPTET.CT_Ape = (PsCustomer.Ape.Length > 7) ? PsCustomer.Ape.Substring(0, 7) : PsCustomer.Ape;

                        if (!string.IsNullOrWhiteSpace(PsCustomer.Website))
                            F_COMPTET.CT_Site = (PsCustomer.Website.Length > 69) ? PsCustomer.Website.Substring(0, 69) : PsCustomer.Website;

                        #region Coordonnées
                        // <JG> 16/06/2017
                        if (PsAddress != null)
                        {
                            Model.Prestashop.PsCountryLangRepository PsCountryLangRepository = new Model.Prestashop.PsCountryLangRepository();
                            if (PsCountryLangRepository.ExistCountryLang(PsAddress.IDCountry, Core.Global.Lang))
                            {
                                Model.Prestashop.PsCountryLang PsCountryLang = PsCountryLangRepository.ReadCountryLang(PsAddress.IDCountry, Core.Global.Lang);
                                F_COMPTET.CT_Pays = (PsCountryLang.Name.Length > 35) ? PsCountryLang.Name.Substring(0, 35) : PsCountryLang.Name;
                            }
                            if (!string.IsNullOrWhiteSpace(PsAddress.Phone))
                            {
                                F_COMPTET.CT_Telephone = (PsAddress.Phone.Length > 21) ? PsAddress.Phone.Substring(0, 21) : PsAddress.Phone;
                                // <JG> 17/11/2014 correction si valeur nulle (tests AM)
                                if (!string.IsNullOrWhiteSpace(PsAddress.PhoneMobile))
                                    F_COMPTET.CT_Telecopie = (PsAddress.PhoneMobile.Length > 21) ? PsAddress.PhoneMobile.Substring(0, 21) : PsAddress.PhoneMobile;
                            }
                            else if (!string.IsNullOrWhiteSpace(PsAddress.PhoneMobile))
                            {
                                if (Core.Global.GetConfig().ConfigClientAdresseTelephonePositionFixe)
                                    F_COMPTET.CT_Telecopie = (PsAddress.PhoneMobile.Length > 21) ? PsAddress.PhoneMobile.Substring(0, 21) : PsAddress.PhoneMobile;
                                else
                                    F_COMPTET.CT_Telephone = (PsAddress.PhoneMobile.Length > 21) ? PsAddress.PhoneMobile.Substring(0, 21) : PsAddress.PhoneMobile;
                            }

                            String Adresse2 = string.Empty;
                            if (PsAddress.Address1.Length > 35)
                            {
                                F_COMPTET.CT_Adresse = PsAddress.Address1.Substring(0, 35);
                                Adresse2 = PsAddress.Address1.Substring(35);
                            }
                            else
                            {
                                F_COMPTET.CT_Adresse = PsAddress.Address1;
                            }
                            if (!string.IsNullOrWhiteSpace(PsAddress.Address2))
                            {
                                Adresse2 += " " + PsAddress.Address2;
                            }

                            F_COMPTET.CT_Complement = (Adresse2.Length > 35) ? Adresse2.Substring(0, 35) : Adresse2;
                            F_COMPTET.CT_CodePostal = (PsAddress.PostCode.Length > 9) ? PsAddress.PostCode.Substring(0, 9) : PsAddress.PostCode;
                            F_COMPTET.CT_Ville = (PsAddress.City.Length > 35) ? PsAddress.City.Substring(0, 35) : PsAddress.City;

                            string tva = !string.IsNullOrWhiteSpace(PsAddress.VatNumber) ? PsAddress.VatNumber : string.Empty;
                            if (tva.Length > 25)
                                tva = tva.Replace(" ", string.Empty);
                            F_COMPTET.CT_Identifiant = (tva.Length > 25) ? tva.Substring(0, 25) : tva;
                        }

                        #endregion

                        #region Option Siret dans le NIF
                        if (Core.Global.GetConfig().ConfigClientNIFActif && string.IsNullOrWhiteSpace(F_COMPTET.CT_Siret))
                        {
                            try
                            {
                                string dni = !string.IsNullOrWhiteSpace(PsAddress.DNi) ? PsAddress.DNi.Replace(" ", string.Empty) : string.Empty;
                                Connexion.Request = "UPDATE F_COMPTET SET CT_Siret='" + ((dni.Length > 15) ? dni.Substring(0, 15) : dni)
                                     + "' WHERE CT_Num='" + F_COMPTET.CT_Num + "'";
                                Connexion.Exec_Request();
                            }
                            catch (Exception ex)
                            {
                                Core.Error.SendMailError("Erreur insertion numéro d'identification fiscale !<br/>" + ex.ToString());
                            }
                        }
                        #endregion

                        #region Paramètres commerciaux

                        Config = new Model.Local.Config();
                        if (ConfigRepository.ExistName(Core.Global.ConfigClientCompteGeneral))
                        {
                            Config = ConfigRepository.ReadName(Core.Global.ConfigClientCompteGeneral);
                            Model.Sage.F_COMPTEGRepository F_COMPTEGRepository = new Model.Sage.F_COMPTEGRepository();
                            if (F_COMPTEGRepository.ExistId(Convert.ToInt32(Config.Con_Value)))
                            {
                                Model.Sage.F_COMPTEG F_COMPTEG = F_COMPTEGRepository.ReadId(Convert.ToInt32(Config.Con_Value));
                                F_COMPTET.CG_NumPrinc = F_COMPTEG.CG_Num;
                            }
                        }

                        Config = new Model.Local.Config();
                        if (ConfigRepository.ExistName(Core.Global.ConfigClientDevise))
                        {
                            Config = ConfigRepository.ReadName(Core.Global.ConfigClientDevise);
                            F_COMPTET.N_Devise = Convert.ToInt32(Config.Con_Value);
                        }

                        Config = new Model.Local.Config();
                        if (ConfigRepository.ExistName(Core.Global.ConfigClientCodeRisque))
                        {
                            Config = ConfigRepository.ReadName(Core.Global.ConfigClientCodeRisque);
                            F_COMPTET.N_Risque = Convert.ToInt32(Config.Con_Value);
                        }

                        Config = new Model.Local.Config();
                        if (ConfigRepository.ExistName(Core.Global.ConfigClientNombreLigne))
                        {
                            Config = ConfigRepository.ReadName(Core.Global.ConfigClientNombreLigne);
                            F_COMPTET.CT_Saut = Convert.ToInt32(Config.Con_Value);
                        }

                        Config = new Model.Local.Config();
                        if (ConfigRepository.ExistName(Core.Global.ConfigClientLettrage))
                        {
                            Config = ConfigRepository.ReadName(Core.Global.ConfigClientLettrage);
                            if (Config.Con_Value == "True")
                                F_COMPTET.CT_Lettrage = ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_Boolean.Oui;
                            else
                                F_COMPTET.CT_Lettrage = ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_Boolean.Non;
                        }

                        Config = new Model.Local.Config();
                        if (ConfigRepository.ExistName(Core.Global.ConfigClientValidationAutomatique))
                        {
                            Config = ConfigRepository.ReadName(Core.Global.ConfigClientValidationAutomatique);
                            if (Config.Con_Value == "True")
                                F_COMPTET.CT_ValidEch = ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_Boolean.Oui;
                            else
                                F_COMPTET.CT_ValidEch = ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_Boolean.Non;
                        }

                        F_COMPTET.CT_Sommeil = ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_Boolean.Non;

                        Config = new Model.Local.Config();
                        if (ConfigRepository.ExistName(Core.Global.ConfigClientRappel))
                        {
                            Config = ConfigRepository.ReadName(Core.Global.ConfigClientRappel);
                            if (Config.Con_Value == "True")
                                F_COMPTET.CT_NotRappel = ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_Boolean.Oui;
                            else
                                F_COMPTET.CT_NotRappel = ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_Boolean.Non;
                        }
                        F_COMPTET.CT_Email = (PsCustomer.Email.Length > 69) ? PsCustomer.Email.Substring(0, 69) : PsCustomer.Email;

                        Config = new Model.Local.Config();
                        if (ConfigRepository.ExistName(Core.Global.ConfigClientSurveillance))
                        {
                            Config = ConfigRepository.ReadName(Core.Global.ConfigClientSurveillance);
                            if (Config.Con_Value == "True")
                                F_COMPTET.CT_Surveillance = ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_Boolean.Oui;
                            else
                                F_COMPTET.CT_Surveillance = ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_Boolean.Non;
                        }

                        Config = new Model.Local.Config();
                        if (ConfigRepository.ExistName(Core.Global.ConfigClientPrioriteLivraison))
                        {
                            Config = ConfigRepository.ReadName(Core.Global.ConfigClientPrioriteLivraison);
                            F_COMPTET.CT_PrioriteLivr = Convert.ToInt32(Config.Con_Value);
                        }

                        Config = new Model.Local.Config();
                        if (ConfigRepository.ExistName(Core.Global.ConfigClientPenalite))
                        {
                            Config = ConfigRepository.ReadName(Core.Global.ConfigClientPenalite);
                            if (Config.Con_Value == "True")
                                F_COMPTET.CT_NotPenal = 1;
                            else
                                F_COMPTET.CT_NotPenal = 0;
                        }

                        Config = new Model.Local.Config();
                        if (ConfigRepository.ExistName(Core.Global.ConfigClientCollaborateur))
                        {
                            Config = ConfigRepository.ReadName(Core.Global.ConfigClientCollaborateur);
                            Model.Sage.F_COLLABORATEURRepository F_COLLABORATEURRepository = new Model.Sage.F_COLLABORATEURRepository();
                            if (F_COLLABORATEURRepository.ExistId(Convert.ToInt32(Config.Con_Value)))
                            {
                                Model.Sage.F_COLLABORATEUR F_COLLABORATEUR = F_COLLABORATEURRepository.ReadId(Convert.ToInt32(Config.Con_Value));
                                F_COMPTET.CO_No = F_COLLABORATEUR.CO_No.Value;
                            }
                        }

                        F_COMPTET.CT_NumPayeur = F_COMPTET.CT_Num;

                        // <JG> 01/03/2013 gestion de la catégorie tarifaire du client importé selon son groupe par défaut
                        Model.Local.GroupRepository GroupRepository = new Model.Local.GroupRepository();
                        if (GroupRepository.Exist((int)PsCustomer.IDDefaultGroup)
                            && GroupRepository.Read((int)PsCustomer.IDDefaultGroup).Grp_CatTarifId != null)
                        {
                            F_COMPTET.N_CatTarif = (int)GroupRepository.Read((int)PsCustomer.IDDefaultGroup).Grp_CatTarifId;
                        }
                        else
                        {
                            Config = new Model.Local.Config();
                            if (ConfigRepository.ExistName(Core.Global.ConfigClientCategorieTarifaire))
                            {
                                Config = ConfigRepository.ReadName(Core.Global.ConfigClientCategorieTarifaire);
                                F_COMPTET.N_CatTarif = Convert.ToInt32(Config.Con_Value);
                            }
                        }
                        #endregion

                        #region Paramètres comptables

                        F_COMPTET.N_CatCompta = ReadCatCompta(PsCustomer, null);
                        if (F_COMPTET.N_CatCompta == 0)
                        {
                            Config = new Model.Local.Config();
                            if (ConfigRepository.ExistName(Core.Global.ConfigClientCompteComptable))
                            {
                                Config = ConfigRepository.ReadName(Core.Global.ConfigClientCompteComptable);
                                F_COMPTET.N_CatCompta = Convert.ToInt32(Config.Con_Value);
                            }
                        }

                        Config = new Model.Local.Config();
                        if (ConfigRepository.ExistName(Core.Global.ConfigClientPeriodicite))
                        {
                            Config = ConfigRepository.ReadName(Core.Global.ConfigClientPeriodicite);
                            F_COMPTET.N_Period = Convert.ToInt32(Config.Con_Value);
                        }

                        Config = new Model.Local.Config();
                        if (ConfigRepository.ExistName(Core.Global.ConfigClientBLFacture))
                        {
                            Config = ConfigRepository.ReadName(Core.Global.ConfigClientBLFacture);
                            if (Config.Con_Value == "True")
                                F_COMPTET.CT_BLFact = ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_Boolean.Oui;
                            else
                                F_COMPTET.CT_BLFact = ABSTRACTION_SAGE.F_COMPTET.Obj._Enum_Boolean.Non;
                        }

                        Config = new Model.Local.Config();
                        if (ConfigRepository.ExistName(Core.Global.ConfigClientModeExpedition))
                        {
                            Config = ConfigRepository.ReadName(Core.Global.ConfigClientModeExpedition);
                            F_COMPTET.N_Expedition = Convert.ToInt32(Config.Con_Value);
                        }

                        Config = new Model.Local.Config();
                        if (ConfigRepository.ExistName(Core.Global.ConfigClientConditionLivraison))
                        {
                            Config = ConfigRepository.ReadName(Core.Global.ConfigClientConditionLivraison);
                            F_COMPTET.N_Condition = Convert.ToInt32(Config.Con_Value);
                        }

                        Config = new Model.Local.Config();
                        if (ConfigRepository.ExistName(Core.Global.ConfigClientDepot))
                        {
                            Config = ConfigRepository.ReadName(Core.Global.ConfigClientDepot);
                            Model.Sage.F_DEPOTRepository F_DEPOTRepository = new Model.Sage.F_DEPOTRepository();
                            if (F_DEPOTRepository.ExistId(Convert.ToInt32(Config.Con_Value)))
                            {
                                Model.Sage.F_DEPOT F_DEPOT = F_DEPOTRepository.ReadId(Convert.ToInt32(Config.Con_Value));
								F_COMPTET.DE_No = F_DEPOT.DE_No == null ? 0 : F_DEPOT.DE_No;
                            }
                        }

                        Config = new Model.Local.Config();
                        if (ConfigRepository.ExistName(Core.Global.ConfigClientCodeAffaire))
                        {
                            Config = ConfigRepository.ReadName(Core.Global.ConfigClientCodeAffaire);
                            Model.Sage.F_COMPTEARepository F_COMPTEARepository = new Model.Sage.F_COMPTEARepository();
                            if (F_COMPTEARepository.ExistId(Convert.ToInt32(Config.Con_Value)))
                            {
                                Model.Sage.F_COMPTEA F_COMPTEA = F_COMPTEARepository.ReadId(Convert.ToInt32(Config.Con_Value));
                                F_COMPTET.N_Analytique = F_COMPTEA.N_Analytique;
                            }
                        }
                        #endregion

                        if (Core.Global.GetConfig().ConfigClientCiviliteActif)
                        {
                            string civilite = string.Empty;
                            Model.Prestashop.PsGenderLangRepository PsGenderLangRepository = new Model.Prestashop.PsGenderLangRepository();
                            if (PsGenderLangRepository.ExistGenderLang(PsCustomer.IDGender, Core.Global.Lang))
                            {
                                civilite = PsGenderLangRepository.ReadGenderLang(PsCustomer.IDGender, Core.Global.Lang).Name;
                                F_COMPTET.CT_Qualite = (civilite.Length > 17) ? civilite.Substring(0, 17) : civilite;
                            }
                        }

                        #region statistiques et infos libres client

                        if (Core.Global.GetConfig().StatInfolibreClientActif && Core.Global.ExistCustomerFeatureModule())
                        {
                            Model.Local.StatistiqueClientRepository StatistiqueClientRepository = new Model.Local.StatistiqueClientRepository();
                            Model.Local.InformationLibreClientRepository InformationLibreClientRepository = new Model.Local.InformationLibreClientRepository();

                            // <JG> 15/07/2015 correction initialisation collection infos libres
                            F_COMPTET.InfosLibres = new ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Col();

                            foreach (Model.Prestashop.PsCustomerFeatureCustomer PsCustomerFeatureCustomer in new Model.Prestashop.PsCustomerFeatureCustomerRepository().List(PsCustomer.IDCustomer))
                            {
                                #region Statistique client
                                if (StatistiqueClientRepository.ExistFeature(PsCustomerFeatureCustomer.IDCustomerFeature))
                                {
                                    Model.Local.StatistiqueClient StatistiqueClient = StatistiqueClientRepository.ReadFeature(PsCustomerFeatureCustomer.IDCustomerFeature);
                                    Model.Sage.P_STATISTIQUERepository P_STATISTIQUERepository = new Model.Sage.P_STATISTIQUERepository();
                                    if (P_STATISTIQUERepository.ExistStatClient(StatistiqueClient.Sag_StatClient))
                                    {
                                        Model.Prestashop.PsCustomerFeatureValueLangRepository PsCustomerFeatureValueLangRepository = new Model.Prestashop.PsCustomerFeatureValueLangRepository();
                                        if (PsCustomerFeatureValueLangRepository.ExistCustomerFeatureValueLang(PsCustomerFeatureCustomer.IDCustomerFeatureValue, Core.Global.Lang))
                                        {
                                            Model.Sage.P_STATISTIQUE P_STATISTIQUE = P_STATISTIQUERepository.ReadStatClient(StatistiqueClient.Sag_StatClient);
                                            String stat_value = PsCustomerFeatureValueLangRepository.ReadCustomerFeatureValueLang(PsCustomerFeatureCustomer.IDCustomerFeatureValue, Core.Global.Lang).Value;
                                            if (!String.IsNullOrEmpty(stat_value))
                                            {
                                                if (stat_value.Length > 21)
                                                    stat_value = stat_value.Substring(0, 21);
                                                #region affectation statistique
                                                switch (P_STATISTIQUE.cbMarq)
                                                {
                                                    case 1:
                                                        F_COMPTET.CT_Statistique01 = stat_value;
                                                        break;
                                                    case 2:
                                                        F_COMPTET.CT_Statistique02 = stat_value;
                                                        break;
                                                    case 3:
                                                        F_COMPTET.CT_Statistique03 = stat_value;
                                                        break;
                                                    case 4:
                                                        F_COMPTET.CT_Statistique04 = stat_value;
                                                        break;
                                                    case 5:
                                                        F_COMPTET.CT_Statistique05 = stat_value;
                                                        break;
                                                    case 6:
                                                        F_COMPTET.CT_Statistique06 = stat_value;
                                                        break;
                                                    case 7:
                                                        F_COMPTET.CT_Statistique07 = stat_value;
                                                        break;
                                                    case 8:
                                                        F_COMPTET.CT_Statistique08 = stat_value;
                                                        break;
                                                    case 9:
                                                        F_COMPTET.CT_Statistique09 = stat_value;
                                                        break;
                                                    case 10:
                                                        F_COMPTET.CT_Statistique10 = stat_value;
                                                        break;
                                                }
                                                #endregion
                                                Model.Sage.F_ENUMSTATRepository F_ENUMSTATRepository = new Model.Sage.F_ENUMSTATRepository();
                                                ABSTRACTION_SAGE.F_ENUMSTAT.Obj F_ENUMSTAT;
                                                if (F_ENUMSTATRepository.ExistEnumere(stat_value, (short)P_STATISTIQUE.cbMarq) == false)
                                                {
                                                    F_ENUMSTAT = new ABSTRACTION_SAGE.F_ENUMSTAT.Obj();
                                                    F_ENUMSTAT.N_Statistique = (short)P_STATISTIQUE.cbMarq;
                                                    F_ENUMSTAT.ES_Intitule = stat_value;
                                                    F_ENUMSTAT.Add(Connexion);
                                                }
                                                else
                                                {
                                                    #region Lecture valeur Sage pour respect casse énuméré
                                                    switch (P_STATISTIQUE.cbMarq)
                                                    {
                                                        case 1:
                                                            F_COMPTET.CT_Statistique01 = F_ENUMSTATRepository.ReadEnumere(stat_value, (short)P_STATISTIQUE.cbMarq).ES_Intitule;
                                                            break;
                                                        case 2:
                                                            F_COMPTET.CT_Statistique02 = F_ENUMSTATRepository.ReadEnumere(stat_value, (short)P_STATISTIQUE.cbMarq).ES_Intitule;
                                                            break;
                                                        case 3:
                                                            F_COMPTET.CT_Statistique03 = F_ENUMSTATRepository.ReadEnumere(stat_value, (short)P_STATISTIQUE.cbMarq).ES_Intitule;
                                                            break;
                                                        case 4:
                                                            F_COMPTET.CT_Statistique04 = F_ENUMSTATRepository.ReadEnumere(stat_value, (short)P_STATISTIQUE.cbMarq).ES_Intitule;
                                                            break;
                                                        case 5:
                                                            F_COMPTET.CT_Statistique05 = F_ENUMSTATRepository.ReadEnumere(stat_value, (short)P_STATISTIQUE.cbMarq).ES_Intitule;
                                                            break;
                                                        case 6:
                                                            F_COMPTET.CT_Statistique06 = F_ENUMSTATRepository.ReadEnumere(stat_value, (short)P_STATISTIQUE.cbMarq).ES_Intitule;
                                                            break;
                                                        case 7:
                                                            F_COMPTET.CT_Statistique07 = F_ENUMSTATRepository.ReadEnumere(stat_value, (short)P_STATISTIQUE.cbMarq).ES_Intitule;
                                                            break;
                                                        case 8:
                                                            F_COMPTET.CT_Statistique08 = F_ENUMSTATRepository.ReadEnumere(stat_value, (short)P_STATISTIQUE.cbMarq).ES_Intitule;
                                                            break;
                                                        case 9:
                                                            F_COMPTET.CT_Statistique09 = F_ENUMSTATRepository.ReadEnumere(stat_value, (short)P_STATISTIQUE.cbMarq).ES_Intitule;
                                                            break;
                                                        case 10:
                                                            F_COMPTET.CT_Statistique10 = F_ENUMSTATRepository.ReadEnumere(stat_value, (short)P_STATISTIQUE.cbMarq).ES_Intitule;
                                                            break;
                                                    }
                                                    #endregion
                                                }
                                            }
                                        }
                                    }
                                }
                                #endregion

                                #region Information libre client
                                if (InformationLibreClientRepository.ExistCustomerFeature(PsCustomerFeatureCustomer.IDCustomerFeature))
                                {
                                    try
                                    {
                                        Model.Local.InformationLibreClient InformationLibreClient = InformationLibreClientRepository.ReadCustomerFeature(PsCustomerFeatureCustomer.IDCustomerFeature);
                                        Model.Sage.cbSysLibreRepository cbSysLibreRepository = new Model.Sage.cbSysLibreRepository();
                                        if (cbSysLibreRepository.ExistInformationLibre(InformationLibreClient.Sag_InfoLibreClient, Model.Sage.cbSysLibreRepository.CB_File.F_COMPTET))
                                        {
                                            Model.Sage.cbSysLibre cbSysLibre = cbSysLibreRepository.ReadInformationLibre(InformationLibreClient.Sag_InfoLibreClient, Model.Sage.cbSysLibreRepository.CB_File.F_COMPTET);
                                            Model.Prestashop.PsCustomerFeatureValueLangRepository PsCustomerFeatureValueLangRepository = new Model.Prestashop.PsCustomerFeatureValueLangRepository();
                                            if (PsCustomerFeatureValueLangRepository.ExistCustomerFeatureValueLang(PsCustomerFeatureCustomer.IDCustomerFeatureValue, Core.Global.Lang))
                                            {
                                                String infolibre_value = PsCustomerFeatureValueLangRepository.ReadCustomerFeatureValueLang(PsCustomerFeatureCustomer.IDCustomerFeatureValue, Core.Global.Lang).Value;
                                                if (!String.IsNullOrEmpty(infolibre_value))
                                                {
                                                    if ((cbSysLibre.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageText
                                                        || cbSysLibre.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageTable)
                                                        && infolibre_value.Length >= cbSysLibre.CB_Len)
                                                    {
                                                        infolibre_value = infolibre_value.Substring(0, cbSysLibre.CB_Len - 1);
                                                    }

                                                    ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj info_abstraction = new ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj()
                                                    {
                                                        Len = cbSysLibre.CB_Len,
                                                        Name = cbSysLibre.CB_Name,
                                                        Pos = cbSysLibre.CB_Pos,
                                                        Table = cbSysLibre.CB_File,
                                                        Value = infolibre_value,
                                                    };
                                                    #region conversion cb_type
                                                    switch ((Model.Sage.cbSysLibreRepository.CB_Type)cbSysLibre.CB_Type)
                                                    {
                                                        case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageSmallDate:
                                                            info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageSmallDate;
                                                            break;
                                                        case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageValeur:
                                                            info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageValeur;
                                                            break;
                                                        case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageText:
                                                            info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageText;
                                                            break;
                                                        case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageDate:
                                                            info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageDate;
                                                            break;
                                                        case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageMontant:
                                                            info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageMontant;
                                                            break;
                                                        case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageTable:
                                                            info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageTable;
                                                            break;
                                                        case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.Deleted:
                                                        default:
                                                            break;
                                                    }
                                                    #endregion
                                                    F_COMPTET.InfosLibres.Add(info_abstraction);
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Core.Error.SendMailError(ex.ToString());
                                    }
                                }
                                #endregion
                            }
                        }

                        #endregion

                        // <JG> 03/06/2016 Ajout mise des infos en majuscule via option
                        if (Core.Global.GetConfig().ConfigClientInfosMajusculeActif)
                        {
                            F_COMPTET.CT_Intitule = F_COMPTET.CT_Intitule.ToUpper();
                            F_COMPTET.CT_Classement = F_COMPTET.CT_Classement.ToUpper();
                            F_COMPTET.CT_Contact = F_COMPTET.CT_Contact.ToUpper();
                            F_COMPTET.CT_Ape = F_COMPTET.CT_Ape.ToUpper();
                            F_COMPTET.CT_Qualite = F_COMPTET.CT_Qualite.ToUpper();
                        }

						#if (SAGE_VERSION_20 || SAGE_VERSION_21)
						F_COMPTET.CT_Facebook = "";
						F_COMPTET.CT_LinkedIn = "";
						#endif

						F_COMPTET.Add(Connexion);

                        // création du lien dans la base Prestaconnect
                        Model.Sage.F_COMPTETRepository F_COMPTETRepository = new Model.Sage.F_COMPTETRepository();
                        if (F_COMPTETRepository.ExistComptet(F_COMPTET.CT_Num))
                        {
                            Model.Local.Customer Customer = new Model.Local.Customer();
                            Customer.Pre_Id = Convert.ToInt32(PsCustomer.IDCustomer);
                            Model.Sage.F_COMPTET F_COMPTETUpdate = F_COMPTETRepository.ReadComptet(F_COMPTET.CT_Num);
                            Customer.Sag_Id = F_COMPTETUpdate.cbMarq;
                            CustomerRepository.Add(Customer);

                            #region Module CustomerInfo
                            if (Core.Global.GetConfig().ModuleAECCustomerInfoActif)
                            {
                                Core.Module.CustomerInfo CustomerInfo = new Module.CustomerInfo();
                                CustomerInfo.Exec(F_COMPTETUpdate, PsCustomer);
                            }
                            #endregion
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        // <JG> 05/08/2013 à 07/08/2013 modification gestion numérotation client
        private String GenerateSageNumber(Model.Prestashop.PsCustomer PsCustomer, out Boolean ValidNumber, out Model.Prestashop.PsAddress PsAddress)
        {
            ValidNumber = false;

            string compo = Core.Global.GetConfig().ConfigClientNumComposition;
            if (String.IsNullOrWhiteSpace(compo))
                compo = Model.Internal.ClientNumComposing.StrPrefixe + Model.Internal.ClientNumComposing.StrNumero;

            #region nom

            string name = (Core.Global.GetConfig().ConfigClientNumTypeNom == Parametres.NameNumComponent.CompanyBeforeName
                        && !String.IsNullOrWhiteSpace(PsCustomer.Company))
                ? PsCustomer.Company
                : PsCustomer.LastName;

            name = name.Replace(" ", string.Empty);
            // <JG> 19/01/2014 correction si nom composé
            name = name.Replace("-", string.Empty);
            name = Core.Global.RemoveDiacritics(name);
            name = Core.Global.ReadLinkRewrite(name).ToUpper();

            #endregion

            #region numero departement

            string dep_default = "00";
            string dep = dep_default;
            uint id_address = 0;
            Model.Prestashop.PsOrdersRepository PsOrdersRepository = new Model.Prestashop.PsOrdersRepository();
            if (PsOrdersRepository.ExistCustomer(Core.Global.CurrentShop.IDShop, PsCustomer.IDCustomer))
                id_address = PsOrdersRepository.ReadCustomer(Core.Global.CurrentShop.IDShop, PsCustomer.IDCustomer).id_address_invoice;
            Model.Prestashop.PsAddressRepository PsAddressRepository = new Model.Prestashop.PsAddressRepository();
            PsAddress = null;
            if (id_address != 0)
                PsAddress = PsAddressRepository.ReadAddress(id_address);
            else if (PsAddressRepository.ExistCustomer(PsCustomer.IDCustomer, 1, 0))
                PsAddress = PsAddressRepository.ReadCustomer(PsCustomer.IDCustomer, 1, 0);
            else if (PsAddressRepository.ExistCustomer(PsCustomer.IDCustomer, 1, 1))
                PsAddress = PsAddressRepository.ReadCustomer(PsCustomer.IDCustomer, 1, 1);
            else if (PsAddressRepository.ExistCustomer(PsCustomer.IDCustomer, 0, 0))
                PsAddress = PsAddressRepository.ReadCustomer(PsCustomer.IDCustomer, 0, 0);
            else if (PsAddressRepository.ExistCustomer(PsCustomer.IDCustomer, 0, 1))
                PsAddress = PsAddressRepository.ReadCustomer(PsCustomer.IDCustomer, 0, 1);

            if (PsAddress != null)
            {
                if (Core.Global.GetConfig().ConfigClientNumDepartementRemplacerCodeISO)
                {
                    Model.Local.CountryRepository CountryRepository = new Model.Local.CountryRepository();
                    if (CountryRepository.ExistCountry(PsAddress.IDCountry))
                    {
                        if (CountryRepository.ReadCountry(PsAddress.IDCountry).Replace_ISOCode)
                        {
                            Model.Prestashop.PsCountryRepository PsCountryRepository = new Model.Prestashop.PsCountryRepository();
                            if (PsCountryRepository.Exist(PsAddress.IDCountry))
                            {
                                Model.Prestashop.PsCountry PsCountry = PsCountryRepository.Read(PsAddress.IDCountry);
                                if (!string.IsNullOrWhiteSpace(PsCountry.IsoCode))
                                    dep = PsCountry.IsoCode.Trim();
                            }
                        }
                    }
                }
                if ((string.IsNullOrWhiteSpace(dep) || dep == dep_default) && !String.IsNullOrWhiteSpace(PsAddress.PostCode))
                    dep = (PsAddress.PostCode.Length > 2) ? PsAddress.PostCode.Substring(0, 2) : "0" + PsAddress.PostCode;
            }

            #endregion

            string sagenumber = string.Empty;
            sagenumber = compo;
            // génération de la partie fixe du numéro client (Préfixe + Nom + Dep)
            sagenumber = sagenumber.Replace(Model.Internal.ClientNumComposing.StrPrefixe,
                Core.Global.GetConfig().ConfigClientNumPrefixe);
            sagenumber = sagenumber.Replace(Model.Internal.ClientNumComposing.StrNom,
                (Core.Global.GetConfig().ConfigClientNumLongueurNom > name.Length)
                ? name
                : name.Substring(0, Core.Global.GetConfig().ConfigClientNumLongueurNom));
            sagenumber = sagenumber.Replace(Model.Internal.ClientNumComposing.StrDep, dep);

            int current_counter;

            // récupération de la valeur de départ
            switch (Core.Global.GetConfig().ConfigClientNumTypeNumero)
            {
                case PRESTACONNECT.Core.Parametres.NumberNumComponent.Counter:
                    switch (Core.Global.GetConfig().ConfigClientNumTypeCompteur)
                    {
                        case PRESTACONNECT.Core.Parametres.CounterType.Decremental:
                        case PRESTACONNECT.Core.Parametres.CounterType.Incremental:
                        default:
                            current_counter = Core.Global.GetConfig().ConfigClientNumDebutCompteur;
                            break;
                    }
                    break;

                case PRESTACONNECT.Core.Parametres.NumberNumComponent.IdPrestashop:
                default:
                    current_counter = (int)PsCustomer.IDCustomer;
                    break;
            }

            // saisie du compteur sur le nombre de chiffres défini en paramètre
            string number = string.Empty;
            for (int i = Core.Global.GetConfig().ConfigClientNumLongueurNumero; i > current_counter.ToString().Length; i--)
                number += "0";
            number += current_counter.ToString();

            // écriture du numéro final dans une variable temporaire pour test
            string temp = sagenumber;
            temp = temp.Replace(Model.Internal.ClientNumComposing.StrNumero, number);

            // si numéro existant 
            Model.Sage.F_COMPTETRepository F_COMPTETRepository = new Model.Sage.F_COMPTETRepository();
            while (F_COMPTETRepository.ExistComptet(temp) && temp.Length <= 17)
            {
                switch (Core.Global.GetConfig().ConfigClientNumTypeNumero)
                {
                    case PRESTACONNECT.Core.Parametres.NumberNumComponent.Counter:
                        switch (Core.Global.GetConfig().ConfigClientNumTypeCompteur)
                        {
                            case PRESTACONNECT.Core.Parametres.CounterType.Decremental:
                                current_counter -= 1;
                                if (current_counter < 1)
                                    if (Core.Global.GetConfig().ConfigClientNumLongueurNumero <= 9
                                        && Core.Global.GetConfig().ConfigClientNumLongueurNumero > 0)
                                    {
                                        string s = string.Empty;
                                        for (int i = 1; i <= Core.Global.GetConfig().ConfigClientNumLongueurNumero; i++)
                                            s += "9";
                                        current_counter = int.Parse(s);
                                    }
                                break;

                            case PRESTACONNECT.Core.Parametres.CounterType.Incremental:
                            default:
                                current_counter += 1;
                                break;
                        }
                        break;

                    case PRESTACONNECT.Core.Parametres.NumberNumComponent.IdPrestashop:
                    default:
                        current_counter += 1;
                        break;
                }

                // saisie du compteur sur le nombre de chiffres défini en paramètre
                number = string.Empty;
                for (int i = Core.Global.GetConfig().ConfigClientNumLongueurNumero; i > current_counter.ToString().Length; i--)
                    number += "0";
                number += current_counter.ToString();

                temp = sagenumber;
                temp = temp.Replace(Model.Internal.ClientNumComposing.StrNumero, number);
            }
            // notification numérotation maximale atteinte
            if (temp.Length > 17)
            {
                List<string> logs = new List<string>();
                logs.Add("Création de nouveau client dans Sage impossible :");
                logs.Add("Numérotation maximale atteinte pour les paramètres définis !");
                logs.Add("Client prestashop n° " + PsCustomer.IDCustomer);
                logs.Add("Nom : " + PsCustomer.LastName);
                logs.Add("Prénom : " + PsCustomer.FirstName);
                logs.Add("Dernier résultat généré pour les paramètres de numérotation : " + temp);
                Core.Log.SendLog(logs, Log.LogIdentifier.SynchroClient);
            }
            else
                ValidNumber = true;

            sagenumber = sagenumber.Replace(Model.Internal.ClientNumComposing.StrNumero, number);

            return sagenumber;
        }

        public static int ReadCatCompta(Model.Prestashop.PsCustomer PsCustomer, Model.Prestashop.PsOrders PsOrders)
        {
            int CatCompta = 0;
            Model.Prestashop.PsAddress PsAddress = null;
            Model.Prestashop.PsAddressRepository PsAddressRepository = new Model.Prestashop.PsAddressRepository();
            Model.Local.CountryRepository CountryRepository = new Model.Local.CountryRepository();
            if (PsOrders != null)
            {
                Model.Prestashop.PsConfigurationRepository PsConfigurationRepository = new Model.Prestashop.PsConfigurationRepository();
                if (PsConfigurationRepository.ExistName(Core.Global.PrestashopTypeAddressBaseTaxKey))
                {
                    uint IDAddress = 0;
                    Model.Prestashop.PsConfiguration PsConfiguration = PsConfigurationRepository.ReadName(Core.Global.PrestashopTypeAddressBaseTaxKey);
                    if (PsConfiguration.Value == Global.PrestashopTypeAddressBaseTax.id_address_invoice.ToString())
                        IDAddress = PsOrders.IDAddressInvoice;
                    else if (PsConfiguration.Value == Global.PrestashopTypeAddressBaseTax.id_address_delivery.ToString())
                        IDAddress = PsOrders.IDAddressDelivery;

                    if (IDAddress != 0)
                        PsAddress = PsAddressRepository.ReadAddress(IDAddress);
                }
            }
            else if (PsCustomer != null)
                PsAddress = PsAddressRepository.ReadCustomer(PsCustomer.IDCustomer, 1, 0);
            else if (PsAddress == null)
                PsAddress = PsAddressRepository.ReadCustomer(PsCustomer.IDCustomer, 1, 1);

            if (PsAddress != null && CountryRepository.ExistCountry(PsAddress.IDCountry))
                CatCompta = (String.IsNullOrWhiteSpace(PsAddress.VatNumber))
                    ? CountryRepository.ReadCountry(PsAddress.IDCountry).Sag_CatCompta
                    : CountryRepository.ReadCountry(PsAddress.IDCountry).Sag_CatComptaPro;

            return CatCompta;
        }
    }
}
