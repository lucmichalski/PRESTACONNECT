using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class MediaRule : INotifyPropertyChanged
    {
        public static string[] MediaRuleName = { "Importation en tant que document",
                                                 "Importation en tant qu'image",
                                                 "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini", "Non défini" };
        
        #region Properties

        public Core.Parametres.MediaRule _MediaRule;

        public int Marq
        {
            get { return (short)_MediaRule; }
        }

        public string Intitule
        {
            get
            {
                switch (_MediaRule)
                {
                    case Core.Parametres.MediaRule.AsAttachment:
                    case Core.Parametres.MediaRule.AsPicture:
                        return MediaRuleName[Marq];

                    default:
                        return string.Empty;
                }
            }
        }

        #endregion
        #region Constructors

        public MediaRule(Core.Parametres.MediaRule MediaRuleEnum)
        {
            _MediaRule = MediaRuleEnum;
        }

        public MediaRule(String MediaRuleValue)
        {
            _MediaRule = Core.Parametres.MediaRule.AsAttachment;
            if (Core.Global.IsInteger(MediaRuleValue))
            {
                int value = int.Parse(MediaRuleValue);
                switch (value)
                {
                    case (int)Core.Parametres.MediaRule.AsAttachment:
                    case (int)Core.Parametres.MediaRule.AsPicture:
                        _MediaRule = (Core.Parametres.MediaRule)value;
                        break;
                    default:
                        //noaction
                        break;
                }
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

        #region Methods

        public override string ToString()
        {
            return Intitule;
        }

        #endregion
    }
}
