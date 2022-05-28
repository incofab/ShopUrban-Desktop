using ShopUrban.Util;
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

namespace ShopUrban.View.Dialogs
{
    /// <summary>
    /// Interaction logic for CustomDialog.xaml
    /// </summary>
    public partial class CustomDialog : Window
    {
        public const string SELECTION_POSITIVE = "positive";
        public const string SELECTION_NEGATIVE = "negative";
        public string selection = "";

        public CustomDialog(string message, string positiveButtonText, string negativeButtonText, string title = "Confirm")
        {
            InitializeComponent();

            tbMessage.Text = message;
            btnPositive.Content = positiveButtonText;
            btnNegative.Content = negativeButtonText;

            this.Title = title;
        }

        private void btnNegative_Click(object sender, RoutedEventArgs e)
        {
            selection = "negative";
            DialogResult = true;
        }

        private void btnPositive_Click(object sender, RoutedEventArgs e)
        {
            selection = "positive";
            DialogResult = true;
        }
    }
}
