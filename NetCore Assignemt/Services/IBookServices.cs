using Microsoft.AspNetCore.Mvc;
using NetCore_Assignemt.Models;

namespace NetCore_Assignemt.Services
{
    public interface IBookServices
    {
        public Task<IActionResult> GetBookCategoriesAsync(Book book);
        public Task<IActionResult> GetBookAuthorssAsync(Book book);

        public Task<IActionResult> SearchAsync(string keyword, string searchType);
        public Task<IActionResult> Search(string keyword, string searchType);
    }
}
