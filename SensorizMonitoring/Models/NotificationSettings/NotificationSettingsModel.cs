namespace SensorizMonitoring.Models.NotificationsSettings
{
    public class NotificationSettingsModel
    {
        public string device_id { get; set; }
        public int sensor_type_id { get; set; }
        public int interval_flag { get; set; }
        public double start_value { get; set; }
        public double end_value { get; set; }
        public double exatc_value { get; set; }
    }
}
