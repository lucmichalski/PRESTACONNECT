using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PRESTACONNECT.Contexts;

namespace PRESTACONNECT
{
    public partial class LienConditionnement : Window
    {
        #region Properties

        internal new LienConditionnementContext DataContext
        {
            get { return (LienConditionnementContext)base.DataContext; }
            private set
            {
                base.DataContext = value;
            }
        }

        #endregion

        #region Constructors

        public LienConditionnement()
        {
            DataContext = new LienConditionnementContext();

            this.InitializeComponent();

            if (Core.Temp.Current != System.Windows.WindowState.Minimized)
                this.WindowState = Core.Temp.Current;
        }

        #endregion

        #region Button

        private void ButtonSubmit_Click(object sender, RoutedEventArgs e)
        {
            var default_cursors = this.Cursor;
            this.Cursor = System.Windows.Input.Cursors.Wait;

            DataContext.SaveReplace();

            this.Cursor = default_cursors;
        }

        #endregion

        private void buttonCheckByRef_Click(object sender, RoutedEventArgs e)
        {
            var default_cursors = this.Cursor;
            this.Cursor = System.Windows.Input.Cursors.Wait;

            if (DataContext.SelectedConditioningArticle != null)
            {
                DataContext.CheckByRef();
            }

            this.Cursor = default_cursors;
        }

        private void buttonAllCheck_Click(object sender, RoutedEventArgs e)
        {
            var default_cursors = this.Cursor;
            this.Cursor = System.Windows.Input.Cursors.Wait;

            DataContext.CheckAll();

            this.Cursor = default_cursors;
        }
    }
}