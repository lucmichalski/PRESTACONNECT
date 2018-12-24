using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public partial class P_CRISQUE
    {
        public enum _Enum_R_Type {Livraison, Surveillance, Blocage }

        private String m_R_TypeString;

        public String R_TypeString
        {
            get
            {
                switch (this.R_Type)
                {
                    case 0:
                        this.m_R_TypeString = "A livrer";
                        break;
                    case 1 :
                        this.m_R_TypeString = "A surveiller";
                        break;
                    case 2 :
                        this.m_R_TypeString = "A bloquer";
                        break;
                }
                return this.m_R_TypeString;
            }
        }

        private Model.Prestashop.PsGroupLang selectedPsGroup;
        public Model.Prestashop.PsGroupLang SelectedPsGroup
        {
            get { return selectedPsGroup; }
            set { selectedPsGroup = value; SendPropertyChanged("SelectedPsGroup"); }
        }

        private Model.Prestashop.PsGroupLang selectedPsGroupDefault;
        public Model.Prestashop.PsGroupLang SelectedPsGroupDefault
        {
            get { return selectedPsGroupDefault; }
            set
            {
                selectedPsGroupDefault = value;
                SendPropertyChanged("SelectedPsGroupDefault");

                if (value == null)
                    LockCondition = false;
            }
        }

        private Boolean lockCondition = false;
        public Boolean LockCondition
        {
            get { return lockCondition; }
            set
            {
                if (SelectedPsGroupDefault == null && value)
                    System.Windows.MessageBox.Show("Veuillez sélectionner un groupe par défaut pour activer le détachement !", "", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                else
                {
                    lockCondition = value;
                    SendPropertyChanged("LockCondition");
                }
            }
        }

    }
}
