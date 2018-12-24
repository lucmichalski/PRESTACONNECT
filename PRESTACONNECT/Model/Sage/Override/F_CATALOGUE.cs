using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Sage
{
    public partial class F_CATALOGUE
    {
        private String m_CL_cbMarqWithParent;
        public String CL_cbMarqWithParent
        {
            get
            {
                return this.m_CL_cbMarqWithParent;
            }
            set
            {
                this.m_CL_cbMarqWithParent = value;
                OnPropertyChanged("CL_cbMarqWithParent");
            }
        }

        private bool toImport;
        public bool ToImport
        {
            get { return toImport; }
            set
            {
                toImport = value;
                OnPropertyChanged("ToImport");
            }
        }

        private bool checkChild = true;
        public bool CheckChild
        {
            get { return checkChild; }
            set
            {
                checkChild = value;
                OnPropertyChanged("CheckChild");
            }
        }

        public String ComboText
        {
            get
            {
                string t = CL_Intitule;
                if (this.F_CATALOGUE1 != null)
                {
                    t = this.F_CATALOGUE1.CL_Intitule + " >> " + t;
                    if (this.F_CATALOGUE1.F_CATALOGUE1 != null)
                    {
                        t = this.F_CATALOGUE1.F_CATALOGUE1.CL_Intitule + " >> " + t;
                        if (this.F_CATALOGUE1.F_CATALOGUE1.F_CATALOGUE1 != null)
                        {
                            t = this.F_CATALOGUE1.F_CATALOGUE1.F_CATALOGUE1.CL_Intitule + " >> " + t;
                            if (this.F_CATALOGUE1.F_CATALOGUE1.F_CATALOGUE1.F_CATALOGUE1 != null)
                            {
                                t = this.F_CATALOGUE1.F_CATALOGUE1.F_CATALOGUE1.F_CATALOGUE1.CL_Intitule + " >> " + t;
                            }
                        }
                    }
                }
                return t;
            }
        }

        public IQueryable<F_CATALOGUE> SortChildren
        {
            get { return F_CATALOGUE2.OrderBy(c => c.CL_Intitule).AsQueryable(); }
        }

        #region Events

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Methods

        public bool ExistLocal
        {
            get
            {
                return Core.Temp.ListCatalogLocal.Contains(this.cbMarq);
            }
        }

        public override string ToString()
        {
            return CL_Intitule;
        }

        #endregion
    }
}
