using Microsoft.AspNetCore.Mvc;
using NetCore_Assignemt.Models;

namespace NetCore_Assignemt.Services
{
    public interface IOrderServices
    {
        public Task<IActionResult> Pay(Order order);

    }
}
