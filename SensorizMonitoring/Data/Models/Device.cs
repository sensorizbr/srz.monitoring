using System.ComponentModel.DataAnnotations.Schema;

namespace SensorizMonitoring.Data.Models
{
    [Table("device")]
    public class Device
    {
        public int id { get; set; }
        public long device_code { get; set; }
        public int branch_id { get; set; }
        public string description { get; set; }
        public int enabled { get; set; }
        public DateTime created_at { get; set; }
    }
}
