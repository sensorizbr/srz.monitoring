using System.ComponentModel.DataAnnotations.Schema;

namespace SensorizMonitoring.Data.Models
{
    [Table("company")]
    public class Company
    {
        public int id { get; set; }
        public string name { get; set; }
        public string document { get; set; }
        public string head_mail { get; set; }
        public string head_phonenumber { get; set; }
        public int enabled { get; set; }
        public DateTime created_at { get; set; }
    }
}
