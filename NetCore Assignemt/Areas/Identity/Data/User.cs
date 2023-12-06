using Microsoft.AspNetCore.Identity;
using NetCore_Assignemt.Models;

namespace NetCore_Assignemt.Areas.Identity.Data
{
    public class User : IdentityUser
    {
        [PersonalData]
        public string? Address { get; set; }
        [PersonalData]
        public DateTime DOB { get; set; }
        [PersonalData]
        public string? City { get; set; }
        [PersonalData]
        public string? District { get; set; }
        [PersonalData]
        public string? Gender { get; set; }

        public virtual ICollection<Order>? Orders { get; set; }
    }
}
