using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Reflection.Metadata.BlobBuilder;

namespace NetCore_Assignemt.Models
{
    public class BookAuthor
    {
        [Key] 
        public int Id { get; set; }
        [ForeignKey("Book")]
        public int BookId { get; set; }
        [ForeignKey("Author")]
        public int AuthorId { get; set; }


        public Book? Books { get; set; }

        public virtual Author? Authors { get; set; }
    }
}

