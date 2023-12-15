using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetCore_Assignemt.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Invalid characters in the name")]
        public string Title { get; set; }

        [Required]
        public string Publisher { get; set; }
        [NotMapped]
        public IFormFile? Image { get; set; }

        public string? ImagePath { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(0,double.MaxValue)]
        public double Price { get; set; }

        [Required]
        [Range(0,9999)]
        public int Quantity { get; set; }

        public DateTime? CreatedDate { get; set; }
        public int AuthorId { get; set; }
        public virtual ICollection<BookAuthor>? BookAuthors { get; set; }
        public int? CategoryId { get; set; }
        public virtual ICollection<BookCategory>? BookCategories { get; set; }

    }
}
