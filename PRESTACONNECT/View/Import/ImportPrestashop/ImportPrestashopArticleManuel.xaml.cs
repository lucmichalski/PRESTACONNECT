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

namespace PRESTACONNECT.View
{
    /// <summary>
    /// Logique d'interaction pour ImportPrestashopArticleManuel.xaml
    /// </summary>
    public partial class ImportPrestashopArticleManuel : Window
    {
        public ImportPrestashopArticleManuel()
        {
            InitializeComponent();
            Model.Prestashop.PsProductRepository PsProductRepository = new Model.Prestashop.PsProductRepository();
            List<Model.Prestashop.ProductResume> list = PsProductRepository.ListResume();
            Model.Local.ArticleRepository ArticleRepository = new Model.Local.ArticleRepository();
            List<int> listlocal = ArticleRepository.ListPrestashop();
            list = list.Where(p => !listlocal.Contains((int)p.id_product)).ToList();
            dataGridPsProduct.ItemsSource = list;
        }
    }
}
