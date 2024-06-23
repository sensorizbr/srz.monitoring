namespace SensorizMonitoring.Models
{
    public class AuthLoginResponseModel
    {
        public int user_id { get; set; }
        public string fullname { get; set; }
        public string mail { get; set; }
        public string phone_number { get; set; }
        public string role { get; set; }
        public string company_name { get; set; }
        public bool logged { get; set; }
        public string token { get; set; }
    }
}
