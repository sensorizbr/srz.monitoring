using System.ComponentModel.DataAnnotations;

namespace SensorizMonitoring.Models
{
    public class DeviceModel
    {
        public string device_code { get; set; }

        public int company_id { get; set; }

        [Required(ErrorMessage = "Field Description is Required")]
        [StringLength(100, ErrorMessage = "Should not have more then 100 Characteres")]
        public string description { get; set; }

        [Required(ErrorMessage = "Field Model is Required")]
        [StringLength(30, ErrorMessage = "Should not have more then 30 Characteres")]
        public int modelID { get; set; }
    }
}
