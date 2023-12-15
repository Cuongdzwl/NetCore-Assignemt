using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetCore_Assignemt.Areas.Identity.Data;
using NetCore_Assignemt.Data;
using NetCore_Assignemt.Models;

namespace NetCore_Assignemt.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Admin/SetRoles
        public async Task<IActionResult> SetRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound(); // Handle the case where the user is not found
            }

            var availableRoles = _roleManager.Roles.ToList();

            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new UserRoles
            {
                UserId = userId,
                AvailableRoles = availableRoles,
                SelectedRoles = userRoles.ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetRoles(UserRoles model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                return NotFound(); // Handle the case where the user is not found
            }

            var existingRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, existingRoles);

            await _userManager.AddToRolesAsync(user, model.SelectedRoles);

            return RedirectToAction("Index", "Home"); // Redirect to the home page or another appropriate page
        }



    }
}
