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
        private async Task<List<Book>> GetAllProducts()
        {
            return await _db.Book.ToListAsync();
        }
        public async Task<IActionResult> Index()
        {
            var books = await GetAllProducts();
            ViewBag.Book = books;
            return View();
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _db.Book == null)
            {
                return NotFound();
            }

            var book = await _db.Book
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
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
    }
}