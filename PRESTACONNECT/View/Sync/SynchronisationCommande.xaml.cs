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
    /// Logique d'interaction pour SynchronisationCommande.xaml
    /// </summary>
    public partial class SynchronisationCommande : Window
    {
        private BackgroundWorker worker = new BackgroundWorker();
        public Int32 CurrentCount = 0;
        ABSTRACTION_SAGE.ALTERNETIS.Connexion Connexion;

        #region MajPrestaShop
        public Int32 ListCount = 0;
        public List<int> ListSync = new List<int>();
        public SynchronizationContext Context;
        public Semaphore Semaphore = (Core.Global.GetConfig().ConfigUnlockProcessorCore)
            ? new Semaphore(Core.Global.GetConfig().ConfigAllocatedProcessorCore, Core.Global.GetConfig().ConfigAllocatedProcessorCore)
            : new Semaphore((System.Environment.ProcessorCount <= 4) ? System.Environment.ProcessorCount : 4,
                (System.Environment.ProcessorCount <= 4) ? System.Environment.ProcessorCount : 4);
        private ParallelOptions ParallelOptions = new ParallelOptions();
        #endregion

        public SynchronisationCommande(DateTime? filtre = null)
        {
            this.InitializeComponent();
            this.LabelInformation.Content = "Recherche des commandes à transférer ...";

            this.worker.WorkerReportsProgress = true;
            this.worker.DoWork += delegate(object s, DoWorkEventArgs args)
            {
                // récupération des commandes PrestaShop (le filtrage par date est appliqué dans la requête SQL)
                Model.Prestashop.PsOrdersRepository PsOrdersRepository = new Model.Prestashop.PsOrdersRepository();
                List<Model.Prestashop.idorder> ListOrders = PsOrdersRepository.ListID(Core.Global.CurrentShop.IDShop);

                if (ListOrders.Count > 0)
                {
                    Model.Local.OrderRepository OrderRepository = new Model.Local.OrderRepository();
                    this.ListSync = OrderRepository.ListPrestaShop();

                    // filtrage sur uniquement les commandes non synchronisées et dont le statut demande une création dans Sage
                    Model.Local.ConfigRepository ConfigRepository = new Model.Local.ConfigRepository();
                    Model.Local.Config ConfigBC = (ConfigRepository.ExistName(Core.Global.ConfigCommandeStatutCreateBC)) ? ConfigRepository.ReadName(Core.Global.ConfigCommandeStatutCreateBC) : null;
                    Model.Local.Config ConfigDevis = (ConfigRepository.ExistName(Core.Global.ConfigCommandeStatutCreateDevis)) ? ConfigRepository.ReadName(Core.Global.ConfigCommandeStatutCreateDevis) : null;
                    List<string> idstate_string = new List<string>();
                    idstate_string.AddRange((ConfigBC != null && !string.IsNullOrEmpty(ConfigBC.Con_Value)) ? ConfigBC.Con_Value.Split('#') : string.Empty.Split('#'));
                    idstate_string.AddRange((ConfigDevis != null && !string.IsNullOrEmpty(ConfigDevis.Con_Value)) ? ConfigDevis.Con_Value.Split('#') : string.Empty.Split('#'));

                    List<Model.Prestashop.idorder> ListNotSync = ListOrders.Where(o => !this.ListSync.Contains((int)o.id_order) && idstate_string.Contains(o.current_state.ToString())).ToList();

                    this.ListCount = ListNotSync.Count;

                    if (ListNotSync.Count > 0)
                    {
                        // ouverture de la connexion ODBC
                        this.worker.ReportProgress(-42);
                        Connexion = Core.Global.GetODBC();

                        Model.Prestashop.PsOrders PsOrders = null;
                        if (Connexion != null)
                        {
                            foreach (Model.Prestashop.idorder Orders in ListNotSync)
                            {
                                try
                                {
                                    PsOrders = PsOrdersRepository.ReadOrder((int)Orders.id_order);
                                    Core.Sync.SynchronisationCommande SynchronisationCommande = new Core.Sync.SynchronisationCommande();
                                    SynchronisationCommande.Exec(Connexion, PsOrders, PsOrdersRepository);
                                }
                                catch (Exception ex)
                                {
                                    Core.Error.SendMailError(ex.ToString());
                                }
                                lock (this)
                                {
                                    this.CurrentCount += 1;
                                }
                                this.worker.ReportProgress((this.CurrentCount * 100 / this.ListCount));
                            }
                        }
                        else
                        {
                            this.CurrentCount += ListNotSync.Count;
                        }
                    }
                }
            };

            this.worker.ProgressChanged += delegate(object s, ProgressChangedEventArgs args)
            {
                if (args.ProgressPercentage >= 0)
                {
                    this.ProgressBarCommande.Value = args.ProgressPercentage;
                    this.LabelInformation.Content = "Informations : " + args.ProgressPercentage + " %";
                }
                else if (args.ProgressPercentage == -42)
                {
                    this.LabelInformation.Content = "Ouverture connexion ODBC Sage...";
                }
            };

            this.worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                if (Connexion != null)
                    Connexion.Close_Connexion();
                this.Close();
            };

            // Insérez le code requis pour la création d’objet sous ce point.
            this.worker.RunWorkerAsync();
        }
    }
}