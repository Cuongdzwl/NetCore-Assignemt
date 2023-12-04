using Microsoft.AspNetCore.Mvc;
using NetCore_Assignemt.Models;

namespace NetCore_Assignemt.Services
{
    public interface ICartServices
    {
        public IActionResult AddToCart(int bookid);
        public IActionResult AddToCart(int book, int quantity);
        public Task<IActionResult> AddToCartAsync(int bookid, int quantity);
        public Task<IActionResult> AddToCartAsync(int bookid);

        public IActionResult CheckOut();
    }
}
