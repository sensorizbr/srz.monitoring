using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ZenviaApi
{
    public class bnZenvia
    {
        private readonly HttpClient _httpClient;

        public bnZenvia()
        {
            _httpClient = new HttpClient();
        }

        public class MessageRequest
        {
            public string from { get; set; }
            public string to { get; set; }
            public Content[] contents { get; set; }
        }

        public class Content
        {
            public string type { get; set; }
            public string text { get; set; }
        }

        public async Task<bool> SendSmsAsync(string to, string message)
        {
            var body = new MessageRequest
            {
                from = "stormy-donut",
                to = to.Trim(),
                contents = new[]
                {
                    new Content
                    {
                        type = "text",
                        text = message.Trim()
                    }
                }
            };

            //RQ9d5D5_xUcFEAduucPtuMPnkhQKAF2KH54d
            //78E30PlUb_lC5KKRpOGzWzTHLXIoSD5bIN4h

            var jsonBody = JsonSerializer.Serialize(body);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.zenvia.com/v2/channels/sms/messages")
            {
                Content = content
            };

            request.Headers.Add("X-API-TOKEN", "78E30PlUb_lC5KKRpOGzWzTHLXIoSD5bIN4h");

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var sendSmsResponse = JsonSerializer.Deserialize<SendSmsResponse>(responseBody);

            return true;
        }

        public async Task<bool> SendWhatsAppAsync(string to, string message)
        {
            var body = new MessageRequest
            {
                from = "crimson-manatee",
                to = to.Trim(),
                contents = new[]
                {
                    new Content
                    {
                        type = "text",
                        text = message.Trim()
                    }
                }
            };

            //RQ9d5D5_xUcFEAduucPtuMPnkhQKAF2KH54d
            //78E30PlUb_lC5KKRpOGzWzTHLXIoSD5bIN4h

            var jsonBody = JsonSerializer.Serialize(body);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.zenvia.com/v2/channels/whatsapp/messages")
            {
                Content = content
            };

            request.Headers.Add("X-API-TOKEN", "78E30PlUb_lC5KKRpOGzWzTHLXIoSD5bIN4h");

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var sendSmsResponse = JsonSerializer.Deserialize<SendSmsResponse>(responseBody);

            return true;
        }
    }

    public class SendSmsResponse
    {
        public string statusCode { get; set; }
        public string statusDescription { get; set; }
        public string detailCode { get; set; }
        public string detailDescription { get; set; }
    }
}