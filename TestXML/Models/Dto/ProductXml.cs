
using System.Xml.Serialization;

namespace TestXML.Models.Dto
{
    [XmlRoot("product")]
    public class ProductXml
    {
        [XmlElement("name")]
        public string Name;
        [XmlElement("price")]
        public decimal Price;
        [XmlElement("quantity")]
        public int Quantity;
    }
}
