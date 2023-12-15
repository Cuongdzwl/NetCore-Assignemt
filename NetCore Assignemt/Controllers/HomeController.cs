using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCore_Assignemt.Data;
using NetCore_Assignemt.Models;
using System.Diagnostics;

namespace NetCore_Assignemt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _db;

        public HomeController(ILogger<HomeController> logger, AppDbContext db)
        {
            this._db = db;
            _logger = logger;
        }
        private List<Book> GetAllProducts()
        {
            return _db.Book.ToList();
        }
        public IActionResult Index()
        {
            var books = GetAllProducts();
            ViewBag.Book = books;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Cart()
        {
            ViewBag.Cart = _db.Cart.ToList();
            return View();
        }
    }
}