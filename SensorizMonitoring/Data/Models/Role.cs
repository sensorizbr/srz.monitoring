using System.ComponentModel.DataAnnotations.Schema;

namespace SensorizMonitoring.Data.Models
{
    [Table("role")]
    public class Role
    {
        public int id { get; set; }
        public string role_name { get; set; }
    }
}
