using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Linq;
using System.Net.Mail;
using System.Net;
using PRESTACONNECT.Core;

namespace PRESTACONNECT
{
    public partial class Commande : Window
    {
        public Model.Prestashop.ps_orders_resume CommandeSelectionne
        {
            get { return (Model.Prestashop.ps_orders_resume)GetValue(CommandeSelectionneProperty); }
            set { SetValue(CommandeSelectionneProperty, value); }
        }
        public static readonly DependencyProperty CommandeSelectionneProperty =
            DependencyProperty.Register("CommandeSelectionne", typeof(Model.Prestashop.ps_orders_resume), typeof(Commande));

        public bool CanAccept
        {
            get { return (bool)GetValue(CanAcceptProperty); }
            set { SetValue(CanAcceptProperty, value); }
        }
        public static readonly DependencyProperty CanAcceptProperty =
            DependencyProperty.Register("CanAccept", typeof(bool), typeof(Commande));

        internal new Contexts.CommandeContext DataContext
        {
            get { return (Contexts.CommandeContext)base.DataContext; }
            private set
            {
                base.DataContext = value;
            }
        }

        public Commande()
        {
            InitializeComponent();
            DataContext = new Contexts.CommandeContext();
            Actualiser();

            this.TabItemPrecommande.IsEnabled = Core.Global.ExistPreorderModule() && Core.Global.GetConfig().ModulePreorderActif;
            this.ButtonSendCartPreorder.IsEnabled = !string.IsNullOrEmpty(Core.Global.GetConfig().TransfertPrestashopCookieKey)
                && new Model.Local.OrderMailRepository().ExistType(33)
                && new Model.Prestashop.PsProductRepository().ProductIsActive((uint)Core.Global.GetConfig().ModulePreorderPrestashopProduct);

            this.TabItemInvoiceHistory.IsEnabled = Core.Global.ExistAECInvoiceHistoryModule() && Core.Global.GetConfig().ModuleAECInvoiceHistoryActif;
            this.ButtonSendInvoiceHistory.IsEnabled = System.IO.File.Exists(System.IO.Path.Combine(Core.Global.GetConfig().Folders.RootReport, "AEC_Invoice.rpt"));
            this.ButtonTestInvoiceCrystal.IsEnabled = System.IO.File.Exists(System.IO.Path.Combine(Core.Global.GetConfig().Folders.RootReport, "AEC_Invoice.rpt"));

            this.buttonSyncPrestaShopOnly.IsEnabled = Core.Global.GetConfig().HasPrestaShopStateToChange();
            this.ButtonSync.IsEnabled = Core.Global.GetConfig().HasPrestaShopStateToChange();

            this.buttonSyncPayment.IsEnabled = Core.Global.GetConfig().SyncReglementActif;

            if (Core.UpdateVersion.License.ExtranetOnly)
                this.TabItemInvoiceHistory.IsSelected = true;

            if (Core.Temp.Current != System.Windows.WindowState.Minimized)
                this.WindowState = Core.Temp.Current;
        }

        private void ButtonSync_Click(object sender, RoutedEventArgs e)
        {
            Sync(true, true);
        }

        private void ActualiserDernieresCommandes_Click(object sender, RoutedEventArgs e)
        {
            Actualiser();
        }

