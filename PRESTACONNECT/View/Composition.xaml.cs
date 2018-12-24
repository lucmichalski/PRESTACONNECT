using System.Windows;
using PRESTACONNECT.Contexts;

namespace PRESTACONNECT.View
{
    /// <summary>
    /// Logique d'interaction pour Composition.xaml
    /// </summary>
    public partial class Composition : Window
    {
        internal new CompositionContext DataContext
        {
            get { return (CompositionContext)base.DataContext; }
            private set
            {
                base.DataContext = value;
            }
        }

        public Composition()
        {
            InitializeComponent();
            DataContext = new CompositionContext();

            if (Core.Temp.Current != System.Windows.WindowState.Minimized)
                this.WindowState = Core.Temp.Current;
        }

        private void ButtonCreateComposition_Click(object sender, RoutedEventArgs e)
        {
            DataContext.CreateCompositionArticle();
        }

        private void buttonInitSearch_Click(object sender, RoutedEventArgs e)
        {
            DataContext.InitSearchArticleComposition();
        }

        private void ButtonSearchCompositionArticle_Click(object sender, RoutedEventArgs e)
        {
            var default_cursors = this.Cursor;
            this.Cursor = System.Windows.Input.Cursors.Wait;
            DataContext.SearchArticleComposition();
            this.Cursor = default_cursors;
        }
    }
}
