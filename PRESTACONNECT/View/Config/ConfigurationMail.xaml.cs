using System.Windows;

namespace PRESTACONNECT
{
	/// <summary>
	/// Logique d'interaction pour ConfigurationMail.xaml
	/// </summary>
	public partial class ConfigurationMail : Window
	{
		public ConfigurationMail()
		{
			this.InitializeComponent();
			
			// Insérez le code requis pour la création d’objet sous ce point.
        }

        public void CopyClipboard(string value)
        {
            try
            {
                Clipboard.SetText(value);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Impossible d'utiliser le presse-papiers !\n\n" + ex.Message, "Presse-papiers", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }


        private void ButtonORDERID_Click(object sender, RoutedEventArgs e)
        {
            CopyClipboard(Core.Global.MailOrderId);
        }

        private void ButtonORDERDATE_Click(object sender, RoutedEventArgs e)
        {
            CopyClipboard(Core.Global.MailOrderDate);
        }

        private void ButtonORDERFIRSTNAME_Click(object sender, RoutedEventArgs e)
        {
            CopyClipboard(Core.Global.MailOrderFirstName);
        }

        private void ButtonORDERLASTNAME_Click(object sender, RoutedEventArgs e)
        {
            CopyClipboard(Core.Global.MailOrderLastName);
        }

        private void ButtonORDERADDRESS_Click(object sender, RoutedEventArgs e)
        {
            CopyClipboard(Core.Global.MailOrderAddress);
        }

        private void ButtonORDERADDRESS1_Click(object sender, RoutedEventArgs e)
        {
            CopyClipboard(Core.Global.MailOrderAddress1);
        }

        private void ButtonORDERADDRESS2_Click(object sender, RoutedEventArgs e)
        {
            CopyClipboard(Core.Global.MailOrderAddress2);
        }

        private void ButtonORDERPOSTCODE_Click(object sender, RoutedEventArgs e)
        {
            CopyClipboard(Core.Global.MailOrderPostCode);
        }

        private void ButtonORDERCITY_Click(object sender, RoutedEventArgs e)
        {
            CopyClipboard(Core.Global.MailOrderCity);
        }

        private void ButtonORDERCOUNTRY_Click(object sender, RoutedEventArgs e)
        {
            CopyClipboard(Core.Global.MailOrderCountry);
        }

        private void ButtonORDERPHONE_Click(object sender, RoutedEventArgs e)
        {
            CopyClipboard(Core.Global.MailOrderPhone);
        }

        private void ButtonORDERMOBILE_Click(object sender, RoutedEventArgs e)
        {
            CopyClipboard(Core.Global.MailOrderMobile);
        }

        private void ButtonORDERTOTALPAIDTTC_Click(object sender, RoutedEventArgs e)
        {
            CopyClipboard(Core.Global.MailOrderTotalPaidTTC);
        }

        private void ButtonORDERTOTALPAIDHT_Click(object sender, RoutedEventArgs e)
        {
            CopyClipboard(Core.Global.MailOrderTotalPaidHT);
        }

        private void ButtonCARTLINK_Click(object sender, RoutedEventArgs e)
        {
            CopyClipboard(Core.Global.MailOrderCartLink);
        }

        private void ButtonTRACKINGLINK_Click(object sender, RoutedEventArgs e)
        {
            CopyClipboard(Core.Global.MailOrderTrackingLink);
        }
    }
}