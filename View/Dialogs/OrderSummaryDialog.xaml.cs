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
        public decimal cartTotalAmount { get; set; }
        public int cartTotalQuantity { get; set; }
        public decimal totalCostPrice { get; set; }
        public decimal grossTotalAmount { get; set; }
        public decimal shippingCost { get; set; }
        public decimal VATamount { get; set; }

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
            string customerPhoneNo = tbCustomerPhoneNo.Text.Trim();

            if (!string.IsNullOrEmpty(customerPhoneNo) && !Helpers.IsPhoneNumberValid(customerPhoneNo))
            {
                MessageBox.Show("Enter a valid phone number");
                return;
            }

            string amountPaidStr = tboxAmountPaid.Text.Trim().Replace(",", "").Replace(" ", "");
            decimal amountPaid = Convert.ToDecimal(amountPaidStr);

            //Helpers.log($"amountPaid = {amountPaid}, grossTotalAmount = {grossTotalAmount}, " +
            //    $"Compare {amountPaid > grossTotalAmount}, Equals {amountPaid == grossTotalAmount}");

            if (amountPaid > grossTotalAmount)
            {
                MessageBox.Show("Payment amount exceeding the invoice total, check the amount you entered");
                return;
            }

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
                customer_name = tbCustomerName.Text,
                channel = "Desktop",
            };

            Cart cart = new Cart {
                amount = cartTotalAmount,
                quantity = cartTotalQuantity,
                status = "1",
                customer_name = tbCustomerName.Text,
                created_for = customerPhoneNo,
            };

            //Save records to DB
            Order savedOrder = Cart.saveRecords(cartItems, order, cart, paymentType);

            DialogResult = true;
            
            if(savedOrder != null)
            {
                if (!string.IsNullOrEmpty(savedOrder.customer_name) && savedOrder.user == null)
                {
                    savedOrder.user = new User{ firstname = savedOrder.customer_name, contact_no = customerPhoneNo };
                }
                Helpers.showReceipt(savedOrder, (bool)cbAutoPrint.IsChecked);
            }
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

            decimal vatPercent = getVATpercent();
            VATamount = (vatPercent / 100) * cartTotalAmount;
            grossTotalAmount = cartTotalAmount + shippingCost + VATamount;
            grossTotalAmount = decimal.Round(grossTotalAmount, 2);

            tbGrossTotalAmount.Text = Helpers.naira(grossTotalAmount);
            tbDeliveryFee.Text = Helpers.naira(shippingCost);

            tboxAmountPaid.Text = Helpers.numberFormat(grossTotalAmount);
            tbVAT.Text = vatPercent + "%";
        }

        private decimal getVATpercent()
        {
            decimal.TryParse(Setting.getValue(Setting.KEY_VAT_PERCENT), out decimal vatPercent);

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

        private void tbCustomerPhoneNo_KeyUp(object sender, KeyEventArgs e)
        {
            string query = (sender as TextBox).Text;
            
            if (query.Length == 0)
            {
                // Clear   
                optionsTbCustomerPhoneNo.Children.Clear();
                borderOptionsTbCustomerPhoneNo.Visibility = Visibility.Collapsed;
                return;
            }

            if (query.StartsWith("0")) query = query.Substring(1);

            if(query.Length < 2)
            {
                return;
            }
            
            //Getting here means we want to search and display results
            optionsTbCustomerPhoneNo.Children.Clear();

            List<ShopCustomer> shopCustomers = ShopCustomer.all(query);

            borderOptionsTbCustomerPhoneNo.Visibility = shopCustomers.Count < 1 ? Visibility.Collapsed : Visibility.Visible;

            foreach (var shopCustomer in shopCustomers)
            {
                addItem(shopCustomer, optionsTbCustomerPhoneNo);
            }
        }

        private void customerSelected(ShopCustomer shopCustomer)
        {
            tbCustomerName.Text = shopCustomer.name;
            tbCustomerPhoneNo.Text = shopCustomer.phone;
        }

        private void addItem(ShopCustomer shopCustomer, StackPanel stackPanel)
        {
            string text = shopCustomer.name + " | " + shopCustomer.phone;

            TextBlock block = new TextBlock();

            // Add the text   
            block.Text = text;

            // A little style...   
            block.Margin = new Thickness(2, 3, 2, 3);
            block.Cursor = Cursors.Hand;
            block.Padding = new Thickness(10, 5, 10, 5);

            // Mouse events   
            block.MouseLeftButtonUp += (sender, e) =>
            {
                this.customerSelected(shopCustomer);
            };

            block.MouseEnter += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.PeachPuff;
            };

            block.MouseLeave += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.Transparent;
            };

            // Add to the panel   
            stackPanel.Children.Add(block);
        }

        private void tbCustomerPhoneNo_LostFocus(object sender, RoutedEventArgs e)
        {
            TimerHelper.SetTimeout(500, () => {
                Helpers.runOnUiThread(() => { 
                    if(borderOptionsTbCustomerPhoneNo.Visibility == Visibility.Visible)
                    {
                        borderOptionsTbCustomerPhoneNo.Visibility = Visibility.Collapsed;
                    }
                });
            });

        }

        private void tboxAmountPaid_KeyUp(object sender, KeyEventArgs e)
        {
            string amountStr = tboxAmountPaid.Text.Trim().Replace(",", "").Replace(" ", "");
            if (amountStr.EndsWith(".")) return;

            decimal.TryParse(amountStr, out decimal amount);
            string amountFormatted = Helpers.numberFormat(amount);

            tboxAmountPaid.Text = amountFormatted;
            tboxAmountPaid.CaretIndex = amountFormatted.Length;
        }
    }
}