        private void Actualiser()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                PRESTACONNECT.Commande.ReadCommandePrestashop(DataGridCommande, this.numericUpDownTopOrder.Value);
            }
            finally { Mouse.OverrideCursor = null; }
        }

        private void DataGridCommande_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool canAccept = false;

            if (CommandeSelectionne != null)
            {
                Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                Model.Local.Config Config = new Model.Local.Config();

                String[] ArrayPaymentConfig = null;

                Config = ConfigRepository.ReadName(Core.Global.ConfigCommandePayment);

                if (Config != null)
                    ArrayPaymentConfig = Config.Con_Value.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var state in ArrayPaymentConfig)
                {
                    int stateId = Convert.ToInt32(state);

                    if ((canAccept = ((stateId != 2) && !CommandeSelectionne.Sync && stateId == CommandeSelectionne.current_state)))
                        break;
                }
            }

            CanAccept = canAccept;
        }

        private void PaiementAccepte_Click(object sender, RoutedEventArgs e)
        {
            if (CommandeSelectionne != null)
            {
                MessageBoxResult result = MessageBox.Show("Confirmez vous le passage de la commande en paiement accepté ?",
                    Title, MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        Model.Prestashop.PsOrderHistoryRepository PsOrderHistoryRepository = new Model.Prestashop.PsOrderHistoryRepository();
                        Model.Prestashop.PsOrderHistory PsOrderHistory = new Model.Prestashop.PsOrderHistory();

                        PsOrderHistory.IDOrder = CommandeSelectionne.id_order;
                        PsOrderHistory.IDOrderState = (uint)2;
                        PsOrderHistory.DateAdd = DateTime.Now;
                        PsOrderHistoryRepository.Add(PsOrderHistory);

                        // <JG> 06/03/2015 correction statut commande entete
                        Model.Prestashop.PsOrdersRepository PsOrdersRepository = new Model.Prestashop.PsOrdersRepository();
                        Model.Prestashop.PsOrders PsOrders = PsOrdersRepository.ReadOrder((int)CommandeSelectionne.id_order);
                        PsOrders.CurrentState = 2;
                        PsOrdersRepository.Save();

                        Model.Local.OrderMailRepository OrderMailRepository = new Model.Local.OrderMailRepository();
                        Model.Local.OrderMail OrderMail = OrderMailRepository.ReadType(9);

                        if (OrderMail != null && OrderMail.OrdMai_Active)
                            PRESTACONNECT.Core.Sync.SynchronisationCommande.SendMail(9, CommandeSelectionne.PsSource);

                        Actualiser();
                    }
                    catch (Exception ex)
                    {
                        Core.Error.SendMailError(ex.ToString());
                    }
                }
            }
        }

        private void ButtonSendCartPreorder_Click(object sender, RoutedEventArgs e)
        {
            PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
            Loading.Show();
            CartPreorder form = new CartPreorder();
            form.ShowDialog();
            Loading.Close();
        }

        private void ButtonSendInvoiceHistory_Click(object sender, RoutedEventArgs e)
        {
            PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
            Loading.Show();
            AECInvoiceHistory AECInvoiceHistory = new PRESTACONNECT.AECInvoiceHistory(this.datePickerStart.SelectedDate, this.datePickerEnd.SelectedDate);
            Loading.Close();
            AECInvoiceHistory.ShowDialog();
        }

        public static void ReadCommandePrestashop(DataGrid DataGrid, int Top)
        {
            Model.Prestashop.PsOrdersRepository PsOrdersRepository = new Model.Prestashop.PsOrdersRepository();
            List<Model.Prestashop.ps_orders_resume> ListPsOrders = PsOrdersRepository.ListTopOrderResumeByDateAdd(Top, Core.Global.Lang, Global.CurrentShop.IDShop);

            DataGrid.ItemsSource = ListPsOrders;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (CommandeSelectionne != null && CommandeSelectionne.Sync)
            {
                if (MessageBox.Show("Voulez-vous supprimer le marquage de la commande n° " + CommandeSelectionne.id_order
                    + " afin de la recréer dans Sage ?\n\nAttention au statut de la commande !", "Suppression mappage commande",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Model.Local.OrderRepository OrderRepository = new Model.Local.OrderRepository();
                    if (OrderRepository.ExistPrestashop((int)CommandeSelectionne.id_order))
                    {
                        OrderRepository.Delete(OrderRepository.ReadPrestashop((int)CommandeSelectionne.id_order));
                    }

                    Model.Local.OrderPaymentRepository OrderPayementRepository = new Model.Local.OrderPaymentRepository();
                    foreach (Model.Local.OrderPayment OrderPayment in OrderPayementRepository.ListOrder((int)CommandeSelectionne.id_order))
                    {
                        OrderPayementRepository.Delete(OrderPayment);
                    }
                }
            }
        }

        private void buttonSyncSageOnly_Click(object sender, RoutedEventArgs e)
        {
            Sync(true, false);
        }

        private void buttonSyncPrestaShopOnly_Click(object sender, RoutedEventArgs e)
        {
            if (Core.Global.GetConfig().HasPrestaShopStateToChange())
            {
                Sync(false, true);
            }
        }

        private void buttonTestInvoiceCrystal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PRESTACONNECT.View.CrystalViewer viewer = new PRESTACONNECT.View.CrystalViewer();
                viewer.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur d'affichage de l'aperçu PDF !", "Erreur aperçu PDF", MessageBoxButton.OK, MessageBoxImage.Error);
                Core.Error.SendMailError(ex.ToString());
            }
        }

        private void Sync(bool Sage, bool Prestashop)
        {

            PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
            Loading.Show();

            DateTime? filtre = null, old_config = Core.Global.GetConfig().ConfigCommandeFiltreDate;
            if (DataContext.SyncDay)
                filtre = DateTime.Now.Date;
            else if (DataContext.SyncWeek)
                filtre = DateTime.Now.Date.AddDays(-7);
            else if (DataContext.SyncMonth)
                filtre = DateTime.Now.Date.AddMonths(-1);

            if (filtre != null)
                Core.Global.GetConfig().UpdateConfigCommandeFiltreDate(filtre, true);

            if (Sage)
            {
                if (Core.Global.GetConfig().ConfigBToC)
                {
                    PRESTACONNECT.SynchronisationClient SynchronisationClient = new SynchronisationClient();
                    SynchronisationClient.ShowDialog();
                }

                PRESTACONNECT.SynchronisationLivraison SynchronisationLivraison = new SynchronisationLivraison();
                SynchronisationLivraison.ShowDialog();

                PRESTACONNECT.SynchronisationCommande Sync = new SynchronisationCommande(filtre);
                Sync.ShowDialog();
            }

            if (Prestashop && Core.Global.GetConfig().HasPrestaShopStateToChange())
            {
                int NJours = 0;
                if (DataContext.SyncDay)
                    NJours = 1;
                else if (DataContext.SyncWeek)
                    NJours = 7;
                else if (DataContext.SyncMonth)
                    NJours = 30;
                PRESTACONNECT.SynchronisationStatutCommande SyncStatut = new SynchronisationStatutCommande(NJours);
                SyncStatut.ShowDialog();
            }

            Loading.Close();

            if (filtre != null)
                Core.Global.GetConfig().UpdateConfigCommandeFiltreDate(old_config, true);

            Actualiser();
        }

        private void buttonSyncPayment_Click(object sender, RoutedEventArgs e)
        {
            PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
            Loading.Show();

            DateTime? filtre = null, old_config = Core.Global.GetConfig().ConfigCommandeFiltreDate;
            if (DataContext.SyncDay)
                filtre = DateTime.Now.Date;
            else if (DataContext.SyncWeek)
                filtre = DateTime.Now.Date.AddDays(-7);
            else if (DataContext.SyncMonth)
                filtre = DateTime.Now.Date.AddMonths(-1);

            if (filtre != null)
                Core.Global.GetConfig().UpdateConfigCommandeFiltreDate(filtre, true);

            SynchronisationPaiement form = new SynchronisationPaiement();
            form.ShowDialog();

            Loading.Close();

            if (filtre != null)
                Core.Global.GetConfig().UpdateConfigCommandeFiltreDate(old_config, true);

        }

        private void ButtonClearInvoiceHistory_Click(object sender, RoutedEventArgs e)
        {
            PRESTACONNECT.Loading Loading = new PRESTACONNECT.Loading();
            Loading.Show();
            AECInvoiceHistory AECInvoiceHistory = new PRESTACONNECT.AECInvoiceHistory(this.datePickerStart.SelectedDate, this.datePickerEnd.SelectedDate, true);
            Loading.Close();
            AECInvoiceHistory.ShowDialog();
        }
    }
}