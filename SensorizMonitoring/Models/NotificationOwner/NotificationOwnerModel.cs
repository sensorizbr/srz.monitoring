namespace SensorizMonitoring.Models.NotificationOwner
{
    public class NotificationOwnerModel
    {
        public string description { get; set; }
        public long device_id { get; set; }
        public int notification_setting_id { get; set; }
        public int notification_type_id { get; set; }
        //notification_type_id = 1 - MAIL
        //notification_type_id = 2 - SMS
        //notification_type_id = 3 - WHATSAPP
        public string phone_number { get; set; }
        public string mail { get; set; }
        public int enabled { get; set; }
    }
}
