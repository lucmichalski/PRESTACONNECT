using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class ImportSageFilterTargetData : INotifyPropertyChanged
    {
        public static string[] ImportSageFilterTargetDataText = { "Articles / Énumérés gammes et conditionnement", // 0
                                                 "Articles / Énumérés gammes", // 1
                                                 "Articles / Énumérés conditionnement", // 2
                                                 "Énumérés gammes et conditionnement", // 3
                                                 "Articles", // 4
                                                 "Énumérés gammes", // 5
                                                 "Énumérés conditionnement", // 6
                                                 "Non défini", "Non défini", "Non défini", // 7-8-9
                                                 "Information libre article", // 10
                                                                };

        #region Properties

        public Core.Parametres.ImportSageFilterTargetData _ImportSageFilterTargetData;

        public int Marq
        {
            get { return (short)_ImportSageFilterTargetData; }
        }

        public string Intitule
        {
            get
            {
                switch (_ImportSageFilterTargetData)
                {
                    case Core.Parametres.ImportSageFilterTargetData.ArticleGammeConditionnement:
                    case Core.Parametres.ImportSageFilterTargetData.ArticleGamme:
                    case Core.Parametres.ImportSageFilterTargetData.ArticleConditionnement:
                    case Core.Parametres.ImportSageFilterTargetData.GammeConditionnement:
                    case Core.Parametres.ImportSageFilterTargetData.Article:
                    case Core.Parametres.ImportSageFilterTargetData.Gamme:
                    case Core.Parametres.ImportSageFilterTargetData.Conditionnement:
                    case Core.Parametres.ImportSageFilterTargetData.InformationLibreArticle:
                        return ImportSageFilterTargetDataText[Marq];
                    default:
                        return string.Empty;
                }
            }
        }

        public System.Windows.Visibility VisibilityInfolibre
        {
            get
            {
                return (_ImportSageFilterTargetData == Core.Parametres.ImportSageFilterTargetData.InformationLibreArticle)
                    ? System.Windows.Visibility.Visible
                    : System.Windows.Visibility.Collapsed;
            }
        }

        #endregion
        #region Constructors

        public ImportSageFilterTargetData(Core.Parametres.ImportSageFilterTargetData ImportSageFilterTargetDataEnum)
        {
            _ImportSageFilterTargetData = ImportSageFilterTargetDataEnum;
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
