using Microsoft.AspNetCore.Mvc;
using NetCore_Assignemt.Data;
using Microsoft.AspNetCore.Authorization;
using NetCore_Assignemt.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using NetCore_Assignemt.Services.DTO;
using System.Drawing;

namespace NetCore_Assignemt.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;


        public CartController(AppDbContext context)
        {
            _context = context;
        }

        private string? getUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }


        private async Task<List<CartDTO>?> getCart()
        {
            // Get the user's unique identifier from the ClaimsPrincipal
            var userId = getUserId();

            // Check if the user has a cart
            var query = _context.Cart
                .Where(c => c.UserId == userId)
            .Include(c => c.Book);

            var cartItems = await query.ToListAsync();

            var cartDto = cartItems
                  .Where(c => c.Book != null) // Filter out items without a corresponding Book
                  .Select(c => new CartDTO
                  {
                      BookId = c.BookId,
                      Quantity = c.Quantity,
                      SubTotal = c.Quantity * c.Book.Price,
                      Book = new BookDTO
                      {
                          BookId = c.Book.BookId,
                          Title = c.Book.Title,
                          Description = c.Book.Description,
                          Price = c.Book.Price,
                          ImagePath = c.Book.ImagePath,
                          Publisher = c.Book.Publisher
                      }
                  })
                  .ToList();

            return cartDto;
        }

        public async Task<IActionResult> Index()
        {
            return View(await getCart());
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = getUserId();

            var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == userId);

            var cart = await getCart();
            if (cart == null)
            {
                return Redirect("/");
            }

            var bill = new BillDTO
            {
                Cart = cart,
                Address = user.Address,
                Name = user.UserName,
                Email = user.Email,
                Phone = user.PhoneNumber
            };
            return View(bill);
        }

    }
}
