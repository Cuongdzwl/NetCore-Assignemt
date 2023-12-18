using NetCore_Assignemt.Models;

namespace NetCore_Assignemt.Services.DTO
{
    public class CartDTO
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public BookDTO Book { get; set; }
        public double SubTotal {  get; set; }
    }
}
