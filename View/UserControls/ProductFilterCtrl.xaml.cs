using Newtonsoft.Json;
using ShopUrban.Model;
using ShopUrban.Util;
using ShopUrban.Util.Network;
using ShopUrban.Util.Network.Responses;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShopUrban.View.UserControls
{
    /// <summary>
    /// Interaction logic for ProductFilterCtrl.xaml
    /// </summary>
    public partial class ProductFilterCtrl : UserControl
    {
        private string dateFromStr = "";
        private string dateToStr = "";
        private readonly OrderHistory orderHistory;

        public ProductFilterCtrl(OrderHistory orderHistory)
        {
            InitializeComponent();
            this.orderHistory = orderHistory;
        }

        private string directDate = "";
        private void cb_Click(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;

            directDate = (cb.IsChecked == null || cb.IsChecked == false) ? "" : cb.Content.ToString();

            #region Clear Checkboxes
            cbYesterday.IsChecked = false;
            cbToday.IsChecked = false;
            cbThisYear.IsChecked = false;
            cbThisWeek.IsChecked = false;
            cbThisMonth.IsChecked = false;
            cbLastMonth.IsChecked = false;

            dateWrapper.Visibility = Visibility.Visible;
            #endregion

            if (string.IsNullOrEmpty(directDate)) return;

            cb.IsChecked = true;
            dateWrapper.Visibility = Visibility.Collapsed;
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            List<KeyValuePair<string, string>> l = parseSelectedFilters();

            if (l == null) return;

            filterNow(l);

        }
        private List<KeyValuePair<string, string>> parseSelectedFilters()
        {
            var search = tbSearch.Text.Trim();
            var status = ((ComboBoxItem)comboStatus.SelectedItem).Content.ToString();
            var channel = ((ComboBoxItem)comboChannel.SelectedItem).Content.ToString();

            if ("All".Equals(status)) status = "";
            if ("All".Equals(channel)) channel = "";

            List<KeyValuePair<string, string>> l = new List<KeyValuePair<string, string>>();

            if(!string.IsNullOrEmpty(status))
                l.Add(new KeyValuePair<string, string>("status", status));
            
            if(!string.IsNullOrEmpty(channel))
                l.Add(new KeyValuePair<string, string>("channel", channel));
            
            if(!string.IsNullOrEmpty(search))
                l.Add(new KeyValuePair<string, string>("search", search));

            if (!string.IsNullOrEmpty(directDate))
            {
                l.Add(new KeyValuePair<string, string>("direct_date", directDate));
            }
            else
            {
                if(!string.IsNullOrEmpty(dateFromStr) && !string.IsNullOrEmpty(dateToStr))
                {
                    var dfrom = DateTime.Parse(dateFromStr);
                    var dto = DateTime.Parse(dateToStr);
                    if(dfrom > dto)
                    {
                        MessageBox.Show("Start date cannot be bigger than end date");

                        return null;
                    }

                    l.Add(new KeyValuePair<string, string>("date_from", dfrom.ToString(KStrings.TIME_FORMAT)));
                    l.Add(new KeyValuePair<string, string>("date_to", dto.ToString(KStrings.TIME_FORMAT)));
                }
            }

            return l;
            //filterNow(l);
        }

        private PaginatedResponse paginatedResponse;
        public async void filterNow(List<KeyValuePair<string, string>> l)
        {
            setOrdersLoading(true);

            BaseResponse baseResponse = await BaseResponse.callEndpoint(
                KStrings.URL_SHOP_ORDERS_INDEX, l, "GET");

            setOrdersLoading(false);

            if (baseResponse == null || baseResponse.status != "success")
            {
                Toast.showError((baseResponse != null)
                    ? baseResponse.message : $"FilerNow: Error connecting to server");
                setOrdersLoading(false);
                return;
            }

            paginatedResponse = 
                JsonConvert.DeserializeObject<PaginatedResponse>(baseResponse.data.ToString());

            List<Order> orders = 
                JsonConvert.DeserializeObject<List<Order>>(paginatedResponse.data.ToString());

            MyEventBus.post(new EventMessage(EventMessage.EVENT_ORDER_INDEX, orders));

            orderHistory.btnNext.Visibility = string.IsNullOrEmpty(paginatedResponse.next_page_url)
                ? Visibility.Collapsed : Visibility.Visible;

            orderHistory.btnPrev.Visibility = string.IsNullOrEmpty(paginatedResponse.prev_page_url)
                ? Visibility.Collapsed : Visibility.Visible;
        }

        private bool isOrdersLoading;
        private void setOrdersLoading(bool isLoading)
        {
            isOrdersLoading = isLoading;

            btnFilter.Content = isLoading ? "Loading..." : "Filter Orders";
        }

        private void dateFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var picker = sender as DatePicker;

            DateTime? date = picker.SelectedDate;

            dateFromStr = (date == null) ? "" : date.Value.ToShortDateString();
        }

        private void dateTo_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var picker = sender as DatePicker;

            DateTime? date = picker.SelectedDate;

            dateToStr = (date == null) ? "" : date.Value.ToShortDateString();
        }

        public void gotoPaginateUrl(string direction)
        {
            if (paginatedResponse == null) return;

            string pageUrl = "next".Equals(direction) 
                ? paginatedResponse.next_page_url : paginatedResponse.prev_page_url;

            if (string.IsNullOrEmpty(pageUrl)) return;

            Uri uri = new Uri(pageUrl);
            string queryString = uri.Query;
            var queryDictionary = System.Web.HttpUtility.ParseQueryString(queryString);

            List<KeyValuePair<string, string>> l = parseSelectedFilters();

            if (l == null) return;

            l.Add(new KeyValuePair<string, string>("cursor", queryDictionary["cursor"]));

            filterNow(l);
        }

    }
}
