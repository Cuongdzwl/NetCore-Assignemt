using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCore_Assignemt.Common;
using NetCore_Assignemt.Data;
using NetCore_Assignemt.Models;
using NetCore_Assignemt.Services;
using NetCore_Assignemt.Services.DTO;
using NuGet.Protocol;

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

        private string? getUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        // GET: api/Carts
        [HttpGet]
        public async Task<ActionResult<Cart>> Get()
        {
            // Get the user's unique identifier from the ClaimsPrincipal
            var userId = getUserId();

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


        [HttpPost("AddToCart/{bookId}")]
        public async Task<IActionResult> AddToCartAsync(int bookId)
        {
            return await AddToCartAsyncInternal(bookId, 1);
        }

        [HttpPost("AddtoCart/{bookId}/{quantity}")]
        public async Task<IActionResult> AddToCartAsync(int bookId, int quantity)
        {
            return await AddToCartAsyncInternal(bookId, quantity);
        }

        private async Task<IActionResult> AddToCartAsyncInternal(int bookId, int quantity)
        {
            try
            {
                var userId = getUserId();
                // User not authenticated
                if (userId == null)
                {
                    return Unauthorized(); 
                }
                // Book Not Exist
                if(await _context.Book.Where(c => c.BookId == bookId).FirstOrDefaultAsync() == null)
                {
                    return NotFound(new { Message = "Book Does Not Exist!" });
                }
                // Check Item in cart
                var userCart = await _context.Cart.Where(c => c.UserId == userId).Where(c => c.BookId == bookId).FirstOrDefaultAsync();
                // New Item
                if (userCart == null)
                {
                    userCart = new Cart { UserId = userId , BookId = bookId, Quantity = quantity };
                    _context.Cart.Add(userCart);
                }
                // Modify
                if (userCart != null)
                {
                    userCart.Quantity += quantity;
                }

                await _context.SaveChangesAsync();

                return CreatedAtAction("Book added to cart successfully", new CartDTO {BookId = bookId, Quantity = quantity});
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        private long GenerateOrderId()
        {
            // Implement your logic to generate a unique order ID
            // Replace this with your actual implementation
            return DateTime.Now.Ticks;
        }

        public async Task<IActionResult> CheckOut()
        {
            try
            {
                var userId = getUserId();
                if(userId == null)
                {
                    return Unauthorized();
                }
                // Generate Order id
                var orderId = GenerateOrderId();

                // Get the user's cart
                var userCart = await _context.Cart.Include(c => c.Book).Where(c => c.UserId == userId).ToListAsync();

                if (userCart == null || !userCart.Any())
                {
                    return BadRequest("Cart is empty. Add more items to the cart before checking out.");
                }
                // Calculate the total price
                var total = userCart.Sum(c => c.Quantity * c.Book.Price);

                // Create an order
                var order = new Order
                {
                    Status = (int)OrderStatus.Pending,
                    Id = orderId,
                    UserId = userId,
                    Total = total,
                };

                // Create order details
                foreach (var cartItem in userCart)
                {
                    var orderDetail = new OrderDetail
                    {
                        BookId = cartItem.BookId,
                        OrderId = orderId,
                        Price = cartItem.Book.Price,
                        Quantity = cartItem.Quantity
                    };

                    // Add order detail to the context
                    _context.OrderDetail.Add(orderDetail);

                    // Remove cart item
                    _context.Cart.Remove(cartItem);
                }

                // Add order to the context
                _context.Order.Add(order);

                // Save changes to the database
                await _context.SaveChangesAsync();

                return Ok(new { OrderInfo = order, OrderItems = userCart });
            }catch(Exception e)
            {
                return BadRequest();
            }
            }
        // Patch: api/Carts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("edit/{bookid}/{quantity}")]
        public async Task<IActionResult> Edit(int bookid, int? quantity)
        {
            try
            {

            var userId = getUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var item = await _context.Cart.Where(c => c.BookId == bookid).Where(c => c.UserId == userId).FirstOrDefaultAsync();

            if (item == null)
            {
                return NotFound(new { Message = "Book Not Found!" });
            }
            
            _context.Cart.Update(item);
            return Ok("Cart Updated successfully");
            }catch (Exception ex)
            {
                return BadRequest();
            }
        }
        // 204 : No Content
        // DELETE: api/Carts/5
        [HttpDelete("delete/{bookid}")]
        public async Task<IActionResult> DeleteCart(int bookid)
        {
            var userId = getUserId();

            if(userId == null)
            {
                return Unauthorized();
            }

            var item = await _context.Cart.Where(c => c.BookId == bookid).Where(c => c.UserId == userId).FirstOrDefaultAsync();
            if (item == null)
            {
                return NotFound(new { Message = "Book Not Found!" });
            }

            _context.Cart.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Deleted" });
        }

        // 204 : No Content
        [HttpDelete("delete/all")]
        public async Task<IActionResult> DeleteCart()
        {
            var userId = getUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var item = await _context.Cart.Where(c => c.UserId == userId).FirstOrDefaultAsync();
            if(item == null)
            {
                return NotFound(new { Message = "Empty Cart!" });
            }
            _context.Cart.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Deleted" });
        }

        private bool CartExists(int id)
        {
            return (_context.Cart?.Any(e => e.Id == id)).GetValueOrDefault();
        }


    }
}
