using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.ComponentModel;

namespace PRESTACONNECT
{
    /// <summary>
    /// Logique d'interaction pour TinyMce.xaml
    /// </summary>
    public partial class TinyMce : UserControl, INotifyPropertyChanged
    {
        public TinyMce()
        {
            InitializeComponent();
        }

        //public static readonly DependencyProperty MassProperty = DependencyProperty.Register("HTML", typeof(String), typeof(TinyMce));
        //public static readonly DependencyProperty HtmlProperty = DependencyProperty.Register("HtmlContent", typeof(String), typeof(TinyMce));

        //public String HTML
        //{
        //    get
        //    {
        //        return "toto";//(String)GetValue(MassProperty);
        //    }
        //    set
        //    {
        //        //SetValue(MassProperty, value);
        //    }
        //}


        public string HtmlContent
        {
            get
            {
                string content = string.Empty;
                if (webBrowserControl.Document != null)
                {
                    object html = webBrowserControl.InvokeScript("GetContent");
                    content = html as string;
                }
                return CleanUpContent(content);
            }
            set
            {
                if (webBrowserControl.Document != null)
                {
                    webBrowserControl.InvokeScript("SetContent", new object[] { SetUpContent(value) });
                    //webBrowserControl.Refresh();
                }
                OnPropertyChanged("HtmlContent");
            }
        }

        public string HtmlContentNoP
        {
            get
            {
                string r = HtmlContent;
                if (r.StartsWith("<p>"))
                    r = r.Remove(0, 3);
                if (r.EndsWith("</p>"))
                    r = r.Remove(r.Length - 4);
                return r;
            }
        }

        public string CleanUpContent(string Content)
        {
            Content = Content.Replace("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"> <html> <head> <title>Untitled document</title> </head> <body>\n", "");
            Content = Content.Replace("\n</body> </html>", "");
            Content = Content.Replace("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n<html>\n<head>\n<title>Untitled document</title>\n</head>\n<body>\n", "");
            Content = Content.Replace("\n</body>\n</html>", "");
            Content = Content.Replace("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"> <html> <head> <title>Untitled document</title> </head> <body>", "");
            Content = Content.Replace("</body> </html>", "");
            return Content;
        }

        public string SetUpContent(string Content)
        {
            String Return = "";
            Return = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"> <html> <head> <title>Untitled document</title> </head> <body>";
            Return += Content;
            Return += " </body> </html>";
            return Return;
        }

        public void CreateEditor()
        {
            if (File.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"tinymce\jscripts\tiny_mce\tiny_mce.js")))
            {
                webBrowserControl.Source = new Uri(@"file:///" + System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tinymce.htm").Replace('\\', '/'));
            }
            else
            {
                MessageBox.Show("Could not find the tinyMCE script directory. Please ensure the directory is in the same location as tinymce.htm", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
