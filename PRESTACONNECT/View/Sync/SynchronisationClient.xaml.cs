using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.ComponentModel;

namespace PRESTACONNECT
{
    /// <summary>
    /// Logique d'interaction pour SynchronisationClient.xaml
    /// </summary>
    public partial class SynchronisationClient : Window
    {
        ABSTRACTION_SAGE.ALTERNETIS.Connexion Connexion;
        private BackgroundWorker worker = new BackgroundWorker();
        public Int32 CurrentCount = 0;

        public SynchronisationClient()
        {
            this.InitializeComponent();
            this.worker.WorkerReportsProgress = true;
            this.LabelInformation.Content = "Recherche des clients à créer ...";

            this.worker.DoWork += delegate(object s, DoWorkEventArgs args)
            {
                if (Core.Global.GetConfig().ConfigBToC)
                {
                    Model.Prestashop.PsCustomerRepository PsCustomerRepository = new Model.Prestashop.PsCustomerRepository();
                    List<Model.Prestashop.idcustomer> ListCustomer = (Core.Global.GetConfig().ConfigClientFiltreCommande)
                        ? PsCustomerRepository.ListIDActive(1, Core.Global.CurrentShop.IDShop)
                        : PsCustomerRepository.ListIDActiveWithOrder(1, Core.Global.CurrentShop.IDShop);

                    // <JG> ajout filtre clients déjà mappés
                    List<Model.Local.Customer> listLocal = new Model.Local.CustomerRepository().List();
                    ListCustomer = ListCustomer.Where(c => listLocal.Count(l => l.Pre_Id == c.id_customer) == 0).ToList();
                    this.worker.ReportProgress(0);

                    if (ListCustomer.Count > 0)
                    {
                        this.worker.ReportProgress(-42);
                        Connexion = Core.Global.GetODBC();

                        foreach (Model.Prestashop.idcustomer Customer in ListCustomer)
                        {
                            if (Connexion != null)
                            {
                                Core.Sync.SynchronisationClient SynchronisationClient = new Core.Sync.SynchronisationClient();
                                SynchronisationClient.Exec(Connexion, Customer.id_customer);
                            }
                            lock (this)
                            {
                                this.CurrentCount += 1;
                            }
                            this.worker.ReportProgress((this.CurrentCount * 100 / ListCustomer.Count));
                        }
                    }
                }
            };

            this.worker.ProgressChanged += delegate(object s, ProgressChangedEventArgs args)
            {
                if (args.ProgressPercentage >= 0)
                {
                    this.ProgressBarClient.Value = args.ProgressPercentage;
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