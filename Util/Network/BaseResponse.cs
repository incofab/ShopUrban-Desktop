using Newtonsoft.Json;
using ShopUrban.Model;
using ShopUrban.Services;
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
            Helpers.log(
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
            List<KeyValuePair<string, string>> l, string method = "POST", Action<BaseResponse> onComplete = null)
        {
            string token = Helpers.getLoginToken();
            var shopId = Helpers.getCurrentShop()?.id+"";

            if (l != null && !string.IsNullOrEmpty(shopId))
            {
                bool containsShopId = false;
                foreach (var kvp in l)
                {
                    if ("shop_id".Equals(kvp.Key)) containsShopId = true;
                }

                if (!containsShopId) l.Add(new KeyValuePair<string, string>("shop_id", shopId));
            }

            #region Log query params on DEV
            if (KStrings.DEV)
            {
                Helpers.log("Calling: " + url);
                Helpers.log($"shopId = {shopId}, Size = {l?.Count}, Token: " + token);

                foreach (var item in l)
                {
                    Helpers.log($"Param: {item.Key} = " + item.Value);
                }
            }
            #endregion

            using (var client = new HttpClient())
            {
                if(token != null)
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                string resultContent = null;
                BaseResponse res = null;
                try
                {
                    var content = new FormUrlEncodedContent(l);

                    HttpResponseMessage result;
                    //client.PostAsync
                    if ("POST".Equals(method))
                    {
                        result = await client.PostAsync(url, content);
                    }
                    else if ("PUT".Equals(method))
                    {
                        result = await client.PutAsync(url, content);
                    }
                    else
                    {
                        var queryParams = new QueryStringBuilder(l).ToString();
                        url = url + (string.IsNullOrEmpty(queryParams) ? "" : "?" + queryParams);
                        
                        result = await client.GetAsync(url);
                    }

                    resultContent = await result.Content.ReadAsStringAsync();
                    
                    Helpers.log("resultContent ");
                    Helpers.log(resultContent);

                    res =  Newtonsoft.Json.JsonConvert.DeserializeObject<BaseResponse>(resultContent);

                    if(res != null && !res.isSuccessful && "You are not logged in".Equals(res.message))
                    {
                        Helpers.runOnUiThread(() => { LoginHelper.logout(true); });
                    }
                }
                catch (Exception e)
                {
                    Helpers.log(url+" | Error: " + e.Message);
                }

                if(onComplete != null) Helpers.runOnUiThread(() => { onComplete(res); });

                return res;
            }
        }

    }
}
