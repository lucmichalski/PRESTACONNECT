using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Core.Sync
{
    public class SynchronisationLivraison
    {
        public void Exec(ABSTRACTION_SAGE.ALTERNETIS.Connexion Connexion, UInt32 IDAddress)
        {
            try
            {
                Model.Prestashop.PsAddress PsAddress = new Model.Prestashop.PsAddressRepository().ReadAddress(IDAddress);
                Model.Local.CustomerRepository CustomerRepository = new Model.Local.CustomerRepository();
                if (CustomerRepository.ExistPrestashop(Convert.ToInt32(PsAddress.IDCustomer)))
                {
                    Model.Local.Customer Customer = CustomerRepository.ReadPrestashop(Convert.ToInt32(PsAddress.IDCustomer));
                    Model.Local.AddressRepository AddressRepository = new Model.Local.AddressRepository();
                    // <JG> 08/11/2012 ajout recréation de l'adresse Prestashop si supprimée de Sage
                    if (AddressRepository.ExistPrestashop(Convert.ToInt32(PsAddress.IDAddress)) == false
                        || new Model.Sage.F_LIVRAISONRepository().ExistId(AddressRepository.ReadPrestashop((Int32)PsAddress.IDAddress).Sag_Id) == false)
                    {
                        this.ExecDistantToLocal(Customer, PsAddress, Connexion, AddressRepository);
                    }
                    else
                    {
                        Model.Local.Address Address = AddressRepository.ReadPrestashop(Convert.ToInt32(PsAddress.IDAddress));
                        Address.Add_Date = Address.Add_Date.AddMilliseconds(-Address.Add_Date.Millisecond);
                        if (Address.Add_Date.Ticks < PsAddress.DateUpd.Ticks)
                        {
                            this.UpdateDistantToLocal(Customer, PsAddress, Connexion, Address);
                            Address.Add_Date = (PsAddress.DateUpd != null && PsAddress.DateUpd > new DateTime(1753, 1, 2)) ? PsAddress.DateUpd : DateTime.Now.Date;
                            AddressRepository.Save();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void ExecDistantToLocal(Model.Local.Customer Customer, Model.Prestashop.PsAddress PsAddress, ABSTRACTION_SAGE.ALTERNETIS.Connexion Connexion, Model.Local.AddressRepository AddressRepository)
        {
            try
            {
                Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                Model.Local.Config Config = new Model.Local.Config();
                ABSTRACTION_SAGE.F_LIVRAISON.Obj F_LIVRAISON = new ABSTRACTION_SAGE.F_LIVRAISON.Obj();
                Model.Sage.F_COMPTETRepository F_COMPTERepository = new Model.Sage.F_COMPTETRepository();
                if (F_COMPTERepository.ExistId(Customer.Sag_Id))
                {
                    Model.Sage.F_COMPTET F_COMPTET = F_COMPTERepository.Read(Customer.Sag_Id);
                    F_LIVRAISON.CT_Num = F_COMPTET.CT_Num;

                    // <JG> 07/09/2012 Modification gestion adresse livraison
                    if (ConfigRepository.ExistName(Core.Global.ConfigClientIntituleAdresse)
                        && ConfigRepository.ReadName(Core.Global.ConfigClientIntituleAdresse).Con_Value == Core.Global.ConfigClientIntituleAdresseEnum.NomPrenomPrestashop.ToString())
                    {
                        string temp = (Core.Global.GetConfig().ConfigClientSocieteIntituleActif && !string.IsNullOrEmpty(PsAddress.Company))
                            ? PsAddress.Company
                            : PsAddress.LastName + " " + PsAddress.FirstName;

                        string CodePrestashop = PsAddress.IDAddress.ToString();
                        Int32 maxlenght = 32 - CodePrestashop.Length;
                        if (temp.Length > maxlenght)
                        {
                            temp = temp.Substring(0, maxlenght);
                        }
                        temp += " [" + CodePrestashop + "]";
                        F_LIVRAISON.LI_Intitule = temp;
                    }
                    else
                    {
                        F_LIVRAISON.LI_Intitule = Core.Global.GetConfig().ConfigClientNumPrefixe + PsAddress.IDAddress.ToString();
                    }

                    #region Module SoColissimo
                    Model.Prestashop.order_address info_cart = null;
                    Model.Prestashop.PsSoDelivery PsSoDelivery = null;
                    if (Core.Global.ExistSoColissimoDeliveryModule()
                           && Core.Global.GetConfig().ModuleSoColissimoDeliveryActive
                           && Core.Global.GetConfig().ModuleSoColissimoReplaceAddressNameActive)
                    {
                        Model.Prestashop.PsOrdersRepository PsOrdersRepository = new Model.Prestashop.PsOrdersRepository();
                        List<Model.Prestashop.order_address> ListAddress = PsOrdersRepository.ListAddress(Core.Global.CurrentShop.IDShop, PsAddress.IDCustomer);
                        if (ListAddress != null)
                        {
                            info_cart = (from L in ListAddress where L.id_address_delivery == PsAddress.IDAddress orderby L.id_order descending select L).FirstOrDefault();
                            if (info_cart != null)
                            {
                                Model.Prestashop.PsSoDeliveryRepository PsSoDeliveryRepository = new Model.Prestashop.PsSoDeliveryRepository();
                                if (PsSoDeliveryRepository.ExistCart(info_cart.id_cart))
                                {
                                    PsSoDelivery = PsSoDeliveryRepository.ReadCart(info_cart.id_cart);
                                    string temp = (!string.IsNullOrWhiteSpace(PsSoDelivery.LiBelle))
                                        ? PsSoDelivery.LiBelle
                                        : PsSoDelivery.FirstName + " " + PsSoDelivery.LastName
                                            + (!string.IsNullOrEmpty(PsSoDelivery.Company) ? " " + PsSoDelivery.Company : string.Empty);
                                    //: ((!string.IsNullOrEmpty(PsSoDelivery.Company))
                                    //    ? PsSoDelivery.Company
                                    //        + (!string.IsNullOrEmpty(PsSoDelivery.LastName) ? " " + PsSoDelivery.LastName : string.Empty)
                                    //        + (!string.IsNullOrEmpty(PsSoDelivery.FirstName) ? " " + PsSoDelivery.FirstName : string.Empty)
                                    //    : PsSoDelivery.LastName + " " + PsSoDelivery.FirstName);

                                    string CodePrestashop = PsAddress.IDAddress.ToString();
                                    Int32 maxlenght = 32 - CodePrestashop.Length;
                                    if (temp.Length > maxlenght)
                                    {
                                        temp = temp.Substring(0, maxlenght);
                                    }
                                    temp += " [" + CodePrestashop + "]";
                                    F_LIVRAISON.LI_Intitule = temp;
                                }
                            }
                        }
                    }
                    #endregion

                    // conversion valeur prestashop
                    string intitule_iso = Core.Global.ConvertUnicodeToISO_8859_1(F_LIVRAISON.LI_Intitule);

                    // <JG> 31/05/2012 Correction adresse livraison existantes
                    Model.Sage.F_LIVRAISONRepository F_LIVRAISONRepository = new Model.Sage.F_LIVRAISONRepository();

                    if (!F_LIVRAISONRepository.ExistComptetIntitule(F_COMPTET.CT_Num, F_LIVRAISON.LI_Intitule)
                        && !F_LIVRAISONRepository.ExistComptetIntitule(F_COMPTET.CT_Num, intitule_iso)
                        && (bool)F_LIVRAISON.ExistCT_Num_LI_Intitule(Connexion) == false)
                    {
                        String Adresse2 = string.Empty;
                        if (PsAddress.Address1.Length > 35)
                        {
                            F_LIVRAISON.LI_Adresse = PsAddress.Address1.Substring(0, 35);
                            Adresse2 = PsAddress.Address1.Substring(35);
                        }
                        else
                        {
                            F_LIVRAISON.LI_Adresse = PsAddress.Address1;
                        }
                        if (!string.IsNullOrWhiteSpace(PsAddress.Address2))
                        {
                            Adresse2 += " " + PsAddress.Address2;
                        }

                        F_LIVRAISON.LI_Complement = (Adresse2.Length > 35) ? Adresse2.Substring(0, 35) : Adresse2;
                        F_LIVRAISON.LI_CodePostal = (PsAddress.PostCode.Length > 9) ? PsAddress.PostCode.Substring(0, 9) : PsAddress.PostCode;
                        F_LIVRAISON.LI_Ville = (PsAddress.City.Length > 35) ? PsAddress.City.Substring(0, 35) : PsAddress.City;

						Model.Prestashop.PsCountryRepository PsCountryRepository = new Model.Prestashop.PsCountryRepository();
						if (PsCountryRepository.Exist(PsAddress.IDCountry))
						{
							Model.Prestashop.PsCountry PsCountry = new Model.Prestashop.PsCountryRepository().Read(PsAddress.IDCountry);
							Model.Sage.F_PAYSRepository F_PAYSRepository = new Model.Sage.F_PAYSRepository();
							if (F_PAYSRepository.ExistPaysIso2(PsCountry.IsoCode))
							{
								F_LIVRAISON.LI_Pays = new Model.Sage.F_PAYSRepository().ReadPaysIso2(PsCountry.IsoCode).PA_Intitule;
							}
							else
							{
								Model.Prestashop.PsCountryLangRepository PsCountryLangRepository = new Model.Prestashop.PsCountryLangRepository();
								if (PsCountryLangRepository.ExistCountryLang(PsAddress.IDCountry, Core.Global.Lang))
								{
									Model.Prestashop.PsCountryLang PsCountryLang = PsCountryLangRepository.ReadCountryLang(PsAddress.IDCountry, Core.Global.Lang);
									F_LIVRAISON.LI_Pays = (PsCountryLang.Name.Length > 35) ? PsCountryLang.Name.Substring(0, 35) : PsCountryLang.Name;
								}
							}
						}

						String Contact = (Core.Global.GetConfig().ConfigClientSocieteIntituleActif && !string.IsNullOrEmpty(PsAddress.Company))
                            ? PsAddress.LastName + " " + PsAddress.FirstName
                            : (!string.IsNullOrEmpty(PsAddress.Company) ? PsAddress.Company : string.Empty);

                        // SI Module SoColissimo
                        //if (Core.Global.ExistSoColissimoDeliveryModule()
                        //    && Core.Global.GetConfig().ModuleSoColissimoDeliveryActive
                        //    && Core.Global.GetConfig().ModuleSoColissimoReplaceAddressNameActive
                        //    && info_cart != null && PsSoDelivery != null)
                        //{
                        //    Contact = (!string.IsNullOrWhiteSpace(PsSoDelivery.LiBelle))
                        //        ? PsSoDelivery.FirstName + " " + PsSoDelivery.LastName
                        //        : (!string.IsNullOrEmpty(PsSoDelivery.Company) ? PsSoDelivery.Company : string.Empty);
                        //}

                        F_LIVRAISON.LI_Contact = (Contact.Length > 35) ? Contact.Substring(0, 35) : Contact;

                        if (!string.IsNullOrWhiteSpace(PsAddress.Phone))
                        {
                            F_LIVRAISON.LI_Telephone = (PsAddress.Phone.Length > 21) ? PsAddress.Phone.Substring(0, 21) : PsAddress.Phone;
                            // <JG> 17/11/2014 correction si valeur nulle (tests AM)
                            if (!string.IsNullOrWhiteSpace(PsAddress.PhoneMobile))
                                F_LIVRAISON.LI_Telecopie = (PsAddress.PhoneMobile.Length > 21) ? PsAddress.PhoneMobile.Substring(0, 21) : PsAddress.PhoneMobile;
                        }
                        else if (!string.IsNullOrWhiteSpace(PsAddress.PhoneMobile))
                        {
                            if (Core.Global.GetConfig().ConfigClientAdresseTelephonePositionFixe)
                                F_LIVRAISON.LI_Telecopie = (PsAddress.PhoneMobile.Length > 21) ? PsAddress.PhoneMobile.Substring(0, 21) : PsAddress.PhoneMobile;
                            else
                                F_LIVRAISON.LI_Telephone = (PsAddress.PhoneMobile.Length > 21) ? PsAddress.PhoneMobile.Substring(0, 21) : PsAddress.PhoneMobile;
                        }

                        #region Module SoColissimo
                        if (Core.Global.ExistSoColissimoDeliveryModule()
                               && Core.Global.GetConfig().ModuleSoColissimoDeliveryActive
                               && Core.Global.GetConfig().ModuleSoColissimoReplacePhoneActive)
                        {
                            if (info_cart == null)
                            {
                                // si info_cart n'a pas déjà été identifié par rapport à l'option intitulé adresse
                                Model.Prestashop.PsOrdersRepository PsOrdersRepository = new Model.Prestashop.PsOrdersRepository();
                                List<Model.Prestashop.order_address> ListAddress = PsOrdersRepository.ListAddress(Core.Global.CurrentShop.IDShop, PsAddress.IDCustomer);
                                if (ListAddress != null)
                                {
                                    info_cart = (from L in ListAddress where L.id_address_delivery == PsAddress.IDAddress orderby L.id_order descending select L).FirstOrDefault();
                                }
                            }
                            if (info_cart != null && PsSoDelivery == null)
                            {
                                Model.Prestashop.PsSoDeliveryRepository PsSoDeliveryRepository = new Model.Prestashop.PsSoDeliveryRepository();
                                if (PsSoDeliveryRepository.ExistCart(info_cart.id_cart))
                                {
                                    PsSoDelivery = PsSoDeliveryRepository.ReadCart(info_cart.id_cart);
                                }
                            }
                            if (PsSoDelivery != null)
                            {
                                if (!string.IsNullOrWhiteSpace(PsSoDelivery.Telephone))
                                    F_LIVRAISON.LI_Telephone = (PsSoDelivery.Telephone.Length > 21) ? PsSoDelivery.Telephone.Substring(0, 21) : PsSoDelivery.Telephone;
                            }
                        }
                        #endregion

                        Model.Prestashop.PsCustomerRepository PsCustomerRepository = new Model.Prestashop.PsCustomerRepository();
                        if (PsCustomerRepository.ExistCustomer(PsAddress.IDCustomer))
                        {
                            string customer_mail = PsCustomerRepository.ReadCustomer(PsAddress.IDCustomer).Email;
                            F_LIVRAISON.LI_Email = (customer_mail.Length > 69) ? customer_mail.Substring(0, 69) : customer_mail;
                        }


                        Config = new Model.Local.Config();
                        if (ConfigRepository.ExistName(Core.Global.ConfigClientModeExpedition))
                        {
                            Config = ConfigRepository.ReadName(Core.Global.ConfigClientModeExpedition);
                            F_LIVRAISON.N_Expedition = Convert.ToInt32(Config.Con_Value);
                        }
                        Config = new Model.Local.Config();
                        if (ConfigRepository.ExistName(Core.Global.ConfigClientConditionLivraison))
                        {
                            Config = ConfigRepository.ReadName(Core.Global.ConfigClientConditionLivraison);
                            F_LIVRAISON.N_Condition = Convert.ToInt32(Config.Con_Value);
                        }

                        if (F_LIVRAISONRepository.ExistComptetPrincipal(F_COMPTET.CT_Num, 1))
                        {
                            F_LIVRAISON.LI_Principal = ABSTRACTION_SAGE.F_LIVRAISON.Obj._Enum_LI_Principal.Non_Principal;
                        }
                        else
                        {
                            F_LIVRAISON.LI_Principal = ABSTRACTION_SAGE.F_LIVRAISON.Obj._Enum_LI_Principal.Lieu_Principal;
                        }

                        // <JG> 03/06/2016 Ajout mise des infos en majuscule via option
                        if (Core.Global.GetConfig().ConfigClientInfosMajusculeActif)
                        {
                            F_LIVRAISON.LI_Intitule = F_LIVRAISON.LI_Intitule.ToUpper();
                            F_LIVRAISON.LI_Adresse = F_LIVRAISON.LI_Adresse.ToUpper();
                            F_LIVRAISON.LI_Complement = F_LIVRAISON.LI_Complement.ToUpper();
                            F_LIVRAISON.LI_CodePostal = F_LIVRAISON.LI_CodePostal.ToUpper();
                            F_LIVRAISON.LI_Ville = F_LIVRAISON.LI_Ville.ToUpper();
                            F_LIVRAISON.LI_Pays = F_LIVRAISON.LI_Pays.ToUpper();
                            F_LIVRAISON.LI_Contact = F_LIVRAISON.LI_Contact.ToUpper();
                        }

                        // sinon absence de mode d'expédition par défaut ou  de conditions de livraison par défaut blocage de l'insertion sinon erreur ODBC
                        if (F_LIVRAISON.N_Expedition == 0 || F_LIVRAISON.N_Condition == 0)
                        {
                            Core.Log.WriteLog("[SYNCHRO_ADRESSE_23] Mode d'expédition par défaut et/ou Conditions de livraison par défaut non définies. Impossible d'ajouter l'adresse de livraison !", true);
                        }
                        // si paramètres obligatoires en création renseignés -> ajout de l'adresse
                        else
                        {
                            F_LIVRAISON.Add(Connexion);

                            // <JG> 17/02/2017 traitement déplacé dans la création de la fiche client
                            // obsolète en V8 puisque l'ODBC ajoute en auto l'adresse de livraison par défaut
                            //if (F_LIVRAISON.LI_Principal == ABSTRACTION_SAGE.F_LIVRAISON.Obj._Enum_LI_Principal.Lieu_Principal)
                            //{
                            //    ABSTRACTION_SAGE.F_COMPTET.Obj ObjF_COMPTET = new ABSTRACTION_SAGE.F_COMPTET.Obj();
                            //    ObjF_COMPTET.CT_Num = F_COMPTET.CT_Num;
                            //    ObjF_COMPTET.ReadCT_Num(Connexion, false);
                            //    ObjF_COMPTET.CT_Adresse = F_LIVRAISON.LI_Adresse;
                            //    ObjF_COMPTET.CT_Complement = F_LIVRAISON.LI_Complement;
                            //    ObjF_COMPTET.CT_CodePostal = F_LIVRAISON.LI_CodePostal;
                            //    ObjF_COMPTET.CT_Ville = F_LIVRAISON.LI_Ville;
                            //    ObjF_COMPTET.CT_Pays = F_LIVRAISON.LI_Pays;
                            //    ObjF_COMPTET.CT_Contact = F_LIVRAISON.LI_Contact;
                            //    ObjF_COMPTET.CT_Telephone = F_LIVRAISON.LI_Telephone;
                            //    ObjF_COMPTET.CT_Telecopie = F_LIVRAISON.LI_Telecopie;

                            //    string tva = !string.IsNullOrWhiteSpace(PsAddress.VatNumber) ? PsAddress.VatNumber : string.Empty;
                            //    if (tva.Length > 25)
                            //        tva = tva.Replace(" ", "");
                            //    ObjF_COMPTET.CT_Identifiant = (tva.Length > 25) ? tva.Substring(0, 25) : tva;

                            //    ObjF_COMPTET.Update(Connexion);
                            //}

                            if (Core.Global.GetConfig().ConfigClientNIFActif && string.IsNullOrWhiteSpace(F_COMPTET.CT_Siret))
                            {
                                try
                                {
                                    string dni = !string.IsNullOrWhiteSpace(PsAddress.DNi) ? PsAddress.DNi.Replace(" ", "") : string.Empty;
                                    Connexion.Request = "UPDATE F_COMPTET SET CT_Siret='" + ((dni.Length > 15) ? dni.Substring(0, 15) : dni)
                                         + "' WHERE CT_Num='" + F_COMPTET.CT_Num + "'";
                                    Connexion.Exec_Request();
                                }
                                catch (Exception ex)
                                {
                                    Core.Error.SendMailError("Erreur insertion numéro d'identification fiscale !<br/>" + ex.ToString());
                                }
                            }
                        }
                    }
                    if (!F_LIVRAISONRepository.ExistComptetIntitule(F_COMPTET.CT_Num, F_LIVRAISON.LI_Intitule)
                        && F_LIVRAISON.LI_Intitule != intitule_iso
                        && (bool)F_LIVRAISON.ExistCT_Num_LI_Intitule(Connexion) == true)
                    {
                        F_LIVRAISON.ReadCT_Num_LI_Intitule(Connexion);
                        F_LIVRAISON.LI_Intitule = intitule_iso;
                        F_LIVRAISON.Update(Connexion);
                    }

                    if (F_LIVRAISONRepository.ExistComptetIntitule(F_COMPTET.CT_Num, F_LIVRAISON.LI_Intitule))
                    {
                        Model.Sage.F_LIVRAISON F_LIVRAISONUpdate = F_LIVRAISONRepository.ReadComptetIntitule(F_COMPTET.CT_Num, F_LIVRAISON.LI_Intitule);
                        Model.Local.Address Address = new Model.Local.Address();
                        Address.Pre_Id = Convert.ToInt32(PsAddress.IDAddress);

                        // <JG> 08/11/2012 suppression de l'ancienne occurence si l'adresse a été supprimée de Sage
                        // =>> clé primaire sur les 2 ID, update impossible
                        if (AddressRepository.ExistPrestashop(Address.Pre_Id))
                        {
                            Address = AddressRepository.ReadPrestashop(Address.Pre_Id);
                            AddressRepository.Delete(Address);
                            Address = new Model.Local.Address();
                            Address.Pre_Id = Convert.ToInt32(PsAddress.IDAddress);
                        }
                        Address.Sag_Id = F_LIVRAISONUpdate.cbMarq;
                        Address.Add_Date = (PsAddress.DateUpd != null && PsAddress.DateUpd > new DateTime(1753, 1, 2)) ? PsAddress.DateUpd : DateTime.Now.Date;
                        AddressRepository.Add(Address);
                    }

                    Core.Temp.ListAddressOnCurrentSync.Add(PsAddress.IDAddress);
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        // <JG> 08/11/2012 Correction mise-à-jour adresses de livraisons
        private void UpdateDistantToLocal(Model.Local.Customer Customer, Model.Prestashop.PsAddress PsAddress, ABSTRACTION_SAGE.ALTERNETIS.Connexion Connexion, Model.Local.Address Address)
        {
            try
            {
                Model.Sage.F_COMPTETRepository F_COMPTERepository = new Model.Sage.F_COMPTETRepository();
                if (F_COMPTERepository.ExistId(Customer.Sag_Id))
                {

                    ABSTRACTION_SAGE.F_LIVRAISON.Obj F_LIVRAISONUpdate = new ABSTRACTION_SAGE.F_LIVRAISON.Obj();
                    Model.Sage.F_COMPTET F_COMPTET = F_COMPTERepository.Read(Customer.Sag_Id);
                    F_LIVRAISONUpdate.CT_Num = F_COMPTET.CT_Num;

                    Model.Sage.F_LIVRAISONRepository F_LIVRAISONRepository = new Model.Sage.F_LIVRAISONRepository();
                    F_LIVRAISONUpdate.LI_No = (F_LIVRAISONRepository.ExistId(Address.Sag_Id) ? F_LIVRAISONRepository.ReadId(Address.Sag_Id).LI_No : 0);

                    if ((Boolean)F_LIVRAISONUpdate.ExistCT_Num_LI_No(Connexion))
                    {
                        F_LIVRAISONUpdate.ReadCT_Num_LI_No(Connexion, false);

                        // gestion de l'intitulé
                        Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                        Model.Local.Config Config = new Model.Local.Config();

                        // <JG> 07/09/2012 Modification gestion adresse livraison
                        if (ConfigRepository.ExistName(Core.Global.ConfigClientIntituleAdresse)
                            && ConfigRepository.ReadName(Core.Global.ConfigClientIntituleAdresse).Con_Value == Core.Global.ConfigClientIntituleAdresseEnum.NomPrenomPrestashop.ToString())
                        {
                            string temp = (Core.Global.GetConfig().ConfigClientSocieteIntituleActif && !string.IsNullOrEmpty(PsAddress.Company))
                            ? PsAddress.Company
                            : PsAddress.LastName + " " + PsAddress.FirstName;

                            string CodePrestashop = PsAddress.IDAddress.ToString();
                            Int32 maxlenght = 32 - CodePrestashop.Length;
                            if (temp.Length > maxlenght)
                            {
                                temp = temp.Substring(0, maxlenght);
                            }
                            temp += " [" + CodePrestashop + "]";
                            F_LIVRAISONUpdate.LI_Intitule = temp;
                        }
                        else
                        {
                            F_LIVRAISONUpdate.LI_Intitule = Core.Global.GetConfig().ConfigClientNumPrefixe + PsAddress.IDAddress.ToString();
                        }

                        String Adresse2 = string.Empty;
                        if (PsAddress.Address1.Length > 35)
                        {
                            F_LIVRAISONUpdate.LI_Adresse = PsAddress.Address1.Substring(0, 35);
                            Adresse2 = PsAddress.Address1.Substring(35);
                        }
                        else
                        {
                            F_LIVRAISONUpdate.LI_Adresse = PsAddress.Address1;
                        }
                        if (!string.IsNullOrWhiteSpace(PsAddress.Address2))
                        {
                            Adresse2 += PsAddress.Address2;
                        }

                        F_LIVRAISONUpdate.LI_Complement = (Adresse2.Length > 35) ? Adresse2.Substring(0, 35) : Adresse2;
                        F_LIVRAISONUpdate.LI_CodePostal = (PsAddress.PostCode.Length > 9) ? PsAddress.PostCode.Substring(0, 9) : PsAddress.PostCode;
                        F_LIVRAISONUpdate.LI_Ville = (PsAddress.City.Length > 35) ? PsAddress.City.Substring(0, 35) : PsAddress.City;

                        Model.Prestashop.PsCountryLangRepository PsCountryLangRepository = new Model.Prestashop.PsCountryLangRepository();
                        if (PsCountryLangRepository.ExistCountryLang(PsAddress.IDCountry, Core.Global.Lang))
                        {
                            Model.Prestashop.PsCountryLang PsCountryLang = PsCountryLangRepository.ReadCountryLang(PsAddress.IDCountry, Core.Global.Lang);
                            F_LIVRAISONUpdate.LI_Pays = (PsCountryLang.Name.Length > 35) ? PsCountryLang.Name.Substring(0, 35) : PsCountryLang.Name;
                        }

                        String Contact = (Core.Global.GetConfig().ConfigClientSocieteIntituleActif && !string.IsNullOrEmpty(PsAddress.Company))
                            ? PsAddress.LastName + " " + PsAddress.FirstName
                            : (!string.IsNullOrEmpty(PsAddress.Company) ? PsAddress.Company : string.Empty);
                        F_LIVRAISONUpdate.LI_Contact = (Contact.Length > 35) ? Contact.Substring(0, 35) : Contact;

                        if (!string.IsNullOrWhiteSpace(PsAddress.Phone))
                        {
                            F_LIVRAISONUpdate.LI_Telephone = (PsAddress.Phone.Length > 21) ? PsAddress.Phone.Substring(0, 21) : PsAddress.Phone;
                            // <JG> 17/11/2014 correction si valeur nulle (tests AM)
                            if (!string.IsNullOrWhiteSpace(PsAddress.PhoneMobile))
                                F_LIVRAISONUpdate.LI_Telecopie = (PsAddress.PhoneMobile.Length > 21) ? PsAddress.PhoneMobile.Substring(0, 21) : PsAddress.PhoneMobile;
                        }
                        else if (!string.IsNullOrWhiteSpace(PsAddress.PhoneMobile))
                        {
                            if (Core.Global.GetConfig().ConfigClientAdresseTelephonePositionFixe)
                                F_LIVRAISONUpdate.LI_Telecopie = (PsAddress.PhoneMobile.Length > 21) ? PsAddress.PhoneMobile.Substring(0, 21) : PsAddress.PhoneMobile;
                            else
                                F_LIVRAISONUpdate.LI_Telephone = (PsAddress.PhoneMobile.Length > 21) ? PsAddress.PhoneMobile.Substring(0, 21) : PsAddress.PhoneMobile;
                        }

                        Model.Prestashop.PsCustomerRepository PsCustomerRepository = new Model.Prestashop.PsCustomerRepository();
                        if (PsCustomerRepository.ExistCustomer(PsAddress.IDCustomer))
                        {
                            string customer_mail = PsCustomerRepository.ReadCustomer(PsAddress.IDCustomer).Email;
                            F_LIVRAISONUpdate.LI_Email = (customer_mail.Length > 69) ? customer_mail.Substring(0, 69) : customer_mail;
                        }

                        // <JG> 03/06/2016 Ajout mise des infos en majuscule via option
                        if (Core.Global.GetConfig().ConfigClientInfosMajusculeActif)
                        {
                            F_LIVRAISONUpdate.LI_Intitule = F_LIVRAISONUpdate.LI_Intitule.ToUpper();
                            F_LIVRAISONUpdate.LI_Adresse = F_LIVRAISONUpdate.LI_Adresse.ToUpper();
                            F_LIVRAISONUpdate.LI_Complement = F_LIVRAISONUpdate.LI_Complement.ToUpper();
                            F_LIVRAISONUpdate.LI_CodePostal = F_LIVRAISONUpdate.LI_CodePostal.ToUpper();
                            F_LIVRAISONUpdate.LI_Ville = F_LIVRAISONUpdate.LI_Ville.ToUpper();
                            F_LIVRAISONUpdate.LI_Pays = F_LIVRAISONUpdate.LI_Pays.ToUpper();
                            F_LIVRAISONUpdate.LI_Contact = F_LIVRAISONUpdate.LI_Contact.ToUpper();
                        }

                        F_LIVRAISONUpdate.Update(Connexion);
                    }
                }

                Core.Temp.ListAddressOnCurrentSync.Add(PsAddress.IDAddress);
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
    }
}
