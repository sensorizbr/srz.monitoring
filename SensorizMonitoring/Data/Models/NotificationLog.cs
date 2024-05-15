//using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations.Schema;

namespace SensorizMonitoring.Data.Models
{
    [Table("notification_log")]
    public class NotificationLog
    {
        public int id { get; set; }
        public long device_id { get; set; }
        public string description { get; set; }
        public int sensor_type_id { get; set; }
        public int comparation_id { get; set; }
        public string phone_number { get; set; }
        public string mail { get; set; }
        public string message { get; set; }
        public string reference_value { get; set; }
        public string monitoring_value { get; set; }
        public DateTime created_at { get; set; }
    }
}
