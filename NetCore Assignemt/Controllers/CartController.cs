using Microsoft.AspNetCore.Mvc;
using NetCore_Assignemt.Data;
using NetCore_Assignemt.ViewModels;
using NetCore_Assignemt.Helper;
using Microsoft.AspNetCore.Authorization;
using NetCore_Assignemt.Models;

namespace NetCore_Assignemt.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;


        public CartController(AppDbContext context)
        {
            _context = context;
        }

        const string CART_KEY = "MYCART";
        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(CART_KEY) ?? new
            List<CartItem>();
        public IActionResult Index()
        {
            return View(Cart);
        }
        public IActionResult AddToCart(int Id, int quantity = 1)
        {
            var _cart = Cart;
            var item =_cart.SingleOrDefault(p => p.BookId == Id);
            if (item == null)
            {
                var book = _context.Book.SingleOrDefault(p => p.BookId == Id);
                //{
                //    TempData["Message"] = $"not found{Id}";
                //    return Redirect("/404");
                //}
                item = new CartItem 
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Price = book.Price,
                    ImagePath = book.ImagePath ?? string.Empty,
                    Quantity = book.Quantity
                };
                _cart.Add(item);
            }
            else
            {
                item.Quantity += quantity;
            }

            HttpContext.Session.Set(CART_KEY, _cart);

            return RedirectToAction("Index");
        }
        public IActionResult RemoveCart(int id)
        {
            var _cart = Cart;
            var item = _cart.SingleOrDefault(p => p.BookId == id);
            if (item != null) 
            {
                _cart.Remove(item);
                HttpContext.Session.Set(CART_KEY, _cart);
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            if(Cart.Count ==0)
            {
                return Redirect("/");
            }
            return View(Cart);
        }

    }
}
