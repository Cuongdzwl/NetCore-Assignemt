using System.ComponentModel.DataAnnotations;

namespace NetCore_Assignemt.Models
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Invalid characters in the name")]
        public string Name { get; set; }

        public string? Description { get; set; }

        public virtual ICollection<BookAuthor>? BookAuthors { get; set; }


    }
}
