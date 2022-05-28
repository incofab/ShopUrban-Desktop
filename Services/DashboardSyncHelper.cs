using Newtonsoft.Json;
using ShopUrban.Model;
using ShopUrban.Util;
using ShopUrban.Util.Network;
using ShopUrban.Util.Network.Responses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShopUrban.Services
{
    public class DashboardSyncHelper
    {

        public static void syncDashboard()
        {
            string desktop_index_last_sync = Setting.getValue(Setting.KEY_DESKTOP_INDEX_LAST_SYNC);

            Task.Run(async () => { 

                List<KeyValuePair<string, string>> l = new List<KeyValuePair<string, string>>();

                //l.Add(new KeyValuePair<string, string>("shop_id", shopId));
                l.Add(new KeyValuePair<string, string>("desktop_index_last_sync", desktop_index_last_sync));

                BaseResponse baseResponse = await BaseResponse.callEndpoint(KStrings.URL_DESKTOP_DASHBOARD_API, l);

                if (baseResponse == null || baseResponse.status != "success")
                {
                    Helpers.log((baseResponse != null) ? baseResponse.message : $"Login(): Error connecting to server");
                    return;
                }

                DashboardIndexResponse dashboardIndexResponse =
                    JsonConvert.DeserializeObject<DashboardIndexResponse>(baseResponse.data.ToString());

                Helpers.runOnUiThread(() => {
                    
                    persistDashboardSync(dashboardIndexResponse);

                });

            });

        }

        private static void persistDashboardSync(DashboardIndexResponse dashboardIndexResponse)
        {
            ShopCustomer.multiCreateOrUpdate(dashboardIndexResponse.shop_customers);
            Setting.multiCreateOrUpdate(dashboardIndexResponse.shop_settings);

            DateTime.Now.ToString(KStrings.TIME_FORMAT);
            var desktop_index_last_sync = DateTime.Now.ToString(KStrings.TIME_FORMAT);

            Setting.createOrUpdate(new Setting()
            {
                key = Setting.KEY_DESKTOP_INDEX_LAST_SYNC,
                value = desktop_index_last_sync,
            });

        }

    }
}
