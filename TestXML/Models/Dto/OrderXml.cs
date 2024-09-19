using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace TestXML.Models.Dto
{
    [XmlRoot("orders")]
    public class RootOrders
    {
        [XmlElement("order")]
        public List<OrderXml> OrdersList { get; set; }
    }

    [XmlRoot("order")]
    public class OrderXml
    {
        [XmlElement("no")]
        public int No { get; set; }
        [XmlElement("reg_date")]
        public string Reg_Date { get; set; }
        [XmlElement("sum")]
        public decimal Sum { get; set; }
        [XmlElement("product")]
        public List<ProductXml> Products { get; set; } = new List<ProductXml>();
        [ForeignKey("User")]
        public int UserId { get; set; }
        [XmlElement("user")]
        public virtual UserXml? User { get; set; }
    }
}
