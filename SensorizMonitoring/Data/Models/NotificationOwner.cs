using System.ComponentModel.DataAnnotations.Schema;

namespace SensorizMonitoring.Data.Models
{
    [Table("notification_owner")]
    public class NotificationOwner
    {
        public int id { get; set; }
        public string description { get; set; }
        public int notification_setting_id { get; set; }
        public long device_id { get; set; }
        public int notification_type_id { get; set; }
        public string phone_number { get; set; }
        public string mail { get; set; }
        public int enabled { get; set; }
        public DateTime created_at { get; set; }
    }
}
