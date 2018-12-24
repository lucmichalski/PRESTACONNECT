using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using PRESTACONNECT.Model.Internal;

namespace PRESTACONNECT.Core.Transfert
{
    // <JG> 26/02/2013 - 27/02/2013 - 28/02/2013
    public class TransfertSageClient
    {
        List<String> log;

        public void Exec(Int32 F_COMPTETSend, out List<String> log_out)
        {
            log = new List<string>();
            try
            {
                if (Core.Global.GetConfig().TransfertPrestashopCookieKey.Trim() == "")
                {
                    log.Add("TC02- La clé cookie de votre site Prestashop n'est pas renseignée dans la configuration de Prestaconnect, transfert de compte client dans Prestashop impossible !");
                }
                else
                {
                    Model.Sage.F_COMPTETRepository F_COMPTETRepository = new Model.Sage.F_COMPTETRepository();
                    if (!F_COMPTETRepository.ExistId(F_COMPTETSend))
                        log.Add("TC10- Client introuvable à partir du l'identifiant " + F_COMPTETSend.ToString() + " !");
                    else
                    {
                        Model.Sage.F_COMPTET F_COMPTET = F_COMPTETRepository.Read(F_COMPTETSend);
                        Model.Local.CustomerRepository CustomerRepository = new Model.Local.CustomerRepository();
                        Model.Prestashop.PsCustomerRepository PsCustomerRepository = new Model.Prestashop.PsCustomerRepository();
                        Model.Local.GroupRepository GroupRepository = new Model.Local.GroupRepository();

                        List<SageMailAddress> ListMailAddress;
                        List<SageAddressToPrestashop> ListAddress;

                        if (CustomerRepository.ExistSage(F_COMPTETSend))
                            log.Add("TC11- Le client [ " + F_COMPTET.CT_Num + " " + F_COMPTET.CT_Intitule + " ] est déjà mappé !");

                        else if (F_COMPTET.N_CatTarif == null
                                || Core.Global.GetConfig().TransfertPriceCategoryAvailable.Contains((int)F_COMPTET.N_CatTarif) == false)
                            log.Add("TC20- La catégorie tarifaire du client [ " + F_COMPTET.CT_Num + " " + F_COMPTET.CT_Intitule + " ] ne fait pas partie des catégories à transférer !");
                        else if (GroupRepository.CatTarifSageMonoGroupe((int)F_COMPTET.N_CatTarif) == false)
                            log.Add("TC21- La catégorie tarifaire du client [ " + F_COMPTET.CT_Num + " " + F_COMPTET.CT_Intitule + " ] est liée à plusieurs groupes transfert impossible !");

                        else if (SearchMailAddress(F_COMPTET, out ListMailAddress) == false)
                            log.Add("TC30- Une erreur est survenue lors de la récupération des adresses mail du compte client [ " + F_COMPTET.CT_Num + " " + F_COMPTET.CT_Intitule + " ] !");
                        else if (ListMailAddress.Count == 0)
                            log.Add("TC31- Le compte client [ " + F_COMPTET.CT_Num + " " + F_COMPTET.CT_Intitule + " ] ne possède pas d'adresse mail !");
                        else if (ListMailAddress.Count(m => m.IsAccountMail == true) == 0)
                            log.Add("TC32- Il n'y a pas d'adresse mail utilisable pour la création du compte du client [ " + F_COMPTET.CT_Num + " " + F_COMPTET.CT_Intitule + " ] !");
                        else if (PsCustomerRepository.ExistMail(ListMailAddress.First(m => m.IsAccountMail == true).MailAddress, Core.Global.CurrentShop.IDShop))
                            log.Add("TC33- Il existe déjà un compte dans Prestashop avec l'adresse mail " + ListMailAddress.First(m => m.IsAccountMail == true).MailAddress + ", transfert du client [ " + F_COMPTET.CT_Num + " " + F_COMPTET.CT_Intitule + " ] !");

                        else if (SearchAddress(F_COMPTET, out ListAddress) == false)
                            // TC42 se trouve dans les fonctions "SearchIdCountry"
                            // et TC43 dans les fonctions "ConvertAddress"
                            log.Add("TC40- Une erreur est survenue lors de la récupération des adresses du compte client [ " + F_COMPTET.CT_Num + " " + F_COMPTET.CT_Intitule + " ] !");
                        else if (ListAddress.Count == 0)
                            log.Add("TC41- Aucune adresse n'a pu être récupérée selon les paramètres pour le client [ " + F_COMPTET.CT_Num + " " + F_COMPTET.CT_Intitule + " ] !");
                        else
                        {
                            // récupération de l'id groupe en fonction de la catégorie tarifaire
                            uint IdPsGroup = (uint)GroupRepository.SearchIdGroupCatTarifSage((int)F_COMPTET.N_CatTarif);

                            // construction du mot de passe aléatoire
                            String RandomPassword = Core.RandomString.GetRandomstring(Core.Global.GetConfig().TransfertRandomPasswordLength, true, true, true, Core.Global.GetConfig().TransfertRandomPasswordIncludeSpecialCharacters);

                            DateTime AccountDate = DateTime.Now;

                            bool check_activation = Core.Global.GetConfig().TransfertAccountActivation;
                            bool check_newsletter = Core.Global.GetConfig().TransfertNewsLetterSuscribe;
                            DateTime? date_news = null;
                            if (check_newsletter)
                                date_news = AccountDate;
                            bool check_optin = Core.Global.GetConfig().TransfertOptInSuscribe;

                            Model.Prestashop.PsCustomer PsCustomer = new Model.Prestashop.PsCustomer()
                            {
                                Active = (check_activation) ? (byte)1 : (byte)0,
                                Birthday = null,
                                Company = GetCompanyValue(F_COMPTET),
                                DateAdd = AccountDate,
                                DateUpd = AccountDate,
                                Deleted = 0,
                                Email = ListMailAddress.First(m => m.IsAccountMail == true).MailAddress,
                                FirstName = GetFirstNameValue(F_COMPTET),
                                IDDefaultGroup = IdPsGroup,
                                IDGender = 0,
                                IPRegistrationNewsletter = string.Empty,
                                IsGuest = 0,
                                LastName = GetLastNameValue(F_COMPTET),
                                LastPassWDGen = AccountDate,
                                Newsletter = (check_newsletter) ? (byte)1 : (byte)0,
                                NewsletterDateAdd = date_news,
                                Note = string.Empty,
                                OptIn = (check_optin) ? (byte)1 : (byte)0,
                                IDShop = Core.Global.CurrentShop.IDShop,
                                IDShopGroup = Core.Global.CurrentShop.IDShopGroup,
                                IDRisk = 1,
                                OutstandingAllowAmount = 0,
                                ShowPublicPrices = 0,
                                SireT = (Core.Global.IsValidateSiret(F_COMPTET.CT_Siret)) ? F_COMPTET.CT_Siret.Replace(" ", string.Empty) : string.Empty,
                                MaXPaymentDays = 0,
                                // utilisation clé cookie renseignée en paramètre pour hash MD5
                                PassWD = Core.RandomString.HashMD5(Core.Global.GetConfig().TransfertPrestashopCookieKey.Trim() + RandomPassword),
                                SecureKey = Core.RandomString.HashMD5(RandomPassword),
                                Ape = (Core.Global.IsValidateAPE(F_COMPTET.CT_Ape)) ? Core.Global.CleanAPE(F_COMPTET.CT_Ape) : string.Empty,
                                Website = F_COMPTET.CT_Site,
                            };
                            // enregistrement du client dans Prestashop
                            PsCustomerRepository.Add(PsCustomer);

                            // attribution du client au groupe
                            Model.Prestashop.PsCustomerGroupRepository PsCustomerGroupRepository = new Model.Prestashop.PsCustomerGroupRepository();
                            Model.Prestashop.PsCustomerGroup Current_PsCustomerGroup = new Model.Prestashop.PsCustomerGroup()
                            {
                                IDCustomer = PsCustomer.IDCustomer,
                                IDGroup = IdPsGroup,
                            };
                            PsCustomerGroupRepository.Add(Current_PsCustomerGroup);

                            // enregistrement du mappage du client en local
                            Model.Local.Customer Customer = new Model.Local.Customer()
                            {
                                Pre_Id = (int)PsCustomer.IDCustomer,
                                Sag_Id = F_COMPTET.cbMarq
                            };
                            CustomerRepository.Add(Customer);

                            log.Add("TC80- Compte prestashop pour le client [ " + F_COMPTET.CT_Num + " " + F_COMPTET.CT_Intitule + " ] créé avec succès.");
                            log.Add("TC81- Clé de transfert du compte : " + Core.Log.EncryptString(Core.Log.EncryptString(RandomPassword, true), true));

                            #region Module AECCustomerOutstanding
                            if (Core.Global.GetConfig().ModuleAECCustomerOutstandingActif && Core.Global.ExistAECCustomerOutstandingModule())
                            {
                                Core.Module.AECCustomerOutstanding gestionencours = new Module.AECCustomerOutstanding();
                                List<string> log_encours;
                                gestionencours.Exec(Customer, out log_encours);
                                if (log_encours.Count > 0)
                                    log.AddRange(log_encours);
                            }
                            #endregion

                            #region Module CustomerInfo
                            if (Core.Global.GetConfig().ModuleAECCustomerInfoActif)
                            {
                                Core.Module.CustomerInfo gestioninfo = new Module.CustomerInfo();
                                gestioninfo.Exec(F_COMPTET, PsCustomer);
                            }
                            #endregion

                            // création des adresses
                            Model.Prestashop.PsAddressRepository PsAdressRepository = new Model.Prestashop.PsAddressRepository();
                            Model.Local.AddressRepository AddressRepository = new Model.Local.AddressRepository();
                            int cpt_adr = 0;
                            foreach (SageAddressToPrestashop Address in ListAddress)
                            {
                                try
                                {
                                    // récupération de l'adresse dans la boucle pour traitements supplémentaires
                                    Model.Prestashop.PsAddress Current_PsAdress = Address.PsAddressAdd;
                                    Current_PsAdress.IDCustomer = PsCustomer.IDCustomer;

                                    // enresgistrement de l'adresse dans Prestashop
                                    PsAdressRepository.Add(Current_PsAdress);

                                    // enregistrement du mappage de l'adresse en local si adresse de livraison
                                    if (Address.Type != Core.Parametres.SageAddressType.AdresseFacturation)
                                        AddressRepository.Add(new Model.Local.Address()
                                        {
                                            Pre_Id = (int)Current_PsAdress.IDAddress,
                                            Sag_Id = Address.F_LIVRAISON_cbMarq,
                                            Add_Date = Current_PsAdress.DateAdd
                                        });

                                    cpt_adr++;
                                    log.Add("TC82- Adresse \"" + Current_PsAdress.Alias + "\" pour le compte prestashop du client [ " + F_COMPTET.CT_Num + " " + F_COMPTET.CT_Intitule + " ] créée avec succès.");
                                }
                                catch (Exception ex)
                                {
                                    log.Add("TC71- Erreur de transfert d'adresse : " + ex.Message);
                                    Core.Error.SendMailError(ex.ToString());
                                }
                            }
                            if (cpt_adr == 0)
                            {
                                log.Add("TC71- Échec du transfert de la ou les adresses du client [ " + F_COMPTET.CT_Num + " " + F_COMPTET.CT_Intitule + " ] vers son compte Prestashop ! Le compte ne sera pas utilisable !");
                                // TODO méthode de désactivation du compte 
                                //PsCustomer.Active = 0;
                            }


                            if (Core.Global.GetConfig().ConfigMailActive)
                            {
                                Model.Local.OrderMailRepository OrderMailRepository = new Model.Local.OrderMailRepository();
                                if (OrderMailRepository.ExistType(31) == false)
                                {
                                    log.Add("TC03- Le modèle de mail pour la notification du transfert de compte client n'a pas été créé !");
                                }
                                else if (OrderMailRepository.ReadType(31).OrdMai_Active == false)
                                {
                                    log.Add("TC04- Le modèle de mail pour la notification du transfert de compte client n'est pas activé !");
                                }
                                else
                                {
                                    // notification de la création du compte par email
                                    bool send = SendMail(PsCustomer, RandomPassword, ListMailAddress);
                                    if (send)
                                        log.Add("TC90- Une notification de la création du compte Prestashop pour le client [ " + F_COMPTET.CT_Num + " " + F_COMPTET.CT_Intitule
                                            + " ] à été transmise à l'adresse mail du compte : \"" + PsCustomer.Email + "\""
                                            + ((ListMailAddress.Count > 1) ? " et aux autres adresses mail selon les paramètres." : "."));
                                    else
                                        log.Add("TC91- La notification de création du compte Prestashop pour le client [ " + F_COMPTET.CT_Num + " " + F_COMPTET.CT_Intitule
                                            + " ] avec l'adresse \"" + PsCustomer.Email + "\" n'a pas pu être envoyé par email !"
                                            + " Vous devrez modifier son mot de passe dans l'administration de votre Prestashop puis lui communiquer !");
                                }
                            }
                            if (Core.Global.GetConfig().TransfertGenerateAccountFile)
                            {
                                if (Core.Csv.LogStream == null)
                                    Core.Csv.WriteLog("\"Numéro client\";\"Zone Nom\";\"Zone Prénom\";\"Adresse mail du compte\";\"Mot de passe Prestashop\"", false, Log.LogIdentifier.TransfertClient);
                                Core.Csv.WriteLog("\"" + F_COMPTET.CT_Num + "\";\"" + PsCustomer.LastName + "\";\"" + PsCustomer.FirstName + "\";\"" + PsCustomer.Email + "\";\"" + RandomPassword + "\"", false, Log.LogIdentifier.TransfertClient);
                            }

                            // <JG> 06/10/2013 ajout gestion CustomerFeature
                            if (Core.Global.ExistCustomerFeatureModule() && Core.Global.GetConfig().StatInfolibreClientActif)
                            {
                                Core.Transfert.TransfertStatInfoLibreClient Sync = new Core.Transfert.TransfertStatInfoLibreClient();
                                Sync.Exec(Customer.Sag_Id);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("TC01- Une erreur est survenue : " + ex.ToString());
            }
            finally
            {
                log_out = log;
            }
        }

        #region récupération des adresses mail

        public Boolean SearchMailAddress(Model.Sage.F_COMPTET F_COMPTET, out List<SageMailAddress> list)
        {
            Boolean valid = false;
            list = new List<SageMailAddress>();
            try
            {
                if (!String.IsNullOrWhiteSpace(F_COMPTET.CT_EMail) && Core.Global.IsMailAddress(F_COMPTET.CT_EMail.Trim()))
                {
                    list.Add(new SageMailAddress()
                    {
                        MailAddress = F_COMPTET.CT_EMail.Trim(),
                        Type = Core.Parametres.SageAddressType.AdresseFacturation,
                    });
                }

                Model.Sage.F_LIVRAISONRepository F_LIVRAISONRepository = new Model.Sage.F_LIVRAISONRepository();
                if (F_LIVRAISONRepository.ExistComptetPrincipal(F_COMPTET.CT_Num, (short)ABSTRACTION_SAGE.F_LIVRAISON.Obj._Enum_LI_Principal.Lieu_Principal))
                {
                    String adresse = F_LIVRAISONRepository.ReadComptetPrincipal(F_COMPTET.CT_Num, (short)ABSTRACTION_SAGE.F_LIVRAISON.Obj._Enum_LI_Principal.Lieu_Principal).LI_EMail;
                    if (!String.IsNullOrWhiteSpace(adresse) && Core.Global.IsMailAddress(adresse.Trim()))
                        list.Add(new SageMailAddress()
                        {
                            MailAddress = F_LIVRAISONRepository.ReadComptetPrincipal(F_COMPTET.CT_Num, (short)ABSTRACTION_SAGE.F_LIVRAISON.Obj._Enum_LI_Principal.Lieu_Principal).LI_EMail.Trim(),
                            Type = Core.Parametres.SageAddressType.AdresseLivraisonPrincipale,
                        });
                }

                // Ajout de la gestion de l'adresse mail d'un contact suivant le service
                Model.Sage.F_CONTACTTRepository F_CONTACTTiersRepository = new Model.Sage.F_CONTACTTRepository();
                if (F_CONTACTTiersRepository.ExistContactService(F_COMPTET.CT_Num, Core.Global.GetConfig().TransfertMailAccountContactService))
                {
                    String adresse = F_CONTACTTiersRepository.ReadContactService(F_COMPTET.CT_Num, Core.Global.GetConfig().TransfertMailAccountContactService).CT_EMail;
                    if (!String.IsNullOrWhiteSpace(adresse) && Core.Global.IsMailAddress(adresse.Trim()))
                        list.Add(new SageMailAddress()
                        {
                            MailAddress = adresse.Trim(),
                            Type = Core.Parametres.SageAddressType.AdresseContact,
                        });
                }

                // définition de l'adresse mail utilisée pour la création du compte
                Boolean AdresseAlternative = Core.Global.GetConfig().TransfertMailAccountAlternative;
                Core.Parametres.MailAccountIdentification AccountMail = Core.Global.GetConfig().TransfertMailAccountIdentification;

                switch (AccountMail)
                {
                    case Core.Parametres.MailAccountIdentification.MailFicheClient:
                        // si la liste contient l'adresse mail de la fiche client
                        if (list.Count(m => m.Type == Core.Parametres.SageAddressType.AdresseFacturation) == 1)
                            list.First(m => m.Type == Core.Parametres.SageAddressType.AdresseFacturation).IsAccountMail = true;
                        // sinon utilisation possible de l'adresse alternative
                        else if (AdresseAlternative && list.Count(m => m.Type == Core.Parametres.SageAddressType.AdresseLivraisonPrincipale) == 1)
                            list.First(m => m.Type == Core.Parametres.SageAddressType.AdresseLivraisonPrincipale).IsAccountMail = true;

                        break;

                    case Core.Parametres.MailAccountIdentification.MailAdressePrincipale:
                        // si la liste contient l'adresse mail de l'adresse de livraison principale du client
                        if (list.Count(m => m.Type == Core.Parametres.SageAddressType.AdresseLivraisonPrincipale) == 1)
                            list.First(m => m.Type == Core.Parametres.SageAddressType.AdresseLivraisonPrincipale).IsAccountMail = true;
                        // sinon utilisation possible de l'adresse alternative
                        else if (AdresseAlternative && list.Count(m => m.Type == Core.Parametres.SageAddressType.AdresseFacturation) == 1)
                            list.First(m => m.Type == Core.Parametres.SageAddressType.AdresseFacturation).IsAccountMail = true;

                        break;


                    case Core.Parametres.MailAccountIdentification.MailContactService:
                        int service = Core.Global.GetConfig().TransfertMailAccountContactService;
                        // si la liste contient l'adresse mail du contact du service
                        if (list.Count(m => m.Type == Core.Parametres.SageAddressType.AdresseContact) == 1)
                            list.First(m => m.Type == Core.Parametres.SageAddressType.AdresseContact).IsAccountMail = true;

                        break;

                    default:
                        // pas de création de compte
                        break;
                }

                if (list.Count(a => a.IsAccountMail == true) == 1)
                {
                    switch (Core.Global.GetConfig().TransfertNotifyAccountAddress)
                    {
                        case Core.Parametres.MailNotification.AccountMail:
                            // si notification uniquement au mail utilisé pour la création du compte
                            if (list.Count > 1)
                                list.RemoveAll(a => a.IsAccountMail == false);
                            break;

                        case Core.Parametres.MailNotification.AllMail:   // Option invalide avec les contacts d'un service
                            // récupération des adresses mail des autres adresses de livraison
                            if (F_LIVRAISONRepository.ExistComptetPrincipal(F_COMPTET.CT_Num, (short)ABSTRACTION_SAGE.F_LIVRAISON.Obj._Enum_LI_Principal.Non_Principal))
                                foreach (Model.Sage.F_LIVRAISON adresse in F_LIVRAISONRepository.ListComptetPrincipale(F_COMPTET.CT_Num, (short)ABSTRACTION_SAGE.F_LIVRAISON.Obj._Enum_LI_Principal.Non_Principal))
                                    if (!String.IsNullOrWhiteSpace(adresse.LI_EMail) && Core.Global.IsMailAddress(adresse.LI_EMail.Trim()))
                                        list.Add(new SageMailAddress()
                                        {
                                            MailAddress = F_LIVRAISONRepository.ReadComptetPrincipal(F_COMPTET.CT_Num, (short)ABSTRACTION_SAGE.F_LIVRAISON.Obj._Enum_LI_Principal.Lieu_Principal).LI_EMail.Trim(),
                                            Type = Core.Parametres.SageAddressType.AdresseLivraisonAutre,
                                        });
                            break;

                        case Core.Parametres.MailNotification.AccountMailAndAlternative:
                        default:
                            break;
                    }

                    // récupération des adresses mail des contacts de la fiche client selon les filtres de Type et Service
                    Model.Sage.F_CONTACTTRepository F_CONTACTTRepository = new Model.Sage.F_CONTACTTRepository();
                    List<int> ListService = Core.Global.GetConfig().TransfertNotifyAccountSageContactService;
                    List<int> ListType = Core.Global.GetConfig().TransfertNotifyAccountSageContactType;
                    foreach (Model.Sage.F_CONTACTT Contact in F_CONTACTTRepository.ListClient(F_COMPTET.CT_Num, true))
                        if (!String.IsNullOrWhiteSpace(Contact.CT_EMail) && Core.Global.IsMailAddress(Contact.CT_EMail.Trim())
                                && Contact.N_Service != null && ListService.Contains((int)Contact.N_Service)
                                && Contact.N_Contact != null && ListType.Contains((int)Contact.N_Contact))
                            list.Add(new SageMailAddress()
                            {
                                MailAddress = Contact.CT_EMail.Trim(),
                                Type = Core.Parametres.SageAddressType.AdresseContact,
                            });
                }
                valid = true;
            }
            catch (Exception ex)
            {
                log.Add("TC30- Une erreur est survenue : " + ex.Message);
                Core.Error.SendMailError(ex.ToString());
            }
            return valid;
        }

        #endregion

        #region récupération des adresses

        public Boolean SearchAddress(Model.Sage.F_COMPTET F_COMPTET, out List<SageAddressToPrestashop> list)
        {
            Boolean valid = false;
            List<SageAddressToPrestashop> list_temp = new List<SageAddressToPrestashop>();
            list = new List<SageAddressToPrestashop>();
            try
            {
                List<int> ListSageAddressSend = Core.Global.GetConfig().TransfertSageAddressSend;

                if (ListSageAddressSend.Contains((int)Core.Parametres.SageAddressSend.AdresseFacturation))
                {
                    uint IdPsCountryFicheClient = SearchIdCountry(F_COMPTET);
                    if (IdPsCountryFicheClient > 0)
                        list_temp.Add(new SageAddressToPrestashop(ConvertAddress(F_COMPTET, IdPsCountryFicheClient), Core.Parametres.SageAddressType.AdresseFacturation, 0));
                }

                if (ListSageAddressSend.Contains((int)Core.Parametres.SageAddressSend.AdressePrincipale))
                {
                    Model.Sage.F_LIVRAISONRepository F_LIVRAISONRepository = new Model.Sage.F_LIVRAISONRepository();
                    if (F_LIVRAISONRepository.ExistComptetPrincipal(F_COMPTET.CT_Num, (short)ABSTRACTION_SAGE.F_LIVRAISON.Obj._Enum_LI_Principal.Lieu_Principal))
                    {
                        Model.Sage.F_LIVRAISON F_LIVRAISON_Principale = F_LIVRAISONRepository.ReadComptetPrincipal(F_COMPTET.CT_Num, (short)ABSTRACTION_SAGE.F_LIVRAISON.Obj._Enum_LI_Principal.Lieu_Principal);
                        uint IdPsCountryAdressePrincipale = SearchIdCountry(F_LIVRAISON_Principale);
                        if (IdPsCountryAdressePrincipale > 0)
                            list_temp.Add(new SageAddressToPrestashop(ConvertAddress(F_LIVRAISON_Principale, IdPsCountryAdressePrincipale), Core.Parametres.SageAddressType.AdresseLivraisonPrincipale, F_LIVRAISON_Principale.cbMarq));
                    }
                }

                if (ListSageAddressSend.Contains((int)Core.Parametres.SageAddressSend.AutreAdresse))
                {
                    Model.Sage.F_LIVRAISONRepository F_LIVRAISONRepository = new Model.Sage.F_LIVRAISONRepository();
                    if (F_LIVRAISONRepository.ExistComptetPrincipal(F_COMPTET.CT_Num, (short)ABSTRACTION_SAGE.F_LIVRAISON.Obj._Enum_LI_Principal.Non_Principal))
                        foreach (Model.Sage.F_LIVRAISON F_LIVRAISON_Secondaire in F_LIVRAISONRepository.ListComptetPrincipale(F_COMPTET.CT_Num, (short)ABSTRACTION_SAGE.F_LIVRAISON.Obj._Enum_LI_Principal.Non_Principal))
                        {
                            uint IdPsCountryAdresse = SearchIdCountry(F_LIVRAISON_Secondaire);
                            if (IdPsCountryAdresse > 0)
                                list_temp.Add(new SageAddressToPrestashop(ConvertAddress(F_LIVRAISON_Secondaire, IdPsCountryAdresse), Core.Parametres.SageAddressType.AdresseLivraisonAutre, F_LIVRAISON_Secondaire.cbMarq));
                        }
                }

                foreach (SageAddressToPrestashop Address in list_temp)
                {
                    bool check = false;
                    switch (Address.Type)
                    {
                        case Core.Parametres.SageAddressType.AdresseFacturation:
                            check = CheckAddressPhoneNumber(Address, ((list_temp.Count(a => a.Type == Core.Parametres.SageAddressType.AdresseLivraisonPrincipale) == 1)
                                                                        ? list_temp.First(a => a.Type == Core.Parametres.SageAddressType.AdresseLivraisonPrincipale) : null));
                            break;
                        case Core.Parametres.SageAddressType.AdresseLivraisonPrincipale:
                            check = CheckAddressPhoneNumber(Address, ((list_temp.Count(a => a.Type == Core.Parametres.SageAddressType.AdresseFacturation) == 1)
                                                                        ? list_temp.First(a => a.Type == Core.Parametres.SageAddressType.AdresseFacturation) : null));
                            break;
                        case Core.Parametres.SageAddressType.AdresseLivraisonAutre:
                            // voir condition dans "CheckAddressPhoneNumber" pour récupération infos adresse livraison principale si facturation vide
                            check = CheckAddressPhoneNumber(Address, ((list_temp.Count(a => a.Type == Core.Parametres.SageAddressType.AdresseFacturation) == 1)
                                                                        ? list_temp.First(a => a.Type == Core.Parametres.SageAddressType.AdresseFacturation) : null));
                            if (!check)
                                check = CheckAddressPhoneNumber(Address, ((list_temp.Count(a => a.Type == Core.Parametres.SageAddressType.AdresseLivraisonPrincipale) == 1)
                                                                            ? list_temp.First(a => a.Type == Core.Parametres.SageAddressType.AdresseLivraisonPrincipale) : null));
                            break;
                        case Core.Parametres.SageAddressType.AdresseContact:
                        default:
                            check = false;
                            break;
                    }
                    if (check)
                        list.Add(Address);
                }
                valid = true;
            }
            catch (Exception ex)
            {
                log.Add("TC40- Une erreur est survenue : " + ex.Message);
                Core.Error.SendMailError(ex.ToString());
            }
            return valid;
        }

        public uint SearchIdCountry(Model.Sage.F_COMPTET Client)
        {
            uint IDPsCountry = 0;
            if (!String.IsNullOrEmpty(Client.CT_Pays))
            {
                Model.Prestashop.PsCountryLangRepository PsCountryLangRepository = new Model.Prestashop.PsCountryLangRepository();
                if (PsCountryLangRepository.ExistCountryLang(Client.CT_Pays, Core.Global.Lang))
                {
                    IDPsCountry = PsCountryLangRepository.ReadCountryLang(Client.CT_Pays, Core.Global.Lang).IDCountry;
                }
                else
                {
                    Model.Sage.F_PAYSRepository F_PAYSRepository = new Model.Sage.F_PAYSRepository();
                    if (F_PAYSRepository.ExistPays(Client.CT_Pays))
                    {
                        Model.Sage.F_PAYS Pays = F_PAYSRepository.ReadPays(Client.CT_Pays);
                        Model.Prestashop.PsCountryRepository PsCountryRepository = new Model.Prestashop.PsCountryRepository();
                        if (!string.IsNullOrWhiteSpace(Pays.PA_Code) && PsCountryRepository.ExistIsoCode(Pays.PA_Code))
                        {
                            IDPsCountry = PsCountryRepository.ReadIsoCode(Pays.PA_Code).IDCountry;
                        }
                        else if (!string.IsNullOrWhiteSpace(Pays.PA_CodeISO2) && PsCountryRepository.ExistIsoCode(Pays.PA_CodeISO2))
                        {
                            IDPsCountry = PsCountryRepository.ReadIsoCode(Pays.PA_CodeISO2).IDCountry;
                        }
                        else
                        {
                            log.Add("TC42- Le pays \"" + Client.CT_Pays + "\" (Codes ISO : " + Pays.PA_Code + " / " + Pays.PA_CodeISO2 + ") de l'adresse de facturation du client " + Client.CT_Num + " " + Client.CT_Intitule + " n'existe pas dans Prestashop !");
                        }
                    }
                    else
                    {
                        log.Add("TC42- Le pays \"" + Client.CT_Pays + "\" de l'adresse de facturation du client " + Client.CT_Num + " " + Client.CT_Intitule + " n'existe pas dans Prestashop !");
                    }
                }
            }
            else
            {
                log.Add("TC42- Le pays de l'adresse de facturation du client " + Client.CT_Num + " " + Client.CT_Intitule + " n'est pas renseigné !");
            }

            return IDPsCountry;
        }
        public uint SearchIdCountry(Model.Sage.F_LIVRAISON Adresse)
        {
            uint IDPsCountry = 0;
            if (!String.IsNullOrEmpty(Adresse.LI_Pays))
            {
                Model.Prestashop.PsCountryLangRepository PsCountryLangRepository = new Model.Prestashop.PsCountryLangRepository();
                if (PsCountryLangRepository.ExistCountryLang(Adresse.LI_Pays, Core.Global.Lang))
                {
                    IDPsCountry = PsCountryLangRepository.ReadCountryLang(Adresse.LI_Pays, Core.Global.Lang).IDCountry;
                }
                else
                {
                    Model.Sage.F_PAYSRepository F_PAYSRepository = new Model.Sage.F_PAYSRepository();
                    if (F_PAYSRepository.ExistPays(Adresse.LI_Pays))
                    {
                        Model.Sage.F_PAYS Pays = F_PAYSRepository.ReadPays(Adresse.LI_Pays);
                        Model.Prestashop.PsCountryRepository PsCountryRepository = new Model.Prestashop.PsCountryRepository();
                        if (!string.IsNullOrWhiteSpace(Pays.PA_Code) && PsCountryRepository.ExistIsoCode(Pays.PA_Code))
                        {
                            IDPsCountry = PsCountryRepository.ReadIsoCode(Pays.PA_Code).IDCountry;
                        }
                        else if (!string.IsNullOrWhiteSpace(Pays.PA_CodeISO2) && PsCountryRepository.ExistIsoCode(Pays.PA_CodeISO2))
                        {
                            IDPsCountry = PsCountryRepository.ReadIsoCode(Pays.PA_CodeISO2).IDCountry;
                        }
                        else
                        {
                            log.Add("TC42- Le pays \"" + Adresse.LI_Pays + "\" (Codes ISO : " + Pays.PA_Code + " / " + Pays.PA_CodeISO2 + ") de l'adresse de livraison \"" + Adresse.LI_Intitule + "\" du client " + Adresse.F_COMPTET.CT_Num + " " + Adresse.F_COMPTET.CT_Intitule + " n'existe pas dans Prestashop !");
                        }
                    }
                    else
                    {
                        log.Add("TC42- Le pays \"" + Adresse.LI_Pays + "\" de l'adresse de livraison \"" + Adresse.LI_Intitule + "\" du client " + Adresse.F_COMPTET.CT_Num + " " + Adresse.F_COMPTET.CT_Intitule + " n'existe pas dans Prestashop !");
                    }
                }
            }
            else
            {
                log.Add("TC42- Le pays de l'adresse de livraison \"" + Adresse.LI_Intitule + "\" du client " + Adresse.F_COMPTET.CT_Num + " " + Adresse.F_COMPTET.CT_Intitule + " n'est pas renseigné !");
            }

            return IDPsCountry;
        }

        public Model.Prestashop.PsAddress ConvertAddress(Model.Sage.F_COMPTET Client, uint IdPsCountry)
        {
            Model.Prestashop.PsAddress PsAddressAdd = new Model.Prestashop.PsAddress();
            try
            {
                PsAddressAdd = new Model.Prestashop.PsAddress()
                {
                    Active = 1,
                    Address1 = Core.Global.CleanAddress(Client.CT_Adresse),
                    Address2 = Core.Global.CleanAddress(Client.CT_Complement),
                    Alias = GetAliasValue(Client),
                    City = Core.Global.CleanCityName(Client.CT_Ville),
                    Company = GetCompanyValue(Client),
                    DateAdd = DateTime.Now,
                    DateUpd = DateTime.Now,
                    Deleted = 0,
                    DNi = string.Empty,
                    FirstName = GetFirstNameValue(Client),
                    LastName = GetLastNameValue(Client),
                    IDCustomer = 0, // add later
                    PostCode = Core.Global.CleanPostCode(Client.CT_CodePostal),
                    Phone = Core.Global.CleanPhoneNumber(Client.CT_Telephone),
                    PhoneMobile = Core.Global.CleanPhoneNumber(Client.CT_Telecopie),
                    IDCountry = IdPsCountry,
                    IDState = 0,
                    IDManufacturer = 0,
                    IDSupplier = 0,
                    Other = string.Empty,
                    VatNumber = Client.CT_Identifiant,
                    IDWarehouse = 0,
                };
                if (PsAddressAdd.LastName == PsAddressAdd.FirstName && PsAddressAdd.LastName.Contains(" "))
                {
                    PsAddressAdd.FirstName = " ";
                }
            }
            catch (Exception ex)
            {
                log.Add("TC44- Une erreur est survenue lors de la transformation de l'adresse de facturation du client " + Client.CT_Num + " " + Client.CT_Intitule + " !");
                Core.Error.SendMailError(ex.ToString());
            }
            return PsAddressAdd;
        }
        public Model.Prestashop.PsAddress ConvertAddress(Model.Sage.F_LIVRAISON Adresse, uint IdPsCountry)
        {
            Model.Prestashop.PsAddress PsAddressAdd = new Model.Prestashop.PsAddress();
            try
            {
                PsAddressAdd = new Model.Prestashop.PsAddress()
                {
                    Active = 1,
                    Address1 = Core.Global.CleanAddress(Adresse.LI_Adresse),
                    Address2 = Core.Global.CleanAddress(Adresse.LI_Complement),
                    Alias = GetAliasValue(Adresse),
                    City = Core.Global.CleanCityName(Adresse.LI_Ville),
                    Company = GetCompanyValue(Adresse),
                    DateAdd = DateTime.Now,
                    DateUpd = DateTime.Now,
                    Deleted = 0,
                    DNi = string.Empty,
                    FirstName = GetFirstNameValue(Adresse),
                    LastName = GetLastNameValue(Adresse),
                    IDCustomer = 0, // add later
                    PostCode = Core.Global.CleanPostCode(Adresse.LI_CodePostal),
                    Phone = Core.Global.CleanPhoneNumber(Adresse.LI_Telephone),
                    PhoneMobile = Core.Global.CleanPhoneNumber(Adresse.LI_Telecopie),
                    IDCountry = IdPsCountry,
                    IDState = 0,
                    IDManufacturer = 0,
                    IDSupplier = 0,
                    Other = string.Empty,
                    VatNumber = (Adresse.F_COMPTET != null) ? Adresse.F_COMPTET.CT_Identifiant : string.Empty,
                    IDWarehouse = 0,
                };
                if (PsAddressAdd.LastName == PsAddressAdd.FirstName && PsAddressAdd.LastName.Contains(" "))
                {
                    PsAddressAdd.FirstName = " ";
                }
            }
            catch (Exception ex)
            {
                log.Add("TC44- Une erreur est survenue lors de la transformation de l'adresse de livraison \"" + Adresse.LI_Intitule + "\" du client " + Adresse.F_COMPTET.CT_Num + " " + Adresse.F_COMPTET.CT_Intitule + " !");
                Core.Error.SendMailError(ex.ToString());
            }
            return PsAddressAdd;
        }

        public Boolean CheckAddressPhoneNumber(SageAddressToPrestashop AddressToCheck, SageAddressToPrestashop AddressAccount)
        {
            Boolean check = true;

            // opération de recopie
            if (String.IsNullOrWhiteSpace(AddressToCheck.PsAddressAdd.Phone)
                    && String.IsNullOrWhiteSpace(AddressToCheck.PsAddressAdd.PhoneMobile))
                if (Core.Global.GetConfig().TransfertLockPhoneNumber.Contains((int)Core.Parametres.LockPhoneNumber.RecopyFirst))
                    if (AddressAccount != null
                        && AddressAccount.PsAddressAdd != null
                        && (!String.IsNullOrWhiteSpace(AddressAccount.PsAddressAdd.Phone)
                            || !String.IsNullOrWhiteSpace(AddressAccount.PsAddressAdd.PhoneMobile)))
                    {
                        AddressToCheck.PsAddressAdd.Phone = AddressAccount.PsAddressAdd.Phone;
                        AddressToCheck.PsAddressAdd.PhoneMobile = AddressAccount.PsAddressAdd.PhoneMobile;
                    }

            // saisie fictive et blocage à ne pas effectuer si adresse de livraison autre et adresse source = facturation
            // cela permet à la fonction "SearchAddress" de récupérer les informations de l'adresse de livraison principale si celles de la facturation sont vides
            if (AddressToCheck.Type != Parametres.SageAddressType.AdresseLivraisonAutre
                || (AddressAccount != null && AddressAccount.Type == Parametres.SageAddressType.AdresseLivraisonPrincipale))
            {

                // opération de saisie fictive
                if (String.IsNullOrWhiteSpace(AddressToCheck.PsAddressAdd.Phone)
                        && String.IsNullOrWhiteSpace(AddressToCheck.PsAddressAdd.PhoneMobile))
                    if (Core.Global.GetConfig().TransfertLockPhoneNumber.Contains((int)Core.Parametres.LockPhoneNumber.ReplaceEntry))
                    {
                        AddressToCheck.PsAddressAdd.Phone =
                            (!String.IsNullOrEmpty(Core.Global.GetConfig().TransfertLockPhoneNumberReplaceEntryValue))
                            ? Core.Global.GetConfig().TransfertLockPhoneNumberReplaceEntryValue
                            : "  .  .  .  .  ";
                    }

                // opération de blocage
                if (String.IsNullOrWhiteSpace(AddressToCheck.PsAddressAdd.Phone)
                        && String.IsNullOrWhiteSpace(AddressToCheck.PsAddressAdd.PhoneMobile))
                    if (Core.Global.GetConfig().TransfertLockPhoneNumber.Contains((int)Core.Parametres.LockPhoneNumber.LockAddress))
                    {
                        check = false;
                    }
            }
            return check;
        }

        #endregion

        #region Gestion des informations d'adresse

        public String GetAliasValue(Model.Sage.F_COMPTET Client)
        {
            string r;
            switch (Core.Global.GetConfig().TransfertAliasValue)
            {
                case Core.Parametres.AliasValue.NumeroClient:
                    r = Client.CT_Num;
                    break;

                case Core.Parametres.AliasValue.AbregeClient:
                    r = (!String.IsNullOrWhiteSpace(Client.CT_Classement)) ? Client.CT_Classement : Client.CT_Intitule;
                    break;

                case Core.Parametres.AliasValue.IntituleCompteAdresse:
                default:
                    r = Client.CT_Intitule;
                    break;
            }
            r = Core.Global.CleanGenericName(r);
            if (r.Length > 32)
                r = r.Substring(0, 32);
            return r;
        }
        public String GetAliasValue(Model.Sage.F_LIVRAISON Adresse)
        {
            string r;
            switch (Core.Global.GetConfig().TransfertAliasValue)
            {
                case Core.Parametres.AliasValue.NumeroClient:
                    r = Adresse.CT_Num;
                    break;

                case Core.Parametres.AliasValue.AbregeClient:
                    r = (!String.IsNullOrWhiteSpace(Adresse.F_COMPTET.CT_Classement)) ? Adresse.F_COMPTET.CT_Classement : Adresse.LI_Intitule;
                    break;

                case Core.Parametres.AliasValue.IntituleCompteAdresse:
                default:
                    r = Adresse.LI_Intitule;
                    break;
            }
            r = Core.Global.CleanGenericName(r);
            if (r.Length > 32)
                r = r.Substring(0, 32);
            return r;
        }

        public String GetLastNameValue(Model.Sage.F_COMPTET Client)
        {
            string r;
            switch (Core.Global.GetConfig().TransfertLastNameValue)
            {
                case Core.Parametres.LastNameValue.NumeroClient:
                    r = Client.CT_Num;
                    break;

                case Core.Parametres.LastNameValue.AbregeClient:
                    r = (!String.IsNullOrWhiteSpace(Client.CT_Classement)) ? Client.CT_Classement : Client.CT_Intitule;
                    break;

                case Core.Parametres.LastNameValue.IntituleCompteAdresseP1:
                    if (!String.IsNullOrEmpty(Core.Global.GetConfig().TransfertClientSeparateurIntitule)
                        && Client.CT_Intitule.Contains(Core.Global.GetConfig().TransfertClientSeparateurIntitule))
                    {
                        string[] sep = { Core.Global.GetConfig().TransfertClientSeparateurIntitule };
                        string[] tabl = Client.CT_Intitule.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        if (tabl.Count() > 0)
                            r = tabl[0].Trim();
                        else
                            r = Client.CT_Intitule;
                    }
                    else if (Client.CT_Intitule.Contains(' '))
                    {
                        string[] tabl = Client.CT_Intitule.Split(' ');
                        if (tabl.Count() > 0)
                            r = tabl[0].Trim();
                        else
                            r = Client.CT_Intitule;
                    }
                    else
                        r = Client.CT_Intitule;
                    break;
                case Core.Parametres.LastNameValue.IntituleCompteAdresseP2:
                    if (!String.IsNullOrEmpty(Core.Global.GetConfig().TransfertClientSeparateurIntitule)
                        && Client.CT_Intitule.Contains(Core.Global.GetConfig().TransfertClientSeparateurIntitule))
                    {
                        string[] sep = { Core.Global.GetConfig().TransfertClientSeparateurIntitule };
                        string[] tabl = Client.CT_Intitule.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        if (tabl.Count() > 1)
                            r = Client.CT_Intitule.Replace(tabl[0] + Core.Global.GetConfig().TransfertClientSeparateurIntitule, string.Empty).Trim();
                        else
                            r = Client.CT_Intitule;
                    }
                    else if (Client.CT_Intitule.Contains(' '))
                    {
                        string[] tabl = Client.CT_Intitule.Split(' ');
                        if (tabl.Count() > 1)
                            r = Client.CT_Intitule.Replace(tabl[0] + " ", string.Empty).Trim();
                        else
                            r = Client.CT_Intitule;
                    }
                    else
                        r = Client.CT_Intitule;
                    break;

                case Parametres.LastNameValue.Contact:
                    r = (!String.IsNullOrWhiteSpace(Client.CT_Contact)) ? Client.CT_Contact : Client.CT_Intitule;
                    break;
                case Parametres.LastNameValue.ContactP1:
                    if (!String.IsNullOrEmpty(Core.Global.GetConfig().TransfertClientSeparateurIntitule)
                        && Client.CT_Contact.Contains(Core.Global.GetConfig().TransfertClientSeparateurIntitule))
                    {
                        string[] sep = { Core.Global.GetConfig().TransfertClientSeparateurIntitule };
                        string[] tabl = Client.CT_Contact.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        if (tabl.Count() > 0)
                            r = tabl[0].Trim();
                        else
                            r = Client.CT_Contact;
                    }
                    else if (Client.CT_Contact.Contains(' '))
                    {
                        string[] tabl = Client.CT_Contact.Split(' ');
                        if (tabl.Count() > 0)
                            r = tabl[0].Trim();
                        else
                            r = Client.CT_Contact;
                    }
                    else
                        r = Client.CT_Contact;
                    break;
                case Parametres.LastNameValue.ContactP2:
                    if (!String.IsNullOrEmpty(Core.Global.GetConfig().TransfertClientSeparateurIntitule)
                        && Client.CT_Intitule.Contains(Core.Global.GetConfig().TransfertClientSeparateurIntitule))
                    {
                        string[] sep = { Core.Global.GetConfig().TransfertClientSeparateurIntitule };
                        string[] tabl = Client.CT_Contact.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        if (tabl.Count() > 1)
                            r = Client.CT_Contact.Replace(tabl[0] + Core.Global.GetConfig().TransfertClientSeparateurIntitule, string.Empty).Trim();
                        else
                            r = Client.CT_Contact;
                    }
                    else if (Client.CT_Contact.Contains(' '))
                    {
                        string[] tabl = Client.CT_Contact.Split(' ');
                        if (tabl.Count() > 1)
                            r = Client.CT_Contact.Replace(tabl[0] + " ", string.Empty).Trim();
                        else
                            r = Client.CT_Contact;
                    }
                    else
                        r = Client.CT_Contact;
                    break;

                case Parametres.LastNameValue.Espace:
                    r = " ";
                    break;

                case Core.Parametres.LastNameValue.IntituleCompteAdresse:
                default:
                    r = Client.CT_Intitule;
                    break;
            }
            r = Core.Global.CleanCustomerName(r);
            if (string.IsNullOrEmpty(r))
                r = "-";
            if (r.Length > 32)
                r = r.Substring(0, 32);
            return r;
        }
        public String GetLastNameValue(Model.Sage.F_LIVRAISON Adresse)
        {
            string r;
            switch (Core.Global.GetConfig().TransfertLastNameValue)
            {
                case Core.Parametres.LastNameValue.NumeroClient:
                    r = Adresse.CT_Num;
                    break;

                case Core.Parametres.LastNameValue.AbregeClient:
                    r = (!String.IsNullOrWhiteSpace(Adresse.F_COMPTET.CT_Classement)) ? Adresse.F_COMPTET.CT_Classement : Adresse.LI_Intitule;
                    break;

                case Core.Parametres.LastNameValue.IntituleCompteAdresseP1:
                    if (!String.IsNullOrEmpty(Core.Global.GetConfig().TransfertClientSeparateurIntitule)
                        && Adresse.LI_Intitule.Contains(Core.Global.GetConfig().TransfertClientSeparateurIntitule))
                    {
                        string[] sep = { Core.Global.GetConfig().TransfertClientSeparateurIntitule };
                        string[] tabl = Adresse.LI_Intitule.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        if (tabl.Count() > 0)
                            r = tabl[0].Trim();
                        else
                            r = Adresse.LI_Intitule;
                    }
                    else if (Adresse.LI_Intitule.Contains(' '))
                    {
                        string[] tabl = Adresse.LI_Intitule.Split(' ');
                        if (tabl.Count() > 0)
                            r = tabl[0].Trim();
                        else
                            r = Adresse.LI_Intitule;
                    }
                    else
                        r = Adresse.LI_Intitule;
                    break;
                case Core.Parametres.LastNameValue.IntituleCompteAdresseP2:
                    if (!String.IsNullOrEmpty(Core.Global.GetConfig().TransfertClientSeparateurIntitule)
                        && Adresse.LI_Intitule.Contains(Core.Global.GetConfig().TransfertClientSeparateurIntitule))
                    {
                        string[] sep = { Core.Global.GetConfig().TransfertClientSeparateurIntitule };
                        string[] tabl = Adresse.LI_Intitule.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        if (tabl.Count() > 1)
                            r = Adresse.LI_Intitule.Replace(tabl[0] + Core.Global.GetConfig().TransfertClientSeparateurIntitule, string.Empty).Trim();
                        else
                            r = Adresse.LI_Intitule;
                    }
                    else if (Adresse.LI_Intitule.Contains(' '))
                    {
                        string[] tabl = Adresse.LI_Intitule.Split(' ');
                        if (tabl.Count() > 1)
                            r = Adresse.LI_Intitule.Replace(tabl[0] + " ", string.Empty).Trim();
                        else
                            r = Adresse.LI_Intitule;
                    }
                    else
                        r = Adresse.LI_Intitule;
                    break;

                case Parametres.LastNameValue.Contact:
                    r = (!String.IsNullOrWhiteSpace(Adresse.LI_Contact)) ? Adresse.LI_Contact : Adresse.LI_Intitule;
                    break;
                case Parametres.LastNameValue.ContactP1:
                    if (!String.IsNullOrEmpty(Core.Global.GetConfig().TransfertClientSeparateurIntitule)
                        && Adresse.LI_Contact.Contains(Core.Global.GetConfig().TransfertClientSeparateurIntitule))
                    {
                        string[] sep = { Core.Global.GetConfig().TransfertClientSeparateurIntitule };
                        string[] tabl = Adresse.LI_Contact.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        if (tabl.Count() > 1)
                            r = Adresse.LI_Contact.Replace(tabl[0] + Core.Global.GetConfig().TransfertClientSeparateurIntitule, string.Empty).Trim();
                        else
                            r = Adresse.LI_Contact;
                    }
                    else if (Adresse.LI_Contact.Contains(' '))
                    {
                        string[] tabl = Adresse.LI_Contact.Split(' ');
                        if (tabl.Count() > 1)
                            r = Adresse.LI_Contact.Replace(tabl[0] + " ", string.Empty).Trim();
                        else
                            r = Adresse.LI_Contact;
                    }
                    else
                        r = Adresse.LI_Contact;
                    break;
                case Parametres.LastNameValue.ContactP2:
                    if (!String.IsNullOrEmpty(Core.Global.GetConfig().TransfertClientSeparateurIntitule)
                        && Adresse.LI_Contact.Contains(Core.Global.GetConfig().TransfertClientSeparateurIntitule))
                    {
                        string[] sep = { Core.Global.GetConfig().TransfertClientSeparateurIntitule };
                        string[] tabl = Adresse.LI_Contact.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        if (tabl.Count() > 1)
                            r = Adresse.LI_Contact.Replace(tabl[0] + Core.Global.GetConfig().TransfertClientSeparateurIntitule, string.Empty).Trim();
                        else
                            r = Adresse.LI_Contact;
                    }
                    else if (Adresse.LI_Contact.Contains(' '))
                    {
                        string[] tabl = Adresse.LI_Contact.Split(' ');
                        if (tabl.Count() > 1)
                            r = Adresse.LI_Contact.Replace(tabl[0] + " ", string.Empty).Trim();
                        else
                            r = Adresse.LI_Contact;
                    }
                    else
                        r = Adresse.LI_Contact;
                    break;
                case Parametres.LastNameValue.Espace:
                    r = " ";
                    break;

                case Core.Parametres.LastNameValue.IntituleCompteAdresse:
                default:
                    r = Adresse.LI_Intitule;
                    break;
            }
            r = Core.Global.CleanCustomerName(r);
            if (string.IsNullOrEmpty(r))
                r = "-";
            if (r.Length > 32)
                r = r.Substring(0, 32);
            return r;
        }

        public String GetFirstNameValue(Model.Sage.F_COMPTET Client)
        {
            string r;
            switch (Core.Global.GetConfig().TransfertFirstNameValue)
            {
                case Core.Parametres.FirstNameValue.AbregeClient:
                    r = (!String.IsNullOrEmpty(Client.CT_Classement)) ? Client.CT_Classement : Client.CT_Num;
                    break;

                case Core.Parametres.FirstNameValue.Contact:
                    r = (!String.IsNullOrEmpty(Client.CT_Contact)) ? Client.CT_Contact : Client.CT_Num;
                    break;

                case Core.Parametres.FirstNameValue.IntituleCompteAdresseP1:
                    if (!String.IsNullOrEmpty(Core.Global.GetConfig().TransfertClientSeparateurIntitule)
                        && Client.CT_Intitule.Contains(Core.Global.GetConfig().TransfertClientSeparateurIntitule))
                    {
                        string[] sep = { Core.Global.GetConfig().TransfertClientSeparateurIntitule };
                        string[] tabl = Client.CT_Intitule.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        if (tabl.Count() > 0)
                            r = tabl[0].Trim();
                        else
                            r = Client.CT_Intitule;
                    }
                    else if (Client.CT_Intitule.Contains(' '))
                    {
                        string[] tabl = Client.CT_Intitule.Split(' ');
                        if (tabl.Count() > 0)
                            r = tabl[0].Trim();
                        else
                            r = Client.CT_Intitule;
                    }
                    else
                        r = Client.CT_Intitule;
                    break;
                case Core.Parametres.FirstNameValue.IntituleCompteAdresseP2:
                    if (!String.IsNullOrEmpty(Core.Global.GetConfig().TransfertClientSeparateurIntitule)
                        && Client.CT_Intitule.Contains(Core.Global.GetConfig().TransfertClientSeparateurIntitule))
                    {
                        string[] sep = { Core.Global.GetConfig().TransfertClientSeparateurIntitule };
                        string[] tabl = Client.CT_Intitule.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        if (tabl.Count() > 1)
                            r = Client.CT_Intitule.Replace(tabl[0] + Core.Global.GetConfig().TransfertClientSeparateurIntitule, string.Empty).Trim();
                        else
                            r = Client.CT_Intitule;
                    }
                    else if (Client.CT_Intitule.Contains(' '))
                    {
                        string[] tabl = Client.CT_Intitule.Split(' ');
                        if (tabl.Count() > 1)
                            r = Client.CT_Intitule.Replace(tabl[0] + " ", string.Empty).Trim();
                        else
                            r = Client.CT_Intitule;
                    }
                    else
                        r = Client.CT_Intitule;
                    break;

                case Core.Parametres.FirstNameValue.ContactP1:
                    if (!String.IsNullOrEmpty(Core.Global.GetConfig().TransfertClientSeparateurIntitule)
                        && Client.CT_Contact.Contains(Core.Global.GetConfig().TransfertClientSeparateurIntitule))
                    {
                        string[] sep = { Core.Global.GetConfig().TransfertClientSeparateurIntitule };
                        string[] tabl = Client.CT_Contact.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        if (tabl.Count() > 0)
                            r = tabl[0].Trim();
                        else
                            r = Client.CT_Contact;
                    }
                    else if (Client.CT_Contact.Contains(' '))
                    {
                        string[] tabl = Client.CT_Contact.Split(' ');
                        if (tabl.Count() > 0)
                            r = tabl[0].Trim();
                        else
                            r = Client.CT_Contact;
                    }
                    else
                        r = Client.CT_Contact;
                    break;
                case Core.Parametres.FirstNameValue.ContactP2:
                    if (!String.IsNullOrEmpty(Core.Global.GetConfig().TransfertClientSeparateurIntitule)
                        && Client.CT_Contact.Contains(Core.Global.GetConfig().TransfertClientSeparateurIntitule))
                    {
                        string[] sep = { Core.Global.GetConfig().TransfertClientSeparateurIntitule };
                        string[] tabl = Client.CT_Contact.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        if (tabl.Count() > 1)
                            r = Client.CT_Contact.Replace(tabl[0] + Core.Global.GetConfig().TransfertClientSeparateurIntitule, string.Empty).Trim();
                        else
                            r = Client.CT_Contact;
                    }
                    else if (Client.CT_Contact.Contains(' '))
                    {
                        string[] tabl = Client.CT_Contact.Split(' ');
                        if (tabl.Count() > 1)
                            r = Client.CT_Contact.Replace(tabl[0] + " ", string.Empty).Trim();
                        else
                            r = Client.CT_Contact;
                    }
                    else
                        r = Client.CT_Contact;
                    break;

                case Core.Parametres.FirstNameValue.Espace:
                    r = " ";
                    break;

                case Core.Parametres.FirstNameValue.NumeroClient:
                default:
                    r = Client.CT_Num;
                    break;
            }
            r = Core.Global.CleanCustomerName(r);
            if (string.IsNullOrEmpty(r))
                r = "-";
            if (r.Length > 32)
                r = r.Substring(0, 32);
            return r;
        }
        public String GetFirstNameValue(Model.Sage.F_LIVRAISON Adresse)
        {
            string r;
            switch (Core.Global.GetConfig().TransfertFirstNameValue)
            {
                case Core.Parametres.FirstNameValue.AbregeClient:
                    r = (!String.IsNullOrWhiteSpace(Adresse.F_COMPTET.CT_Classement)) ? Adresse.F_COMPTET.CT_Classement : Adresse.CT_Num;
                    break;

                case Core.Parametres.FirstNameValue.Contact:
                    r = (!String.IsNullOrWhiteSpace(Adresse.LI_Contact)) ? Adresse.LI_Contact : Adresse.CT_Num;
                    break;

                case Core.Parametres.FirstNameValue.IntituleCompteAdresseP1:
                    if (!String.IsNullOrEmpty(Core.Global.GetConfig().TransfertClientSeparateurIntitule)
                        && Adresse.LI_Intitule.Contains(Core.Global.GetConfig().TransfertClientSeparateurIntitule))
                    {
                        string[] sep = { Core.Global.GetConfig().TransfertClientSeparateurIntitule };
                        string[] tabl = Adresse.LI_Intitule.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        if (tabl.Count() > 0)
                            r = tabl[0].Trim();
                        else
                            r = Adresse.LI_Intitule;
                    }
                    else if (Adresse.LI_Intitule.Contains(' '))
                    {
                        string[] tabl = Adresse.LI_Intitule.Split(' ');
                        if (tabl.Count() > 0)
                            r = tabl[0].Trim();
                        else
                            r = Adresse.LI_Intitule;
                    }
                    else
                        r = Adresse.LI_Intitule;
                    break;
                case Core.Parametres.FirstNameValue.IntituleCompteAdresseP2:
                    if (!String.IsNullOrEmpty(Core.Global.GetConfig().TransfertClientSeparateurIntitule)
                        && Adresse.LI_Intitule.Contains(Core.Global.GetConfig().TransfertClientSeparateurIntitule))
                    {
                        string[] sep = { Core.Global.GetConfig().TransfertClientSeparateurIntitule };
                        string[] tabl = Adresse.LI_Intitule.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        if (tabl.Count() > 1)
                            r = Adresse.LI_Intitule.Replace(tabl[0] + Core.Global.GetConfig().TransfertClientSeparateurIntitule, string.Empty).Trim();
                        else
                            r = Adresse.LI_Intitule;
                    }
                    else if (Adresse.LI_Intitule.Contains(' '))
                    {
                        string[] tabl = Adresse.LI_Intitule.Split(' ');
                        if (tabl.Count() > 1)
                            r = Adresse.LI_Intitule.Replace(tabl[0] + " ", string.Empty).Trim();
                        else
                            r = Adresse.LI_Intitule;
                    }
                    else
                        r = Adresse.LI_Intitule;
                    break;

                case Core.Parametres.FirstNameValue.ContactP1:
                    if (!String.IsNullOrEmpty(Core.Global.GetConfig().TransfertClientSeparateurIntitule)
                        && Adresse.LI_Contact.Contains(Core.Global.GetConfig().TransfertClientSeparateurIntitule))
                    {
                        string[] sep = { Core.Global.GetConfig().TransfertClientSeparateurIntitule };
                        string[] tabl = Adresse.LI_Contact.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        if (tabl.Count() > 0)
                            r = tabl[0].Trim();
                        else
                            r = Adresse.LI_Contact;
                    }
                    else if (Adresse.LI_Contact.Contains(' '))
                    {
                        string[] tabl = Adresse.LI_Contact.Split(' ');
                        if (tabl.Count() > 0)
                            r = tabl[0].Trim();
                        else
                            r = Adresse.LI_Contact;
                    }
                    else
                        r = Adresse.LI_Contact;
                    break;
                case Core.Parametres.FirstNameValue.ContactP2:
                    if (!String.IsNullOrEmpty(Core.Global.GetConfig().TransfertClientSeparateurIntitule)
                        && Adresse.LI_Contact.Contains(Core.Global.GetConfig().TransfertClientSeparateurIntitule))
                    {
                        string[] sep = { Core.Global.GetConfig().TransfertClientSeparateurIntitule };
                        string[] tabl = Adresse.LI_Contact.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                        if (tabl.Count() > 1)
                            r = Adresse.LI_Contact.Replace(tabl[0] + Core.Global.GetConfig().TransfertClientSeparateurIntitule, string.Empty).Trim();
                        else
                            r = Adresse.LI_Contact;
                    }
                    else if (Adresse.LI_Contact.Contains(' '))
                    {
                        string[] tabl = Adresse.LI_Contact.Split(' ');
                        if (tabl.Count() > 1)
                            r = Adresse.LI_Contact.Replace(tabl[0] + " ", string.Empty).Trim();
                        else
                            r = Adresse.LI_Contact;
                    }
                    else
                        r = Adresse.LI_Contact;
                    break;

                case Core.Parametres.FirstNameValue.Espace:
                    r = " ";
                    break;

                case Core.Parametres.FirstNameValue.NumeroClient:
                default:
                    r = Adresse.CT_Num;
                    break;
            }
            r = Core.Global.CleanCustomerName(r);
            if (string.IsNullOrEmpty(r))
                r = "-";
            if (r.Length > 32)
                r = r.Substring(0, 32);
            return r;
        }

        public String GetCompanyValue(Model.Sage.F_COMPTET Client)
        {
            string r;
            switch (Core.Global.GetConfig().TransfertCompanyValue)
            {
                case Core.Parametres.CompanyValue.Contact:
                    r = Client.CT_Contact;
                    break;

                case Core.Parametres.CompanyValue.AbregeClient:
                    r = Client.CT_Classement;
                    break;

                case Core.Parametres.CompanyValue.IntituleCompteAdresse:
                    r = Client.CT_Intitule;
                    break;

                case Core.Parametres.CompanyValue.Empty:
                default:
                    r = string.Empty;
                    break;
            }
            r = Core.Global.CleanGenericName(r);
            if (r.Length > 64)
                r = r.Substring(0, 64);
            return r;
        }
        public String GetCompanyValue(Model.Sage.F_LIVRAISON Adresse)
        {
            string r;
            switch (Core.Global.GetConfig().TransfertCompanyValue)
            {
                case Core.Parametres.CompanyValue.Contact:
                    r = Adresse.LI_Contact;
                    break;

                case Core.Parametres.CompanyValue.AbregeClient:
                    r = Adresse.F_COMPTET.CT_Classement;
                    break;

                case Core.Parametres.CompanyValue.IntituleCompteAdresse:
                    r = Adresse.LI_Intitule;
                    break;

                case Core.Parametres.CompanyValue.Empty:
                default:
                    r = string.Empty;
                    break;
            }
            r = Core.Global.CleanGenericName(r);
            if (r.Length > 64)
                r = r.Substring(0, 64);
            return r;
        }

        #endregion

        #region Notification création du compte

        public Boolean SendMail(Model.Prestashop.PsCustomer PsCustomer, String CustomerPassword, List<SageMailAddress> ListMailAddress)
        {
            Boolean ShowMessage = true;
            try
            {
                String User = Core.Global.GetConfig().ConfigMailUser;
                String Password = Core.Global.GetConfig().ConfigMailPassword;
                String SMTP = Core.Global.GetConfig().ConfigMailSMTP;
                Int32 Port = Core.Global.GetConfig().ConfigMailPort;
                Boolean isSSL = Core.Global.GetConfig().ConfigMailSSL;

                if (!string.IsNullOrWhiteSpace(User)
                    && !string.IsNullOrWhiteSpace(Password)
                    && !string.IsNullOrWhiteSpace(SMTP))
                {
                    Model.Local.OrderMailRepository OrderMailRepository = new Model.Local.OrderMailRepository();
                    if (OrderMailRepository.ExistType(31))
                    {

                        Model.Local.OrderMail OrderMail = OrderMailRepository.ReadType(31);
                        OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailAccountLastName, PsCustomer.LastName);
                        OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailAccountFirstName, PsCustomer.FirstName);
                        OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailAccountPassword, CustomerPassword);

                        MailAddress Expediteur = new MailAddress(User);
                        MailAddress Destinataire;
                        SmtpClient SmtpClient = new SmtpClient(SMTP, Port);
                        MailMessage Msg;
                        // gestion envoi multi-adresse ou en copie
                        switch (Core.Global.GetConfig().TransfertNotifyAccountDeliveryMethod)
                        {
                            case Core.Parametres.DeliveryMethod.Independent:
                                foreach (SageMailAddress Mail in ListMailAddress)
                                {
                                    Msg = new MailMessage();
                                    Destinataire = new MailAddress(Mail.MailAddress);

                                    if (Core.Global.GetConfig().TransfertNotifyAccountAdminCopy
                                            && !String.IsNullOrWhiteSpace(Core.Global.GetConfig().AdminMailAddress)
                                            && Core.Global.IsMailAddress(Core.Global.GetConfig().AdminMailAddress, Parametres.RegexMail.lvl08_lUdS))
                                        Msg.Bcc.Add(new MailAddress(Core.Global.GetConfig().AdminMailAddress));

                                    Msg.From = Expediteur;
                                    Msg.To.Add(Destinataire);
                                    Msg.Subject = OrderMail.OrdMai_Header;
                                    Msg.Body = OrderMail.OrdMai_Content;
                                    Msg.IsBodyHtml = true;
                                    SmtpClient.EnableSsl = isSSL;
                                    SmtpClient.Credentials = new NetworkCredential(User, Password);
                                    SmtpClient.Send(Msg);
                                }
                                ShowMessage = false;
                                break;

                            case Core.Parametres.DeliveryMethod.Copy:
                            default:
                                Msg = new MailMessage();
                                Destinataire = new MailAddress(PsCustomer.Email);
                                Msg.From = Expediteur;
                                Msg.To.Add(Destinataire);
                                foreach (SageMailAddress Mail in ListMailAddress.Where(a => a.IsAccountMail == false))
                                {
                                    Msg.CC.Add(new MailAddress(Mail.MailAddress));
                                }

                                if (Core.Global.GetConfig().TransfertNotifyAccountAdminCopy
                                        && !String.IsNullOrWhiteSpace(Core.Global.GetConfig().AdminMailAddress)
                                        && Core.Global.IsMailAddress(Core.Global.GetConfig().AdminMailAddress, Parametres.RegexMail.lvl08_lUdS))
                                    Msg.Bcc.Add(new MailAddress(Core.Global.GetConfig().AdminMailAddress));

                                Msg.Subject = OrderMail.OrdMai_Header;
                                Msg.Body = OrderMail.OrdMai_Content;
                                Msg.IsBodyHtml = true;

                                SmtpClient.EnableSsl = isSSL;
                                SmtpClient.Credentials = new NetworkCredential(User, Password);
                                SmtpClient.Send(Msg);
                                ShowMessage = false;
                                break;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                log.Add("TC91- Erreur d'envoi de mail : " + ex.Message);
                //Core.Error.SendMailError(ex.ToString());
            }
            if (ShowMessage)
            {
                if (Core.Global.UILaunch)
                    System.Windows.MessageBox.Show("La notification de création du compte Prestashop pour le client " + PsCustomer.LastName + " " + PsCustomer.FirstName
                                + " avec l'adresse \"" + PsCustomer.Email + "\" n'a pas pu être envoyé par email !"
                                + "\nVous devrez modifier son mot de passe dans l'administration de votre Prestashop puis lui communiquer !",
                                "", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            return !ShowMessage;
        }

        #endregion
    }
}
