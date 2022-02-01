using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ShopUrban.Util.Network
{
    class UploadOrders
    {

        public async void uploadOrders()
        {
            List<KeyValuePair<string, string>> l = new List<KeyValuePair<string, string>>();

            //Connect to the internet to generate activation code and activate 
            BaseResponse baseResponse = await pushOrders(l);
            
            if (baseResponse == null || !baseResponse.isSuccessful)
            {
                Console.WriteLine((baseResponse != null)
                    ? baseResponse.ToString()
                    : $"pushOrders(): Error connecting to server");
                return;
            }

            //Use the response here
        }

        private async Task<BaseResponse> pushOrders(
            List<KeyValuePair<string, string>> l)
        {
            using (var client = new HttpClient())
            {
                string resultContent = null;
                try
                {
                    var content = new FormUrlEncodedContent(l);

                    //client.PostAsync
                    var result = await client.PostAsync(KStrings.URL_UPLOAD_ORDERS_API, content);

                    resultContent = await result.Content.ReadAsStringAsync();
                    //Console.WriteLine("resultContent ");
                    //Console.WriteLine(resultContent);
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<BaseResponse>(resultContent);
                }
                catch (Exception e)
                {
                    Console.WriteLine("pushOrders() error: " + e.Message);
                }
                return null;
            }
        }


    }
}
