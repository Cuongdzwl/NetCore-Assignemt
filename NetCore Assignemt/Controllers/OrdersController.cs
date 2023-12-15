using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
            return View("Order",await appDbContext.ToListAsync());
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
        [HttpGet]
        public IActionResult Return([FromQuery]VnPayCallbackDTO callback)
        {
            string rawUrl = HttpContext.Request.QueryString + "";

            _logger.LogInformation(rawUrl.ToString());   

            if (_payment.CallBackValidate(callback,rawUrl))
            {
                return View("Return", callback);
            }

            return Redirect("/orders");
        }

        public Task<IActionResult> Cancel(long id)
        {
            throw new NotImplementedException();
        }



        public Task<IActionResult> NextStage(long id)
        {
            throw new NotImplementedException();
        }
    }
}
