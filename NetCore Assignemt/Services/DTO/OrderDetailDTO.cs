using NetCore_Assignemt.Areas.Identity.Data;
using NetCore_Assignemt.Models;

namespace NetCore_Assignemt.Services.DTO
{
    public class OrderDetailDTO
    {
        public Order Order { get; set; }
        public User User { get; set; }
        public Transaction Transaction { get; set; }
        public ICollection<BookDTO> Books { get; set; }
    }
}
