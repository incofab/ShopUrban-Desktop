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
    public class User : BaseModel
    {
        public int id { get; set; }
        [JsonProperty("first_name")]
        public string firstname { get; set; }

        [JsonProperty("other_name")]
        public string othername { get; set; }
        public string surname { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string contact_no { get; set; }
        public string gender { get; set; }

        public override string ToString()
        {
            Trace.WriteLine(
                "id = " + id
                + "firstname = " + firstname
                + ", surname = " + surname
                + ", othername = " + othername
                + ", username = " + username
                + ", email = " + email
                + ", contact_no = " + contact_no
                + ", gender = " + gender
                );

            return base.ToString();
        }

    }
}
