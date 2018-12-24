using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Data;

namespace PRESTACONNECT.Core.Sync
{
    public class SynchronisationCommande
    {
        private int DL_LIGNE_CommentaireReferencePaiement = 0;
        // liste temporaire des règles de paniers déjà traités (gestion OLEA + bons de réductions)
        private List<uint> temp_order_cart_rule = new List<uint>();
        private enum _tydeDocCreate { Aucun, BC, Devis };

        public void Exec(ABSTRACTION_SAGE.ALTERNETIS.Connexion Connexion, Model.Prestashop.PsOrders PsOrders, Model.Prestashop.PsOrdersRepository PsOrdersRepository)
        {
            try
            {
                Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                Model.Local.Config ConfigBC = (ConfigRepository.ExistName(Core.Global.ConfigCommandeStatutCreateBC)) ? ConfigRepository.ReadName(Core.Global.ConfigCommandeStatutCreateBC) : null;
                Model.Local.Config ConfigDevis = (ConfigRepository.ExistName(Core.Global.ConfigCommandeStatutCreateDevis)) ? ConfigRepository.ReadName(Core.Global.ConfigCommandeStatutCreateDevis) : null;
                Boolean doCreateBC = false;
                Boolean doCreateDevis = false;

                String[] ValueBC = (ConfigBC != null && !string.IsNullOrEmpty(ConfigBC.Con_Value)) ? ConfigBC.Con_Value.Split('#') : string.Empty.Split('#');
                String[] ValueDevis = (ConfigDevis != null && !string.IsNullOrEmpty(ConfigDevis.Con_Value)) ? ConfigDevis.Con_Value.Split('#') : string.Empty.Split('#');

                if (ConfigBC != null || ConfigDevis != null)
                {
                    doCreateBC = ValueBC.Contains(PsOrders.CurrentState.ToString());
                    doCreateDevis = ValueDevis.Contains(PsOrders.CurrentState.ToString());

                    Model.Local.CustomerRepository CustomerRepository = new Model.Local.CustomerRepository();
                    if (CustomerRepository.ExistPrestashop(Convert.ToInt32(PsOrders.IDCustomer)))
                    {
                        Model.Local.OrderRepository OrderRepository = new Model.Local.OrderRepository();
                        if (doCreateBC || doCreateDevis)
                        {
                            // <JG> 15/07/2015 ajout filtre article solde précommande en paramètre
                            #region Complément paiement précommande
                            if (Core.Global.GetConfig().ModulePreorderActif
                                                    && PsOrders.IDCart != 0
                                                    && new Model.Prestashop.PsCartPreOrderRepository().ExistCart(PsOrders.IDCart)
                                                    && new Model.Prestashop.PsOrderDetailRepository().ListOrder(PsOrders.IDOrder).Count == 1
                                                    && new Model.Prestashop.PsOrderDetailRepository().ListOrder(PsOrders.IDOrder).First().ProductID == Core.Global.GetConfig().ModulePreorderPrestashopProduct
                                                    && !OrderRepository.ExistPrestashop((int)PsOrders.IDOrder))
                            {
                                uint Preorder_ID = new Model.Prestashop.PsCartPreOrderRepository().ReadCart(PsOrders.IDCart).IDPreOrder;

                                Model.Sage.F_DOCENTETERepository F_DOCENTETERepository = new Model.Sage.F_DOCENTETERepository();
                                Model.Sage.F_DOCENTETE F_DOCENTETE = F_DOCENTETERepository.ListWeb(Preorder_ID.ToString()).FirstOrDefault();

                                ABSTRACTION_SAGE.F_DOCENTETE.Obj ObjF_DOCENTETE = new ABSTRACTION_SAGE.F_DOCENTETE.Obj()
                                {
                                    DO_Domaine = (ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Domaine)F_DOCENTETE.DO_Domaine,
                                    DO_Type = (ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Type)F_DOCENTETE.DO_Type,
                                    DO_Piece = F_DOCENTETE.DO_Piece
                                };

                                ObjF_DOCENTETE.ReadDO_Domaine_DO_Type_DO_Piece(Connexion, false);
                                if (ObjF_DOCENTETE != null)
                                {
                                    bool exist_payment;
                                    this.ExecReglement(PsOrders, Connexion, ObjF_DOCENTETE, out exist_payment);

                                    // <JG> 13/06/2014 ajout saisie infolibre entete
                                    #region Infolibre entete
                                    try
                                    {
                                        Model.Sage.cbSysLibreRepository cbSysLibreRepository = new Model.Sage.cbSysLibreRepository();
                                        if (cbSysLibreRepository.ExistInformationLibre(Core.Global.GetConfig().ModulePreorderInfolibreEnteteName, Model.Sage.cbSysLibreRepository.CB_File.F_DOCENTETE))
                                        {
                                            Model.Sage.cbSysLibre cbSysLibre = cbSysLibreRepository.ReadInformationLibre(Core.Global.GetConfig().ModulePreorderInfolibreEnteteName, Model.Sage.cbSysLibreRepository.CB_File.F_DOCENTETE);
                                            if (cbSysLibre.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageText
                                                || cbSysLibre.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageTable)
                                            {
                                                if (!String.IsNullOrEmpty(Core.Global.GetConfig().ModulePreorderInfolibreEnteteValue))
                                                {
                                                    if (Core.Global.GetConfig().ModulePreorderInfolibreEnteteValue.Length >= cbSysLibre.CB_Len)
                                                        Core.Global.GetConfig().UpdateModulePreorderInfolibreEnteteValue(Core.Global.GetConfig().ModulePreorderInfolibreEnteteValue.Substring(0, cbSysLibre.CB_Len - 1));

                                                    ObjF_DOCENTETE.InfosLibres = new ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Col();
                                                    ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj info_abstraction = new ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj()
                                                    {
                                                        Len = cbSysLibre.CB_Len,
                                                        Name = cbSysLibre.CB_Name,
                                                        Pos = cbSysLibre.CB_Pos,
                                                        Table = cbSysLibre.CB_File,
                                                        Value = Core.Global.GetConfig().ModulePreorderInfolibreEnteteValue,
                                                    };
                                                    #region conversion cb_type
                                                    switch ((Model.Sage.cbSysLibreRepository.CB_Type)cbSysLibre.CB_Type)
                                                    {
                                                        //case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageSmallDate:
                                                        //    info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageSmallDate;
                                                        //    break;
                                                        //case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageValeur:
                                                        //    info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageValeur;
                                                        //    break;
                                                        case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageText:
                                                            info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageText;
                                                            break;
                                                        //case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageDate:
                                                        //    info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageDate;
                                                        //    break;
                                                        //case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageMontant:
                                                        //    info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageMontant;
                                                        //    break;
                                                        case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageTable:
                                                            info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageTable;
                                                            break;
                                                        case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.Deleted:
                                                        default:
                                                            break;
                                                    }
                                                    #endregion
                                                    ObjF_DOCENTETE.InfosLibres.Add(info_abstraction);
                                                    ObjF_DOCENTETE.Update(Connexion);
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Core.Error.SendMailError(ex.ToString());
                                    }
                                    #endregion

                                    // <JG> 21/02/2014 Modification de la commande originale dans Prestashop
                                    #region Update PrestaShop Order

                                    Model.Prestashop.PsOrderPaymentRepository PsOrderPaymentRepository = new Model.Prestashop.PsOrderPaymentRepository();
                                    Model.Prestashop.PsOrdersRepository PsOrdersRepository_PreOrder = new Model.Prestashop.PsOrdersRepository();

                                    if (PsOrderPaymentRepository.ExistOrderReference(PsOrders.Reference)
                                        && PsOrdersRepository_PreOrder.ExistOrder((int)Preorder_ID))
                                    {
                                        Model.Prestashop.PsOrders PreOrder = PsOrdersRepository_PreOrder.ReadOrder((int)Preorder_ID);
                                        bool flag_delete = true;

                                        // déplacement des règlements de solde sur la commande originale
                                        foreach (Model.Prestashop.PsOrderPayment PsOrderPayment in PsOrderPaymentRepository.ReadOrderReference(PsOrders.Reference))
                                        {
                                            if (new Model.Local.OrderPaymentRepository().ExistPayment(PsOrderPayment.IDOrderPayment))
                                            {
                                                PsOrderPayment.OrderReference = PreOrder.Reference;
                                                PsOrderPaymentRepository.Save();

                                                // changement statut Prestashop
                                                Model.Prestashop.PsOrderStateRepository PsOrderStateRepository = new Model.Prestashop.PsOrderStateRepository();
                                                if (Core.Global.GetConfig().ModulePreorderPrestashopOrderState != 0
                                                    && PsOrderStateRepository.ExistState((uint)Core.Global.GetConfig().ModulePreorderPrestashopOrderState))
                                                {
                                                    PsOrders.CurrentState = (uint)Core.Global.GetConfig().ModulePreorderPrestashopOrderState;
                                                    PsOrdersRepository.Save();
                                                    Model.Prestashop.PsOrderHistory PsOrderHistory = new Model.Prestashop.PsOrderHistory()
                                                    {
                                                        DateAdd = DateTime.Now,
                                                        IDOrder = PsOrders.IDOrder,
                                                        IDOrderState = PsOrders.CurrentState,
                                                    };
                                                    new Model.Prestashop.PsOrderHistoryRepository().Add(PsOrderHistory);
                                                }
                                                // marquage de la commande de solde comme synchronisée
                                                OrderRepository.Add(new Model.Local.Order()
                                                {
                                                    Pre_Id = Convert.ToInt32(PsOrders.IDOrder),
                                                    Sag_Id = F_DOCENTETE.cbMarq,
                                                });
                                            }
                                            else
                                            {
                                                flag_delete = false;
                                            }
                                            break;
                                        }

                                        if (flag_delete)
                                        {
                                            // <AM> 27/02/2014 Modification de l'adresse de livraison dans Sage
                                            try
                                            {
                                                Model.Local.AddressRepository AddressRepository = new Model.Local.AddressRepository();
                                                if (AddressRepository.ExistPrestashop((int)PsOrders.IDAddressDelivery))
                                                {
                                                    Model.Local.Address Adresse = AddressRepository.ReadPrestashop((int)PsOrders.IDAddressDelivery);
                                                    Model.Sage.F_LIVRAISONRepository F_LIVRAISONRepository = new Model.Sage.F_LIVRAISONRepository();
                                                    if (F_LIVRAISONRepository.ExistId(Adresse.Sag_Id))
                                                    {
                                                        Model.Sage.F_LIVRAISON F_LIVRAISON = F_LIVRAISONRepository.ReadId(Adresse.Sag_Id);
                                                        ObjF_DOCENTETE.LI_No = F_LIVRAISON.LI_No;
                                                        ObjF_DOCENTETE.Update(Connexion);
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Core.Error.SendMailError(ex.ToString());
                                            }

                                            // <JG> 07/07/2014 Ajout modification de l'adresse de facturation
                                            #region Update adresse facturation

                                            if (Core.Global.GetConfig().CommandeUpdateAdresseFacturation)
                                            {
                                                try
                                                {
                                                    Model.Prestashop.PsAddressRepository PsAddressRepository = new Model.Prestashop.PsAddressRepository();
                                                    if (PsAddressRepository.ExistAddress(PsOrders.IDAddressInvoice))
                                                    {
                                                        Model.Prestashop.PsAddress PsAddress = PsAddressRepository.ReadAddress(PsOrders.IDAddressInvoice);
                                                        ABSTRACTION_SAGE.F_COMPTET.Obj F_COMPTET_update = new ABSTRACTION_SAGE.F_COMPTET.Obj();
                                                        F_COMPTET_update.CT_Num = ObjF_DOCENTETE.DO_Tiers;
                                                        F_COMPTET_update.ReadCT_Num(Connexion, false);


                                                        String Intitule = //(!String.IsNullOrWhiteSpace(PsCustomer.Company) ? PsCustomer.Company + " ": "") + 
                                                            PsAddress.LastName + " " + PsAddress.FirstName;

                                                        F_COMPTET_update.CT_Intitule = (Intitule.Length > 35) ? Intitule.Substring(0, 35) : Intitule;

                                                        String Adresse2 = string.Empty;
                                                        if (PsAddress.Address1.Length > 35)
                                                        {
                                                            F_COMPTET_update.CT_Adresse = PsAddress.Address1.Substring(0, 35);
                                                            Adresse2 = PsAddress.Address1.Substring(35);
                                                        }
                                                        else
                                                        {
                                                            F_COMPTET_update.CT_Adresse = PsAddress.Address1;
                                                        }
                                                        if (!string.IsNullOrWhiteSpace(PsAddress.Address2))
                                                        {
                                                            Adresse2 += PsAddress.Address2;
                                                        }

                                                        F_COMPTET_update.CT_Complement = (Adresse2.Length > 35) ? Adresse2.Substring(0, 35) : Adresse2;
                                                        F_COMPTET_update.CT_CodePostal = (PsAddress.PostCode.Length > 9) ? PsAddress.PostCode.Substring(0, 9) : PsAddress.PostCode;
                                                        F_COMPTET_update.CT_Ville = (PsAddress.City.Length > 35) ? PsAddress.City.Substring(0, 35) : PsAddress.City;

                                                        Model.Prestashop.PsCountryLangRepository PsCountryLangRepository = new Model.Prestashop.PsCountryLangRepository();
                                                        if (PsCountryLangRepository.ExistCountryLang(PsAddress.IDCountry, Core.Global.Lang))
                                                        {
                                                            Model.Prestashop.PsCountryLang PsCountryLang = PsCountryLangRepository.ReadCountryLang(PsAddress.IDCountry, Core.Global.Lang);
                                                            F_COMPTET_update.CT_Pays = (PsCountryLang.Name.Length > 35) ? PsCountryLang.Name.Substring(0, 35) : PsCountryLang.Name;
                                                        }

                                                        F_COMPTET_update.CT_Contact = (PsAddress.Company.Length > 35) ? PsAddress.Company.Substring(0, 35) : PsAddress.Company;

                                                        if (!string.IsNullOrWhiteSpace(PsAddress.Phone))
                                                        {
                                                            F_COMPTET_update.CT_Telephone = (PsAddress.Phone.Length > 21) ? PsAddress.Phone.Substring(0, 21) : PsAddress.Phone;
                                                            F_COMPTET_update.CT_Telecopie = (PsAddress.PhoneMobile.Length > 21) ? PsAddress.PhoneMobile.Substring(0, 21) : PsAddress.PhoneMobile;
                                                        }
                                                        else if (!string.IsNullOrWhiteSpace(PsAddress.PhoneMobile))
                                                        {
                                                            F_COMPTET_update.CT_Telephone = (PsAddress.PhoneMobile.Length > 21) ? PsAddress.PhoneMobile.Substring(0, 21) : PsAddress.PhoneMobile;
                                                        }

                                                        Model.Prestashop.PsCustomerRepository PsCustomerRepository = new Model.Prestashop.PsCustomerRepository();
                                                        if (PsCustomerRepository.ExistCustomer(PsAddress.IDCustomer))
                                                        {
                                                            string customer_mail = PsCustomerRepository.ReadCustomer(PsAddress.IDCustomer).Email;
                                                            F_COMPTET_update.CT_Email = (customer_mail.Length > 69) ? customer_mail.Substring(0, 69) : customer_mail;
                                                        }

                                                        string tva = PsAddress.VatNumber;
                                                        if (tva.Length > 25)
                                                            tva = tva.Replace(" ", "");

                                                        F_COMPTET_update.CT_Identifiant = (tva.Length > 25) ? tva.Substring(0, 25) : tva;

                                                        F_COMPTET_update.Update(Connexion);
                                                    }
                                                }
                                                catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
                                            }

                                            #endregion


                                            // modification des montants de la commande originale
                                            PreOrder.TotalPaid += PreOrder.TotalDiscounts;
                                            PreOrder.TotalPaidReal += PreOrder.TotalDiscounts;
                                            PreOrder.TotalDiscounts = 0;
                                            PreOrder.TotalPaidTaxExCl += PreOrder.TotalDiscountsTaxExCl;
                                            PreOrder.TotalDiscountsTaxExCl = 0;
                                            PreOrder.TotalPaidTaxInCl += PreOrder.TotalDiscountsTaxInCl;
                                            PreOrder.TotalDiscountsTaxInCl = 0;
                                            PreOrder.IDAddressDelivery = PsOrders.IDAddressDelivery;
                                            PreOrder.IDAddressInvoice = PsOrders.IDAddressInvoice;
                                            PsOrdersRepository_PreOrder.Save();

                                            // suppression de la règle de panier sur la commande originale
                                            Model.Prestashop.PsOrderCartRuleRepository PsOrderCartRuleRepository = new Model.Prestashop.PsOrderCartRuleRepository();
                                            if (PsOrderCartRuleRepository.ExistOrder(PreOrder.IDOrder))
                                            {
                                                foreach (Model.Prestashop.PsOrderCartRule PsOrderCartRule in PsOrderCartRuleRepository.ListOrder(PreOrder.IDOrder))
                                                {
                                                    PsOrderCartRuleRepository.Delete(PsOrderCartRule);
                                                }
                                            }

                                            // suppression de la commande de solde
                                            Model.Prestashop.PsOrderDetailRepository PsOrderDetailRepository = new Model.Prestashop.PsOrderDetailRepository();
                                            if (PsOrdersRepository.ExistOrder((int)PsOrders.IDOrder))
                                            {
                                                PsOrderDetailRepository.DeleteAll(PsOrderDetailRepository.ListOrder(PsOrders.IDOrder));
                                            }
                                            PsOrdersRepository_PreOrder.Delete(PsOrders);
                                        }
                                    }

                                    #endregion
                                }
                            }
                            #endregion
                            else if (OrderRepository.ExistPrestashop(Convert.ToInt32(PsOrders.IDOrder)) == false)
                            {
								ABSTRACTION_SAGE.F_DOCENTETE.Obj Obj_F_DOCENTETE = 
									this.ExecDistantToLocal(PsOrders, Connexion, OrderRepository,
										((doCreateBC) ? _tydeDocCreate.BC
														: ((doCreateDevis) ? _tydeDocCreate.Devis
																			: _tydeDocCreate.Aucun)));

								if (PsOrders.IDOrder != 0 && Obj_F_DOCENTETE != null)
								{
									Model.Local.OrderMacketplaceRepository orderMacketplaceRepository = new Model.Local.OrderMacketplaceRepository();
									if (orderMacketplaceRepository.Exist())
									{
										foreach (Model.Local.OrderMacketplace order in orderMacketplaceRepository.List())
										{
											string request = order.Ord_MySQLRequest.Replace(order.Ord_ReplaceText, PsOrders.IDOrder.ToString());
											// Execution de la requete pour l'insertion dans la table F_DOCENTETE
											ConnectionInfos connection = Core.Global.GetConnectionInfos();
											DataTable result = connection.MySQLConnexionRequest(request);
											if (result != null && result.Rows.Count > 0 && !string.IsNullOrEmpty(result.Rows[0][0].ToString().Trim()))
											{
												// Insersion dans la table F_DOCENTETE
												connection.SAGEODBCConnexionF_DOCENTETE(order.Ord_ColoumName, result.Rows[0][0].ToString().Trim(), Obj_F_DOCENTETE);
											}
										}
									}
								}

								// <JG> 08/08/2013 ajout modification du statut de la commande PrestaShop directement après création du document dans Sage
								try
                                {
                                    if (OrderRepository.ExistPrestashop((int)PsOrders.IDOrder))
                                    {
                                        Model.Local.Order Order = OrderRepository.ReadPrestashop(Convert.ToInt32(PsOrders.IDOrder));
                                        this.ExecLocalToDistant(PsOrders, PsOrdersRepository);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Core.Error.SendMailError(ex.ToString());
                                }
                            }
                            else
                            {
                                Model.Local.Order Order = OrderRepository.ReadPrestashop(Convert.ToInt32(PsOrders.IDOrder));
                                this.ExecLocalToDistant(PsOrders, PsOrdersRepository);
                            }
                        }
                        else
                        {
                            if (OrderRepository.ExistPrestashop(Convert.ToInt32(PsOrders.IDOrder)) == true)
                            {
                                Model.Local.Order Order = OrderRepository.ReadPrestashop(Convert.ToInt32(PsOrders.IDOrder));
                                this.ExecLocalToDistant(PsOrders, PsOrdersRepository);
                            }
						}

						if (PsOrders.IDOrder != 0 && !string.IsNullOrWhiteSpace(Core.Global.GetConfig().CronCommandeURL))
						{
							string url = Core.Global.GetConfig().CronCommandeURL.Replace(Core.Global.GetConfig().CronCommandeBalise, PsOrders.IDOrder.ToString());
							Core.Global.LaunchCron(url, Core.Global.GetConfig().CronCommandeTimeout);
						}
					}
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private ABSTRACTION_SAGE.F_DOCENTETE.Obj ExecDistantToLocal(Model.Prestashop.PsOrders PsOrders,
            ABSTRACTION_SAGE.ALTERNETIS.Connexion Connexion,
            Model.Local.OrderRepository OrderRepository,
            _tydeDocCreate typeDoc)
		{
			ABSTRACTION_SAGE.F_DOCENTETE.Obj ObjF_DOCENTETE = null;
			try
            {
                #region lecture param config
                Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                Model.Local.Config Config = new Model.Local.Config();
                short Souche = 0;
                if (ConfigRepository.ExistName(Core.Global.ConfigCommandeSouche))
                {
                    Config = ConfigRepository.ReadName(Core.Global.ConfigCommandeSouche);
                    if (Core.Global.IsNumeric(Config.Con_Value))
                    {
                        Souche = Convert.ToInt16(Config.Con_Value);
                    }
                }
                if (Souche > 0) Souche -= 1;

                Config = new Model.Local.Config();
                Int32 Depot = 0;
                if (ConfigRepository.ExistName(Core.Global.ConfigCommandeDepot))
                {
                    Config = ConfigRepository.ReadName(Core.Global.ConfigCommandeDepot);
                    if (Core.Global.IsNumeric(Config.Con_Value))
                    {
                        Model.Sage.F_DEPOTRepository F_DEPOTRepository = new Model.Sage.F_DEPOTRepository();
                        if (F_DEPOTRepository.ExistId(Convert.ToInt32(Config.Con_Value)))
                        {
                            Model.Sage.F_DEPOT F_DEPOT = F_DEPOTRepository.ReadId(Convert.ToInt32(Config.Con_Value));
                            Depot = F_DEPOT.DE_No == null ? 0 : F_DEPOT.DE_No;
						}
                    }
                }

                Config = new Model.Local.Config();
                Int32 Statut = 0;
                switch (typeDoc)
                {
                    case _tydeDocCreate.BC:
                        if (ConfigRepository.ExistName(Core.Global.ConfigCommandeSageBCStatut))
                        {
                            Config = ConfigRepository.ReadName(Core.Global.ConfigCommandeSageBCStatut);
                            if (Core.Global.IsNumeric(Config.Con_Value))
                            {
                                Statut = Convert.ToInt32(Config.Con_Value);
                            }
                        }
                        break;
                    case _tydeDocCreate.Devis:
                        if (ConfigRepository.ExistName(Core.Global.ConfigCommandeSageDevisStatut))
                        {
                            Config = ConfigRepository.ReadName(Core.Global.ConfigCommandeSageDevisStatut);
                            if (Core.Global.IsNumeric(Config.Con_Value))
                            {
                                Statut = Convert.ToInt32(Config.Con_Value);
                            }
                        }
                        break;
                    case _tydeDocCreate.Aucun:
                    default:
                        break;
                }
                #endregion

                Model.Local.CustomerRepository CustomerRepository = new Model.Local.CustomerRepository();
                if (CustomerRepository.ExistPrestashop(Convert.ToInt32(PsOrders.IDCustomer)))
                {
                    Model.Local.Customer Customer = CustomerRepository.ReadPrestashop(Convert.ToInt32(PsOrders.IDCustomer));
                    Model.Sage.F_COMPTETRepository F_COMPTETRepository = new Model.Sage.F_COMPTETRepository();
                    if (F_COMPTETRepository.ExistId(Customer.Sag_Id))
                    {
                        Model.Sage.F_COMPTET F_COMPTET = F_COMPTETRepository.Read(Customer.Sag_Id);

                        #region Traitement update de l'adresse si non synchronisée

                        if (!Core.Temp.ListAddressOnCurrentSync.Contains(PsOrders.IDAddressDelivery))
                        {
                            Core.Sync.SynchronisationLivraison SynchronisationLivraison = new Core.Sync.SynchronisationLivraison();
                            SynchronisationLivraison.Exec(Connexion, PsOrders.IDAddressDelivery);
                        }

                        #endregion

                        #region Update adresse facturation

                        if (Core.Global.GetConfig().CommandeUpdateAdresseFacturation && typeDoc == _tydeDocCreate.BC)
                        {
                            try
                            {
                                Model.Prestashop.PsAddressRepository PsAddressRepository = new Model.Prestashop.PsAddressRepository();
                                if (PsAddressRepository.ExistAddress(PsOrders.IDAddressInvoice))
                                {
                                    Model.Prestashop.PsAddress PsAddress = PsAddressRepository.ReadAddress(PsOrders.IDAddressInvoice);
                                    ABSTRACTION_SAGE.F_COMPTET.Obj F_COMPTET_update = new ABSTRACTION_SAGE.F_COMPTET.Obj();
                                    F_COMPTET_update.CT_Num = F_COMPTET.CT_Num;
                                    F_COMPTET_update.ReadCT_Num(Connexion, false);


                                    String Intitule = (Core.Global.GetConfig().ConfigClientSocieteIntituleActif && !string.IsNullOrEmpty(PsAddress.Company))
                                        ? PsAddress.Company
                                        : PsAddress.LastName + " " + PsAddress.FirstName;

                                    F_COMPTET_update.CT_Intitule = (Intitule.Length > 35) ? Intitule.Substring(0, 35) : Intitule;

                                    String Adresse2 = string.Empty;
                                    if (PsAddress.Address1.Length > 35)
                                    {
                                        F_COMPTET_update.CT_Adresse = PsAddress.Address1.Substring(0, 35);
                                        Adresse2 = PsAddress.Address1.Substring(35);
                                    }
                                    else
                                    {
                                        F_COMPTET_update.CT_Adresse = PsAddress.Address1;
                                    }
                                    if (!string.IsNullOrWhiteSpace(PsAddress.Address2))
                                    {
                                        Adresse2 += PsAddress.Address2;
                                    }

                                    F_COMPTET_update.CT_Complement = (Adresse2.Length > 35) ? Adresse2.Substring(0, 35) : Adresse2;
                                    F_COMPTET_update.CT_CodePostal = (PsAddress.PostCode.Length > 9) ? PsAddress.PostCode.Substring(0, 9) : PsAddress.PostCode;
                                    F_COMPTET_update.CT_Ville = (PsAddress.City.Length > 35) ? PsAddress.City.Substring(0, 35) : PsAddress.City;

                                    Model.Prestashop.PsCountryLangRepository PsCountryLangRepository = new Model.Prestashop.PsCountryLangRepository();
                                    if (PsCountryLangRepository.ExistCountryLang(PsAddress.IDCountry, Core.Global.Lang))
                                    {
                                        Model.Prestashop.PsCountryLang PsCountryLang = PsCountryLangRepository.ReadCountryLang(PsAddress.IDCountry, Core.Global.Lang);
                                        F_COMPTET_update.CT_Pays = (PsCountryLang.Name.Length > 35) ? PsCountryLang.Name.Substring(0, 35) : PsCountryLang.Name;
                                    }

                                    String Contact = (Core.Global.GetConfig().ConfigClientSocieteIntituleActif && !string.IsNullOrEmpty(PsAddress.Company))
                                        ? PsAddress.LastName + " " + PsAddress.FirstName
                                        : (!string.IsNullOrEmpty(PsAddress.Company) ? PsAddress.Company : string.Empty);
                                    F_COMPTET_update.CT_Contact = (Contact.Length > 35) ? Contact.Substring(0, 35) : Contact;

                                    if (!string.IsNullOrWhiteSpace(PsAddress.Phone))
                                    {
                                        F_COMPTET_update.CT_Telephone = (PsAddress.Phone.Length > 21) ? PsAddress.Phone.Substring(0, 21) : PsAddress.Phone;
                                        if (!string.IsNullOrWhiteSpace(PsAddress.PhoneMobile))
                                            F_COMPTET_update.CT_Telecopie = (PsAddress.PhoneMobile.Length > 21) ? PsAddress.PhoneMobile.Substring(0, 21) : PsAddress.PhoneMobile;
                                    }
                                    else if (!string.IsNullOrWhiteSpace(PsAddress.PhoneMobile))
                                    {
                                        F_COMPTET_update.CT_Telephone = (PsAddress.PhoneMobile.Length > 21) ? PsAddress.PhoneMobile.Substring(0, 21) : PsAddress.PhoneMobile;
                                    }

                                    Model.Prestashop.PsCustomerRepository PsCustomerRepository = new Model.Prestashop.PsCustomerRepository();
                                    if (PsCustomerRepository.ExistCustomer(PsAddress.IDCustomer))
                                    {
                                        string customer_mail = PsCustomerRepository.ReadCustomer(PsAddress.IDCustomer).Email;
                                        F_COMPTET_update.CT_Email = (customer_mail.Length > 69) ? customer_mail.Substring(0, 69) : customer_mail;
                                    }

                                    string tva = PsAddress.VatNumber;
                                    if (tva.Length > 25)
                                        tva = tva.Replace(" ", "");

                                    F_COMPTET_update.CT_Identifiant = (tva.Length > 25) ? tva.Substring(0, 25) : tva;

                                    if (Core.Global.GetConfig().ConfigClientCiviliteActif)
                                    {
                                        if (PsCustomerRepository.ExistCustomer(PsOrders.IDCustomer))
                                        {
                                            uint Gender = PsCustomerRepository.ReadCustomer(PsOrders.IDCustomer).IDGender;
                                            string civilite = string.Empty;
                                            Model.Prestashop.PsGenderLangRepository PsGenderLangRepository = new Model.Prestashop.PsGenderLangRepository();
                                            if (PsGenderLangRepository.ExistGenderLang(Gender, Core.Global.Lang))
                                            {
                                                civilite = PsGenderLangRepository.ReadGenderLang(Gender, Core.Global.Lang).Name;
                                                F_COMPTET.CT_Qualite = (civilite.Length > 17) ? civilite.Substring(0, 17) : civilite;
                                            }
                                        }
                                    }

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

                                    // <JG> 03/06/2016 Ajout mise des infos en majuscule via option
                                    if (Core.Global.GetConfig().ConfigClientInfosMajusculeActif)
                                    {
                                        F_COMPTET_update.CT_Intitule = F_COMPTET_update.CT_Intitule.ToUpper();
                                        F_COMPTET_update.CT_Classement = F_COMPTET_update.CT_Classement.ToUpper();
                                        F_COMPTET_update.CT_Contact = F_COMPTET_update.CT_Contact.ToUpper();
                                        F_COMPTET_update.CT_Ape = F_COMPTET_update.CT_Ape.ToUpper();
                                        F_COMPTET_update.CT_Qualite = F_COMPTET_update.CT_Qualite.ToUpper();

                                        F_COMPTET_update.CT_Adresse = F_COMPTET_update.CT_Adresse.ToUpper();
                                        F_COMPTET_update.CT_Complement = F_COMPTET_update.CT_Complement.ToUpper();
                                        F_COMPTET_update.CT_CodePostal = F_COMPTET_update.CT_CodePostal.ToUpper();
                                        F_COMPTET_update.CT_Ville = F_COMPTET_update.CT_Ville.ToUpper();
                                        F_COMPTET_update.CT_Pays = F_COMPTET_update.CT_Pays.ToUpper();
                                        F_COMPTET_update.CT_Contact = F_COMPTET_update.CT_Contact.ToUpper();
                                    }

                                    F_COMPTET_update.Update(Connexion);
                                }
                            }
                            catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
                        }

                        #endregion

                        Model.Local.AddressRepository AddressRepository = new Model.Local.AddressRepository();

                        if (AddressRepository.ExistPrestashop(Convert.ToInt32(PsOrders.IDAddressDelivery)))
                        {
                            Model.Local.Address Address = AddressRepository.ReadPrestashop(Convert.ToInt32(PsOrders.IDAddressDelivery));
                            Model.Sage.F_LIVRAISONRepository F_LIVRAISONRepository = new Model.Sage.F_LIVRAISONRepository();
                            if (F_LIVRAISONRepository.ExistId(Address.Sag_Id))
                            {
                                Model.Sage.F_LIVRAISON F_LIVRAISON = F_LIVRAISONRepository.ReadId(Address.Sag_Id);

                                // <JG> 31/05/2012 Correction préventive adresse de livraison existante mais démappée par rapport au client
                                if (F_LIVRAISON.CT_Num == F_COMPTET.CT_Num)
                                {
                                    ObjF_DOCENTETE = new ABSTRACTION_SAGE.F_DOCENTETE.Obj();
                                    ObjF_DOCENTETE.DO_Domaine = ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Domaine.Vente;
                                    ObjF_DOCENTETE.DO_Type = (typeDoc == _tydeDocCreate.BC)
                                        ? ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Type.Bon_Commande_Vente
                                        : ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Type.Devis_Vente;

                                    #region lecture n° de pièce
                                    Model.Sage.F_DOCCURRENTPIECERepository F_DOCCURRENTPIECERepository = new Model.Sage.F_DOCCURRENTPIECERepository();
                                    short doc = (typeDoc == _tydeDocCreate.BC) ? (short)1 : (short)0;
                                    Model.Sage.F_DOCCURRENTPIECE F_DOCCURRENTPIECE = F_DOCCURRENTPIECERepository.ReadDomaineColSouche(0, doc, Souche);

                                    // <JG> bloc de controle de numéro de pièce 28/03/2014
                                    String CurrentNumeroPiece = F_DOCCURRENTPIECE.DC_Piece;
                                    try
                                    {
                                        while (new Model.Sage.F_DOCENTETERepository().ListLight((short)ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Domaine.Vente,
                                                (short)ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Type.Devis_Vente)
                                                .Count(d => d.DO_Piece == CurrentNumeroPiece) > 0
                                            || new Model.Sage.F_DOCENTETERepository().ListLight((short)ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Domaine.Vente,
                                                (short)ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Type.Bon_Commande_Vente)
                                                .Count(d => d.DO_Piece == CurrentNumeroPiece) > 0
                                            || new Model.Sage.F_DOCLIGNERepository().ListLight((short)ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DO_Domaine.Vente,
                                                (short)ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DO_Type.Preparation_Livraison_Vente,
                                                (short)ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DO_Type.Bon_Livraison_Vente,
                                                (short)ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DO_Type.Facture_Vente,
                                                (short)ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DO_Type.Facture_Comptabilisee_Vente)
                                                .Count(d => d.DL_PieceBC == CurrentNumeroPiece) > 0)
                                        {
                                            string lettrage;
                                            long numero;
                                            if (Core.Global.ExtractNumeroPiece(CurrentNumeroPiece, out lettrage, out numero) == true)
                                            {
                                                CurrentNumeroPiece = lettrage + numero.ToString();
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Core.Error.SendMailError(ex.ToString());
                                    }

                                    ObjF_DOCENTETE.DO_Piece = CurrentNumeroPiece;
                                    #endregion

                                    ObjF_DOCENTETE.DO_Date = PsOrders.DateAdd;
                                    ObjF_DOCENTETE.DO_Tiers = F_COMPTET.CT_Num;
                                    ObjF_DOCENTETE.CO_No = F_COMPTET.CO_No.Value;
                                    ObjF_DOCENTETE.DO_Period = F_COMPTET.N_Period.Value;
                                    ObjF_DOCENTETE.DO_Devise = F_COMPTET.N_Devise.Value;
                                    ObjF_DOCENTETE.DE_No = Depot;
                                    ObjF_DOCENTETE.LI_No = F_LIVRAISON.LI_No;
                                    ObjF_DOCENTETE.CT_NumPayeur = F_COMPTET.CT_NumPayeur;

                                    if (Core.Global.GetConfig().CommandeReferencePrestaShop)
                                        ObjF_DOCENTETE.DO_Ref = (PsOrders.Reference.Length > 17) ? PsOrders.Reference.Substring(0, 17) : PsOrders.Reference;

                                    Model.Local.CarrierRepository CarrierRepository = new Model.Local.CarrierRepository();
                                    if (CarrierRepository.ExistPrestashop(Convert.ToInt32(PsOrders.IDCarrier)))
                                    {
                                        Model.Local.Carrier Carrier = CarrierRepository.ReadPrestashop(Convert.ToInt32(PsOrders.IDCarrier));
                                        ObjF_DOCENTETE.DO_Expedit = Carrier.Sag_Id;

                                        #region old_system
                                        //<JG> 28/08/2012 récupération taux de taxe
                                        //Model.Prestashop.PsAddressRepository PsAddressRepository = new Model.Prestashop.PsAddressRepository();
                                        //Model.Prestashop.PsAddress PsAddress = PsAddressRepository.ReadAddress(PsOrders.IDAddressDelivery);

                                        //Model.Prestashop.PsCarrierRepository PsCarrierRepository = new Model.Prestashop.PsCarrierRepository();
                                        //Model.Prestashop.PsCarrier PsCarrier = PsCarrierRepository.ReadCarrier((uint)Carrier.Pre_Id);

                                        //Model.Prestashop.PsTaxRuleRepository PsTaxRuleRepository = new Model.Prestashop.PsTaxRuleRepository();
                                        //if (PsTaxRuleRepository.ExistTaxeRulesGroupCountry((int)PsCarrier.IDTaxRulesGroup, (int)PsAddress.IDCountry))
                                        //{
                                        //    Model.Prestashop.PsTaxRule PsTaxRule = PsTaxRuleRepository.ReadTaxesRulesGroupCountry((int)PsCarrier.IDTaxRulesGroup, (int)PsAddress.IDCountry);

                                        //    Model.Prestashop.PsTaxRepository PsTaxRepository = new Model.Prestashop.PsTaxRepository();
                                        //    Model.Prestashop.PsTax PsTax = PsTaxRepository.ReadTax((uint)PsTaxRule.IDTax);

                                        //    ObjF_DOCENTETE.DO_Taxe1 = (Double)PsTax.Rate;
                                        //}
                                        //-----END <JG> 28/08/2012
                                        #endregion
                                    }
                                    else
                                    {
                                        Model.Prestashop.PsCarrierRepository PsCarrierRepository = new Model.Prestashop.PsCarrierRepository();
                                        Model.Prestashop.PsCarrier PsCarrier = PsCarrierRepository.ReadCarrier(PsOrders.IDCarrier);
                                        bool findCarrier = false;
                                        if (PsCarrier != null)
                                        {
                                            foreach (Model.Prestashop.PsCarrier old in PsCarrierRepository.ListIDReference(PsCarrier.IDReference))
                                            {
                                                if (CarrierRepository.ExistPrestashop((int)old.IDCarrier))
                                                {
                                                    Model.Local.Carrier Carrier = CarrierRepository.ReadPrestashop((int)old.IDCarrier);
                                                    ObjF_DOCENTETE.DO_Expedit = Carrier.Sag_Id;
                                                    findCarrier = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (!findCarrier)
                                        {
                                            ObjF_DOCENTETE.DO_Expedit = F_COMPTET.N_Expedition.Value;
                                        }
                                    }
                                    ObjF_DOCENTETE.DO_BLFact = (ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_Boolean)F_COMPTET.CT_BLFact;
                                    ObjF_DOCENTETE.DO_Reliquat = ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_Boolean.Non;
                                    ObjF_DOCENTETE.DO_Imprim = ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_Boolean.Non;
                                    if (F_COMPTET.CA_Num != null)
                                    {
                                        ObjF_DOCENTETE.CA_Num = F_COMPTET.CA_Num;
                                    }
                                    else
                                    {
                                        Config = new Model.Local.Config();
                                        if (ConfigRepository.ExistName(Core.Global.ConfigClientCodeAffaire))
                                        {
                                            Config = ConfigRepository.ReadName(Core.Global.ConfigClientCodeAffaire);
                                            Model.Sage.F_COMPTEARepository F_COMPTEARepository = new Model.Sage.F_COMPTEARepository();
                                            if (F_COMPTEARepository.ExistId(Convert.ToInt32(Config.Con_Value)))
                                            {
                                                Model.Sage.F_COMPTEA F_COMPTEA = F_COMPTEARepository.ReadId(Convert.ToInt32(Config.Con_Value));
                                                ObjF_DOCENTETE.CA_Num = F_COMPTEA.CA_Num;
                                            }
                                        }
                                    }
                                    ObjF_DOCENTETE.DO_Souche = Souche;
									//ObjF_DOCENTETE.DO_DateLivr = ObjF_DOCENTETE.DO_Date;
									ObjF_DOCENTETE.DO_DateLivr = Global.GetConfig().DateLivraisonMode == Parametres.DateLivraisonMode.DateCommandeInc ?
										PsOrders.DateAdd.AddDays(Global.GetConfig().DateLivraisonJours)
										: PsOrders.DeliveryDate != DateTime.MinValue ? PsOrders.DeliveryDate : PsOrders.DateAdd;
									ObjF_DOCENTETE.DO_Condition = (F_LIVRAISON.N_Condition != null) ? F_LIVRAISON.N_Condition.Value : F_COMPTET.N_Condition.Value;
                                    ObjF_DOCENTETE.DO_Tarif = F_COMPTET.N_CatTarif.Value;

                                    ObjF_DOCENTETE.N_CatCompta = SynchronisationClient.ReadCatCompta(null, PsOrders);
                                    if (ObjF_DOCENTETE.N_CatCompta == 0)
                                        ObjF_DOCENTETE.N_CatCompta = F_COMPTET.N_CatCompta.Value;
                                    ObjF_DOCENTETE.CG_Num = F_COMPTET.CG_NumPrinc;

                                    ObjF_DOCENTETE.DO_Statut = (ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Statut)Statut;
                                    ObjF_DOCENTETE.DO_NoWeb = PsOrders.IDOrder.ToString();
                                    ObjF_DOCENTETE.CT_NumCentrale = F_COMPTET.CT_NumCentrale;

                                    #region frais de port

                                    ObjF_DOCENTETE.DO_TypeFrais = ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_TypeFrais.Montant_Forfaitaire;
                                    ObjF_DOCENTETE.DO_TypeFranco = ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_TypeFranco.Montant_Forfaitaire;
                                    // <JG> 26/10/2012 Correction insertion des frais de port HT ou TTC
                                    Model.Sage.P_EXPEDITION P_EXPEDITION = new Model.Sage.P_EXPEDITIONRepository().Read(ObjF_DOCENTETE.DO_Expedit);
                                    ObjF_DOCENTETE.DO_TypeLigneFrais = (P_EXPEDITION.E_TypeLigneFrais.Value == (int)ABSTRACTION_SAGE.P_EXPEDITION.Obj._Enum_HT_TTC.TTC)
                                                                            ? ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_TypeLigne.TTC
                                                                            : ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_TypeLigne.HT;
                                    ObjF_DOCENTETE.DO_TypeLigneFranco = (P_EXPEDITION.E_TypeLigneFranco.Value == (int)ABSTRACTION_SAGE.P_EXPEDITION.Obj._Enum_HT_TTC.TTC)
                                                                            ? ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_TypeLigne.TTC
                                                                            : ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_TypeLigne.HT;
                                    if (!Core.Global.GetConfig().LigneFraisPort
                                        || !new Model.Sage.F_ARTICLERepository().ExistReference(Core.Global.GetConfig().LigneArticlePort))
                                    {
                                        // Presence de taxe sur Prestashop
                                        if (PsOrders.CarrierTaxRate != 0)
                                        {
                                            string TA_Code = string.Empty;
                                            // <AM> 20/02/2014 Ajout pour trouver le Ta_Code sur F_DOCENTETE
                                            Model.Prestashop.PsCarrierTaxRulesGroupShopRepository PsCarrierTaxRulesGroupShopRepository = new Model.Prestashop.PsCarrierTaxRulesGroupShopRepository();
                                            if (PsCarrierTaxRulesGroupShopRepository.ExistCarrierShop(PsOrders.IDCarrier, Core.Global.CurrentShop.IDShop))
                                            {
                                                uint IdTaxeRulesGroup = PsCarrierTaxRulesGroupShopRepository.ReadCarrierShop(PsOrders.IDCarrier, Core.Global.CurrentShop.IDShop).IDTaxRulesGroup;
                                                Model.Local.TaxRepository TaxRepository = new Model.Local.TaxRepository();
                                                if (TaxRepository.ExistPrestashop((int)IdTaxeRulesGroup))
                                                {
                                                    Int32 cbmarq = TaxRepository.ReadPrestashop((int)IdTaxeRulesGroup).Sag_Id;
                                                    Model.Sage.F_TAXERepository F_TAXERepository = new Model.Sage.F_TAXERepository();
                                                    if (F_TAXERepository.Exist(cbmarq))
                                                    {
                                                        TA_Code = F_TAXERepository.Read(cbmarq).TA_Code;
                                                    }
                                                }
                                            }
                                            switch (Core.Global.GetConfig().TaxSageTVA)
                                            {
                                                case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe1:
                                                    ObjF_DOCENTETE.DO_Taxe1 = PsOrders.CarrierTaxRate;
													#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
													ObjF_DOCENTETE.DO_CodeTaxe1 = TA_Code;
													#endif
                                                    break;
                                                case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe2:
                                                    ObjF_DOCENTETE.DO_Taxe2 = PsOrders.CarrierTaxRate;
													#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                                    ObjF_DOCENTETE.DO_CodeTaxe2 = TA_Code;
													#endif
                                                    break;
                                                case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe3:
                                                    ObjF_DOCENTETE.DO_Taxe3 = PsOrders.CarrierTaxRate;
													#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                                    ObjF_DOCENTETE.DO_CodeTaxe3 = TA_Code;
													#endif
                                                    break;
                                                case PRESTACONNECT.Core.Parametres.TaxSage.Empty:
                                                default:
                                                    break;
                                            }
                                            if (P_EXPEDITION.E_TypeLigneFrais.Value == (int)ABSTRACTION_SAGE.P_EXPEDITION.Obj._Enum_HT_TTC.TTC)
                                                ObjF_DOCENTETE.DO_ValFrais = PsOrders.TotalShipping;
                                            else
                                                ObjF_DOCENTETE.DO_ValFrais = PsOrders.TotalShipping / (1 + (PsOrders.CarrierTaxRate / 100));
                                        }
                                        // absence de taxe sur Prestashop
                                        else
                                        {
                                            ObjF_DOCENTETE.DO_ValFrais = PsOrders.TotalShipping;
                                        }
                                    }
                                    #endregion

                                    // HardCoded
                                    ObjF_DOCENTETE.DO_TypeColis = 1;
                                    ObjF_DOCENTETE.DO_Colisage = 1;
                                    ObjF_DOCENTETE.DO_Transaction = 11;
                                    ObjF_DOCENTETE.DO_Regime = 21;
                                    ObjF_DOCENTETE.DO_Ventile = ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_Boolean.Non;
                                    ObjF_DOCENTETE.AB_No = 0;
                                    //ObjF_DOCENTETE.DO_BLFact = ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_Boolean.Oui;
                                    //ObjF_DOCENTETE.DO_StatutFacture = ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_StatutFacture.Aucun;

                                    ObjF_DOCENTETE.DO_NbFacture = (F_COMPTET.CT_Facture != null) ? (int)F_COMPTET.CT_Facture : 1;

                                    if (F_COMPTET.CT_Langue != null)
                                        ObjF_DOCENTETE.DO_Langue = (ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Langue)F_COMPTET.CT_Langue.Value;


                                    #region Adresse facturation Entete 1 à 4
                                    Model.Prestashop.PsAddressRepository PsAddressRepository = new Model.Prestashop.PsAddressRepository();
                                    if (Core.Global.GetConfig().CommandeInsertFacturationEntete && PsAddressRepository.ExistAddress(PsOrders.IDAddressInvoice))
                                    {
                                        try
                                        {
                                            int do_coord_length = 25;
                                            Model.Prestashop.PsAddress PsAddressInvoice = PsAddressRepository.ReadAddress(PsOrders.IDAddressInvoice);
                                            if (PsAddressInvoice != null)
                                            {
                                                string name = (PsAddressInvoice.FirstName + " " + PsAddressInvoice.LastName).Trim();
                                                ObjF_DOCENTETE.DO_Coord01 = (!string.IsNullOrWhiteSpace(name)) ?
                                                    ((name.Length > do_coord_length) ? name.Substring(0, do_coord_length) : name)
                                                    : string.Empty;

                                                ObjF_DOCENTETE.DO_Coord02 = (!string.IsNullOrWhiteSpace(PsAddressInvoice.Company)) ?
                                                    ((PsAddressInvoice.Company.Length > do_coord_length) ? PsAddressInvoice.Company.Substring(0, do_coord_length) : PsAddressInvoice.Company)
                                                    : string.Empty;

                                                string address = (PsAddressInvoice.Address1 + " " + PsAddressInvoice.Address2).Trim();
                                                ObjF_DOCENTETE.DO_Coord03 = (!string.IsNullOrWhiteSpace(address)) ?
                                                    ((address.Length > do_coord_length) ? address.Substring(0, do_coord_length) : address)
                                                    : string.Empty;

                                                string cp_ville = (PsAddressInvoice.PostCode + " " + PsAddressInvoice.City).Trim();
                                                ObjF_DOCENTETE.DO_Coord04 = (!string.IsNullOrWhiteSpace(cp_ville)) ?
                                                    ((cp_ville.Length > do_coord_length) ? cp_ville.Substring(0, do_coord_length) : cp_ville)
                                                    : string.Empty;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Core.Error.SendMailError(ex.ToString());
                                        }
                                    }
                                    #endregion

                                    #region Module SoColissimo
                                    Model.Prestashop.PsSoDelivery PsSoDelivery = null;
                                    if (Core.Global.ExistSoColissimoDeliveryModule()
                                           && Core.Global.GetConfig().ModuleSoColissimoDeliveryActive)
                                    {
                                        Model.Sage.cbSysLibreRepository cbSysLibreRepository = new Model.Sage.cbSysLibreRepository();
                                        if (Core.Global.GetConfig().ModuleSoColissimoInfolibreTypePointActive
                                            && !string.IsNullOrWhiteSpace(Core.Global.GetConfig().ModuleSoColissimoInfolibreEnteteTypePointName)
                                            && cbSysLibreRepository.ExistInformationLibre(Core.Global.GetConfig().ModuleSoColissimoInfolibreEnteteTypePointName, Model.Sage.cbSysLibreRepository.CB_File.F_DOCENTETE))
                                        {

                                            Model.Prestashop.PsSoDeliveryRepository PsSoDeliveryRepository = new Model.Prestashop.PsSoDeliveryRepository();
                                            if (PsSoDeliveryRepository.ExistCart(PsOrders.IDCart))
                                            {
                                                PsSoDelivery = PsSoDeliveryRepository.ReadCart(PsOrders.IDCart);

                                                if (PsSoDelivery != null)
                                                {
                                                    string relais = PsSoDelivery.Type + (PsSoDelivery.PointID != 0 ? PsSoDelivery.PointID.ToString() : string.Empty);

                                                    Model.Sage.cbSysLibre InfolibrePointRelais = cbSysLibreRepository.ReadInformationLibre(Core.Global.GetConfig().ModuleSoColissimoInfolibreEnteteTypePointName, Model.Sage.cbSysLibreRepository.CB_File.F_DOCENTETE);

                                                    if (InfolibrePointRelais.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageText
                                                        || InfolibrePointRelais.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageTable)
                                                    {
                                                        if (relais.Length >= InfolibrePointRelais.CB_Len)
                                                            relais = relais.Substring(0, InfolibrePointRelais.CB_Len - 1);

                                                        if (ObjF_DOCENTETE.InfosLibres == null)
                                                            ObjF_DOCENTETE.InfosLibres = new ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Col();
                                                        ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj info_abstraction_pointrelais = new ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj()
                                                        {
                                                            Len = InfolibrePointRelais.CB_Len,
                                                            Name = InfolibrePointRelais.CB_Name,
                                                            Pos = InfolibrePointRelais.CB_Pos,
                                                            Table = InfolibrePointRelais.CB_File,
                                                            Value = relais,
                                                        };
                                                        #region conversion cb_type
                                                        switch ((Model.Sage.cbSysLibreRepository.CB_Type)InfolibrePointRelais.CB_Type)
                                                        {
                                                            //case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageSmallDate:
                                                            //    info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageSmallDate;
                                                            //    break;
                                                            //case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageValeur:
                                                            //    info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageValeur;
                                                            //    break;
                                                            case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageText:
                                                                info_abstraction_pointrelais.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageText;
                                                                break;
                                                            //case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageDate:
                                                            //    info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageDate;
                                                            //    break;
                                                            //case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageMontant:
                                                            //    info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageMontant;
                                                            //    break;
                                                            case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageTable:
                                                                info_abstraction_pointrelais.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageTable;
                                                                break;
                                                            case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.Deleted:
                                                            default:
                                                                break;
                                                        }
                                                        #endregion
                                                        ObjF_DOCENTETE.InfosLibres.Add(info_abstraction_pointrelais);
                                                    }
                                                }
                                            }
                                        }

                                        if (Core.Global.GetConfig().ModuleSoColissimoInfolibreDestinataireActive
                                            && !string.IsNullOrWhiteSpace(Core.Global.GetConfig().ModuleSoColissimoInfolibreEnteteDestinataireName)
                                            && cbSysLibreRepository.ExistInformationLibre(Core.Global.GetConfig().ModuleSoColissimoInfolibreEnteteDestinataireName, Model.Sage.cbSysLibreRepository.CB_File.F_DOCENTETE))
                                        {
                                            if (PsSoDelivery == null)
                                            {
                                                // si PsSoDelivery n'a pas déjà été identifié
                                                Model.Prestashop.PsSoDeliveryRepository PsSoDeliveryRepository = new Model.Prestashop.PsSoDeliveryRepository();
                                                if (PsSoDeliveryRepository.ExistCart(PsOrders.IDCart))
                                                {
                                                    PsSoDelivery = PsSoDeliveryRepository.ReadCart(PsOrders.IDCart);
                                                }
                                            }
                                            if (PsSoDelivery != null)
                                            {
                                                string temp = PsSoDelivery.LastName + " " + PsSoDelivery.FirstName;

                                                Model.Sage.cbSysLibre InfolibreDestinataire = cbSysLibreRepository.ReadInformationLibre(Core.Global.GetConfig().ModuleSoColissimoInfolibreEnteteDestinataireName, Model.Sage.cbSysLibreRepository.CB_File.F_DOCENTETE);

                                                if (InfolibreDestinataire.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageText
                                                    || InfolibreDestinataire.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageTable)
                                                {
                                                    if (temp.Length >= InfolibreDestinataire.CB_Len)
                                                        temp = temp.Substring(0, InfolibreDestinataire.CB_Len - 1);

                                                    if (ObjF_DOCENTETE.InfosLibres == null)
                                                        ObjF_DOCENTETE.InfosLibres = new ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Col();
                                                    ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj info_abstraction_destinataire = new ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj()
                                                    {
                                                        Len = InfolibreDestinataire.CB_Len,
                                                        Name = InfolibreDestinataire.CB_Name,
                                                        Pos = InfolibreDestinataire.CB_Pos,
                                                        Table = InfolibreDestinataire.CB_File,
                                                        Value = temp,
                                                    };
                                                    #region conversion cb_type
                                                    switch ((Model.Sage.cbSysLibreRepository.CB_Type)InfolibreDestinataire.CB_Type)
                                                    {
                                                        //case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageSmallDate:
                                                        //    info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageSmallDate;
                                                        //    break;
                                                        //case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageValeur:
                                                        //    info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageValeur;
                                                        //    break;
                                                        case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageText:
                                                            info_abstraction_destinataire.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageText;
                                                            break;
                                                        //case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageDate:
                                                        //    info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageDate;
                                                        //    break;
                                                        //case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageMontant:
                                                        //    info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageMontant;
                                                        //    break;
                                                        case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageTable:
                                                            info_abstraction_destinataire.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageTable;
                                                            break;
                                                        case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.Deleted:
                                                        default:
                                                            break;
                                                    }
                                                    #endregion
                                                    ObjF_DOCENTETE.InfosLibres.Add(info_abstraction_destinataire);
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    ObjF_DOCENTETE.Add(Connexion);

                                    this.ExecLigneDistantToLocal(PsOrders, Connexion, ObjF_DOCENTETE, F_COMPTET);

                                    bool exist_payment = false;
                                    if (Core.Global.GetConfig().SyncReglementActif)
                                    {
                                        this.ExecReglement(PsOrders, Connexion, ObjF_DOCENTETE, out exist_payment);
                                    }
                                    if (!exist_payment && Core.Global.GetConfig().ModeReglementEcheancierActif)
                                        this.ExecEcheancier(ObjF_DOCENTETE, PsOrders, Connexion);

                                    Model.Sage.F_DOCENTETERepository F_DOCENTETERepository = new Model.Sage.F_DOCENTETERepository();
                                    if (F_DOCENTETERepository.ExistDomaineTypePiece((short)ObjF_DOCENTETE.DO_Domaine, (short)ObjF_DOCENTETE.DO_Type, ObjF_DOCENTETE.DO_Piece))
                                    {
                                        Model.Sage.F_DOCENTETE F_DOCENTETE = F_DOCENTETERepository.ReadDomaineTypePiece((short)ObjF_DOCENTETE.DO_Domaine, (short)ObjF_DOCENTETE.DO_Type, ObjF_DOCENTETE.DO_Piece);
                                        Model.Local.Order Order = new Model.Local.Order();
                                        Boolean isOrder = false;
                                        if (OrderRepository.ExistSage(F_DOCENTETE.cbMarq))
                                        {
                                            Order = OrderRepository.ReadSage(F_DOCENTETE.cbMarq);
                                            isOrder = true;
                                        }
                                        Order.Pre_Id = Convert.ToInt32(PsOrders.IDOrder);
                                        if (isOrder == true)
                                        {
                                            OrderRepository.Save();
                                        }
                                        else
                                        {
                                            Order.Sag_Id = F_DOCENTETE.cbMarq;
                                            OrderRepository.Add(Order);
                                        }

                                        #region controle montant
                                        // <JG> 28/05/2013 ajout contrôle de montant inter-base
                                        decimal total_sage = 0;
                                        if (F_DOCENTETE.DO_ValFrais != null)
                                        {
                                            if (F_DOCENTETE.DO_TypeLigneFrais == (short)ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_TypeLigne.TTC)
                                                total_sage += (decimal)F_DOCENTETE.DO_ValFrais;
                                            else
                                                total_sage += (decimal)F_DOCENTETE.DO_ValFrais * (1 + (F_DOCENTETE.DO_Taxe1.Value / 100));
                                        }
                                        Model.Sage.F_DOCLIGNERepository F_DOCLIGNERepository = new Model.Sage.F_DOCLIGNERepository();
                                        if (F_DOCLIGNERepository.ExistDomaineTypePieceValorise((short)ObjF_DOCENTETE.DO_Domaine, (short)ObjF_DOCENTETE.DO_Type, ObjF_DOCENTETE.DO_Piece))
                                            total_sage += (decimal)F_DOCLIGNERepository.ListDomaineTypePieceValorise((short)ObjF_DOCENTETE.DO_Domaine, (short)ObjF_DOCENTETE.DO_Type, ObjF_DOCENTETE.DO_Piece).Where(l => l.DL_Valorise == 1).Sum(l => l.DL_MontantTTC);

                                        if (PsOrders.TotalPaidTaxInCl != total_sage)
                                        {
                                            SendMailErrorCommande(PsOrders, F_DOCENTETE, total_sage);
                                        }
                                        #endregion
                                    }
                                }
                                // <JG> 31/05/2012 démappage de l'adresse de livraison prestashop avec l'adresse Sage car Compte Tiers non correspondant
                                else
                                {
                                    AddressRepository.Delete(Address);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
			return ObjF_DOCENTETE;
		}

        public void ExecReglement(Model.Prestashop.PsOrders PsOrders, ABSTRACTION_SAGE.ALTERNETIS.Connexion Connexion,
            ABSTRACTION_SAGE.F_DOCENTETE.Obj ObjF_DOCENTETE, out bool exist_payment)
        {
            exist_payment = false;
            try
            {
                Model.Prestashop.PsOrderPaymentRepository PsOrderPaymentRepository = new Model.Prestashop.PsOrderPaymentRepository();
                Model.Local.OrderPaymentRepository OrderPaymentRepository = new Model.Local.OrderPaymentRepository();
                if (PsOrderPaymentRepository.ExistOrderReference(PsOrders.Reference))
                {
                    Model.Local.SettlementRepository SettlementRepository = new Model.Local.SettlementRepository();
                    foreach (Model.Prestashop.PsOrderPayment PsOrderPayment in PsOrderPaymentRepository.ReadOrderReference(PsOrders.Reference))
                    {
                        if ((SettlementRepository.ExistPayment(PsOrderPayment.PaymentMethod) || SettlementRepository.ExistPaymentPartiel(PsOrderPayment.PaymentMethod))
                            && PsOrderPayment.Amount != 0
                            && !OrderPaymentRepository.ExistPayment(PsOrderPayment.IDOrderPayment))
                        {
                            try
                            {
                                if (ObjF_DOCENTETE.DO_Type == ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Type.Bon_Commande_Vente
                                    || ObjF_DOCENTETE.DO_Type == ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Type.Preparation_Livraison_Vente
                                    || ObjF_DOCENTETE.DO_Type == ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Type.Bon_Livraison_Vente
                                    || ObjF_DOCENTETE.DO_Type == ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Type.Facture_Vente)
                                {

                                    Model.Local.Settlement Settlement = (SettlementRepository.ExistPayment(PsOrderPayment.PaymentMethod))
                                        ? SettlementRepository.ReadPayment(PsOrderPayment.PaymentMethod)
                                        : SettlementRepository.ReadPaymentPartiel(PsOrderPayment.PaymentMethod);

                                    ABSTRACTION_SAGE.F_DOCREGL.Obj ObjF_DOCREGL = new ABSTRACTION_SAGE.F_DOCREGL.Obj();
                                    ObjF_DOCREGL.DO_Domaine = (ABSTRACTION_SAGE.F_DOCREGL.Obj._Enum_DO_Domaine)ObjF_DOCENTETE.DO_Domaine;
                                    ObjF_DOCREGL.DO_Type = (ABSTRACTION_SAGE.F_DOCREGL.Obj._Enum_DO_Type)ObjF_DOCENTETE.DO_Type;
                                    ObjF_DOCREGL.DO_Piece = ObjF_DOCENTETE.DO_Piece;
                                    //ObjF_DOCREGL.DR_TypeRegl = ((ObjF_DOCENTETE.DO_Type != ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Type.Facture_Vente
                                    //                    && ObjF_DOCENTETE.DO_Type != ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Type.Facture_Comptabilisee_Vente)
                                    //                ? ABSTRACTION_SAGE.F_DOCREGL.Obj._Enum_DR_TypeRegl.Acompte
                                    //                : ABSTRACTION_SAGE.F_DOCREGL.Obj._Enum_DR_TypeRegl.Reglement);
                                    // OK BLOCAGE EN V8
                                    ObjF_DOCREGL.DR_TypeRegl = ABSTRACTION_SAGE.F_DOCREGL.Obj._Enum_DR_TypeRegl.Acompte;
                                    ObjF_DOCREGL.DR_Montant = PsOrderPayment.Amount;
                                    ObjF_DOCREGL.DR_Libelle = (PsOrderPayment.PaymentMethod.Length > 35) ? PsOrderPayment.PaymentMethod.Substring(0, 35) : PsOrderPayment.PaymentMethod;
                                    ObjF_DOCREGL.N_Reglement = Settlement.Sag_Id;
                                    ObjF_DOCREGL.DR_Date = PsOrderPayment.DateAdd;
                                    ObjF_DOCREGL.Add(Connexion);

                                    Model.Local.OrderPayment OrderPayment = new Model.Local.OrderPayment()
                                    {
                                        Pre_Id_Order = PsOrders.IDOrder,
                                        Pre_Id_Order_Payment = PsOrderPayment.IDOrderPayment
                                    };
                                    OrderPaymentRepository.Add(OrderPayment);
                                    exist_payment = true;
                                    try
                                    {
                                        Model.Sage.F_DOCREGLRepository F_DOCREGLRepository = new Model.Sage.F_DOCREGLRepository();
                                        Model.Sage.F_DOCREGL F_DOCREGL = F_DOCREGLRepository.ReadDomaineTypePiece(
                                            ObjF_DOCREGL.DO_Domaine, ObjF_DOCREGL.DO_Type, ObjF_DOCREGL.DO_Piece, ObjF_DOCREGL.DR_Date.Date,
                                            ObjF_DOCREGL.DR_Libelle, (decimal)ObjF_DOCREGL.DR_Montant, ObjF_DOCREGL.DR_TypeRegl);

                                        Model.Sage.F_REGLECHRepository F_REGLECHRepository = new Model.Sage.F_REGLECHRepository();
                                        Model.Sage.F_REGLECH F_REGLECH = F_REGLECHRepository.Read(F_DOCREGL.DR_No);
                                        Model.Sage.F_CREGLEMENTRepository F_CREGLEMENTRepository = new Model.Sage.F_CREGLEMENTRepository();
                                        Model.Sage.F_CREGLEMENT F_CREGLEMENT = F_CREGLEMENTRepository.Read(F_REGLECH.RG_No == null ? 0 : F_REGLECH.RG_No);
                                        if (F_CREGLEMENT.JO_Num != Settlement.Set_Journal)
                                        {
                                            string request_odbc = "UPDATE F_CREGLEMENT SET JO_Num = '" + Core.Global.EscapeSqlString(Settlement.Set_Journal) + "' "
                                                + " WHERE RG_No = " + F_REGLECH.RG_No + " ";
                                            Connexion.Request = request_odbc;
                                            Connexion.Exec_Request();
                                        }
                                        try
                                        {
                                            F_CREGLEMENTRepository = new Model.Sage.F_CREGLEMENTRepository();
                                            F_CREGLEMENT = F_CREGLEMENTRepository.Read(F_REGLECH.RG_No == null ? 0 : F_REGLECH.RG_No);
                                            if (string.IsNullOrEmpty(F_CREGLEMENT.CT_NumPayeurOrig))
                                            {
                                                F_CREGLEMENT.CT_NumPayeurOrig = ObjF_DOCENTETE.CT_NumPayeur;
                                                F_CREGLEMENTRepository.SaveSQL();
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Core.Error.SendMailError("[SYNC COMMANDE] Erreur insertion numéro payeur règlement <br />" + ex.ToString());
                                        }
                                        try
                                        {
                                            F_CREGLEMENTRepository = new Model.Sage.F_CREGLEMENTRepository();
                                            F_CREGLEMENT = F_CREGLEMENTRepository.Read(F_REGLECH.RG_No == null ? 0 : F_REGLECH.RG_No);
                                            string rg_ref = Core.Global.EscapeSqlString(PsOrders.IDOrder.ToString());
                                            if (rg_ref.Length > 8)
                                                rg_ref = rg_ref.Substring(0, 8);
                                            if (String.IsNullOrWhiteSpace(F_CREGLEMENT.RG_Reference))// || F_CREGLEMENT.RG_Reference != rg_ref)
                                            {
                                                F_CREGLEMENT.RG_Reference = rg_ref;
                                                F_CREGLEMENTRepository.SaveSQL();
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Core.Error.SendMailError("[SYNC COMMANDE] Erreur insertion reference commande règlement <br />" + ex.ToString());
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        this.SendMailErrorReglement(PsOrders, ObjF_DOCENTETE, PsOrderPayment, Settlement.Set_Journal);
                                        Core.Error.SendMailError("[SYNC COMMANDE] Erreur modification code journal règlement <br />" + ex.ToString());
                                    }
                                    try
                                    {
                                        if (Core.Global.GetConfig().CommentaireReferencePaiementActif
                                            && DL_LIGNE_CommentaireReferencePaiement != 0)
                                        {
                                            if (!string.IsNullOrEmpty(PsOrderPayment.TransactionID))
                                                InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, Core.Global.GetConfig().CommentaireReferencePaiementTexte + PsOrderPayment.TransactionID, this.DL_LIGNE_CommentaireReferencePaiement, null);
                                            this.DL_LIGNE_CommentaireReferencePaiement = 0;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Core.Error.SendMailError("[SYNC COMMANDE] Erreur saisie commentaire référement paiement <br />" + ex.ToString());
                                    }
                                }
                                else
                                {
                                    Core.Log.WriteLog("[SYNC COMMANDE] Règlement non transférable sur un document de type : " + ObjF_DOCENTETE.DO_Type.ToString(), true);
                                }
                            }
                            catch (Exception ex)
                            {
                                this.SendMailErrorReglement(PsOrders, ObjF_DOCENTETE, PsOrderPayment);
                                Core.Error.SendMailError("[SYNC COMMANDE] Erreur transfert règlement <br />" + ex.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        public void ExecEcheancier(ABSTRACTION_SAGE.F_DOCENTETE.Obj ObjF_DOCENTETE, Model.Prestashop.PsOrders PsOrders, ABSTRACTION_SAGE.ALTERNETIS.Connexion Connexion)
        {
            try
            {
                if (Core.Global.GetConfig().ModeReglementEcheancierActif)
                {
                    Model.Local.SettlementRepository SettlementRepository = new Model.Local.SettlementRepository();
                    if (SettlementRepository.ExistPayment(PsOrders.Payment))
                    {
                        Model.Local.Settlement Settlement = SettlementRepository.ReadPayment(PsOrders.Payment);
                        Model.Sage.F_DOCREGLRepository F_DOCREGLRepository = new Model.Sage.F_DOCREGLRepository();
                        Model.Sage.F_DOCREGL F_DOCREGL = F_DOCREGLRepository.ReadDomaineTypePiece(
                            (short)ObjF_DOCENTETE.DO_Domaine, (short)ObjF_DOCENTETE.DO_Type, ObjF_DOCENTETE.DO_Piece);

                        if (F_DOCREGL != null)
                        {
                            string request_odbc = "UPDATE F_DOCREGL SET N_Reglement = " + (Settlement.Sag_Id) + " "
                                + " WHERE DR_No = " + F_DOCREGL.DR_No + " ";
                            Connexion.Request = request_odbc;
                            Connexion.Exec_Request();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void ExecLigneDistantToLocal(Model.Prestashop.PsOrders PsOrders, ABSTRACTION_SAGE.ALTERNETIS.Connexion Connexion,
         ABSTRACTION_SAGE.F_DOCENTETE.Obj ObjF_DOCENTETE, Model.Sage.F_COMPTET F_COMPTET)
        {
            try
            {
                Int32 DL_LIGNE = 10000;
                Model.Prestashop.PsOrderDetailRepository PsOrderDetailRepository = new Model.Prestashop.PsOrderDetailRepository();
                List<Model.Prestashop.PsOrderDetail> ListPsOrderDetail = PsOrderDetailRepository.ListOrder(PsOrders.IDOrder);

                // <JG> 29/10/2012 gestion valorisation selon HT / TTC
                Boolean ValorisationTTC = true;
                foreach (Model.Sage.P_CATTARIF P_Cattarif in new Model.Sage.P_CATTARIFRepository().ListIntituleNotNull())
                    if (P_Cattarif.cbMarq.ToString() == F_COMPTET.N_CatTarif.Value.ToString())
                    {
                        ValorisationTTC = (P_Cattarif.CT_PrixTTC != null && P_Cattarif.CT_PrixTTC == 1);
                        break;
                    }

                if (ObjF_DOCENTETE.DO_Devise > 0)
                    ValorisationTTC = false;

                ABSTRACTION_SAGE.F_DOCLIGNE.Obj ObjF_DOCLIGNE;


                if (Core.Global.GetConfig().CommentaireDebutDocument)
                {
                    #region Commentaires
                    if (Core.Global.GetConfig().CommentaireLibre1Actif && !string.IsNullOrWhiteSpace(Core.Global.GetConfig().CommentaireLibre1Texte))
                    {
                        InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, Core.Global.GetConfig().CommentaireLibre1Texte, DL_LIGNE, null);
                        DL_LIGNE += 10000;
                    }
                    if (Core.Global.GetConfig().CommentaireLibre2Actif && !string.IsNullOrWhiteSpace(Core.Global.GetConfig().CommentaireLibre2Texte))
                    {
                        InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, Core.Global.GetConfig().CommentaireLibre2Texte, DL_LIGNE, null);
                        DL_LIGNE += 10000;
                    }
                    if (Core.Global.GetConfig().CommentaireLibre3Actif && !string.IsNullOrWhiteSpace(Core.Global.GetConfig().CommentaireLibre3Texte))
                    {
                        InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, Core.Global.GetConfig().CommentaireLibre3Texte, DL_LIGNE, null);
                        DL_LIGNE += 10000;
                    }

                    if (Core.Global.GetConfig().CommentaireBoutiqueActif && !string.IsNullOrWhiteSpace(Core.Global.Selected_Shop))
                    {
                        InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, Core.Global.GetConfig().CommentaireBoutiqueTexte + Core.Global.Selected_Shop, DL_LIGNE, null);
                        DL_LIGNE += 10000;
                    }
                    if (Core.Global.GetConfig().CommentaireNumeroActif)
                    {
                        InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, Core.Global.GetConfig().CommentaireNumeroTexte + PsOrders.IDOrder.ToString(), DL_LIGNE, null);
                        DL_LIGNE += 10000;
                    }
                    if (Core.Global.GetConfig().CommentaireReferencePaiementActif)
                    {
                        this.DL_LIGNE_CommentaireReferencePaiement = DL_LIGNE;
                        DL_LIGNE += 10000;
                    }
                    if (Core.Global.GetConfig().CommentaireDateActif)
                    {
                        InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, Core.Global.GetConfig().CommentaireDateTexte + PsOrders.DateAdd.ToShortDateString(), DL_LIGNE, null);
                        DL_LIGNE += 10000;
                    }
                    #endregion

                    #region Messages Commande Prestashop

                    try
                    {
                        if (Core.Global.GetConfig().CommentaireClientActif)
                        {
                            Model.Prestashop.PsMessageRepository PsMessageRepository = new Model.Prestashop.PsMessageRepository();
                            if (PsMessageRepository.ExistOrder(PsOrders.IDOrder))
                            {
                                foreach (Model.Prestashop.PsMessage PsMessage in PsMessageRepository.ListOrderPrivate(PsOrders.IDOrder, 0))
                                {
                                    if (!string.IsNullOrWhiteSpace(PsMessage.Message))
                                    {
                                        InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, Core.Global.GetConfig().CommentaireClientTexte, DL_LIGNE, PsMessage.Message);
                                        DL_LIGNE += 10000;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex) { Core.Error.SendMailError("[SYNC COMMANDE] Erreur insertion glossaire commentaire client" + ex.ToString()); }

                    #endregion

                    #region Messages Adresses Prestashop

                    uint id_delivery_address = 0;

                    try
                    {
                        if (Core.Global.GetConfig().CommentaireAdresseLivraisonActif)
                        {
                            Model.Prestashop.PsAddressRepository PsAddressRepository = new Model.Prestashop.PsAddressRepository();
                            if (PsAddressRepository.ExistAddress(PsOrders.IDAddressDelivery))
                            {
                                Model.Prestashop.PsAddress PsAddress = PsAddressRepository.ReadAddress(PsOrders.IDAddressDelivery);
                                if (!string.IsNullOrWhiteSpace(PsAddress.Other))
                                {
                                    InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, Core.Global.GetConfig().CommentaireAdresseLivraisonTexte, DL_LIGNE, PsAddress.Other);
                                    DL_LIGNE += 10000;
                                    id_delivery_address = PsOrders.IDAddressDelivery;
                                }
                            }
                        }
                    }
                    catch (Exception ex) { Core.Error.SendMailError("[SYNC COMMANDE] Erreur insertion glossaire commentaire adresse livraison" + ex.ToString()); }

                    try
                    {
                        if (Core.Global.GetConfig().CommentaireAdresseFacturationActif)
                        {
                            Model.Prestashop.PsAddressRepository PsAddressRepository = new Model.Prestashop.PsAddressRepository();
                            if (PsAddressRepository.ExistAddress(PsOrders.IDAddressInvoice))
                            {
                                Model.Prestashop.PsAddress PsAddress = PsAddressRepository.ReadAddress(PsOrders.IDAddressInvoice);
                                if (!string.IsNullOrWhiteSpace(PsAddress.Other) && (id_delivery_address == 0 || PsOrders.IDAddressInvoice != PsOrders.IDAddressDelivery))
                                {
                                    InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, Core.Global.GetConfig().CommentaireAdresseFacturationTexte, DL_LIGNE, PsAddress.Other);
                                    DL_LIGNE += 10000;
                                }
                            }
                        }
                    }
                    catch (Exception ex) { Core.Error.SendMailError("[SYNC COMMANDE] Erreur insertion glossaire commentaire adresse facturation" + ex.ToString()); }

                    #endregion
                }


                foreach (Model.Prestashop.PsOrderDetail PsOrderDetail in ListPsOrderDetail)
                {
                    ObjF_DOCLIGNE = new ABSTRACTION_SAGE.F_DOCLIGNE.Obj();

                    #region insertion ligne
                    try
                    {
                        Boolean insert = false;
                        Boolean LigneRemise = false;
                        Boolean isConditioning = false;
                        Decimal conditioning_units = 1;

                        Model.Prestashop.PsProductRepository PsProductRepository = new Model.Prestashop.PsProductRepository();
                        if (PsProductRepository.ExistId(PsOrderDetail.ProductID))
                        {
                            Model.Prestashop.PsProduct PsProduct = PsProductRepository.ReadId(PsOrderDetail.ProductID);

                            Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
                            if (ArticleRepository.ExistPre_Id(Convert.ToInt32(PsOrderDetail.ProductID)))
                            {
                                Model.Local.Article Article = ArticleRepository.ReadPre_Id(Convert.ToInt32(PsOrderDetail.ProductID));
                                Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
                                Model.Sage.F_ARTICLE F_ARTICLE = null;

                                if (Article.TypeArticle == Model.Local.Article.enum_TypeArticle.ArticleComposition)
                                {
                                    if (PsOrderDetail.ProductAttributeID != null && PsOrderDetail.ProductAttributeID != 0)
                                    {
                                        Model.Local.CompositionArticleRepository CompositionArticleRepository = new Model.Local.CompositionArticleRepository();
                                        if (CompositionArticleRepository.ExistPrestaShop((int)PsOrderDetail.ProductAttributeID))
                                        {
                                            Model.Local.CompositionArticle CompositionArticle = CompositionArticleRepository.ReadPrestaShop((int)PsOrderDetail.ProductAttributeID);
                                            Model.Sage.F_ARTICLE_Composition F_ARTICLE_Composition = F_ARTICLERepository.ReadComposition(CompositionArticle.ComArt_F_ARTICLE_SagId, CompositionArticle.ComArt_F_ARTENUMREF_SagId, CompositionArticle.ComArt_F_CONDITION_SagId);
                                            if (F_ARTICLE_Composition != null)
                                            {
                                                F_ARTICLE = F_ARTICLERepository.ReadArticle(CompositionArticle.ComArt_F_ARTICLE_SagId);
                                                if (CompositionArticle.ComArt_F_ARTENUMREF_SagId != null)
                                                {
                                                    Model.Sage.F_ARTENUMREF F_ARTENUMREF = CompositionArticle.EnumereF_ARTENUMREF;
                                                    if (F_ARTENUMREF != null && F_ARTENUMREF.cbMarq != 0)
                                                    {
                                                        ObjF_DOCLIGNE.AG_No1 = (F_ARTENUMREF.AG_No1 != null) ? F_ARTENUMREF.AG_No1.Value : 0;
                                                        ObjF_DOCLIGNE.AG_No2 = (F_ARTENUMREF.AG_No2 != null) ? F_ARTENUMREF.AG_No2.Value : 0;
                                                    }
                                                }
                                                else if (CompositionArticle.ComArt_F_CONDITION_SagId != null)
                                                {
                                                    Model.Sage.F_CONDITION F_CONDITION = CompositionArticle.EnumereF_CONDITION;
                                                    if (F_CONDITION != null && F_CONDITION.cbMarq != 0)
                                                    {
                                                        ObjF_DOCLIGNE.EU_Enumere = F_CONDITION.EC_Enumere;
                                                        conditioning_units = (decimal)F_CONDITION.EC_Quantite;
                                                        ObjF_DOCLIGNE.EU_Qte = PsOrderDetail.ProductQuantity;
                                                        isConditioning = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (F_ARTICLERepository.ExistArticle(Article.Sag_Id))
                                {
                                    F_ARTICLE = F_ARTICLERepository.ReadArticle(Article.Sag_Id);
                                    if (PsOrderDetail.ProductAttributeID != null && PsOrderDetail.ProductAttributeID != 0)
                                    {
                                        #region Gammes
                                        if (new Model.Local.AttributeArticleRepository().ExistPrestashop((int)PsOrderDetail.ProductAttributeID))
                                        {
                                            Model.Local.AttributeArticle AttributeArticle = new Model.Local.AttributeArticleRepository().ReadPrestashop((int)PsOrderDetail.ProductAttributeID);

                                            Model.Sage.F_ARTENUMREF F_ARTENUMREF = AttributeArticle.EnumereF_ARTENUMREF;
                                            if (F_ARTENUMREF != null && F_ARTENUMREF.cbMarq != 0)
                                            {
                                                ObjF_DOCLIGNE.AG_No1 = (F_ARTENUMREF.AG_No1 != null) ? F_ARTENUMREF.AG_No1.Value : 0;
                                                ObjF_DOCLIGNE.AG_No2 = (F_ARTENUMREF.AG_No2 != null) ? F_ARTENUMREF.AG_No2.Value : 0;
                                            }
                                        }
                                        #endregion
                                        #region Conditionnement
                                        if (new Model.Local.ConditioningArticleRepository().ExistPrestashop((int)PsOrderDetail.ProductAttributeID))
                                        {
                                            Model.Local.ConditioningArticle ConditioningArticle = new Model.Local.ConditioningArticleRepository().ReadPrestashop((int)PsOrderDetail.ProductAttributeID);

                                            Model.Sage.F_CONDITION F_CONDITION = ConditioningArticle.EnumereF_CONDITION;
                                            if (F_CONDITION != null && F_CONDITION.cbMarq != 0)
                                            {
                                                ObjF_DOCLIGNE.EU_Enumere = F_CONDITION.EC_Enumere;
                                                conditioning_units = (decimal)F_CONDITION.EC_Quantite;
                                                ObjF_DOCLIGNE.EU_Qte = PsOrderDetail.ProductQuantity;
                                                isConditioning = true;
                                            }
                                        }
                                        #endregion
                                    }
                                }

                                if (F_ARTICLE != null)
                                {
                                    ObjF_DOCLIGNE.CT_Num = ObjF_DOCENTETE.DO_Tiers;
                                    ObjF_DOCLIGNE.CO_No = ObjF_DOCENTETE.CO_No;

                                    ObjF_DOCLIGNE.DO_Domaine = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DO_Domaine)ObjF_DOCENTETE.DO_Domaine;
                                    ObjF_DOCLIGNE.DO_Type = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DO_Type)ObjF_DOCENTETE.DO_Type;
                                    ObjF_DOCLIGNE.DO_Piece = ObjF_DOCENTETE.DO_Piece;
                                    ObjF_DOCLIGNE.DO_Date = ObjF_DOCENTETE.DO_Date;
                                    ObjF_DOCLIGNE.DL_DateBC = ObjF_DOCLIGNE.DO_Date;
                                    ObjF_DOCLIGNE.DL_DateBL = ObjF_DOCLIGNE.DO_Date;

                                    #if (SAGE_VERSION_18 || SAGE_VERSION_19 || SAGE_VERSION_20 || SAGE_VERSION_21)
                                    ObjF_DOCLIGNE.DL_DatePL = ObjF_DOCLIGNE.DO_Date;
                                    ObjF_DOCLIGNE.DL_DateDE = ObjF_DOCLIGNE.DO_Date;
                                    #endif

                                    ObjF_DOCLIGNE.DO_DateLivr = ObjF_DOCENTETE.DO_DateLivr;
                                    if (F_ARTICLE.AR_SuiviStock == 0)
                                        ObjF_DOCLIGNE.DE_No = 0;
                                    else
                                        ObjF_DOCLIGNE.DE_No = ObjF_DOCENTETE.DE_No;

                                    ObjF_DOCLIGNE.DL_Ligne = DL_LIGNE;
                                    ObjF_DOCLIGNE.DL_Qte = (isConditioning) ? (PsOrderDetail.ProductQuantity * conditioning_units) : PsOrderDetail.ProductQuantity;

                                    #region Taxes

                                    Core.Sync.SynchronisationArticle SyncArticle = new SynchronisationArticle();
                                    // <JG> 21/04/2017 Correction lecture eco-taxe sur famille
                                    Model.Sage.F_TAXE F_TAXETVA = SyncArticle.ReadTaxe(F_ARTICLE, PsProduct, ObjF_DOCENTETE.N_CatCompta);
                                    if (F_TAXETVA.TA_Taux != null)
                                    {
                                        switch (Core.Global.GetConfig().TaxSageTVA)
                                        {
                                            case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe1:
                                                ObjF_DOCLIGNE.DL_Taxe1 = (decimal)F_TAXETVA.TA_Taux;
                                                ObjF_DOCLIGNE.DL_TypeTaux1 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux)F_TAXETVA.TA_TTaux;
                                                ObjF_DOCLIGNE.DL_TypeTaxe1 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_TAXETVA.TA_Type;
												#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                                ObjF_DOCLIGNE.DL_CodeTaxe1 = F_TAXETVA.TA_Code;
												#endif
                                                break;
                                            case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe2:
                                                ObjF_DOCLIGNE.DL_Taxe2 = (decimal)F_TAXETVA.TA_Taux;
                                                ObjF_DOCLIGNE.DL_TypeTaux2 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux)F_TAXETVA.TA_TTaux;
                                                ObjF_DOCLIGNE.DL_TypeTaxe2 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_TAXETVA.TA_Type;
												#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                                ObjF_DOCLIGNE.DL_CodeTaxe2 = F_TAXETVA.TA_Code;
												#endif
                                                break;
                                            case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe3:
                                                ObjF_DOCLIGNE.DL_Taxe3 = (decimal)F_TAXETVA.TA_Taux;
                                                ObjF_DOCLIGNE.DL_TypeTaux3 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux)F_TAXETVA.TA_TTaux;
                                                ObjF_DOCLIGNE.DL_TypeTaxe3 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_TAXETVA.TA_Type;
												#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                                ObjF_DOCLIGNE.DL_CodeTaxe3 = F_TAXETVA.TA_Code;
												#endif
                                                break;
                                            case PRESTACONNECT.Core.Parametres.TaxSage.Empty:
                                            default:
                                                break;
                                        }
                                    }

                                    Model.Sage.F_TAXE F_ECOTAXE = SyncArticle.ReadEcoTaxe(F_ARTICLE, PsProduct, F_TAXETVA, ObjF_DOCENTETE.N_CatCompta);
                                    if (F_ECOTAXE.TA_Taux != null)
                                    {
                                        switch (Core.Global.GetConfig().TaxSageEco)
                                        {
                                            case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe1:
                                                ObjF_DOCLIGNE.DL_Taxe1 = (decimal)F_ECOTAXE.TA_Taux;
                                                ObjF_DOCLIGNE.DL_TypeTaux1 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux)F_ECOTAXE.TA_TTaux;
                                                ObjF_DOCLIGNE.DL_TypeTaxe1 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_ECOTAXE.TA_Type;
												#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                                ObjF_DOCLIGNE.DL_CodeTaxe1 = F_ECOTAXE.TA_Code;
												#endif
                                                break;
                                            case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe2:
                                                ObjF_DOCLIGNE.DL_Taxe2 = (decimal)F_ECOTAXE.TA_Taux;
                                                ObjF_DOCLIGNE.DL_TypeTaux2 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux)F_ECOTAXE.TA_TTaux;
                                                ObjF_DOCLIGNE.DL_TypeTaxe2 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_ECOTAXE.TA_Type;
												#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                                ObjF_DOCLIGNE.DL_CodeTaxe2 = F_ECOTAXE.TA_Code;
												#endif
                                                break;
                                            case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe3:
                                                ObjF_DOCLIGNE.DL_Taxe3 = (decimal)F_ECOTAXE.TA_Taux;
                                                ObjF_DOCLIGNE.DL_TypeTaux3 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux)F_ECOTAXE.TA_TTaux;
                                                ObjF_DOCLIGNE.DL_TypeTaxe3 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_ECOTAXE.TA_Type;
												#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                                ObjF_DOCLIGNE.DL_CodeTaxe3 = F_ECOTAXE.TA_Code;
												#endif
                                                break;
                                            case PRESTACONNECT.Core.Parametres.TaxSage.Empty:
                                            default:
                                                break;
                                        }
                                    }

                                    #endregion

                                    if (new Model.Local.TaxRepository().ExistSage(F_TAXETVA.cbMarq))
                                    {
                                        Model.Local.Tax Tax = new Model.Local.TaxRepository().ReadSage(F_TAXETVA.cbMarq);
                                        LigneRemise = Core.Global.GetConfig().LigneRemiseMode == Parametres.LigneRemiseMode.LigneRemise
                                                        && !String.IsNullOrEmpty(Tax.Sag_ArticleRemise)
                                                        && new Model.Sage.F_ARTICLERepository().ExistReference(Tax.Sag_ArticleRemise)
                                                        && (PsOrderDetail.ReductionAmount != 0
                                                            || PsOrderDetail.ReductionPercent != 0);
                                    }

                                    if (!LigneRemise)
                                    {
                                        if (Core.Global.GetConfig().LigneRemiseMode == Parametres.LigneRemiseMode.PrixBrutEtRemise
                                            && PsOrderDetail.ReductionPercent != 0)
                                        {
                                            Model.Prestashop.PsOrderDetailTaxRepository PsOrderDetailTaxRepository = new Model.Prestashop.PsOrderDetailTaxRepository();
                                            Model.Prestashop.PsTaxRepository PsTaxRepository = new Model.Prestashop.PsTaxRepository();
                                            decimal taxamount = 0;
                                            if (PsOrderDetailTaxRepository.ExistOrderDetail(PsOrderDetail.IDOrderDetail))
                                                foreach (Model.Prestashop.PsOrderDetailTax PsOrderDetailTax in PsOrderDetailTaxRepository.ListOrderDetail(PsOrderDetail.IDOrderDetail))
                                                    if (PsTaxRepository.ExistTaxe((uint)PsOrderDetailTax.IDTax))
                                                        taxamount += (PsOrderDetail.ProductPrice * PsTaxRepository.ReadTax((uint)PsOrderDetailTax.IDTax).Rate / 100);

                                            ObjF_DOCLIGNE.DL_PUTTC = (PsOrderDetail.ProductPrice + taxamount) / conditioning_units;
                                            ObjF_DOCLIGNE.DL_PrixUnitaire = PsOrderDetail.ProductPrice / conditioning_units;

                                            ObjF_DOCLIGNE.DL_Remise01REM_Type = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_RemiseREM_Type.Pourcentage;
                                            ObjF_DOCLIGNE.DL_Remise01REM_Valeur = PsOrderDetail.ReductionPercent;

                                        }
                                        else //if (Core.Global.GetConfig().LigneRemiseMode == Parametres.LigneRemiseMode.PrixNets)
                                        {
                                            ObjF_DOCLIGNE.DL_PUTTC = (PsOrderDetail.UnitPriceTaxInCl / conditioning_units);
                                            ObjF_DOCLIGNE.DL_PrixUnitaire = PsOrderDetail.UnitPriceTaxExCl / conditioning_units;
                                        }
                                        ObjF_DOCLIGNE.DL_TTC = (ValorisationTTC) ? ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TTC.TTC : ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TTC.HT;

                                        if (!ValorisationTTC && PsOrderDetail.EcOtAx != 0)
                                            ObjF_DOCLIGNE.DL_PrixUnitaire -= PsOrderDetail.EcOtAx;

                                        if (ObjF_DOCENTETE.DO_Devise > 0)
                                            ObjF_DOCLIGNE.DL_PUDevise = ObjF_DOCLIGNE.DL_PrixUnitaire;
                                    }
                                    else
                                    {
                                        Model.Prestashop.PsOrderDetailTaxRepository PsOrderDetailTaxRepository = new Model.Prestashop.PsOrderDetailTaxRepository();
                                        Model.Prestashop.PsTaxRepository PsTaxRepository = new Model.Prestashop.PsTaxRepository();
                                        decimal taxamount = 0;
                                        if (PsOrderDetailTaxRepository.ExistOrderDetail(PsOrderDetail.IDOrderDetail))
                                            foreach (Model.Prestashop.PsOrderDetailTax PsOrderDetailTax in PsOrderDetailTaxRepository.ListOrderDetail(PsOrderDetail.IDOrderDetail))
                                                if (PsTaxRepository.ExistTaxe((uint)PsOrderDetailTax.IDTax))
                                                    taxamount += (PsOrderDetail.ProductPrice * PsTaxRepository.ReadTax((uint)PsOrderDetailTax.IDTax).Rate / 100);

                                        ObjF_DOCLIGNE.DL_PUTTC = (PsOrderDetail.ProductPrice + taxamount) / conditioning_units;
                                        ObjF_DOCLIGNE.DL_PrixUnitaire = PsOrderDetail.ProductPrice / conditioning_units;
                                        ObjF_DOCLIGNE.DL_TTC = (ValorisationTTC) ? ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TTC.TTC : ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TTC.HT;

                                        if (!ValorisationTTC && PsOrderDetail.EcOtAx != 0)
                                            ObjF_DOCLIGNE.DL_PrixUnitaire -= PsOrderDetail.EcOtAx;

                                        if (ObjF_DOCENTETE.DO_Devise > 0)
                                            ObjF_DOCLIGNE.DL_PUDevise = ObjF_DOCLIGNE.DL_PrixUnitaire;
                                    }

                                    ObjF_DOCLIGNE.DL_MontantHT = 0;
                                    ObjF_DOCLIGNE.DL_MontantTTC = 0;
                                    ObjF_DOCLIGNE.AR_Ref = F_ARTICLE.AR_Ref;
                                    ObjF_DOCLIGNE.DL_Design = F_ARTICLE.AR_Design;
                                    switch (ObjF_DOCENTETE.DO_Langue)
                                    {
                                        case ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Langue.Langue_1:
                                            if (!string.IsNullOrWhiteSpace(F_ARTICLE.AR_Langue1))
                                                ObjF_DOCLIGNE.DL_Design = F_ARTICLE.AR_Langue1;
                                            break;
                                        case ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Langue.Langue_2:
                                            if (!string.IsNullOrWhiteSpace(F_ARTICLE.AR_Langue2))
                                                ObjF_DOCLIGNE.DL_Design = F_ARTICLE.AR_Langue2;
                                            break;
                                        case ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Langue.Aucune:
                                        default:
                                            // l'affectation par défaut est réalisée avant
                                            break;
                                    }
                                    ObjF_DOCLIGNE.DL_No = 0;
                                    ObjF_DOCLIGNE.CA_Num = ObjF_DOCENTETE.CA_Num;
                                    ObjF_DOCLIGNE.DL_Escompte = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_Escompte.Soumis;
                                    ObjF_DOCLIGNE.DL_Valorise = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_Valorise.Ligne_Valorisee;
                                    if (!isConditioning)
                                    {
                                        ObjF_DOCLIGNE.EU_Qte = ObjF_DOCLIGNE.DL_Qte;
                                        ObjF_DOCLIGNE.EU_Enumere = F_ARTICLE.UniteVenteString;
                                    }
                                    #if (SAGE_VERSION_1770)
                                    ObjF_DOCLIGNE.DL_QteBC = ObjF_DOCLIGNE.DL_Qte;
                                    #else
									//ObjF_DOCLIGNE.DL_QteBC = ObjF_DOCLIGNE.DL_Qte;
                                    //ObjF_DOCLIGNE.DL_QteDE = ObjF_DOCLIGNE.DL_Qte;
                                    #endif

                                    #region Gestion poids

                                    ObjF_DOCLIGNE.DL_PoidsNet = (F_ARTICLE.AR_PoidsNet != null) ? (decimal)F_ARTICLE.AR_PoidsNet * ObjF_DOCLIGNE.DL_Qte : 0;
                                    ObjF_DOCLIGNE.DL_PoidsBrut = (F_ARTICLE.AR_PoidsBrut != null) ? (decimal)F_ARTICLE.AR_PoidsBrut * ObjF_DOCLIGNE.DL_Qte : 0;

                                    if (F_ARTICLE.AR_UnitePoids != null)
                                    {
                                        switch (F_ARTICLE.AR_UnitePoids.Value)
                                        {
                                            case 0:
                                                ObjF_DOCLIGNE.DL_PoidsNet = ObjF_DOCLIGNE.DL_PoidsNet * 1000000;
                                                ObjF_DOCLIGNE.DL_PoidsBrut = ObjF_DOCLIGNE.DL_PoidsBrut * 1000000;
                                                break;
                                            case 1:
                                                ObjF_DOCLIGNE.DL_PoidsNet = ObjF_DOCLIGNE.DL_PoidsNet * 100000;
                                                ObjF_DOCLIGNE.DL_PoidsBrut = ObjF_DOCLIGNE.DL_PoidsBrut * 100000;
                                                break;
                                            case 2:
                                                ObjF_DOCLIGNE.DL_PoidsNet = ObjF_DOCLIGNE.DL_PoidsNet * 1000;
                                                ObjF_DOCLIGNE.DL_PoidsBrut = ObjF_DOCLIGNE.DL_PoidsBrut * 1000;
                                                break;
                                            case 3:
                                                ObjF_DOCLIGNE.DL_PoidsNet = ObjF_DOCLIGNE.DL_PoidsNet * 1;
                                                ObjF_DOCLIGNE.DL_PoidsBrut = ObjF_DOCLIGNE.DL_PoidsBrut * 1;
                                                break;
                                            case 4:
                                                ObjF_DOCLIGNE.DL_PoidsNet = ObjF_DOCLIGNE.DL_PoidsNet / 1000;
                                                ObjF_DOCLIGNE.DL_PoidsBrut = ObjF_DOCLIGNE.DL_PoidsBrut / 1000;
                                                break;
                                        }
                                    }

                                    #endregion

                                    //Hard Coded
                                    ObjF_DOCLIGNE.DL_FactPoids = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_FactPoids.Non_Facture;
                                    if (F_ARTICLE.AR_Nomencl == 2)
                                    {
                                        ObjF_DOCLIGNE.AR_RefCompose = F_ARTICLE.AR_Ref;
                                    }
                                    ObjF_DOCLIGNE.Add(Connexion);
                                    DL_LIGNE += 10000;
                                    insert = true;
                                    if (F_ARTICLE.AR_Nomencl == 2)
                                    {
                                        this.ExecLigneDistantToLocalPack(Connexion, ObjF_DOCENTETE, F_COMPTET, F_ARTICLE, ObjF_DOCLIGNE.AG_No1, ObjF_DOCLIGNE.AG_No2, DL_LIGNE, PsOrderDetail, ValorisationTTC, out DL_LIGNE);
                                        //DL_LIGNE += 10000;
                                    }
                                    if (LigneRemise)
                                    {
                                        this.ExecLigneRemise(Connexion, ObjF_DOCENTETE, DL_LIGNE, "Remise " + F_ARTICLE.AR_Ref + " " + ObjF_DOCLIGNE.DL_Design,
                                            F_TAXETVA, PsOrderDetail, ValorisationTTC, ObjF_DOCLIGNE, out DL_LIGNE);
                                        //DL_LIGNE += 10000;
                                    }
                                    if (!string.IsNullOrEmpty(Core.Global.GetConfig().ArticleInfolibrePackaging)
                                        && F_ARTICLERepository.ExistArticleInformationLibreText(Core.Global.GetConfig().ArticleInfolibrePackaging, F_ARTICLE.AR_Ref))
                                    {
                                        string RefArticleLie = F_ARTICLERepository.ReadArticleInformationLibreText(Core.Global.GetConfig().ArticleInfolibrePackaging, F_ARTICLE.AR_Ref);
                                        if (F_ARTICLERepository.ExistReference(RefArticleLie))
                                        {
                                            Model.Sage.F_ARTICLE F_ARTICLE_ArticleLie = F_ARTICLERepository.ReadReference(RefArticleLie);
                                            if (F_ARTICLE_ArticleLie.AR_Nomencl == (short)ABSTRACTION_SAGE.F_ARTICLE.Obj._Enum_AR_Nomencl.Article_Lie)
                                                ExecLigneDistantToLocalPack(Connexion, ObjF_DOCENTETE, F_COMPTET, F_ARTICLE_ArticleLie, 0, 0, DL_LIGNE, PsOrderDetail, ValorisationTTC, out DL_LIGNE);
                                        }
                                    }
                                }
                            }
                        }

                        if (!insert)
                        {
                            this.ExecLigneRemplacement(Connexion, ObjF_DOCENTETE, DL_LIGNE, PsOrderDetail, ValorisationTTC, out DL_LIGNE);
                        }
                        this.ExecOleaPromo(PsOrders, PsOrderDetail, Connexion, ObjF_DOCENTETE, DL_LIGNE, out DL_LIGNE);
                    }
                    catch (Exception ex)
                    {
                        if (!ex.Message.Contains("Record is lock"))
                        {
                            try
                            {
                                string ligne_info = "## Blocage insertion ligne produit : " + PsOrderDetail.ProductReference + " ##";
                                InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, ligne_info, DL_LIGNE, null);
                                DL_LIGNE += 10000;
                                InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, (PsOrderDetail.ProductName.Length <= 69 ? PsOrderDetail.ProductName : PsOrderDetail.ProductName.Substring(0, 69)), DL_LIGNE, null);
                                DL_LIGNE += 10000;
                                string ligne_message = "Message : " + ex.Message.Replace("ERROR [HY000] [Simba][SimbaEngine ODBC Driver][DRM File Library]", string.Empty);
                                if (ligne_message.Length > 69)
                                    ligne_message = ligne_message.Substring(0, 69);
                                InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, ligne_message, DL_LIGNE, ex.Message);
                                DL_LIGNE += 10000;
                            }
                            catch (Exception ex_comm)
                            {
                                Core.Error.SendMailError("[SYNC COMMNANDE] Erreur commentaire <br/>" + ex_comm.ToString());
                            }
                        }
                        else
                        {
                            // TODO AJOUTER UN RETRY DE L'INSERTION DE LA LIGNE ???

                            // TODO SEND MESSAGE TO ADMIN TO ALERT DOC SAGE OPENING IN EXECUTION
                            SendMailErrorCommandeRecordIsLock(ObjF_DOCENTETE);
                        }

                        Core.Error.SendMailError("[SYNC COMMNANDE] Erreur ligne <br/>" + ex.ToString());
                    }
                    #endregion
                }


                #region Article Packaging

                Model.Sage.F_ARTICLERepository F_ARTICLERepository_Packaging = new Model.Sage.F_ARTICLERepository();
                if (!string.IsNullOrEmpty(Core.Global.GetConfig().CommandeArticlePackaging)
                    && F_ARTICLERepository_Packaging.ExistReference(Core.Global.GetConfig().CommandeArticlePackaging))
                {
                    Model.Sage.F_ARTICLE F_ARTICLE_ArticleLie = F_ARTICLERepository_Packaging.ReadReference(Core.Global.GetConfig().CommandeArticlePackaging);
                    if (F_ARTICLE_ArticleLie.AR_Nomencl == (short)ABSTRACTION_SAGE.F_ARTICLE.Obj._Enum_AR_Nomencl.Article_Lie)
                        ExecLigneDistantToLocalPack(Connexion, ObjF_DOCENTETE, F_COMPTET, F_ARTICLE_ArticleLie, 0, 0, DL_LIGNE, null, ValorisationTTC, out DL_LIGNE);
                }

                #endregion


                #region Ligne Frais de port
                if (Core.Global.GetConfig().LigneFraisPort)
                    if (new Model.Sage.F_ARTICLERepository().ExistReference(Core.Global.GetConfig().LigneArticlePort))
                    {
                        Model.Sage.P_EXPEDITION P_EXPEDITION = new Model.Sage.P_EXPEDITIONRepository().Read(ObjF_DOCENTETE.DO_Expedit);

                        #region ligne article
                        Model.Sage.F_ARTICLE F_ARTICLE = new Model.Sage.F_ARTICLERepository().ReadReference(Core.Global.GetConfig().LigneArticlePort);
                        ABSTRACTION_SAGE.F_DOCLIGNE.Obj F_DOCLIGNE_PORT = new ABSTRACTION_SAGE.F_DOCLIGNE.Obj();
                        F_DOCLIGNE_PORT.CT_Num = ObjF_DOCENTETE.DO_Tiers;
                        F_DOCLIGNE_PORT.CO_No = ObjF_DOCENTETE.CO_No;

                        F_DOCLIGNE_PORT.DO_Domaine = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DO_Domaine)ObjF_DOCENTETE.DO_Domaine;
                        F_DOCLIGNE_PORT.DO_Type = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DO_Type)ObjF_DOCENTETE.DO_Type;
                        F_DOCLIGNE_PORT.DO_Piece = ObjF_DOCENTETE.DO_Piece;
                        F_DOCLIGNE_PORT.DO_Date = ObjF_DOCENTETE.DO_Date;
                        F_DOCLIGNE_PORT.DL_DateBC = F_DOCLIGNE_PORT.DO_Date;
                        F_DOCLIGNE_PORT.DL_DateBL = F_DOCLIGNE_PORT.DO_Date;
                    	#if (SAGE_VERSION_18 || SAGE_VERSION_19 || SAGE_VERSION_20 || SAGE_VERSION_21)
                        F_DOCLIGNE_PORT.DL_DatePL = F_DOCLIGNE_PORT.DO_Date;
                        F_DOCLIGNE_PORT.DL_DateDE = F_DOCLIGNE_PORT.DO_Date;
                        #endif

                        F_DOCLIGNE_PORT.DO_DateLivr = ObjF_DOCENTETE.DO_DateLivr;

                        if (F_ARTICLE.AR_SuiviStock == 0)
                            F_DOCLIGNE_PORT.DE_No = 0;
                        else
                            F_DOCLIGNE_PORT.DE_No = ObjF_DOCENTETE.DE_No;

                        F_DOCLIGNE_PORT.DL_Ligne = DL_LIGNE;
                        F_DOCLIGNE_PORT.DL_Qte = 1;

                        #region Taxes
                        Core.Sync.SynchronisationArticle SyncArticle = new SynchronisationArticle();
                        Model.Sage.F_TAXE F_TAXETVA = SyncArticle.ReadTaxe(F_ARTICLE, new Model.Prestashop.PsProduct(), ObjF_DOCENTETE.N_CatCompta);
                        if (F_TAXETVA.TA_Taux != null)
                        {
                            switch (Core.Global.GetConfig().TaxSageTVA)
                            {
                                case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe1:
                                    F_DOCLIGNE_PORT.DL_Taxe1 = PsOrders.CarrierTaxRate;
                                    F_DOCLIGNE_PORT.DL_TypeTaux1 = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux.Taux;
                                    F_DOCLIGNE_PORT.DL_TypeTaxe1 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_TAXETVA.TA_Type;
									#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                    F_DOCLIGNE_PORT.DL_CodeTaxe1 = F_TAXETVA.TA_Code;
									#endif
                                    break;
                                case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe2:
                                    F_DOCLIGNE_PORT.DL_Taxe2 = PsOrders.CarrierTaxRate;
                                    F_DOCLIGNE_PORT.DL_TypeTaux2 = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux.Taux;
                                    F_DOCLIGNE_PORT.DL_TypeTaxe2 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_TAXETVA.TA_Type;
									#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                    F_DOCLIGNE_PORT.DL_CodeTaxe2 = F_TAXETVA.TA_Code;
									#endif
                                    break;
                                case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe3:
                                    F_DOCLIGNE_PORT.DL_Taxe3 = PsOrders.CarrierTaxRate;
                                    F_DOCLIGNE_PORT.DL_TypeTaux3 = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux.Taux;
                                    F_DOCLIGNE_PORT.DL_TypeTaxe3 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_TAXETVA.TA_Type;
									#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                    F_DOCLIGNE_PORT.DL_CodeTaxe3 = F_TAXETVA.TA_Code;
									#endif
                                    break;
                                case PRESTACONNECT.Core.Parametres.TaxSage.Empty:
                                default:
                                    break;
                            }
                        }
                        #endregion

                        F_DOCLIGNE_PORT.DL_PUTTC = PsOrders.TotalShipping;
                        F_DOCLIGNE_PORT.DL_PrixUnitaire = (F_TAXETVA.TA_Taux != null) ? PsOrders.TotalShipping / (1 + (PsOrders.CarrierTaxRate / 100)) : PsOrders.TotalShipping;
                        F_DOCLIGNE_PORT.DL_TTC = (P_EXPEDITION.E_TypeLigneFrais.Value == (int)ABSTRACTION_SAGE.P_EXPEDITION.Obj._Enum_HT_TTC.TTC)
                                                    ? ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TTC.TTC
                                                    : ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TTC.HT;
                        if (ObjF_DOCENTETE.DO_Devise > 0)
                            F_DOCLIGNE_PORT.DL_PUDevise = F_DOCLIGNE_PORT.DL_PrixUnitaire;

                        F_DOCLIGNE_PORT.DL_MontantHT = 0;
                        F_DOCLIGNE_PORT.DL_MontantTTC = 0;
                        F_DOCLIGNE_PORT.AR_Ref = F_ARTICLE.AR_Ref;
                        string expe = F_ARTICLE.AR_Design + " " + P_EXPEDITION.E_Intitule;
                        F_DOCLIGNE_PORT.DL_Design = (expe.Length <= 69) ? expe : expe.Substring(0, 69);
                        F_DOCLIGNE_PORT.DL_No = 0;
                        F_DOCLIGNE_PORT.CA_Num = ObjF_DOCENTETE.CA_Num;
                        F_DOCLIGNE_PORT.DL_Escompte = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_Escompte.Soumis;
                        F_DOCLIGNE_PORT.DL_Valorise = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_Valorise.Ligne_Valorisee;
                        F_DOCLIGNE_PORT.EU_Qte = F_DOCLIGNE_PORT.DL_Qte;
                        F_DOCLIGNE_PORT.EU_Enumere = F_ARTICLE.UniteVenteString;
                        F_DOCLIGNE_PORT.DL_QteBC = F_DOCLIGNE_PORT.DL_Qte;

                        F_DOCLIGNE_PORT.DL_FactPoids = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_FactPoids.Non_Facture;
                        F_DOCLIGNE_PORT.Add(Connexion);
                        DL_LIGNE += 10000;
                        #endregion

                    }
                    else
                    {
                        InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, "Article de port inexistant : " + Core.Global.GetConfig().LigneArticlePort, DL_LIGNE, null);
                        DL_LIGNE += 10000;
                    }
                #endregion


                #region Bons de réductions

                ExecBonReduction(PsOrders, Connexion, ObjF_DOCENTETE, ValorisationTTC, DL_LIGNE, out DL_LIGNE);

                #endregion


                if (Core.Global.GetConfig().CommentaireFinDocument)
                {
                    #region Commentaires
                    if (Core.Global.GetConfig().CommentaireLibre1Actif && !string.IsNullOrWhiteSpace(Core.Global.GetConfig().CommentaireLibre1Texte))
                    {
                        InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, Core.Global.GetConfig().CommentaireLibre1Texte, DL_LIGNE, null);
                        DL_LIGNE += 10000;
                    }
                    if (Core.Global.GetConfig().CommentaireLibre2Actif && !string.IsNullOrWhiteSpace(Core.Global.GetConfig().CommentaireLibre2Texte))
                    {
                        InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, Core.Global.GetConfig().CommentaireLibre2Texte, DL_LIGNE, null);
                        DL_LIGNE += 10000;
                    }
                    if (Core.Global.GetConfig().CommentaireLibre3Actif && !string.IsNullOrWhiteSpace(Core.Global.GetConfig().CommentaireLibre3Texte))
                    {
                        InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, Core.Global.GetConfig().CommentaireLibre3Texte, DL_LIGNE, null);
                        DL_LIGNE += 10000;
                    }

                    if (Core.Global.GetConfig().CommentaireBoutiqueActif && !string.IsNullOrWhiteSpace(Core.Global.Selected_Shop))
                    {
                        InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, Core.Global.GetConfig().CommentaireBoutiqueTexte + Core.Global.Selected_Shop, DL_LIGNE, null);
                        DL_LIGNE += 10000;
                    }
                    if (Core.Global.GetConfig().CommentaireNumeroActif)
                    {
                        InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, Core.Global.GetConfig().CommentaireNumeroTexte + PsOrders.IDOrder.ToString(), DL_LIGNE, null);
                        DL_LIGNE += 10000;
                    }
                    if (Core.Global.GetConfig().CommentaireReferencePaiementActif)
                    {
                        this.DL_LIGNE_CommentaireReferencePaiement = DL_LIGNE;
                        DL_LIGNE += 10000;
                    }
                    if (Core.Global.GetConfig().CommentaireDateActif)
                    {
                        InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, Core.Global.GetConfig().CommentaireDateTexte + PsOrders.DateAdd.ToShortDateString(), DL_LIGNE, null);
                        DL_LIGNE += 10000;
                    }
                    #endregion

                    #region Messages Commande Prestashop

                    try
                    {
                        if (Core.Global.GetConfig().CommentaireClientActif)
                        {
                            Model.Prestashop.PsMessageRepository PsMessageRepository = new Model.Prestashop.PsMessageRepository();
                            if (PsMessageRepository.ExistOrder(PsOrders.IDOrder))
                            {
                                foreach (Model.Prestashop.PsMessage PsMessage in PsMessageRepository.ListOrderPrivate(PsOrders.IDOrder, 0))
                                {
                                    if (!string.IsNullOrWhiteSpace(PsMessage.Message))
                                    {
                                        InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, Core.Global.GetConfig().CommentaireClientTexte, DL_LIGNE, PsMessage.Message);
                                        DL_LIGNE += 10000;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex) { Core.Error.SendMailError("[SYNC COMMANDE] Erreur insertion glossaire commentaire client" + ex.ToString()); }

                    #endregion

                    #region Messages Adresses Prestashop

                    uint id_delivery_address = 0;

                    try
                    {
                        if (Core.Global.GetConfig().CommentaireAdresseLivraisonActif)
                        {
                            Model.Prestashop.PsAddressRepository PsAddressRepository = new Model.Prestashop.PsAddressRepository();
                            if (PsAddressRepository.ExistAddress(PsOrders.IDAddressDelivery))
                            {
                                Model.Prestashop.PsAddress PsAddress = PsAddressRepository.ReadAddress(PsOrders.IDAddressDelivery);
                                if (!string.IsNullOrWhiteSpace(PsAddress.Other))
                                {
                                    InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, Core.Global.GetConfig().CommentaireAdresseLivraisonTexte, DL_LIGNE, PsAddress.Other);
                                    DL_LIGNE += 10000;
                                    id_delivery_address = PsOrders.IDAddressDelivery;
                                }
                            }
                        }
                    }
                    catch (Exception ex) { Core.Error.SendMailError("[SYNC COMMANDE] Erreur insertion glossaire commentaire adresse livraison" + ex.ToString()); }

                    try
                    {
                        if (Core.Global.GetConfig().CommentaireAdresseFacturationActif)
                        {
                            Model.Prestashop.PsAddressRepository PsAddressRepository = new Model.Prestashop.PsAddressRepository();
                            if (PsAddressRepository.ExistAddress(PsOrders.IDAddressInvoice))
                            {
                                Model.Prestashop.PsAddress PsAddress = PsAddressRepository.ReadAddress(PsOrders.IDAddressInvoice);
                                if (!string.IsNullOrWhiteSpace(PsAddress.Other) && (id_delivery_address == 0 || PsOrders.IDAddressInvoice != PsOrders.IDAddressDelivery))
                                {
                                    InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, Core.Global.GetConfig().CommentaireAdresseFacturationTexte, DL_LIGNE, PsAddress.Other);
                                    DL_LIGNE += 10000;
                                }
                            }
                        }
                    }
                    catch (Exception ex) { Core.Error.SendMailError("[SYNC COMMANDE] Erreur insertion glossaire commentaire adresse facturation" + ex.ToString()); }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void ExecLigneRemise(ABSTRACTION_SAGE.ALTERNETIS.Connexion Connexion, ABSTRACTION_SAGE.F_DOCENTETE.Obj ObjF_DOCENTETE,
            Int32 DL_Ligne, String Article, Model.Sage.F_TAXE TaxeTVA, Model.Prestashop.PsOrderDetail PsOrderDetail, Boolean ValorisationTTC, ABSTRACTION_SAGE.F_DOCLIGNE.Obj Ligne, out Int32 DL_LIGNE_OUT)
        {
            try
            {
                Model.Local.TaxRepository LocalTaxRepository = new Model.Local.TaxRepository();
                Model.Local.Tax LocalTax = LocalTaxRepository.ReadSage(TaxeTVA.cbMarq);
                Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();

                Model.Sage.F_ARTICLE F_ARTICLE = F_ARTICLERepository.ReadReference(LocalTax.Sag_ArticleRemise);
                ABSTRACTION_SAGE.F_DOCLIGNE.Obj ObjF_DOCLIGNE = new ABSTRACTION_SAGE.F_DOCLIGNE.Obj();
                ObjF_DOCLIGNE.DO_Domaine = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DO_Domaine)ObjF_DOCENTETE.DO_Domaine;
                ObjF_DOCLIGNE.DO_Type = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DO_Type)ObjF_DOCENTETE.DO_Type;
                ObjF_DOCLIGNE.CT_Num = ObjF_DOCENTETE.DO_Tiers;
                ObjF_DOCLIGNE.DO_Piece = ObjF_DOCENTETE.DO_Piece;
                ObjF_DOCLIGNE.DO_Date = ObjF_DOCENTETE.DO_Date;
                ObjF_DOCLIGNE.DL_DateBC = ObjF_DOCLIGNE.DO_Date;
                ObjF_DOCLIGNE.DL_DateBL = ObjF_DOCLIGNE.DO_Date;
            	#if (SAGE_VERSION_18 || SAGE_VERSION_19 || SAGE_VERSION_20 || SAGE_VERSION_21)
                ObjF_DOCLIGNE.DL_DatePL = ObjF_DOCLIGNE.DO_Date;
                ObjF_DOCLIGNE.DL_DateDE = ObjF_DOCLIGNE.DO_Date;
                #endif
                ObjF_DOCLIGNE.DL_Ligne = DL_Ligne;
                ObjF_DOCLIGNE.CO_No = ObjF_DOCENTETE.CO_No;
                ObjF_DOCLIGNE.DO_DateLivr = ObjF_DOCENTETE.DO_DateLivr;


                if (F_ARTICLE.AR_SuiviStock == 0)
                    ObjF_DOCLIGNE.DE_No = 0;
                else
                    ObjF_DOCLIGNE.DE_No = ObjF_DOCENTETE.DE_No;

                ObjF_DOCLIGNE.DL_Qte = PsOrderDetail.ProductQuantity;

                if (ValorisationTTC)
                {
                    ObjF_DOCLIGNE.DL_PUTTC = Math.Round(0 - PsOrderDetail.ReductionAmountTaxInCl - ((Ligne.DL_PUTTC * PsOrderDetail.ReductionPercent) / 100), 2);
                    ObjF_DOCLIGNE.DL_TTC = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TTC.TTC;
                }
                else
                {
                    ObjF_DOCLIGNE.DL_PrixUnitaire = Math.Round(0 - PsOrderDetail.ReductionAmountTaxExCl - ((Ligne.DL_PrixUnitaire * PsOrderDetail.ReductionPercent) / 100), 2);
                    ObjF_DOCLIGNE.DL_TTC = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TTC.HT;
                    if (ObjF_DOCENTETE.DO_Devise > 0)
                        ObjF_DOCLIGNE.DL_PUDevise = ObjF_DOCLIGNE.DL_PrixUnitaire;
                }

                ObjF_DOCLIGNE.DL_MontantHT = 0;
                ObjF_DOCLIGNE.DL_MontantTTC = 0;
                ObjF_DOCLIGNE.AR_Ref = F_ARTICLE.AR_Ref;
                if (Article.Length > 69)
                {
                    Article = Article.Substring(0, 69);
                }
                ObjF_DOCLIGNE.DL_Design = Article;
                ObjF_DOCLIGNE.DL_No = 0;
                ObjF_DOCLIGNE.CA_Num = ObjF_DOCENTETE.CA_Num;
                ObjF_DOCLIGNE.DL_Escompte = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_Escompte.Soumis;
                ObjF_DOCLIGNE.DL_Valorise = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_Valorise.Ligne_Valorisee;
                ObjF_DOCLIGNE.EU_Qte = ObjF_DOCLIGNE.DL_Qte;
                ObjF_DOCLIGNE.EU_Enumere = F_ARTICLE.UniteVenteString;
                ObjF_DOCLIGNE.DL_QteBC = ObjF_DOCLIGNE.DL_Qte;

                #region Gestion poids
                ObjF_DOCLIGNE.DL_PoidsNet = (F_ARTICLE.AR_PoidsNet != null) ? (decimal)F_ARTICLE.AR_PoidsNet * ObjF_DOCLIGNE.DL_Qte : 0;
                ObjF_DOCLIGNE.DL_PoidsBrut = (F_ARTICLE.AR_PoidsBrut != null) ? (decimal)F_ARTICLE.AR_PoidsBrut * ObjF_DOCLIGNE.DL_Qte : 0;
                if (F_ARTICLE.AR_UnitePoids != null)
                {
                    switch (F_ARTICLE.AR_UnitePoids.Value)
                    {
                        case 0:
                            ObjF_DOCLIGNE.DL_PoidsNet = ObjF_DOCLIGNE.DL_PoidsNet * 1000000;
                            ObjF_DOCLIGNE.DL_PoidsBrut = ObjF_DOCLIGNE.DL_PoidsBrut * 1000000;
                            break;
                        case 1:
                            ObjF_DOCLIGNE.DL_PoidsNet = ObjF_DOCLIGNE.DL_PoidsNet * 100000;
                            ObjF_DOCLIGNE.DL_PoidsBrut = ObjF_DOCLIGNE.DL_PoidsBrut * 100000;
                            break;
                        case 2:
                            ObjF_DOCLIGNE.DL_PoidsNet = ObjF_DOCLIGNE.DL_PoidsNet * 1000;
                            ObjF_DOCLIGNE.DL_PoidsBrut = ObjF_DOCLIGNE.DL_PoidsBrut * 1000;
                            break;
                        case 3:
                            ObjF_DOCLIGNE.DL_PoidsNet = ObjF_DOCLIGNE.DL_PoidsNet * 1;
                            ObjF_DOCLIGNE.DL_PoidsBrut = ObjF_DOCLIGNE.DL_PoidsBrut * 1;
                            break;
                        case 4:
                            ObjF_DOCLIGNE.DL_PoidsNet = ObjF_DOCLIGNE.DL_PoidsNet / 1000;
                            ObjF_DOCLIGNE.DL_PoidsBrut = ObjF_DOCLIGNE.DL_PoidsBrut / 1000;
                            break;
                    }
                }
                #endregion

                #region Taxes

                if (TaxeTVA.TA_Taux != null)
                {
                    switch (Core.Global.GetConfig().TaxSageTVA)
                    {
                        case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe1:
                            ObjF_DOCLIGNE.DL_Taxe1 = (decimal)TaxeTVA.TA_Taux;
                            ObjF_DOCLIGNE.DL_TypeTaux1 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux)TaxeTVA.TA_TTaux;
                            ObjF_DOCLIGNE.DL_TypeTaxe1 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)TaxeTVA.TA_Type;
							#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                            ObjF_DOCLIGNE.DL_CodeTaxe1 = TaxeTVA.TA_Code;
							#endif
                            break;
                        case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe2:
                            ObjF_DOCLIGNE.DL_Taxe2 = (decimal)TaxeTVA.TA_Taux;
                            ObjF_DOCLIGNE.DL_TypeTaux2 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux)TaxeTVA.TA_TTaux;
                            ObjF_DOCLIGNE.DL_TypeTaxe2 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)TaxeTVA.TA_Type;
							#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                            ObjF_DOCLIGNE.DL_CodeTaxe2 = TaxeTVA.TA_Code;
							#endif
                            break;
                        case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe3:
                            ObjF_DOCLIGNE.DL_Taxe3 = (decimal)TaxeTVA.TA_Taux;
                            ObjF_DOCLIGNE.DL_TypeTaux3 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux)TaxeTVA.TA_TTaux;
                            ObjF_DOCLIGNE.DL_TypeTaxe3 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)TaxeTVA.TA_Type;
							#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                            ObjF_DOCLIGNE.DL_CodeTaxe3 = TaxeTVA.TA_Code;
							#endif
                            break;
                        case PRESTACONNECT.Core.Parametres.TaxSage.Empty:
                        default:
                            break;
                    }
                }

                #endregion

                //Hard Coded
                ObjF_DOCLIGNE.DL_FactPoids = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_FactPoids.Non_Facture;

                ObjF_DOCLIGNE.Add(Connexion);
                DL_Ligne += 10000;
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[SYNC COMMANDE] Erreur insertion remise sur l'article " + Article + "<br />" + ex.ToString());
                throw;
            }
            DL_LIGNE_OUT = DL_Ligne;
        }

        private void ExecLigneRemplacement(ABSTRACTION_SAGE.ALTERNETIS.Connexion Connexion, ABSTRACTION_SAGE.F_DOCENTETE.Obj ObjF_DOCENTETE,
            Int32 DL_Ligne, Model.Prestashop.PsOrderDetail PsOrderDetail, Boolean ValorisationTTC, out Int32 DL_LIGNE_OUT)
        {
            try
            {
                Model.Prestashop.PsOrderDetailTaxRepository PsOrderDetailTaxRepository = new Model.Prestashop.PsOrderDetailTaxRepository();
                if (PsOrderDetailTaxRepository.ExistOrderDetail(PsOrderDetail.IDOrderDetail))
                {
                    List<int> list_idtaxrulesgroup = new Model.Prestashop.PsTaxRuleRepository().ListTaxRulesGroup(PsOrderDetailTaxRepository.ListOrderDetail(PsOrderDetail.IDOrderDetail).First().IDTax);
                    Model.Local.TaxRepository TaxRepository = new Model.Local.TaxRepository();
                    int idtaxrulesgroup = 0;
                    string ar_ref = string.Empty;
                    Boolean LigneRemise = false;
                    foreach (int id in list_idtaxrulesgroup)
                        if (TaxRepository.ExistPrestashop(id))
                        {
                            Model.Local.Tax Tax = TaxRepository.ReadPrestashop(id);
                            if (!String.IsNullOrEmpty(Tax.Sag_ArticleRemplacement)
                                && new Model.Sage.F_ARTICLERepository().ExistReference(Tax.Sag_ArticleRemplacement))
                            {
                                ar_ref = Tax.Sag_ArticleRemplacement;
                                idtaxrulesgroup = id;

                                LigneRemise = Core.Global.GetConfig().LigneRemiseMode == Parametres.LigneRemiseMode.LigneRemise
                                            && !String.IsNullOrEmpty(Tax.Sag_ArticleRemise)
                                            && new Model.Sage.F_ARTICLERepository().ExistReference(Tax.Sag_ArticleRemise)
                                            && (PsOrderDetail.ReductionAmount != 0
                                                || PsOrderDetail.ReductionPercent != 0);
                                break;
                            }
                        }

                    if (idtaxrulesgroup != 0 && !String.IsNullOrEmpty(ar_ref))
                    {

                        InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, "## Article de remplacement ##", DL_Ligne, null);
                        DL_Ligne += 10000;

                        #region ligne article remplacement
                        Model.Sage.F_ARTICLE F_ARTICLE = new Model.Sage.F_ARTICLERepository().ReadReference(ar_ref);
                        ABSTRACTION_SAGE.F_DOCLIGNE.Obj ObjF_DOCLIGNE = new ABSTRACTION_SAGE.F_DOCLIGNE.Obj();
                        ObjF_DOCLIGNE.CT_Num = ObjF_DOCENTETE.DO_Tiers;
                        ObjF_DOCLIGNE.CO_No = ObjF_DOCENTETE.CO_No;

                        ObjF_DOCLIGNE.DO_Domaine = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DO_Domaine)ObjF_DOCENTETE.DO_Domaine;
                        ObjF_DOCLIGNE.DO_Type = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DO_Type)ObjF_DOCENTETE.DO_Type;
                        ObjF_DOCLIGNE.DO_Piece = ObjF_DOCENTETE.DO_Piece;
                        ObjF_DOCLIGNE.DO_Date = ObjF_DOCENTETE.DO_Date;
                        ObjF_DOCLIGNE.DL_DateBC = ObjF_DOCLIGNE.DO_Date;
                        ObjF_DOCLIGNE.DL_DateBL = ObjF_DOCLIGNE.DO_Date;
            			#if (SAGE_VERSION_18 || SAGE_VERSION_19 || SAGE_VERSION_20 || SAGE_VERSION_21)
                        ObjF_DOCLIGNE.DL_DatePL = ObjF_DOCLIGNE.DO_Date;
                        ObjF_DOCLIGNE.DL_DateDE = ObjF_DOCLIGNE.DO_Date;
                        #endif

                        ObjF_DOCLIGNE.DO_DateLivr = ObjF_DOCENTETE.DO_DateLivr;

                        if (F_ARTICLE.AR_SuiviStock == 0)
                            ObjF_DOCLIGNE.DE_No = 0;
                        else
                            ObjF_DOCLIGNE.DE_No = ObjF_DOCENTETE.DE_No;

                        ObjF_DOCLIGNE.DL_Ligne = DL_Ligne;
                        ObjF_DOCLIGNE.DL_Qte = PsOrderDetail.ProductQuantity;

                        #region Taxes

                        Core.Sync.SynchronisationArticle SyncArticle = new SynchronisationArticle();
                        Model.Sage.F_TAXE F_TAXETVA = SyncArticle.ReadTaxe(F_ARTICLE, new Model.Prestashop.PsProduct(), ObjF_DOCENTETE.N_CatCompta);
                        if (F_TAXETVA.TA_Taux != null)
                        {
                            switch (Core.Global.GetConfig().TaxSageTVA)
                            {
                                case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe1:
                                    ObjF_DOCLIGNE.DL_Taxe1 = (decimal)F_TAXETVA.TA_Taux;
                                    ObjF_DOCLIGNE.DL_TypeTaux1 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux)F_TAXETVA.TA_TTaux;
                                    ObjF_DOCLIGNE.DL_TypeTaxe1 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_TAXETVA.TA_Type;
									#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                    ObjF_DOCLIGNE.DL_CodeTaxe1 = F_TAXETVA.TA_Code;
									#endif
                                    break;
                                case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe2:
                                    ObjF_DOCLIGNE.DL_Taxe2 = (decimal)F_TAXETVA.TA_Taux;
                                    ObjF_DOCLIGNE.DL_TypeTaux2 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux)F_TAXETVA.TA_TTaux;
                                    ObjF_DOCLIGNE.DL_TypeTaxe2 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_TAXETVA.TA_Type;
									#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                    ObjF_DOCLIGNE.DL_CodeTaxe2 = F_TAXETVA.TA_Code;
									#endif
                                    break;
                                case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe3:
                                    ObjF_DOCLIGNE.DL_Taxe3 = (decimal)F_TAXETVA.TA_Taux;
                                    ObjF_DOCLIGNE.DL_TypeTaux3 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux)F_TAXETVA.TA_TTaux;
                                    ObjF_DOCLIGNE.DL_TypeTaxe3 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_TAXETVA.TA_Type;
									#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                    ObjF_DOCLIGNE.DL_CodeTaxe3 = F_TAXETVA.TA_Code;
									#endif
                                    break;
                                case PRESTACONNECT.Core.Parametres.TaxSage.Empty:
                                default:
                                    break;
                            }
                        }

                        #endregion

                        ObjF_DOCLIGNE.DL_PUTTC = (PsOrderDetail.UnitPriceTaxInCl);
                        ObjF_DOCLIGNE.DL_PrixUnitaire = PsOrderDetail.UnitPriceTaxExCl;
                        ObjF_DOCLIGNE.DL_TTC = (ValorisationTTC) ? ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TTC.TTC : ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TTC.HT;

                        if (!LigneRemise)
                        {
                            ObjF_DOCLIGNE.DL_PUTTC = (PsOrderDetail.UnitPriceTaxInCl);
                            ObjF_DOCLIGNE.DL_PrixUnitaire = PsOrderDetail.UnitPriceTaxExCl;
                            ObjF_DOCLIGNE.DL_TTC = (ValorisationTTC) ? ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TTC.TTC : ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TTC.HT;
                            if (ObjF_DOCENTETE.DO_Devise > 0)
                                ObjF_DOCLIGNE.DL_PUDevise = ObjF_DOCLIGNE.DL_PrixUnitaire;
                        }
                        else
                        {
                            Model.Prestashop.PsTaxRepository PsTaxRepository = new Model.Prestashop.PsTaxRepository();
                            decimal taxamount = 0;
                            if (PsOrderDetailTaxRepository.ExistOrderDetail(PsOrderDetail.IDOrderDetail))
                                foreach (Model.Prestashop.PsOrderDetailTax PsOrderDetailTax in PsOrderDetailTaxRepository.ListOrderDetail(PsOrderDetail.IDOrderDetail))
                                    if (PsTaxRepository.ExistTaxe((uint)PsOrderDetailTax.IDTax))
                                        taxamount += (PsOrderDetail.ProductPrice * PsTaxRepository.ReadTax((uint)PsOrderDetailTax.IDTax).Rate / 100);

                            ObjF_DOCLIGNE.DL_PUTTC = (PsOrderDetail.ProductPrice + taxamount);
                            ObjF_DOCLIGNE.DL_PrixUnitaire = Math.Round(PsOrderDetail.ProductPrice, 2);
                            ObjF_DOCLIGNE.DL_TTC = (ValorisationTTC) ? ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TTC.TTC : ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TTC.HT;
                            if (ObjF_DOCENTETE.DO_Devise > 0)
                                ObjF_DOCLIGNE.DL_PUDevise = ObjF_DOCLIGNE.DL_PrixUnitaire;
                        }

                        ObjF_DOCLIGNE.DL_MontantHT = 0;
                        ObjF_DOCLIGNE.DL_MontantTTC = 0;
                        ObjF_DOCLIGNE.AR_Ref = F_ARTICLE.AR_Ref;
                        ObjF_DOCLIGNE.DL_Design = (PsOrderDetail.ProductName.Length <= 69) ? PsOrderDetail.ProductName : PsOrderDetail.ProductName.Substring(0, 69);
                        ObjF_DOCLIGNE.DL_No = 0;
                        ObjF_DOCLIGNE.CA_Num = ObjF_DOCENTETE.CA_Num;
                        ObjF_DOCLIGNE.DL_Escompte = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_Escompte.Soumis;
                        ObjF_DOCLIGNE.DL_Valorise = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_Valorise.Ligne_Valorisee;
                        ObjF_DOCLIGNE.EU_Qte = ObjF_DOCLIGNE.DL_Qte;
                        ObjF_DOCLIGNE.EU_Enumere = F_ARTICLE.UniteVenteString;
                        ObjF_DOCLIGNE.DL_QteBC = ObjF_DOCLIGNE.DL_Qte;

                        #region Gestion poids

                        ObjF_DOCLIGNE.DL_PoidsNet = (decimal)PsOrderDetail.ProductWeight;
                        ObjF_DOCLIGNE.DL_PoidsBrut = (decimal)PsOrderDetail.ProductWeight;

                        #endregion

                        //Hard Coded
                        ObjF_DOCLIGNE.DL_FactPoids = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_FactPoids.Non_Facture;
                        if (F_ARTICLE.AR_Nomencl == 2)
                        {
                            ObjF_DOCLIGNE.AR_RefCompose = F_ARTICLE.AR_Ref;
                        }
                        ObjF_DOCLIGNE.Add(Connexion);
                        DL_Ligne += 10000;
                        #endregion

                        if (LigneRemise)
                        {
                            this.ExecLigneRemise(Connexion, ObjF_DOCENTETE, DL_Ligne, "Remise " + PsOrderDetail.ProductName,
                                F_TAXETVA, PsOrderDetail, ValorisationTTC, ObjF_DOCLIGNE, out DL_Ligne);
                        }

                        #region Gammes

                        if (PsOrderDetail.ProductAttributeID != null && PsOrderDetail.ProductAttributeID != 0)
                        {
                            Model.Prestashop.PsProductAttributeCombinationRepository PsProductAttributeCombinationRepository = new Model.Prestashop.PsProductAttributeCombinationRepository();
                            List<Model.Prestashop.PsProductAttributeCombination> ListPsProductAttributeCombination = PsProductAttributeCombinationRepository.ListProductAttribute(PsOrderDetail.ProductAttributeID.Value);
                            Model.Prestashop.PsAttributeRepository PsAttributeRepository = new Model.Prestashop.PsAttributeRepository();
                            Model.Prestashop.PsAttributeLangRepository PsAttributeLangRepository = new Model.Prestashop.PsAttributeLangRepository();
                            Model.Prestashop.PsAttributeGroupLangRepository PsAttributeGroupLangRepository = new Model.Prestashop.PsAttributeGroupLangRepository();
                            foreach (Model.Prestashop.PsProductAttributeCombination PsProductAttributeCombination in ListPsProductAttributeCombination)
                            {
                                try
                                {
                                    string commentaire_gamme = string.Empty;
                                    Model.Prestashop.PsAttribute PsAttribute = PsAttributeRepository.Read(PsProductAttributeCombination.IDAttribute);
                                    Model.Prestashop.PsAttributeLang PsAttributeLang = PsAttributeLangRepository.ReadAttributeLang(PsAttribute.IDAttribute, Core.Global.Lang);
                                    Model.Prestashop.PsAttributeGroupLang PsAttributeGroupLang = PsAttributeGroupLangRepository.ReadAttributeGroupLang(PsAttribute.IDAttributeGroup, Core.Global.Lang);
                                    commentaire_gamme = PsAttributeGroupLang.Name + " : " + PsAttributeLang.Name;

                                    while (commentaire_gamme.Length > 69)
                                    {
                                        string str = commentaire_gamme.Substring(0, 69);
                                        InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, str, DL_Ligne, null);
                                        DL_Ligne += 10000;
                                        commentaire_gamme = commentaire_gamme.Substring(69);
                                    }
                                    InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, commentaire_gamme, DL_Ligne, null);
                                    DL_Ligne += 10000;
                                }
                                catch (Exception ex)
                                {
                                    Core.Error.SendMailError(ex.ToString());
                                }
                            }
                        }
                        #endregion


                        InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, "## Fin ##", DL_Ligne, null);
                        DL_Ligne += 10000;
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("[SYNC COMMANDE] Erreur article remplacement <br/>" + ex.ToString());
            }
            DL_LIGNE_OUT = DL_Ligne;
        }

        private void ExecLigneDistantToLocalPack(ABSTRACTION_SAGE.ALTERNETIS.Connexion Connexion,
            ABSTRACTION_SAGE.F_DOCENTETE.Obj ObjF_DOCENTETE, Model.Sage.F_COMPTET F_COMPTET,
            Model.Sage.F_ARTICLE F_ARTICLE, Int32 Gamme1, Int32 Gamme2, Int32 DL_Ligne,
            Model.Prestashop.PsOrderDetail PsOrderDetail, Boolean ValorisationTTC, out Int32 DL_LIGNE_OUT)
        {
            try
            {
                Decimal PUHT, PUTTC;
                ABSTRACTION_SAGE.F_DOCLIGNE.Obj ObjF_DOCLIGNE = new ABSTRACTION_SAGE.F_DOCLIGNE.Obj();
                #region entete nomenclature lorsque packaging
                if ((!string.IsNullOrEmpty(Core.Global.GetConfig().ArticleInfolibrePackaging) || !string.IsNullOrEmpty(Core.Global.GetConfig().CommandeArticlePackaging))
                    && (F_ARTICLE.AR_Ref == Core.Global.GetConfig().ArticleInfolibrePackaging || F_ARTICLE.AR_Ref == Core.Global.GetConfig().CommandeArticlePackaging)
                    && F_ARTICLE.AR_Nomencl == (short)ABSTRACTION_SAGE.F_ARTICLE.Obj._Enum_AR_Nomencl.Article_Lie)
                {
                    if (PsOrderDetail == null)
                        PsOrderDetail = new Model.Prestashop.PsOrderDetail()
                        {
                            ProductQuantity = 1,
                        };

                    ObjF_DOCLIGNE = new ABSTRACTION_SAGE.F_DOCLIGNE.Obj();
                    ObjF_DOCLIGNE.CT_Num = ObjF_DOCENTETE.DO_Tiers;
                    ObjF_DOCLIGNE.CO_No = ObjF_DOCENTETE.CO_No;

                    ObjF_DOCLIGNE.DO_Domaine = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DO_Domaine)ObjF_DOCENTETE.DO_Domaine;
                    ObjF_DOCLIGNE.DO_Type = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DO_Type)ObjF_DOCENTETE.DO_Type;
                    ObjF_DOCLIGNE.DO_Piece = ObjF_DOCENTETE.DO_Piece;
                    ObjF_DOCLIGNE.DO_Date = ObjF_DOCENTETE.DO_Date;
                    ObjF_DOCLIGNE.DL_DateBC = ObjF_DOCLIGNE.DO_Date;
                    ObjF_DOCLIGNE.DL_DateBL = ObjF_DOCLIGNE.DO_Date;
            		#if (SAGE_VERSION_18 || SAGE_VERSION_19 || SAGE_VERSION_20 || SAGE_VERSION_21)
                    ObjF_DOCLIGNE.DL_DatePL = ObjF_DOCLIGNE.DO_Date;
                    ObjF_DOCLIGNE.DL_DateDE = ObjF_DOCLIGNE.DO_Date;
                    #endif

                    ObjF_DOCLIGNE.DO_DateLivr = ObjF_DOCENTETE.DO_DateLivr;

                    if (F_ARTICLE.AR_SuiviStock == 0)
                        ObjF_DOCLIGNE.DE_No = 0;
                    else
                        ObjF_DOCLIGNE.DE_No = ObjF_DOCENTETE.DE_No;

                    ObjF_DOCLIGNE.DL_Ligne = DL_Ligne;
                    ObjF_DOCLIGNE.DL_Qte = PsOrderDetail.ProductQuantity;

                    #region Taxes
                    Core.Sync.SynchronisationArticle SyncArticle = new SynchronisationArticle();
                    Model.Sage.F_TAXE F_TAXETVA = SyncArticle.ReadTaxe(F_ARTICLE, new Model.Prestashop.PsProduct(), ObjF_DOCENTETE.N_CatCompta);
                    if (F_TAXETVA.TA_Taux != null)
                    {
                        switch (Core.Global.GetConfig().TaxSageTVA)
                        {
                            case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe1:
                                ObjF_DOCLIGNE.DL_Taxe1 = (decimal)F_TAXETVA.TA_Taux;
                                ObjF_DOCLIGNE.DL_TypeTaux1 = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux.Taux;
                                ObjF_DOCLIGNE.DL_TypeTaxe1 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_TAXETVA.TA_Type;
								#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                ObjF_DOCLIGNE.DL_CodeTaxe1 = F_TAXETVA.TA_Code;
								#endif
                                break;
                            case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe2:
                                ObjF_DOCLIGNE.DL_Taxe2 = (decimal)F_TAXETVA.TA_Taux;
                                ObjF_DOCLIGNE.DL_TypeTaux2 = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux.Taux;
                                ObjF_DOCLIGNE.DL_TypeTaxe2 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_TAXETVA.TA_Type;
								#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                ObjF_DOCLIGNE.DL_CodeTaxe2 = F_TAXETVA.TA_Code;
								#endif
                                break;
                            case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe3:
                                ObjF_DOCLIGNE.DL_Taxe3 = (decimal)F_TAXETVA.TA_Taux;
                                ObjF_DOCLIGNE.DL_TypeTaux3 = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux.Taux;
                                ObjF_DOCLIGNE.DL_TypeTaxe3 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_TAXETVA.TA_Type;
								#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                ObjF_DOCLIGNE.DL_CodeTaxe3 = F_TAXETVA.TA_Code;
								#endif
                                break;
                            case PRESTACONNECT.Core.Parametres.TaxSage.Empty:
                            default:
                                break;
                        }
                    }
                    #endregion

                    this.ReadPricePack(F_ARTICLE, F_COMPTET, ObjF_DOCENTETE, ValorisationTTC, out PUHT, out PUTTC);
                    ObjF_DOCLIGNE.DL_PUTTC = PUTTC;
                    ObjF_DOCLIGNE.DL_PrixUnitaire = PUHT;
                    ObjF_DOCLIGNE.DL_TTC = (ValorisationTTC)
                                                ? ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TTC.TTC
                                                : ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TTC.HT;
                    if (ObjF_DOCENTETE.DO_Devise > 0)
                    {
                        if (PUHT == 0 && PUTTC != 0)
                        {
                            if (F_TAXETVA != null && F_TAXETVA.TA_Taux != null && F_TAXETVA.TA_Taux != 0)
                                PUHT = PUTTC / (1 + (F_TAXETVA.TA_Taux.Value / 100));
                            else
                                PUHT = PUTTC;

                            ObjF_DOCLIGNE.DL_PrixUnitaire = PUHT;
                        }
                        ObjF_DOCLIGNE.DL_PUDevise = ObjF_DOCLIGNE.DL_PrixUnitaire;
                    }

                    if (PUHT != 0 || PUTTC != 0)
                    {
                        ObjF_DOCLIGNE.DL_Remise01REM_Valeur = 100;
                        ObjF_DOCLIGNE.DL_Remise01REM_Type = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_RemiseREM_Type.Pourcentage;
                    }

                    ObjF_DOCLIGNE.DL_MontantHT = 0;
                    ObjF_DOCLIGNE.DL_MontantTTC = 0;
                    ObjF_DOCLIGNE.AR_Ref = F_ARTICLE.AR_Ref;
                    ObjF_DOCLIGNE.DL_Design = (F_ARTICLE.AR_Design.Length <= 69) ? F_ARTICLE.AR_Design : F_ARTICLE.AR_Design.Substring(0, 69);

                    switch (ObjF_DOCENTETE.DO_Langue)
                    {
                        case ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Langue.Langue_1:
                            if (!string.IsNullOrWhiteSpace(F_ARTICLE.AR_Langue1))
                                ObjF_DOCLIGNE.DL_Design = F_ARTICLE.AR_Langue1;
                            break;
                        case ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Langue.Langue_2:
                            if (!string.IsNullOrWhiteSpace(F_ARTICLE.AR_Langue2))
                                ObjF_DOCLIGNE.DL_Design = F_ARTICLE.AR_Langue2;
                            break;
                        case ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Langue.Aucune:
                        default:
                            // l'affectation par défaut est réalisée avant
                            break;
                    }

                    ObjF_DOCLIGNE.DL_No = 0;
                    ObjF_DOCLIGNE.CA_Num = ObjF_DOCENTETE.CA_Num;
                    ObjF_DOCLIGNE.DL_Escompte = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_Escompte.Soumis;
                    ObjF_DOCLIGNE.DL_Valorise = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_Valorise.Ligne_Valorisee;
                    ObjF_DOCLIGNE.EU_Qte = ObjF_DOCLIGNE.DL_Qte;
                    ObjF_DOCLIGNE.EU_Enumere = F_ARTICLE.UniteVenteString;
                    ObjF_DOCLIGNE.DL_QteBC = ObjF_DOCLIGNE.DL_Qte;

                    ObjF_DOCLIGNE.DL_FactPoids = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_FactPoids.Non_Facture;
                    ObjF_DOCLIGNE.Add(Connexion);
                    DL_Ligne += 10000;
                }
                #endregion

                Model.Sage.F_NOMENCLATRepository F_NOMENCLATRepository = new Model.Sage.F_NOMENCLATRepository();
                List<Model.Sage.F_NOMENCLAT> ListF_NOMENCLAT = F_NOMENCLATRepository.ListRef(F_ARTICLE.AR_Ref);
                // <JG> 12/01/2015 ajout filtrage par gamme des composés
                if (Gamme1 != 0 || Gamme2 != 0)
                {
                    ListF_NOMENCLAT = ListF_NOMENCLAT.Where(n => n.AG_No1Comp == Gamme1 && n.AG_No2Comp == Gamme2).ToList();
                }
                Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
                Model.Sage.F_ARTICLE F_ARTICLENOMENCLAT;
                foreach (Model.Sage.F_NOMENCLAT F_NOMENCLAT in ListF_NOMENCLAT)
                {
                    #region composé
                    try
                    {
                        ObjF_DOCLIGNE = new ABSTRACTION_SAGE.F_DOCLIGNE.Obj();
                        PUHT = 0;
                        PUTTC = 0;
                        if (F_ARTICLERepository.ExistReference(F_NOMENCLAT.NO_RefDet))
                        {
                            F_ARTICLENOMENCLAT = F_ARTICLERepository.ReadReference(F_NOMENCLAT.NO_RefDet);
                            if (F_ARTICLENOMENCLAT.AR_Gamme1 != null && F_ARTICLENOMENCLAT.AR_Gamme1 != 0)
                            {
                                InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, "Article en gamme non géré en pack : " + F_ARTICLENOMENCLAT.AR_Ref, DL_Ligne, null);
                            }
                            else
                            {
                                this.ReadPricePack(F_ARTICLENOMENCLAT, F_COMPTET, ObjF_DOCENTETE, ValorisationTTC, out PUHT, out PUTTC);
                                ObjF_DOCLIGNE.DO_Domaine = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DO_Domaine)ObjF_DOCENTETE.DO_Domaine;
                                ObjF_DOCLIGNE.DO_Type = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DO_Type)ObjF_DOCENTETE.DO_Type;
                                ObjF_DOCLIGNE.CT_Num = ObjF_DOCENTETE.DO_Tiers;
                                ObjF_DOCLIGNE.DO_Piece = ObjF_DOCENTETE.DO_Piece;
                                ObjF_DOCLIGNE.DO_Date = ObjF_DOCENTETE.DO_Date;
                                ObjF_DOCLIGNE.DL_DateBC = ObjF_DOCLIGNE.DO_Date;
                                ObjF_DOCLIGNE.DL_DateBL = ObjF_DOCLIGNE.DO_Date;
            					#if (SAGE_VERSION_18 || SAGE_VERSION_19 || SAGE_VERSION_20 || SAGE_VERSION_21)
                                ObjF_DOCLIGNE.DL_DatePL = ObjF_DOCLIGNE.DO_Date;
                                ObjF_DOCLIGNE.DL_DateDE = ObjF_DOCLIGNE.DO_Date;
                                #endif
                                ObjF_DOCLIGNE.DL_Ligne = DL_Ligne;
                                ObjF_DOCLIGNE.CO_No = ObjF_DOCENTETE.CO_No;
                                ObjF_DOCLIGNE.DO_DateLivr = ObjF_DOCENTETE.DO_DateLivr;

                                ObjF_DOCLIGNE.DE_No = (F_ARTICLENOMENCLAT.AR_SuiviStock == 0) ? 0 : ObjF_DOCENTETE.DE_No;

                                // <JG> 01/07/2013 suppression mise en commentaire
                                ObjF_DOCLIGNE.DL_Qte = (PsOrderDetail.ProductQuantity * F_NOMENCLAT.NO_Qte.Value);

                                #region Taxes

                                Core.Sync.SynchronisationArticle SyncArticle = new SynchronisationArticle();
                                Model.Sage.F_TAXE F_TAXETVA = SyncArticle.ReadTaxe(F_ARTICLENOMENCLAT, new Model.Prestashop.PsProduct(), ObjF_DOCENTETE.N_CatCompta);
                                if (F_TAXETVA.TA_Taux != null)
                                {
                                    switch (Core.Global.GetConfig().TaxSageTVA)
                                    {
                                        case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe1:
                                            ObjF_DOCLIGNE.DL_Taxe1 = (decimal)F_TAXETVA.TA_Taux;
                                            ObjF_DOCLIGNE.DL_TypeTaux1 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux)F_TAXETVA.TA_TTaux;
                                            ObjF_DOCLIGNE.DL_TypeTaxe1 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_TAXETVA.TA_Type;
											#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                            ObjF_DOCLIGNE.DL_CodeTaxe1 = F_TAXETVA.TA_Code;
											#endif
                                            break;
                                        case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe2:
                                            ObjF_DOCLIGNE.DL_Taxe2 = (decimal)F_TAXETVA.TA_Taux;
                                            ObjF_DOCLIGNE.DL_TypeTaux2 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux)F_TAXETVA.TA_TTaux;
                                            ObjF_DOCLIGNE.DL_TypeTaxe2 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_TAXETVA.TA_Type;
											#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                            ObjF_DOCLIGNE.DL_CodeTaxe2 = F_TAXETVA.TA_Code;
											#endif
                                            break;
                                        case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe3:
                                            ObjF_DOCLIGNE.DL_Taxe3 = (decimal)F_TAXETVA.TA_Taux;
                                            ObjF_DOCLIGNE.DL_TypeTaux3 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux)F_TAXETVA.TA_TTaux;
                                            ObjF_DOCLIGNE.DL_TypeTaxe3 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_TAXETVA.TA_Type;
											#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                            ObjF_DOCLIGNE.DL_CodeTaxe3 = F_TAXETVA.TA_Code;
											#endif
                                            break;
                                        case PRESTACONNECT.Core.Parametres.TaxSage.Empty:
                                        default:
                                            break;
                                    }
                                }
                                Model.Sage.F_TAXE F_ECOTAXE = SyncArticle.ReadEcoTaxe(F_ARTICLENOMENCLAT, new Model.Prestashop.PsProduct(), F_TAXETVA, ObjF_DOCENTETE.N_CatCompta);
                                if (F_ECOTAXE.TA_Taux != null)
                                {
                                    switch (Core.Global.GetConfig().TaxSageEco)
                                    {
                                        case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe1:
                                            ObjF_DOCLIGNE.DL_Taxe1 = (decimal)F_ECOTAXE.TA_Taux;
                                            ObjF_DOCLIGNE.DL_TypeTaux1 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux)F_ECOTAXE.TA_TTaux;
                                            ObjF_DOCLIGNE.DL_TypeTaxe1 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_ECOTAXE.TA_Type;
											#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                            ObjF_DOCLIGNE.DL_CodeTaxe1 = F_ECOTAXE.TA_Code;
											#endif
                                            break;
                                        case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe2:
                                            ObjF_DOCLIGNE.DL_Taxe2 = (decimal)F_ECOTAXE.TA_Taux;
                                            ObjF_DOCLIGNE.DL_TypeTaux2 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux)F_ECOTAXE.TA_TTaux;
                                            ObjF_DOCLIGNE.DL_TypeTaxe2 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_ECOTAXE.TA_Type;
											#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                            ObjF_DOCLIGNE.DL_CodeTaxe2 = F_ECOTAXE.TA_Code;
											#endif
                                            break;
                                        case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe3:
                                            ObjF_DOCLIGNE.DL_Taxe3 = (decimal)F_ECOTAXE.TA_Taux;
                                            ObjF_DOCLIGNE.DL_TypeTaux3 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux)F_ECOTAXE.TA_TTaux;
                                            ObjF_DOCLIGNE.DL_TypeTaxe3 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_ECOTAXE.TA_Type;
											#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                            ObjF_DOCLIGNE.DL_CodeTaxe3 = F_ECOTAXE.TA_Code;
											#endif
                                            break;
                                        case PRESTACONNECT.Core.Parametres.TaxSage.Empty:
                                        default:
                                            break;
                                    }
                                }

                                #endregion

                                // <JG> 21/03/2013 Correction calcul montant HT et TTC en 1.5
                                ObjF_DOCLIGNE.DL_PUTTC = PUTTC;
                                ObjF_DOCLIGNE.DL_PrixUnitaire = PUHT;
                                ObjF_DOCLIGNE.DL_TTC = (ValorisationTTC)
                                                            ? ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TTC.TTC
                                                            : ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TTC.HT;

                                if (PUHT == 0 && PUTTC != 0)
                                {
                                    if (F_TAXETVA != null && F_TAXETVA.TA_Taux != null && F_TAXETVA.TA_Taux != 0)
                                        PUHT = PUTTC / (1 + (F_TAXETVA.TA_Taux.Value / 100));
                                    else
                                        PUHT = PUTTC;

                                    ObjF_DOCLIGNE.DL_PrixUnitaire = PUHT;
                                }

                                if (!ValorisationTTC && PsOrderDetail.EcOtAx != 0)
                                    ObjF_DOCLIGNE.DL_PrixUnitaire -= PsOrderDetail.EcOtAx;

                                if (ObjF_DOCENTETE.DO_Devise > 0)
                                    ObjF_DOCLIGNE.DL_PUDevise = ObjF_DOCLIGNE.DL_PrixUnitaire;

                                if ((F_ARTICLE.AR_Nomencl == (short)ABSTRACTION_SAGE.F_ARTICLE.Obj._Enum_AR_Nomencl.Article_Lie)
                                    && (PUHT != 0 || PUTTC != 0))
                                {
                                    ObjF_DOCLIGNE.DL_Remise01REM_Valeur = 100;
                                    ObjF_DOCLIGNE.DL_Remise01REM_Type = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_RemiseREM_Type.Pourcentage;
                                }

                                ObjF_DOCLIGNE.DL_MontantHT = 0;
                                ObjF_DOCLIGNE.DL_MontantTTC = 0;
                                ObjF_DOCLIGNE.AR_Ref = F_ARTICLENOMENCLAT.AR_Ref;
                                ObjF_DOCLIGNE.DL_Design = F_ARTICLENOMENCLAT.AR_Design;

                                switch (ObjF_DOCENTETE.DO_Langue)
                                {
                                    case ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Langue.Langue_1:
                                        if (!string.IsNullOrWhiteSpace(F_ARTICLENOMENCLAT.AR_Langue1))
                                            ObjF_DOCLIGNE.DL_Design = F_ARTICLENOMENCLAT.AR_Langue1;
                                        break;
                                    case ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Langue.Langue_2:
                                        if (!string.IsNullOrWhiteSpace(F_ARTICLENOMENCLAT.AR_Langue2))
                                            ObjF_DOCLIGNE.DL_Design = F_ARTICLENOMENCLAT.AR_Langue2;
                                        break;
                                    case ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Langue.Aucune:
                                    default:
                                        // l'affectation par défaut est réalisée avant
                                        break;
                                }

                                ObjF_DOCLIGNE.DL_No = 0;
                                ObjF_DOCLIGNE.CA_Num = ObjF_DOCENTETE.CA_Num;
                                ObjF_DOCLIGNE.DL_Escompte = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_Escompte.Soumis;
                                ObjF_DOCLIGNE.DL_Valorise = (F_ARTICLE.AR_Nomencl == (short)ABSTRACTION_SAGE.F_ARTICLE.Obj._Enum_AR_Nomencl.Article_Lie)
                                                                ? ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_Valorise.Ligne_Valorisee
                                                                : ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_Valorise.Ligne_Non_Valorisee;
                                ObjF_DOCLIGNE.EU_Qte = ObjF_DOCLIGNE.DL_Qte;
                                ObjF_DOCLIGNE.EU_Enumere = F_ARTICLENOMENCLAT.UniteVenteString;
                                ObjF_DOCLIGNE.DL_QteBC = ObjF_DOCLIGNE.DL_Qte;
                                ObjF_DOCLIGNE.DL_PoidsNet = (F_ARTICLENOMENCLAT.AR_PoidsNet != null) ? (decimal)F_ARTICLENOMENCLAT.AR_PoidsNet * ObjF_DOCLIGNE.DL_Qte : 0;
                                ObjF_DOCLIGNE.DL_PoidsBrut = (F_ARTICLENOMENCLAT.AR_PoidsBrut != null) ? (decimal)F_ARTICLENOMENCLAT.AR_PoidsBrut * ObjF_DOCLIGNE.DL_Qte : 0;
                                ObjF_DOCLIGNE.AR_RefCompose = F_ARTICLE.AR_Ref;
                                if (F_ARTICLENOMENCLAT.AR_UnitePoids != null)
                                {
                                    switch (F_ARTICLENOMENCLAT.AR_UnitePoids.Value)
                                    {
                                        case 0:
                                            ObjF_DOCLIGNE.DL_PoidsNet = ObjF_DOCLIGNE.DL_PoidsNet * 1000000;
                                            ObjF_DOCLIGNE.DL_PoidsBrut = ObjF_DOCLIGNE.DL_PoidsBrut * 1000000;
                                            break;
                                        case 1:
                                            ObjF_DOCLIGNE.DL_PoidsNet = ObjF_DOCLIGNE.DL_PoidsNet * 100000;
                                            ObjF_DOCLIGNE.DL_PoidsBrut = ObjF_DOCLIGNE.DL_PoidsBrut * 100000;
                                            break;
                                        case 2:
                                            ObjF_DOCLIGNE.DL_PoidsNet = ObjF_DOCLIGNE.DL_PoidsNet * 1000;
                                            ObjF_DOCLIGNE.DL_PoidsBrut = ObjF_DOCLIGNE.DL_PoidsBrut * 1000;
                                            break;
                                        case 3:
                                            ObjF_DOCLIGNE.DL_PoidsNet = ObjF_DOCLIGNE.DL_PoidsNet * 1;
                                            ObjF_DOCLIGNE.DL_PoidsBrut = ObjF_DOCLIGNE.DL_PoidsBrut * 1;
                                            break;
                                        case 4:
                                            ObjF_DOCLIGNE.DL_PoidsNet = ObjF_DOCLIGNE.DL_PoidsNet / 1000;
                                            ObjF_DOCLIGNE.DL_PoidsBrut = ObjF_DOCLIGNE.DL_PoidsBrut / 1000;
                                            break;
                                    }
                                }

                                //Hard Coded
                                ObjF_DOCLIGNE.DL_FactPoids = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_FactPoids.Non_Facture;

                                if (ObjF_DOCLIGNE.DL_Qte != 0)
                                {
                                    ObjF_DOCLIGNE.Add(Connexion);
                                }
                                else
                                {
                                    InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, "Attention quantité 0 pour le composé : " + F_ARTICLENOMENCLAT.AR_Ref, DL_Ligne, null);
                                }
                            }
                            DL_Ligne += 10000;
                        }
                    }
                    catch (Exception ex)
                    {
                        Core.Error.SendMailError("Erreur ligne composé :" + ex.ToString());
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
            DL_LIGNE_OUT = DL_Ligne;
        }

        private void ReadPricePack(Model.Sage.F_ARTICLE F_ARTICLE, Model.Sage.F_COMPTET F_COMPTET, ABSTRACTION_SAGE.F_DOCENTETE.Obj ObjF_DOCENTETE, Boolean ValorisationTTC, out Decimal PUHT, out Decimal PUTTC)
        {
            PUHT = 0;
            PUTTC = 0;
            try
            {
                Core.Sync.SynchronisationArticle SyncArticle = new SynchronisationArticle();
                Model.Sage.F_TAXE F_TAXETVA = SyncArticle.ReadTaxe(F_ARTICLE, new Model.Prestashop.PsProduct(), ObjF_DOCENTETE.N_CatCompta);

                Model.Sage.F_ARTCLIENTRepository F_ARTCLIENTRepository = new Model.Sage.F_ARTCLIENTRepository();
                if (F_COMPTET.N_CatTarif != null && F_ARTCLIENTRepository.ExistReferenceCategorie(F_ARTICLE.AR_Ref, (short)F_COMPTET.N_CatTarif))
                {
                    Model.Sage.F_ARTCLIENT F_ARTCLIENT = F_ARTCLIENTRepository.ReadReferenceCategorie(F_ARTICLE.AR_Ref, (short)F_COMPTET.N_CatTarif);
                    if (F_ARTCLIENT.AC_PrixTTC == 1)
                    {
                        Model.Local.TaxRepository TaxRepository = new Model.Local.TaxRepository();
                        if (TaxRepository.ExistSage(F_TAXETVA.cbMarq))
                        {
                            Model.Local.Tax Tax = TaxRepository.ReadSage(F_TAXETVA.cbMarq);
                            Model.Prestashop.PsTaxRepository PsTaxRepository = new Model.Prestashop.PsTaxRepository();
                            if (PsTaxRepository.ExistTaxe((UInt32)Tax.Pre_Id))
                            {
                                Model.Prestashop.PsTax PsTax = PsTaxRepository.ReadTax((UInt32)Tax.Pre_Id);
                                if (ValorisationTTC)
                                    PUTTC = (Decimal)F_ARTCLIENT.AC_PrixVen;
                                else
                                    PUHT = (Decimal)F_ARTCLIENT.AC_PrixVen / (1 + (PsTax.Rate / 100));
                            }
                        }
                    }
                    else
                    {
                        PUHT = (Decimal)F_ARTCLIENT.AC_PrixVen;
                        PUTTC = (Decimal)F_ARTCLIENT.AC_PrixVen;
                    }
                    if (PUHT == 0 && PUTTC == 0 && F_ARTCLIENT.AC_Coef != null)
                    {
                        if (F_ARTCLIENT.AC_PrixTTC == 1)
                        {
                            Model.Local.TaxRepository TaxRepository = new Model.Local.TaxRepository();
                            if (TaxRepository.ExistSage(F_TAXETVA.cbMarq))
                            {
                                Model.Local.Tax Tax = TaxRepository.ReadSage(F_TAXETVA.cbMarq);
                                Model.Prestashop.PsTaxRepository PsTaxRepository = new Model.Prestashop.PsTaxRepository();
                                if (PsTaxRepository.ExistTaxe((UInt32)Tax.Pre_Id))
                                {
                                    Model.Prestashop.PsTax PsTax = PsTaxRepository.ReadTax((UInt32)Tax.Pre_Id);
                                    if (ValorisationTTC)
                                        PUTTC = (Decimal)(F_ARTICLE.AR_PrixAch * F_ARTCLIENT.AC_Coef);
                                    else
                                        PUHT = (Decimal)(F_ARTICLE.AR_PrixAch * F_ARTCLIENT.AC_Coef) / (1 + (PsTax.Rate / 100));
                                }
                            }
                        }
                        else
                        {
                            PUHT = (Decimal)F_ARTICLE.AR_PrixAch * F_ARTCLIENT.AC_Coef.Value;
                            PUTTC = (Decimal)F_ARTICLE.AR_PrixAch * F_ARTCLIENT.AC_Coef.Value;
                        }
                    }
                }
                if (PUHT == 0 && PUTTC == 0)
                {
                    if (F_ARTICLE.AR_PrixTTC == 1)
                    {
                        Model.Local.TaxRepository TaxRepository = new Model.Local.TaxRepository();
                        if (TaxRepository.ExistSage(F_TAXETVA.cbMarq))
                        {
                            Model.Local.Tax Tax = TaxRepository.ReadSage(F_TAXETVA.cbMarq);
                            Model.Prestashop.PsTaxRepository PsTaxRepository = new Model.Prestashop.PsTaxRepository();
                            if (PsTaxRepository.ExistTaxe((UInt32)Tax.Pre_Id))
                            {
                                Model.Prestashop.PsTax PsTax = PsTaxRepository.ReadTax((UInt32)Tax.Pre_Id);
                                if (ValorisationTTC)
                                    PUTTC = (Decimal)F_ARTICLE.AR_PrixVen;
                                else
                                    PUHT = (Decimal)F_ARTICLE.AR_PrixVen / (1 + (PsTax.Rate / 100));
                            }
                        }
                    }
                    else
                    {
                        PUHT = (Decimal)F_ARTICLE.AR_PrixVen;
                        PUTTC = (Decimal)F_ARTICLE.AR_PrixVen;
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        public void ExecLocalToDistant(Model.Prestashop.PsOrders PsOrders, Model.Prestashop.PsOrdersRepository PsOrdersRepository)
        {
            Model.Prestashop.PsOrderHistoryRepository PsOrderHistoryRepository = new Model.Prestashop.PsOrderHistoryRepository();
            Model.Prestashop.PsOrderHistory PsOrderHistory = new Model.Prestashop.PsOrderHistory();
            Model.Sage.F_DOCENTETERepository F_DOCENTETERepository = new Model.Sage.F_DOCENTETERepository();
            bool trackingsend = false;
            if (F_DOCENTETERepository.ExistWeb(PsOrders.IDOrder.ToString()))
            {
                List<Model.Sage.F_DOCENTETE> ListF_DOCENTETE = F_DOCENTETERepository.ListWeb(PsOrders.IDOrder.ToString());
                if (ListF_DOCENTETE.Count == 1)
                {
                    foreach (Model.Sage.F_DOCENTETE F_DOCENTETE in ListF_DOCENTETE)
                    {
                        if (F_DOCENTETE.DO_Domaine == 0)
                        {
                            UInt32 statut_facture = 0;
                            switch (F_DOCENTETE.DO_Type)
                            {
                                case 0:
                                    PsOrderHistory.IDOrderState = (uint)Core.Global.GetConfig().ConfigCommandeDE;
                                    break;
                                case 1:
                                    PsOrderHistory.IDOrderState = (uint)Core.Global.GetConfig().ConfigCommandeBC;
                                    break;
                                case 2:
                                    PsOrderHistory.IDOrderState = (uint)Core.Global.GetConfig().ConfigCommandePL;
                                    break;
                                case 3:
                                    PsOrderHistory.IDOrderState = (uint)Core.Global.GetConfig().ConfigCommandeBL;
                                    break;
                                case 6:
                                    PsOrderHistory.IDOrderState = (uint)Core.Global.GetConfig().ConfigCommandeFA;
                                    break;
                                // <JG> 17/05/2013 ajout facture comptabilisée
                                case 7:
                                    statut_facture = (uint)Core.Global.GetConfig().ConfigCommandeFA;
                                    PsOrderHistory.IDOrderState = (uint)Core.Global.GetConfig().ConfigCommandeFC;
                                    break;
                            }

                            if (PsOrderHistory.IDOrderState != 0)
                            {
                                // <JG> 09/01/2013 Correction modification en boucle du statut si modification externe ou manuelle du statut dans Prestashop
                                Boolean isvalid = true;
                                isvalid = !PsOrderHistoryRepository.ExistOrderState(PsOrders.IDOrder, PsOrderHistory.IDOrderState);
                                if (isvalid == true)
                                {
                                    PsOrderHistory.IDOrder = PsOrders.IDOrder;
                                    PsOrderHistory.DateAdd = DateTime.Now;
                                    PsOrderHistoryRepository.Add(PsOrderHistory);

                                    // <JG> 04/04/2013 Correction modification statut de commande
                                    PsOrders.CurrentState = PsOrderHistory.IDOrderState;

                                    #region invoice number
                                    Model.Prestashop.PsOrderStateRepository PsOrderStateRepository = new Model.Prestashop.PsOrderStateRepository();
                                    Model.Prestashop.PsOrderInvoiceRepository PsOrderInvoiceRepository = new Model.Prestashop.PsOrderInvoiceRepository();
                                    if ((F_DOCENTETE.DO_Type == 6
                                            || (F_DOCENTETE.DO_Type == 7
                                                && statut_facture != 0
                                                && (!PsOrderHistoryRepository.ExistOrderState(PsOrders.IDOrder, statut_facture)
                                                    || !PsOrderInvoiceRepository.ExistOrder(PsOrders.IDOrder)
                                                    || Core.Global.GetConfig().CommandeNumeroFactureSageForceActif)))
                                        && PsOrderStateRepository.ExistState(PsOrders.CurrentState))
                                    //&& PsOrderStateRepository.ReadState(PsOrders.CurrentState).Invoice == (byte)1)
                                    {
                                        String IdInvoice = string.Empty;
                                        foreach (Char Item in F_DOCENTETE.DO_Piece)
                                            if (Core.Global.IsInteger(Item.ToString()))
                                                // correction insertion 0 si contenu après d'autres chiffres
                                                if (Item != '0' || IdInvoice != string.Empty)
                                                    IdInvoice += Item;

                                        // <JG> 03/05/2013 - 06/05/2013 Modification gestion facture 1.5
                                        if (IdInvoice != string.Empty
                                            && Core.Global.IsInteger(IdInvoice)
                                            && !PsOrderInvoiceRepository.ExistOrder(PsOrders.IDOrder))
                                        {
                                            #region OrderInvoice

                                            Model.Prestashop.PsOrderInvoice OrderInvoice = new Model.Prestashop.PsOrderInvoice()
                                            {
                                                DateAdd = (DateTime)F_DOCENTETE.DO_Date,
                                                DeliveryDate = (F_DOCENTETE.DO_DateLivr != null) ? F_DOCENTETE.DO_DateLivr : null,
                                                DeliveryNumber = (int)PsOrders.DeliveryNumber,
                                                IDOrder = (int)PsOrders.IDOrder,
                                                Note = string.Empty,
                                                Number = Convert.ToInt32(IdInvoice),
                                                ShippingTaxComputationMethod = 0,
                                                TotalDiscountTaxExCl = PsOrders.TotalDiscountsTaxExCl,
                                                TotalDiscountTaxInCl = PsOrders.TotalDiscountsTaxInCl,
                                                TotalPaidTaxExCl = PsOrders.TotalPaidTaxExCl,
                                                TotalPaidTaxInCl = PsOrders.TotalPaidTaxInCl,
                                                TotalProducts = PsOrders.TotalProducts,
                                                TotalProductsWT = PsOrders.TotalProductsWT,
                                                TotalShippingTaxExCl = PsOrders.TotalShippingTaxExCl,
                                                TotalShippingTaxInCl = PsOrders.TotalShippingTaxInCl,
                                                TotalWrappingTaxExCl = PsOrders.TotalWrappingTaxExCl,
                                                TotalWrappingTaxInCl = PsOrders.TotalWrappingTaxInCl
                                            };
                                            PsOrderInvoiceRepository.Add(OrderInvoice);
                                            #endregion

                                            #region OrderPayment

                                            Model.Prestashop.PsOrderPaymentRepository PsOrderPaymentRepository = new Model.Prestashop.PsOrderPaymentRepository();
                                            Model.Prestashop.PsOrderInvoicePaymentRepository PsOrderInvoicePaymentRepository = new Model.Prestashop.PsOrderInvoicePaymentRepository();
                                            if (PsOrderPaymentRepository.ExistOrderReference(PsOrders.Reference) == false)
                                            {
                                                // si présence d'un règlement dans Sage TODO  ????
                                                //Model.Prestashop.PsOrderPayment OrderPayment = new Model.Prestashop.PsOrderPayment()
                                                //{
                                                //    Amount = PsOrders.TotalPaidReal,
                                                //    CardBrand = "",
                                                //    CardExpiration = "",
                                                //    CardHolder = "",
                                                //    CardNumber = "",
                                                //    ConversionRate = PsOrders.ConversionRate,
                                                //    DateAdd = (DateTime)F_DOCENTETE.DO_Date,
                                                //    IDCurrency = PsOrders.IDCurrency,
                                                //    OrderReference = PsOrders.Reference,
                                                //    PaymentMethod = PsOrders.Payment,
                                                //    TransactionID = "",
                                                //};
                                                //PsOrderPaymentRepository.Add(OrderPayment);

                                                //#region OrderInvoicePayment
                                                //Model.Prestashop.PsOrderInvoicePayment OrderInvoicePayment = new Model.Prestashop.PsOrderInvoicePayment()
                                                //{
                                                //    IDOrder = PsOrders.IDOrder,
                                                //    IDOrderInvoice = OrderInvoice.IDOrderInvoice,
                                                //    IDOrderPayment = (uint)OrderPayment.IDOrderPayment,
                                                //};
                                                //PsOrderInvoicePaymentRepository.Add(OrderInvoicePayment);
                                                //#endregion
                                            }
                                            else
                                            {
                                                foreach (Model.Prestashop.PsOrderPayment OrderPayment in PsOrderPaymentRepository.ReadOrderReference(PsOrders.Reference))
                                                {
                                                    #region OrderInvoicePayment
                                                    if (PsOrderInvoicePaymentRepository.ExistPayement((UInt32)OrderPayment.IDOrderPayment) == false)
                                                    {
                                                        Model.Prestashop.PsOrderInvoicePayment OrderInvoicePayment = new Model.Prestashop.PsOrderInvoicePayment()
                                                        {
                                                            IDOrder = PsOrders.IDOrder,
                                                            IDOrderInvoice = OrderInvoice.IDOrderInvoice,
                                                            IDOrderPayment = (uint)OrderPayment.IDOrderPayment,
                                                        };
                                                        PsOrderInvoicePaymentRepository.Add(OrderInvoicePayment);
                                                    }
                                                    #endregion
                                                }
                                            }
                                            #endregion

                                            #region OrderCarrier et OrderInvoiceTax (shipping)

                                            string tracking_number = ReadTrackingNumber(PsOrders, F_DOCENTETE, F_DOCENTETERepository);

                                            Model.Prestashop.PsOrderCarrierRepository PsOrderCarrierRepository = new Model.Prestashop.PsOrderCarrierRepository();
                                            foreach (Model.Prestashop.PsOrderCarrier OrderCarrier in PsOrderCarrierRepository.ListOrder(PsOrders.IDOrder))
                                            {
                                                if (OrderCarrier.IDOrderInvoice == 0)
                                                {
                                                    OrderCarrier.IDOrderInvoice = OrderInvoice.IDOrderInvoice;
                                                    if (OrderCarrier.IDCarrier == PsOrders.IDCarrier && string.IsNullOrWhiteSpace(OrderCarrier.TrackingNumber) && !string.IsNullOrWhiteSpace(tracking_number))
                                                    {
                                                        OrderCarrier.TrackingNumber = tracking_number;
                                                        trackingsend = true;
                                                        PsOrders.Tracking_Number = tracking_number;
                                                    }
                                                    PsOrderCarrierRepository.Save();

                                                    //UInt32 ID_Tax = 0;

                                                    //Model.Prestashop.PsCarrierTaxRulesGroupShopRepository PsCarrierTaxRulesGroupShopRepository = new Model.Prestashop.PsCarrierTaxRulesGroupShopRepository();
                                                    //if (PsCarrierTaxRulesGroupShopRepository.ExistCarrierShop(OrderCarrier.IDCarrier, Core.Global.CurrentShop.IDShop))
                                                    //{
                                                    //    #region Récupération taux de taxe transport
                                                    //    Model.Prestashop.PsCarrierTaxRulesGroupShop PsCarrierTaxRulesGroupShop = PsCarrierTaxRulesGroupShopRepository.ReadCarrierShop(OrderCarrier.IDCarrier, Core.Global.CurrentShop.IDShop);
                                                    //    Model.Prestashop.PsAddressRepository PsAddressRepository = new Model.Prestashop.PsAddressRepository();
                                                    //    Model.Prestashop.PsAddress PsAddress = PsAddressRepository.ReadAddress(PsOrders.IDAddressInvoice);
                                                    //    Model.Prestashop.PsTaxRuleRepository PsTaxRuleRepository = new Model.Prestashop.PsTaxRuleRepository();
                                                    //    if (PsTaxRuleRepository.ExistTaxeRulesGroupCountry((int)PsCarrierTaxRulesGroupShop.IDTaxRulesGroup, (int)PsAddress.IDCountry))
                                                    //    {
                                                    //        Model.Prestashop.PsTaxRule PsTaxRule = PsTaxRuleRepository.ReadTaxesRulesGroupCountry((int)PsCarrierTaxRulesGroupShop.IDTaxRulesGroup, (int)PsAddress.IDCountry);
                                                    //        ID_Tax = (uint)PsTaxRule.IDTax;
                                                    //        //Model.Prestashop.PsTaxRepository PsTaxRepository = new Model.Prestashop.PsTaxRepository();
                                                    //        //Model.Prestashop.PsTax PsTax = PsTaxRepository.ReadTax((uint)PsTaxRule.IDTax);
                                                    //    }
                                                    //    #endregion

                                                    //    #region OrderInvoiceTax (shipping)
                                                    //    if (ID_Tax != 0)
                                                    //    {
                                                    //        Model.Prestashop.PsOrderInvoiceTaxRepository PsOrderInvoiceTaxRepository = new Model.Prestashop.PsOrderInvoiceTaxRepository();
                                                    //        if (PsOrderInvoiceTaxRepository.ExistInvoiceTax(OrderInvoice.IDOrderInvoice, ID_Tax))
                                                    //        {
                                                    //            Model.Prestashop.PsOrderInvoiceTax OrderInvoiceTax = PsOrderInvoiceTaxRepository.ReadInvoiceTaxType(OrderInvoice.IDOrderInvoice, ID_Tax, Model.Prestashop.PsOrderInvoiceTaxRepository.Type.shipping);
                                                    //            OrderInvoiceTax.Amount += (OrderCarrier.ShippingCostTaxInCl != null && OrderCarrier.ShippingCostTaxExCl != null)
                                                    //                    ? (decimal)OrderCarrier.ShippingCostTaxInCl - (decimal)OrderCarrier.ShippingCostTaxExCl : 0;
                                                    //            PsOrderInvoiceTaxRepository.Save();
                                                    //        }
                                                    //        else
                                                    //        {
                                                    //            Model.Prestashop.PsOrderInvoiceTax OrderInvoiceTax = new Model.Prestashop.PsOrderInvoiceTax()
                                                    //            {
                                                    //                IDOrderInvoice = (int)OrderInvoice.IDOrderInvoice,
                                                    //                Amount = (OrderCarrier.ShippingCostTaxInCl != null && OrderCarrier.ShippingCostTaxExCl != null)
                                                    //                    ? (decimal)OrderCarrier.ShippingCostTaxInCl - (decimal)OrderCarrier.ShippingCostTaxExCl : 0,
                                                    //                IDTax = (int)ID_Tax,
                                                    //                Type = Model.Prestashop.PsOrderInvoiceTaxRepository.Type.shipping.ToString()
                                                    //            };
                                                    //            PsOrderInvoiceTaxRepository.Add(OrderInvoiceTax);
                                                    //        }
                                                    //    }
                                                    //    #endregion
                                                    //}
                                                }
                                            }
                                            #endregion

                                            #region OrderInvoiceTax

                                            // TODO étudier le contenu à générer
                                            // si autre en plus de "shipping"
                                            #endregion

                                            #region OrderCartRule

                                            Model.Prestashop.PsOrderCartRuleRepository PsOrderCartRuleRepository = new Model.Prestashop.PsOrderCartRuleRepository();
                                            foreach (Model.Prestashop.PsOrderCartRule OrderCartRule in PsOrderCartRuleRepository.ListOrder(PsOrders.IDOrder))
                                            {
                                                if (OrderCartRule.IDOrderInvoice == 0)
                                                {
                                                    OrderCartRule.IDOrderInvoice = OrderInvoice.IDOrderInvoice;
                                                    PsOrderCartRuleRepository.Save();
                                                }
                                            }
                                            #endregion

                                            #region OrderDetail

                                            Model.Prestashop.PsOrderDetailRepository PsOrderDetailRepository = new Model.Prestashop.PsOrderDetailRepository();
                                            foreach (Model.Prestashop.PsOrderDetail OrderDetail in PsOrderDetailRepository.ListOrder(PsOrders.IDOrder))
                                            {
                                                if (OrderDetail.IDOrderInvoice == 0)
                                                {
                                                    OrderDetail.IDOrderInvoice = (int)OrderInvoice.IDOrderInvoice;
                                                    PsOrderDetailRepository.Save();
                                                }
                                            }
                                            #endregion

                                            PsOrders.InvoiceNumber = Convert.ToUInt32(IdInvoice);
                                            PsOrders.InvoiceDate = (F_DOCENTETE.DO_Date != null) ? F_DOCENTETE.DO_Date.Value : DateTime.Now.Date;
                                            PsOrdersRepository.Save();
                                        }
                                        // <JG> 10/02/2015 ajout option remplacement forcé du numéro de facture
                                        else if (IdInvoice != string.Empty
                                            && Core.Global.IsInteger(IdInvoice)
                                            && PsOrderInvoiceRepository.ExistOrder(PsOrders.IDOrder)
                                            && Core.Global.GetConfig().CommandeNumeroFactureSageForceActif)
                                        {
                                            Model.Prestashop.PsOrderInvoice PsOrderInvoice = PsOrderInvoiceRepository.ReadOrder(PsOrders.IDOrder);
                                            PsOrderInvoice.Number = Convert.ToInt32(IdInvoice);
                                            PsOrderInvoice.DateAdd = (DateTime)F_DOCENTETE.DO_Date;
                                            if (PsOrderInvoice.DeliveryDate == null)
                                                PsOrderInvoice.DeliveryDate = (F_DOCENTETE.DO_DateLivr != null) ? F_DOCENTETE.DO_DateLivr : null;
                                            try
                                            {
                                                PsOrderInvoice.DeliveryNumber = (int)PsOrders.DeliveryNumber;
                                            }
                                            catch (Exception ex) { Core.Error.SendMailError("ExecLocalToDistant-DeliveryNumber" + ex.ToString()); }
                                            PsOrderInvoiceRepository.Save();

                                            PsOrders.InvoiceNumber = Convert.ToUInt32(IdInvoice);
                                            PsOrders.InvoiceDate = (F_DOCENTETE.DO_Date != null) ? F_DOCENTETE.DO_Date.Value : DateTime.Now.Date;
                                            PsOrdersRepository.Save();
                                        }
                                    }
                                    else
                                    {
                                        PsOrdersRepository.Save();
                                    }
                                    #endregion

                                    if (!trackingsend)
                                    {
                                        string tracking_number = ReadTrackingNumber(PsOrders, F_DOCENTETE, F_DOCENTETERepository);
                                        Model.Prestashop.PsOrderCarrierRepository PsOrderCarrierRepository = new Model.Prestashop.PsOrderCarrierRepository();
                                        foreach (Model.Prestashop.PsOrderCarrier OrderCarrier in PsOrderCarrierRepository.ListOrder(PsOrders.IDOrder))
                                        {
                                            if (OrderCarrier.IDCarrier == PsOrders.IDCarrier && string.IsNullOrWhiteSpace(OrderCarrier.TrackingNumber) && !string.IsNullOrWhiteSpace(tracking_number))
                                            {
                                                OrderCarrier.TrackingNumber = tracking_number;
                                                PsOrderCarrierRepository.Save();
                                                trackingsend = true;
                                                PsOrders.Tracking_Number = tracking_number;
                                            }
                                        }
                                    }

                                    try
                                    {
                                        this.SendMailOrder(F_DOCENTETE, PsOrders);
                                    }
                                    catch (Exception ex)
                                    {
                                        Core.Error.SendMailError(ex.ToString());
                                    }
                                }
                            }
                        }
                        if (!trackingsend)
                        {
                            string tracking_number = ReadTrackingNumber(PsOrders, F_DOCENTETE, F_DOCENTETERepository);
                            Model.Prestashop.PsOrderCarrierRepository PsOrderCarrierRepository = new Model.Prestashop.PsOrderCarrierRepository();
                            foreach (Model.Prestashop.PsOrderCarrier OrderCarrier in PsOrderCarrierRepository.ListOrder(PsOrders.IDOrder))
                            {
                                if (OrderCarrier.IDCarrier == PsOrders.IDCarrier && string.IsNullOrWhiteSpace(OrderCarrier.TrackingNumber) && !string.IsNullOrWhiteSpace(tracking_number))
                                {
                                    OrderCarrier.TrackingNumber = tracking_number;
                                    PsOrderCarrierRepository.Save();
                                    trackingsend = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        private string ReadTrackingNumber(Model.Prestashop.PsOrders PsOrders, Model.Sage.F_DOCENTETE F_DOCENTETE, Model.Sage.F_DOCENTETERepository F_DOCENTETERepository)
        {
            string value = string.Empty;
            try
            {
                if (Core.Global.GetConfig().CommandeTrackingEnteteActif)
                {
                    switch (Core.Global.GetConfig().CommandeTrackingEntete)
                    {
                        case PRESTACONNECT.Core.Parametres.FieldDocumentEntete.Entete1:
                            if (!string.IsNullOrWhiteSpace(F_DOCENTETE.DO_Coord01))
                                value = F_DOCENTETE.DO_Coord01;
                            break;
                        case PRESTACONNECT.Core.Parametres.FieldDocumentEntete.Entete2:
                            if (!string.IsNullOrWhiteSpace(F_DOCENTETE.DO_Coord02))
                                value = F_DOCENTETE.DO_Coord02;
                            break;
                        case PRESTACONNECT.Core.Parametres.FieldDocumentEntete.Entete3:
                            if (!string.IsNullOrWhiteSpace(F_DOCENTETE.DO_Coord03))
                                value = F_DOCENTETE.DO_Coord03;
                            break;
                        case PRESTACONNECT.Core.Parametres.FieldDocumentEntete.Entete4:
                            if (!string.IsNullOrWhiteSpace(F_DOCENTETE.DO_Coord04))
                                value = F_DOCENTETE.DO_Coord04;
                            break;
                    }

                }
                else if (Core.Global.GetConfig().CommandeTrackingInfolibreActif)
                {
                    if (new Model.Sage.cbSysLibreRepository().ExistInformationLibre(Core.Global.GetConfig().CommandeTrackingInfolibre, Model.Sage.cbSysLibreRepository.CB_File.F_DOCENTETE)
                        && F_DOCENTETERepository.ExistDocumentInformationLibreText(Core.Global.GetConfig().CommandeTrackingInfolibre, F_DOCENTETE.DO_Domaine.Value, F_DOCENTETE.DO_Type.Value, F_DOCENTETE.DO_Piece))
                    {
                        value = F_DOCENTETERepository.ReadDocumentInformationLibreText(Core.Global.GetConfig().CommandeTrackingInfolibre, F_DOCENTETE.DO_Domaine.Value, F_DOCENTETE.DO_Type.Value, F_DOCENTETE.DO_Piece);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
            return value;
        }

        private void SendMailOrder(Model.Sage.F_DOCENTETE F_DOCENTETE, Model.Prestashop.PsOrders PsOrders)
        {
            SynchronisationCommande.SendMail(Convert.ToInt32(F_DOCENTETE.DO_Type), PsOrders);
        }

        public static void SendMail(int type, Model.Prestashop.PsOrders PsOrders)
        {
            try
            {
                if (Core.Global.GetConfig().ConfigMailActive)
                {

                    String User = Core.Global.GetConfig().ConfigMailUser;
                    String Password = Core.Global.GetConfig().ConfigMailPassword;
                    String SMTP = Core.Global.GetConfig().ConfigMailSMTP;
                    Int32 Port = Core.Global.GetConfig().ConfigMailPort;
                    Boolean isSSL = Core.Global.GetConfig().ConfigMailSSL;

                    Model.Local.OrderMailRepository OrderMailRepository = new Model.Local.OrderMailRepository();
                    if (OrderMailRepository.ExistType(type))
                    {
                        Model.Local.OrderMail OrderMail = OrderMailRepository.ReadType(type);
                        // <JG> 05/12/2014 correction test type de mail actif
                        if (OrderMail.OrdMai_Active)
                        {
                            OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderId, PsOrders.IDOrder.ToString());
                            OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderTotalPaidHT, PsOrders.TotalPaidTaxExCl.ToString());
                            OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderTotalPaidTTC, PsOrders.TotalPaidTaxInCl.ToString());
                            OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderDate, PsOrders.DateAdd.ToString());
                            if (!string.IsNullOrEmpty(PsOrders.Cart_URL))
                                OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderCartLink, PsOrders.Cart_URL);

                            Model.Prestashop.PsCustomerRepository PsCustomerRepository = new Model.Prestashop.PsCustomerRepository();
                            if (PsCustomerRepository.ExistCustomer(PsOrders.IDCustomer))
                            {
                                Model.Prestashop.PsCustomer PsCustomer = PsCustomerRepository.ReadCustomer(PsOrders.IDCustomer);
                                OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderLastName, PsCustomer.LastName);
                                OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderFirstName, PsCustomer.FirstName);

                                OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailAccountFirstName, PsCustomer.FirstName);
                                OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailAccountLastName, PsCustomer.LastName);
                                OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailAccountCompany, PsCustomer.Company);
                                OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailInvoiceNumbers, PsOrders.Mail_Invoice_numbers);
                                // TODO Générer tracking link
                                Model.Prestashop.PsCarrierRepository PsCarrierRepository = new Model.Prestashop.PsCarrierRepository();
                                Model.Prestashop.PsCarrier PsCarrier = PsCarrierRepository.ReadCarrier(PsOrders.IDCarrier);
                                string url = PsCarrier.URL.Replace("@", PsOrders.Tracking_Number);
                                OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderTrackingLink, url);

                                Model.Prestashop.PsAddressRepository PsAddressRepository = new Model.Prestashop.PsAddressRepository();
                                if (PsAddressRepository.ExistAddress(PsOrders.IDAddressDelivery))
                                {
                                    Model.Prestashop.PsAddress PsAddress = PsAddressRepository.ReadAddress(PsOrders.IDAddressDelivery);
                                    OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderAddress, PsAddress.Address1 + " " + PsAddress.Address2);
                                    OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderAddress1, PsAddress.Address1);
                                    OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderAddress2, PsAddress.Address2);
                                    OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderPostCode, PsAddress.PostCode);
                                    OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderCity, PsAddress.City);

                                    Model.Prestashop.PsCountryLangRepository PsCountryLangRepository = new Model.Prestashop.PsCountryLangRepository();
                                    if (PsCountryLangRepository.ExistCountryLang(PsAddress.IDCountry, Core.Global.Lang))
                                    {
                                        OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderCountry, PsCountryLangRepository.ReadCountryLang(PsAddress.IDCountry, Core.Global.Lang).Name);
                                    }
                                    OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderPhone, PsAddress.Phone);
                                    OrderMail.OrdMai_Content = OrderMail.OrdMai_Content.Replace(Core.Global.MailOrderMobile, PsAddress.PhoneMobile);
                                }

                                MailMessage ObjMessage = new MailMessage();
                                MailAddress ObjAdrExp = new MailAddress(User);
                                MailAddress ObjAdrRec = new MailAddress(PsCustomer.Email);
                                SmtpClient ObjSmtpClient = new SmtpClient(SMTP, Port);

                                ObjMessage.From = ObjAdrExp;
                                ObjMessage.To.Add(ObjAdrRec);
                                ObjMessage.Subject = OrderMail.OrdMai_Header;
                                ObjMessage.Body = OrderMail.OrdMai_Content;
                                ObjMessage.IsBodyHtml = true;
                                ObjSmtpClient.EnableSsl = isSSL;
                                ObjSmtpClient.Credentials = new NetworkCredential(User, Password);
                                ObjSmtpClient.Send(ObjMessage);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void InsertLigneCommentaire(ABSTRACTION_SAGE.ALTERNETIS.Connexion Connexion, ABSTRACTION_SAGE.F_DOCENTETE.Obj ObjF_DOCENTETE, String Commentaire, Int32 DL_Ligne, string Glossaire)
        {
            ABSTRACTION_SAGE.F_DOCLIGNE.Obj ObjLigne = new ABSTRACTION_SAGE.F_DOCLIGNE.Obj();
            ObjLigne.DO_Domaine = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DO_Domaine)(short)ObjF_DOCENTETE.DO_Domaine;
            ObjLigne.DO_Type = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DO_Type)(short)ObjF_DOCENTETE.DO_Type;
            ObjLigne.DO_Piece = ObjF_DOCENTETE.DO_Piece;
            ObjLigne.DL_Ligne = DL_Ligne;
            ObjLigne.DE_No = ObjF_DOCENTETE.DE_No;
            ObjLigne.CT_Num = ObjF_DOCENTETE.DO_Tiers;

            ObjLigne.DO_Date = ObjF_DOCENTETE.DO_Date;
            ObjLigne.DO_Ref = ObjF_DOCENTETE.DO_Ref;
            ObjLigne.DL_Qte = 0;
            ObjLigne.EU_Qte = 0;
            ObjLigne.DL_Valorise = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_Valorise.Ligne_Non_Valorisee;
            ObjLigne.DL_Design = (Commentaire.Length <= 69) ? Commentaire : Commentaire.Substring(0, 69);

            if (!string.IsNullOrWhiteSpace(Glossaire))
            {
                ABSTRACTION_SAGE.F_DOCLIGNETEXT.Obj F_DOCLIGNETEXT = new ABSTRACTION_SAGE.F_DOCLIGNETEXT.Obj();
                F_DOCLIGNETEXT.DT_Text = Glossaire;
                F_DOCLIGNETEXT.Add(Connexion);

                Model.Sage.F_DOCLIGNETEXTRepository F_DOCLIGNETEXTRepository = new Model.Sage.F_DOCLIGNETEXTRepository();
                Model.Sage.F_DOCLIGNETEXT F_DOCLIGNETEXT_SQL = F_DOCLIGNETEXTRepository.ReadLast(Glossaire);
                if (F_DOCLIGNETEXT_SQL == null)
                    F_DOCLIGNETEXT_SQL = F_DOCLIGNETEXTRepository.ReadLastODBC();
                ObjLigne.DT_No = (F_DOCLIGNETEXT_SQL.DT_No != null) ? F_DOCLIGNETEXT_SQL.DT_No.Value : 0;
            }

            ObjLigne.AddCommentaire(Connexion);
        }

