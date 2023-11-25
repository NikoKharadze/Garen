using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace App1
{
    internal class ServerCommunication
    {
        private const string BaseUrl = "http://10.42.134.5:500";
        private const string Endpoint = "/api/questions";

        public static async Task<string> FetchJsonDataAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = BaseUrl + Endpoint;
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        // Handle error
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    // Handle exception
                    return null;
                }
            }
        }

        public static int ParseJsonAndGetSpinsLeft(string json)
        {
            try
            {
                JObject jsonObject = JObject.Parse(json);
                int spinsLeft = (int)jsonObject["user"]["spinsLeft"];
                return spinsLeft;
            }
            catch (Exception ex)
            {
                // Handle exception
                return 0; // Default value if parsing fails
            }
        }
    }
}
