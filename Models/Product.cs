using System.Xml.Serialization;

namespace TestXML.Models
{
    [XmlRoot("product")]
    public class Product
    {
        [XmlElement("quantity")]
        public int Quantity { get; set; }
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("price")]
        public decimal Price { get; set; }
    }
}
