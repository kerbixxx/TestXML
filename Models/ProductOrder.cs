using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestXML.Models
{
    public class ProductOrder
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public int OrderId { get; set; }
        public virtual Order? Order { get; set; }
        public int Quantity { get; set; }
    }
}
