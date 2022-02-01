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
    public partial class A4Receipt : Window
    {
        public string dateTime { get; }
        public Staff staff { get; }
        public Order order { get; }
        public Cart cart { get; }
        public Shop shop { get; }
        public List<CartItem> cartItems { get; }

        public A4Receipt(Order order)
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
        }

        private void print()
        {
            btnPrint.Visibility = Visibility.Collapsed;

            PrintDialog p = new PrintDialog();
            p.PrintVisual(this, $"Order Receipt - {shop.name} - {dateTime}");

            //TimerHelper.SetTimeout(1000, () => {
            //    btnPrint.Visibility = Visibility.Visible;
            //});
            btnPrint.Visibility = Visibility.Visible;
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            print();
        }
    }
}
