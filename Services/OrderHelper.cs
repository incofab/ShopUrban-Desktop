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
    public class OrderHelper
    {
        public static void uploadPendingOrders(Action<BaseResponse> onComplete = null, bool showMessageIfEmpty = true)
        {
            List<Order> orders = Order.getUnUploadedOrders();

            string shopId = Setting.getShopId();

            if (orders == null || orders.Count < 1)
            {
                if (!showMessageIfEmpty) return;

                MessageBox.Show($"No new item to upload");

                return;
            }

            List<KeyValuePair<string, string>> l = new List<KeyValuePair<string, string>>();

            l.Add(new KeyValuePair<string, string>("orders", JsonConvert.SerializeObject(orders)));
            l.Add(new KeyValuePair<string, string>("shop_id", shopId));

            _ = BaseResponse.callEndpoint(KStrings.URL_UPLOAD_ORDERS_API, l, "POST", (BaseResponse baseResponse) => 
                { 
                    if (baseResponse == null || baseResponse.status != "success")
                    {
                        if (showMessageIfEmpty){
                            MessageBox.Show(baseResponse.message ?? $"Order Sync(): Error connecting to server");
                        }

                        return;
                    }

                    Setting.createOrUpdate(new Setting(){
                        key = Setting.KEY_ORDER_LAST_SYNC_ID,
                        value = orders[orders.Count - 1].id + "",
                    });

                    var lastOrderSyncTime = DateTime.Now.ToString(KStrings.TIME_FORMAT);
                    Setting.createOrUpdate(new Setting(){key = Setting.KEY_ORDER_LAST_SYNC_TIME, value = lastOrderSyncTime});

                    if (onComplete != null) onComplete(baseResponse);
                }
            );

        }

    }
}
