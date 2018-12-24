using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using PRESTACONNECT.Model.Internal;

namespace PRESTACONNECT.Contexts
{
    internal sealed class ConfigurationContext : Context
    {
        #region Properties

        private const string NameSample = "DUPONT-MARTIN";
        private const string NumSample = "0034567890123456789";
        private const string DepSample = "86";

        #region Param

        private Model.Local.GroupRepository GroupRepository { get; set; }
        private Model.Local.CountryRepository CountryRepository { get; set; }

        private BackgroundWorker loadGroupsWorker;
        private BackgroundWorker LoadGroupsWorker
        {
            get { return loadGroupsWorker; }
            set
            {
                if (loadGroupsWorker != null)
                {
                    loadGroupsWorker.DoWork -= new DoWorkEventHandler(LoadGroupsWorker_DoWork);
                    loadGroupsWorker.ProgressChanged -= new ProgressChangedEventHandler(LoadGroupsWorker_ProgressChanged);
                    loadGroupsWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(LoadGroupsWorker_RunWorkerCompleted);
                }

                loadGroupsWorker = value;

                if (loadGroupsWorker != null)
                {
                    loadGroupsWorker.DoWork += new DoWorkEventHandler(LoadGroupsWorker_DoWork);
                    loadGroupsWorker.ProgressChanged += new ProgressChangedEventHandler(LoadGroupsWorker_ProgressChanged);
                    loadGroupsWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadGroupsWorker_RunWorkerCompleted);
                }
            }
        }

