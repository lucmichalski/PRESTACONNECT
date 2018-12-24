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
using System.Drawing;
using System.ComponentModel;

namespace PRESTACONNECT.View
{
    /// <summary>
    /// Logique d'interaction pour PrestaMessage.xaml
    /// </summary>
    public partial class PrestaMessage : Window
    {
        public double form_height = 0;

        public PrestaMessage(string text, string title, MessageBoxButton Buttons, MessageBoxImage Image)
        {
            this.InitializeComponent();
            this.SetMessage(text, title, Buttons, Image);
        }
        public PrestaMessage(string text, string title, MessageBoxButton Buttons, MessageBoxImage Image, int setWidth)
        {
            this.InitializeComponent();
            this.SetMessage(text, title, Buttons, Image);
            this.Width = setWidth;
        }
        public PrestaMessage(string text, string title, MessageBoxButton Buttons, MessageBoxImage Image, string errordetail)
        {
            this.InitializeComponent();
            this.SetMessage(text, title, Buttons, Image, errordetail);
        }
        public PrestaMessage(string text, string title, MessageBoxButton Buttons, MessageBoxImage Image, int setWidth, string errordetail)
        {
            this.InitializeComponent();
            this.SetMessage(text, title, Buttons, Image, errordetail);
            this.Width = setWidth;
        }

        private void SetMessage(string text, string title, MessageBoxButton Buttons, MessageBoxImage Image)
        {
            this.SetMessage(text, title, Buttons, Image, string.Empty);
        }
        private void SetMessage(string text, string title, MessageBoxButton Buttons, MessageBoxImage Image, string errordetail)
        {
            this.Title = title;
            this.TextBlockContent.Text = text;
            switch (Buttons)
            {
                case MessageBoxButton.YesNo:
                case MessageBoxButton.YesNoCancel:
                    this.GridYesNo.Visibility = System.Windows.Visibility.Visible;
                    this.GridOkCancel.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case MessageBoxButton.OK:
                    this.GridOkCancel.Visibility = System.Windows.Visibility.Visible;
                    this.buttonCancel.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case MessageBoxButton.OKCancel:
                default:
                    this.GridOkCancel.Visibility = System.Windows.Visibility.Visible;
                    break;
            }
            switch (Image)
            {
                case MessageBoxImage.Question:
                    MessageIcon.Source = new BitmapImage(new Uri("/PRESTACONNECT;component/Resources/question.png", UriKind.RelativeOrAbsolute));
                    break;
                case MessageBoxImage.Warning:
                    MessageIcon.Source = new BitmapImage(new Uri("/PRESTACONNECT;component/Resources/alert.png", UriKind.RelativeOrAbsolute));
                    break;
                case MessageBoxImage.Error:
                    MessageIcon.Source = new BitmapImage(new Uri("/PRESTACONNECT;component/Resources/error.png", UriKind.RelativeOrAbsolute));
                    break;
                default:
                case MessageBoxImage.Information:
                    MessageIcon.Source = new BitmapImage(new Uri("/PRESTACONNECT;component/Resources/info.png", UriKind.RelativeOrAbsolute));
                    break;
                case MessageBoxImage.None:
                    break;
            }

            if (!string.IsNullOrWhiteSpace(errordetail))
            {
                this.expanderError.Visibility = System.Windows.Visibility.Visible;
                this.TextBlockErrorDetail.Text = errordetail;
                this.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
                this.MinHeight = 220;
            }
            else
            {
                this.expanderError.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void buttonYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void buttonNo_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void expanderError_Expanded(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                if (this.expanderError.IsExpanded)
                {
                    this.Height += (form_height == 0) ? 100 : form_height;
                }
                else
                {
                    this.form_height = this.Height - 220;
                    this.Height = 220;
                }
            }
        }

        private void MenuItemCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(this.TextBlockErrorDetail.Text);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Impossible d'utiliser le presse-papiers !\n\n" + ex.Message, "Presse-papiers", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}