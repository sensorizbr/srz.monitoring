namespace SensorizMonitoring.Models
{
    public class MonitoringModel
    {
        public int rxTime { get; set; }
        public PosModel pos { get; set; }
        public int device { get; set; }
        public string deviceId { get; set; }
        public StatusModel status { get; set; }
        public StateFlagsModel stateFlags { get; set; }
    }
}
