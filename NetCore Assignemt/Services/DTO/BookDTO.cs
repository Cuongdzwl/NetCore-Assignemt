using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NetCore_Assignemt.Services.DTO
{
    public class BookDTO
    {
        public int BookId { get; set; }
        public string? Title { get; set; }
        public string? Publisher { get; set; }
        public string? ImagePath { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
    }
}
