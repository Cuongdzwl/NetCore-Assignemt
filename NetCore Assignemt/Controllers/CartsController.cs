using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCore_Assignemt.Data;
using NetCore_Assignemt.Models;
using NetCore_Assignemt.Services;
using NetCore_Assignemt.Services.DTO;

namespace NetCore_Assignemt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartsController : ControllerBase, ICartServices
    {
        private readonly AppDbContext _context;

        public CartsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Carts
        [HttpGet]
        public async Task<ActionResult<Cart>> Get()
        {
            // Get the user's unique identifier from the ClaimsPrincipal
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Check if the user has a cart
            var cartItems = await _context.Cart
                .Where(c => c.UserId == userId)
                .Include(c => c.Book)
                .ToListAsync();

            if (cartItems == null)
            {
                // Handle the case when the cart is not found
                return NoContent();
            }

            var cartDto = cartItems
                  .Where(c => c.Book != null) // Filter out items without a corresponding Book
                  .Select(c => new CartDTO
                  {
                      BookId = c.BookId,
                      Quantity = c.Quantity,
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

            return Ok(cartDto);
        }

        public IActionResult AddToCart(int bookid)
        {
            throw new NotImplementedException();
        }

        public IActionResult AddToCart(int book, int quantity)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> AddToCartAsync(int bookid, int quantity)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> AddToCartAsync(int bookid)
        {
            throw new NotImplementedException();
        }

        public IActionResult CheckOut()
        {
            throw new NotImplementedException();
        }
        // PUT: api/Carts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCart(int id, Cart cart)
        {
            if (id != cart.Id)
            {
                return BadRequest();
            }

            _context.Entry(cart).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Carts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cart>> PostCart(Cart cart)
        {
          if (_context.Cart == null)
          {
              return Problem("Entity set 'AppDbContext.Cart'  is null.");
          }
            _context.Cart.Add(cart);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCart", new { id = cart.Id }, cart);
        }

        // DELETE: api/Carts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            if (_context.Cart == null)
            {
                return NotFound();
            }
            var cart = await _context.Cart.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            _context.Cart.Remove(cart);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CartExists(int id)
        {
            return (_context.Cart?.Any(e => e.Id == id)).GetValueOrDefault();
        }


    }
}
