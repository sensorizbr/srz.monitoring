//using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations.Schema;

namespace SensorizMonitoring.Data.Models
{
    [Table("monitoring")]
    public class Monitoring
    {
        public int id { get; set; }
        public long device_id { get; set; }
        public double temperature { get; set; }
        public double atmospheric_pressure { get; set; }
        public double? lat { get; set; }
        public double? lon { get; set; }
        public double? cep { get; set; }
        public bool external_power { get; set; }
        public bool charging { get; set; }
        public double battery_voltage { get; set; }
        public int light_level { get; set; }
        public double? orientation_x { get; set; }
        public double? orientation_y { get; set; }
        public double? orientation_z { get; set; }
        public double? vibration_x { get; set; }
        public double? vibration_y { get; set; }
        public double? vibration_z { get; set; }
        public int com_signal { get; set; }
        public int tamper { get; set; }
        public string? movement { get; set; }
        public DateTime created_at { get; set; }
        public DateTime report_date { get; set; }
    }
}