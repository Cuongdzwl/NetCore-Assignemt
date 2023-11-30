using System.ComponentModel.DataAnnotations;

namespace NetCore_Assignemt.Models
{
    public class BookAuthor
    {
        [Key]
        public int BookId { get; set; }
        [Key]
        public int AuthorId { get; set; }
    }
}
