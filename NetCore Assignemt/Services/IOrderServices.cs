using Microsoft.AspNetCore.Mvc;
using NetCore_Assignemt.Common;
using NetCore_Assignemt.Models;

namespace NetCore_Assignemt.Services
{
    public interface IOrderServices
    {
        public Task<IActionResult> PayAsync(int orderid);

        public Task<IActionResult> CancelAsync(int orderid);

        // API
        public Task<IActionResult> NextStage(int orderid);
    }
}
