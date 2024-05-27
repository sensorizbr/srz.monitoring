namespace SensorizMonitoring.Models
{
    public class LocoPaginationRequest
    {
        public int limit { get; set; }
        public Filters filters { get; set; }

        public Sort sort { get; set; }
    }

    public class Filters
    {
        public List<string> owners { get; set; } = new List<string>();
    }

    public class Sort
    {
        public string by { get; set; }
        public string direction { get; set; }
    }
}
