using Newtonsoft.Json;
using ShopUrban.Model;
using ShopUrban.Util;
using ShopUrban.View.Receipt;
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

namespace ShopUrban.View
{
    /// <summary>
    /// Interaction logic for OrderHistory.xaml
    /// </summary>
    public partial class OrderHistory : UserControl
    {
        public List<Order> orders
        {
            get { return (List<Order>)GetValue(ordersProperty); }
            set { SetValue(ordersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for orders.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ordersProperty =
            DependencyProperty.Register("orders", typeof(List<Order>), 
                typeof(OrderHistory), new PropertyMetadata(null));
        //public List<Order> orders { get; set; }

        public OrderHistory()
        {
            InitializeComponent();

            DataContext = this;
        }

        public void refresh()
        {
            orders = Order.all(0, "DESC");
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedItem == null || !(dataGrid.SelectedItem is Order))
                return;
            
            var order = (Order)dataGrid.SelectedItem;
            //Helpers.log(JsonConvert.SerializeObject(order));
            //Helpers.log(JsonConvert.SerializeObject(order.orderPayments));

            //new Thermal88mm(order).ShowDialog();
            Helpers.showReceipt(order);
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dataGrid.SelectedItem == null || !(dataGrid.SelectedItem is Order))
                return;

            var order = (Order)dataGrid.SelectedItem;

            //new Thermal88mm(order).ShowDialog();
            Helpers.showReceipt(order);
        }
    }
}
