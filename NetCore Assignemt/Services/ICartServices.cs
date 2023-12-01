using Microsoft.AspNetCore.Mvc;
using NetCore_Assignemt.Models;

namespace NetCore_Assignemt.Services
{
    public interface ICartServices
    {
        public IActionResult AddToCart(Book book);
        public IActionResult AddToCart(Book book, int quantity);
        public Task<IActionResult> AddToCartAsync(Book book, int quantity);
        public Task<IActionResult> AddToCartAsync(Book book);
    }
}
