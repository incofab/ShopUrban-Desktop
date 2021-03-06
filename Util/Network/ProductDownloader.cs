using Newtonsoft.Json;
using ShopUrban.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShopUrban.Util.Network
{
    [Obsolete("Downloads and updates have been moved to SynchronizeShopProduct class")]
    class ProductDownloader
    {
        public static string imagesFolder = KStrings.BASE_FOLDER + "products\\images\\";

        public ProductDownloader()
        {
            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }
        }

        private static ProductDownloader instance;
        public static ProductDownloader getInstance()
        {
            if (instance == null) instance = new ProductDownloader();

            return instance;
        }

        private Func<int, int> action;
        public async Task<List<Product>> loadProducts(string lastSync)
        {
            var shopId = Helpers.getCurrentShop()?.id + "";
            //string lastSync = Setting.getValue(Setting.KEY_PRODUCT_LAST_SYNC_SERVER);

            List<KeyValuePair<string, string>> l = new List<KeyValuePair<string, string>>();

            l.Add(new KeyValuePair<string, string>("last_sync", lastSync));
            l.Add(new KeyValuePair<string, string>("shop_id", shopId));

            BaseResponse baseResponse = await BaseResponse.callEndpoint(KStrings.URL_GET_PRODUCTS_API, l);

            if (baseResponse == null || baseResponse.status != "success")
            {
                Helpers.runOnUiThread(() => { 
                    MessageBox.Show((baseResponse != null)
                        ? baseResponse.message : $"Error connecting to synchronizing products. Check your internet");

                    MyEventBus.post(new EventMessage(EventMessage.EVENT_PRODUCT_SYNC_FAILED, null));
                });

                return null;
            }

            List<Product> products =
                JsonConvert.DeserializeObject<List<Product>>(baseResponse.data.ToString());

            return products;
        }

        public string downloadProductImage(string imgUrl, Func<int, int> action = null)
        {
            if (imgUrl == null) return null;

            this.action = action;

            using (WebClient client = new WebClient())
            {
                //client.DownloadFile(remoteFilename, localFilename);
                if (action != null)
                {
                    client.DownloadProgressChanged += wc_DownloadProgressChanged;
                }
                
                Uri uri = new Uri(imgUrl);
                
                string filename = System.IO.Path.GetFileName(uri.LocalPath);

                var random = new Random().Next(10000, 99999);
                string filepath = imagesFolder + (KStrings.DEV ? $"{random}-{filename}" : filename);

                if (File.Exists(filepath))
                {
                    Helpers.log($"Image ({filename}) File already exists");
                    File.Delete(filepath);
                    filepath = imagesFolder + $"{new Random().Next(10000, 99999)}-{filename}";
                }


                //client.DownloadFileAsync(
                client.DownloadFile(
                    // Param1 = Link of file
                    uri,
                    // Param2 = Path to save
                    filepath
                );

                return filepath;
            }
            
        }

        // Event to track the progress
        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (action != null)
            {
                action(e.ProgressPercentage);
            }
        }

        public async Task<BaseResponse> uploadOrders(bool showMessageIfEmpty = true)
        {
            //int.TryParse(Setting.getValue(Setting.KEY_ORDER_LAST_SYNC_ID), out int orderLastId);

            //List<Order> orders = Order.all(orderLastId);
            List<Order> orders = Order.getUnUploadedOrders();

            string shopId = Setting.getShopId();

            if(orders == null || orders.Count < 1)
            {
                //Helpers.log($"No new item to upload");

                if (!showMessageIfEmpty) return new BaseResponse();

                MessageBox.Show($"No new item to upload");

                return new BaseResponse();
            }

            List<KeyValuePair<string, string>> l = new List<KeyValuePair<string, string>>();

            l.Add(new KeyValuePair<string, string>("orders", JsonConvert.SerializeObject(orders)));
            l.Add(new KeyValuePair<string, string>("shop_id", shopId));

            BaseResponse baseResponse = await BaseResponse.callEndpoint(KStrings.URL_UPLOAD_ORDERS_API, l);

            if (baseResponse == null || baseResponse.status != "success")
            {
                if (showMessageIfEmpty)
                {
                    MessageBox.Show((baseResponse != null)
                        ? baseResponse.message : $"Order Sync(): Error connecting to server");
                }

                return null;
            }

            Setting.createOrUpdate(new Setting()
            {
                key = Setting.KEY_ORDER_LAST_SYNC_ID,
                value = orders[orders.Count - 1].id + "",
            });

            var lastOrderSyncTime = DateTime.Now.ToString(KStrings.TIME_FORMAT);

            Setting.createOrUpdate(new Setting()
            {
                key = Setting.KEY_ORDER_LAST_SYNC_TIME,
                value = lastOrderSyncTime,
            });

            return baseResponse;
        }

    }
}
