using Newtonsoft.Json;
using ShopUrban.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ShopUrban.Util.Network.Responses
{
    class DashboardIndexResponse
    {
        public DashboardIndexResponse()
        {

        }

        public List<ShopCustomer> shop_customers { get; set; }
        public List<Setting> shop_settings { get; set; }
        
    }
}
