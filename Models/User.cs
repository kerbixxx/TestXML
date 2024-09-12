using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace TestXML.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Fio { get; set; }
        public string Email { get; set; }
    }
}
