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
    class LoginResponse
    {
        public LoginResponse()
        {

        }

        public string token { get; set; }
        public Shop shop { get; set; }
        public User user { get; set; }
        
        public override string ToString()
        {
            Trace.WriteLine(
                "token = " + token +
                "shop = " + shop +
                ", User = " + user
                );

            return base.ToString();
        }


    }
}
