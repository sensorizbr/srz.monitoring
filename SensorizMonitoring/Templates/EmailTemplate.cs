namespace SensorizMonitoring.Templates
{
    public class EmailMensagem
    {
        public string from { get; set; }
        public string to { get; set; }
        public List<EmailContent> contents { get; set; }
        public representative representative { get; set; }
    }

    public class EmailContent
    {
        public string type { get; set; }
        public string subject { get; set; }
        public string html { get; set; }
        public List<string> attachment { get; set; }
        public List<string> cc { get; set; }
        public List<string> bcc { get; set; }
    }

    public class replyTo {

        public string email { get; set; }
        public string name { get; set; }
    }

    public class representative
    {
        public string name { get; set; }
    }
}
