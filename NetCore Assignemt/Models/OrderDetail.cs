using System.ComponentModel.DataAnnotations;

namespace NetCore_Assignemt.Models
{
    public class OrderDetail
    {
        public long Id { get; set; }
        public int BookId { get; set; }
        public long OrderId { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public int Quantity { get; set; }

        public virtual Order? Order { get; set; }
        public virtual Book? Book { get; set; }
    }
}
