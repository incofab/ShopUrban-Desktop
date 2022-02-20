using ShopUrban.Model;
using ShopUrban.Util;
using ShopUrban.Util.Network;
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
using System.Windows.Shapes;

namespace ShopUrban.View.Dialogs
{
    /// <summary>
    /// Interaction logic for EditShopProductDialog.xaml
    /// </summary>
    public partial class EditShopProductDialog : Window
    {
        public ShopProduct shopProduct { get; set; }
        private UpdateShopProductObject updateShopProductObject;

        public EditShopProductDialog(ShopProduct shopProduct)
        {
            InitializeComponent();
            DataContext = this;

            this.shopProduct = shopProduct;
            updateShopProductObject = new UpdateShopProductObject();

            tbStockCountToUpdate.Text = shopProduct.stock_count + "";
        }

        private void btnUpdateProduct_Click(object sender, RoutedEventArgs e)
        {
            if (isProductUpdateLoading) return;

            setProductUpdateLoading(true);

            float.TryParse(tbCostPrice.Text.Trim(), out float costPrice);
            float.TryParse(tbSellPrice.Text.Trim(), out float sellPrice);
            int.TryParse(tbRestockAlert.Text.Trim(), out int restockAlert);

            updateShopProductObject.id = shopProduct.id;
            updateShopProductObject.cost_price = costPrice;
            updateShopProductObject.sell_price = sellPrice;
            updateShopProductObject.restock_alert = restockAlert;
            updateShopProductObject.expired_date = tbExpiredDate.Text.Trim();

            List<KeyValuePair<string, string>> l = new List<KeyValuePair<string, string>>();

            l.Add(new KeyValuePair<string, string>("cost_price", updateShopProductObject.cost_price+""));
            l.Add(new KeyValuePair<string, string>("sell_price", updateShopProductObject.sell_price+""));
            l.Add(new KeyValuePair<string, string>("restock_alert", updateShopProductObject.restock_alert+""));
            l.Add(new KeyValuePair<string, string>("expired_date", updateShopProductObject.expired_date));
            l.Add(new KeyValuePair<string, string>("shop_product_id", shopProduct.id+""));

            updateShopProduct(l, updateShopProductObject);
        }

        private async void updateShopProduct(List<KeyValuePair<string, string>> l, UpdateShopProductObject updateObject)
        {
            BaseResponse baseResponse = await BaseResponse.callEndpoint(KStrings.URL_SHOP_PRODUCT_UPDATE, l);

            setProductUpdateLoading(false);

            if (baseResponse == null || baseResponse.status != "success")
            {
                MessageBox.Show((baseResponse != null)
                    ? baseResponse.message : $"Update(): Error connecting to server");
                setProductUpdateLoading(false);
                return;
            }

            ShopProduct.update(shopProduct.id, updateObject);

            shopProduct.cost_price = updateObject.cost_price;
            shopProduct.sell_price = updateObject.sell_price;
            shopProduct.restock_alert = updateObject.restock_alert;
            shopProduct.expired_date = updateObject.expired_date;

            MessageBox.Show("Product detail updated");

            // Close the dialog if the stock count has some values
            if (shopProduct.stock_count > 0) DialogResult = true;
        }

        private bool isProductUpdateLoading;
        private void setProductUpdateLoading(bool isLoading)
        {
            isProductUpdateLoading = isLoading;

            btnUpdateProduct.Content = isLoading ? "Loading..." : "Update";
        }

        private string quantityPrefix = "";
        private void btnUpdateStock_Click(object sender, RoutedEventArgs e)
        {
            if (isStockUpdateLoading) return;
            
            setStockUpdateLoading(true);

            int.TryParse(tbStockCountToUpdate.Text.Trim(), out int quantity);

            quantity = Math.Abs(quantity);

            var updated_stock_count = string.IsNullOrEmpty(quantityPrefix)
                    ? shopProduct.stock_count + quantity
                    : shopProduct.stock_count - quantity;

            List<KeyValuePair<string, string>> l = new List<KeyValuePair<string, string>>();

            //Helpers.log("shopProduct.stock_count 1 = " + shopProduct.stock_count);
            //Helpers.log("quantity = " + quantity);

            l.Add(new KeyValuePair<string, string>("action_type", string.IsNullOrEmpty(quantityPrefix) ? "1" : "0"));
            l.Add(new KeyValuePair<string, string>("action_reason", tbReason.Text.Trim()));
            l.Add(new KeyValuePair<string, string>("quantity", quantity+""));
            l.Add(new KeyValuePair<string, string>("shop_product_id", shopProduct.id + ""));

            processStockAction(l, updated_stock_count);
        }

        private async void processStockAction(List<KeyValuePair<string, string>> l, int updated_stock_count)
        {
            //Helpers.log("updated_stock_count = " + updated_stock_count);
            //Helpers.log("shopProduct.stock_count = " + shopProduct.stock_count);
            //return;
            BaseResponse baseResponse = await BaseResponse.callEndpoint(KStrings.URL_SHOP_PRODUCT_STOCK_ACTION, l);

            setStockUpdateLoading(false);

            if (baseResponse == null || baseResponse.status != "success")
            {
                MessageBox.Show((baseResponse != null)
                    ? baseResponse.message : $"Update(): Error connecting to server");
                setProductUpdateLoading(false);
                return;
            }

            ShopProduct.update(shopProduct.id, new { stock_count = updated_stock_count, id = shopProduct.id });

            shopProduct.stock_count = updated_stock_count;

            MessageBox.Show("Product stock count updated");

            if (shopProduct.sell_price > 0) DialogResult = true;
        }

        private bool isStockUpdateLoading;
        private void setStockUpdateLoading(bool isLoading)
        {
            isStockUpdateLoading = isLoading;

            btnUpdateStock.Content = isLoading ? "Loading..." : "Update";
        }

        private class UpdateShopProductObject
        {
            public int id { get;  set; }
            public float cost_price { get;  set; }
            public float sell_price { get;  set; }
            public int restock_alert { get;  set; }
            public string expired_date { get;  set; }
        }

        private void cbIncrease_Click(object sender, RoutedEventArgs e)
        {
            updateCB("");
        }

        private void cbDecrease_Click(object sender, RoutedEventArgs e)
        {
            updateCB("-");
        }

        private void updateCB(string prefix)
        {
            quantityPrefix = prefix;

            if(prefix == "")
            {
                cbIncrease.IsChecked = true;
                cbDecrease.IsChecked = false;
            }
            else
            {
                cbIncrease.IsChecked = false;
                cbDecrease.IsChecked = true;
            }

            var text = tbStockCountToUpdate.Text.Trim();

            if (text.StartsWith("-")) text = text.Substring(1);

            tbStockCountToUpdate.Text = prefix + text;
        }

        private void tbStockCountToUpdate_KeyUp(object sender, KeyEventArgs e)
        {
            var text = tbStockCountToUpdate.Text.Trim();
            if (text.Length < 1) return;

            updateCB(text.StartsWith("-") ? "-" : "");
        }
    }

}
