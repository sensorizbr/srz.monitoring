using System.ComponentModel.DataAnnotations;

namespace SensorizMonitoring.Models
{
    public class BranchModel
    {

        public int company_id { get; set; }

        [Required(ErrorMessage = "Field Name is Required")]
        [StringLength(50, ErrorMessage = "Should not have more then 50 Characteres")]
        public string name { get; set; }

        [Required(ErrorMessage = "Field Document is Required")]
        [StringLength(15, ErrorMessage = "Should not have more then 15 Characteres")]
        public string document { get; set; }

        [Required(ErrorMessage = "Field Mail is Required")]
        [StringLength(30, ErrorMessage = "Should not have more then 30 Characteres")]
        public string head_mail { get; set; }

        [Required(ErrorMessage = "Field Phone Number is Required")]
        [StringLength(15, ErrorMessage = "Should not have more then 15 Characteres. Use +5500112314568 format")]
        public string head_phonenumber { get; set; }
    }
}
