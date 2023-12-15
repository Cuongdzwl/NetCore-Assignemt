using System.ComponentModel.DataAnnotations;

namespace NetCore_Assignemt.Models
{
    public class BookView
    {
        public Book Book { get; set; }

        [Display(Name = "Selected Authors")]
        public List<int>? SelectedAuthorIds { get; set; }

        [Display(Name = "Selected Categories")]
        public int? SelectedCategoryIds { get; set; }

        public List<Author>? Authors { get; set; }

        public Category? Categories { get; set; }

        [Display(Name = "Book Image")]
        public IFormFile Image { get; set; }
    }
}
