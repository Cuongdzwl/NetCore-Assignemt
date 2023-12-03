using Microsoft.AspNetCore.Identity;
namespace NetCore_Assignemt.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? CustomProperty { get; set; }

        public string? Address { get; set; }
    }
}
