using System.ComponentModel.DataAnnotations;

namespace NetCore_Assignemt.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(450)]
        public string UserId { get; set; }

        public double Total { get; set; }

        [Timestamp]
        public byte[]? CreatedDate { get; set; }

        public long? PaymentTranId { get; set; }
        public string? BankCode { get; set; }
        public string? PayStatus { get; set; }
    }
}
