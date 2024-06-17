//using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations.Schema;

namespace SensorizMonitoring.Data.Models
{
    [Table("notification_control")]
    public class NotificationControl
    {
        public int id { get; set; }
        public long device_id { get; set; }
        public int notification_id { get; set; }
    }
}
