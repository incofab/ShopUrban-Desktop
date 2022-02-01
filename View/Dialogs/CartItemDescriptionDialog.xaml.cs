using ShopUrban.Model;
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

namespace ShopUrban.View.Dialogs
{
    /// <summary>
    /// Interaction logic for CartItemDescriptionDialog.xaml
    /// </summary>
    public partial class CartItemDescriptionDialog : Window
    {
        public CartItem cartItem { get; set; }

        public CartItemDescriptionDialog(CartItem cartItem)
        {
            InitializeComponent();
            this.cartItem = cartItem;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnOkay_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
