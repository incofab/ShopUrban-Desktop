using ShopUrban.Model;
using ShopUrban.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace ShopUrban.View.UserControls.Cart
{
    /// <summary>
    /// Interaction logic for CartItemUserCtrl.xaml
    /// </summary>
    public partial class CartItemUserCtrl : UserControl
    {
        public static readonly DependencyProperty cartItemProperty =
            DependencyProperty.Register("cartItem", typeof(CartItem),
                typeof(CartItemUserCtrl), new UIPropertyMetadata(null));

        public CartItem cartItem
        {
            get { return (CartItem)GetValue(cartItemProperty); }
            set { SetValue(cartItemProperty, value); }
        }

        //public CartItem cartItem { get; set; }
        public CartSectionCtrl cartSectionCtrl { get; set; }

        public CartItemUserCtrl(CartSectionCtrl cartSectionCtrl, CartItem cartItem)
        {
            DataContext = this;

            this.cartSectionCtrl = cartSectionCtrl;
            this.cartItem = cartItem;

            //cartItem.content = "9jesiedsijdijsd";

            InitializeComponent();

            //if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            //{
            //    this.Background = Brushes.Transparent;
            //}
        }
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            cartSectionCtrl.removeCartItem(this);
        }
        public void increaseQuantity()
        {
            var newValue = cartItem.quantity + 1;
            if(cartItem.shopProduct.stock_count < newValue)
            {
                Helpers.playErrorSound();
                return;
            }
            cartItem.quantity = newValue;
            cartSectionCtrl.updateCartTotalPrice();
        }
        private void btnDecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            var newValue = cartItem.quantity - 1;
            if(cartItem.shopProduct.stock_count < newValue)
            {
                Helpers.playErrorSound();
                return;
            }
            if(newValue == 0)
            {
                cartSectionCtrl.removeCartItem(this);
                return;
            }
            cartItem.quantity = newValue;
            cartSectionCtrl.updateCartTotalPrice();
        }

        private void btnIncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            increaseQuantity();
        }

        private void tbPrice_KeyUp(object sender, KeyEventArgs e)
        {
            cartSectionCtrl.updateCartTotalPrice();
        }

        private void tbQuantity_KeyUp(object sender, KeyEventArgs e)
        {
            int.TryParse(tbQuantity.Text.ToString(), out int quantity);

            if (cartItem.shopProduct.stock_count >= quantity) return;

            Helpers.playErrorSound();

            tbQuantity.Text = cartItem.shopProduct.stock_count + "";
        }

        private void cartItemName_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            cartItemEditContainer.Visibility = (cartItemEditContainer.Visibility == Visibility.Collapsed)
                ? Visibility.Visible : Visibility.Collapsed;
        }

        private void cartItemEditContainer_LostFocus(object sender, RoutedEventArgs e)
        {
            cartItemEditContainer.Visibility = Visibility.Collapsed;
        }

        private void tbCartItemContent_KeyUp(object sender, KeyEventArgs e)
        {
            //Helpers.log(tbCartItemContent.Text);
            //Helpers.log("cartItem.content = " + cartItem.content);
        }
    }
}
