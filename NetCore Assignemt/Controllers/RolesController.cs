using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace NetCore_Assignemt.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        // GET: Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Handle errors
                    ModelState.AddModelError("", "Error creating role");
                }
            }

            return View(role);
        }
    }
}
