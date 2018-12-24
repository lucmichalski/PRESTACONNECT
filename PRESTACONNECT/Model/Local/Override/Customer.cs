using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Local
{
    public partial class Customer
    {
        private String pre_numero = string.Empty;
        private String pre_name = string.Empty;
        private String sag_name = string.Empty;
        private String sag_numero = string.Empty;

        private bool load_prenumero = false;
        private bool load_prename = false;
        private bool load_sagname = false;
        private bool load_sagnumero = false;

        public String Pre_Numero
        {
            get
            {
                if (!load_prenumero)
                {
                    pre_numero = Pre_Id.ToString();
                    load_prenumero = true;
                }
                return pre_numero;
            }
        }
        public String Pre_Name
        {
            get
            {
                if (!load_prename)
                {
                    if(Core.Temp.ListPrestashopCustomerBtoB.Count(p => p.id_customer == Pre_Id) == 1)
                        pre_name = Core.Temp.ListPrestashopCustomerBtoB.FirstOrDefault(p => p.id_customer == Pre_Id).BtoBShortString;
                    load_prename = true;
                }
                return pre_name;
            }
        }

        public String Sag_Name
        {
            get
            {
                if (!load_sagname)
                {
                    if (Core.Temp.ListF_COMPTET_BtoB.Count(s => s.cbMarq == Sag_Id) == 1)
                        sag_name = Core.Temp.ListF_COMPTET_BtoB.FirstOrDefault(s => s.cbMarq == Sag_Id).CT_Intitule;
                    load_sagname = true;
                }
                return sag_name;
            }
        }
        public String Sag_Numero
        {
            get
            {
                if (!load_sagnumero)
                {
                    if (Core.Temp.ListF_COMPTET_BtoB.Count(s => s.cbMarq == Sag_Id) == 1)
                        sag_numero = Core.Temp.ListF_COMPTET_BtoB.FirstOrDefault(s => s.cbMarq == Sag_Id).CT_Num;
                    load_sagnumero = true;
                }
                return sag_numero;
            }
        }

        #region Methods

        public override string ToString()
        {
            return Pre_Name + " / " + Sag_Name;
        }

        #endregion
    }

    public class Customer_Progress : INotifyPropertyChanged
    {
        public string _CT_Num = string.Empty;
        public string CT_Num
        {
            get { return _CT_Num; }
            set { _CT_Num = value; OnPropertyChanged("CT_Num"); }
        }
        public string _CT_Intitule = string.Empty;
        public string CT_Intitule
        {
            get { return _CT_Intitule; }
            set { _CT_Intitule = value; OnPropertyChanged("CT_Intitule"); }
        }

        public string _Comment = string.Empty;
        public string Comment
        {
            get { return _Comment; }
            set { _Comment = (value != null) ? value : string.Empty; OnPropertyChanged("Comment"); OnPropertyChanged("StringProgress"); }
        }

        // constructor for clone datas
        public Customer_Progress() { }
        public Customer_Progress(Model.Local.Customer_Progress source)
        {
            this.CT_Num = source.CT_Num;
            this.CT_Intitule = source.CT_Intitule;
            this.Comment = source.Comment;
        }

        public string StringProgress
        {
            get { return (!string.IsNullOrWhiteSpace(CT_Num) ? CT_Num + " - " + CT_Intitule : CT_Intitule) + (!string.IsNullOrWhiteSpace(Comment) ? " : " + Comment : string.Empty); }
            set { }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
