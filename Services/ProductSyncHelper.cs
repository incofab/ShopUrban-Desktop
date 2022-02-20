using ShopUrban.Model;
using ShopUrban.Util;
using ShopUrban.Util.Network;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShopUrban.Services
{
    public class ProductSyncHelper
    {

        public static async Task<List<Product>> syncProducts()
        {
            var productLastSyncServer = Setting.getValue(Setting.KEY_PRODUCT_LAST_SYNC_SERVER);

            ProductDownloader productDownloader = new ProductDownloader();
            List<Product> products = await productDownloader.loadProducts();

            //setIsProductSyncing(false);

            if (products == null) return null;

            int totalCount = 0;
            foreach (var product in products)
            {
                Product.createOrUpdate(product);
                ProductUnit.multiCreateOrUpdate(product.productUnits);

                if (product.shopProducts != null)
                {
                    var len = product.shopProducts.Count;
                    productLastSyncServer = product.shopProducts[len - 1].updated_at;

                    ShopProduct.multiCreateOrUpdate(product.shopProducts);

                    totalCount += len;
                }
            }

            var productLastSync = DateTime.Now.ToString(KStrings.TIME_FORMAT);
            Setting.createOrUpdate(new Setting
            {
                key = Setting.KEY_PRODUCT_LAST_SYNC,
                value = productLastSync
            });

            productLastSyncServer = DateTime.Now.ToString(KStrings.TIME_FORMAT);
            Setting.createOrUpdate(new Setting
            {
                key = Setting.KEY_PRODUCT_LAST_SYNC_SERVER,
                value = productLastSyncServer
            });

            //handleViews();
            //MessageBox.Show((string)(products == null ? "Nothing" : totalCount + " products downloaded"));

            return products;
        }

    }
}
