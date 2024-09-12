using System.Xml.Serialization;

namespace TestXML.Models.Dto
{
    [XmlRoot("user")]
    public class UserXml
    {
        [XmlElement("fio")]
        public string Fio { get; set; }
        [XmlElement("email")]
        public string Email { get; set; }
    }
}
