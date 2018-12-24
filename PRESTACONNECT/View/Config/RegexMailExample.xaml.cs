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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace PRESTACONNECT.View.Config
{
    /// <summary>
    /// Logique d'interaction pour RegexMailExample.xaml
    /// </summary>
    public partial class RegexMailExample : UserControl, INotifyPropertyChanged
    {
        public readonly static DependencyProperty ListRegexMailProperty = DependencyProperty.Register("ListRegexMail", typeof(ObservableCollection<Model.Internal.RegexMail>), typeof(RegexMailExample), new UIPropertyMetadata(new ObservableCollection<Model.Internal.RegexMail>()));
        public readonly static DependencyProperty SelectedRegexMailProperty = DependencyProperty.Register("SelectedRegexMail", typeof(Model.Internal.RegexMail), typeof(RegexMailExample), new UIPropertyMetadata(new Model.Internal.RegexMail(Core.Parametres.RegexMail.lvl00_ld)));

        public RegexMailExample()
        {
            InitializeComponent();

            TestMail();
        }

        public ObservableCollection<Model.Internal.RegexMail> ListRegexMail
        {
            get { return (ObservableCollection<Model.Internal.RegexMail>)GetValue(ListRegexMailProperty); }
            set
            {
                SetValue(ListRegexMailProperty, value);
                OnPropertyChanged("ListRegexMail");
            }
        }

        public Model.Internal.RegexMail SelectedRegexMail
        {
            get { return (Model.Internal.RegexMail)GetValue(SelectedRegexMailProperty); }
            set
            {
                SetValue(SelectedRegexMailProperty, value);
                OnPropertyChanged("SelectedRegexMail");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TestMail();
        }

        public void TestMail()
        {
            test(this.textBox1, image1, image2);
            test(this.textBox2, image3, image4);
            test(this.textBox3, image5, image6);
            test(this.textBox4, image7, image8);
            test(this.textBox5, image9, image10);
            test(this.textBox6, image11, image12);
            test(this.textBox7, image13, image14);
            test(this.textBox8, image15, image16);
            test(this.textBox9, image17, image18);
            test(this.textBox10, image19, image20);
            test(this.textBox11, image21, image22);
            test(this.textBox12, image23, image24);
            test(this.textBox13, image25, image26);
            test(this.textBox14, image27, image28);
            test(this.textBox15, image29, image30);
            test(this.textBox16, image31, image32);
            test(this.textBox17, image33, image34);
        }
        public void test(TextBox t, Image no, Image ok)
        {
            if (no != null && ok != null)
                if (IsMailAddress(t.Text))
                {
                    no.Visibility = System.Windows.Visibility.Hidden;
                    ok.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    no.Visibility = System.Windows.Visibility.Visible;
                    ok.Visibility = System.Windows.Visibility.Hidden;
                }
        }

        public bool IsMailAddress(string Value)
        {
            bool flag = false;

            if (!string.IsNullOrWhiteSpace(Value))
            {
                try
                {
                    flag = Regex.IsMatch(Value, this.SelectedRegexMail.RegexExpression);
                }
                catch {};
            }
            return flag;
        }

        private void ListBoxRegexMail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TestMail();
        }
    }
}
