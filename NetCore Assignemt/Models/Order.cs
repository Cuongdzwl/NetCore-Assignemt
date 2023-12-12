using NetCore_Assignemt.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetCore_Assignemt.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        [Required]
        [MaxLength(450)]
        public string UserId { get; set; }

        public double Total { get; set; }

        public int Status { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public long? PaymentTranId { get; set; }
        public string? BankCode { get; set; }
        public string? PayStatus { get; set; }

        public virtual ICollection<OrderDetail>? OrderDetail { get; set; }
        public virtual User? User { get; set; }
    }
}
