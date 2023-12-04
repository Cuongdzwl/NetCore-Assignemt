using Microsoft.AspNetCore.Mvc;
using NetCore_Assignemt.Models;

namespace NetCore_Assignemt.Services
{
    public interface ICartServices
    {
        public Task<IActionResult> AddToCartAsync(int bookid, int quantity);
        public Task<IActionResult> AddToCartAsync(int bookid);

        public IActionResult CheckOut();
    }
}
