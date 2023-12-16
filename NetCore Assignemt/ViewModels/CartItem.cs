using NetCore_Assignemt.Areas.Identity.Data;
using NetCore_Assignemt.Models;

namespace NetCore_Assignemt.ViewModels
{
    public class CartItem
    {
        public int BookId { get; set; }
        public string Title { get; set; }

        public int Quantity { get; set; }
        public string ImagePath { get; set; }

        public double Price { get; set; }
        public double Total => Quantity * Price;
        public virtual Book? Book { get; set; }
        public virtual User? User { get; set; }
    }
}
