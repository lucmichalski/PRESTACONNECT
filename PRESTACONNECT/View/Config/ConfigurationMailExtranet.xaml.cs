using System.Windows;

namespace PRESTACONNECT
{
	/// <summary>
    /// Logique d'interaction pour ConfigurationMailExtranet.xaml
	/// </summary>
	public partial class ConfigurationMailExtranet : Window
	{
        public ConfigurationMailExtranet()
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

        private void ButtonCUSTOMERFIRSTNAME_Click(object sender, RoutedEventArgs e)
        {
            CopyClipboard(Core.Global.MailAccountFirstName);
        }
        private void ButtonCUSTOMERLASTNAME_Click(object sender, RoutedEventArgs e)
        {
            CopyClipboard(Core.Global.MailAccountLastName);
        }
        private void ButtonCUSTOMERCOMPANY_Click(object sender, RoutedEventArgs e)
        {
            CopyClipboard(Core.Global.MailAccountCompany);
        }

        private void ButtonINVOICENUMBERS_Click(object sender, RoutedEventArgs e)
        {
            CopyClipboard(Core.Global.MailInvoiceNumbers);
        }

	}
}