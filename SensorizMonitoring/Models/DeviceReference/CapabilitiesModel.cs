namespace SensorizMonitoring.Models.DeviceReference
{
    public class CapabilitiesModel
    {
        public bool reader { get; set; }
        public bool tag { get; set; }
        public bool profiles { get; set; }
        public bool settings { get; set; }
        public bool otaFirmware { get; set; }
        public bool flightModel { get; set; }
        public bool oceanModel { get; set; }
        public bool movementDetection { get; set; }
        public bool shockDetection { get; set; }
        public bool tipDetection { get; set; }
        public bool wifi { get; set; }
        public List<string>? sensors { get; set; }
        public PairingModel pairing { get; set; }
    }
}
