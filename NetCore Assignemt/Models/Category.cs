using NuGet.Packaging.Signing;
using System.ComponentModel.DataAnnotations;

namespace NetCore_Assignemt.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Timestamp]
        public byte[] CreatedDate { get; set; }
    }
}
