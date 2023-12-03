using Microsoft.AspNetCore.Identity;

namespace NetCore_Assignemt.Areas.Identity.Data
{
    public class NetCore_AssignemtUser : IdentityUser
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

    }
}
