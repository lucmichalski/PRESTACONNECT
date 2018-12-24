using System;
using System.Windows;
using PRESTACONNECT.Contexts;
using System.Windows.Controls;

namespace PRESTACONNECT.View.Config
{
    /// <summary>
    /// Logique d'interaction pour ConfigurationMultiBonReduction.xaml
    /// </summary>
    public partial class ConfigurationMultiBonReduction : Window
    {
        internal new ConfigurationMultiBonReductionContext DataContext
        {
            get { return (ConfigurationMultiBonReductionContext)base.DataContext; }
            private set
            {
                base.DataContext = value;
            }
        }

        public ConfigurationMultiBonReduction()
        {
            InitializeComponent();
            DataContext = new ConfigurationMultiBonReductionContext();
        }

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            DataContext.SaveCatCompta();
        }

		private void ButtonUnselectArticle_Click(object sender, RoutedEventArgs e)
		{
			DataContext.UnselectArticle();
		}

		private void SageArticleList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (sender is ComboBox && ((ComboBox)sender).SelectedValue != null && ((ComboBox)sender).SelectedValue is Model.Sage.F_ARTICLE_Light)
				DataContext.SageArticleChange((Model.Sage.F_ARTICLE_Light)((ComboBox)sender).SelectedValue);
		}
	}
}
