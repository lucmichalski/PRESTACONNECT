using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class ProductFilterActiveDefault : INotifyPropertyChanged
    {
        public static string[] ProductFilterActiveDefaultText = { "Tous les articles",
                                                 "Uniquement les articles actifs",
                                                 "Uniquement les articles inactifs",
                                                 "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini" };

        #region Properties

        public Core.Parametres.ProductFilterActiveDefault _ProductFilterActiveDefault;

        public int Marq
        {
            get { return (short)_ProductFilterActiveDefault; }
        }

        public string Intitule
        {
            get
            {
                switch (_ProductFilterActiveDefault)
                {
                    case Core.Parametres.ProductFilterActiveDefault.ActiveProducts:
                    case Core.Parametres.ProductFilterActiveDefault.InactiveProducts:
                    case Core.Parametres.ProductFilterActiveDefault.AllProducts:
                        return ProductFilterActiveDefaultText[Marq];
                    default:
                        return string.Empty;
                }
            }
        }

        #endregion
        #region Constructors

        public ProductFilterActiveDefault(Core.Parametres.ProductFilterActiveDefault ProductFilterActiveDefaultEnum)
        {
            _ProductFilterActiveDefault = ProductFilterActiveDefaultEnum;
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

        #region Methods

        public override string ToString()
        {
            return Intitule;
        }

        #endregion
    }
}
