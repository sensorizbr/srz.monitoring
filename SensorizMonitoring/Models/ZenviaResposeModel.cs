using System.Net;

namespace SensorizMonitoring.Models
{
    public class ZenviaResposeModel
    {
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

        public class SendResponse
        {
            public HttpStatusCode StatusCode { get; set; }
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public SendMessageResponse Response { get; set; }
        }

        public class ErrorResponse
        {
            public string ErrorMessage { get; set; }
        }

        public class SendMessageResponse
        {
            public string id { get; set; }
            public string from { get; set; }
            public string to { get; set; }
            public string direction { get; set; }
            public string channel { get; set; }
            public Content[] contents { get; set; }
            public DateTime timestamp { get; set; }
            public Conversation conversation { get; set; }
        }

        public class Conversation
        {
            public string solution { get; set; }
        }
    }
}
