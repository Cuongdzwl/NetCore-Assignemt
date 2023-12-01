using System.ComponentModel.DataAnnotations;

namespace NetCore_Assignemt.Models
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }
        [Required]
        [RegularExpression("/^[a-zA-Z0-9_]+$/")]
        public string Name { get; set; }

        public string? Description { get; set; }

        public virtual ICollection<BookAuthor> BookAuthors { get; set; }


    }
}
