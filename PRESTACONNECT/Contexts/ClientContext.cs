using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Contexts
{
    internal sealed class ClientContext : Context
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

        public ClientContext()
            : base()
        {
        }
    }
}
