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
using PRESTACONNECT.Contexts;

namespace PRESTACONNECT
{
	/// <summary>
	/// Logique d'interaction pour Gamme.xaml
	/// </summary>
	public partial class Gamme : Window
	{
        internal new GammeContext DataContext
        {
            get { return (GammeContext)base.DataContext; }
            private set
            {
                base.DataContext = value;
            }
        }

		public Gamme()
		{
			this.InitializeComponent();
            DataContext = new GammeContext();
            if (Core.Temp.Current != System.Windows.WindowState.Minimized)
                this.WindowState = Core.Temp.Current;
		}
        
        private void buttonCreatePsAttributeGroup_Click(object sender, RoutedEventArgs e)
        {
            DataContext.AddPsAttributeGroup();
        }

        private void ButtonAddNewAttribute_Click(object sender, RoutedEventArgs e)
        {
            var default_cursors = this.Cursor;
            this.Cursor = System.Windows.Input.Cursors.Wait;
            DataContext.AddPsAttribute();
            this.Cursor = default_cursors;
        }

        private void ButtonFilterPsAttributeLang_Click(object sender, RoutedEventArgs e)
        {
            DataContext.FilterAttributeValue();
        }
	}
}