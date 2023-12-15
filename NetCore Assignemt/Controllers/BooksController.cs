using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NetCore_Assignemt.Data;
using NetCore_Assignemt.Models;

namespace NetCore_Assignemt.Controllers
{
    public class BooksController : Controller
    {
        private readonly AppDbContext _context;


        public BooksController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Mod")]
        // GET: Books
        public async Task<IActionResult> Index()
        {
              return _context.Book != null ? 
                          View(await _context.Book.ToListAsync()) :
                          Problem("Entity set 'AppDbContext.Book'  is null.");
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public async Task<IActionResult> CreateAsync()
        {
            var authors = await _context.Author.ToListAsync();
            var categories = await _context.Category.ToListAsync();

            ViewData["AuthorId"] = new SelectList(authors, "AuthorId", "Name");
            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "Name");

          

            return View();
        }
 

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book, BookCategory bookCategory, BookAuthor bookAuthor)
        {
     
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                bookCategory.CategoryId = book.CategoryId;
                bookCategory.BookId = book.BookId;
                _context.Add(bookCategory);
                bookAuthor.BookId = book.BookId;
                bookAuthor.AuthorId = book.AuthorId;
                _context.Add(bookAuthor);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            
            


            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            var authors = await _context.Author.ToListAsync();
            var categories = await _context.Category.ToListAsync();

            ViewData["AuthorId"] = new SelectList(authors, "AuthorId", "Name");
            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "Name");
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book, BookCategory bookCategory, BookAuthor bookAuthor)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                    var existingCategories = await _context.BookCategory.Where(bc => bc.BookId == book.BookId).ToListAsync();
                    _context.BookCategory.RemoveRange(existingCategories);
                    if (bookCategory.CategoryId != 0) 
                    {
                        var newBookCategory = new BookCategory { BookId = book.BookId, CategoryId = bookCategory.CategoryId };
                        _context.BookCategory.Add(newBookCategory);
                    }
                    var existingAuthors = await _context.BookAuthor.Where(ba => ba.BookId == book.BookId).ToListAsync();
                    _context.BookAuthor.RemoveRange(existingAuthors);

                    if (bookAuthor.AuthorId != 0) 
                    {
                        var newBookAuthor = new BookAuthor { BookId = book.BookId, AuthorId = bookAuthor.AuthorId };
                        _context.BookAuthor.Add(newBookAuthor);
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Book == null)
            {
                return Problem("Entity set 'AppDbContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                var existingCategories = await _context.BookCategory.Where(bc => bc.BookId == book.BookId).ToListAsync();
                _context.BookCategory.RemoveRange(existingCategories);
                var existingAuthors = await _context.BookAuthor.Where(ba => ba.BookId == book.BookId).ToListAsync();
                _context.BookAuthor.RemoveRange(existingAuthors);
                await _context.SaveChangesAsync();
                _context.Book.Remove(book);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
          return (_context.Book?.Any(e => e.BookId == id)).GetValueOrDefault();
        }
    }
}
