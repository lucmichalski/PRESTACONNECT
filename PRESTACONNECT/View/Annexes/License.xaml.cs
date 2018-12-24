using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PRESTACONNECT
{
    /// <summary>
    /// Logique d'interaction pour License.xaml
    /// </summary>
    public partial class License : Window
    {
        public License()
        {
            this.InitializeComponent();
            string value = "Votre licence Prestaconnect est inactive !";
            switch (Core.UpdateVersion.LicenseIsActive)
            {
                default:
                case PRESTACONNECT.Core.UpdateVersion.LicenceActivation.disabled:
                    value = "Votre licence Prestaconnect est inactive !";
                    break;
                case PRESTACONNECT.Core.UpdateVersion.LicenceActivation.expired:
                    value = "Nous vous informons que votre DUA est arrivé à expiration !" 
                        + "\n" + "Veuillez contactez votre revendeur pour le renouveler !";
                    break;
                case PRESTACONNECT.Core.UpdateVersion.LicenceActivation.incorrect:
                    value = "Les informations de connexion liées à la licence \"" + Core.UpdateVersion.License.LicenseKey + "\" ne correspondent pas à celle de votre instance !"
                        + "\n" + "Vous pouvez contacter votre revendeur pour vérifier vos paramètres !";
                    break;
                case PRESTACONNECT.Core.UpdateVersion.LicenceActivation.enabled:
                    value = "Votre licence Prestaconnect est opérationnelle !";
                    break;
            }
            this.TextBlockLicense.Text = value;
        }
    }
}