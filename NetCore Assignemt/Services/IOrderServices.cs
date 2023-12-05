using Microsoft.AspNetCore.Mvc;
using NetCore_Assignemt.Models;

namespace NetCore_Assignemt.Services
{
    public interface IOrderServices
    {
        public Task<IActionResult> Get();
        public Task<IActionResult> Get(int orderid);
        public Task<IActionResult> PayAsync(int orderid);

        public Task<IActionResult> CancelAsync(int orderid);
    }
}
