//using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace SensorizMonitoring.Data.Models
{
    [Table("notification_log")]
    public class NotificationLog
    {
        public int id { get; set; }
        public long device_id { get; set; }
        public long seq { get; set; }
        public int branch_id { get; set; }
        public int setting_id { get; set; }
        public string phone_number { get; set; }
        public string mail { get; set; }
        public string message_request { get; set; }
        public string message_response { get; set; }
        public DateTime created_at { get; set; }
    }
}
