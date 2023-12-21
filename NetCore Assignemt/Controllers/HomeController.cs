using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NetCore_Assignemt.Data;
using NetCore_Assignemt.Models;
using SendGrid.Helpers.Mail;
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
        public async Task<IActionResult> IndexAsync()
        {
            var books = await _db.Book
         .Include(b => b.BookAuthors)
         .Include(b => b.BookCategories)
         .Include(b => b.BookCategories.Categories)
         .Include(b => b.BookAuthors.Authors)
         .ToListAsync();
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
        public IActionResult SearchString(string SearchString)
        {
            var books = _db.Book
         .Where(b => b.Title.Contains(SearchString))
         .Include(b => b.BookAuthors)
         .Include(b => b.BookCategories)
         .Include(b => b.BookCategories.Categories)
         .Include(b => b.BookAuthors.Authors)
         .ToList();
            ViewBag.Book = books;
            return View("Index", SearchString);
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