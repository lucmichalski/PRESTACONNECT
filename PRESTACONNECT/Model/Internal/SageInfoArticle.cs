using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class SageInfoArticle : INotifyPropertyChanged
    {
        public static string[] SageInfoArticleText = { "Article de substitution", 
                                                         "Code Famille",
                                                         "Intitulé Famille",
                                                         "Durée de garantie",
                                                         "Pays d'origine",
                                                         "Poids Net",
                                                         "Poids Brut",
                                                         "Non défini", "Non défini", "Non défini" };

        #region Properties

        public Core.Parametres.SageInfoArticle _SageInfoArticle;

        public int Marq
        {
            get { return (short)_SageInfoArticle; }
        }

        public string Intitule
        {
            get
            {
                switch (_SageInfoArticle)
                {
                    case Core.Parametres.SageInfoArticle.Substitut:
                    case Core.Parametres.SageInfoArticle.FamilleCode:
                    case Core.Parametres.SageInfoArticle.FamilleIntitule:
                    case Core.Parametres.SageInfoArticle.Garantie:
                    case Core.Parametres.SageInfoArticle.Pays:
                    case Core.Parametres.SageInfoArticle.PoidsNet:
                    case Core.Parametres.SageInfoArticle.PoidsBrut:
                        return SageInfoArticleText[Marq];
                    default:
                        return string.Empty;
                }
            }
        }

        #endregion
        #region Constructors

        public SageInfoArticle(Core.Parametres.SageInfoArticle SageInfoArticleEnum)
        {
            _SageInfoArticle = SageInfoArticleEnum;
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