        private BackgroundWorker loadConfigWorker;
        private BackgroundWorker LoadConfigWorker
        {
            get { return loadConfigWorker; }
            set
            {
                if (loadConfigWorker != null)
                {
                    loadConfigWorker.DoWork -= new DoWorkEventHandler(LoadConfigWorker_DoWork);
                    loadConfigWorker.ProgressChanged -= new ProgressChangedEventHandler(LoadConfigWorker_ProgressChanged);
                    loadConfigWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(LoadConfigWorker_RunWorkerCompleted);
                }

                loadConfigWorker = value;

                if (loadConfigWorker != null)
                {
                    loadConfigWorker.DoWork += new DoWorkEventHandler(LoadConfigWorker_DoWork);
                    loadConfigWorker.ProgressChanged += new ProgressChangedEventHandler(LoadConfigWorker_ProgressChanged);
                    loadConfigWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadConfigWorker_RunWorkerCompleted);
                }
            }
        }

        private ObservableCollection<ConfigurationGroup> listGroup;
        public ObservableCollection<ConfigurationGroup> ListGroup
        {
            get { return listGroup; }
            set
            {
                listGroup = value;
                OnPropertyChanged("ListGroup");
            }
        }

        private ConfigurationGroup selectedGroup;
        public ConfigurationGroup SelectedGroup
        {
            get { return selectedGroup; }
            set { selectedGroup = value; OnPropertyChanged("SelectedGroup"); }
        }

        private ObservableCollection<Model.Sage.P_CATTARIF> listCategoriesTarifaires;
        public ObservableCollection<Model.Sage.P_CATTARIF> ListCategoriesTarifaires
        {
            get { return listCategoriesTarifaires; }
            set
            {
                listCategoriesTarifaires = value;
                OnPropertyChanged("ListCategoriesTarifaires");
            }
        }

        private ObservableCollection<RemiseMode> listRemiseMode;
        public ObservableCollection<RemiseMode> ListRemiseMode
        {
            get { return listRemiseMode; }
            set
            {
                listRemiseMode = value;
                OnPropertyChanged("ListRemiseMode");
            }
        }

        private RemiseMode selectedRemiseMode;
        public RemiseMode SelectedRemiseMode
        {
            get { return selectedRemiseMode; }
            set
            {
                selectedRemiseMode = value;
                OnPropertyChanged("SelectedRemiseMode");
            }
        }

        private ObservableCollection<RemiseConflit> listRemiseConflit;
        public ObservableCollection<RemiseConflit> ListRemiseConflit
        {
            get { return listRemiseConflit; }
            set
            {
                listRemiseConflit = value;
                OnPropertyChanged("ListRemiseMode");
            }
        }

        private RemiseConflit selectedRemiseConflit;
        public RemiseConflit SelectedRemiseConflit
        {
            get { return selectedRemiseConflit; }
            set
            {
                selectedRemiseConflit = value;
                OnPropertyChanged("SelectedRemiseConflit");
            }
        }

        #endregion

        #region Gestion images

        private LocalStorageMode currentLocalStorageMode;
        public LocalStorageMode CurrentLocalStorageMode
        {
            get { return currentLocalStorageMode; }
            set { currentLocalStorageMode = value; OnPropertyChanged("CurrentLocalStorageMode"); OnPropertyChanged("LocalStorageModeIsSimple"); }
        }

        public Boolean LocalStorageModeIsSimple
        {
            get
            {
                return (CurrentLocalStorageMode != null && CurrentLocalStorageMode._LocalStorageMode == Core.Parametres.LocalStorageMode.simple_system);
            }
        }

        private Boolean imageSynchroPositionLegende;
        public Boolean ImageSynchroPositionLegende
        {
            get { return imageSynchroPositionLegende; }
            set { imageSynchroPositionLegende = value; OnPropertyChanged("ImageSynchroPositionLegende"); }
        }

        #endregion

        #region Paramètres articles

        private Boolean importConditionnementActif;
        public Boolean ImportConditionnementActif
        {
            get { return importConditionnementActif; }
            set
            {
                importConditionnementActif = value;
                OnPropertyChanged("GestionConditionnementActif");
            }
        }
        private Boolean conditionnementQuantiteToUPC;
        public Boolean ConditionnementQuantiteToUPC
        {
            get { return conditionnementQuantiteToUPC; }
            set { conditionnementQuantiteToUPC = value; OnPropertyChanged("ConditionnementQuantiteToUPC"); }
        }
        private Boolean limiteStockConditionnementMini;
        public Boolean LimiteStockConditionnementMini
        {
            get { return limiteStockConditionnementMini; }
            set { limiteStockConditionnementMini = value; OnPropertyChanged("LimiteStockConditionnementMini"); }
        }

        private Boolean stockArticleContremarqueActif;
        public Boolean StockArticleContremarqueActif
        {
            get { return stockArticleContremarqueActif; }
            set { stockArticleContremarqueActif = value; OnPropertyChanged("StockArticleContremarqueActif"); }
        }

        private Boolean canDeleteCatalogProductAssociation;
        public Boolean CanDeleteCatalogProductAssociation
        {
            get { return canDeleteCatalogProductAssociation; }
            set { canDeleteCatalogProductAssociation = value; OnPropertyChanged("CanDeleteCatalogProductAssociation"); }
        }

        private Boolean dateDispoInfoLibreActif;
        public Boolean DateDispoInfoLibreActif
        {
            get { return dateDispoInfoLibreActif; }
            set { dateDispoInfoLibreActif = value; OnPropertyChanged("DateDispoInfoLibreActif"); }
        }
        private ObservableCollection<Model.Sage.cbSysLibre> listDateDispoInfoLibre;
        public ObservableCollection<Model.Sage.cbSysLibre> ListDateDispoInfoLibre
        {
            get { return listDateDispoInfoLibre; }
            set { listDateDispoInfoLibre = value; OnPropertyChanged("ListDateDispoInfoLibre"); }
        }
        private Model.Sage.cbSysLibre selectedDateDispoInfoLibre;
        public Model.Sage.cbSysLibre SelectedDateDispoInfoLibre
        {
            get { return selectedDateDispoInfoLibre; }
            set { selectedDateDispoInfoLibre = value; OnPropertyChanged("SelectedDateDispoInfoLibre"); }
        }

        #region Gamme calcul coefficient poids

        private ObservableCollection<Model.Sage.P_GAMME> listGamme;
        public ObservableCollection<Model.Sage.P_GAMME> ListGamme
        {
            get { return listGamme; }
            set
            {
                listGamme = value;
                OnPropertyChanged("ListGamme");
            }
        }

        private ObservableCollection<Model.Sage.cbSysLibre> listInfolibreCoefficient;
        public ObservableCollection<Model.Sage.cbSysLibre> ListInfolibreCoefficient
        {
            get { return listInfolibreCoefficient; }
            set { listInfolibreCoefficient = value; OnPropertyChanged("ListInfolibreCoefficient"); }
        }

        private Model.Sage.cbSysLibre selectedInfolibreCoefficient;
        public Model.Sage.cbSysLibre SelectedInfolibreCoefficient
        {
            get { return selectedInfolibreCoefficient; }
            set { selectedInfolibreCoefficient = value; OnPropertyChanged("SelectedInfolibreCoefficient"); }
        }

        #endregion

        private Boolean poidsSynchroStock;
        public Boolean PoidsSynchroStock
        {
            get { return poidsSynchroStock; }
            set { poidsSynchroStock = value; OnPropertyChanged("PoidsSynchroStock"); }
        }

        private Boolean quantiteMiniActif;
        public Boolean QuantiteMiniActif
        {
            get { return quantiteMiniActif; }
            set { quantiteMiniActif = value; OnPropertyChanged("QuantiteMiniActif"); }
        }
        private Boolean quantiteMiniConditionnement;
        public Boolean QuantiteMiniConditionnement
        {
            get { return quantiteMiniConditionnement; }
            set { quantiteMiniConditionnement = value; OnPropertyChanged("QuantiteMiniConditionnement"); }
        }
        private Boolean quantiteMiniUniteVente;
        public Boolean QuantiteMiniUniteVente
        {
            get { return quantiteMiniUniteVente; }
            set { quantiteMiniUniteVente = value; OnPropertyChanged("QuantiteMiniUniteVente"); }
        }

        public Boolean CanActiveTransfertInfosFournisseur
        {
            get { return !FournisseurInfolibreActif && !FournisseurStatActif; }
        }
        private Boolean transfertInfosFournisseurActif;
        public Boolean TransfertInfosFournisseurActif
        {
            get { return transfertInfosFournisseurActif; }
            set
            {
                transfertInfosFournisseurActif = value;
                OnPropertyChanged("TransfertInfosFournisseurActif");
                OnPropertyChanged("CanActiveFournisseurInfolibre");
                OnPropertyChanged("CanActiveFournisseurStat");
            }
        }

        private Boolean redirectionComposition;
        public Boolean RedirectionComposition
        {
            get { return redirectionComposition; }
            set { redirectionComposition = value; OnPropertyChanged("RedirectionComposition"); }
        }

        private Boolean stockNegatifZero;
        public Boolean StockNegatifZero
        {
            get { return stockNegatifZero; }
            set { stockNegatifZero = value; OnPropertyChanged("StockNegatifZero"); }
        }
        private Boolean stockNegatifZeroParDepot;
        public Boolean StockNegatifZeroParDepot
        {
            get { return stockNegatifZeroParDepot; }
            set { stockNegatifZeroParDepot = value; OnPropertyChanged("StockNegatifZeroParDepot"); }
        }

        private Boolean filtreDatePrixPrestashop;
        public Boolean FiltreDatePrixPrestashop
        {
            get { return filtreDatePrixPrestashop; }
            set { filtreDatePrixPrestashop = value; OnPropertyChanged("FiltreDatePrixPrestashop"); }
        }

        private Boolean regleBasePrixSpecifique;
        public Boolean RegleBasePrixSpecifique
        {
            get { return regleBasePrixSpecifique; }
            set { regleBasePrixSpecifique = value; OnPropertyChanged("RegleBasePrixSpecifique"); }
        }

        #endregion

        #region Taxes

        private ObservableCollection<TaxSage> listTaxSage;
        public ObservableCollection<TaxSage> ListTaxSage
        {
            get { return listTaxSage; }
            set
            {
                listTaxSage = value;
                OnPropertyChanged("ListTaxSage");
            }
        }

        private TaxSage selectedTaxSageTVA;
        public TaxSage SelectedTaxSageTVA
        {
            get { return selectedTaxSageTVA; }
            set
            {
                selectedTaxSageTVA = value;
                OnPropertyChanged("SelectedTaxSageTVA");
            }
        }

        private TaxSage selectedTaxSageEco;
        public TaxSage SelectedTaxSageEco
        {
            get { return selectedTaxSageEco; }
            set
            {
                selectedTaxSageEco = value;
                OnPropertyChanged("SelectedTaxSageEco");
            }
        }

        #endregion

        #region Création Clients

        private Boolean clientFiltreCommande;
        public Boolean ClientFiltreCommande
        {
            get { return clientFiltreCommande; }
            set
            {
                clientFiltreCommande = value;
                OnPropertyChanged("ClientFiltreCommande");
            }
        }

        private string clientNumComposition;
        public string ClientNumComposition
        {
            get { return clientNumComposition; }
            set
            {
                clientNumComposition = value;
                OnPropertyChanged("ClientNumComposition");
                VerifyClientNum();
            }
        }

        private string clientNumPrefixe;
        public string ClientNumPrefixe
        {
            get { return clientNumPrefixe; }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    value = Core.Global.RemovePurge(value, 69).Replace("_", string.Empty).Replace("-", string.Empty).Replace(" ", string.Empty);
                }
                clientNumPrefixe = value;
                OnPropertyChanged("ClientNumPrefixe");
                VerifyClientNum();
            }
        }

        private int clientNumLongueurNom;
        public int ClientNumLongueurNom
        {
            get { return clientNumLongueurNom; }
            set
            {
                clientNumLongueurNom = value;
                OnPropertyChanged("ClientNumLongueurNom");
                OnPropertyChanged("GetNameLengthSample");
                VerifyClientNum();
            }
        }

        private ObservableCollection<NameNumComponent> listTypeNom;
        public ObservableCollection<NameNumComponent> ListTypeNom
        {
            get { return listTypeNom; }
            set
            {
                listTypeNom = value;
                OnPropertyChanged("ListTypeNom");
            }
        }
        private NameNumComponent selectedTypeNom;
        public NameNumComponent SelectedTypeNom
        {
            get { return selectedTypeNom; }
            set
            {
                selectedTypeNom = value;
                OnPropertyChanged("SelectedTypeNom");
            }
        }

        private int clientNumLongueurNumero;
        public int ClientNumLongueurNumero
        {
            get { return clientNumLongueurNumero; }
            set
            {
                clientNumLongueurNumero = value;
                OnPropertyChanged("ClientNumLongueurNumero");

                CalcMaxCompteur();
                OnPropertyChanged("GetNumberLengthSample");
                VerifyClientNum();
            }
        }

        private ObservableCollection<NumberNumComponent> listTypeNumero;
        public ObservableCollection<NumberNumComponent> ListTypeNumero
        {
            get { return listTypeNumero; }
            set
            {
                listTypeNumero = value;
                OnPropertyChanged("ListTypeNumero");
            }
        }
        private NumberNumComponent selectedTypeNumero;
        public NumberNumComponent SelectedTypeNumero
        {
            get { return selectedTypeNumero; }
            set
            {
                selectedTypeNumero = value;
                OnPropertyChanged("SelectedTypeNumero");
            }
        }

        private ObservableCollection<CounterType> listTypeCompteur;
        public ObservableCollection<CounterType> ListTypeCompteur
        {
            get { return listTypeCompteur; }
            set
            {
                listTypeCompteur = value;
                OnPropertyChanged("ListTypeCompteur");
            }
        }
        private CounterType selectedTypeCompteur;
        public CounterType SelectedTypeCompteur
        {
            get { return selectedTypeCompteur; }
            set
            {
                selectedTypeCompteur = value;
                OnPropertyChanged("SelectedTypeCompteur");
            }
        }

        private int clientNumDebutCompteur;
        public int ClientNumDebutCompteur
        {
            get { return clientNumDebutCompteur; }
            set
            {
                clientNumDebutCompteur = value;
                OnPropertyChanged("ClientNumDebutCompteur");
            }
        }
        private int clientNumMaxCompteur;
        public int ClientNumMaxCompteur
        {
            get { return clientNumMaxCompteur; }
            set
            {
                clientNumMaxCompteur = value;
                OnPropertyChanged("ClientNumMaxCompteur");
            }
        }

        private bool clientNumDepartementRemplacerCodeISO;
        public bool ClientNumDepartementRemplacerCodeISO
        {
            get { return clientNumDepartementRemplacerCodeISO; }
            set
            {
                clientNumDepartementRemplacerCodeISO = value;
                OnPropertyChanged("ClientNumDepartementRemplacerCodeISO");
            }
        }
        private List<Model.Local.Country> listCountryReplaceISOCode;
        public List<Model.Local.Country> ListCountryReplaceISOCode
        {
            get { return listCountryReplaceISOCode; }
            set
            {
                listCountryReplaceISOCode = value;
                OnPropertyChanged("ListCountryReplaceISOCode");
            }
        }

        private Boolean clientMultiMappageBtoB;
        public Boolean ClientMultiMappageBtoB
        {
            get { return clientMultiMappageBtoB; }
            set { clientMultiMappageBtoB = value; OnPropertyChanged("ClientMultiMappageBtoB"); }
        }

        private Boolean clientCiviliteActif;
        public Boolean ClientCiviliteActif
        {
            get { return clientCiviliteActif; }
            set { clientCiviliteActif = value; OnPropertyChanged("ClientCiviliteActif"); }
        }

        private Boolean clientSocieteIntituleActif;
        public Boolean ClientSocieteIntituleActif
        {
            get { return clientSocieteIntituleActif; }
            set { clientSocieteIntituleActif = value; OnPropertyChanged("ClientSocieteIntituleActif"); }
        }

        private Boolean clientNIFActif;
        public Boolean ClientNIFActif
        {
            get { return clientNIFActif; }
            set { clientNIFActif = value; OnPropertyChanged("ClientNIFActif"); }
        }

        private Boolean clientInfosMajusculeActif;
        public Boolean ClientInfosMajusculeActif
        {
            get { return clientInfosMajusculeActif; }
            set { clientInfosMajusculeActif = value; OnPropertyChanged("ClientInfosMajusculeActif"); }
        }

        public string GetNumSample
        {
            get
            {
                string control = CleanComposition();
                control = control.Replace(ClientNumComposing.StrPrefixe, ClientNumPrefixe);
                control = control.Replace(ClientNumComposing.StrNumero, (ClientNumLongueurNumero > NumSample.Length) ? NumSample : NumSample.Substring(0, ClientNumLongueurNumero));
                control = control.Replace(ClientNumComposing.StrNom, (ClientNumLongueurNom > NameSample.Length) ? NameSample : NameSample.Substring(0, ClientNumLongueurNom));
                control = control.Replace(ClientNumComposing.StrDep, DepSample);
                return control;
            }
        }
        public string GetNameLengthSample { get { return (ClientNumLongueurNom > NameSample.Length) ? "Valeur maximale dépassée" : NameSample.Substring(0, ClientNumLongueurNom); } }
        public string GetNumberLengthSample { get { return (ClientNumLongueurNumero > NumSample.Length) ? "Valeur maximale dépassée" : NumSample.Substring(0, ClientNumLongueurNumero); } }

        #endregion

        #region Adresses

        private Boolean clientAdresseTelephonePositionFixe;
        public Boolean ClientAdresseTelephonePositionFixe
        {
            get { return clientAdresseTelephonePositionFixe; }
            set { clientAdresseTelephonePositionFixe = value; OnPropertyChanged("ClientAdresseTelephonePositionFixe"); }
        }

        #endregion

        #region Transfert Clients
        private ObservableCollection<PriceCategory> listPriceCategoryAvailable;
        public ObservableCollection<PriceCategory> ListPriceCategoryAvailable
        {
            get { return listPriceCategoryAvailable; }
            set
            {
                listPriceCategoryAvailable = value;
                OnPropertyChanged("ListPriceCategoryAvailable");
            }
        }

        private ObservableCollection<MailAccountIdentification> listMailAccountIdentification;
        public ObservableCollection<MailAccountIdentification> ListMailAccountIdentification
        {
            get { return listMailAccountIdentification; }
            set
            {
                listMailAccountIdentification = value;
                OnPropertyChanged("ListMailAccountIdentification");
            }
        }

        private MailAccountIdentification selectedMailAccountIdentification;
        public MailAccountIdentification SelectedMailAccountIdentification
        {
            get { return selectedMailAccountIdentification; }
            set
            {
                selectedMailAccountIdentification = value; 
                VisibleMailAccountIdentificationContact = (selectedMailAccountIdentification.Marq == (int)Core.Parametres.MailAccountIdentification.MailContactService ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
                SelectedMailAccountIdentificationContactService = (selectedMailAccountIdentification.Marq != (int)Core.Parametres.MailAccountIdentification.MailContactService ? null : selectedMailAccountIdentificationContactService);
                OnPropertyChanged("SelectedMailAccountIdentification");
            }
        }

        private bool visibleMailAccountIdentificationContact;
        public System.Windows.Visibility VisibleMailAccountIdentificationContact
        {
            get { return (visibleMailAccountIdentificationContact) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden; }
            set
            {
                visibleMailAccountIdentificationContact = (value == System.Windows.Visibility.Visible ? true: false);
                OnPropertyChanged("VisibleMailAccountIdentificationContact");
            }
        }

        private MailToContactService selectedMailAccountIdentificationContactService;
        public MailToContactService SelectedMailAccountIdentificationContactService
        {
            get { return selectedMailAccountIdentificationContactService; }
            set
            {
                selectedMailAccountIdentificationContactService = value;
                OnPropertyChanged("SelectedMailAccountIdentificationContactService");
            }
        }

        private ObservableCollection<int> randomPasswordLength;
        public ObservableCollection<int> RandomPasswordLength
        {
            get { return randomPasswordLength; }
            set
            {
                randomPasswordLength = value;
                OnPropertyChanged("RandomPasswordLength");
            }
        }

        private int selectedRandomPasswordLength;
        public int SelectedRandomPasswordLength
        {
            get { return selectedRandomPasswordLength; }
            set
            {
                selectedRandomPasswordLength = value;
                OnPropertyChanged("SelectedRandomPasswordLength");
            }
        }

        private ObservableCollection<MailNotification> listMailNotification;
        public ObservableCollection<MailNotification> ListMailNotification
        {
            get { return listMailNotification; }
            set
            {
                listMailNotification = value;
                OnPropertyChanged("ListMailNotification");
            }
        }

        private MailNotification selectedMailNotification;
        public MailNotification SelectedMailNotification
        {
            get { return selectedMailNotification; }
            set
            {
                selectedMailNotification = value;
                OnPropertyChanged("SelectedMailNotification");
            }
        }

        private ObservableCollection<MailToContactService> listMailToContactService;
        public ObservableCollection<MailToContactService> ListMailToContactService
        {
            get { return listMailToContactService; }
            set
            {
                listMailToContactService = value;
                OnPropertyChanged("ListMailToContactService");
            }
        }

        private ObservableCollection<MailToContactType> listMailToContactType;
        public ObservableCollection<MailToContactType> ListMailToContactType
        {
            get { return listMailToContactType; }
            set
            {
                listMailToContactType = value;
                OnPropertyChanged("ListMailToContactType");
            }
        }

        private ObservableCollection<SageAddressSend> listSageAddressSend;
        public ObservableCollection<SageAddressSend> ListSageAddressSend
        {
            get { return listSageAddressSend; }
            set
            {
                listSageAddressSend = value;
                OnPropertyChanged("ListSageAddressSend");
            }
        }

        private ObservableCollection<LockPhoneNumber> listLockPhoneNumber;
        public ObservableCollection<LockPhoneNumber> ListLockPhoneNumber
        {
            get { return listLockPhoneNumber; }
            set
            {
                listLockPhoneNumber = value;
                OnPropertyChanged("ListLockPhoneNumber");
            }
        }

        private ObservableCollection<AliasValue> listAliasValue;
        public ObservableCollection<AliasValue> ListAliasValue
        {
            get { return listAliasValue; }
            set
            {
                listAliasValue = value;
                OnPropertyChanged("ListAliasValue");
            }
        }

        private AliasValue selectedAliasValue;
        public AliasValue SelectedAliasValue
        {
            get { return selectedAliasValue; }
            set
            {
                selectedAliasValue = value;
                OnPropertyChanged("SelectedAliasValue");
            }
        }

        private ObservableCollection<LastNameValue> listLastNameValue;
        public ObservableCollection<LastNameValue> ListLastNameValue
        {
            get { return listLastNameValue; }
            set
            {
                listLastNameValue = value;
                OnPropertyChanged("ListLastNameValue");
            }
        }

        private LastNameValue selectedLastNameValue;
        public LastNameValue SelectedLastNameValue
        {
            get { return selectedLastNameValue; }
            set
            {
                selectedLastNameValue = value;
                if (value._LastNameValue == Core.Parametres.LastNameValue.Espace
                    && SelectedFirstNameValue._FirstNameValue == Core.Parametres.FirstNameValue.Espace)
                {
                    SelectedFirstNameValue = ListFirstNameValue.First(v => v._FirstNameValue != Core.Parametres.FirstNameValue.Espace);
                }
                OnPropertyChanged("SelectedLastNameValue");
            }
        }

        private ObservableCollection<FirstNameValue> listFirstNameValue;
        public ObservableCollection<FirstNameValue> ListFirstNameValue
        {
            get { return listFirstNameValue; }
            set
            {
                listFirstNameValue = value;
                OnPropertyChanged("ListFirstNameValue");
            }
        }

        private FirstNameValue selectedFirstNameValue;
        public FirstNameValue SelectedFirstNameValue
        {
            get { return selectedFirstNameValue; }
            set
            {
                selectedFirstNameValue = value;
                if (value._FirstNameValue == Core.Parametres.FirstNameValue.Espace
                    && SelectedLastNameValue._LastNameValue == Core.Parametres.LastNameValue.Espace)
                {
                    SelectedLastNameValue = ListLastNameValue.First(v => v._LastNameValue != Core.Parametres.LastNameValue.Espace);
                }
                OnPropertyChanged("SelectedFirstNameValue");
            }
        }

        private ObservableCollection<CompanyValue> listCompanyValue;
        public ObservableCollection<CompanyValue> ListCompanyValue
        {
            get { return listCompanyValue; }
            set
            {
                listCompanyValue = value;
                OnPropertyChanged("ListCompanyValue");
            }
        }

        private CompanyValue selectedCompanyValue;
        public CompanyValue SelectedCompanyValue
        {
            get { return selectedCompanyValue; }
            set
            {
                selectedCompanyValue = value;
                OnPropertyChanged("SelectedCompanyValue");
            }
        }

        private string clientSeparateurIntitule;
        public string ClientSeparateurIntitule
        {
            get { return clientSeparateurIntitule; }
            set { clientSeparateurIntitule = value; OnPropertyChanged("ClientSeparateurIntitule"); }
        }

        private ObservableCollection<RegexMail> listRegexMail;
        public ObservableCollection<RegexMail> ListRegexMail
        {
            get { return listRegexMail; }
            set
            {
                listRegexMail = value;
                OnPropertyChanged("ListRegexMail");
            }
        }

        private RegexMail selectedRegexMail;
        public RegexMail SelectedRegexMail
        {
            get { return selectedRegexMail; }
            set
            {
                selectedRegexMail = value;
                OnPropertyChanged("SelectedRegexMail");
            }
        }

        private Boolean transfertClientNameIncludeNumbers;
        public Boolean TransfertClientNameIncludeNumbers
        {
            get { return transfertClientNameIncludeNumbers; }
            set
            {
                if (!value ||
                    (value && new PRESTACONNECT.View.PrestaMessage("Attention l'activation de cette option peut bloquer l'utilisation des comptes clients dans PrestaShop"
                        + " si vous ne modifiez pas les types \"validate\" pour les champs \"lastname\" et \"firstname\" des fichiers \"classes\\Customer.php\" et \"classes\\Address.php\" !"
                        + "\nVous pouvez aussi modifier la fonction \"isName\" du fichier \"classes\\Validate.php\" !"
                        + "\n\n"
                        + "Valider l'activation de cette option ?",
                    "Activation envoi des chiffres - champs nom et prénom", MessageBoxButton.YesNo, MessageBoxImage.Warning, 600).ShowDialog() == true))
                {
                    transfertClientNameIncludeNumbers = value;
                }
                OnPropertyChanged("TransfertClientNameIncludeNumbers");
            }
        }

        #endregion

        #region Mode de stockage des images
        private ObservableCollection<ImageStorageMode> listImageStorageMode;
        public ObservableCollection<ImageStorageMode> ListImageStorageMode
        {
            get { return listImageStorageMode; }
            set
            {
                listImageStorageMode = value;
                OnPropertyChanged("ListImageStorageMode");
            }
        }

        private ImageStorageMode selectedImageStorageMode;
        public ImageStorageMode SelectedImageStorageMode
        {
            get { return selectedImageStorageMode; }
            set
            {
                selectedImageStorageMode = value;
                OnPropertyChanged("SelectedImageStorageMode");
            }
        }
        #endregion

        #region Informations libres article

        private ObservableCollection<Model.Local.InformationLibreArticle> listInformationLibreArticle;
        public ObservableCollection<Model.Local.InformationLibreArticle> ListInformationLibreArticle
        {
            get { return listInformationLibreArticle; }
            set
            {
                listInformationLibreArticle = value;
                OnPropertyChanged("ListInformationLibreArticle");
            }
        }

        private ObservableCollection<InformationLibre> listInformationLibre;
        public ObservableCollection<InformationLibre> ListInformationLibre
        {
            get { return listInformationLibre; }
            set
            {
                listInformationLibre = value;
                OnPropertyChanged("ListInformationLibre");
            }
        }

        private InformationLibre selectedInformationLibre;
        public InformationLibre SelectedInformationLibre
        {
            get { return selectedInformationLibre; }
            set
            {
                selectedInformationLibre = value;
                OnPropertyChanged("SelectedInformationLibre");
            }
        }

        private Model.Local.InformationLibreArticle selectedInformationLibreArticle;
        public Model.Local.InformationLibreArticle SelectedInformationLibreArticle
        {
            get { return selectedInformationLibreArticle; }
            set
            {
                selectedInformationLibreArticle = value;
                OnPropertyChanged("SelectedInformationLibreArticle");
            }
        }

        private ObservableCollection<Model.Prestashop.PsFeatureLang> listFeature;
        public ObservableCollection<Model.Prestashop.PsFeatureLang> ListFeature
        {
            get { return listFeature; }
            set
            {
                listFeature = value;
                OnPropertyChanged("ListFeature");
            }
        }

        private ObservableCollection<InformationLibreValeursMode> listInformationLibreValeursMode;
        public ObservableCollection<InformationLibreValeursMode> ListInformationLibreValeursMode
        {
            get { return this.listInformationLibreValeursMode; }
            set
            {
                this.listInformationLibreValeursMode = value;
                OnPropertyChanged("ListInformationLibreValeursMode");
            }
        }

        private ObservableCollection<Model.Sage.cbSysLibre> listSageInfolibreArticleTextTable;
        public ObservableCollection<Model.Sage.cbSysLibre> ListSageInfolibreArticleTextTable
        {
            get { return listSageInfolibreArticleTextTable; }
            set
            {
                listSageInfolibreArticleTextTable = value;
                OnPropertyChanged("ListSageInfolibreArticleTextTable");
            }
        }

        private Model.Sage.cbSysLibre selectedFournisseurSageInfolibreArticle;
        public Model.Sage.cbSysLibre SelectedFournisseurSageInfolibreArticle
        {
            get { return selectedFournisseurSageInfolibreArticle; }
            set
            {
                selectedFournisseurSageInfolibreArticle = value;
                OnPropertyChanged("SelectedFournisseurSageInfolibreArticle");
            }
        }
        private Model.Sage.cbSysLibre selectedMarqueSageInfolibreArticle;
        public Model.Sage.cbSysLibre SelectedMarqueSageInfolibreArticle
        {
            get { return selectedMarqueSageInfolibreArticle; }
            set
            {
                selectedMarqueSageInfolibreArticle = value;
                OnPropertyChanged("SelectedMarqueSageInfolibreArticle");
            }
        }

        private Boolean marqueInfolibreActif;
        public Boolean MarqueInfolibreActif
        {
            get { return marqueInfolibreActif; }
            set
            {
                marqueInfolibreActif = value;
                OnPropertyChanged("MarqueInfolibreActif");
            }
        }

        public Boolean CanActiveFournisseurInfolibre
        {
            get { return !TransfertInfosFournisseurActif; }
        }
        private Boolean fournisseurInfolibreActif;
        public Boolean FournisseurInfolibreActif
        {
            get { return fournisseurInfolibreActif; }
            set
            {
                fournisseurInfolibreActif = value;
                OnPropertyChanged("FournisseurInfolibreActif");
                OnPropertyChanged("CanActivetransfertInfosFournisseur");
            }
        }

        private Boolean suppressionAutoCaracteristique;
        public Boolean SuppressionAutoCaracteristique
        {
            get { return suppressionAutoCaracteristique; }
            set { suppressionAutoCaracteristique = value; OnPropertyChanged("SuppressionAutoCaracteristique"); }
        }

        #endregion

        #region Informations libres client

        private Boolean canActiveCustomerFeatureModule;
        public Boolean CanActiveCustomerFeatureModule
        {
            get { return canActiveCustomerFeatureModule; }
            set
            {
                canActiveCustomerFeatureModule = value;
                OnPropertyChanged("CanActiveCustomerFeatureModule");
            }
        }

        private Boolean moduleStatInfolibreClientActif;
        public Boolean ModuleStatInfolibreClientActif
        {
            get { return moduleStatInfolibreClientActif; }
            set
            {
                moduleStatInfolibreClientActif = value;
                OnPropertyChanged("ModuleStatInfolibreClientActif");
            }
        }

        private ObservableCollection<InformationLibreClient> listInformationLibreClient;
        public ObservableCollection<InformationLibreClient> ListInformationLibreClient
        {
            get { return listInformationLibreClient; }
            set
            {
                listInformationLibreClient = value;
                OnPropertyChanged("ListInformationLibreClient");
            }
        }

        private InformationLibreClient selectedInformationLibreClient;
        public InformationLibreClient SelectedInformationLibreClient
        {
            get { return selectedInformationLibreClient; }
            set
            {
                selectedInformationLibreClient = value;
                OnPropertyChanged("SelectedInformationLibreClient");
            }
        }

        private ObservableCollection<Model.Prestashop.PsCustomerFeatureLang> listCustomerFeature;
        public ObservableCollection<Model.Prestashop.PsCustomerFeatureLang> ListCustomerFeature
        {
            get { return listCustomerFeature; }
            set
            {
                listCustomerFeature = value;
                OnPropertyChanged("ListCustomerFeature");
            }
        }

        #endregion

        #region Statistiques article

        private ObservableCollection<StatistiqueArticle> listStatistiqueArticle;
        public ObservableCollection<StatistiqueArticle> ListStatistiqueArticle
        {
            get { return listStatistiqueArticle; }
            set
            {
                listStatistiqueArticle = value;
                OnPropertyChanged("ListStatistiqueArticle");
            }
        }

        private StatistiqueArticle selectedStatistiqueArticle;
        public StatistiqueArticle SelectedStatistiqueArticle
        {
            get { return selectedStatistiqueArticle; }
            set
            {
                selectedStatistiqueArticle = value;
                OnPropertyChanged("SelectedStatistiqueArticle");
            }
        }

        private ObservableCollection<Model.Sage.P_INTSTATART> listSageStatistiqueArticle;
        public ObservableCollection<Model.Sage.P_INTSTATART> ListSageStatistiqueArticle
        {
            get { return listSageStatistiqueArticle; }
            set
            {
                listSageStatistiqueArticle = value;
                OnPropertyChanged("ListSageStatistiqueArticle");
            }
        }

        private Model.Sage.P_INTSTATART selectedMarqueSageStatistiqueArticle;
        public Model.Sage.P_INTSTATART SelectedMarqueSageStatistiqueArticle
        {
            get { return selectedMarqueSageStatistiqueArticle; }
            set
            {
                selectedMarqueSageStatistiqueArticle = value;
                OnPropertyChanged("SelectedMarqueSageStatistiqueArticle");
            }
        }
        private Model.Sage.P_INTSTATART selectedFournisseurSageStatistiqueArticle;
        public Model.Sage.P_INTSTATART SelectedFournisseurSageStatistiqueArticle
        {
            get { return selectedFournisseurSageStatistiqueArticle; }
            set
            {
                selectedFournisseurSageStatistiqueArticle = value;
                OnPropertyChanged("SelectedFournisseurSageStatistiqueArticle");
            }
        }

        private Boolean marqueStatActif;
        public Boolean MarqueStatActif
        {
            get { return marqueStatActif; }
            set
            {
                marqueStatActif = value;
                OnPropertyChanged("MarqueStatActif");
            }
        }

        public Boolean CanActiveFournisseurStat
        {
            get { return !TransfertInfosFournisseurActif; }
        }
        private Boolean fournisseurStatActif;
        public Boolean FournisseurStatActif
        {
            get { return fournisseurStatActif; }
            set
            {
                fournisseurStatActif = value;
                OnPropertyChanged("FournisseurStatActif");
                OnPropertyChanged("CanActivetransfertInfosFournisseur");
            }
        }

        #endregion

        #region Statistiques client
        private ObservableCollection<StatistiqueClient> listStatistiqueClient;
        public ObservableCollection<StatistiqueClient> ListStatistiqueClient
        {
            get { return listStatistiqueClient; }
            set
            {
                listStatistiqueClient = value;
                OnPropertyChanged("ListStatistiqueClient");
            }
        }

        private StatistiqueClient selectedStatistiqueClient;
        public StatistiqueClient SelectedStatistiqueClient
        {
            get { return selectedStatistiqueClient; }
            set
            {
                selectedStatistiqueClient = value;
                OnPropertyChanged("SelectedStatistiqueClient");
            }
        }

        private ObservableCollection<Model.Sage.P_STATISTIQUE> listSageStatistiqueClient;
        public ObservableCollection<Model.Sage.P_STATISTIQUE> ListSageStatistiqueClient
        {
            get { return listSageStatistiqueClient; }
            set
            {
                listSageStatistiqueClient = value;
                OnPropertyChanged("ListSageStatistiqueClient");
            }
        }
        #endregion

        #region Commandes

        private ObservableCollection<LigneRemiseMode> listLigneRemiseMode;
        public ObservableCollection<LigneRemiseMode> ListLigneRemiseMode
        {
            get { return listLigneRemiseMode; }
            set
            {
                listLigneRemiseMode = value;
                OnPropertyChanged("ListLigneRemiseMode");
            }
        }

        private LigneRemiseMode selectedLigneRemiseMode;
        public LigneRemiseMode SelectedLigneRemiseMode
        {
            get { return selectedLigneRemiseMode; }
            set
            {
                selectedLigneRemiseMode = value;
                OnPropertyChanged("SelectedLigneRemiseMode");
            }
        }

        private DateTime? commandeFiltreDate;
        public DateTime? CommandeFiltreDate
        {
            get { return commandeFiltreDate; }
            set
            {
                commandeFiltreDate = value;
                OnPropertyChanged("CommandeFiltreDate");
            }
        }

        private Boolean updateAdresseFacturation;
        public Boolean UpdateAdresseFacturation
        {
            get { return updateAdresseFacturation; }
            set { updateAdresseFacturation = value; OnPropertyChanged("UpdateAdresseFacturation"); }
        }

        private Boolean insertFacturationEntete;
        public Boolean InsertFacturationEntete
        {
            get { return insertFacturationEntete; }
            set { insertFacturationEntete = value; OnPropertyChanged("InsertFacturationEntete"); }
        }

        private Boolean copyReferencePrestashop;
        public Boolean CopyReferencePrestashop
        {
            get { return copyReferencePrestashop; }
            set { copyReferencePrestashop = value; OnPropertyChanged("CopyReferencePrestashop"); }
        }

        private Boolean forceNumeroFactureSage;
        public Boolean ForceNumeroFactureSage
        {
            get { return forceNumeroFactureSage; }
            set { forceNumeroFactureSage = value; OnPropertyChanged("ForceNumeroFactureSage"); }
        }

        private Model.Sage.F_ARTICLE_Light selectedArticleReduction;
        public Model.Sage.F_ARTICLE_Light SelectedArticleReduction
        {
            get { return selectedArticleReduction; }
            set { selectedArticleReduction = value; OnPropertyChanged("SelectedArticleReduction"); }
        }

        private int joursAutomateStatut;
        public int JoursAutomateStatut
        {
            get { return joursAutomateStatut; }
            set { joursAutomateStatut = value; OnPropertyChanged("JoursAutomateStatut"); }
        }

		#region Frais de port

		private Boolean commandeLigneFraisPort;
		public Boolean CommandeLigneFraisPort
		{
			get { return commandeLigneFraisPort; }
			set { commandeLigneFraisPort = value; OnPropertyChanged("CommandeLigneFraisPort"); }
		}

		private ObservableCollection<Model.Sage.F_ARTICLE_Light> listArticleHorsStock;
		public ObservableCollection<Model.Sage.F_ARTICLE_Light> ListArticleHorsStock
		{
			get { return listArticleHorsStock; }
			set { listArticleHorsStock = value; OnPropertyChanged("ListArticleHorsStock"); }
		}

		private Model.Sage.F_ARTICLE_Light selectedArticlePort;
		public Model.Sage.F_ARTICLE_Light SelectedArticlePort
		{
			get { return selectedArticlePort; }
			set { selectedArticlePort = value; OnPropertyChanged("SelectedArticlePort"); }
		}

		#endregion

		#region Date de livraison

		private Core.Parametres.DateLivraisonMode commandeDateLivraisonMode;
		public Core.Parametres.DateLivraisonMode CommandeDateLivraisonMode
		{
			get { return commandeDateLivraisonMode; }
			set { commandeDateLivraisonMode = value; OnPropertyChanged("CommandeDateLivraisonMode"); }
		}

		private int commandeDateLivraisonJours;
		public int CommandeDateLivraisonJours
		{
			get { return commandeDateLivraisonJours; }
			set { commandeDateLivraisonJours = value; OnPropertyChanged("CommandeDateLivraisonJours"); }
		}

		#endregion

		#region Commentaires

		private bool commandeCommentaireBoutiqueActif;
        public bool CommandeCommentaireBoutiqueActif
        {
            get { return commandeCommentaireBoutiqueActif; }
            set { commandeCommentaireBoutiqueActif = value; OnPropertyChanged("CommandeCommentaireBoutiqueActif"); }
        }
        private string commandeCommentaireBoutiqueTexte;
        public string CommandeCommentaireBoutiqueTexte
        {
            get { return commandeCommentaireBoutiqueTexte; }
            set { commandeCommentaireBoutiqueTexte = (String.IsNullOrEmpty(value) ? "Commande enregistrée sur la boutique : " : value); OnPropertyChanged("CommandeCommentaireBoutiqueTexte"); }
        }

        private bool commandeCommentaireNumeroActif;
        public bool CommandeCommentaireNumeroActif
        {
            get { return commandeCommentaireNumeroActif; }
            set { commandeCommentaireNumeroActif = value; OnPropertyChanged("CommandeCommentaireNumeroActif"); }
        }
        private string commandeCommentaireNumeroTexte;
        public string CommandeCommentaireNumeroTexte
        {
            get { return commandeCommentaireNumeroTexte; }
            set { commandeCommentaireNumeroTexte = (String.IsNullOrEmpty(value) ? "Commande enregistrée sous le numéro : " : value); OnPropertyChanged("CommandeCommentaireNumeroTexte"); }
        }

        private bool commandeCommentaireReferencePaiementActif;
        public bool CommandeCommentaireReferencePaiementActif
        {
            get { return commandeCommentaireReferencePaiementActif; }
            set { commandeCommentaireReferencePaiementActif = value; OnPropertyChanged("CommandeCommentaireReferencePaiementActif"); }
        }
        private string commandeCommentaireReferencePaiementTexte;
        public string CommandeCommentaireReferencePaiementTexte
        {
            get { return commandeCommentaireReferencePaiementTexte; }
            set { commandeCommentaireReferencePaiementTexte = (String.IsNullOrEmpty(value) ? "Paiement enregistré sous la référence : " : value); OnPropertyChanged("CommandeCommentaireReferencePaiementTexte"); }
        }

        private bool commandeCommentaireDateActif;
        public bool CommandeCommentaireDateActif
        {
            get { return commandeCommentaireDateActif; }
            set { commandeCommentaireDateActif = value; OnPropertyChanged("CommandeCommentaireDateActif"); }
        }
        private string commandeCommentaireDateTexte;
        public string CommandeCommentaireDateTexte
        {
            get { return commandeCommentaireDateTexte; }
            set { commandeCommentaireDateTexte = (String.IsNullOrEmpty(value) ? "Commande enregistrée le : " : value); OnPropertyChanged("CommandeCommentaireDateTexte"); }
        }

        private bool commandeCommentaireClientActif;
        public bool CommandeCommentaireClientActif
        {
            get { return commandeCommentaireClientActif; }
            set { commandeCommentaireClientActif = value; OnPropertyChanged("CommandeCommentaireClientActif"); }
        }
        private string commandeCommentaireClientTexte;
        public string CommandeCommentaireClientTexte
        {
            get { return commandeCommentaireClientTexte; }
            set { commandeCommentaireClientTexte = value; OnPropertyChanged("CommandeCommentaireClientTexte"); }
        }

        private bool commandeCommentaireAdresseFacturationActif;
        public bool CommandeCommentaireAdresseFacturationActif
        {
            get { return commandeCommentaireAdresseFacturationActif; }
            set { commandeCommentaireAdresseFacturationActif = value; OnPropertyChanged("CommandeCommentaireAdresseFacturationActif"); }
        }
        private string commandeCommentaireAdresseFacturationTexte;
        public string CommandeCommentaireAdresseFacturationTexte
        {
            get { return commandeCommentaireAdresseFacturationTexte; }
            set { commandeCommentaireAdresseFacturationTexte = value; OnPropertyChanged("CommandeCommentaireAdresseFacturationTexte"); }
        }

        private bool commandeCommentaireAdresseLivraisonActif;
        public bool CommandeCommentaireAdresseLivraisonActif
        {
            get { return commandeCommentaireAdresseLivraisonActif; }
            set { commandeCommentaireAdresseLivraisonActif = value; OnPropertyChanged("CommandeCommentaireAdresseLivraisonActif"); }
        }
        private string commandeCommentaireAdresseLivraisonTexte;
        public string CommandeCommentaireAdresseLivraisonTexte
        {
            get { return commandeCommentaireAdresseLivraisonTexte; }
            set { commandeCommentaireAdresseLivraisonTexte = value; OnPropertyChanged("CommandeCommentaireAdresseLivraisonTexte"); }
        }

        private bool commandeCommentaireLibre1Actif;
        public bool CommandeCommentaireLibre1Actif
        {
            get { return commandeCommentaireLibre1Actif; }
            set { commandeCommentaireLibre1Actif = value; OnPropertyChanged("CommandeCommentaireLibre1Actif"); }
        }
        private string commandeCommentaireLibre1Texte;
        public string CommandeCommentaireLibre1Texte
        {
            get { return commandeCommentaireLibre1Texte; }
            set { commandeCommentaireLibre1Texte = value; OnPropertyChanged("CommandeCommentaireLibre1Texte"); }
        }
        private bool commandeCommentaireLibre2Actif;
        public bool CommandeCommentaireLibre2Actif
        {
            get { return commandeCommentaireLibre2Actif; }
            set { commandeCommentaireLibre2Actif = value; OnPropertyChanged("CommandeCommentaireLibre2Actif"); }
        }
        private string commandeCommentaireLibre2Texte;
        public string CommandeCommentaireLibre2Texte
        {
            get { return commandeCommentaireLibre2Texte; }
            set { commandeCommentaireLibre2Texte = value; OnPropertyChanged("CommandeCommentaireLibre2Texte"); }
        }
        private bool commandeCommentaireLibre3Actif;
        public bool CommandeCommentaireLibre3Actif
        {
            get { return commandeCommentaireLibre3Actif; }
            set { commandeCommentaireLibre3Actif = value; OnPropertyChanged("CommandeCommentaireLibre3Actif"); }
        }
        private string commandeCommentaireLibre3Texte;
        public string CommandeCommentaireLibre3Texte
        {
            get { return commandeCommentaireLibre3Texte; }
            set { commandeCommentaireLibre3Texte = value; OnPropertyChanged("CommandeCommentaireLibre3Texte"); }
        }

        private Boolean commentStart;
        public Boolean CommentStart
        {
            get { return commentStart; }
            set { commentStart = value; OnPropertyChanged("CommentStart"); }
        }
        private Boolean commentEnds;
        public Boolean CommentEnds
        {
            get { return commentEnds; }
            set { commentEnds = value; OnPropertyChanged("CommentEnds"); }
        }

        #endregion

        #region Packaging

        private ObservableCollection<Model.Sage.cbSysLibre> listInfolibrePackaging;
        public ObservableCollection<Model.Sage.cbSysLibre> ListInfolibrePackaging
        {
            get { return listInfolibrePackaging; }
            set { listInfolibrePackaging = value; OnPropertyChanged("ListInfolibrePackaging"); }
        }

        private Model.Sage.cbSysLibre selectedInfolibrePackaging;
        public Model.Sage.cbSysLibre SelectedInfolibrePackaging
        {
            get { return selectedInfolibrePackaging; }
            set { selectedInfolibrePackaging = value; OnPropertyChanged("SelectedInfolibrePackaging"); }
        }

        private ObservableCollection<Model.Sage.F_ARTICLE_Light> listArticlePackaging;
        public ObservableCollection<Model.Sage.F_ARTICLE_Light> ListArticlePackaging
        {
            get { return listArticlePackaging; }
            set { listArticlePackaging = value; OnPropertyChanged("ListArticlePackaging"); }
        }

        private Model.Sage.F_ARTICLE_Light selectedArticlePackaging;
        public Model.Sage.F_ARTICLE_Light SelectedArticlePackaging
        {
            get { return selectedArticlePackaging; }
            set { selectedArticlePackaging = value; OnPropertyChanged("SelectedArticlePackaging"); }
        }

        #endregion

        #endregion

        #region Reglement

        private Boolean reglementEcheancierActif;
        public Boolean ReglementEcheancierActif
        {
            get { return reglementEcheancierActif; }
            set { reglementEcheancierActif = value; OnPropertyChanged("ReglementEcheancierActif"); }
        }
        private Boolean reglementLibellePartielActif;
        public Boolean ReglementLibellePartielActif
        {
            get { return reglementLibellePartielActif; }
            set { reglementLibellePartielActif = value; OnPropertyChanged("ReglementLibellePartielActif"); }
        }

        #endregion

        #region Outils

        private String cronSynchroArticleURL = string.Empty;
        public String CronSynchroArticleURL
        {
            get { return cronSynchroArticleURL; }
            set { cronSynchroArticleURL = value; OnPropertyChanged("CronSynchroArticleURL"); }
        }

        private bool logChronoSynchroStockPriceActif;
        public bool LogChronoSynchroStockPriceActif
        {
            get { return logChronoSynchroStockPriceActif; }
            set { logChronoSynchroStockPriceActif = value; OnPropertyChanged("LogChronoSynchroStockPriceActif"); }
        }

        private bool refreshTempCustomerListDisabled;
        public bool RefreshTempCustomerListDisabled
        {
            get { return refreshTempCustomerListDisabled; }
            set { refreshTempCustomerListDisabled = value; OnPropertyChanged("RefreshTempCustomerListDisabled"); }
        }

        private bool unlockProcessorCore;
        public bool UnlockProcessorCore
        {
            get { return unlockProcessorCore; }
            set { unlockProcessorCore = value; OnPropertyChanged("UnlockProcessorCore"); }
        }

        private int allocatedProcessorCore;
        public int AllocatedProcessorCore
        {
            get { return allocatedProcessorCore; }
            set { allocatedProcessorCore = value; OnPropertyChanged("AllocatedProcessorCore"); }
        }

        public int SystemEnvironmentProcessorCount
        {
            get { return System.Environment.ProcessorCount; }
        }

        private bool crystalForceConnectionInfoOnSubReports;
        public bool CrystalForceConnectionInfoOnSubReports
        {
            get { return crystalForceConnectionInfoOnSubReports; }
            set { crystalForceConnectionInfoOnSubReports = value; OnPropertyChanged("CrystalForceConnectionInfoOnSubReports"); }
        }

        #region Cron/Scripts

        private string cronArticleURL;
        public string CronArticleURL
        {
            get { return cronArticleURL; }
            set { cronArticleURL = value; OnPropertyChanged("CronArticleURL"); }
        }
        private string cronArticleBalise;
        public string CronArticleBalise
        {
            get { return cronArticleBalise; }
            set { cronArticleBalise = value; OnPropertyChanged("CronArticleBalise"); }
        }
        private int cronArticleTimeout;
        public int CronArticleTimeout
        {
            get { return cronArticleTimeout; }
            set { cronArticleTimeout = value; OnPropertyChanged("CronArticleTimeout"); }
        }

		private string cronCommandeURL;
		public string CronCommandeURL
		{
			get { return cronCommandeURL; }
			set { cronCommandeURL = value; OnPropertyChanged("CronCommandeURL"); }
		}
		private string cronCommandeBalise;
		public string CronCommandeBalise
		{
			get { return cronCommandeBalise; }
			set { cronCommandeBalise = value; OnPropertyChanged("CronCommandeBalise"); }
		}
		private int cronCommandeTimeout;
		public int CronCommandeTimeout
		{
			get { return cronCommandeTimeout; }
			set { cronCommandeTimeout = value; OnPropertyChanged("CronCommandeTimeout"); }
		}

		#endregion

        #endregion

        #region Remplacement de texte

        private String replacementOriginText;
        public String ReplacementOriginText
        {
            get { return replacementOriginText; }
            set
            {
                replacementOriginText = value;
                OnPropertyChanged("ReplacementOriginText");
            }
        }

        private String replacementNewText;
        public String ReplacementNewText
        {
            get { return replacementNewText; }
            set
            {
                replacementNewText = value;
                OnPropertyChanged("ReplacementNewText");
            }
        }

        private ObservableCollection<Model.Local.Replacement> listReplacement;
        public ObservableCollection<Model.Local.Replacement> ListReplacement
        {
            get { return listReplacement; }
            set
            {
                listReplacement = value;
                OnPropertyChanged("ListReplacement");
            }
        }

        private Model.Local.Replacement selectedReplacement;
        public Model.Local.Replacement SelectedReplacement
        {
            get { return selectedReplacement; }
            set
            {
                selectedReplacement = value;
                OnPropertyChanged("SelectedReplacement");
            }
        }

        #endregion

        #region Module OleaPromo

        private bool canActiveOleaPromoModule;
        public bool CanActiveOleaPromoModule
        {
            get { return canActiveOleaPromoModule; }
            set
            {
                canActiveOleaPromoModule = value;
                OnPropertyChanged("CanActiveOleaPromoModule");
            }
        }

        private bool oleaPromoActif;
        public bool OleaPromoActif
        {
            get { return oleaPromoActif; }
            set
            {
                oleaPromoActif = value;
                OnPropertyChanged("OleaPromoActif");
            }
        }

        private string oleaSuffixeGratuit;
        public string OleaSuffixeGratuit
        {
            get { return oleaSuffixeGratuit; }
            set
            {
                oleaSuffixeGratuit = value;
                OnPropertyChanged("OleaSuffixeGratuit");
            }
        }
        #endregion

        #region Interface

        private ObservableCollection<ProductFilterActiveDefault> listProductFilterActiveDefault;
        public ObservableCollection<ProductFilterActiveDefault> ListProductFilterActiveDefault
        {
            get { return listProductFilterActiveDefault; }
            set { listProductFilterActiveDefault = value; OnPropertyChanged("ListProductFilterActiveDefault"); }
        }

        private ProductFilterActiveDefault selectedProductFilterActiveDefault;
        public ProductFilterActiveDefault SelectedProductFilterActiveDefault
        {
            get { return selectedProductFilterActiveDefault; }
            set { selectedProductFilterActiveDefault = value; OnPropertyChanged("SelectedProductFilterActiveDefault"); }
        }

        private bool windowsMaximized;
        public bool WindowsMaximized
        {
            get { return windowsMaximized; }
            set { windowsMaximized = value; OnPropertyChanged("WindowsMaximized"); }
        }

        private int sleepTimeWYSIWYG;
        public int SleepTimeWYSIWYG
        {
            get { return sleepTimeWYSIWYG; }
            set { sleepTimeWYSIWYG = value; OnPropertyChanged("SleepTimeWYSIWYG"); }
        }

        private bool disabledWYSIWYG;
        public bool DisabledWYSIWYG
        {
            get { return disabledWYSIWYG; }
            set { disabledWYSIWYG = value; OnPropertyChanged("DisabledWYSIWYG"); }
        }

        private bool ie11EmulationModeDisabled;
        public bool IE11EmulationModeDisabled
        {
            get { return ie11EmulationModeDisabled; }
            set { ie11EmulationModeDisabled = value; OnPropertyChanged("IE11EmulationModeDisabled"); }
        }

        private bool productUpdateValidationDisabled;
        public bool ProductUpdateValidationDisabled
        {
            get { return productUpdateValidationDisabled; }
            set { productUpdateValidationDisabled = value; OnPropertyChanged("ProductUpdateValidationDisabled"); }
        }

        #endregion

        #region Module Preorder

        private bool canActivePreorderModule;
        public bool CanActivePreorderModule
        {
            get { return canActivePreorderModule; }
            set
            {
                canActivePreorderModule = value;
                OnPropertyChanged("CanActivePreorderModule");
            }
        }

        private bool preorderActif;
        public bool PreorderActif
        {
            get { return preorderActif; }
            set
            {
                preorderActif = value;
                OnPropertyChanged("PreorderActif");
            }
        }

        private Model.Sage.cbSysLibre selectedInfolibrePreorder;
        public Model.Sage.cbSysLibre SelectedInfolibrePreorder
        {
            get { return selectedInfolibrePreorder; }
            set
            {
                selectedInfolibrePreorder = value;
                OnPropertyChanged("SelectedInfolibrePreorder");
            }
        }

        private string preorderInfolibreValue;
        public string PreorderInfolibreValue
        {
            get { return preorderInfolibreValue; }
            set
            {
                preorderInfolibreValue = value;
                OnPropertyChanged("PreorderInfolibreValue");
            }
        }

        private ObservableCollection<Model.Prestashop.ProductLight> listPreorderPrestashopProduct;
        public ObservableCollection<Model.Prestashop.ProductLight> ListPreorderPrestashopProduct
        {
            get { return listPreorderPrestashopProduct; }
            set { listPreorderPrestashopProduct = value; OnPropertyChanged("ListPreorderPrestashopProduct"); }
        }

        private Model.Prestashop.ProductLight selectedPreorderPrestashopProduct;
        public Model.Prestashop.ProductLight SelectedPreorderPrestashopProduct
        {
            get { return selectedPreorderPrestashopProduct; }
            set { selectedPreorderPrestashopProduct = value; OnPropertyChanged("SelectedPreorderPrestashopProduct"); }
        }

        private ObservableCollection<Model.Prestashop.PsOrderStateLang> listPreorderPrestashopOrderState;
        public ObservableCollection<Model.Prestashop.PsOrderStateLang> ListPreorderPrestashopOrderState
        {
            get { return listPreorderPrestashopOrderState; }
            set { listPreorderPrestashopOrderState = value; OnPropertyChanged("ListPreorderPrestashopOrderState"); }
        }

        private Model.Prestashop.PsOrderStateLang selectedPreorderPrestashopOrderState;
        public Model.Prestashop.PsOrderStateLang SelectedPreorderPrestashopOrderState
        {
            get { return selectedPreorderPrestashopOrderState; }
            set { selectedPreorderPrestashopOrderState = value; OnPropertyChanged("SelectedPreorderPrestashopOrderState"); }
        }

        private ObservableCollection<Model.Sage.cbSysLibre> listInfolibreEntete;
        public ObservableCollection<Model.Sage.cbSysLibre> ListInfolibreEntete
        {
            get { return listInfolibreEntete; }
            set { listInfolibreEntete = value; OnPropertyChanged("ListInfolibreEntete"); }
        }

        private Model.Sage.cbSysLibre selectedInfolibreEntetePreorder;
        public Model.Sage.cbSysLibre SelectedInfolibreEntetePreorder
        {
            get { return selectedInfolibreEntetePreorder; }
            set
            {
                selectedInfolibreEntetePreorder = value;
                OnPropertyChanged("SelectedInfolibreEntetePreorder");
            }
        }

        private string preorderInfolibreEnteteValue;
        public string PreorderInfolibreEnteteValue
        {
            get { return preorderInfolibreEnteteValue; }
            set
            {
                preorderInfolibreEnteteValue = value;
                OnPropertyChanged("PreorderInfolibreEnteteValue");
            }
        }

        #endregion

        #region Information Article Sage

        private ObservableCollection<InformationArticle> listSageInfoArticle;
        public ObservableCollection<InformationArticle> ListSageInfoArticle
        {
            get { return listSageInfoArticle; }
            set
            {
                listSageInfoArticle = value;
                OnPropertyChanged("ListSageInfoArticle");
            }
        }

        private InformationArticle selectedSageInfoArticle;
        public InformationArticle SelectedSageInfoArticle
        {
            get { return selectedSageInfoArticle; }
            set
            {
                selectedSageInfoArticle = value;
                OnPropertyChanged("SelectedSageInfoArticle");
            }
        }

        #endregion

        #region Tracking

        private Boolean trackingEnteteActif;
        public Boolean TrackingEnteteActif
        {
            get { return trackingEnteteActif; }
            set
            {
                trackingEnteteActif = value;
                OnPropertyChanged("TrackingEnteteActif");
            }
        }
        private Boolean trackingInfolibreActif;
        public Boolean TrackingInfolibreActif
        {
            get { return trackingInfolibreActif; }
            set
            {
                trackingInfolibreActif = value;
                OnPropertyChanged("TrackingInfolibreActif");
            }
        }

        private ObservableCollection<FieldDocumentEntete> listSageFieldDocumentEntete;
        public ObservableCollection<FieldDocumentEntete> ListSageFieldDocumentEntete
        {
            get { return listSageFieldDocumentEntete; }
            set { listSageFieldDocumentEntete = value; OnPropertyChanged("ListSageFieldDocumentEntete"); }
        }
        private FieldDocumentEntete selectedTrackingFieldDocumentEntete;
        public FieldDocumentEntete SelectedTrackingFieldDocumentEntete
        {
            get { return selectedTrackingFieldDocumentEntete; }
            set
            {
                selectedTrackingFieldDocumentEntete = value;
                OnPropertyChanged("SelectedTrackingFieldDocumentEntete");
            }
        }
        private ObservableCollection<Model.Sage.cbSysLibre> listSageInfolibreDocumentTextTable;
        public ObservableCollection<Model.Sage.cbSysLibre> ListSageInfolibreDocumentTextTable
        {
            get { return listSageInfolibreDocumentTextTable; }
            set { listSageInfolibreDocumentTextTable = value; OnPropertyChanged("ListSageInfolibreDocumentTextTable"); }
        }
        private Model.Sage.cbSysLibre selectedTrackingInfolibreDocument;
        public Model.Sage.cbSysLibre SelectedTrackingInfolibreDocument
        {
            get { return selectedTrackingInfolibreDocument; }
            set
            {
                selectedTrackingInfolibreDocument = value;
                OnPropertyChanged("SelectedTrackingInfolibreDocument");
            }
        }

        #endregion

        #region Module AECInvoiceHistory

        private bool canActiveAECInvoiceHistory;
        public bool CanActiveAECInvoiceHistory
        {
            get { return canActiveAECInvoiceHistory; }
            set
            {
                canActiveAECInvoiceHistory = value;
                OnPropertyChanged("CanActiveAECInvoiceHistory");
            }
        }

        private bool aecInvoiceHistoryActif;
        public bool AECInvoiceHistoryActif
        {
            get { return aecInvoiceHistoryActif; }
            set
            {
                aecInvoiceHistoryActif = value;
                OnPropertyChanged("AECInvoiceHistoryActif");
            }
        }

        private ObservableCollection<Model.Sage.cbSysLibre> listSageInfolibreClientTextTable;
        public ObservableCollection<Model.Sage.cbSysLibre> ListSageInfolibreClientTextTable
        {
            get { return listSageInfolibreClientTextTable; }
            set
            {
                listSageInfolibreClientTextTable = value;
                OnPropertyChanged("ListSageInfolibreClientTextTable");
            }
        }

        private Model.Sage.cbSysLibre selectedInfolibreInvoiceHistorySendMail;
        public Model.Sage.cbSysLibre SelectedInfolibreInvoiceHistorySendMail
        {
            get { return selectedInfolibreInvoiceHistorySendMail; }
            set
            {
                selectedInfolibreInvoiceHistorySendMail = value;
                OnPropertyChanged("SelectedInfolibreInvoiceHistorySendMail");
            }
        }

        private String infolibreInvoiceHistorySendMailValue;
        public String InfolibreInvoiceHistorySendMailValue
        {
            get { return infolibreInvoiceHistorySendMailValue; }
            set
            {
                infolibreInvoiceHistorySendMailValue = value;
                OnPropertyChanged("InfolibreInvoiceHistorySendMailValue");
            }
        }

        private Boolean aecInvoiceHistoryArchivePDF;
        public Boolean AECInvoiceHistoryArchivePDF
        {
            get { return aecInvoiceHistoryArchivePDF; }
            set
            {
                aecInvoiceHistoryArchivePDF = value;
                OnPropertyChanged("AECInvoiceHistoryArchivePDF");
            }
        }

        private String aecInvoiceHistoryArchivePDFFolder;
        public String AECInvoiceHistoryArchivePDFFolder
        {
            get { return aecInvoiceHistoryArchivePDFFolder; }
            set
            {
                aecInvoiceHistoryArchivePDFFolder = value;
                OnPropertyChanged("AECInvoiceHistoryArchivePDFFolder");
            }
        }

        #endregion

        #region Module AECStock

        private bool canActiveAECStockModule;
        public bool CanActiveAECStockModule
        {
            get { return canActiveAECStockModule; }
            set
            {
                canActiveAECStockModule = value;
                OnPropertyChanged("CanActiveAECStockModule");
            }
        }

        private bool aecStockActif;
        public bool AECStockActif
        {
            get { return aecStockActif; }
            set
            {
                aecStockActif = value;
                OnPropertyChanged("AECStockActif");
            }
        }

        #endregion

        #region Module AECCollaborateur

        private bool canActiveAECCollaborateurModule;
        public bool CanActiveAECCollaborateurModule
        {
            get { return canActiveAECCollaborateurModule; }
            set
            {
                canActiveAECCollaborateurModule = value;
                OnPropertyChanged("CanActiveAECCollaborateurModule");
            }
        }

        private bool aecCollaborateurActif;
        public bool AECCollaborateurActif
        {
            get { return aecCollaborateurActif; }
            set
            {
                aecCollaborateurActif = value;
                OnPropertyChanged("AECCollaborateurActif");
            }
        }

        #endregion

        #region Module AECPaiement

        private bool canActiveAECPaiementModule;
        public bool CanActiveAECPaiementModule
        {
            get { return canActiveAECPaiementModule; }
            set
            {
                canActiveAECPaiementModule = value;
                OnPropertyChanged("CanActiveAECPaiementModule");
            }
        }

        private bool aecPaiementActif;
        public bool AECPaiementActif
        {
            get { return aecPaiementActif; }
            set
            {
                aecPaiementActif = value;
                OnPropertyChanged("AECPaiementActif");
            }
        }

        #endregion

        #region Module AECCustomerOutstanding

        private bool canActiveAECCustomerOutstandingModule;
        public bool CanActiveAECCustomerOutstandingModule
        {
            get { return canActiveAECCustomerOutstandingModule; }
            set
            {
                canActiveAECCustomerOutstandingModule = value;
                OnPropertyChanged("CanActiveAECCustomerOutstandingModule");
            }
        }

        private bool aecCustomerOutstandingActif;
        public bool AECCustomerOutstandingActif
        {
            get { return aecCustomerOutstandingActif; }
            set
            {
                aecCustomerOutstandingActif = value;
                OnPropertyChanged("AECCustomerOutstandingActif");
            }
        }

        #endregion

        #region Module AECCustomerInfo

        private bool canActiveAECCustomerInfoModule;
        public bool CanActiveAECCustomerInfoModule
        {
            get { return canActiveAECCustomerInfoModule; }
            set
            {
                canActiveAECCustomerInfoModule = value;
                OnPropertyChanged("CanActiveAECCustomerInfoModule");
            }
        }

        private bool aecCustomerInfoActif;
        public bool AECCustomerInfoActif
        {
            get { return aecCustomerInfoActif; }
            set
            {
                aecCustomerInfoActif = value;
                OnPropertyChanged("AECCustomerInfoActif");
            }
        }

        #endregion

        #region Module GroupCRisque

        private ObservableCollection<Model.Sage.P_CRISQUE> listCodeRisque;
        public ObservableCollection<Model.Sage.P_CRISQUE> ListCodeRisque
        {
            get { return listCodeRisque; }
            set { listCodeRisque = value; OnPropertyChanged("ListCodeRisque"); }
        }

        private Model.Sage.P_CRISQUE selectedCodeRisque;
        public Model.Sage.P_CRISQUE SelectedCodeRisque
        {
            get { return selectedCodeRisque; }
            set { selectedCodeRisque = value; OnPropertyChanged("SelectedCodeRisque"); }
        }

        private ObservableCollection<Model.Prestashop.PsGroupLang> listPsGroup;
        public ObservableCollection<Model.Prestashop.PsGroupLang> ListPsGroup
        {
            get { return listPsGroup; }
            set { listPsGroup = value; OnPropertyChanged("ListPsGroup"); }
        }

        #endregion

        #region Module Portfolio Customer Employee

        private bool canActivePortfolioCustomerEmployee;
        public bool CanActivePortfolioCustomerEmployee
        {
            get { return canActivePortfolioCustomerEmployee; }
            set
            {
                canActivePortfolioCustomerEmployee = value;
                OnPropertyChanged("CanActivePortfolioCustomerEmployee");
            }
        }

        private bool portfolioCustomerEmployeeActif;
        public bool PortfolioCustomerEmployeeActif
        {
            get { return portfolioCustomerEmployeeActif; }
            set
            {
                portfolioCustomerEmployeeActif = value;
                OnPropertyChanged("PortfolioCustomerEmployeeActif");
            }
        }

        private ObservableCollection<Model.Sage.F_COLLABORATEUR> listCollaborateur;
        public ObservableCollection<Model.Sage.F_COLLABORATEUR> ListCollaborateur
        {
            get { return listCollaborateur; }
            set { listCollaborateur = value; OnPropertyChanged("ListCollaborateur"); }
        }

        private Model.Sage.F_COLLABORATEUR selectedCollaborateur;
        public Model.Sage.F_COLLABORATEUR SelectedCollaborateur
        {
            get { return selectedCollaborateur; }
            set { selectedCollaborateur = value; OnPropertyChanged("SelectedCollaborateur"); }
        }

        private ObservableCollection<Model.Prestashop.PsEmployee> listPsEmployee;
        public ObservableCollection<Model.Prestashop.PsEmployee> ListPsEmployee
        {
            get { return listPsEmployee; }
            set { listPsEmployee = value; OnPropertyChanged("ListPsEmployee"); }
        }

        #endregion

        #region Module SoColissimo

        private bool canActiveSoColissimoDelivery;
        public bool CanActiveSoColissimoDelivery
        {
            get { return canActiveSoColissimoDelivery; }
            set { canActiveSoColissimoDelivery = value; OnPropertyChanged("CanActiveSoColissimoDelivery"); }
        }

        private Boolean soColissimoDeliveryActive;
        public Boolean SoColissimoDeliveryActive
        {
            get { return soColissimoDeliveryActive; }
            set { soColissimoDeliveryActive = value; OnPropertyChanged("SoColissimoDeliveryActive"); }
        }

        private ObservableCollection<Model.Sage.cbSysLibre> soColissimolistInfolibreEntete;
        public ObservableCollection<Model.Sage.cbSysLibre> SoColissimoListInfolibreEntete
        {
            get { return soColissimolistInfolibreEntete; }
            set { soColissimolistInfolibreEntete = value; OnPropertyChanged("SoColissimoListInfolibreEntete"); }
        }

        private Boolean soColissimoInfolibreTypePointActive;
        public Boolean SoColissimoInfolibreTypePointActive
        {
            get { return soColissimoInfolibreTypePointActive; }
            set { soColissimoInfolibreTypePointActive = value; OnPropertyChanged("SoColissimoInfolibreTypePointActive"); }
        }
        private Model.Sage.cbSysLibre selectedInfolibreEnteteSoColissimoTypePoint;
        public Model.Sage.cbSysLibre SelectedInfolibreEnteteSoColissimoTypePoint
        {
            get { return selectedInfolibreEnteteSoColissimoTypePoint; }
            set
            {
                selectedInfolibreEnteteSoColissimoTypePoint = value;
                OnPropertyChanged("SelectedInfolibreEnteteSoColissimoTypePoint");
            }
        }

        private Boolean soColissimoInfolibreDestinataireActive;
        public Boolean SoColissimoInfolibreDestinataireActive
        {
            get { return soColissimoInfolibreDestinataireActive; }
            set { soColissimoInfolibreDestinataireActive = value; OnPropertyChanged("SoColissimoInfolibreDestinataireActive"); }
        }
        private Model.Sage.cbSysLibre selectedInfolibreEnteteSoColissimoDestinataire;
        public Model.Sage.cbSysLibre SelectedInfolibreEnteteSoColissimoDestinataire
        {
            get { return selectedInfolibreEnteteSoColissimoDestinataire; }
            set
            {
                selectedInfolibreEnteteSoColissimoDestinataire = value;
                OnPropertyChanged("SelectedInfolibreEnteteSoColissimoDestinataire");
            }
        }

        private Boolean soColissimoReplacePhoneActive;
        public Boolean SoColissimoReplacePhoneActive
        {
            get { return soColissimoReplacePhoneActive; }
            set { soColissimoReplacePhoneActive = value; OnPropertyChanged("SoColissimoReplacePhoneActive"); }
        }

        private Boolean soColissimoReplaceAddressNameActive;
        public Boolean SoColissimoReplaceAddressNameActive
        {
            get { return soColissimoReplaceAddressNameActive; }
            set { soColissimoReplaceAddressNameActive = value; OnPropertyChanged("SoColissimoReplaceAddressNameActive"); }
        }

        #endregion

		#region Module DWFProductGuiderates

		private bool canActiveCanActiveDWFProductGuideratesModule;
		public bool CanActiveCanActiveDWFProductGuideratesModule
		{
			get { return canActiveCanActiveDWFProductGuideratesModule; }
			set
			{
				canActiveCanActiveDWFProductGuideratesModule = value;
				OnPropertyChanged("CanActiveCanActiveDWFProductGuideratesModule");
			}
		}

		private bool dwfProductGuideratesActif;
		public bool DWFProductGuideratesActif
		{
			get { return dwfProductGuideratesActif; }
			set
			{
				dwfProductGuideratesActif = value;
				OnPropertyChanged("DWFProductGuideratesActif");
			}
		}

		#endregion

		#region Module DWFProductExtraFields

		private bool canActiveCanActiveDWFProductExtraFieldsModule;
		public bool CanActiveCanActiveDWFProductExtraFieldsModule
		{
			get { return canActiveCanActiveDWFProductExtraFieldsModule; }
			set
			{
				canActiveCanActiveDWFProductExtraFieldsModule = value;
				OnPropertyChanged("CanActiveCanActiveDWFProductExtraFieldsModule");
			}
		}

		private bool dwfProductExtraFieldsActif;
		public bool DWFProductExtraFieldsActif
		{
			get { return dwfProductExtraFieldsActif; }
			set
			{
				dwfProductExtraFieldsActif = value;
				OnPropertyChanged("DWFProductExtraFieldsActif");
			}
		}

		#endregion
		
        #endregion

        #region Constructors

        public ConfigurationContext()
            : base()
        {

            LoadGroupsWorker = new BackgroundWorker();
            LoadGroupsWorker.WorkerReportsProgress = true;

            LoadConfigWorker = new BackgroundWorker();
            LoadConfigWorker.WorkerReportsProgress = true;

            ListGroup = new ObservableCollection<ConfigurationGroup>();
            ListCategoriesTarifaires = new ObservableCollection<Model.Sage.P_CATTARIF>();

            //SelectedCatalogItems.GroupDescriptions.Add(new PropertyGroupDescription(string.Empty));

            #region General

            ListRemiseMode = new ObservableCollection<RemiseMode>();

            ListRemiseConflit = new ObservableCollection<RemiseConflit>();
            foreach (Core.Parametres.RemiseConflit value in Enum.GetValues(typeof(Core.Parametres.RemiseConflit)))
                ListRemiseConflit.Add(new RemiseConflit(value));
            SelectedRemiseConflit = ListRemiseConflit.FirstOrDefault(l => l._RemiseConflit == Core.Global.GetConfig().ConflitRemise);

            #endregion

            #region Gestion des images

            CurrentLocalStorageMode = new LocalStorageMode(Core.Global.GetConfig().ConfigLocalStorageMode);
            ImageSynchroPositionLegende = Core.Global.GetConfig().ConfigImageSynchroPositionLegende;

            #endregion

            #region Création clients

            ClientMultiMappageBtoB = Core.Global.GetConfig().ConfigClientMultiMappageBtoB;

            ClientCiviliteActif = Core.Global.GetConfig().ConfigClientCiviliteActif;

            ClientSocieteIntituleActif = Core.Global.GetConfig().ConfigClientSocieteIntituleActif;
            ClientNIFActif = Core.Global.GetConfig().ConfigClientNIFActif;
            ClientInfosMajusculeActif = Core.Global.GetConfig().ConfigClientInfosMajusculeActif;

            #endregion

            #region Adresses

            ClientAdresseTelephonePositionFixe = Core.Global.GetConfig().ConfigClientAdresseTelephonePositionFixe;

            #endregion

            #region Transfert clients

            ListPriceCategoryAvailable = new ObservableCollection<PriceCategory>();
            Model.Local.GroupRepository GroupRepository = new Model.Local.GroupRepository();
            foreach (Model.Sage.P_CATTARIF P_CATTARIF in new Model.Sage.P_CATTARIFRepository().ListIntituleNotNullOrderByIntitule())
                if (GroupRepository.ListCatTarifSage().Contains(P_CATTARIF.cbMarq))
                    ListPriceCategoryAvailable.Add(new PriceCategory(P_CATTARIF));

            ListMailToContactService = new ObservableCollection<MailToContactService>();
            foreach (Model.Sage.P_SERVICECPTA service in new Model.Sage.P_SERVICECPTARepository().ListIntituleNotNull())
                ListMailToContactService.Add(new MailToContactService(service));

            ListMailAccountIdentification = new ObservableCollection<MailAccountIdentification>();
            foreach (Core.Parametres.MailAccountIdentification mail in Enum.GetValues(typeof(Core.Parametres.MailAccountIdentification)))
                ListMailAccountIdentification.Add(new MailAccountIdentification(mail));
            SelectedMailAccountIdentification = ListMailAccountIdentification.FirstOrDefault(r => r._MailAccountIdentification == Core.Global.GetConfig().TransfertMailAccountIdentification);
            SelectedMailAccountIdentificationContactService = ListMailToContactService.FirstOrDefault(r => r.P_SERVICECPTA.cbMarq == Core.Global.GetConfig().TransfertMailAccountContactService);

            RandomPasswordLength = new ObservableCollection<int>();
            for (int i = 6; i <= 32; i++)
                RandomPasswordLength.Add(i);
            SelectedRandomPasswordLength = (Core.Global.GetConfig().TransfertRandomPasswordLength >= 6 && Core.Global.GetConfig().TransfertRandomPasswordLength <= 32) ? Core.Global.GetConfig().TransfertRandomPasswordLength : 6;

            ListMailNotification = new ObservableCollection<MailNotification>();
            foreach (Core.Parametres.MailNotification listmail in Enum.GetValues(typeof(Core.Parametres.MailNotification)))
                ListMailNotification.Add(new MailNotification(listmail));
            SelectedMailNotification = ListMailNotification.FirstOrDefault(r => r._MailNotification == Core.Global.GetConfig().TransfertNotifyAccountAddress);
            
            ListMailToContactType = new ObservableCollection<MailToContactType>();
            foreach (Model.Sage.P_CONTACT type in new Model.Sage.P_CONTACTRepository().ListIntituleNotNull())
                ListMailToContactType.Add(new MailToContactType(type));

            ListSageAddressSend = new ObservableCollection<SageAddressSend>();
            foreach (Core.Parametres.SageAddressSend listmail in Enum.GetValues(typeof(Core.Parametres.SageAddressSend)))
                ListSageAddressSend.Add(new SageAddressSend(listmail));

            ListLockPhoneNumber = new ObservableCollection<LockPhoneNumber>();
            foreach (Core.Parametres.LockPhoneNumber lockaction in Enum.GetValues(typeof(Core.Parametres.LockPhoneNumber)))
                ListLockPhoneNumber.Add(new LockPhoneNumber(lockaction));

            ListAliasValue = new ObservableCollection<AliasValue>();
            foreach (Core.Parametres.AliasValue value in Enum.GetValues(typeof(Core.Parametres.AliasValue)))
                ListAliasValue.Add(new AliasValue(value));
            SelectedAliasValue = ListAliasValue.FirstOrDefault(r => r._AliasValue == Core.Global.GetConfig().TransfertAliasValue);

            ListLastNameValue = new ObservableCollection<LastNameValue>();
            foreach (Core.Parametres.LastNameValue value in Enum.GetValues(typeof(Core.Parametres.LastNameValue)))
                ListLastNameValue.Add(new LastNameValue(value));
            SelectedLastNameValue = ListLastNameValue.FirstOrDefault(r => r._LastNameValue == Core.Global.GetConfig().TransfertLastNameValue);

            ListFirstNameValue = new ObservableCollection<FirstNameValue>();
            foreach (Core.Parametres.FirstNameValue value in Enum.GetValues(typeof(Core.Parametres.FirstNameValue)))
                ListFirstNameValue.Add(new FirstNameValue(value));
            SelectedFirstNameValue = ListFirstNameValue.FirstOrDefault(r => r._FirstNameValue == Core.Global.GetConfig().TransfertFirstNameValue);

            ListCompanyValue = new ObservableCollection<CompanyValue>();
            foreach (Core.Parametres.CompanyValue value in Enum.GetValues(typeof(Core.Parametres.CompanyValue)))
                ListCompanyValue.Add(new CompanyValue(value));
            SelectedCompanyValue = ListCompanyValue.FirstOrDefault(r => r._CompanyValue == Core.Global.GetConfig().TransfertCompanyValue);

            ClientSeparateurIntitule = Core.Global.GetConfig().TransfertClientSeparateurIntitule;

            ListRegexMail = new ObservableCollection<RegexMail>();
            foreach (Core.Parametres.RegexMail regex in Enum.GetValues(typeof(Core.Parametres.RegexMail)))
                ListRegexMail.Add(new RegexMail(regex));
            SelectedRegexMail = ListRegexMail.FirstOrDefault(r => r._RegexMailLevel == Core.Global.GetConfig().RegexMailLevel);

            // appel direct à la variable sinon affichage constant de l'alerte lorsque activé 
            transfertClientNameIncludeNumbers = Core.Global.GetConfig().TransfertNameIncludeNumbers;

            #endregion

            #region images

            ListImageStorageMode = new ObservableCollection<ImageStorageMode>();
            foreach (Core.Parametres.ImageStorageMode value in Enum.GetValues(typeof(Core.Parametres.ImageStorageMode)))
                ListImageStorageMode.Add(new ImageStorageMode(value));
            SelectedImageStorageMode = ListImageStorageMode.FirstOrDefault(r => r._ImageStorageMode == Core.Global.GetConfig().ConfigImageStorageMode);

            #endregion

            #region infos libres article

            ListInformationLibreValeursMode = new ObservableCollection<InformationLibreValeursMode>();
            foreach (Core.Parametres.InformationLibreValeursMode value in Enum.GetValues(typeof(Core.Parametres.InformationLibreValeursMode)))
                ListInformationLibreValeursMode.Add(new InformationLibreValeursMode(value));

            ListFeature = new ObservableCollection<Model.Prestashop.PsFeatureLang>(new Model.Prestashop.PsFeatureLangRepository().ListLang(Core.Global.Lang, Core.Global.CurrentShop.IDShop).OrderBy(f => f.Name));

            ListInformationLibre = new ObservableCollection<InformationLibre>();

            foreach (Model.Sage.cbSysLibre SageInfoLibre in new Model.Sage.cbSysLibreRepository().ListFileOrderByPosition(Model.Sage.cbSysLibreRepository.CB_File.F_ARTICLE))
                ListInformationLibre.Add(new InformationLibre(SageInfoLibre, ListInformationLibreValeursMode, ListFeature));

            SuppressionAutoCaracteristique = Core.Global.GetConfig().ArticleSuppressionAutoCaracteristique;

            #endregion

            #region infos libres catalogue article
            ListInformationLibreArticle = new ObservableCollection<Model.Local.InformationLibreArticle>();
            foreach (Model.Local.InformationLibreArticle InformationLibreArticle in new Model.Local.InformationLibreArticleRepository().List())
                ListInformationLibreArticle.Add(InformationLibreArticle);

            #endregion

            FiltreDatePrixPrestashop = Core.Global.GetConfig().ArticleFiltreDatePrixPrestashop;

            RegleBasePrixSpecifique = Core.Global.GetConfig().ArticleSpecificPriceLetBasePriceRule;

            #region INACTIF mode ecriture remises

            ListLigneRemiseMode = new ObservableCollection<LigneRemiseMode>();
            foreach (Core.Parametres.LigneRemiseMode value in Enum.GetValues(typeof(Core.Parametres.LigneRemiseMode)))
                ListLigneRemiseMode.Add(new LigneRemiseMode(value));
            SelectedLigneRemiseMode = ListLigneRemiseMode.FirstOrDefault(l => l._LigneRemiseMode == Core.Global.GetConfig().LigneRemiseMode);

            #endregion

            #region Taxes

            ListTaxSage = new ObservableCollection<TaxSage>();
            foreach (Core.Parametres.TaxSage value in Enum.GetValues(typeof(Core.Parametres.TaxSage)))
                ListTaxSage.Add(new TaxSage(value));
            SelectedTaxSageTVA = ListTaxSage.FirstOrDefault(t => t._TaxSage == Core.Global.GetConfig().TaxSageTVA);
            SelectedTaxSageEco = ListTaxSage.FirstOrDefault(t => t._TaxSage == Core.Global.GetConfig().TaxSageEco);

            #endregion

            #region filtres commandes

            CommandeFiltreDate = Core.Global.GetConfig().ConfigCommandeFiltreDate;
            ClientFiltreCommande = Core.Global.GetConfig().ConfigClientFiltreCommande;

            #endregion

            PoidsSynchroStock = Core.Global.GetConfig().MajPoidsSynchroStock;

            #region conversion en poids énumérés de gamme

            ListGamme = new ObservableCollection<Model.Sage.P_GAMME>(new Model.Sage.P_GAMMERepository().ListIntituleNotNull());
            foreach (int IdCombination in Core.Global.GetConfig().CombinationWithWeightConversion)
                if (ListGamme.Count(c => c.cbMarq == IdCombination) == 1)
                    ListGamme.FirstOrDefault(c => c.cbMarq == IdCombination).WeightConversion = true;

            List<Model.Sage.cbSysLibre> list = new List<Model.Sage.cbSysLibre>();
            list.Add(new Model.Sage.cbSysLibre());
            list.AddRange(new Model.Sage.cbSysLibreRepository().ListFileOrderByPosition(Model.Sage.cbSysLibreRepository.CB_File.F_ARTICLE)
                .Where(i => i.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageMontant
                    || i.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageValeur
                    || i.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageText
                    || i.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageTable));
            ListInfolibreCoefficient = new ObservableCollection<Model.Sage.cbSysLibre>(list);
            SelectedInfolibreCoefficient = ListInfolibreCoefficient.FirstOrDefault(i => i.CB_Name == Core.Global.GetConfig().InformationLibreCoefficientConversion);

            #endregion

            #region stats article

            // <JG> 01/08/2013 ajout gestion statistiques
            ListStatistiqueArticle = new ObservableCollection<StatistiqueArticle>();
            foreach (Model.Sage.P_INTSTATART SageStatArticle in new Model.Sage.P_INTSTATARTRepository().List())
                ListStatistiqueArticle.Add(new StatistiqueArticle(SageStatArticle, ListInformationLibreValeursMode, ListFeature));

            #endregion

            #region numérotation clients

            // <JG> 05/08/2013 ajout options de numérotation des comptes clients
            ClientNumPrefixe = Core.Global.GetConfig().ConfigClientNumPrefixe;
            ClientNumLongueurNom = Core.Global.GetConfig().ConfigClientNumLongueurNom;

            ListTypeNom = new ObservableCollection<NameNumComponent>();
            foreach (Core.Parametres.NameNumComponent value in Enum.GetValues(typeof(Core.Parametres.NameNumComponent)))
                ListTypeNom.Add(new NameNumComponent(value));
            SelectedTypeNom = ListTypeNom.FirstOrDefault(t => t._NameNumComponent == Core.Global.GetConfig().ConfigClientNumTypeNom);

            ClientNumLongueurNumero = Core.Global.GetConfig().ConfigClientNumLongueurNumero;

            ListTypeNumero = new ObservableCollection<NumberNumComponent>();
            foreach (Core.Parametres.NumberNumComponent value in Enum.GetValues(typeof(Core.Parametres.NumberNumComponent)))
                ListTypeNumero.Add(new NumberNumComponent(value));
            SelectedTypeNumero = ListTypeNumero.FirstOrDefault(t => t._NumberNumComponent == Core.Global.GetConfig().ConfigClientNumTypeNumero);

            ListTypeCompteur = new ObservableCollection<CounterType>();
            foreach (Core.Parametres.CounterType value in Enum.GetValues(typeof(Core.Parametres.CounterType)))
                ListTypeCompteur.Add(new CounterType(value));
            SelectedTypeCompteur = ListTypeCompteur.FirstOrDefault(t => t._CounterType == Core.Global.GetConfig().ConfigClientNumTypeCompteur);

            ClientNumDebutCompteur = Core.Global.GetConfig().ConfigClientNumDebutCompteur;
            ClientNumComposition = Core.Global.GetConfig().ConfigClientNumComposition;

            ClientNumDepartementRemplacerCodeISO = Core.Global.GetConfig().ConfigClientNumDepartementRemplacerCodeISO;
            if (Core.Temp.ListPsCountryLang == null || Core.Temp.ListPsCountryLang.Count == 0)
                Core.Temp.ListPsCountryLang = new Model.Prestashop.PsCountryLangRepository().ListActive(1, Core.Global.CurrentShop.IDShop).Where(c => c.IDLang == Core.Global.Lang).ToList();
            CountryRepository = new Model.Local.CountryRepository();
            ListCountryReplaceISOCode = CountryRepository.List();
            foreach (Model.Prestashop.PsCountryLang PsCountryLang in Core.Temp.ListPsCountryLang.Where(pcl => ListCountryReplaceISOCode.Count(c => c.Pre_IdCountry == (int)pcl.IDCountry) == 0))
                if (!CountryRepository.ExistCountry(PsCountryLang.IDCountry))
                    CountryRepository.Add(new Model.Local.Country()
                    {
                        Pre_IdCountry = (int)PsCountryLang.IDCountry,
                    });
            CountryRepository = new Model.Local.CountryRepository();
            ListCountryReplaceISOCode = CountryRepository.List().OrderBy(c => c.PsCountryLang.Name).ToList();

            #endregion

            ListReplacement = new ObservableCollection<Model.Local.Replacement>(new Model.Local.ReplacementRepository().List());

            #region renseignement marques/fournisseurs

            ListSageStatistiqueArticle = new ObservableCollection<Model.Sage.P_INTSTATART>(new Model.Sage.P_INTSTATARTRepository().List());
            ListSageInfolibreArticleTextTable = new ObservableCollection<Model.Sage.cbSysLibre>(new Model.Sage.cbSysLibreRepository().ListFileOrderByPosition(Model.Sage.cbSysLibreRepository.CB_File.F_ARTICLE)
                .Where(i => i.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageText
                    || i.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageTable).ToList());

            MarqueStatActif = Core.Global.GetConfig().MarqueAutoStatistiqueActif;
            MarqueInfolibreActif = Core.Global.GetConfig().MarqueAutoInfolibreActif;
            FournisseurStatActif = Core.Global.GetConfig().FournisseurAutoStatistiqueActif;
            FournisseurInfolibreActif = Core.Global.GetConfig().FournisseurAutoInfolibreActif;
            if (MarqueStatActif)
                SelectedMarqueSageStatistiqueArticle = ListSageStatistiqueArticle.FirstOrDefault(s => s.P_IntStatArt1 == Core.Global.GetConfig().MarqueAutoStatistiqueName);
            if (MarqueInfolibreActif)
                SelectedMarqueSageInfolibreArticle = ListSageInfolibreArticleTextTable.FirstOrDefault(i => i.CB_Name == Core.Global.GetConfig().MarqueAutoInfolibreName);
            if (FournisseurStatActif)
                SelectedFournisseurSageStatistiqueArticle = ListSageStatistiqueArticle.FirstOrDefault(s => s.P_IntStatArt1 == Core.Global.GetConfig().FournisseurAutoStatistiqueName);
            if (FournisseurInfolibreActif)
                SelectedFournisseurSageInfolibreArticle = ListSageInfolibreArticleTextTable.FirstOrDefault(i => i.CB_Name == Core.Global.GetConfig().FournisseurAutoInfolibreName);

            #endregion

            #region stats et infos libres client

            CanActiveCustomerFeatureModule = Core.Global.ExistCustomerFeatureModule();
            ModuleStatInfolibreClientActif = Core.Global.GetConfig().StatInfolibreClientActif;
            if (CanActiveCustomerFeatureModule)
            {

                // <JG> 03/10/2013
                ListCustomerFeature = new ObservableCollection<Model.Prestashop.PsCustomerFeatureLang>(new Model.Prestashop.PsCustomerFeatureLangRepository().ListLang(Core.Global.Lang, Core.Global.CurrentShop.IDShop).OrderBy(f => f.Name));

                ListInformationLibreClient = new ObservableCollection<InformationLibreClient>();
                foreach (Model.Sage.cbSysLibre SageInfoLibre in new Model.Sage.cbSysLibreRepository().ListFileOrderByPosition(Model.Sage.cbSysLibreRepository.CB_File.F_COMPTET))
                    ListInformationLibreClient.Add(new InformationLibreClient(SageInfoLibre, ListInformationLibreValeursMode, ListCustomerFeature));

                ListStatistiqueClient = new ObservableCollection<StatistiqueClient>();
                foreach (Model.Sage.P_STATISTIQUE SageStatClient in new Model.Sage.P_STATISTIQUERepository().List())
                    ListStatistiqueClient.Add(new StatistiqueClient(SageStatClient, ListInformationLibreValeursMode, ListCustomerFeature));
            }
            else
            {
                ListCustomerFeature = new ObservableCollection<Model.Prestashop.PsCustomerFeatureLang>();
                ListInformationLibreClient = new ObservableCollection<InformationLibreClient>();
                ListStatistiqueClient = new ObservableCollection<StatistiqueClient>();
            }

            #endregion

            ImportConditionnementActif = Core.Global.GetConfig().ArticleImportConditionnementActif;
            ConditionnementQuantiteToUPC = Core.Global.GetConfig().ArticleConditionnementQuantiteToUPC;
            LimiteStockConditionnementMini = Core.Global.GetConfig().LimiteStockConditionnement;

            StockArticleContremarqueActif = Core.Global.GetConfig().ArticleContremarqueStockActif;

            StockNegatifZero = Core.Global.GetConfig().ArticleStockNegatifZero;
            StockNegatifZeroParDepot = Core.Global.GetConfig().ArticleStockNegatifZeroParDepot;

            CanDeleteCatalogProductAssociation = Core.Global.GetConfig().DeleteCatalogProductAssociation;

            DateDispoInfoLibreActif = Core.Global.GetConfig().ArticleDateDispoInfoLibreActif;
            List<Model.Sage.cbSysLibre> list_temp = new List<Model.Sage.cbSysLibre>();
            list_temp.Add(new Model.Sage.cbSysLibre());
            list_temp.AddRange(new Model.Sage.cbSysLibreRepository().ListFileOrderByPosition(Model.Sage.cbSysLibreRepository.CB_File.F_ARTICLE)
                .Where(i => i.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageSmallDate
                    || i.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageDate
                    || i.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageText
                    || i.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageTable));
            ListDateDispoInfoLibre = new ObservableCollection<Model.Sage.cbSysLibre>(list_temp);
            SelectedDateDispoInfoLibre = ListDateDispoInfoLibre.FirstOrDefault(i => i.CB_Name == Core.Global.GetConfig().ArticleDateDispoInfoLibreName);

            QuantiteMiniActif = Core.Global.GetConfig().ArticleQuantiteMiniActif;
            QuantiteMiniConditionnement = Core.Global.GetConfig().ArticleQuantiteMiniConditionnement;
            QuantiteMiniUniteVente = Core.Global.GetConfig().ArticleQuantiteMiniUniteVente;

            TransfertInfosFournisseurActif = Core.Global.GetConfig().ArticleTransfertInfosFournisseurActif;

            RedirectionComposition = Core.Global.GetConfig().ArticleRedirectionCompositionActif;

            UpdateAdresseFacturation = Core.Global.GetConfig().CommandeUpdateAdresseFacturation;
            InsertFacturationEntete = Core.Global.GetConfig().CommandeInsertFacturationEntete;

            CopyReferencePrestashop = Core.Global.GetConfig().CommandeReferencePrestaShop;
            ForceNumeroFactureSage = Core.Global.GetConfig().CommandeNumeroFactureSageForceActif;
            JoursAutomateStatut = Core.Global.GetConfig().CommandeStatutJoursAutomate;

            #region Tracking

            TrackingEnteteActif = Core.Global.GetConfig().CommandeTrackingEnteteActif;
            TrackingInfolibreActif = Core.Global.GetConfig().CommandeTrackingInfolibreActif;
            ListSageFieldDocumentEntete = new ObservableCollection<FieldDocumentEntete>();
            foreach (Core.Parametres.FieldDocumentEntete field in Enum.GetValues(typeof(Core.Parametres.FieldDocumentEntete)))
                ListSageFieldDocumentEntete.Add(new FieldDocumentEntete(field));
            SelectedTrackingFieldDocumentEntete = ListSageFieldDocumentEntete.FirstOrDefault(r => r._FieldDocumentEnteteValue == Core.Global.GetConfig().CommandeTrackingEntete);

            ListSageInfolibreDocumentTextTable = new ObservableCollection<Model.Sage.cbSysLibre>(new Model.Sage.cbSysLibreRepository().ListFileOrderByPosition(Model.Sage.cbSysLibreRepository.CB_File.F_DOCENTETE)
                .Where(i => i.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageText
                    || i.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageTable));
            SelectedTrackingInfolibreDocument = ListSageInfolibreDocumentTextTable.FirstOrDefault(i => i.CB_Name == Core.Global.GetConfig().CommandeTrackingInfolibre);

            #endregion

            #region Module OleaPromo

            CanActiveOleaPromoModule = Core.Global.ExistOleaPromoModule();
            OleaPromoActif = Core.Global.GetConfig().ModuleOleaPromoActif;
            OleaSuffixeGratuit = Core.Global.GetConfig().ModuleOleaSuffixeGratuit;

            #endregion

            #region Reglement

            ReglementEcheancierActif = Core.Global.GetConfig().ModeReglementEcheancierActif;
            ReglementLibellePartielActif = Core.Global.GetConfig().ReglementLibellePartielActif;

            #endregion

            #region Outils

            CronSynchroArticleURL = Core.Global.GetConfig().ConfigCronSynchroArticleURL;
            LogChronoSynchroStockPriceActif = Core.Global.GetConfig().ChronoSynchroStockPriceActif;
            RefreshTempCustomerListDisabled = Core.Global.GetConfig().ConfigRefreshTempCustomerListDisabled;

            UnlockProcessorCore = Core.Global.GetConfig().ConfigUnlockProcessorCore;
            AllocatedProcessorCore = Core.Global.GetConfig().ConfigAllocatedProcessorCore;

            CrystalForceConnectionInfoOnSubReports = Core.Global.GetConfig().CrystalForceConnectionInfoOnSubReports;

            #region Cron/Scripts

            CronArticleURL = Core.Global.GetConfig().CronArticleURL;
            CronArticleBalise = Core.Global.GetConfig().CronArticleBalise;
            CronArticleTimeout = Core.Global.GetConfig().CronArticleTimeout;
			CronCommandeURL = Core.Global.GetConfig().CronCommandeURL;
			CronCommandeBalise = Core.Global.GetConfig().CronCommandeBalise;
			CronCommandeTimeout = Core.Global.GetConfig().CronCommandeTimeout;

            #endregion

            #endregion

            #region Interface

            ListProductFilterActiveDefault = new ObservableCollection<ProductFilterActiveDefault>();
            foreach (Core.Parametres.ProductFilterActiveDefault filter in Enum.GetValues(typeof(Core.Parametres.ProductFilterActiveDefault)))
                ListProductFilterActiveDefault.Add(new ProductFilterActiveDefault(filter));
            SelectedProductFilterActiveDefault = ListProductFilterActiveDefault.FirstOrDefault(r => r._ProductFilterActiveDefault == Core.Global.GetConfig().UIProductFilterActiveDefault);

            ProductUpdateValidationDisabled = Core.Global.GetConfig().UIProductUpdateValidationDisabled;

            WindowsMaximized = Core.Global.GetConfig().UIMaximizeWindow;

            SleepTimeWYSIWYG = Core.Global.GetConfig().UISleepTimeWYSIWYG;
            DisabledWYSIWYG = Core.Global.GetConfig().UIDisabledWYSIWYG;

            IE11EmulationModeDisabled = Core.Global.GetConfig().UIIE11EmulationModeDisabled;

            #endregion

            ListArticleHorsStock = new ObservableCollection<Model.Sage.F_ARTICLE_Light>(new Model.Sage.F_ARTICLERepository().ListLightHorsStock());

			#region Frais de port

			CommandeLigneFraisPort = Core.Global.GetConfig().LigneFraisPort;
			SelectedArticlePort = (ListArticleHorsStock.Count(a => a.AR_Ref == Core.Global.GetConfig().LigneArticlePort) == 1)
									? ListArticleHorsStock.FirstOrDefault(a => a.AR_Ref == Core.Global.GetConfig().LigneArticlePort)
									: null;
			#endregion

			#region Date de livraison

			commandeDateLivraisonMode = Core.Global.GetConfig().DateLivraisonMode;
			commandeDateLivraisonJours = Core.Global.GetConfig().DateLivraisonJours;

			#endregion

			#region Bons de réductions

			SelectedArticleReduction = (ListArticleHorsStock.Count(a => a.AR_Ref == Core.Global.GetConfig().CommandeArticleReduction) == 1)
                                    ? ListArticleHorsStock.FirstOrDefault(a => a.AR_Ref == Core.Global.GetConfig().CommandeArticleReduction)
                                    : null;
            #endregion

            #region Commentaires

            CommandeCommentaireBoutiqueActif = Core.Global.GetConfig().CommentaireBoutiqueActif;
            CommandeCommentaireBoutiqueTexte = Core.Global.GetConfig().CommentaireBoutiqueTexte;
            CommandeCommentaireNumeroActif = Core.Global.GetConfig().CommentaireNumeroActif;
            CommandeCommentaireNumeroTexte = Core.Global.GetConfig().CommentaireNumeroTexte;
            CommandeCommentaireReferencePaiementActif = Core.Global.GetConfig().CommentaireReferencePaiementActif;
            CommandeCommentaireReferencePaiementTexte = Core.Global.GetConfig().CommentaireReferencePaiementTexte;
            CommandeCommentaireDateActif = Core.Global.GetConfig().CommentaireDateActif;
            CommandeCommentaireDateTexte = Core.Global.GetConfig().CommentaireDateTexte;

            CommandeCommentaireClientActif = Core.Global.GetConfig().CommentaireClientActif;
            CommandeCommentaireClientTexte = Core.Global.GetConfig().CommentaireClientTexte;
            CommandeCommentaireAdresseFacturationActif = Core.Global.GetConfig().CommentaireAdresseFacturationActif;
            CommandeCommentaireAdresseFacturationTexte = Core.Global.GetConfig().CommentaireAdresseFacturationTexte;
            CommandeCommentaireAdresseLivraisonActif = Core.Global.GetConfig().CommentaireAdresseLivraisonActif;
            CommandeCommentaireAdresseLivraisonTexte = Core.Global.GetConfig().CommentaireAdresseLivraisonTexte;

            CommandeCommentaireLibre1Actif = Core.Global.GetConfig().CommentaireLibre1Actif;
            CommandeCommentaireLibre1Texte = Core.Global.GetConfig().CommentaireLibre1Texte;
            CommandeCommentaireLibre2Actif = Core.Global.GetConfig().CommentaireLibre2Actif;
            CommandeCommentaireLibre2Texte = Core.Global.GetConfig().CommentaireLibre2Texte;
            CommandeCommentaireLibre3Actif = Core.Global.GetConfig().CommentaireLibre3Actif;
            CommandeCommentaireLibre3Texte = Core.Global.GetConfig().CommentaireLibre3Texte;

            CommentStart = Core.Global.GetConfig().CommentaireDebutDocument;
            CommentEnds = Core.Global.GetConfig().CommentaireFinDocument;

            #endregion

            #region Packaging
            List<Model.Sage.cbSysLibre> listpackaging = new List<Model.Sage.cbSysLibre>();
            listpackaging.Add(new Model.Sage.cbSysLibre());
            listpackaging.AddRange(new Model.Sage.cbSysLibreRepository().ListFileOrderByPosition(Model.Sage.cbSysLibreRepository.CB_File.F_ARTICLE)
                .Where(i => i.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageText
                    || i.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageTable));
            ListInfolibrePackaging = new ObservableCollection<Model.Sage.cbSysLibre>(list);
            SelectedInfolibrePackaging = ListInfolibrePackaging.FirstOrDefault(i => i.CB_Name == Core.Global.GetConfig().ArticleInfolibrePackaging);

            ListArticlePackaging = new ObservableCollection<Model.Sage.F_ARTICLE_Light>(new Model.Sage.F_ARTICLERepository().ListLightNomenclature());
            SelectedArticlePackaging = ListArticlePackaging.FirstOrDefault(a => a.AR_Ref == Core.Global.GetConfig().CommandeArticlePackaging);
            #endregion

            #region Module Preorder

            CanActivePreorderModule = Core.Global.ExistPreorderModule();
            PreorderActif = Core.Global.GetConfig().ModulePreorderActif;
            SelectedInfolibrePreorder = ListSageInfolibreArticleTextTable.FirstOrDefault(i => i.CB_Name == Core.Global.GetConfig().ModulePreorderInfolibreName);
            PreorderInfolibreValue = Core.Global.GetConfig().ModulePreorderInfolibreValue;
            List<Model.Prestashop.ProductLight> temp_list = new Model.Prestashop.PsProductRepository().ListLightPrecommande();
            List<int> local_id = new Model.Local.ArticleRepository().ListPrestashop();
            temp_list = temp_list.Where(p => local_id.Count(a => a == p.id_product) == 0).ToList();
            ListPreorderPrestashopProduct = new ObservableCollection<Model.Prestashop.ProductLight>(temp_list);
            SelectedPreorderPrestashopProduct = ListPreorderPrestashopProduct.FirstOrDefault(p => p.id_product == Core.Global.GetConfig().ModulePreorderPrestashopProduct);
            ListPreorderPrestashopOrderState = new ObservableCollection<Model.Prestashop.PsOrderStateLang>(new Model.Prestashop.PsOrderStateLangRepository().ListLang(Core.Global.Lang));
            SelectedPreorderPrestashopOrderState = ListPreorderPrestashopOrderState.FirstOrDefault(p => p.IDOrderState == Core.Global.GetConfig().ModulePreorderPrestashopOrderState);

            ListInfolibreEntete = new ObservableCollection<Model.Sage.cbSysLibre>(new Model.Sage.cbSysLibreRepository().ListFileOrderByPosition(Model.Sage.cbSysLibreRepository.CB_File.F_DOCENTETE)
                .Where(i => i.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageText
                    || i.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageTable).ToList());
            SelectedInfolibreEntetePreorder = ListInfolibreEntete.FirstOrDefault(i => i.CB_Name == Core.Global.GetConfig().ModulePreorderInfolibreEnteteName);
            PreorderInfolibreEnteteValue = Core.Global.GetConfig().ModulePreorderInfolibreEnteteValue;

            #endregion

            #region Informations Article Sage

            ListSageInfoArticle = new ObservableCollection<InformationArticle>();
            foreach (Core.Parametres.SageInfoArticle infosage in Enum.GetValues(typeof(Core.Parametres.SageInfoArticle)))
                ListSageInfoArticle.Add(new InformationArticle(new SageInfoArticle(infosage), ListInformationLibreValeursMode, ListFeature));

            #endregion

            #region Module AECInvoiceHistory

            CanActiveAECInvoiceHistory = Core.Global.ExistAECInvoiceHistoryModule();
            AECInvoiceHistoryActif = Core.Global.GetConfig().ModuleAECInvoiceHistoryActif;

            ListSageInfolibreClientTextTable = new ObservableCollection<Model.Sage.cbSysLibre>(new Model.Sage.cbSysLibreRepository().ListFileOrderByPosition(Model.Sage.cbSysLibreRepository.CB_File.F_COMPTET)
                .Where(i => i.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageText
                    || i.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageTable).ToList());
            SelectedInfolibreInvoiceHistorySendMail = ListSageInfolibreClientTextTable.FirstOrDefault(il => il.CB_Name == Core.Global.GetConfig().ModuleAECInvoiceHistoryInfoLibreClientSendMail);
            InfolibreInvoiceHistorySendMailValue = Core.Global.GetConfig().ModuleAECInvoiceHistoryInfoLibreClientSendMailValue;

            AECInvoiceHistoryArchivePDF = Core.Global.GetConfig().ModuleAECInvoiceHistoryArchivePDFActive;
            AECInvoiceHistoryArchivePDFFolder = Core.Global.GetConfig().ModuleAECInvoiceHistoryArchivePDFFolder;

            #endregion

            #region Module AECStock

            CanActiveAECStockModule = Core.Global.ExistAECStockModule();
            AECStockActif = Core.Global.GetConfig().ModuleAECStockActif && Core.Global.ExistAECStockModule();

            #endregion

            #region Module AECCollaborateur

            CanActiveAECCollaborateurModule = Core.Global.ExistAECCollaborateurModule();
            AECCollaborateurActif = Core.Global.GetConfig().ModuleAECCollaborateurActif && Core.Global.ExistAECCollaborateurModule();

            #endregion

            #region Module AECPaiement

            CanActiveAECPaiementModule = Core.Global.ExistAECPaiementModule();
            AECPaiementActif = Core.Global.GetConfig().ModuleAECPaiementActif && Core.Global.ExistAECPaiementModule();

            #endregion

            #region AECCustomerOutstanding

            CanActiveAECCustomerOutstandingModule = Core.Global.ExistAECCustomerOutstandingModule();
            AECCustomerOutstandingActif = Core.Global.GetConfig().ModuleAECCustomerOutstandingActif && Core.Global.ExistAECCustomerOutstandingModule();

            #endregion

            #region AECCustomerInfo

            CanActiveAECCustomerInfoModule = Core.Global.ExistCustomerInfoModule();
            AECCustomerInfoActif = Core.Global.GetConfig().ModuleAECCustomerInfoActif && Core.Global.ExistCustomerInfoModule();

            #endregion

            #region Module GroupCRisque

            ListCodeRisque = new ObservableCollection<Model.Sage.P_CRISQUE>(new Model.Sage.P_CRISQUERepository().ListIntituleNotNull());
            ListPsGroup = new ObservableCollection<Model.Prestashop.PsGroupLang>(new Model.Prestashop.PsGroupLangRepository().ListLang(Core.Global.Lang));
            Model.Local.Group_CRisqueRepository Group_CRisqueRepository = new Model.Local.Group_CRisqueRepository();
            foreach (Model.Sage.P_CRISQUE P_CRISQUE in ListCodeRisque)
            {
                if (Group_CRisqueRepository.ExistCRisque(P_CRISQUE.cbMarq))
                {
                    Model.Local.Group_CRisque Group_CRisque = Group_CRisqueRepository.ReadCRisque(P_CRISQUE.cbMarq);
                    P_CRISQUE.SelectedPsGroup = ListPsGroup.FirstOrDefault(g => g.IDGroup == (uint)Group_CRisque.Grp_Pre_Id);
                    P_CRISQUE.SelectedPsGroupDefault = listPsGroup.FirstOrDefault(g => g.IDGroup == (uint)Group_CRisque.Grp_PreId_Default);
                    P_CRISQUE.LockCondition = Group_CRisque.Grp_LockCondition;
                }
            }

            #endregion

            #region Module Portfolio Customer Employee

            CanActivePortfolioCustomerEmployee = Core.Global.ExistPortfolioCustomerEmployeeModule();
            PortfolioCustomerEmployeeActif = Core.Global.GetConfig().ModulePortfolioCustomerEmployeeActif;

            ListCollaborateur = new ObservableCollection<Model.Sage.F_COLLABORATEUR>(new Model.Sage.F_COLLABORATEURRepository().ListVendeur());

            Model.Prestashop.PsConfigurationRepository PsConfigurationRepository = new Model.Prestashop.PsConfigurationRepository();
            uint IDProfile = 0;
            if (PsConfigurationRepository.ExistName(Core.Global.PrestaShopCustomerPortFolioProfilKey))
            {
                Model.Prestashop.PsConfiguration PsConfiguration = PsConfigurationRepository.ReadName(Core.Global.PrestaShopCustomerPortFolioProfilKey);
                if (!string.IsNullOrWhiteSpace(PsConfiguration.Value) && Core.Global.IsIntegerUnsigned(PsConfiguration.Value))
                {
                    IDProfile = uint.Parse(PsConfiguration.Value);
                }
            }
            Model.Prestashop.PsEmployeeRepository PsEmployeeRepository = new Model.Prestashop.PsEmployeeRepository();
            ListPsEmployee = new ObservableCollection<Model.Prestashop.PsEmployee>(
                (IDProfile != 0)
                ? PsEmployeeRepository.List(IDProfile)
                : PsEmployeeRepository.List());

            Model.Local.Employee_CollaborateurRepository Employee_CollaborateurRepository = new Model.Local.Employee_CollaborateurRepository();
            foreach (Model.Sage.F_COLLABORATEUR F_COLLABORATEUR in ListCollaborateur)
            {
                if (Employee_CollaborateurRepository.ExistCollaborateur(F_COLLABORATEUR.CO_No.Value))
                {
                    Model.Local.Employee_Collaborateur Employee_Collaborateur = Employee_CollaborateurRepository.ReadCollaborateur(F_COLLABORATEUR.CO_No.Value);
                    F_COLLABORATEUR.SelectedPsEmployee = ListPsEmployee.FirstOrDefault(g => g.IDEmployee == (uint)Employee_Collaborateur.IdEmployee);
                }
            }

            #endregion

            #region Module SoColissimo

            CanActiveSoColissimoDelivery = Core.Global.ExistSoColissimoDeliveryModule();
            SoColissimoDeliveryActive = Core.Global.GetConfig().ModuleSoColissimoDeliveryActive;
            SoColissimoInfolibreTypePointActive = Core.Global.GetConfig().ModuleSoColissimoInfolibreTypePointActive;
            SoColissimoInfolibreDestinataireActive = Core.Global.GetConfig().ModuleSoColissimoInfolibreDestinataireActive;
            SoColissimoListInfolibreEntete = new ObservableCollection<Model.Sage.cbSysLibre>(new Model.Sage.cbSysLibreRepository().ListFileOrderByPosition(Model.Sage.cbSysLibreRepository.CB_File.F_DOCENTETE)
                .Where(i => i.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageText
                    || i.CB_Type == (short)Model.Sage.cbSysLibreRepository.CB_Type.SageTable).ToList());

            SelectedInfolibreEnteteSoColissimoTypePoint = SoColissimoListInfolibreEntete.FirstOrDefault(i => i.CB_Name == Core.Global.GetConfig().ModuleSoColissimoInfolibreEnteteTypePointName);
            SelectedInfolibreEnteteSoColissimoDestinataire = SoColissimoListInfolibreEntete.FirstOrDefault(i => i.CB_Name == Core.Global.GetConfig().ModuleSoColissimoInfolibreEnteteDestinataireName);
            SoColissimoReplacePhoneActive = Core.Global.GetConfig().ModuleSoColissimoReplacePhoneActive;
            SoColissimoReplaceAddressNameActive = Core.Global.GetConfig().ModuleSoColissimoReplaceAddressNameActive;

			#endregion

			#region Module DWFProductGuiderates

			CanActiveCanActiveDWFProductGuideratesModule = Core.Global.ExistCanActiveDWFProductGuideratesModule();
			DWFProductGuideratesActif = Core.Global.GetConfig().ModuleDWFProductGuideratesActif && CanActiveCanActiveDWFProductGuideratesModule;

			#endregion

			#region Module DWFProductExtraFields

			CanActiveCanActiveDWFProductExtraFieldsModule = Core.Global.ExistCanActiveDWFProductExtraFieldsModule();
			DWFProductExtraFieldsActif = Core.Global.GetConfig().ModuleDWFProductExtraFieldsActif && CanActiveCanActiveDWFProductExtraFieldsModule;

			#endregion
		}

        #endregion

        #region Overriden methods

        protected override void OnLoaded()
        {
            base.OnLoaded();
        }

        #endregion

        #region Event methods

        private void LoadGroupsWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate { Mouse.OverrideCursor = Cursors.Wait; }), null);
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate { ListGroup.Clear(); }), null);

            IsBusy = true;
            LoadingStep = "Chargement des catégories tarifaires ...";

            Model.Sage.P_CATTARIFRepository P_CATTARIFRepository = new Model.Sage.P_CATTARIFRepository();

            List<Model.Sage.P_CATTARIF> categories = new List<Model.Sage.P_CATTARIF>();

            foreach (Model.Sage.P_CATTARIF categorieTarifaire in P_CATTARIFRepository.ListIntituleNotNullOrderByIntitule())
            {
                categories.Add(categorieTarifaire);
                LoadGroupsWorker.ReportProgress(0, categorieTarifaire);
            }

            LoadingStep = "Chargement des groupes de clients ...";
            GroupRepository = new Model.Local.GroupRepository();
            Model.Prestashop.PsGroupRepository PsGroupRepository = new Model.Prestashop.PsGroupRepository();
            Model.Prestashop.PsGroupLangRepository PsGroupLangRepository = new Model.Prestashop.PsGroupLangRepository();

            foreach (var psGroup in PsGroupRepository.List(Core.Global.CurrentShop.IDShop))
            {
                Model.Local.Group group = GroupRepository.Read(Convert.ToInt32(psGroup.IDGroup)) ?? new Model.Local.Group()
                    {
                        Grp_Pre_Id = Convert.ToInt32(psGroup.IDGroup),
                    };
                Model.Prestashop.PsGroupLang lang = PsGroupLangRepository.Read(Core.Global.Lang, psGroup.IDGroup);

                LoadGroupsWorker.ReportProgress(0, new ConfigurationGroup(group, psGroup)
                    {
                        Intitule = (lang == null) ? string.Empty : lang.Name,
                        CategorieTarifaire = categories.FirstOrDefault(result => result.cbIndice == group.Grp_CatTarifId),
                    });
            }
        }
        private void LoadGroupsWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is ConfigurationGroup)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(
                delegate { ListGroup.Add(e.UserState as ConfigurationGroup); }), null);
            }
            else if (e.UserState is Model.Sage.P_CATTARIF)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(
                delegate { ListCategoriesTarifaires.Add(e.UserState as Model.Sage.P_CATTARIF); }), null);
            }
        }
        private void LoadGroupsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LoadingStep = string.Empty;
            IsBusy = false;

            Application.Current.Dispatcher.BeginInvoke(new Action(delegate { Mouse.OverrideCursor = null; }), null);
        }

        private void LoadConfigWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate { Mouse.OverrideCursor = Cursors.Wait; }), null);

            IsBusy = true;
            LoadingStep = "Chargement de la configuration ...";

            foreach (Core.Parametres.RemiseMode mode in Enum.GetValues(typeof(Core.Parametres.RemiseMode)))
                LoadConfigWorker.ReportProgress(0, mode);

        }
        private void LoadConfigWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is Core.Parametres.RemiseMode)
            {
                ListRemiseMode.Add(new RemiseMode((Core.Parametres.RemiseMode)e.UserState));
            }
        }
        private void LoadConfigWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Core.AppConfig config = Core.Global.GetConfig();

            SelectedRemiseMode = ListRemiseMode.FirstOrDefault(result => result._RemiseMode == config.ModeRemise);

            LoadingStep = string.Empty;
            IsBusy = false;

            Application.Current.Dispatcher.BeginInvoke(new Action(delegate { Mouse.OverrideCursor = null; }), null);
        }

        #endregion

        #region Methods

        public void LoadGroups()
        {
            if (!LoadGroupsWorker.IsBusy)
                LoadGroupsWorker.RunWorkerAsync();
        }
        public void LoadConfig()
        {
            if (!LoadConfigWorker.IsBusy)
                LoadConfigWorker.RunWorkerAsync();
        }
        public void SaveGroups()
        {
            foreach (var group in ListGroup)
            {
                if (group.CategorieTarifaire != null)
                    group.Source.Grp_CatTarifId = group.CategorieTarifaire.cbIndice;

                if (GroupRepository.Exist(group.Source.Grp_Pre_Id))
                    GroupRepository.Save();
                else
                    GroupRepository.Add(group.Source);
            }
        }
        public void SaveCountry()
        {
            try
            {
                CountryRepository.Save();
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError("Configuration-SaveCountry" + ex.ToString());
            }
        }
        public void SaveGroupCRisque()
        {
            Model.Local.Group_CRisqueRepository Group_CRisqueRepository = new Model.Local.Group_CRisqueRepository();
            List<Model.Local.Group_CRisque> ListLocal = Group_CRisqueRepository.List();
            // suppression paramètre pour les codes risques n'existant plus
            Group_CRisqueRepository.DeleteList(ListLocal.Where(l => ListCodeRisque.Count(c => c.cbMarq == l.Sag_CRisque) == 0).ToList());
            foreach (Model.Sage.P_CRISQUE P_CRISQUE in ListCodeRisque)
            {
                if (Group_CRisqueRepository.ExistCRisque(P_CRISQUE.cbMarq))
                {
                    Model.Local.Group_CRisque Group_CRisque = Group_CRisqueRepository.ReadCRisque(P_CRISQUE.cbMarq);
                    Group_CRisque.Grp_Pre_Id = (P_CRISQUE.SelectedPsGroup != null) ? (int)P_CRISQUE.SelectedPsGroup.IDGroup : 0;
                    Group_CRisque.Grp_PreId_Default = (P_CRISQUE.SelectedPsGroupDefault != null) ? (int)P_CRISQUE.SelectedPsGroupDefault.IDGroup : 0;
                    Group_CRisque.Grp_LockCondition = P_CRISQUE.LockCondition;
                    Group_CRisqueRepository.Save();
                }
                else
                {
                    Group_CRisqueRepository.Add(new Model.Local.Group_CRisque()
                    {
                        Sag_CRisque = P_CRISQUE.cbMarq,
                        Grp_Pre_Id = (P_CRISQUE.SelectedPsGroup != null) ? (int)P_CRISQUE.SelectedPsGroup.IDGroup : 0,
                        Grp_PreId_Default = (P_CRISQUE.SelectedPsGroupDefault != null) ? (int)P_CRISQUE.SelectedPsGroupDefault.IDGroup : 0,
                        Grp_LockCondition = P_CRISQUE.LockCondition,
                    });
                }
            };
        }
        public void SavePortfolioCustomerEmployee()
        {
            Core.Global.GetConfig().UpdateModulePortfolioCustomerEmployeeActif(PortfolioCustomerEmployeeActif);

            Model.Local.Employee_CollaborateurRepository Employee_CollaborateurRepository = new Model.Local.Employee_CollaborateurRepository();
            List<Model.Local.Employee_Collaborateur> ListLocal = Employee_CollaborateurRepository.List();

            // suppression paramètre pour les codes risques n'existant plus
            Employee_CollaborateurRepository.DeleteList(ListLocal.Where(l => ListCollaborateur.Count(c => c.CO_No == l.Sage_CO_No) == 0).ToList());

            foreach (Model.Sage.F_COLLABORATEUR F_COLLABORATEUR in ListCollaborateur)
            {
                if (Employee_CollaborateurRepository.ExistCollaborateur(F_COLLABORATEUR.CO_No.Value))
                {
                    Model.Local.Employee_Collaborateur Employee_Collaborateur = Employee_CollaborateurRepository.ReadCollaborateur(F_COLLABORATEUR.CO_No.Value);
                    Employee_Collaborateur.IdEmployee = (F_COLLABORATEUR.SelectedPsEmployee != null) ? (int)F_COLLABORATEUR.SelectedPsEmployee.IDEmployee : 0;
                    Employee_CollaborateurRepository.Save();
                }
                else
                {
                    Employee_CollaborateurRepository.Add(new Model.Local.Employee_Collaborateur()
                    {
                        Sage_CO_No = F_COLLABORATEUR.CO_No.Value,
                        IdEmployee = (F_COLLABORATEUR.SelectedPsEmployee != null) ? (int)F_COLLABORATEUR.SelectedPsEmployee.IDEmployee : 0,
                    });
                }
            }
        }

        public void SaveConfig()
        {
            this.SaveGroups();
            this.SaveGroupCRisque();
            this.SavePortfolioCustomerEmployee();

            Core.Global.GetConfig().UpdateModeRemise(SelectedRemiseMode._RemiseMode);
            Core.Global.GetConfig().UpdateConflitRemise(SelectedRemiseConflit._RemiseConflit);

            Core.Global.GetConfig().UpdateConfigImageSynchroPositionLegende(ImageSynchroPositionLegende);

            Core.Global.GetConfig().UpdateTransfertPriceCategoryAvailable(this.ListPriceCategoryAvailable.Where(ct => ct.TransfertClient).Select(ct => ct.P_CATTARIF.cbMarq).ToList());

            Core.Global.GetConfig().UpdateTransfertMailAccountIdentification(this.SelectedMailAccountIdentification._MailAccountIdentification);
            Core.Global.GetConfig().UpdateTransfertRandomPasswordLength(this.SelectedRandomPasswordLength);

            Core.Global.GetConfig().UpdateTransfertNotifyAccountAddress(this.SelectedMailNotification._MailNotification);
            
            if (this.SelectedMailAccountIdentificationContactService != null)
                Core.Global.GetConfig().UpdateTransfertMailAccountContactService(this.SelectedMailAccountIdentificationContactService.P_SERVICECPTA.cbMarq);

            Core.Global.GetConfig().UpdateTransfertNotifyAccountSageContactService((from t in ListMailToContactService
                                                                                    where t.MailTo == true
                                                                                    select t.P_SERVICECPTA.cbMarq).ToList());
            Core.Global.GetConfig().UpdateTransfertNotifyAccountSageContactType((from t in ListMailToContactType
                                                                                 where t.MailTo == true
                                                                                 select t.P_CONTACT.cbMarq).ToList());

            Core.Global.GetConfig().UpdateTransfertSageAddressSend((from t in ListSageAddressSend
                                                                    where t.ToSend == true
                                                                    select t.Marq).ToList());
            Core.Global.GetConfig().UpdateTransfertLockPhoneNumber((from t in ListLockPhoneNumber
                                                                    where t.Active == true
                                                                    select t.Marq).ToList());

            Core.Global.GetConfig().UpdateTransfertAliasValue(SelectedAliasValue._AliasValue);
            Core.Global.GetConfig().UpdateTransfertLastNameValue(SelectedLastNameValue._LastNameValue);
            Core.Global.GetConfig().UpdateTransfertFirstNameValue(SelectedFirstNameValue._FirstNameValue);
            Core.Global.GetConfig().UpdateTransfertCompanyValue(SelectedCompanyValue._CompanyValue);

            Core.Global.GetConfig().UpdateTransfertClientSeparateurIntitule(ClientSeparateurIntitule);

            Core.Global.GetConfig().UpdateRegexMailLevel(SelectedRegexMail._RegexMailLevel);

            Core.Global.GetConfig().UpdateTransfertNameIncludeNumbers(TransfertClientNameIncludeNumbers);

            Core.Global.GetConfig().UpdateConfigImageStorageMode(SelectedImageStorageMode._ImageStorageMode);

            Core.Global.GetConfig().UpdateConfigLigneRemiseMode(SelectedLigneRemiseMode._LigneRemiseMode);

            Core.Global.GetConfig().UpdateConfigCommandeFiltreDate(CommandeFiltreDate);

            Core.Global.GetConfig().UpdateConfigClientFiltreCommande(ClientFiltreCommande);

            Core.Global.GetConfig().UpdateMajPoidsSynchroStock(PoidsSynchroStock);

            Core.Global.GetConfig().UpdateArticleFiltreDatePrixPrestashop(FiltreDatePrixPrestashop);

            Core.Global.GetConfig().UpdateArticleSpecificPriceLetBasePriceRule(RegleBasePrixSpecifique);

            Core.Global.GetConfig().UpdateCombinationWithWeightConversion((from t in ListGamme
                                                                           where t.WeightConversion == true
                                                                           select t.cbMarq).ToList());

            Core.Global.GetConfig().UpdateInformationLibreCoefficientConversion((SelectedInfolibreCoefficient != null && SelectedInfolibreCoefficient.CB_Name != null) ? SelectedInfolibreCoefficient.CB_Name : string.Empty);

            Core.Global.GetConfig().UpdateConfigClientNumComposition(CleanComposition());
            Core.Global.GetConfig().UpdateConfigClientNumPrefixe(ClientNumPrefixe);
            Core.Global.GetConfig().UpdateConfigClientNumLongueurNom(ClientNumLongueurNom);
            Core.Global.GetConfig().UpdateConfigClientNumTypeNom(SelectedTypeNom._NameNumComponent);
            Core.Global.GetConfig().UpdateConfigClientNumLongueurNumero(ClientNumLongueurNumero);
            Core.Global.GetConfig().UpdateConfigClientNumTypeNumero(SelectedTypeNumero._NumberNumComponent);
            Core.Global.GetConfig().UpdateConfigClientNumTypeCompteur(SelectedTypeCompteur._CounterType);
            Core.Global.GetConfig().UpdateConfigClientNumDebutCompteur(ClientNumDebutCompteur);

            Core.Global.GetConfig().UpdateClientNumDepartementRemplacerCodeISO(ClientNumDepartementRemplacerCodeISO);
            this.SaveCountry();

            Core.Global.GetConfig().UpdateClientMultiMappageBtoB(ClientMultiMappageBtoB);

            Core.Global.GetConfig().UpdateConfigClientCiviliteActif(ClientCiviliteActif);
            Core.Global.GetConfig().UpdateConfigClientSocieteIntituleActif(ClientSocieteIntituleActif);
            Core.Global.GetConfig().UpdateConfigClientNIFActif(ClientNIFActif);
            Core.Global.GetConfig().UpdateConfigClientInfosMajusculeActif(ClientInfosMajusculeActif);

            Core.Global.GetConfig().UpdateConfigClientAdresseTelephonePositionFixe(ClientAdresseTelephonePositionFixe);

            Core.Global.GetConfig().UpdateConfigCronSynchroArticleURL(CronSynchroArticleURL);
            Core.Global.GetConfig().UpdateChronoSynchroStockPriceActif(LogChronoSynchroStockPriceActif);
            Core.Global.GetConfig().UpdateConfigRefreshTempCustomerListDisabled(RefreshTempCustomerListDisabled);

            Core.Global.GetConfig().UpdateCronArticleURL(CronArticleURL);
            Core.Global.GetConfig().UpdateCronArticleBalise(CronArticleBalise);
            Core.Global.GetConfig().UpdateCronArticleTimeout(CronArticleTimeout);
			Core.Global.GetConfig().UpdateCronCommandeURL(CronCommandeURL);
			Core.Global.GetConfig().UpdateCronCommandeBalise(CronCommandeBalise);
			Core.Global.GetConfig().UpdateCronCommandeTimeout(CronCommandeTimeout);

            Core.Global.GetConfig().UpdateMarqueAutoStatistiqueActif(MarqueStatActif);
            Core.Global.GetConfig().UpdateMarqueAutoStatistiqueName((MarqueStatActif && SelectedMarqueSageStatistiqueArticle != null) ? SelectedMarqueSageStatistiqueArticle.P_IntStatArt1 : string.Empty);

            Core.Global.GetConfig().UpdateMarqueAutoInfolibreActif(MarqueInfolibreActif);
            Core.Global.GetConfig().UpdateMarqueAutoInfolibreName((MarqueInfolibreActif && SelectedMarqueSageInfolibreArticle != null) ? SelectedMarqueSageInfolibreArticle.CB_Name : string.Empty);

            Core.Global.GetConfig().UpdateFournisseurAutoStatistiqueActif(FournisseurStatActif);
            Core.Global.GetConfig().UpdateFournisseurAutoStatistiqueName((FournisseurStatActif && SelectedFournisseurSageStatistiqueArticle != null) ? SelectedFournisseurSageStatistiqueArticle.P_IntStatArt1 : string.Empty);

            Core.Global.GetConfig().UpdateFournisseurAutoInfolibreActif(FournisseurInfolibreActif);
            Core.Global.GetConfig().UpdateFournisseurAutoInfolibreName((FournisseurInfolibreActif && SelectedFournisseurSageInfolibreArticle != null) ? SelectedFournisseurSageInfolibreArticle.CB_Name : string.Empty);

            Core.Global.GetConfig().UpdateStatInfolibreClientActif(ModuleStatInfolibreClientActif);

            Core.Global.GetConfig().UpdateImportConditionnementActif(ImportConditionnementActif);
            Core.Global.GetConfig().UpdateConditionnementQuantiteToUPC(ConditionnementQuantiteToUPC);
            Core.Global.GetConfig().UpdateLimiteStockConditionnement(LimiteStockConditionnementMini);

            Core.Global.GetConfig().UpdateArticleContremarqueStockActif(StockArticleContremarqueActif);

            Core.Global.GetConfig().UpdateDeleteCatalogProductAssociation(CanDeleteCatalogProductAssociation);

            Core.Global.GetConfig().UpdateArticleDateDispoInfoLibreActif(DateDispoInfoLibreActif);
            Core.Global.GetConfig().UpdateArticleDateDispoInfoLibreName((SelectedDateDispoInfoLibre != null && !string.IsNullOrWhiteSpace(SelectedDateDispoInfoLibre.CB_Name)) ? SelectedDateDispoInfoLibre.CB_Name : string.Empty);

            Core.Global.GetConfig().UpdateArticleQuantiteMiniActif(QuantiteMiniActif);
            Core.Global.GetConfig().UpdateArticleQuantiteMiniConditionnement(QuantiteMiniConditionnement);
            Core.Global.GetConfig().UpdateArticleQuantiteMiniUniteVente(QuantiteMiniUniteVente);

            Core.Global.GetConfig().UpdateArticleTransfertInfosFournisseurActif(TransfertInfosFournisseurActif);

            Core.Global.GetConfig().UpdateArticleRedirectionCompositionActif(RedirectionComposition);

            Core.Global.GetConfig().UpdateArticleStockNegatifZero(StockNegatifZero);
            Core.Global.GetConfig().UpdateArticleStockNegatifZeroParDepot(StockNegatifZeroParDepot);

            Core.Global.GetConfig().UpdateModuleOleaPromoActif(OleaPromoActif);
            Core.Global.GetConfig().UpdateModuleOleaSuffixeGratuit(OleaSuffixeGratuit);

			Core.Global.GetConfig().UpdateModuleDWFProductGuideratesActif(DWFProductGuideratesActif);
			Core.Global.GetConfig().UpdateModuleDWFProductExtraFieldsActif(DWFProductExtraFieldsActif);

			Core.Global.GetConfig().UpdateUIProductFilterActiveDefault(SelectedProductFilterActiveDefault._ProductFilterActiveDefault);
            Core.Global.GetConfig().UpdateUIProductUpdateValidationDisabled(ProductUpdateValidationDisabled);
            Core.Global.GetConfig().UpdateUIMaximizeWindow(WindowsMaximized);

            Core.Global.GetConfig().UpdateUISleepTimeWYSIWYG(SleepTimeWYSIWYG);
            Core.Global.GetConfig().UpdateUIDisabledWYSIWYG(DisabledWYSIWYG);
            Core.Global.GetConfig().UpdateUIIE11EmulationModeDisabled(IE11EmulationModeDisabled);

            Core.Global.GetConfig().UpdateConfigUnlockProcessorCore(UnlockProcessorCore);
            Core.Global.GetConfig().UpdateConfigAllocatedProcessorCore(AllocatedProcessorCore);

            Core.Global.GetConfig().UpdateCrystalForceConnectionInfoOnSubReports(CrystalForceConnectionInfoOnSubReports);

            Core.Global.GetConfig().UpdateConfigLigneFraisPort(CommandeLigneFraisPort);
            Core.Global.GetConfig().UpdateConfigLigneArticlePort((CommandeLigneFraisPort && SelectedArticlePort != null) ? SelectedArticlePort.AR_Ref : string.Empty);

			Core.Global.GetConfig().UpdateConfigDateLivraisonMode(CommandeDateLivraisonMode);
			Core.Global.GetConfig().UpdateConfigDateLivraisonJours(commandeDateLivraisonJours);

			Core.Global.GetConfig().UpdateCommandeArticleReduction((SelectedArticleReduction != null) ? SelectedArticleReduction.AR_Ref : string.Empty);

            Core.Global.GetConfig().UpdateConfigCommentaireBoutiqueActif(CommandeCommentaireBoutiqueActif);
            Core.Global.GetConfig().UpdateConfigCommentaireBoutiqueTexte(CommandeCommentaireBoutiqueTexte);
            Core.Global.GetConfig().UpdateConfigCommentaireNumeroActif(CommandeCommentaireNumeroActif);
            Core.Global.GetConfig().UpdateConfigCommentaireNumeroTexte(CommandeCommentaireNumeroTexte);
            Core.Global.GetConfig().UpdateConfigCommentaireReferencePaiementActif(CommandeCommentaireReferencePaiementActif);
            Core.Global.GetConfig().UpdateConfigCommentaireReferencePaiementTexte(CommandeCommentaireReferencePaiementTexte);
            Core.Global.GetConfig().UpdateConfigCommentaireDateActif(CommandeCommentaireDateActif);
            Core.Global.GetConfig().UpdateConfigCommentaireDateTexte(CommandeCommentaireDateTexte);

            Core.Global.GetConfig().UpdateConfigCommentaireClientActif(CommandeCommentaireClientActif);
            Core.Global.GetConfig().UpdateConfigCommentaireClientTexte(CommandeCommentaireClientTexte);
            Core.Global.GetConfig().UpdateConfigCommentaireAdresseFacturationActif(CommandeCommentaireAdresseFacturationActif);
            Core.Global.GetConfig().UpdateConfigCommentaireAdresseFacturationTexte(CommandeCommentaireAdresseFacturationTexte);
            Core.Global.GetConfig().UpdateConfigCommentaireAdresseLivraisonActif(CommandeCommentaireAdresseLivraisonActif);
            Core.Global.GetConfig().UpdateConfigCommentaireAdresseLivraisonTexte(CommandeCommentaireAdresseLivraisonTexte);

            Core.Global.GetConfig().UpdateConfigCommentaireLibre1Actif(CommandeCommentaireLibre1Actif);
            Core.Global.GetConfig().UpdateConfigCommentaireLibre1Texte(CommandeCommentaireLibre1Texte);
            Core.Global.GetConfig().UpdateConfigCommentaireLibre2Actif(CommandeCommentaireLibre2Actif);
            Core.Global.GetConfig().UpdateConfigCommentaireLibre2Texte(CommandeCommentaireLibre2Texte);
            Core.Global.GetConfig().UpdateConfigCommentaireLibre3Actif(CommandeCommentaireLibre3Actif);
            Core.Global.GetConfig().UpdateConfigCommentaireLibre3Texte(CommandeCommentaireLibre3Texte);

            Core.Global.GetConfig().UpdateConfigCommentaireDebutDocument(CommentStart);
            Core.Global.GetConfig().UpdateConfigCommentaireFinDocument(CommentEnds);

            Core.Global.GetConfig().UpdateArticleInfolibrePackaging((SelectedInfolibrePackaging != null) ? SelectedInfolibrePackaging.CB_Name : string.Empty);

            Core.Global.GetConfig().UpdateCommandeArticlePackaging((SelectedArticlePackaging != null) ? SelectedArticlePackaging.AR_Ref : string.Empty);

            Core.Global.GetConfig().UpdateCommandeUpdateAdresseFacturation(UpdateAdresseFacturation);
            Core.Global.GetConfig().UpdateCommandeInsertFacturationEntete(InsertFacturationEntete);

            Core.Global.GetConfig().UpdateTaxSageTVA(SelectedTaxSageTVA._TaxSage);
            Core.Global.GetConfig().UpdateTaxSageEco(SelectedTaxSageEco._TaxSage);

            Core.Global.GetConfig().UpdateCommandeReferencePrestashop(CopyReferencePrestashop);
            Core.Global.GetConfig().UpdateCommandeNumeroFactureSageForceActif(ForceNumeroFactureSage);
            Core.Global.GetConfig().UpdateCommandeStatutJoursAutomate(JoursAutomateStatut);

            Core.Global.GetConfig().UpdateCommandeTrackingEnteteActif(TrackingEnteteActif);
            Core.Global.GetConfig().UpdateCommandeTrackingInfolibreActif(TrackingInfolibreActif);
            Core.Global.GetConfig().UpdateCommandeTrackingEntete((TrackingEnteteActif && SelectedTrackingFieldDocumentEntete != null) ? SelectedTrackingFieldDocumentEntete._FieldDocumentEnteteValue : 0);
            Core.Global.GetConfig().UpdateCommandeTrackingInfolibre((TrackingInfolibreActif && SelectedTrackingInfolibreDocument != null) ? SelectedTrackingInfolibreDocument.CB_Name : string.Empty);

            Core.Global.GetConfig().UpdateModeReglementEcheancierActif(ReglementEcheancierActif);
            Core.Global.GetConfig().UpdateReglementLibellePartielActif(ReglementLibellePartielActif);

            Core.Global.GetConfig().UpdateModulePreorderActif(PreorderActif);
            Core.Global.GetConfig().UpdateModulePreorderInfolibreName((SelectedInfolibrePreorder != null) ? SelectedInfolibrePreorder.CB_Name : string.Empty);
            Core.Global.GetConfig().UpdateModulePreorderInfolibreValue(PreorderInfolibreValue);
            Core.Global.GetConfig().UpdateModulePreorderPrestashopProduct((SelectedPreorderPrestashopProduct != null) ? (int)SelectedPreorderPrestashopProduct.id_product : 0);
            Core.Global.GetConfig().UpdateModulePreorderPrestashopOrderState((SelectedPreorderPrestashopOrderState != null) ? (int)SelectedPreorderPrestashopOrderState.IDOrderState : 0);
            Core.Global.GetConfig().UpdateModulePreorderInfolibreEnteteName((SelectedInfolibreEntetePreorder != null) ? SelectedInfolibreEntetePreorder.CB_Name : string.Empty);
            Core.Global.GetConfig().UpdateModulePreorderInfolibreEnteteValue(PreorderInfolibreEnteteValue);

            Core.Global.GetConfig().UpdateModuleAECInvoiceHistoryActif(AECInvoiceHistoryActif);
            Core.Global.GetConfig().UpdateModuleAECInvoiceHistoryInfoLibreClientSendMail((SelectedInfolibreInvoiceHistorySendMail != null) ? SelectedInfolibreInvoiceHistorySendMail.CB_Name : string.Empty);
            Core.Global.GetConfig().UpdateModuleAECInvoiceHistoryInfoLibreClientSendMailValue(InfolibreInvoiceHistorySendMailValue);
            Core.Global.GetConfig().UpdateModuleAECInvoiceHistoryArchivePDFActive(AECInvoiceHistoryArchivePDF);
            Core.Global.GetConfig().UpdateModuleAECInvoiceHistoryArchivePDFFolder(AECInvoiceHistoryArchivePDFFolder);

            Core.Global.GetConfig().UpdateModuleAECStockActif(AECStockActif);

            Core.Global.GetConfig().UpdateModuleAECCollaborateurActif(AECCollaborateurActif);

            Core.Global.GetConfig().UpdateModuleAECPaiementActif(AECPaiementActif);

            Core.Global.GetConfig().UpdateModuleAECCustomerOutstandingActif(AECCustomerOutstandingActif);

            Core.Global.GetConfig().UpdateModuleAECCustomerInfoActif(AECCustomerInfoActif);

            Core.Global.GetConfig().UpdateModuleSoColissimoDeliveryActive(SoColissimoDeliveryActive);
            Core.Global.GetConfig().UpdateModuleSoColissimoInfolibreTypePointActive(SoColissimoInfolibreTypePointActive);
            Core.Global.GetConfig().UpdateModuleSoColissimoInfolibreEnteteTypePointName((SelectedInfolibreEnteteSoColissimoTypePoint != null) ? SelectedInfolibreEnteteSoColissimoTypePoint.CB_Name : string.Empty);
            Core.Global.GetConfig().UpdateModuleSoColissimoInfolibreDestinataireActive(SoColissimoInfolibreDestinataireActive);
            Core.Global.GetConfig().UpdateModuleSoColissimoInfolibreEnteteDestinataireName((SelectedInfolibreEnteteSoColissimoDestinataire != null) ? SelectedInfolibreEnteteSoColissimoDestinataire.CB_Name : string.Empty);
            Core.Global.GetConfig().UpdateModuleSoColissimoReplacePhoneActive(SoColissimoReplacePhoneActive);
            Core.Global.GetConfig().UpdateModuleSoColissimoReplaceAddressNameActive(SoColissimoReplaceAddressNameActive);

            this.SaveInformationLibre();
        }

        public void CreateFeatureFromSageInformationLibre()
        {
            try
            {
                if (this.SelectedInformationLibre != null)
                {
                    if (MessageBox.Show("Valider la création d'une caratéristique nommée " + this.SelectedInformationLibre.InfoLibre.Sag_InfoLibre + " dans votre Prestashop ?", "Création de caractéristique",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Model.Prestashop.PsFeatureLang newCharacteristic;
                        this.SelectedInformationLibre.CreateFeature(out newCharacteristic);
                        if (newCharacteristic != null)
                        {
                            this.ListFeature.Add(newCharacteristic);
                            this.SelectedInformationLibre.Feature = newCharacteristic;
                            this.SelectedInformationLibre.CanCreateFeature = false;
                            this.SelectedInformationLibre.InfoValeursMode = ListInformationLibreValeursMode.FirstOrDefault(m => m._InformationLibreValeursMode == Core.Parametres.InformationLibreValeursMode.Predefinies);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
        public void CreateFeatureFromSageStatistiqueArticle()
        {
            try
            {
                if (this.SelectedStatistiqueArticle != null)
                {
                    if (MessageBox.Show("Valider la création d'une caratéristique nommée " + this.SelectedStatistiqueArticle.SageStatistique.P_IntStatArt1 + " dans votre Prestashop ?", "Création de caractéristique",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Model.Prestashop.PsFeatureLang newCharacteristic;
                        this.SelectedStatistiqueArticle.CreateFeature(out newCharacteristic);
                        if (newCharacteristic != null)
                        {
                            this.ListFeature.Add(newCharacteristic);
                            this.SelectedStatistiqueArticle.Feature = newCharacteristic;
                            this.SelectedStatistiqueArticle.CanCreateFeature = false;
                            this.SelectedStatistiqueArticle.InfoValeursMode = ListInformationLibreValeursMode.FirstOrDefault(m => m._InformationLibreValeursMode == Core.Parametres.InformationLibreValeursMode.Predefinies);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
        public void CreateFeatureFromSageInformationArticle()
        {
            try
            {
                if (this.SelectedSageInfoArticle != null)
                {
                    if (MessageBox.Show("Valider la création d'une caratéristique nommée " + this.SelectedSageInfoArticle.SageInfoArticle.Intitule + " dans votre Prestashop ?", "Création de caractéristique",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Model.Prestashop.PsFeatureLang newCharacteristic;
                        this.SelectedSageInfoArticle.CreateFeature(out newCharacteristic);
                        if (newCharacteristic != null)
                        {
                            this.ListFeature.Add(newCharacteristic);
                            this.SelectedSageInfoArticle.Feature = newCharacteristic;
                            this.SelectedSageInfoArticle.CanCreateFeature = false;
                            this.SelectedSageInfoArticle.InfoValeursMode = ListInformationLibreValeursMode.FirstOrDefault(m => m._InformationLibreValeursMode == Core.Parametres.InformationLibreValeursMode.Predefinies);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
        public void CreateCustomerFeatureFromSageInformationLibreClient()
        {
            try
            {
                if (this.SelectedInformationLibreClient != null)
                {
                    if (MessageBox.Show("Valider la création d'une caratéristique client nommée " + this.SelectedInformationLibreClient.InfoLibreClient.Sag_InfoLibreClient + " dans votre Prestashop ?", "Création de caractéristique client",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Model.Prestashop.PsCustomerFeatureLang newCharacteristic;
                        this.SelectedInformationLibreClient.CreateCustomerFeature(out newCharacteristic);
                        if (newCharacteristic != null)
                        {
                            this.ListCustomerFeature.Add(newCharacteristic);
                            this.SelectedInformationLibreClient.CustomerFeature = newCharacteristic;
                            this.SelectedInformationLibreClient.CanCreateCustomerFeature = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }
        public void CreateCustomerFeatureFromSageStatistiqueClient()
        {
            try
            {
                if (this.SelectedStatistiqueClient != null)
                {
                    if (MessageBox.Show("Valider la création d'une caratéristique client nommée " + this.SelectedStatistiqueClient.SageStatistique.S_Intitule + " dans votre Prestashop ?", "Création de caractéristique client",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Model.Prestashop.PsCustomerFeatureLang newCharacteristic;
                        this.SelectedStatistiqueClient.CreateCustomerFeature(out newCharacteristic);
                        if (newCharacteristic != null)
                        {
                            this.ListCustomerFeature.Add(newCharacteristic);
                            this.SelectedStatistiqueClient.CustomerFeature = newCharacteristic;
                            this.SelectedStatistiqueClient.CanCreateCustomerFeature = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void SaveInformationLibre()
        {
            #region Statistiques Article
            Model.Local.StatistiqueArticleRepository StatistiqueArticleRepository = new Model.Local.StatistiqueArticleRepository();
            foreach (StatistiqueArticle StatArt in ListStatistiqueArticle)
            {
                if (StatistiqueArticleRepository.ExistStatArticle(StatArt.SageStatistique.P_IntStatArt1))
                {
                    Model.Local.StatistiqueArticle StatistiqueArticle = StatistiqueArticleRepository.ReadStatArticle(StatArt.SageStatistique.P_IntStatArt1);
                    StatistiqueArticle.Inf_Mode = StatArt.StatArt.Inf_Mode;
                    StatistiqueArticle.Cha_Id = StatArt.StatArt.Cha_Id;
                    StatistiqueArticleRepository.Save();
                }
                else
                    StatistiqueArticleRepository.Add(StatArt.StatArt);
            }
            #endregion

            #region Informations libres Article
            Model.Local.InformationLibreRepository InformationLibreRepository = new Model.Local.InformationLibreRepository();
            foreach (InformationLibre InfoLibre in ListInformationLibre)
            {
                if (InformationLibreRepository.ExistInfoLibre(InfoLibre.InfoLibre.Sag_InfoLibre))
                {
                    Model.Local.InformationLibre InformationLibre = InformationLibreRepository.ReadInfoLibre(InfoLibre.InfoLibre.Sag_InfoLibre);
                    InformationLibre.Inf_Mode = InfoLibre.InfoLibre.Inf_Mode;
                    InformationLibre.Cha_Id = InfoLibre.InfoLibre.Cha_Id;
                    InformationLibreRepository.Save();
                }
                else
                    InformationLibreRepository.Add(InfoLibre.InfoLibre);
            }
            #endregion

            #region Informations Sage Article
            Model.Local.InformationArticleRepository InformationArticleRepository = new Model.Local.InformationArticleRepository();
            foreach (InformationArticle InfoArticle in ListSageInfoArticle)
            {
                if (InformationArticleRepository.ExistSageInfoArticle(InfoArticle.SageInfoArticle.Marq))
                {
                    Model.Local.InformationArticle InformationArticle = InformationArticleRepository.ReadSageInfoArticle(InfoArticle.SageInfoArticle.Marq);
                    InformationArticle.Inf_Mode = InfoArticle.InfoArticle.Inf_Mode;
                    InformationArticle.Cha_Id = InfoArticle.InfoArticle.Cha_Id;
                    InformationArticleRepository.Save();
                }
                else
                    InformationArticleRepository.Add(InfoArticle.InfoArticle);
            }
            #endregion

            Core.Global.GetConfig().UpdateArticleSuppressionAutoCaracteristique(SuppressionAutoCaracteristique);

            #region Statistiques Client
            Model.Local.StatistiqueClientRepository StatistiqueClientRepository = new Model.Local.StatistiqueClientRepository();
            foreach (StatistiqueClient StatClient in ListStatistiqueClient)
            {
                if (StatistiqueClientRepository.ExistStatClient(StatClient.SageStatistique.S_Intitule))
                {
                    Model.Local.StatistiqueClient StatistiqueClient = StatistiqueClientRepository.ReadStatClient(StatClient.SageStatistique.S_Intitule);
                    StatistiqueClient.Inf_Mode = StatClient.StatClient.Inf_Mode;
                    StatistiqueClient.Cha_Id = StatClient.StatClient.Cha_Id;
                    StatistiqueClientRepository.Save();
                }
                else
                    StatistiqueClientRepository.Add(StatClient.StatClient);
            }
            #endregion

            #region Informations libres Client
            Model.Local.InformationLibreClientRepository InformationLibreClientRepository = new Model.Local.InformationLibreClientRepository();
            foreach (InformationLibreClient InfoLibreClient in ListInformationLibreClient)
            {
                if (InformationLibreClientRepository.ExistInfoLibre(InfoLibreClient.InfoLibreClient.Sag_InfoLibreClient))
                {
                    Model.Local.InformationLibreClient InformationLibreClient = InformationLibreClientRepository.ReadInfoLibre(InfoLibreClient.InfoLibreClient.Sag_InfoLibreClient);
                    InformationLibreClient.Inf_Mode = InfoLibreClient.InfoLibreClient.Inf_Mode;
                    InformationLibreClient.Cha_Id = InfoLibreClient.InfoLibreClient.Cha_Id;
                    InformationLibreClientRepository.Save();
                }
                else
                    InformationLibreClientRepository.Add(InfoLibreClient.InfoLibreClient);
            }
            #endregion
        }

        public Boolean VerifyClientNum()
        {
            Boolean valid = true;
            Application.Current.Dispatcher.BeginInvoke(new Action(delegate { Mouse.OverrideCursor = Cursors.Wait; }), null);
            IsBusy = true;
            if (ClientNumComposition != null)
            {
                Boolean ContainsPrefixe = false,
                ContainsNom = false,
                ContainsDep = false,
                ContainsNumero = false;

                string componew = ClientNumComposition;
                ContainsPrefixe = componew.Contains(ClientNumComposing.StrPrefixe);
                ContainsNom = componew.Contains(ClientNumComposing.StrNom);
                ContainsDep = componew.Contains(ClientNumComposing.StrDep);
                ContainsNumero = componew.Contains(ClientNumComposing.StrNumero);

                Int32 MaxLength = 17;
                if (ContainsDep)
                    MaxLength -= 2;
                if (ContainsPrefixe && ClientNumPrefixe != null)
                    MaxLength -= ClientNumPrefixe.Length;
                if (ContainsNumero)
                    MaxLength -= ClientNumLongueurNumero;
                if (ContainsNom)
                {
                    if (MaxLength >= 2)
                    {
                        if (MaxLength < ClientNumLongueurNom)
                        {
                            ClientNumLongueurNom = MaxLength;
                        }
                    }
                    else
                    {
                        ClientNumLongueurNumero -= (MaxLength == 1) ? 1 : 2;
                        ClientNumLongueurNom = 2;
                    }
                }

                if (!ContainsNumero)
                    ClientNumComposition += ClientNumComposing.StrNumero;

            }
            else
                ClientNumComposition = ClientNumComposing.StrPrefixe + ClientNumComposing.StrNumero;

            OnPropertyChanged("GetNumSample");

            Application.Current.Dispatcher.BeginInvoke(new Action(delegate { Mouse.OverrideCursor = null; }), null);
            IsBusy = false;

            return valid;
        }
        public string CleanComposition()
        {
            string componew = ClientNumComposition;
            string control = componew;
            control = control.Replace(ClientNumComposing.StrPrefixe, "■");
            control = control.Replace(ClientNumComposing.StrNom, "■");
            control = control.Replace(ClientNumComposing.StrDep, "■");
            control = control.Replace(ClientNumComposing.StrNumero, "■");

            string[] tabl = { "■" };
            if (control.Split(tabl, StringSplitOptions.RemoveEmptyEntries).Count() > 0)
            {
                componew = componew.Replace(ClientNumComposing.StrPrefixe, "╔");
                componew = componew.Replace(ClientNumComposing.StrNom, "╚");
                componew = componew.Replace(ClientNumComposing.StrDep, "╩");
                componew = componew.Replace(ClientNumComposing.StrNumero, "╦");
                foreach (string s in control.Split(tabl, StringSplitOptions.RemoveEmptyEntries))
                    componew = componew.Replace(s, string.Empty);

                componew = componew.Replace("╔", ClientNumComposing.StrPrefixe);
                componew = componew.Replace("╚", ClientNumComposing.StrNom);
                componew = componew.Replace("╩", ClientNumComposing.StrDep);
                componew = componew.Replace("╦", ClientNumComposing.StrNumero);
            }
            return componew;
        }
        private void CalcMaxCompteur()
        {
            if (ClientNumLongueurNumero <= 9 && ClientNumLongueurNumero > 0)
            {
                string s = string.Empty;
                for (int i = 1; i <= ClientNumLongueurNumero; i++)
                    s += "9";
                ClientNumMaxCompteur = int.Parse(s);
                if (ClientNumDebutCompteur > ClientNumMaxCompteur)
                    ClientNumDebutCompteur = ClientNumMaxCompteur;
            }
        }

        public void DeleteReplacement()
        {
            try
            {
                Model.Local.ReplacementRepository ReplacementRepository = new Model.Local.ReplacementRepository();
                if (SelectedReplacement != null && ReplacementRepository.ExistOrigin(SelectedReplacement.OriginText))
                {
                    ReplacementRepository.Delete(ReplacementRepository.ReadOrigin(SelectedReplacement.OriginText));
                    ListReplacement.Remove(SelectedReplacement);
                }
                ReplacementRepository = null;
            }
            catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
        }
        public void AddReplacement()
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(ReplacementOriginText)
                    && !String.IsNullOrWhiteSpace(ReplacementNewText))
                {
                    Model.Local.ReplacementRepository ReplacementRepository = new Model.Local.ReplacementRepository();
                    if (!ReplacementRepository.ExistOrigin(ReplacementOriginText))
                    {
                        Model.Local.Replacement newReplacement = new Model.Local.Replacement()
                        {
                            OriginText = ReplacementOriginText,
                            ReplacementText = ReplacementNewText,
                        };
                        ReplacementRepository.Add(newReplacement);
                        ListReplacement.Add(newReplacement);

                        ReplacementOriginText = string.Empty;
                        ReplacementNewText = string.Empty;
                    }
                    else
                    {
                        MessageBox.Show("Cette valeur est déjà remplacée, veuillez supprimer le remplacement actuel d'abord !", "Prestaconnect", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Vous devez renseigner les deux valeurs !", "Prestaconnect", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex) { Core.Error.SendMailError(ex.ToString()); }
        }

        public void ChangeLocalStorageMode()
        {
            try
            {
                Loading Loading = new Loading();
                Loading.Show();
                ChangeStorageMode move = new ChangeStorageMode();
                Loading.Close();
                move.ShowDialog();
                Core.Global.GetConfig().UpdateConfigLocalStorageMode(Core.Parametres.LocalStorageMode.advanced_system);
                CurrentLocalStorageMode = new LocalStorageMode(Core.Global.GetConfig().ConfigLocalStorageMode);
            }
            catch (Exception ex)
            {
                Core.Error.SendMailError(ex.ToString());
            }
        }

        public void CheckAllCountryReplaceCodeISO(bool Replace)
        {
            foreach (Model.Local.Country Country in ListCountryReplaceISOCode)
                Country.Replace_ISOCode = Replace;
        }

        #endregion

    }
}