using Microsoft.AspNetCore.Mvc;
using NetCore_Assignemt.Models;

namespace NetCore_Assignemt.Services
{
    public interface IOrderServices
    {
        public IActionResult ShowAllOrder();
        public IActionResult Get(Order order);
        public Task<IActionResult> PayAsync(Order order);

        public Task<IActionResult> CancelAsync(Order order);
    }
}
