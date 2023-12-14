using System;
using System.Collections.Generic;
using System.Linq;
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

namespace NetCore_Assignemt.Controllers
{
    [Authorize]
    public class OrdersController : Controller, IOrderServices
    {
        private readonly AppDbContext _context;
        private readonly IPaymentServices _payment;
        private ILogger<OrdersController> _logger;

        public OrdersController(AppDbContext context, IPaymentServices paymentServices, ILogger<OrdersController> logger)
        {
            _context = context;
            _payment = paymentServices;
            _logger = logger;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Order.Include(o => o.User);
            return View(await appDbContext.ToListAsync());
        }

        public async Task<IActionResult> MyOrders()
        {

            var appDbContext = _context.Order.Include(o => o.User);
            return View(await appDbContext.ToListAsync());
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

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Total,Status,CreatedDate,PaymentTranId,BankCode,PayStatus")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,UserId,Total,Status,CreatedDate,PaymentTranId,BankCode,PayStatus")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(long? id)
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

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Order == null)
            {
                return Problem("Entity set 'AppDbContext.Order'  is null.");
            }
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                _context.Order.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

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
            var order = _context.Order.FirstOrDefault(x => x.Id == callback.vnp_TxnRef);

            if (order == null) return new PaymentResponseDTO { RspCode = "-01", Message = "Order Not Found" };

            var paymentId = DateTime.Now.Ticks;
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

            // Check Responsecode
            switch (callback.vnp_ResponseCode)
            {
                case "00":
                    message = "Transaction successful";
                    break;
                case "01":
                    message = "Transaction not completed";
                    break;
                case "02":
                    message = "Transaction error";
                    break;
                case "04":
                    message = "Reversed transaction (Customer has been debited at the bank but the transaction has not been successful at VNPAY)";
                    break;
                case "05":
                    message = "VNPAY is processing this transaction (Refund in progress)";
                    break;
                case "06":
                    message = "VNPAY has sent a refund request to the bank (Refund in progress)";
                    break;
                case "07":
                    message = "Transaction suspected of fraud";
                    break;
                case "09":
                    message = "Refund request denied";
                    break;
                default:
                    message = "Unknown response code";
                    break;
            }

            order.PaymentTranId = paymentId;
            order.BankCode = callback.vnp_BankCode;
            order.PayStatus = message;

            _context.Order.Update(order);

            _context.SaveChanges();
            return new PaymentResponseDTO { RspCode = callback.vnp_ResponseCode, Message = message };
        }

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

        [HttpGet]
        [Route("/IPN")]
        public IActionResult IPN([FromQuery] VnPayCallbackDTO callback)
        {
            _logger.LogInformation("IPN Catched: " + HttpContext.Request.QueryString);
            string rawUrl = HttpContext.Request.QueryString + "";

            // Encoding Response
            using (var result = new StringContent(_payment.InstantPaymentNotification(callback, rawUrl), System.Text.Encoding.UTF8, "application/json"))
            {
                HttpContext.Response.Clear();
                HttpContext.Response.WriteAsJsonAsync(result);

                _logger.LogInformation(result.ToString());
            }

            HttpContext.Response.Body.Close();

            return Ok();
        }

        public async Task<IActionResult> Cancel(long id)
        {
            var order = await _context.Order.FirstOrDefaultAsync(c => c.Id == id);

            return Redirect("/myorder");
        }

        public Task<IActionResult> NextStage(long id)
        {
            throw new NotImplementedException();
        }
    }
}
