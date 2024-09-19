using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestXML.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int No { get; set; }
        public string Reg_Date { get; set; }
        public decimal Sum { get; set; }

        [ForeignKey("Users")]
        public int UserId { get; set; }
        public virtual User? User { get; set; }
    }
}
