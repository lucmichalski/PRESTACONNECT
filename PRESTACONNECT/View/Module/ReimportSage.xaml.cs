using System;
using System.Collections.Generic;
using System.Linq;
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

namespace PRESTACONNECT.View.Module
{
    /// <summary>
    /// Logique d'interaction pour ReimportSage.xaml
    /// </summary>
    public partial class ReimportSage : Window
    {
        #region Properties

        internal new ReimportSageContext DataContext
        {
            get { return (ReimportSageContext)base.DataContext; }
            private set
            {
                base.DataContext = value;
            }
        }

        #endregion

        #region Constructors

        public ReimportSage(List<int> items)
        {
            InitializeComponent();
            DataContext = new ReimportSageContext(items);
        }

        #endregion

        private void ButtonReimportSage_Click(object sender, RoutedEventArgs e)
        {
            DataContext.LaunchReimportSage();
        }
    }
}
