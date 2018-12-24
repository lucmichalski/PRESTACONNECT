using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

namespace PRESTACONNECT
{
    /// <summary>
    /// Logique d'interaction pour SynchronisationLivraison.xaml
    /// </summary>
    public partial class SynchronisationLivraison : Window
    {
        private BackgroundWorker worker = new BackgroundWorker();
        ABSTRACTION_SAGE.ALTERNETIS.Connexion Connexion;
        public Int32 CurrentCount = 0;

        public SynchronisationLivraison()
        {
            this.InitializeComponent();
            this.LabelInformation.Content = "Recherche des adresses à créer ...";

            this.worker.WorkerReportsProgress = true;
            this.worker.DoWork += delegate(object s, DoWorkEventArgs args)
            {
                Model.Prestashop.PsAddressRepository PsAddressRepository = new Model.Prestashop.PsAddressRepository();
                List<Model.Prestashop.PsAddress_Light> ListPsAddress = (Core.Global.GetConfig().ConfigClientFiltreCommande)
                        ? PsAddressRepository.List()
                        : PsAddressRepository.ListWithOrder();

                // filtrage des adresses par rapport aux clients déjà synchronisés
                List<Model.Local.Customer> listCustomer = new Model.Local.CustomerRepository().List();
                ListPsAddress = ListPsAddress.Where(a => listCustomer.Count(c => c.Pre_Id == a.id_customer) > 0).ToList();

                List<Model.Local.Address> listLocal = new Model.Local.AddressRepository().List();
                ListPsAddress = ListPsAddress.Where(a => listLocal.Count(l => l.Pre_Id == a.id_address) == 0).ToList();

                if (ListPsAddress.Count > 0)
                {
                    this.worker.ReportProgress(-42);
                    Connexion = Core.Global.GetODBC();

                    Core.Temp.ListAddressOnCurrentSync = new List<uint>();
                    foreach (Model.Prestashop.PsAddress_Light PsAddress_Light in ListPsAddress)
                    {
                        if (Connexion != null)
                        {
                            Core.Sync.SynchronisationLivraison SynchronisationLivraison = new Core.Sync.SynchronisationLivraison();
                            SynchronisationLivraison.Exec(Connexion, PsAddress_Light.id_address);
                        }
                        lock (this)
                        {
                            this.CurrentCount += 1;
                        }
                        this.worker.ReportProgress((this.CurrentCount * 100 / ListPsAddress.Count));
                    }
                }
            };

            this.worker.ProgressChanged += delegate(object s, ProgressChangedEventArgs args)
            {
                if (args.ProgressPercentage >= 0)
                {
                    this.ProgressBarLivraison.Value = args.ProgressPercentage;
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