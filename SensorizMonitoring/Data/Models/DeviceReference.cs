using System.ComponentModel.DataAnnotations.Schema;

namespace SensorizMonitoring.Data.Models
{
    [Table("device_reference")]
    public class DeviceReference
    {
        public int id { get; set; }
        public string family_name { get; set; }
        public string name { get; set; }
        public int reader { get; set; }
        public int tag { get; set; }
        public int profiles { get; set; }
        public int settings { get; set; }
        public int otaFirmware { get; set; }
        public int flightMode { get; set; }
        public int oceanMode { get; set; }
        public int movementDetection { get; set; }
        public int shockDetection { get; set; }
        public int tipDetection { get; set; }
        public int wifi { get; set; }
        public string sensors { get; set; }
        public DateTime created_at { get; set; }
    }
}
