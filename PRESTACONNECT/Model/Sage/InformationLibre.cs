using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Sage
{
    public class InformationLibre : INotifyPropertyChanged
    {
        #region Attributes

        private String m_Name;

        private String m_Value;

        private Int32 m_Pos;

        #endregion

        #region Properties

        public String Name
        {
            get
            {
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
                this.OnPropertyChanged("Name");
            }
        }

        public String Value
        {
            get
            {
                return this.m_Value;
            }
            set
            {
                this.m_Value = value;
                this.OnPropertyChanged("Value");
            }
        }

        public Int32 Pos
        {
            get
            {
                return this.m_Pos;
            }
            set
            {
                this.m_Pos = value;
                this.OnPropertyChanged("Pos");
            }
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
