using Newtonsoft.Json;
using ShopUrban.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ShopUrban.Util.Network
{
    public class BaseResponse
    {
        public BaseResponse()
        {

        }

        public BaseResponse(bool isSuccessful, string message)
        {
            this.isSuccessful = isSuccessful;
            this.message = message;
        }

        public int errorCode { get; set; }
        public string status { get; set; }
        //public int successful { get; set; }
        [JsonProperty("success")]
        public bool isSuccessful { get; set; }
        //public bool isSuccessful { get { return successful > 0; } set { isSuccessful = value; } }
        public string message { get; set; }
        public string result { get; set; }
        public object data { get; set; }
        public Product[] products { get; set; }
        
        public override string ToString()
        {
            Trace.WriteLine(
                "ErrorCode = " + errorCode
                //+ ", successful = " + successful
                + ", isSuccessful = " + isSuccessful
                + ", result = " + result
                + ", status = " + status
                + ", data = " + data
                + ", message = " + message
                );

            return base.ToString();
        }

        public static async Task<BaseResponse> callEndpoint(string url,
            List<KeyValuePair<string, string>> l)
        {
            string token = Helpers.getLoginToken();
            
            if (KStrings.DEV)
            {
                Trace.WriteLine("Calling: " + url);
                Trace.WriteLine("Token: " + token);
                
                foreach (var item in l)
                {
                    Trace.WriteLine($"Param: {item.Key} = " + item.Value);
                }
            }

            using (var client = new HttpClient())
            {
                if(token != null)
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                string resultContent = null;
                try
                {
                    var content = new FormUrlEncodedContent(l);

                    //client.PostAsync
                    var result = await client.PostAsync(url, content);

                    resultContent = await result.Content.ReadAsStringAsync();
                    
                    Trace.WriteLine("resultContent ");
                    Trace.WriteLine(resultContent);

                    return Newtonsoft.Json.JsonConvert.DeserializeObject<BaseResponse>(resultContent);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(url+" | Error: " + e.Message);
                }
                return null;
            }
        }

    }
}
