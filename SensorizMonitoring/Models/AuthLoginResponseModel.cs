namespace SensorizMonitoring.Models
{
    public class AuthLoginResponseModel
    {
        public int user_id { get; set; }
        public int branch_id { get; set; }
        public int role_id { get; set; }
        public string role_name { get; set; }
        public string fullname { get; set; }
        public string mail { get; set; }
    }
}
