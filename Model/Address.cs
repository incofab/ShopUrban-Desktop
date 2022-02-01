using Dapper;
using Newtonsoft.Json;
using ShopUrban.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ShopUrban.Model
{
    public class Address : BaseModel
    {
        public int id { get; set; }
        public int? country_id { get; set; }
        public string street_name { get; set; }
        public string land_mark { get; set; }
        public int? city_id { get; set; }
        public int? state_id { get; set; }
        public City city { get; set; }
        public State state { get; set; }
    }
}
