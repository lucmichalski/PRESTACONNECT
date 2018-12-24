using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRESTACONNECT.Model.Sage
{
    public partial class F_COLLABORATEUR
    {
        public string CO_Intitule
        {
            get
            {
                return CO_Nom + " " + CO_Prenom;
            }
        }

        private Model.Prestashop.PsEmployee selectedPsEmployee;
        public Model.Prestashop.PsEmployee SelectedPsEmployee
        {
            get { return selectedPsEmployee; }
            set { selectedPsEmployee = value; SendPropertyChanged("SelectedPsEmployee"); }
        }
    }
}
