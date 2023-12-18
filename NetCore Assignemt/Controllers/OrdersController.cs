using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NetCore_Assignemt.Common;
using NetCore_Assignemt.Data;
using NetCore_Assignemt.Models;
using NetCore_Assignemt.Services;
using NetCore_Assignemt.Services.DTO;
using sib_api_v3_sdk.Model;

namespace NetCore_Assignemt.Controllers
{
    [Authorize]
    public class OrdersController : Controller, IOrderServices
    {
        private readonly AppDbContext _context;
        private readonly IVnPayServices _payment;
        private ILogger<OrdersController> _logger;

        public OrdersController(AppDbContext context, IVnPayServices paymentServices, ILogger<OrdersController> logger)
        {
            _context = context;
            _payment = paymentServices;
            _logger = logger;
        }
        private string? getUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        // GET: Orders
        [Authorize(Roles = ("Admin,Mod"))]
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Order.Include(o => o.User);
            return View(await appDbContext.ToListAsync());
        }

        public async Task<IActionResult> MyOrders()
        {
            var appDbContext = _context.Order.Where(c => c.UserId == getUserId());
            return View("Index", await appDbContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        //// GET: Orders/Create
        //public IActionResult Create()
        //{
        //    ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
        //    return View();
        //}

        //// POST: Orders/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,UserId,Total,Status,CreatedDate,PaymentTranId,BankCode,PayStatus")] NetCore_Assignemt.Models.Order order)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(order);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
        //    return View(order);
        //}

        //// GET: Orders/Edit/5
        //public async Task<IActionResult> Edit(long? id)
        //{
        //    if (id == null || _context.Order == null)
        //    {
        //        return NotFound();
        //    }

        //    var order = await _context.Order.FindAsync(id);
        //    if (order == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
        //    return View(order);
        //}

        //// POST: Orders/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(long id, [Bind("Id,UserId,Total,Status,CreatedDate,PaymentTranId,BankCode,PayStatus")] Models.Order order)
        //{
        //    if (id != order.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(order);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!OrderExists(order.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
        //    return View(order);
        //}

        //// GET: Orders/Delete/5
        //public async Task<IActionResult> Delete(long? id)
        //{
        //    if (id == null || _context.Order == null)
        //    {
        //        return NotFound();
        //    }

        //    var order = await _context.Order
        //        .Include(o => o.User)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (order == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(order);
        //}

        //// POST: Orders/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(long id)
        //{
        //    if (_context.Order == null)
        //    {
        //        return Problem("Entity set 'AppDbContext.Order'  is null.");
        //    }
        //    var order = await _context.Order.FindAsync(id);
        //    if (order != null)
        //    {
        //        _context.Order.Remove(order);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool OrderExists(long id)
        {
            return (_context.Order?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> Pay(long id)
        {
            bool flag;
            // Find Order
            var order = await _context.Order.FirstOrDefaultAsync(c => c.Id == id);
            if (order == null) return NotFound();

            // Get Ip address
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();

            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = HttpContext.Request.Headers["X-Forwarded-For"];
            }
            else if (string.IsNullOrEmpty(ipAddress))
            {
                return BadRequest();
            }
            // Generate URL
            string url = _payment.Pay(order, null, ipAddress, out flag);
            // Check  if URL build Success
            if (!flag) return BadRequest();

            return Redirect(url);
        }

        private PaymentResponseDTO UpdateOrderInfo(VnPayCallbackDTO callback)
        {
            string message;
            // Check Responsecode
            VnPayServices.RETURN_RESPONSE_DICTIONARY.TryGetValue(callback.vnp_ResponseCode, out message);
            if (message == null) message = "Unknown Error!";
            try
            {
                // Find Order
                var order = _context.Order.FirstOrDefault(x => x.Id == callback.vnp_TxnRef);

                if (order == null) return new PaymentResponseDTO { RspCode = "-01", Message = "Order Not Found" };

                var paymentId = callback.vnp_TransactionNo;
                // Update to Db
                _context.Transaction.Add(new Transaction
                {
                    Id = paymentId,
                    vnp_TransactionStatus = callback.vnp_TransactionStatus,
                    vnp_BankCode = callback.vnp_BankCode,
                    vnp_Amount = callback.vnp_Amount,
                    vnp_OrderInfo = callback.vnp_OrderInfo,
                    vnp_PayDate = callback.vnp_PayDate,
                    vnp_CardType = callback.vnp_CardType,
                    vnp_BankTranNo = callback.vnp_BankTranNo,
                    vnp_ResponseCode = callback.vnp_ResponseCode,
                    vnp_TransactionNo = callback.vnp_TransactionNo
                });


                // Update Inventory
                if (callback.vnp_ResponseCode == "00" && callback.vnp_TransactionStatus == "00")
                {
                    var orderDetails = _context.OrderDetail
                    .Where(od => od.OrderId == order.Id)
                    .ToList();

                    // Update book quantities based on order details
                    foreach (var orderDetail in orderDetails)
                    {
                        var book = _context.Book.Find(orderDetail.BookId);

                        if (book != null)
                        {
                            // Update book quantity based on order detail quantity
                            book.Quantity -= orderDetail.Quantity;

                        }
                    }
                    order.Status = (int)OrderStatus.Packaging;
                }
                else
                {
                    order.Status = (int)OrderStatus.Canceled;
                }

                order.PaymentTranId = paymentId;
                order.BankCode = callback.vnp_BankCode;
                order.PayStatus = message;


                _context.Order.Update(order);

                _context.SaveChanges();
                return new PaymentResponseDTO { RspCode = callback.vnp_ResponseCode, Message = message };
            }
            catch (Exception e)
            {
                return new PaymentResponseDTO { RspCode = callback.vnp_ResponseCode, Message = message };

            }
        }

        // Client to Server
        [HttpGet]
        public IActionResult Return([FromQuery] VnPayCallbackDTO callback)
        {
            string rawUrl = HttpContext.Request.QueryString + "";

            _logger.LogInformation(rawUrl.ToString());

            if (_payment.CallBackValidate(callback, rawUrl))
            {
                var result = UpdateOrderInfo(callback);
                return View("Return", result);
            }
            // Unvalidate 
            return View("Return", new PaymentResponseDTO { RspCode = callback.vnp_ResponseCode, Message = "Invalid Transaction" });
        }

        // Server to Server
        [HttpGet]
        [Route("/IPN")]
        public async Task<IActionResult> IPN([FromQuery] VnPayCallbackDTO callback)
        {
            // Debug WIP
            _logger.LogInformation("IPN Catched: " + HttpContext.Request.QueryString);

            string rawUrl = HttpContext.Request.QueryString + "";
            // Get order info
            var order = await _context.Order.FirstOrDefaultAsync(c => c.Id == callback.vnp_TxnRef);

            // Encoding Response
            using (var result = new StringContent(_payment.InstantPaymentNotification(callback, rawUrl, order), System.Text.Encoding.UTF8, "application/json"))
            {
                HttpContext.Response.Clear();
                HttpContext.Response.WriteAsJsonAsync(result);

                _logger.LogInformation(result.ToString());
            }

            HttpContext.Response.Body.Close();

            return Ok();
        }

        [HttpPost]
        [Route("api/orders/nextstage/{id}")]
        public async Task<IActionResult> Cancel(long id)
        {
            string userId = getUserId();
            if (userId == null) return Unauthorized();

            var order = await _context.Order.Where(c => c.UserId == userId).FirstOrDefaultAsync(c => c.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            if (order.Status != (int)OrderStatus.Canceled)
            {
                order.Status = (int)OrderStatus.Canceled;
                _context.SaveChanges();
            }
            return Redirect("/orders");
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Mod")]
        [Route("api/orders/nextstage/{id}")]
        public async Task<IActionResult> NextStage(long id)
        {
            string userId = getUserId();
            if (userId == null) return Unauthorized();

            var order = await _context.Order.Where(c => c.UserId == userId).FirstOrDefaultAsync(c => c.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            if (order.Status <= (int)OrderStatus.Canceled)
            {
                return Ok(new { Message = "Order already cancelled." });
            }
            if (order.Status <= (int)OrderStatus.Completed && order.Status >= (int)OrderStatus.Pending)
            {
                order.Status += 1;
                return Ok(new { Message = "Changed." });
            }
            _context.SaveChanges();
            return NotFound();
        }
    }
}
