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
    class PaginatedResponse
    {
        public PaginatedResponse()
        {

        }

        public object data { get; set; }
        public string path { get; set; }
        public int per_page { get; set; }
        public string next_page_url { get; set; }
        public string prev_page_url { get; set; }
        
        public override string ToString()
        {
            Trace.WriteLine(
                "data = " + data +
                "path = " + path +
                "next_page_url = " + next_page_url +
                "prev_page_url = " + prev_page_url +
                ", per_page = " + per_page
                );

            return base.ToString();
        }


    }
}
