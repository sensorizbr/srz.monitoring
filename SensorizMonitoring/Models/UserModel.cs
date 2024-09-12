using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SensorizMonitoring.Models
{
    public class UserModel
    {

        public int branch_id { get; set; }

        [Required(ErrorMessage = "Field Document is Required")]
        [StringLength(15, ErrorMessage = "Should not have more then 15 Characteres")]
        public string document { get; set; }

        public string functional_number { get; set; }

        public int role_id { get; set; }

        [Required(ErrorMessage = "Field Full Name is Required")]
        [StringLength(50, ErrorMessage = "Should not have more then 50 Characteres")]
        public string full_name { get; set; }


        [Required(ErrorMessage = "Field Mail is Required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,63}(?:\.[a-zA-Z]{2,63})?$", ErrorMessage = "Invalid email address")]
        [DefaultValue("user@provider.com")]
        public string mail { get; set; }

        [Required(ErrorMessage = "Field Phone Number is Required")]
        [StringLength(15, ErrorMessage = "Should not have more then 15 Characteres. Use +5500112314568 format")]
        public string phone_number { get; set; }

        [Required(ErrorMessage = "Field Password is Required")]
        [StringLength(15, ErrorMessage = "Should not have more then 15 Characteres.")]
        public string password { get; set; }
    }
}