        private List<string> DivisionMessageClientEnMultiCommentaire(string message)
        {
            List<string> message_lignes = message.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<string> result = new List<string>();
            int depart, line_length, limit = 69;
            string line;
            foreach (string message_ligne in message_lignes)
            {
                depart = 0;
                while (depart < message_ligne.Length)
                {
                    if (depart < (message_ligne.Length - limit))
                    {
                        // on prend le morceau de la chaine de notre point de départ à notre limite
                        line = message_ligne.Substring(depart, limit);

                        line_length = Core.Global.MaxIndexOfPonctuation(line);
                        if (line_length <= 1)
                            line_length = limit / 2;

                        line = line.Substring(0, line_length);

                        depart += line_length;
                    }
                    else
                    {
                        line = message_ligne.Substring(depart);
                        depart += line.Length;
                    }
                    result.Add(line);
                }
            }
            return result;
        }

        private void SendMailErrorCommande(Model.Prestashop.PsOrders PsOrder, Model.Sage.F_DOCENTETE F_DOCENTETE, Decimal TotalSage)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(Core.Global.GetConfig().AdminMailAddress) && Core.Global.IsMailAddress(Core.Global.GetConfig().AdminMailAddress, Parametres.RegexMail.lvl08_lUdS))
                {

                    string sujet = "Synchronisation à vérifier : commande n°" + PsOrder.IDOrder;

                    StringBuilder Body = new StringBuilder();
                    Body.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">");
                    Body.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
                    Body.Append("<head>");
                    Body.Append("<meta http-equiv=\"content-type\" content=\"text/html\"; charset=UTF-8\" />");
                    Body.Append("<title>Contrôle de commande n°" + PsOrder.IDOrder + "</title>");
                    Body.Append("</head>");
                    Body.Append("<body>");
                    Body.Append("<br />");
                    Body.Append("La synchronisation de la commande n°" + PsOrder.IDOrder + " nécessite d'être vérifiée.");
                    Body.Append("<br />");
                    Body.Append("<br />");
                    Body.Append("<ul>");
                    Body.Append("<li>" + "Numéro de commande PrestaShop : " + PsOrder.IDOrder + "</li>");
                    Body.Append("<li>" + "Numéro de document Sage : " + F_DOCENTETE.DO_Piece + "</li>");
                    Body.Append("<li>" + "Montant total PrestaShop (TTC) : " + PsOrder.TotalPaidTaxInCl.ToString("N2") + "</li>");
                    Body.Append("<li>" + "Montant total Sage (TTC) : " + TotalSage.ToString("N2") + "</li>");
                    Body.Append("</ul>");
                    Body.Append("<br />");
                    Body.Append("</body>");
                    Body.Append("</html>");

