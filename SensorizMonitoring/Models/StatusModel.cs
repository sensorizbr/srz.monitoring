namespace SensorizMonitoring.Models
{
    public class StatusModel
    {
        public int seqNum { get; set; }
        public bool externalPower { get; set; }
        public bool charging { get; set; }
        public float batteryVoltage { get; set; }
        public float temperature { get; set; }
        public int lightLevel { get; set; }
        public float atmosphericPressure { get; set; }
        public OrientationModel orientation { get; set; }
        public VibrationModel vibration { get; set; }
        public int signal { get; set; }
        public string movement { get; set; }
        public string batteryState { get; set; }
        public int batteryLevel { get; set; }
        public bool lowSignal { get; set; }
        public bool gpsFail { get; set; }
    }
}
