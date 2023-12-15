using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NetCore_Assignemt.Models
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        public string vnp_ResponseCode { get; set; }

        public long vnp_Amount { get; set; }

        public string? vnp_BankCode { get; set; }

        public string? vnp_BankTranNo { get; set; }

        public string? vnp_CardType { get; set; }

        public string vnp_OrderInfo { get; set; }

        public DateTime vnp_PayDate { get; set; }

        public long vnp_TransactionNo { get; set; }

        public string vnp_TransactionStatus { get; set; } = string.Empty;
    }
}
