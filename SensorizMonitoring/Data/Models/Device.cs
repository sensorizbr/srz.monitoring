using System.ComponentModel.DataAnnotations.Schema;

namespace SensorizMonitoring.Data.Models
{
    [Table("device")]
    public class Device
    {
        public int id { get; set; }
        public string device_code { get; set; }
        public int company_id { get; set; }
        public string description { get; set; }
        public int device_reference_id { get; set; }
        public int enabled { get; set; }
        public DateTime created_at { get; set; }
    }
}
