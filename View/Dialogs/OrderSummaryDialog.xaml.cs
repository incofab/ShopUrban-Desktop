using ShopUrban.Model;
using ShopUrban.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for OrderSummaryDialog.xaml
    /// </summary>
    public partial class OrderSummaryDialog : Window
    {


        public string paymentType
        {
            get { return (string)GetValue(paymentTypeProperty); }
            set { SetValue(paymentTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for paymentType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty paymentTypeProperty =
            DependencyProperty.Register("paymentType", typeof(string), typeof(OrderSummaryDialog), new PropertyMetadata(""));

        public List<CartItem> cartItems { get; set; }

        //public string paymentType { get; set; }
        public double cartTotalAmount { get; set; }
        public int cartTotalQuantity { get; set; }
        public double totalCostPrice { get; set; }
        public double grossTotalAmount { get; set; }
        public double shippingCost { get; set; }
        public double VATamount { get; set; }

        public OrderSummaryDialog(List<CartItem> cartItems)
        {
            this.cartItems = cartItems;
            InitializeComponent();

            paymentType = "Cash";

            DataContext = this;

            processItems();
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            double amountPaid = grossTotalAmount;
            double.TryParse(tboxAmountPaid.Text, out amountPaid);

            var staff = Helpers.getLoggedInStaff();
            // Create the Order
            Order order = new Order()
            {
                amount = cartTotalAmount,
                amount_to_pay = grossTotalAmount,
                amount_paid = amountPaid,
                remaining_amount = grossTotalAmount - amountPaid,
                total_cost_price = totalCostPrice,
                profit = cartTotalAmount - totalCostPrice,
                staff_id = staff.id,
                user_id = staff.user_id,
                vat_amount = VATamount,
            };

            Cart cart = new Cart {
                amount = cartTotalAmount,
                quantity = cartTotalQuantity,
                status = "1"
            };

            //Save records to DB
            Order savedOrder = Cart.saveRecords(cartItems, order, cart, paymentType);

            DialogResult = true;

            Helpers.showReceipt(savedOrder);
        }

        private void processItems()
        {
            cartTotalAmount = 0;
            cartTotalQuantity = 0;
            totalCostPrice = 0;

            foreach (var cartItem in cartItems)
            {
                cartTotalAmount += cartItem.totalPrice;
                cartTotalQuantity += cartItem.quantity;
                totalCostPrice += cartItem.cost_price * cartItem.quantity;
            }

            double vatPercent = getVATpercent();
            VATamount = (vatPercent / 100) * cartTotalAmount;
            grossTotalAmount = cartTotalAmount + shippingCost + VATamount;

            tbGrossTotalAmount.Text = Helpers.naira(grossTotalAmount);
            tbDeliveryFee.Text = Helpers.naira(shippingCost);

            tboxAmountPaid.Text = grossTotalAmount+"";
            tbVAT.Text = vatPercent + "%";
        }

        private double getVATpercent()
        {
            float.TryParse(Setting.getValue(Setting.KEY_VAT_PERCENT), out float vatPercent);

            return vatPercent;
        }

        private void btnCash_Click(object sender, RoutedEventArgs e)
        {
            paymentType = "Cash";
        }

        private void btnPOS_Click(object sender, RoutedEventArgs e)
        {
            paymentType = "POS";
        }

        private void btnBankTransfer_Click(object sender, RoutedEventArgs e)
        {
            paymentType = "Bank Transfer";
        }

        private void btnDraft_Click(object sender, RoutedEventArgs e)
        {
            List<CartItemDraft> cartItemDrafts = new List<CartItemDraft>();

            foreach (var cartItem in cartItems)
            {
                cartItemDrafts.Add(new CartItemDraft(cartItem));
            }

            CartDraft.saveRecords(cartItemDrafts);

            DialogResult = true;
        }

    }
}
