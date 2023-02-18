
//Created by Alexander Fields 

using Optimization.Objects;
using System.Net.Http;
using System.Text;

namespace APIStarportGE.Models
{
    public class Webhooks
    {
        HttpClient client = new HttpClient();

        public void SendWebhook(string url, string message)
        {
            message = message.Replace("\"", "").Replace("'", "").Replace("\\", "");
            // Create the message payload
            string payload = "{ \"text\": \"" + message + "\" }";

            try
            {
                HttpResponseMessage response = client.PostAsync(url, new StringContent(payload, Encoding.UTF8, "application/json")).Result;

                if (!response.IsSuccessStatusCode)
                {
                    string body = response.Content.ReadAsStringAsync().Result;                   
                }
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e);
            }
        }

        public void AddHeaders()
        {
            client.DefaultRequestHeaders.Add("Host", Settings.Configuration["Microsoft:Teams:host"]);
        }

        public void Dispose() { client.Dispose(); }
    }
}