                    Core.Error.SendMailGoogle(sujet, Body.ToString(), Core.Global.GetConfig().AdminMailAddress);
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
        private void SendMailErrorCommandeRecordIsLock(ABSTRACTION_SAGE.F_DOCENTETE.Obj F_DOCENTETE)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(Core.Global.GetConfig().AdminMailAddress) && Core.Global.IsMailAddress(Core.Global.GetConfig().AdminMailAddress, Parametres.RegexMail.lvl08_lUdS))
                {

                    string sujet = "Ecriture commande Sage interrompue : " + F_DOCENTETE.DO_Piece;

                    StringBuilder Body = new StringBuilder();
                    Body.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">");
                    Body.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
                    Body.Append("<head>");
                    Body.Append("<meta http-equiv=\"content-type\" content=\"text/html\"; charset=UTF-8\" />");
                    Body.Append("<title>Ecriture commande Sage interrompue : " + F_DOCENTETE.DO_Piece + "</title>");
                    Body.Append("</head>");
                    Body.Append("<body>");
                    Body.Append("<br />");
                    Body.Append("La synchronisation de la commande Prestashop n°" + F_DOCENTETE.DO_NoWeb + " vers le document Sage " + F_DOCENTETE.DO_Piece + " a été interrompue par l'ouverture de celui-ci par un utilisateur !");
                    Body.Append("<br />");
                    Body.Append("Si le statut de la commande Prestashop reste inchangé, la prochaine synchronisation commande de PrestaConnect écrira un nouveau document !");
                    Body.Append("<br />");
                    Body.Append("Vous pourrez alors supprimer le document " + F_DOCENTETE.DO_Piece + " de Sage qui est incomplet.");
                    Body.Append("<br />");
                    Body.Append("</body>");
                    Body.Append("</html>");

                    Core.Error.SendMailGoogle(sujet, Body.ToString(), Core.Global.GetConfig().AdminMailAddress);
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void SendMailErrorReglement(Model.Prestashop.PsOrders PsOrder, ABSTRACTION_SAGE.F_DOCENTETE.Obj F_DOCENTETE, Model.Prestashop.PsOrderPayment PsOrderPayment)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(Core.Global.GetConfig().AdminMailAddress) && Core.Global.IsMailAddress(Core.Global.GetConfig().AdminMailAddress, Parametres.RegexMail.lvl08_lUdS))
                {
                    string sujet = "Synchronisation règlement à vérifier : commande n°" + PsOrder.IDOrder;

                    StringBuilder Body = new StringBuilder();
                    Body.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">");
                    Body.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
                    Body.Append("<head>");
                    Body.Append("<meta http-equiv=\"content-type\" content=\"text/html\"; charset=UTF-8\" />");
                    Body.Append("<title>Contrôle de commande n°" + PsOrder.IDOrder + "</title>");
                    Body.Append("</head>");
                    Body.Append("<body>");
                    Body.Append("<br />");
                    Body.Append("Le règlement"
                        + ((!String.IsNullOrWhiteSpace(PsOrderPayment.TransactionID)) ? " référence " + PsOrderPayment.TransactionID : "")
                        + " de la commande n°" + PsOrder.IDOrder + " nécessite d'être vérifié.");
                    Body.Append("<br />");
                    Body.Append("<br />");
                    Body.Append("<ul>");
                    Body.Append("<li>" + "Numéro de commande PrestaShop : " + PsOrder.IDOrder + "</li>");
                    Body.Append("<li>" + "Numéro de document Sage : " + F_DOCENTETE.DO_Piece + "</li>");
                    Body.Append("</ul>");
                    Body.Append("<br />");
                    Body.Append("Montant du paiement PrestaShop (indicatif) : " + PsOrderPayment.Amount.ToString("N2"));
                    Body.Append("<br />");
                    Body.Append("<i>Pour la valeur exacte et les informations complètes du paiement veuillez vous reporter à votre back-office PrestaShop</i>");
                    Body.Append("<br />");
                    Body.Append("</body>");
                    Body.Append("</html>");

                    Core.Error.SendMailGoogle(sujet, Body.ToString(), Core.Global.GetConfig().AdminMailAddress);

                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
        private void SendMailErrorReglement(Model.Prestashop.PsOrders PsOrder, ABSTRACTION_SAGE.F_DOCENTETE.Obj F_DOCENTETE, Model.Prestashop.PsOrderPayment PsOrderPayment, String CodeJournal)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(Core.Global.GetConfig().AdminMailAddress) && Core.Global.IsMailAddress(Core.Global.GetConfig().AdminMailAddress, Parametres.RegexMail.lvl08_lUdS))
                {

                    string sujet = "Synchronisation règlement à vérifier : commande n°" + PsOrder.IDOrder;

                    StringBuilder Body = new StringBuilder();
                    Body.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">");
                    Body.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
                    Body.Append("<head>");
                    Body.Append("<meta http-equiv=\"content-type\" content=\"text/html\"; charset=UTF-8\" />");
                    Body.Append("<title>Contrôle de commande n°" + PsOrder.IDOrder + "</title>");
                    Body.Append("</head>");
                    Body.Append("<body>");
                    Body.Append("<br />");
                    Body.Append("Le journal " + CodeJournal + " n'a pas pu être affecté au règlement"
                        + ((!String.IsNullOrWhiteSpace(PsOrderPayment.TransactionID)) ? " référence " + PsOrderPayment.TransactionID : "")
                        + " de la commande n°" + PsOrder.IDOrder + ", le journal sera donc celui par défaut.");
                    Body.Append("<br />");
                    Body.Append("<br />");
                    Body.Append("<ul>");
                    Body.Append("<li>" + "Numéro de commande PrestaShop : " + PsOrder.IDOrder + "</li>");
                    Body.Append("<li>" + "Numéro de document Sage : " + F_DOCENTETE.DO_Piece + "</li>");
                    Body.Append("</ul>");
                    Body.Append("<br />");
                    Body.Append("Montant du paiement PrestaShop (indicatif) : " + PsOrderPayment.Amount.ToString("N2"));
                    Body.Append("<br />");
                    Body.Append("<i>Pour la valeur exacte et les informations complètes du paiement veuillez vous reporter à votre back-office PrestaShop</i>");
                    Body.Append("<br />");
                    Body.Append("</body>");
                    Body.Append("</html>");

                    Core.Error.SendMailGoogle(sujet, Body.ToString(), Core.Global.GetConfig().AdminMailAddress);

                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void ExecOleaPromo(Model.Prestashop.PsOrders PsOrders, Model.Prestashop.PsOrderDetail PsOrderDetailExec, ABSTRACTION_SAGE.ALTERNETIS.Connexion Connexion, ABSTRACTION_SAGE.F_DOCENTETE.Obj ObjF_DOCENTETE, Int32 DL_Ligne, out Int32 DL_LIGNE_OUT)
        {
            if (Core.Global.GetConfig().ModuleOleaPromoActif && Core.Global.ExistOleaPromoModule())
            {
                String Log_Message = "";
                String Suffixe_Gratuit = Core.Global.GetConfig().ModuleOleaSuffixeGratuit;
                string IDOlea = "";
                Model.Prestashop.PsOrderCartRuleRepository PsOrderCartRuleRepository = new Model.Prestashop.PsOrderCartRuleRepository();
                if (PsOrderCartRuleRepository.ExistOrder(PsOrders.IDOrder))
                {
                    Model.Prestashop.PsCartRuleRepository PsCartRuleRepository = new Model.Prestashop.PsCartRuleRepository();
                    foreach (Model.Prestashop.PsOrderCartRule PsOrderCartRule in PsOrderCartRuleRepository.ListOrder(PsOrders.IDOrder))
                    {
                        Log_Message = string.Empty;
                        if (PsCartRuleRepository.ExistCartRule(PsOrderCartRule.IDCartRule))
                        {
                            Log_Message = "OP01 - Présence d'une règle de panier";
                            Model.Prestashop.PsCartRule PsCartRule = PsCartRuleRepository.ReadCartRule(PsOrderCartRule.IDCartRule);
                            if (!String.IsNullOrWhiteSpace(PsCartRule.Description) && PsCartRule.Description.Contains('='))
                            {
                                string ref_product = string.Empty;

                                #region Identification règle de promo OLEA + identification produit
                                int lastindex = PsCartRule.Description.LastIndexOf('=');
                                string ID = (PsCartRule.Description.Length > lastindex + 1) ? PsCartRule.Description.Substring(lastindex + 1) : string.Empty;
                                if (Core.Global.IsInteger(ID))
                                {
                                    IDOlea = ID;
                                    Log_Message = "OP02 - Récupération identifiant règle de promo OLEA : " + ID;
                                }
                                if (!String.IsNullOrWhiteSpace(IDOlea))
                                {
                                    uint IdOleaPromo = uint.Parse(IDOlea);
                                    Model.Prestashop.PsOleaPromoRepository PsOleaPromoRepository = new Model.Prestashop.PsOleaPromoRepository();
                                    if (PsOleaPromoRepository.ExistOleAPromo(IdOleaPromo))
                                    {
                                        Model.Prestashop.PsOleaPromo PsOleaPromo = PsOleaPromoRepository.ReadOleAPromo(IdOleaPromo);
                                        ref_product = PsOleaPromo.Name.Trim();
                                        Log_Message = "OP03 - Identification produit cible de la règle de promo OLEA : " + ref_product;
                                    }
                                    else
                                        Log_Message = "OP11 - Règle de promo OLEA n°" + IDOlea + " introuvable !";
                                }
                                #endregion

                                if (!String.IsNullOrWhiteSpace(ref_product))
                                {
                                    string AR_Ref = ref_product;

                                    Model.Prestashop.PsOrderDetailRepository PsOrderDetailRepository = new Model.Prestashop.PsOrderDetailRepository();
                                    Model.Sage.F_CONDITIONRepository F_CONDITION_Repository = new Model.Sage.F_CONDITIONRepository();

                                    if (!PsOrderDetailRepository.ExistOrderProduct(PsOrders.IDOrder, ref_product))
                                    {
                                        if (F_CONDITION_Repository.ExistReferenceConditionnement(ref_product + Suffixe_Gratuit))
                                        {
                                            Model.Sage.F_CONDITION F_CONDITION = F_CONDITION_Repository.ReadReferenceConditionnement(ref_product + Suffixe_Gratuit);
                                            AR_Ref = F_CONDITION.AR_Ref;
                                        }
                                        else if (F_CONDITION_Repository.ExistEnumereConditionnement(ref_product + Suffixe_Gratuit))
                                        {
                                            Model.Sage.F_CONDITION F_CONDITION = F_CONDITION_Repository.ReadEnumereConditionnement(ref_product + Suffixe_Gratuit);
                                            AR_Ref = F_CONDITION.AR_Ref;
                                        }
                                    }
                                    if (PsOrderDetailRepository.ExistOrderProduct(PsOrders.IDOrder, AR_Ref))
                                    {
                                        Model.Prestashop.PsOrderDetail PsOrderDetail = PsOrderDetailRepository.ReadOrderProduct(PsOrders.IDOrder, AR_Ref);
                                        Log_Message = "OP04 - Récupération ligne de commande Prestashop : " + AR_Ref;

                                        if (PsOrderDetail != null && PsOrderDetail.IDOrderDetail == PsOrderDetailExec.IDOrderDetail)
                                        {
                                            Model.Sage.F_ARTICLERepository F_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
                                            decimal qte = PsOrderCartRule.Value / PsOrderDetail.UnitPriceTaxInCl;
                                            decimal pu = PsOrderDetail.UnitPriceTaxInCl;
                                            decimal puht = PsOrderDetail.UnitPriceTaxExCl;

                                            if (!F_ARTICLERepository.ExistReference(ref_product + Suffixe_Gratuit))
                                            {
                                                if (F_CONDITION_Repository.ExistReferenceConditionnement(ref_product + Suffixe_Gratuit))
                                                {
                                                    Model.Sage.F_CONDITION F_CONDITION = F_CONDITION_Repository.ReadReferenceConditionnement(ref_product + Suffixe_Gratuit);
                                                    ref_product = F_CONDITION.AR_Ref;
                                                    qte *= F_CONDITION.EC_Quantite.Value;
                                                    puht /= F_CONDITION.EC_Quantite.Value;
                                                    pu /= F_CONDITION.EC_Quantite.Value;
                                                }
                                                else if (F_CONDITION_Repository.ExistEnumereConditionnement(ref_product + Suffixe_Gratuit))
                                                {
                                                    Model.Sage.F_CONDITION F_CONDITION = F_CONDITION_Repository.ReadEnumereConditionnement(ref_product + Suffixe_Gratuit);
                                                    ref_product = F_CONDITION.AR_Ref;
                                                    qte *= F_CONDITION.EC_Quantite.Value;
                                                    puht /= F_CONDITION.EC_Quantite.Value;
                                                    pu /= F_CONDITION.EC_Quantite.Value;
                                                }
                                            }

                                            if (F_ARTICLERepository.ExistReference(ref_product + Suffixe_Gratuit))
                                            {
                                                Log_Message = "OP05 - Identification article gratuit Sage : " + ref_product + Suffixe_Gratuit;
                                                #region ligne article
                                                Model.Sage.F_ARTICLE F_ARTICLE = new Model.Sage.F_ARTICLERepository().ReadReference(ref_product + Suffixe_Gratuit);
                                                ABSTRACTION_SAGE.F_DOCLIGNE.Obj ObjF_DOCLIGNE = new ABSTRACTION_SAGE.F_DOCLIGNE.Obj();
                                                ObjF_DOCLIGNE.CT_Num = ObjF_DOCENTETE.DO_Tiers;
                                                ObjF_DOCLIGNE.CO_No = ObjF_DOCENTETE.CO_No;

                                                ObjF_DOCLIGNE.DO_Domaine = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DO_Domaine)ObjF_DOCENTETE.DO_Domaine;
                                                ObjF_DOCLIGNE.DO_Type = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DO_Type)ObjF_DOCENTETE.DO_Type;
                                                ObjF_DOCLIGNE.DO_Piece = ObjF_DOCENTETE.DO_Piece;
                                                ObjF_DOCLIGNE.DO_Date = ObjF_DOCENTETE.DO_Date;
                                                ObjF_DOCLIGNE.DL_DateBC = ObjF_DOCLIGNE.DO_Date;
                                                ObjF_DOCLIGNE.DL_DateBL = ObjF_DOCLIGNE.DO_Date;
            									#if (SAGE_VERSION_18 || SAGE_VERSION_19 || SAGE_VERSION_20 || SAGE_VERSION_21)
                                                ObjF_DOCLIGNE.DL_DatePL = ObjF_DOCLIGNE.DO_Date;
                                                ObjF_DOCLIGNE.DL_DateDE = ObjF_DOCLIGNE.DO_Date;
                                                #endif

                                                ObjF_DOCLIGNE.DO_DateLivr = ObjF_DOCENTETE.DO_DateLivr;

                                                if (F_ARTICLE.AR_SuiviStock == 0)
                                                    ObjF_DOCLIGNE.DE_No = 0;
                                                else
                                                    ObjF_DOCLIGNE.DE_No = ObjF_DOCENTETE.DE_No;

                                                ObjF_DOCLIGNE.DL_Ligne = DL_Ligne;
                                                ObjF_DOCLIGNE.DL_Qte = qte;

                                                #region Taxes
                                                Core.Sync.SynchronisationArticle SyncArticle = new SynchronisationArticle();
                                                Model.Sage.F_TAXE F_TAXETVA = SyncArticle.ReadTaxe(F_ARTICLE, new Model.Prestashop.PsProduct(), ObjF_DOCENTETE.N_CatCompta);
                                                if (F_TAXETVA.TA_Taux != null)
                                                {
                                                    switch (Core.Global.GetConfig().TaxSageTVA)
                                                    {
                                                        case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe1:
                                                            ObjF_DOCLIGNE.DL_Taxe1 = (decimal)F_TAXETVA.TA_Taux;
                                                            ObjF_DOCLIGNE.DL_TypeTaux1 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux)F_TAXETVA.TA_TTaux;
                                                            ObjF_DOCLIGNE.DL_TypeTaxe1 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_TAXETVA.TA_Type;
															#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                                            ObjF_DOCLIGNE.DL_CodeTaxe1 = F_TAXETVA.TA_Code;
															#endif
                                                            break;
                                                        case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe2:
                                                            ObjF_DOCLIGNE.DL_Taxe2 = (decimal)F_TAXETVA.TA_Taux;
                                                            ObjF_DOCLIGNE.DL_TypeTaux2 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux)F_TAXETVA.TA_TTaux;
                                                            ObjF_DOCLIGNE.DL_TypeTaxe2 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_TAXETVA.TA_Type;
															#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                                            ObjF_DOCLIGNE.DL_CodeTaxe2 = F_TAXETVA.TA_Code;
															#endif
                                                            break;
                                                        case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe3:
                                                            ObjF_DOCLIGNE.DL_Taxe3 = (decimal)F_TAXETVA.TA_Taux;
                                                            ObjF_DOCLIGNE.DL_TypeTaux3 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux)F_TAXETVA.TA_TTaux;
                                                            ObjF_DOCLIGNE.DL_TypeTaxe3 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_TAXETVA.TA_Type;
															#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                                            ObjF_DOCLIGNE.DL_CodeTaxe3 = F_TAXETVA.TA_Code;
															#endif
                                                            break;
                                                        case PRESTACONNECT.Core.Parametres.TaxSage.Empty:
                                                        default:
                                                            break;
                                                    }
                                                }

                                                #endregion

                                                // <AM> 21/02/2014 Gratuit pour stock avec même article
                                                //ObjF_DOCLIGNE.DL_PUTTC = 0 - pu;
                                                ObjF_DOCLIGNE.DL_PUTTC = pu;
                                                //ObjF_DOCLIGNE.DL_PrixUnitaire = 0 - puht;
                                                ObjF_DOCLIGNE.DL_PrixUnitaire = puht;
                                                ObjF_DOCLIGNE.DL_Remise01REM_Type = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_RemiseREM_Type.Pourcentage;
                                                ObjF_DOCLIGNE.DL_Remise01REM_Valeur = 100;

                                                ObjF_DOCLIGNE.DL_TTC = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TTC.TTC;
                                                if (ObjF_DOCENTETE.DO_Devise > 0)
                                                    ObjF_DOCLIGNE.DL_PUDevise = ObjF_DOCLIGNE.DL_PrixUnitaire;


                                                ObjF_DOCLIGNE.DL_MontantHT = 0;
                                                ObjF_DOCLIGNE.DL_MontantTTC = 0;
                                                ObjF_DOCLIGNE.AR_Ref = F_ARTICLE.AR_Ref;
                                                ObjF_DOCLIGNE.DL_Design = (PsOrderDetail.ProductName.Length <= 69) ? PsOrderDetail.ProductName : PsOrderDetail.ProductName.Substring(0, 69);
                                                ObjF_DOCLIGNE.DL_No = 0;
                                                ObjF_DOCLIGNE.CA_Num = ObjF_DOCENTETE.CA_Num;
                                                ObjF_DOCLIGNE.DL_Escompte = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_Escompte.Soumis;
                                                ObjF_DOCLIGNE.DL_Valorise = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_Valorise.Ligne_Valorisee;
                                                ObjF_DOCLIGNE.EU_Qte = ObjF_DOCLIGNE.DL_Qte;
                                                ObjF_DOCLIGNE.EU_Enumere = F_ARTICLE.UniteVenteString;
                                                ObjF_DOCLIGNE.DL_QteBC = ObjF_DOCLIGNE.DL_Qte;

                                                #region Gestion poids

                                                //ObjF_DOCLIGNE.DL_PoidsNet = (decimal)PsOrderDetail.ProductWeight;
                                                //ObjF_DOCLIGNE.DL_PoidsBrut = (decimal)PsOrderDetail.ProductWeight;

                                                ObjF_DOCLIGNE.DL_PoidsNet = (F_ARTICLE.AR_PoidsNet != null) ? (decimal)F_ARTICLE.AR_PoidsNet * ObjF_DOCLIGNE.DL_Qte : 0;
                                                ObjF_DOCLIGNE.DL_PoidsBrut = (F_ARTICLE.AR_PoidsBrut != null) ? (decimal)F_ARTICLE.AR_PoidsBrut * ObjF_DOCLIGNE.DL_Qte : 0;

                                                if (F_ARTICLE.AR_UnitePoids != null)
                                                {
                                                    switch (F_ARTICLE.AR_UnitePoids.Value)
                                                    {
                                                        case 0:
                                                            ObjF_DOCLIGNE.DL_PoidsNet = ObjF_DOCLIGNE.DL_PoidsNet * 1000000;
                                                            ObjF_DOCLIGNE.DL_PoidsBrut = ObjF_DOCLIGNE.DL_PoidsBrut * 1000000;
                                                            break;
                                                        case 1:
                                                            ObjF_DOCLIGNE.DL_PoidsNet = ObjF_DOCLIGNE.DL_PoidsNet * 100000;
                                                            ObjF_DOCLIGNE.DL_PoidsBrut = ObjF_DOCLIGNE.DL_PoidsBrut * 100000;
                                                            break;
                                                        case 2:
                                                            ObjF_DOCLIGNE.DL_PoidsNet = ObjF_DOCLIGNE.DL_PoidsNet * 1000;
                                                            ObjF_DOCLIGNE.DL_PoidsBrut = ObjF_DOCLIGNE.DL_PoidsBrut * 1000;
                                                            break;
                                                        case 3:
                                                            ObjF_DOCLIGNE.DL_PoidsNet = ObjF_DOCLIGNE.DL_PoidsNet * 1;
                                                            ObjF_DOCLIGNE.DL_PoidsBrut = ObjF_DOCLIGNE.DL_PoidsBrut * 1;
                                                            break;
                                                        case 4:
                                                            ObjF_DOCLIGNE.DL_PoidsNet = ObjF_DOCLIGNE.DL_PoidsNet / 1000;
                                                            ObjF_DOCLIGNE.DL_PoidsBrut = ObjF_DOCLIGNE.DL_PoidsBrut / 1000;
                                                            break;
                                                    }
                                                }

                                                #endregion

                                                //Hard Coded
                                                ObjF_DOCLIGNE.DL_FactPoids = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_FactPoids.Non_Facture;
                                                if (F_ARTICLE.AR_Nomencl == 2)
                                                {
                                                    ObjF_DOCLIGNE.AR_RefCompose = F_ARTICLE.AR_Ref;
                                                }
                                                ObjF_DOCLIGNE.Add(Connexion);

                                                // ATTENTION MAJ DE LA LIGNE PRINCIPALE
                                                Connexion.Request = "UPDATE F_DOCLIGNE SET DL_QTE = DL_QTE - " + ObjF_DOCLIGNE.DL_Qte.ToString().Replace(',', '.') + " WHERE DO_DOMAINE = 0 AND DO_TYPE = 1 AND DO_PIECE = '" + ObjF_DOCLIGNE.DO_Piece + "' AND AR_REF = '" + ObjF_DOCLIGNE.AR_Ref + "' AND DL_LIGNE <> " + ObjF_DOCLIGNE.DL_Ligne.ToString();
                                                Connexion.Exec_Request();

                                                Log_Message = "OP06 - insertion article gratuit Sage : " + ref_product + Suffixe_Gratuit;
                                                DL_Ligne += 10000;
                                                #endregion
                                            }
                                            else
                                            {
                                                InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, "Article gratuit Sage introuvable : " + ref_product + Suffixe_Gratuit, DL_Ligne, null);
                                                DL_Ligne += 10000;
                                                InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, "Quantité gratuit(s) : " + qte.ToString() + " / Montant gratuit(s) : " + (0 - PsOrderCartRule.Value).ToString(), DL_Ligne, null);
                                                DL_Ligne += 10000;
                                                Log_Message = "OP13 - Article gratuit Sage introuvable : " + ref_product + Suffixe_Gratuit + " !";
                                            }

                                            // ajout dans liste temporaire pour éviter un doublon de ligne via la gestion des bons de réduction
                                            this.temp_order_cart_rule.Add(PsOrderCartRule.IDOrderCartRule);
                                        }
                                        else
                                            Log_Message = "";
                                    }
                                    //else
                                    //{
                                    //    Log_Message = "OP12 - Produit : " + ref_product + " introuvable dans la commande n°" + PsOrders.IDOrder + " !";
                                    //    InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, "Article de la règle de promo suivante non identifiable : ", DL_Ligne, null);
                                    //    DL_Ligne += 10000;
                                    //    InsertLigneCommentaire(Connexion, ObjF_DOCENTETE, ref_product, DL_Ligne, null);
                                    //    DL_Ligne += 10000;
                                    //}
                                }
                            }
                            else
                                Log_Message = "OP10 - Identifiant règle de promo OLEA introuvable dans la règle de panier !";


                            //if (!String.IsNullOrWhiteSpace(Log_Message))
                            //{
                            //    Core.Log.WriteLog(Log_Message, true);
                            //}
                        }
                    }
                }
            }
            DL_LIGNE_OUT = DL_Ligne;
        }

        private void ExecBonReduction(Model.Prestashop.PsOrders PsOrders, ABSTRACTION_SAGE.ALTERNETIS.Connexion Connexion, ABSTRACTION_SAGE.F_DOCENTETE.Obj ObjF_DOCENTETE, Boolean ValorisationTTC, Int32 DL_Ligne, out Int32 DL_LIGNE_OUT)
        {
            if (!string.IsNullOrWhiteSpace(Core.Global.GetConfig().CommandeArticleReduction)
                && new Model.Sage.F_ARTICLERepository().ExistReference(Core.Global.GetConfig().CommandeArticleReduction))
            {
                Boolean _isPrecommande = false;

                if (Core.Global.GetConfig().ModulePreorderActif)
                {
                    Model.Sage.cbSysLibreRepository cbSysLibreRepository = new Model.Sage.cbSysLibreRepository();
                    if (!string.IsNullOrEmpty(Core.Global.GetConfig().ModulePreorderInfolibreName)
                        && cbSysLibreRepository.ExistInformationLibre(Core.Global.GetConfig().ModulePreorderInfolibreName, Model.Sage.cbSysLibreRepository.CB_File.F_ARTICLE))
                    {
                        Model.Local.InformationLibreRepository InformationLibreRepository = new Model.Local.InformationLibreRepository();
                        if (InformationLibreRepository.ExistInfoLibre(Core.Global.GetConfig().ModulePreorderInfolibreName))
                        {
                            Model.Local.InformationLibre Infolibre = InformationLibreRepository.ReadInfoLibre(Core.Global.GetConfig().ModulePreorderInfolibreName);
                            Model.Prestashop.PsFeatureRepository PsFeatureRepository = new Model.Prestashop.PsFeatureRepository();
                            if (PsFeatureRepository.Exist((uint)Infolibre.Cha_Id))
                            {
                                Model.Prestashop.PsFeatureProductRepository PsFeatureProductRepository = new Model.Prestashop.PsFeatureProductRepository();
                                Model.Prestashop.PsFeatureValueLangRepository PsFeatureValueLangRepository = new Model.Prestashop.PsFeatureValueLangRepository();
                                string replace_value = Core.Global.SageValueReplacement(Core.Global.GetConfig().ModulePreorderInfolibreValue);

                                foreach (Model.Prestashop.PsOrderDetail PsOrderDetail in new Model.Prestashop.PsOrderDetailRepository().ListOrder(PsOrders.IDOrder))
                                {
                                    if (PsFeatureProductRepository.ExistFeatureProduct((uint)Infolibre.Cha_Id, PsOrderDetail.ProductID))
                                    {
                                        Model.Prestashop.PsFeatureProduct PsFeatureProduct = PsFeatureProductRepository.ReadFeatureProduct((uint)Infolibre.Cha_Id, PsOrderDetail.ProductID);
                                        if (PsFeatureValueLangRepository.ExistFeatureValueLang(PsFeatureProduct.IDFeatureValue, Core.Global.Lang))
                                        {
                                            Model.Prestashop.PsFeatureValueLang PsFeatureValueLang = PsFeatureValueLangRepository.ReadFeatureValueLang(PsFeatureProduct.IDFeatureValue, Core.Global.Lang);
                                            if (PsFeatureValueLang.Value == Core.Global.GetConfig().ModulePreorderInfolibreValue
                                                || PsFeatureValueLang.Value == replace_value)
                                            {
                                                _isPrecommande = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (_isPrecommande)
                {
                    // <JG> 13/06/2014 ajout saisie infolibre entete
                    #region Infolibre entete
                    try
                    {
                        Model.Sage.cbSysLibreRepository cbSysLibreRepository = new Model.Sage.cbSysLibreRepository();
                        if (cbSysLibreRepository.ExistInformationLibre(Core.Global.GetConfig().ModulePreorderInfolibreEnteteName, Model.Sage.cbSysLibreRepository.CB_File.F_DOCENTETE))
                        {
                            Model.Sage.cbSysLibre cbSysLibre = cbSysLibreRepository.ReadInformationLibre(Core.Global.GetConfig().ModulePreorderInfolibreEnteteName, Model.Sage.cbSysLibreRepository.CB_File.F_DOCENTETE);
                            if (cbSysLibre.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageText
                                || cbSysLibre.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageTable)
                            {
                                if (!String.IsNullOrEmpty(Core.Global.GetConfig().ModulePreorderInfolibreEnteteValue))
                                {
                                    if (Core.Global.GetConfig().ModulePreorderInfolibreEnteteValue.Length >= cbSysLibre.CB_Len)
                                        Core.Global.GetConfig().UpdateModulePreorderInfolibreEnteteValue(Core.Global.GetConfig().ModulePreorderInfolibreEnteteValue.Substring(0, cbSysLibre.CB_Len - 1));

                                    ObjF_DOCENTETE.InfosLibres = new ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Col();
                                    ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj info_abstraction = new ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj()
                                    {
                                        Len = cbSysLibre.CB_Len,
                                        Name = cbSysLibre.CB_Name,
                                        Pos = cbSysLibre.CB_Pos,
                                        Table = cbSysLibre.CB_File,
                                        Value = Core.Global.GetConfig().ModulePreorderInfolibreEnteteValue,
                                    };
                                    #region conversion cb_type
                                    switch ((Model.Sage.cbSysLibreRepository.CB_Type)cbSysLibre.CB_Type)
                                    {
                                        //case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageSmallDate:
                                        //    info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageSmallDate;
                                        //    break;
                                        //case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageValeur:
                                        //    info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageValeur;
                                        //    break;
                                        case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageText:
                                            info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageText;
                                            break;
                                        //case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageDate:
                                        //    info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageDate;
                                        //    break;
                                        //case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageMontant:
                                        //    info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageMontant;
                                        //    break;
                                        case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.SageTable:
                                            info_abstraction.Type = ABSTRACTION_SAGE.ALTERNETIS.InfoLibre.Obj._Enum_Type.SageTable;
                                            break;
                                        case PRESTACONNECT.Model.Sage.cbSysLibreRepository.CB_Type.Deleted:
                                        default:
                                            break;
                                    }
                                    #endregion
                                    ObjF_DOCENTETE.InfosLibres.Add(info_abstraction);
                                    ObjF_DOCENTETE.Update(Connexion);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Core.Error.SendMailError(ex.ToString());
                    }
                    #endregion
                }

                if (!_isPrecommande)
                {
                    String Log_Message = string.Empty;
                    Model.Prestashop.PsOrderCartRuleRepository PsOrderCartRuleRepository = new Model.Prestashop.PsOrderCartRuleRepository();
                    if (PsOrderCartRuleRepository.ExistOrder(PsOrders.IDOrder))
                    {
						#region Recherche des différents articles Sage devant remplacer des règles de panier PrestaShop
						Model.Local.OrderCartRuleRepository orderCartRuleRepository = new Model.Local.OrderCartRuleRepository();
						Model.Sage.F_ARTICLERepository f_ARTICLERepository = new Model.Sage.F_ARTICLERepository();
						Dictionary<int, string> listOrderCartRule = new Dictionary<int, string>();
						if (orderCartRuleRepository.List().Count > 0)
						{
							foreach (Model.Local.OrderCartRule orderCartRule in orderCartRuleRepository.List())
							{
								if (orderCartRule.Sag_id != null && orderCartRule.Sag_id >0 && f_ARTICLERepository.ExistArticle(orderCartRule.Sag_id ?? 0))
								listOrderCartRule.Add(orderCartRule.Pre_id, f_ARTICLERepository.ReadArticle(orderCartRule.Sag_id ?? 0).AR_Ref);
							}
						}
						#endregion

						Model.Prestashop.PsCartRuleRepository PsCartRuleRepository = new Model.Prestashop.PsCartRuleRepository();
                        List<Model.Prestashop.PsOrderCartRule> list = PsOrderCartRuleRepository.ListOrder(PsOrders.IDOrder);
                        // filtrage des règles de paniers de la commande par rapport aux règles déjà utilisées dans le module OleaPromo
                        list = list.Where(ocr => this.temp_order_cart_rule.Count(tcr => tcr == ocr.IDOrderCartRule) == 0).ToList();
                        foreach (Model.Prestashop.PsOrderCartRule PsOrderCartRule in list)
                        {
                            Log_Message = string.Empty;
                            //if (PsCartRuleRepository.ExistCartRule(PsOrderCartRule.IDCartRule))
                            {
								//Log_Message = "CPN01 - Présence d'une règle de panier";

								#region ligne article
								#region Recherche de l'article de réduction correspondant
								string ArticleReduction = Core.Global.GetConfig().CommandeArticleReduction;
								if (listOrderCartRule.Count > 0 && listOrderCartRule.ContainsKey((int)PsOrderCartRule.IDCartRule))
								{
									ArticleReduction = listOrderCartRule[(int)PsOrderCartRule.IDCartRule];
								}
								#endregion

								Model.Sage.F_ARTICLE F_ARTICLE = new Model.Sage.F_ARTICLERepository().ReadReference(ArticleReduction);
                                ABSTRACTION_SAGE.F_DOCLIGNE.Obj ObjF_DOCLIGNE = new ABSTRACTION_SAGE.F_DOCLIGNE.Obj();
                                ObjF_DOCLIGNE.CT_Num = ObjF_DOCENTETE.DO_Tiers;
                                ObjF_DOCLIGNE.CO_No = ObjF_DOCENTETE.CO_No;

                                ObjF_DOCLIGNE.DO_Domaine = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DO_Domaine)ObjF_DOCENTETE.DO_Domaine;
                                ObjF_DOCLIGNE.DO_Type = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DO_Type)ObjF_DOCENTETE.DO_Type;
                                ObjF_DOCLIGNE.DO_Piece = ObjF_DOCENTETE.DO_Piece;
                                ObjF_DOCLIGNE.DO_Date = ObjF_DOCENTETE.DO_Date;
                                ObjF_DOCLIGNE.DL_DateBC = ObjF_DOCLIGNE.DO_Date;
                                ObjF_DOCLIGNE.DL_DateBL = ObjF_DOCLIGNE.DO_Date;
            					#if (SAGE_VERSION_18 || SAGE_VERSION_19 || SAGE_VERSION_20 || SAGE_VERSION_21)
                                ObjF_DOCLIGNE.DL_DatePL = ObjF_DOCLIGNE.DO_Date;
                                ObjF_DOCLIGNE.DL_DateDE = ObjF_DOCLIGNE.DO_Date;
                                #endif

                                ObjF_DOCLIGNE.DO_DateLivr = ObjF_DOCENTETE.DO_DateLivr;

                                if (F_ARTICLE.AR_SuiviStock == 0)
                                    ObjF_DOCLIGNE.DE_No = 0;
                                else
                                    ObjF_DOCLIGNE.DE_No = ObjF_DOCENTETE.DE_No;

                                ObjF_DOCLIGNE.DL_Ligne = DL_Ligne;
                                ObjF_DOCLIGNE.DL_Qte = 1;

                                #region Taxes
                                Core.Sync.SynchronisationArticle SyncArticle = new SynchronisationArticle();
                                Model.Sage.F_TAXE F_TAXETVA = SyncArticle.ReadTaxe(F_ARTICLE, new Model.Prestashop.PsProduct(), ObjF_DOCENTETE.N_CatCompta);
                                if (F_TAXETVA.TA_Taux != null)
                                {
                                    switch (Core.Global.GetConfig().TaxSageTVA)
                                    {
                                        case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe1:
                                            ObjF_DOCLIGNE.DL_Taxe1 = (decimal)F_TAXETVA.TA_Taux;
                                            ObjF_DOCLIGNE.DL_TypeTaux1 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux)F_TAXETVA.TA_TTaux;
                                            ObjF_DOCLIGNE.DL_TypeTaxe1 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_TAXETVA.TA_Type;
											#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                            ObjF_DOCLIGNE.DL_CodeTaxe1 = F_TAXETVA.TA_Code;
											#endif
                                            break;
                                        case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe2:
                                            ObjF_DOCLIGNE.DL_Taxe2 = (decimal)F_TAXETVA.TA_Taux;
                                            ObjF_DOCLIGNE.DL_TypeTaux2 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux)F_TAXETVA.TA_TTaux;
                                            ObjF_DOCLIGNE.DL_TypeTaxe2 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_TAXETVA.TA_Type;
											#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                            ObjF_DOCLIGNE.DL_CodeTaxe2 = F_TAXETVA.TA_Code;
											#endif
                                            break;
                                        case PRESTACONNECT.Core.Parametres.TaxSage.CodeTaxe3:
                                            ObjF_DOCLIGNE.DL_Taxe3 = (decimal)F_TAXETVA.TA_Taux;
                                            ObjF_DOCLIGNE.DL_TypeTaux3 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaux)F_TAXETVA.TA_TTaux;
                                            ObjF_DOCLIGNE.DL_TypeTaxe3 = (ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TypeTaxe)F_TAXETVA.TA_Type;
											#if !(SAGE_VERSION_16 || SAGE_VERSION_17)
                                            ObjF_DOCLIGNE.DL_CodeTaxe3 = F_TAXETVA.TA_Code;
											#endif
                                            break;
                                        case PRESTACONNECT.Core.Parametres.TaxSage.Empty:
                                        default:
                                            break;
                                    }
                                }

                                #endregion

                                ObjF_DOCLIGNE.DL_PUTTC = 0 - PsOrderCartRule.Value;
                                ObjF_DOCLIGNE.DL_PrixUnitaire = (F_TAXETVA.TA_Taux != null)
                                    ? (0 - (PsOrderCartRule.Value / (1 + (F_TAXETVA.TA_Taux.Value / 100))))
                                    : (0 - PsOrderCartRule.Value);

                                ObjF_DOCLIGNE.DL_TTC = (ValorisationTTC)
                                    ? ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TTC.TTC
                                    : ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_TTC.HT;

                                if (ObjF_DOCENTETE.DO_Devise > 0)
                                    ObjF_DOCLIGNE.DL_PUDevise = ObjF_DOCLIGNE.DL_PrixUnitaire;

                                ObjF_DOCLIGNE.DL_MontantHT = 0;
                                ObjF_DOCLIGNE.DL_MontantTTC = 0;
                                ObjF_DOCLIGNE.AR_Ref = F_ARTICLE.AR_Ref;
                                ObjF_DOCLIGNE.DL_Design = (PsOrderCartRule.Name.Length > 69) ? PsOrderCartRule.Name.Substring(0, 69) : PsOrderCartRule.Name;
                                ObjF_DOCLIGNE.DL_No = 0;
                                ObjF_DOCLIGNE.CA_Num = ObjF_DOCENTETE.CA_Num;
                                ObjF_DOCLIGNE.DL_Escompte = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_Escompte.Soumis;
                                ObjF_DOCLIGNE.DL_Valorise = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_Valorise.Ligne_Valorisee;
                                ObjF_DOCLIGNE.EU_Qte = ObjF_DOCLIGNE.DL_Qte;
                                ObjF_DOCLIGNE.EU_Enumere = F_ARTICLE.UniteVenteString;
                                ObjF_DOCLIGNE.DL_QteBC = ObjF_DOCLIGNE.DL_Qte;

                                #region Gestion poids

                                ObjF_DOCLIGNE.DL_PoidsNet = (F_ARTICLE.AR_PoidsNet != null) ? (decimal)F_ARTICLE.AR_PoidsNet : 0;
                                ObjF_DOCLIGNE.DL_PoidsBrut = (F_ARTICLE.AR_PoidsBrut != null) ? (decimal)F_ARTICLE.AR_PoidsBrut : 0;

                                #endregion

                                //Hard Coded
                                ObjF_DOCLIGNE.DL_FactPoids = ABSTRACTION_SAGE.F_DOCLIGNE.Obj._Enum_DL_FactPoids.Non_Facture;
                                if (F_ARTICLE.AR_Nomencl == 2)
                                {
                                    ObjF_DOCLIGNE.AR_RefCompose = F_ARTICLE.AR_Ref;
                                }
                                ObjF_DOCLIGNE.Add(Connexion);

                                DL_Ligne += 10000;
                                #endregion

                                if (!String.IsNullOrWhiteSpace(Log_Message))
                                {
                                    Core.Log.WriteLog(Log_Message, true);
                                }
                            }
                        }
                    }
                }
            }
            DL_LIGNE_OUT = DL_Ligne;
        }
    }
}
