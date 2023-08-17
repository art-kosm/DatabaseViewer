using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DatabaseViewer
{
    /// <summary>
    /// Interaction logic for SaveAsTemplateDialog.xaml
    /// </summary>
    public partial class SaveAsTemplateDialog : Window
    {
        public SaveAsTemplateDialog()
        {
            InitializeComponent();
        }

        public string GetTemplateName()
        {
            return templateNameEdit.Text;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MinHeight = Height;
            MaxHeight = Height;
        }

        private void Accept(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Reject(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
