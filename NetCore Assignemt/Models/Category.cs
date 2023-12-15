using NuGet.Packaging.Signing;
using System.ComponentModel.DataAnnotations;

namespace NetCore_Assignemt.Models
{
    public class Category
    {
        [Key] 
        public int CategoryId { get; set; }

        public string? Name { get; set; }

        public DateTime? CreatedDate { get; set; }

        public virtual ICollection<BookCategory>? BookCategories { get; set; }
    }
}
