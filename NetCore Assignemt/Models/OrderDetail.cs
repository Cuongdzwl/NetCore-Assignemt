using System.ComponentModel.DataAnnotations;

namespace NetCore_Assignemt.Models
{
    public class OrderDetail
    {
        [Key]
        public long Id { get; set; }
        public long OrderId { get; set; }
        public int BookId { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public int Quantity { get; set; }

        public virtual Order? Order { get; set; }
        public virtual Book? Book { get; set; }
    }
}
