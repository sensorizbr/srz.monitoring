namespace SensorizMonitoring.Models
{
    public class LocoDevicesResponse
    {
        public int limit { get; set; }
        public int totalResults { get; set; }
        public Cursor cursor { get; set; }
        public List<Result> results { get; set; }
    }

    public class Cursor
    {
        public string previous { get; set; }
        public string next { get; set; }
        public bool hasPrevious { get; set; }
        public bool hasNext { get; set; }
    }

    public class Firmware
    {
        public string current { get; set; }
        public object pending { get; set; }
    }

    public class Global
    {
        public double lat { get; set; }
        public double lon { get; set; }
        public double cep { get; set; }
        public string address { get; set; }
    }

    public class LastKnownLocation
    {
        public string summary { get; set; }
        public string type { get; set; }
        public DateTime time { get; set; }
        public Global global { get; set; }
        public Site site { get; set; }
        public List<Zone> zones { get; set; }
    }

    public class Model
    {
        public string name { get; set; }
        public string family { get; set; }
        public string product { get; set; }
    }

    public class Owner
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Profile
    {
        public object accepted { get; set; }
        public object pending { get; set; }
    }

    public class Result
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<object> labels { get; set; }
        public Owner owner { get; set; }
        public Model model { get; set; }
        public Profile profile { get; set; }
        public LastKnownLocation lastKnownLocation { get; set; }
        public Firmware firmware { get; set; }
        public StatusIndicators statusIndicators { get; set; }
        public DateTime lastReportTime { get; set; }
        public DateTime? nextReportTime { get; set; }
    }

    public class Site
    {
        public string id { get; set; }
        public string name { get; set; }
        public int level { get; set; }
        public double cep { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public List<object> rooms { get; set; }
    }

    public class StatusIndicators
    {
        public string battery { get; set; }
        public bool moving { get; set; }
        public bool gpsFailure { get; set; }
        public bool lowSignal { get; set; }
        public bool charging { get; set; }
        public bool externalPower { get; set; }
        public bool flightMode { get; set; }
        public bool pendingSettings { get; set; }
    }

    public class Zone
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}
