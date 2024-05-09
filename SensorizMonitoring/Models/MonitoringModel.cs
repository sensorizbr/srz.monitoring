namespace SensorizMonitoring.Models
{
    public class MonitoringModel
    {
        public long rxTime { get; set; }
        public PosModel pos { get; set; }
        public long device { get; set; }
        public long deviceId { get; set; }
        public StatusModel status { get; set; }
        public StateFlagsModel stateFlags { get; set; }
    }
}
