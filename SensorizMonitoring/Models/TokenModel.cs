using System.ComponentModel.DataAnnotations;

namespace SensorizMonitoring.Models
{
    public class TokenModel
    {
        public string token { get; set; }
        public string refresh_token { get; set; }
    }
}
