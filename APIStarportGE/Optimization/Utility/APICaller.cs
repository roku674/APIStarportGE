﻿
//Created by Alexander Fields 

using Optimization.Objects.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Optimization.Utility
{
    public class APICaller
    {
        /// <summary>
        /// Remember to dispose the HttpClient you create to use for the methods
        /// </summary>
        public APICaller()
        {
            MaxRetries = 3;
        }

        /// <summary>
        /// Starts at 3
        /// </summary>
        public static int MaxRetries { get; set; }

        /// <summary>
        /// Returns the response body logging the successes and failures
        /// </summary>
        /// <param name="client">pass in the client to use you'll already need request headers but not json</param>
        /// <param name="endpoint">ex: https://www.optimized.com/get/money</param>
        /// <param name="retry">Start at 0</param>
        /// <param name="logs">This should be a static list if null won't add anything </param>
        /// <returns>Generic Reponse</returns>
        public async Task<string> GetResponseBodyFromApiAsync(HttpClient client, string endpoint, int retry, List<LogMessage> logs)
        {
            string responseBody = await GetApiResponseAsync(client, endpoint, logs);

            if (responseBody != null)
            {
                return responseBody;
            }
            else if (retry < MaxRetries)
            {
                responseBody = await GetResponseBodyFromApiAsync(client, endpoint, ++retry, logs);
                logs?.Add(new LogMessage(logs.Count, System.DateTime.Now, "GetLoadRawAsync()", MessageType.Warning, "Attempted with retries: " + retry));
            }
            else
            {
                logs?.Add(new LogMessage(logs.Count, System.DateTime.Now, "GetLoadRawAsync()", MessageType.Error, "Failed"));
            }

            return responseBody;
        }

        private static async Task<string> GetApiResponseAsync(HttpClient client, string endpoint, List<LogMessage> logs)
        {
            string responseBody = null;

            try
            {
                HttpResponseMessage response = await client.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    responseBody = await response.Content.ReadAsStringAsync();
                    logs?.Add(new LogMessage(logs.Count, System.DateTime.Now, "GetApiResponseAsync()", MessageType.Success, $"Endpoint: {endpoint} Status Code: {(int)response.StatusCode} Reason Phrase: {response.ReasonPhrase}"));
                }
                else
                {
                    System.Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    logs?.Add(new LogMessage(logs.Count, System.DateTime.Now, "GetApiResponseAsync()", MessageType.Warning, $"Endpoint: {endpoint} Status Code: {(int)response.StatusCode} Reason Phrase: {response.ReasonPhrase}"));
                }
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e);
                logs?.Add(new LogMessage(logs.Count, System.DateTime.Now, "GetApiResponseAsync", MessageType.Error, $"API Reponse from {endpoint} threw error: {e}"));
            }

            return responseBody;
        }

        /// <summary>
        /// For posting a json to an API make sure you add request headers to your client
        /// </summary>
        /// <param name="client">pass in the client to use you'll already need request headers but not json</param>
        /// <param name="logs"></param>
        /// <param name="json">json that will be sent</param>
        /// <param name="endpoint">ex: https://www.optimized.com/poster/child</param>
        /// <returns>response message</returns>
        public static async Task<string> PostJson(HttpClient client, List<LogMessage> logs,string json, string endpoint)
        {
            string actualMessage = null;

            try
            {
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(endpoint, content);
                string result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    actualMessage = await response.Content.ReadAsStringAsync();
                    logs.Add(new LogMessage(logs.Count, System.DateTime.Now, "PostLogs()", MessageType.Success, $"Endpoint: {endpoint} Status Code: {(int)response.StatusCode} Reason Phrase: {response.ReasonPhrase}  response: {response} result: {result}"));
                }
                else
                {
                    System.Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    logs.Add(new LogMessage(logs.Count, System.DateTime.Now, "PostLogs()", MessageType.Warning, $"Endpoint: {endpoint} Status Code: {(int)response.StatusCode} Reason Phrase: {response.ReasonPhrase}  response: {response} result: {result}"));
                }
            }
            catch (System.Exception e)
            {
                logs.Add(new LogMessage(logs.Count, System.DateTime.Now, "PostLogs()", MessageType.Error, $"API Reponse from {endpoint} threw error: {e}"));
            }

            return actualMessage;
        }

        /// <summary>
        /// For putting a json to an API make sure you add request headers to your client
        /// </summary>
        /// <param name="client">pass in the client to use you'll already need request headers but not json</param>
        /// <param name="logs"></param>
        /// <param name="json">json that will be sent</param>
        /// <param name="endpoint">ex: https://www.optimized.com/put/theminthedirt</param>
        /// <returns>response message</returns>
        public static async Task<string> PutJson(HttpClient client, List<LogMessage> logs, string json, string endpoint)
        {
            string actualMessage = null;

            try
            {
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync(endpoint, content);
                string result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    actualMessage = await response.Content.ReadAsStringAsync();
                    logs.Add(new LogMessage(logs.Count, System.DateTime.Now, "PostLogs()", MessageType.Success, $"Endpoint: {endpoint} Status Code: {(int)response.StatusCode} Reason Phrase: {response.ReasonPhrase}  response: {response} result: {result}"));
                }
                else
                {
                    System.Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    logs.Add(new LogMessage(logs.Count, System.DateTime.Now, "PostLogs()", MessageType.Warning, $"Endpoint: {endpoint} Status Code: {(int)response.StatusCode} Reason Phrase: {response.ReasonPhrase}  response: {response} result: {result}"));
                }
            }
            catch (System.Exception e)
            {
                logs.Add(new LogMessage(logs.Count, System.DateTime.Now, "PostLogs()", MessageType.Error, $"API Reponse from {endpoint} threw error: {e}"));
            }

            return actualMessage;
        }

        /// <summary>
        /// Will add the request headers to client object passed in
        /// </summary>
        /// <param name="client"></param>
        /// <param name="host"></param>
        /// <param name="apiKeyName"></param>
        /// <param name="apiKey"></param>
        /// <param name="trace"></param>
        /// <returns>HttpClient</returns>
        public static HttpClient AddRequestHeaders(HttpClient client, string host, string apiKeyName, string apiKey, string trace)
        {
            if (!string.IsNullOrEmpty(host))
            {
                client.DefaultRequestHeaders.Add("Host", host);
            }
            if (!string.IsNullOrEmpty(apiKeyName))
            {
                client.DefaultRequestHeaders.Add(apiKeyName, apiKey);
            }
            if (!string.IsNullOrEmpty(trace))
            {
                client.DefaultRequestHeaders.Add(trace, "true");
            }

            return client;
        }

    }

}
