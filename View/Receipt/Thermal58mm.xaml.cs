using ShopUrban.Model;
using ShopUrban.Util;
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

namespace ShopUrban.View.Receipt
{
    /// <summary>
    /// Interaction logic for Thermal88mm.xaml
    /// </summary>
    public partial class Thermal58mm : Window
    {
        public string dateTime { get; }
        public Staff staff { get; }
        public Order order { get; }
        public Cart cart { get; }
        public Shop shop { get; }
        public List<CartItem> cartItems { get; }

        public Thermal58mm(Order order, bool autoPrint)
        {
            InitializeComponent();

            this.order = order;
            this.cart = order.cart;
            this.cartItems = cart.cartItems;

            this.shop = Setting.getShopDetail();
            this.staff = Staff.findById(order.staff_id) ?? Helpers.getLoggedInStaff();

            dateTime = DateTime.Now.ToString("dddd , MMM dd yyyy, HH:mm:ss");

            foreach (var cartItem in this.cartItems)
            {
                var shopProducts = ShopProduct.all(null, null, cartItem.shop_product_id);
                
                if (cartItem.shopProduct != null) continue;

                cartItem.shopProduct = shopProducts.Count > 0 ? shopProducts[0] : null;
            }

            DataContext = this;

            tbBottomNote.Text = Setting.getValue(Setting.KEY_RECEIPT_BOTTOM_NOTE)?? "Thanks for your Patronage";

            var customerName = order.user?.name?.Trim();
            if (!string.IsNullOrEmpty(customerName) || !string.IsNullOrEmpty(order.customer_name))
            {
                boxCustomer.Visibility = Visibility.Visible;
                tbCustomer.Text = !string.IsNullOrEmpty(customerName) ? customerName : order.customer_name;
            }
            else boxCustomer.Visibility = Visibility.Collapsed;

            if (order.shipping_cost < 1) boxDeliveryFee.Visibility = Visibility.Collapsed;
            if (order.vat_amount < 1) boxVAT.Visibility = Visibility.Collapsed;

            if(autoPrint) print();
        }

        private void print()
        {
            btnPrint.Visibility = Visibility.Collapsed;

            PrintDialog p = new PrintDialog();

            //if (p.ShowDialog() == true)
            //{
                p.PrintVisual(printArea, $"Order Receipt - {shop.name} - {dateTime}");
            //}

            btnPrint.Visibility = Visibility.Visible;
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            print();
        }
    }
}
