using ShopUrban.Model;
using ShopUrban.Util;
using ShopUrban.Util.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShopUrban.Services
{
    public class ProductSyncHelper
    {
        //public static void syncProductsAndImages(Action<bool> onProdutsDownloaded = null)
        //{
        //    var lastSync = Setting.getValue(Setting.KEY_PRODUCT_LAST_SYNC_SERVER);

        //    Task.Run(async () => {

        //        List<Product> products = await syncProducts(lastSync);

        //        Helpers.runOnUiThread(() => {

        //            var downlaodingImages = false;

        //            if (products != null && products.Count > 0)
        //            {
        //                downloadImages();
        //                downlaodingImages = true;
        //            }

        //            if (onProdutsDownloaded != null) onProdutsDownloaded(downlaodingImages);

        //            MyEventBus.post(new EventMessage(EventMessage.EVENT_PRODUCT_SYNC_COMPLETED, null));
        //        });
        //    });
        //}

        //public static async Task<List<Product>> syncProducts(string lastSync)
        //{
        //    ProductDownloader productDownloader = new ProductDownloader();
        //    List<Product> products = await productDownloader.loadProducts(lastSync);

        //    //setIsProductSyncing(false);

        //    if (products == null || products.Count == 0) return null;

        //    Helpers.runOnUiThread(() => {
                
        //        persistProductUpdate(products);
            
        //    });

        //    return products;
        //}

        //private static void persistProductUpdate(List<Product> products)
        //{
        //    var productLastSyncServer = Setting.getValue(Setting.KEY_PRODUCT_LAST_SYNC_SERVER);

        //    int totalCount = 0;
        //    foreach (var product in products)
        //    {
        //        Product.createOrUpdate(product);
        //        ProductUnit.multiCreateOrUpdate(product.productUnits);

        //        if (product.shopProducts != null && product.shopProducts.Count > 0)
        //        {
        //            var len = product.shopProducts.Count;
        //            productLastSyncServer = product.shopProducts[len - 1].updated_at;

        //            ShopProduct.multiCreateOrUpdate(product.shopProducts);

        //            totalCount += len;
        //        }

        //        ProductCategory.createOrUpdate(product.productCategory);
        //    }

        //    var productLastSync = DateTime.Now.ToString(KStrings.TIME_FORMAT);
        //    Setting.createOrUpdate(new Setting
        //    {
        //        key = Setting.KEY_PRODUCT_LAST_SYNC,
        //        value = productLastSync
        //    });

        //    productLastSyncServer = DateTime.Now.ToString(KStrings.TIME_FORMAT);
        //    Setting.createOrUpdate(new Setting
        //    {
        //        key = Setting.KEY_PRODUCT_LAST_SYNC_SERVER,
        //        value = productLastSyncServer
        //    });

        //}

        //private static void downloadImages()
        //{
        //    ProductImagesDownloader productImagesDownloader = new ProductImagesDownloader(null, downloadImages_completed);

        //    List<ProductUnit> productUnits = ProductUnit.all(
        //        " Where photo IS NOT NULL AND local_photo IS NULL ");

        //    if (productUnits == null) return;

        //    productImagesDownloader.beginDownloadProcess(productUnits);
        //}
        //private static void downloadImages_completed(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    TimerHelper.SetTimeout(1000, () =>
        //    {
        //        Application.Current.Dispatcher.Invoke((Action)delegate ()
        //        {
        //            MyEventBus.post(new EventMessage(EventMessage.EVENT_PRODUCT_SYNC_COMPLETED, null));

        //        }, null);
        //    });
        //}

    }
}
