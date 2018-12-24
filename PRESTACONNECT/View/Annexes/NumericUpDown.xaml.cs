using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Reflection;
using System.ComponentModel;

namespace PRESTACONNECT.View
{
    /// <summary>
    /// Logique d'interaction pour NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl, INotifyPropertyChanged
    {
        public readonly static DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(int), typeof(NumericUpDown), new UIPropertyMetadata(100));
        public readonly static DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(int), typeof(NumericUpDown), new UIPropertyMetadata(0));
        public readonly static DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(NumericUpDown), new UIPropertyMetadata(0));
        //public readonly static DependencyProperty ValueStringProperty = DependencyProperty.Register("ValueString", typeof(string), typeof(NumericUpDown), new UIPropertyMetadata(""));
        public readonly static DependencyProperty StartValueProperty = DependencyProperty.Register("StartValue", typeof(int), typeof(NumericUpDown), new UIPropertyMetadata(0));

        //int minvalue = 0,
        //    maxvalue = 100,
        //    startvalue = 10;
        //static NumericUpDown()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
        //    MaximumProperty = DependencyProperty.Register("Maximum", typeof(int), typeof(NumericUpDown), new UIPropertyMetadata(100));
        //    MinimumProperty = DependencyProperty.Register("Minimum", typeof(int), typeof(NumericUpDown), new UIPropertyMetadata(0));
        //    ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(NumericUpDown), new UIPropertyMetadata(0));
        //    StartValueProperty = DependencyProperty.Register("StartValue", typeof(int), typeof(NumericUpDown), new UIPropertyMetadata(0));
        //}

        public NumericUpDown()
        {
            InitializeComponent();
            this.LayoutRoot.DataContext = this;
        }

        private void NUDButtonUP_Click(object sender, RoutedEventArgs e)
        {
            Value = (Value < Maximum) ? Value + 1 : Maximum;
        }

        private void NUDButtonDown_Click(object sender, RoutedEventArgs e)
        {
            Value = (Value > Minimum) ? Value - 1 : Minimum;
        }

        private void NUDTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Up)
            {
                NUDButtonUP.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(NUDButtonUP, new object[] { true });
            }


            if (e.Key == Key.Down)
            {
                NUDButtonDown.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(NUDButtonDown, new object[] { true });
            }
        }

        private void NUDTextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
                typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(NUDButtonUP, new object[] { false });

            if (e.Key == Key.Down)
                typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(NUDButtonDown, new object[] { false });
        }
        
        //public string ValueString
        //{
        //    //get { return Value.ToString(); }
        //    get { return (string)GetValue(MaximumProperty); }
        //    set
        //    {
        //        SetValue(ValueStringProperty, value);
        //        OnPropertyChanged("ValueString");
        //    }
        //}

        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set
            {
                SetValue(MaximumProperty, value);
                if (value < Value)
                    Value = value;
                OnPropertyChanged("Maximum");
            }
        }
        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set
            {
                SetValue(MinimumProperty, value);
                if (value > Value)
                    Value = value;
                OnPropertyChanged("Minimum");
            }
        }
        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set
            {
                if (value < Minimum)
                    value = Minimum;
                if (value > Maximum)
                    value = Maximum;
                SetValue(ValueProperty, value);
                OnPropertyChanged("Value");
                //ValueString = Value.ToString();
                //OnPropertyChanged("ValueString");
            }
        }
        public int StartValue
        {
            get { return (int)GetValue(StartValueProperty); }
            set
            {
                SetValue(StartValueProperty, value);
                if (value < Minimum)
                    StartValue = Minimum;
                if (value > Maximum)
                    StartValue = Maximum;
                OnPropertyChanged("StartValue");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void NUDTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int number = Value;
            if (!string.IsNullOrWhiteSpace(NUDTextBox.Text)
                    && int.TryParse(NUDTextBox.Text, out number))
            {
                if (number < Minimum)
                {
                    Value = Minimum;
                    NUDTextBox.SelectionStart = NUDTextBox.Text.Length;
                }
                else if (number > Maximum)
                {
                    Value = Maximum;
                    NUDTextBox.SelectionStart = NUDTextBox.Text.Length;
                }
                OnPropertyChanged("Value");
            }
            else
                Value = StartValue;
        }
    }
}
