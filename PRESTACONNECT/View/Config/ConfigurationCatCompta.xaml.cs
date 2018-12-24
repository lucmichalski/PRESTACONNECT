using System;
using System.Windows;
using PRESTACONNECT.Contexts;

namespace PRESTACONNECT.View.Config
{
    /// <summary>
    /// Logique d'interaction pour ConfigurationCatCompta.xaml
    /// </summary>
    public partial class ConfigurationCatCompta : Window
    {
        internal new ConfigurationCatComptaContext DataContext
        {
            get { return (ConfigurationCatComptaContext)base.DataContext; }
            private set
            {
                base.DataContext = value;
            }
        }

        public ConfigurationCatCompta()
        {
            InitializeComponent();
            DataContext = new ConfigurationCatComptaContext();
        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            DataContext.SaveCatCompta();
        }

        private void ButtonUnselectCatComptaPro_Click(object sender, RoutedEventArgs e)
        {
            DataContext.UnselectCatComptaPro();
        }

        private void ButtonUnselectCatCompta_Click(object sender, RoutedEventArgs e)
        {
            DataContext.UnselectCatCompta();
        }
    }
}
