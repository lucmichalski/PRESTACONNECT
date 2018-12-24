using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace PRESTACONNECT.Contexts
{
    internal sealed class CommandeContext : Context
    {
        private bool syncFull = true;
        public bool SyncFull
        {
            get { return syncFull; }
            set
            {
                syncFull = value;
                OnPropertyChanged("SyncFull");
            }
        }
        private bool syncDay = false;
        public bool SyncDay
        {
            get { return syncDay; }
            set
            {
                syncDay = value;
                OnPropertyChanged("SyncDay");
            }
        }
        private bool syncWeek = false;
        public bool SyncWeek
        {
            get { return syncWeek; }
            set
            {
                syncWeek = value;
                OnPropertyChanged("SyncWeek");
            }
        }
        private bool syncMonth = false;
        public bool SyncMonth
        {
            get { return syncMonth; }
            set
            {
                syncMonth = value;
                OnPropertyChanged("SyncMonth");
            }
        }

        public string ModuleInvoiceNumero
        {
            get { return Core.Temp.ModuleAECInvoiceHistory_Numero; }
            set { Core.Temp.ModuleAECInvoiceHistory_Numero = value; OnPropertyChanged("ModuleInvoiceNumero"); }
        }
        public string ModuleInvoiceIntitule
        {
            get { return Core.Temp.ModuleAECInvoiceHistory_Intitule; }
            set { Core.Temp.ModuleAECInvoiceHistory_Intitule = value; OnPropertyChanged("ModuleInvoiceIntitule"); }
        }

        public Visibility VisibilityProcessorCoreOverride
        {
            get { return (System.Environment.ProcessorCount <= 4) ? Visibility.Hidden : Visibility.Visible; }
        }
        public bool UnlockProcessorCore
        {
            get { return Core.Temp.UnlockProcessorCore; }
            set { Core.Temp.UnlockProcessorCore = value; OnPropertyChanged("UnlockProcessorCore"); }
        }

        public CommandeContext()
            : base()
        {
        }
    }
}
