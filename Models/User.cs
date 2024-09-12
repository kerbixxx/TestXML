using System.Xml.Serialization;

namespace TestXML.Models
{
    [XmlRoot("user")]
    public class User
    {
        [XmlElement("fio")]
        public string Fio { get; set; }
        [XmlElement("email")]
        public string Email { get; set; }
    }
}
