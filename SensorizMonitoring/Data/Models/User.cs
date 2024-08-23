using System.ComponentModel.DataAnnotations.Schema;

namespace SensorizMonitoring.Data.Models
{
    [Table("user")]
    public class User
    {
        public int id { get; set; }
        public int branch_id { get; set; }
        public int user_type_id { get; set; }
        public string full_name { get; set; }
        public string document { get; set; }
        public string mail { get; set; }
        public string phone_number { get; set; }
        public byte[] password { get; set; }
        public int enabled { get; set; }
        public DateTime created_at { get; set; }
    }
}
