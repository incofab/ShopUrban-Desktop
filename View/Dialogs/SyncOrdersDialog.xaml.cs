using Newtonsoft.Json;
using ShopUrban.Model;
using ShopUrban.Services;
using ShopUrban.Util;
using ShopUrban.Util.Network;
using System.Windows;

namespace ShopUrban.View.Dialogs
{
    /// <summary>
    /// Interaction logic for SyncOrdersDialog.xaml
    /// </summary>
    public partial class SyncOrdersDialog : Window
    {
        private string lastOrderSyncTime;

        public SyncOrdersDialog()
        {
            InitializeComponent();

            updateUI();
        }

        private void updateUI()
        {
            lastOrderSyncTime = Setting.getValue(Setting.KEY_ORDER_LAST_SYNC_TIME);

            tbLastSync.Text = "Last Sync Time: " + lastOrderSyncTime;
        }

        //public async Task<BaseResponse> uploadOrders()
        //{
        //    setLoading(true);
            
        //    int orderLastId = 0;
        //    int.TryParse(Setting.getValue(Setting.KEY_ORDER_LAST_SYNC_ID), out orderLastId);

        //    string shopId = Setting.getShopId();

        //    List<Order> orders = Order.all(orderLastId);

        //    List<KeyValuePair<string, string>> l = new List<KeyValuePair<string, string>>();

        //    l.Add(new KeyValuePair<string, string>("orders", JsonConvert.SerializeObject(orders)));
        //    l.Add(new KeyValuePair<string, string>("shop_id", shopId));

        //    BaseResponse baseResponse = await BaseResponse.callEndpoint(KStrings.URL_UPLOAD_ORDERS_API, l);

        //    if (baseResponse == null || baseResponse.status != "success")
        //    {
        //        MessageBox.Show((baseResponse != null)
        //            ? baseResponse.message : $"Order Sync(): Error connecting to server");

        //        return null;
        //    }

        //    if(orders != null)
        //    {
        //        Setting.createOrUpdate(new Setting()
        //        {
        //            key = Setting.KEY_ORDER_LAST_SYNC_ID,
        //            value = orders[orders.Count - 1].id + "",
        //        });

        //        lastOrderSyncTime = DateTime.Now.ToString("y-M-d H:m:s.s");
        //        Setting.createOrUpdate(new Setting()
        //        {
        //            key = Setting.KEY_ORDER_LAST_SYNC_TIME,
        //            value = lastOrderSyncTime,
        //        });
        //    }

        //    return baseResponse;
        //}

        private bool isLoading;
        private void setLoading(bool isLoading)
        {
            this.isLoading = isLoading;

            if (isLoading)
            {
                progressBar.Visibility = Visibility.Visible;
                btnSyncNow.Content = "Loading...";
            }
            else
            {
                progressBar.Visibility = Visibility.Collapsed;
                btnSyncNow.Content = "Upload Now";
            }
        }

        private void btnSyncNow_Click(object sender, RoutedEventArgs e)
        {
            handleUpload();
        }

        private void handleUpload()
        {
            if (isLoading) return;

            setLoading(true);

            OrderHelper.uploadPendingOrders((BaseResponse baseResponse) => {
                    
                    setLoading(false);
                    
                    if (baseResponse == null) return;

                    DialogResult = true;

                    Toast.showSuccess("Orders uploaded successfully");

                    updateUI();
                },
            false);

            //BaseResponse baseResponse = await ProductDownloader.getInstance().uploadOrders();
            //setLoading(false);
            //if (baseResponse == null) return;
            //DialogResult = true;
            //Toast.showSuccess("Orders uploaded successfully");
            //updateUI();
        }


    }
}
