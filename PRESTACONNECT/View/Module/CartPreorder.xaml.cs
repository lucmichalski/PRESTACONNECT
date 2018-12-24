using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace PRESTACONNECT
{
    /// <summary>
    /// Logique d'interaction pour CartPreorder.xaml
    /// </summary>
    public partial class CartPreorder : Window
    {
        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public Semaphore Semaphore = new Semaphore(1, 4);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public CartPreorder()
        {
            this.InitializeComponent();

            Model.Sage.F_DOCENTETERepository F_DOCENTETERepository = new Model.Sage.F_DOCENTETERepository();
            // Récupération des documents pour lesquels au moins un des articles possède la valeur saisie en paramètre dans l'information libre sélectionnée
            List<Model.Sage.DocPreorder> ListPreorder = F_DOCENTETERepository.ListDocumentArticlePrecommande(Core.Global.GetConfig().ModulePreorderInfolibreName,
                Core.Global.GetConfig().ModulePreorderInfolibreValue,
                (short)ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Domaine.Vente, 
                (short)ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_Type.Bon_Commande_Vente);
            List<int> ListSage = ListPreorder.Select(d => d.cbMarq).Distinct().ToList();
            this.ListCount = ListSage.Count;

            Context = SynchronizationContext.Current;

            this.ParallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListSage, this.ParallelOptions, Exec);
            });
        }

        public void Exec(int DocumentSend)
        {
            this.Semaphore.WaitOne();

            try
            {
                Model.Sage.F_DOCENTETERepository F_DOCENTETERepository = new Model.Sage.F_DOCENTETERepository();
                Model.Sage.F_DOCENTETE F_DOCENTETE = F_DOCENTETERepository.Read(DocumentSend);

                Model.Local.OrderRepository OrderRepository = new Model.Local.OrderRepository();
                int PreID;
                // Contrôle de la provenance Web de la commande via une synchro Prestaconnect
                if (int.TryParse(F_DOCENTETE.DO_NoWeb, out PreID)
                    && OrderRepository.ExistPrestashop(PreID))
                {
                    Model.Prestashop.PsOrdersRepository PsOrdersRepository = new Model.Prestashop.PsOrdersRepository();
                    if (PsOrdersRepository.ExistOrder(PreID))
                    {
                        Model.Prestashop.PsCartPreOrderRepository PsCartPreOrderRepository = new Model.Prestashop.PsCartPreOrderRepository();
                        if (!PsCartPreOrderRepository.ExistOrder(PreID))
                        {
                            Model.Prestashop.PsOrders PsOrder = PsOrdersRepository.ReadOrder(PreID);

                            Model.Prestashop.PsCustomerRepository PsCustomerRepository = new Model.Prestashop.PsCustomerRepository();
                            if (PsCustomerRepository.ExistCustomer(PsOrder.IDCustomer))
                            {
                                // récupération montant total des lignes
                                decimal? total_document = new Model.Sage.F_DOCLIGNERepository().MontantDomaineTypePieceValorise(F_DOCENTETE.DO_Domaine.Value, F_DOCENTETE.DO_Type.Value, F_DOCENTETE.DO_Piece);
                                // ajout frais de port pied de document
                                decimal total_port = 0;
                                if (F_DOCENTETE.DO_ValFrais != null)
                                {
                                    if (F_DOCENTETE.DO_TypeLigneFrais == (short)ABSTRACTION_SAGE.F_DOCENTETE.Obj._Enum_DO_TypeLigne.TTC)
                                        total_port = (decimal)F_DOCENTETE.DO_ValFrais;
                                    else
                                        total_port = (decimal)F_DOCENTETE.DO_ValFrais * (1 + (F_DOCENTETE.DO_Taxe1.Value / 100));
                                }
                                // calcul des acomptes et règlements
                                decimal? total_reglements = new Model.Sage.F_DOCREGLRepository().MontantDomaineTypePiece(F_DOCENTETE.DO_Domaine.Value, F_DOCENTETE.DO_Type.Value, F_DOCENTETE.DO_Piece);
                                if (total_document != null && total_reglements != null)
                                {
                                    decimal solde = (decimal)((total_document + total_port) - total_reglements);
                                    if (solde > 0)
                                    {
                                        #region création nouveau panier
                                        Model.Prestashop.PsCartRepository PsCartRepository = new Model.Prestashop.PsCartRepository();
                                        Model.Prestashop.PsCart PsCart = new Model.Prestashop.PsCart()
                                        {
                                            DateAdd = DateTime.Now,
                                            DateUpd = DateTime.Now,
                                            DeliveryOption = string.Empty,
                                            Gift = 0,
                                            GiftMessage = string.Empty,
                                            IDAddressDelivery = PsOrder.IDAddressDelivery,
                                            IDAddressInvoice = PsOrder.IDAddressInvoice,
                                            IDCarrier = 0,
                                            IDCurrency = PsOrder.IDCurrency,
                                            IDCustomer = PsOrder.IDCustomer,
                                            IDGuest = 0,
                                            IDLang = PsOrder.IDLang,
                                            IDShop = PsOrder.IDShop,
                                            IDShopGroup = PsOrder.IDShopGroup,
                                            Recyclable = PsOrder.Recyclable,
                                            SecureKey = PsOrder.SecureKey,
                                        };
                                        PsCartRepository.Add(PsCart);
                                        #endregion

                                        #region attribution du solde en tant que prix spécifique pour le produit
                                        Model.Prestashop.PsSpecificPriceRepository PsSpecificPriceRepository = new Model.Prestashop.PsSpecificPriceRepository();
                                        Model.Prestashop.PsSpecificPrice PsSpecificPrice = new Model.Prestashop.PsSpecificPrice()
                                        {
                                            IDProduct = (uint)Core.Global.GetConfig().ModulePreorderPrestashopProduct,
                                            FromQuantity = 1,
                                            IDCart = PsCart.IDCart,
                                            IDCustomer = PsCart.IDCustomer,
                                            Price = solde,
                                            ReductionType = Model.Prestashop.PsSpecificPrice._ReductionType_Amount,
                                            From = new DateTime(),
                                            To = new DateTime(),
                                        };
                                        PsSpecificPriceRepository.Add(PsSpecificPrice);
                                        PsSpecificPriceRepository.SaveReductionTypeFromDateToDate(PsSpecificPrice);
                                        #endregion

                                        #region attribution de l'article au panier
                                        Model.Prestashop.PsCartProductRepository PsCartProductRepository = new Model.Prestashop.PsCartProductRepository();
                                        Model.Prestashop.PsCartProduct PsCartProduct = new Model.Prestashop.PsCartProduct()
                                        {
                                            DateAdd = DateTime.Now,
                                            IDAddressDelivery = PsCart.IDAddressDelivery,
                                            IDCart = PsCart.IDCart,
                                            IDProduct = (uint)Core.Global.GetConfig().ModulePreorderPrestashopProduct,
                                            IDProductAttribute = 0,
                                            IDShop = PsCart.IDShop,
                                            Quantity = 1,
                                        };
                                        PsCartProductRepository.Add(PsCartProduct);
                                        #endregion

                                        #region ajout de l'identification du panier en tant que solde de précommande
                                        Model.Prestashop.PsCartPreOrder PsCartPreorder = new Model.Prestashop.PsCartPreOrder()
                                        {
                                            IDCart = PsCart.IDCart,
                                            IDPreOrder = PsOrder.IDOrder,
                                        };
                                        PsCartPreOrderRepository.Add(PsCartPreorder);
                                        #endregion

                                        // génération URL directe panier
                                        PsOrder.Cart_URL = Core.Global.URL_Prestashop + "/commande?step=1&recover_cart=" + PsCart.IDCart
                                            + "&token_cart=" + Core.RandomString.HashMD5(Core.Global.GetConfig().TransfertPrestashopCookieKey + "recover_cart_" + PsCart.IDCart.ToString());

                                        Core.Sync.SynchronisationCommande.SendMail(33, PsOrder);
                                    }
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

            lock (this)
            {
                this.CurrentCount += 1;
            }
            this.ReportProgress(this.CurrentCount * 100 / this.ListCount);
            this.Semaphore.Release();
        }

        public void ReportProgress(Int32 Percentage)
        {
            Context.Post(state =>
            {
                this.ProgressBar.Value = Percentage;
                this.LabelInformation.Content = "Informations : " + Percentage + " %";
                if (this.CurrentCount == this.ListCount)
                {
                    this.Close();
                }
            }, null);
        }
    }
}