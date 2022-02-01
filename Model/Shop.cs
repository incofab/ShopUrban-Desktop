using Dapper;
using ShopUrban.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace ShopUrban.Model
{
    public class Shop : BaseModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public int? address_id { get; set; }
        public string whats_app_number { get; set; }
        public int? city_id { get; set; }
        public int? state_id { get; set; }
        public string logo { get; set; }
        public string shop_id { get; set; }
        public string qr_code { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }

        public Address address { get; set; }

    }
}
