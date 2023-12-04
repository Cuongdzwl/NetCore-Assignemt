using Microsoft.EntityFrameworkCore;
using NetCore_Assignemt.Areas.Identity.Data;

namespace NetCore_Assignemt.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public int BookId { get; set; }
        public int Quantity { get; set; }


        public virtual Book? Book { get; set; }
        public virtual User? User { get; set; }

    }
}
