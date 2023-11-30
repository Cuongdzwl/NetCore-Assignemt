using System.ComponentModel.DataAnnotations;

namespace NetCore_Assignemt.Models
{
    public class BookCategory
    {
        [Key]
        public int BookId { get; set; }
        [Key]
        public int CategoryId { get; set; }

    }
}
