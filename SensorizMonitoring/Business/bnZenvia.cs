using Newtonsoft.Json;
using System.Text;
using static SensorizMonitoring.Models.ZenviaResposeModel;

namespace ZenviaApi
{
    public class bnZenvia
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public bnZenvia(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _configuration = configuration;
        }

        public SendResponse SendSms(string to, string message)
        {
            string ApiToken = _configuration["Settings:ZENVIA_X-API-TOKEN"];
            string ApiUrl = _configuration["Settings:ZENVIA_URL_SMS"]; 

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

            var jsonBody = JsonConvert.SerializeObject(body);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl)
            {
                Content = content
            };

            request.Headers.Add("X-API-TOKEN", ApiToken);

            var response = _httpClient.Send(request);

            SendResponse result;

            if (response.IsSuccessStatusCode)
            {
                var responseBody = response.Content.ReadAsStringAsync().Result;
                var sendResponse = JsonConvert.DeserializeObject<SendMessageResponse>(responseBody);
                result = new SendResponse { StatusCode = response.StatusCode, Success = true, Response = sendResponse };
            }
            else
            {
                result = new SendResponse { StatusCode = response.StatusCode, Success = false, ErrorMessage = $"Error sending SMS message: {response.ReasonPhrase}" };
            }

            return result;
        }


        public SendResponse SendWhatsApp(string to, string message)
        {
            string ApiToken = _configuration["Settings:ZENVIA_X-API-TOKEN"];
            string ApiUrl = _configuration["Settings:ZENVIA_URL_WHATSAPP"];

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

            var jsonBody = JsonConvert.SerializeObject(body);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl)
            {
                Content = content
            };

            request.Headers.Add("X-API-TOKEN", ApiToken);

            var response = _httpClient.Send(request);

            SendResponse result;

            if (response.IsSuccessStatusCode)
            {
                var responseBody = response.Content.ReadAsStringAsync().Result;
                var sendResponse = JsonConvert.DeserializeObject<SendMessageResponse>(responseBody);
                result = new SendResponse { StatusCode = response.StatusCode, Success = true, Response = sendResponse };
            }
            else
            {
                result = new SendResponse { StatusCode = response.StatusCode, Success = false, ErrorMessage = $"Error sending Whatsapp message: {response.ReasonPhrase}" };
            }

            return result;
        }
    }
}