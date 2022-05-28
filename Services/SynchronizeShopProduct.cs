using Newtonsoft.Json;
using ShopUrban.Model;
using ShopUrban.Util;
using ShopUrban.Util.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShopUrban.Services
{
    public class SynchronizeShopProduct
    {
        public static string imagesFolder = KStrings.BASE_FOLDER + "products\\images\\";

        private readonly Action<List<ShopProduct>> onProdutsDownloaded;
        private readonly ProgressChangedEventHandler onImageDownloadProgress;
        private readonly Action imageSyncComplete;

        public SynchronizeShopProduct(
            Action<List<ShopProduct>> onProdutsDownloaded = null,
            ProgressChangedEventHandler onImageDownloadProgress = null,
            Action imageSyncComplete = null
        ){
            this.onProdutsDownloaded = onProdutsDownloaded;
            this.onImageDownloadProgress = onImageDownloadProgress;
            this.imageSyncComplete = imageSyncComplete;

            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }
        }

        public void syncProductsAndImages()
        {
            var lastSync = Setting.getValue(Setting.KEY_PRODUCT_LAST_SYNC_SERVER);

            syncProducts(lastSync);
        }

        private void syncProducts(string lastSync)
        {
            var shopId = Helpers.getCurrentShop()?.id + "";
            List<KeyValuePair<string, string>> l = new List<KeyValuePair<string, string>>();
            l.Add(new KeyValuePair<string, string>("last_sync", lastSync));
            l.Add(new KeyValuePair<string, string>("shop_id", shopId));

            _ = BaseResponse.callEndpoint(KStrings.URL_GET_PRODUCTS_API_V2, l, "POST", (BaseResponse baseResponse) =>
            {
                if (baseResponse == null || baseResponse.status != "success")
                {
                    MessageBox.Show(baseResponse?.message ?? $"Error connecting to synchronizing products. Check your internet");

                    MyEventBus.post(new EventMessage(EventMessage.EVENT_PRODUCT_SYNC_FAILED, null));
                    return;
                }

                List<ShopProduct> sp = JsonConvert.DeserializeObject<List<ShopProduct>>(baseResponse.data.ToString());

                persistProductUpdate(sp);
                    
                if(onProdutsDownloaded != null) onProdutsDownloaded(sp);

                if (sp != null && sp.Count > 0) downloadImages();

                MyEventBus.post(new EventMessage(EventMessage.EVENT_PRODUCT_SYNC_COMPLETED, null));
            });
        }

        /// <summary>
        /// Save shopProducts and its relations to Database
        /// </summary>
        /// <param name="shopProducts"></param>
        /// <returns></returns>
        private List<Product> persistProductUpdate(List<ShopProduct> shopProducts)
        {
            var productLastSyncServer = Setting.getValue(Setting.KEY_PRODUCT_LAST_SYNC_SERVER);

            int totalCount = 0;
            Dictionary<int, Product> addedProducts = new Dictionary<int, Product>();
            Dictionary<int, ProductCategory> addedProductCategories = new Dictionary<int, ProductCategory>();

            foreach (var shopProduct in shopProducts)
            {
                totalCount++;

                ShopProduct.createOrUpdate(shopProduct);
                
                ProductUnit.createOrUpdate(shopProduct.productUnit);

                Product product = shopProduct.product;

                if (!addedProducts.ContainsKey(product.id))
                {
                    Product.createOrUpdate(product);
                    addedProducts.Add(product.id, product);
                }

                ProductCategory productCategory = product.productCategory;
                if(productCategory != null && !addedProductCategories.ContainsKey(productCategory.id))
                {
                    ProductCategory.createOrUpdate(productCategory);
                    addedProductCategories.Add(productCategory.id, productCategory);
                }
            }

            var productLastSync = DateTime.Now.ToString(KStrings.TIME_FORMAT);
            Setting.createOrUpdate(new Setting{key = Setting.KEY_PRODUCT_LAST_SYNC, value = productLastSync});

            productLastSyncServer = DateTime.Now.ToString(KStrings.TIME_FORMAT);
            Setting.createOrUpdate(new Setting {key = Setting.KEY_PRODUCT_LAST_SYNC_SERVER, value = productLastSyncServer});

            return new List<Product>(addedProducts.Values);
        }

        private void downloadImages()
        {
            ProductImagesDownloader productImagesDownloader = new ProductImagesDownloader(onImageDownloadProgress, downloadImages_completed);

            List<ProductUnit> productUnits = ProductUnit.all(" Where photo IS NOT NULL AND local_photo IS NULL ");

            if (productUnits == null) return;

            productImagesDownloader.beginDownloadProcess(productUnits);
        }

        private void downloadImages_completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (imageSyncComplete != null) imageSyncComplete();

            TimerHelper.SetTimeout(1000, () =>
            {
                Helpers.runOnUiThread(() => { 
                    MyEventBus.post(new EventMessage(EventMessage.EVENT_PRODUCT_SYNC_COMPLETED, null));
                });
            });
        }

    }
}
