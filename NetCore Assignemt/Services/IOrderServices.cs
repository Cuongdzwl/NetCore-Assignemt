using Microsoft.AspNetCore.Mvc;
using NetCore_Assignemt.Common;
using NetCore_Assignemt.Models;

namespace NetCore_Assignemt.Services
{
    public interface IOrderServices
    {
        public Task<IActionResult> Pay(long id);

        public Task<IActionResult> Cancel(long id);

        public IActionResult Return();
        // API
        public Task<IActionResult> NextStage(long id);
    }
}
