using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace NetCore_Assignemt.Services
{
    public interface IAdminServices
    {
        public IActionResult Orders();
        public IActionResult Users();
        public IActionResult Roles();

        public IActionResult AssignRoles(IdentityUser user, IdentityRole role);
        public IActionResult AssignRoles(string userId, string roleId);
    }
}
