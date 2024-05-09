namespace SensorizMonitoring.Models.NotificationsSettings
{
    public class NotificationSettingsModel
    {
        public long device_id { get; set; }
        public int sensor_type_id { get; set; }
        public int comparation_id { get; set; }
        public int interval_flag { get; set; }
        public string start_value { get; set; }
        public string end_value { get; set; }
        public string exact_value { get; set; }
    }
}
