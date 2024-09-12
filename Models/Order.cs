using System.Xml.Serialization;

namespace TestXML.Models
{
    [XmlRoot("orders")]
    public class RootOrders
    {
        [XmlElement("order")]
        public List<Order> OrdersList { get; set; }
    }

    [XmlRoot("order")]
    public class Order
    {
        [XmlElement("no")]
        public int No { get; set; }
        [XmlElement("reg_date")]
        public string Reg_Date { get; set; }
        [XmlElement("sum")]
        public decimal Sum { get; set; }
        [XmlElement("product")]
        public List<Product> Products { get; set; } = new List<Product>();

        [XmlElement("user")]
        public User User { get; set; } = new User();
    }
}
