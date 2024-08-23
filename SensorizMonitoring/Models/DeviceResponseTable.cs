namespace SensorizMonitoring.Models
{
    public class DeviceResponseTable
    {
        public int id { get; set; }
        public string device_code { get; set; }
        public string model { get; set; }
        public string description { get; set; }
        public bool charging { get; set; }
        public string? firmware { get; set; }
        public int enabled { get; set; }
        public double lkl_lat { get; set; }
        public double lkl_lng { get; set; }
        public string battery { get; set; }
        public DateTime LastReport { get; set; }
        public DateTime created_at { get; set; }
    }
}
