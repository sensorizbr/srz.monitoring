using System.ComponentModel.DataAnnotations;

namespace SensorizMonitoring.Models
{
    public class DeviceModel
    {
        public long device_code { get; set; }

        public int branch_id { get; set; }

        [Required(ErrorMessage = "Field Description is Required")]
        [StringLength(100, ErrorMessage = "Should not have more then 100 Characteres")]
        public string description { get; set; }

        public int device_reference_id { get; set; }
    }
}
