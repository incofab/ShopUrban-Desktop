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
    public class City : BaseModel
    {
        public int id { get; set; }
        public int state_id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string region_id { get; set; }

    }
}
