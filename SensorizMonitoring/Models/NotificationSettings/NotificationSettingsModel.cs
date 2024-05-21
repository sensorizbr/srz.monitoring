﻿namespace SensorizMonitoring.Models.NotificationsSettings
{
    public class NotificationSettingsModel
    {
        public long device_id { get; set; }
        public string description { get; set; }
        public int sensor_type_id { get; set; }
        public int comparation_id { get; set; }
        public double min_value { get; set; }
        public double max_value { get; set; }
        public double lat_origin { get; set; }
        public double long_origin { get; set; }
        public double lat_destination { get; set; }
        public double long_destination { get; set; }
        public int tolerance_radius { get; set; }
        public bool b_value { get; set; }


        //public long device_id { get; set; }
        //public int sensor_type_id { get; set; }
        //public int comparation_id { get; set; }
        //public int interval_flag { get; set; }
        //public string start_value { get; set; }
        //public string end_value { get; set; }
        //public string exact_value { get; set; }
    }
}
