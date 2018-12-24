using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Threading;
using System.Threading.Tasks;

namespace PRESTACONNECT
{
    /// <summary>
    /// Logique d'interaction pour AECRepresentative.xaml
    /// </summary>
    public partial class AECRepresentative : Window
    {

        public Int32 CurrentCount = 0;
        public Int32 ListCount = 0;
        public SynchronizationContext Context;
        public static int MaximumThreadCount;
        public Semaphore Semaphore = Core.Global.GetSemaphore(out MaximumThreadCount);
        private ParallelOptions ParallelOptions = new ParallelOptions();

        public AECRepresentative(List<Model.Sage.F_COLLABORATEUR> ListF_COLLABORATEUR = null)
        {
            this.InitializeComponent();
            if (ListF_COLLABORATEUR == null)
            {
                Model.Sage.F_COLLABORATEURRepository F_COLLABORATEURRepository = new Model.Sage.F_COLLABORATEURRepository();
                ListF_COLLABORATEUR = F_COLLABORATEURRepository.ListVendeur();
            }
            this.ListCount = ListF_COLLABORATEUR.Count;

            Context = SynchronizationContext.Current;
            this.ParallelOptions.MaxDegreeOfParallelism = MaximumThreadCount;
            this.ReportProgress(0);
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(ListF_COLLABORATEUR, this.ParallelOptions, Sync);
            });
        }

        public void Sync(Model.Sage.F_COLLABORATEUR F_COLLABORATEUR)
        {
            this.Semaphore.WaitOne();

            try
            {
                Model.Prestashop.PsAECRepresentativeRepository PsAECRepresentativeRepository = new Model.Prestashop.PsAECRepresentativeRepository();
                Model.Prestashop.PsAEcRepresentative PsAECRepresentative;
                if (PsAECRepresentativeRepository.ExistSage((uint)F_COLLABORATEUR.CO_No))
                {
                    PsAECRepresentative = PsAECRepresentativeRepository.ReadSage((uint)F_COLLABORATEUR.CO_No);
                    Affect(F_COLLABORATEUR, PsAECRepresentative);
                    PsAECRepresentativeRepository.Save();
                }
                else
                {
                    PsAECRepresentative = new Model.Prestashop.PsAEcRepresentative();
                    PsAECRepresentative.IDSage = (uint)F_COLLABORATEUR.CO_No;
                    Affect(F_COLLABORATEUR, PsAECRepresentative);
                    PsAECRepresentativeRepository.Add(PsAECRepresentative);
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

        private void Affect(Model.Sage.F_COLLABORATEUR F_COLLABORATEUR, Model.Prestashop.PsAEcRepresentative PsAECRepresentative)
        {
            PsAECRepresentative.LastName = F_COLLABORATEUR.CO_Nom;
            PsAECRepresentative.FirstName = F_COLLABORATEUR.CO_Prenom;
            PsAECRepresentative.Email = F_COLLABORATEUR.CO_EMail;
            PsAECRepresentative.Phone = F_COLLABORATEUR.CO_Telephone;
            PsAECRepresentative.Mobile = F_COLLABORATEUR.CO_TelPortable;
            PsAECRepresentative.Fax = F_COLLABORATEUR.CO_Telecopie;
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