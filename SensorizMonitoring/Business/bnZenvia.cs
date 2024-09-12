﻿using Google.Protobuf.WellKnownTypes;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SensorizMonitoring.Data.Models;
using SensorizMonitoring.Models;
using SensorizMonitoring.Templates;
using System.Text;
using static Org.BouncyCastle.Math.EC.ECCurve;
using static SensorizMonitoring.Models.ZenviaResposeModel;

namespace ZenviaApi
{
    public class bnZenvia
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public bnZenvia(IConfiguration configuration, ILogger logger)
        {
            _httpClient = new HttpClient();
            _configuration = configuration;
            _logger = logger;
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

                _logger.LogInformation("Enviou Notificacao");
                _logger.LogInformation(responseBody);
            }
            else
            {
                result = new SendResponse { StatusCode = response.StatusCode, Success = false, ErrorMessage = $"Error sending SMS message: {response.ReasonPhrase}" };

                _logger.LogCritical("Não Enviou Notificacao");
                _logger.LogCritical(result.ToString());
            }

            return result;
        }

        public SendResponse SendWhatsApp(string to, dynamic varsfieldsTemplate, string sTemplateID)
        {
            string ApiToken = _configuration["Settings:ZENVIA_X-API-TOKEN"];
            string ApiUrl = _configuration["Settings:ZENVIA_URL_WHATSAPP"];
            string ApiSender = _configuration["Settings:ZENVIA_WHATSAPP_SENDER"];
            //string ApiTemplateID = _configuration["Settings:ZENVIA_WHATSAPP_TEMPLATE_ID"];

            WhatsAppContent wpContents = new WhatsAppContent();
            wpContents.type = "template";
            wpContents.templateId = sTemplateID;
            wpContents.fields = varsfieldsTemplate;

            WhatsAppMensagem wpMessage = new WhatsAppMensagem();
            wpMessage.from = ApiSender;
            wpMessage.to = to.Trim();
            
            wpMessage.contents = new List<WhatsAppContent>();
            wpMessage.contents.Add(wpContents);

            var jsonBody = JsonConvert.SerializeObject(wpMessage);
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

                _logger.LogInformation("Enviou Notificacao");
                _logger.LogInformation(responseBody);
            }
            else
            {
                result = new SendResponse { StatusCode = response.StatusCode, Success = false, ErrorMessage = $"Error sending Whatsapp message: {response.ReasonPhrase}" };

                _logger.LogCritical("Não Enviou Notificacao");
                _logger.LogCritical(result.ToString());
            }

            return result;
        }

        //public SendResponse SendEmail(string to, dynamic varsfieldsTemplate)
        //{
        //    string ApiToken = _configuration["Settings:ZENVIA_X-API-TOKEN"];
        //    string ApiUrl = _configuration["Settings:ZENVIA_URL_EMAIL"];
        //    string ApiSender = _configuration["Settings:ZENVIA_EMAIL_SENDER"];
        //    //string ApiTemplateID = _configuration["Settings:ZENVIA_WHATSAPP_TEMPLATE_ID"];

        //    EmailContent emContents = new EmailContent();
        //    emContents.type = "email";
        //    emContents.subject = "Alerta Sensoriz";
        //    emContents.html = varsfieldsTemplate;

        //    EmailMensagem emMessage = new EmailMensagem();
        //    emMessage.from = ApiSender;
        //    emMessage.to = to.Trim();

        //    emMessage.contents = new List<WhatsAppContent>();
        //    emMessage.contents.Add(emContents);

        //    var jsonBody = JsonConvert.SerializeObject(emMessage);
        //    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        //    var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl)
        //    {
        //        Content = content
        //    };

        //    request.Headers.Add("X-API-TOKEN", ApiToken);

        //    var response = _httpClient.Send(request);

        //    SendResponse result;

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var responseBody = response.Content.ReadAsStringAsync().Result;
        //        var sendResponse = JsonConvert.DeserializeObject<SendMessageResponse>(responseBody);
        //        result = new SendResponse { StatusCode = response.StatusCode, Success = true, Response = sendResponse };

        //        _logger.LogInformation("Enviou Notificacao");
        //        _logger.LogInformation(responseBody);
        //    }
        //    else
        //    {
        //        result = new SendResponse { StatusCode = response.StatusCode, Success = false, ErrorMessage = $"Error sending Whatsapp message: {response.ReasonPhrase}" };

        //        _logger.LogCritical("Não Enviou Notificacao");
        //        _logger.LogCritical(result.ToString());
        //    }

        //    return result;
        //}
    }
}