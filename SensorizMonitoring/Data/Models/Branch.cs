using System.ComponentModel.DataAnnotations.Schema;

namespace SensorizMonitoring.Data.Models
{
    [Table("branch")]
    public class Branch
    {
        public int id { get; set; }
        public int company_id { get; set; }
        public string full_name { get; set; }
        public string document { get; set; }
        public string head_mail { get; set; }
        public string head_phonenumber { get; set; }
        public int enabled { get; set; }
        public DateTime created_at { get; set; }
    }
}
