//using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations.Schema;

namespace SensorizMonitoring.Data.Models
{
    [Table("sensor_type")]
    public class SensorType
    {
        public int id { get; set; }
        public string description { get; set; }
        public DateTime created_at { get; set; }
    }
}
