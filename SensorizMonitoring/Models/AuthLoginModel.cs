using System.ComponentModel.DataAnnotations;

namespace SensorizMonitoring.Models
{
    public class AuthLoginModel
    {
        public string mail { get; set; }
        public string password { get; set; }
    }
}
