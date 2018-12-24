using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PRESTACONNECT.Model.Internal
{
    public class RegexMail : INotifyPropertyChanged
    {
        public static string[] RegexMailText = { "Niveau 0",
                                                    "Non défini", "Non défini", "Non défini",
                                                 "Niveau 4",
                                                    "Non défini", "Non défini", "Non défini",
                                                 "Niveau 8",
                                                    "Non défini", "Non défini", "Non défini",
                                                 "Niveau 12",
                                                    "Non défini", "Non défini", "Non défini",
                                                 "Niveau 16",
                                                    "Non défini", "Non défini", "Non défini", "Non défini", };
        public static string[] RegexMailDescription = { "Les lettres minuscules, chiffres et les séparateurs -.",
                                                    "Non défini", "Non défini", "Non défini",
                                                 "Les lettres minuscules, majuscules, chiffres et les séparateurs -.",
                                                    "Non défini", "Non défini", "Non défini",
                                                 "Les lettres minuscules, majuscules, chiffres et caractères spéciaux suivants !#$%&'*+/=?^_`{|}~-" + "\nCaractères spéciaux autorisés uniquement dans \"l'identifiant\".",
                                                    "Non défini", "Non défini", "Non défini",
                                                 "Les lettres minuscules, majuscules, accentuées, chiffres et caractères spéciaux suivants !#$%&'*+/=?^_`{|}~-" + "\nCaractères spéciaux autorisés uniquement dans \"l'identifiant\".",
                                                    "Non défini", "Non défini", "Non défini",
                                                 "Tous les caractères pour \"l'identifiant\" de l'adresse s'il est entouré de guillemets, puis le niveau 12 pour le domaine",
                                                    "Non défini", "Non défini", "Non défini", "Non défini", };

        #region Properties

        public Core.Parametres.RegexMail _RegexMailLevel;

        public int Marq
        {
            get { return (short)_RegexMailLevel; }
        }

        public string Intitule
        {
            get
            {                
                switch (_RegexMailLevel)
                {
                    case Core.Parametres.RegexMail.lvl00_ld:
                    case Core.Parametres.RegexMail.lvl04_lUd:
                    case Core.Parametres.RegexMail.lvl08_lUdS:
                    case Core.Parametres.RegexMail.lvl12_lUAdS:
                    case Core.Parametres.RegexMail.lvl16_Q:
                        return RegexMailText[Marq];
                    default:
                        return string.Empty;
                }
            }
        }
        public string Description
        {
            get
            {
                switch (_RegexMailLevel)
                {
                    case Core.Parametres.RegexMail.lvl00_ld:
                    case Core.Parametres.RegexMail.lvl04_lUd:
                    case Core.Parametres.RegexMail.lvl08_lUdS:
                    case Core.Parametres.RegexMail.lvl12_lUAdS:
                    case Core.Parametres.RegexMail.lvl16_Q:
                        return "Autorise : " + RegexMailDescription[Marq];
                    default:
                        return string.Empty;
                }
            }
        }

        public string RegexExpression
        {
            // regex PrestaShop 
            // [a-z\p{L}0-9!#$%&\'*+\/=?^`{}|~_-]+[.a-z\p{L}0-9!#$%&\'*+\/=?^`{}|~_-]*@[a-z\p{L}0-9]+(?:[.]?[_a-z\p{L}0-9-])*\.[a-z\p{L}0-9]
            get
            {
                switch (_RegexMailLevel)
                {
                    default:
                    case Core.Parametres.RegexMail.lvl00_ld:
                        return @"\A(?:[a-z0-9]+((?:\.[a-z0-9]+)|(?:\-[a-z0-9]+))*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
                    case Core.Parametres.RegexMail.lvl04_lUd:
                        return @"\A(?:[a-zA-Z0-9]+((?:\.[a-zA-Z0-9]+)|(?:\-[a-zA-Z0-9]+))*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?)\Z";
                    case Core.Parametres.RegexMail.lvl08_lUdS:
                        return @"\A(?:[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?)\Z";
                    case Core.Parametres.RegexMail.lvl12_lUAdS:
                        return @"\A(?:[\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\w](?:[\w-]*[`\w])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
                    case Core.Parametres.RegexMail.lvl16_Q:
                        return @"\A(?:([\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*" + "|\"([^\"]*)\")" + @"@(?:[\w](?:[\w-]*[`\w])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
                }
            }
        }

        #endregion
        #region Constructors

        public RegexMail(Core.Parametres.RegexMail RegexMailEnum)
        {
            _RegexMailLevel = RegexMailEnum;
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
