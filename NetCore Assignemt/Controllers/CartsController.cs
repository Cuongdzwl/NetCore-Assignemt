using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NetCore_Assignemt.Areas.Identity.Data;
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
        private readonly IEmailSender _sender;

        public CartsController(AppDbContext context, IEmailSender sender)
        {
            _context = context;
            _sender = sender;
        }

        private string? getUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        // GET: api/Carts/?size=5&getall=false
        [HttpGet]
        public async Task<ActionResult<Cart>> Get([FromQuery] int? size)
        {
            // Get the user's unique identifier from the ClaimsPrincipal
            var userId = getUserId();

            // Check if the user has a cart
            var query = _context.Cart
                .Where(c => c.UserId == userId)
                .Include(c => c.Book);

            if (size.HasValue == true || size != null)
            {
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Cart, Book?>)query.Take(size.Value);
            }
            var cartItems = await query.ToListAsync();

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
                if (await _context.Book.Where(c => c.BookId == bookId).FirstOrDefaultAsync() == null)
                {
                    return NotFound(new { Message = "Book Does Not Exist!" });
                }
                // Check Item in cart
                var userCart = await _context.Cart.Where(c => c.UserId == userId).Where(c => c.BookId == bookId).FirstOrDefaultAsync();
                // New Item
                if (userCart == null)
                {
                    userCart = new Cart { UserId = userId, BookId = bookId, Quantity = quantity };
                    _context.Cart.Add(userCart);
                }
                // Modify
                if (userCart != null)
                {
                    userCart.Quantity += quantity;
                }

                await _context.SaveChangesAsync();

                return CreatedAtAction("Book added to cart successfully", new CartDTO { BookId = bookId, Quantity = quantity });
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
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        [HttpPost("checkout")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut()
        {
            try
            {
                var userId = getUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

            var userInfo = _context.Users.Where(c => c.Id == userId).FirstOrDefault();

            if (userInfo == null) return Unauthorized(new { message = "Some thing went wrong!" });
            if (userInfo.Address  == null || userInfo.Address.Length == 0) { return Unauthorized(new { message = "You need to provide your detailed Address before checking out!"}); }
            if (userInfo.City == null || userInfo.City.Length == 0) { return Unauthorized(new { message = "You need to provide your City before checking out!" }); }
            if (userInfo.District == null || userInfo.District.Length == 0) { return Unauthorized(new { message = "You need to provide your District before checking out!" }); }
            if (userInfo.PhoneNumber == null || userInfo.PhoneNumber.Length < 8 || userInfo.PhoneNumber.Length > 12) { return Unauthorized(new { message = "You need to provide your PhoneNumber before checking out!" }); }

            // Generate Order i
                long orderId = GenerateOrderId();

                // Get the user's cart
                var userCart = await _context.Cart.Include(c => c.Book).Where(c => c.Book.BookId == c.BookId).Where(c => c.UserId == userId).ToListAsync();
                if (userCart == null || !userCart.Any())
                {
                    return BadRequest(new { message = "Cart is empty. Add more items to the cart before checking out." });
                }
                // Calculate the total price
                var total = userCart.Sum(c => c.Quantity * c.Book.Price);
                // Create an order
                var order = new Order
                {
                    Id = orderId,
                    UserId = userId,
                    Total = total,
                    Status = (int)OrderStatus.Pending,
                };
                _context.Order.Add(order);

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

                // Save changes to the database
                await _context.SaveChangesAsync();
                string htmlContent = "Your Order ID." + orderId + " has been placed.";

                await _sender.SendEmailAsync(userInfo.Email, "Order Placed on FPT Book",htmlContent);

                return Ok(new {OrderId = order.Id.ToString() ,  message = "Checked Out Successfully!"});
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
        // Patch: api/Carts/edit/
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

                var item = await _context.Cart
                .Where(c => c.BookId == bookid)
                .Where(c => c.UserId == userId)
                .FirstOrDefaultAsync();

                if (item == null)
                {
                    return NotFound(new { Message = "Book Not Found!" });
                }
				item.Quantity = quantity ?? item.Quantity;  

				_context.Cart.Update(item);
				await _context.SaveChangesAsync(); // Save changes to the database

				return Ok("Cart Updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        // 204 : No Content
        // DELETE: api/Carts/delete
        [HttpDelete("delete/{bookid}")]
        public async Task<IActionResult> DeleteCart(int bookid)
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

            _context.Cart.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Deleted" });
        }

        // 204 : No Content
        [HttpDelete("deleteall")]
        public async Task<IActionResult> DeleteCart()
        {
            var userId = getUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var item = await _context.Cart.Where(c => c.UserId == userId).FirstOrDefaultAsync();
            if (item == null)
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
