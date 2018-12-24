using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace PRESTACONNECT
{
    /// <summary>
    /// Logique d'interaction pour SynchronisationStatutCommande.xaml
    /// </summary>
    public partial class SynchronisationStatutCommande : Window
    {
        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;

        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public SynchronisationStatutCommande(int NJours = 0)
        {
            this.InitializeComponent();

            DateTime filtredatestatut = DateTime.Now.Date.AddDays(-NJours);

            // récupération des commandes PrestaShop (le filtrage par date est appliqué dans la requête SQL)
            Model.Prestashop.PsOrdersRepository PsOrdersRepository = new Model.Prestashop.PsOrdersRepository();
            List<Model.Prestashop.idorder> ListOrders =
                (NJours == 0) ? PsOrdersRepository.ListID(Core.Global.CurrentShop.IDShop)
                : PsOrdersRepository.ListID(Core.Global.CurrentShop.IDShop, filtredatestatut); ;

            Model.Local.OrderRepository OrderRepository = new Model.Local.OrderRepository();
            List<int> ListLocal = OrderRepository.ListPrestaShop();

            // filtrage sur uniquement les commandes synchronisées
            List<Model.Prestashop.idorder> ListSync = ListOrders.Where(o => ListLocal.Contains((int)o.id_order)).OrderByDescending(o => o.id_order).ToList();
            this.ListCount = ListSync.Count;

            Context = SynchronizationContext.Current;
            this.ReportProgress(0);
            if (ListCount > 0)
            {
                this.ParallelOptions.MaxDegreeOfParallelism = MaximumThreadCount;
                Task.Factory.StartNew(() =>
                {
                    Parallel.ForEach(ListSync, this.ParallelOptions, Sync);
                });
            }
        }

        public void Sync(Model.Prestashop.idorder Order)
        {
            this.Semaphore.WaitOne();

            try
            {
                Model.Prestashop.PsOrdersRepository PsOrdersRepository = new Model.Prestashop.PsOrdersRepository();
                Model.Prestashop.PsOrders PsOrders = PsOrdersRepository.ReadOrder((int)Order.id_order);

                Core.Sync.SynchronisationCommande SynchronisationCommande = new Core.Sync.SynchronisationCommande();
                SynchronisationCommande.ExecLocalToDistant(PsOrders, PsOrdersRepository);
                PsOrders = null;
                PsOrdersRepository = null;
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
                this.ProgressBarCommande.Value = Percentage;
                this.LabelInformation.Content = "Informations : " + Percentage + " %";
                if (this.CurrentCount == this.ListCount)
                {
                    this.Close();
                }
            }, null);
        }
    }
}