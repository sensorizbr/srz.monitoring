//using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations.Schema;

namespace SensorizMonitoring.Data.Models
{
    [Table("device_notification_settings")]
    public class NotificationSettings
    {
        public int id { get; set; }
        public long device_id { get; set; }
        public string description { get; set; }
        public int sensor_type_id { get; set; }
        public int comparation_id { get; set; }
        //comparation_type = 1 - Menor
        //comparation_type = 2 - Igual
        //comparation_type = 3 - Maior
        //comparation_type = 4 - Entre
        //comparation_type = 5 - Diferente
        public double min_value { get; set; }
        public double max_value { get; set; }
        public double lat_origin { get; set; }
        public double long_origin { get; set; }
        public double lat_destination { get; set; }
        public double long_destination { get; set; }
        public int? tolerance_radius { get; set; }
        public bool b_value { get; set; }
        public int enabled { get; set; }
        public DateTime created_at { get; set; }
    }
}
