using Newtonsoft.Json;
using ShopUrban.Model;
using ShopUrban.Util;
using ShopUrban.Util.Network;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ShopUrban.View.Dialogs
{
    /// <summary>
    /// Interaction logic for ViewOrder.xaml
    /// </summary>
    public partial class ViewOrder : Window
    {
        public Order order { get; }

        public ViewOrder(Order order)
        {
            InitializeComponent();
            this.order = order;
            DataContext = this;

            string name = "";
            if (order.user != null) name = order.user.name;
            else if (order.staff != null) name = order.staff.name;

            tbStaff.Text = $"Staff: {name}";
            tbChannel.Text = $"Channel: {order.channel}";
            tbOrderNumber.Text = $"Order Number: {order.order_number}";
            tbStatus.Text = $"Status: {order.status}";

        }

        private string getPaymentType()
        {
            string paymentType = "";

            if (rdCash.IsChecked == true) paymentType = rdCash.Content.ToString();
            else if (rdPOS.IsChecked == true) paymentType = rdPOS.Content.ToString();
            else if (rdBankTransfer.IsChecked == true) paymentType = rdBankTransfer.Content.ToString();
            else paymentType = "cash";

            return paymentType.ToLower();
        }

        private async void addPaymentNow()
        {
            decimal.TryParse(tbPaymentAmount.Text.Trim(), out decimal amount);

            List<KeyValuePair<string, string>> l = new List<KeyValuePair<string, string>>();
            l.Add(new KeyValuePair<string, string>("amount_to_pay", amount+""));
            l.Add(new KeyValuePair<string, string>("payment_type", getPaymentType()));

            var url = KStrings.URL_ORDER_ADD_PAYMENT + "/" + order.order_number;
            
            setAddPaymentLoading(true);

            BaseResponse baseResponse = await BaseResponse.callEndpoint(url, l, "PUT");

            setAddPaymentLoading(false);

            if (baseResponse == null || baseResponse.status != "success")
            {
                MessageBox.Show((baseResponse != null)
                    ? baseResponse.message : $"Update(): Error connecting to server");

                return;
            }

            order.orderPayments.Add(new OrderPayment { 
                is_confirmed = 1,
                order_number_track = order.order_number,
                amount = amount,
                created_at = DateTime.Now.ToString(KStrings.TIME_FORMAT),
            });

            listPayments.ItemsSource = order.orderPayments;
            tbPaymentAmount.Text = "";

            Toast.showSuccess("Payment update recorded successfully, kindly refresh to see changes");
        }

        private bool isAddPaymentLoading;
        private void setAddPaymentLoading(bool isLoading)
        {
            isAddPaymentLoading = isLoading;

            btnAddPayment.Content = isLoading ? "Loading..." : "Add Payment";
        }

        private void btnAddPayment_Click(object sender, RoutedEventArgs e)
        {
            if (isAddPaymentLoading) return;

            addPaymentNow();
        }

    }
}
