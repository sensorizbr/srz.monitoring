namespace SensorizMonitoring.Templates
{
    public class WhatsAppMensagem
    {
        public string from { get; set; }
        public string to { get; set; }
        public List<WhatsAppContent> contents { get; set; }
    }

    public class WhatsAppContent
    {
        public string type { get; set; }
        public string templateId { get; set; }
        public dynamic fields { get; set; }
    }
}
