using Newtonsoft.Json;
using ShopUrban.Model;
using ShopUrban.Util;
using ShopUrban.Util.Network;
using ShopUrban.View.Dialogs;
using ShopUrban.View.Receipt;
using ShopUrban.View.UserControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private ProductFilterCtrl productFilterCtrl;

        public OrderHistory()
        {
            InitializeComponent();

            DataContext = this;

            this.Loaded += UserControl_Loaded;

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.Background = Brushes.Transparent;
            }

            productFilterCtrl = new ProductFilterCtrl(this);

            borderProductFilterCtrl.Child = productFilterCtrl;

            MyEventBus.subscribe(handleEvent);

            TimerHelper.SetTimeout(10000, () => {
                Helpers.runOnUiThread(() =>
                {
                    productFilterCtrl.filterNow(new List<KeyValuePair<string, string>>());
                });
            });
        }

        void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window.Closing += window_Closing;
        }

        void window_Closing(object sender, global::System.ComponentModel.CancelEventArgs e)
        {
            //do something before the window is closed...
            MyEventBus.unSubscribe(handleEvent);
        }

        public void refresh(List<Order> orders = null)
        {
            this.orders = (orders == null) ? Order.all(0, "DESC") : orders;
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (dataGrid.SelectedItem == null || !(dataGrid.SelectedItem is Order))
            //    return;
            
            //var order = (Order)dataGrid.SelectedItem;

            //Helpers.showReceipt(order);
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if (dataGrid.SelectedItem == null || !(dataGrid.SelectedItem is Order))
            //    return;

            //var order = (Order)dataGrid.SelectedItem;

            //Helpers.showReceipt(order);
        }


        private void handleEvent(EventMessage eventMessage)
        {
            switch (eventMessage.eventId)
            {
                case EventMessage.EVENT_ORDER_INDEX:
                    
                    refresh((List<Order>)eventMessage.data);

                    break;
            }
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button.Name.Equals("btnNext"))
            {
                productFilterCtrl.gotoPaginateUrl("next");
            }
            else
            {
                productFilterCtrl.gotoPaginateUrl("prev");
            }
        }

        private async Task<Order> getOrderDetails(Order order)
        {
            setOrderDetailLoading(true);

            List<KeyValuePair<string, string>> l = new List<KeyValuePair<string, string>>();

            var url = KStrings.URL_SHOW_ORDER + "/" + order.order_number;

            BaseResponse baseResponse = await BaseResponse.callEndpoint(url, l, "GET");

            setOrderDetailLoading(false);

            if (baseResponse == null || baseResponse.status != "success")
            {
                MessageBox.Show((baseResponse != null)
                    ? baseResponse.message : $"Update(): Error connecting to server");
                
                return null;
            }

            if (baseResponse.data == null) return null;

            return JsonConvert.DeserializeObject<Order>(baseResponse.data.ToString());
        }

        private bool isOrderDetailLoading;
        private void setOrderDetailLoading(bool isLoading)
        {
            isOrderDetailLoading = isLoading;

            progressBar.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (isOrderDetailLoading) return;

            Order order = ((FrameworkElement)sender).DataContext as Order;

            if(order.cart != null && order.cart.cartItems != null)
            {
                Helpers.showReceipt(order, true);

                return;
            }

            var orderDetail = await getOrderDetails(order);

            if (orderDetail == null)
            {
                MessageBox.Show("Order record not found. Please sync your orders");
                return;
            }

            Helpers.showReceipt(orderDetail, true);
        }

        private async void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            if (isOrderDetailLoading) return;

            Order order = ((FrameworkElement)sender).DataContext as Order;

            var orderDetail = await getOrderDetails(order);

            if (orderDetail == null)
            {
                MessageBox.Show("Order record not found. Please sync your orders");
                return;
            }

            new ViewOrder(orderDetail).ShowDialog();
        }


    }
}
