using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetCore_Assignemt.Models
{
    public class BookCategory
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Book")]
        public int? BookId { get; set; }
        public virtual Book? Books { get; set; }

        [ForeignKey("Category")]
        public int? CategoryId { get; set; }

        public virtual Category? Categories { get; set; }

    }
}
