using System.ComponentModel.DataAnnotations;

namespace NetCore_Assignemt.Models
{
    public class OrderDetail
    {
        [Key]
        public int BookId { get; set; }

        [Key]
        public int OrderId { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
