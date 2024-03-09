namespace SensorizMonitoring.Models.DeviceReference
{
    public class ReferenceFamilyModel
    {
        public string family { get; set; }
        public CapabilitiesModel capabilities { get; set; }
        public string name { get; set; }
        public int version { get; set; }
        public int revision { get; set; }
        public BatteryModel battery { get; set; }
    }
}
