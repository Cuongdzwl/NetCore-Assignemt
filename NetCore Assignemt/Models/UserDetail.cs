using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace NetCore_Assignemt.Models
{
    public class UserDetail
    {
        [Key]
        public string UserId { get; set; }

        [Required]
        [MaxLength(12)]
        [RegularExpression("^\\d+$\r\n")]
        public string PhoneNum { get; set; }

        [Required]
        public byte Gender { get; set; }

        public string? City { get; set; }

        public string? District { get; set; }

        public string? Detail { get; set; }

        public string GetGenderToString()
        {
            switch (Gender)
            {
                case 0:
                    return "Male";
                case 1:
                    return "Female";
                default:
                    return "Unspecified";
            }
        }
    }
}
